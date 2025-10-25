using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace PlaySound.Common
{
    /// <summary>
    /// 通用文件资源管理器，统一管理嵌入的文件资源
    /// </summary>
    public static class ResourceManager
    {
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private static readonly EmbeddedFileProvider _embeddedProvider = new(_assembly);

        public static Stream GetResourceStream(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("资源路径或名称不能为空", nameof(path));

            var fileInfo = _embeddedProvider.GetFileInfo(path);

            // 明确检查文件是否存在
            if (!fileInfo.Exists)
            {
                // 抛出特定的 FileNotFoundException
                throw new FileNotFoundException($"嵌入的资源文件不存在或路径错误: {path}");
            }

            // CreateReadStream 可能会抛出 IOException/UnauthorizedAccessException 等，
            // 这里让这些异常自然抛出，让调用者去处理 I/O 错误。
            return fileInfo.CreateReadStream();
        }

        /// <summary>
        /// 检查指定的资源是否存在
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <returns>资源是否存在</returns>
        public static bool ResourceExists(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                return false;

            return _embeddedProvider.GetFileInfo(resourceName).Exists;
        }
    }
}