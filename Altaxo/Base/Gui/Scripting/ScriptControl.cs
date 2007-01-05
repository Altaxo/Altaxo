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
  /// Summary description for ScriptControl.
  /// </summary>
  [UserControlForController(typeof(IScriptViewEventSink))]
  public class ScriptControl : System.Windows.Forms.UserControl, IScriptView
  {
    private System.Windows.Forms.Splitter _vertSplitter;
    private System.Windows.Forms.ListBox lbCompilerErrors;
    private System.Windows.Forms.Panel _panelText;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public ScriptControl()
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
      this._vertSplitter = new System.Windows.Forms.Splitter();
      this.lbCompilerErrors = new System.Windows.Forms.ListBox();
      this._panelText = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // _vertSplitter
      // 
      this._vertSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
      this._vertSplitter.Location = new System.Drawing.Point(0, 357);
      this._vertSplitter.Name = "_vertSplitter";
      this._vertSplitter.Size = new System.Drawing.Size(408, 3);
      this._vertSplitter.TabIndex = 1;
      this._vertSplitter.TabStop = false;
      // 
      // lbCompilerErrors
      // 
      this.lbCompilerErrors.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.lbCompilerErrors.Location = new System.Drawing.Point(0, 262);
      this.lbCompilerErrors.Name = "lbCompilerErrors";
      this.lbCompilerErrors.Size = new System.Drawing.Size(408, 95);
      this.lbCompilerErrors.TabIndex = 2;
      this.lbCompilerErrors.DoubleClick += new System.EventHandler(this.lbCompilerErrors_DoubleClick);
      // 
      // _panelText
      // 
      this._panelText.Dock = System.Windows.Forms.DockStyle.Fill;
      this._panelText.Location = new System.Drawing.Point(0, 0);
      this._panelText.Name = "_panelText";
      this._panelText.Size = new System.Drawing.Size(408, 262);
      this._panelText.TabIndex = 0;
      // 
      // ScriptControl
      // 
      this.Controls.Add(this._panelText);
      this.Controls.Add(this._vertSplitter);
      this.Controls.Add(this.lbCompilerErrors);
     
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
      if (object.Equals(_scriptView, scriptView))
      {
        return;
      }

      if(null!=_scriptView)
        this._panelText.Controls.Remove(_scriptView);
      _scriptView = (Control)scriptView;
      if(null!=_scriptView)
      {
        _scriptView.Location = new Point(0,0);
        _scriptView.Size = _panelText.Size;
        _scriptView.Dock = DockStyle.Fill;
        this._panelText.Controls.Add(_scriptView);
        this.ActiveControl = _scriptView;
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
