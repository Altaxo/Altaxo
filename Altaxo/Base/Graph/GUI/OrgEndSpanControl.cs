using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Altaxo.Main.GUI;

namespace Altaxo.Graph.GUI
{
	/// <summary>
	/// Summary description for OrgEndSpanControl.
	/// </summary>
	[UserControlForController(typeof(IOrgEndSpanControlEventReceiver))]
	public class OrgEndSpanControl : System.Windows.Forms.UserControl, IOrgEndSpanControl
	{
    IOrgEndSpanControlEventReceiver _controller;

    private System.Windows.Forms.Label lblLabel1;
    private System.Windows.Forms.TextBox edText1;
    private System.Windows.Forms.ComboBox cbCombo1;
    private System.Windows.Forms.ComboBox cbCombo2;
    private System.Windows.Forms.TextBox edText2;
    private System.Windows.Forms.Label lblLabel2;
    private System.Windows.Forms.ComboBox cbCombo3;
    private System.Windows.Forms.TextBox edText3;
    private System.Windows.Forms.Label lblLabel3;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

    public IOrgEndSpanControlEventReceiver Controller 
    {
      get { return _controller; }
      set { _controller = value; }
    }

		public OrgEndSpanControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      this.lblLabel1 = new System.Windows.Forms.Label();
      this.edText1 = new System.Windows.Forms.TextBox();
      this.cbCombo1 = new System.Windows.Forms.ComboBox();
      this.cbCombo2 = new System.Windows.Forms.ComboBox();
      this.edText2 = new System.Windows.Forms.TextBox();
      this.lblLabel2 = new System.Windows.Forms.Label();
      this.cbCombo3 = new System.Windows.Forms.ComboBox();
      this.edText3 = new System.Windows.Forms.TextBox();
      this.lblLabel3 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lblLabel1
      // 
      this.lblLabel1.Location = new System.Drawing.Point(8, 8);
      this.lblLabel1.Name = "lblLabel1";
      this.lblLabel1.Size = new System.Drawing.Size(64, 20);
      this.lblLabel1.TabIndex = 0;
      this.lblLabel1.Text = "label1";
      // 
      // edText1
      // 
      this.edText1.Location = new System.Drawing.Point(216, 8);
      this.edText1.Name = "edText1";
      this.edText1.TabIndex = 1;
      this.edText1.Text = "textBox1";
      this.edText1.Validating += new System.ComponentModel.CancelEventHandler(this.edText1_Validating);
      // 
      // cbCombo1
      // 
      this.cbCombo1.Location = new System.Drawing.Point(80, 8);
      this.cbCombo1.Name = "cbCombo1";
      this.cbCombo1.Size = new System.Drawing.Size(121, 21);
      this.cbCombo1.TabIndex = 2;
      this.cbCombo1.Text = "comboBox1";
      this.cbCombo1.SelectionChangeCommitted += new System.EventHandler(this.cbCombo1_SelectionChangeCommitted);
      // 
      // cbCombo2
      // 
      this.cbCombo2.Location = new System.Drawing.Point(80, 48);
      this.cbCombo2.Name = "cbCombo2";
      this.cbCombo2.Size = new System.Drawing.Size(121, 21);
      this.cbCombo2.TabIndex = 5;
      this.cbCombo2.Text = "comboBox2";
      this.cbCombo2.SelectionChangeCommitted += new System.EventHandler(this.cbCombo2_SelectionChangeCommitted);
      // 
      // edText2
      // 
      this.edText2.Location = new System.Drawing.Point(216, 48);
      this.edText2.Name = "edText2";
      this.edText2.TabIndex = 4;
      this.edText2.Text = "textBox2";
      this.edText2.Validating += new System.ComponentModel.CancelEventHandler(this.edText2_Validating);
      // 
      // lblLabel2
      // 
      this.lblLabel2.Location = new System.Drawing.Point(8, 48);
      this.lblLabel2.Name = "lblLabel2";
      this.lblLabel2.Size = new System.Drawing.Size(64, 20);
      this.lblLabel2.TabIndex = 3;
      this.lblLabel2.Text = "label2";
      // 
      // cbCombo3
      // 
      this.cbCombo3.Location = new System.Drawing.Point(80, 88);
      this.cbCombo3.Name = "cbCombo3";
      this.cbCombo3.Size = new System.Drawing.Size(121, 21);
      this.cbCombo3.TabIndex = 8;
      this.cbCombo3.Text = "comboBox3";
      this.cbCombo3.SelectionChangeCommitted += new System.EventHandler(this.cbCombo3_SelectionChangeCommitted);
      // 
      // edText3
      // 
      this.edText3.Location = new System.Drawing.Point(216, 88);
      this.edText3.Name = "edText3";
      this.edText3.TabIndex = 7;
      this.edText3.Text = "textBox3";
      this.edText3.Validating += new System.ComponentModel.CancelEventHandler(this.edText3_Validating);
      // 
      // lblLabel3
      // 
      this.lblLabel3.Location = new System.Drawing.Point(8, 88);
      this.lblLabel3.Name = "lblLabel3";
      this.lblLabel3.Size = new System.Drawing.Size(64, 20);
      this.lblLabel3.TabIndex = 6;
      this.lblLabel3.Text = "label3";
      // 
      // OrgEndSpanControl
      // 
      this.Controls.Add(this.cbCombo3);
      this.Controls.Add(this.edText3);
      this.Controls.Add(this.lblLabel3);
      this.Controls.Add(this.cbCombo2);
      this.Controls.Add(this.edText2);
      this.Controls.Add(this.lblLabel2);
      this.Controls.Add(this.cbCombo1);
      this.Controls.Add(this.edText1);
      this.Controls.Add(this.lblLabel1);
      this.Name = "OrgEndSpanControl";
      this.Size = new System.Drawing.Size(328, 120);
      this.ResumeLayout(false);

    }
		#endregion

    #region IOrgEndSpanControl Members

    public void SetLabel1(string txt)
    {
      this.lblLabel1.Text = txt;
    }

    public void SetLabel2(string txt)
    {
      this.lblLabel2.Text = txt;
    }

    public void SetLabel3(string txt)
    {
      this.lblLabel3.Text = txt;
    }

    static void SetChoice(ComboBox cb, string[] choices, int selected)
    {
      cb.Items.Clear();
      cb.Items.AddRange(choices);
      cb.SelectedIndex = selected;
    }

    public void SetChoice1(string[] choices, int selected)
    {
      SetChoice(cbCombo1,choices,selected);
    }

    public void SetChoice2(string[] choices, int selected)
    {
      SetChoice(cbCombo2,choices,selected);
    }

    public void SetChoice3(string[] choices, int selected)
    {
      SetChoice(cbCombo3,choices,selected);
    }

    public void SetValue1(string txt)
    {
      this.edText1.Text = txt;
    }

    public void SetValue2(string txt)
    {
      this.edText2.Text = txt;
    }

    public void SetValue3(string txt)
    {
      this.edText3.Text = txt;
    }

    public void EnableChoiceValue1(bool enable)
    {
      this.cbCombo1.Enabled = enable;
      this.edText1.Enabled = enable;
    }

    public void EnableChoiceValue2(bool enable)
    {
      this.cbCombo2.Enabled = enable;
      this.edText2.Enabled = enable;
    }

    public void EnableChoiceValue3(bool enable)
    {
      this.cbCombo3.Enabled = enable;
      this.edText3.Enabled = enable;
    }

    #endregion

    private void cbCombo1_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhChoice1Changed((string)cbCombo1.SelectedItem,cbCombo1.SelectedIndex);
    }

    private void cbCombo2_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhChoice2Changed((string)cbCombo2.SelectedItem,cbCombo2.SelectedIndex);
    }

    private void cbCombo3_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhChoice3Changed((string)cbCombo3.SelectedItem,cbCombo3.SelectedIndex);
    }

    private void edText1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhValue1Changed(this.edText1.Text);
    }

    private void edText2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhValue2Changed(this.edText2.Text);
    }

    private void edText3_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhValue3Changed(this.edText3.Text);
    }
  }

  public interface IOrgEndSpanControl
  {
    IOrgEndSpanControlEventReceiver Controller { get; set; }

    void SetLabel1(string txt);
    void SetLabel2(string txt);
    void SetLabel3(string txt);
    void SetChoice1(string[] choices, int selected);
    void SetChoice2(string[] choices, int selected);
    void SetChoice3(string[] choices, int selected);
    void SetValue1(string txt);
    void SetValue2(string txt);
    void SetValue3(string txt);

    void EnableChoiceValue1(bool enable);
    void EnableChoiceValue2(bool enable);
    void EnableChoiceValue3(bool enable);

  }
  public interface IOrgEndSpanControlEventReceiver
  {
    void EhChoice1Changed(string txt, int selected);
    void EhChoice2Changed(string txt, int selected);
    void EhChoice3Changed(string txt, int selected);
    bool EhValue1Changed(string txt);
    bool EhValue2Changed(string txt);
    bool EhValue3Changed(string txt);
  }
}
