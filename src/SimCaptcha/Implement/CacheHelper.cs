using SimCaptcha.Interface;
using System;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Implement
{
    public class CacheHelper : ICacheHelper
    {
        #region Fields
        private static object cacheLocker = new object();// 缓存锁对象
        private ICache _cache = null;// 缓存接口 
        #endregion

        #region Ctor
        public CacheHelper(ICache cache)
        {
            this._cache = cache;
        }
        #endregion

        #region Methods
        public ICache GetCache()
        {
            return _cache;
        }

        /// <summary>
        /// 缓存过期时间
        /// </summary>
        public int TimeOut
        {
            get
            {
                return _cache.TimeOut;
            }
            set
            {
                lock (cacheLocker)
                {
                    _cache.TimeOut = value;
                }
            }
        }

        /// <summary>
        /// 获得指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        public object Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            return _cache.Get(key);
        }

        /// <summary>
        /// 获得指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        /// <summary>
        /// 将指定键的对象添加到缓存中
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        public void Insert(string key, object data)
        {
            if (string.IsNullOrWhiteSpace(key) || data == null)
                return;
            //lock (cacheLocker)
            {
                _cache.Insert(key, data);
            }
        }
        /// <summary>
        /// 将指定键的对象添加到缓存中
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        public void Insert<T>(string key, T data)
        {
            if (string.IsNullOrWhiteSpace(key) || data == null)
                return;
            //lock (cacheLocker)
            {
                _cache.Insert<T>(key, data);
            }
        }
        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间(分钟)</param>
        public void Insert(string key, object data, int cacheTime)
        {
            if (!string.IsNullOrWhiteSpace(key) && data != null)
            {
                //lock (cacheLocker)
                {
                    _cache.Insert(key, data, cacheTime);
                }
            }
        }

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间(分钟)</param>
        public void Insert<T>(string key, T data, int cacheTime)
        {
            if (!string.IsNullOrWhiteSpace(key) && data != null)
            {
                //lock (cacheLocker)
                {
                    _cache.Insert<T>(key, data, cacheTime);
                }
            }
        }

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间</param>
        public void Insert(string key, object data, DateTime cacheTime)
        {
            if (!string.IsNullOrWhiteSpace(key) && data != null)
            {
                //lock (cacheLocker)
                {
                    _cache.Insert(key, data, cacheTime);
                }
            }
        }

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间</param>
        public void Insert<T>(string key, T data, DateTime cacheTime)
        {
            if (!string.IsNullOrWhiteSpace(key) && data != null)
            {
                //lock (cacheLocker)
                {
                    _cache.Insert<T>(key, data, cacheTime);
                }
            }
        }

        /// <summary>
        /// 从缓存中移除指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            lock (cacheLocker)
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        public bool Exists(string key)
        {
            return _cache.Exists(key);
        } 
        #endregion

    }
}
