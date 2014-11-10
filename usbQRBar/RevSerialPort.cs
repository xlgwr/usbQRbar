using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace usbQRBar
{

    public class RevSerialPort
    {
        SerialPort serialPort;
        public StringBuilder sb;
        public RevSerialPort()
        {
            sb = new StringBuilder();
            serialPort = new SerialPort();
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
        }

        public RevSerialPort(string portName)
        {
            sb = new StringBuilder();
            serialPort = new SerialPort(portName);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
        }

        public RevSerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            sb = new StringBuilder();
            serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            sb.Append(serialPort.ReadExisting());
            int index = sb.ToString().IndexOf((char)0xd);//index = sb.ToString().Length;
            if (index > 0)
            {
                try
                {
                    System.Windows.Forms.SendKeys.SendWait(sb.ToString(0, index) + "~");
                }
                finally
                {
                    sb.Remove(0, index + 1);//sb.Remove(0, index); 
                }
                System.Windows.Forms.MessageBox.Show(sb.ToString());
            }
        }

        public void Start()
        {
            serialPort.Open();
        }

        public void Stop()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }
    }
}
