using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Tests.Utility
{
    public static class SerializationExtensions
    {
        private static JsonSerializerSettings jsonSettings;

        /// <summary>
        /// Reconstruct a object from a JSON object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeJson<T>(this string json) where T : class
        {
            if (jsonSettings == null)
            {
                jsonSettings = new JsonSerializerSettings();
                jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                jsonSettings.TypeNameHandling = TypeNameHandling.Auto;
                jsonSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
                jsonSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            }

            return JsonConvert.DeserializeObject<T>(json, jsonSettings);
        }

        /// <summary>
        /// Convert a class object into a JSON object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string SerializeToJson<T>(this T objectToSerialize) where T : class
        {
            if (jsonSettings == null)
            {
                jsonSettings = new JsonSerializerSettings();
                jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                jsonSettings.TypeNameHandling = TypeNameHandling.Auto;
                jsonSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
                jsonSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            }

            jsonSettings.Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
                var m = args.ErrorContext.Error.Message;
                args.ErrorContext.Handled = true;
            };

            return JsonConvert.SerializeObject(objectToSerialize, jsonSettings);
        }
    }
}
