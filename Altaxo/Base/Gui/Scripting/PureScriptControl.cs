using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Altaxo.Main.GUI;

namespace Altaxo.Gui.Scripting
{
	/// <summary>
	/// Summary description for PureScriptControl.
	/// </summary>
	[UserControlForController(typeof(IPureScriptViewEventSink),100)]
	public class PureScriptControl : System.Windows.Forms.UserControl, IPureScriptView
	{
    private System.Windows.Forms.TextBox _edScriptText;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PureScriptControl()
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
      this._edScriptText = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // _edScriptText
      // 
      this._edScriptText.Dock = System.Windows.Forms.DockStyle.Fill;
      this._edScriptText.Location = new System.Drawing.Point(0, 0);
      this._edScriptText.Multiline = true;
      this._edScriptText.Name = "_edScriptText";
      this._edScriptText.Size = new System.Drawing.Size(448, 360);
      this._edScriptText.TabIndex = 0;
      this._edScriptText.Text = "";
      // 
      // PureScriptControl
      // 
      this.Controls.Add(this._edScriptText);
      this.Name = "PureScriptControl";
      this.Size = new System.Drawing.Size(448, 360);
      this.ResumeLayout(false);

    }
		#endregion

    #region IPureScriptView Members

    IPureScriptViewEventSink _controller;
    public IPureScriptViewEventSink Controller
    {
      get
      {
        
        return _controller;
      }
      set
      {
        _controller=value;
      }
    }

    public string ScriptText
    {
      get
      {
        
        return _edScriptText.Text;
      }
      set
      {
        _edScriptText.Text = value;
      }
    }

    public int ScriptCursorLocation
    {
      set
      {
        _edScriptText.Select(value,0);
      }
    }

    public int InitialScriptCursorLocation
    {
      set
      {
        this.ScriptCursorLocation = value;
      }
    }

    public void SetScriptCursorLocation(int line, int column)
    {
      // TODO:  Add PureScriptControl.SetScriptCursorLocation implementation
    }

    public void MarkText(int pos1, int pos2)
    {
      // TODO:  Add PureScriptControl.MarkText implementation
    }

    #endregion
  }
}
