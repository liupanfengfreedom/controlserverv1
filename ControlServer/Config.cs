using System;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Samples
{
    internal class Config
    {
        public static string AccessKeyId = "LTAIhCY2qLcYpbN2";

        public static string AccessKeySecret = "bFhFxbE2ANMb8RXjiV6KyBmpC3AlTv";

        public static string Endpoint = "oss-cn-hangzhou.aliyuncs.com";

        public static string DirToDownload = "<your local dir to download file>";

        public static string FileToUpload = @"F:\netbox/index.html";

        public static string BigFileToUpload = "<your local big file to upload>";
        public static string ImageFileToUpload = "<your local image file to upload>";
        public static string CallbackServer = "<your callback server uri>";
    }
}
