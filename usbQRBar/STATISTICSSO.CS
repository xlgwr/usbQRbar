// File STATISTICSSO.CS
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.PointOfService;
using Microsoft.PointOfService.BaseServiceObjects;

[assembly: PosAssembly("Service Object Contractors, Inc.")]

namespace usbQRBar
{
    [ServiceObject(DeviceType.Msr,
        "SampleStatistics",
        "Sample Statistics Service Object",
        1,
        9)]

    public class StatisticsTest : MsrBase
    {
        // This will be incremented every time DeviceEnabled
        // is set so that different sets of demonstration
        // statistics can be generated.
        int enableCount = 0;

        // The name of a custom created statistic used to demonstrate
        // custom Statistic handlers.
        private const string PollingStatistic = "Polling Interval";

        // Statistic used to demonstrate IncrementStatistic.
        private const string TestIncrement = "MyIncrementableStat";

        // String returned from CheckHealth
        private string MyHealthText;

        public StatisticsTest()
        {
            DevicePath = "Sample Statistics";
        }

        // This is the delegate which will be called for each
        // statistic that we have associated with this delegate by
        // calling SetStatisticHandler(). 
        //
        // Delegates like this should most commonly be used
        // to get and set statistics in hardware. The delegate
        // allows the POS for .NET statistic subsystem to query 
        // the value of a statistic in real time, before it is
        // returned to the application.
        private string GetHardwareInfo(string name)
        {
            // Add your code to query values from hardware here.

            // Very simple demonstration: just return the name
            // of the statistic as its value;
            return name;
        }

        // In a typical Service Object implementation, statistics
        // will be modified throughout the Service Object code. This
        // method demonstrates the methods used to modify statistic
        // values.
        public void SetDemonstrationStatistics()
        {
            // IncrementStatistic can be used to easily
            // increment a numeric statistic.
            IncrementStatistic(TestIncrement);

            switch (enableCount)
            {
                case 0:
                    SetStatisticValue(StatisticManufacturerName,
                        "Microsoft Corporation");
                    break;
                case 1:
                    SetStatisticValue(StatisticManufacturerName,
                        "Service Control Contractors, Inc.");
                    break;
                case 2:
                    SetStatisticValue(StatisticManufacturerName,
                        "Point of Service Controls .com");
                    break;
            }

            if (++enableCount == 3)
            {
                enableCount = 0;
            }
        }

        #region PosCommon overrides
        // Returns the result of the last call to CheckHealth()

        public override void Open()
        {
            base.Open();

            // In your implementation of Open(), your Service Object
            // code should:
            //    1. Initialize statistics.
            //    2. Create custom statistics.
            //    3. Set statistic handlers for hardware Statistics.

            // 1. Initialize statistics
            SetStatisticValue(StatisticManufacturerName,
                            "Microsoft Corporation");
            SetStatisticValue(StatisticManufactureDate,
                            "2004-10-23");
            SetStatisticValue(StatisticModelName,
                            "Statistic Sample");
            SetStatisticValue(StatisticMechanicalRevision,
                            "1.0");
            SetStatisticValue(StatisticInterface,
                            "USB");

            // 2a. Create a new new statistic to test Increment 
            // method. No custom handler needed.
            CreateStatistic(TestIncrement, false, "blobs");

            // 2b. Create a new manufacturer statistic to demonstrate
            // custom attributes with StatisticHandlers.
            CreateStatistic(PollingStatistic, false,
                            "milliseconds");

            // 3. Set handlers for statistics stored in hardware.
            SetStatisticHandlers(PollingStatistic,
                        new GetStatistic(GetHardwareInfo), null);
            SetStatisticHandlers(StatisticSerialNumber,
                        new GetStatistic(GetHardwareInfo), null);
        }

        public override bool DeviceEnabled
        {
            get
            {
                return base.DeviceEnabled;
            }
            set
            {
                // This method will set various statistics to
                // demonstrate the statistic APIs. We are going 
                // to change statistic values every time the device
                // is enabled. This operation is just for 
                // demonstration and would not be found in live code.
                SetDemonstrationStatistics();

                base.DeviceEnabled = value;
            }
        }

        public override string CheckHealthText
        {
            get
            {
                // MsrBasic.VerifyState(musBeClaimed,
                // mustBeEnabled). This may throw an exception
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
            // Verify that the device is open.
            VerifyState(false, false);

            return new DirectIOData(data, obj);
        }
        #endregion  PosCommon overrides

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
            // properties to the application (e.g.,FirstName,
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
        #endregion MsrBasic Overrides
    }
}