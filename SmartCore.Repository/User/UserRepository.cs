using SmartCore.Models.Entity;
using SmartCore.Repository.Base.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Repository.User
{
   public class UserRepository:  BaseRepository<UserEntity>, IUserRepository
    {
        public async Task<UserEntity> CheckUser(string loginName)
        {
            return await Task.FromResult(new UserEntity());
        }
    }
}
