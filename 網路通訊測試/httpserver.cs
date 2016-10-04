using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace 網路通訊測試
{
    class httpserver
    {
        Form1 f1;
        public void start(Form1 f,string ip,int port,string root)
        {
            try
            {
                rootPath = root;
                f1 = f;
                IPAddress serverip = IPAddress.Parse(ip);
                serverlistener = new TcpListener(serverip, port);
                serverlistener.Start();
                ClientNum = 0;
                serverstate = "啟動中";
                f.httpformupdate();
                //開始接聽
                listen = true;
                ThreadStart ts = new ThreadStart(ServerListen);
                td = new Thread(ts);
                td.Start();
                f.pm("[HTTPserver]" + serverip.ToString() + ":" + port.ToString() + "開始接聽", 4);
                serverstate = "接聽中";
                f.httpformupdate();
            }
            catch (Exception e)
            {
                f.pm(e.ToString(), 3);
                
            }
        }
        public int ClientNum;
        TcpListener serverlistener;
        public string serverstate;
        public Thread td;
        public void connect(Socket client)
        {
            ClientNum++;
            f1.pm("連線完成(編號:" + ClientNum.ToString() + ")Client-" + client.RemoteEndPoint.ToString(), 4);
            f1.pm("正在等候" + ClientNum.ToString() + "連線", 5);
            string re = receive(client);
            f1.pm("編號" + ClientNum.ToString() + "回應:" + re, 1);

            //GET
            string recall = "";
         
            switch (re.Trim().Split(' ')[0].ToUpper())
            {
                case "GET":
                    string request = re.Trim().Split(' ')[1];
                    if (request.Length == 1)
                    {
                        request = "/home.html";
                    }
                    
                    check(client, request,client.RemoteEndPoint.ToString());
                    break;
                case "POST":
                      string post = re.Trim().Split(' ')[1];
                      if (post == "/filerequest")
                      {
                          f1.adduser(ClientNum.ToString(), client.RemoteEndPoint.ToString());
                          recall += "HTTP/1.0 200 OK\r\n";
                          recall += "Server: oscar60310's server\r\n\r\n";
                          recall += ClientNum.ToString();
                          send(client, recall, ClientNum);
                          
                      }
                      else if (post.StartsWith("/pass"))
                      {
                          string id = post.Split(',')[1];
                          string pass = post.Split(',')[2];
                          recall += "HTTP/1.0 200 OK\r\n";
                          recall += "Server: oscar60310's server\r\n\r\n";
                          recall += f1.usercheck(id, pass);
                          send(client, recall, ClientNum);
                      }
                    break;
                

            }
           
           

            client.Shutdown(SocketShutdown.Both);
            f1.pm("交易完成，中斷連線 " + ClientNum.ToString(), 6);
            

        }
        public string rootPath = "";
        public void directsendfile(Socket s, string directpath)
        {
            try
            {
                FileInfo fs = new FileInfo (directpath);
                string re = "";
                re += "HTTP/1.0 200 OK\r\n";
                re += "Server: oscar60310's server\r\n";
                re += "Content-Type: application/octet-stream\r\n";
                re += "Content-Disposition: attachment; filename=" + fs.Name + "\r\n";

                
                re += "Content-Length: " + fs.Length.ToString() + "\r\n";
                re += "\r\n\r\n";

                send(s, re, ClientNum);
                FileStream fss = new FileStream(directpath, FileMode.Open);
                byte[] ba = new byte[fss.Length];
                fss.Read(ba, 0, (int)fss.Length);
                fss.Close();
                s.Send(ba);
                f1.pm("傳送" + directpath + " 給" + s.RemoteEndPoint.ToString(), 5);

            }
            catch
            {
                f1.pm(s.RemoteEndPoint.ToString() + "檔案傳送失敗", 3);
            }
        }
      
        public void sendfile(Socket s, string path)
        {
            try
            {
                FileInfo fs = new FileInfo(rootPath + "\\" + path);
                string re = "";
                re += "HTTP/1.1 200 OK\r\n";
                re += "Server: oscar60310's server\r\n";
                re += "Content-Type: ";
                if (path.EndsWith("html"))
                {
                    re += "text/html";
                }
                else if (path.EndsWith("htm"))
                {
                    re += "text/html";
                }
                else if (path.EndsWith("pdf"))
                {
                    re += "application/pdf";
                }
                else if (path.EndsWith("jpg"))
                {
                    re += "image/jpeg";
                }

                else
                {
                    re += "text/plain";
                }
                re += "\r\n";
                re += "Content-Length: " + fs.Length.ToString() + "\r\n";
                re += "\r\n\r\n";

                send(s, re, ClientNum);
                FileStream fss = new FileStream(rootPath + "\\" + path, FileMode.Open);
                byte []ba = new byte[fss.Length];
                fss.Read(ba, 0, (int)fss.Length);
                fss.Close();
                s.Send(ba);
                f1.pm("傳送" + path + " 給" + s.RemoteEndPoint.ToString()+"("+fss.Length+")", 5);

            }
            catch
            {
                f1.pm(s.RemoteEndPoint.ToString() + "檔案傳送失敗", 3);
            }
        }
        public void check(Socket s, string path,string ip)
        {
            string re = "";

            string fullpath = rootPath + "\\" + path;
            if (File.Exists(fullpath))
            {

                if (path.ToUpper().Contains("ADMIN") )
                {
                    re += "HTTP/1.0 302 Redirect\r\n";
                    re += "Server: oscar60310's server\r\n";
                    re += "Content-Type: text/html\r\n";
                    re += "Location: /PreLogin.html\r\n\r\n";
                    send(s, re, ClientNum);
                }
               
                else
                {
                    sendfile(s, path);
                    
                }
                
            }
            else
            {
                if (path.ToUpper().Split('?')[0] == "/DOWNLOAD")
                {
                   
                    string key = path.Split('?')[1];
                    string sp = f1.download(key, ip);
                    if (sp == "null")
                    {
                        sendfile(s, "passerror.html");
                    }
                    else if (sp == "downloaded")
                    {
                      
                        sendfile(s, "downloaded.html");
                    }
                    else
                    {
                        directsendfile(s, sp);
                    }
                }
                else
                {
                    sendfile(s, "/404.html");
                }
            }
          
           

         

        }
        
      
        public string receive(Socket client)
        {
            try
            {
                string s = "";
                byte[] b = new byte[1024];
                int n = 0;
                client.ReceiveTimeout = 2000;
                n = client.Receive(b, 0, b.Length, SocketFlags.None);
                if (n == 0)
                {
                    return "Receive None Data";
                }
                s = Encoding.UTF8.GetString(b); 
              
                
                return s;
            }
            catch 
            {

                return "無回應";
            }
        }
        public void stop()
        {
            listen = false;
            serverlistener.Stop();
            f1.pm("HTTPserver停止接聽", 2);
            serverstate = "停止運作";
            ClientNum = 0;
            td.Abort();
            f1.httpformupdate();
        }
        bool listen;
        public bool send(Socket client, string s,int num)
        {
            try
            {
                byte[] b = Encoding.UTF8.GetBytes(s);
                client.Send(b);
                f1.pm("傳送給" + num.ToString() + ":" + s,1);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ServerListen()
        {
            while (listen)
            {
                try
                {
                    Socket client = serverlistener.AcceptSocket();
                    f1.pm("[HTTPserver]取得連線要求", 4);
                    f1.httpformupdate();
                    connect(client);
                }
                catch (Exception e)
                {
                    f1.pm(e.ToString(), 3);
                }
            }
            f1.pm("HTTPserver停止接聽", 5);
        }
    }
}
