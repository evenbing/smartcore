using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Storage
{
    public class DiskStorageProvider : IStorageProvider
    {
        public string BuildFileUrl(params object[] subPath)
        {
            //var joinString = string.Join("", subPath);
            //return VirtualPathUtility.ToAbsolute(string.Concat(SiteConstants.Instance.UploadFolderPath, joinString));
            return "";
        }

        public string GetUploadFolderPath(bool createIfNotExist, params object[] subFolders)
        {
            //var sf = new List<object>();
            //sf.AddRange(subFolders);

            //var folder = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, string.Join("\\", sf)));

            //if (createIfNotExist && !Directory.Exists(folder))
            //{
            //    Directory.CreateDirectory(folder);
            //}

            //return folder;
            return "";
        }

        //public string SaveAs(string uploadFolderPath, string fileName, HttpPostedFileBase file)
        //{
            //if (!Directory.Exists(uploadFolderPath))
            //{
            //    Directory.CreateDirectory(uploadFolderPath);
            //}

            //var path = Path.Combine(uploadFolderPath, fileName);
            //file.SaveAs(path);

            //var hostingRoot = HostingEnvironment.MapPath("~/") ?? "";
            //return path.Substring(hostingRoot.Length).Replace('\\', '/').Insert(0, "/");
              
       // }
    }
}
