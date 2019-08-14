using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
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

            IPAddress ipAd = IPAddress.Parse("192.168.1.240");
            TcpListener myList = new TcpListener(ipAd, 8003);
            myList.Start();

            string projectpath = @"F:\uev/pro422.uproject";
            string Arguments = "";
            projectpath = @"C:\Program Files\Epic Games\UE_4.22\Engine\Binaries\Win64/UE4Editor.exe";
            Arguments = @"F:\uev/pro422.uproject";
            Process mpro = Utility.CommandRun(projectpath, Arguments);
  

            /* Start Listeneting at the specified port */
            while (true)
            {
                Socket st = myList.AcceptSocket();
                TCPClient tcpClient = new TCPClient(st);
                tcpClient.mprocess = mpro;


            }
        }
    }
}
