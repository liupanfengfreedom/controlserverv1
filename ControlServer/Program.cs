//#define OSSDOWNLOAD 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Net.Sockets;
using Aliyun.OSS.Samples;
using Aliyun.OSS;
using Newtonsoft.Json;
using System.Net.Http;
namespace ControlServer
{
    enum MessageType
    {
        EMPTY,
        ASSIGNID,
        ASSIGNOK,
    }
    struct FMessagePackage
    {
        public MessageType MT;

        public string PayLoad;
        public FMessagePackage(string s)
        {
            MT = MessageType.EMPTY;
            PayLoad = "";
        }
    }
    struct rarmessage
    {
        public string rarpath;
        public string mid;
    }
    class Program
    {
        public static Config config = new Config();
        public static string signidpropath = @"C:\UEprojects\UEeditor";      
        public static string unrealprojectpath = @"C:\Program Files\Epic Games\UE_4.23\Engine\Binaries\Win64/UE4Editor.exe";
        public static string projectshouldlaunched = @"C:\UEprojects\UEeditor/UEeditor.uproject";
        public static string packagpropath_android = @"C:\package_android/InfiniteLife1_0";
        public static string packagpropath_ios = @"C:\package_ios/InfiniteLife1_0";
        public static string unrealbatchfilepath = @"C:\Program Files\Epic Games\UE_4.23\Engine\Build\BatchFiles\RunUAT.bat";
        public static string argumentsforandroid = "BuildCookRun -project=C:\\package_android/InfiniteLife1_0/InfiniteLife1_0.uproject  -noP4 -platform=Android -clientconfig=Development -serverconfig=Development -cook -allmaps -Compressed -build -stage -pak -archive";
        public static string argumentsforios = "BuildCookRun -project=C:\\package_ios/InfiniteLife1_0/InfiniteLife1_0.uproject  -noP4 -platform=IOS -clientconfig=Development -serverconfig=Development -cook -allmaps -Compressed -build -stage -pak -archive";

/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string fbxlocation = @"C:\fbxlocation";
        public static string fileserverlocation = @"D:\xujie";
        public static string fileserverpath = "http://192.168.20.96:8000/";
        public static string username = "user1";
        public static string passworld = "password";
        public static string useranimationtablename = "animationtable_"+ username;
        public static string animationundercontent = signidpropath+"\\Content\\skeletonmesh\\animation";


        public static string zip7app = @"C:\Program Files\7-Zip\7zG.exe";
        public static string zip7appargument = "x C:/uev/Saved/x.rar -oC:/uev/Content";
        public static string tcplocal = "192.168.20.96";
        public static string httpserver = "http://+:7000/";//"http://192.168.20.96:7000/";

        public static string remotehttpserver = "http://172.16.11.32:8080/api4site/pakCallback";//remote http server;

        public static string currentwid = "";
        public static Queue<rarmessage> rarqueue = new Queue<rarmessage>();
        //public static ManualResetEvent evtObj = new ManualResetEvent(false);
        public static AutoResetEvent evtObj = new AutoResetEvent(false);
        static void databaseinitial()
        {
            MySQLOperation msqlo = MySQLOperation.getinstance();
            String cmd;
            Dictionary<String, String> value;
            cmd = String.Format(
              "SELECT UserName FROM {0} WHERE UserName='{1}'", MySQLOperation.tablebasename, "user1"
               );
            bool b = msqlo.find(cmd);
            if (!b)
            {
                //cmd = "INSERT INTO students(name,class) VALUES(@name,@class)";
                cmd = String.Format(
                   "INSERT INTO {0}(UserName,PassWord) VALUES(@UserName,@PassWord)", MySQLOperation.tablebasename
                    );
                value = new Dictionary<string, string>();
                value.Add("@UserName", "user1");
                value.Add("@PassWord", "password");
                msqlo.add(cmd, value);
            }
            string animationtablename = "animationtable_user1";
            cmd = String.Format(
                  " UPDATE {0} SET {1} = '{2}' WHERE UserName='{3}'", MySQLOperation.tablebasename, "animationtable", Program.useranimationtablename, "user1"
                   );
            msqlo.modify(cmd);

            bool b2 = msqlo.tableeexist(MySQLOperation.databasename, animationtablename);
            if (!b2)
            {
                msqlo.creatanimationtable(MySQLOperation.databasename, animationtablename);
            }

        }
        static int Main(string[] args)
        {
            signidpropath = config.configinfor.signidpropath;
            unrealprojectpath = config.configinfor.unrealprojectpath;
            projectshouldlaunched = config.configinfor.projectshouldlaunched;
            packagpropath_android = config.configinfor.packagpropath_android;
            packagpropath_ios = config.configinfor.packagpropath_ios;
            unrealbatchfilepath = config.configinfor.unrealbatchfilepath;
            argumentsforandroid = config.configinfor.argumentsforandroid;
            argumentsforios = config.configinfor.argumentsforios;
            fbxlocation = config.configinfor.fbxlocation;
            fileserverlocation = config.configinfor.fileserverlocation;
            fileserverpath = config.configinfor.fileserverpath;
            tcplocal = config.configinfor.tcplocal;
            ////////////////////////////////////////////////////////////////////
            databaseinitial();
            rarmessage mmessage;
            mmessage.rarpath = "http://192.168.20.96:8000/tmp.fbx";
            mmessage.mid = "";
            window_file_log.Log(" mmessage.rarpath :" + mmessage.rarpath);
            window_file_log.Log(" mmessage.mid :" + mmessage.mid);
            //Program.rarqueue.Enqueue(mmessage);

            //IPAddress ipAd = IPAddress.Parse("192.168.1.240");
            //ipAd = IPAddress.Parse(tcplocal);
            //TcpListener myList = new TcpListener(ipAd, 8003);
            //myList.Start();
            //Socket st = myList.AcceptSocket();
            //TCPClient tcpClient = new TCPClient(st);


            Thread HttpServerThread;
            Thread ConvertThread;
            //string path = AppDomain.CurrentDomain.BaseDirectory;
            //string filestring = File.ReadAllText(@"D:\SVNprository\program\Project\InfiniteLife1_1\Content\Movies/makeup.ini");


            //string apppath = @"E:\Program Files\7-Zip\7zG.exe";
            //string passArguments = "x G:/UE4projects/MyProject4/Content/TwinStick.rar -oG:/UE4projects/MyProject4/Content/12";
            //Utility.CommandRun(apppath, passArguments);

            HttpServerThread = new Thread(new ThreadStart(httpserverthread));
            HttpServerThread.IsBackground = true;
            HttpServerThread.Start();

            ConvertThread = new Thread(new ThreadStart(convertthreadwork));
            ConvertThread.IsBackground = true;
            ConvertThread.Start();

            while (true)
            {
                bool b = HttpServerThread.IsAlive;
                if (!b)
                {
                    HttpServerThread = new Thread(new ThreadStart(httpserverthread));
                    HttpServerThread.IsBackground = true;
                    HttpServerThread.Start();
                }
                Thread.Sleep(1000);
            }
            //  IPAddress ipAd = IPAddress.Parse("192.168.1.240");
            //  TcpListener myList = new TcpListener(ipAd, 8003);
            //  myList.Start();

            //  string projectpath = @"F:\uev/pro422.uproject";
            //  string Arguments = "";
            //  projectpath = @"C:\Program Files\Epic Games\UE_4.22\Engine\Binaries\Win64/UE4Editor.exe";
            //  Arguments = @"F:\uev/pro422.uproject";
            ////  Process mpro = Utility.CommandRun(projectpath, Arguments);

            //  while (true)
            //  {
            //      Socket st = myList.AcceptSocket();
            //      TCPClient tcpClient = new TCPClient(st);
            //    //  tcpClient.mprocess = mpro;
            //  }
            //  return 9;
        }
        static void httpserverthread()
        {
            //string[] prefixes = { "http://172.16.5.186:7000/" };           

            string[] prefixes = { httpserver };//host http serer
            //string[] prefixes = { "http://localhost:7000/" };//host http serer
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8000/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes. 
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();
            Console.WriteLine("Listening...");
            while (true)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    new HttpListenerContextClass(context);
                    //throw (new Exception());
                }
                catch
                {
                    listener.Stop();                  
                    Thread.Sleep(1000);
                    break;
                }

            }
        }
        static void convertthreadwork()
        {
            while (true) {
                Thread.Sleep(1000);               
                if (rarqueue.Count > 0)
                {
                    rarmessage newrar = rarqueue.Dequeue();
                    currentwid = newrar.mid;
#if OSSDOWNLOAD
                    OssClient client = new OssClient(Config.Endpoint, Config.AccessKeyId, Config.AccessKeySecret);
                    const string bucketName = "coresnow-circle";
                    try
                    {
                        var result = client.GetObject(bucketName, newrar.rarpath);

                        using (var requestStream = result.Content)
                        {
                            string path = @"F:\uev";//\Content;
                            path = signidpropath;
                            Utility.SubDirectoryDelete(path + "/Content");
                            using (var fs = File.Open(path + "/Saved/x.rar", FileMode.Create))
                            {
                                int length = 4 * 1024;
                                var buf = new byte[length];
                                do
                                {
                                    length = requestStream.Read(buf, 0, length);
                                    fs.Write(buf, 0, length);
                                } while (length != 0);
                            }
                            Console.WriteLine("writefileok :" + result.ContentLength);
                            string apppath = @"E:\Program Files\7-Zip\7zG.exe";
                            apppath = zip7app;
                            string passArguments = "x F:/uev/Saved/x.rar -oF:/uev/Content";
                            passArguments = zip7appargument;
                            Process z7p = Utility.CommandRun(apppath, passArguments);
                            z7p.WaitForExit();

                            IPAddress ipAd = IPAddress.Parse("192.168.1.240");
                            ipAd = IPAddress.Parse(tcplocal);
                            TcpListener myList = new TcpListener(ipAd, 8003);
                            myList.Start();
                            string projectpath = @"F:\uev/pro422.uproject";         
                            string Arguments = "";
                            projectpath = unrealprojectpath;
                            Arguments = projectshouldlaunched;
                            Process mpro = Utility.CommandRun(projectpath, Arguments);
                            Socket st = myList.AcceptSocket();
                            TCPClient tcpClient = new TCPClient(st);
                            tcpClient.mtcplistener = myList;
                            tcpClient.mprocess = mpro;
                        }
                    }
                    catch(Exception e)
                    {
                        window_file_log.Log(" mmessage.rarpath :" + e);

                        var payload = new Dictionary<string, string>
                        {
                          {"result","failure"},
                          {"reason","访问的RAR文件不存在"},
                          {"mid",Program.currentwid},
                        };
                        string strPayload = JsonConvert.SerializeObject(payload);
                        HttpContent httpContent = new StringContent(strPayload, Encoding.UTF8, "application/json");
                        int retrycounter = 0;
retry:
                        window_file_log.Log("retrycounter :" + retrycounter);
                        window_file_log.Log(strPayload);
                        int shouldretry = 0;
                        HttpclientHelper.httppost(Program.remotehttpserver, httpContent, (ref string strparameter, ref byte[] bytearray) => {
                            window_file_log.Log(strparameter);
                            if (!String.IsNullOrEmpty(strparameter))
                            {
                                if (!strparameter.Contains("\"data\":true"))
                                {
                                    shouldretry = 1;
                                    retrycounter++;
                                }
                            }
                        });
                        if (shouldretry == 1 && retrycounter < 10)
                        {
                            Thread.Sleep(1000 * 60 * 10);
                            goto retry;
                        }
                    }
#else //httpdownload
                    HttpclientHelper.httpget(newrar.rarpath, (ref string str, ref byte[] bytearray) => {
                        string path = AppDomain.CurrentDomain.BaseDirectory;
                        path += "x.rar";
                        path = @"F:\uev";//\Content;
                        path = fbxlocation;
                        Utility.SubDirectoryDelete(path);
                        //Thread.Sleep(5000);
                        string filetowrite = path + "/x.fbx";
                        File.WriteAllBytes(filetowrite, bytearray);
                        Console.WriteLine("writefileok :"+ bytearray.Length);
                        //Thread.Sleep(5000);
                        //string apppath = @"E:\Program Files\7-Zip\7zG.exe";
                        //string passArguments = "x F:/uev/Saved/x.rar -oF:/uev/Content";
                        //Process z7p = Utility.CommandRun(apppath, passArguments);
                        //z7p.WaitForExit();

                        IPAddress ipAd = IPAddress.Parse("192.168.1.240");
                        ipAd = IPAddress.Parse(tcplocal);
                        TcpListener myList = new TcpListener(ipAd, 8003);
                        myList.Start();
                        Utility.SubDirectoryDelete(animationundercontent);
                        string projectpath = @"F:\uev/pro422.uproject";
                        string Arguments = "";
                        projectpath = @"C:\Program Files\Epic Games\UE_4.22\Engine\Binaries\Win64/UE4Editor.exe";
                        Arguments = @"F:\uev/pro422.uproject";
                        projectpath = unrealprojectpath;
                        Arguments = projectshouldlaunched;
                        Process mpro = Utility.CommandRun(projectpath, Arguments);
                        Socket st = myList.AcceptSocket();
                        TCPClient tcpClient = new TCPClient(st);
                        tcpClient.mtcplistener = myList;
                        tcpClient.mprocess = mpro;
                    });
#endif
                    evtObj.WaitOne();
                }
            }
            
        }
    }
}
