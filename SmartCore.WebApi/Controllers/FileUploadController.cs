using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartCore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {

        public FileUploadController()
        {
            //_hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// Form表单之单文件上传
        /// </summary>
        /// <param name="formFile">form表单文件流信息</param>
        /// <returns></returns>
        public IActionResult FormSingleFileUpload(IFormFile formFile)
        {
            var currentDate = DateTime.Now;
            // var webRootPath = _hostingEnvironment.WebRootPath;//>>>相当于HttpContext.Current.Server.MapPath("")  
            //var filePath = $"/UploadFile/{currentDate:yyyyMMdd}/";

            ////创建每日存储文件夹
            //if (!Directory.Exists(webRootPath + filePath))
            //{
            //    Directory.CreateDirectory(webRootPath + filePath);
            //}

            //if (formFile != null)
            //{
            //    //文件后缀
            //    var fileExtension = Path.GetExtension(formFile.FileName);//获取文件格式，拓展名

            //    //判断文件大小
            //    var fileSize = formFile.Length;

            //    if (fileSize > 1024 * 1024 * 10) //10M TODO:(1mb=1024X1024b)
            //    {
            //        return new JsonResult(new { isSuccess = false, resultMsg = "上传的文件不能大于10M" });
            //    }

            //    //保存的文件名称(以名称和保存时间命名)
            //    var saveName = formFile.FileName.Substring(0, formFile.FileName.LastIndexOf('.')) + "_" + currentDate.ToString("HHmmss") + fileExtension;

            //    //文件保存
            //    using (var fs = System.IO.File.Create(webRootPath + filePath + saveName))
            //    {
            //        formFile.CopyTo(fs);
            //        fs.Flush();
            //    }

            //    //完整的文件路径
            //    var completeFilePath = Path.Combine(filePath, saveName);

            //    return new JsonResult(new { isSuccess = true, returnMsg = "上传成功", completeFilePath = completeFilePath });
            //}
            //else
            //{
            //    return new JsonResult(new { isSuccess = false, resultMsg = "上传失败，未检测上传的文件信息~" });
            //}

            return Ok();

        }

        public IActionResult MultiFileUpload(IFormCollection formCollection)
        {
            var files = (FormFileCollection)formCollection.Files;
            return Ok();
        }
    }
}