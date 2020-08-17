using SmartCore.Models.DTO;
using SmartCore.Models.Entity;
using SmartCore.Repository.Base.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Repository.User
{
   public class UserRepository:  BaseRepository<UserAccountEntity>, IUserRepository
    {
        /// <summary>
        /// 根据用户名或手机号或邮箱地址 获取用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<UserAccountEntity> CheckUser(string userName)
        {
            //根据用户名或手机号或者邮件登录
            string sql = @"SELECT TOP 1 [Id],[UserName],[UserEmail]
      ,[NickName],[RealName],[Letter],[EmpId],[Phone],[Avatar]
      ,[Birthday],[Sex],[OrgCode],[Status],[IsEnabled],[ThirdId] FROM UserAccountEntity WHERE UserName=@UserName OR UserEmail=@UserName OR  Phone=@UserName";
            return await Get<UserAccountEntity>(sql, new { UserName = userName }); 
        }
    }
}
