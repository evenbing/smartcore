using SmartCore.Models.Entity;
using SmartCore.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Repository.User
{
   public interface IUserRepository : IBaseRepository<Models.Entity.UserEntity>
    {
        Task<UserEntity> CheckUser(string loginName);
    }
}
