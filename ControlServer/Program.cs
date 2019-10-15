#define OSSDOWNLOAD 
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
        public string wid;
    }
    class Program
    {
        public static string currentwid = "";
        public static Queue<rarmessage> rarqueue = new Queue<rarmessage>();
        //public static ManualResetEvent evtObj = new ManualResetEvent(false);
        public static AutoResetEvent evtObj = new AutoResetEvent(false);
        static int Main(string[] args)
        {
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
            //string[] prefixes = { "http://localhost:7000/", "http://192.168.1.240:7000/" };
            string[] prefixes = {"http://192.168.1.240:7000/" };//host http serer
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
                    currentwid = newrar.wid;
#if OSSDOWNLOAD
                    OssClient client = new OssClient(Config.Endpoint, Config.AccessKeyId, Config.AccessKeySecret);
                    const string bucketName = "coresnow-circle";
                    var result = client.GetObject(bucketName, newrar.rarpath);
                    using (var requestStream = result.Content)
                    {
                        string path = @"F:\uev";//\Content;
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
                        string passArguments = "x F:/uev/Saved/x.rar -oF:/uev/Content";
                        Process z7p = Utility.CommandRun(apppath, passArguments);
                        z7p.WaitForExit();

                        IPAddress ipAd = IPAddress.Parse("192.168.1.240");
                        TcpListener myList = new TcpListener(ipAd, 8003);
                        myList.Start();
                        string projectpath = @"F:\uev/pro422.uproject";
                        string Arguments = "";
                        projectpath = @"C:\Program Files\Epic Games\UE_4.22\Engine\Binaries\Win64/UE4Editor.exe";
                        Arguments = @"F:\uev/pro422.uproject";
                        Process mpro = Utility.CommandRun(projectpath, Arguments);
                        Socket st = myList.AcceptSocket();
                        TCPClient tcpClient = new TCPClient(st);
                        tcpClient.mtcplistener = myList;
                        tcpClient.mprocess = mpro;
                    }
#else
                    HttpclientHelper.httpget(newrar.rarpath, (ref string str, ref byte[] bytearray) => {
                        string path = AppDomain.CurrentDomain.BaseDirectory;
                        path += "x.rar";
                        path = @"F:\uev";//\Content;
                        Utility.SubDirectoryDelete(path + "/Content");
                        //Thread.Sleep(5000);
                        File.WriteAllBytes(path + "/Saved/x.rar", bytearray);
                        Console.WriteLine("writefileok :"+ bytearray.Length);
                        //Thread.Sleep(5000);
                        string apppath = @"E:\Program Files\7-Zip\7zG.exe";
                        string passArguments = "x F:/uev/Saved/x.rar -oF:/uev/Content";
                        Process z7p = Utility.CommandRun(apppath, passArguments);
                        z7p.WaitForExit();

                        IPAddress ipAd = IPAddress.Parse("192.168.1.240");
                        TcpListener myList = new TcpListener(ipAd, 8003);
                        myList.Start();
                        string projectpath = @"F:\uev/pro422.uproject";
                        string Arguments = "";
                        projectpath = @"C:\Program Files\Epic Games\UE_4.22\Engine\Binaries\Win64/UE4Editor.exe";
                        Arguments = @"F:\uev/pro422.uproject";
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
