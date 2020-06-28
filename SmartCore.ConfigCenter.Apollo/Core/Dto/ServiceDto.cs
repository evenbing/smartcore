using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.ConfigCenter.Apollo.Core.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceDto
    {
        public string AppName { get; set; } = default!;

        public string HomepageUrl { get; set; } = default!;

        public string InstanceId { get; set; } = default!;

        public override string ToString() => $"ServiceDTO{{appName='{AppName}{'\''}, instanceId='{InstanceId}{'\''}, homepageUrl='{HomepageUrl}{'\''}{'}'}";
    }
}
