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
  /// Graphical interface for adjusting the style of a DensityImagePlot.
  /// </summary>
  public class DensityImagePlotStyleControl : System.Windows.Forms.UserControl, IDensityImagePlotStyleView
  {
    private IDensityImagePlotStyleViewEventSink m_Ctrl;
    private System.Windows.Forms.Label label24;
    private System.Windows.Forms.Label label23;
    private System.Windows.Forms.TextBox m_edRangeTo;
    private System.Windows.Forms.TextBox m_edRangeFrom;
    private System.Windows.Forms.ComboBox m_cbColorBelow;
    private System.Windows.Forms.ComboBox m_cbColorAbove;
    private System.Windows.Forms.ComboBox m_cbColorInvalid;
    private System.Windows.Forms.ComboBox m_cbScalingStyle;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.CheckBox m_chkClipToLayer;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public DensityImagePlotStyleControl()
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
      this.label24 = new System.Windows.Forms.Label();
      this.m_edRangeTo = new System.Windows.Forms.TextBox();
      this.m_edRangeFrom = new System.Windows.Forms.TextBox();
      this.label23 = new System.Windows.Forms.Label();
      this.m_cbColorBelow = new System.Windows.Forms.ComboBox();
      this.m_cbColorAbove = new System.Windows.Forms.ComboBox();
      this.m_cbColorInvalid = new System.Windows.Forms.ComboBox();
      this.m_cbScalingStyle = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.m_chkClipToLayer = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // label24
      // 
      this.label24.Location = new System.Drawing.Point(64, 104);
      this.label24.Name = "label24";
      this.label24.Size = new System.Drawing.Size(56, 16);
      this.label24.TabIndex = 19;
      this.label24.Text = "To:";
      this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_edRangeTo
      // 
      this.m_edRangeTo.Location = new System.Drawing.Point(128, 104);
      this.m_edRangeTo.Name = "m_edRangeTo";
      this.m_edRangeTo.Size = new System.Drawing.Size(136, 20);
      this.m_edRangeTo.TabIndex = 16;
      this.m_edRangeTo.Text = "";
      this.m_edRangeTo.Validating += new System.ComponentModel.CancelEventHandler(this.EhRangeTo_Validating);
      // 
      // m_edRangeFrom
      // 
      this.m_edRangeFrom.Location = new System.Drawing.Point(128, 72);
      this.m_edRangeFrom.Name = "m_edRangeFrom";
      this.m_edRangeFrom.Size = new System.Drawing.Size(136, 20);
      this.m_edRangeFrom.TabIndex = 15;
      this.m_edRangeFrom.Text = "";
      this.m_edRangeFrom.Validating += new System.ComponentModel.CancelEventHandler(this.EhRangeFrom_Validating);
      // 
      // label23
      // 
      this.label23.Location = new System.Drawing.Point(56, 72);
      this.label23.Name = "label23";
      this.label23.Size = new System.Drawing.Size(64, 16);
      this.label23.TabIndex = 14;
      this.label23.Text = "From:";
      this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_cbColorBelow
      // 
      this.m_cbColorBelow.Location = new System.Drawing.Point(128, 152);
      this.m_cbColorBelow.Name = "m_cbColorBelow";
      this.m_cbColorBelow.Size = new System.Drawing.Size(136, 21);
      this.m_cbColorBelow.TabIndex = 20;
      this.m_cbColorBelow.Text = "comboBox1";
      this.m_cbColorBelow.SelectionChangeCommitted += new System.EventHandler(this.EhColorBelow_SelectionChangeCommitted);
      // 
      // m_cbColorAbove
      // 
      this.m_cbColorAbove.Location = new System.Drawing.Point(128, 184);
      this.m_cbColorAbove.Name = "m_cbColorAbove";
      this.m_cbColorAbove.Size = new System.Drawing.Size(136, 21);
      this.m_cbColorAbove.TabIndex = 21;
      this.m_cbColorAbove.Text = "comboBox1";
      this.m_cbColorAbove.SelectionChangeCommitted += new System.EventHandler(this.EhColorAbove_SelectionChangeCommitted);
      // 
      // m_cbColorInvalid
      // 
      this.m_cbColorInvalid.Location = new System.Drawing.Point(128, 216);
      this.m_cbColorInvalid.Name = "m_cbColorInvalid";
      this.m_cbColorInvalid.Size = new System.Drawing.Size(136, 21);
      this.m_cbColorInvalid.TabIndex = 22;
      this.m_cbColorInvalid.Text = "comboBox1";
      this.m_cbColorInvalid.SelectionChangeCommitted += new System.EventHandler(this.EhColorInvalid_SelectionChangeCommitted);
      // 
      // m_cbScalingStyle
      // 
      this.m_cbScalingStyle.Location = new System.Drawing.Point(128, 40);
      this.m_cbScalingStyle.Name = "m_cbScalingStyle";
      this.m_cbScalingStyle.Size = new System.Drawing.Size(136, 21);
      this.m_cbScalingStyle.TabIndex = 23;
      this.m_cbScalingStyle.Text = "comboBox1";
      this.m_cbScalingStyle.SelectionChangeCommitted += new System.EventHandler(this.EhScalingStyle_SelectionChangedCommitted);
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(64, 40);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(56, 16);
      this.label1.TabIndex = 24;
      this.label1.Text = "Scale:";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(24, 160);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(96, 16);
      this.label2.TabIndex = 25;
      this.label2.Text = "Color below:";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(48, 184);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(72, 16);
      this.label3.TabIndex = 26;
      this.label3.Text = "Color above:";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(8, 216);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(112, 16);
      this.label4.TabIndex = 27;
      this.label4.Text = "Color of invalid point:";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_chkClipToLayer
      // 
      this.m_chkClipToLayer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.m_chkClipToLayer.Location = new System.Drawing.Point(16, 256);
      this.m_chkClipToLayer.Name = "m_chkClipToLayer";
      this.m_chkClipToLayer.Size = new System.Drawing.Size(128, 24);
      this.m_chkClipToLayer.TabIndex = 28;
      this.m_chkClipToLayer.Text = "Clip image to layer:";
      this.m_chkClipToLayer.CheckedChanged += new System.EventHandler(this.EhClipToLayer_CheckedChanged);
      // 
      // DensityImagePlotStyleControl
      // 
      this.Controls.Add(this.m_chkClipToLayer);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.m_cbScalingStyle);
      this.Controls.Add(this.m_cbColorInvalid);
      this.Controls.Add(this.m_cbColorAbove);
      this.Controls.Add(this.m_cbColorBelow);
      this.Controls.Add(this.label24);
      this.Controls.Add(this.m_edRangeTo);
      this.Controls.Add(this.m_edRangeFrom);
      this.Controls.Add(this.label23);
      this.Name = "DensityImagePlotStyleControl";
      this.Size = new System.Drawing.Size(272, 296);
      this.ResumeLayout(false);

    }
    #endregion

  
    private void EhRangeFrom_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
      {
        bool bCancel = e.Cancel;
        Controller.EhView_RangeFromValidating(((TextBox)sender).Text,ref bCancel);
        e.Cancel = bCancel;
      }
    }

    private void EhRangeTo_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
      {
        bool bCancel = e.Cancel;
        Controller.EhView_RangeToValidating(((TextBox)sender).Text,ref bCancel);
        e.Cancel = bCancel;
      }
    }

    private void EhScalingStyle_SelectionChangedCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_ScalingStyleChanged((string)this.m_cbScalingStyle.SelectedItem);
    }   

    private void EhColorBelow_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        string name = (string)this.m_cbColorBelow.SelectedItem;
        if(name!="Custom")
        {
          Controller.EhView_ColorBelowChanged(System.Drawing.Color.FromName(name));
        }
      }
    }

    private void EhColorAbove_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        string name = (string)this.m_cbColorAbove.SelectedItem;
        if(name!="Custom")
        {
          Controller.EhView_ColorAboveChanged(System.Drawing.Color.FromName(name));
        }
      }
    
    }

    private void EhColorInvalid_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        string name = (string)this.m_cbColorInvalid.SelectedItem;
        if(name!="Custom")
        {
          Controller.EhView_ColorInvalidChanged(System.Drawing.Color.FromName(name));
        }
      }
    
    }

    private void EhClipToLayer_CheckedChanged(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_ClipToLayerChanged(this.m_chkClipToLayer.Checked);
    }



    #region ILinkAxisView Members

    public IDensityImagePlotStyleViewEventSink Controller
    {
      get
      {
        return m_Ctrl;
      }
      set
      {
        m_Ctrl = value;
      }
    }

    public Form Form
    {
      get
      {
        return this.ParentForm;
      }
    }

    public static void InitComboBox(System.Windows.Forms.ComboBox box, string[] names, string name)
    {
      box.Items.Clear();
      box.Items.AddRange(names);
      box.SelectedItem = name;
    }


    public static void InitColorComboBox(System.Windows.Forms.ComboBox box, System.Drawing.Color color)
    {
      box.Items.Clear();
      string[] names = System.Enum.GetNames(typeof(System.Drawing.KnownColor));
      box.Items.Add("Custom");
      box.Items.AddRange(names);
      box.SelectedItem = color.IsKnownColor ? color.Name : "Custom";
    }


    public void ScalingStyle_Initialize(string[] names, string name)
    {
      InitComboBox(this.m_cbScalingStyle,names,name);
    }

    public void ColorBelow_Initialize(System.Drawing.Color colorBelow)
    {
      InitColorComboBox(this.m_cbColorBelow,colorBelow);
    }

    public void ColorAbove_Initialize(System.Drawing.Color colorAbove)
    {
      InitColorComboBox(this.m_cbColorAbove,colorAbove);
    }

    public void ColorInvalid_Initialize(System.Drawing.Color colorInvalid)
    {
      InitColorComboBox(this.m_cbColorInvalid,colorInvalid);
    }

    public void RangeFrom_Initialize(string text)
    {
      this.m_edRangeFrom.Text = text;
    }

    public void RangeTo_Initialize(string text)
    {
      this.m_edRangeTo.Text = text;
    }

    public void ClipToLayer_Initialize(bool bClip)
    {
      this.m_chkClipToLayer.Checked = bClip;
    }

    #endregion


  

    #region IMVCView Members

    public object ControllerObject
    {
      get
      {
        return Controller;
      }
      set
      {
        Controller = value as IDensityImagePlotStyleViewEventSink;
      }
    }

    #endregion
  }
}
