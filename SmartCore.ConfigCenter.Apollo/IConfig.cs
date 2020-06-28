using SmartCore.ConfigCenter.Apollo.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;
namespace SmartCore.ConfigCenter.Apollo
{
    /// <summary>
    /// Config change event fired when there is any config change for the namespace.
    /// </summary>
    /// <param name="sender"> the sender </param>
    /// <param name="args"> the changes </param>
    public delegate void ConfigChangeEvent(IConfig sender, ConfigChangeEventArgs args);

    public interface IConfig
    {
        /// <summary>Return the property value with the given key. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="value"> the value </param>
        /// <returns> true: the key is found; false the key is not found </returns>
        bool TryGetProperty(string key, [NotNullWhen(true)]out string? value);

        /// <summary>
        /// Return a set of the property names
        /// </summary>
        /// <returns> the property names </returns>
        IEnumerable<string> GetPropertyNames();

        /// <summary>
        /// Config change event subscriber
        /// </summary>
        event ConfigChangeEvent ConfigChanged;
    }
}
