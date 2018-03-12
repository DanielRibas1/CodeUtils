using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollections
{
    public delegate TContext ContextStarter<TContext>();
    public class SqlInMemoryCollection<TKey, TValue, TContext> : InMemoryCollection<TKey, TValue> where TContext : class
    {
        private readonly object _refreshLock = new object();
        private readonly ContextStarter<TContext> _contextStarter;
        private readonly Func<ContextStarter<TContext>, IEnumerable<KeyValuePair<TKey, TValue>>> _loadValues;
        private bool _refreshing = false;

        public SqlInMemoryCollection(ContextStarter<TContext> contextStarter, 
            Func<ContextStarter<TContext>, IEnumerable<KeyValuePair<TKey, TValue>>> loadValues) : base()
        {
            _contextStarter = contextStarter;
            _loadValues = loadValues;            
        }

        public override bool Loaded { get; protected set; }

        public override void Refresh()
        {
            if (!_refreshing)
            {
                lock (_refreshLock)
                {
                    _refreshing = true;
                    try
                    {
                        var values = _loadValues(_contextStarter).ToList();
                        InnerValues.Clear();
                        foreach (var value in values)
                            InnerValues.TryAdd(value.Key, value.Value);
                        Loaded = true;
                    }
                    finally
                    {
                        _refreshing = false;
                    }
                }
            }
        }
    }
}
