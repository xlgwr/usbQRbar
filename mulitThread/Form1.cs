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
    public partial class Form1 : Form
    {
        Form2 _f2;
        public Form1()
        {
            InitializeComponent();
        }
        //新开一个线程，执行一个方法，没有参数传递
        public void DoWork1()
        {
            Thread t = new Thread(new ThreadStart(DoSomeThing));
            t.Start();

        }
        //带参数
        public void DoWork2()
        {
            Thread t = new Thread(new ParameterizedThreadStart(DoSomeThing));
            t.Start("form1 use paramer");
        }
        //使用线程池
        public void DoWork3()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoSomeThing), "use from 1 paramer");

        }
        //使用自定义委托
        public void DoWork4()
        {
            WaitCallback wc = new WaitCallback(DoSomeThing4);
            ThreadPool.QueueUserWorkItem(wc, "invoke from 1");
        }
        public delegate void MyInvokeDelegate(string name);
        public void ChangeText(string name)
        {
            this.textBox1.Text = name;
        }
        public void DoSomeThing4(object o)
        {
            this.Invoke(new MyInvokeDelegate(ChangeText), o.ToString());
        }

        public void DoSomeThing()
        {
            Form2 f2 = new Form2(this);
            f2.textBox1.Text = "form1 use";
            MessageBox.Show("form1 use");
            f2.Show();
        }
        public void DoSomeThing(object o)
        {
            Form2 f2 = new Form2(this);
            f2.textBox1.Text = o.ToString();
            MessageBox.Show(o.ToString());
            f2.Show();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DoWork1();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DoWork2();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DoWork3();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DoWork4();
            _f2 = new Form2(this);
            //_f2.btn1.Click += btn1_Click;
            _f2.Show();
        }

        void btn1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(ChangeTexts));
            t.Start("good");
            
        }

        public void ChangeTexts(object name)
        {
            for (int i = 0; i < 100000; i++)
            {
                this.Invoke(new Action(delegate() { _f2.textBox1.Text = name.ToString() + i.ToString(); }));
            }

        }
    }
}
