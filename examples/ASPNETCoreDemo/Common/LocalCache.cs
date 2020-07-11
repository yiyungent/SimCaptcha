using SimCaptcha.Common.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCoreDemo.Common
{
    public class LocalCache : ICache
    {
        public int TimeOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool Exists(string key)
        {
            throw new NotImplementedException();
        }

        public object Get(string key)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void Insert(string key, object data)
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(string key, T data)
        {
            throw new NotImplementedException();
        }

        public void Insert(string key, object data, int cacheTime)
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(string key, T data, int cacheTime)
        {
            throw new NotImplementedException();
        }

        public void Insert(string key, object data, DateTime cacheTime)
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(string key, T data, DateTime cacheTime)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}
