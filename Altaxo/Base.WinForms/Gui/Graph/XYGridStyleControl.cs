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



namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for XYGridStyleControl.
  /// </summary>
  [UserControlForController(typeof(IXYGridStyleViewEventSink))]
  public class XYGridStyleControl : System.Windows.Forms.UserControl, IXYGridStyleView
  {
    private System.Windows.Forms.CheckBox _cbEnable;
    private System.Windows.Forms.CheckBox _cbShowMinor;
    private System.Windows.Forms.CheckBox _cbShowZeroOnly;
    private Altaxo.Gui.Common.Drawing.ColorTypeThicknessPenControl _majorStyle;
    private Altaxo.Gui.Common.Drawing.ColorTypeThicknessPenControl _minorStyle;
    private TableLayoutPanel _tableLayout;
    private IContainer components;

    public XYGridStyleControl()
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
      this.components = new System.ComponentModel.Container();
      this._majorStyle = new Altaxo.Gui.Common.Drawing.ColorTypeThicknessPenControl();
      this._minorStyle = new Altaxo.Gui.Common.Drawing.ColorTypeThicknessPenControl();
      this._cbEnable = new System.Windows.Forms.CheckBox();
      this._cbShowMinor = new System.Windows.Forms.CheckBox();
      this._cbShowZeroOnly = new System.Windows.Forms.CheckBox();
      this._tableLayout = new System.Windows.Forms.TableLayoutPanel();
      this._tableLayout.SuspendLayout();
      this.SuspendLayout();
      // 
      // _majorStyle
      // 
      this._majorStyle.AutoSize = true;
      this._majorStyle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._majorStyle.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._majorStyle.Controller = null;
      this._majorStyle.DocPen = null;
      this._majorStyle.Location = new System.Drawing.Point(3, 49);
      this._majorStyle.Name = "_majorStyle";
      this._majorStyle.Size = new System.Drawing.Size(198, 87);
      this._majorStyle.TabIndex = 0;
      // 
      // _minorStyle
      // 
      this._minorStyle.AutoSize = true;
      this._minorStyle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._minorStyle.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._minorStyle.Controller = null;
      this._minorStyle.DocPen = null;
      this._minorStyle.Location = new System.Drawing.Point(207, 49);
      this._minorStyle.Name = "_minorStyle";
      this._minorStyle.Size = new System.Drawing.Size(198, 87);
      this._minorStyle.TabIndex = 1;
      // 
      // _cbEnable
      // 
      this._cbEnable.AutoSize = true;
      this._cbEnable.Location = new System.Drawing.Point(3, 3);
      this._cbEnable.Name = "_cbEnable";
      this._cbEnable.Size = new System.Drawing.Size(59, 17);
      this._cbEnable.TabIndex = 2;
      this._cbEnable.Text = "Enable";
      this._cbEnable.CheckedChanged += new System.EventHandler(this._cbEnable_CheckedChanged);
      // 
      // _cbShowMinor
      // 
      this._cbShowMinor.AutoSize = true;
      this._cbShowMinor.Location = new System.Drawing.Point(207, 26);
      this._cbShowMinor.Name = "_cbShowMinor";
      this._cbShowMinor.Size = new System.Drawing.Size(101, 17);
      this._cbShowMinor.TabIndex = 3;
      this._cbShowMinor.Text = "Show minor grid";
      this._cbShowMinor.CheckedChanged += new System.EventHandler(this._cbShowMinor_CheckedChanged);
      // 
      // _cbShowZeroOnly
      // 
      this._cbShowZeroOnly.AutoSize = true;
      this._cbShowZeroOnly.Location = new System.Drawing.Point(3, 26);
      this._cbShowZeroOnly.Name = "_cbShowZeroOnly";
      this._cbShowZeroOnly.Size = new System.Drawing.Size(81, 17);
      this._cbShowZeroOnly.TabIndex = 4;
      this._cbShowZeroOnly.Text = "At zero only";
      this._cbShowZeroOnly.CheckedChanged += new System.EventHandler(this._cbShowZeroOnly_CheckedChanged);
      // 
      // _tableLayout
      // 
      this._tableLayout.AutoSize = true;
      this._tableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._tableLayout.ColumnCount = 2;
      this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._tableLayout.Controls.Add(this._majorStyle, 0, 2);
      this._tableLayout.Controls.Add(this._cbEnable, 0, 0);
      this._tableLayout.Controls.Add(this._cbShowMinor, 1, 1);
      this._tableLayout.Controls.Add(this._minorStyle, 1, 2);
      this._tableLayout.Controls.Add(this._cbShowZeroOnly, 0, 1);
      this._tableLayout.Location = new System.Drawing.Point(0, 3);
      this._tableLayout.Name = "_tableLayout";
      this._tableLayout.RowCount = 3;
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.Size = new System.Drawing.Size(408, 139);
      this._tableLayout.TabIndex = 5;
      // 
      // XYGridStyleControl
      // 
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this._tableLayout);
      this.Name = "XYGridStyleControl";
      this.Size = new System.Drawing.Size(411, 145);
      this._tableLayout.ResumeLayout(false);
      this._tableLayout.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    #region IXYGridStyleView Members

    IXYGridStyleViewEventSink _controller;
    public IXYGridStyleViewEventSink Controller
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

    public void InitializeBegin()
    {
      this.SuspendLayout();
    }
    public void InitializeEnd()
    {
      this.ResumeLayout();
    }

    public void InitializeMajorGridStyle(Altaxo.Gui.Common.Drawing.IColorTypeThicknessPenController controller)
    {
      controller.ViewObject = this._majorStyle;
    }

    public void InitializeMinorGridStyle(Altaxo.Gui.Common.Drawing.IColorTypeThicknessPenController controller)
    {
      controller.ViewObject = this._minorStyle;
    }

    public void InitializeShowGrid(bool value)
    {
      this._cbEnable.Checked = value;
    }

    public void InitializeShowMinorGrid(bool value)
    {
      this._cbShowMinor.Checked = value;
    }

    public void InitializeShowZeroOnly(bool value)
    {
      this._cbShowZeroOnly.Checked = value;
    }

    public void InitializeElementEnabling(bool majorstyle, bool minorstyle, bool showminor, bool showzeroonly)
    {
      this._majorStyle.Enabled = majorstyle;
      this._minorStyle.Enabled = minorstyle;
      this._cbShowMinor.Enabled = showminor;
      this._cbShowZeroOnly.Enabled = showzeroonly;
    }

    #endregion

    private void _cbEnable_CheckedChanged(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_ShowGridChanged(this._cbEnable.Checked);
    
    }

    private void _cbShowZeroOnly_CheckedChanged(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_ShowZeroOnly(this._cbShowZeroOnly.Checked);
    
    }

    private void _cbShowMinor_CheckedChanged(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_ShowMinorGridChanged(this._cbShowMinor.Checked);
    
    }
  }
}
