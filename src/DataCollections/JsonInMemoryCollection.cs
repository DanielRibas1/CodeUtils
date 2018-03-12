using DataCollections.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataCollections
{
    public class JsonInMemoryCollection<TKey, TValue> : InMemoryCollection<TKey, TValue>
    {
        private readonly object _refreshLock = new object();        
        private bool _refreshing = false;
        private JsonInMemoryCollectionSettings<TKey, TValue> _collectionSettings;

        public JsonInMemoryCollection(JsonInMemoryCollectionSettings<TKey, TValue> collectionSettings) : base()
        {
            _collectionSettings = collectionSettings;
        }     

        public override bool Loaded { get; protected set; }

        public override void Refresh()
        {
            if (!_refreshing)
            {
                lock (_refreshLock)
                {
                    _refreshing = true;
                    try
                    {
                        switch (_collectionSettings.LoadType)
                        {
                            case JsonInMemoryLoadTypes.File:
                                LoadFromFile();
                                break;
                            case JsonInMemoryLoadTypes.Uri:
                                LoadFromUrl();
                                break;
                            case JsonInMemoryLoadTypes.Raw:
                                LoadFromRaw();
                                break;
                        }                      
                        Loaded = true;
                    }
                    finally
                    {
                        _refreshing = false;
                    }                   
                }
            }
        }

        private void LoadFromFile()
        {
            if (!File.Exists(_collectionSettings.JsonFilePath))
                Trace.WriteLine($"Path for JSON not found at {_collectionSettings.JsonFilePath}");
            else
            {
                var jsonRaw = File.ReadAllText(_collectionSettings.JsonFilePath);
                StoreValues(ref jsonRaw);
            }
        }

        private void LoadFromUrl()
        {
            if (!_collectionSettings.UrlLocation.IsAbsoluteUri)
                Trace.WriteLine($"URL is not absolute, {_collectionSettings.UrlLocation}");
            else
                using (var client = new WebClient())
                {
                    var jsonRaw = client.DownloadString(_collectionSettings.UrlLocation.ToString());
                    StoreValues(ref jsonRaw);
                }
        }

        private void LoadFromRaw()
        {
            if (string.IsNullOrEmpty(_collectionSettings.RawJsonText))
                Trace.WriteLine($"Raw JSON file {_collectionSettings.RawJsonTextName} has no content");
            else
            {
                var jsonRaw = _collectionSettings.RawJsonText;
                StoreValues(ref jsonRaw);
            }
        }

        private void StoreValues(ref string jsonRaw)
        {
            try
            {
                List<TValue> result;
                if (_collectionSettings.DeserialzeFunc == null)
                    result = JsonConvert.DeserializeObject<List<TValue>>(jsonRaw);
                else
                    result = _collectionSettings.DeserialzeFunc(jsonRaw);
                InnerValues.Clear();
                foreach (var item in result)
                    InnerValues.TryAdd(_collectionSettings.LocateKey(item), item);
            }
            catch (JsonReaderException ex)
            {                
                Trace.WriteLine($"Error Deserializing {_collectionSettings.LoadType} {_collectionSettings.GetCurrentSource}, {ex.ToString()}");
                InnerValues.Clear();
            }           
        }
    }
}
