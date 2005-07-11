using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Altaxo.Worksheet.GUI
{
	/// <summary>
	/// Summary description for ScriptControl.
	/// </summary>
	public class ScriptControl : System.Windows.Forms.UserControl, IScriptView
	{
    private System.Windows.Forms.Splitter _vertSplitter;
    private System.Windows.Forms.ListBox lbCompilerErrors;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ScriptControl()
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
      this._vertSplitter = new System.Windows.Forms.Splitter();
      this.lbCompilerErrors = new System.Windows.Forms.ListBox();
      this.SuspendLayout();
      // 
      // _vertSplitter
      // 
      this._vertSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
      this._vertSplitter.Location = new System.Drawing.Point(0, 357);
      this._vertSplitter.Name = "_vertSplitter";
      this._vertSplitter.Size = new System.Drawing.Size(408, 3);
      this._vertSplitter.TabIndex = 0;
      this._vertSplitter.TabStop = false;
      // 
      // lbCompilerErrors
      // 
      this.lbCompilerErrors.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.lbCompilerErrors.Location = new System.Drawing.Point(0, 262);
      this.lbCompilerErrors.Name = "lbCompilerErrors";
      this.lbCompilerErrors.Size = new System.Drawing.Size(408, 95);
      this.lbCompilerErrors.TabIndex = 1;
      this.lbCompilerErrors.DoubleClick += new System.EventHandler(this.lbCompilerErrors_DoubleClick);
      // 
      // ScriptControl
      // 
      this.Controls.Add(this.lbCompilerErrors);
      this.Controls.Add(this._vertSplitter);
      this.Name = "ScriptControl";
      this.Size = new System.Drawing.Size(408, 360);
      this.ResumeLayout(false);

    }
		#endregion

    #region IScriptView Members

    IScriptViewEventSink _controller;
    public IScriptViewEventSink Controller
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

    Control _scriptView;
    public void AddPureScriptView(object scriptView)
    {
      if(object.Equals(_scriptView,scriptView))
        return;

      if(null!=_scriptView)
        this.Controls.Remove(_scriptView);
      _scriptView = _scriptView;
      if(null!=_scriptView)
      {
        _scriptView.Location = new Point(0,0);
        _scriptView.Size = new Size(this.ClientSize.Width,this.lbCompilerErrors.Location.Y);
        _scriptView.Dock = DockStyle.Top;
        this.Controls.Add(_scriptView);
      }
    }

    public void ClearCompilerErrors()
    {
      lbCompilerErrors.Items.Clear();
    }

    public void AddCompilerError(string s)
    {
       this.lbCompilerErrors.Items.Add(s);
    }

    #endregion

    private void lbCompilerErrors_DoubleClick(object sender, System.EventArgs e)
    {
      string msg = lbCompilerErrors.SelectedItem as string;

      if(null!=_controller && null!=msg)
        _controller.EhView_GotoCompilerError(msg);
    }
  }
}
