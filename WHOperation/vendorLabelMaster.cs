using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml;
namespace WHOperation
{
    public partial class vendorLabelMaster : Form
    {
        String cConnStr = "Persist Security Info=False;User ID=appuser;pwd=application;Initial Catalog=dbWHOperation;Data Source=142.2.70.81;pooling=true";
        List<byte[]> lLabelImage;
        public vendorLabelMaster()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            this.pictureBox1.MouseHover += new EventHandler(this.pictureBox1_MouseOverHandle);
            this.pictureBox1.MouseLeave += new EventHandler(this.pictureBox1_MouseLeaveHandle);
            base.OnLoad(e);
        }
        private void pictureBox1_MouseOverHandle(object sender, EventArgs e)
        {
            pictureBox1.Height = 300;
            pictureBox1.Width = 250;
            Point x = new Point();
            x.X = groupBox2.Location.X;
            x.Y = groupBox2.Location.Y;
            pictureBox1.Location = x;
        }
        private void pictureBox1_MouseLeaveHandle(object sender, EventArgs e)
        {
            pictureBox1.Height = 115;
            pictureBox1.Width = 150;
            Point x = new Point();
            x.X = 107;
            x.Y = 129;
            pictureBox1.Location = x;
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            setFieldData();
            procSelRowData();
            tfvendor.ReadOnly = true;
            tftemplateid.ReadOnly = true;
            lhMode.Text = "";
            bhSave.Enabled = false;
            lhMode.Text = "";
        }
        void setFieldData() {
            cbdataname.Items.Clear();
            cbdataname.Items.Add("DATECODE");
            cbdataname.Items.Add("MFGRPART");
            cbdataname.Items.Add("MFGDATE");
            cbdataname.Items.Add("EXPIREDATE");
            cbdataname.Items.Add("RECQTY");
            cbdataname.Items.Add("LOTNUMBER");
            cbdataname.Items.Add("DNPARTNUMBER");
        }
        private void bhAdd_Click(object sender, EventArgs e)
        {
            bhSave.Enabled = true;
            lhMode.Text = "A";
            tfvendor.ReadOnly = false;
            tftemplateid.ReadOnly = false;
            dataGridView2.Rows.Clear();
        }
        private void bhEdit_Click(object sender, EventArgs e)
        {
            bhSave.Enabled = true;
            lhMode.Text = "E";
            tfvendor.ReadOnly = true;
            tftemplateid.ReadOnly = true;
        }
        private void bhDelete_Click(object sender, EventArgs e)
        {
            String cVendor, cTemplateID,cQuery;
            DataGridViewRow cRow;
            DialogResult cRet;
            cRow = dataGridView1.CurrentRow;
            if (cRow == null)
                return;
            cVendor = cRow.Cells[0].Value.ToString();
            cTemplateID = cRow.Cells[1].Value.ToString();
            cRet = MessageBox.Show("Are you sure to delete?", "Warning", MessageBoxButtons.YesNo);
            if (cRet == DialogResult.No)
                return;
            using (SqlConnection conn1 = new SqlConnection(cConnStr))
            {
                conn1.Open();
                cQuery = "delete from PIMLVendorTemplate where vendorid='" + cVendor + "' and TemplateID='" + cTemplateID + "'";
                SqlCommand myComm = new SqlCommand(cQuery, conn1);
                myComm.ExecuteNonQuery();
                loadVendorData();
            }
        }
        private void bhSave_Click(object sender, EventArgs e)
        {
            if (saveHeader() == 0) {
                bhSave.Enabled = false;
                tfvendor.ReadOnly = true;
                tftemplateid.ReadOnly = true;
            }
        }
        string valHeader() {
            String cRet,cQuery;
            int cCo;
            SqlDataReader myReader;
            SqlCommand cmdIns;
            cRet = "0";
            cCo = 0;
            using (SqlConnection conn1 = new SqlConnection(cConnStr))
            {
                conn1.Open();
                cQuery = "select count(*) from PIMLVendorTemplate where vendorID='"+tfvendor.Text+"' and templateID='"+tftemplateid.Text+"'";
                cmdIns = new SqlCommand(cQuery,conn1);
                myReader = cmdIns.ExecuteReader();
                while (myReader.Read()) {
                    cCo = myReader.GetInt32(0);
                }
                if (cCo > 0) {
                    cRet = "1";
                }
            }
            return cRet;
        }
        int saveHeader() {
            byte[] cImage;
            String cDefault,cQuery;
            SqlParameter _guidParam;
            SqlCommand cmdIns;
            if (lhMode.Text == "A")
            {
                if (valHeader() != "0")
                {
                    MessageBox.Show("Data validation failed");
                    return 1;
                }
            }
            if (tffilename.Text.Length > 0)
                cImage = File.ReadAllBytes(@tffilename.Text);
            else
                cImage = File.ReadAllBytes(Application.StartupPath + @"\images\NotAvailable.png");

            if (cbdefault.Checked == true)
                cDefault = "Y";
            else
                cDefault = "N";
            using (SqlConnection conn1 = new SqlConnection(cConnStr))
            {
                conn1.Open();
                if (lhMode.Text == "E")
                {
                    cQuery = "update PIMLVendorTemplate set isDefault='" + cDefault + "' where VendorID='" + tfvendor.Text + "' and TemplateID='" + tftemplateid.Text + "' ";
                    cmdIns = new SqlCommand(cQuery, conn1);
                }
                else {
                    cQuery = "insert into PIMLVendorTemplate (VendorID,TemplateID,isDefault,templateImage) values('" + tfvendor.Text + "','" + tftemplateid.Text + "','" + cDefault + "',@fileD) ";
                    _guidParam = new SqlParameter("@fileD", System.Data.SqlDbType.Binary);
                    _guidParam.Value = cImage;
                    cmdIns = new SqlCommand(cQuery, conn1);
                    cmdIns.Parameters.Add(_guidParam);
                }
                cmdIns.ExecuteNonQuery();
                if (lhMode.Text == "E"){
                    if (tffilename.Text.Length > 0)
                    {
                        cQuery = "update PIMLVendorTemplate set templateImage=@fileD where VendorID='" + tfvendor.Text + "' and TemplateID='" + tftemplateid.Text + "' ";
                        cmdIns = new SqlCommand(cQuery, conn1);
                        _guidParam = new SqlParameter("@fileD", System.Data.SqlDbType.Binary);
                        _guidParam.Value = cImage;
                        cmdIns.Parameters.Add(_guidParam);
                        cmdIns.ExecuteNonQuery();
                        pictureBox1.Image = getImage(cImage);
                    }
                }
            }
            tffilename.Text = "";
            loadVendorData();
            return 0;
        }
        void procSelRowData() {
            byte[] cImage;
            int cR;
            DataGridViewRow cRow;
            StringBuilder cXMLData = new StringBuilder();
            if (dataGridView1.RowCount == 0)
            {
                dataGridView2.Rows.Clear();
                return;
            }
            cR = dataGridView1.CurrentRow.Index;
            cRow = dataGridView1.CurrentRow;
            cImage = lLabelImage[cR];
            if (cImage.Length > 0) { }
            else {
                cImage = File.ReadAllBytes(Application.StartupPath + @"\images\notavailable.png");
            }
            pictureBox1.Image = getImage(cImage);
            
            tfvendor.Text = cRow.Cells[0].Value.ToString();
            tftemplateid.Text = cRow.Cells[1].Value.ToString();
            cXMLData.Append(cRow.Cells[3].Value.ToString());
            if (cRow.Cells[2].Value.ToString().Trim().ToUpper() == "Y")
                cbdefault.Checked = true;
            else
                cbdefault.Checked = false;

            //processXML(cXMLData); //old version
            if (cXMLData.Length > 0)
                processXMLv01(cXMLData); // new version
            else 
                dataGridView2.Rows.Clear();
            
        }
        private void bhgetimagefile_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            tffilename.Text = openFileDialog1.FileName;
        }
        void processXMLv01(StringBuilder cXMLData) {
            XmlDocument xmlDoc = new XmlDocument();
            String cLabelType,c2DSeperator;
            xmlDoc.LoadXml(cXMLData.ToString());
            String[] cRec = new String[4];
            XmlNodeList xmlDevices = xmlDoc.SelectNodes("/Header/Field");
            dataGridView2.Rows.Clear();
            foreach (XmlNode device in xmlDevices)
            {
                cRec = new String[4];
                cRec[0] = device.SelectNodes("Name").Item(0).InnerXml;
                cRec[1] = device.SelectNodes("Prefix").Item(0).InnerXml;
                cRec[2] = device.SelectNodes("Seperator").Item(0).InnerXml;
                cRec[3] = device.SelectNodes("Index").Item(0).InnerXml;
                dataGridView2.Rows.Add(cRec);
                if (cbdataname.Items.IndexOf(cRec[0].ToUpper()) >= 0)
                {
                    cbdataname.Items.RemoveAt(cbdataname.Items.IndexOf(cRec[0].ToUpper()));
                }
            }
            cLabelType = "Single"; c2DSeperator = "";
            try {
                xmlDevices = xmlDoc.SelectNodes("/Header/type");
                cLabelType = xmlDevices.Item(0).InnerXml;
                xmlDevices = xmlDoc.SelectNodes("/Header/twodseperator");
                c2DSeperator = xmlDevices.Item(0).InnerXml;
            }
            catch (Exception ex) { }
            tflabeltype.Text = cLabelType;
            tf2dseperator.Text = c2DSeperator;
        }
        void processXML(StringBuilder cXMLData) {
            DataRow cR;
            String[] cRec = new String[4];
            DataSet dsAuthors = new DataSet("Template");
            byte[] byteArray = Encoding.ASCII.GetBytes(cXMLData.ToString());
            MemoryStream stream = new MemoryStream(byteArray);
            StreamReader xx1 = new StreamReader(stream);
            dataGridView2.Rows.Clear();
            try
            {
                dsAuthors.ReadXml(xx1);
            }
            catch (Exception) { 
                return; }
            finally { }
            int i = 0;
            if (dsAuthors.Tables.IndexOf("Field") < 0)
                return;

            while (i <= dsAuthors.Tables["Field"].Rows.Count - 1)
            {
                cRec = new String[4];
                cR = dsAuthors.Tables["Field"].Rows[i];
                cRec[0] = cR.ItemArray[0].ToString();
                cRec[1] = cR.ItemArray[1].ToString();
                cRec[2] = cR.ItemArray[2].ToString();
                cRec[3] = cR.ItemArray[3].ToString();
                dataGridView2.Rows.Add(cRec);
                if (cbdataname.Items.IndexOf(cRec[0].ToUpper()) >= 0) {
                    cbdataname.Items.RemoveAt(cbdataname.Items.IndexOf(cRec[0].ToUpper()));
                }
                i += 1;
            } 
            if (dsAuthors.Tables.IndexOf("Header") >= 0)
                tflabeltype.Text = dsAuthors.Tables["Header"].Rows[0].ItemArray[1].ToString();
            else
                tflabeltype.Text = "Single";
        
        }
        void loadVendorData() {
            String cQuery;
            String[] cRec = new String[4];
            SqlDataReader myReader;
            byte[] cImage;
            dataGridView1.Rows.Clear();
            lLabelImage = new List<byte[]>();
            try {
                if (tfsearchvendor.Text.Length > 0){
                    cQuery = "select VendorID,TemplateID,isDefault,xmlVendorData,templateImage from PIMLVendorTemplate where vendorid like '"+tfsearchvendor.Text +"%' Order by VendorID,TemplateID ";
                    //cQuery = "select VendorID,TemplateID,isDefault,xmlVendorData from PIMLVendorTemplate where vendorid like '" + tfsearchvendor.Text + "%' Order by VendorID,TemplateID ";
                }else{
                    cQuery = "select VendorID,TemplateID,isDefault,xmlVendorData,templateImage from PIMLVendorTemplate Order by VendorID,TemplateID ";
                    //cQuery = "select VendorID,TemplateID,isDefault,xmlVendorData from PIMLVendorTemplate Order by VendorID,TemplateID ";
                }
                using (SqlConnection conn = new SqlConnection(cConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(cQuery, conn);
                    myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        cRec[0] = myReader.GetValue(0).ToString();
                        cRec[1] = myReader.GetValue(1).ToString();
                        cRec[2] = myReader.GetValue(2).ToString();
                        cRec[3] = myReader.GetValue(3).ToString();
                        cImage = new byte[0];
                        //lLabelImage.Add(cImage);
                        //lLabelImage.Add((byte[])myReader[4]);
                        if (myReader[4] is DBNull) {
                            cImage = new byte[0];
                            lLabelImage.Add(cImage);
                        } else {
                            lLabelImage.Add((byte[])myReader[4]);
                        } 
                        dataGridView1.Rows.Add(cRec);
                    }
                    myReader.Close();
                    procSelRowData();
                }
            }
            catch (Exception) { }
            finally { }
        }
        Image getImage(byte[] cByte)
        {
            MemoryStream ms = new MemoryStream(cByte);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            loadVendorData();
        }
        private void bdDelete_Click(object sender, EventArgs e)
        {
            String cField;
            int cR;
            DataGridViewRow cRow;
            cRow = dataGridView2.CurrentRow;
            if (dataGridView2.RowCount ==0)
                return;
            cField = cRow.Cells[0].Value.ToString();
            cR = dataGridView2.CurrentRow.Index;
            dataGridView2.Rows.RemoveAt(cR); 
            cbdataname.Items.Add(cField);
        }

        private void bdSave_Click(object sender, EventArgs e)
        {
            String cField,cPrefix,cSeperator,cIndex;
            Double cTemp;
            if (cbdataname.SelectedItem == null)
                return;
            cField = cbdataname.SelectedItem.ToString();
            cPrefix = tfprefix.Text;
            cSeperator= tfseperator.Text;
            cIndex = tfindex.Text;
            if (cSeperator == " ")
                cSeperator = "SPACE";
            if (cSeperator.Length > 0) {
                if (cIndex.Length == 0)
                {
                    cIndex = "1";
                }
                else {
                    if (!Double.TryParse(cIndex, out cTemp)) {
                        cIndex = "1";
                    }
                }
            }
            cbdataname.Items.RemoveAt(cbdataname.Items.IndexOf(cbdataname.SelectedItem));
            dataGridView2.Rows.Add(cField,cPrefix,cSeperator,cIndex);
            tfprefix.Text = "";
            tfseperator.Text = "";
            tfindex.Text = "";
        }
        private void bdsavexml_Click(object sender, EventArgs e)
        {
            String cXMLData,cPrefix,cFieldName,cQuery,cIndex,cSeperator;
            DataGridViewRow cRow;
            SqlCommand cmdIns;
            int i;
            if (lhMode.Text == "A" || dataGridView1.RowCount == 0) {
                MessageBox.Show("Save Header and Add Data Labels");
                return;
            }
            if (tflabeltype.SelectedIndex < 0) {
                tflabeltype.SelectedIndex = 0;
            }
            cXMLData = "<Header>";
            i = 0;
            while (i <= dataGridView2.Rows.Count - 1) {
                cRow = dataGridView2.Rows[i];
                cFieldName = cRow.Cells[0].Value.ToString();
                cPrefix = cRow.Cells[1].Value.ToString();
                cSeperator = cRow.Cells[2].Value.ToString();
                cIndex = cRow.Cells[3].Value.ToString();
                cPrefix = cPrefix.Replace("<", "&lt;");
                cPrefix = cPrefix.Replace(">", "&gt;");
                //cFieldName = cFieldName.Substring(3, cFieldName.Length - 3);
                cXMLData += "<Field>";
                cXMLData += "<Name>" + cFieldName + "</Name>";
                cXMLData += "<Prefix>" + cPrefix + "</Prefix>";
                cXMLData += "<Seperator>" + cSeperator + "</Seperator>";
                cXMLData += "<Index>" + cIndex + "</Index>";
                cXMLData += "</Field>";
                i += 1;
            }
            cXMLData += "<type>" + tflabeltype.SelectedItem.ToString() + "</type>";
            cXMLData += "<twodseperator>" + @tf2dseperator.Text + "</twodseperator>";
            cXMLData += "</Header>";
            cRow = dataGridView1.CurrentRow;
            cRow.Cells[3].Value = cXMLData;
            cQuery = "update PIMLVendorTemplate set xmlVendorData='"+cXMLData+"' where vendorid='"+tfvendor.Text+"' and templateID='"+tftemplateid.Text+"'";
            using (SqlConnection conn1 = new SqlConnection(cConnStr))
            {
                conn1.Open();
                cmdIns = new SqlCommand(cQuery, conn1);
                cmdIns.ExecuteNonQuery();
            }
        }
    }
}