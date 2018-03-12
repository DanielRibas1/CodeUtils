using System;
using System.Collections.Generic;

namespace XslTransform.Abstractions
{
    public interface ITemplatesContainer<TRequest, TKey, TValue> : IDictionary<TKey, TValue>
    {
        bool Loaded { get; }
        void Load(params TRequest[] args);
    }
}
