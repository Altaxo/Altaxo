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

using Altaxo.Gui;
using Altaxo.Gui.Common;

namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Summary description for InterpolationControl.
  /// </summary>
  [UserControlForController(typeof(IInterpolationParameterViewEventSink))]
  public class InterpolationControl : System.Windows.Forms.UserControl, IInterpolationParameterView
  {
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox _cbInterpolationClass;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox _edFrom;
    private System.Windows.Forms.TextBox _edTo;
    private System.Windows.Forms.TextBox _edNumberOfPoints;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public InterpolationControl()
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
      this.label1 = new System.Windows.Forms.Label();
      this._cbInterpolationClass = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this._edFrom = new System.Windows.Forms.TextBox();
      this._edTo = new System.Windows.Forms.TextBox();
      this._edNumberOfPoints = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(8, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(48, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "Using:";
      // 
      // _cbInterpolationClass
      // 
      this._cbInterpolationClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this._cbInterpolationClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbInterpolationClass.Location = new System.Drawing.Point(64, 8);
      this._cbInterpolationClass.Name = "_cbInterpolationClass";
      this._cbInterpolationClass.Size = new System.Drawing.Size(192, 21);
      this._cbInterpolationClass.TabIndex = 1;
      this._cbInterpolationClass.SelectionChangeCommitted += new System.EventHandler(this._cbInterpolationClass_SelectionChangeCommitted);
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(8, 56);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(56, 16);
      this.label2.TabIndex = 2;
      this.label2.Text = "From:";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(8, 88);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(56, 16);
      this.label3.TabIndex = 3;
      this.label3.Text = "To:";
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(8, 120);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(56, 16);
      this.label4.TabIndex = 4;
      this.label4.Text = "No of pts:";
      // 
      // _edFrom
      // 
      this._edFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this._edFrom.Location = new System.Drawing.Point(64, 56);
      this._edFrom.Name = "_edFrom";
      this._edFrom.Size = new System.Drawing.Size(192, 20);
      this._edFrom.TabIndex = 5;
      this._edFrom.Text = "textBox1";
      this._edFrom.Validating += new System.ComponentModel.CancelEventHandler(this._edFrom_Validating);
      // 
      // _edTo
      // 
      this._edTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this._edTo.Location = new System.Drawing.Point(64, 88);
      this._edTo.Name = "_edTo";
      this._edTo.Size = new System.Drawing.Size(192, 20);
      this._edTo.TabIndex = 6;
      this._edTo.Text = "textBox2";
      this._edTo.Validating += new System.ComponentModel.CancelEventHandler(this._edTo_Validating);
      // 
      // _edNumberOfPoints
      // 
      this._edNumberOfPoints.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this._edNumberOfPoints.Location = new System.Drawing.Point(64, 120);
      this._edNumberOfPoints.Name = "_edNumberOfPoints";
      this._edNumberOfPoints.Size = new System.Drawing.Size(192, 20);
      this._edNumberOfPoints.TabIndex = 7;
      this._edNumberOfPoints.Text = "textBox3";
      this._edNumberOfPoints.Validating += new System.ComponentModel.CancelEventHandler(this._edNumberOfPoints_Validating);
      // 
      // InterpolationControl
      // 
      this.Controls.Add(this._edNumberOfPoints);
      this.Controls.Add(this._edTo);
      this.Controls.Add(this._edFrom);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this._cbInterpolationClass);
      this.Controls.Add(this.label1);
      this.Name = "InterpolationControl";
      this.Size = new System.Drawing.Size(264, 152);
      this.ResumeLayout(false);

    }
    #endregion

    private void _cbInterpolationClass_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhValidatingClassName(this._cbInterpolationClass.SelectedIndex);
    }

    private void _edFrom_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)    
        Controller.EhValidatingXOrg(this._edFrom.Text, e);
    }

    private void _edTo_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)    
        Controller.EhValidatingXEnd(this._edTo.Text, e);
    }

    private void _edNumberOfPoints_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)    
        Controller.EhValidatingNumberOfPoints(this._edNumberOfPoints.Text, e);
    }
    #region IInterpolationParameterView Members

    IInterpolationParameterViewEventSink _controller = null;
    public IInterpolationParameterViewEventSink Controller
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

    public void InitializeClassList(string[] classes, int preselection)
    {
      this._cbInterpolationClass.Items.Clear();
      this._cbInterpolationClass.Items.AddRange(classes);
      this._cbInterpolationClass.SelectedIndex = preselection;
    }

    public void InitializeNumberOfPoints(int val)
    {
      this._edNumberOfPoints.Text = Altaxo.Serialization.GUIConversion.ToString(val);
    }

    public void InitializeXOrg(double val)
    {
      this._edFrom.Text = Altaxo.Serialization.GUIConversion.ToString(val);
    }

    public void InitializeXEnd(double val)
    {
      this._edTo.Text = Altaxo.Serialization.GUIConversion.ToString(val);
    }

    UserControl _detailControl;
    public void SetDetailControl(object detailControl)
    {
      UserControl ctrl = (UserControl)detailControl;
      // remove the old control first
      if(_detailControl!=null)
        this.Controls.Remove(_detailControl);

      _detailControl = ctrl;
      if(_detailControl!=null)
      {
        _detailControl.Location = new Point(0,this._edNumberOfPoints.Bounds.Bottom);
        _detailControl.Size = new Size(this.ClientSize.Width,_detailControl.Size.Height);
        _detailControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        this.Controls.Add(_detailControl);
        this.ClientSize = new Size(this.ClientSize.Width,_detailControl.Bounds.Bottom);
      }
    }
    #endregion
  }
}
