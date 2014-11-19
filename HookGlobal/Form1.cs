using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HookGlobal.model;

namespace HookGlobal
{
    public partial class frm0Main : Form
    {
        public static string getQRcode = "";

        public List<string> _strlit = new List<string>();
        public static List<prefixContent> _prefixcontList;

        KeyBordHook kbh;

        public frm0Main()
        {
            InitializeComponent();
            this.AcceptButton = button1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _prefixcontList = new List<prefixContent>();

            _prefixcontList.Add(new prefixContent() { _prefix = txt0_Prefix.Text.Trim(), _cl = txt00_Content });
            _prefixcontList.Add(new prefixContent() { _prefix = txt1_Prefix.Text.Trim(), _cl = txt01_Content });
            _prefixcontList.Add(new prefixContent() { _prefix = txt2_Prefix.Text.Trim(), _cl = txt02_Content });
            ///
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

                                //
                                foreach (var fc in _prefixcontList)
                                {
                                    if (item.StartsWith(fc._prefix,true,null))
                                    {
                                        fc._cl.Text = item.Substring(fc._prefix.Length);
                                        _strlit.Add(item);
                                    }
                                }

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
            txt00_Content.Text = "";
            txt01_Content.Text = "";
            txt02_Content.Text = "";
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
            frmList fl = new frmList(this, txt00_Content);
            fl.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmList fl = new frmList(this, txt01_Content);
            fl.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frmList fl = new frmList(this, txt02_Content);
            fl.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmList fl = new frmList(this, txt03_Content);
            fl.ShowDialog();
        }


        private void removeExistCl(Control prefix, Control cl)
        {
            for (int i = 0; i < _prefixcontList.Count; i++)
            {
                if (_prefixcontList[i]._cl==cl)
                {
                    _prefixcontList.RemoveAt(i);
                }
            }
            if (!string.IsNullOrEmpty(prefix.Text))
            {

                _prefixcontList.Add(new prefixContent { _prefix = prefix.Text.Trim(), _cl = cl });
            }
            getQRcode = "";
        }
        private void txt0_Prefix_TextChanged(object sender, EventArgs e)
        {
            removeExistCl(txt0_Prefix, txt00_Content);
        }

        private void txt1_Prefix_TextChanged(object sender, EventArgs e)
        {
            removeExistCl(txt1_Prefix, txt01_Content);
        }

        private void txt2_Prefix_TextChanged(object sender, EventArgs e)
        {
            removeExistCl(txt2_Prefix, txt02_Content);
        }

        private void txt3_Prefix_TextChanged(object sender, EventArgs e)
        {
            removeExistCl(txt3_Prefix, txt03_Content);
        }

    }
}
