﻿using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartCore.Services;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;
using SmartCore.Models;
using Newtonsoft.Json;
using System.Diagnostics;

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
                //获取权限是否需要HTTPS 
                x.RequireHttpsMetadata = false; 
                //x.SecurityTokenValidators.Clear();
                //x.SecurityTokenValidators.Add(new CustomJwtSecurityTokenHandler());//services.BuildServiceProvider()
                //在成功的授权之后令牌是否应该存储在Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties中
                x.SaveToken = true;
                //Jwt bearer token 信息验证
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateActor=true,
                    ValidateLifetime = true, // Validate the token expiry 是否验证失效时间
                    ValidateIssuerSigningKey =true,////是否验证SecurityKey
                    //获取或设置需要使用的microsoft.identitymodel.tokens.securitykey用于签名验证
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,//是否验证Issuer
                    ValidIssuer = tokenSetting.Issuer,
                    ValidateAudience = true,//是否验证Audience
                    //这里采用动态验证的方式，在退出、修改密码、重新登陆等动作时，刷新token，旧token就强制失效了 也可以用常量的形式，ValidAudience = tokenSetting.Audience,
                    AudienceValidator = (m, n, z) =>
                    {
                        if (n!=null)
                        {
                            var jwtSecurityToken = n as JwtSecurityToken;
                            string userId = jwtSecurityToken.Payload["nameid"]?.ToString();
                            //通过userid解密出来，然后找到对应的redis就是Const.ValidAudience对应的值
                            return m != null && m.FirstOrDefault().Equals(Const.ValidAudience);
                        }
                        return false;
                       // return m != null && m.FirstOrDefault().Equals(Const.ValidAudience);
                    }, 
                    //If you want to allow a certain amount of clock drift, set that here:注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟  ClockSkew = TimeSpan.FromSeconds(30)
                    ClockSkew = TimeSpan.FromSeconds(30),
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
                    //此处为权限验证失败后触发的事件
                    OnChallenge = context =>
                    {
                        //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦，必须
                        context.HandleResponse();
                        //自定义返回的数据类型
                        context.Response.ContentType = "application/json";
                        //自定义返回状态码，默认为401 我这里改成 200
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //自定义自己想要返回的数据结果，我这里要返回的是Json对象，通过引用Newtonsoft.Json库进行转换
                        var result = new ApiResultModels();
                        result.code = context.Response.StatusCode;
                        result.message = "很抱歉，您无权访问该接口!";
                        //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //输出Json数据结果
                        context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                        return Task.FromResult(0);
                    },
                    OnAuthenticationFailed = context =>
                    {
                        //如果过期，则把<是否过期>添加到，返回头信息中
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
