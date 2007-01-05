#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;



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
