// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace Bodoconsult.NetworkCommunication.Helpers
{
    /// <summary>
    /// JSON helper class
    /// </summary>
    public static class JsonHelper
    {
        private static JsonSerializer _jsonSerializer;

        private static JsonSerializer JsonSerializer
        {
            get
            {
                if (_jsonSerializer == null)
                {
                    _jsonSerializer = new JsonSerializer
                    {
                        Formatting = Formatting.Indented,
                        TypeNameHandling = TypeNameHandling.Auto,
                        //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        MaxDepth = 10
                    };

                    _jsonSerializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                }

                return _jsonSerializer;
            }
        }


        private static readonly JsonSerializerSettings DefaultSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MaxDepth = 10
        };

        private static readonly JsonSerializerSettings NiceSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
            MaxDepth = 10
        };

        private static readonly JsonSerializerSettings ReadSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            // MaxDepth = 10  // Do not set MaxDepth!!!!
        };

        /// <summary>
        /// Serialize a object to JSON
        /// </summary>
        /// <param name="data">object to serialize</param>
        /// <returns>JSON string</returns>
        public static string JsonSerialize(object data)
        {
            if (data == null)
            {
                return null;
            }

            var jsonStr = JsonConvert.SerializeObject(data, DefaultSettings);
            return jsonStr;
        }

        /// <summary>
        /// Serialize a object to JSON
        /// </summary>
        /// <param name="data">object to serialize</param>
        /// <param name="maxDepth">Max depth level for the serialization</param>
        /// <returns>JSON string</returns>
        public static string JsonSerialize(object data, int maxDepth)
        {
            if (data == null)
            {
                return null;
            }

            try
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    MaxDepth = maxDepth
                };
                var jsonStr = JsonConvert.SerializeObject(data, jsonSerializerSettings);
                return jsonStr;
            }
            catch //(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Save an object of type T as JSON to a file
        /// </summary>
        /// <param name="fileName">full path for the JSON file</param>
        /// <param name="data">object to serialize to JSON</param>
        /// <typeparam name="T">type of the object to serialize to JSON</typeparam>
        /// <returns>JSON string</returns>
        public static string JsonSerialize<T>(T data, string fileName)
        {
            if (data == null)
            {
                return null;
            }

            var json = SerializeObject(data);

            File.WriteAllText(fileName, json, Encoding.UTF8);

            return json;
        }

        /// <summary>
        /// Deserialize a JSON string to a object of type T
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="json">JSON input string</param>
        /// <returns>Deserialized data object</returns>
        public static T JsonDeserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            try
            {
                var x = (T)JsonConvert.DeserializeObject(json, typeof(T), DefaultSettings);
                return x;
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                return default;
            }
        }

        /// <summary>
        /// Load a object from a JSON file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T LoadJsonFile<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return default;
            }

            //try
            //{
            //    var json = File.ReadAllText(fileName, Encoding.UTF8);
            //    //var x = (T)JsonConvert.DeserializeObject(json,  _readSettings);

            //    var serializer = new JsonSerializer
            //    {
            //        TypeNameHandling = TypeNameHandling.All,
            //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //        MaxDepth = 10
            //    };

            //   var  x = (T)serializer.Deserialize(json, typeof(T));

            //    return x;
            //}
            //catch (Exception e)
            //{
            //    Debug.Print(e.ToString());
            //    return default;
            //}

            try
            {
                T job;

                using (var file = File.OpenText(fileName))
                {
                    job = (T)JsonSerializer.Deserialize(file, typeof(T));
                }

                return job;
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                return default;
            }
        }


        /// <summary>
        /// Load a object from a JSON resource
        /// </summary>
        /// <param name="resourceName">Fully qualified resource name</param>
        /// <returns>object of type T or null</returns>
        public static T LoadJsonFromResource<T>(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                return default;
            }

            try
            {
                T job;

                var ass = typeof(JsonHelper).Assembly;
                var str = ass.GetManifestResourceStream(resourceName);

                if (str == null)
                {
                    return default;
                }

                using (var file = new StreamReader(str))
                {
                    var serializer = new JsonSerializer
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        // MaxDepth = 10
                    };

                    job = (T)serializer.Deserialize(file, typeof(T));

                }

                return job;
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                return default;
            }
        }


        /// <summary>
        /// Load an object from a JSON resource
        /// </summary>
        /// <param name="json">JSON formatted string</param>
        /// <returns>object of type T or null</returns>
        public static T LoadJsonFromString<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            try
            {

                //var settings = new JsonSerializerSettings
                //{
                //    TypeNameHandling = TypeNameHandling.All
                //};

                var job = JsonConvert.DeserializeObject(json, typeof(T), DefaultSettings);

                return (T)job;
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                return default;
            }
        }


        /// <summary>
        /// Serialize a object to JSON
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <typeparam name="T">type of object to convert</typeparam>
        /// <returns>Returns a JSON string or null if the JSON serialization failed</returns>
        public static string SerializeObject<T>(T value)
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                var sb = new StringBuilder(256);
                var sw = new StringWriter(sb, CultureInfo.InvariantCulture);

                //var jsonSerializer = new JsonSerializer
                //{
                //    Formatting = Formatting.Indented,
                //    TypeNameHandling = TypeNameHandling.Auto,
                //    //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //    MaxDepth = 10
                //};

                //jsonSerializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                using (var jsonWriter = new JsonTextWriter(sw))
                {
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.IndentChar = '\t';
                    jsonWriter.Indentation = 1;

                    JsonSerializer.Serialize(jsonWriter, value, typeof(T));
                }

                return sw.ToString();
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                return null;
            }


        }
        /// <summary>
        /// Serialize a object to a nicely readable JSON string
        /// </summary>
        /// <param name="data">object to serialize</param>
        /// <returns>Returns a JSON string or null if the JSON serialization failed</returns>
        public static string JsonSerializeNice(object data)
        {
            if (data == null)
            {
                return null;
            }

            try
            {
                var jsonStr = JsonConvert.SerializeObject(data, NiceSettings);
                return jsonStr;
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Save an object of type T as JSON to a file
        /// </summary>
        /// <param name="fileName">full path for the JSON file</param>
        /// <param name="data">object to serialize to JSON</param>
        /// <typeparam name="T">type of the object to serialize to JSON</typeparam>
        /// <returns>Returns a JSON string or null if the JSON serialization failed</returns>
        public static string JsonSerializeNice<T>(T data, string fileName)
        {
            if (data == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            try
            {
                var json = JsonConvert.SerializeObject(data, NiceSettings);

                File.WriteAllText(fileName, json, Encoding.UTF8);

                return json;
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                return null;
            }
        }
    }
}