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
        Control _cl_prefix;
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
        public frmList(frm0Main frmmain, Control cl_content, Control cl_prefix)
        {
            InitializeComponent();
            _frm0main = frmmain;
            _cl = cl_content;
            _cl_prefix = cl_prefix;

            if (_frm0main._firstOpenSelectList == 1)
            {
                _frm0main._strNoPrefixlit = _frm0main._strNoPrefixlitTmp;
            }
            if (!string.IsNullOrEmpty(cl_prefix.Text))
            {
                foreach (var item in _frm0main._strlit)
                {
                    listBox1.Items.Add(item);
                }
            }
            else
            {
                foreach (var item in _frm0main._strNoPrefixlit)
                {
                    listBox1.Items.Add(item);
                }
            }





        }
        private void frmList_Load(object sender, EventArgs e)
        {
            listBox1.HorizontalScrollbar = true;
            this.Location = new Point(Control.MousePosition.X - this.Width/2, Control.MousePosition.Y + _cl.Height);

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                return;
            }
            //MessageBox.Show(listBox1.SelectedItem.ToString());
            if (string.IsNullOrEmpty(_cl_prefix.Text))
            {
                _strtmp = _cl.Text;
                _cl.Text = listBox1.SelectedItem.ToString();
                _changeSelectp = true;
            }
            else
            {
                _cl.Text = _frm0main.getPrefixOfContent(listBox1.SelectedItem.ToString());
            }
        }

        private void frmList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_changeSelectp)
            {
                _frm0main._strNoPrefixlit.Remove(_cl.Text);
                if (!string.IsNullOrEmpty(_strtmp))
                {
                    _frm0main._strNoPrefixlit.Add(_strtmp);
                }
            }
            _changeSelectp = false;

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
