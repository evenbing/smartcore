using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SmartCore.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Services
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       PermissionRequirement requirement)
        {
           var tokenSetting = ConfigUtil.GetAppSettings<JwtConfig>("JwtConfig");
            List<PermissionRequirement> requirements = new List<PermissionRequirement>();
            foreach (var item in context.Requirements)
            {
                requirements.Add((PermissionRequirement)item);
            }
            foreach (var item in requirements)
            {
                // 校验 颁发和接收对象
                if (!(item.Issuer == tokenSetting.Issuer ?
                    item.Audience == tokenSetting.Audience ?
                    true : false : false))
                {
                    context.Fail();
                }
                // 校验过期时间
                var nowTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                var issued = item.IssuedTime + Convert.ToInt64(item.Expiration.TotalSeconds);
                if (issued < nowTime)
                    context.Fail(); 
                // 是否有访问此 API 的权限
                var resource = ((Microsoft.AspNetCore.Routing.RouteEndpoint)context.Resource).RoutePattern;
                #region TODO 暂时没有获取到权限
                //var permissions = item.Roles.Permissions.ToList();
                //var apis = permissions.Any(x => x.Name.ToLower() == item.Roles.Name.ToLower() && x.Url.ToLower() == resource.RawText.ToLower());
                //if (!apis)
                //    context.Fail();
                #endregion
                context.Succeed(requirement);
                // 无权限时跳转到某个页面
                //var httpcontext = new HttpContextAccessor();
                //httpcontext.HttpContext.Response.Redirect(item.DeniedAction);
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 用户所属角色
        /// </summary>
        public Role Roles { get; set; } = new Role();
        public void SetRolesName(string roleName)
        {
            Roles.Name = roleName;
        }
        /// <summary>
        /// 无权限时跳转到此API
        /// </summary>
        public string DeniedAction { get; set; }

        /// <summary>
        /// 认证授权类型
        /// </summary>
        public string ClaimType { internal get; set; }
        /// <summary>
        /// 未授权时跳转
        /// </summary>
        public string LoginPath { get; set; } = "/account/login";
        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 订阅人
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan Expiration { get; set; }
        /// <summary>
        /// 颁发时间
        /// </summary>
        public long IssuedTime { get; set; }
        /// <summary>
        /// 签名验证
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="deniedAction">无权限时跳转到此API</param>
        /// <param name="userPermissions">用户权限集合</param>
        /// <param name="deniedAction">拒约请求的url</param>
        /// <param name="permissions">权限集合</param>
        /// <param name="claimType">声明类型</param>
        /// <param name="issuer">发行人</param>
        /// <param name="audience">订阅人</param>
        /// <param name="issusedTime">颁发时间</param>
        /// <param name="signingCredentials">签名验证实体</param>
        public PermissionRequirement(string deniedAction, Role Role, string claimType, string issuer, string audience, SigningCredentials signingCredentials, long issusedTime, TimeSpan expiration)
        {
            ClaimType = claimType;
            DeniedAction = deniedAction;
            Roles = Role;
            Issuer = issuer;
            Audience = audience;
            Expiration = expiration;
            IssuedTime = issusedTime;
            SigningCredentials = signingCredentials;
        }

    }

    public class Role
    {
        public string Name { get; set; }
    }
}
