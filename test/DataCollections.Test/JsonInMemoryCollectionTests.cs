using DataCollections.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Xunit;

namespace DataCollections.Test
{    
    public class JsonInMemoryCollectionTests
    {
        
        public JsonInMemoryCollectionTests()
        {
            JsonInMemoryCollectionSettings<int, IntStringPair>.RelativeSearchPath = Directory.GetCurrentDirectory();
        }

        [Theory]
        [InlineData("Files\\CodeList.json")]
        public void FileJsonCollections_Success(string path)
        {
            var opDelaysCodesSettings = new JsonInMemoryCollectionSettings<int, IntStringPair>((v) => v.Code, path);
            Assert.Equal(opDelaysCodesSettings.JsonFilePath, opDelaysCodesSettings.GetCurrentSource);
            TestJsonCollection(opDelaysCodesSettings);
        }

        [Fact]
        public void FileJsonCollections_Null_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonInMemoryCollectionSettings<int, IntStringPair>((v) => v.Code, jsonFilePath: null));
        }

        [Theory]
        [InlineData("NoRoute")]
        public void FileJsonCollections_NoElements_Fail(string path)
        {
            var settings = new JsonInMemoryCollectionSettings<int, IntStringPair>((v) => v.Code, path);
            var sut = new JsonInMemoryCollection<int, IntStringPair>(settings);
            sut.Refresh();
            Assert.NotNull(sut.GetAll());
            Assert.False(sut.GetAll().Any());
        }

        [Theory]
        [InlineData("Files\\CodeList.json")]
        public void RawJsonCollection_Success(string fileSourcePath)
        {
            var rawJsonName = "TestJson";
            var rawCollectionSettings = new JsonInMemoryCollectionSettings<int, Tuple<int, string>>((v) => v.Item1, rawJsonName, File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), fileSourcePath)));
            Assert.Equal(rawJsonName, rawCollectionSettings.RawJsonTextName);
            TestJsonCollection(rawCollectionSettings);
        }

        [Theory]
        [InlineData("Files\\CodeList.json")]
        public void RawJsonCollection_BadFormat_Fail(string fileSourcePath)
        {
            var rawJsonName = "TestJson";
            var rawCollectionSettings = new JsonInMemoryCollectionSettings<int, Tuple<int, string>>(
                (v) => v.Item1, 
                rawJsonName, 
                File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), fileSourcePath)).Substring(0, 15));
            var failSup = new JsonInMemoryCollection<int, Tuple<int, string>>(rawCollectionSettings);
            failSup.Refresh();
            Assert.NotNull(failSup.GetAll());
            Assert.False(failSup.GetAll().Any());
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void RawJsonCollection_NoElements_Fail(string rawFailText)
        {
            var failSup = new JsonInMemoryCollection<int, Tuple<int, string>>(new JsonInMemoryCollectionSettings<int, Tuple<int, string>>((v) => v.Item1, "TestJson", rawFailText));
            failSup.Refresh();
            Assert.NotNull(failSup.GetAll());
            Assert.False(failSup.GetAll().Any());
        }

        [Theory]
        [InlineData("https://raw.githubusercontent.com/bahamas10/css-color-names/master/css-color-names.json")]        
        public void UrlJsonCollection_Success(string url)
        {
            var colorsCollection =  new JsonInMemoryCollectionSettings<string, Color>(
               locateKey: (v) => v.Name,
               urlLocation: new Uri(url, UriKind.Absolute),
               deserialzeFunc: (s) =>
               {
                   var rawResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
                   return rawResult.Select(x => Color.FromName(x.Key)).ToList();
               });
            TestJsonCollection(colorsCollection);
        }

        [Theory]
        [InlineData("whereaver\nana", UriKind.Relative)]
        [InlineData(@"..\..\Test.html", UriKind.Relative)]
        [InlineData(@"http://www.google.com", UriKind.Absolute)]
        public void UrlJsonCollection_NoElemetns_Fail(string url, UriKind uriKind)
        {
            var collection = new JsonInMemoryCollectionSettings<string, object>(
               locateKey: (v) => v.ToString(),
               urlLocation: new Uri(url, uriKind));               
            var sut = BuildCollection(collection);
            sut.Refresh();
            Assert.NotNull(sut.GetAll());
            Assert.False(sut.GetAll().Any());
        }       

        private JsonInMemoryCollection<TKey, TValue> BuildCollection<TKey, TValue>(JsonInMemoryCollectionSettings<TKey, TValue> settings)
        {
            return new JsonInMemoryCollection<TKey, TValue>(settings);
        }

        private void TestJsonCollection<TKey, TValue>(JsonInMemoryCollectionSettings<TKey, TValue> settings)
        {
            var sut = new JsonInMemoryCollection<TKey, TValue>(settings);
            sut.Refresh();
            Assert.True(sut.Loaded);
            var keys = sut.GetAllKeys();
            Assert.True(keys.Count() > 0);
            Assert.Equal(keys.Count(), sut.GetAll().Count());
            TValue value;
            Assert.True(sut.TryGet(keys.First(), out value));            
        }

        private class IntStringPair
        {
            public int Code { get; set; }
            public string Name { get; set; }
        }

    }
}
