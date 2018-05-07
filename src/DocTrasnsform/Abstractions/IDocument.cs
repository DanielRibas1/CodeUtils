using System;
using System.Collections.Generic;
using System.Text;

namespace DocTrasnsform.Abstractions
{
    public interface IDocument<T>
    {
        string Name { get; set; }
        string Extension { get; }
        T Document { get; set; }
    }
}
