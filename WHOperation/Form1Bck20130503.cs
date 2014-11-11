using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
//using System.Xml.Linq;
using System.IO;
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
        String cConnStr = "Persist Security Info=False;User ID=prabhakar;pwd=trustyou;Initial Catalog=dbWHOperation;Data Source=142.2.70.81;pooling=true";
        String cUserID;
        List<String> lXML = new List<String>();
        List<byte[]> lVendorLabelImage = new List<byte[]>();
        List<vendorLabelDefinition> lVendorLabel = new List<vendorLabelDefinition>();
        String cTemplateType;
        public Form1()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            dataGridView3.SelectionChanged += new EventHandler(dataGridView3_SelectionChanged);
            //this.tflotno.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownHandler);
            this.tfscanarea.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownHandlerscanArea);
            cbport.SelectedIndex = 0;
            cbprintertype.SelectedIndex = 0;
            cbsystem.Text = GlobalClass1.systemID;
            try {
                MFGProService.GetTable(cbsystem.Text, "wsas001", tfdnno.Text + "," + tfdndate.Text); 
            }
            catch (Exception) { }
            cUserID = GlobalClass1.userID;
            cTemplateType = "";
            base.OnLoad(e);

        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                completeTrans();
            }
        }
        private void OnKeyDownHandlerscanArea(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (tfscanarea.Text.ToUpper() == "SAVE")
                    completeTrans();
                else {
                    GrabLabelData();
                }
            }
        }
        void completeTrans() {
            String cLot;
            int cVal;
            cLot = tflotno.Text;
            cLot = cLot.Replace(Convert.ToChar(13).ToString(),"");
            tflotno.Text = cLot;
            if (cbautocomplete.Checked == true) {
                cVal = valData();
                if (cVal == 0) {
                    updData();
                    tflotno.Text = "";
                } else {
                    //MessageBox.Show("Data Validation failed");
                }
            }
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
            resetForm();
            setPIMLData();
            getTemplate();
        }
        void setFields(List<vendorLabelDefinition> vendorLabel) {
            String cFieldName, cPrefix;
            tflotno.Visible = false;
            tfmfgdate.Visible = false;
            tfexpiredate.Visible = false;
            tfdatecode.Visible = false;
            llotnumber.Visible = false;
            lmfgdate.Visible = false;
            lexpiredate.Visible = false;
            ldatecode.Visible = false;
            int i = 0;
            while (i <= vendorLabel.Count - 1) {
                cFieldName = vendorLabel[i].cFieldName;
                cPrefix = vendorLabel[i].cPrefix;
                if (cFieldName.ToUpper() == "LOTNUMBER") {
                    tflotno.Visible = true;
                    llotnumber.Visible = true;
                }
                if (cFieldName.ToUpper() == "MFGDATE") {
                    tfmfgdate.Visible = true;
                    lmfgdate.Visible = true;
                }
                if (cFieldName.ToUpper() == "EXPIREDATE") {
                    tfexpiredate.Visible = true;
                    lexpiredate.Visible = true;
                }
                if (cFieldName.ToUpper() == "DATECODE") {
                    tfdatecode.Visible = true;
                    ldatecode.Visible = true;
                }
                i += 1;
            } 
        }
        void ParseLabelData() {
            String cCompoundData,cSingleLabel;
            String[] cArrayData;
            int i;
            cCompoundData = tfscanarea.Text;
            cArrayData = cCompoundData.Split(",");
            while (j <= cArrayData.Length - 1)
            {
                cSingleLabel = cArrayData[i];
                GrabLabelData(cSingleData);
            }
        }
        void GrabLabelData(String cLabelData)
        {
            String cFieldName, cPrefix,cSeperator;
            int cIndex;
            Char cSplitter;
            int i = 0;
            if (cTemplateType.ToUpper() != "SINGLE"){
                MessageBox.Show("Only support 1D Barcode labels in this version");
                return;
            }
            if (tfscanarea.Text.Length == 0)
                return;
            while (i <= lVendorLabel.Count - 1)
            {
                cFieldName = lVendorLabel[i].cFieldName;
                cPrefix = lVendorLabel[i].cPrefix;
                cSeperator= lVendorLabel[i].cSeperator;
                if (cPrefix.Length == 0 || cPrefix.Length > tfscanarea.Text.Length ) { i += 1; continue; }
                
                if (lVendorLabel[i].cIndex.Length > 0)
                    cIndex = Convert.ToInt32(lVendorLabel[i].cIndex);
                else
                    cIndex = 1;
                String[] cTemp;
                if (cPrefix.ToUpper() == tfscanarea.Text.Substring(0, cPrefix.Length).ToUpper())
                {
                    if (cFieldName.ToUpper() == "LOTNUMBER")
                    {
                        tflotno.Text = tfscanarea.Text.Substring(cPrefix.Length, tfscanarea.Text.Length - cPrefix.Length);
                        if (cSeperator.Length > 0) {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tflotno.Text.Split(cSplitter );
                            if (cTemp.Length >= cIndex)
                                tflotno.Text = cTemp[cIndex-1];
                        }
                        tflotno.Text = tflotno.Text.Trim();
                        
                    }
                    else if (cFieldName.ToUpper() == "MFGDATE")
                    {
                        tfmfgdate.Text = tfscanarea.Text.Substring(cPrefix.Length, tfscanarea.Text.Length - cPrefix.Length);
                        if (cSeperator.Length > 0)
                        {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tfmfgdate.Text.Split(cSplitter);
                            if (cTemp.Length >= cIndex)
                                tfmfgdate.Text = cTemp[cIndex-1];
                        }
                        tfmfgdate.Text = tfmfgdate.Text.Trim();
                    }
                    else if (cFieldName.ToUpper() == "EXPIREDATE")
                    {
                        tfexpiredate.Text = tfscanarea.Text.Substring(cPrefix.Length, tfscanarea.Text.Length - cPrefix.Length);
                        if (cSeperator.Length > 0)
                        {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tfexpiredate.Text.Split(cSplitter);
                            if (cTemp.Length >= cIndex)
                                tfexpiredate.Text = cTemp[cIndex-1];
                        }
                        tfexpiredate.Text = tfexpiredate.Text.Trim();
                    }
                    else if (cFieldName.ToUpper() == "RECQTY")
                    {
                        tfrecqty.Text = tfscanarea.Text.Substring(cPrefix.Length, tfscanarea.Text.Length - cPrefix.Length);
                        if (cSeperator.Length > 0)
                        {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tfrecqty.Text.Split(cSplitter);
                            if (cTemp.Length >= cIndex)
                                tfrecqty.Text = cTemp[cIndex-1];
                        }
                        tfrecqty.Text = tfrecqty.Text.Trim();
                        tfrecqty.Text = tfrecqty.Text.Replace(",", "");
                    }
                    else if (cFieldName.ToUpper() == "DATECODE")
                    {
                        tfdatecode.Text = tfscanarea.Text.Substring(cPrefix.Length, tfscanarea.Text.Length - cPrefix.Length);
                        if (cSeperator.Length > 0)
                        {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tfdatecode.Text.Split(cSplitter);
                            if (cTemp.Length >= cIndex)
                                tfdatecode.Text = cTemp[cIndex-1];
                        }
                        tfdatecode.Text = tfdatecode.Text.Trim();
                    }
                    else if (cFieldName.ToUpper() == "MFGRPART")
                    {
                        tfrecmfgrpart.Text = tfscanarea.Text.Substring(cPrefix.Length, tfscanarea.Text.Length - cPrefix.Length);
                        if (cSeperator.Length > 0)
                        {
                            if (cSeperator == "SPACE")
                                cSplitter = ' ';
                            else
                                cSplitter = cSeperator[0];
                            cTemp = tfrecmfgrpart.Text.Split(cSplitter);
                            if (cTemp.Length >= cIndex)
                                tfrecmfgrpart.Text = cTemp[cIndex - 1];
                        }
                        tfrecmfgrpart.Text = tfrecmfgrpart.Text.Trim();
                    }
                }
                i += 1;
            }
            tfscanarea.Text="";
        }
        void resetForm() {
            tfmfgpart.Text = "";
            tflotno.Text = "";
            tfrecqty.Text = "";
            tfmfgdate.Text = "";
            tfexpiredate.Text = "";
            tfdatecode.Text = "";
            tfdnqty.Text = "";
            tfcumqty.Text = "";
            tfvendor.Text = "";
            tfpartno.Text = "";
            tfrirno.Text = "";
            tfrecmfgrpart.Text = "";
            tfrecqty.BackColor = Color.White;
            tfcumqty.BackColor = Color.White;
        }
        void setPIMLData() {
            String cQuery,cSelDNNo,cSelPONo,cSelPOLine,cSelDNDate,cSelVendor;
            SqlDataReader myReader;
            String[] cRec = new String[14];
            DataGridViewRow cR = new DataGridViewRow();
            Double cCumQty;
            int i;
            cCumQty = 0;
            cR = dataGridView1.CurrentRow;
            cSelDNNo = cR.Cells["DNNo"].Value.ToString() ;
            cSelDNDate = cR.Cells["DNDate"].Value.ToString();
            cSelPOLine = cR.Cells["POLine"].Value.ToString();
            cSelVendor = cR.Cells["Vendor"].Value.ToString();
            cSelPONo = cR.Cells["PONumber"].Value.ToString();
            tfdnqty.Text = cR.Cells["DNQty"].Value.ToString();
            tfsite.Text = cR.Cells["DNSite"].Value.ToString();

            tfhdnno.Text = cSelDNNo;
            tfhvendor.Text = cSelVendor;
            tfhdndate.Text = cSelDNDate;
            cQuery = "select TransLine,PIMSNumber,DNNo,VendorID,PONo,POLine,PartNumber,MFGPartNumber,LotNo,LineQty,RIRNo,transID,DateCode,ExpDate from PIMLDetail where DNNo='" + cSelDNNo + "' and DNDate='" + cSelDNDate + "' and VendorID='" + cSelVendor + "' and PONO='" + cSelPONo + "' and POLine='" + cSelPOLine + "'";
            dataGridView2.Rows.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(cConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(cQuery, conn);
                    myReader = cmd.ExecuteReader();
                    
                    while (myReader.Read()) {
                        i = 0;
                        for (i = 0; i <= myReader.FieldCount - 1; i += 1) {
                            cRec[i] = myReader.GetValue(i).ToString();
                        }
                        cCumQty += Convert.ToDouble(cRec[9]);
                        dataGridView2.Rows.Add(cRec);
                    }
                    myReader.Close();
                    tfcumqty.Text = cCumQty.ToString();
                    tfvendor.Text = cSelVendor;
                    tfpartno.Text = cR.Cells["PartNumber"].Value.ToString();
                    tfrirno.Text = cR.Cells["RIRNo"].Value.ToString();
                    tfmfgpart.Text = cR.Cells["MFGPartNo"].Value.ToString();
                }
            }
            catch (Exception) { }
            finally { }
        }
        void getTemplate() {
            String cQuery, cSelVendor;
            SqlDataReader myReader;
            String cRec;
            DataGridViewRow cR = new DataGridViewRow();
            String cXMLTemplate;
            byte[] cImageData;
            lXML = new List<String>();
            lVendorLabelImage = new List<byte[]>();
            cR = dataGridView1.CurrentRow;
            cSelVendor = cR.Cells[2].Value.ToString();
            cQuery = "select TemplateID,xmlVendorData,templateImage from PIMLVendorTemplate where VendorID='" + cSelVendor + "' Order By isDefault desc,TemplateID ";
            dataGridView3.Rows.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(cConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(cQuery, conn);
                    myReader = cmd.ExecuteReader();
                    while (myReader.Read()) {
                        cRec = myReader.GetValue(0).ToString();
                        cXMLTemplate = myReader.GetValue(1).ToString();
                        if (cXMLTemplate.Length > 0)
                        {
                            dataGridView3.Rows.Add(cRec);
                            lXML.Add(cXMLTemplate);
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
            int cVal;
            lStatus.Text = "Processing...";
            cVal = valData();
            if (cVal == 0) {
                updData();
                if (tfrecmfgrpart.Text.Length > 0)
                {
                    if (tfrecmfgrpart.Text.ToUpper() != tfmfgpart.Text.ToUpper())
                    {
                        MessageBox.Show("PO QPL Part & Received QPL Part mismatch");
                    }
                }
            } else {
                //MessageBox.Show("Data Validation failed");
            }
            lStatus.Text = "";
        }
        String getPIMSData() {
            String cRet;
            DataRow cR;
            DataSet pimlData;
            StreamReader cRetReader;
            cRet = "";
            pimlData = new DataSet("pimlData");
            cRetReader = callMFGService(cbsystem.Text, "wsas003", cbsystem.Text);
            try {
                pimlData.ReadXml(cRetReader);
                if (pimlData.Tables.IndexOf("row") >= 0)
                {
                    if (pimlData.Tables["row"].Rows.Count > 0) {
                        cR = pimlData.Tables["Row"].Rows[0];
                        cRet = cR.ItemArray[0].ToString();
                    }
                } 
            } catch (Exception serEx) { MessageBox.Show("PIMS Label Service Error:\n" + serEx.Message.ToString(), "System Message"); }
            return cRet;
        }
        void updData() {
            String cQuery,cTransID,cTransLine,cPIMSNumber;
            DataGridViewRow cR = new DataGridViewRow();
            DataGridViewRow cR1 = new DataGridViewRow();
            List<String> lPIMSData = new List<String>();

            int i;
            cR = dataGridView1.CurrentRow;
            if (cR == null)
                return;

            String[] cRec = new String[cR.Cells.Count];
            for (i = 0; i <= cR.Cells.Count - 1; i += 1) {
                cRec[i] = cR.Cells[i].Value.ToString();
            }
            cR1 = dataGridView2.CurrentRow;

            if (cR1 != null) {
                cTransID = cR1.Cells[11].Value.ToString();
                cTransLine = getLastLine(cTransID);
            } else {
                cTransID = getLastRec();
                cTransLine = "001";
            }
            cPIMSNumber = "tmpPIMS";
            //cPIMSNumber = getPIMSData();
            cQuery = "Insert into PIMLDetail (SystemID,TransID,TransLine,DNNo,DNDate,VendorID,PONo,POLine,PartNumber,DNQty,LineQty,LotNo,RIRNo,MFGPartNumber,ExpDate,DateCode, " +
            " t_site,t_urg,t_loc,t_msd,t_cust_part,t_shelf_life,t_wt,t_wt_ind,t_conn,mfgDate,PIMSNumber) " +
            " values('" + cbsystem.Text + "','" + cTransID + "','" + cTransLine + "','" + cRec[0] + "','" + cRec[9] + "','" + cRec[2] + "','" + cRec[3] + "','" + cRec[1] + "','" + cRec[4] + "','" + cRec[8] + "','" + tfrecqty.Text + "','" + tflotno.Text + "','" + tfrirno.Text + "','" + tfmfgpart.Text + "','" + tfexpiredate.Text + "','" + tfdatecode.Text + "', " +
            " '" + cRec[10] + "','" + cRec[11] + "','" + cRec[12] + "','" + cRec[13] + "','" + cRec[14] + "','" + cRec[15] + "','" + cRec[16] + "','" + cRec[17] + "','" + cRec[18] + "','" + tfmfgdate.Text + "','" + cPIMSNumber + "') ";

            try {
                int cPrintLoop;
                int cNoOfLabels;
                cPrintLoop = 1;
                cNoOfLabels = Convert.ToInt32(tfnooflabels.Text);
                while (cPrintLoop <= cNoOfLabels)
                {
                    cPIMSNumber = getPIMSData();
                    lPIMSData = updateMFGPro(cPIMSNumber);
                    if (lPIMSData.Count > 0){
                        if (lPIMSData[0].ToString() == "-2") { MessageBox.Show("Must Input Date Code or Lot No"); }
                        else { printPIML(lPIMSData);}
                    }
                    cPrintLoop += 1;
                }
                if (lPIMSData.Count > 0)
                {
                   if (lPIMSData[0].ToString() == "-2") { }
                   else  SQLUpdate(cQuery);
                }
                setPIMLData();
            }
            catch (Exception) { }
            finally { }
        }
        void SQLUpdate(String cQuery) {
            try
            {
                using (SqlConnection conn = new SqlConnection(cConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(cQuery, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { MessageBox.Show("SQL Error:"+ex.Message.ToString());}
            finally { }
        }
        List<String> updateMFGPro(String cPIMSNumber) {
            int i;
            String cServiceID;
            StringBuilder cPara = new StringBuilder();
            StreamReader cRetReader;
            DataSet pimsData;
            DataGridViewRow cR = new DataGridViewRow();
            DataRow cDR;
            List<String> lPIMSData = new List<String>();
            cR = dataGridView1.CurrentRow; 
            cServiceID = "wsas002";
            pimsData = new DataSet("pimlData");
            
            /*cPara.Append(cR.Cells["DNSite"].Value.ToString()+","+cR.Cells["PartNumber"].Value.ToString()+
                "," + cR.Cells["RIRNo"].Value.ToString() + ",'',''," + tfrecqty.Text + "," + tfmfgpart.Text + "," + cUserID + "," + tflotno.Text + ",''," +
                tfexpiredate.Text+",'',"+cR.Cells["t_shelf_life"].Value.ToString()+",'YES','NO','R'");*/ 
            cPara.Append(cPIMSNumber+ ","+cR.Cells["RIRNo"].Value.ToString()+","+tfdatecode.Text+","+tfmfgdate.Text+","+tfexpiredate.Text+","+tfrecqty.Text+","+cUserID+","+tflotno.Text);
            cRetReader = callMFGService("WEC", cServiceID,cPara.ToString());
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
                else { 
                
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
            cQuery = "select top 1 TransLine from PIMLDetail where TransID='"+cTransID+"' Order by TransLine desc";
            cRet = "000";
            try
            {
                using (SqlConnection conn = new SqlConnection(cConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(cQuery, conn);
                    myReader = cmd.ExecuteReader();
                    while (myReader.Read()) {
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
        int valData()
        {
            String cErrMsg;
            int cRet;
            DateTime value;
            Double cTemp;
            DateTime cMfgDate;
            DateTime cOldMfgDate;
            tflotno.BackColor = Color.White;
            cOldMfgDate = DateTime.Now.AddDays(-730);
            cRet = 0;
            cErrMsg = "";
            toolTip1.SetToolTip(tfcumqty, "");
            toolTip1.SetToolTip(tfrecqty, "");
            toolTip1.SetToolTip(tfexpiredate, "");
            toolTip1.SetToolTip(tfmfgdate, "");
            toolTip1.SetToolTip(tflotno, "");
            toolTip1.SetToolTip(tfdatecode, "");

            tfcumqty.BackColor = Color.White;
            tfrecqty.BackColor = Color.White;
            tfexpiredate.BackColor = Color.White;
            tfmfgdate.BackColor = Color.White;
            tflotno.BackColor = Color.White;
            tfdatecode.BackColor = Color.White;

            if (!Double.TryParse(tfrecqty.Text, out cTemp)) {
                cRet += 1;
                tfrecqty.BackColor = Color.Red;
                toolTip1.SetToolTip(tfrecqty, "Require Number");
                cErrMsg += "\nRequire Number in received Qty";
            } else {
                tfrecqty.BackColor = Color.White;
                toolTip1.SetToolTip(tfrecqty, "");
            }
            if (!Double.TryParse(tfcumqty.Text, out cTemp)) {
                cRet += 1;
                tfcumqty.BackColor = Color.Red;
                toolTip1.SetToolTip(tfcumqty, "Require Number");
                cErrMsg += "\nInvalid Cumulative Qty";
            } else {
                tfcumqty.BackColor = Color.White;
                toolTip1.SetToolTip(tfcumqty, "");
            }
            if (tfsite.Text.ToUpper() =="MG0337" ){
                if (tflotno.Text.Length == 0) {
                    cRet += 1;
                    tflotno.BackColor = Color.Red;
                    toolTip1.SetToolTip(tflotno, "Lot Number can not be empty for MG0337");
                    cErrMsg += "\nLot Number can not be empty for MG0337";
                }
            }
            if (tfsite.Text.ToUpper() == "MG7024" || tfsite.Text.ToUpper() == "MG5007" || tfsite.Text.ToUpper() == "MG7030" || tfsite.Text.ToUpper() == "MG7029" || tfsite.Text.ToUpper() == "MG5008" || tfsite.Text.ToUpper() == "MG0248" || tfsite.Text.ToUpper() == "MG7028")
            {
                if (tfpartno.Text.Substring(0,1) == "1" || tfpartno.Text.Substring(0,1) == "2" || tfpartno.Text.Substring(0,1) == "3" || tfpartno.Text.Substring(0,1) == "5" || tfpartno.Text.Substring(0,2) == "70"){
                    if (tfdatecode.Text.Length == 0 && tflotno.Text.Length == 0) {
                        cRet += 1;
                        tfdatecode.BackColor = Color.Red;
                        toolTip1.SetToolTip(tfdatecode, "DateCode or Lot Number required for 1x,2x,3x,5x,70x parts");
                        cErrMsg += "\nDateCode or Lot Number required for 1x,2x,3x,5x,70x parts";
                    }
                }
            }
            if (tfmfgdate.Text.Length > 0) {
                if (!DateTime.TryParse(tfmfgdate.Text, out value))
                {
                    cRet += 1;
                    tfmfgdate.BackColor = Color.Red;
                    toolTip1.SetToolTip(tfmfgdate, "Invalid Date");
                    cErrMsg += "\nInvalid Date in Mfgr Date";
                }else {
                    tfmfgdate.Text = Convert.ToDateTime(tfmfgdate.Text).ToString("MM/dd/yy");
                    cMfgDate = Convert.ToDateTime(tfmfgdate.Text);
                    if (cMfgDate.CompareTo(DateTime.Now) > 0 )
                    {
                        cRet += 1;
                        tfmfgdate.BackColor = Color.Red;
                        toolTip1.SetToolTip(tfmfgdate, "Should not be later than today");
                        cErrMsg += "\nMfgr Date should not be later than today";
                    }
                    else if (cMfgDate.CompareTo(cOldMfgDate) < 0)
                    {
                        cRet += 1;
                        tfmfgdate.BackColor = Color.Red;
                        toolTip1.SetToolTip(tfmfgdate, "Mfgr Date is too old");
                        cErrMsg += "\nMfgr Date is too old";
                    }
                    else {
                        tfmfgdate.BackColor = Color.White;
                        toolTip1.SetToolTip(tfmfgdate, "");
                    }
                }
            }
            if (tfexpiredate.Text.Length > 0)
            {
                if (!DateTime.TryParse(tfexpiredate.Text, out value))
                {
                    cRet += 1;
                    tfexpiredate.BackColor = Color.Red;
                    toolTip1.SetToolTip(tfexpiredate, "Invalid Date");
                    cErrMsg += "\nInvalid expire date";

                } else {
                    tfexpiredate.BackColor = Color.White;
                    tfexpiredate.Text = Convert.ToDateTime(tfexpiredate.Text).ToString("MM/dd/yy");
                }
            }
            try
            {
                if (Double.TryParse(tfrecqty.Text, out cTemp) && Double.TryParse(tfcumqty.Text, out cTemp) && Double.TryParse(tfdnqty.Text, out cTemp)) {
                    if ((Convert.ToDouble(tfcumqty.Text) + Convert.ToDouble(tfrecqty.Text)) > Convert.ToDouble(tfdnqty.Text))
                    {
                        cRet += 1;
                        tfcumqty.BackColor = Color.Red;
                        toolTip1.SetToolTip(tfcumqty, "PIMS Already printed for all DN QTY");
                        cErrMsg += "\nPIMS Already printed for all DN QTY";
                    }
                    else
                    {
                        tfcumqty.BackColor = Color.White;
                        toolTip1.SetToolTip(tfcumqty, "");
                    }
                }
            }
            catch (Exception) { }
            if (cErrMsg.Length > 0) {
                MessageBox.Show(cErrMsg,"Error Message");
            }
            return cRet;



        }
        void setDataFieldLabel() {
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
            try {
                cImage = lVendorLabelImage[dataGridView3.CurrentRow.Index];
                if (cImage.Length == 0)
                    pb1.ImageLocation = @"C:\tmp\notavailable.png";
                else
                    pb1.Image = getImage(cImage);
            }
            catch (Exception ex) {  }
        }
        private void dataGridView3_SelectionChanged(object sender, EventArgs e) {
            setDataFieldLabel();
        }
        StreamReader callMFGService(String cSystemID,String progID,String cParam) {
            String cRet;
            cRet = "";
            try {
                cRet = MFGProService.GetTable(cSystemID, progID, cParam);
            }
            catch (Exception) { }
            byte[] byteArray = Encoding.ASCII.GetBytes(cRet);
            MemoryStream stream2 = new MemoryStream(byteArray);
            StreamReader cSReader = new StreamReader(stream2);
            return cSReader;
        }
        void getMFGDNData() {
            DataRow cR;
            StreamReader cRetReader;
            dsDNDetail = new DataSet("dsDNDetail");
            cRetReader = callMFGService(cbsystem.Text, "wsas001", tfdnno.Text + "," + tfdndate.Text);
            try{
                dsDNDetail.ReadXml(cRetReader);
            }
            catch (Exception serEx) { MessageBox.Show("MFGPro Service Error:\n" + serEx.Message.ToString(),"System Message");  return; }
            
            int i = 0;
            dataGridView1.Rows.Clear();
            if (dsDNDetail.Tables.Count >= 7) {
                while (i <= dsDNDetail.Tables[6].Rows.Count - 1) {
                    cR = dsDNDetail.Tables[6].Rows[i];
                    dataGridView1.Rows.Add(cR.ItemArray[0], cR.ItemArray[10], cR.ItemArray[7], cR.ItemArray[4], cR.ItemArray[3], cR.ItemArray[9], "",cR.ItemArray[2], cR.ItemArray[6], cR.ItemArray[1], cR.ItemArray[5], cR.ItemArray[11], cR.ItemArray[12], cR.ItemArray[13], cR.ItemArray[14], cR.ItemArray[15], cR.ItemArray[16], cR.ItemArray[17], cR.ItemArray[18]);
                    i += 1;
                }
            } else { 
                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
                dataGridView3.Rows.Clear();
                resetForm();
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
           if (dsAuthors.Tables.IndexOf("Header") >= 0)
               cTemplateType = dsAuthors.Tables["Header"].Rows[0].ItemArray[1].ToString();
           else
               cTemplateType = "Single";
           return lRet;
        }
        void toPrinter(StringBuilder cStringToPrint,String cPIMS) {
            lStatus.Text = "Printing....";
            StreamWriter outputfile = new StreamWriter("c://tmp/piml"+cPIMS+".txt", false, Encoding.UTF8);
            try
            {
                PrinterHandle.LPTControl printHandle = new PrinterHandle.LPTControl(cbport.SelectedItem.ToString());
                if (printHandle.Open()) {
                    printHandle.Write(cStringToPrint.ToString());
                    printHandle.Close();
                } 
                //printHandle.Close(); 
                outputfile.Write(cStringToPrint.ToString()); 
            }
            catch (Exception prEx) { MessageBox.Show("Print Error :\n" + prEx.Message.ToString()); }
            finally { outputfile.Close(); }
        }
        void printPIML(List<String> lPIMSData) {
            StringBuilder cRet = new StringBuilder();
            PIMLPrint pimlPrint = new PIMLPrint();
            String cSelPrinter;
            int cNoLabel;
            DataGridViewRow cR = new DataGridViewRow();
            cR = dataGridView1.CurrentRow;
            cSelPrinter = "1"; 
            cNoLabel = Convert.ToInt32(tfnooflabels.Text);
            cSelPrinter = (cbprintertype.SelectedIndex + 1).ToString();

            try {
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
                            lPIMSData[5].ToString(), tflotno.Text.ToUpper(), lPIMSData[2].ToString(), lPIMSData[3].ToString(),
                            lPIMSData[7].ToString(), tfdnqty.Text, lPIMSData[6].ToString(), lPIMSData[4].ToString(),
                            lPIMSData[9].ToString(), lPIMSData[10].ToString(), lPIMSData[11].ToString(), lPIMSData[12].ToString(),
                            lPIMSData[0].ToString(), lPIMSData[13].ToString(),
                            cSelPrinter, lPIMSData[14].ToString(), lPIMSData[15].ToString(), lPIMSData[15].ToString(),
                            lPIMSData[16].ToString(), cUserID, lPIMSData[16].ToString(), "", 1,tfrirno.Text.ToUpper()
                 );
                toPrinter(cRet, lPIMSData[0].ToString());
            }
            catch (Exception labEr) { MessageBox.Show("Data Error:" + labEr.Message.ToString()); }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            String cSelPrinter, cSelPIMS,cNewPIMS;
            StringBuilder cRet = new StringBuilder();
            PIMLPrint pimlPrint = new PIMLPrint();
            DataGridViewRow cR = new DataGridViewRow();
            DataGridViewRow cR2 = new DataGridViewRow();
            cR = dataGridView1.CurrentRow;
            cR2 = dataGridView2.CurrentRow;
            cSelPrinter = "1";
            cSelPIMS = cR2.Cells["PIMSNumber"].Value.ToString();
            cSelPrinter = (cbprintertype.SelectedIndex + 1).ToString();
            cNewPIMS = cSelPIMS;
            //cNewPIMS = getPIMSData();
            try { 
                cRet = pimlPrint.genPIML(
                    cR.Cells["DNDate"].Value.ToString().Substring(cR.Cells["DNDate"].Value.ToString().Length - 2, 2), 
                    "*IQC", cR2.Cells["dLotNo"].Value.ToString(), tfpartno.Text.ToUpper(), cR.Cells["DNSite"].Value.ToString(),
                    cR2.Cells["dLineQty"].Value.ToString(), tfdnqty.Text, "ref", cR.Cells["t_loc"].Value.ToString(),
                    tfexpiredate.Text, "R", tfmfgpart.Text.ToUpper(), cR.Cells["t_cust_part"].Value.ToString(),
                    cNewPIMS, cR2.Cells["DNDateCode"].Value.ToString(),
                    cSelPrinter, "by", cR.Cells["t_wt_ind"].Value.ToString(), cR.Cells["t_wt"].Value.ToString(), 
                    cR.Cells["t_MSD"].Value.ToString(), cUserID, cR.Cells["t_msd"].Value.ToString(), "",1,tfrirno.Text.ToUpper());
                toPrinter(cRet,cNewPIMS);
                SQLUpdate("update PIMLDetail set PIMSNumber='" + cNewPIMS + "' where PIMSNumber='"+cSelPIMS+"'");
            }catch (Exception labEx) { MessageBox.Show("Data Error:" + labEx.Message.ToString()); }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {   if(dsDNDetail.Tables.Count>=7)
                setGV1();
        }
        void setGV1() {
            int i = 0;
            DataRow cR;
            dataGridView1.Rows.Clear();
            while (i <= dsDNDetail.Tables[6].Rows.Count - 1)
            {
                cR = dsDNDetail.Tables[6].Rows[i];
                if (cR.ItemArray[9].ToString().ToUpper().StartsWith(textBox2.Text.ToUpper()) || textBox2.Text.Length == 0) {
                    dataGridView1.Rows.Add(cR.ItemArray[0], cR.ItemArray[10], cR.ItemArray[7], cR.ItemArray[4], cR.ItemArray[3], cR.ItemArray[9], "",cR.ItemArray[2], cR.ItemArray[6], cR.ItemArray[1], cR.ItemArray[5], cR.ItemArray[11], cR.ItemArray[12], cR.ItemArray[13], cR.ItemArray[14], cR.ItemArray[15], cR.ItemArray[16], cR.ItemArray[17], cR.ItemArray[18]);
                }
                i += 1;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            
        }
        Image getImage(byte[] cByte) {
            MemoryStream ms = new MemoryStream(cByte);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

    }
    public class vendorLabelDefinition
    {
        private String FieldName, Prefix,RecQty,ExpireDate,MfgDate,Seperator,Index;
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
    /* public class LPTControl
    {
        private string LptStr = "lpt1";
        public LPTControl(string l_LPT_Str)
        {
            LptStr = l_LPT_Str;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct OVERLAPPED
        {
            int Internal;
            int InternalHigh;
            int Offset;
            int OffSetHigh;
            int hEvent;
        }
        [DllImport("kernel32.dll")]
        private static extern int CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            int dwShareMode,
            int lpSecurityAttributes,
            int dwCreationDisposition,
            int dwFlagsAndAttributes,
            int hTemplateFile
            );
        [DllImport("kernel32.dll")]
        private static extern bool WriteFile(
            int hFile,
            byte[] lpBuffer,
            int nNumberOfBytesToWrite,
            ref int lpNumberOfBytesWritten,
            ref OVERLAPPED lpOverlapped
            );
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(
            int hObject
            );
        private int iHandle;
        public bool Open()
        {
            iHandle = CreateFile(LptStr, 0x40000000, 0, 0, 3, 0, 0);
            if (iHandle != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Write(String Mystring)
        {
            if (iHandle != -1)
            {
                OVERLAPPED x = new OVERLAPPED();
                int i = 0;

                byte[] mybyte = System.Text.Encoding.Default.GetBytes(Mystring);
                bool b = WriteFile(iHandle, mybyte, mybyte.Length, ref i, ref x);
                return b;
            }
            else
            {
                throw new Exception("");
            }
        }
        public bool Write(byte[] mybyte)
        {
            if (iHandle != -1)
            {
                OVERLAPPED x = new OVERLAPPED();
                int i = 0;
                WriteFile(iHandle, mybyte, mybyte.Length,
                    ref i, ref x);
                return true;
            }
            else
            {
                throw new Exception("!");
            }
        }
        public bool Close()
        {
            return CloseHandle(iHandle);
        }
    } */

}