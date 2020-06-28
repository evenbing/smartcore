using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.ConfigCenter.Apollo.ConfigAdapter
{
    public class ConfigurationPath
    {
        internal static string Combine(params string[] pathSegments) =>
           string.Join(":", pathSegments ?? throw new ArgumentNullException(nameof(pathSegments)));

        internal static string Combine(IEnumerable<string> pathSegments) =>
            string.Join(":", pathSegments ?? throw new ArgumentNullException(nameof(pathSegments)));
    }
}
