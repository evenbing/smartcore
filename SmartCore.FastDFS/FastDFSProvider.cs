
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FastDFS.Client;
namespace SmartCore.FastDFS
{
    public class FastDFSConfig
    {
        public string TackerIp { get; set; }

        public string TackerPort { get; set; }

        public string StorageGroup { get; set; }

        public string StorageServerLink { get; set; }
    }
    public class FastDFSProvider : IStorageProvider, ITransientDependency
    {
        /// <summary>
        /// 提供程序名称
        /// </summary>
        public string ProviderName => "FastDFS";

        //配置
        private readonly FastDFSConfig _fastDFSConfig;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fastDFSConfig">注入配置数据</param>
        public FastDFSProvider(FastDFSConfig fastDFSConfig)
        {
            _fastDFSConfig = fastDFSConfig ?? throw new ArgumentNullException(nameof(fastDFSConfig));
            string[] trackers = fastDFSConfig.Trackers.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var trackerIPs = new List<IPEndPoint>();
            foreach (var onetracker in trackers)
            {
                trackerIPs.Add(new IPEndPoint(IPAddress.Parse(onetracker), fastDFSConfig.Port));
            }
            ConnectionManager.Initialize(trackerIPs);
        }

        /// <summary>
        /// 通过文件名删除对应的文件
        /// </summary>
        /// <param name="objectName">对象名称</param>
        /// <param name="groupName">分组名称</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteObjectByNameAsync(string objectName, string groupName = "")
        {
            RestoreFileName(objectName, out string fileName, out groupName);
            await FastDFSClient.RemoveFileAsync(groupName, fileName);
            return true;
        }

        /// <summary>
        /// 获取网关连接
        /// </summary>
        /// <returns>网关连接</returns>
        public Task<string> GetGateWayUrl() => Task.FromResult(_fastDFSConfig.GateWayUrl);

        /// <summary>
        /// 获取对象二进制数组
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="objectName">对象名称</param>
        /// <returns>对象的二进制数组</returns>
        public async Task<byte[]> GetObjectByteAsync(string objectName, string groupName = "")
        {
            RestoreFileName(objectName, out string fileName, out groupName);
            var storageNode = await FastDFSClient.GetStorageNodeAsync(groupName);
            return await FastDFSClient.DownloadFileAsync(storageNode, fileName);
        }

        /// <summary>
        /// 获取对象流
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="objectName">对象名称</param>
        /// <returns>对象流</returns>
        public async Task<Stream> GetObjectStreamAsync(string objectName, string groupName = "")
        {
            RestoreFileName(objectName, out string fileName, out groupName);
            var storageNode = await FastDFSClient.GetStorageNodeAsync(groupName);
            FDFSFileInfo fileInfo = await FastDFSClient.GetFileInfoAsync(storageNode, fileName);
            Stream stream = new MemoryStream();
            int limit = 1024 * 100;//每次获取100KB的数据

            //如果文件大小大于100KB  分次写入
            if (fileInfo.FileSize >= limit)
            {
                long offset = 0;
                long len = limit;
                while (len > 0)
                {
                    byte[] buffer = await FastDFSClient.DownloadFileAsync(storageNode, fileName, offset, len);
                    stream.Write(buffer, 0, int.Parse(len.ToString()));
                    stream.Flush();
                    offset += len;
                    len = (fileInfo.FileSize - offset) >= limit ? limit : (fileInfo.FileSize - offset);
                }

            }
            else
            {
                //如果文件大小小小于100KB  直接写入文件
                byte[] buffer = await FastDFSClient.DownloadFileAsync(storageNode, fileName);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// 存储文件
        /// </summary>
        /// <param name="objectByte">对象</param>
        /// <param name="objectName">对象名</param>
        /// <param name="groupName">分组</param>
        /// <returns></returns>
        public async Task<string> StoreObjectByteAsync(byte[] objectByte, string objectName, string groupName = "")
        {
            var storageNode = await FastDFSClient.GetStorageNodeAsync("");
            var filePath = await FastDFSClient.UploadFileAsync(storageNode, objectByte, Path.GetExtension(objectName));
            return storageNode.GroupName + "/" + filePath;
        }

        /// <summary>
        /// 存储文件
        /// </summary>
        /// <param name="objectStream">文件流</param>
        /// <param name="objectName">对象名</param>
        /// <param name="groupName">分组</param>
        /// <returns>文件路径</returns>
        public async Task<string> StoreObjectStreamAsync(Stream objectStream, string objectName, string groupName = "")
        {
            var storageNode = await FastDFSClient.GetStorageNodeAsync("");
            var filePath = await FastDFSClient.UploadFileAsync(storageNode, objectStream, Path.GetExtension(objectName));
            return storageNode.GroupName + "/" + filePath;
        }
    }
}
