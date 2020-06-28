using SmartCore.ConfigCenter.Apollo.Core.Utils; 
using System.IO;
namespace SmartCore.ConfigCenter.Apollo.ConfigAdapter
{
    /// <summary>
    /// 
    /// </summary>
    internal class XmlConfigAdapter : ContentConfigAdapter
    {
        public override Properties GetProperties(string content)
        {
            using var reader = new StringReader(content);
            return new Properties(XmlConfigurationParser.Read(reader));
        }
    }
}
