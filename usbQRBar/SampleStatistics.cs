using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using Microsoft.PointOfService;
using Microsoft.PointOfService.BaseServiceObjects;

namespace usbQRBar
{
    public partial class SampleStatistics : Form
    {
        // Indicates whether or not the Service Object has
        // been started.
        bool ServiceObjectStarted = false;

        // The Service Object.
        PosCommon so = null;

        public SampleStatistics()
        {
            InitializeComponent();

            // Disable the buttons until the SO is loaded successfully.
            UpdateControls();
        }

        public void UpdateControls()
        {
            btnGenerateStatistics.Enabled = ServiceObjectStarted;
            btnRetrieveStatistics.Enabled = ServiceObjectStarted ;
            txtStatisticName.Enabled = ServiceObjectStarted;

            // The statistic name text box is disabled until the
            // Service Object is loaded.
            if (ServiceObjectStarted)
            {
                btnStartSO.Text = "Close SO";
            }
            else
            {
                txtStatisticName.Clear();
                txtRetrievedStatistics.Clear();
                btnStartSO.Text = "Start SO";
            }

            // The retrieve one statistic button is disabled until
            // the user has entered a statistic name.
            if (txtStatisticName.TextLength > 0)
            {
                btnRetreiveStatistic.Enabled = true;
            }
            else
            {
                btnRetreiveStatistic.Enabled = false;
            }
        }

        private void StartServiceObject(object sender, EventArgs e)
        {
            PosExplorer explorer = new PosExplorer(this);
            string SOName = "SampleStatistics";

            if (ServiceObjectStarted)
            {
                so.DeviceEnabled = false;
                so.Close();
                so = null;
                ServiceObjectStarted = false;
                UpdateControls();
            }
            else
            {
                foreach (DeviceInfo d in explorer.GetDevices())
                {
                    if (d.ServiceObjectName == SOName)
                    {
                        try
                        {
                            // Standard Service Object startup.
                            so = explorer.CreateInstance(d) 
                                            as PosCommon;

                            so.Open();
                            so.Claim(0);
                            so.DeviceEnabled = true;

                            // Application-specific setup.
                            ServiceObjectStarted = true;
                            UpdateControls();
                        }
                        catch
                        {
                            // Something went wrong starting the SO
                            MessageBox.Show("The Service Object '" +
                                SOName + "' failed to load",
                                "Service Object Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            return;
                        }

                        break;
                    }
                }

                if (so == null)
                {
                    // No Service Object with the 
                    // specified name could be found.
                    MessageBox.Show("The Service Object '" +
                        SOName + "' could not be found",
                        "Service Object Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
        }

        private void GenerateStatistics(object sender, EventArgs e)
        {
            // In this example case, you use this 
            // property to tell the Service Object to change statistic
            // values.
            so.DeviceEnabled = true;

            // Report status.
            txtRetrievedStatistics.Text = "DeviceEnabled called to" +
                        " to modify statistic values.";
        }

        private void RetrieveStatistics(object sender, EventArgs e)
        {
            string statistics;
            bool IsXml = true;

            try
            {
                statistics = so.RetrieveStatistics();
            }
            catch
            {
                statistics = "No statistics found";
                IsXml = false;
            }

            DisplayStatistics(statistics, IsXml);
        }

        private void RetrieveOneStatistic(object sender, EventArgs e)
        {
            string statistics;
            bool IsXml = true;

            try
            {
                statistics = so.RetrieveStatistic(
                            txtStatisticName.Text);
            }
            catch
            {
                statistics = "Statistic not found: " + 
                            txtStatisticName.Text;

                IsXml = false;
            }

            DisplayStatistics(statistics, IsXml);
            txtStatisticName.Clear();
            btnRetreiveStatistic.Enabled = false;
        }

        private void StatisticSizeChanged(object sender, EventArgs e)
        {
            if (txtStatisticName.TextLength > 0)
            {
                btnRetreiveStatistic.Enabled = true;
            }
            else
            {
                btnRetreiveStatistic.Enabled = false;
            }
        }

        // When retrieving statistics, POS for .NET returns a block
        // of XML (as specified in the UPOS specification). This
        // method will make it look readable with white space 
        // and indenting and then display it in the text box.
        private void DisplayStatistics(string inputString, bool isXml)
        {
            string s = null;

            // In case of an exception, you do not have an XML 
            // string, so just display the error description. Otherwise,
            // load the XML string into an XmlDocument object and
            // make it look readable.
            if (!isXml)
            {
                s = inputString;
            }
            if(isXml)
            {
                // Create new XML document and load the statistics 
                // XML string.
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(inputString);

                // Create a XmlTextWriter using a MemoryStream and
                // tell it to indent the XML output (so that it is 
                // readable.)
                MemoryStream m = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(m, null);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;

                // Save the document to the memory stream using the
                // XmlWriter.
                doc.Save(writer);

                // The stream will be encoded as UTF8, so convert the
                // buffer into a string.
                UTF8Encoding u = new UTF8Encoding();
                s = u.GetString(m.GetBuffer());
            }

            // Write the string to the text box.
            txtRetrievedStatistics.Text = s;
        }
    }
}