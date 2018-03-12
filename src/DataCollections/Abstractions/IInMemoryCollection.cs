using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollections.Abstractions
{
    public interface IInMemoryCollection<TKey, TValue>
    {        
        bool Loaded { get; }
        bool TryGet(TKey key, out TValue value);
        void Refresh();
        IEnumerable<TValue> GetAll();
        IEnumerable<TKey> GetAllKeys();
    }
}
