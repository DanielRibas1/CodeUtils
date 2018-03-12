using DataCollections.Abstractions;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DataCollections
{
    public abstract class InMemoryCollection<TKey, TValue> : IInMemoryCollection<TKey, TValue>
    {        
        private readonly ConcurrentDictionary<TKey, TValue> _innerValues;        
        public InMemoryCollection()
        {            
            _innerValues = new ConcurrentDictionary<TKey, TValue>();            
        }

        protected ConcurrentDictionary<TKey, TValue> InnerValues
        {
            get
            {
                return _innerValues;
            }
        }

        public abstract bool Loaded { get; protected set; }

        public abstract void Refresh();

        public bool TryGet(TKey key, out TValue value)
        {
            return _innerValues.TryGetValue(key, out value);
        }        

        public IEnumerable<TValue> GetAll()
        {
            return new List<TValue>(_innerValues.Values);
        }

        public IEnumerable<TKey> GetAllKeys()
        {
            return new HashSet<TKey>(_innerValues.Keys);
        }
    }
}
