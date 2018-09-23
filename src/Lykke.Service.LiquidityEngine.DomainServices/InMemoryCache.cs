using System;
using System.Collections.Generic;

namespace Lykke.Service.LiquidityEngine.DomainServices
{
    public class InMemoryCache<T> where T : class
    {
        private readonly Func<T, string> _getKeyFunc;
        private readonly object _sync = new object();

        private readonly Dictionary<string, T> _cache = new Dictionary<string, T>();

        public bool Initialized { get; private set; }

        public InMemoryCache(Func<T, string> getKeyFunc, bool initialized)
        {
            _getKeyFunc = getKeyFunc;
            Initialized = initialized;
        }
        
        public IReadOnlyCollection<T> GetAll()
        {
            lock (_sync)
            {
                return Initialized ? _cache.Values : null;
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
            if(!Initialized)
                return;
            
            lock (_sync)
            {
                _cache[_getKeyFunc(item)] = item;
            }
        }
        
        public void Set(IReadOnlyCollection<T> items)
        {
            if(!Initialized)
                return;
            
            lock (_sync)
            {
                foreach (T item in items)
                    _cache[_getKeyFunc(item)] = item;
            }
        }

        public void Initialize(IReadOnlyCollection<T> items)
        {
            if (Initialized)
                return;

            lock (_sync)
            {
                if (Initialized)
                    return;

                foreach (T item in items)
                    _cache[_getKeyFunc(item)] = item;

                Initialized = true;
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
