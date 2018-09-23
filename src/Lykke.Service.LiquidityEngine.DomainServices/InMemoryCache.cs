using System;
using System.Collections.Generic;

namespace Lykke.Service.LiquidityEngine.DomainServices
{
    public class InMemoryCache<T> where T : class
    {
        private readonly Func<T, string> _getKeyFunc;
        private readonly object _sync = new object();

        private readonly Dictionary<string, T> _cache = new Dictionary<string, T>();

        private bool _initialized;

        public InMemoryCache(Func<T, string> getKeyFunc)
        {
            _getKeyFunc = getKeyFunc;
        }
        
        public IReadOnlyCollection<T> GetAll()
        {
            lock (_sync)
            {
                return _initialized ? _cache.Values : null;
            }
        }

        public T Get(string key)
        {
            lock (_sync)
            {
                if (_cache.ContainsKey(key))
                    return _cache[key];
            }

            return null;
        }

        public void Set(T item)
        {
            if(!_initialized)
                return;
            
            lock (_sync)
            {
                _cache[_getKeyFunc(item)] = item;
            }
        }

        public void Initialize(IReadOnlyCollection<T> items)
        {
            if (_initialized)
                return;

            lock (_sync)
            {
                if (_initialized)
                    return;

                foreach (T item in items)
                    _cache[_getKeyFunc(item)] = item;

                _initialized = true;
            }
        }

        public void Remove(string key)
        {
            lock (_sync)
            {
                if (_cache.ContainsKey(key))
                    _cache.Remove(key);
            }
        }
    }
}
