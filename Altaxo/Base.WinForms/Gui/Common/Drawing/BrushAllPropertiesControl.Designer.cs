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

namespace Altaxo.Gui.Common.Drawing
{
  partial class BrushAllPropertiesControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this._lblTextureScale = new System.Windows.Forms.Label();
      this._lblGradientScale = new System.Windows.Forms.Label();
      this._lblGradientFocus = new System.Windows.Forms.Label();
      this._lblWrapMode = new System.Windows.Forms.Label();
      this._cbColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._cbBrushType = new Altaxo.Gui.Common.Drawing.BrushTypeComboBox();
      this._cbBackColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._lblBrushColor = new System.Windows.Forms.Label();
      this._lblBackColor = new System.Windows.Forms.Label();
      this._lblBrushType = new System.Windows.Forms.Label();
      this._lblHatchStyle = new System.Windows.Forms.Label();
      this._cbWrapMode = new Altaxo.Gui.Common.Drawing.WrapModeComboBox();
      this._cbHatchStyle = new Altaxo.Gui.Common.Drawing.HatchStyleComboBox();
      this._cbGradientMode = new Altaxo.Gui.Common.Drawing.LinearGradientModeComboBox();
      this._lblGradientMode = new System.Windows.Forms.Label();
      this._cbGradientShape = new Altaxo.Gui.Common.Drawing.LinearGradientShapeComboBox();
      this._lblGradientShape = new System.Windows.Forms.Label();
      this._cbGradientFocus = new Altaxo.Gui.Common.Drawing.GradientFocusComboBox();
      this._cbGradientScale = new Altaxo.Gui.Common.Drawing.GradientScaleComboBox();
      this._cbTextureScale = new Altaxo.Gui.Common.Drawing.TextureScaleComboBox();
      this._lblTextureImage = new System.Windows.Forms.Label();
      this._cbTextureImage = new Altaxo.Gui.Common.Drawing.TextureImageComboBox();
      this._brushPreviewPanel = new System.Windows.Forms.Panel();
      this._brushGlue = new Altaxo.Gui.Common.Drawing.BrushControlsGlue();
      this._lblExchangeColors = new System.Windows.Forms.Label();
      this._chkExchangeColors = new System.Windows.Forms.CheckBox();
      this.flowLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel1.Controls.Add(this.tableLayoutPanel1);
      this.flowLayoutPanel1.Controls.Add(this._brushPreviewPanel);
      this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(402, 343);
      this.flowLayoutPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this._lblTextureScale, 0, 5);
      this.tableLayoutPanel1.Controls.Add(this._lblGradientScale, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this._lblGradientFocus, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this._lblWrapMode, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this._cbColor, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this._cbBrushType, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this._cbBackColor, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this._lblBrushColor, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this._lblBackColor, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this._lblBrushType, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this._lblHatchStyle, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this._cbWrapMode, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this._cbHatchStyle, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this._cbGradientMode, 3, 2);
      this.tableLayoutPanel1.Controls.Add(this._lblGradientMode, 2, 2);
      this.tableLayoutPanel1.Controls.Add(this._cbGradientShape, 3, 3);
      this.tableLayoutPanel1.Controls.Add(this._lblGradientShape, 2, 3);
      this.tableLayoutPanel1.Controls.Add(this._cbGradientFocus, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this._cbGradientScale, 1, 4);
      this.tableLayoutPanel1.Controls.Add(this._cbTextureScale, 1, 5);
      this.tableLayoutPanel1.Controls.Add(this._lblTextureImage, 2, 5);
      this.tableLayoutPanel1.Controls.Add(this._cbTextureImage, 3, 5);
      this.tableLayoutPanel1.Controls.Add(this._lblExchangeColors, 2, 4);
      this.tableLayoutPanel1.Controls.Add(this._chkExchangeColors, 3, 4);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 6;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(396, 162);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // _lblTextureScale
      // 
      this._lblTextureScale.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblTextureScale.AutoSize = true;
      this._lblTextureScale.Location = new System.Drawing.Point(8, 142);
      this._lblTextureScale.Name = "_lblTextureScale";
      this._lblTextureScale.Size = new System.Drawing.Size(58, 13);
      this._lblTextureScale.TabIndex = 21;
      this._lblTextureScale.Text = "Tex.Scale:";
      // 
      // _lblGradientScale
      // 
      this._lblGradientScale.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblGradientScale.AutoSize = true;
      this._lblGradientScale.Location = new System.Drawing.Point(11, 115);
      this._lblGradientScale.Name = "_lblGradientScale";
      this._lblGradientScale.Size = new System.Drawing.Size(55, 13);
      this._lblGradientScale.TabIndex = 17;
      this._lblGradientScale.Text = "Col.Scale:";
      // 
      // _lblGradientFocus
      // 
      this._lblGradientFocus.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblGradientFocus.AutoSize = true;
      this._lblGradientFocus.Location = new System.Drawing.Point(27, 88);
      this._lblGradientFocus.Name = "_lblGradientFocus";
      this._lblGradientFocus.Size = new System.Drawing.Size(39, 13);
      this._lblGradientFocus.TabIndex = 15;
      this._lblGradientFocus.Text = "Focus:";
      // 
      // _lblWrapMode
      // 
      this._lblWrapMode.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblWrapMode.AutoSize = true;
      this._lblWrapMode.Location = new System.Drawing.Point(3, 61);
      this._lblWrapMode.Name = "_lblWrapMode";
      this._lblWrapMode.Size = new System.Drawing.Size(63, 13);
      this._lblWrapMode.TabIndex = 9;
      this._lblWrapMode.Text = "WrapMode:";
      // 
      // _cbColor
      // 
      this._cbColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._cbColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbColor.FormattingEnabled = true;
      this._cbColor.ItemHeight = 15;
      this._cbColor.Items.AddRange(new object[] {
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
      this._cbColor.Location = new System.Drawing.Point(72, 3);
      this._cbColor.Name = "_cbColor";
      this._cbColor.Size = new System.Drawing.Size(121, 21);
      this._cbColor.TabIndex = 0;
      // 
      // _cbBrushType
      // 
      this._cbBrushType.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbBrushType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbBrushType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbBrushType.FormattingEnabled = true;
      this._cbBrushType.ItemHeight = 15;
      this._cbBrushType.Location = new System.Drawing.Point(272, 3);
      this._cbBrushType.Name = "_cbBrushType";
      this._cbBrushType.Size = new System.Drawing.Size(121, 21);
      this._cbBrushType.TabIndex = 1;
      // 
      // _cbBackColor
      // 
      this._cbBackColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbBackColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._cbBackColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbBackColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbBackColor.FormattingEnabled = true;
      this._cbBackColor.ItemHeight = 15;
      this._cbBackColor.Items.AddRange(new object[] {
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
      this._cbBackColor.Location = new System.Drawing.Point(72, 30);
      this._cbBackColor.Name = "_cbBackColor";
      this._cbBackColor.Size = new System.Drawing.Size(121, 21);
      this._cbBackColor.TabIndex = 2;
      // 
      // _lblBrushColor
      // 
      this._lblBrushColor.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblBrushColor.AutoSize = true;
      this._lblBrushColor.Location = new System.Drawing.Point(32, 7);
      this._lblBrushColor.Name = "_lblBrushColor";
      this._lblBrushColor.Size = new System.Drawing.Size(34, 13);
      this._lblBrushColor.TabIndex = 4;
      this._lblBrushColor.Text = "Color:";
      // 
      // _lblBackColor
      // 
      this._lblBackColor.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblBackColor.AutoSize = true;
      this._lblBackColor.Location = new System.Drawing.Point(7, 34);
      this._lblBackColor.Name = "_lblBackColor";
      this._lblBackColor.Size = new System.Drawing.Size(59, 13);
      this._lblBackColor.TabIndex = 5;
      this._lblBackColor.Text = "BackColor:";
      // 
      // _lblBrushType
      // 
      this._lblBrushType.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblBrushType.AutoSize = true;
      this._lblBrushType.Location = new System.Drawing.Point(205, 7);
      this._lblBrushType.Name = "_lblBrushType";
      this._lblBrushType.Size = new System.Drawing.Size(61, 13);
      this._lblBrushType.TabIndex = 6;
      this._lblBrushType.Text = "BrushType:";
      // 
      // _lblHatchStyle
      // 
      this._lblHatchStyle.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblHatchStyle.AutoSize = true;
      this._lblHatchStyle.Location = new System.Drawing.Point(204, 34);
      this._lblHatchStyle.Name = "_lblHatchStyle";
      this._lblHatchStyle.Size = new System.Drawing.Size(62, 13);
      this._lblHatchStyle.TabIndex = 7;
      this._lblHatchStyle.Text = "HatchStyle:";
      // 
      // _cbWrapMode
      // 
      this._cbWrapMode.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbWrapMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbWrapMode.FormattingEnabled = true;
      this._cbWrapMode.ItemHeight = 15;
      this._cbWrapMode.Location = new System.Drawing.Point(72, 57);
      this._cbWrapMode.Name = "_cbWrapMode";
      this._cbWrapMode.Size = new System.Drawing.Size(121, 21);
      this._cbWrapMode.TabIndex = 8;
      // 
      // _cbHatchStyle
      // 
      this._cbHatchStyle.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbHatchStyle.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbHatchStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbHatchStyle.FormattingEnabled = true;
      this._cbHatchStyle.ItemHeight = 15;
      this._cbHatchStyle.Location = new System.Drawing.Point(272, 30);
      this._cbHatchStyle.Name = "_cbHatchStyle";
      this._cbHatchStyle.Size = new System.Drawing.Size(121, 21);
      this._cbHatchStyle.TabIndex = 3;
      // 
      // _cbGradientMode
      // 
      this._cbGradientMode.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbGradientMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbGradientMode.FormattingEnabled = true;
      this._cbGradientMode.ItemHeight = 15;
      this._cbGradientMode.Location = new System.Drawing.Point(272, 57);
      this._cbGradientMode.Name = "_cbGradientMode";
      this._cbGradientMode.Size = new System.Drawing.Size(121, 21);
      this._cbGradientMode.TabIndex = 10;
      // 
      // _lblGradientMode
      // 
      this._lblGradientMode.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblGradientMode.AutoSize = true;
      this._lblGradientMode.Location = new System.Drawing.Point(203, 61);
      this._lblGradientMode.Name = "_lblGradientMode";
      this._lblGradientMode.Size = new System.Drawing.Size(63, 13);
      this._lblGradientMode.TabIndex = 11;
      this._lblGradientMode.Text = "Grad.Mode:";
      // 
      // _cbGradientShape
      // 
      this._cbGradientShape.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbGradientShape.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbGradientShape.FormattingEnabled = true;
      this._cbGradientShape.ItemHeight = 15;
      this._cbGradientShape.Location = new System.Drawing.Point(272, 84);
      this._cbGradientShape.Name = "_cbGradientShape";
      this._cbGradientShape.Size = new System.Drawing.Size(121, 21);
      this._cbGradientShape.TabIndex = 12;
      // 
      // _lblGradientShape
      // 
      this._lblGradientShape.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblGradientShape.AutoSize = true;
      this._lblGradientShape.Location = new System.Drawing.Point(199, 88);
      this._lblGradientShape.Name = "_lblGradientShape";
      this._lblGradientShape.Size = new System.Drawing.Size(67, 13);
      this._lblGradientShape.TabIndex = 13;
      this._lblGradientShape.Text = "Grad.Shape:";
      // 
      // _cbGradientFocus
      // 
      this._cbGradientFocus.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbGradientFocus.FormattingEnabled = true;
      this._cbGradientFocus.ItemHeight = 15;
      this._cbGradientFocus.Location = new System.Drawing.Point(72, 84);
      this._cbGradientFocus.Name = "_cbGradientFocus";
      this._cbGradientFocus.Size = new System.Drawing.Size(121, 21);
      this._cbGradientFocus.TabIndex = 14;
      // 
      // _cbGradientScale
      // 
      this._cbGradientScale.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbGradientScale.FormattingEnabled = true;
      this._cbGradientScale.ItemHeight = 15;
      this._cbGradientScale.Location = new System.Drawing.Point(72, 111);
      this._cbGradientScale.Name = "_cbGradientScale";
      this._cbGradientScale.Size = new System.Drawing.Size(121, 21);
      this._cbGradientScale.TabIndex = 16;
      // 
      // _cbTextureScale
      // 
      this._cbTextureScale.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbTextureScale.FormattingEnabled = true;
      this._cbTextureScale.ItemHeight = 15;
      this._cbTextureScale.Location = new System.Drawing.Point(72, 138);
      this._cbTextureScale.Name = "_cbTextureScale";
      this._cbTextureScale.Size = new System.Drawing.Size(121, 21);
      this._cbTextureScale.TabIndex = 20;
      // 
      // _lblTextureImage
      // 
      this._lblTextureImage.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblTextureImage.AutoSize = true;
      this._lblTextureImage.Location = new System.Drawing.Point(220, 142);
      this._lblTextureImage.Name = "_lblTextureImage";
      this._lblTextureImage.Size = new System.Drawing.Size(46, 13);
      this._lblTextureImage.TabIndex = 19;
      this._lblTextureImage.Text = "Texture:";
      // 
      // _cbTextureImage
      // 
      this._cbTextureImage.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbTextureImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbTextureImage.FormattingEnabled = true;
      this._cbTextureImage.ItemHeight = 15;
      this._cbTextureImage.Location = new System.Drawing.Point(272, 138);
      this._cbTextureImage.Name = "_cbTextureImage";
      this._cbTextureImage.Size = new System.Drawing.Size(121, 21);
      this._cbTextureImage.TabIndex = 18;
      // 
      // _brushPreviewPanel
      // 
      this._brushPreviewPanel.Location = new System.Drawing.Point(3, 171);
      this._brushPreviewPanel.Name = "_brushPreviewPanel";
      this._brushPreviewPanel.Size = new System.Drawing.Size(387, 169);
      this._brushPreviewPanel.TabIndex = 1;
      this._brushPreviewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.EhBrushPreview_Paint);
      // 
      // _brushGlue
      // 
      this._brushGlue.CbBrushType = this._cbBrushType;
      this._brushGlue.CbColor1 = this._cbColor;
      this._brushGlue.CbColor2 = this._cbBackColor;
      this._brushGlue.CbGradientFocus = this._cbGradientFocus;
      this._brushGlue.CbGradientMode = this._cbGradientMode;
      this._brushGlue.CbGradientScale = this._cbGradientScale;
      this._brushGlue.CbGradientShape = this._cbGradientShape;
      this._brushGlue.CbHatchStyle = this._cbHatchStyle;
      this._brushGlue.CbTextureImage = this._cbTextureImage;
      this._brushGlue.CbTextureScale = this._cbTextureScale;
      this._brushGlue.CbWrapMode = this._cbWrapMode;
      this._brushGlue.ChkExchangeColors = this._chkExchangeColors;
      this._brushGlue.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._brushGlue.LabelColor2 = this._lblBackColor;
      this._brushGlue.LabelExchangeColors = this._lblExchangeColors;
      this._brushGlue.LabelGradientFocus = this._lblGradientFocus;
      this._brushGlue.LabelGradientMode = this._lblGradientMode;
      this._brushGlue.LabelGradientScale = this._lblGradientScale;
      this._brushGlue.LabelGradientShape = this._lblGradientShape;
      this._brushGlue.LabelHatchStyle = this._lblHatchStyle;
      this._brushGlue.LabelTextureImage = this._lblTextureImage;
      this._brushGlue.LabelTextureScale = this._lblTextureScale;
      this._brushGlue.LabelWrapMode = this._lblWrapMode;
      this._brushGlue.BrushChanged += new System.EventHandler(this.EhBrushChanged);
      // 
      // _lblExchangeColors
      // 
      this._lblExchangeColors.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblExchangeColors.AutoSize = true;
      this._lblExchangeColors.Location = new System.Drawing.Point(200, 115);
      this._lblExchangeColors.Name = "_lblExchangeColors";
      this._lblExchangeColors.Size = new System.Drawing.Size(66, 13);
      this._lblExchangeColors.TabIndex = 22;
      this._lblExchangeColors.Text = "Xchg colors:";
      // 
      // _chkExchangeColors
      // 
      this._chkExchangeColors.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._chkExchangeColors.AutoSize = true;
      this._chkExchangeColors.Location = new System.Drawing.Point(272, 114);
      this._chkExchangeColors.Name = "_chkExchangeColors";
      this._chkExchangeColors.Size = new System.Drawing.Size(15, 14);
      this._chkExchangeColors.TabIndex = 23;
      this._chkExchangeColors.UseVisualStyleBackColor = true;
      // 
      // BrushAllPropertiesControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.Controls.Add(this.flowLayoutPanel1);
      this.Name = "BrushAllPropertiesControl";
      this.Size = new System.Drawing.Size(405, 346);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private ColorComboBox _cbColor;
    private BrushTypeComboBox _cbBrushType;
    private ColorComboBox _cbBackColor;
    private HatchStyleComboBox _cbHatchStyle;
    private System.Windows.Forms.Label _lblBrushColor;
    private System.Windows.Forms.Label _lblBackColor;
    private System.Windows.Forms.Label _lblBrushType;
    private System.Windows.Forms.Label _lblHatchStyle;
    private System.Windows.Forms.Panel _brushPreviewPanel;
    private BrushControlsGlue _brushGlue;
    private System.Windows.Forms.Label _lblWrapMode;
    private WrapModeComboBox _cbWrapMode;
    private LinearGradientModeComboBox _cbGradientMode;
    private System.Windows.Forms.Label _lblGradientMode;
    private LinearGradientShapeComboBox _cbGradientShape;
    private System.Windows.Forms.Label _lblGradientShape;
    private System.Windows.Forms.Label _lblGradientFocus;
    private GradientFocusComboBox _cbGradientFocus;
    private System.Windows.Forms.Label _lblGradientScale;
    private GradientScaleComboBox _cbGradientScale;
    private TextureImageComboBox _cbTextureImage;
    private System.Windows.Forms.Label _lblTextureImage;
    private System.Windows.Forms.Label _lblTextureScale;
    private TextureScaleComboBox _cbTextureScale;
    private System.Windows.Forms.Label _lblExchangeColors;
    private System.Windows.Forms.CheckBox _chkExchangeColors;
  }
}
