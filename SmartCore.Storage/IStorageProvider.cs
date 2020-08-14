using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SmartCore.Storage
{
    /// <summary>
    /// 文件存储服务提供程序 interface
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// 提供程序名称
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// 保存对象到指定的容器
        /// </summary>
        /// <param name="containerName">目录名称</param>
        /// <param name="blobName">文件对象名称</param>
        /// <param name="source">流</param>
        Task SaveBlobStream(string containerName, string blobName, Stream source);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="containerName">目录</param>
        /// <param name="blobName">文件对象名称</param>
        /// <returns></returns>
        Task<Stream> GetBlobStream(string containerName, string blobName);

        /// <summary>
        /// 获取文件链接
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task<string> GetBlobUrl(string containerName, string blobName);

        /// <summary>
        /// 获取对象属性
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task<BlobFileInfo> GetBlobFileInfo(string containerName, string blobName);

        /// <summary>
        /// 列出指定容器下的对象列表
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        Task<IList<BlobFileInfo>> ListBlobs(string containerName);

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        Task DeleteBlob(string containerName, string blobName);

        /// <summary>
        /// 删除容器
        /// </summary>
        /// <param name="containerName"></param>
        Task DeleteContainer(string containerName);

        /// <summary>
        /// 获取授权访问链接
        /// </summary>
        /// <param name="containerName">容器名称</param>
        /// <param name="blobName">文件名称</param>
        /// <param name="expiry">过期时间</param>
        /// <param name="isDownload">是否允许下载</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="access">访问限制</param>
        /// <returns></returns>
        Task<string> GetBlobUrl(string containerName, string blobName, DateTime expiry, bool isDownload = false, string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read);

        ///// <summary>
        ///// 保存对象到指定的容器
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="blobName"></param>
        ///// <param name="source"></param>
        ///// <param name="properties"></param>
        //void SaveBlobStream(string containerName, string blobName, Stream source, BlobProperties properties = null);
        ///// <summary>
        ///// 保存对象到指定的容器
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="blobName"></param>
        ///// <param name="source"></param>
        ///// <param name="properties"></param>
        ///// <returns></returns>
        //Task SaveBlobStreamAsync(string containerName, string blobName, Stream source, BlobProperties properties = null);
        ///// <summary>
        ///// 获取对象
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="blobName"></param>
        ///// <returns></returns>
        //Stream GetBlobStream(string containerName, string blobName);
        ///// <summary>
        ///// 获取对象
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="blobName"></param>
        ///// <returns></returns>
        //Task<Stream> GetBlobStreamAsync(string containerName, string blobName);
        ///// <summary>
        ///// 获取Url
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="blobName"></param>
        ///// <returns></returns>
        //string GetBlobUrl(string containerName, string blobName);
        ///// <summary>
        ///// 获取SAS Url
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="blobName"></param>
        ///// <param name="expiry"></param>
        ///// <param name="isDownload"></param>
        ///// <param name="fileName"></param>
        ///// <param name="contentType"></param>
        ///// <param name="access"></param>
        ///// <returns></returns>
        //string GetBlobSasUrl(string containerName, string blobName, DateTimeOffset expiry, bool isDownload = false,
        //    string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read);
        ///// <summary>
        ///// 获取对象属性
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="blobName"></param>
        ///// <returns></returns>
        //BlobDescriptor GetBlobDescriptor(string containerName, string blobName);
        ///// <summary>
        ///// 获取对象属性
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="blobName"></param>
        ///// <returns></returns>
        //Task<BlobDescriptor> GetBlobDescriptorAsync(string containerName, string blobName);
        ///// <summary>
        ///// 列出指定容器下的对象列表
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <returns></returns>
        //IList<BlobDescriptor> ListBlobs(string containerName);
        ///// <summary>
        ///// 列出指定容器下的对象列表
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <returns></returns>
        //Task<IList<BlobDescriptor>> ListBlobsAsync(string containerName);
        ///// <summary>
        ///// 删除对象
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="blobName"></param>
        //void DeleteBlob(string containerName, string blobName);
        ///// <summary>
        ///// 删除对象
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="blobName"></param>
        ///// <returns></returns>
        //Task DeleteBlobAsync(string containerName, string blobName);
        ///// <summary>
        ///// 删除容器
        ///// </summary>
        ///// <param name="containerName"></param>
        //void DeleteContainer(string containerName);
        ///// <summary>
        ///// 删除容器
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <returns></returns>
        //Task DeleteContainerAsync(string containerName);
        ///// <summary>
        ///// 更新属性
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="blobName"></param>
        ///// <param name="properties"></param>
        //void UpdateBlobProperties(string containerName, string blobName, BlobProperties properties);
        ///// <summary>
        ///// 更新属性
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="blobName"></param>
        ///// <param name="properties"></param>
        ///// <returns></returns>
        //Task UpdateBlobPropertiesAsync(string containerName, string blobName, BlobProperties properties);
    }
}
