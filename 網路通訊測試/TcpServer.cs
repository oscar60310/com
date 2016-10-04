using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace 網路通訊測試
{
    class TcpServer
    {
        private Socket serverSocket;
        private Form1 f;
        public void setSocket(Socket s,Form1 ff)
        {
            serverSocket = s;
            f = ff;
            listen = true;
        }
        public bool listen = true;
        public bool receive = true;
        public void start()
        {
            while (listen==true)
            {
                try
                {
                    
                    serverSocket = serverSocket.Accept();
                    if (f.multiConnect == false)
                    {
                        listen = false;
                    }
                    f.pm("[TCP]連接成功!", 4);
                    f.Tcpscess(serverSocket);
                    startlisten(serverSocket);
                }
                catch (Exception e)
                {
                    f.pm(e.ToString(), 3);
                }
            }
        }
        public void stop()
        {
            serverSocket.Close();
            f.pm("[TCP]連線關閉", 2);
            listen = false;

        }
      
        public void startlisten(Socket s)
        {
            byte[] b;
            while (receive == true)
            {
                try
                {
                    b = new byte[1024];

                    int n = s.Receive(b, 0, b.Length, SocketFlags.None);
                    if (n > 0)
                    {
                        string ss = f.b2s(b);

                        f.pm("[TPC]接收:" + ss, 1);
                      
                    }


                }
                catch (Exception ex)
                {
                    receive = false;
                    f.pm("[TPC]停止接收", 4);
                    f.pm(ex.ToString(), 3);
                }

            }
        }
    }
}
