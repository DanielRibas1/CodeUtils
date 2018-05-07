using DocTrasnsform.Abstractions;
using DocTrasnsform.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace DocTrasnsform
{
    public class XslDocumentContainer : ConcurrentDictionary<string, XslDocument>, IDocumentContainer<XslTemplateRequest, string, XslDocument>
    {
        private const string XmlFormat = "xml";
        private const string XsltFormat = "xslt";
        private const string LocalizationFileNameFormat = "{0}_{1}.{2}";
        private const string TemplateFileNameFormat = "{0}.{1}";

        internal string RelativePath = AppDomain.CurrentDomain.RelativeSearchPath;

        public bool Loaded { get; private set; }

        public void Load(params XslTemplateRequest[] args)
        {
            Loaded = false;
            foreach (var request in args)
            {
                if (request.FileFormat.Equals(XmlFormat))
                {
                    try
                    {
                        CultureInfo.GetCultureInfo(request.CultureCode);
                    }
                    catch (CultureNotFoundException)
                    {
                        Trace.WriteLine($"Culture code {request.CultureCode} not valid");
                        continue;
                    }
                    var filePath = Path.Combine(RelativePath, request.Path, string.Format(LocalizationFileNameFormat, request.Name, request.CultureCode, request.FileFormat));
                    if (File.Exists(filePath))
                    {
                        this.AddOrUpdate(request.Name,
                            new XslDocument(request.Name, request.CultureCode, LoadDoc(filePath)),
                            (k, o) =>
                            {
                                o.AddOrUpdateLocalization(request.CultureCode, LoadDoc(filePath));
                                return o;
                            });
                    }
                    else
                        Trace.WriteLine($"File {filePath} don't exists");
                }
                else if (request.FileFormat.Equals(XsltFormat))
                {
                    var filePath = Path.Combine(RelativePath, request.Path, string.Format(TemplateFileNameFormat, request.Name, request.FileFormat));
                    if (File.Exists(filePath))
                    {
                        this.AddOrUpdate(request.Name,
                            new XslDocument(request.Name, LoadDoc(filePath)),
                            (k, o) =>
                            {
                                o.SetTemplate(LoadDoc(filePath));
                                return o;
                            });
                    }
                    else
                        Trace.WriteLine($"File {filePath} don't exists");
                }
                else
                    continue;
            }
            Loaded = true;
        }

        private XmlDocument LoadDoc(string path)
        {
            var doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(path));
            return doc;
        }
    }
}
