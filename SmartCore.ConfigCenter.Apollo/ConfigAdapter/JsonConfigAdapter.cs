using SmartCore.ConfigCenter.Apollo.Core.Utils;
 
namespace SmartCore.ConfigCenter.Apollo.ConfigAdapter
{
    internal class JsonConfigAdapter : ContentConfigAdapter
    {
        public override Properties GetProperties(string content) => new Properties(JsonConfigurationParser.Parse(content));
    }
}
