using System;
using System.Collections.Generic;
using System.Text;

namespace XslTransform.Models
{
    public abstract class TemplateHolder
    {
        public ICollection<KeyValuePair<string, string>> Labels { get; set; }
    }
}
