using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.ConfigCenter.Apollo.Internals
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConfigRepositoryFactory
    {
        IConfigRepository GetConfigRepository(string @namespace);
    }
}
