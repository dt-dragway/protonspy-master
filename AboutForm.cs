using System;
using System.ComponentModel;
using System.Windows.Forms;
using MetroFramework.Forms;
using MetroFramework.Controls;
using MetroFramework;

namespace iSpyApplication
{
    public class AboutForm : MetroForm
    {
        private MetroLabel _lblCopyright;
        private PictureBox _pictureBox1;
        private MetroLabel _lblVersion;
        private MetroButton _btnOk;
        private MetroLink linkLabel1;
        private TableLayoutPanel tableLayoutPanel1;
        private IContainer components = null;

        public AboutForm()
        {
            InitializeComponent();
            this.Theme = MetroThemeStyle.Dark;
            this.Style = MetroColorStyle.Blue;
            RenderResources();
        }

        private void RenderResources()
        {
            _lblVersion.Text = string.Format("{0}{1} v{2}",
                Application.ProductName, Program.Platform != "x86" ? " 64" : "",
                Application.ProductVersion);

            Text = LocRm.GetString("About");

            _lblCopyright.Text = "Copyright \u00a9 2007-" + Helper.Now.Year + " DeveloperInABox";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this._lblCopyright = new MetroLabel();
            this._pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkLabel1 = new MetroLink();
            this._btnOk = new MetroButton();
            this._lblVersion = new MetroLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lblCopyright
            // 
            this._lblCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lblCopyright.Location = new System.Drawing.Point(174, 25);
            this._lblCopyright.Name = "_lblCopyright";
            this._lblCopyright.Size = new System.Drawing.Size(249, 25);
            this._lblCopyright.TabIndex = 13;
            this._lblCopyright.Text = "Copyright © 2016 DeveloperInABox";
            this._lblCopyright.Theme = MetroThemeStyle.Dark;
            // 
            // _pictureBox1
            // 
            this._pictureBox1.Image = global::iSpyApplication.Properties.Resources.ispy;
            this._pictureBox1.Location = new System.Drawing.Point(3, 3);
            this._pictureBox1.Name = "_pictureBox1";
            this.tableLayoutPanel1.SetRowSpan(this._pictureBox1, 5);
            this._pictureBox1.Size = new System.Drawing.Size(152, 142);
            this._pictureBox1.TabIndex = 17;
            this._pictureBox1.TabStop = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(174, 50);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Padding = new System.Windows.Forms.Padding(3);
            this.linkLabel1.Size = new System.Drawing.Size(154, 23);
            this.linkLabel1.TabIndex = 21;
            this.linkLabel1.Text = "www.iSpyConnect.com";
            this.linkLabel1.Click += new System.EventHandler(this.linkLabel1_LinkClicked);
            this.linkLabel1.Theme = MetroThemeStyle.Dark;
            // 
            // _btnOk
            // 
            this._btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnOk.Location = new System.Drawing.Point(333, 78);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(90, 26);
            this._btnOk.TabIndex = 19;
            this._btnOk.Text = "OK";
            this._btnOk.Click += new System.EventHandler(this.BtnOkClick);
            // 
            // _lblVersion
            // 
            this._lblVersion.AutoSize = true;
            this._lblVersion.Location = new System.Drawing.Point(174, 0);
            this._lblVersion.Name = "_lblVersion";
            this._lblVersion.Size = new System.Drawing.Size(56, 17);
            this._lblVersion.TabIndex = 18;
            this._lblVersion.Text = "Version";
            this._lblVersion.Theme = MetroThemeStyle.Dark;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.17595F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.82405F));
            this.tableLayoutPanel1.Controls.Add(this._pictureBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._lblVersion, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this._lblCopyright, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.linkLabel1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this._btnOk, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(20, 60);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(402, 65);
            this.tableLayoutPanel1.TabIndex = 22;
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            // 
            // AboutForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(442, 145);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.AboutFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private void AboutFormLoad(object sender, EventArgs e)
        {

        }

        private void LinkLabel2LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MainForm.StartBrowser(MainForm.Website + "/");
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, EventArgs e)
        {
            MainForm.StartBrowser("http://www.ispyconnect.com");
        }
    }
}