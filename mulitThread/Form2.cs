using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;

namespace mulitThread
{
    public partial class Form2 : Form
    {
        public Form1 _frm1;
        public Form2()
        {
            InitializeComponent();
        }
        public Form2(Form1 frm1)
        {
            InitializeComponent();
            _frm1 = frm1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoWork();
        }
        public void DoWork()
        {
            //WaitCallback wc = new WaitCallback(DoSomething);
            //ThreadPool.QueueUserWorkItem(wc, "from2 set");
            Thread t = new Thread(new ParameterizedThreadStart(ChangeText));
            t.IsBackground = true;
            t.Start("from2 set");
        }
        public void DoWork2()
        {
            WaitCallback wc = new WaitCallback(DoSomething2);
            ThreadPool.QueueUserWorkItem(wc, "from2 set use paras");
        }
        public void DoWork3()
        {
            WaitCallback wc = new WaitCallback(DoSomething3);
            ThreadPool.QueueUserWorkItem(wc, "dowork3 frm2");
        }
        public void DoSomething3(object o)
        {
            System.Func<string, int> f = new Func<string, int>(GetId);
            object result = _frm1.textBox1.Invoke(f, o.ToString());
            _frm1.textBox1.Text = result.ToString();

        }
        public int GetId(string name)
        {
            _frm1.textBox1.Text = name;
            if (name == "dd")
            {
                return 0;
            }
            else
            {
                return 999;
            }
        }
        public delegate void MyInvokeDelegate(object name);
        public void ChangeText(object name)
        {
            for (int i = 0; i < 100000; i++)
            {
                textBox1.Text = name.ToString() + i;
            }

        }
        public void DoSomething(object o)
        {
            _frm1.textBox1.Invoke(new MyInvokeDelegate(ChangeText), o.ToString());
        }
        public void DoSomething2(object o)
        {
            for (int i = 0; i < 100000; i++)
            {
                _frm1.Invoke(new Action(delegate()
                {
                    _frm1.textBox1.Text = o.ToString() + i;
                    textBox1.Text = o.ToString() + i;

                }));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DoWork2();
        }

        private void btn1_Click(object sender, EventArgs e)
        {
           // Thread t = new Thread(new ParameterizedThreadStart(DoSomething2));
           // t.Start("ddo");
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoSomething2), "ddo");

        }
    }
}
