using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.ConfigCenter.Apollo.Core
{
    /// <summary>
    /// 配置中心全局静态常量 应用于默认的配置信息 比如默认的命名空间，默认的集群名称等等
    /// </summary>
    public class ConfigConsts
    {
        public const string NamespaceApplication = "application";
        public const string ClusterNameDefault = "default";
        public const string ClusterNamespaceSeparator = "+";
        public const string ConfigFileContentKey = "content";
        public const string NoAppidPlaceholder = "ApolloNoAppIdPlaceHolder";
        public const string DefaultMetaServerUrl = "http://localhost:8080";

        public const string ConfigService = "apollo-configservice";

        public static bool IsUnix { get; } = Environment.CurrentDirectory[0] == '/';
        public static string DefaultLocalCacheDir { get; } = IsUnix ? "/opt/data" : @"C:\opt\data";
    }
}
