using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using finger;

namespace UniqueKey
{
    public partial class Form1 : Form
    {
        private static string id = "HttpTool1";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label3.Text = id;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SystemID si = new SystemID();
            if(si.register(id, textBox1.Text))
            {
                MessageBox.Show("註冊成功，謝謝您的使用，重新開啟程式即可");
                Close();
            }
            else
            {
                MessageBox.Show("註冊失敗，請檢察您的金鑰和網路連線");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://goofygae.appspot.com/service/appkey.jsp?id="+id);
        }
    }
}
