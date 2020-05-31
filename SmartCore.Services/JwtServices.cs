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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SmartCore.Services
{
    public interface IJwtServices {
        string GenerateSecurityToken(UserTokenDTO userTokenDTO);
    }
    public class JwtServices: IJwtServices
    {
       
        private string _base64Secret;
        private TokenManagement tokenSetting = new TokenManagement();
        //public List<JwtAuthorizationDTO> _tokens = new List<JwtAuthorizationDTO>();
        //public JwtServices()
        //{
        //    tokenSetting = ConfigUtil.GetAppSettings<TokenManagement>("JwtConfig");
        //    //GetSecret();
        //}
        /// <summary>
        /// 获取 HTTP 请求上下文  必须在startup注册服务 services.AddHttpContextAccessor();
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JwtServices(IHttpContextAccessor httpContextAccessor)
        {
            tokenSetting = ConfigUtil.GetAppSettings<TokenManagement>("JwtConfig");
            _httpContextAccessor = httpContextAccessor;
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
            //每次登陆动态刷新
            Const.ValidAudience = userTokenDTO.Id + userTokenDTO.UserPass + DateTime.Now.ToString();
            DateTime nowTime = DateTime.UtcNow;
            long currentTimeStamp = new DateTimeOffset(nowTime).ToUnixTimeSeconds();
            long expiredTimeStamp = new DateTimeOffset(nowTime.AddMinutes(tokenSetting.AccessExpiration)).ToUnixTimeSeconds();
            var key = Encoding.UTF8.GetBytes(tokenSetting.Secret);
            //将用户信息添加到 Claim 中
            var claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new Claim(JwtRegisteredClaimNames.Iat, $"{currentTimeStamp}"),
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{currentTimeStamp}"),
                      new Claim(JwtRegisteredClaimNames.Exp,$"{expiredTimeStamp}")  ,
                    new Claim(ClaimTypes.Email, userTokenDTO.Email),
                    new Claim(ClaimTypes.Role,userTokenDTO.UserRole??""),
                    new Claim(ClaimTypes.NameIdentifier,userTokenDTO.UserName??""),//这里存的是aes加密后的用户主键id
                     new Claim(ClaimTypes.Expiration,expiredTimeStamp.ToString())
                }); 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = tokenSetting.Issuer,
                Audience = Const.ValidAudience,
                Subject = claimsIdentity,
                Expires = nowTime.AddMinutes(tokenSetting.AccessExpiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            if (_httpContextAccessor != null)
            {
                //签发一个加密后的用户信息凭证，用来标识用户的身份 
                _httpContextAccessor.HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)); 
            } 
            return tokenHandler.WriteToken(token);

        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="claims"></param>
        ///// <param name="tokenManagement"></param>
        ///// <returns></returns>
        //public dynamic BuildJwtToken(Claim[] claims, TokenManagement tokenManagement)
        //{
        //    var key = Encoding.UTF8.GetBytes(tokenManagement.Secret);
        //    var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        //    var now = DateTime.UtcNow;
        //    var jwt = new JwtSecurityToken(
        //        issuer: tokenManagement.Issuer,
        //        audience: tokenManagement.Audience,
        //        claims: claims,
        //        notBefore: now,
        //        expires: now.Add(TimeSpan.FromMinutes(tokenManagement.AccessExpiration)),
        //        signingCredentials: signingCredentials
        //    );
        //    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        //    var response = new
        //    {
        //        access_token = encodedJwt,
        //        expires_in = tokenManagement.AccessExpiration,
        //        token_type = "Bearer"
        //    };
        //    return response;
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //public ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        //{
        //    var handler = new JwtSecurityTokenHandler();
        //    try
        //    {
        //        var key = Encoding.UTF8.GetBytes(tokenSetting.Secret);
        //        return handler.ValidateToken(token, new TokenValidationParameters
        //        {
        //            ValidateActor = true,
        //            ValidateIssuerSigningKey = true,
        //            //获取或设置需要使用的microsoft.identitymodel.tokens.securitykey用于签名验证
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = true,
        //            ValidIssuer = tokenSetting.Issuer,
        //            ValidateAudience = true,
        //            //ValidIssuer = "ExampleIssuer",
        //            ValidAudience = tokenSetting.Audience,
        //            // Validate the token expiry
        //            ValidateLifetime = true,
        //            // If you want to allow a certain amount of clock drift, set that here:
        //            //注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟  ClockSkew = TimeSpan.FromSeconds(30)
        //            ClockSkew = TimeSpan.Zero,
        //            RequireExpirationTime = true
        //        }, out SecurityToken securityToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="Token"></param>
        ///// <param name="Clims"></param>
        ///// <returns></returns>
        //public bool ValidateToken(string Token, out Dictionary<string, string> Clims)
        //{
        //    Clims = new Dictionary<string, string>();
        //    ClaimsPrincipal principal = null;
        //    if (string.IsNullOrWhiteSpace(Token))
        //    {
        //        return false;
        //    }
        //    var handler = new JwtSecurityTokenHandler();
        //    try
        //    {
        //        var jwt = handler.ReadJwtToken(Token);

        //        if (jwt == null)
        //        {
        //            return false;
        //        }
        //        var key = Encoding.UTF8.GetBytes(tokenSetting.Secret);
        //        //var secretBytes = Convert.FromBase64String(this._base64Secret);
        //        var validationParameters = new TokenValidationParameters
        //        {
        //            RequireExpirationTime = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ClockSkew = TimeSpan.FromSeconds(30),
        //            ValidateIssuer = true,//是否验证Issuer
        //            ValidateAudience = true,//是否验证Audience
        //            ValidateLifetime = true,//是否验证失效时间
        //            ValidateIssuerSigningKey = true,//是否验证SecurityKey
        //            ValidAudience = this.tokenSetting.Audience,
        //            ValidIssuer = this.tokenSetting.Issuer
        //        };
        //        SecurityToken securityToken;
        //        principal = handler.ValidateToken(Token, validationParameters, out securityToken);
        //        foreach (var item in principal.Claims)
        //        {
        //            Clims.Add(item.Type, item.Value);
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        ///// <summary>
        ///// 从Token中获取用户身份
        ///// </summary>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //public ClaimsPrincipal GetPrincipalFromAccessToken(string token)
        //{
        //    var handler = new JwtSecurityTokenHandler();

        //    try
        //    {
        //        var key = Encoding.UTF8.GetBytes(tokenSetting.Secret);
        //        return handler.ValidateToken(token, new TokenValidationParameters
        //        {
        //            ValidateActor = true,
        //            ValidateLifetime = true, // Validate the token expiry 是否验证失效时间
        //            ValidateIssuerSigningKey = true,////是否验证SecurityKey
        //            //获取或设置需要使用的microsoft.identitymodel.tokens.securitykey用于签名验证
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = true,//是否验证Issuer
        //            ValidIssuer = tokenSetting.Issuer,
        //            ValidateAudience = true,//是否验证Audience
        //            //这里采用动态验证的方式，在退出、修改密码、重新登陆等动作时，刷新token，旧token就强制失效了 也可以用常量的形式，ValidAudience = tokenSetting.Audience,
        //            AudienceValidator = (m, n, z) =>
        //            {
        //                //if (n != null)
        //                //{
        //                //    var jwtSecurityToken = n as JwtSecurityToken;
        //                //    string userId = jwtSecurityToken.Payload["nameid"]?.ToString();
        //                //    return m != null && m.FirstOrDefault().Equals(Const.ValidAudience);
        //                //}
        //                return true;
        //                // return m != null && m.FirstOrDefault().Equals(Const.ValidAudience);
        //            },
        //            //If you want to allow a certain amount of clock drift, set that here:注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟  ClockSkew = TimeSpan.FromSeconds(30)
        //            ClockSkew = TimeSpan.FromSeconds(30),
        //            RequireExpirationTime = true
        //        }, out SecurityToken validatedToken);
               
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        ///// <summary>
        ///// 从Token中获取用户身份
        ///// </summary>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //public SecurityToken GetSecurityToken(string token)
        //{
        //    var handler = new JwtSecurityTokenHandler();

        //    try
        //    {
        //        var key = Encoding.UTF8.GetBytes(tokenSetting.Secret);
        //        handler.ValidateToken(token, new TokenValidationParameters
        //        {
        //            ValidateActor = true,
        //            ValidateLifetime = true, // Validate the token expiry 是否验证失效时间
        //            ValidateIssuerSigningKey = true,////是否验证SecurityKey
        //            //获取或设置需要使用的microsoft.identitymodel.tokens.securitykey用于签名验证
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = true,//是否验证Issuer
        //            ValidIssuer = tokenSetting.Issuer,
        //            ValidateAudience = true,//是否验证Audience
        //            //这里采用动态验证的方式，在退出、修改密码、重新登陆等动作时，刷新token，旧token就强制失效了 也可以用常量的形式，ValidAudience = tokenSetting.Audience,
        //            AudienceValidator = (m, n, z) =>
        //            {
        //                //if (n != null)
        //                //{
        //                //    var jwtSecurityToken = n as JwtSecurityToken;
        //                //    string userId = jwtSecurityToken.Payload["nameid"]?.ToString();
        //                //    return m != null && m.FirstOrDefault().Equals(Const.ValidAudience);
        //                //}
        //                return true;
        //                // return m != null && m.FirstOrDefault().Equals(Const.ValidAudience);
        //            },
        //            //If you want to allow a certain amount of clock drift, set that here:注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟  ClockSkew = TimeSpan.FromSeconds(30)
        //            ClockSkew = TimeSpan.FromSeconds(30),
        //            RequireExpirationTime = true
        //        }, out SecurityToken validatedToken);
        //        return validatedToken;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        //public async Task<JwtAuthorizationDTO> RefreshToken(string accessToken)
        //{
        //    var jwt = new JwtAuthorizationDTO();
        //    //将原来的token失效，并将refresh token也失效
        //    //var securityToken = GetPrincipalFromAccessToken(accessToken);
        //    var securityToken = GetPrincipalFromAccessToken(accessToken);
        //    if (securityToken is null)
        //    {
        //        return await Task.FromResult(jwt);
        //    }
        //    var jwtSecurityToken = securityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        //    //Debug.WriteLine(JsonConvert.SerializeObject(list));
        //    JwtAuthorizationDTO jwtAuthorizationDTO = new JwtAuthorizationDTO();
        //   // var jwtSecurityToken = securityToken as JwtSecurityToken;
        //    //Debug.WriteLine(JsonConvert.SerializeObject(jwtSecurityToken));
        //    string userId=jwtSecurityToken.Value?.ToString();
        //    //根据用户id获取userTokenDto
        //    UserTokenDTO userTokenDTO = new UserTokenDTO();
        //    jwtAuthorizationDTO.access_token = GenerateSecurityToken(userTokenDTO);
        //    return await Task.FromResult<JwtAuthorizationDTO>(jwt);
        //    //var id = principal.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value; 
        //    //if (string.IsNullOrEmpty(id))
        //    //{
        //    //    return await Task.FromResult<JwtAuthorizationDTO>(jwt);
        //    //}

        //}
        //    /// <summary>
        //    /// 刷新 Token
        //    /// </summary>
        //    /// <param name="token">Token</param>
        //    /// <param name="dto">用户信息</param>
        //    /// <returns></returns>
        //    public async Task<JwtAuthorizationDTO> RefreshAsync(string token, UserTokenDTO userTokenDto)
        //{
        //    JwtAuthorizationDTO jwtAuthorizationDTO = new JwtAuthorizationDTO();
        //    var jwtOld = GetExistenceToken(token);
        //    if (jwtOld == null)
        //    {
        //        return new JwtAuthorizationDTO()
        //        {
        //            access_token = "未获取到当前 Token 信息"
        //        };
        //    } 
        //    jwtAuthorizationDTO.access_token = GenerateSecurityToken(userTokenDto);
        //    //停用修改前的 Token 信息
        //    //await DeactivateCurrentAsync();

        //    return jwtAuthorizationDTO;
        //}

        ///// <summary>
        ///// 判断是否存在当前 Token
        ///// </summary>
        ///// <param name="token">Token</param>
        ///// <returns></returns>
        //private JwtAuthorizationDTO GetExistenceToken(string token)
        //    => _tokens.SingleOrDefault(x => x.access_token == token);
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
