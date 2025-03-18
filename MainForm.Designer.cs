namespace WebScreenshot
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            pnlToolbar = new Panel();
            btnScreenshot = new Button();
            btnGo = new Button();
            txtUrl = new TextBox();
            btnForward = new Button();
            btnBack = new Button();
            pnlBrowser = new Panel();
            pnlToolbar.SuspendLayout();
            SuspendLayout();
            // 
            // pnlToolbar
            // 
            pnlToolbar.Controls.Add(btnScreenshot);
            pnlToolbar.Controls.Add(btnGo);
            pnlToolbar.Controls.Add(txtUrl);
            pnlToolbar.Controls.Add(btnForward);
            pnlToolbar.Controls.Add(btnBack);
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Location = new Point(0, 0);
            pnlToolbar.Margin = new Padding(5);
            pnlToolbar.Name = "pnlToolbar";
            pnlToolbar.Size = new Size(1867, 78);
            pnlToolbar.TabIndex = 0;
            // 
            // btnScreenshot
            // 
            btnScreenshot.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnScreenshot.Location = new Point(1696, 16);
            btnScreenshot.Margin = new Padding(5);
            btnScreenshot.Name = "btnScreenshot";
            btnScreenshot.Size = new Size(156, 52);
            btnScreenshot.TabIndex = 2;
            btnScreenshot.Text = "截图";
            btnScreenshot.UseVisualStyleBackColor = true;
            btnScreenshot.Click += btnScreenshot_Click;
            // 
            // btnGo
            // 
            btnGo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGo.Location = new Point(1524, 16);
            btnGo.Margin = new Padding(5);
            btnGo.Name = "btnGo";
            btnGo.Size = new Size(156, 52);
            btnGo.TabIndex = 1;
            btnGo.Text = "转到";
            btnGo.UseVisualStyleBackColor = true;
            btnGo.Click += btnGo_Click;
            // 
            // txtUrl
            // 
            txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUrl.Location = new Point(196, 16);
            txtUrl.Margin = new Padding(5);
            txtUrl.Multiline = true;
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new Size(1311, 52);
            txtUrl.TabIndex = 0;
            txtUrl.Text = "https://www.52pojie.cn/";
            txtUrl.KeyPress += txtUrl_KeyPress;
            // 
            // btnForward
            // 
            btnForward.Location = new Point(106, 16);
            btnForward.Margin = new Padding(5);
            btnForward.Name = "btnForward";
            btnForward.Size = new Size(80, 52);
            btnForward.TabIndex = 4;
            btnForward.Text = "→";
            btnForward.UseVisualStyleBackColor = true;
            btnForward.Click += btnForward_Click;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(16, 16);
            btnBack.Margin = new Padding(5);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(80, 52);
            btnBack.TabIndex = 3;
            btnBack.Text = "←";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // pnlBrowser
            // 
            pnlBrowser.Dock = DockStyle.Fill;
            pnlBrowser.Location = new Point(0, 78);
            pnlBrowser.Margin = new Padding(5);
            pnlBrowser.Name = "pnlBrowser";
            pnlBrowser.Size = new Size(1867, 1162);
            pnlBrowser.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1867, 1240);
            Controls.Add(pnlBrowser);
            Controls.Add(pnlToolbar);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "【吾爱独家首发】开源网页全屏截图工具 by pikapikapi";
            pnlToolbar.ResumeLayout(false);
            pnlToolbar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlToolbar;
        private Button btnGo;
        private TextBox txtUrl;
        private Panel pnlBrowser;
        private Button btnScreenshot;
        private Button btnBack;
        private Button btnForward;
    }
}
