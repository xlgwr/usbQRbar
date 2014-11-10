using Microsoft.PointOfService;
using Microsoft.PointOfService.BaseServiceObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace usbQRBar
{
    //[HardwareId(@"HID\Vid_05e0&Pid_038a", @"HID\Vid_05e0&Pid_038a")]
    [HardwareId(@"HID\VID_0C2E&PID_0BE1", @"HID\VID_0C2E&PID_0BE1")]
    [ServiceObject(DeviceType.Msr,
        "SampleMsr",
        "Sample Msr Service Object",
        1,
        9)]
    public class SampleMsr : MsrBase
    {
        //  String returned from CheckHealth
        private string MyHealthText;

        public SampleMsr()
        {
            // Initialize device capability properties.
            Properties.CapIso = true;
            Properties.CapTransmitSentinels = true;
            Properties.DeviceDescription = "Sample MSR";

           
            // Initialize other class variables.
            MyHealthText = "";
        }

        ~SampleMsr()
        {
            Dispose(false);
        }

        // Release any resources managed by this object.
        protected override void Dispose(bool disposing)
        {
            try
            {
                // Your code here.
            }
            finally
            {
                // Must call base class Dispose.
                base.Dispose(disposing);
            }
        }

        #region PosCommon overrides
        // Returns the result of the last call to CheckHealth().
        public override string CheckHealthText
        {
            get
            {
                // MsrBasic.VerifyState(mustBeClaimed,
                // mustBeEnabled). This may throw an exception.
                VerifyState(false, false);

                return MyHealthText;
            }
        }

        public override string CheckHealth(
                    HealthCheckLevel level)
        {
            // Verify that device is open, claimed, and enabled.
            VerifyState(true, true);

            // Your code here:
            // check the health of the device and return a 
            // descriptive string.

            // Cache result in the CheckHealthText property.
            MyHealthText = "Ok";
            return MyHealthText;
        }

        public override DirectIOData DirectIO(
                        int command,
                        int data,
                        object obj)
        {
            // Verify that device is open.
            VerifyState(false, false);

            return new DirectIOData(data, obj);
        }
        #endregion // PosCommon overrides

        #region MsrBasic Overrides
        protected override MsrFieldData ParseMsrFieldData(
                        byte[] track1Data,
                        byte[] track2Data,
                        byte[] track3Data,
                        byte[] track4Data,
                        CardType cardType)
        {
            // Your code here:
            // Implement this method to parse track data
            // into fields which will be returned as
            // properties to the application
            // (for example, FirstName,
            // AccountNumber, etc.)
            return new MsrFieldData();
        }

        protected override MsrTrackData ParseMsrTrackData(
                        byte[] track1Data,
                        byte[] track2Data,
                        byte[] track3Data,
                        byte[] track4Data,
                        CardType cardType)
        {

            // Your code here:
            // Implement this method to convert raw track data.
            return new MsrTrackData();
        }
        #endregion
    }

}
