using System;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
namespace SmartCore.Services
{
    public class JwtService
    {
        //private readonly string _secret;
        private readonly string _expDate; 
        private TokenManagement tokenSetting = new TokenManagement();
        public JwtService(IConfiguration config)
        {
            //tokenSetting= config.GetSection("JwtConfig").Get<TokenManagement>();
            tokenSetting.Secret = config.GetSection("JwtConfig").GetSection("Secret").Value;
            _expDate = config.GetSection("JwtConfig").GetSection("AccessExpiration").Value;
            tokenSetting.Issuer = config.GetSection("JwtConfig").GetSection("Issuer").Value;
            tokenSetting.Audience = config.GetSection("JwtConfig").GetSection("Audience").Value;
        }

        public string GenerateSecurityToken(string email)
        {
         
            var key = Encoding.UTF8.GetBytes(tokenSetting.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer= tokenSetting.Issuer,
                Audience= tokenSetting.Audience,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim(ClaimTypes.Email, email),
                       //这个Role是官方UseAuthentication要要验证的Role，我们就不用手动设置Role这个属性了
                    //new Claim(ClaimTypes.Role,tokenModel.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_expDate)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

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
