using System;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt; 
using SmartCore.Infrastructure.Config; 
using System.Security.Cryptography;
using SmartCore.Models.DTO; 
using System.Threading.Tasks;
using SmartCore.Infrastructure;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SmartCore.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJwtServices
    {
        #region 生成token
        Task<JwtAuthorizationDTO> GenerateSecurityToken(UserTokenDTO userTokenDTO);
        #endregion
        #region 校验token的有效性

        Task<ClaimsPrincipal> ValidateToken(string token);
        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public class JwtServices : BaseService, IJwtServices
    { 
        private JwtIssuerOptions tokenSetting = new JwtIssuerOptions();
        private readonly ILogger<JwtServices> _logger;
        private IConfiguration _configration;

        public JwtServices(ILogger<JwtServices> logger, IConfiguration configuration)
        {
            _configration = configuration;
            string city = _configration["city"];
            // Config appConfig = ConfigService.getAppConfig();
            var jwt = _configration["JwtConfig"];
            var te = _configration.GetSection("JwtConfig");
            tokenSetting = ConfigUtil.GetAppSettings<JwtIssuerOptions>("JwtConfig");
            _logger = logger;
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
                //this._base64Secret = Convert.ToBase64String(hashmessage);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userTokenDTO"></param>
        /// <returns></returns>
        public async Task<JwtAuthorizationDTO> GenerateSecurityToken(UserTokenDTO userTokenDTO)
        {
            if (tokenSetting.AccessExpiration == 0)
            {
                tokenSetting.AccessExpiration = 120;
            } 
            string userConfoundId = DigitsUtil.ConvertTo62RadixString(userTokenDTO.Id);
            string deviceType = DeviceType;
            DateTime nowTime = DateTime.UtcNow.ToLocalTime();
            long currentTimeStamp = new DateTimeOffset(nowTime).ToUnixTimeSeconds();
            long expiredTimeStamp = new DateTimeOffset(nowTime.AddMinutes(tokenSetting.AccessExpiration)).ToUnixTimeSeconds();
            long refreshExpiredTimeStamp = new DateTimeOffset(nowTime.AddMinutes(tokenSetting.RefreshExpiration)).ToUnixTimeSeconds();
            DateTime expiresTime = nowTime.AddMinutes(tokenSetting.AccessExpiration).AddSeconds(30);//过期时间+30m
            var secretKey = Encoding.UTF8.GetBytes(tokenSetting.Secret);
            string jti = Guid.NewGuid().ToString("N");
            //每次登陆动态刷新
            string redisValue = string.Concat(userConfoundId, "|", currentTimeStamp, "|", expiredTimeStamp,"|",Guid.NewGuid().ToString("N"));
            Const.ValidAudience = redisValue;
            //将用户信息添加到 Claim 中
            var claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti,jti),
                    new Claim(JwtRegisteredClaimNames.Iat, $"{currentTimeStamp}",ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{currentTimeStamp}",ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Exp,$"{expiredTimeStamp}",ClaimValueTypes.Integer64)  ,
                    new Claim(ClaimTypes.Email, userTokenDTO.Email??""),
                    new Claim(ClaimTypes.Role,userTokenDTO.UserRole??""),
                    new Claim(ClaimTypes.NameIdentifier,userConfoundId),//这里存的是aes加密后的用户主键id
                    new Claim(ClaimTypes.Expiration,$"{expiredTimeStamp}",ClaimValueTypes.Integer64),
                    new Claim(ClaimTypes.Name, deviceType),
                });
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = tokenSetting.Issuer,
                Audience = redisValue,
                Subject = claimsIdentity,
                Expires = expiresTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //签发一个加密后的用户信息凭证，用来标识用户的身份 
            //var defaultScheme =  JwtBearerDefaults.AuthenticationScheme;
            //await _httpContextAccessor.HttpContext.SignInAsync(defaultScheme, new ClaimsPrincipal(claimsIdentity));
            string refreshToken = Guid.NewGuid().ToString("N");
            var result = new JwtAuthorizationDTO
            {
                id = userConfoundId,
                token_type = "Bearer",
                access_token = new AccessToken { token = tokenHandler.WriteToken(token), expired = expiredTimeStamp, expires_in = tokenSetting.AccessExpiration },
                refresh_token = new AccessToken { token = refreshToken, expired = refreshExpiredTimeStamp, expires_in = tokenSetting.RefreshExpiration }
            };
            //await CacheManager.Instance.Set($"user:{deviceType}:token:{userTokenDTO.Id}", redisValue, expiresTime);
            //await CacheManager.Instance.Set($"user:{deviceType}:refresh_token:{refreshToken}", result.access_token.token, refreshExpiredTimeStamp);
            _logger.LogInformation("生成token成功", token);
            return await Task.FromResult(result);
        }
        //private ClaimsIdentity GenerateClaimsIdentity(UserTokenDTO userTokenDTO)
        //{ 
        
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ClaimsPrincipal> ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var securityKey = Encoding.UTF8.GetBytes(tokenSetting.Secret);
                var result = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateLifetime = true, // Validate the token expiry 是否验证失效时间
                    ValidateIssuerSigningKey = true,////是否验证SecurityKey
                    //获取或设置需要使用的microsoft.identitymodel.tokens.securitykey用于签名验证
                    IssuerSigningKey = new SymmetricSecurityKey(securityKey),
                    ValidateIssuer = true,//是否验证Issuer
                    ValidIssuer = tokenSetting.Issuer,
                    ValidateAudience = true,//是否验证Audience
                    //这里采用动态验证的方式，在退出、修改密码、重新登陆等动作时，刷新token，旧token就强制失效了 也可以用常量的形式，ValidAudience = tokenSetting.Audience,
                    AudienceValidator = (m, n, z) =>
                    {
                        if (m!=null&&n != null)
                        {
                            var jwtSecurityToken = n as JwtSecurityToken;
                            string userId = jwtSecurityToken.Payload["nameid"]?.ToString();
                            string deviceType = jwtSecurityToken.Payload["unique_name"]?.ToString();
                            int userKeyId = DigitsUtil.RadixString(userId);
                            if (userKeyId > 0)
                            {
                                string redisKey = $"user:{deviceType}:token:{userKeyId}";
                                string audience = CacheManager.Instance.Get(redisKey).Result;
                                if (!string.IsNullOrEmpty(audience))
                                {
                                    return m != null && m.FirstOrDefault().Equals(audience);
                                }
                            }
                        }
                        return false;
                    },
                    //If you want to allow a certain amount of clock drift, set that here:注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟  ClockSkew = TimeSpan.FromSeconds(30)
                    ClockSkew = TimeSpan.FromSeconds(30),
                    RequireExpirationTime = true
                }, out SecurityToken validatedToken);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ValidateToken");
                return null;
            }
        }
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
