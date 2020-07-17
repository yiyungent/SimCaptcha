using Microsoft.Extensions.Caching.Memory;
using SimCaptcha.Interface;
using System;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.AspNetCore
{
    /// <summary>
    /// 实现缓存
    /// <para>注意: 均为绝对过期，非滑动过期</para>
    /// </summary>
    public class LocalCache : ICache
    {
        private IMemoryCache _cache;

        public LocalCache(IMemoryCache memoryCache)
        {
            this._cache = memoryCache;
        }

        /// <summary>
        /// 默认超时/过期时间(秒)
        /// </summary>
        public int TimeOut { get; set; } = 60;

        public bool Exists(string key)
        {
            object result = null;
            try
            {
                result = _cache.Get(key);
            }
            catch (Exception ex)
            {
            }
            return result != null;
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void Insert(string key, object data)
        {
            _cache.Set(key, data, TimeSpan.FromSeconds(TimeOut));
        }

        public void Insert<T>(string key, T data)
        {
            _cache.Set<T>(key, data, TimeSpan.FromSeconds(TimeOut));
        }

        public void Insert(string key, object data, int cacheTime)
        {
            _cache.Set(key, data, TimeSpan.FromSeconds(cacheTime));
        }

        public void Insert<T>(string key, T data, int cacheTime)
        {
            _cache.Set<T>(key, data, TimeSpan.FromSeconds(cacheTime));
        }

        public void Insert(string key, object data, DateTime cacheTime)
        {
            _cache.Set(key, data, cacheTime);
        }

        public void Insert<T>(string key, T data, DateTime cacheTime)
        {
            _cache.Set<T>(key, data, cacheTime);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
