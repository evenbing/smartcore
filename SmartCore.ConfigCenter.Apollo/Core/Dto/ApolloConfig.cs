using System.Collections.Generic;

namespace SmartCore.ConfigCenter.Apollo.Core.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class ApolloConfig
    {
        public string AppId { get; set; } = default!;

        public string Cluster { get; set; } = default!;

        public string NamespaceName { get; set; } = default!;

        public string ReleaseKey { get; set; } = default!;

        public IDictionary<string, string> Configurations { get; set; } = default!;

        public override string ToString() => $"ApolloConfig{{appId='{AppId}{'\''}, cluster='{Cluster}{'\''}, namespaceName='{NamespaceName}{'\''}, configurations={Configurations}, releaseKey='{ReleaseKey}{'\''}{'}'}";
    }
}
