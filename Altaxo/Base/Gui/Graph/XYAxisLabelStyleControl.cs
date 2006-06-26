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
using Altaxo.Main.GUI;

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Graphical interface for adjusting the style of a XYPlotLabel.
  /// </summary>
  [UserControlForController(typeof(IXYAxisLabelStyleViewEventSink))]
  public class XYAxisLabelStyleControl : System.Windows.Forms.UserControl, IXYAxisLabelStyleView
  {
    private IXYAxisLabelStyleViewEventSink _controller;
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
    private System.Windows.Forms.ComboBox m_cbFontSize;
    private System.Windows.Forms.TextBox m_edRotation;
    private System.Windows.Forms.ComboBox m_cbHorizontalAlignment;
    private System.Windows.Forms.ComboBox m_cbVerticalAlignment;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.ComboBox m_cbLabelStyle;
    private System.Windows.Forms.CheckBox _chkAutomaticAlignment;
    private System.Windows.Forms.Label label8;
    private BackgroundStyleControl _ctrlBackground;
    private Altaxo.Gui.Common.Drawing.ColorComboBox m_cbColor;
    private IContainer components;

    public XYAxisLabelStyleControl()
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
      this.components = new System.ComponentModel.Container();
      this.label24 = new System.Windows.Forms.Label();
      this.m_edYOffset = new System.Windows.Forms.TextBox();
      this.m_edXOffset = new System.Windows.Forms.TextBox();
      this.label23 = new System.Windows.Forms.Label();
      this.m_cbFontSize = new System.Windows.Forms.ComboBox();
      this.m_cbHorizontalAlignment = new System.Windows.Forms.ComboBox();
      this.m_cbFont = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.m_cbVerticalAlignment = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.m_edRotation = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.m_cbLabelStyle = new System.Windows.Forms.ComboBox();
      this._chkAutomaticAlignment = new System.Windows.Forms.CheckBox();
      this.label8 = new System.Windows.Forms.Label();
      this._ctrlBackground = new Altaxo.Gui.Graph.BackgroundStyleControl();
      this.m_cbColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
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
      // m_cbFontSize
      // 
      this.m_cbFontSize.Location = new System.Drawing.Point(112, 72);
      this.m_cbFontSize.Name = "m_cbFontSize";
      this.m_cbFontSize.Size = new System.Drawing.Size(136, 21);
      this.m_cbFontSize.TabIndex = 21;
      this.m_cbFontSize.Validating += new System.ComponentModel.CancelEventHandler(this.m_cbFontSize_Validating);
      this.m_cbFontSize.SelectionChangeCommitted += new System.EventHandler(this.EhFontSize_SelectionChangeCommitted);
      // 
      // m_cbHorizontalAlignment
      // 
      this.m_cbHorizontalAlignment.Location = new System.Drawing.Point(112, 136);
      this.m_cbHorizontalAlignment.Name = "m_cbHorizontalAlignment";
      this.m_cbHorizontalAlignment.Size = new System.Drawing.Size(136, 21);
      this.m_cbHorizontalAlignment.TabIndex = 22;
      this.m_cbHorizontalAlignment.Text = "comboBox1";
      this.m_cbHorizontalAlignment.SelectionChangeCommitted += new System.EventHandler(this.EhHorizontalAlignment_SelectionChangeCommitted);
      // 
      // m_cbFont
      // 
      this.m_cbFont.Location = new System.Drawing.Point(112, 40);
      this.m_cbFont.Name = "m_cbFont";
      this.m_cbFont.Size = new System.Drawing.Size(136, 21);
      this.m_cbFont.TabIndex = 23;
      this.m_cbFont.Text = "comboBox1";
      this.m_cbFont.SelectionChangeCommitted += new System.EventHandler(this.EhFont_SelectionChangedCommitted);
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(48, 40);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(56, 16);
      this.label1.TabIndex = 24;
      this.label1.Text = "Font:";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(32, 72);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(72, 16);
      this.label3.TabIndex = 26;
      this.label3.Text = "Font size:";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(16, 136);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(88, 16);
      this.label4.TabIndex = 27;
      this.label4.Text = "Horz. alignment:";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.m_edXOffset);
      this.groupBox1.Controls.Add(this.label23);
      this.groupBox1.Controls.Add(this.m_edYOffset);
      this.groupBox1.Controls.Add(this.label24);
      this.groupBox1.Location = new System.Drawing.Point(272, 96);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(136, 96);
      this.groupBox1.TabIndex = 29;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Offset (%)";
      // 
      // m_cbVerticalAlignment
      // 
      this.m_cbVerticalAlignment.Location = new System.Drawing.Point(112, 168);
      this.m_cbVerticalAlignment.Name = "m_cbVerticalAlignment";
      this.m_cbVerticalAlignment.Size = new System.Drawing.Size(136, 21);
      this.m_cbVerticalAlignment.TabIndex = 30;
      this.m_cbVerticalAlignment.Text = "comboBox1";
      this.m_cbVerticalAlignment.SelectionChangeCommitted += new System.EventHandler(this.EhVerticalAlignment_SelectionChangeCommitted);
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(24, 168);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(88, 16);
      this.label5.TabIndex = 31;
      this.label5.Text = "Vert. alignment:";
      this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_edRotation
      // 
      this.m_edRotation.Location = new System.Drawing.Point(344, 40);
      this.m_edRotation.Name = "m_edRotation";
      this.m_edRotation.Size = new System.Drawing.Size(56, 20);
      this.m_edRotation.TabIndex = 20;
      this.m_edRotation.Validating += new System.ComponentModel.CancelEventHandler(this.EhRotation_Validating);
      // 
      // label6
      // 
      this.label6.Location = new System.Drawing.Point(312, 16);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(96, 16);
      this.label6.TabIndex = 20;
      this.label6.Text = "Rotation (deg.):";
      this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(64, 8);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(40, 16);
      this.label2.TabIndex = 36;
      this.label2.Text = "Color:";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label7
      // 
      this.label7.Location = new System.Drawing.Point(32, 232);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(64, 16);
      this.label7.TabIndex = 37;
      this.label7.Text = "Label style:";
      this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_cbLabelStyle
      // 
      this.m_cbLabelStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_cbLabelStyle.Location = new System.Drawing.Point(112, 232);
      this.m_cbLabelStyle.Name = "m_cbLabelStyle";
      this.m_cbLabelStyle.Size = new System.Drawing.Size(232, 21);
      this.m_cbLabelStyle.TabIndex = 38;
      this.m_cbLabelStyle.SelectionChangeCommitted += new System.EventHandler(this.m_cbLabelStyle_SelectionChangeCommitted);
      // 
      // _chkAutomaticAlignment
      // 
      this._chkAutomaticAlignment.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this._chkAutomaticAlignment.Location = new System.Drawing.Point(24, 112);
      this._chkAutomaticAlignment.Name = "_chkAutomaticAlignment";
      this._chkAutomaticAlignment.Size = new System.Drawing.Size(104, 16);
      this._chkAutomaticAlignment.TabIndex = 39;
      this._chkAutomaticAlignment.Text = "AutoAlign:";
      this._chkAutomaticAlignment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this._chkAutomaticAlignment.CheckedChanged += new System.EventHandler(this._chkAutomaticAlignment_CheckedChanged);
      // 
      // label8
      // 
      this.label8.Location = new System.Drawing.Point(32, 205);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(72, 16);
      this.label8.TabIndex = 41;
      this.label8.Text = "Background:";
      this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // _ctrlBackground
      // 
      this._ctrlBackground.Controller = null;
      this._ctrlBackground.Location = new System.Drawing.Point(110, 196);
      this._ctrlBackground.Name = "_ctrlBackground";
      this._ctrlBackground.Size = new System.Drawing.Size(262, 30);
      this._ctrlBackground.TabIndex = 42;
      // 
      // m_cbColor
      // 
      this.m_cbColor.Color = System.Drawing.Color.Black;
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
            System.Drawing.SystemColors.MenuHighlight});
      this.m_cbColor.Location = new System.Drawing.Point(112, 8);
      this.m_cbColor.Name = "m_cbColor";
      this.m_cbColor.Size = new System.Drawing.Size(136, 21);
      this.m_cbColor.TabIndex = 43;
      this.m_cbColor.SelectionChangeCommitted += new System.EventHandler(this.EhColor_SelectionChangeCommitted);
      // 
      // XYAxisLabelStyleControl
      // 
      this.Controls.Add(this.m_cbColor);
      this.Controls.Add(this._ctrlBackground);
      this.Controls.Add(this.label8);
      this.Controls.Add(this._chkAutomaticAlignment);
      this.Controls.Add(this.m_cbLabelStyle);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.m_cbVerticalAlignment);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.m_cbFont);
      this.Controls.Add(this.m_cbHorizontalAlignment);
      this.Controls.Add(this.m_cbFontSize);
      this.Controls.Add(this.m_edRotation);
      this.Controls.Add(this.label6);
      this.Name = "XYAxisLabelStyleControl";
      this.Size = new System.Drawing.Size(440, 360);
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
       
          Controller.EhView_ColorChanged(m_cbColor.Color);
        
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

    private void m_cbFontSize_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
      {
        string name = (string)this.m_cbFontSize.Text;
        Controller.EhView_FontSizeChanged(name);
      }
    }

    private void m_cbLabelStyle_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        Controller.EhView_LabelStyleChanged(this.m_cbLabelStyle.SelectedIndex);
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

   

    

    private void _chkAutomaticAlignment_CheckedChanged(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_AutomaticAlignmentChanged(this._chkAutomaticAlignment.Checked);

      this.m_cbHorizontalAlignment.Enabled = !_chkAutomaticAlignment.Checked;
      this.m_cbVerticalAlignment.Enabled = !_chkAutomaticAlignment.Checked;
    }
  


   
    #region IXYAxisLabelStyleView Members

    public IXYAxisLabelStyleViewEventSink Controller
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
      box.Text = name;
    }


    public static void InitFontComboBox(ComboBox box, string choosenfontfamily)
    {
      box.Items.Clear();
      foreach(System.Drawing.FontFamily fontfamily in System.Drawing.FontFamily.Families)
        box.Items.Add(fontfamily.Name);
      box.SelectedItem = choosenfontfamily;
    }


   
    public void Font_Initialize(string fontfamily)
    {
      InitFontComboBox(this.m_cbFont,fontfamily);
    }


   

    public void Color_Initialize(System.Drawing.Color color)
    {
      this.m_cbColor.Color=color;
    }

    public object BackgroundControl_Initialize(BackgroundStyleController controller)
    {
      _ctrlBackground.Controller = controller;
      return _ctrlBackground;
    }

    public void FontSize_Initialize(string[] names, string name)
    {
      InitComboBox(this.m_cbFontSize,names,name);
    }
    
    public void LabelStyle_Initialize(string[] names, string name)
    {
      InitComboBox(this.m_cbLabelStyle,names,name);
    }
    

    public void HorizontalAlignment_Initialize(string[] names, string name)
    {
      InitComboBox(this.m_cbHorizontalAlignment,names,name);
    }

    public void VerticalAlignment_Initialize(string[] names, string name)
    {
      InitComboBox(this.m_cbVerticalAlignment,names,name);
    }

    public void AutomaticAlignment_Initialize(bool value)
    {
      _chkAutomaticAlignment.Checked = value;
      this.m_cbHorizontalAlignment.Enabled = !_chkAutomaticAlignment.Checked;
      this.m_cbVerticalAlignment.Enabled = !_chkAutomaticAlignment.Checked;
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


 
   

    #endregion


  




   
  }
}
