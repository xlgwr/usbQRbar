using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HookGlobal
{
    public partial class Form1 : Form
    {
        KeyBordHook kbh;
        public Form1()
        {
            InitializeComponent();
            this.AcceptButton = button1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             kbh= new KeyBordHook();
             kbh.OnKeyUpEvent += kbh_OnKeyUpEvent;
             kbh.OnKeyDownEvent += kbh_OnKeyDownEvent;
             kbh.OnKeyPressEvent += kbh_OnKeyPressEvent;

        }

        void kbh_OnKeyUpEvent(object sender, KeyEventArgs e)
        {
             if (e.KeyData==Keys.Enter)
            {
                textBox2.Text += "\n";
            }            
        }

        void kbh_OnKeyPressEvent(object sender, KeyPressEventArgs e)
        {
            textBox2.Text += e.KeyChar.ToString();
        }

        void kbh_OnKeyDownEvent(object sender, KeyEventArgs e)
        {
            //if (e.KeyData==Keys.Enter)
            //{
            //    textBox2.Text += "\n";
            //}
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (kbh!=null)
            {
                kbh.Stop();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
