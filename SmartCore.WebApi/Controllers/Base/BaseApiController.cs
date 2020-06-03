using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace SmartCore.WebApi
{
  /// <summary>
  /// 
  /// </summary>
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// 会话
        /// </summary>
        public virtual ISession Session => Sessions.Session.Instance;
    }
}
