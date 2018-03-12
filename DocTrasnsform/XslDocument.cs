using DocTrasnsform.Abstractions;
using DocTrasnsform.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace DocTrasnsform
{
    public class XslDocument : IDocument<XmlDocument>
    {
        private const string KeyAttributeName = "id";
        private const string IdentCharacters = "\t";
        private const string LabelNodesXPath = "localization/label";
        private const string Xslt = "xslt";
        private StringBuilder _stringBuilder = new StringBuilder();

        public XslDocument(string name, XmlDocument template)
        {
            this.Name = name;
            SetTemplate(template);
            this.Localization = new Dictionary<string, List<KeyValuePair<string, string>>>();
            this.XsltSettings = new XsltSettings(false, true);
            this.Document = template;
            SetWriter();
        }

        public XslDocument(string name, string cultureCode, XmlDocument localization)
        {
            this.Name = name;
            this.Localization = new Dictionary<string, List<KeyValuePair<string, string>>>();
            this.AddOrUpdateLocalization(cultureCode, localization);
            this.XsltSettings = new XsltSettings(false, true);
            this.Document = localization;
            SetWriter();
        }

        public string Name { get; set; }
        public string Extension => Xslt;
        public XmlDocument Document { get; set; }        
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

        public string Transform<TData>(TData data, string cultureCode) where TData : XslTemplateWithLabels
        {
            if (!string.IsNullOrEmpty(cultureCode))
            {
                List<KeyValuePair<string, string>> lang;
                if (!this.Localization.TryGetValue(cultureCode, out lang))
                    throw new Exception($"Language {cultureCode} not found for template {Name}");
                data.Labels = lang.Select(x => new Label { Key = x.Key, Value = x.Value }).ToList();
            }
            var xmlData = SerializeData(data);
            _stringBuilder.Clear();
            SetWriter();
            Template.Transform(input: xmlData.CreateNavigator(), results: XmlWriter);
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

        private XmlDocument SerializeData<TData>(TData data) where TData : XslTemplateWithLabels
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
                    IndentChars = IdentCharacters,
                    ConformanceLevel = ConformanceLevel.Auto,
                    CloseOutput = true,
                    WriteEndDocumentOnClose = true
                };
            }
            if (this.XmlWriter != null)
                this.XmlWriter.Dispose();
            this.XmlWriter = XmlWriter.Create(_stringBuilder, settings);
        }
    }
}
