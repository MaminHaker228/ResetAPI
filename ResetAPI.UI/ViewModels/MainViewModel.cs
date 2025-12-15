using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Series;
using ResetAPI.Domain.DTO;
using ResetAPI.Domain.Enums;
using ResetAPI.Services.Cache;
using ResetAPI.Services.Market;
using ResetAPI.Services.Steam;
using Serilog;

namespace ResetAPI.UI.ViewModels
{
    /// <summary>
    /// Main application ViewModel
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly ISteamMarketService _marketService;
        private readonly IMarketDataService _dataService;
        private readonly CacheService _cacheService;
        private static readonly ILogger Log = Serilog.Log.ForContext<MainViewModel>();

        private ObservableCollection<SkinDTO> _skins;
        private ObservableCollection<SkinDTO> _filteredSkins;
        private SkinDTO _selectedSkin;
        private string _searchQuery;
        private int _selectedGameId;
        private decimal _minPrice;
        private decimal _maxPrice;
        private bool _isLoading;
        private string _statusMessage;
        private PlotModel _priceChart;
        private GameType[] _supportedGames;

        public MainViewModel(ISteamMarketService marketService, IMarketDataService dataService, CacheService cacheService)
        {
            _marketService = marketService ?? throw new ArgumentNullException(nameof(marketService));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));

            _skins = new ObservableCollection<SkinDTO>();
            _filteredSkins = new ObservableCollection<SkinDTO>();
            _supportedGames = new[] { GameType.CS2, GameType.Dota2 };
            _selectedGameId = 730; // CS2 by default
            _minPrice = 0m;
            _maxPrice = 10000m;

            LoadSkinsCommand = new RelayCommand(async () => await LoadSkinsAsync());
            SearchCommand = new RelayCommand(PerformSearch);
            SelectSkinCommand = new RelayCommand<SkinDTO>(async skin => await LoadSkinDetails(skin));
            FilterCommand = new RelayCommand(async () => await ApplyFiltersAsync());
            ChangeModeCommand = new RelayCommand<int>(async gameId => await ChangeGameAsync(gameId));
        }

        public ICommand LoadSkinsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SelectSkinCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand ChangeModeCommand { get; }

        public ObservableCollection<SkinDTO> FilteredSkins
        {
            get => _filteredSkins;
            set => SetField(ref _filteredSkins, value);
        }

        public SkinDTO SelectedSkin
        {
            get => _selectedSkin;
            set => SetField(ref _selectedSkin, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetField(ref _searchQuery, value))
                {
                    PerformSearch();
                }
            }
        }

        public int SelectedGameId
        {
            get => _selectedGameId;
            set => SetField(ref _selectedGameId, value);
        }

        public decimal MinPrice
        {
            get => _minPrice;
            set => SetField(ref _minPrice, value);
        }

        public decimal MaxPrice
        {
            get => _maxPrice;
            set => SetField(ref _maxPrice, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public PlotModel PriceChart
        {
            get => _priceChart;
            set => SetField(ref _priceChart, value);
        }

        public GameType[] SupportedGames => _supportedGames;

        private async Task LoadSkinsAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading skins...";

                var skins = await _dataService.GetPopularSkinsAsync(_selectedGameId);
                _skins.Clear();

                foreach (var skin in skins)
                {
                    _skins.Add(skin);
                }

                PerformSearch();
                StatusMessage = $"Loaded {skins.Count} skins";
                Log.Information("Loaded {Count} skins for game {GameId}", skins.Count, _selectedGameId);
            }
            catch (Exception ex)
            {
                StatusMessage = "Error loading skins";
                Log.Error(ex, "Error loading skins");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void PerformSearch()
        {
            var query = SearchQuery?.ToLowerInvariant() ?? "";
            var filtered = _skins
                .Where(s => string.IsNullOrEmpty(query) || s.MarketHashName.ToLowerInvariant().Contains(query))
                .OrderByDescending(s => s.Volume)
                .ToList();

            FilteredSkins.Clear();
            foreach (var skin in filtered)
            {
                FilteredSkins.Add(skin);
            }
        }

        private async Task LoadSkinDetails(SkinDTO skin)
        {
            if (skin == null) return;

            try
            {
                IsLoading = true;
                SelectedSkin = skin;
                StatusMessage = $"Loading details for {skin.MarketHashName}...";

                var history = await _marketService.GetPriceHistoryAsync(_selectedGameId, skin.MarketHashName);
                BuildPriceChart(history, skin.MarketHashName);

                StatusMessage = "Ready";
            }
            catch (Exception ex)
            {
                StatusMessage = "Error loading details";
                Log.Error(ex, "Error loading skin details");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void BuildPriceChart(System.Collections.Generic.List<PriceHistoryDTO> history, string title)
        {
            var plotModel = new PlotModel { Title = title };
            plotModel.Axes.Add(new OxyPlot.Axes.DateTimeAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                StringFormat = "yyyy-MM-dd"
            });
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Price (USD)"
            });

            var series = new LineSeries { Title = "Price" };
            foreach (var point in history)
            {
                series.Points.Add(new DataPoint(
                    OxyPlot.Axes.DateTimeAxis.ToDouble(point.Date),
                    (double)point.Price
                ));
            }

            plotModel.Series.Add(series);
            PriceChart = plotModel;
        }

        private async Task ApplyFiltersAsync()
        {
            try
            {
                IsLoading = true;
                var filtered = await _dataService.FilterSkinsAsync(_skins.ToList(), _minPrice, _maxPrice);

                FilteredSkins.Clear();
                foreach (var skin in filtered)
                {
                    FilteredSkins.Add(skin);
                }

                StatusMessage = $"Showing {filtered.Count} skins";
            }
            catch (Exception ex)
            {
                StatusMessage = "Error applying filters";
                Log.Error(ex, "Error applying filters");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ChangeGameAsync(int gameId)
        {
            _selectedGameId = gameId;
            await LoadSkinsAsync();
        }
    }
}
