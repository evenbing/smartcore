using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Mail
{
    /// <summary>
    /// 系统内置发送邮件
    /// </summary>
    public class SystemNetMail
    {
        /// <summary> 
        /// 发送电子邮件 
        /// </summary> 
        /// <param name="smtpserver">SMTP服务器</param> 
        /// <param name="enableSsl">是否启用SSL加密</param> 
        /// <param name="userName">登录帐号</param> 
        /// <param name="pwd">登录密码</param> 
        /// <param name="nickName">发件人昵称</param> 
        /// <param name="fromEmail">发件人</param> 
        /// <param name="toEmail">收件人</param> 
        /// <param name="subject">主题</param> 
        /// <param name="bodys">内容</param>

        public static async Task SendMailAsync(string smtpserver, bool enableSsl, string userName, string pwd, string nickName, string fromMail, string toMail, string subject, string bodys)
        {
            SmtpClient smtpClient = new SmtpClient(); 
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式 
            smtpClient.Host = smtpserver;//指定SMTP服务器 
            smtpClient.Credentials = new NetworkCredential(userName, pwd);//用户名和密码 
            smtpClient.EnableSsl = enableSsl; 
            MailAddress fromAddress = new MailAddress(fromMail, nickName); 
            MailAddress toAddress = new MailAddress(toMail); 
            MailMessage mailMessage = new MailMessage(fromAddress, toAddress); 
            mailMessage.Subject = subject;//主题 
            mailMessage.Body = bodys;//内容 
            mailMessage.BodyEncoding = Encoding.UTF8;//正文编码 
            mailMessage.IsBodyHtml = true;//设置为HTML格式 
            mailMessage.Priority = MailPriority.Normal;//优先级 
            await smtpClient.SendMailAsync(mailMessage);

        }
    }
}
