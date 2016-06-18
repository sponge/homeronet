using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeronet.Services
{
    public interface IConfiguration
    {
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

        /// <summary>
        /// Gets a value indicating whether this <see cref="IConfiguration"/> was loaded from some source.
        /// </summary>
        /// <value><c>true</c> if existed; otherwise, <c>false</c>.</value>
        bool Exists { get; }
    }
}
