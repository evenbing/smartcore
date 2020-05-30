using System;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using SmartCore.Infrastructure.Config;
using System.Collections.Generic;
using System.Security.Cryptography;
using SmartCore.Models.DTO;

namespace SmartCore.Services
{
    public class JwtService
    {
        private string _base64Secret;
        private TokenManagement tokenSetting = new TokenManagement();
        public JwtService()
        {
            tokenSetting = ConfigUtil.GetAppSettings<TokenManagement>("JwtConfig");
            //GetSecret();
        }
        /// <summary>
        /// 获取到加密串
        /// </summary>
        private void GetSecret()
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes("salt");
            byte[] messageBytes = encoding.GetBytes(this.tokenSetting.Secret);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                this._base64Secret = Convert.ToBase64String(hashmessage);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string GenerateSecurityToken(UserTokenDTO userTokenDTO)
        {

            var key = Encoding.UTF8.GetBytes(tokenSetting.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = tokenSetting.Issuer,
                Audience = tokenSetting.Audience,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim(ClaimTypes.Email, userTokenDTO.Email),
                    new Claim(ClaimTypes.Role,userTokenDTO.UserRole),
                     new Claim(ClaimTypes.Expiration,DateTime.Now.AddMinutes(tokenSetting.AccessExpiration).ToString()),
                       //这个Role是官方UseAuthentication要要验证的Role，我们就不用手动设置Role这个属性了
                    //new Claim(ClaimTypes.Role,tokenModel.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(tokenSetting.AccessExpiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="tokenManagement"></param>
        /// <returns></returns>
        public  dynamic BuildJwtToken(Claim[] claims, TokenManagement tokenManagement)
        {
            var key = Encoding.UTF8.GetBytes(tokenManagement.Secret);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: tokenManagement.Issuer,
                audience: tokenManagement.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(tokenManagement.AccessExpiration)),
                signingCredentials: signingCredentials
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                access_token = encodedJwt,
                expires_in = tokenManagement.AccessExpiration,
                token_type = "Bearer"
            };
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var key = Encoding.UTF8.GetBytes(tokenSetting.Secret);
                return handler.ValidateToken(token, new  TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateIssuerSigningKey = true,
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
                    RequireExpirationTime = true
                } ,out SecurityToken securityToken);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Clims"></param>
        /// <returns></returns>
        public bool ValidateToken(string Token, out Dictionary<string, string> Clims)
        {
            Clims = new Dictionary<string, string>();
            ClaimsPrincipal principal = null;
            if (string.IsNullOrWhiteSpace(Token))
            {
                return false;
            }
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwt = handler.ReadJwtToken(Token);

                if (jwt == null)
                {
                    return false;
                }
                var key = Encoding.UTF8.GetBytes(tokenSetting.Secret);
                //var secretBytes = Convert.FromBase64String(this._base64Secret);
                var validationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,//是否验证Issuer
                    ValidateAudience = true,//是否验证Audience
                    ValidateLifetime =true,//是否验证失效时间
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    ValidAudience = this.tokenSetting.Audience,
                    ValidIssuer = this.tokenSetting.Issuer
                };
                SecurityToken securityToken;
                principal = handler.ValidateToken(Token, validationParameters, out securityToken);
                foreach (var item in principal.Claims)
                {
                    Clims.Add(item.Type, item.Value);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    //Something like this...

    //    public string GenerateSecurityToken(User user)
    //    {
    //        ...    
    //Subject = new ClaimsIdentity(new[]
    //                {
    //                    new Claim(ClaimTypes.Email, user.Email),
    //                    new Claim(ClaimTypes.Name, user.Name),
    //                    new Claim(ClaimTypes.Role, user.Role),
    //                    new Claim(ClaimTypes.DateOfBirth, user.DOB),
    //                })...    
    //}
}
