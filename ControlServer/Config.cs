using System;
using Aliyun.OSS.Common;
using System.IO;
using Newtonsoft.Json;
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
           public struct Configinfor
        {
            public String signidpropath;
            public String unrealprojectpath;
            public String projectshouldlaunched;
            public String packagpropath_android;
            public String packagpropath_ios;
            public String unrealbatchfilepath;
            public String argumentsforandroid;
            public String argumentsforios;
            public String fbxlocation;
            public String fileserverlocation;
            public String fileserverpath;
            public String tcplocal;
        }
        public Configinfor configinfor;
        public Config()
        {
            try {

                String ss = File.ReadAllText("./config.ini");
                configinfor =  JsonConvert.DeserializeObject<Configinfor>(ss);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
