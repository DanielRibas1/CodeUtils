using System;
using System.Collections.Generic;
using System.Text;

namespace DocTrasnsform.Abstractions
{
    public interface IDocumentContainer<TRequest, TKey, TValue> : IDictionary<TKey, TValue>
    {
        bool Loaded { get; }
        void Load(params TRequest[] args);
    }
}
