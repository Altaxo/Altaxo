#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

using Altaxo.Serialization;


namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for ArrangeLayersControl.
  /// </summary>
  [UserControlForController(typeof(IArrangeLayersViewEventSink))]
  public class ArrangeLayersControl : System.Windows.Forms.UserControl, IArrangeLayersView
  {
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox _edNumberOfRows;
    private System.Windows.Forms.TextBox _edNumberOfColumns;
    private System.Windows.Forms.TextBox _edHorizontalSpacing;
    private System.Windows.Forms.TextBox _edVerticalSpacing;
    private System.Windows.Forms.TextBox _edTopMargin;
    private System.Windows.Forms.TextBox _edLeftMargin;
    private System.Windows.Forms.TextBox _edBottomMargin;
    private System.Windows.Forms.TextBox _edRightMargin;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public ArrangeLayersControl()
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
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this._edNumberOfRows = new System.Windows.Forms.TextBox();
      this._edNumberOfColumns = new System.Windows.Forms.TextBox();
      this._edHorizontalSpacing = new System.Windows.Forms.TextBox();
      this._edVerticalSpacing = new System.Windows.Forms.TextBox();
      this._edTopMargin = new System.Windows.Forms.TextBox();
      this._edLeftMargin = new System.Windows.Forms.TextBox();
      this._edBottomMargin = new System.Windows.Forms.TextBox();
      this._edRightMargin = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(16, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(112, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "Number of rows:";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(16, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(112, 16);
      this.label2.TabIndex = 1;
      this.label2.Text = "Number of columns:";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(16, 88);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(112, 16);
      this.label3.TabIndex = 2;
      this.label3.Text = "Horizontal spacing (%):";
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(16, 120);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(112, 16);
      this.label4.TabIndex = 3;
      this.label4.Text = "Vertical spacing (%):";
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(16, 160);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(112, 16);
      this.label5.TabIndex = 4;
      this.label5.Text = "Top margin (%):";
      // 
      // label6
      // 
      this.label6.Location = new System.Drawing.Point(16, 192);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(112, 16);
      this.label6.TabIndex = 5;
      this.label6.Text = "Left margin (%):";
      // 
      // label7
      // 
      this.label7.Location = new System.Drawing.Point(16, 232);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(112, 16);
      this.label7.TabIndex = 6;
      this.label7.Text = "Bottom margin (%):";
      // 
      // label8
      // 
      this.label8.Location = new System.Drawing.Point(16, 264);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(112, 16);
      this.label8.TabIndex = 7;
      this.label8.Text = "Right margin (%):";
      // 
      // _edNumberOfRows
      // 
      this._edNumberOfRows.Location = new System.Drawing.Point(144, 16);
      this._edNumberOfRows.Name = "_edNumberOfRows";
      this._edNumberOfRows.TabIndex = 8;
      this._edNumberOfRows.Text = "1";
      this._edNumberOfRows.Validating += new System.ComponentModel.CancelEventHandler(this._edNumberOfRows_Validating);
      // 
      // _edNumberOfColumns
      // 
      this._edNumberOfColumns.Location = new System.Drawing.Point(144, 51);
      this._edNumberOfColumns.Name = "_edNumberOfColumns";
      this._edNumberOfColumns.TabIndex = 9;
      this._edNumberOfColumns.Text = "1";
      this._edNumberOfColumns.Validating += new System.ComponentModel.CancelEventHandler(this._edNumberOfColumns_Validating);
      // 
      // _edHorizontalSpacing
      // 
      this._edHorizontalSpacing.Location = new System.Drawing.Point(144, 86);
      this._edHorizontalSpacing.Name = "_edHorizontalSpacing";
      this._edHorizontalSpacing.TabIndex = 10;
      this._edHorizontalSpacing.Text = "0";
      this._edHorizontalSpacing.Validating += new System.ComponentModel.CancelEventHandler(this._edHorizontalSpacing_Validating);
      // 
      // _edVerticalSpacing
      // 
      this._edVerticalSpacing.Location = new System.Drawing.Point(144, 121);
      this._edVerticalSpacing.Name = "_edVerticalSpacing";
      this._edVerticalSpacing.TabIndex = 11;
      this._edVerticalSpacing.Text = "0";
      this._edVerticalSpacing.Validating += new System.ComponentModel.CancelEventHandler(this._edVerticalSpacing_Validating);
      // 
      // _edTopMargin
      // 
      this._edTopMargin.Location = new System.Drawing.Point(144, 156);
      this._edTopMargin.Name = "_edTopMargin";
      this._edTopMargin.TabIndex = 12;
      this._edTopMargin.Text = "10";
      this._edTopMargin.Validating += new System.ComponentModel.CancelEventHandler(this._edTopMargin_Validating);
      // 
      // _edLeftMargin
      // 
      this._edLeftMargin.Location = new System.Drawing.Point(144, 191);
      this._edLeftMargin.Name = "_edLeftMargin";
      this._edLeftMargin.TabIndex = 13;
      this._edLeftMargin.Text = "10";
      this._edLeftMargin.Validating += new System.ComponentModel.CancelEventHandler(this._edLeftMargin_Validating);
      // 
      // _edBottomMargin
      // 
      this._edBottomMargin.Location = new System.Drawing.Point(144, 226);
      this._edBottomMargin.Name = "_edBottomMargin";
      this._edBottomMargin.TabIndex = 14;
      this._edBottomMargin.Text = "10";
      this._edBottomMargin.Validating += new System.ComponentModel.CancelEventHandler(this._edBottomMargin_Validating);
      // 
      // _edRightMargin
      // 
      this._edRightMargin.Location = new System.Drawing.Point(144, 261);
      this._edRightMargin.Name = "_edRightMargin";
      this._edRightMargin.TabIndex = 15;
      this._edRightMargin.Text = "10";
      this._edRightMargin.Validating += new System.ComponentModel.CancelEventHandler(this._edRightMargin_Validating);
      // 
      // ArrangeLayersControl
      // 
      this.Controls.Add(this._edRightMargin);
      this.Controls.Add(this._edBottomMargin);
      this.Controls.Add(this._edLeftMargin);
      this.Controls.Add(this._edTopMargin);
      this.Controls.Add(this._edVerticalSpacing);
      this.Controls.Add(this._edHorizontalSpacing);
      this.Controls.Add(this._edNumberOfColumns);
      this.Controls.Add(this._edNumberOfRows);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Name = "ArrangeLayersControl";
      this.Size = new System.Drawing.Size(256, 296);
      this.ResumeLayout(false);

    }
    #endregion

    #region IArrangeLayersView Members

    IArrangeLayersViewEventSink _controller;
    public IArrangeLayersViewEventSink Controller
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

    public void InitializeRowsColumns(int numRows, int numColumns)
    {
      this._edNumberOfRows.Text = GUIConversion.ToString(numRows);
      this._edNumberOfColumns.Text = GUIConversion.ToString(numColumns);
    }

    public void InitializeSpacing(double horzSpacing, double vertSpacing)
    {
      this._edHorizontalSpacing.Text = GUIConversion.ToString(horzSpacing);
      this._edVerticalSpacing.Text = GUIConversion.ToString(vertSpacing);
    }

    public void InitializeMargins(double top, double left, double bottom, double right)
    {
      this._edTopMargin.Text =  GUIConversion.ToString(top);
      this._edBottomMargin.Text =  GUIConversion.ToString(bottom);
      this._edRightMargin.Text =  GUIConversion.ToString(right);
      this._edLeftMargin.Text =  GUIConversion.ToString(left);
    }

    #endregion

    private void _edNumberOfRows_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhNumberOfRowsChanged(this._edNumberOfRows.Text);
    }

    private void _edNumberOfColumns_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhNumberOfColumnsChanged(this._edNumberOfColumns.Text);
    
    }

    private void _edHorizontalSpacing_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhHorizontalSpacingChanged(this._edHorizontalSpacing.Text);

    }

    private void _edVerticalSpacing_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhVerticalSpacingChanged(this._edVerticalSpacing.Text);
    }

    private void _edTopMargin_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhTopMarginChanged(this._edTopMargin.Text);

    }

    private void _edLeftMargin_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhLeftMarginChanged(this._edLeftMargin.Text);
    
    }

    private void _edBottomMargin_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhBottomMarginChanged(this._edBottomMargin.Text);
    
    }

    private void _edRightMargin_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhRightMarginChanged(this._edRightMargin.Text);
    
    }
  }
}
