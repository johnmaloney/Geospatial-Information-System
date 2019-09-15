using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Universal.Contracts.Serial
{
    public static class Extensions
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
            SetJsonSettingUp();

            return JsonConvert.DeserializeObject<T>(json, jsonSettings);
        }
        
        public static T DeserializeJson<T>(this string json, Type typeOfObject)
        {
            SetJsonSettingUp();

            return (T)JsonConvert.DeserializeObject(json, typeOfObject, jsonSettings);
        }
        
        public static T DeserializeJson<T>(this string json, string typeOfObject)
        {
            SetJsonSettingUp();

            // Clean up the type name to avoid version information //
            // this is assuming the typeOfObject is the full assembly qualified name //
            typeOfObject = Regex.Replace(typeOfObject, @", Version=\d+.\d+.\d+.\d+", string.Empty);

            var type = Type.GetType(typeOfObject);

            if (type != null)
                return (T)JsonConvert.DeserializeObject(json, type, jsonSettings);
            else
                throw new NotSupportedException($"The typeOfObject: {typeOfObject} was not found.");
        }

        /// <summary>
        /// Convert a class object into a JSON object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string SerializeToJson<T>(this T objectToSerialize) where T : class
        {
            SetJsonSettingUp();

            jsonSettings.Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
                var m = args.ErrorContext.Error.Message;
                args.ErrorContext.Handled = true;
            };

            return 
                JsonConvert.SerializeObject(objectToSerialize, jsonSettings);
        }

        public static T FromJsonInto<T>(this string jsonData) where T : class
        {
            return jsonData.DeserializeJson<T>();
        }

        private static void SetJsonSettingUp()
        {
            if (jsonSettings == null)
            {
                jsonSettings = new JsonSerializerSettings();
                jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                jsonSettings.TypeNameHandling = TypeNameHandling.Auto;
                jsonSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
                jsonSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            }
        }

    }
}
