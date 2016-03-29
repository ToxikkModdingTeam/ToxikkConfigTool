using System;
using System.IO;
using System.Windows.Forms;
using ToxikkConfigTool.Properties;

namespace ToxikkConfigTool
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
      this.txtPath.Text = Settings.Default.ToxikkConfigDir;
    }

    protected override void OnClosed(EventArgs e)
    {
      Settings.Default.ToxikkConfigDir = this.txtPath.Text;
      Settings.Default.Save();
      base.OnClosed(e);
    }

    private void btnFileDialog_Click(object sender, EventArgs e)
    {
      using (var dlg = new OpenFileDialog())
      {
        dlg.CheckFileExists = true;
        dlg.RestoreDirectory = true;
        dlg.InitialDirectory = this.txtPath.Text;
        dlg.FileName = "DefaultGame.ini";
        dlg.Filter = "Toxikk Game Config|DefaultGame.ini";
        dlg.FilterIndex = 0;
        if (dlg.ShowDialog() == DialogResult.OK)
          this.txtPath.Text = dlg.FileName;
      }
    }

    private void btnFixTimestamps_Click(object sender, EventArgs e)
    {
      if (!ValidateConfigFolder())
        return;
      var fixer = new IniFixer(Path.GetDirectoryName(this.txtPath.Text));
      fixer.FixTimestamps();
    }

    private void btnGenerateUdkIni_Click(object sender, EventArgs e)
    {
      if (!ValidateConfigFolder())
        return;
      var fixer = new IniFixer(Path.GetDirectoryName(this.txtPath.Text));
      fixer.GenerateUdkConfigFiles();
    }

    private void btnUpgrade_Click(object sender, EventArgs e)
    {
      if (!ValidateConfigFolder())
        return;
      var fixer = new IniFixer(Path.GetDirectoryName(this.txtPath.Text));
      fixer.Upgrade();
    }

    private bool ValidateConfigFolder()
    {
      var path = this.txtPath.Text;
      if (string.IsNullOrEmpty(path) || !File.Exists(path))
      {
        this.txtPath.Focus();
        return false;
      }
      return true;
    }

  }
}
