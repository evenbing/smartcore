using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
/// <summary>
/// 但我们还需要关注Token失效的问题。我们可以尽量设置短的过期时间，同时配合RefreshToken。不过这里不讨论这种方案，只采用黑名单机制（Redis）。

///在用户重新登录、退出登录的时候将相应Token加入黑名单；
///修改人员的时候记录人员和时间，将当时间之前的所有Token判定为无效；
///每次请求都通过Redis黑名单验证Token的有效性。 
/// </summary>
namespace SmartCore.Middleware
{
    /// <summary>
    /// 配合Redis验证Token有效性
    /// </summary>
    public class CustomJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        //private readonly IRedisRepo _redis;

        //public RevokableJwtSecurityTokenHandler(IServiceProvider serviceProvider)
        //{
        //    _redis = serviceProvider.GetRequiredService<IRedisRepo>();
        //}

        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters,
            out SecurityToken validatedToken)
        {
            var claimsPrincipal = base.ValidateToken(token, validationParameters, out validatedToken);
            //通过Redis验证Token
            if (IsTokenActive(claimsPrincipal))
            {
                throw new UnauthorizedException();
            }

            return claimsPrincipal;
        }
        public bool IsTokenActive(ClaimsPrincipal principal)
        {
            //解析ClaimsPrincipal取出UserId、Iat和Jti
            //具体的验证步骤有两个：
            //- 到Redis查找该用户的Token失效时间，如果当前Token的颁发时间在此之前就是无效的；
            //- 到Redis的黑名单里判断是否存在该Token；
            return true;
        }
    }

    //public void ConfigureServices(IServiceCollection services)
    //{
    //    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
    //    {
    //        opt.SecurityTokenValidators.Clear();
    //        opt.SecurityTokenValidators.Add(new CustomJwtSecurityTokenHandler(services.BuildServiceProvider()));
    //    });
    //}

    //public bool IsTokenActive(ClaimsPrincipal principal)
    //{
    //    //解析ClaimsPrincipal取出UserId、Iat和Jti
    //    //具体的验证步骤有两个：
    //    //- 到Redis查找该用户的Token失效时间，如果当前Token的颁发时间在此之前就是无效的；
    //    //- 到Redis的黑名单里判断是否存在该Token；
    //    return true;
    //}

}
