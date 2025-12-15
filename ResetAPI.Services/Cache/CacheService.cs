using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ResetAPI.Services.Cache
{
    /// <summary>
    /// In-memory cache service with TTL support
    /// </summary>
    public class CacheService
    {
        private readonly ConcurrentDictionary<string, CacheEntry> _cache;
        private readonly int _maxEntries;
        private readonly int _durationMinutes;
        private static readonly ILogger Log = Serilog.Log.ForContext<CacheService>();

        private class CacheEntry
        {
            public object Value { get; set; }
            public DateTime ExpiresAt { get; set; }
        }

        public CacheService(int maxEntries = 1000, int durationMinutes = 5)
        {
            _cache = new ConcurrentDictionary<string, CacheEntry>();
            _maxEntries = maxEntries;
            _durationMinutes = durationMinutes;
        }

        public void Set<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

            // Evict oldest entries if cache is full
            if (_cache.Count >= _maxEntries)
            {
                var oldestKey = _cache.OrderBy(x => x.Value.ExpiresAt).FirstOrDefault().Key;
                if (oldestKey != null)
                {
                    _cache.TryRemove(oldestKey, out _);
                    Log.Debug("Cache entry evicted: {Key}", oldestKey);
                }
            }

            var entry = new CacheEntry
            {
                Value = value,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_durationMinutes)
            };

            _cache.AddOrUpdate(key, entry, (k, v) => entry);
            Log.Debug("Cache entry set: {Key}, expires in {Minutes} minutes", key, _durationMinutes);
        }

        public bool TryGet<T>(string key, out T value)
        {
            value = default;

            if (string.IsNullOrWhiteSpace(key))
                return false;

            if (!_cache.TryGetValue(key, out var entry))
            {
                Log.Debug("Cache miss: {Key}", key);
                return false;
            }

            // Check if expired
            if (DateTime.UtcNow > entry.ExpiresAt)
            {
                _cache.TryRemove(key, out _);
                Log.Debug("Cache entry expired: {Key}", key);
                return false;
            }

            value = (T)entry.Value;
            Log.Debug("Cache hit: {Key}", key);
            return true;
        }

        public void Remove(string key)
        {
            _cache.TryRemove(key, out _);
            Log.Debug("Cache entry removed: {Key}", key);
        }

        public void Clear()
        {
            _cache.Clear();
            Log.Information("Cache cleared");
        }

        public int Count => _cache.Count;
    }
}
