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
    class Program
    {
        static int Main(string[] args)
        {
            Thread HttpServerThread;
            //string path = AppDomain.CurrentDomain.BaseDirectory;
            //string filestring = File.ReadAllText(@"D:\SVNprository\program\Project\InfiniteLife1_1\Content\Movies/makeup.ini");


            //string apppath = @"E:\Program Files\7-Zip\7zG.exe";
            //string passArguments = "x G:/UE4projects/MyProject4/Content/TwinStick.rar -oG:/UE4projects/MyProject4/Content/12";
            //Utility.CommandRun(apppath, passArguments);
            HttpServerThread = new Thread(new ThreadStart(httpserverthread));
            HttpServerThread.IsBackground = true;
            HttpServerThread.Start();

            while (true)
            {
                bool b =  HttpServerThread.IsAlive;
                if (!b)
                {
                    HttpServerThread = new Thread(new ThreadStart(httpserverthread));
                    HttpServerThread.IsBackground = true;
                    HttpServerThread.Start();
                }
                Thread.Sleep(1000);
            }
            //IPAddress ipAd = IPAddress.Parse("192.168.1.240");
            //TcpListener myList = new TcpListener(ipAd, 8003);
            //myList.Start();

            //string projectpath = @"F:\uev/pro422.uproject";
            //string Arguments = "";
            //projectpath = @"C:\Program Files\Epic Games\UE_4.22\Engine\Binaries\Win64/UE4Editor.exe";
            //Arguments = @"F:\uev/pro422.uproject";
            //Process mpro = Utility.CommandRun(projectpath, Arguments);
          
            //while (true)
            //{
            //    Socket st = myList.AcceptSocket();
            //    TCPClient tcpClient = new TCPClient(st);
            //    tcpClient.mprocess = mpro;


            //}
            return 9;
        }
        static void httpserverthread()
        {
            string[] prefixes = { "http://localhost:7000/", "http://192.168.1.240:7000/" };
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
       
    }
}
