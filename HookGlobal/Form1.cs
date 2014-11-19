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
    public partial class frm0Main : Form
    {
        public static string getQRcode = "";

        public List<string> _strlit=new List<string>();
        KeyBordHook kbh;
        public frm0Main()
        {
            InitializeComponent();
            this.AcceptButton = button1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            kbh = new KeyBordHook();
            kbh.OnKeyUpEvent += kbh_OnKeyUpEvent;
            kbh.OnKeyDownEvent += kbh_OnKeyDownEvent;
            kbh.OnKeyPressEvent += kbh_OnKeyPressEvent;
            listBox1.HorizontalScrollbar = true;

        }

        void kbh_OnKeyUpEvent(object sender, KeyEventArgs e)
        {
            if (getQRcode.Length > 3)
            {
                getQRcode = getQRcode.Replace("<|>", "\n").Replace(",>", "\n").Replace("<>", "\n").Replace("\r", "\n");
            }

            if (e.KeyData == Keys.Enter)
            {
               
                var arr = getQRcode.Split('\n');
                foreach (var arrNext in arr)
                {
                    string[] strlist = arrNext.Split(' ');
                    foreach (var item in strlist)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (!listBox1.Items.Contains(item))
                            {
                                listBox1.Items.Add(item);
                            }
                        }
                    }
                }

            }
        }

        void kbh_OnKeyPressEvent(object sender, KeyPressEventArgs e)
        {
            getQRcode += e.KeyChar;
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
            if (kbh != null)
            {
                kbh.Stop();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //getQRcode = "";
            textBox1.Text = "";
            listBox1.Items.Clear();
            _strlit.Clear();
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            //textBox1.Focus();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            getQRcode = "";
        }

        private void txt0Divide_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmList fl = new frmList(this, textBox2);
            fl.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmList fl = new frmList(this, textBox3);
            fl.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frmList fl = new frmList(this, textBox4);
            fl.ShowDialog();
        }
    }
}
