using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Storage
{
    /// <summary>
    ///     容器描述
    /// </summary>
    public class BlobDescriptor
    {
        /// <summary>
        ///     内容类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        ///     内容MD5
        /// </summary>
        public string ContentMD5 { get; set; }

        public string ETag { get; set; }

        /// <summary>
        ///     大小
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        ///     最后修改时间
        /// </summary>
        public DateTimeOffset? LastModified { get; set; }

        /// <summary>
        ///     安全设置类型
        /// </summary>
        public BlobSecurity Security { get; set; }

        /// <summary>
        ///     名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     容器
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        ///     路径
        /// </summary>
        public string Url { get; set; }
    }
}
