using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Homero.Core.Services.Configuration
{
    public class JsonConfiguration : IConfiguration
    {
        private string _backingFilePath;
        private Dictionary<string, object> _backingObject;

        public JsonConfiguration(string filePath)
        {
            _backingFilePath = filePath;

            if (File.Exists(filePath))
            {
                Exists = true;
                _backingObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(filePath));
            }
            else
            {
                _backingObject = new Dictionary<string, object>();
            }
        }

        public T GetValue<T>(string key)
        {
            if (_backingObject.ContainsKey(key))
            {
                var result = _backingObject[key];
                if (result is T)
                {
                    return (T) result;
                }

                try
                {
                    return (T) Convert.ChangeType(result, typeof(T));
                }
                catch (InvalidCastException)
                {
                }
            }

            return default(T);
        }

        public void SetValue(string key, object value)
        {
            _backingObject[key] = value;
            File.WriteAllText(_backingFilePath, JsonConvert.SerializeObject(_backingObject));
        }

        public bool Exists { get; private set; }
    }
}