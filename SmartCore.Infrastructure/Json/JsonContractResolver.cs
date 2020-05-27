using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection; 
namespace SmartCore.Infrastructure.Json
{
    /// <summary>
    /// 所有的key转换成小写
    /// </summary>
    public class ToLowerPropertyNamesContractResolver : DefaultContractResolver
    {
        public ToLowerPropertyNamesContractResolver()
        {
            base.NamingStrategy = new NamingStrategyToLower();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class NamingStrategyToLower : NamingStrategy
    {
        /// <summary>
        /// Resolves the specified property name.
        /// </summary>
        /// <param name="name">The property name to resolve.</param>
        /// <returns>The resolved property name.</returns>
        protected override string ResolvePropertyName(string name)
        {
            return name.ToLower();
        }
    }
        /// <summary>
        /// ASP.NET Core WebApi 返回统一格式参数（Json 中 Null 替换为空字符串）
        /// </summary>
        public class NullToEmptyStringResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return type.GetProperties()
                    .Select(p => {
                        var jp = base.CreateProperty(p, memberSerialization);
                        jp.ValueProvider = new NullToEmptyStringValueProvider(p);
                        return jp;
                    }).ToList();
        }
    }

    public class NullToEmptyStringValueProvider : IValueProvider
    {
        PropertyInfo _MemberInfo;
        public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
        {
            _MemberInfo = memberInfo;
        }

        public object GetValue(object target)
        {
            object result = _MemberInfo.GetValue(target);
            if (result == null) result = "";
            return result;

        }

        public void SetValue(object target, object value)
        {
            _MemberInfo.SetValue(target, value);
        }
    }
}
