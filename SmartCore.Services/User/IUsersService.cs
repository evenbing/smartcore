using SmartCore.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Services.User
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserService
    {
        Task<JwtAuthorizationDTO> SignIn(UserLoginDTO userLoginDTO);
    }
}
