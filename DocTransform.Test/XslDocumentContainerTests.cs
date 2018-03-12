using DocTrasnsform;
using DocTrasnsform.Abstractions;
using DocTrasnsform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DocTransform.Test
{
    public class XslDocumentContainerTests
    {
        private IDocumentContainer<XslTemplateRequest, string, XslDocument> _sut;

        public XslDocumentContainerTests()
        {
            _sut = new XslDocumentContainer();
            ((XslDocumentContainer)_sut).RelativePath = System.IO.Directory.GetCurrentDirectory();
        }

        [Theory]
        [InlineData("TestXslt", "en-GB", "es-ES", "fr-FR")]        
        public void Load_Success(params string[] args)
        {
            Load(new Tuple<string, IEnumerable<string>>(args.First(), args.Skip(1)));
            Assert.True(_sut.Loaded);
            Assert.Equal(1, _sut.Count);
            XslDocument value;
            var result = _sut.TryGetValue(_sut.Keys.First(), out value);
            Assert.True(result);
            Assert.NotNull(value);
            Assert.Equal(args.First(), value.Name);
            Assert.NotNull(value.Template);
            Assert.Equal(3, value.Localization.Count);
        }

        [Theory]
        [InlineData("Inexistent", "de-DE", "en-GB", "es-ES", "fr-FR", "it-IT")]
        [InlineData("BadRoute", "zh-CH", "ko-KR")]
        public void Load_Fail(params string[] args)
        {
            Load(new Tuple<string, IEnumerable<string>>(args.First(), args.Skip(1)));
            Assert.True(_sut.Loaded);
            Assert.Equal(0, _sut.Count);
        }

        [Theory]
        [InlineData("TestXslt", "en-GB", "es-ES", "fr-FR")]
        public void Load_Twice(params string[] args)
        {
            Load(new Tuple<string, IEnumerable<string>>(args.First(), args.Skip(1)));
            Load(new Tuple<string, IEnumerable<string>>(args.First(), args.Skip(1)));
            Assert.True(_sut.Loaded);
        }

        [Theory]
        [InlineData("chm")]
        [InlineData("txt")]
        public void Load_UnknownFormat(string format)
        {
            _sut.Load(new XslTemplateRequest { FileFormat = format, Name = "Unknown", Path = "." });
            Assert.True(_sut.Loaded);
        }

        [Theory]
        [InlineData("zz-ZZ")]
        [InlineData("xx-YY")]
        [InlineData("zztt-ZZHH")]
        public void Load_UnknownCulture(string cultureCode)
        {
            _sut.Load(new XslTemplateRequest { CultureCode = cultureCode, FileFormat = "xml", Name = "Unknown", Path = "." });
            Assert.True(_sut.Loaded);
        }

        [Theory]
        [InlineData("TestXslt", "en-GB", "es-ES", "fr-FR")]
        public void Transform_Success(params string[] args)
        {
            Load(new Tuple<string, IEnumerable<string>>(args.First(), args.Skip(1)));
            var treeDoc = _sut[args.First()];
            Assert.NotNull(treeDoc);
            foreach (var culture in args.Skip(1))
            {
                var output = treeDoc.Transform(new MockXsltTransformRequest(), args.Last());
                Assert.NotNull(output);
                Assert.True(output != string.Empty);
            }
        }

        [Theory]
        [InlineData("TestXslt", "en-GB", "es-ES", "fr-FR")]
        public void Transform_Fail(params string[] args)
        {
            Load(new Tuple<string, IEnumerable<string>>(args.First(), args.Skip(1)));
            var treeDoc = _sut[args.First()];
            Assert.NotNull(treeDoc);
            Assert.Throws<Exception>(() => treeDoc.Transform(new MockXsltTransformRequest(), "zz-ZZ"));
        }

        private void Load(Tuple<string, IEnumerable<string>> input)
        {
            var requests = BuildRequests(input);
            _sut.Load(requests.ToArray());
        }

        private List<XslTemplateRequest> BuildRequests(Tuple<string, IEnumerable<string>> input)
        {
            var list = new List<XslTemplateRequest>();
            list.Add(new XslTemplateRequest
            {
                Path = "Files",
                Name = input.Item1,
                FileFormat = "xslt"

            });
            list.AddRange(input.Item2.Select(x => new XslTemplateRequest
            {
                Path = $"Files/{input.Item1}",
                Name = input.Item1,
                FileFormat = "xml",
                CultureCode = x
                
            }));
            return list;
        }

        [Serializable]
        public class MockXsltTransformRequest : XslTemplateWithLabels
        {
        }
    }
}
