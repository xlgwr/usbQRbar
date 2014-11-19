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
    public partial class frmList : Form
    {
        frm0Main _frm0main;
        Control _cl;
        string _strtmp;
        bool _changeSelectp;
        public frmList()
        {
            InitializeComponent();
        }

        public frmList(frm0Main frmmain)
        {
            InitializeComponent();
            _frm0main = frmmain;
        }
        public frmList(frm0Main frmmain, Control cl)
        {
            InitializeComponent();
            _frm0main = frmmain;
            _cl = cl;
            foreach (var item in _frm0main.listBox1.Items)
            {
                listBox1.Items.Add(item);
            }
            foreach (var item in _frm0main._strlit)
            {
                listBox1.Items.Remove(item);
            }


        }
        private void frmList_Load(object sender, EventArgs e)
        {
            listBox1.HorizontalScrollbar = true;
            this.Location = new Point(Control.MousePosition.X - this.Width, Control.MousePosition.Y + _cl.Height);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem==null)
            {
                return;
            }
            //MessageBox.Show(listBox1.SelectedItem.ToString());
            if (string.IsNullOrEmpty(_cl.Text))
            {
                _strtmp = "";
                _cl.Text = listBox1.SelectedItem.ToString();
            }
            else
            {
                _strtmp = _cl.Text;
                _cl.Text = listBox1.SelectedItem.ToString();
            }
            _changeSelectp = true;
        }

        private void frmList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_changeSelectp)
            {
                if (!string.IsNullOrEmpty(_strtmp))
                {
                    _frm0main._strlit.Remove(_strtmp);
                    _strtmp = "";
                }
                _frm0main._strlit.Add(_cl.Text);
            }
            else
            {
                _changeSelectp = false;
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
