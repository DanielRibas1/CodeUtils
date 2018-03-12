using DataCollections.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollections
{
    public class TreeInMemoryCollection<TKey, TInnerKey, TValue> : InMemoryCollection<TKey, IInMemoryCollection<TInnerKey, TValue>>, ITreeInMemoryCollection<TKey, TInnerKey, TValue>
    {
        private readonly object _refreshLock = new object();
        private bool _refreshing = false;

        public TreeInMemoryCollection()
        {
        }

        public TreeInMemoryCollection(IEnumerable<KeyValuePair<TKey, IInMemoryCollection<TInnerKey, TValue>>> innerCollections)
        {
            foreach (var item in innerCollections)
                this.InnerValues.TryAdd(item.Key, item.Value);
        }

        public override bool Loaded { get; protected set; }

        public IEnumerable<TValue> GetAllFromNode(TKey firstLevelKey)
        {
            IInMemoryCollection<TInnerKey, TValue> node;
            if (this.InnerValues.TryGetValue(firstLevelKey, out node))
                return node.GetAll();
            return null;
        }

        public bool TryGetNodeValue(TKey firstLevelKey, TInnerKey secondLevelKey, out TValue value)
        {            
            IInMemoryCollection<TInnerKey, TValue> node;
            if (this.InnerValues.TryGetValue(firstLevelKey, out node))
                return node.TryGet(secondLevelKey, out value);
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public override void Refresh()
        {
            if (!_refreshing)
            {                                   
                lock (_refreshLock)
                {
                    try
                    {
                        _refreshing = true;
                        InnerRefresh();
                        Loaded = true;
                    }
                    finally
                    {
                        _refreshing = false;
                    }                       
                }                
            }
        }

        protected virtual void InnerRefresh()
        {
            foreach (var item in this.InnerValues)
                item.Value.Refresh();
        }

        public IDictionary<TKey, IEnumerable<TInnerKey>> GetAllKeysByNode()
        {
            var dic = new Dictionary<TKey, IEnumerable<TInnerKey>>();
            foreach (var item in this.InnerValues)            
                dic.Add(item.Key, item.Value.GetAllKeys());
            return dic;
        }
    }
}
