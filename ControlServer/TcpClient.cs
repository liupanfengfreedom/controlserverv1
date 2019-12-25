#define UTF16
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using Aliyun.OSS.Samples;
using Aliyun.OSS;
namespace ControlServer
{

    // public delegate void OnReceivedCompleted(List<byte>mcontent);
    public delegate void OnReceivedCompleted(byte[] buffer);
    public delegate void OnFileReceivedCompleted(ref String content);
    class TCPClient
    {
        //string remotehttpserver = "http://192.168.1.174:8080/api/pakCallback";//remote http server;
        /// <summary>
        /// //////////////////////////////////////////////
        /// </summary>
        public bool isinmatchpool = false;
        public bool mclosed = false;
        public TcpListener mtcplistener;
        Socket clientsocket;
         OnReceivedCompleted OnReceivedCompletePointer = null;
         OnFileReceivedCompleted onfilereceivedcompleted = null;
        const int BUFFER_SIZE = 65536;
        public byte[] receivebuffer = new byte[BUFFER_SIZE];

        Thread ReceiveThread;
        public Process mprocess;
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
            filesend.MT = MessageType.ASSIGNID;//     
            //SkeletalMesh'/Game/Vehicles/VH_Buggy/Mesh/SK_Buggy_Vehicle.SK_Buggy_Vehicle'
            string path = "/Game/Vehicles/VH_Buggy/Mesh/SK_Buggy_Vehicle";
            string id = "1";
            filesend.PayLoad = "?"+path+"?"+id;
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
                        string androidkey = "";
                        string ioskey = "";
                        if (mp.PayLoad != "")
                        {                        
                            Thread.Sleep(6000);
                            if (mprocess!=null&&!mprocess.HasExited)
                            {
                                mprocess.Kill();
                            }
                            //content copy begin
                            string sourcepath = @"F:\uev\Content";
                            sourcepath = Program.signidpropath + "/Content";
                            string despath = Program.packagpropath + "/Content";

                            Utility.SubDirectoryDelete(despath);
                            Utility.DirectoryCopy(sourcepath, despath, true);
                            //content copy end
                            //delete old data begin
                            string pakpath = Program.packagpropath + "/Saved";
                            Utility.SubDirectoryDelete(pakpath);
                            //delete old data end


                            //////////////////////////////////////////////////////////////////
                            ////////////////////////cook for Android begin
                            string path = Program.unrealbatchfilepath;
                            string Arguments = Program.argumentsforandroid;
                            Process tempprocess = Utility.CommandRun(path, Arguments);
                            tempprocess.WaitForExit();
                            /////////////////////////////////////////////////////////////////
                            /////////////////////////cook for Android end
                            //////////////////////////////////////////////////////////////////
                            ////////////////////////cook for ios begin
                            Arguments = Program.argumentsforios;
                            tempprocess = Utility.CommandRun(path, Arguments);
                            tempprocess.WaitForExit();
                            /////////////////////////////////////////////////////////////////
                            /////////////////////////cook for ios end

                            //Console.WriteLine("hi");
                            string androidpaksfilepath = Program.packagpropath+ "/Saved/StagedBuilds/Android/InfiniteLife1_0/Content/Paks/pakchunk1-Android.pak";
                            string iospaksfilepath = Program.packagpropath + "/Saved/StagedBuilds/IOS/cookeddata/infinitelife1_0/content/paks/pakchunk1-ios.pak";
                            Guid g;
                            // Create and display the value of two GUIDs.
                            g = Guid.NewGuid();
                            string guidstring = g.ToString();
                            string androidmd5 = guidstring+".pak";
                            g = Guid.NewGuid();
                            guidstring = g.ToString();
                            string iosmd5 = guidstring + ".pak";
                            OssClient client = new OssClient(Config.Endpoint, Config.AccessKeyId, Config.AccessKeySecret);
                            const string bucketName = "coresnow-circle";
                            string key = "yourfolder/"+ androidmd5;
                            androidkey = "works/"+ androidmd5;
                            client.PutObject(bucketName, androidkey, androidpaksfilepath);
                            ioskey = "works/" + iosmd5;
                            client.PutObject(bucketName, ioskey, iospaksfilepath);
                        }
                        Program.evtObj.Set();//next one
                        string result = "success";
                        string reason = "";
                        if (mp.PayLoad == "")
                        {
                            result = "failure";
                            reason = "资产无效";
                        }
                        var payload = new Dictionary<string, string>
                        {
                          {"result",result},
                          {"reason",reason},
                          {"wid",Program.currentwid},
                          {"assetpath", mp.PayLoad},//if this value is null then this asset is invalid
                          {"android_pak", androidkey},
                          {"ios_pak", ioskey}
                        };
                        string strPayload = JsonConvert.SerializeObject(payload);
                        HttpContent httpContent = new StringContent(strPayload, Encoding.UTF8, "application/json");
                        int retrycounter = 0;
retry:
                        window_file_log.Log("retrycounter :"+ retrycounter);
                        window_file_log.Log(strPayload);
                        int shouldretry=0;
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
                        if (shouldretry == 1&& retrycounter<10)
                        {
                            Thread.Sleep(1000*60*10);
                            goto retry;
                        }
                        //Program.evtObj.Set();
                        OnClientExit();
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
            mtcplistener?.Stop();
            ReceiveThread.Abort();
        }

    }
}
