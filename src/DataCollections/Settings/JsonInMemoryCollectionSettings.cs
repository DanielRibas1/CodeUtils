using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollections.Settings
{
    public class JsonInMemoryCollectionSettings<TKey, TValue>
    {
        internal static string RelativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
        public JsonInMemoryCollectionSettings(Func<TValue, TKey> locateKey, Uri urlLocation, Func<string, List<TValue>> deserialzeFunc = null)
        {
            this.LocateKey = locateKey;            
            this.DeserialzeFunc = deserialzeFunc;
            this.UrlLocation = urlLocation;
            this.LoadType = JsonInMemoryLoadTypes.Uri;
        }

        public JsonInMemoryCollectionSettings(Func<TValue, TKey> locateKey, string jsonFilePath, Func<string, List<TValue>> deserialzeFunc = null)
        {
            this.LocateKey = locateKey;
            this.DeserialzeFunc = deserialzeFunc;
            if (string.IsNullOrEmpty(jsonFilePath))
                throw new ArgumentNullException("jsonFilePath");
            this.JsonFilePath = Path.Combine(RelativeSearchPath, jsonFilePath);
            this.LoadType = JsonInMemoryLoadTypes.File;
        }

        public JsonInMemoryCollectionSettings(Func<TValue, TKey> locateKey, string rawJsonTextName, string rawJsonText, Func<string, List<TValue>> deserialzeFunc = null)
        {
            this.LocateKey = locateKey;
            this.DeserialzeFunc = deserialzeFunc;
            this.RawJsonTextName = rawJsonTextName;
            this.RawJsonText = rawJsonText;
            this.LoadType = JsonInMemoryLoadTypes.Raw;
        }

        public Func<TValue, TKey> LocateKey { get; private set; }
        public Func<string, List<TValue>> DeserialzeFunc { get; private set; }
        public Uri UrlLocation { get; private set; }
        public string JsonFilePath { get; private set; }
        public string RawJsonTextName { get; private set; }
        public string RawJsonText { get; private set; }
        public JsonInMemoryLoadTypes LoadType { get; private set; }
        public string GetCurrentSource
        {
            get
            {
                switch (this.LoadType)
                {
                    case JsonInMemoryLoadTypes.File:
                        return this.JsonFilePath;                    
                    case JsonInMemoryLoadTypes.Uri:
                        return this.UrlLocation.ToString();
                    case JsonInMemoryLoadTypes.Raw:
                    default:
                        return this.RawJsonText;
                }
            }
        }

    }
}
