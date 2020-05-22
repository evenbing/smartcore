using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace SmartCore.Middleware
{
    public static class AuthMiddlewareExtension
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var secret = config.GetSection("JwtConfig").GetSection("secret").Value;
            var audience = config.GetSection("JwtConfig").GetSection("audience").Value;
            var issuer = config.GetSection("JwtConfig").GetSection("issuer").Value; //issuer
            var key = Encoding.ASCII.GetBytes(secret);
            //添加基于策略授权的方法
            //验证授权模式是否为jwt bearer
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                //Jwt bearer token 信息验证
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = audience,
                    ValidAudience = issuer
                };
            });

            return services;
        }
    }
}
