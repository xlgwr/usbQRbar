using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices; // Needed for Marshal functions
using Code;
using System.Threading;
using System.Xml;
using System.IO;
using System.Linq;


//using System.Runtime.InteropServices;
//using Microsoft.Win32.SafeHandles;
//batch
//batchj0b
namespace WHOperation
{
    public partial class Form1 : Form
    {
        WebReference.Service MFGProService = new WebReference.Service();
        DataSet dsDNDetail = new DataSet("dsDNDetail");
        String cConnStr = "Persist Security Info=False;User ID=appuser;pwd=application;Initial Catalog=dbWHOperation;Data Source=142.2.70.81;pooling=true";
        String cUserID, cLastLabel;
        List<String> lXML = new List<String>();
        List<byte[]> lVendorLabelImage = new List<byte[]>();
        List<vendorLabelDefinition> lVendorLabel = new List<vendorLabelDefinition>();
        String cTemplateType, c2DSeperator;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        Thread readerThread;
        CaptureBarCode bcReader = new CaptureBarCode();
        IntPtr CodeReaderhandle;
        int cDisable;
        DateTime cLastPrint;
        int cSearchEnable;
        //add new qr listen
        KeyBordHook kbh;
        public static string getStrQRcode = "";
        DateTime _dt;  //定义一个成员函数用于保存每次的时间点
        int _spanint = 30;
        string _strold = "";
        string _strnew = "";


        public struct cCaptureData
        {
            public String cDNPartumber;
            public String cMFGPart;
            public String cDateCode;
            public String cMfgDate;
            public String cExpiredate;
            public String cRecQty;
            public String cLotNumber;
            public Image cPMFGPart;
            public Image cPDateCode;
            public Image cPMfgDate;
            public Image cPExpiredate;
            public Image cPRecQty;
            public Image cPLotNumber;
            public Image cPDNPartNumber;
        };

        cCaptureData cBufferData;

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
        }

        protected override void OnLoad(EventArgs e)
        {
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            dataGridView3.SelectionChanged += new EventHandler(dataGridView3_SelectionChanged);
            dgDNNumber.SelectionChanged += new EventHandler(dgDNNumber_SelectionChanged);
            //this.tflotno.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownHandler);
            this.tfscanarea.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownHandlerscanArea);
            //add
            //this.txt0ListKeyMsg.KeyDown+=new System.Windows.Forms.KeyEventHandler(this.OnKeyDownHandlerscanArea);
            //this.pb1.MouseHover += new EventHandler (this.pb1_MouseOverHandle);
            //this.pb1.MouseLeave  += new EventHandler(this.pb1_MouseLeaveHandle);
            cbport.SelectedIndex = 0;
            cbprintertype.SelectedIndex = 0;
            cbsystem.Text = GlobalClass1.systemID;
            cUserID = GlobalClass1.userID;
            try
            {
                MFGProService.GetTable(cbsystem.Text, "wsas001", tfdnno.Text + "," + tfdndate.Text + "," + tftodndate.Text);
                //MFGProService.GetTable(cbsystem.Text, "wsas001", tfdnno.Text + "," + tfdndate.Text); 
            }
            catch (Exception ex) { }
            cTemplateType = ""; c2DSeperator = ""; cLastPrint = DateTime.Now;
            cBufferData = new cCaptureData();
            cSearchEnable = 0;
            tfdndate.Text = DateTime.Now.Date.ToString();
            tftodndate.Text = DateTime.Now.Date.ToString();
            base.OnLoad(e);
        }

        void dgDNNumber_SelectionChanged(object sender, EventArgs e)
        {
            handleDNChange();
            //getTemplate();
        }
        void handleDNChange()
        {

            DataGridViewRow cDGR = new DataGridViewRow();
            DataRow cR;
            DataTable dt = new DataTable();
            String cSelDNNo;
            int i = 0;
            Double cDNQty, cPrintQty;
            if (dsDNDetail.Tables.Count < 7)
                return;

            dt = (DataTable)dsDNDetail.Tables[6];
            cDGR = dgDNNumber.CurrentRow;
            cSelDNNo = cDGR.Cells["DNNumber"].Value.ToString();
            dataGridView1.Rows.Clear();

            while (i <= dsDNDetail.Tables[6].Rows.Count - 1)
            {
                cR = dsDNDetail.Tables[6].Rows[i];
                dsDNDetail.Tables[6].Rows[i]["RowID"] = i.ToString();
                if (cR.ItemArray[0].ToString().ToUpper() == cSelDNNo.ToUpper())
                {

                    cDNQty = Convert.ToDouble(cR.ItemArray[6].ToString());
                    cPrintQty = getCompleteQty(cR["t_dn"].ToString(), cR["t_po"].ToString(), cR["t_id"].ToString(), cR["t_rir"].ToString(), cR["t_deli_date"].ToString(), cR["t_supp"].ToString());
                    /*if (cR.ItemArray[20].ToString().Length == 0)
                        cPrintQty = 0;
                    else
                        cPrintQty = Convert.ToDouble(cR.ItemArray[20].ToString()); */

                    cR["PrintedQty"] = cPrintQty;
                    if (cDNQty > cPrintQty)
                        dataGridView1.Rows.Add(cR.ItemArray[0], cR.ItemArray[10], cR.ItemArray[7], cR["t_part"], cR["t_mfgr_part"], cR["t_rir"], cR.ItemArray[4], "", cR.ItemArray[6], cR.ItemArray[1], cR.ItemArray[5], cR.ItemArray[11], cR.ItemArray[12], cR.ItemArray[13], cR.ItemArray[14], cR.ItemArray[15], cR.ItemArray[16], cR.ItemArray[17], cR.ItemArray[18], cR.ItemArray[20], i.ToString());

                }
                i += 1;
            }
            setCompleteDN();
        }
        Double getCompleteQty(String cDNNo, String cPoNo, String cPoLine, String cRIRNo, String cDNDate, String cVendorID)
        {
            double cRet, cPQty;
            String cQuery, cTotQty;
            SqlDataReader myReader;
            cTotQty = "0";
            //cQuery = "select case when sum(LineQty) is null then 0 else sum(LineQty) end from PIMLDetail where DNNo='" + cDNNo + "' and PONo='" + cPoNo + "' and PoLine='" + cPoLine + "' and RIRNo='" + cRIRNo + "' and DNDate='" + cDNDate + "' and VendorID='" + cVendorID + "'";
            cQuery = "select case when sum(LineQty) is null then 0 else sum(LineQty) end from PIMLDetail where DNNo='" + cDNNo + "' and PONo='" + cPoNo + "' and RIRNo='" + cRIRNo + "' and DNDate='" + cDNDate + "' and VendorID='" + cVendorID + "'";
            try
            {
                using (SqlConnection conn = new SqlConnection(cConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(cQuery, conn);
                    myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        cTotQty = myReader.GetValue(0).ToString();
                        cPQty = Convert.ToDouble(cTotQty);
                        //cTotQty = (Convert.ToDouble(cTotQty) + cPQty).ToString();
                        cTotQty = (Convert.ToDouble(cPQty)).ToString();
                    }
                    myReader.Close();
                }
            }
            catch (Exception ex) { }
            cRet = Convert.ToDouble(cTotQty);
            return cRet;
        }
        void setCompleteDN()
        {
            String cQuery, cDNNo;
            SqlDataReader myReader;
            cDNNo = dgDNNumber.CurrentRow.Cells["DNNumber"].Value.ToString();
            cQuery = "select PartNumber,PONo,MFGPartNumber,'',RIRNo,DNQty,LineQty from PIMLDetail where DNNo='" + cDNNo + "' ";
            dgComplete.Rows.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(cConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(cQuery, conn);
                    myReader = cmd.ExecuteReader();
                    String[] cRec = new String[myReader.FieldCount];
                    int i;
                    while (myReader.Read())
                    {
                        i = 0;
                        for (i = 0; i <= myReader.FieldCount - 1; i += 1)
                        {
                            cRec[i] = myReader.GetValue(i).ToString();
                        }
                        dgComplete.Invoke(new Action(delegate() { dgComplete.Rows.Add(cRec); }));
                    }
                    myReader.Close();
                }
            }
            catch (Exception ex) { }

        }
        void setDSPrintedQty()
        {
            DataGridViewRow cR;
            String cPrintedQty, cCurrRow;
            Double dPrintedQty, dDNQty;
            int i;
            try
            {
                //cR = dataGridView1.CurrentRow;
                cR = dataGridView1.SelectedRows[0];
                cCurrRow = cR.Cells["RowID"].Value.ToString();
                i = Convert.ToInt32(cCurrRow);
                cPrintedQty = dsDNDetail.Tables[6].Rows[i]["PrintedQty"].ToString();

                if (cPrintedQty.Length == 0)
                    cPrintedQty = "0";
                dPrintedQty = 0;
                dPrintedQty = Convert.ToDouble(cPrintedQty) + Convert.ToDouble(tfrecqty.Text);

                dsDNDetail.Tables[6].Rows[i]["PrintedQty"] = dPrintedQty.ToString();

                dDNQty = Convert.ToDouble(cR.Cells["DNQty"].Value);
                cR.Cells["PrintedQty"].Value = dPrintedQty.ToString();
                if (dDNQty <= dPrintedQty)
                {
                    dataGridView1.Invoke(new Action(delegate() { dataGridView1.Rows.Remove(cR); }));
                }
            }
            catch (Exception ex) { }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (readerThread.IsAlive)
                {
                    StopCodeReader(CodeReaderhandle);
                    readerThread.Abort();
                }
            }
            catch (Exception) { }
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                completeTrans();
            }
        }
        private void OnKeyDownHandlerscanArea(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return || e.KeyValue == 13)
            {
                if (tfscanarea.Text.ToUpper() == "SAVE" || tfscanarea.Text.ToUpper() == "PRINT")
                    completeTrans();
                else
                {
                    ParseLabelData();
                    //GrabLabelData();
                }
            }
        }
        private void pb1_MouseOverHandle(object sender, EventArgs e)
        {
            groupBox4.Height = 510;
            groupBox4.Width = 510;
            pb1.Height = 505;
            pb1.Width = 505;
            dataGridView3.Visible = false;
            Point x = new Point();
            x.X = groupBox4.Location.X + 5;
            x.Y = groupBox4.Location.Y + 5;
            pb1.Location = x;
        }
        private void pb1_MouseLeaveHandle(object sender, EventArgs e)
        {
            groupBox4.Height = 270;
            groupBox4.Width = 345;
            pb1.Height = 105;
            pb1.Width = 165;
            dataGridView3.Visible = true;
            Point x = new Point();
            x.X = 165;
            x.Y = 20;
            pb1.Location = x;

        }
        int completeTrans()
        {
            String cLot;
            int cVal;
            //cLot = tflotno.Text;
            //cLot = cLot.Replace(Convert.ToChar(13).ToString(),"");
            //tflotno.Text = cLot;
            cVal = valData();
            if (cVal == 0)
            {
                updData();
                //tflotno.Text = "";
            }
            else
            {
                //MessageBox.Show("Data Validation failed");
            }
            return cVal;
        }
        private void bGetDNDetail_Click(object sender, EventArgs e)
        {
            /*
            For testing...
            String xmlData;
            lVendorLabel = new List<vendorLabelDefinition>();
            xmlData = "<Header><Field><Name>LOTNUMBER</Name><Prefix>&lt;LL&gt;</Prefix></Field> " +
                              "<Field><Name>RECQTY</Name><Prefix>LQ</Prefix></Field> " +
                              "<Field><Name>DATECODE</Name><Prefix>DC</Prefix></Field> " +
                              "<Field><Name>expiredate</Name><Prefix>ex</Prefix></Field> " +
                              "<type>Single</type>" +
                              "</Header>";
            setFields(lVendorLabel = parseTempXMLTest(xmlData));
            GrabLabelData(); */
            getMFGDNData();
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            resetForm(1);
            setPIMLData();
            //getTemplate();
            setMandField();
        }
        void setFields(List<vendorLabelDefinition> vendorLabel)
        {
            String cFieldName, cPrefix, cIndex;
            tflotno.Visible = false;
            tfmfgdate.Visible = false;
            tfexpiredate.Visible = false;
            tfdatecode.Visible = false;
            tfrecmfgrpart.Visible = false;
            tfdnpartnumber.Visible = false;

            llotnumber.Visible = false;
            lmfgdate.Visible = false;
            lexpiredate.Visible = false;
            ldatecode.Visible = false;
            lrecmfgpart.Visible = false;
            ldnpartnumber.Visible = false;
            lMRecPartNumber.Visible = false;
            pblotnumber.Visible = false;
            pbmfgdate.Visible = false;
            pbexpiredate.Visible = false;
            pbdatecode.Visible = false;
            pbrecmfgpart.Visible = false;
            pbdnpartnumber.Visible = false;

            int i = 0;
            while (i <= vendorLabel.Count - 1)
            {
                cFieldName = vendorLabel[i].cFieldName;
                cPrefix = vendorLabel[i].cPrefix;
                cIndex = vendorLabel[i].cIndex;
                if (cFieldName.ToUpper() == "LOTNUMBER")
                {
                    tflotno.Visible = true;
                    llotnumber.Visible = true;
                    if (cPrefix.Length > 0 || cIndex.Length > 0)
                        pblotnumber.Visible = true;
                    else
                        pblotnumber.Visible = false;
                }
                if (cFieldName.ToUpper() == "MFGDATE")
                {
                    tfmfgdate.Visible = true;
                    lmfgdate.Visible = true;
                    if (cPrefix.Length > 0 || cIndex.Length > 0)
                        pbmfgdate.Visible = true;
                    else
                        pbmfgdate.Visible = false;

                }
                if (cFieldName.ToUpper() == "EXPIREDATE")
                {
                    tfexpiredate.Visible = true;
                    lexpiredate.Visible = true;
                    if (cPrefix.Length > 0 || cIndex.Length > 0)
                        pbexpiredate.Visible = true;
                    else
                        pbexpiredate.Visible = false;
                }
                if (cFieldName.ToUpper() == "DATECODE")
                {
                    tfdatecode.Visible = true;
                    ldatecode.Visible = true;
                    if (cPrefix.Length > 0 || cIndex.Length > 0)
                        pbdatecode.Visible = true;
                    else
                        pbdatecode.Visible = false;
                }
                if (cFieldName.ToUpper() == "MFGRPART")
                {
                    tfrecmfgrpart.Visible = true;
                    lrecmfgpart.Visible = true;
                    if (cPrefix.Length > 0 || cIndex.Length > 0)
                        pbrecmfgpart.Visible = true;
                    else
                        pbrecmfgpart.Visible = false;
                }
                if (cFieldName.ToUpper() == "DNPARTNUMBER")
                {
                    tfdnpartnumber.Visible = true;
                    ldnpartnumber.Visible = true;
                    if (cPrefix.Length > 0 || cIndex.Length > 0)
                        pbdnpartnumber.Visible = true;
                    else
                        pbdnpartnumber.Visible = false;
                }
                i += 1;
            }
        }
        void ParseLabelData()
        {
            String cCompoundData, cSingleLabel;
            String[] cArrayData;
            int i;
            cCompoundData = tfscanarea.Text;
            cCompoundData = cCompoundData.Replace("\n", "");
            cCompoundData = cCompoundData.Replace("\r", "");
            if (cCompoundData.Length >= 3)
            {
                if (cCompoundData.Substring(0, 3) != "<|>")
                {
                    cCompoundData = "<|>" + cCompoundData;
                }
            }
            cArrayData = cCompoundData.Split(new string[] { "<|>" }, StringSplitOptions.None);
            if (cTemplateType.ToUpper() == "SINGLE")
            {
                i = 0;
                while (i <= cArrayData.Length - 1)
                {
                    cSingleLabel = cArrayData[i];
                    GrabLabelData(cSingleLabel);
                    i += 1;
                }
            }
            else if (cTemplateType.ToUpper() == "COMPOUND")
            {
                if (c2DSeperator.Length > 0)
                {
                    cArrayData = cArrayData[1].Split(new string[] { c2DSeperator }, StringSplitOptions.None);
                }
                Grab2DData(cArrayData);
            }
            else
            {
                i = 0;
                while (i <= cArrayData.Length - 1)
                {
                    cSingleLabel = cArrayData[i];
                    GrabGeneralData(cSingleLabel);
                    i += 1;
                }
            }
            tfscanarea.Invoke(new Action(delegate() { tfscanarea.Text = ""; }));
            tfscanarea.Invoke(new Action(delegate() { tfscanarea.Text = tfscanarea.Text.Replace("\n", ""); }));
            tfscanarea.Invoke(new Action(delegate() { tfscanarea.Text = tfscanarea.Text.Replace("\r", ""); }));
        }
        void GrabGeneralData(String cLabelData)
        {
            String cFieldName, cPrefix, cSeperator;
            int cIndex;
            Char cSplitter;
            String[] aPrefix;
            int i = 0;

            if (cLabelData.Length == 0)
                return;
            while (i <= lVendorLabel.Count - 1)
            {
                cFieldName = lVendorLabel[i].cFieldName;
                cPrefix = lVendorLabel[i].cPrefix;
                cSeperator = lVendorLabel[i].cSeperator;
                //if (cPrefix.Length == 0 || cPrefix.Length > cLabelData.Length) { i += 1; continue; }
                if (cPrefix.Length == 0) { i += 1; continue; }

                if (lVendorLabel[i].cIndex.Length > 0)
                    cIndex = Convert.ToInt32(lVendorLabel[i].cIndex);
                else
                    cIndex = 1;
                aPrefix = cPrefix.Split(';');
                int cLoopPrefix;
                cLoopPrefix = 0;
                while (cLoopPrefix <= aPrefix.Length - 1)
                {
                    String[] cTemp;
                    cPrefix = aPrefix[cLoopPrefix];
                    if (cPrefix.Length == 0 || cPrefix.Length > cLabelData.Length) { cLoopPrefix += 1; continue; }
                    if (cPrefix.ToUpper() == cLabelData.Substring(0, cPrefix.Length).ToUpper())
                    {
                        if (cPrefix.Length == 0) { cLoopPrefix += 1; continue; }
                        if (cFieldName.ToUpper() == "LOTNUMBER")
                        {
                            //tflotno.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length);
                            tflotno.Invoke(new Action(delegate() { tflotno.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length); }));
                            if (cSeperator.Length > 0)
                            {
                                if (cSeperator == "SPACE")
                                    cSplitter = ' ';
                                else
                                    cSplitter = cSeperator[0];
                                cTemp = tflotno.Text.Split(cSplitter);
                                if (cTemp.Length >= cIndex)
                                    //tflotno.Text = cTemp[cIndex-1];
                                    tflotno.Invoke(new Action(delegate() { tflotno.Text = cTemp[cIndex - 1]; }));
                            }
                            //tflotno.Text = tflotno.Text.Trim();
                            tflotno.Invoke(new Action(delegate() { tflotno.Text = tflotno.Text.Trim(); }));
                            pblotnumber.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                        }
                        else if (cFieldName.ToUpper() == "MFGDATE")
                        {
                            tfmfgdate.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length);
                            if (cSeperator.Length > 0)
                            {
                                if (cSeperator == "SPACE")
                                    cSplitter = ' ';
                                else
                                    cSplitter = cSeperator[0];
                                cTemp = tfmfgdate.Text.Split(cSplitter);
                                if (cTemp.Length >= cIndex)
                                    tfmfgdate.Text = cTemp[cIndex - 1];
                            }
                            tfmfgdate.Text = tfmfgdate.Text.Trim();
                            pbmfgdate.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                        }
                        else if (cFieldName.ToUpper() == "EXPIREDATE")
                        {
                            tfexpiredate.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length);
                            if (cSeperator.Length > 0)
                            {
                                if (cSeperator == "SPACE")
                                    cSplitter = ' ';
                                else
                                    cSplitter = cSeperator[0];
                                cTemp = tfexpiredate.Text.Split(cSplitter);
                                if (cTemp.Length >= cIndex)
                                    tfexpiredate.Text = cTemp[cIndex - 1];
                            }
                            tfexpiredate.Text = tfexpiredate.Text.Trim();
                            pbexpiredate.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                        }
                        else if (cFieldName.ToUpper() == "RECQTY")
                        {
                            //tfrecqty.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length);
                            tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length); }));
                            if (cSeperator.Length > 0)
                            {
                                if (cSeperator == "SPACE")
                                    cSplitter = ' ';
                                else
                                    cSplitter = cSeperator[0];
                                cTemp = tfrecqty.Text.Split(cSplitter);
                                if (cTemp.Length >= cIndex)
                                    //tfrecqty.Text = cTemp[cIndex-1];
                                    tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = cTemp[cIndex - 1]; }));
                            }
                            //tfrecqty.Text = tfrecqty.Text.Trim();
                            //tfrecqty.Text = tfrecqty.Text.Replace(",", "");
                            tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = tfrecqty.Text.Trim(); }));
                            tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = tfrecqty.Text.Replace(",", ""); }));
                            pbrecqty.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                        }
                        else if (cFieldName.ToUpper() == "DATECODE")
                        {
                            //tfdatecode.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length);
                            tfdatecode.Invoke(new Action(delegate() { tfdatecode.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length); }));
                            if (cSeperator.Length > 0)
                            {
                                if (cSeperator == "SPACE")
                                    cSplitter = ' ';
                                else
                                    cSplitter = cSeperator[0];
                                cTemp = tfdatecode.Text.Split(cSplitter);
                                if (cTemp.Length >= cIndex)
                                    //tfdatecode.Text = cTemp[cIndex-1];
                                    tfdatecode.Invoke(new Action(delegate() { tfdatecode.Text = cTemp[cIndex - 1]; }));
                            }
                            //tfdatecode.Text = tfdatecode.Text.Trim();
                            tfdatecode.Invoke(new Action(delegate() { tfdatecode.Text = tfdatecode.Text.Trim(); }));
                            pbdatecode.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                        }
                        else if (cFieldName.ToUpper() == "DNPARTNUMBER")
                        {
                            tfdnpartnumber.Invoke(new Action(delegate() { tfdnpartnumber.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length); }));
                            if (cSeperator.Length > 0)
                            {
                                if (cSeperator == "SPACE")
                                    cSplitter = ' ';
                                else
                                    cSplitter = cSeperator[0];
                                cTemp = tfdnpartnumber.Text.Split(cSplitter);
                                if (cTemp.Length >= cIndex)
                                    tfdnpartnumber.Invoke(new Action(delegate() { tfdnpartnumber.Text = cTemp[cIndex - 1]; }));
                            }
                            tfdnpartnumber.Invoke(new Action(delegate() { tfdnpartnumber.Text = tfdnpartnumber.Text.Trim(); }));
                            pbdnpartnumber.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                            if (cbSmartScan.Checked == true)
                            {
                                if (tfdnpartnumber.Text.Length > 0 && tfrecmfgrpart.Text.Length > 0 && cSearchEnable == 0)
                                {
                                    SearchDNPart();
                                }
                            }
                        }

                        else if (cFieldName.ToUpper() == "MFGRPART")
                        {
                            //tfrecmfgrpart.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length);
                            tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length); }));
                            if (cSeperator.Length > 0)
                            {
                                if (cSeperator == "SPACE")
                                    cSplitter = ' ';
                                else
                                    cSplitter = cSeperator[0];
                                cTemp = tfrecmfgrpart.Text.Split(cSplitter);
                                if (cTemp.Length >= cIndex)
                                    //tfrecmfgrpart.Text = cTemp[cIndex - 1];
                                    tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.Text = cTemp[cIndex - 1]; }));
                            }
                            //tfrecmfgrpart.Text = tfrecmfgrpart.Text.Trim();
                            if (cbtrimmfgpart.Checked)
                                tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.Text = tfrecmfgrpart.Text.Replace(" ", ""); tfrecmfgrpart.Text = tfrecmfgrpart.Text.Trim(); }));
                            else
                                tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.Text = tfrecmfgrpart.Text.Trim(); }));
                            pbrecmfgpart.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                            if (cbSmartScan.Checked == true)
                            {
                                if (cSearchEnable == 0)
                                {
                                    if (tfdnpartnumber.Visible)
                                    {
                                        if (tfdnpartnumber.Text.Length > 0 && tfrecmfgrpart.Text.Length > 0)
                                        {
                                            SearchDNPart();
                                        }
                                    }
                                    else
                                    {
                                        if (tfrecmfgrpart.Text.Length > 0)
                                        {
                                            tfdnpartnumber.Text = tfpartno.Text;
                                            SearchDNPart();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    cLoopPrefix += 1;
                }
                i += 1;
            }
            cLabelData = "";
            cLastLabel = "";
        }
        void Grab2DData(String[] c2DDataArray)
        {
            int i, cIndex, cCo;
            String cFieldName, cPrefix, cSeperator, cLabelData;
            cCo = 0;
            i = 0;
            while (i <= lVendorLabel.Count - 1)
            {
                if (lVendorLabel[i].cIndex.Length > 0)
                    cCo += 1;
                i += 1;
            }
            i = 0;
            if (c2DDataArray.Length < cCo)
                return;
            while (i <= lVendorLabel.Count - 1)
            {
                cFieldName = lVendorLabel[i].cFieldName;
                cPrefix = lVendorLabel[i].cPrefix;
                cSeperator = lVendorLabel[i].cSeperator;
                if (lVendorLabel[i].cIndex.Length > 0)
                    cIndex = Convert.ToInt32(lVendorLabel[i].cIndex);
                else
                    cIndex = 0;
                if (cIndex == 0)
                {
                    i += 1;
                    continue;
                }
                if (c2DDataArray.Length < cIndex)
                {
                    i += 1;
                    continue;
                }
                cLabelData = c2DDataArray[cIndex - 1];
                cLabelData = cLabelData.Trim();
                if (cPrefix.Length > 0 && cLabelData.Length > 0)
                    cLabelData = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length).ToUpper();

                if (cFieldName.ToUpper() == "LOTNUMBER")
                {

                    tflotno.Invoke(new Action(delegate() { tflotno.Text = cLabelData; }));
                    pblotnumber.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                }
                if (cFieldName.ToUpper() == "MFGDATE")
                {
                    tfmfgdate.Invoke(new Action(delegate() { tfmfgdate.Text = cLabelData; }));
                    pbmfgdate.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                }
                if (cFieldName.ToUpper() == "EXPIREDATE")
                {
                    tfexpiredate.Invoke(new Action(delegate() { tfexpiredate.Text = cLabelData; }));
                    pbexpiredate.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                }
                if (cFieldName.ToUpper() == "RECQTY")
                {
                    //tfrecqty.Text = cLabelData;
                    tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = cLabelData; }));
                    pbrecqty.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                    /*if (cSeperator.Length > 0)
                    {
                        if (cSeperator == "SPACE")
                            cSplitter = ' ';
                        else
                            cSplitter = cSeperator[0];
                        cTemp = tfrecqty.Text.Split(cSplitter);
                        if (cTemp.Length >= cIndex)
                            //tfrecqty.Text = cTemp[cIndex-1];
                            tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = cTemp[cIndex - 1]; }));
                    }
                    //tfrecqty.Text = tfrecqty.Text.Trim();
                    //tfrecqty.Text = tfrecqty.Text.Replace(",", "");
                    tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = tfrecqty.Text.Trim(); }));
                    tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = tfrecqty.Text.Replace(",", ""); }));
                    pbrecqty.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png"); */

                }
                if (cFieldName.ToUpper() == "DATECODE")
                {
                    tfdatecode.Invoke(new Action(delegate() { tfdatecode.Text = cLabelData; }));
                    pbdatecode.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                }
                if (cFieldName.ToUpper() == "DNPARTNUMBER")
                {
                    tfdnpartnumber.Invoke(new Action(delegate() { tfdnpartnumber.Text = cLabelData; }));
                    pbdnpartnumber.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                }
                if (cFieldName.ToUpper() == "MFGRPART")
                {
                    tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.Text = cLabelData; }));
                    pbrecmfgpart.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                }
                if (cbSmartScan.Checked == true)
                {
                    if (cSearchEnable == 0)
                    {
                        if (tfdnpartnumber.Visible)
                        {
                            if (tfdnpartnumber.Text.Length > 0 && tfrecmfgrpart.Text.Length > 0)
                            {
                                SearchDNPart();
                            }
                        }
                        else
                        {
                            if (tfrecmfgrpart.Text.Length > 0)
                            {
                                tfdnpartnumber.Text = tfpartno.Text;
                                SearchDNPart();
                            }
                        }
                    }
                }
                i += 1;
            }
            handleBeep();
        }
        void GrabLabelData(String cLabelData)
        {
            String cFieldName, cPrefix, cSeperator;
            int cIndex;
            Char cSplitter;
            int i = 0;
            if (cTemplateType.ToUpper() != "SINGLE")
            {
                MessageBox.Show("Only support 1D Barcode labels in this version");
                return;
            }
            if (cLabelData.Length == 0)
                return;
            while (i <= lVendorLabel.Count - 1)
            {
                cFieldName = lVendorLabel[i].cFieldName;
                cPrefix = lVendorLabel[i].cPrefix;
                cSeperator = lVendorLabel[i].cSeperator;
                //cPrefix = "<|>" + cPrefix;
                if (cPrefix.Length == 0 || cPrefix.Length > cLabelData.Length) { i += 1; continue; }

                if (lVendorLabel[i].cIndex.Length > 0)
                    cIndex = Convert.ToInt32(lVendorLabel[i].cIndex);
                else
                    cIndex = 1;
                String[] cTemp;
                if (cPrefix.ToUpper() == cLabelData.Substring(0, cPrefix.Length).ToUpper())
                {
                    if (cFieldName.ToUpper() == "LOTNUMBER")
                    {
                        //tflotno.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length);
                        tflotno.Invoke(new Action(delegate() { tflotno.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length); }));
                        if (cSeperator.Length > 0)
                        {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tflotno.Text.Split(cSplitter);
                            if (cTemp.Length >= cIndex)
                                //tflotno.Text = cTemp[cIndex-1];
                                tflotno.Invoke(new Action(delegate() { tflotno.Text = cTemp[cIndex - 1]; }));
                        }
                        //tflotno.Text = tflotno.Text.Trim();
                        tflotno.Invoke(new Action(delegate() { tflotno.Text = tflotno.Text.Trim(); }));
                        pblotnumber.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                    }
                    else if (cFieldName.ToUpper() == "MFGDATE")
                    {
                        tfmfgdate.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length);
                        if (cSeperator.Length > 0)
                        {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tfmfgdate.Text.Split(cSplitter);
                            if (cTemp.Length >= cIndex)
                                tfmfgdate.Text = cTemp[cIndex - 1];
                        }
                        tfmfgdate.Text = tfmfgdate.Text.Trim();
                        pbmfgdate.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                    }
                    else if (cFieldName.ToUpper() == "EXPIREDATE")
                    {
                        tfexpiredate.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length);
                        if (cSeperator.Length > 0)
                        {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tfexpiredate.Text.Split(cSplitter);
                            if (cTemp.Length >= cIndex)
                                tfexpiredate.Text = cTemp[cIndex - 1];
                        }
                        tfexpiredate.Text = tfexpiredate.Text.Trim();
                        pbexpiredate.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                    }
                    else if (cFieldName.ToUpper() == "RECQTY")
                    {
                        //tfrecqty.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length);
                        tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length); }));
                        if (cSeperator.Length > 0)
                        {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tfrecqty.Text.Split(cSplitter);
                            if (cTemp.Length >= cIndex)
                                //tfrecqty.Text = cTemp[cIndex-1];
                                tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = cTemp[cIndex - 1]; }));
                        }
                        //tfrecqty.Text = tfrecqty.Text.Trim();
                        //tfrecqty.Text = tfrecqty.Text.Replace(",", "");
                        tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = tfrecqty.Text.Trim(); }));
                        tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = tfrecqty.Text.Replace(",", ""); }));
                        pbrecqty.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                    }
                    else if (cFieldName.ToUpper() == "DATECODE")
                    {
                        //tfdatecode.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length);
                        tfdatecode.Invoke(new Action(delegate() { tfdatecode.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length); }));
                        if (cSeperator.Length > 0)
                        {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tfdatecode.Text.Split(cSplitter);
                            if (cTemp.Length >= cIndex)
                                //tfdatecode.Text = cTemp[cIndex-1];
                                tfdatecode.Invoke(new Action(delegate() { tfdatecode.Text = cTemp[cIndex - 1]; }));
                        }
                        //tfdatecode.Text = tfdatecode.Text.Trim();
                        tfdatecode.Invoke(new Action(delegate() { tfdatecode.Text = tfdatecode.Text.Trim(); }));
                        pbdatecode.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                    }
                    else if (cFieldName.ToUpper() == "DNPARTNUMBER")
                    {
                        tfdnpartnumber.Invoke(new Action(delegate() { tfdnpartnumber.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length); }));
                        if (cSeperator.Length > 0)
                        {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tfdnpartnumber.Text.Split(cSplitter);
                            if (cTemp.Length >= cIndex)
                                tfdnpartnumber.Invoke(new Action(delegate() { tfdnpartnumber.Text = cTemp[cIndex - 1]; }));
                        }
                        tfdnpartnumber.Invoke(new Action(delegate() { tfdnpartnumber.Text = tfdnpartnumber.Text.Trim(); }));
                        pbdnpartnumber.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                        if (cbSmartScan.Checked == true)
                        {
                            if (tfdnpartnumber.Text.Length > 0 && tfrecmfgrpart.Text.Length > 0 && cSearchEnable == 0)
                            {
                                SearchDNPart();
                            }
                        }
                    }

                    else if (cFieldName.ToUpper() == "MFGRPART")
                    {
                        //tfrecmfgrpart.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length);
                        tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.Text = cLabelData.Substring(cPrefix.Length, cLabelData.Length - cPrefix.Length); }));
                        if (cSeperator.Length > 0)
                        {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tfrecmfgrpart.Text.Split(cSplitter);
                            if (cTemp.Length >= cIndex)
                                //tfrecmfgrpart.Text = cTemp[cIndex - 1];
                                tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.Text = cTemp[cIndex - 1]; }));
                        }
                        //tfrecmfgrpart.Text = tfrecmfgrpart.Text.Trim();
                        if (cbtrimmfgpart.Checked)
                            tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.Text = tfrecmfgrpart.Text.Replace(" ", ""); tfrecmfgrpart.Text = tfrecmfgrpart.Text.Trim(); }));
                        else
                            tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.Text = tfrecmfgrpart.Text.Trim(); }));
                        pbrecmfgpart.Image = Image.FromFile(Application.StartupPath + @"\images\tick41.png");
                        if (cbSmartScan.Checked == true)
                        {
                            if (cSearchEnable == 0)
                            {
                                if (tfdnpartnumber.Visible)
                                {
                                    if (tfdnpartnumber.Text.Length > 0 && tfrecmfgrpart.Text.Length > 0)
                                    {
                                        SearchDNPart();
                                    }
                                }
                                else
                                {
                                    if (tfrecmfgrpart.Text.Length > 0)
                                    {
                                        tfdnpartnumber.Text = tfpartno.Text;
                                        SearchDNPart();
                                    }
                                }
                            }
                        }
                    }
                }
                i += 1;
            }
            cLabelData = "";
            cLastLabel = "";
            handleBeep();
        }

        void SearchDNPart()
        {
            var query = from DataGridViewRow row in dataGridView1.Rows
                        where row.Cells["PartNumber"].Value.ToString() == tfdnpartnumber.Text &&
                        row.Cells["MFGPartNo"].Value.ToString() == tfrecmfgrpart.Text
                        select row;
            int cSearchFound = 0;
            cBufferData.cDNPartumber = tfdnpartnumber.Text;
            cBufferData.cMFGPart = tfrecmfgrpart.Text;
            cBufferData.cDateCode = tfdatecode.Text;
            cBufferData.cRecQty = tfrecqty.Text;
            cBufferData.cLotNumber = tflotno.Text;
            cBufferData.cMfgDate = tfmfgdate.Text;
            cBufferData.cExpiredate = tfexpiredate.Text;

            cBufferData.cPMFGPart = pbrecmfgpart.Image;
            cBufferData.cPDateCode = pbdatecode.Image;
            cBufferData.cPRecQty = pbrecqty.Image;
            cBufferData.cPLotNumber = pblotnumber.Image;
            cBufferData.cPMfgDate = pbmfgdate.Image;
            cBufferData.cPExpiredate = pbexpiredate.Image;
            cBufferData.cPDNPartNumber = pbdnpartnumber.Image;
            foreach (DataGridViewRow onlineOrder in query)
            {
                onlineOrder.Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = onlineOrder.Index;
                cSearchFound = 1;
                break;
            }
            if (cSearchFound == 0 && tfdnpartnumber.Visible == true)
            {
                var query1 = from DataGridViewRow row in dataGridView1.Rows
                             where row.Cells["PartNumber"].Value.ToString().ToUpper() == tfdnpartnumber.Text.ToUpper()
                             select row;
                foreach (DataGridViewRow onlineOrder in query1)
                {
                    onlineOrder.Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = onlineOrder.Index;
                    cSearchFound = 1;
                    break;
                }
            }
            if (cSearchFound == 0 && tfdnpartnumber.Visible == false)
            {
                var query1 = from DataGridViewRow row in dataGridView1.Rows
                             where row.Cells["MFGPartNo"].Value.ToString().ToUpper() == tfrecmfgrpart.Text.ToUpper()
                             select row;
                foreach (DataGridViewRow onlineOrder in query1)
                {
                    onlineOrder.Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = onlineOrder.Index;
                    cSearchFound = 1;
                    break;
                }
            }
            tfdnpartnumber.Text = cBufferData.cDNPartumber;
            tfrecmfgrpart.Text = cBufferData.cMFGPart;
            tfdatecode.Text = cBufferData.cDateCode;
            tfrecqty.Text = cBufferData.cRecQty;
            tflotno.Text = cBufferData.cLotNumber;
            tfmfgdate.Text = cBufferData.cMfgDate;
            tfexpiredate.Text = cBufferData.cExpiredate;

            pbrecmfgpart.Image = cBufferData.cPMFGPart;
            pbdatecode.Image = cBufferData.cPDateCode;
            pbrecqty.Image = cBufferData.cPRecQty;
            pblotnumber.Image = cBufferData.cPLotNumber;
            pbmfgdate.Image = cBufferData.cPMfgDate;
            pbexpiredate.Image = cBufferData.cPExpiredate;
            pbdnpartnumber.Image = cBufferData.cPDNPartNumber;
            if (cSearchFound == 0)
            {
                tfdnpartnumber.Invoke(new Action(delegate() { tfdnpartnumber.Text = ""; }));
                tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.Text = ""; }));
                tfdatecode.Invoke(new Action(delegate() { tfdatecode.Text = ""; }));
                tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = "0"; }));
                tflotno.Invoke(new Action(delegate() { tflotno.Text = ""; }));
                tfmfgdate.Invoke(new Action(delegate() { tfmfgdate.Text = ""; }));
                tfexpiredate.Invoke(new Action(delegate() { tfexpiredate.Text = ""; }));

                pbrecmfgpart.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
                pbdnpartnumber.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
                pbdatecode.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
                pbrecqty.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
                pblotnumber.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
                pbmfgdate.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
                pbexpiredate.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
                MessageBox.Show("Can not find Part/Mfgr PartNumber");
            }
            else
            {
                cSearchEnable = 1;
            }

        }
        void handleBeep()
        {
            int cDone;
            cDone = 0;
            if (tfdnpartnumber.Visible)
                if (tfdnpartnumber.Text.Length == 0)
                    cDone += 1;
            if (pbrecmfgpart.Visible)
                if (tfrecmfgrpart.Text.Length == 0)
                    cDone += 1;

            if (pbdatecode.Visible)
                if (tfdatecode.Text.Length == 0)
                    cDone += 1;

            if (pbmfgdate.Visible)
                if (tfmfgdate.Text.Length == 0)
                    cDone += 1;

            if (pbexpiredate.Visible)
                if (tfexpiredate.Text.Length == 0)
                    cDone += 1;

            if (pbrecqty.Visible)
                if (tfrecqty.Text.Length == 0)
                    cDone += 1;

            if (pblotnumber.Visible)
                if (tflotno.Text.Length == 0)
                    cDone += 1;

            String myComm;
            if (cDone == 0)
            {
                if (bStart.Enabled == false)
                {
                    myComm = "P%2650";
                    CodeUtil.NativeMethods.Code_TerminalSendCommand(deviceHandle, myComm, myComm.Length);
                    myComm = "#%01";
                    CodeUtil.NativeMethods.Code_TerminalSendCommand(deviceHandle, myComm, myComm.Length);
                    myComm = "P%260";
                    CodeUtil.NativeMethods.Code_TerminalSendCommand(deviceHandle, myComm, myComm.Length);
                    //captureImage(); //need bug fix
                }
                cLastPrint = DateTime.Now;
                handleAutoPrint();
                cSearchEnable = 0;
            }
        }
        void handleAutoPrint()
        {
            int cCompVal;
            if (cbAutoPrint.Checked == true)
            {
                cCompVal = completeTrans();
                resetForm(0);
            }
        }

        void captureImage()
        { //need bug fix
            byte[] bytes = new byte[1024];
            IntPtr cImage;
            Int32 cImageSize;
            cImageSize = 1024;
            cImage = new IntPtr();

            CodeUtil.OnProgressCallback OnProgresscallback = new CodeUtil.OnProgressCallback(UploadProgress);
            Int32 success = CodeUtil.NativeMethods.Code_UploadImage(deviceHandle, cImage, ref cImageSize, 0, 0, OnProgresscallback);
            if (0 == success)
            {
                Int32 err = CodeUtil.NativeMethods.Code_GetLastError(deviceHandle); //returning error 1002
                return;
            };
            Marshal.PtrToStructure(cImage, bytes);
            Image myImage = getImage(bytes); //internal lib
            //Marshal.Copy(cImage, bytes, 0, bytes.Length);                
            //myImage = Image.FromHbitmap(cImage);

            //pb1.Image = myImage;
            //myImage.Save("c:\\tmp\\myreader.bmp");

        }
        static private Int32 UploadProgress(IntPtr handle, int progress)
        {
            Console.WriteLine("{0}", progress);
            return 0;
        }
        void resetForm(int cFlag)
        {

            tflotno.Text = "";
            tfrecqty.Text = "";
            tfmfgdate.Text = "";
            tfexpiredate.Text = "";
            tfdatecode.Text = "";
            tfrecmfgrpart.Text = "";
            tfdnpartnumber.Text = "";

            tfrecmfgrpart.BackColor = Color.White;
            tfrecqty.BackColor = Color.White;
            tfcumqty.BackColor = Color.White;
            tfmfgpart.BackColor = Color.White;
            tfdatecode.BackColor = Color.White;
            tfexpiredate.BackColor = Color.White;
            tfmfgdate.BackColor = Color.White;
            tflotno.BackColor = Color.White;

            pbdnpartnumber.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
            pbrecmfgpart.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
            pbdatecode.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
            pbrecqty.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
            pbexpiredate.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
            pbmfgdate.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");
            pblotnumber.Image = Image.FromFile(Application.StartupPath + @"\images\bdelete.jpg");

            if (cFlag == 1)
            {
                //tfmfgpart.Text = "";
                //tfvendor.Text = "";
                //tfpartno.Text = "";
                //tfrirno.Text = "";
                //tfcumqty.Text = "";
                //tfdnqty.Text = "";
            }
        }
        void setPIMLData()
        {
            String cSelDNNo, cSelPONo, cSelPOLine, cSelDNDate, cSelVendor;
            //SqlDataReader myReader;
            String[] cRec = new String[14];
            DataGridViewRow cR = new DataGridViewRow();
            Double cCumQty;
            int i;
            cCumQty = 0;
            //cR = dataGridView1.CurrentRow;
            if (dataGridView1.SelectedRows.Count <= 0)
            {
                cSelDNNo = "";
                cSelDNDate = "";
                cSelPOLine = "";
                cSelVendor = "";
                cSelPONo = "";
                tfdnqty.Text = "";
                tfsite.Text = "";
                tfhdndate.Invoke(new Action(delegate() { tfhdndate.Text = cSelDNDate; }));
                tfvendor.Invoke(new Action(delegate() { tfvendor.Text = cSelVendor; }));
                tfpartno.Invoke(new Action(delegate() { tfpartno.Text = ""; }));
                tfrirno.Invoke(new Action(delegate() { tfrirno.Text = ""; }));
                tfmfgpart.Invoke(new Action(delegate() { tfmfgpart.Text = ""; }));
            }
            else
            {
                cR = dataGridView1.SelectedRows[0];
                cSelDNNo = cR.Cells["DNNo"].Value.ToString();
                cSelDNDate = cR.Cells["DNDate"].Value.ToString();
                cSelPOLine = cR.Cells["POLine"].Value.ToString();
                cSelVendor = cR.Cells["Vendor"].Value.ToString();
                cSelPONo = cR.Cells["PONumber"].Value.ToString();
                tfdnqty.Text = cR.Cells["DNQty"].Value.ToString();
                tfsite.Text = cR.Cells["DNSite"].Value.ToString();
                //tfhdnno.Text = cSelDNNo;
                //tfhvendor.Text = cSelVendor;
                tfhdndate.Invoke(new Action(delegate() { tfhdndate.Text = cSelDNDate; }));
                tfvendor.Invoke(new Action(delegate() { tfvendor.Text = cSelVendor; }));
                tfpartno.Invoke(new Action(delegate() { tfpartno.Text = cR.Cells["PartNumber"].Value.ToString(); }));
                tfrirno.Invoke(new Action(delegate() { tfrirno.Text = cR.Cells["RIRNo"].Value.ToString(); }));
                tfmfgpart.Invoke(new Action(delegate() { tfmfgpart.Text = cR.Cells["MFGPartNo"].Value.ToString(); }));
            }
        }
        void getTemplate()
        {
            String cQuery, cSelVendor;
            SqlDataReader myReader;
            String cRec;
            DataGridViewRow cR = new DataGridViewRow();
            String cXMLTemplate;
            byte[] cImageData;
            lXML = new List<String>();
            lVendorLabelImage = new List<byte[]>();
            //cR = dataGridView1.CurrentRow;

            if (dataGridView1.SelectedRows.Count <= 0)
            {
                cSelVendor = "";
            }
            else
            {
                cR = dataGridView1.SelectedRows[0];
                cSelVendor = cR.Cells[2].Value.ToString();
            }
            cSelVendor = tfdnno.Text;
            cQuery = "select TemplateID,xmlVendorData,templateImage from PIMLVendorTemplate where VendorID='" + cSelVendor + "' Order By isDefault desc,TemplateID ";
            //cQuery = "select TemplateID,xmlVendorData from PIMLVendorTemplate where VendorID='" + cSelVendor + "' Order By isDefault desc,TemplateID ";
            dataGridView3.Rows.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(cConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(cQuery, conn);
                    myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        cRec = myReader.GetValue(0).ToString();
                        cXMLTemplate = myReader.GetValue(1).ToString();
                        if (cXMLTemplate.Length > 0)
                        {
                            dataGridView3.Rows.Add(cRec);
                            lXML.Add(cXMLTemplate);
                            cImageData = new byte[0];
                            try
                            {
                                cImageData = (byte[])myReader[2];
                            }
                            catch (Exception) { cImageData = new byte[0]; }
                            lVendorLabelImage.Add(cImageData);
                        }
                    }
                    myReader.Close();
                    setDataFieldLabel();
                }
            }
            catch (Exception) { }
            finally { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*if (cbSmartScan.Checked == true)
            {
                if (tfdnpartnumber.Text.Length > 0 && tfrecmfgrpart.Text.Length > 0 && cSearchEnable == 0)
                {

                    SearchDNPart();
                }
            }*/
            int cVal;
            lStatus.Invoke(new Action(delegate() { lStatus.Text = "Processing..."; }));
            cVal = valData();
            if (cVal == 0)
            {
                updData();
                if (tfrecmfgrpart.Text.Length > 0)
                {
                    if (tfrecmfgrpart.Text.ToUpper() != tfmfgpart.Text.ToUpper())
                    {
                        //MessageBox.Show("PO QPL Part & Received QPL Part mismatch");
                    }
                }
            }
            else
            {
                //MessageBox.Show("Data Validation failed");
            }
            lStatus.Invoke(new Action(delegate() { lStatus.Text = ""; }));
        }
        String getPIMSData()
        {
            String cRet;
            DataRow cR;
            DataSet pimlData;
            StreamReader cRetReader;
            cRet = "";
            pimlData = new DataSet("pimlData");
            cRetReader = callMFGService(cbsystem.Text, "wsas003", cbsystem.Text);
            try
            {
                pimlData.ReadXml(cRetReader);
                if (pimlData.Tables.IndexOf("row") >= 0)
                {
                    if (pimlData.Tables["row"].Rows.Count > 0)
                    {
                        cR = pimlData.Tables["Row"].Rows[0];
                        cRet = cR.ItemArray[0].ToString();
                    }
                }
            }
            catch (Exception serEx) { MessageBox.Show("PIMS Label Service Error:\n" + serEx.Message.ToString(), "System Message"); }
            return cRet;
        }
        void updData()
        {
            String cQuery, cPIMSNumber, cTotQty, cDNNo, cCartonQty;
            DataGridViewRow cR = new DataGridViewRow();
            DataGridViewRow cR1 = new DataGridViewRow();
            List<String> lPIMSData = new List<String>();
            int cCartonLoop, cNoOfCartons;
            int i;
            Double cPIMSQty;
            //cR = dataGridView1.CurrentRow;

            if (dataGridView1.SelectedRows.Count <= 0)
                return;

            cR = dataGridView1.SelectedRows[0];
            String[] cRec = new String[cR.Cells.Count];
            for (i = 0; i <= cR.Cells.Count - 1; i += 1)
            {
                cRec[i] = cR.Cells[i].Value.ToString();
            }

            cPIMSNumber = "tmpPIMS";
            cPIMSNumber = getPIMSData();
            cTotQty = (Convert.ToDouble(tfrecqty.Text) * Convert.ToDouble(tfnooflabels.Text)).ToString();

            try
            {
                int cPrintLoop;
                int cNoOfLabels;
                cPrintLoop = 1;
                cNoOfLabels = Convert.ToInt32(tfnooflabels.Text);
                while (cPrintLoop <= cNoOfLabels)
                {
                    if (cbprintcartonlabel.Checked == true && cPrintLoop == 1)
                    {
                        cCartonLoop = 1;
                        cNoOfCartons = Convert.ToInt32(tfnoofcartons.Text);
                        while (cCartonLoop <= cNoOfCartons)
                        {
                            cPIMSNumber = getPIMSData();
                            lPIMSData = updateMFGPro(cPIMSNumber);
                            if (lPIMSData[0].ToString() == "-2") { }
                            else
                            {
                                cCartonQty = "0";
                                cPIMSQty = (Convert.ToDouble(tfrecqty.Text) * Convert.ToDouble(tfnooflabels.Text)) / cNoOfCartons;
                                try
                                {
                                    if (Convert.ToDouble(cCartonQty) > 0)
                                        lPIMSData[7] = cCartonQty;
                                    else
                                        //lPIMSData[7] = (Convert.ToDouble(tfrecqty.Text) * Convert.ToDouble(tfnooflabels.Text)).ToString();
                                        lPIMSData[7] = cPIMSQty.ToString();
                                }
                                catch (Exception ex) { lPIMSData[7] = "0"; }
                                printPIML(lPIMSData, 1);
                            }
                            cCartonLoop += 1;
                        }
                    }
                    cPIMSNumber = getPIMSData();
                    lPIMSData = updateMFGPro(cPIMSNumber);

                    if (lPIMSData.Count > 0)
                    {
                        if (lPIMSData[0].ToString() == "-2") { MessageBox.Show("Must Input Date Code or Lot No"); }
                        else
                        {
                            Double cPrintQty;
                            cDNNo = dgDNNumber.CurrentRow.Cells[0].Value.ToString();
                            cPrintQty = getCompleteQty(cDNNo, cRec[6], cRec[1], tfrirno.Text, cRec[9], cRec[2]);
                            if (cPrintQty == 0 && cPrintLoop == 1)
                            {
                                cQuery = "Insert into PIMLDetail (SystemID,TransID,TransLine,DNNo,DNDate,VendorID,PONo,POLine,PartNumber,DNQty,LineQty,LotNo,RIRNo,MFGPartNumber,ExpDate,DateCode, " +
                                        " t_site,t_urg,t_loc,t_msd,t_cust_part,t_shelf_life,t_wt,t_wt_ind,t_conn,mfgDate,PIMSNumber,NoOfLabels) " +
                                        " values('" + cbsystem.Text + "','001','001','" + cDNNo + "','" + cRec[9] + "','" + cRec[2] + "','" + cRec[6] + "','" + cRec[1] + "','" + cRec[3] + "','" + cRec[8] + "','" + tfrecqty.Text + "','" + tflotno.Text + "','" + tfrirno.Text + "','" + tfmfgpart.Text + "','" + tfexpiredate.Text + "','" + tfdatecode.Text + "', " +
                                        " '" + cRec[10] + "','" + cRec[11] + "','" + cRec[12] + "','" + cRec[13] + "','" + cRec[14] + "','" + cRec[15] + "','" + cRec[16] + "','" + cRec[17] + "','" + cRec[18] + "','" + tfmfgdate.Text + "','" + cPIMSNumber + ";','1') ";
                            }
                            else
                            {
                                //cPrintQty = Convert.ToDouble(tfrecqty.Text) * Convert.ToDouble(tfnooflabels.Text);
                                cQuery = "update PIMLDetail set LineQty=LineQty + '" + tfrecqty.Text + "',NoOfLabels=NoofLabels+1,PIMSNumber=PIMSNumber+'" + cPIMSNumber + ";' where DNNo='" + cDNNo + "' and PONo='" + cRec[6] + "' and PoLine='" + cRec[1] + "' and RIRNo='" + tfrirno.Text + "' and DNDate='" + cRec[9] + "' and VendorID='" + cRec[2] + "'";
                            }
                            SQLUpdate(cQuery);
                            if (lPIMSData[5].ToUpper().Contains("MRB"))
                            {
                                cQuery = "insert into PIMSMRBException (DNNo,DNDate,RIRNo,SupplierID,MfgrID,MG,PIMS,PartNumber,ReqMfgrPart,RecMfgrPart,CustPart,RecQty) " +
                                    "values('" + cDNNo + "','" + cRec[9] + "','" + tfrirno.Text + "','" + cRec[2] + "','" + lPIMSData[6] + "','" + cRec[10] + "','" + cPIMSNumber + "','" + cRec[3] + "','" + tfmfgpart.Text + "','" + tfrecmfgrpart.Text + "','" + cRec[14] + "','" + tfrecqty.Text + "')";
                                SQLUpdate(cQuery);
                            }
                            setCompleteDN();
                            printPIML(lPIMSData, 0);
                        }
                    }
                    cPrintLoop += 1;
                }

                setPIMLData();
            }
            catch (Exception ex) { }
            finally { }
        }
        void SQLUpdate(String cQuery)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(cConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(cQuery, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { MessageBox.Show("SQL Error:" + ex.Message.ToString()); }
            finally { }
        }
        List<String> updateMFGPro(String cPIMSNumber)
        {
            int i;
            String cServiceID, cLocalSysID;
            StringBuilder cPara = new StringBuilder();
            StreamReader cRetReader;
            DataSet pimsData;
            DataGridViewRow cR = new DataGridViewRow();
            DataRow cDR;
            List<String> lPIMSData = new List<String>();
            //cR = dataGridView1.CurrentRow;
            cR = dataGridView1.SelectedRows[0];
            cServiceID = "wsas002";
            pimsData = new DataSet("pimlData");
            cLocalSysID = cbsystem.Text;
            /*cPara.Append(cR.Cells["DNSite"].Value.ToString()+","+cR.Cells["PartNumber"].Value.ToString()+
                "," + cR.Cells["RIRNo"].Value.ToString() + ",'',''," + tfrecqty.Text + "," + tfmfgpart.Text + "," + cUserID + "," + tflotno.Text + ",''," +
                tfexpiredate.Text+",'',"+cR.Cells["t_shelf_life"].Value.ToString()+",'YES','NO','R'");*/
            cPara.Append(cPIMSNumber + "," + cR.Cells["RIRNo"].Value.ToString() + "," + tfdatecode.Text + "," + tfmfgdate.Text + "," + tfexpiredate.Text + "," + tfrecqty.Text + "," + cUserID + "," + tflotno.Text + "," + tfrecmfgrpart.Text);
            cRetReader = callMFGService(cLocalSysID, cServiceID, cPara.ToString());
            try
            {
                pimsData.ReadXml(cRetReader);
                if (pimsData.Tables["Row"].Rows.Count > 0)
                {
                    cDR = pimsData.Tables["Row"].Rows[0];
                    i = 0;
                    while (i <= cDR.ItemArray.Length - 1)
                    {
                        lPIMSData.Add(cDR.ItemArray[i].ToString());
                        i += 1;
                    }
                }
                else
                {

                }
            }
            catch (Exception serEx) { MessageBox.Show("PIMS Label Data MFGPro Service Error:\n" + serEx.Message.ToString(), "System Message"); }

            return lPIMSData;
        }
        String getLastRec()
        {
            String cQuery, cRet;
            SqlDataReader myReader;
            cQuery = "select top 1 TransID from PIMLDetail Order by TransID desc";
            cRet = "00000000";
            try
            {
                using (SqlConnection conn = new SqlConnection(cConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(cQuery, conn);
                    myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        cRet = myReader.GetValue(0).ToString();
                    }
                    myReader.Close();
                    if (cRet.Length > 0)
                        cRet = (Convert.ToInt32(cRet) + 1).ToString("00000000");
                    else
                        cRet = "00000001";
                }
            }
            catch (Exception) { }
            finally { }
            return cRet;
        }
        String getLastLine(String cTransID)
        {
            String cQuery, cRet;
            SqlDataReader myReader;
            cQuery = "select top 1 TransLine from PIMLDetail where TransID='" + cTransID + "' Order by TransLine desc";
            cRet = "000";
            try
            {
                using (SqlConnection conn = new SqlConnection(cConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(cQuery, conn);
                    myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        cRet = myReader.GetValue(0).ToString();
                    }
                    myReader.Close();
                    if (cRet.Length > 0)
                        cRet = (Convert.ToInt32(cRet) + 1).ToString("000");
                    else
                        cRet = "001";
                }
            }
            catch (Exception) { }
            finally { }
            return cRet;
        }
        void removePrefix()
        {
            String cPX, cFN, cFieldVal;
            var xx = from x1 in lVendorLabel select new { x1.cFieldName, x1.cPrefix };
            foreach (var tt in xx)
            {
                cFN = tt.cFieldName;
                cPX = tt.cPrefix.ToUpper();

                if (cFN.ToUpper() == "LOTNUMBER")
                {
                    cFieldVal = tflotno.Text.ToUpper();
                    if (cFieldVal.Length > cPX.Length && cPX.Length > 0)
                    {
                        if (cFieldVal.Substring(0, cPX.Length) == cPX)
                            tflotno.Invoke(new Action(delegate() { tflotno.Text = cFieldVal.Replace(cPX, ""); }));
                    }
                }
                if (cFN.ToUpper() == "MFGRPART")
                {
                    cFieldVal = tfrecmfgrpart.Text.ToUpper();
                    if (cFieldVal.Length > cPX.Length && cPX.Length > 0)
                    {
                        if (cFieldVal.Substring(0, cPX.Length) == cPX)
                            tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.Text = cFieldVal.Replace(cPX, ""); }));
                    }
                }
                if (cFN.ToUpper() == "MFGDATE")
                {
                    cFieldVal = tfmfgdate.Text.ToUpper();
                    if (cFieldVal.Length > cPX.Length && cPX.Length > 0)
                    {
                        if (cFieldVal.Substring(0, cPX.Length) == cPX)
                            tfmfgdate.Invoke(new Action(delegate() { tfmfgdate.Text = cFieldVal.Replace(cPX, ""); }));
                    }
                }
                if (cFN.ToUpper() == "EXPIREDATE")
                {
                    cFieldVal = tfexpiredate.Text.ToUpper();
                    if (cFieldVal.Length > cPX.Length && cPX.Length > 0)
                    {
                        if (cFieldVal.Substring(0, cPX.Length) == cPX)
                            tfexpiredate.Invoke(new Action(delegate() { tfexpiredate.Text = cFieldVal.Replace(cPX, ""); }));
                    }
                }
                if (cFN.ToUpper() == "DATECODE")
                {
                    cFieldVal = tfdatecode.Text.ToUpper();
                    if (cFieldVal.Length > cPX.Length && cPX.Length > 0)
                    {
                        if (cFieldVal.Substring(0, cPX.Length) == cPX)
                            tfdatecode.Invoke(new Action(delegate() { tfdatecode.Text = cFieldVal.Replace(cPX, ""); }));
                    }
                }
                if (cFN.ToUpper() == "DNPARTNUMBER")
                {
                    cFieldVal = tfdnpartnumber.Text.ToUpper();
                    if (cFieldVal.Length > cPX.Length && cPX.Length > 0)
                    {
                        if (cFieldVal.Substring(0, cPX.Length) == cPX)
                            tfdnpartnumber.Invoke(new Action(delegate() { tfdnpartnumber.Text = cFieldVal.Replace(cPX, ""); }));
                    }
                }
                if (cFN.ToUpper() == "RECQTY")
                {
                    cFieldVal = tfrecqty.Text.ToUpper();
                    if (cFieldVal.Length > cPX.Length && cPX.Length > 0)
                    {
                        if (cFieldVal.Substring(0, cPX.Length) == cPX)
                            tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = cFieldVal.Replace(cPX, ""); }));
                    }
                }
            }
        }
        void setMandField()
        {
            String cErrMsg, cSpecialPartVal, cExpireDatePartVal, cQuery;
            DateTime cOldMfgDate;
            tflotno.BackColor = Color.White;
            cOldMfgDate = DateTime.Now.AddDays(-730);
            MiscDLL1.dbClass mydbClass = new MiscDLL1.dbClass();
            cErrMsg = ""; cExpireDatePartVal = ""; cSpecialPartVal = "";
            cQuery = "select tmp_Part from tmp_tab where tmp_system='wse869a4' and tmp_part='" + tfpartno.Text + "' and tmp_site='" + tfsite.Text + "'";
            cSpecialPartVal = mydbClass.getSingleFieldData(cConnStr, cQuery);
            cQuery = "select tmp_Part from tmp_tab where tmp_system='expidate' and tmp_part='" + tfpartno.Text + "' ";
            cExpireDatePartVal = mydbClass.getSingleFieldData(cConnStr, cQuery);
            lMDateCode.Visible = false; lMExpireDate.Visible = false; lMLotNumber.Visible = false;
            lMRecMfgPart.Visible = true;
            //lMDateCode.ForeColor = Color.Black; lMLotNumber.ForeColor = Color.Black;
            //"\nRequire Rec Mfgr Part Number";
            if (cSpecialPartVal.Length > 0)
            {
                lMDateCode.Visible = true; lMLotNumber.Visible = true; //lMDateCode.ForeColor = Color.DarkBlue; lMLotNumber.ForeColor = Color.DarkBlue;
            }
            if (cExpireDatePartVal.Length > 0) lMExpireDate.Visible = true;

            if (tfsite.Text.ToUpper() == "MG0337") { lMLotNumber.Visible = true; lMDateCode.Visible = true; }

            if (tfsite.Text.ToUpper() == "MG7024" || tfsite.Text.ToUpper() == "MG5007" || tfsite.Text.ToUpper() == "MG7030" || tfsite.Text.ToUpper() == "MG7029" || tfsite.Text.ToUpper() == "MG5008" || tfsite.Text.ToUpper() == "MG0248" || tfsite.Text.ToUpper() == "MG7028" ||
                tfsite.Text.ToUpper() == "MG7022" || tfsite.Text.ToUpper() == "MG0208" || tfsite.Text.ToUpper() == "MG0220" || tfsite.Text.ToUpper() == "MG0217")
            {
                if (tfpartno.Text.Substring(0, 1) == "1" || tfpartno.Text.Substring(0, 1) == "2" || tfpartno.Text.Substring(0, 1) == "3" || tfpartno.Text.Substring(0, 1) == "5" || tfpartno.Text.Substring(0, 2) == "70")
                {
                    //"nDateCode or Lot Number required for 1x,2x,3x,5x,70x parts";
                    lMDateCode.Visible = true; lMLotNumber.Visible = true; //lMDateCode.ForeColor = Color.DarkBlue; lMLotNumber.ForeColor = Color.DarkBlue;
                }
            }
            //lMDateCode.ForeColor = Color.DarkBlue; lMLotNumber.ForeColor = Color.DarkBlue;
            return;
        }
        int valData()
        {
            String cErrMsg, cSpecialPartVal, cExpireDatePartVal, cQuery;
            int cRet;
            DataGridViewRow cR;
            DateTime value;
            Double cTemp;
            DateTime cMfgDate;
            DateTime cOldMfgDate;
            tflotno.BackColor = Color.White;
            cOldMfgDate = DateTime.Now.AddDays(-730);
            MiscDLL1.dbClass mydbClass = new MiscDLL1.dbClass();
            cRet = 0;
            cErrMsg = ""; cExpireDatePartVal = ""; cSpecialPartVal = "";
            cR = dataGridView1.SelectedRows[0];
            /*toolTip1.SetToolTip(tfcumqty, "");
            toolTip1.SetToolTip(tfrecqty, "");
            toolTip1.SetToolTip(tfexpiredate, "");
            toolTip1.SetToolTip(tfmfgdate, "");
            toolTip1.SetToolTip(tflotno, "");
            toolTip1.SetToolTip(tfdatecode, "");
            */
            // tfrecqty.Invoke(new Action(delegate() { tfrecqty.Text = tfrecqty.Text.Trim(); }));
            removePrefix();
            tfcumqty.Invoke(new Action(delegate() { tfcumqty.BackColor = Color.White; }));
            tfrecqty.Invoke(new Action(delegate() { tfrecqty.BackColor = Color.White; }));
            tfexpiredate.Invoke(new Action(delegate() { tfexpiredate.BackColor = Color.White; }));
            tfmfgdate.Invoke(new Action(delegate() { tfmfgdate.BackColor = Color.White; }));
            tflotno.Invoke(new Action(delegate() { tflotno.BackColor = Color.White; }));
            tfdatecode.Invoke(new Action(delegate() { tfdatecode.BackColor = Color.White; }));
            tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.BackColor = Color.White; }));
            String cPrintQty, cDNQty;
            //cPrintQty = dataGridView1.CurrentRow.Cells["PrintedQty"].Value.ToString();
            //cDNQty = dataGridView1.CurrentRow.Cells["DNQty"].Value.ToString();
            cPrintQty = dataGridView1.SelectedRows[0].Cells["PrintedQty"].Value.ToString();
            cDNQty = dataGridView1.SelectedRows[0].Cells["DNQty"].Value.ToString();
            Double dLinePrintQty;
            //cPrintQty = getCompleteQty(cR["t_dn"].ToString(), cR["t_po"].ToString(), cR["t_id"].ToString(), cR["t_rir"].ToString(), cR["t_deli_date"].ToString(), cR["t_supp"].ToString()); 
            dLinePrintQty = getCompleteQty(cR.Cells["DNNo"].Value.ToString(), cR.Cells["PONumber"].Value.ToString(), cR.Cells["POLine"].Value.ToString(), tfrirno.Text, tfhdndate.Text, tfvendor.Text);
            cPrintQty = dLinePrintQty.ToString();
            if (cPrintQty.Length == 0) cPrintQty = "0";
            if (cDNQty.Length == 0) cDNQty = "0";
            if (tfrecqty.Text.Length == 0) tfrecqty.Text = "0";
            if (Convert.ToDouble(cPrintQty) + (Convert.ToDouble(tfrecqty.Text) * Convert.ToDouble(tfnooflabels.Text)) > Convert.ToDouble(cDNQty))
            {
                cRet += 1;
                cErrMsg += "\nCannot Print PIMS more than DNQty";
            }
            cQuery = "select tmp_Part from tmp_tab where tmp_system='wse869a4' and tmp_part='" + tfpartno.Text + "' and tmp_site='" + tfsite.Text + "'";
            cSpecialPartVal = mydbClass.getSingleFieldData(cConnStr, cQuery);
            cQuery = "select tmp_Part from tmp_tab where tmp_system='expidate' and tmp_part='" + tfpartno.Text + "' ";
            cExpireDatePartVal = mydbClass.getSingleFieldData(cConnStr, cQuery);

            if (tfrecmfgrpart.Text.Length == 0)
            {
                cRet += 1;
                //tfrecmfgrpart.BackColor = Color.Red;
                tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.BackColor = Color.Red; }));
                cErrMsg += "\nRequire Rec Mfgr Part Number";
            }
            else
            {
                tfrecmfgrpart.Invoke(new Action(delegate() { tfrecmfgrpart.BackColor = Color.White; }));
            }
            if (cSpecialPartVal.Length > 0)
            {
                if (tfdatecode.Text.Length == 0 && tflotno.Text.Length == 0)
                {
                    cRet += 1;
                    tfdatecode.Invoke(new Action(delegate() { tfdatecode.BackColor = Color.Red; }));
                    cErrMsg += "\nDate Code or Lot Number Required for this Parts";
                }
            }
            if (cExpireDatePartVal.Length > 0)
            {
                if (tfexpiredate.Text.Length == 0)
                {
                    cRet += 1;
                    //tfdatecode.BackColor = Color.Red;
                    tfdatecode.Invoke(new Action(delegate() { tfdatecode.BackColor = Color.Red; }));
                    cErrMsg += "\nExpire Date Required for this Part";
                }
            }
            if (!Double.TryParse(tfrecqty.Text, out cTemp))
            {
                cRet += 1;
                //tfrecqty.BackColor = Color.Red;
                tfrecqty.Invoke(new Action(delegate() { tfrecqty.BackColor = Color.Red; }));
                cErrMsg += "\nRequire Number in received Qty";
            }
            else
            {
                //tfrecqty.BackColor = Color.White;
                tfrecqty.Invoke(new Action(delegate() { tfrecqty.BackColor = Color.White; }));
            }
            /*if (!Double.TryParse(tfcumqty.Text, out cTemp)) {
                cRet += 1;
                //tfcumqty.BackColor = Color.Red;
                tfcumqty.Invoke(new Action(delegate() { tfcumqty.BackColor = Color.Red; }));
                cErrMsg += "\nInvalid Cumulative Qty";
            } else {
                tfcumqty.Invoke(new Action(delegate() { tfcumqty.BackColor = Color.White; }));
            }*/
            if (tfsite.Text.ToUpper() == "MG0337")
            {
                if (tflotno.Text.Length == 0 && tfdatecode.Text.Length == 0)
                {
                    cRet += 1;
                    //tflotno.BackColor = Color.Red;
                    tflotno.Invoke(new Action(delegate() { tflotno.BackColor = Color.Red; }));
                    cErrMsg += "\nLot Number/DateCode can not be empty for MG0337";
                }
            }
            if (tfsite.Text.ToUpper() == "MG7024" || tfsite.Text.ToUpper() == "MG5007" || tfsite.Text.ToUpper() == "MG7030" || tfsite.Text.ToUpper() == "MG7029" || tfsite.Text.ToUpper() == "MG5008" || tfsite.Text.ToUpper() == "MG0248" || tfsite.Text.ToUpper() == "MG7028" ||
                tfsite.Text.ToUpper() == "MG7022" || tfsite.Text.ToUpper() == "MG0208" || tfsite.Text.ToUpper() == "MG0220" || tfsite.Text.ToUpper() == "MG0217")
            {
                if (tfpartno.Text.Substring(0, 1) == "1" || tfpartno.Text.Substring(0, 1) == "2" || tfpartno.Text.Substring(0, 1) == "3" || tfpartno.Text.Substring(0, 1) == "5" || tfpartno.Text.Substring(0, 2) == "70")
                {
                    if (tfdatecode.Text.Length == 0 && tflotno.Text.Length == 0)
                    {
                        cRet += 1;
                        tfdatecode.Invoke(new Action(delegate() { tfdatecode.BackColor = Color.Red; }));
                        cErrMsg += "\nDateCode or Lot Number required for 1x,2x,3x,5x,70x parts";
                    }
                }
            }
            if (tfmfgdate.Text.Length > 0)
            {
                if (!DateTime.TryParse(tfmfgdate.Text, out value))
                {
                    cRet += 1;
                    tfmfgdate.Invoke(new Action(delegate() { tfmfgdate.BackColor = Color.Red; }));
                    cErrMsg += "\nInvalid Date in Mfgr Date";
                }
                else
                {
                    //tfmfgdate.Text = Convert.ToDateTime(tfmfgdate.Text).ToString("MM/dd/yy");
                    tfmfgdate.Invoke(new Action(delegate() { tfmfgdate.Text = Convert.ToDateTime(tfmfgdate.Text).ToString("MM/dd/yy"); }));
                    cMfgDate = Convert.ToDateTime(tfmfgdate.Text);
                    if (cMfgDate.CompareTo(DateTime.Now) > 0)
                    {
                        cRet += 1;
                        //tfmfgdate.BackColor = Color.Red;
                        tfmfgdate.Invoke(new Action(delegate() { tfmfgdate.BackColor = Color.Red; }));
                        cErrMsg += "\nMfgr Date should not be later than today";
                    }
                    else if (cMfgDate.CompareTo(cOldMfgDate) < 0)
                    {
                        cRet += 1;
                        tfmfgdate.Invoke(new Action(delegate() { tfmfgdate.BackColor = Color.White; }));
                        cErrMsg += "\nMfgr Date is too old";
                    }
                    else
                    {
                        tfmfgdate.Invoke(new Action(delegate() { tfmfgdate.BackColor = Color.White; }));
                    }
                }
            }
            if (tfexpiredate.Text.Length > 0)
            {
                if (!DateTime.TryParse(tfexpiredate.Text, out value))
                {
                    cRet += 1;
                    tfexpiredate.Invoke(new Action(delegate() { tfmfgdate.BackColor = Color.Red; }));
                    cErrMsg += "\nInvalid expire date";

                }
                else
                {
                    tfexpiredate.Invoke(new Action(delegate() { tfmfgdate.BackColor = Color.White; }));
                    tfexpiredate.Invoke(new Action(delegate() { tfexpiredate.Text = Convert.ToDateTime(tfexpiredate.Text).ToString("MM/dd/yy"); }));
                }
            }

            /* to be removed, suggested by business users
            try
            {
                if (Double.TryParse(tfrecqty.Text, out cTemp) && Double.TryParse(tfcumqty.Text, out cTemp) && Double.TryParse(tfdnqty.Text, out cTemp)) {
                    if ((Convert.ToDouble(tfcumqty.Text) + Convert.ToDouble(tfrecqty.Text)) > Convert.ToDouble(tfdnqty.Text))
                    {
                        cRet += 1;
                        tfcumqty.Invoke(new Action(delegate() { tfcumqty.BackColor = Color.Red; }));
                        cErrMsg += "\nPIMS Already printed for all DN QTY/\nInvalid Receive Qty";
                    } else {
                        tfcumqty.Invoke(new Action(delegate() { tfcumqty.BackColor = Color.White; }));
                    }
                }
            }
            catch (Exception) { } */
            if (cErrMsg.Length > 0)
            {
                MessageBox.Show(cErrMsg, "Error Message");
            }
            return cRet;
        }
        void setDataFieldLabel()
        {
            int cRow;
            String cXMLData;
            byte[] cImage;
            List<String> cFieldList = new List<String>();
            cRow = dataGridView3.CurrentRow.Index;
            if (cRow < lXML.Count)
                cXMLData = lXML[cRow];
            else
                return;
            lVendorLabel = new List<vendorLabelDefinition>();
            setFields(lVendorLabel = parseTempXMLTest(cXMLData));
            try
            {
                cImage = lVendorLabelImage[dataGridView3.CurrentRow.Index];
                if (cImage.Length == 0)
                    pb1.ImageLocation = Application.StartupPath + @"\images\notavailable.png";
                else
                    pb1.Image = getImage(cImage);
            }
            catch (Exception ex) { }
            /*if (cTemplateType.ToUpper() == "GENERAL") {
                cbAutoPrint.Checked = false;
                cbAutoPrint.Enabled = false;
            } else {
                cbAutoPrint.Checked = true;
                cbAutoPrint.Enabled = true;
            }*/
        }
        private void dataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            cCaptureData cDG3 = new cCaptureData();
            cDG3.cDNPartumber = tfdnpartnumber.Text;
            cDG3.cMFGPart = tfrecmfgrpart.Text;
            cDG3.cDateCode = tfdatecode.Text;
            cDG3.cRecQty = tfrecqty.Text;
            cDG3.cLotNumber = tflotno.Text;
            cDG3.cMfgDate = tfmfgdate.Text;
            cDG3.cExpiredate = tfexpiredate.Text;

            resetForm(0);
            setDataFieldLabel();

            tfdnpartnumber.Text = cDG3.cDNPartumber;
            tfrecmfgrpart.Text = cDG3.cMFGPart;
            tfdatecode.Text = cDG3.cDateCode;
            tfrecqty.Text = cDG3.cRecQty;
            tflotno.Text = cDG3.cLotNumber;
            tfmfgdate.Text = cDG3.cMfgDate;
            tfexpiredate.Text = cDG3.cExpiredate;
        }
        StreamReader callMFGService(String cSystemID, String progID, String cParam)
        {
            String cRet;
            cRet = "";
            try
            {
                cRet = MFGProService.GetTable(cSystemID, progID, cParam);
            }
            catch (Exception) { }
            byte[] byteArray = Encoding.ASCII.GetBytes(cRet);
            MemoryStream stream2 = new MemoryStream(byteArray);
            StreamReader cSReader = new StreamReader(stream2);
            return cSReader;
        }
        void getMFGDNData()
        {
            DataRow cR;
            StreamReader cRetReader;
            int cFound;
            List<String> lDNNumber = new List<string>();
            dsDNDetail = new DataSet("dsDNDetail");
            cRetReader = callMFGService(cbsystem.Text, "wsas001", tfdnno.Text + "," + tfdndate.Text + "," + tftodndate.Text);
            //cRetReader = callMFGService(cbsystem.Text, "wsas001", tfdnno.Text + "," + tfdndate.Text);
            try
            {
                dsDNDetail.ReadXml(cRetReader);
            }
            catch (Exception serEx) { MessageBox.Show("MFGPro Service Error:\n" + serEx.Message.ToString(), "System Message"); return; }

            int i = 0;
            //t1 dataGridView1.Rows.Clear();

            int cRowCount;
            if (dsDNDetail.Tables.Count >= 7)
            {
                dsDNDetail.Tables[6].Columns.Add("PrintedQty");
                dsDNDetail.Tables[6].Columns.Add("RowID");

                dgDNNumber.Rows.Clear();
                cRowCount = dsDNDetail.Tables[6].Rows.Count;
                while (i <= dsDNDetail.Tables[6].Rows.Count - 1)
                {
                    cR = dsDNDetail.Tables[6].Rows[i];
                    //t1 dataGridView1.Rows.Add(cR.ItemArray[0], cR.ItemArray[10], cR.ItemArray[7], cR.ItemArray[4], cR.ItemArray[3], cR.ItemArray[9], "",cR.ItemArray[2], cR.ItemArray[6], cR.ItemArray[1], cR.ItemArray[5], cR.ItemArray[11], cR.ItemArray[12], cR.ItemArray[13], cR.ItemArray[14], cR.ItemArray[15], cR.ItemArray[16], cR.ItemArray[17], cR.ItemArray[18],"0");
                    var query = from p in lDNNumber
                                where lDNNumber.Contains(cR.ItemArray[0])
                                select p;
                    cFound = 0;
                    foreach (String t in query)
                    {
                        cFound += 1;
                    }
                    if (cFound == 0)
                    {
                        lDNNumber.Add(cR.ItemArray[0].ToString());
                    }

                    i += 1;
                }
                var xx = from t in lDNNumber select t;
                foreach (String t1 in xx)
                    dgDNNumber.Rows.Add(t1);

            }
            else
            {
                //t1 dataGridView1.Rows.Clear();
                dataGridView3.Rows.Clear();
                dgDNNumber.Rows.Clear();
                resetForm(1);
                MessageBox.Show("No Data Found");
            }
        }
        List<String> parseTempXML(String cXMLData)
        {
            DataRow cR;
            List<String> lRet = new List<String>();
            DataSet dsAuthors = new DataSet("Template");
            byte[] byteArray = Encoding.ASCII.GetBytes(cXMLData);
            MemoryStream stream = new MemoryStream(byteArray);
            StreamReader xx1 = new StreamReader(stream);

            dsAuthors.ReadXml(xx1);
            int i = 0;
            while (i <= dsAuthors.Tables[0].Rows.Count - 1)
            {
                cR = dsAuthors.Tables[0].Rows[i];
                lRet.Add(cR.ItemArray[0].ToString());
                i += 1;
            }
            return lRet;
        }
        List<vendorLabelDefinition> parseTempXMLTest(String cXMLData)
        {
            DataRow cR;
            List<vendorLabelDefinition> lRet = new List<vendorLabelDefinition>();
            DataSet dsAuthors = new DataSet("Template");
            byte[] byteArray = Encoding.ASCII.GetBytes(cXMLData);
            MemoryStream stream = new MemoryStream(byteArray);
            StreamReader xx1 = new StreamReader(stream);

            dsAuthors.ReadXml(xx1);
            vendorLabelDefinition vendorLabel = new vendorLabelDefinition();

            int i = 0;
            while (i <= dsAuthors.Tables["Field"].Rows.Count - 1)
            {
                cR = dsAuthors.Tables["Field"].Rows[i];
                vendorLabel = new vendorLabelDefinition();
                vendorLabel.cFieldName = cR.ItemArray[0].ToString();
                vendorLabel.cPrefix = cR.ItemArray[1].ToString();
                vendorLabel.cSeperator = cR.ItemArray[2].ToString();
                vendorLabel.cIndex = cR.ItemArray[3].ToString();
                lRet.Add(vendorLabel);
                i += 1;
            }
            c2DSeperator = "";
            if (dsAuthors.Tables.IndexOf("Header") >= 0)
            {
                cTemplateType = dsAuthors.Tables["Header"].Rows[0].ItemArray[1].ToString();
                if (dsAuthors.Tables["Header"].Rows[0].ItemArray.Length > 2)
                    c2DSeperator = dsAuthors.Tables["Header"].Rows[0].ItemArray[2].ToString();

                if (c2DSeperator == "\\r\\n")
                {
                    c2DSeperator = "\r\n";
                }
                if (c2DSeperator == "\\t")
                {
                    c2DSeperator = "\t";
                }
            }
            else
            {
                cTemplateType = "Single";
                c2DSeperator = "";
            }
            return lRet;
        }
        void toPrinter(StringBuilder cStringToPrint, String cPIMS)
        {
            String cSelPort;
            cSelPort = "LPT1";
            lStatus.Invoke(new Action(delegate() { lStatus.Text = "Printing...."; }));
            cbport.Invoke(new Action(delegate() { cSelPort = cbport.SelectedItem.ToString(); }));
            StreamWriter outputfile = new StreamWriter("c://tmp//PIMS/spool//piml" + cPIMS + ".txt", false, Encoding.UTF8);
            try
            {
                PrinterHandle.LPTControl printHandle = new PrinterHandle.LPTControl(cSelPort);
                if (printHandle.Open())
                {
                    printHandle.Write(cStringToPrint.ToString());
                    printHandle.Close();
                }
                outputfile.Write(cStringToPrint.ToString());
            }
            catch (Exception prEx) { MessageBox.Show("Print Error :\n" + prEx.Message.ToString()); }
            finally { outputfile.Close(); }
            lStatus.Invoke(new Action(delegate() { lStatus.Text = ""; }));
        }
        void printPIML(List<String> lPIMSData, int cLabelType)
        {
            StringBuilder cRet = new StringBuilder();
            PIMLPrint pimlPrint = new PIMLPrint();
            String cSelPrinter;
            int cNoLabel;
            DataGridViewRow cR = new DataGridViewRow();
            //cR = dataGridView1.CurrentRow;
            cR = dataGridView1.SelectedRows[0];
            cSelPrinter = "1";
            cNoLabel = Convert.ToInt32(tfnooflabels.Text);
            //cSelPrinter = (cbprintertype.SelectedIndex + 1).ToString();
            cbprintertype.Invoke(new Action(delegate() { cSelPrinter = (cbprintertype.SelectedIndex + 1).ToString(); }));
            try
            {
                /* cRet = pimlPrint.genPIML(
                        tfdndate.Text.Substring(tfdndate.Text.Length - 2, 2), 
                        "*IQC", tflotno.Text.ToUpper(), tfpartno.Text.ToUpper(), cR.Cells["DNSite"].Value.ToString(), 
                        tfrecqty.Text, tfdnqty.Text, "Ref", cR.Cells["t_loc"].Value.ToString(),
                        tfexpiredate.Text, "R", tfmfgpart.Text.ToUpper(), cR.Cells["t_cust_part"].Value.ToString(), 
                        cPIMSNumber, tfdatecode.Text,
                        cSelPrinter, "by", cR.Cells["t_wt_ind"].Value.ToString(), cR.Cells["t_wt"].Value.ToString(), 
                        cR.Cells["t_MSD"].Value.ToString(), cUserID, cR.Cells["t_msd"].Value.ToString(), "",cNoLabel
                ); */
                //6=type;3=Part;4=Site;8=Qty_Per;9=Qty_Tot;7=Ref;5=Loc;10=ExpiDate;11=ExpiType;12=MfgrPart;13;CustPart;1=PIMSNnbr;14=DateCode
                //15=by;16=wt;17=msd;

                cRet = pimlPrint.genPIML(
                            tfdndate.Text.Substring(tfdndate.Text.Length - 2, 2),
                            lPIMSData[5].ToString().ToUpper(), tflotno.Text.ToUpper(), lPIMSData[2].ToString().ToUpper(), lPIMSData[3].ToString().ToUpper(),
                            lPIMSData[7].ToString().ToUpper(), tfdnqty.Text, lPIMSData[6].ToString().ToUpper(), lPIMSData[4].ToString().ToUpper(),
                            lPIMSData[9].ToString().ToUpper(), lPIMSData[10].ToString().ToUpper(), lPIMSData[11].ToString().ToUpper(), lPIMSData[12].ToString().ToUpper(),
                            lPIMSData[0].ToString().ToUpper(), lPIMSData[13].ToString().ToUpper(),
                            cSelPrinter, lPIMSData[14].ToString().ToUpper(), lPIMSData[15].ToString().ToUpper(), lPIMSData[15].ToString().ToUpper(),
                            lPIMSData[16].ToString().ToUpper(), cUserID, lPIMSData[16].ToString().ToUpper(), "", 1, tfrirno.Text.ToUpper(), lPIMSData[17].ToString().ToUpper()
                 );
                toPrinter(cRet, lPIMSData[0].ToString());
                if (cLabelType == 0)
                    setDSPrintedQty();

            }
            catch (Exception labEr) { MessageBox.Show("Data Error:" + labEr.Message.ToString()); }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (dsDNDetail.Tables.Count >= 7)
                setGV1();
        }
        void getlinq1()
        {
            int[] iArray = { 1, 2, 4, 7, 3, 41, 5, 6 };
            var x = from myArray in iArray where myArray > 2 orderby myArray select myArray;
            foreach (var x1 in x)
            {

            }
        }
        void setGV1()
        {
            int i = 0;
            String cDNNo;
            DataRow cR;
            DataGridViewRow cDGR;
            dataGridView1.Rows.Clear();
            DataTable dt = new DataTable();
            dt = (DataTable)dsDNDetail.Tables[6];
            cDGR = dgDNNumber.CurrentRow;
            cDNNo = cDGR.Cells["DNNumber"].Value.ToString();
            while (i <= dsDNDetail.Tables[6].Rows.Count - 1)
            {
                cR = dsDNDetail.Tables[6].Rows[i];
                if (cbfiltertype.SelectedIndex == 0)
                {
                    if ((cR.ItemArray[3].ToString().ToUpper().StartsWith(textBox2.Text.ToUpper()) && cR.ItemArray[0].ToString() == cDNNo) || (textBox2.Text.Length == 0 && cR.ItemArray[0].ToString() == cDNNo))
                    {
                        dataGridView1.Rows.Add(cR.ItemArray[0], cR.ItemArray[10], cR.ItemArray[7], cR.ItemArray[3], cR.ItemArray[9], cR.ItemArray[2], cR.ItemArray[4], "", cR.ItemArray[6], cR.ItemArray[1], cR.ItemArray[5], cR.ItemArray[11], cR.ItemArray[12], cR.ItemArray[13], cR.ItemArray[14], cR.ItemArray[15], cR.ItemArray[16], cR.ItemArray[17], cR.ItemArray[18], cR.ItemArray[20], i.ToString());
                    }
                }
                else
                {
                    if ((cR.ItemArray[9].ToString().ToUpper().StartsWith(textBox2.Text.ToUpper()) && cR.ItemArray[0].ToString() == cDNNo) || (textBox2.Text.Length == 0 && cR.ItemArray[0].ToString() == cDNNo))
                    {
                        dataGridView1.Rows.Add(cR.ItemArray[0], cR.ItemArray[10], cR.ItemArray[7], cR.ItemArray[3], cR.ItemArray[9], cR.ItemArray[2], cR.ItemArray[4], "", cR.ItemArray[6], cR.ItemArray[1], cR.ItemArray[5], cR.ItemArray[11], cR.ItemArray[12], cR.ItemArray[13], cR.ItemArray[14], cR.ItemArray[15], cR.ItemArray[16], cR.ItemArray[17], cR.ItemArray[18], cR.ItemArray[20], i.ToString());
                    }
                }
                i += 1;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {

        }
        Image getImage(byte[] cByte)
        {
            MemoryStream ms = new MemoryStream(cByte);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
        void timer_Tick(object sender, EventArgs e)
        {
            StartCodeReader();
        }
        /*private void tsStart_Click(object sender, EventArgs e)
        {
            tsStart.Text = "Running";
            tsStop.Enabled = true;
            readerThread = new Thread(new ThreadStart(startThread));
            readerThread.Start();
        }*/
        private void button2_Click(object sender, EventArgs e)
        {
            bStart.Text = "Starting";
            bStart.Enabled = false;

            readerThread = new Thread(new ThreadStart(startThread));
            readerThread.Start();

        } // End NewData()
        private void startThread()
        {
            CodeReaderhandle = StartCodeReader();

            MessageBox.Show("Time Expired/Device not available\nRestart Again", "Notice");
            if (CodeReaderhandle.ToString() != "0")
            {
                StopCodeReader(CodeReaderhandle);
            }
            bStart.Invoke(new Action(delegate() { bStart.Text = "Start"; }));
            bStart.Invoke(new Action(delegate() { bStart.Enabled = true; }));
            bStop.Invoke(new Action(delegate() { bStop.Enabled = false; }));

        }

        /*private void tsStop_Click(object sender, EventArgs e)
        {
            StopCodeReader(CodeReaderhandle);
            readerThread.Abort();
            tsStart.Text = "Start";
            tsStop.Enabled = false;
        }*/
        private void bStop_Click(object sender, EventArgs e)
        {
            StopCodeReader(CodeReaderhandle);
            readerThread.Abort();
            bStart.Text = "Start";
            bStart.Invoke(new Action(delegate() { bStart.Enabled = true; }));
            bStop.Invoke(new Action(delegate() { bStop.Enabled = false; }));
        }
        delegate void TextBoxDelegate(string message);
        public void UpdatingTextBox(string msg)
        {
            if (tfscanarea.InvokeRequired)
                tfscanarea.Invoke(new TextBoxDelegate(UpdatingTextBox), new object[] { msg });
            else
                this.tfscanarea.Text = msg;
        }

        //---
        IntPtr deviceHandle;
        Int32 success;
        public IntPtr StartCodeReader()
        {
            IntPtr hardwareDetector = CodeUtil.NativeMethods.Code_CreateHardwareDetector(null);
            uint maxSize = 5000;
            Int32 commandLength = 1024;
            StringBuilder hardwareXml = new StringBuilder((int)maxSize + 1);
            CodeUtil.NativeMethods.Code_SwitchKeyboardToHidNative();
            Thread.Sleep(5000);

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
            Console.WriteLine("For the next 15 mins, scan a bar code or Ctrl+C to exit");
            bStart.Invoke(new Action(delegate() { bStart.Enabled = false; }));
            bStop.Invoke(new Action(delegate() { bStop.Enabled = true; }));
            bStart.Invoke(new Action(delegate() { bStart.Text = "Running"; }));
            String myComm;
            myComm = "P%260";
            CodeUtil.NativeMethods.Code_TerminalSendCommand(deviceHandle, myComm, myComm.Length);
            Thread.Sleep(1800000);

            return deviceHandle;
        }
        public void StopCodeReader(IntPtr deviceHandleMain)
        {

            /* Close the Terminal connection to the Reader */
            try
            {
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
            catch (Exception) { }
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
        int CheckScanStatus()
        {
            DateTime cLP = new DateTime();
            cDisable = 1;
            DateTime cThisTime = new DateTime();
            cThisTime = DateTime.Now;
            cLP = cLastPrint;
            cLP = cLP.AddSeconds(5);
            if (cLP.CompareTo(cThisTime) > 0)
                cDisable = 1;
            else
                cDisable = 0;

            return cDisable;
        }

        private Int32 NewData(IntPtr handle, IntPtr data, Int32 length)
        {
            int cCompVal;
            Form1 form1 = new Form1();
            //Int32 commandLength = 1024;
            string dataString = Marshal.PtrToStringAnsi(data);
            //Console.WriteLine();
            Console.WriteLine("Data from Reader:");
            Console.WriteLine(dataString);
            //this.tfscanarea.Text += dataString;
            //MessageBox.Show(dataString);
            /*MethodInvoker action = delegate
            { tfscanarea.Text += dataString; };
            tfscanarea.BeginInvoke(action); */
            if (dataString.Length > 3)
            {
                if (cbAutoPrint.Checked == true)
                {
                    if (CheckScanStatus() == 1)
                        return 0;
                }
                if (dataString.ToUpper() == "<|>SAVE" || dataString.ToUpper() == "<|>PRINT")
                {
                    if (cLastLabel != dataString)
                    {
                        cCompVal = completeTrans();
                        if (cCompVal == 0)
                        {
                            cLastLabel = "<|>SAVE";
                            makeBeep();
                        }
                        else
                        {
                            cLastLabel = "";
                        }
                    }
                }
                else
                {
                    tfscanarea.Invoke(new Action(delegate() { tfscanarea.Text += dataString; }));
                    ParseLabelData();
                }
            }

            return 0;
        }
        void makeBeep()
        {
            String myComm;
            myComm = "P%2650";
            CodeUtil.NativeMethods.Code_TerminalSendCommand(deviceHandle, myComm, myComm.Length);
            myComm = "#%02";
            CodeUtil.NativeMethods.Code_TerminalSendCommand(deviceHandle, myComm, myComm.Length);
            myComm = "P%260";
            CodeUtil.NativeMethods.Code_TerminalSendCommand(deviceHandle, myComm, myComm.Length);
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            /*
           For testing...
           String xmlData;
           lVendorLabel = new List<vendorLabelDefinition>();
           xmlData = "<Header><Field><Name>LOTNUMBER</Name><Prefix>&lt;LL&gt;</Prefix></Field> " +
                             "<Field><Name>RECQTY</Name><Prefix>LQ</Prefix></Field> " +
                             "<Field><Name>DATECODE</Name><Prefix>DC</Prefix></Field> " +
                             "<Field><Name>expiredate</Name><Prefix>ex</Prefix></Field> " +
                             "<type>Single</type>" +
                             "</Header>";
           setFields(lVendorLabel = parseTempXMLTest(xmlData));
           GrabLabelData(); */
            resetAll();
            getMFGDNData();
        }

        //---
        public void resetAll()
        {
            dataGridView1.Rows.Clear();
            dataGridView3.Rows.Clear();

            tfvendor.Text = "";
            tfpartno.Text = "";
            tfrirno.Text = ""; tfmfgpart.Text = ""; tfhdndate.Text = ""; tfsite.Text = "";
        }

        private void bGo_Click(object sender, EventArgs e)
        {
            bGo.Text = "...";
            bGo.Enabled = false;
            getMFGDNData();
            bGo.Text = "Go";
            bGo.Enabled = true;
            getTemplate(); //added 25-06-2013
        }

        /* private void bDisableScan_Click(object sender, EventArgs e)
        {
            HH_Lib hwh = new HH_Lib();
            string[] devices = new string[1];
            devices[0] = "xx";
            //hwh.SetDeviceState(devices, false);
        }

        private void bEnableScan_Click(object sender, EventArgs e)
        {
            HH_Lib hwh = new HH_Lib();
            string[] devices = new string[1];
            devices[0] = "xx";
            //hwh.SetDeviceState(devices, true);
        } */

        private void cbSmartScan_CheckedChanged(object sender, EventArgs e)
        {
            cSearchEnable = 0;
        }

        private void btn5_startQR_Click(object sender, EventArgs e)
        {
            txt0ListKeyMsg.Items.Clear();
            kbh = new KeyBordHook();
            kbh.OnKeyDownEvent += new KeyEventHandler(kbh_OnKeyDownEvent);
            kbh.OnKeyPressEvent += new KeyPressEventHandler(kbh_OnKeyPressEvent);

            btn6_StopQR.Enabled = true;
            btn5_startQR.Enabled = false;

            tab3_QRBar.Focus();

        }

        void kbh_OnKeyPressEvent(object sender, KeyPressEventArgs e)
        {
            getStrQRcode += e.KeyChar.ToString();
        }

        void kbh_OnKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                for (int i = 0; i < txt0ListKeyMsg.Items.Count; i++)
                {
                    var dd = txt0ListKeyMsg.Items[i].ToString().Trim();
                    if (dd.Equals(getStrQRcode.Trim()))
                    {
                        getStrQRcode = "";
                        _spanint = 0;
                        txt0ListKeyMsg.SelectedIndex = i;
                        return;
                    }
                }
                txt0ListKeyMsg.Items.Add(getStrQRcode.Trim());
                getStrQRcode = "";
                txt0ListKeyMsg.SelectedIndex = txt0ListKeyMsg.Items.Count - 1;
            }



        }

        private void btn6_StopQR_Click(object sender, EventArgs e)
        {
            txt0ListKeyMsg.Items.Clear();
            if (kbh != null)
            {
                kbh.Stop();
                kbh.OnKeyDownEvent -= new KeyEventHandler(kbh_OnKeyDownEvent);
                kbh.OnKeyPressEvent -= new KeyPressEventHandler(kbh_OnKeyPressEvent);
            }
            btn5_startQR.Enabled = true;
            btn6_StopQR.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btn6_StopQR.Enabled = false;
            btn5_startQR.Enabled = true;
        }
        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (kbh != null)
            {
                kbh.Stop();
            }
        }
        private void tfnooflabels_TextChanged(object sender, EventArgs e)
        {

        }


        //C#中判断扫描枪输入与键盘输入
        private void tfnooflabels_KeyPress(object sender, KeyPressEventArgs e)
        {
            setEhandle(sender, e, 30);
        }

        private void setEhandle(object sender, KeyPressEventArgs e, int spanint)
        {
            if (_spanint > spanint)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        private void setEhandle(object sender, KeyEventArgs e, int spanint)
        {
            if (_spanint > spanint)
            {

                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        public void getInto()
        {
            DateTime tempDt = DateTime.Now;          //保存按键按下时刻的时间点
            TimeSpan ts = tempDt.Subtract(_dt);     //获取时间间隔
            //txt0ListKeyMsg.Items.Add(ts.Milliseconds);
            _spanint = ts.Milliseconds;
            //判断时间间隔，如果时间间隔大于50毫秒，则将TextBox清空

        }

        private void tfnooflabels_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            _dt = DateTime.Now;
        }
        private void tfnooflabels_KeyUp(object sender, KeyEventArgs e)
        {
            getInto();
            setEhandle(sender, e, 30);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txt0ListKeyMsg.Items.Clear();
        }

        private void reStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (kbh != null && btn6_StopQR.Enabled == true)
            {
                btn6_StopQR_Click(sender, e);
                btn5_startQR_Click(sender, e);
            }
        }

    }

    public class vendorLabelDefinition
    {
        private String FieldName, Prefix, RecQty, ExpireDate, MfgDate, Seperator, Index;
        public string cFieldName
        {
            get { return FieldName; }
            set { FieldName = value; }
        }
        public string cPrefix
        {
            get { return Prefix; }
            set { Prefix = value; }
        }
        public string cSeperator
        {
            get { return Seperator; }
            set { Seperator = value; }
        }
        public string cIndex
        {
            get { return Index; }
            set { Index = value; }
        }
        public string cRecQty
        {
            get { return RecQty; }
            set { RecQty = value; }
        }
        public string cExpireDate
        {
            get { return ExpireDate; }
            set { ExpireDate = value; }
        }
        public string cMfgDate
        {
            get { return MfgDate; }
            set { MfgDate = value; }
        }
    }
    //---
    class CaptureBarCode
    {

    }
    //---





}