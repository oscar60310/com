using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace 網路通訊測試
{
    class TcpClientReader
    {
        Socket client;
        IPEndPoint ip;
        public void set(Socket s,IPEndPoint i,Form1 f1)
        {
            try
            {
               
                client = s;
                f = f1;
                receive = true;
                ip = i;

            }
            catch (Exception ex)
            {
                f1.pm(ex.ToString(), 3);

            }
        }
        public bool receive = true;
        Form1 f;
        public void stop()
        {
            client.Close();
            f.pm("[TCP]連線關閉", 2);
            receive = false;
        }
        public void start()
        {
            try
            {
                client.Connect(ip);
                f.TcpClientsecess(client);
                startlisten();
            }
            catch (Exception e)
            {
                f.pm(e.ToString(), 3);
                f.ServerConnectFail();
            }
        }
        public void startlisten()
        {
          
            while (client.Connected==true && receive == true)
            {
                try
                {
                    byte[] b;
                    b = new byte[1024];

                       int n = client.Receive(b, 0, b.Length, SocketFlags.None);
                       if (n > 0)
                       {
                           string ss = f.b2s(b);

                           f.pm("[TCP]接收:" + ss, 1);
                       }
                    
                   
                    
                }
                catch(Exception ex)
                {
                    receive = false;
                    f.pm("[TCP]停止接收", 4);
                    f.pm(ex.ToString(), 3);
                }
            }
        }
    }
}
