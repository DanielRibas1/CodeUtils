using DataCollections.Abstractions;
using DataCollections.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Xunit;

namespace DataCollections.Test
{    
    public class TreeInMemoryCollectionTests
    {
        private static string[] _cultures = { "en-GB", "es-ES", "fr-FR" };
        private TreeInMemoryCollection<string, string, Color> _sup;
                
        public TreeInMemoryCollectionTests()
        {
            _sup = new TreeInMemoryCollection<string, string, Color>(BuildInnerCollections());
            _sup.Refresh();
        }

        [Fact]
        public void Build_Success()
        {
            Assert.True(_sup.Loaded);
        }

      
        [Theory]
        [InlineData("en-GB")]
        [InlineData("es-ES")]
        [InlineData("fr-FR")]
        public void GetAllFromNode_Success(string firstKey)
        {
            var result = _sup.GetAllFromNode(firstKey);
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Theory]
        [InlineData("zh-CN")]
        [InlineData("ko-KR")]
        [InlineData("en-US")]
        public void GetAllFromNode_Fail(string firstKey)
        {
            var result = _sup.GetAllFromNode(firstKey);
            Assert.Null(result);
        }

        [Theory]
        [InlineData("en-GB", "AliceBlue", "FFF0F8FF")]
        [InlineData("en-GB", "BurlyWood", "FFDEB887")]
        [InlineData("en-GB", "DarkOliveGreen", "FF556B2F")]
        [InlineData("fr-FR", "Goldenrod", "FFDAA520")]
        [InlineData("fr-FR", "Lime", "FF00FF00")]
        [InlineData("fr-FR", "Moccasin", "FFFFE4B5")]
        public void GetNodeValue_Success(string firstKey, string secondKey, string expectedValue)
        {
            Color color;
            var result = _sup.TryGetNodeValue(firstKey,  secondKey, out color);
            Assert.True(result);            
            Assert.Equal(secondKey, color.Name);
            Assert.Equal(int.Parse(expectedValue, System.Globalization.NumberStyles.HexNumber), color.ToArgb());
        }

        [Theory]
        [InlineData("en-GB", "SomeUnexistentColor")]
        [InlineData("es-ES", "asdasdad")]
        public void GetNodeValue_Fail(string firstKey, string secondKey)
        {
            Color color;
            var result = _sup.TryGetNodeValue(firstKey, secondKey, out color);
            Assert.False(result);
        }

        [Fact]
        public void GetAllKeysByNode()
        {
            var result = _sup.GetAllKeysByNode();
            Assert.NotNull(result);
            Assert.Equal(_cultures.Length, result.Count);
            Assert.Equal(_cultures.OrderBy(x => x).First(), result.Keys.OrderBy(x => x).First());
            Assert.Equal(_cultures.OrderBy(x => x).Last(), result.Keys.OrderBy(x => x).Last());
        }

        private IEnumerable<KeyValuePair<string, IInMemoryCollection<string, Color>>> BuildInnerCollections()
        {
            Func<string, List<Color>> buildLambda = (s) =>
            {
                var rawResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
                return rawResult.Select(x => Color.FromName(x.Key)).ToList();
            };

            foreach (var culture in _cultures)
            {
                yield return new KeyValuePair<string, IInMemoryCollection<string, Color>>(
                    culture,
                    new JsonInMemoryCollection<string, Color>(
                        new JsonInMemoryCollectionSettings<string, Color>((v) => v.Name, new Uri("https://raw.githubusercontent.com/bahamas10/css-color-names/master/css-color-names.json"), buildLambda)));
            }
        }

    }
}
