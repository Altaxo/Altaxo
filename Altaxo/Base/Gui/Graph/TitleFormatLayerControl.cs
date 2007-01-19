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
  /// Summary description for TitleFormatLayerControl.
  /// </summary>
  public class TitleFormatLayerControl : System.Windows.Forms.UserControl, ITitleFormatLayerView
  {
    private System.Windows.Forms.TextBox m_Format_edAxisPositionValue;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.TextBox m_Format_edTitle;
    private System.Windows.Forms.Label label1;
    private AxisLineStyleControl _lineStyleControl;
    private Altaxo.Gui.Common.CheckableGroupBox _axisLineGroupBox;
    private CheckBox _chkShowMajorLabels;
    private CheckBox _chkShowMinorLabels;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public TitleFormatLayerControl()
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
      this.m_Format_edAxisPositionValue = new System.Windows.Forms.TextBox();
      this.label11 = new System.Windows.Forms.Label();
      this.m_Format_edTitle = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this._chkShowMajorLabels = new System.Windows.Forms.CheckBox();
      this._chkShowMinorLabels = new System.Windows.Forms.CheckBox();
      this._axisLineGroupBox = new Altaxo.Gui.Common.CheckableGroupBox();
      this._lineStyleControl = new Altaxo.Gui.Graph.AxisLineStyleControl();
      this._axisLineGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_Format_edAxisPositionValue
      // 
      this.m_Format_edAxisPositionValue.Location = new System.Drawing.Point(319, 3);
      this.m_Format_edAxisPositionValue.Name = "m_Format_edAxisPositionValue";
      this.m_Format_edAxisPositionValue.Size = new System.Drawing.Size(96, 20);
      this.m_Format_edAxisPositionValue.TabIndex = 33;
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(206, 8);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(107, 13);
      this.label11.TabIndex = 25;
      this.label11.Text = "Shift Axis Position (%)";
      // 
      // m_Format_edTitle
      // 
      this.m_Format_edTitle.Location = new System.Drawing.Point(49, 3);
      this.m_Format_edTitle.Name = "m_Format_edTitle";
      this.m_Format_edTitle.Size = new System.Drawing.Size(100, 20);
      this.m_Format_edTitle.TabIndex = 19;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(16, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(27, 13);
      this.label1.TabIndex = 18;
      this.label1.Text = "Title";
      // 
      // _chkShowMajorLabels
      // 
      this._chkShowMajorLabels.AutoSize = true;
      this._chkShowMajorLabels.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this._chkShowMajorLabels.Location = new System.Drawing.Point(38, 29);
      this._chkShowMajorLabels.Name = "_chkShowMajorLabels";
      this._chkShowMajorLabels.Size = new System.Drawing.Size(111, 17);
      this._chkShowMajorLabels.TabIndex = 36;
      this._chkShowMajorLabels.Text = "Show major labels";
      this._chkShowMajorLabels.UseVisualStyleBackColor = true;
      // 
      // _chkShowMinorLabels
      // 
      this._chkShowMinorLabels.AutoSize = true;
      this._chkShowMinorLabels.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this._chkShowMinorLabels.Location = new System.Drawing.Point(38, 52);
      this._chkShowMinorLabels.Name = "_chkShowMinorLabels";
      this._chkShowMinorLabels.Size = new System.Drawing.Size(111, 17);
      this._chkShowMinorLabels.TabIndex = 37;
      this._chkShowMinorLabels.Text = "Show minor labels";
      this._chkShowMinorLabels.UseVisualStyleBackColor = true;
      // 
      // _axisLineGroupBox
      // 
      this._axisLineGroupBox.AutoSize = true;
      this._axisLineGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._axisLineGroupBox.Checked = false;
      this._axisLineGroupBox.Controls.Add(this._lineStyleControl);
      this._axisLineGroupBox.Location = new System.Drawing.Point(3, 75);
      this._axisLineGroupBox.Name = "_axisLineGroupBox";
      this._axisLineGroupBox.Size = new System.Drawing.Size(415, 242);
      this._axisLineGroupBox.TabIndex = 35;
      this._axisLineGroupBox.TabStop = false;
      this._axisLineGroupBox.Text = "Show axis line and ticks";
      this._axisLineGroupBox.CheckedChanged += new System.EventHandler(this.EhShowAxisLine_CheckChanged);
      // 
      // _lineStyleControl
      // 
      this._lineStyleControl.AutoSize = true;
      this._lineStyleControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._lineStyleControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this._lineStyleControl.Enabled = false;
      this._lineStyleControl.LinePen = null;
      this._lineStyleControl.Location = new System.Drawing.Point(3, 16);
      this._lineStyleControl.MajorPen = null;
      this._lineStyleControl.MajorTickLength = 1F;
      this._lineStyleControl.MinorPen = null;
      this._lineStyleControl.MinorTickLength = 1F;
      this._lineStyleControl.Name = "_lineStyleControl";
      this._lineStyleControl.ShowLine = false;
      this._lineStyleControl.Size = new System.Drawing.Size(409, 223);
      this._lineStyleControl.TabIndex = 34;
      // 
      // TitleFormatLayerControl
      // 
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this._chkShowMinorLabels);
      this.Controls.Add(this._chkShowMajorLabels);
      this.Controls.Add(this._axisLineGroupBox);
      this.Controls.Add(this.m_Format_edAxisPositionValue);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.m_Format_edTitle);
      this.Controls.Add(this.label1);
      this.Name = "TitleFormatLayerControl";
      this.Size = new System.Drawing.Size(421, 320);
      this._axisLineGroupBox.ResumeLayout(false);
      this._axisLineGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

  

    #region ITitleFormatLayerView Members

    public event EventHandler ShowAxisLineChanged;

    public bool ShowAxisLine
    {
      get
      {
        return _axisLineGroupBox.Checked;
      }
      set
      {
        _axisLineGroupBox.Checked = value;
      }
    }

    private void EhShowAxisLine_CheckChanged(object sender, EventArgs e)
    {
      if (null != ShowAxisLineChanged)
        ShowAxisLineChanged(this, EventArgs.Empty);

      foreach (Control c in _axisLineGroupBox.Controls)
        c.Enabled = _axisLineGroupBox.Checked;
    }

    public bool ShowMajorLabels
    {
      get
      {
        return _chkShowMajorLabels.Checked;
      }
      set
      {
        _chkShowMajorLabels.Checked = value;
      }
    }

    public bool ShowMinorLabels
    {
      get
      {
        return _chkShowMinorLabels.Checked;
      }
      set
      {
        _chkShowMinorLabels.Checked = value;
      }
    }

    public string AxisTitle
    {
      get
      {
        return m_Format_edTitle.Text;
      }
      set
      {
        m_Format_edTitle.Text = value;
      }
    }

    public object LineStyleView 
    {
      set
      {
        Control ctrl = value as Control;
        if (ctrl == null)
        {
          _axisLineGroupBox.Checked = false;
        }
        else
        {
          _axisLineGroupBox.SuspendLayout();
          _axisLineGroupBox.Controls.Clear();
          ctrl.Dock = DockStyle.Fill;
          _axisLineGroupBox.Controls.Add(ctrl);
          _axisLineGroupBox.Checked = true;
          _axisLineGroupBox.ResumeLayout(false);
          _axisLineGroupBox.PerformLayout();
        }
      }
    }

    public double PositionOffset
    {
      get
      {
        double val=0;
        if (Altaxo.Serialization.GUIConversion.IsDouble(m_Format_edAxisPositionValue.Text, out val))
          return val;
        else
          return 0;
      }
    }
   
   

    #endregion

   
   


  }
}
