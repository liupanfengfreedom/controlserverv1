using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
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
        static void Main(string[] args)
        {
            string sourcepath = @"F:\uev\Content";
            string despath = @"F:\UE4 projects\bplab\Content";
            Utility.DirectoryCopy(sourcepath, despath, true);
            Utility.SubDirectoryDelete(despath);


            IPAddress ipAd = IPAddress.Parse("192.168.1.240");
            TcpListener myList = new TcpListener(ipAd, 8003);

            /* Start Listeneting at the specified port */
            myList.Start();
            while (true)
            {
                Socket st = myList.AcceptSocket();
                TCPClient tcpClient = new TCPClient(st);
    
            }
        }
    }
}
