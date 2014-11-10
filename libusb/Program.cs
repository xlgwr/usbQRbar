using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace libusb
{
    class Program
    {

        public static UsbDevice MyUsbDevice;

        #region SET YOUR USB Vendor and Product ID!

        public static UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(0x0c2e, 0x0be2);

        #endregion

        /// <summary>Use the first read endpoint</summary>
        public static readonly byte TRANFER_ENDPOINT = UsbConstants.ENDPOINT_DIR_MASK ;//| (byte)ReadEndpointID.Ep04;

        /// <summary>Number of transfers to sumbit before waiting begins</summary>
        public static readonly int TRANFER_MAX_OUTSTANDING_IO = 3;

        /// <summary>Number of transfers before terminating the test</summary>
        public static readonly int TRANSFER_COUNT = 30;

        /// <summary>Size of each transfer</summary>
        public static int TRANFER_SIZE = 4096;

        private static DateTime mStartTime = DateTime.MinValue;
        private static double mTotalBytes = 0.0;
        private static int mTransferCount = 0;

        static void Main(string[] args)
        {
            var d1 = Convert.ToString(0x24, 10);
            var d2 = Convert.ToString(0x28, 10);
            var d3 = Convert.ToString(0x25, 10);
            var d4 = Convert.ToString(0x26, 10);
            string ansii = d1 + d2 + d3+ d4;
            List<byte> buffer = new List<byte>();
            for (byte i = 0; i < ansii.Length; i+=2)
            {
                var dd = ansii.Substring(i, 2);
                byte value = Convert.ToByte(dd,16);
                buffer.Add(value);
            }
            string str1 = System.Text.Encoding.ASCII.GetString(buffer.ToArray());
            //readpool();
            //var dd = Console.Read();
            Console.WriteLine(str1);
            Console.WriteLine(d1+","+d2+","+d3+","+d4);
            Console.Read();
            //getdevices();
            asusb();

        }
        public static void asusb()
        {
            ErrorCode ec = ErrorCode.None;

            try
            {
                // Find and open the usb device.
                UsbRegDeviceList regList = UsbDevice.AllDevices.FindAll(MyUsbFinder);
                if (regList.Count == 0) throw new Exception("Device Not Found.");

                UsbInterfaceInfo usbInterfaceInfo = null;
                UsbEndpointInfo usbEndpointInfo = null;

                // Look through all conected devices with this vid and pid until
                // one is found that has and and endpoint that matches TRANFER_ENDPOINT.
                // 
                foreach (UsbRegistry regDevice in regList)
                {
                    if (regDevice.Open(out MyUsbDevice))
                    {
                        if (MyUsbDevice.Configs.Count > 0)
                        {
                            // if TRANFER_ENDPOINT is 0x80 or 0x00, LookupEndpointInfo will return the 
                            // first read or write (respectively).
                            if (UsbEndpointBase.LookupEndpointInfo(MyUsbDevice.Configs[0], TRANFER_ENDPOINT,
                                out usbInterfaceInfo, out usbEndpointInfo))
                                break;

                            MyUsbDevice.Close();
                            MyUsbDevice = null;
                        }
                    }
                }

                // If the device is open and ready
                if (MyUsbDevice == null) throw new Exception("Device Not Found.");

                // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                // it exposes an IUsbDevice interface. If not (WinUSB) the 
                // 'wholeUsbDevice' variable will be null indicating this is 
                // an interface of a device; it does not require or support 
                // configuration and interface selection.
                IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    // This is a "whole" USB device. Before it can be used, 
                    // the desired configuration and interface must be selected.

                    // Select config #1
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface #0.
                    wholeUsbDevice.ClaimInterface(usbInterfaceInfo.Descriptor.InterfaceID);
                }

                // open read endpoint.
                UsbEndpointReader reader = MyUsbDevice.OpenEndpointReader(
                    (ReadEndpointID)usbEndpointInfo.Descriptor.EndpointID,
                    0,
                    (EndpointType)(usbEndpointInfo.Descriptor.Attributes & 0x3));

                if (ReferenceEquals(reader, null))
                {
                    throw new Exception("Failed locating read endpoint.");
                }

                reader.Reset();

                // The benchmark device firmware works with this example but it must be put into PC read mode.
#if IS_BENCHMARK_DEVICE
                int transferred;
                byte[] ctrlData=new byte[1];
                UsbSetupPacket setTestTypePacket = 
                    new UsbSetupPacket((byte) (UsbCtrlFlags.Direction_In | UsbCtrlFlags.Recipient_Device | UsbCtrlFlags.RequestType_Vendor),
                        0x0E,0x01,usbInterfaceInfo.Descriptor.InterfaceID,1);
                MyUsbDevice.ControlTransfer(ref setTestTypePacket,ctrlData, 1, out transferred);
#endif
                TRANFER_SIZE -= (TRANFER_SIZE % usbEndpointInfo.Descriptor.MaxPacketSize);

                UsbTransferQueue transferQeue = new UsbTransferQueue(reader,
                                                                     TRANFER_MAX_OUTSTANDING_IO,
                                                                     TRANFER_SIZE,
                                                                     5000,
                                                                     usbEndpointInfo.Descriptor.MaxPacketSize);

                do
                {
                    UsbTransferQueue.Handle handle;

                    // Begin submitting transfers until TRANFER_MAX_OUTSTANDING_IO has benn reached.
                    // then wait for the oldest outstanding transfer to complete.
                    // 
                    ec = transferQeue.Transfer(out handle);
                    if (ec != ErrorCode.Success)
                        throw new Exception("Failed getting async result");

                    // Show some information on the completed transfer.
                    showTransfer(handle, mTransferCount);
                } while (mTransferCount++ < TRANSFER_COUNT);

                // Cancels any oustanding transfers and free's the transfer queue handles.
                // NOTE: A transfer queue can be reused after it's freed.
                transferQeue.Free();

                Console.WriteLine("\r\nDone!\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
            }
            finally
            {
                if (MyUsbDevice != null)
                {
                    if (MyUsbDevice.IsOpen)
                    {
                        // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                        // it exposes an IUsbDevice interface. If not (WinUSB) the 
                        // 'wholeUsbDevice' variable will be null indicating this is 
                        // an interface of a device; it does not require or support 
                        // configuration and interface selection.
                        IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                        if (!ReferenceEquals(wholeUsbDevice, null))
                        {
                            // Release interface #0.
                            wholeUsbDevice.ReleaseInterface(0);
                        }

                        MyUsbDevice.Close();
                    }
                    MyUsbDevice = null;
                }

                // Wait for user input..
                Console.ReadKey();

                // Free usb resources
                UsbDevice.Exit();
            }
        }

        private static void showTransfer(UsbTransferQueue.Handle handle, int transferIndex)
        {
            if (mStartTime == DateTime.MinValue)
            {
                mStartTime = DateTime.Now;
                Console.WriteLine("Synchronizing..");
                return;
            }

            mTotalBytes += handle.Transferred;
            double bytesSec = mTotalBytes / (DateTime.Now - mStartTime).TotalSeconds;

            Console.WriteLine("#{0} complete. {1} bytes/sec ({2} bytes) Data[1]={3:X2}",
                              transferIndex,
                              Math.Round(bytesSec, 2),
                              handle.Transferred,
                              handle.Data[1]);
        }

        public static void readpool()
        {
            ErrorCode ec = ErrorCode.None;

            try
            {
                // Find and open the usb device.
                MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);

                // If the device is open and ready
                if (MyUsbDevice == null) throw new Exception("Device Not Found.");

                // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                // it exposes an IUsbDevice interface. If not (WinUSB) the 
                // 'wholeUsbDevice' variable will be null indicating this is 
                // an interface of a device; it does not require or support 
                // configuration and interface selection.
                IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    // This is a "whole" USB device. Before it can be used, 
                    // the desired configuration and interface must be selected.

                    // Select config #1
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface #0.
                    wholeUsbDevice.ClaimInterface(0);
                }

                // open read endpoint 1.Ep01
                UsbEndpointReader reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep04);


                byte[] readBuffer = new byte[1024];//1024
                while (ec == ErrorCode.None)
                {
                    int bytesRead;

                    // If the device hasn't sent data in the last 5 seconds,
                    // a timeout error (ec = IoTimedOut) will occur. 
                    ec = reader.Read(readBuffer, 5000, out bytesRead);

                    if (bytesRead == 0) throw new Exception(string.Format("{0}:No more bytes!", ec));
                    Console.WriteLine("{0} bytes read", bytesRead);

                    // Write that output to the console.
                    Console.Write(Encoding.Default.GetString(readBuffer, 0, bytesRead));
                }

                Console.WriteLine("\r\nDone!\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
            }
            finally
            {
                if (MyUsbDevice != null)
                {
                    if (MyUsbDevice.IsOpen)
                    {
                        // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                        // it exposes an IUsbDevice interface. If not (WinUSB) the 
                        // 'wholeUsbDevice' variable will be null indicating this is 
                        // an interface of a device; it does not require or support 
                        // configuration and interface selection.
                        IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                        if (!ReferenceEquals(wholeUsbDevice, null))
                        {
                            // Release interface #0.
                            wholeUsbDevice.ReleaseInterface(0);
                        }

                        MyUsbDevice.Close();
                    }
                    MyUsbDevice = null;

                    // Free usb resources
                    UsbDevice.Exit();

                }

                // Wait for user input..
                Console.ReadKey();
            }
        }
        private static void getdevices()
        {
            // Dump all devices and descriptor information to console output.
            UsbRegDeviceList allDevices = UsbDevice.AllDevices;
            foreach (UsbRegistry usbRegistry in allDevices)
            {
                if (usbRegistry.Open(out MyUsbDevice))
                {
                    Console.WriteLine(MyUsbDevice.Info.ToString());
                    for (int iConfig = 0; iConfig < MyUsbDevice.Configs.Count; iConfig++)
                    {
                        UsbConfigInfo configInfo = MyUsbDevice.Configs[iConfig];
                        Console.WriteLine(configInfo.ToString());

                        ReadOnlyCollection<UsbInterfaceInfo> interfaceList = configInfo.InterfaceInfoList;
                        for (int iInterface = 0; iInterface < interfaceList.Count; iInterface++)
                        {
                            UsbInterfaceInfo interfaceInfo = interfaceList[iInterface];
                            Console.WriteLine(interfaceInfo.ToString());

                            ReadOnlyCollection<UsbEndpointInfo> endpointList = interfaceInfo.EndpointInfoList;
                            for (int iEndpoint = 0; iEndpoint < endpointList.Count; iEndpoint++)
                            {
                                Console.WriteLine(endpointList[iEndpoint].ToString());
                            }
                        }
                    }
                }
            }


            // Free usb resources.
            // This is necessary for libusb-1.0 and Linux compatibility.
            UsbDevice.Exit();

            // Wait for user input..
            Console.ReadKey();
        }
    }
}
