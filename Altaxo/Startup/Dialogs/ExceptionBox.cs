// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Kr�ger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1182 $</version>
// </file>

// project created on 2/6/2003 at 11:10 AM
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.Drawing;
using System.Threading;
using System.Globalization;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
  public class ExceptionBox : System.Windows.Forms.Form
  {
    private System.Windows.Forms.TextBox exceptionTextBox;
    private System.Windows.Forms.CheckBox copyErrorCheckBox;
    //private System.Windows.Forms.CheckBox includeSysInfoCheckBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label;
    private System.Windows.Forms.Button continueButton;
    private System.Windows.Forms.Button reportButton;
    private System.Windows.Forms.PictureBox pictureBox;
    Exception exceptionThrown;
    string message;

    public ExceptionBox(Exception e, string message, bool mustTerminate)
    {
      this.exceptionThrown = e;
      this.message = message;
      InitializeComponent();
      if (mustTerminate)
      {
        closeButton.Visible = false;
        continueButton.Text = closeButton.Text;
        continueButton.Left -= closeButton.Width - continueButton.Width;
        continueButton.Width = closeButton.Width;
      }

      exceptionTextBox.Text = getClipboardString();

      try
      {
        ResourceManager resources = new ResourceManager("Resources.BitmapResources", Assembly.GetEntryAssembly());
        this.pictureBox.Image = (Bitmap)resources.GetObject("ErrorReport");
      }
      catch { }
    }

    string getClipboardString()
    {
      string str = "";
      str += ".NET Version        : " + Environment.Version.ToString() + Environment.NewLine;
      str += "OS Version           : " + Environment.OSVersion.ToString() + Environment.NewLine;
      string cultureName = null;
      try
      {
        cultureName = CultureInfo.CurrentCulture.Name;
        str += "Current culture      : " + CultureInfo.CurrentCulture.EnglishName + " (" + cultureName + ")" + Environment.NewLine;
      }
      catch { }
      try
      {
        if (cultureName == null || !cultureName.StartsWith(ResourceService.Language))
        {
          str += "Current UI language  : " + ResourceService.Language + Environment.NewLine;
        }
      }
      catch { }
      try
      {
        if (IntPtr.Size != 4)
        {
          str += "Running as " + (IntPtr.Size * 8) + " bit process" + Environment.NewLine;
        }
        if (SystemInformation.TerminalServerSession)
        {
          str += "Terminal Server Session" + Environment.NewLine;
        }
        if (SystemInformation.BootMode != BootMode.Normal)
        {
          str += "Boot Mode            : " + SystemInformation.BootMode + Environment.NewLine;
        }
      }
      catch { }
      str += "Working Set Memory   : " + (Environment.WorkingSet / 1024) + "kb" + Environment.NewLine;
      Version v = Assembly.GetEntryAssembly().GetName().Version;
#if ModifiedForAltaxo
      str += "Altaxo Version : " + v.Major + "." + v.Minor + "." + v.Build + "." + v.Revision + Environment.NewLine;
#else
      str += "SharpDevelop Version : " + v.Major + "." + v.Minor + "." + v.Build + "." + v.Revision + Environment.NewLine;
#endif
      str += Environment.NewLine;

      if (message != null)
      {
        str += message + Environment.NewLine;
      }
      str += "Exception thrown: " + Environment.NewLine;
      str += exceptionThrown.ToString();
      return str;
    }

    void CopyInfoToClipboard()
    {
      if (copyErrorCheckBox.Checked)
      {
        if (Application.OleRequired() == ApartmentState.STA)
        {
          ClipboardWrapper.SetText(getClipboardString());
        }
        else
        {
          Thread th = new Thread((ThreadStart)delegate
          {
            ClipboardWrapper.SetText(getClipboardString());
          });
          th.SetApartmentState(ApartmentState.STA);
          th.Start();
        }
      }
    }

    void buttonClick(object sender, System.EventArgs e)
    {
      CopyInfoToClipboard();
#if ModifiedForAltaxo
      StartUrl("http://sourceforge.net/tracker/?func=add&group_id=73395&atid=537651");
#else
      StartUrl("http://community.sharpdevelop.net/forums/23/ShowForum.aspx");
#endif

      //Version v = Assembly.GetEntryAssembly().GetName().Version;
      //StartUrl("http://www.icsharpcode.net/OpenSource/SD/BugReporting.aspx?version=" + v.Major + "." + v.Minor + "." + v.Revision + "." + v.Build);

      /*
      string text = "This version of SharpDevelop is an internal build, " +
        "not a released version.\n" +
        "Please report problems in the internal builds to the " +
        "SVN-SharpDevelop-Users mailing list.";
			
      int result = MessageService.ShowCustomDialog("SharpDevelop", text,
                                                   "Join the list", "Write mail", "Cancel");
      if (result == 0) {
        StartUrl("http://www.glengamoi.com/mailman/listinfo/icsharpcode.svn-sharpdevelop-users");
      } else if (result == 1) {
        // clipboard text is too long to be inserted into the mail-url
        string exceptionTitle = "";
        Exception ex = exceptionThrown;
        if (ex != null) {
          try {
            while (ex.InnerException != null) ex = ex.InnerException;
            exceptionTitle = " (" + ex.GetType().Name + ")";
          } catch {}
        }
        string url = "mailto:icsharpcode.svn-sharpdevelop-users@glengamoi.com?subject=Bug Report"
          + Uri.EscapeDataString(exceptionTitle)
          + "&body="
          + Uri.EscapeDataString("Write an English description on how to reproduce the error and paste the exception text.");
        StartUrl(url);
      }
       */
    }

    static void StartUrl(string url)
    {
      try
      {
        Process.Start(url);
      }
      catch (Exception e)
      {
        LoggingService.Warn("Cannot start " + url, e);
      }
    }

    void continueButtonClick(object sender, System.EventArgs e)
    {
      DialogResult = System.Windows.Forms.DialogResult.Ignore;
      Close();
    }

    void CloseButtonClick(object sender, EventArgs e)
    {
#if ModifiedForAltaxo
      if (MessageBox.Show("Do you really want to quit Altaxo?", "Altaxo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
#else
      if (MessageBox.Show("Do you really want to quit SharpDevelop?", "SharpDevelop", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
#endif
      {
        Application.Exit();
      }
    }

    void InitializeComponent()
    {
      this.pictureBox = new System.Windows.Forms.PictureBox();
      // 
      // ExceptionBox
      // 
      this.ClientSize = new System.Drawing.Size(688, 453);
      closeButton = new System.Windows.Forms.Button();
      // 
      // closeButton
      // 
      closeButton.Location = new System.Drawing.Point(454, 424);
      closeButton.Name = "closeButton";
      closeButton.Size = new System.Drawing.Size(140, 23);
      closeButton.TabIndex = 5;
#if ModifiedForAltaxo
      closeButton.Text = "Exit Altaxo";
#else
      closeButton.Text = "Exit SharpDevelop";
#endif
      closeButton.Click += new System.EventHandler(this.CloseButtonClick);
      label3 = new System.Windows.Forms.Label();
      // 
      // label3
      // 
      label3.Location = new System.Drawing.Point(232, 152);
      label3.Name = "label3";
      label3.Size = new System.Drawing.Size(448, 23);
      label3.TabIndex = 9;
#if ModifiedForAltaxo
      label3.Text = "Thank you for helping make Altaxo a better program for everyone!";
#else
      label3.Text = "Thank you for helping make SharpDevelop a better program for everyone!";
#endif
      label2 = new System.Windows.Forms.Label();
      // 
      // label2
      // 
      label2.Location = new System.Drawing.Point(232, 64);
      label2.Name = "label2";
      label2.Size = new System.Drawing.Size(448, 80);
      label2.TabIndex = 8;
#if ModifiedForAltaxo
      label2.Text = "How to report errors efficiently: We have set up a Web-based forum to report and track errors that are reported by users of Altaxo. To minimize necessary questions by the team members, in addition to providing the error message that is copied to the clipboard for easier pasting in the error report, we ask that you provide us with an as detailed as possible step-by-step procedure to reproduce this bug.";
#else
      label2.Text = "How to report errors efficiently: We have set up a Web-based forum to report and track errors that are reported by users of SharpDevelop. To minimize necessary questions by the team members, in addition to providing the error message that is copied to the clipboard for easier pasting in the error report, we ask that you provide us with an as detailed as possible step-by-step procedure to reproduce this bug.";
#endif
      label = new System.Windows.Forms.Label();
      // 
      // label
      // 
      label.Location = new System.Drawing.Point(232, 8);
      label.Name = "label";
      label.Size = new System.Drawing.Size(448, 48);
      label.TabIndex = 6;
#if ModifiedForAltaxo
      label.Text = "An unhandled exception has occurred in Altaxo. This is unexpected and we\'d " +
        "ask you to help us improve Altaxo by reporting this error to the Altaxo team.";
#else
      label.Text = "An unhandled exception has occurred in SharpDevelop. This is unexpected and we\'d " +
        "ask you to help us improve SharpDevelop by reporting this error to the SharpDeve" +
        "lop team.";
#endif
      continueButton = new System.Windows.Forms.Button();
      // 
      // continueButton
      // 
      continueButton.Location = new System.Drawing.Point(600, 424);
      continueButton.Name = "continueButton";
      continueButton.Size = new System.Drawing.Size(75, 23);
      continueButton.TabIndex = 6;
      continueButton.Text = "Continue";
      continueButton.Click += new System.EventHandler(this.continueButtonClick);
      reportButton = new System.Windows.Forms.Button();
      // 
      // reportButton
      // 
      reportButton.Location = new System.Drawing.Point(232, 424);
      reportButton.Name = "reportButton";
      reportButton.Size = new System.Drawing.Size(216, 23);
      reportButton.TabIndex = 4;
#if ModifiedForAltaxo
      reportButton.Text = "Report Error to Altaxo Team";
#else
      reportButton.Text = "Report Error to SharpDevelop Team";
#endif
      reportButton.Click += new System.EventHandler(this.buttonClick);
      copyErrorCheckBox = new System.Windows.Forms.CheckBox();
      // 
      // copyErrorCheckBox
      // 
      copyErrorCheckBox.Checked = true;
      copyErrorCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      copyErrorCheckBox.Location = new System.Drawing.Point(232, 368);
      copyErrorCheckBox.Name = "copyErrorCheckBox";
      copyErrorCheckBox.Size = new System.Drawing.Size(440, 24);
      copyErrorCheckBox.TabIndex = 2;
      copyErrorCheckBox.Text = "Copy error message to clipboard";
      exceptionTextBox = new System.Windows.Forms.TextBox();
      // 
      // exceptionTextBox
      // 
      exceptionTextBox.Location = new System.Drawing.Point(232, 176);
      exceptionTextBox.Multiline = true;
      exceptionTextBox.Name = "exceptionTextBox";
      exceptionTextBox.ReadOnly = true;
      exceptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      exceptionTextBox.Size = new System.Drawing.Size(448, 184);
      exceptionTextBox.TabIndex = 1;
      exceptionTextBox.Text = "textBoxExceptionText";
      pictureBox = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(pictureBox)).BeginInit();
      // 
      // pictureBox
      // 
      pictureBox.Location = new System.Drawing.Point(0, 0);
      pictureBox.Name = "pictureBox";
      pictureBox.Size = new System.Drawing.Size(224, 464);
      pictureBox.TabIndex = 0;
      pictureBox.TabStop = false;
      ((System.ComponentModel.ISupportInitialize)(pictureBox)).EndInit();
      this.Controls.Add(closeButton);
      this.Controls.Add(label3);
      this.Controls.Add(label2);
      this.Controls.Add(label);
      this.Controls.Add(continueButton);
      this.Controls.Add(reportButton);
      this.Controls.Add(copyErrorCheckBox);
      this.Controls.Add(exceptionTextBox);
      this.Controls.Add(pictureBox);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ExceptionBox";
      this.Text = "Unhandled exception has occured";
      this.SuspendLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
    private System.Windows.Forms.Button closeButton;
  }
}
