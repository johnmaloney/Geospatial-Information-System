using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TileFactory.Tests.Utility
{
    public interface IConfigurationStrategy
    {
        string GetJson(string identifier);
        T Into<T>(string identifier, params Func<JObject, string>[] jsonAlterations) where T : class;
        T Into<T>(string identifier) where T : class;
        T FromInto<T>(string jsonData) where T : class;

    }

    public class ConfigurationStrategy : IConfigurationStrategy
    {
        #region Fields

        internal static string Location;
        private static ConcurrentDictionary<string, string> mockJson = new ConcurrentDictionary<string, string>();

        private string dataDirectory;
        private object fileLock = new Object();
        
        #endregion

        #region Properties



        #endregion

        #region Methods

        public ConfigurationStrategy()
        {
            if (string.IsNullOrEmpty(Location))
                Location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            dataDirectory = $"{Location}\\Data\\";
        }

        public string GetJson(string identifier)
        {
            if (!mockJson.ContainsKey(identifier))
            {
                lock (fileLock)
                {
                    // Attempt to find the file with the keyHydrob name i.e. 272.json //
                    var mockJsonFile = Directory.GetFiles(
                        this.dataDirectory).FirstOrDefault(f => 
                        f.Contains($"{identifier}.json") || f.Contains($"{identifier}.geojson"));

                    if (mockJsonFile == null)
                        throw new NotSupportedException(string.Format("The file named: {0} was not found in the Directory: {1}. The file must be first built and stored in this directory to allow MockTables to work.",
                            identifier.ToString() + ".json|.geojson",
                            this.dataDirectory));


                    var key = Path.GetFileNameWithoutExtension(mockJsonFile);
                    string text = File.ReadAllText(mockJsonFile);


                    if (!mockJson.TryAdd(key, text))
                    {
                        throw new Exception($"Adding the JSON with Id: {identifier} was unsuccessful.");
                    }
                }
            }
            return mockJson[identifier];
        }

        public T Into<T>(string identifier, params Func<JObject, string>[] jsonAlterations) where T : class
        {
            var json = GetJson(identifier);
            foreach (var jsonAlteration in jsonAlterations)
            {
                json = jsonAlteration((JObject)JsonConvert.DeserializeObject(json));
            }
            return json.DeserializeJson<T>();
        }

        public T Into<T>(string identifier) where T : class
        {
            var json = GetJson(identifier);
            return json.DeserializeJson<T>();
        }

        public T FromInto<T>(string jsonData) where T : class
        {
            return jsonData.DeserializeJson<T>();
        }

        #endregion
    }
}
