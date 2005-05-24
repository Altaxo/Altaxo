using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Altaxo.Main.GUI
{
  /// <summary>
  /// Summary description for BooleanValueControl.
  /// </summary>
  [UserControlForController(typeof(IBooleanValueViewEventSink))]
  public class BooleanValueControl : System.Windows.Forms.UserControl, IBooleanValueView
  {
    private System.Windows.Forms.CheckBox _cbCheckBox1;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public BooleanValueControl()
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
      this._cbCheckBox1 = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // _cbCheckBox1
      // 
      this._cbCheckBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this._cbCheckBox1.Location = new System.Drawing.Point(8, 8);
      this._cbCheckBox1.Name = "_cbCheckBox1";
      this._cbCheckBox1.Size = new System.Drawing.Size(248, 24);
      this._cbCheckBox1.TabIndex = 0;
      this._cbCheckBox1.Text = "checkBox1";
      this._cbCheckBox1.CheckedChanged += new System.EventHandler(this._cbCheckBox1_CheckedChanged);
      // 
      // BooleanValueControl
      // 
      this.Controls.Add(this._cbCheckBox1);
      this.Name = "BooleanValueControl";
      this.Size = new System.Drawing.Size(264, 32);
      this.ResumeLayout(false);

    }
    #endregion

    #region IBooleanValueView Members

    IBooleanValueViewEventSink _controller;
    public IBooleanValueViewEventSink Controller
    {
      get
      {
        return _controller;
      }
      set
      {
        _controller = value;
      }
    }

    public void InitializeDescription(string value)
    {
      _cbCheckBox1.Text = value;
    }

    public void InitializeBool1(bool value)
    {
      this._cbCheckBox1.Checked = value;
    }

    #endregion

  

    private void _cbCheckBox1_CheckedChanged(object sender, System.EventArgs e)
    {
      if(null!=_controller)
        _controller.EhValidatingBool1(this._cbCheckBox1.Checked);
    }
  }
}
