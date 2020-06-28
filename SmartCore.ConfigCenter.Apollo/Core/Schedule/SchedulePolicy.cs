using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.ConfigCenter.Apollo.Core.Schedule
{
    /// <summary>
    /// 定时任务策略  
    /// </summary>
    public interface ISchedulePolicy
    {
        int Fail();

        void Success();
    }
}
