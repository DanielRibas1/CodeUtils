using System;
using System.Collections.Generic;
using System.Text;

namespace DocTrasnsform.Models
{
    [Serializable]
    public abstract class XslTemplateWithLabels
    {
        public List<Label> Labels { get; set; }
    }
}
