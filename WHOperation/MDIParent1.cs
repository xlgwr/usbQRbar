using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Resources;
using System.Data.SqlClient;
namespace WHOperation
{
    public partial class MDIParent1 : Form
    {
        String cConnStr = "Persist Security Info=False;User ID=appuser;pwd=application;Initial Catalog=dbWHOperation;Data Source=142.2.70.81;pooling=true";
        String cVer = "002";
        private int childFormNumber = 0;
        public MDIParent1()
        {
            InitializeComponent();
            if (checkSys() == 0) {
                MessageBox.Show("Your Application is outdated/different!!!\nContact MIS to update latest version","System Message");
            }
            setupMenu("1");
            fLogin childForm = new fLogin(this);
            childForm.MdiParent = this;
            childForm.Show();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            // Create a new instance of the child form.
            Form childForm = new Form();
            // Make it a child of this MDI form before showing it.
            childForm.MdiParent = this;
            childForm.Text = "Window " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
                // TODO: Add code here to save the current contents of the form to a file.
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Use System.Windows.Forms.Clipboard.GetText() or System.Windows.Forms.GetData to retrieve information from the clipboard.
        }

       

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }
        private void pIMSLabelDataCaptureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 childForm = new Form1();
            childForm.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            setupMenu("1");
            fLogin childForm = new fLogin(this);
            childForm.MdiParent = this;            
            childForm.Show();
        }

        private void MDIParent1_Load(object sender, EventArgs e)
        {
            
        }
        public void setupMenu(String cFlag) {
            if (cFlag == "0"){
                m001001.Enabled = true;
                m001002.Enabled = true;
            } else {
                m001001.Enabled = false;
                m001002.Enabled = false;
            }
        }
        int checkSys() {
            String cQuery,cTmp;
            int cRet;
            SqlDataReader myReader;
            cQuery = "select verNo from sysMaster";
            cRet = 0;
            cTmp = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(cConnStr)) {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(cQuery, conn);
                    myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        cTmp = myReader.GetValue(0).ToString();
                    }
                    myReader.Close();
                }
                if (cTmp == cVer)
                    cRet = 1;
            }catch(Exception ){}
            return cRet;
        }

        private void m001002_Click(object sender, EventArgs e)
        {
            vendorLabelMaster childForm = new vendorLabelMaster();
            childForm.Show();
        }
       
    }
}
