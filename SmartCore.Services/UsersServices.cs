using Microsoft.IdentityModel.Tokens;
using SmartCore.Models.DTO;
using SmartCore.Repository.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Services
{
    public class UserLoginDTO
    {
        
        public string Username { get; set; }
        //[JsonIngore]
        public string Password { get; set; } 
    }
    public class UserDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Salt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Birthday { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrgCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }
    }
    
    public interface IUserService
    {
        Task<JwtAuthorizationDTO> SignIn(UserLoginDTO userLoginDTO);
        //IEnumerable<UserVO> GetAll();
    }
    public class UsersServices:BaseServices, IUserService
    {
        /// <summary>
        /// Jwt 服务
        /// </summary>
        private readonly IJwtServices _jwtServices;
        private readonly IUserRepository _userRepository;
        public UsersServices(IJwtServices jwtServices, IUserRepository userRepository)
        {
            _jwtServices = jwtServices;
            _userRepository = userRepository;
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userLoginDTO"></param>
        /// <returns></returns>
        public async Task<JwtAuthorizationDTO> SignIn(UserLoginDTO userLoginDTO)
        {
            //判断用户的账号是否在数据表中存在
            var userInfo =await _userRepository.CheckUser(userLoginDTO.Username);
            if (userInfo!=null&& userInfo.Id>0)
            {
                UserTokenDTO userTokenDTO = new UserTokenDTO();
                userTokenDTO.Id = userInfo.Id;
                userTokenDTO.UserName = userInfo.UserName; 
                var result = await _jwtServices.GenerateSecurityToken(userTokenDTO);
                return result;
            }
            return null;
        } 
    }
}
