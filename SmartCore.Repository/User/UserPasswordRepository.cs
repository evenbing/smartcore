using SmartCore.Models.Entity;
using SmartCore.Repository.Base.Impl;
using System; 
using System.Threading.Tasks;

namespace SmartCore.Repository.User
{
    public class UserPasswordRepository : BaseRepository<UserPasswordEntity>, IUserPasswordRepository
    {
        /// <summary>
        /// 根据用户id获取密码数据
        /// </summary>
        /// <param name="userId">用户主键id</param>
        /// <returns></returns>
        public async Task<UserPasswordEntity> GetUserHashPassword(int userId)
        {
            //根据用户名或手机号或者邮件登录
            string sql = @"SELECT TOP 1 Id,UserId,PasswordHash,PasswordSalt,SecurityKey FROM UserPassword WHERE UserId=@UserId ";
            return await Get<UserPasswordEntity>(sql, new { UserId = userId });
        }
    }
}
