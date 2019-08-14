#define UTF16
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
namespace ControlServer
{

    // public delegate void OnReceivedCompleted(List<byte>mcontent);
    public delegate void OnReceivedCompleted(byte[] buffer);
    public delegate void OnFileReceivedCompleted(ref String content);
    class TCPClient
    {

        /// <summary>
        /// //////////////////////////////////////////////
        /// </summary>
        public bool isinmatchpool = false;
        public bool mclosed = false;
        Socket clientsocket;
         OnReceivedCompleted OnReceivedCompletePointer = null;
         OnFileReceivedCompleted onfilereceivedcompleted = null;
        const int BUFFER_SIZE = 65536;
        public byte[] receivebuffer = new byte[BUFFER_SIZE];

        Thread ReceiveThread;

        public TCPClient(Socket msocket)
        {
            Console.WriteLine("TCPClient " + msocket.RemoteEndPoint);
            clientsocket = msocket;
            OnReceivedCompletePointer += messagehandler;
            onfilereceivedcompleted += ReceiveFilehandler;
           // onfilereceivedcompleted += (ref String s) => { Send("hi completed!");  };
            ReceiveThread = new Thread(new ThreadStart(ReceiveLoop));
            ReceiveThread.IsBackground = true;
            ReceiveThread.Start();
            Thread.Sleep(2000);
            sendstringtest();
        }
        ~TCPClient()
        {
            mclosed = true;
            CloseSocket();
            ReceiveThread.Abort();
            Console.WriteLine("TCPClient In destructor.");
        }
        void sendstringtest()
        {
            FMessagePackage filesend = new FMessagePackage();
            filesend.MT = MessageType.ASSIGNID;//receive ok           
            string strsend = JsonConvert.SerializeObject(filesend);
            Send(strsend);
        }
        public void Send(byte[] buffer)
        {
            if (clientsocket != null)
            {
                clientsocket.Send(buffer);
            }
        }
        public void Send(String message)
        {
#if UTF16
            UnicodeEncoding asen = new UnicodeEncoding();
#else
            ASCIIEncoding asen = new ASCIIEncoding();
#endif
            //Console.WriteLine(message);
            if (clientsocket != null)
            {
                clientsocket.Send(asen.GetBytes(message));
            }
        }
        void ReceiveLoop()
        {
            while (true)
            {
                try
                {
                    Array.Clear(receivebuffer, 0, receivebuffer.Length);
                    clientsocket.Receive(receivebuffer);
                    OnReceivedCompletePointer?.Invoke(receivebuffer);
                    Thread.Sleep(30);
                }
                catch (SocketException)
                {
                    mclosed = true;
                    CloseSocket();
                   // room.Remove(this);
                    ReceiveThread.Abort();
                }
            }

        }
        public void CloseSocket()
        {
            clientsocket.Close();
        }
        void ReceiveFilehandler(ref String str)
        {


        }
        void sendfilework(Object pobject)
        {

        }
        void messagehandler(byte[] buffer)
        {
            try
            {
#if UTF16
                var str = System.Text.Encoding.Unicode.GetString(buffer);
#else
            var str = System.Text.Encoding.UTF8.GetString(buffer);
#endif

                FMessagePackage  mp = JsonConvert.DeserializeObject<FMessagePackage>(str);
                switch (mp.MT)
                {
                    case MessageType.ASSIGNOK:
                        Thread.Sleep(6000);
                         string path = @"C:\Program Files\Epic Games\UE_4.22\Engine\Build\BatchFiles\RunUAT.bat";
                         string Arguments = "BuildCookRun -project=D:\\ueprojecttest/MyProject/MyProject.uproject  -noP4 -platform=Android -clientconfig=Development -serverconfig=Development -cook -allmaps -stage -pak -archive";
                        Utility.CommandRun(path,Arguments); ;

                        break;
                    case MessageType.EMPTY:
                
                        break;
                    default:

                        break;
                }


            }
            catch (Newtonsoft.Json.JsonSerializationException)
            {//buffer all zero//occur when mobile client force kill the game client
                OnClientExit();
            }
        }
        public void OnClientExit()
        {    
            mclosed = true;
            CloseSocket();
            ReceiveThread.Abort();
        }

    }
}
