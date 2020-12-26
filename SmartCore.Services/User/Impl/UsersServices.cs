using Microsoft.IdentityModel.Tokens;
using SmartCore.Infrastructure.Regexs;
using SmartCore.Models.DTO;
using SmartCore.Models.Entity;
using SmartCore.Repository.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Services.User.Impl
{


    public class UsersService : BaseService, IUserService
    {
        /// <summary>
        /// Jwt 服务
        /// </summary>
        private readonly IJwtServices _jwtServices;
        private readonly IUserRepository _userRepository;
        private readonly IUserPasswordRepository _userPasswordRepository;
        public UsersService(IJwtServices jwtServices, IUserRepository userRepository, IUserPasswordRepository userPasswordRepository)
        {
            _jwtServices = jwtServices;
            _userRepository = userRepository;
            _userPasswordRepository = userPasswordRepository;
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userLoginDTO"></param>
        /// <returns></returns>
        public async Task<JwtAuthorizationDTO> SignIn(UserLoginDTO userLoginDTO)
        {
            //step1：判断用户的账号的格式 如邮箱、电话、用户名、工号等
            //step2:判断账户信息是否在数据表中存在
            var userInfo = new UserAccountEntity();
            userLoginDTO.LoginType= 0;
            //if (RegexUtil.IsEmail(userLoginDTO.Username))
            //{
            //    userLoginDTO.LoginType=
            //}
            userInfo = await _userRepository.CheckUser(userLoginDTO.Username);
            if (userInfo != null && userInfo.Id > 0)
            {
                //判断用户密码是否正确
                var userPasswordInfo = await _userPasswordRepository.GetUserHashPassword(userInfo.Id);
                if (userPasswordInfo == null)
                {
                }
                byte[] decBytesHashData = Convert.FromBase64String(userPasswordInfo.PasswordHash);
                byte[] decBytesSalt = Convert.FromBase64String(userPasswordInfo.PasswordSalt);
                bool verifyResult = Infrastructure.Security.SHAUtil.VerifyHash(userLoginDTO.Password, decBytesHashData, decBytesSalt);
                if (verifyResult)
                {
                    UserTokenDTO userTokenDTO = new UserTokenDTO();
                    userTokenDTO.Id = userInfo.Id;
                    userTokenDTO.UserName = userInfo.UserName;
                    var result = await _jwtServices.GenerateSecurityToken(userTokenDTO);
                    #region 记录登录数据
                    #endregion
                    return result;
                }
            }
            return null;
        }
    }
}
