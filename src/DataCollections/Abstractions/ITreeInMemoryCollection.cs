using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollections.Abstractions
{
    public interface ITreeInMemoryCollection<TKey, TInnerKey, TValue> : IInMemoryCollection<TKey, IInMemoryCollection<TInnerKey, TValue>>
    {
        bool TryGetNodeValue(TKey firstLevelKey, TInnerKey secondLevelKey, out TValue value);
        IEnumerable<TValue> GetAllFromNode(TKey firstLevelKey);
        IDictionary<TKey, IEnumerable<TInnerKey>> GetAllKeysByNode();
    }
}
