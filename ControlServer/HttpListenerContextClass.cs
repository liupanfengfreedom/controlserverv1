using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Diagnostics;

namespace ControlServer
{
    class HttpListenerContextClass
    {
        HttpListenerContext mhttplistenercontext;
        Thread handleThread;
        public HttpListenerContextClass(HttpListenerContext p)
        {
            mhttplistenercontext = p;
            handleThread = new Thread(new ThreadStart(handlethreadfunc));
            handleThread.IsBackground = true;
            handleThread.Start();
        }
        ~HttpListenerContextClass()
        {
            Console.WriteLine("HttpListenerContextClass deconstruct");
        }
        void handlethreadfunc()
        {
            try
            {
                HttpListenerRequest request = mhttplistenercontext.Request;
                System.Collections.Specialized.NameValueCollection header = request.Headers;
                string[] headerallkeys = header.AllKeys;
                foreach (var a in headerallkeys)
                {
                    if (a.Equals("RARpath"))//
                    {
                        string[] values = header.GetValues(a);
                        HttpclientHelper.httpget(values[0],(ref string str,ref byte[] bytearray)=> {
                            string path = AppDomain.CurrentDomain.BaseDirectory;
                            path += "x.rar";
                            path = @"F:\uev";//\Content;
                            Utility.SubDirectoryDelete(path+ "/Content");
                            //Thread.Sleep(5000);
                            File.WriteAllBytes(path+"/x.rar", bytearray);
                            Console.WriteLine("writefileok");
                           //Thread.Sleep(5000);
                            string apppath = @"E:\Program Files\7-Zip\7zG.exe";
                            string passArguments = "x F:/uev/x.rar -oF:/uev/Content";
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
                    }
                }
                System.IO.Stream input = request.InputStream;
                byte[] array = new byte[request.ContentLength64];
                input.Read(array, 0, (int)request.ContentLength64);//larg file may encounter error
                string utfString = Encoding.UTF8.GetString(array, 0, array.Length);

                HttpListenerResponse response = mhttplistenercontext.Response;
                // Construct a response.
                string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
                responseString += utfString;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                Thread.Sleep(30);
                input.Close();
                output.Close();
            }
            catch(Exception e)
            {
                //throw (e);
                Console.WriteLine(e);

            }

        }
    }
}
