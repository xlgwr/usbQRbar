using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace usbQRBar
{

    public partial class Form1 : Form
    {
        RevSerialPort rsp;
        BardCodeHooK BarCode = new BardCodeHooK();
        SampleMsr msr;
        MsrThreadingObject msrt;
        public Form1()
        {
            InitializeComponent();

            //msr = new SampleMsr();

            button1.Focus();
            //BarCode.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            //usbQRBar.MsrThreadingObject.AdvancedSampleMsr asm = new usbQRBar.MsrThreadingObject.AdvancedSampleMsr();

            //msrt = new MsrThreadingObject(asm);
            //msrt.OpenThread();
            //msrt.OpenThread();
           // msr.Open();
        }
        private delegate void ShowInfoDelegate(BardCodeHooK.BarCodes barCode);
        private void ShowInfo(BardCodeHooK.BarCodes barCode)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowInfoDelegate(ShowInfo), new object[] { barCode });
            }
            else
            {
                textBox1.Text = barCode.KeyName;
                textBox2.Text = barCode.VirtKey.ToString();
                textBox3.Text = barCode.ScanCode.ToString();
                textBox4.Text = barCode.Ascll.ToString();
                textBox5.Text = barCode.Chr.ToString();
                textBox6.Text = barCode.IsValid ? barCode.BarCode : "";//是否为扫描枪输入，如果为true则是 否则为键盘输入
                textBox7.Text += barCode.KeyName;
                //MessageBox.Show(barCode.IsValid.ToString());
            }
        }
        //C#中判断扫描枪输入与键盘输入

        //Private DateTime _dt = DateTime.Now;  //定义一个成员函数用于保存每次的时间点
        //private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    DateTime tempDt = DateTime.Now;          //保存按键按下时刻的时间点
        //    TimeSpan ts = tempDt .Subtract(_dt);     //获取时间间隔
        //    if (ts.Milliseconds > 50)                           //判断时间间隔，如果时间间隔大于50毫秒，则将TextBox清空
        //        textBox1.Text = "";
        //    dt = tempDt ;
        //}
        void BarCode_BarCodeEvent(BardCodeHooK.BarCodes barCode)
        {
            ShowInfo(barCode);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Focus();
            try
            {
                //如果不是默认的COM1端口，这里需要传入端口号
                rsp = new RevSerialPort("COM1");
                rsp.Start();
                //usb
               // BarCode.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rsp != null)
                rsp.Stop();

            BarCode.Stop();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (textBox6.Text.Length > 0)
            {
                MessageBox.Show("条码长度：" + textBox6.Text.Length + "\n条码内容：" + textBox6.Text, "系统提示");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
