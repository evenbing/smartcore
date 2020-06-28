using SmartCore.ConfigCenter.Apollo.Core.Utils;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartCore.ConfigCenter.Apollo
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheFileProvider
    {
        Properties Get(string configFile);

        void Save(string configFile, Properties properties);
    }
    /// <summary>
    /// 
    /// </summary>
    public class LocalPlaintextCacheFileProvider : ICacheFileProvider
    {
        public Properties Get(string configFile)
        {
            if (!File.Exists(configFile)) return new Properties(new Dictionary<string, string>());

            using var reader = new StreamReader(configFile, Encoding.UTF8);
            return new Properties(reader);
        }

        public void Save(string configFile, Properties properties)
        {
            using var file = new StreamWriter(configFile, false, Encoding.UTF8);
            properties.Store(file);
        }
    }
}
