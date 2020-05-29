using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartCore.Services;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SmartCore.Middleware
{
    public static class AuthMiddlewareExtension
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration config)
        { 
            var tokenSetting = config.GetSection("JwtConfig").Get<TokenManagement>();
            var key = Encoding.UTF8.GetBytes(tokenSetting.Secret);
            //添加基于策略授权的方法
            //验证授权模式是否为jwt bearer
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                //Jwt bearer token 信息验证
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateActor=true,
                    ValidateIssuerSigningKey=true,
                    //获取或设置需要使用的microsoft.identitymodel.tokens.securitykey用于签名验证
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = tokenSetting.Issuer,
                    ValidateAudience = true,
                    //ValidIssuer = "ExampleIssuer",
                    ValidAudience = tokenSetting.Audience,
                     // Validate the token expiry
                    ValidateLifetime = true,
                    // If you want to allow a certain amount of clock drift, set that here:
                    //注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟  ClockSkew = TimeSpan.FromSeconds(30)
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime=true
                };
                //x.Events = new JwtBearerEvents()
                //{
                //    // 在安全令牌通过验证和ClaimsIdentity通过验证之后调用
                //    // 如果用户访问注销页面
                //    OnTokenValidated = context =>
                //    {
                //        if (context.Request.Path.Value.ToString() == "/account/logout")
                //        {
                //            var token = ((context as TokenValidatedContext).SecurityToken as JwtSecurityToken).RawData;
                //        }
                //        return Task.CompletedTask;
                //    }
                //};
                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        //Token expired
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
     
}
