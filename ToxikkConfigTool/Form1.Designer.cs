namespace ToxikkConfigTool
{
  partial class Form1
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
      this.label1 = new System.Windows.Forms.Label();
      this.txtPath = new System.Windows.Forms.TextBox();
      this.btnFileDialog = new System.Windows.Forms.Button();
      this.btnFixTimestamps = new System.Windows.Forms.Button();
      this.btnGenerateUdkIni = new System.Windows.Forms.Button();
      this.btnUpgrade = new System.Windows.Forms.Button();
      this.btnFixDstTimestamps = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 13);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(257, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Path to TOXIKK\\UDKGame\\Config\\DefaultGame.ini:";
      // 
      // txtPath
      // 
      this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPath.Location = new System.Drawing.Point(13, 30);
      this.txtPath.Name = "txtPath";
      this.txtPath.Size = new System.Drawing.Size(530, 20);
      this.txtPath.TabIndex = 1;
      // 
      // btnFileDialog
      // 
      this.btnFileDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnFileDialog.Location = new System.Drawing.Point(549, 29);
      this.btnFileDialog.Name = "btnFileDialog";
      this.btnFileDialog.Size = new System.Drawing.Size(23, 22);
      this.btnFileDialog.TabIndex = 2;
      this.btnFileDialog.Text = "...";
      this.btnFileDialog.UseVisualStyleBackColor = true;
      this.btnFileDialog.Click += new System.EventHandler(this.btnFileDialog_Click);
      // 
      // btnFixTimestamps
      // 
      this.btnFixTimestamps.Location = new System.Drawing.Point(165, 88);
      this.btnFixTimestamps.Name = "btnFixTimestamps";
      this.btnFixTimestamps.Size = new System.Drawing.Size(105, 23);
      this.btnFixTimestamps.TabIndex = 3;
      this.btnFixTimestamps.Text = "Fix all Timestamps";
      this.btnFixTimestamps.UseVisualStyleBackColor = true;
      this.btnFixTimestamps.Click += new System.EventHandler(this.btnFixTimestamps_Click);
      // 
      // btnGenerateUdkIni
      // 
      this.btnGenerateUdkIni.Location = new System.Drawing.Point(288, 88);
      this.btnGenerateUdkIni.Name = "btnGenerateUdkIni";
      this.btnGenerateUdkIni.Size = new System.Drawing.Size(105, 23);
      this.btnGenerateUdkIni.TabIndex = 4;
      this.btnGenerateUdkIni.Text = "Generate UDK*.ini";
      this.btnGenerateUdkIni.UseVisualStyleBackColor = true;
      this.btnGenerateUdkIni.Visible = false;
      this.btnGenerateUdkIni.Click += new System.EventHandler(this.btnGenerateUdkIni_Click);
      // 
      // btnUpgrade
      // 
      this.btnUpgrade.Location = new System.Drawing.Point(412, 88);
      this.btnUpgrade.Name = "btnUpgrade";
      this.btnUpgrade.Size = new System.Drawing.Size(105, 23);
      this.btnUpgrade.TabIndex = 5;
      this.btnUpgrade.Text = "Upgrade Config";
      this.btnUpgrade.UseVisualStyleBackColor = true;
      this.btnUpgrade.Visible = false;
      this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
      // 
      // btnFixDstTimestamps
      // 
      this.btnFixDstTimestamps.Location = new System.Drawing.Point(13, 88);
      this.btnFixDstTimestamps.Name = "btnFixDstTimestamps";
      this.btnFixDstTimestamps.Size = new System.Drawing.Size(130, 23);
      this.btnFixDstTimestamps.TabIndex = 6;
      this.btnFixDstTimestamps.Text = "Fix DST Timestamps";
      this.btnFixDstTimestamps.UseVisualStyleBackColor = true;
      this.btnFixDstTimestamps.Click += new System.EventHandler(this.btnFixDstTimestamps_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(584, 123);
      this.Controls.Add(this.btnFixDstTimestamps);
      this.Controls.Add(this.btnUpgrade);
      this.Controls.Add(this.btnGenerateUdkIni);
      this.Controls.Add(this.btnFixTimestamps);
      this.Controls.Add(this.btnFileDialog);
      this.Controls.Add(this.txtPath);
      this.Controls.Add(this.label1);
      this.Name = "Form1";
      this.Text = "TOXIKK Config Tool";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtPath;
    private System.Windows.Forms.Button btnFileDialog;
    private System.Windows.Forms.Button btnFixTimestamps;
    private System.Windows.Forms.Button btnGenerateUdkIni;
    private System.Windows.Forms.Button btnUpgrade;
    private System.Windows.Forms.Button btnFixDstTimestamps;
  }
}

