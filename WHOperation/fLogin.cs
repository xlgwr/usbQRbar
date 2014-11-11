using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Linq;
namespace WHOperation
{
    public partial class fLogin : Form
    {
        WebReference.Service MFGProService = new WebReference.Service();
        DataSet dsServiceRet = new DataSet("dsServiceRet");
        private MDIParent1 mdiParent1;

        public fLogin(MDIParent1 parent)
        {
            InitializeComponent();
            mdiParent1 = parent;
            getIP();
            setSystem();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fLogin_Load(object sender, EventArgs e)
        {
            cbsystem.SelectedIndex = 0;
            this.AcceptButton = button1;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            valUser();
        }
        void valUser() {
            int cRet;
            cRet = 0;
            cRet = getMFGProService();
            
            if (cRet == 0) {
                GlobalClass1.systemID = cbsystem.SelectedItem.ToString();
                GlobalClass1.userID = tfuserid.Text;
                mdiParent1.setupMenu("0");
                this.Close();
            } else {
                GlobalClass1.systemID = "";
                GlobalClass1.userID = "";
                if (cRet == 1) {
                    MessageBox.Show("Invalid User ID/Password");
                }
                mdiParent1.setupMenu("1");
            }
        }
        int getMFGProService() {
            int cRet;
            String cSerData,cQuery,cConnStr,cIP;
            MiscDLL1.dbClass mydbClass = new MiscDLL1.dbClass();
            cRet = 1;
            cIP = getIP();
            cConnStr = "Persist Security Info=False;User ID=appuser;pwd=application;Initial Catalog=dbWHOperation;Data Source=142.2.70.81;pooling=true";
            cQuery = "Insert into dbMiscService.dbo.UserLoginLog(SystemID,ADPath,ADUserName,ADPassWord,ClientIP) " +
           " values('WHOperation','','" + tfuserid.Text + "','','" + cIP + "')";
            try
            {
                cSerData = MFGProService.GetTable(cbsystem.Text, "wsas004", tfuserid.Text + "," + tfpassword.Text);

                dsServiceRet = new DataSet("dsServiceRet");
                byte[] byteArray = Encoding.ASCII.GetBytes(cSerData);
                MemoryStream stream2 = new MemoryStream(byteArray);
                StreamReader xx2 = new StreamReader(stream2);
                dsServiceRet.ReadXml(xx2);
                if (dsServiceRet.Tables.IndexOf("row") >= 0)
                {
                    if (dsServiceRet.Tables["row"].Rows.Count > 0)
                    {
                        cRet = 0;
                        mydbClass.UpdData(cConnStr, cQuery);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("MFG/Pro Service Error", "Message"); cRet = 2; }
            return cRet;
        }
        String getIP() {
            string cHost;
            string cIP;
            cHost = "";
            try {
                cHost = System.Net.Dns.GetHostName();
                cIP = System.Net.Dns.GetHostEntry(cHost).AddressList[0].ToString();
                /*Console.Write(myHost);
                Console.Write(myIP);
                System.Net.IPHostEntry cIPs = System.Net.Dns.GetHostEntry(cHost);
                foreach (System.Net.IPAddress cIPx in cIPs.AddressList)
                {
                    Console.Write(cIPx.ToString()+"\n");
                } */
            }
            catch (Exception) { }
            return cHost;
        }
        void setSystem() {
            String cSysID;
            cbsystem.Items.Clear();
            XmlDocument xmlDoc = new XmlDocument();
            try {
                xmlDoc.Load(Application.StartupPath +"\\SystemList.xml");
                foreach (XmlNode cHead in xmlDoc.SelectNodes("/Header/System")) {
                    foreach (XmlNode cSys in cHead) {
                        cSysID = cSys.InnerXml;
                        cbsystem.Items.Add(cSysID);
                    }
                }
            }
            catch (Exception ex) { cbsystem.Items.Add("WEC"); cbsystem.Items.Add("WELCO"); }
        }
    }
}