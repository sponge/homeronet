using System;
using System.Collections.Generic;
using System.Linq;
using Homero.Core.Utility;

namespace Homero.Core.Services
{
    public interface IConfiguration
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IConfiguration"/> was loaded from some source.
        /// </summary>
        /// <value><c>true</c> if existed; otherwise, <c>false</c>.</value>
        bool Exists { get; }

        /// <summary>
        /// Gets a configuration value by a key value.
        /// </summary>
        /// <typeparam name="T">Expected type.</typeparam>
        /// <param name="key">Option key.</param>
        /// <returns>T.</returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Sets a configuration value.
        /// </summary>
        /// <param name="key">Option key.</param>
        /// <param name="value">The value to save.</param>
        void SetValue(string key, object value);

        event EventHandler<System.EventArgs> Changed;
        void Update(Dictionary<string, object> updatedDictionary);
        
        List<string> Keys { get; }
        Dictionary<string, object> BackingDictionary { get; }
        string Name { get; }
    }

    public class Configuration : IConfiguration
    {
        private Dictionary<string, object> _backingObject;
        public string Name { get; private set; }

        public Configuration(string className)
        {
            Name = className;
            _backingObject = new Dictionary<string, object>();
        }

        public void Update(Dictionary<string, object> updatedDictionary)
        {
            if (!_backingObject.DictionaryEqual(updatedDictionary))
            {
                Exists = true;
                _backingObject = updatedDictionary;
                Changed?.Invoke(this, new System.EventArgs());
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
            Changed?.Invoke(this, new System.EventArgs());
        }

        public event EventHandler<System.EventArgs> Changed;

        public bool Exists { get; private set; }

        public List<string> Keys
        {
            get { return _backingObject.Keys.ToList(); }
        }

        public Dictionary<string, object> BackingDictionary
        {
            get { return _backingObject; }
        }
    }
}