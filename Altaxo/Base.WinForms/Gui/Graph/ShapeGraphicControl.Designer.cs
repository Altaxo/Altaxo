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

namespace Altaxo.Gui.Graph
{
  partial class ShapeGraphicControl
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
      this._grpbPen = new System.Windows.Forms.GroupBox();
      this._grpbBrush = new System.Windows.Forms.GroupBox();
      this._brushTableLayout = new System.Windows.Forms.TableLayoutPanel();
      this._lblFillBrush = new System.Windows.Forms.Label();
      this._lblFillBrushEnable = new System.Windows.Forms.Label();
      this._chkFillShapeEnable = new System.Windows.Forms.CheckBox();
      this._grpbPosition = new System.Windows.Forms.GroupBox();
      this._ctrlPenColorTypeThickness = new Altaxo.Gui.Common.Drawing.ColorTypeThicknessPenControl();
      this._cbFillBrush = new Altaxo.Gui.Common.Drawing.BrushColorComboBox();
      this._ctrlPosSize = new Altaxo.Gui.Graph.PositionSizeRotationScaleControl();
      this.flowLayoutPanel1.SuspendLayout();
      this._grpbPen.SuspendLayout();
      this._grpbBrush.SuspendLayout();
      this._brushTableLayout.SuspendLayout();
      this._grpbPosition.SuspendLayout();
      this.SuspendLayout();
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel1.Controls.Add(this._grpbPen);
      this.flowLayoutPanel1.Controls.Add(this._grpbBrush);
      this.flowLayoutPanel1.Controls.Add(this._grpbPosition);
      this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(356, 351);
      this.flowLayoutPanel1.TabIndex = 0;
      // 
      // _grpbPen
      // 
      this._grpbPen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this._grpbPen.AutoSize = true;
      this._grpbPen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._grpbPen.Controls.Add(this._ctrlPenColorTypeThickness);
      this._grpbPen.Location = new System.Drawing.Point(3, 3);
      this._grpbPen.Name = "_grpbPen";
      this._grpbPen.Size = new System.Drawing.Size(350, 125);
      this._grpbPen.TabIndex = 0;
      this._grpbPen.TabStop = false;
      this._grpbPen.Text = "Pen:";
      // 
      // _grpbBrush
      // 
      this._grpbBrush.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this._grpbBrush.AutoSize = true;
      this._grpbBrush.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._grpbBrush.Controls.Add(this._brushTableLayout);
      this._grpbBrush.Location = new System.Drawing.Point(3, 134);
      this._grpbBrush.Name = "_grpbBrush";
      this._grpbBrush.Size = new System.Drawing.Size(350, 85);
      this._grpbBrush.TabIndex = 1;
      this._grpbBrush.TabStop = false;
      this._grpbBrush.Text = "Filling:";
      // 
      // _brushTableLayout
      // 
      this._brushTableLayout.AutoSize = true;
      this._brushTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._brushTableLayout.ColumnCount = 2;
      this._brushTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._brushTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._brushTableLayout.Controls.Add(this._cbFillBrush, 1, 1);
      this._brushTableLayout.Controls.Add(this._lblFillBrush, 0, 1);
      this._brushTableLayout.Controls.Add(this._lblFillBrushEnable, 0, 0);
      this._brushTableLayout.Controls.Add(this._chkFillShapeEnable, 1, 0);
      this._brushTableLayout.Location = new System.Drawing.Point(6, 19);
      this._brushTableLayout.Name = "_brushTableLayout";
      this._brushTableLayout.RowCount = 2;
      this._brushTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._brushTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._brushTableLayout.Size = new System.Drawing.Size(188, 47);
      this._brushTableLayout.TabIndex = 0;
      // 
      // _lblFillBrush
      // 
      this._lblFillBrush.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblFillBrush.AutoSize = true;
      this._lblFillBrush.Location = new System.Drawing.Point(22, 27);
      this._lblFillBrush.Name = "_lblFillBrush";
      this._lblFillBrush.Size = new System.Drawing.Size(37, 13);
      this._lblFillBrush.TabIndex = 1;
      this._lblFillBrush.Text = "Brush:";
      // 
      // _lblFillBrushEnable
      // 
      this._lblFillBrushEnable.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblFillBrushEnable.AutoSize = true;
      this._lblFillBrushEnable.Location = new System.Drawing.Point(3, 3);
      this._lblFillBrushEnable.Name = "_lblFillBrushEnable";
      this._lblFillBrushEnable.Size = new System.Drawing.Size(56, 13);
      this._lblFillBrushEnable.TabIndex = 2;
      this._lblFillBrushEnable.Text = "Fill Shape:";
      // 
      // _chkFillShapeEnable
      // 
      this._chkFillShapeEnable.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._chkFillShapeEnable.AutoSize = true;
      this._chkFillShapeEnable.Location = new System.Drawing.Point(65, 3);
      this._chkFillShapeEnable.Name = "_chkFillShapeEnable";
      this._chkFillShapeEnable.Size = new System.Drawing.Size(15, 14);
      this._chkFillShapeEnable.TabIndex = 3;
      this._chkFillShapeEnable.UseVisualStyleBackColor = true;
      this._chkFillShapeEnable.CheckedChanged += new System.EventHandler(this.EhIsShapedFilled_CheckChanged);
      // 
      // _grpbPosition
      // 
      this._grpbPosition.AutoSize = true;
      this._grpbPosition.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._grpbPosition.Controls.Add(this._ctrlPosSize);
      this._grpbPosition.Location = new System.Drawing.Point(3, 225);
      this._grpbPosition.Name = "_grpbPosition";
      this._grpbPosition.Size = new System.Drawing.Size(350, 123);
      this._grpbPosition.TabIndex = 2;
      this._grpbPosition.TabStop = false;
      this._grpbPosition.Text = "Position/Size";
      // 
      // _ctrlPenColorTypeThickness
      // 
      this._ctrlPenColorTypeThickness.AutoSize = true;
      this._ctrlPenColorTypeThickness.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._ctrlPenColorTypeThickness.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._ctrlPenColorTypeThickness.Controller = null;
      this._ctrlPenColorTypeThickness.DocPen = null;
      this._ctrlPenColorTypeThickness.Location = new System.Drawing.Point(6, 19);
      this._ctrlPenColorTypeThickness.Name = "_ctrlPenColorTypeThickness";
      this._ctrlPenColorTypeThickness.Size = new System.Drawing.Size(198, 87);
      this._ctrlPenColorTypeThickness.TabIndex = 0;
      // 
      // _cbFillBrush
      // 
      this._cbFillBrush.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbFillBrush.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._cbFillBrush.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbFillBrush.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbFillBrush.FormattingEnabled = true;
      this._cbFillBrush.ItemHeight = 15;
      this._cbFillBrush.Items.AddRange(new object[] {
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
      this._cbFillBrush.Location = new System.Drawing.Point(65, 23);
      this._cbFillBrush.Name = "_cbFillBrush";
      this._cbFillBrush.Size = new System.Drawing.Size(120, 21);
      this._cbFillBrush.TabIndex = 0;
      // 
      // _ctrlPosSize
      // 
      this._ctrlPosSize.AutoSize = true;
      this._ctrlPosSize.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._ctrlPosSize.Location = new System.Drawing.Point(6, 19);
      this._ctrlPosSize.Name = "_ctrlPosSize";
      this._ctrlPosSize.Size = new System.Drawing.Size(338, 85);
      this._ctrlPosSize.TabIndex = 0;
      // 
      // ShapeGraphicControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this.flowLayoutPanel1);
      this.Name = "ShapeGraphicControl";
      this.Size = new System.Drawing.Size(362, 357);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this._grpbPen.ResumeLayout(false);
      this._grpbPen.PerformLayout();
      this._grpbBrush.ResumeLayout(false);
      this._grpbBrush.PerformLayout();
      this._brushTableLayout.ResumeLayout(false);
      this._brushTableLayout.PerformLayout();
      this._grpbPosition.ResumeLayout(false);
      this._grpbPosition.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.GroupBox _grpbPen;
    private Altaxo.Gui.Common.Drawing.ColorTypeThicknessPenControl _ctrlPenColorTypeThickness;
    private System.Windows.Forms.GroupBox _grpbBrush;
    private Altaxo.Gui.Common.Drawing.BrushColorComboBox _cbFillBrush;
    private System.Windows.Forms.TableLayoutPanel _brushTableLayout;
    private System.Windows.Forms.Label _lblFillBrush;
    private System.Windows.Forms.Label _lblFillBrushEnable;
    private System.Windows.Forms.CheckBox _chkFillShapeEnable;
    private System.Windows.Forms.GroupBox _grpbPosition;
    private PositionSizeRotationScaleControl _ctrlPosSize;
  }
}
