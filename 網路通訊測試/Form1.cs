using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using finger;

namespace 網路通訊測試
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            printIP();
        }
        void httpformreset()
        {
            ConnectNUM.Text = "0";
            ServerState.Text = "未啟動";
            ServerIP.Text = "伺服器尚未啟動";
            ConnectNUM.ForeColor = Color.Red;
            ServerState.ForeColor = Color.Red;
            ServerIP.ForeColor = Color.Red;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            SystemID si = new SystemID();
            if (!si.check("HttpTool1"))
            {
                UniqueKey.Form1 uf = new UniqueKey.Form1();
                uf.ShowDialog();
                Close();
            }
            else
            {
                httplistreset();
                Form.CheckForIllegalCrossThreadCalls = false;
                printIP();
                comboBox1.Items.Add("Unicode");
                comboBox1.Items.Add("UTF8");
                comboBox1.Items.Add("UTF32");
                comboBox1.Items.Add("ASCII");
                comboBox1.Items.Add("BEU");
                comboBox1.Items.Add("Default");
                httpformreset();
                this.Enabled = true;
            }

        }
      
        public string b2s(byte[] b)
        {
            string s = "";
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    s=Encoding.Unicode.GetString(b);
                    break;
                case 1:
                    s = Encoding.UTF8.GetString(b);
                    break;
                case 2:
                    s = Encoding.UTF32.GetString(b);
                    break;
                case 3:
                    s = Encoding.ASCII.GetString(b);
                    break;
                case 4:
                    s = Encoding.BigEndianUnicode.GetString(b);
                    break;
                case 5:
                    s = Encoding.Default.GetString(b);
                    break;
            }

            return s;
        }
        public byte [] s2b(string s)
        {
            byte[] b = null;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    b = Encoding.Unicode.GetBytes(s);
                    break;
                case 1:
                    b = Encoding.UTF8.GetBytes(s);
                    break;
                case 2:
                    b = Encoding.UTF32.GetBytes(s);
                    break;
                case 3:
                    b = Encoding.ASCII.GetBytes(s);
                    break;
                case 4:
                    b = Encoding.BigEndianUnicode.GetBytes(s);
                    break;
                case 5:
                    b = Encoding.Default.GetBytes(s);
                    break;
            }

            return b;
        }
        public void Tcpscess(Socket s)
        {
             
            label5.Text="連線成功";
            pm("[TCP]客戶端IP:" + s.RemoteEndPoint.ToString(), 1);
            button4.Text = "斷開";
            label6.Text = "傳送給客戶端:";
            input = 1;
            server = s;
        }
        int input = 0;
        void printIP()
        {
            string strHostName = Dns.GetHostName();
            pm("本機名稱:" + strHostName, 2);
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    pm("本機IP(只取IP4):" + ipaddress.ToString(), 2);
                    label3.Text = ipaddress.ToString();
                }
            }
            
        
        }
        
        public void pm(string msg,int m)
        {
            info.SelectionStart = info.Text.Length;
            switch (m)
            {
                case 1://一般
                    info.SelectionColor = Color.Black;
                    info.AppendText(msg);
                    info.AppendText(Environment.NewLine);
                    break;
                case 2://程式動作
                    info.SelectionColor = Color.Blue;
                    info.AppendText("[系統]"+msg+Environment.NewLine);    
                    break;
                case 3://錯誤
                    info.SelectionColor = Color.Red;
                    info.AppendText("[錯誤]" + msg + Environment.NewLine);
                    break;
                case 4://成功
                    info.SelectionColor = Color.Green;
                    info.AppendText( msg + Environment.NewLine);
                    break;
                case 5:
                    info.SelectionColor = Color.FromArgb(47, 222, 184);
                    info.AppendText(msg + Environment.NewLine);
                    break;
                case 6:
                    info.SelectionColor = Color.FromArgb(31, 65, 145);
                    info.AppendText(msg + Environment.NewLine);
                    break;

            }
            info.SelectionStart = info.Text.Length;
            info.ScrollToCaret();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            info.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 0)
            {

                this.textBox1.Focus();
                string s = textBox1.Text;
                if (checkBox1.Checked == true)
                {
                    s = s + "\r\n";
                }
                textBox1.Text = "";
                switch (input)
                {
                    case 0:
                        pm("尚未接受指令", 3);
                        break;
                    case 1:
                        sendtoClient(s);
                        break;
                    case 2:
                        sendtoServer(s);
                        break;
                    case 3:
                        sendtoUDPclient(s);
                        break;
                    case 4:
                        sendtoUDPserver(s);
                        break;

                }
            }
        }
        TcpServer ts = new TcpServer();
        Socket server;
        
        public void sendtoServer(string s)
        {
            byte[] b = s2b(s);
            client.Send(b);
            pm("[TCP]傳送:" + s, 1);
        }
        public void sendtoClient(string s)
        {
            try
            {
                byte[] b = s2b(s);
                server.Send(b);
                pm("[TCP]傳送:" + s, 1);
            }
            catch
            {
                pm("連線已被關閉，請斷開連線", 3);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (button4.Text == "啟動")
                {
                    int port = 0;
                    try
                    {
                        port = int.Parse(portinput.Text);
                    }
                    catch
                    {
                        pm("通訊埠名稱不合法!", 3);
                        return;
                    }
                    try
                    {
                        if (checkBox2.Checked)
                        {
                            multiConnect = true;
                            pm("開啟多重連線，最大連線數4", 4);
                        }
                        else
                        {
                            multiConnect = false;
                        }
                        IPAddress ip = IPAddress.Parse(label3.Text);
                        IPEndPoint serverhost = new IPEndPoint(ip, port);
                        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        serverSocket.Bind(serverhost);
                        serverSocket.Listen(4);
                        ts = new TcpServer();
                      
                        ts.setSocket(serverSocket, this);

                        ThreadStart tst = new ThreadStart(ts.start);
                        Thread td = new Thread(tst);
                        td.Start();

                        pm("開啟TCP通訊，IP:" + label3.Text + " port:" + port.ToString(), 2);
                        label5.Text = "等待連線中";
                        button4.Text = "停止接聽";

                        button5.Enabled = false;

                    }
                    catch (Exception ex)
                    {
                        pm(ex.ToString(), 3);
                    }
                }
                else if (button4.Text == "停止接聽")
                {

                    ts.stop();
                    button4.Text = "啟動";
                    label5.Text = "尚未啟動";
                   
                    button5.Enabled = true;
                }
                else
                {
                    ts.stop();
                    input = 0;
                    label5.Text = "尚未啟動";
                    button4.Text = "啟動";
                    label6.Text = "";
                    button5.Enabled = true;
                }
            }
            catch (Exception exc)
            {
                pm(exc.ToString(), 3);
            }
        }
        public bool multiConnect;
        private void label7_Click(object sender, EventArgs e)
        {

        }

        Socket client;
        public void TcpClientsecess(Socket s)
        {
            client = s;
            pm("連線成功", 4);
            label6.Text = "傳送給主機:";
            button5.Text = "關閉";
            input = 2;
            button5.Enabled = true;
        }
        TcpClientReader tcr;
        public void ServerConnectFail()
        {
            button5.Enabled = true;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (button5.Text == "連接")
                {
                    IPAddress ip = null;
                    try
                    {
                        ip = IPAddress.Parse(textBox2.Text);
                    }
                    catch
                    {
                        pm("正在解析主機名稱", 1);
                        foreach (IPAddress ips in Dns.GetHostAddresses(textBox2.Text))
                        {
                          //  pm("取得IP:" + ips.ToString(), 1);
                            ip = ips;
                        }
                        pm("使用" + ip.ToString(), 4);
                    }
                    pm("[TCP]正在要求主機連線IP: " + textBox2.Text + " Port:" + textBox3.Text, 2);

                    Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint serverhost=new IPEndPoint(ip,int.Parse(textBox3.Text));
                    tcr = new TcpClientReader();
                    tcr.set(clientSocket, serverhost, this);
                    ThreadStart ts = new ThreadStart(tcr.start);
                    Thread td = new Thread(ts);
                    td.Start();
                    button5.Enabled = false;
                  
                }
                else
                {
                    button5.Text = "連接";
                    tcr.stop();
                    input = 0;
                    label6.Text = "";
                    button4.Enabled = true;
                }
            }
            catch (Exception ez)
            {
                pm(ez.ToString(), 3);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.set(this);
            f2.Show();

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.set(this);
            f3.Show();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
        httpserver hs;
        public void httpformupdate()
        {
            ServerState.Text = hs.serverstate;
            ServerIP.Text = label3.Text;
            ConnectNUM.Text = hs.ClientNum.ToString();
        }
        void httplistreset()
        {

            listView2.Clear();
            listView2.GridLines = true;
            listView2.FullRowSelect = true;
            listView2.View = View.Details;
            listView2.Scrollable = true;
            listView2.MultiSelect = false;

            listView1.Clear();
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.View = View.Details;
            listView1.Scrollable = true;
            listView1.MultiSelect = false;

            listView1.Columns.Add("序號", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("狀態", 100, HorizontalAlignment.Center);
            listView1.Columns.Add("密碼", 200, HorizontalAlignment.Center);
            listView1.Columns.Add("路徑", 300, HorizontalAlignment.Center);
            filenum = 0;

            listView2.Columns.Add("ID", 30, HorizontalAlignment.Center);
            listView2.Columns.Add("狀態", 150, HorizontalAlignment.Center);
        }
        public void adduser(string id,string pass)
        {
            ListViewItem l = new ListViewItem();
            l.SubItems[0].Text = id;
            l.SubItems.Add(pass);
            listView2.Items.Add(l);
            (right.Items[5] as ToolStripMenuItem).DropDownItems.Add("連線:" + id);
            (right.Items[5] as ToolStripMenuItem).DropDownItems[(right.Items[5] as ToolStripMenuItem).DropDownItems.Count - 1].Click += userAccept;
        }
        public void userAccept(object sender, EventArgs e)
        {
            try
            {
                string id = sender.ToString().Split(':')[1];
                for (int i = 0; i < listView2.Items.Count; i++)
                {
                    if (id == listView2.Items[i].SubItems[0].Text)
                    {
                        listView2.Items[i].SubItems[1].Text = "同意連線 " + randomString(4) + " " + listView1.SelectedItems[0].SubItems[2].Text;
                    }
                }
            }
            catch
            {
            }
        }
        public string usercheck(string id, string pass)
        {
            string re = "null";
            for (int i = 0; i < listView2.Items.Count; i++)
            {
                if (id == listView2.Items[i].SubItems[0].Text)
                {
               
                    if (listView2.Items[i].SubItems[1].Text.StartsWith("同意連線"))
                    {
                        if (listView2.Items[i].SubItems[1].Text.Split(' ')[1] == pass)
                        {
                            re = listView2.Items[i].SubItems[1].Text.Split(' ')[2];
                        }

                    }
                    i = listView2.Items.Count;
                }
            }
            return re;
        }
        private void button8_Click(object sender, EventArgs e)
        {
            
            int port = int.Parse(textBox4.Text);
            hs = new httpserver();
            hs.start(this, label3.Text, port,System.IO.Directory.GetCurrentDirectory()+"\\wwwroot");
            pm("共用資料夾為:" + System.IO.Directory.GetCurrentDirectory() + @"\wwwroot\", 4);

        }

        private void ServerIP_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            hs.stop();
        }

        private void 新增檔案ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName.Length != 0)
            {
                filenum++;
                ListViewItem l = new ListViewItem();
                l.SubItems[0].Text = filenum.ToString();
                l.SubItems.Add("等待下載");
                l.SubItems.Add(randomString(30));
                l.SubItems.Add(openFileDialog1.FileName);
                l.SubItems[1].ForeColor = Color.Green;
             
                listView1.Items.Add(l);
                listView1.Items[filenum - 1].UseItemStyleForSubItems = false;
            }
        }
        public int filenum;
        public string download(string pass,string ip)
        {
            string s = "null";
            for(int i=0;i<listView1.Items.Count;i++)
            {
                if (pass == listView1.Items[i].SubItems[2].Text)
                {
                    if (listView1.Items[i].SubItems[1].Text.StartsWith("等待下載"))
                    {
                        s = listView1.Items[i].SubItems[3].Text;

                        if (listView1.Items[i].SubItems[1].Text == "等待下載")
                        {
                            listView1.Items[i].SubItems[1].Text = "已下載";
                            listView1.Items[i].SubItems[1].ForeColor = Color.Blue;
                        }
                        else
                        {
                            int n = int.Parse(listView1.Items[i].SubItems[1].Text.Split('[')[1].Split(']')[0]);
                            n = n + 1;
                            listView1.Items[i].SubItems[1].Text = "等待下載[" + n.ToString() + "]";
                            listView1.Items[i].SubItems[1].ForeColor = Color.Blue;
                        }
                  
                        i = listView1.Items.Count;
                    }
                    else
                    {
                        s = "downloaded";
                        i = listView1.Items.Count;
                    }
                }
            }

            return s;
        }
        public string randomString(int n)
        {
            string s = "";
            string c = "a,b,c,d,e,f,g,h,i,j,k,l,m,m,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,1,2,3,4,5,6,7,8,9,0";
            string[] rc = c.Split(',');
            Random r = new Random();
            for (int i = 0; i < n; i++)
            {
                s += rc[r.Next(0, rc.Length)];
            }
            

            return s;
        }

        private void 複製連結ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetData(DataFormats.Text, "http://" + label3.Text + ":" + textBox4.Text + "/download?" + listView1.SelectedItems[0].SubItems[2].Text);
            }
            catch { }
        }

        private void 停止下載ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                listView1.SelectedItems[0].SubItems[1].Text = "停止下載";
                listView1.SelectedItems[0].SubItems[1].ForeColor = Color.Red;
            }
            catch { }
        }

        private void 開放下載ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                listView1.SelectedItems[0].SubItems[1].Text = "等待下載";
                listView1.SelectedItems[0].SubItems[1].ForeColor = Color.Green;
            }
            catch { }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            httplistreset();
        }

        private void 不限次數ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                listView1.SelectedItems[0].SubItems[1].Text = "等待下載[0]";
                listView1.SelectedItems[0].SubItems[1].ForeColor = Color.Green;
            }
            catch { }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView2.SelectedItems[0].SubItems[1].Text.StartsWith("同意連線"))
                {
                    Clipboard.SetData(DataFormats.Text, listView2.SelectedItems[0].SubItems[1].Text.Split(' ')[1]);

                }
               
            }
            catch { }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                listView2.SelectedItems[0].SubItems[1].Text = "拒絕連線";
               

            }
            catch { }
        }
        bool udplisten;
        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                if (button11.Text == "啟動")
                {
                    int port = 0;
                    try
                    {
                        port = int.Parse(textBox5.Text);
                    }
                    catch
                    {
                        pm("通訊埠名稱不合法!", 3);
                        return;
                    }
                    try
                    {
                        input = 3;
                        IPAddress ip = IPAddress.Parse(label3.Text);
                        IPEndPoint serverhost = new IPEndPoint(ip, port);
                        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        serverSocket.Bind(serverhost);
                        udplisten = true;

                        udpsoc = serverSocket;
                        ThreadStart tst = new ThreadStart(udp);
                        Thread td = new Thread(tst);
                        td.Start();

                        pm("開啟UDP通訊，IP:" + label3.Text + " port:" + port.ToString(), 2);
                        label17.Text = "等待連線中";
                        button11.Text = "停止接聽";

                        

                    }
                    catch (Exception ex)
                    {
                        pm(ex.ToString(), 3);
                    }
                }
                else if (button11.Text == "停止接聽")
                {

                    udplisten = false;
                    button11.Text = "啟動";
                    label17.Text = "尚未啟動";
                    
                   
                }
                else
                {
                    udplisten = false;
                    input = 0;
                    label17.Text = "尚未啟動";
                    button11.Text = "啟動";
                   
                   
                }
            }
            catch (Exception exc)
            {
                pm(exc.ToString(), 3);
            }
        }
        Socket udpsoc;
        public void sendtoUDPclient(string s)
        {
            try
            {
                udpsoc.Send(s2b(s));
                pm("[UDP]傳送:" + s, 1);
            }
            catch (Exception e)
            {
                pm(e.ToString(), 3);
            }
        }
        public void sendtoUDPserver(string s)
        {
            try
            {
                udpcsoc.Send(s2b(s));
                pm("[UDP]傳送:" + s, 1);
            }
            catch (Exception e)
            {
                pm(e.ToString(), 3);
            }
        }
        public void udp()
        {
            try
            {
                while (udplisten)
                {
                    pm("無資料", 1); 
                    byte[] data = new byte[1024]; // 設定接收緩衝區的陣列變數。
                    int recv = udpsoc.Receive(data);
                    if (recv > 0)
                    {
                        pm("[UDP]傳送:" + b2s(data), 1);
                    }
                }

                udpsoc.Close();
                pm("停止UDP伺服器", 2);
                
            }
            catch (Exception e)
            {
                pm(e.ToString(), 3);
            }
            finally
            {
              
            }
        }
        Socket udpcsoc;
        bool udpclis;
        public void udpc()
        {
            try
            {
                while (udpclis)
                {
                 /*   pm("無資料", 1);
                    byte[] data = new byte[1024]; // 設定接收緩衝區的陣列變數。
                    int recv = udpcsoc.Receive(data);
                    if (recv > 0)
                    {?
                        pm("[UDP]傳送:" + b2s(data), 1);
                    }*/

                }
                udpcsoc.Close();
                pm("停止UDP客戶", 2);

            }
            catch (Exception e)
            {
                pm(e.ToString(), 3);
            }
            finally
            {

            }
        }
        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                if (button12.Text == "連接")
                {
                    int port = 0;
                    try
                    {
                        port = int.Parse(textBox7.Text);
                    }
                    catch
                    {
                        pm("通訊埠名稱不合法!", 3);
                        return;
                    }
                    try
                    {
                        input = 4;
                        IPAddress ip = IPAddress.Parse(textBox6.Text);
                        IPEndPoint serverhost = new IPEndPoint(ip, port);
                        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


                        udpclis = true;
                        udpcsoc = serverSocket;
                        ThreadStart tst = new ThreadStart(udpc);
                        Thread td = new Thread(tst);
                        td.Start();

                        pm("開啟UDP通訊，IP:" + ip + " port:" + port.ToString(), 2);
                        label21.Text = "等待連線中";
                        button12.Text = "斷開";



                    }
                    catch (Exception ex)
                    {
                        pm(ex.ToString(), 3);
                    }
                }
                else if (button12.Text == "斷開")
                {

                    udpclis = false;
                    button12.Text = "連接";
                    label21.Text = "尚未啟動";


                }
                else
                {
                    udpclis = false;
                    input = 0;
                    button12.Text = "連接";
                    label21.Text = "尚未啟動";


                }
            }
            catch (Exception exc)
            {
                pm(exc.ToString(), 3);
            }
        }
    }
   
}

