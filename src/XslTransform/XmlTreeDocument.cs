using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using XslTransform.Models;

namespace XslTransform
{
    public class XmlTreeDocument
    {
        private const string KeyAttributeName = "key";
        private const string IdentCharacters = "\t";
        private const string LabelNodesXPath = "language/label";
        private StringBuilder _stringBuilder = new StringBuilder();
        public XmlTreeDocument(string name, XmlDocument template)
        {
            this.Name = name;
            SetTemplate(template);
            this.Localization = new Dictionary<string, List<KeyValuePair<string, string>>>();
            this.XsltSettings = new XsltSettings(false, true);
            SetWriter();
        }
        public XmlTreeDocument(string name, string cultureCode, XmlDocument localization)
        {
            this.Name = name;
            this.Localization = new Dictionary<string, List<KeyValuePair<string, string>>>();
            this.AddOrUpdateLocalization(cultureCode, localization);            
            this.XsltSettings = new XsltSettings(false, true);
            SetWriter();
        }

        public string Name { get; set; }
        public XsltSettings XsltSettings { get; set; }
        public XmlWriter XmlWriter { get; private set; }
        public XslCompiledTransform Template { get; private set; }
        public Dictionary<string, List<KeyValuePair<string, string>>> Localization { get; set; }
        public void SetTemplate(XmlDocument document)
        {
#if DEBUG
            var xsltTransformation = new XslCompiledTransform(true);
#else
            var xsltTransformation = new XslCompiledTransform();
#endif

            xsltTransformation.Load(document, XsltSettings, new XmlUrlResolver());
            this.Template = xsltTransformation;
        }

        public string Transform<TData>(TData data, string cultureCode) where TData : TemplateHolder
        {
            if (!string.IsNullOrEmpty(cultureCode))
            {
                List<KeyValuePair<string, string>> lang;
                if (!this.Localization.TryGetValue(cultureCode, out lang))
                    throw new Exception($"Language {cultureCode} not found for template {Name}");
                data.Labels = lang;
            }
            var xmlData = SerializeData(data);
            _stringBuilder.Clear();
            SetWriter();
            Template.Transform(xmlData, XmlWriter);
            var result = _stringBuilder.ToString();
            XmlWriter.Flush();
            _stringBuilder.Clear();
            return result;
        }

        public void AddOrUpdateLocalization(string cultureCode, XmlDocument doc)
        {
            if (!this.Localization.ContainsKey(cultureCode))
                this.Localization.Add(cultureCode, DeserializeLabels(doc));
            else
                this.Localization[cultureCode] = DeserializeLabels(doc);
        }

        private XmlDocument SerializeData<TData>(TData data) where TData : TemplateHolder
        {
            var serializer = new XmlSerializer(typeof(TData));
            var doc = new XmlDocument();
            this.XmlWriter.Flush();
            var stream = new MemoryStream();
            serializer.Serialize(stream, data);
            stream.Position = 0;
            doc.Load(stream);
            return doc;
        }

        private List<KeyValuePair<string, string>> DeserializeLabels(XmlDocument localization)
        {
            var labelNodes = localization.SelectNodes(LabelNodesXPath).OfType<XmlNode>();
            return new List<KeyValuePair<string, string>>(
                labelNodes.Select(x => new KeyValuePair<string, string>(x.Attributes[KeyAttributeName].Value, x.InnerText)));
        }

        private void SetWriter(XmlWriterSettings settings = null)
        {
            if (settings == null)
            {
                settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = IdentCharacters
                };
            }      
            if (this.XmlWriter != null)            
                this.XmlWriter.Dispose();            
            this.XmlWriter = XmlWriter.Create(_stringBuilder, settings);            
        }
    }
}
