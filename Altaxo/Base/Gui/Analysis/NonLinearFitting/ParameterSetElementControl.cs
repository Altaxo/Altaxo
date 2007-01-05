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



namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  /// <summary>
  /// Summary description for ParameterSetControl.
  /// </summary>
  [UserControlForController(typeof(IParameterSetElementViewEventSink))]
  public class ParameterSetElementControl : System.Windows.Forms.UserControl, IParameterSetElementView
  {
    private System.Windows.Forms.Label _lblParameterName;
    private System.Windows.Forms.TextBox _edParameterValue;
    private System.Windows.Forms.CheckBox _chkParameterVaries;
    private System.Windows.Forms.TextBox _edVarianceValue;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public ParameterSetElementControl()
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
      this._lblParameterName = new System.Windows.Forms.Label();
      this._edParameterValue = new System.Windows.Forms.TextBox();
      this._chkParameterVaries = new System.Windows.Forms.CheckBox();
      this._edVarianceValue = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // _lblParameterName
      // 
      this._lblParameterName.Location = new System.Drawing.Point(0, 0);
      this._lblParameterName.Name = "_lblParameterName";
      this._lblParameterName.Size = new System.Drawing.Size(136, 20);
      this._lblParameterName.TabIndex = 0;
      this._lblParameterName.Text = "label1";
      // 
      // _edParameterValue
      // 
      this._edParameterValue.Location = new System.Drawing.Point(152, 0);
      this._edParameterValue.Name = "_edParameterValue";
      this._edParameterValue.Size = new System.Drawing.Size(128, 20);
      this._edParameterValue.TabIndex = 1;
      this._edParameterValue.Text = "textBox1";
      this._edParameterValue.Validating += new System.ComponentModel.CancelEventHandler(this._edParameterValue_Validating);
      // 
      // _chkParameterVaries
      // 
      this._chkParameterVaries.Location = new System.Drawing.Point(288, 0);
      this._chkParameterVaries.Name = "_chkParameterVaries";
      this._chkParameterVaries.Size = new System.Drawing.Size(20, 20);
      this._chkParameterVaries.TabIndex = 2;
      this._chkParameterVaries.CheckedChanged += new System.EventHandler(this._chkParameterVaries_CheckedChanged);
      // 
      // _edVarianceValue
      // 
      this._edVarianceValue.Location = new System.Drawing.Point(312, 0);
      this._edVarianceValue.Name = "_edVarianceValue";
      this._edVarianceValue.Size = new System.Drawing.Size(128, 20);
      this._edVarianceValue.TabIndex = 3;
      this._edVarianceValue.Text = "textBox1";
      this._edVarianceValue.Validating += new System.ComponentModel.CancelEventHandler(this._edVarianceValue_Validating);
      // 
      // ParameterSetElementControl
      // 
      this.Controls.Add(this._edVarianceValue);
      this.Controls.Add(this._chkParameterVaries);
      this.Controls.Add(this._edParameterValue);
      this.Controls.Add(this._lblParameterName);
      this.Name = "ParameterSetElementControl";
      this.Size = new System.Drawing.Size(448, 24);
      this.ResumeLayout(false);

    }
    #endregion

    public  void Initialize(string name, string value, bool vary, string variance)
    {
      this._lblParameterName.Text = name;
      this._edParameterValue.Text = value;
      this._chkParameterVaries.Checked = vary;
      this._edVarianceValue.Text = variance;
    }


    IParameterSetElementViewEventSink _controller;

    public IParameterSetElementViewEventSink Controller 
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

    private void _edParameterValue_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_ParameterValidating(this._edParameterValue.Text,e);    
    }

    private void _chkParameterVaries_CheckedChanged(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_VarySelectionChanged(this._chkParameterVaries.Checked);
    }

    private void _edVarianceValue_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_VarianceValidating(this._edVarianceValue.Text,e);    
    }
  
  }
}
