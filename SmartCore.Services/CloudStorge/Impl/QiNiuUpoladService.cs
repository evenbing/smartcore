using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SmartCore.Services.CloudStorge
{
    /// <summary>
    /// ASP.NET Core 实现七牛图片上传（FormData 和 Base64） 
    /// </summary>
    //public class QiNiuUpoladService
    //{
    //    private readonly static string[] _imageExtensions = new string[] { ".jpg", ".png", ".gif", ".jpeg", ".bmp" };
    //    private AppSettings _appSettings;

    //    public UpoladService(IOptions<AppSettings> appSettings)
    //    {
    //        _appSettings = appSettings.Value;
    //    }

    //    public async Task<SubmitResult> UploadStream(Stream stream, string fileName, AppType appType)
    //    {
    //        if (stream == null)
    //        {
    //            return SubmitResult.Fail("图片为null");
    //        }
    //        if (string.IsNullOrWhiteSpace(fileName))
    //        {
    //            return SubmitResult.Fail("图片名称为空");
    //        }
    //        try
    //        {
    //            var extensionName = fileName.Substring(fileName.LastIndexOf("."));
    //            if (!_imageExtensions.Contains(extensionName.ToLower()))
    //            {
    //                return SubmitResult.Fail("图片格式有误");
    //            }
    //            var generateFileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff-")}{Guid.NewGuid().GetHashCode().ToString().Replace("-", string.Empty)}{extensionName}";
    //            var saveKey = $"wl/{appType.ToString().ToLower()}/{generateFileName}";

    //            // 生成(上传)凭证时需要使用此Mac
    //            // 这个示例单独使用了一个Settings类，其中包含AccessKey和SecretKey
    //            // 实际应用中，请自行设置您的AccessKey和SecretKey
    //            Mac mac = new Mac(_appSettings.AccessKey, _appSettings.SecretKey);
    //            // 上传策略，参见 
    //            // https://developer.qiniu.com/kodo/manual/put-policy
    //            PutPolicy putPolicy = new PutPolicy();
    //            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
    //            // putPolicy.Scope = bucket + ":" + saveKey;
    //            putPolicy.Scope = _appSettings.Bucket;
    //            // 上传策略有效期(对应于生成的凭证的有效期)   
    //            putPolicy.SetExpires(3600);
    //            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
    //            //putPolicy.DeleteAfterDays = 1;

    //            // 生成上传凭证，参见
    //            // https://developer.qiniu.com/kodo/manual/upload-token            
    //            string jstr = putPolicy.ToJsonString();
    //            string token = Auth.CreateUploadToken(mac, jstr);

    //            FormUploader fu = new FormUploader();
    //            var result = await fu.UploadStreamAsync(stream, saveKey, token);
    //            if (result.Code == 200)
    //            {
    //                return SubmitResult.Success($"{_appSettings.Domain}/{saveKey}");
    //            }
    //            return SubmitResult.Fail("上传失败");
    //        }
    //        catch (Exception ex)
    //        {
    //            return SubmitResult.Fail($"上传失败：{ex.Message}");
    //        }
    //    }
    //}
}
