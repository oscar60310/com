﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 網路通訊測試
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            f1.sendtoClient(textBox1.Text);
          
            Close();
        }
        Form1 f1;
        public void set(Form1 f)
        {
            f1 = f;
        }
    }
}
