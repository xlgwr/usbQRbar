using Microsoft.PointOfService;
using Microsoft.PointOfService.BaseServiceObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace usbQRBar
{
    public class MsrThreadingObject: ServiceObjectThreadHelper,IDisposable
    {
        // This is a helper class which will depend on
        // being able to call back into the actual Service
        // Object to pass along data. However, you cannot 
        // keep a strong reference to the Service Object,
        // since that will prevent proper disposal, which
        // may create a state in which all hardware resources
        // are not properly released by the SO. Therefore,
        // create a weak reference. From this reference,
        // you can get a temporary strong reference, which 
        // you act on and then release.
        WeakReference ServiceObjectReference;

        // The name of the Service Object.
        string ObjectName;

        public MsrThreadingObject(AdvancedSampleMsr so)
        {
            ObjectName = GetType().Name;
            ServiceObjectReference = new WeakReference(so);
        }

        ~MsrThreadingObject()
        {
            Dispose(true);
        }

        private bool IsDisposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                base.Dispose(disposing);
            }
        }

        public void Dispose()
        {
            Dispose(false);
        }

        #region Methods of ServiceObjectThreadHelper

        // This will be called during initialization.
        public override void ServiceObjectThreadOpen()
        {
            Logger.Info(ObjectName, "Msr Thread Open");
        }

        // This method will be called during shutdown.
        public override void ServiceObjectThreadClose()
        {
            Logger.Info(ObjectName, "Msr Thread Open");
        }

        public override void ServiceObjectThreadProcedure(
                            AutoResetEvent ThreadStopEvent)
        {
            // Convert a C# string into a sample byte array.
            UTF8Encoding encoder = new UTF8Encoding();

            // Convert sample data to a byte array.
            byte[] MsrData = encoder.GetBytes(
                        "This is MSR test data");

            Logger.Info(ObjectName, "Msr Thread Procedure Entered");

            while (true)
            {
                // When this method is called by the 
                // ServiceObjectThreadHelper, it is obligated to
                // exit when the event ThreadStopEvent has been
                // set.

                // Additionally, this method will also wait for
                // hardware events or for a time-out. That should
                // be done here.

                // This example waits for the event to be set
                // or times out after several seconds.

                if (ThreadStopEvent.WaitOne(2000, false))
                {
                    break;
                }

                Logger.Info(ObjectName, "Reader Thread cycling");

                // Try to get a strong reference to the Service
                // Object using the weak reference saved when
                // this helper object was created.
                AdvancedSampleMsr msr =
                    ServiceObjectReference.Target
                    as AdvancedSampleMsr;

                // If this fails, that means the Service 
                // Object has already been disposed of. Exit the
                // thread.
                if (msr == null)
                {
                    break;
                }

                // Using the strong reference, you can now make
                // calls back into the Service Object.
                msr.OnCardSwipe(MsrData);
                msr = null;
            }
        #endregion Methods of ServiceObjectThreadHelper
        }

        // Implementation of the Service Object class. This class
        // implements all the methods needed for an MSR Service
        // Object. 
        //
        // A Service Object which supports a Plug and Play device
        // should also have a HardwareId attribute here.
        //HID\VID_0C2E&PID_0BE1&REV_001:&MI_01
        [HardwareId(
                @"HID\VID_0C2E&PID_0BE1",
                @"HID\VID_0C2E&PID_0BE1")]

        [ServiceObject(
                DeviceType.Msr,
                "AdvancedSampleMsr",
                "Advanced Sample Msr Service Object",
                1,
                9)]
        public class AdvancedSampleMsr : MsrBase
        {
            // String returned for various health checks.
            private string MyHealthText;
            private const string PollingStatistic =
                            "Polling Interval";

            // Create a class with interface methods called from the
            // threading object.
            MsrThreadingObject ReadThread;
            public AdvancedSampleMsr()
            {
                // DevicePath must be set before Open() is called. 
                // In the case of Plug and Play hardware, the POS
                // for .NET Base class will set this value.
                DevicePath = "Sample Msr";

                Properties.CapIso = true;
                Properties.CapTransmitSentinels = true;

                Properties.DeviceDescription =
                            "Advanced Sample Msr";

                // Initialize the string to be returned from
                // CheckHealthText().
                MyHealthText = "";
            }

            ~AdvancedSampleMsr()
            {
                // Code added from previous sections to terminate
                // the read thread started by the thread-helper
                // object.
                ReadThread.CloseThread();

                Dispose(false);
            }

            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (disposing)
                    {
                        if (ReadThread != null)
                        {
                            ReadThread.Dispose();
                            ReadThread = null;
                        }
                    }
                }
                finally
                {
                    // Must call base class Dispose.
                    base.Dispose(disposing);
                }
            }

            #region Internal Members
            // This is a private method called from the task
            // interface when a data event occurs in the reader
            // thread. 
            internal void OnCardSwipe(byte[] CardData)
            {
                // Simple sample data.
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] track1Data = utf8.GetBytes(
                                "this is test track 1");
                byte[] track2Data = utf8.GetBytes(
                                "this is test track 2");

                // Call GoodRead(), UnreadableCard, or FailedRead
                // from here.
                GoodRead(
                        track1Data,
                        track2Data,
                        null,
                        null,
                        Microsoft.PointOfService.BaseServiceObjects.CardType.Iso);
            }
            #endregion Internal Members

            #region PosCommon overrides
            //  PosCommon.Open.
            public override void Open()
            {
                // Call base class Open.
                base.Open();

                // Initialize statistic values.

                // Set values for common statistics.
                SetStatisticValue(StatisticManufacturerName,
                                "Microsoft Corporation");
                SetStatisticValue(
                            StatisticManufactureDate, "2004-10-23");
                SetStatisticValue(
                            StatisticModelName, "Msr Simulator");
                SetStatisticValue(
                            StatisticMechanicalRevision, "1.0");
                SetStatisticValue(
                            StatisticInterface, "USB");

                // Create a new manufacturer statistic.
                CreateStatistic(
                            PollingStatistic,
                            false,
                            "milliseconds");

                // Set handlers for statistics stored in hardware.
                // Create a class with interface methods called 
                // from the threading object.
                ReadThread = new MsrThreadingObject(this);
            }

            // PosCommon.CheckHealthText.
            public override string CheckHealthText
            {
                get
                {
                    // MsrBasic.VerifyState(mustBeClaimed,
                    // mustBeEnabled).
                    VerifyState(false, false);
                    return MyHealthText;
                }
            }

            //  PosCommon.CheckHealth.
            public override string CheckHealth(
                            HealthCheckLevel
                            level)
            {
                // Verify that device is open, claimed, and enabled.
                VerifyState(true, true);

                // Your code here checks the health of the device and
                // returns a descriptive string.

                // Cache result in the CheckHealthText property.
                MyHealthText = "Ok";
                return MyHealthText;
            }

            //  PosCommon.DirectIO.
            public override DirectIOData DirectIO(
                                int command,
                                int data,
                                object obj)
            {
                return new DirectIOData(data, obj);
            }

            public override bool DeviceEnabled
            {
                get
                {
                    return base.DeviceEnabled;
                }
                set
                {
                    if (value != base.DeviceEnabled)
                    {
                        base.DeviceEnabled = value;

                        if (value == false)
                        {
                            // Stop the reader thread when the 
                            // device is disabled.
                            ReadThread.CloseThread();
                        }
                        else
                        {
                            try
                            {
                                // Enabling the device, start the 
                                // reader thread.
                                ReadThread.OpenThread();
                            }
                            catch (Exception e)
                            {
                                base.DeviceEnabled = false;

                                if (e is PosControlException)
                                    throw;

                                throw new PosControlException(
                                        "Unable to Enable Device",
                                        ErrorCode.Failure, e);
                            }
                        }
                    }
                }
            }
            #endregion PosCommon overrides.

            #region MsrBasic Overrides

            // MsrBasic.MsrFieldData
            // Once the track data is retrieved, this method is
            // called when the application accesses various data
            // properties in the MsrBasic class. For example,
            // FirstName and AccountNumber.
            protected override MsrFieldData ParseMsrFieldData(
                                    byte[] track1Data,
                                    byte[] track2Data,
                                    byte[] track3Data,
                                    byte[] track4Data,
                                    CardType cardType)
            {
                // MsrFieldData contains the data elements that
                // MsrBasic will return as properties to the
                // application, as they are requested.
                MsrFieldData data = new MsrFieldData();

                // Parse the raw track data and store in fields to 
                // be used by the app.
                data.FirstName = "FirstName";
                data.Surname = "LastName";
                data.Title = "Mr.";
                data.AccountNumber = "123412341234";

                return data;
            }

            // MsrBasic.MsrTrackData.
            protected override MsrTrackData ParseMsrTrackData(
                                    byte[] track1Data,
                                    byte[] track2Data,
                                    byte[] track3Data,
                                    byte[] track4Data,
                                    CardType cardType)
            {
                MsrTrackData data = new MsrTrackData();

                // Modify the track data as appropriate for your SO.
                // Remove the sentinal characters from the track data,
                // for example.
                data.Track1Data = (byte[])track1Data.Clone();
                data.Track2Data = (byte[])track2Data.Clone();

                return data;
            }
           
            #endregion MsrBasic overrides
        }

      
    }

}
