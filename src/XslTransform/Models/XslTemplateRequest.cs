using System;
using System.Collections.Generic;
using System.Text;

namespace XslTransform.Models
{
    public class XslTemplateRequest
    {
        public string FileFormat { get; set; }
        public string CultureCode { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
    }
}
