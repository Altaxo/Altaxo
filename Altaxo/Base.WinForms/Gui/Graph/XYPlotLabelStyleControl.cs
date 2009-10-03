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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Altaxo.Collections;

using Altaxo.Gui.Common.Drawing;

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
    private FontComboBox m_cbFont;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox m_edYOffset;
    private System.Windows.Forms.TextBox m_edXOffset;
    private FontSizeComboBox m_cbFontSize;
    private System.Windows.Forms.CheckBox m_chkAttachToAxis;
    private System.Windows.Forms.ComboBox m_cbAttachedAxis;
    private System.Windows.Forms.ComboBox m_cbHorizontalAlignment;
    private System.Windows.Forms.ComboBox m_cbVerticalAlignment;
    private System.Windows.Forms.CheckBox m_chkIndependentColor;
    private System.Windows.Forms.TextBox _edLabelColumn;
    private System.Windows.Forms.Button _btSelectLabelColumn;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label7;
    private Altaxo.Gui.Common.Drawing.ColorComboBox m_cbColor;
    private Altaxo.Gui.Common.Drawing.FontControlsGlue _fontControlsGlue;
    private BackgroundControlsGlue _backgroundGlue;
    private BrushColorComboBox _backgroundBrush;
    private ComboBox _cbBackgroundStyle;
    private RotationComboBox m_edRotation;
    private IContainer components;

    public XYPlotLabelStyleControl()
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
      this.label24 = new System.Windows.Forms.Label();
      this.m_edYOffset = new System.Windows.Forms.TextBox();
      this.m_edXOffset = new System.Windows.Forms.TextBox();
      this.label23 = new System.Windows.Forms.Label();
      this.m_cbHorizontalAlignment = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.m_chkAttachToAxis = new System.Windows.Forms.CheckBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.m_cbVerticalAlignment = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.m_cbAttachedAxis = new System.Windows.Forms.ComboBox();
      this.label6 = new System.Windows.Forms.Label();
      this.m_chkIndependentColor = new System.Windows.Forms.CheckBox();
      this._edLabelColumn = new System.Windows.Forms.TextBox();
      this._btSelectLabelColumn = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this._cbBackgroundStyle = new System.Windows.Forms.ComboBox();
      this._backgroundBrush = new Altaxo.Gui.Common.Drawing.BrushColorComboBox();
      this.m_cbColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this.m_cbFont = new Altaxo.Gui.Common.Drawing.FontComboBox();
      this.m_cbFontSize = new Altaxo.Gui.Common.Drawing.FontSizeComboBox();
      this._fontControlsGlue = new Altaxo.Gui.Common.Drawing.FontControlsGlue();
      this._backgroundGlue = new Altaxo.Gui.Graph.BackgroundControlsGlue();
      this.m_edRotation = new Altaxo.Gui.Common.Drawing.RotationComboBox();
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
      this.m_edYOffset.Validating += new System.ComponentModel.CancelEventHandler(this.EhYOffset_Validating);
      // 
      // m_edXOffset
      // 
      this.m_edXOffset.Location = new System.Drawing.Point(72, 32);
      this.m_edXOffset.Name = "m_edXOffset";
      this.m_edXOffset.Size = new System.Drawing.Size(56, 20);
      this.m_edXOffset.TabIndex = 15;
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
      // m_cbHorizontalAlignment
      // 
      this.m_cbHorizontalAlignment.Location = new System.Drawing.Point(126, 125);
      this.m_cbHorizontalAlignment.Name = "m_cbHorizontalAlignment";
      this.m_cbHorizontalAlignment.Size = new System.Drawing.Size(136, 21);
      this.m_cbHorizontalAlignment.TabIndex = 22;
      this.m_cbHorizontalAlignment.Text = "comboBox1";
      this.m_cbHorizontalAlignment.SelectionChangeCommitted += new System.EventHandler(this.EhHorizontalAlignment_SelectionChangeCommitted);
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(64, 71);
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
      this.label4.Location = new System.Drawing.Point(13, 130);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(112, 16);
      this.label4.TabIndex = 27;
      this.label4.Text = "Horz. alignment:";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_chkAttachToAxis
      // 
      this.m_chkAttachToAxis.Location = new System.Drawing.Point(24, 179);
      this.m_chkAttachToAxis.Name = "m_chkAttachToAxis";
      this.m_chkAttachToAxis.Size = new System.Drawing.Size(96, 21);
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
      this.groupBox1.Location = new System.Drawing.Point(296, 104);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(136, 96);
      this.groupBox1.TabIndex = 29;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Offset (%)";
      // 
      // m_cbVerticalAlignment
      // 
      this.m_cbVerticalAlignment.Location = new System.Drawing.Point(126, 152);
      this.m_cbVerticalAlignment.Name = "m_cbVerticalAlignment";
      this.m_cbVerticalAlignment.Size = new System.Drawing.Size(136, 21);
      this.m_cbVerticalAlignment.TabIndex = 30;
      this.m_cbVerticalAlignment.Text = "comboBox1";
      this.m_cbVerticalAlignment.SelectionChangeCommitted += new System.EventHandler(this.EhVerticalAlignment_SelectionChangeCommitted);
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(8, 157);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(112, 16);
      this.label5.TabIndex = 31;
      this.label5.Text = "Vert. alignment:";
      this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_cbAttachedAxis
      // 
      this.m_cbAttachedAxis.Location = new System.Drawing.Point(126, 179);
      this.m_cbAttachedAxis.Name = "m_cbAttachedAxis";
      this.m_cbAttachedAxis.Size = new System.Drawing.Size(136, 21);
      this.m_cbAttachedAxis.TabIndex = 32;
      this.m_cbAttachedAxis.Text = "comboBox2";
      this.m_cbAttachedAxis.SelectedIndexChanged += new System.EventHandler(this.EhAttachedAxis_SelectionChangeCommitted);
      // 
      // label6
      // 
      this.label6.Location = new System.Drawing.Point(293, 52);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(96, 16);
      this.label6.TabIndex = 20;
      this.label6.Text = "Rotation (deg.):";
      this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // m_chkIndependentColor
      // 
      this.m_chkIndependentColor.Location = new System.Drawing.Point(47, 44);
      this.m_chkIndependentColor.Name = "m_chkIndependentColor";
      this.m_chkIndependentColor.Size = new System.Drawing.Size(73, 21);
      this.m_chkIndependentColor.TabIndex = 34;
      this.m_chkIndependentColor.Text = "Ind. color:";
      this.m_chkIndependentColor.CheckedChanged += new System.EventHandler(this.EhIndependentColor_CheckChanged);
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
      // label7
      // 
      this.label7.Location = new System.Drawing.Point(48, 211);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(72, 16);
      this.label7.TabIndex = 42;
      this.label7.Text = "Background:";
      this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // _cbBackgroundStyle
      // 
      this._cbBackgroundStyle.FormattingEnabled = true;
      this._cbBackgroundStyle.Location = new System.Drawing.Point(126, 206);
      this._cbBackgroundStyle.Name = "_cbBackgroundStyle";
      this._cbBackgroundStyle.Size = new System.Drawing.Size(136, 21);
      this._cbBackgroundStyle.TabIndex = 46;
      // 
      // _backgroundBrush
      // 
      this._backgroundBrush.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._backgroundBrush.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._backgroundBrush.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._backgroundBrush.FormattingEnabled = true;
      this._backgroundBrush.ItemHeight = 15;
      this._backgroundBrush.Location = new System.Drawing.Point(296, 206);
      this._backgroundBrush.Name = "_backgroundBrush";
      this._backgroundBrush.Size = new System.Drawing.Size(136, 21);
      this._backgroundBrush.TabIndex = 45;
      // 
      // m_cbColor
      // 
      this.m_cbColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this.m_cbColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.m_cbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_cbColor.FormattingEnabled = true;
      this.m_cbColor.ItemHeight = 15;
      this.m_cbColor.Items.AddRange(new object[] {
            System.Drawing.SystemColors.ActiveBorder,
            System.Drawing.SystemColors.ActiveCaption,
            System.Drawing.SystemColors.ActiveCaptionText,
            System.Drawing.SystemColors.AppWorkspace,
            System.Drawing.SystemColors.Control,
            System.Drawing.SystemColors.ControlDark,
            System.Drawing.SystemColors.ControlDarkDark,
            System.Drawing.SystemColors.ControlLight,
            System.Drawing.SystemColors.ControlLightLight,
            System.Drawing.SystemColors.ControlText,
            System.Drawing.SystemColors.Desktop,
            System.Drawing.SystemColors.GrayText,
            System.Drawing.SystemColors.Highlight,
            System.Drawing.SystemColors.HighlightText,
            System.Drawing.SystemColors.HotTrack,
            System.Drawing.SystemColors.InactiveBorder,
            System.Drawing.SystemColors.InactiveCaption,
            System.Drawing.SystemColors.InactiveCaptionText,
            System.Drawing.SystemColors.Info,
            System.Drawing.SystemColors.InfoText,
            System.Drawing.SystemColors.Menu,
            System.Drawing.SystemColors.MenuText,
            System.Drawing.SystemColors.ScrollBar,
            System.Drawing.SystemColors.Window,
            System.Drawing.SystemColors.WindowFrame,
            System.Drawing.SystemColors.WindowText,
            System.Drawing.Color.Transparent,
            System.Drawing.Color.AliceBlue,
            System.Drawing.Color.AntiqueWhite,
            System.Drawing.Color.Aqua,
            System.Drawing.Color.Aquamarine,
            System.Drawing.Color.Azure,
            System.Drawing.Color.Beige,
            System.Drawing.Color.Bisque,
            System.Drawing.Color.Black,
            System.Drawing.Color.BlanchedAlmond,
            System.Drawing.Color.Blue,
            System.Drawing.Color.BlueViolet,
            System.Drawing.Color.Brown,
            System.Drawing.Color.BurlyWood,
            System.Drawing.Color.CadetBlue,
            System.Drawing.Color.Chartreuse,
            System.Drawing.Color.Chocolate,
            System.Drawing.Color.Coral,
            System.Drawing.Color.CornflowerBlue,
            System.Drawing.Color.Cornsilk,
            System.Drawing.Color.Crimson,
            System.Drawing.Color.Cyan,
            System.Drawing.Color.DarkBlue,
            System.Drawing.Color.DarkCyan,
            System.Drawing.Color.DarkGoldenrod,
            System.Drawing.Color.DarkGray,
            System.Drawing.Color.DarkGreen,
            System.Drawing.Color.DarkKhaki,
            System.Drawing.Color.DarkMagenta,
            System.Drawing.Color.DarkOliveGreen,
            System.Drawing.Color.DarkOrange,
            System.Drawing.Color.DarkOrchid,
            System.Drawing.Color.DarkRed,
            System.Drawing.Color.DarkSalmon,
            System.Drawing.Color.DarkSeaGreen,
            System.Drawing.Color.DarkSlateBlue,
            System.Drawing.Color.DarkSlateGray,
            System.Drawing.Color.DarkTurquoise,
            System.Drawing.Color.DarkViolet,
            System.Drawing.Color.DeepPink,
            System.Drawing.Color.DeepSkyBlue,
            System.Drawing.Color.DimGray,
            System.Drawing.Color.DodgerBlue,
            System.Drawing.Color.Firebrick,
            System.Drawing.Color.FloralWhite,
            System.Drawing.Color.ForestGreen,
            System.Drawing.Color.Fuchsia,
            System.Drawing.Color.Gainsboro,
            System.Drawing.Color.GhostWhite,
            System.Drawing.Color.Gold,
            System.Drawing.Color.Goldenrod,
            System.Drawing.Color.Gray,
            System.Drawing.Color.Green,
            System.Drawing.Color.GreenYellow,
            System.Drawing.Color.Honeydew,
            System.Drawing.Color.HotPink,
            System.Drawing.Color.IndianRed,
            System.Drawing.Color.Indigo,
            System.Drawing.Color.Ivory,
            System.Drawing.Color.Khaki,
            System.Drawing.Color.Lavender,
            System.Drawing.Color.LavenderBlush,
            System.Drawing.Color.LawnGreen,
            System.Drawing.Color.LemonChiffon,
            System.Drawing.Color.LightBlue,
            System.Drawing.Color.LightCoral,
            System.Drawing.Color.LightCyan,
            System.Drawing.Color.LightGoldenrodYellow,
            System.Drawing.Color.LightGray,
            System.Drawing.Color.LightGreen,
            System.Drawing.Color.LightPink,
            System.Drawing.Color.LightSalmon,
            System.Drawing.Color.LightSeaGreen,
            System.Drawing.Color.LightSkyBlue,
            System.Drawing.Color.LightSlateGray,
            System.Drawing.Color.LightSteelBlue,
            System.Drawing.Color.LightYellow,
            System.Drawing.Color.Lime,
            System.Drawing.Color.LimeGreen,
            System.Drawing.Color.Linen,
            System.Drawing.Color.Magenta,
            System.Drawing.Color.Maroon,
            System.Drawing.Color.MediumAquamarine,
            System.Drawing.Color.MediumBlue,
            System.Drawing.Color.MediumOrchid,
            System.Drawing.Color.MediumPurple,
            System.Drawing.Color.MediumSeaGreen,
            System.Drawing.Color.MediumSlateBlue,
            System.Drawing.Color.MediumSpringGreen,
            System.Drawing.Color.MediumTurquoise,
            System.Drawing.Color.MediumVioletRed,
            System.Drawing.Color.MidnightBlue,
            System.Drawing.Color.MintCream,
            System.Drawing.Color.MistyRose,
            System.Drawing.Color.Moccasin,
            System.Drawing.Color.NavajoWhite,
            System.Drawing.Color.Navy,
            System.Drawing.Color.OldLace,
            System.Drawing.Color.Olive,
            System.Drawing.Color.OliveDrab,
            System.Drawing.Color.Orange,
            System.Drawing.Color.OrangeRed,
            System.Drawing.Color.Orchid,
            System.Drawing.Color.PaleGoldenrod,
            System.Drawing.Color.PaleGreen,
            System.Drawing.Color.PaleTurquoise,
            System.Drawing.Color.PaleVioletRed,
            System.Drawing.Color.PapayaWhip,
            System.Drawing.Color.PeachPuff,
            System.Drawing.Color.Peru,
            System.Drawing.Color.Pink,
            System.Drawing.Color.Plum,
            System.Drawing.Color.PowderBlue,
            System.Drawing.Color.Purple,
            System.Drawing.Color.Red,
            System.Drawing.Color.RosyBrown,
            System.Drawing.Color.RoyalBlue,
            System.Drawing.Color.SaddleBrown,
            System.Drawing.Color.Salmon,
            System.Drawing.Color.SandyBrown,
            System.Drawing.Color.SeaGreen,
            System.Drawing.Color.SeaShell,
            System.Drawing.Color.Sienna,
            System.Drawing.Color.Silver,
            System.Drawing.Color.SkyBlue,
            System.Drawing.Color.SlateBlue,
            System.Drawing.Color.SlateGray,
            System.Drawing.Color.Snow,
            System.Drawing.Color.SpringGreen,
            System.Drawing.Color.SteelBlue,
            System.Drawing.Color.Tan,
            System.Drawing.Color.Teal,
            System.Drawing.Color.Thistle,
            System.Drawing.Color.Tomato,
            System.Drawing.Color.Turquoise,
            System.Drawing.Color.Violet,
            System.Drawing.Color.Wheat,
            System.Drawing.Color.White,
            System.Drawing.Color.WhiteSmoke,
            System.Drawing.Color.Yellow,
            System.Drawing.Color.YellowGreen,
            System.Drawing.SystemColors.ButtonFace,
            System.Drawing.SystemColors.ButtonHighlight,
            System.Drawing.SystemColors.ButtonShadow,
            System.Drawing.SystemColors.GradientActiveCaption,
            System.Drawing.SystemColors.GradientInactiveCaption,
            System.Drawing.SystemColors.MenuBar,
            System.Drawing.SystemColors.MenuHighlight,
            System.Drawing.SystemColors.ActiveBorder,
            System.Drawing.SystemColors.ActiveCaption,
            System.Drawing.SystemColors.ActiveCaptionText,
            System.Drawing.SystemColors.AppWorkspace,
            System.Drawing.SystemColors.Control,
            System.Drawing.SystemColors.ControlDark,
            System.Drawing.SystemColors.ControlDarkDark,
            System.Drawing.SystemColors.ControlLight,
            System.Drawing.SystemColors.ControlLightLight,
            System.Drawing.SystemColors.ControlText,
            System.Drawing.SystemColors.Desktop,
            System.Drawing.SystemColors.GrayText,
            System.Drawing.SystemColors.Highlight,
            System.Drawing.SystemColors.HighlightText,
            System.Drawing.SystemColors.HotTrack,
            System.Drawing.SystemColors.InactiveBorder,
            System.Drawing.SystemColors.InactiveCaption,
            System.Drawing.SystemColors.InactiveCaptionText,
            System.Drawing.SystemColors.Info,
            System.Drawing.SystemColors.InfoText,
            System.Drawing.SystemColors.Menu,
            System.Drawing.SystemColors.MenuText,
            System.Drawing.SystemColors.ScrollBar,
            System.Drawing.SystemColors.Window,
            System.Drawing.SystemColors.WindowFrame,
            System.Drawing.SystemColors.WindowText,
            System.Drawing.Color.Transparent,
            System.Drawing.Color.AliceBlue,
            System.Drawing.Color.AntiqueWhite,
            System.Drawing.Color.Aqua,
            System.Drawing.Color.Aquamarine,
            System.Drawing.Color.Azure,
            System.Drawing.Color.Beige,
            System.Drawing.Color.Bisque,
            System.Drawing.Color.Black,
            System.Drawing.Color.BlanchedAlmond,
            System.Drawing.Color.Blue,
            System.Drawing.Color.BlueViolet,
            System.Drawing.Color.Brown,
            System.Drawing.Color.BurlyWood,
            System.Drawing.Color.CadetBlue,
            System.Drawing.Color.Chartreuse,
            System.Drawing.Color.Chocolate,
            System.Drawing.Color.Coral,
            System.Drawing.Color.CornflowerBlue,
            System.Drawing.Color.Cornsilk,
            System.Drawing.Color.Crimson,
            System.Drawing.Color.Cyan,
            System.Drawing.Color.DarkBlue,
            System.Drawing.Color.DarkCyan,
            System.Drawing.Color.DarkGoldenrod,
            System.Drawing.Color.DarkGray,
            System.Drawing.Color.DarkGreen,
            System.Drawing.Color.DarkKhaki,
            System.Drawing.Color.DarkMagenta,
            System.Drawing.Color.DarkOliveGreen,
            System.Drawing.Color.DarkOrange,
            System.Drawing.Color.DarkOrchid,
            System.Drawing.Color.DarkRed,
            System.Drawing.Color.DarkSalmon,
            System.Drawing.Color.DarkSeaGreen,
            System.Drawing.Color.DarkSlateBlue,
            System.Drawing.Color.DarkSlateGray,
            System.Drawing.Color.DarkTurquoise,
            System.Drawing.Color.DarkViolet,
            System.Drawing.Color.DeepPink,
            System.Drawing.Color.DeepSkyBlue,
            System.Drawing.Color.DimGray,
            System.Drawing.Color.DodgerBlue,
            System.Drawing.Color.Firebrick,
            System.Drawing.Color.FloralWhite,
            System.Drawing.Color.ForestGreen,
            System.Drawing.Color.Fuchsia,
            System.Drawing.Color.Gainsboro,
            System.Drawing.Color.GhostWhite,
            System.Drawing.Color.Gold,
            System.Drawing.Color.Goldenrod,
            System.Drawing.Color.Gray,
            System.Drawing.Color.Green,
            System.Drawing.Color.GreenYellow,
            System.Drawing.Color.Honeydew,
            System.Drawing.Color.HotPink,
            System.Drawing.Color.IndianRed,
            System.Drawing.Color.Indigo,
            System.Drawing.Color.Ivory,
            System.Drawing.Color.Khaki,
            System.Drawing.Color.Lavender,
            System.Drawing.Color.LavenderBlush,
            System.Drawing.Color.LawnGreen,
            System.Drawing.Color.LemonChiffon,
            System.Drawing.Color.LightBlue,
            System.Drawing.Color.LightCoral,
            System.Drawing.Color.LightCyan,
            System.Drawing.Color.LightGoldenrodYellow,
            System.Drawing.Color.LightGray,
            System.Drawing.Color.LightGreen,
            System.Drawing.Color.LightPink,
            System.Drawing.Color.LightSalmon,
            System.Drawing.Color.LightSeaGreen,
            System.Drawing.Color.LightSkyBlue,
            System.Drawing.Color.LightSlateGray,
            System.Drawing.Color.LightSteelBlue,
            System.Drawing.Color.LightYellow,
            System.Drawing.Color.Lime,
            System.Drawing.Color.LimeGreen,
            System.Drawing.Color.Linen,
            System.Drawing.Color.Magenta,
            System.Drawing.Color.Maroon,
            System.Drawing.Color.MediumAquamarine,
            System.Drawing.Color.MediumBlue,
            System.Drawing.Color.MediumOrchid,
            System.Drawing.Color.MediumPurple,
            System.Drawing.Color.MediumSeaGreen,
            System.Drawing.Color.MediumSlateBlue,
            System.Drawing.Color.MediumSpringGreen,
            System.Drawing.Color.MediumTurquoise,
            System.Drawing.Color.MediumVioletRed,
            System.Drawing.Color.MidnightBlue,
            System.Drawing.Color.MintCream,
            System.Drawing.Color.MistyRose,
            System.Drawing.Color.Moccasin,
            System.Drawing.Color.NavajoWhite,
            System.Drawing.Color.Navy,
            System.Drawing.Color.OldLace,
            System.Drawing.Color.Olive,
            System.Drawing.Color.OliveDrab,
            System.Drawing.Color.Orange,
            System.Drawing.Color.OrangeRed,
            System.Drawing.Color.Orchid,
            System.Drawing.Color.PaleGoldenrod,
            System.Drawing.Color.PaleGreen,
            System.Drawing.Color.PaleTurquoise,
            System.Drawing.Color.PaleVioletRed,
            System.Drawing.Color.PapayaWhip,
            System.Drawing.Color.PeachPuff,
            System.Drawing.Color.Peru,
            System.Drawing.Color.Pink,
            System.Drawing.Color.Plum,
            System.Drawing.Color.PowderBlue,
            System.Drawing.Color.Purple,
            System.Drawing.Color.Red,
            System.Drawing.Color.RosyBrown,
            System.Drawing.Color.RoyalBlue,
            System.Drawing.Color.SaddleBrown,
            System.Drawing.Color.Salmon,
            System.Drawing.Color.SandyBrown,
            System.Drawing.Color.SeaGreen,
            System.Drawing.Color.SeaShell,
            System.Drawing.Color.Sienna,
            System.Drawing.Color.Silver,
            System.Drawing.Color.SkyBlue,
            System.Drawing.Color.SlateBlue,
            System.Drawing.Color.SlateGray,
            System.Drawing.Color.Snow,
            System.Drawing.Color.SpringGreen,
            System.Drawing.Color.SteelBlue,
            System.Drawing.Color.Tan,
            System.Drawing.Color.Teal,
            System.Drawing.Color.Thistle,
            System.Drawing.Color.Tomato,
            System.Drawing.Color.Turquoise,
            System.Drawing.Color.Violet,
            System.Drawing.Color.Wheat,
            System.Drawing.Color.White,
            System.Drawing.Color.WhiteSmoke,
            System.Drawing.Color.Yellow,
            System.Drawing.Color.YellowGreen,
            System.Drawing.SystemColors.ButtonFace,
            System.Drawing.SystemColors.ButtonHighlight,
            System.Drawing.SystemColors.ButtonShadow,
            System.Drawing.SystemColors.GradientActiveCaption,
            System.Drawing.SystemColors.GradientInactiveCaption,
            System.Drawing.SystemColors.MenuBar,
            System.Drawing.SystemColors.MenuHighlight,
            System.Drawing.SystemColors.ActiveBorder,
            System.Drawing.SystemColors.ActiveCaption,
            System.Drawing.SystemColors.ActiveCaptionText,
            System.Drawing.SystemColors.AppWorkspace,
            System.Drawing.SystemColors.Control,
            System.Drawing.SystemColors.ControlDark,
            System.Drawing.SystemColors.ControlDarkDark,
            System.Drawing.SystemColors.ControlLight,
            System.Drawing.SystemColors.ControlLightLight,
            System.Drawing.SystemColors.ControlText,
            System.Drawing.SystemColors.Desktop,
            System.Drawing.SystemColors.GrayText,
            System.Drawing.SystemColors.Highlight,
            System.Drawing.SystemColors.HighlightText,
            System.Drawing.SystemColors.HotTrack,
            System.Drawing.SystemColors.InactiveBorder,
            System.Drawing.SystemColors.InactiveCaption,
            System.Drawing.SystemColors.InactiveCaptionText,
            System.Drawing.SystemColors.Info,
            System.Drawing.SystemColors.InfoText,
            System.Drawing.SystemColors.Menu,
            System.Drawing.SystemColors.MenuText,
            System.Drawing.SystemColors.ScrollBar,
            System.Drawing.SystemColors.Window,
            System.Drawing.SystemColors.WindowFrame,
            System.Drawing.SystemColors.WindowText,
            System.Drawing.Color.Transparent,
            System.Drawing.Color.AliceBlue,
            System.Drawing.Color.AntiqueWhite,
            System.Drawing.Color.Aqua,
            System.Drawing.Color.Aquamarine,
            System.Drawing.Color.Azure,
            System.Drawing.Color.Beige,
            System.Drawing.Color.Bisque,
            System.Drawing.Color.Black,
            System.Drawing.Color.BlanchedAlmond,
            System.Drawing.Color.Blue,
            System.Drawing.Color.BlueViolet,
            System.Drawing.Color.Brown,
            System.Drawing.Color.BurlyWood,
            System.Drawing.Color.CadetBlue,
            System.Drawing.Color.Chartreuse,
            System.Drawing.Color.Chocolate,
            System.Drawing.Color.Coral,
            System.Drawing.Color.CornflowerBlue,
            System.Drawing.Color.Cornsilk,
            System.Drawing.Color.Crimson,
            System.Drawing.Color.Cyan,
            System.Drawing.Color.DarkBlue,
            System.Drawing.Color.DarkCyan,
            System.Drawing.Color.DarkGoldenrod,
            System.Drawing.Color.DarkGray,
            System.Drawing.Color.DarkGreen,
            System.Drawing.Color.DarkKhaki,
            System.Drawing.Color.DarkMagenta,
            System.Drawing.Color.DarkOliveGreen,
            System.Drawing.Color.DarkOrange,
            System.Drawing.Color.DarkOrchid,
            System.Drawing.Color.DarkRed,
            System.Drawing.Color.DarkSalmon,
            System.Drawing.Color.DarkSeaGreen,
            System.Drawing.Color.DarkSlateBlue,
            System.Drawing.Color.DarkSlateGray,
            System.Drawing.Color.DarkTurquoise,
            System.Drawing.Color.DarkViolet,
            System.Drawing.Color.DeepPink,
            System.Drawing.Color.DeepSkyBlue,
            System.Drawing.Color.DimGray,
            System.Drawing.Color.DodgerBlue,
            System.Drawing.Color.Firebrick,
            System.Drawing.Color.FloralWhite,
            System.Drawing.Color.ForestGreen,
            System.Drawing.Color.Fuchsia,
            System.Drawing.Color.Gainsboro,
            System.Drawing.Color.GhostWhite,
            System.Drawing.Color.Gold,
            System.Drawing.Color.Goldenrod,
            System.Drawing.Color.Gray,
            System.Drawing.Color.Green,
            System.Drawing.Color.GreenYellow,
            System.Drawing.Color.Honeydew,
            System.Drawing.Color.HotPink,
            System.Drawing.Color.IndianRed,
            System.Drawing.Color.Indigo,
            System.Drawing.Color.Ivory,
            System.Drawing.Color.Khaki,
            System.Drawing.Color.Lavender,
            System.Drawing.Color.LavenderBlush,
            System.Drawing.Color.LawnGreen,
            System.Drawing.Color.LemonChiffon,
            System.Drawing.Color.LightBlue,
            System.Drawing.Color.LightCoral,
            System.Drawing.Color.LightCyan,
            System.Drawing.Color.LightGoldenrodYellow,
            System.Drawing.Color.LightGray,
            System.Drawing.Color.LightGreen,
            System.Drawing.Color.LightPink,
            System.Drawing.Color.LightSalmon,
            System.Drawing.Color.LightSeaGreen,
            System.Drawing.Color.LightSkyBlue,
            System.Drawing.Color.LightSlateGray,
            System.Drawing.Color.LightSteelBlue,
            System.Drawing.Color.LightYellow,
            System.Drawing.Color.Lime,
            System.Drawing.Color.LimeGreen,
            System.Drawing.Color.Linen,
            System.Drawing.Color.Magenta,
            System.Drawing.Color.Maroon,
            System.Drawing.Color.MediumAquamarine,
            System.Drawing.Color.MediumBlue,
            System.Drawing.Color.MediumOrchid,
            System.Drawing.Color.MediumPurple,
            System.Drawing.Color.MediumSeaGreen,
            System.Drawing.Color.MediumSlateBlue,
            System.Drawing.Color.MediumSpringGreen,
            System.Drawing.Color.MediumTurquoise,
            System.Drawing.Color.MediumVioletRed,
            System.Drawing.Color.MidnightBlue,
            System.Drawing.Color.MintCream,
            System.Drawing.Color.MistyRose,
            System.Drawing.Color.Moccasin,
            System.Drawing.Color.NavajoWhite,
            System.Drawing.Color.Navy,
            System.Drawing.Color.OldLace,
            System.Drawing.Color.Olive,
            System.Drawing.Color.OliveDrab,
            System.Drawing.Color.Orange,
            System.Drawing.Color.OrangeRed,
            System.Drawing.Color.Orchid,
            System.Drawing.Color.PaleGoldenrod,
            System.Drawing.Color.PaleGreen,
            System.Drawing.Color.PaleTurquoise,
            System.Drawing.Color.PaleVioletRed,
            System.Drawing.Color.PapayaWhip,
            System.Drawing.Color.PeachPuff,
            System.Drawing.Color.Peru,
            System.Drawing.Color.Pink,
            System.Drawing.Color.Plum,
            System.Drawing.Color.PowderBlue,
            System.Drawing.Color.Purple,
            System.Drawing.Color.Red,
            System.Drawing.Color.RosyBrown,
            System.Drawing.Color.RoyalBlue,
            System.Drawing.Color.SaddleBrown,
            System.Drawing.Color.Salmon,
            System.Drawing.Color.SandyBrown,
            System.Drawing.Color.SeaGreen,
            System.Drawing.Color.SeaShell,
            System.Drawing.Color.Sienna,
            System.Drawing.Color.Silver,
            System.Drawing.Color.SkyBlue,
            System.Drawing.Color.SlateBlue,
            System.Drawing.Color.SlateGray,
            System.Drawing.Color.Snow,
            System.Drawing.Color.SpringGreen,
            System.Drawing.Color.SteelBlue,
            System.Drawing.Color.Tan,
            System.Drawing.Color.Teal,
            System.Drawing.Color.Thistle,
            System.Drawing.Color.Tomato,
            System.Drawing.Color.Turquoise,
            System.Drawing.Color.Violet,
            System.Drawing.Color.Wheat,
            System.Drawing.Color.White,
            System.Drawing.Color.WhiteSmoke,
            System.Drawing.Color.Yellow,
            System.Drawing.Color.YellowGreen,
            System.Drawing.SystemColors.ButtonFace,
            System.Drawing.SystemColors.ButtonHighlight,
            System.Drawing.SystemColors.ButtonShadow,
            System.Drawing.SystemColors.GradientActiveCaption,
            System.Drawing.SystemColors.GradientInactiveCaption,
            System.Drawing.SystemColors.MenuBar,
            System.Drawing.SystemColors.MenuHighlight,
            System.Drawing.SystemColors.ActiveBorder,
            System.Drawing.SystemColors.ActiveCaption,
            System.Drawing.SystemColors.ActiveCaptionText,
            System.Drawing.SystemColors.AppWorkspace,
            System.Drawing.SystemColors.Control,
            System.Drawing.SystemColors.ControlDark,
            System.Drawing.SystemColors.ControlDarkDark,
            System.Drawing.SystemColors.ControlLight,
            System.Drawing.SystemColors.ControlLightLight,
            System.Drawing.SystemColors.ControlText,
            System.Drawing.SystemColors.Desktop,
            System.Drawing.SystemColors.GrayText,
            System.Drawing.SystemColors.Highlight,
            System.Drawing.SystemColors.HighlightText,
            System.Drawing.SystemColors.HotTrack,
            System.Drawing.SystemColors.InactiveBorder,
            System.Drawing.SystemColors.InactiveCaption,
            System.Drawing.SystemColors.InactiveCaptionText,
            System.Drawing.SystemColors.Info,
            System.Drawing.SystemColors.InfoText,
            System.Drawing.SystemColors.Menu,
            System.Drawing.SystemColors.MenuText,
            System.Drawing.SystemColors.ScrollBar,
            System.Drawing.SystemColors.Window,
            System.Drawing.SystemColors.WindowFrame,
            System.Drawing.SystemColors.WindowText,
            System.Drawing.Color.Transparent,
            System.Drawing.Color.AliceBlue,
            System.Drawing.Color.AntiqueWhite,
            System.Drawing.Color.Aqua,
            System.Drawing.Color.Aquamarine,
            System.Drawing.Color.Azure,
            System.Drawing.Color.Beige,
            System.Drawing.Color.Bisque,
            System.Drawing.Color.Black,
            System.Drawing.Color.BlanchedAlmond,
            System.Drawing.Color.Blue,
            System.Drawing.Color.BlueViolet,
            System.Drawing.Color.Brown,
            System.Drawing.Color.BurlyWood,
            System.Drawing.Color.CadetBlue,
            System.Drawing.Color.Chartreuse,
            System.Drawing.Color.Chocolate,
            System.Drawing.Color.Coral,
            System.Drawing.Color.CornflowerBlue,
            System.Drawing.Color.Cornsilk,
            System.Drawing.Color.Crimson,
            System.Drawing.Color.Cyan,
            System.Drawing.Color.DarkBlue,
            System.Drawing.Color.DarkCyan,
            System.Drawing.Color.DarkGoldenrod,
            System.Drawing.Color.DarkGray,
            System.Drawing.Color.DarkGreen,
            System.Drawing.Color.DarkKhaki,
            System.Drawing.Color.DarkMagenta,
            System.Drawing.Color.DarkOliveGreen,
            System.Drawing.Color.DarkOrange,
            System.Drawing.Color.DarkOrchid,
            System.Drawing.Color.DarkRed,
            System.Drawing.Color.DarkSalmon,
            System.Drawing.Color.DarkSeaGreen,
            System.Drawing.Color.DarkSlateBlue,
            System.Drawing.Color.DarkSlateGray,
            System.Drawing.Color.DarkTurquoise,
            System.Drawing.Color.DarkViolet,
            System.Drawing.Color.DeepPink,
            System.Drawing.Color.DeepSkyBlue,
            System.Drawing.Color.DimGray,
            System.Drawing.Color.DodgerBlue,
            System.Drawing.Color.Firebrick,
            System.Drawing.Color.FloralWhite,
            System.Drawing.Color.ForestGreen,
            System.Drawing.Color.Fuchsia,
            System.Drawing.Color.Gainsboro,
            System.Drawing.Color.GhostWhite,
            System.Drawing.Color.Gold,
            System.Drawing.Color.Goldenrod,
            System.Drawing.Color.Gray,
            System.Drawing.Color.Green,
            System.Drawing.Color.GreenYellow,
            System.Drawing.Color.Honeydew,
            System.Drawing.Color.HotPink,
            System.Drawing.Color.IndianRed,
            System.Drawing.Color.Indigo,
            System.Drawing.Color.Ivory,
            System.Drawing.Color.Khaki,
            System.Drawing.Color.Lavender,
            System.Drawing.Color.LavenderBlush,
            System.Drawing.Color.LawnGreen,
            System.Drawing.Color.LemonChiffon,
            System.Drawing.Color.LightBlue,
            System.Drawing.Color.LightCoral,
            System.Drawing.Color.LightCyan,
            System.Drawing.Color.LightGoldenrodYellow,
            System.Drawing.Color.LightGray,
            System.Drawing.Color.LightGreen,
            System.Drawing.Color.LightPink,
            System.Drawing.Color.LightSalmon,
            System.Drawing.Color.LightSeaGreen,
            System.Drawing.Color.LightSkyBlue,
            System.Drawing.Color.LightSlateGray,
            System.Drawing.Color.LightSteelBlue,
            System.Drawing.Color.LightYellow,
            System.Drawing.Color.Lime,
            System.Drawing.Color.LimeGreen,
            System.Drawing.Color.Linen,
            System.Drawing.Color.Magenta,
            System.Drawing.Color.Maroon,
            System.Drawing.Color.MediumAquamarine,
            System.Drawing.Color.MediumBlue,
            System.Drawing.Color.MediumOrchid,
            System.Drawing.Color.MediumPurple,
            System.Drawing.Color.MediumSeaGreen,
            System.Drawing.Color.MediumSlateBlue,
            System.Drawing.Color.MediumSpringGreen,
            System.Drawing.Color.MediumTurquoise,
            System.Drawing.Color.MediumVioletRed,
            System.Drawing.Color.MidnightBlue,
            System.Drawing.Color.MintCream,
            System.Drawing.Color.MistyRose,
            System.Drawing.Color.Moccasin,
            System.Drawing.Color.NavajoWhite,
            System.Drawing.Color.Navy,
            System.Drawing.Color.OldLace,
            System.Drawing.Color.Olive,
            System.Drawing.Color.OliveDrab,
            System.Drawing.Color.Orange,
            System.Drawing.Color.OrangeRed,
            System.Drawing.Color.Orchid,
            System.Drawing.Color.PaleGoldenrod,
            System.Drawing.Color.PaleGreen,
            System.Drawing.Color.PaleTurquoise,
            System.Drawing.Color.PaleVioletRed,
            System.Drawing.Color.PapayaWhip,
            System.Drawing.Color.PeachPuff,
            System.Drawing.Color.Peru,
            System.Drawing.Color.Pink,
            System.Drawing.Color.Plum,
            System.Drawing.Color.PowderBlue,
            System.Drawing.Color.Purple,
            System.Drawing.Color.Red,
            System.Drawing.Color.RosyBrown,
            System.Drawing.Color.RoyalBlue,
            System.Drawing.Color.SaddleBrown,
            System.Drawing.Color.Salmon,
            System.Drawing.Color.SandyBrown,
            System.Drawing.Color.SeaGreen,
            System.Drawing.Color.SeaShell,
            System.Drawing.Color.Sienna,
            System.Drawing.Color.Silver,
            System.Drawing.Color.SkyBlue,
            System.Drawing.Color.SlateBlue,
            System.Drawing.Color.SlateGray,
            System.Drawing.Color.Snow,
            System.Drawing.Color.SpringGreen,
            System.Drawing.Color.SteelBlue,
            System.Drawing.Color.Tan,
            System.Drawing.Color.Teal,
            System.Drawing.Color.Thistle,
            System.Drawing.Color.Tomato,
            System.Drawing.Color.Turquoise,
            System.Drawing.Color.Violet,
            System.Drawing.Color.Wheat,
            System.Drawing.Color.White,
            System.Drawing.Color.WhiteSmoke,
            System.Drawing.Color.Yellow,
            System.Drawing.Color.YellowGreen,
            System.Drawing.SystemColors.ButtonFace,
            System.Drawing.SystemColors.ButtonHighlight,
            System.Drawing.SystemColors.ButtonShadow,
            System.Drawing.SystemColors.GradientActiveCaption,
            System.Drawing.SystemColors.GradientInactiveCaption,
            System.Drawing.SystemColors.MenuBar,
            System.Drawing.SystemColors.MenuHighlight});
      this.m_cbColor.Location = new System.Drawing.Point(126, 44);
      this.m_cbColor.Name = "m_cbColor";
      this.m_cbColor.Size = new System.Drawing.Size(136, 21);
      this.m_cbColor.TabIndex = 44;
      this.m_cbColor.SelectionChangeCommitted += new System.EventHandler(this.EhColor_SelectionChangeCommitted);
      // 
      // m_cbFont
      // 
      this.m_cbFont.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.m_cbFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_cbFont.ItemHeight = 15;
      this.m_cbFont.Location = new System.Drawing.Point(126, 71);
      this.m_cbFont.Name = "m_cbFont";
      this.m_cbFont.Size = new System.Drawing.Size(136, 21);
      this.m_cbFont.TabIndex = 23;
      // 
      // m_cbFontSize
      // 
      this.m_cbFontSize.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.m_cbFontSize.ItemHeight = 15;
      this.m_cbFontSize.Location = new System.Drawing.Point(126, 98);
      this.m_cbFontSize.Name = "m_cbFontSize";
      this.m_cbFontSize.Size = new System.Drawing.Size(136, 21);
      this.m_cbFontSize.TabIndex = 21;
      this.m_cbFontSize.Text = "comboBox1";
      // 
      // _fontControlsGlue
      // 
      this._fontControlsGlue.CbFont = this.m_cbFont;
      this._fontControlsGlue.CbFontSize = this.m_cbFontSize;
      this._fontControlsGlue.FontUnit = System.Drawing.GraphicsUnit.World;
      this._fontControlsGlue.FontChanged += new System.EventHandler(this.EhFont_SelectionChangedCommitted);
      // 
      // _backgroundGlue
      // 
      this._backgroundGlue.CbBrush = this._backgroundBrush;
      this._backgroundGlue.CbStyle = this._cbBackgroundStyle;
      this._backgroundGlue.LabelBrush = null;
      // 
      // m_edRotation
      // 
      this.m_edRotation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.m_edRotation.FormattingEnabled = true;
      this.m_edRotation.ItemHeight = 15;
      this.m_edRotation.Location = new System.Drawing.Point(296, 71);
      this.m_edRotation.Name = "m_edRotation";
      this.m_edRotation.Size = new System.Drawing.Size(136, 21);
      this.m_edRotation.TabIndex = 47;
      // 
      // XYPlotLabelStyleControl
      // 
      this.Controls.Add(this.m_edRotation);
      this.Controls.Add(this._cbBackgroundStyle);
      this.Controls.Add(this._backgroundBrush);
      this.Controls.Add(this.m_cbColor);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label2);
      this.Controls.Add(this._btSelectLabelColumn);
      this.Controls.Add(this._edLabelColumn);
      this.Controls.Add(this.m_chkIndependentColor);
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
      this.Controls.Add(this.label6);
      this.Name = "XYPlotLabelStyleControl";
      this.Size = new System.Drawing.Size(440, 253);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

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

    


    private void EhFont_SelectionChangedCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_FontChanged(_fontControlsGlue.Font);
    }   

    private void EhColor_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        Controller.EhView_ColorChanged(m_cbColor.ColorChoice);
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
        ListNode name = (ListNode)this.m_cbAttachedAxis.SelectedItem;
        Controller.EhView_AttachedAxisChanged(name);
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

    public static void InitComboBox(System.Windows.Forms.ComboBox box, List<ListNode> names, int sel)
    {
      box.Items.Clear();
      box.Items.AddRange(names.ToArray());
      box.SelectedIndex = sel;
    }

    public static void InitFontComboBox(ComboBox box, string choosenfontfamily)
    {
      box.Items.Clear();
      foreach(System.Drawing.FontFamily fontfamily in System.Drawing.FontFamily.Families)
        box.Items.Add(fontfamily.Name);
      box.SelectedItem = choosenfontfamily;
    }

    public Altaxo.Graph.Gdi.Background.IBackgroundStyle Background
    {
      get
      {
        return _backgroundGlue.BackgroundStyle;
      }
      set
      {
        _backgroundGlue.BackgroundStyle = value;
      }
    }

    public void LabelColumn_Initialize(string labelColumnAsText)
    {
      this._edLabelColumn.Text = labelColumnAsText;
    }

    public void Font_Initialize(Font font)
    {
      _fontControlsGlue.Font = font;
    }


    public void IndependentColor_Initialize(bool bIndependent)
    {
      this.m_chkIndependentColor.Checked = bIndependent;
      this.m_cbColor.Enabled = bIndependent;      
    }

    public void Color_Initialize(System.Drawing.Color color)
    {
      this.m_cbColor.ColorChoice = color;
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

    public void AttachedAxis_Initialize(List<ListNode> names, int sel)
    {
      InitComboBox(this.m_cbAttachedAxis,names,sel);
    }

    public float Rotation
    {

      get
      {
        return this.m_edRotation.Rotation;
      }
      set
      {
        this.m_edRotation.Rotation = value;
      }
    }

    public void XOffset_Initialize(string text)
    {
      this.m_edXOffset.Text = text;
    }

    public void YOffset_Initialize(string text)
    {
      this.m_edYOffset.Text = text;
    }


  

    #endregion


   
  }
}
