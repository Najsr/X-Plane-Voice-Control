namespace X_Plane_Voice_Control
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonListen = new System.Windows.Forms.Button();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelVoice = new System.Windows.Forms.Label();
            this.comboBoxVoices = new System.Windows.Forms.ComboBox();
            this.comboBoxRecognizer = new System.Windows.Forms.ComboBox();
            this.labelRecognizer = new System.Windows.Forms.Label();
            this.labelConnectionStatus = new System.Windows.Forms.Label();
            this.buttonBind = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonListen
            // 
            this.buttonListen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonListen.AutoSize = true;
            this.buttonListen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonListen.Location = new System.Drawing.Point(263, 102);
            this.buttonListen.Margin = new System.Windows.Forms.Padding(4);
            this.buttonListen.Name = "buttonListen";
            this.buttonListen.Size = new System.Drawing.Size(67, 26);
            this.buttonListen.TabIndex = 0;
            this.buttonListen.Text = "Connect";
            this.buttonListen.UseVisualStyleBackColor = true;
            this.buttonListen.Click += new System.EventHandler(this.ButtonListen_Click);
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(14, 107);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(96, 16);
            this.labelInfo.TabIndex = 1;
            this.labelInfo.Text = "Made by Najsr";
            // 
            // labelVoice
            // 
            this.labelVoice.AutoSize = true;
            this.labelVoice.Location = new System.Drawing.Point(14, 10);
            this.labelVoice.Name = "labelVoice";
            this.labelVoice.Size = new System.Drawing.Size(85, 16);
            this.labelVoice.TabIndex = 2;
            this.labelVoice.Text = "Select voice:";
            // 
            // comboBoxVoices
            // 
            this.comboBoxVoices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxVoices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVoices.FormattingEnabled = true;
            this.comboBoxVoices.Location = new System.Drawing.Point(105, 7);
            this.comboBoxVoices.Name = "comboBoxVoices";
            this.comboBoxVoices.Size = new System.Drawing.Size(226, 24);
            this.comboBoxVoices.TabIndex = 3;
            this.comboBoxVoices.SelectedIndexChanged += new System.EventHandler(this.ComboBoxVoices_SelectedIndexChanged);
            // 
            // comboBoxRecognizer
            // 
            this.comboBoxRecognizer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRecognizer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRecognizer.FormattingEnabled = true;
            this.comboBoxRecognizer.Location = new System.Drawing.Point(136, 37);
            this.comboBoxRecognizer.Name = "comboBoxRecognizer";
            this.comboBoxRecognizer.Size = new System.Drawing.Size(195, 24);
            this.comboBoxRecognizer.TabIndex = 5;
            this.comboBoxRecognizer.SelectedIndexChanged += new System.EventHandler(this.ComboBoxRecognizer_SelectedIndexChanged);
            // 
            // labelRecognizer
            // 
            this.labelRecognizer.AutoSize = true;
            this.labelRecognizer.Location = new System.Drawing.Point(14, 40);
            this.labelRecognizer.Name = "labelRecognizer";
            this.labelRecognizer.Size = new System.Drawing.Size(115, 16);
            this.labelRecognizer.TabIndex = 4;
            this.labelRecognizer.Text = "Select recognizer:";
            // 
            // labelConnectionStatus
            // 
            this.labelConnectionStatus.AutoSize = true;
            this.labelConnectionStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelConnectionStatus.ForeColor = System.Drawing.Color.Red;
            this.labelConnectionStatus.Location = new System.Drawing.Point(14, 74);
            this.labelConnectionStatus.Name = "labelConnectionStatus";
            this.labelConnectionStatus.Size = new System.Drawing.Size(110, 16);
            this.labelConnectionStatus.TabIndex = 6;
            this.labelConnectionStatus.Text = "Not Connected";
            // 
            // buttonBind
            // 
            this.buttonBind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBind.AutoSize = true;
            this.buttonBind.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonBind.Enabled = false;
            this.buttonBind.Location = new System.Drawing.Point(263, 69);
            this.buttonBind.MinimumSize = new System.Drawing.Size(68, 27);
            this.buttonBind.Name = "buttonBind";
            this.buttonBind.Size = new System.Drawing.Size(68, 27);
            this.buttonBind.TabIndex = 7;
            this.buttonBind.Text = "BIND";
            this.buttonBind.UseVisualStyleBackColor = true;
            this.buttonBind.Click += new System.EventHandler(this.ButtonBind_Click);
            this.buttonBind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ButtonBind_KeyDown);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 139);
            this.Controls.Add(this.buttonBind);
            this.Controls.Add(this.labelConnectionStatus);
            this.Controls.Add(this.comboBoxRecognizer);
            this.Controls.Add(this.labelRecognizer);
            this.Controls.Add(this.comboBoxVoices);
            this.Controls.Add(this.labelVoice);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.buttonListen);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "C# Voice control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonListen;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelVoice;
        private System.Windows.Forms.ComboBox comboBoxVoices;
        private System.Windows.Forms.ComboBox comboBoxRecognizer;
        private System.Windows.Forms.Label labelRecognizer;
        private System.Windows.Forms.Label labelConnectionStatus;
        private System.Windows.Forms.Button buttonBind;
    }
}

