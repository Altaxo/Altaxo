#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using Altaxo.Main.GUI;

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Graphical interface for adjusting the style of a XYPlotLabel.
  /// </summary>
  [UserControlForController(typeof(IXYPlotLabelStyleViewEventSink))]
  public class XYPlotLabelStyleControl : System.Windows.Forms.UserControl, IXYPlotLabelStyleView
  {
    private IXYPlotLabelStyleViewEventSink _controller;
    private System.Windows.Forms.Label label24;
    private System.Windows.Forms.Label label23;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox m_cbFont;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox m_edYOffset;
    private System.Windows.Forms.TextBox m_edXOffset;
    private System.Windows.Forms.ComboBox m_cbColor;
    private System.Windows.Forms.ComboBox m_cbFontSize;
    private System.Windows.Forms.CheckBox m_chkAttachToAxis;
    private System.Windows.Forms.ComboBox m_cbAttachedAxis;
    private System.Windows.Forms.CheckBox m_chkWhiteOut;
    private System.Windows.Forms.TextBox m_edRotation;
    private System.Windows.Forms.ComboBox m_cbHorizontalAlignment;
    private System.Windows.Forms.ComboBox m_cbVerticalAlignment;
    private System.Windows.Forms.CheckBox m_chkIndependentColor;
    private System.Windows.Forms.ComboBox m_cbBackgroundColor;
    private System.Windows.Forms.TextBox _edLabelColumn;
    private System.Windows.Forms.Button _btSelectLabelColumn;
    private System.Windows.Forms.Label label2;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public XYPlotLabelStyleControl()
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
      this.label24 = new System.Windows.Forms.Label();
      this.m_edYOffset = new System.Windows.Forms.TextBox();
      this.m_edXOffset = new System.Windows.Forms.TextBox();
      this.label23 = new System.Windows.Forms.Label();
      this.m_cbColor = new System.Windows.Forms.ComboBox();
      this.m_cbFontSize = new System.Windows.Forms.ComboBox();
      this.m_cbHorizontalAlignment = new System.Windows.Forms.ComboBox();
      this.m_cbFont = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.m_chkAttachToAxis = new System.Windows.Forms.CheckBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.m_cbVerticalAlignment = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.m_cbAttachedAxis = new System.Windows.Forms.ComboBox();
      this.m_chkWhiteOut = new System.Windows.Forms.CheckBox();
      this.m_edRotation = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.m_chkIndependentColor = new System.Windows.Forms.CheckBox();
      this.m_cbBackgroundColor = new System.Windows.Forms.ComboBox();
      this._edLabelColumn = new System.Windows.Forms.TextBox();
      this._btSelectLabelColumn = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // label24
      // 
      this.label24.Location = new System.Drawing.Point(24, 64);
      this.label24.Name = "label24";
      this.label24.Size = new System.Drawing.Size(16, 16);
      this.label24.TabIndex = 19;
      this.label24.Text = "Y";
      this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_edYOffset
      // 
      this.m_edYOffset.Location = new System.Drawing.Point(72, 64);
      this.m_edYOffset.Name = "m_edYOffset";
      this.m_edYOffset.Size = new System.Drawing.Size(56, 20);
      this.m_edYOffset.TabIndex = 16;
      this.m_edYOffset.Text = "";
      this.m_edYOffset.Validating += new System.ComponentModel.CancelEventHandler(this.EhYOffset_Validating);
      // 
      // m_edXOffset
      // 
      this.m_edXOffset.Location = new System.Drawing.Point(72, 32);
      this.m_edXOffset.Name = "m_edXOffset";
      this.m_edXOffset.Size = new System.Drawing.Size(56, 20);
      this.m_edXOffset.TabIndex = 15;
      this.m_edXOffset.Text = "";
      this.m_edXOffset.Validating += new System.ComponentModel.CancelEventHandler(this.EhXOffset_Validating);
      // 
      // label23
      // 
      this.label23.Location = new System.Drawing.Point(24, 24);
      this.label23.Name = "label23";
      this.label23.Size = new System.Drawing.Size(16, 16);
      this.label23.TabIndex = 14;
      this.label23.Text = "X";
      this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_cbColor
      // 
      this.m_cbColor.Location = new System.Drawing.Point(128, 72);
      this.m_cbColor.Name = "m_cbColor";
      this.m_cbColor.Size = new System.Drawing.Size(136, 21);
      this.m_cbColor.TabIndex = 20;
      this.m_cbColor.Text = "comboBox1";
      this.m_cbColor.SelectionChangeCommitted += new System.EventHandler(this.EhColor_SelectionChangeCommitted);
      // 
      // m_cbFontSize
      // 
      this.m_cbFontSize.Location = new System.Drawing.Point(128, 104);
      this.m_cbFontSize.Name = "m_cbFontSize";
      this.m_cbFontSize.Size = new System.Drawing.Size(136, 21);
      this.m_cbFontSize.TabIndex = 21;
      this.m_cbFontSize.Text = "comboBox1";
      this.m_cbFontSize.SelectionChangeCommitted += new System.EventHandler(this.EhFontSize_SelectionChangeCommitted);
      // 
      // m_cbHorizontalAlignment
      // 
      this.m_cbHorizontalAlignment.Location = new System.Drawing.Point(128, 144);
      this.m_cbHorizontalAlignment.Name = "m_cbHorizontalAlignment";
      this.m_cbHorizontalAlignment.Size = new System.Drawing.Size(136, 21);
      this.m_cbHorizontalAlignment.TabIndex = 22;
      this.m_cbHorizontalAlignment.Text = "comboBox1";
      this.m_cbHorizontalAlignment.SelectionChangeCommitted += new System.EventHandler(this.EhHorizontalAlignment_SelectionChangeCommitted);
      // 
      // m_cbFont
      // 
      this.m_cbFont.Location = new System.Drawing.Point(128, 40);
      this.m_cbFont.Name = "m_cbFont";
      this.m_cbFont.Size = new System.Drawing.Size(136, 21);
      this.m_cbFont.TabIndex = 23;
      this.m_cbFont.Text = "comboBox1";
      this.m_cbFont.SelectionChangeCommitted += new System.EventHandler(this.EhFont_SelectionChangedCommitted);
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(64, 40);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(56, 16);
      this.label1.TabIndex = 24;
      this.label1.Text = "Font:";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(48, 104);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(72, 16);
      this.label3.TabIndex = 26;
      this.label3.Text = "Font size:";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(8, 144);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(112, 16);
      this.label4.TabIndex = 27;
      this.label4.Text = "Horz. alignment:";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_chkAttachToAxis
      // 
      this.m_chkAttachToAxis.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.m_chkAttachToAxis.Location = new System.Drawing.Point(16, 216);
      this.m_chkAttachToAxis.Name = "m_chkAttachToAxis";
      this.m_chkAttachToAxis.Size = new System.Drawing.Size(96, 24);
      this.m_chkAttachToAxis.TabIndex = 28;
      this.m_chkAttachToAxis.Text = "Attach to axis:";
      this.m_chkAttachToAxis.CheckedChanged += new System.EventHandler(this.EhAttachToAxis_CheckedChanged);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.m_edXOffset);
      this.groupBox1.Controls.Add(this.label23);
      this.groupBox1.Controls.Add(this.m_edYOffset);
      this.groupBox1.Controls.Add(this.label24);
      this.groupBox1.Location = new System.Drawing.Point(280, 256);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(136, 96);
      this.groupBox1.TabIndex = 29;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Offset (%)";
      // 
      // m_cbVerticalAlignment
      // 
      this.m_cbVerticalAlignment.Location = new System.Drawing.Point(128, 176);
      this.m_cbVerticalAlignment.Name = "m_cbVerticalAlignment";
      this.m_cbVerticalAlignment.Size = new System.Drawing.Size(136, 21);
      this.m_cbVerticalAlignment.TabIndex = 30;
      this.m_cbVerticalAlignment.Text = "comboBox1";
      this.m_cbVerticalAlignment.SelectionChangeCommitted += new System.EventHandler(this.EhVerticalAlignment_SelectionChangeCommitted);
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(16, 176);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(112, 16);
      this.label5.TabIndex = 31;
      this.label5.Text = "Vert. alignment:";
      this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_cbAttachedAxis
      // 
      this.m_cbAttachedAxis.Location = new System.Drawing.Point(128, 216);
      this.m_cbAttachedAxis.Name = "m_cbAttachedAxis";
      this.m_cbAttachedAxis.Size = new System.Drawing.Size(136, 21);
      this.m_cbAttachedAxis.TabIndex = 32;
      this.m_cbAttachedAxis.Text = "comboBox2";
      this.m_cbAttachedAxis.SelectedIndexChanged += new System.EventHandler(this.EhAttachedAxis_SelectionChangeCommitted);
      // 
      // m_chkWhiteOut
      // 
      this.m_chkWhiteOut.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.m_chkWhiteOut.Location = new System.Drawing.Point(16, 248);
      this.m_chkWhiteOut.Name = "m_chkWhiteOut";
      this.m_chkWhiteOut.Size = new System.Drawing.Size(96, 24);
      this.m_chkWhiteOut.TabIndex = 33;
      this.m_chkWhiteOut.Text = "Background:";
      this.m_chkWhiteOut.CheckedChanged += new System.EventHandler(this.EhWhiteOut_CheckedChanged);
      // 
      // m_edRotation
      // 
      this.m_edRotation.Location = new System.Drawing.Point(352, 224);
      this.m_edRotation.Name = "m_edRotation";
      this.m_edRotation.Size = new System.Drawing.Size(56, 20);
      this.m_edRotation.TabIndex = 20;
      this.m_edRotation.Text = "";
      this.m_edRotation.Validating += new System.ComponentModel.CancelEventHandler(this.EhRotation_Validating);
      // 
      // label6
      // 
      this.label6.Location = new System.Drawing.Point(320, 200);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(96, 16);
      this.label6.TabIndex = 20;
      this.label6.Text = "Rotation (deg.):";
      this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_chkIndependentColor
      // 
      this.m_chkIndependentColor.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.m_chkIndependentColor.Location = new System.Drawing.Point(8, 64);
      this.m_chkIndependentColor.Name = "m_chkIndependentColor";
      this.m_chkIndependentColor.Size = new System.Drawing.Size(96, 40);
      this.m_chkIndependentColor.TabIndex = 34;
      this.m_chkIndependentColor.Text = "Independent color";
      this.m_chkIndependentColor.CheckedChanged += new System.EventHandler(this.EhIndependentColor_CheckChanged);
      // 
      // m_cbBackgroundColor
      // 
      this.m_cbBackgroundColor.Location = new System.Drawing.Point(128, 248);
      this.m_cbBackgroundColor.Name = "m_cbBackgroundColor";
      this.m_cbBackgroundColor.Size = new System.Drawing.Size(136, 21);
      this.m_cbBackgroundColor.TabIndex = 35;
      this.m_cbBackgroundColor.Text = "comboBox1";
      this.m_cbBackgroundColor.SelectionChangeCommitted += new System.EventHandler(this.EhBackgroundColor_SelectionChangeCommitted);
      // 
      // _edLabelColumn
      // 
      this._edLabelColumn.Location = new System.Drawing.Point(128, 8);
      this._edLabelColumn.Name = "_edLabelColumn";
      this._edLabelColumn.Size = new System.Drawing.Size(240, 20);
      this._edLabelColumn.TabIndex = 36;
      this._edLabelColumn.Text = "textBox1";
      // 
      // _btSelectLabelColumn
      // 
      this._btSelectLabelColumn.Location = new System.Drawing.Point(376, 8);
      this._btSelectLabelColumn.Name = "_btSelectLabelColumn";
      this._btSelectLabelColumn.Size = new System.Drawing.Size(56, 20);
      this._btSelectLabelColumn.TabIndex = 37;
      this._btSelectLabelColumn.Text = "Select ..";
      this._btSelectLabelColumn.Click += new System.EventHandler(this.EhSelectLabelColumn_Click);
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(8, 8);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(112, 20);
      this.label2.TabIndex = 38;
      this.label2.Text = "LabelColumn:";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // XYPlotLabelStyleControl
      // 
      this.Controls.Add(this.label2);
      this.Controls.Add(this._btSelectLabelColumn);
      this.Controls.Add(this._edLabelColumn);
      this.Controls.Add(this.m_cbBackgroundColor);
      this.Controls.Add(this.m_chkIndependentColor);
      this.Controls.Add(this.m_chkWhiteOut);
      this.Controls.Add(this.m_cbAttachedAxis);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.m_cbVerticalAlignment);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.m_chkAttachToAxis);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.m_cbFont);
      this.Controls.Add(this.m_cbHorizontalAlignment);
      this.Controls.Add(this.m_cbFontSize);
      this.Controls.Add(this.m_cbColor);
      this.Controls.Add(this.m_edRotation);
      this.Controls.Add(this.label6);
      this.Name = "XYPlotLabelStyleControl";
      this.Size = new System.Drawing.Size(440, 360);
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion

  
    private void EhXOffset_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
      {
        bool bCancel = e.Cancel;
        Controller.EhView_XOffsetValidating(((TextBox)sender).Text,ref bCancel);
        e.Cancel = bCancel;
      }
    }

    private void EhYOffset_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
      {
        bool bCancel = e.Cancel;
        Controller.EhView_YOffsetValidating(((TextBox)sender).Text,ref bCancel);
        e.Cancel = bCancel;
      }
    }

    private void EhRotation_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
      {
        bool bCancel = e.Cancel;
        Controller.EhView_RotationValidating(((TextBox)sender).Text,ref bCancel);
        e.Cancel = bCancel;
      }
    }


    private void EhFont_SelectionChangedCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_FontChanged((string)this.m_cbFont.SelectedItem);
    }   

    private void EhColor_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        string name = (string)this.m_cbColor.SelectedItem;
        if(name!="Custom")
        {
          Controller.EhView_ColorChanged(System.Drawing.Color.FromName(name));
        }
      }
    }

    private void EhBackgroundColor_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        string name = (string)this.m_cbBackgroundColor.SelectedItem;
        if(name!="Custom")
        {
          Controller.EhView_BackgroundColorChanged(System.Drawing.Color.FromName(name));
        }
      }
    }

    private void EhFontSize_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        string name = (string)this.m_cbFontSize.SelectedItem;
        Controller.EhView_FontSizeChanged(name);
      }
    }

    private void EhHorizontalAlignment_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        string name = (string)this.m_cbHorizontalAlignment.SelectedItem;
        Controller.EhView_HorizontalAlignmentChanged(name);
      }
    }

    private void EhVerticalAlignment_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        string name = (string)this.m_cbVerticalAlignment.SelectedItem;
        Controller.EhView_VerticalAlignmentChanged(name);
      }
    
    }


    private void EhAttachToAxis_CheckedChanged(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        Controller.EhView_AttachToAxisChanged(this.m_chkAttachToAxis.Checked);
        this.m_cbAttachedAxis.Enabled = this.m_chkAttachToAxis.Checked;
      }
    }

    private void EhAttachedAxis_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        string name = (string)this.m_cbAttachedAxis.SelectedItem;
        Controller.EhView_AttachedAxisChanged(name);
      }
    }

    private void EhWhiteOut_CheckedChanged(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        Controller.EhView_WhiteOutChanged(this.m_chkWhiteOut.Checked);
      }
    }

    private void EhIndependentColor_CheckChanged(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        Controller.EhView_IndependentColorChanged(this.m_chkIndependentColor.Checked);
        this.m_cbColor.Enabled = this.m_chkIndependentColor.Checked;
      }
    
    }

    private void EhSelectLabelColumn_Click(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_SelectLabelColumn();
    }


   
    #region IXYPlotLabelStyleView Members

    public IXYPlotLabelStyleViewEventSink Controller
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

    public static void InitFontComboBox(ComboBox box, string choosenfontfamily)
    {
      box.Items.Clear();
      foreach(System.Drawing.FontFamily fontfamily in System.Drawing.FontFamily.Families)
        box.Items.Add(fontfamily.Name);
      box.SelectedItem = choosenfontfamily;
    }


    public void LabelColumn_Initialize(string labelColumnAsText)
    {
      this._edLabelColumn.Text = labelColumnAsText;
    }

    public void Font_Initialize(string fontfamily)
    {
      InitFontComboBox(this.m_cbFont,fontfamily);
    }


    public void IndependentColor_Initialize(bool bIndependent)
    {
      this.m_chkIndependentColor.Checked = bIndependent;
      this.m_cbColor.Enabled = bIndependent;      
    }

    public void Color_Initialize(System.Drawing.Color color)
    {
      InitColorComboBox(this.m_cbColor,color);
    }

    public void BackgroundColor_Initialize(System.Drawing.Color color)
    {
      InitColorComboBox(this.m_cbBackgroundColor,color);
    }


    public void FontSize_Initialize(string[] names, string name)
    {
      InitComboBox(this.m_cbFontSize,names,name);
    }
    

    public void HorizontalAlignment_Initialize(string[] names, string name)
    {
      InitComboBox(this.m_cbHorizontalAlignment,names,name);
    }

    public void VerticalAlignment_Initialize(string[] names, string name)
    {
      InitComboBox(this.m_cbVerticalAlignment,names,name);
    }


    public void AttachToAxis_Initialize(bool bAttach)
    {
      this.m_chkAttachToAxis.Checked = bAttach;
      this.m_cbAttachedAxis.Enabled = !bAttach;     
    }

    public void AttachedAxis_Initialize(string[] names, string name)
    {
      InitComboBox(this.m_cbAttachedAxis,names,name);
    }

    public void Rotation_Initialize(string text)
    {
      this.m_edRotation.Text = text;
    }

    public void XOffset_Initialize(string text)
    {
      this.m_edXOffset.Text = text;
    }

    public void YOffset_Initialize(string text)
    {
      this.m_edYOffset.Text = text;
    }


    public void WhiteOut_Initialize(bool bWhiteOut)
    {
      this.m_chkWhiteOut.Checked = bWhiteOut;
    }

    #endregion


   
  }
}
