using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code;
using System.Xml;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
namespace WHOperation
{
    class CaptureBarCode1 : System.Windows.Forms.Form
    {
        IntPtr deviceHandle;
        Int32 success;
        public IntPtr StartCodeReader()
        {
            IntPtr hardwareDetector = CodeUtil.NativeMethods.Code_CreateHardwareDetector(null);
            uint maxSize = 5000;
            StringBuilder hardwareXml = new StringBuilder((int)maxSize + 1);
            maxSize = CodeUtil.NativeMethods.Code_DetectHardwareXML(hardwareDetector, hardwareXml, maxSize, false);

            CodeUtil.NativeMethods.Code_DestroyHardwareDetector(hardwareDetector);

            List<string> devices = ParseHardwareList(hardwareXml.ToString());
            string deviceInfo = SelectHardwareDevice(devices, "Hid_Native", "");
            if (0 == deviceInfo.Length)
                return deviceHandle;

            deviceHandle = CodeUtil.NativeMethods.Code_CreateDevice(deviceInfo, deviceInfo.Length);

            StringBuilder buffer = new StringBuilder(1024);
            int info = 0;

            /* Upload CodeUtil Version String */
            CodeUtil.NativeMethods.Code_GetVersionString(buffer, buffer.Capacity);
            /* Upload Reader Info */
            info = CodeUtil.NativeMethods.Code_GetReaderInfo(deviceHandle, buffer, buffer.Capacity);
            /* Upload Communication Settings */
            info = CodeUtil.NativeMethods.Code_GetCommSettings(deviceHandle, buffer, buffer.Capacity);
            /* Upload Last Error */
            info = CodeUtil.NativeMethods.Code_GetLastError(deviceHandle);
            /* Upload Configuration */
            info = CodeUtil.NativeMethods.Code_GetConfiguration(deviceHandle, buffer, buffer.Capacity);
            /* Upload File List */
            info = CodeUtil.NativeMethods.Code_GetFileList(deviceHandle, "", 0, buffer, buffer.Capacity);
            /* Open a Terminal connection to the Reader */
            CodeUtil.OnNewDataCallback onNewDataCallback = new CodeUtil.OnNewDataCallback(NewData);
            success = CodeUtil.NativeMethods.Code_TerminalStart(deviceHandle, onNewDataCallback, true);
            if (0 == success)
            {
                Int32 err = CodeUtil.NativeMethods.Code_GetLastError(deviceHandle);
                CodeUtil.NativeMethods.Code_DestroyDevice(deviceHandle);
                return deviceHandle;
            };
            Console.WriteLine();
            Console.WriteLine("For the next 10 seconds, scan a bar code or Ctrl+C to exit");
            Thread.Sleep(10000);
            
            return deviceHandle;
        }
        public void StopCodeReader(IntPtr deviceHandleMain) {

            /* Close the Terminal connection to the Reader */
            success = CodeUtil.NativeMethods.Code_TerminalStop(deviceHandle);
            if (0 == success)
            {
                Int32 err = CodeUtil.NativeMethods.Code_GetLastError(deviceHandle);
                CodeUtil.NativeMethods.Code_DestroyDevice(deviceHandle);
                return;
            }

            CodeUtil.NativeMethods.Code_DestroyDevice(deviceHandle);
            //Console.Write("Press Key to end");
            //Console.ReadKey();
        }
        List<string> ParseHardwareList(string hardware)
        {
            List<string> devices = new List<string>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(hardware);
            XmlNodeList xmlDevices = doc.SelectNodes("/codedevices/device");

            foreach (XmlNode device in xmlDevices)
            {
                devices.Add(device.OuterXml);
            }

            return devices;
        } // End ParseHardwareList()
        string SelectHardwareDevice(List<string> devices, string type, string path)
        {
            foreach (string device in devices)
            {
                if (device.Contains(type))
                {
                    if (path.Length == 0 || device.Contains(path))
                    {
                        return device;
                    }
                }
            }
            return "";
        }
        string GetErrorText(int number)
        {
            switch (number)
            {
                case 0:
                    return "Success";
                case 1000:
                    return "CodeUtilErrorFatal";
                case 1001:
                    return "CodeUtilErrorNoDevice";
                case 1002:
                    return "CodeUtilErrorCommError";
                case 1003:
                    return "CodeUtilErrorFileInstall";
                case 1004:
                    return "CodeUtilErrorReboot";
                case 1005:
                    return "CodeUtilErrorNoFile";
                case 1006:
                    return "CodeUtilErrorInvalidLength";
                case 1007:
                    return "CodeUtilErrorUnsupportedFile";
                case 1008:
                    return "CodeUtilErrorNoTerminal";
                case 1009:
                    return "CodeUtilErrorInvalidCommand";
                case 1010:
                    return "CodeUtilErrorCanceled";
                default:
                    return "Not a CodeUtil error: " + number.ToString();
            }
        } // End GetErrorText()
        public Int32 NewData(IntPtr handle, IntPtr data, Int32 length)
        {Form1 xx;
        
            string dataString = Marshal.PtrToStringAnsi(data);
            int i;
            i = 100;
            //Console.WriteLine();
            Console.WriteLine("Data from Reader:");
            Console.WriteLine(dataString);
            //this.tfscanarea.Text += dataString;
            //MessageBox.Show(dataString);
            /* MethodInvoker action = delegate
            { xx.tfscanarea.Text += dataString; };
            xx.tfscanarea.BeginInvoke(action); */
            
            return 0;
        }
        
    }
}
