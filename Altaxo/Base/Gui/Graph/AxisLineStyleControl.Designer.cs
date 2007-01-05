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
  partial class AxisLineStyleControl
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
      this._verticalPanel = new System.Windows.Forms.FlowLayoutPanel();
      this._lineGroupBox = new System.Windows.Forms.GroupBox();
      this._lineLayoutTable = new System.Windows.Forms.TableLayoutPanel();
      this._lblLineColor = new System.Windows.Forms.Label();
      this._lblLineThickness = new System.Windows.Forms.Label();
      this._chkEnableLine = new System.Windows.Forms.CheckBox();
      this._majorGroupBox = new System.Windows.Forms.GroupBox();
      this._majorLayoutTable = new System.Windows.Forms.TableLayoutPanel();
      this._chkCustomMajorColor = new System.Windows.Forms.CheckBox();
      this._chkCustomMajorThickness = new System.Windows.Forms.CheckBox();
      this._lblMajorLength = new System.Windows.Forms.Label();
      this._majorWhichTicksLayout = new System.Windows.Forms.FlowLayoutPanel();
      this._chkWhichMajor1 = new System.Windows.Forms.CheckBox();
      this._chkWhichMajor2 = new System.Windows.Forms.CheckBox();
      this._minorGroupBox = new System.Windows.Forms.GroupBox();
      this._minorLayoutTable = new System.Windows.Forms.TableLayoutPanel();
      this._chkCustomMinorColor = new System.Windows.Forms.CheckBox();
      this._chkCustomMinorThickness = new System.Windows.Forms.CheckBox();
      this._lblMinorLength = new System.Windows.Forms.Label();
      this._minorWhichTicksLayout = new System.Windows.Forms.FlowLayoutPanel();
      this._chkWhichMinor1 = new System.Windows.Forms.CheckBox();
      this._chkWhichMinor2 = new System.Windows.Forms.CheckBox();
      this._lineBrushColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._lineLineThickness = new Altaxo.Gui.Common.Drawing.LineThicknessComboBox();
      this._majorLineColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._lineMajorThickness = new Altaxo.Gui.Common.Drawing.LineThicknessComboBox();
      this._lineMajorLength = new Altaxo.Gui.Common.Drawing.LineThicknessComboBox();
      this._minorLineColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._lineMinorThickness = new Altaxo.Gui.Common.Drawing.LineThicknessComboBox();
      this._lineMinorLength = new Altaxo.Gui.Common.Drawing.LineThicknessComboBox();
      this._linePenGlue = new Altaxo.Gui.Common.Drawing.PenControlsGlue();
      this._majorPenGlue = new Altaxo.Gui.Common.Drawing.PenControlsGlue();
      this._minorPenGlue = new Altaxo.Gui.Common.Drawing.PenControlsGlue();
      this._verticalPanel.SuspendLayout();
      this._lineGroupBox.SuspendLayout();
      this._lineLayoutTable.SuspendLayout();
      this._majorGroupBox.SuspendLayout();
      this._majorLayoutTable.SuspendLayout();
      this._majorWhichTicksLayout.SuspendLayout();
      this._minorGroupBox.SuspendLayout();
      this._minorLayoutTable.SuspendLayout();
      this._minorWhichTicksLayout.SuspendLayout();
      this.SuspendLayout();
      // 
      // _verticalPanel
      // 
      this._verticalPanel.AutoSize = true;
      this._verticalPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._verticalPanel.Controls.Add(this._lineGroupBox);
      this._verticalPanel.Controls.Add(this._majorGroupBox);
      this._verticalPanel.Controls.Add(this._minorGroupBox);
      this._verticalPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this._verticalPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this._verticalPanel.Location = new System.Drawing.Point(0, 0);
      this._verticalPanel.Name = "_verticalPanel";
      this._verticalPanel.Size = new System.Drawing.Size(409, 223);
      this._verticalPanel.TabIndex = 0;
      // 
      // _lineGroupBox
      // 
      this._lineGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this._lineGroupBox.AutoSize = true;
      this._lineGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._lineGroupBox.Controls.Add(this._lineLayoutTable);
      this._lineGroupBox.Location = new System.Drawing.Point(3, 3);
      this._lineGroupBox.Name = "_lineGroupBox";
      this._lineGroupBox.Size = new System.Drawing.Size(403, 67);
      this._lineGroupBox.TabIndex = 0;
      this._lineGroupBox.TabStop = false;
      this._lineGroupBox.Text = "Line";
      // 
      // _lineLayoutTable
      // 
      this._lineLayoutTable.AutoSize = true;
      this._lineLayoutTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._lineLayoutTable.ColumnCount = 5;
      this._lineLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._lineLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._lineLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this._lineLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._lineLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._lineLayoutTable.Controls.Add(this._lineBrushColor, 1, 1);
      this._lineLayoutTable.Controls.Add(this._lineLineThickness, 4, 1);
      this._lineLayoutTable.Controls.Add(this._lblLineColor, 0, 1);
      this._lineLayoutTable.Controls.Add(this._lblLineThickness, 3, 1);
      this._lineLayoutTable.Controls.Add(this._chkEnableLine, 1, 0);
      this._lineLayoutTable.Dock = System.Windows.Forms.DockStyle.Fill;
      this._lineLayoutTable.Location = new System.Drawing.Point(3, 16);
      this._lineLayoutTable.Name = "_lineLayoutTable";
      this._lineLayoutTable.RowCount = 2;
      this._lineLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._lineLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._lineLayoutTable.Size = new System.Drawing.Size(397, 48);
      this._lineLayoutTable.TabIndex = 0;
      // 
      // _lblLineColor
      // 
      this._lblLineColor.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblLineColor.AutoSize = true;
      this._lblLineColor.Location = new System.Drawing.Point(3, 29);
      this._lblLineColor.Name = "_lblLineColor";
      this._lblLineColor.Padding = new System.Windows.Forms.Padding(18, 0, 0, 0);
      this._lblLineColor.Size = new System.Drawing.Size(52, 13);
      this._lblLineColor.TabIndex = 2;
      this._lblLineColor.Text = "Color:";
      // 
      // _lblLineThickness
      // 
      this._lblLineThickness.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblLineThickness.AutoSize = true;
      this._lblLineThickness.Location = new System.Drawing.Point(208, 29);
      this._lblLineThickness.Name = "_lblLineThickness";
      this._lblLineThickness.Size = new System.Drawing.Size(59, 13);
      this._lblLineThickness.TabIndex = 3;
      this._lblLineThickness.Text = "Thickness:";
      // 
      // _chkEnableLine
      // 
      this._chkEnableLine.AutoSize = true;
      this._chkEnableLine.Location = new System.Drawing.Point(61, 3);
      this._chkEnableLine.Name = "_chkEnableLine";
      this._chkEnableLine.Size = new System.Drawing.Size(59, 17);
      this._chkEnableLine.TabIndex = 4;
      this._chkEnableLine.Text = "Enable";
      this._chkEnableLine.UseVisualStyleBackColor = true;
      // 
      // _majorGroupBox
      // 
      this._majorGroupBox.AutoSize = true;
      this._majorGroupBox.Controls.Add(this._majorLayoutTable);
      this._majorGroupBox.Location = new System.Drawing.Point(3, 76);
      this._majorGroupBox.Name = "_majorGroupBox";
      this._majorGroupBox.Size = new System.Drawing.Size(403, 69);
      this._majorGroupBox.TabIndex = 1;
      this._majorGroupBox.TabStop = false;
      this._majorGroupBox.Text = "Major Ticks";
      // 
      // _majorLayoutTable
      // 
      this._majorLayoutTable.AutoSize = true;
      this._majorLayoutTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._majorLayoutTable.ColumnCount = 4;
      this._majorLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._majorLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._majorLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._majorLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._majorLayoutTable.Controls.Add(this._chkCustomMajorColor, 0, 0);
      this._majorLayoutTable.Controls.Add(this._chkCustomMajorThickness, 2, 0);
      this._majorLayoutTable.Controls.Add(this._lblMajorLength, 0, 1);
      this._majorLayoutTable.Controls.Add(this._majorLineColor, 1, 0);
      this._majorLayoutTable.Controls.Add(this._lineMajorThickness, 3, 0);
      this._majorLayoutTable.Controls.Add(this._lineMajorLength, 1, 1);
      this._majorLayoutTable.Controls.Add(this._majorWhichTicksLayout, 3, 1);
      this._majorLayoutTable.Dock = System.Windows.Forms.DockStyle.Fill;
      this._majorLayoutTable.Location = new System.Drawing.Point(3, 16);
      this._majorLayoutTable.Name = "_majorLayoutTable";
      this._majorLayoutTable.RowCount = 2;
      this._majorLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._majorLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._majorLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this._majorLayoutTable.Size = new System.Drawing.Size(397, 50);
      this._majorLayoutTable.TabIndex = 0;
      // 
      // _chkCustomMajorColor
      // 
      this._chkCustomMajorColor.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._chkCustomMajorColor.AutoSize = true;
      this._chkCustomMajorColor.Location = new System.Drawing.Point(3, 4);
      this._chkCustomMajorColor.Name = "_chkCustomMajorColor";
      this._chkCustomMajorColor.Size = new System.Drawing.Size(53, 17);
      this._chkCustomMajorColor.TabIndex = 0;
      this._chkCustomMajorColor.Text = "Color:";
      this._chkCustomMajorColor.UseVisualStyleBackColor = true;
      this._chkCustomMajorColor.CheckedChanged += new System.EventHandler(this.EhIndividualMajorColor_CheckChanged);
      // 
      // _chkCustomMajorThickness
      // 
      this._chkCustomMajorThickness.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._chkCustomMajorThickness.AutoSize = true;
      this._chkCustomMajorThickness.Location = new System.Drawing.Point(189, 4);
      this._chkCustomMajorThickness.Name = "_chkCustomMajorThickness";
      this._chkCustomMajorThickness.Size = new System.Drawing.Size(78, 17);
      this._chkCustomMajorThickness.TabIndex = 1;
      this._chkCustomMajorThickness.Text = "Thickness:";
      this._chkCustomMajorThickness.UseVisualStyleBackColor = true;
      this._chkCustomMajorThickness.CheckedChanged += new System.EventHandler(this.EhIndividualMajorThickness_CheckChanged);
      // 
      // _lblMajorLength
      // 
      this._lblMajorLength.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblMajorLength.AutoSize = true;
      this._lblMajorLength.Location = new System.Drawing.Point(13, 31);
      this._lblMajorLength.Name = "_lblMajorLength";
      this._lblMajorLength.Size = new System.Drawing.Size(43, 13);
      this._lblMajorLength.TabIndex = 2;
      this._lblMajorLength.Text = "Length:";
      // 
      // _majorWhichTicksLayout
      // 
      this._majorWhichTicksLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._majorWhichTicksLayout.AutoSize = true;
      this._majorWhichTicksLayout.Controls.Add(this._chkWhichMajor1);
      this._majorWhichTicksLayout.Controls.Add(this._chkWhichMajor2);
      this._majorWhichTicksLayout.Location = new System.Drawing.Point(270, 25);
      this._majorWhichTicksLayout.Margin = new System.Windows.Forms.Padding(0);
      this._majorWhichTicksLayout.Name = "_majorWhichTicksLayout";
      this._majorWhichTicksLayout.Size = new System.Drawing.Size(127, 25);
      this._majorWhichTicksLayout.TabIndex = 6;
      // 
      // _chkWhichMajor1
      // 
      this._chkWhichMajor1.AutoSize = true;
      this._chkWhichMajor1.Location = new System.Drawing.Point(3, 3);
      this._chkWhichMajor1.Name = "_chkWhichMajor1";
      this._chkWhichMajor1.Size = new System.Drawing.Size(35, 17);
      this._chkWhichMajor1.TabIndex = 0;
      this._chkWhichMajor1.Text = "In";
      this._chkWhichMajor1.UseVisualStyleBackColor = true;
      // 
      // _chkWhichMajor2
      // 
      this._chkWhichMajor2.AutoSize = true;
      this._chkWhichMajor2.Location = new System.Drawing.Point(44, 3);
      this._chkWhichMajor2.Name = "_chkWhichMajor2";
      this._chkWhichMajor2.Size = new System.Drawing.Size(43, 17);
      this._chkWhichMajor2.TabIndex = 1;
      this._chkWhichMajor2.Text = "Out";
      this._chkWhichMajor2.UseVisualStyleBackColor = true;
      // 
      // _minorGroupBox
      // 
      this._minorGroupBox.AutoSize = true;
      this._minorGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._minorGroupBox.Controls.Add(this._minorLayoutTable);
      this._minorGroupBox.Location = new System.Drawing.Point(3, 151);
      this._minorGroupBox.Name = "_minorGroupBox";
      this._minorGroupBox.Size = new System.Drawing.Size(403, 69);
      this._minorGroupBox.TabIndex = 2;
      this._minorGroupBox.TabStop = false;
      this._minorGroupBox.Text = "Minor Ticks";
      // 
      // _minorLayoutTable
      // 
      this._minorLayoutTable.AutoSize = true;
      this._minorLayoutTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._minorLayoutTable.ColumnCount = 4;
      this._minorLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._minorLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._minorLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._minorLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._minorLayoutTable.Controls.Add(this._chkCustomMinorColor, 0, 0);
      this._minorLayoutTable.Controls.Add(this._chkCustomMinorThickness, 2, 0);
      this._minorLayoutTable.Controls.Add(this._lblMinorLength, 0, 1);
      this._minorLayoutTable.Controls.Add(this._minorLineColor, 1, 0);
      this._minorLayoutTable.Controls.Add(this._lineMinorThickness, 3, 0);
      this._minorLayoutTable.Controls.Add(this._lineMinorLength, 1, 1);
      this._minorLayoutTable.Controls.Add(this._minorWhichTicksLayout, 3, 1);
      this._minorLayoutTable.Dock = System.Windows.Forms.DockStyle.Fill;
      this._minorLayoutTable.Location = new System.Drawing.Point(3, 16);
      this._minorLayoutTable.Name = "_minorLayoutTable";
      this._minorLayoutTable.RowCount = 2;
      this._minorLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._minorLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._minorLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this._minorLayoutTable.Size = new System.Drawing.Size(397, 50);
      this._minorLayoutTable.TabIndex = 3;
      // 
      // _chkCustomMinorColor
      // 
      this._chkCustomMinorColor.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._chkCustomMinorColor.AutoSize = true;
      this._chkCustomMinorColor.Location = new System.Drawing.Point(3, 4);
      this._chkCustomMinorColor.Name = "_chkCustomMinorColor";
      this._chkCustomMinorColor.Size = new System.Drawing.Size(53, 17);
      this._chkCustomMinorColor.TabIndex = 0;
      this._chkCustomMinorColor.Text = "Color:";
      this._chkCustomMinorColor.UseVisualStyleBackColor = true;
      this._chkCustomMinorColor.CheckedChanged += new System.EventHandler(this.EhIndividualMinorColor_CheckChanged);
      // 
      // _chkCustomMinorThickness
      // 
      this._chkCustomMinorThickness.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._chkCustomMinorThickness.AutoSize = true;
      this._chkCustomMinorThickness.Location = new System.Drawing.Point(189, 4);
      this._chkCustomMinorThickness.Name = "_chkCustomMinorThickness";
      this._chkCustomMinorThickness.Size = new System.Drawing.Size(78, 17);
      this._chkCustomMinorThickness.TabIndex = 1;
      this._chkCustomMinorThickness.Text = "Thickness:";
      this._chkCustomMinorThickness.UseVisualStyleBackColor = true;
      this._chkCustomMinorThickness.CheckedChanged += new System.EventHandler(this.EhIndividualMinorThickness_CheckChanged);
      // 
      // _lblMinorLength
      // 
      this._lblMinorLength.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblMinorLength.AutoSize = true;
      this._lblMinorLength.Location = new System.Drawing.Point(13, 31);
      this._lblMinorLength.Name = "_lblMinorLength";
      this._lblMinorLength.Size = new System.Drawing.Size(43, 13);
      this._lblMinorLength.TabIndex = 2;
      this._lblMinorLength.Text = "Length:";
      // 
      // _minorWhichTicksLayout
      // 
      this._minorWhichTicksLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._minorWhichTicksLayout.AutoSize = true;
      this._minorWhichTicksLayout.Controls.Add(this._chkWhichMinor1);
      this._minorWhichTicksLayout.Controls.Add(this._chkWhichMinor2);
      this._minorWhichTicksLayout.Location = new System.Drawing.Point(270, 25);
      this._minorWhichTicksLayout.Margin = new System.Windows.Forms.Padding(0);
      this._minorWhichTicksLayout.Name = "_minorWhichTicksLayout";
      this._minorWhichTicksLayout.Size = new System.Drawing.Size(127, 25);
      this._minorWhichTicksLayout.TabIndex = 6;
      // 
      // _chkWhichMinor1
      // 
      this._chkWhichMinor1.AutoSize = true;
      this._chkWhichMinor1.Location = new System.Drawing.Point(3, 3);
      this._chkWhichMinor1.Name = "_chkWhichMinor1";
      this._chkWhichMinor1.Size = new System.Drawing.Size(35, 17);
      this._chkWhichMinor1.TabIndex = 2;
      this._chkWhichMinor1.Text = "In";
      this._chkWhichMinor1.UseVisualStyleBackColor = true;
      // 
      // _chkWhichMinor2
      // 
      this._chkWhichMinor2.AutoSize = true;
      this._chkWhichMinor2.Location = new System.Drawing.Point(44, 3);
      this._chkWhichMinor2.Name = "_chkWhichMinor2";
      this._chkWhichMinor2.Size = new System.Drawing.Size(43, 17);
      this._chkWhichMinor2.TabIndex = 3;
      this._chkWhichMinor2.Text = "Out";
      this._chkWhichMinor2.UseVisualStyleBackColor = true;
      // 
      // _lineBrushColor
      // 
      this._lineBrushColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._lineBrushColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._lineBrushColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._lineBrushColor.FormattingEnabled = true;
      this._lineBrushColor.ItemHeight = 13;
      this._lineBrushColor.Items.AddRange(new object[] {
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
      this._lineBrushColor.Location = new System.Drawing.Point(61, 26);
      this._lineBrushColor.Name = "_lineBrushColor";
      this._lineBrushColor.Size = new System.Drawing.Size(121, 19);
      this._lineBrushColor.TabIndex = 0;
      // 
      // _lineLineThickness
      // 
      this._lineLineThickness.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._lineLineThickness.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._lineLineThickness.FormattingEnabled = true;
      this._lineLineThickness.ItemHeight = 13;
      this._lineLineThickness.Location = new System.Drawing.Point(273, 26);
      this._lineLineThickness.Name = "_lineLineThickness";
      this._lineLineThickness.Size = new System.Drawing.Size(121, 19);
      this._lineLineThickness.TabIndex = 1;
      this._lineLineThickness.PenWidthChoiceChanged += new System.EventHandler(this.EhLineThickness_Changed);
      // 
      // _majorLineColor
      // 
      this._majorLineColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._majorLineColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._majorLineColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._majorLineColor.Enabled = false;
      this._majorLineColor.FormattingEnabled = true;
      this._majorLineColor.ItemHeight = 13;
      this._majorLineColor.Items.AddRange(new object[] {
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
      this._majorLineColor.Location = new System.Drawing.Point(62, 3);
      this._majorLineColor.Name = "_majorLineColor";
      this._majorLineColor.Size = new System.Drawing.Size(121, 19);
      this._majorLineColor.TabIndex = 3;
      // 
      // _lineMajorThickness
      // 
      this._lineMajorThickness.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._lineMajorThickness.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._lineMajorThickness.Enabled = false;
      this._lineMajorThickness.FormattingEnabled = true;
      this._lineMajorThickness.ItemHeight = 13;
      this._lineMajorThickness.Location = new System.Drawing.Point(273, 3);
      this._lineMajorThickness.Name = "_lineMajorThickness";
      this._lineMajorThickness.Size = new System.Drawing.Size(121, 19);
      this._lineMajorThickness.TabIndex = 4;
      // 
      // _lineMajorLength
      // 
      this._lineMajorLength.Anchor = System.Windows.Forms.AnchorStyles.None;
      this._lineMajorLength.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._lineMajorLength.FormattingEnabled = true;
      this._lineMajorLength.ItemHeight = 13;
      this._lineMajorLength.Location = new System.Drawing.Point(62, 28);
      this._lineMajorLength.Name = "_lineMajorLength";
      this._lineMajorLength.Size = new System.Drawing.Size(121, 19);
      this._lineMajorLength.TabIndex = 5;
      // 
      // _minorLineColor
      // 
      this._minorLineColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._minorLineColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._minorLineColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._minorLineColor.Enabled = false;
      this._minorLineColor.FormattingEnabled = true;
      this._minorLineColor.ItemHeight = 13;
      this._minorLineColor.Items.AddRange(new object[] {
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
      this._minorLineColor.Location = new System.Drawing.Point(62, 3);
      this._minorLineColor.Name = "_minorLineColor";
      this._minorLineColor.Size = new System.Drawing.Size(121, 19);
      this._minorLineColor.TabIndex = 3;
      // 
      // _lineMinorThickness
      // 
      this._lineMinorThickness.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._lineMinorThickness.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._lineMinorThickness.Enabled = false;
      this._lineMinorThickness.FormattingEnabled = true;
      this._lineMinorThickness.ItemHeight = 13;
      this._lineMinorThickness.Location = new System.Drawing.Point(273, 3);
      this._lineMinorThickness.Name = "_lineMinorThickness";
      this._lineMinorThickness.Size = new System.Drawing.Size(121, 19);
      this._lineMinorThickness.TabIndex = 4;
      // 
      // _lineMinorLength
      // 
      this._lineMinorLength.Anchor = System.Windows.Forms.AnchorStyles.None;
      this._lineMinorLength.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._lineMinorLength.FormattingEnabled = true;
      this._lineMinorLength.ItemHeight = 13;
      this._lineMinorLength.Location = new System.Drawing.Point(62, 28);
      this._lineMinorLength.Name = "_lineMinorLength";
      this._lineMinorLength.Size = new System.Drawing.Size(121, 19);
      this._lineMinorLength.TabIndex = 5;
      // 
      // _linePenGlue
      // 
      this._linePenGlue.CbBrushColor = this._lineBrushColor;
      this._linePenGlue.CbBrushColor2 = null;
      this._linePenGlue.CbBrushHatchStyle = null;
      this._linePenGlue.CbBrushType = null;
      this._linePenGlue.CbDashCap = null;
      this._linePenGlue.CbDashStyle = null;
      this._linePenGlue.CbEndCap = null;
      this._linePenGlue.CbEndCapSize = null;
      this._linePenGlue.CbLineJoin = null;
      this._linePenGlue.CbLineThickness = this._lineLineThickness;
      this._linePenGlue.CbMiterLimit = null;
      this._linePenGlue.CbStartCap = null;
      this._linePenGlue.CbStartCapSize = null;
      this._linePenGlue.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._linePenGlue.PenChanged += new System.EventHandler(this.EhLinePen_Changed);
      // 
      // _majorPenGlue
      // 
      this._majorPenGlue.CbBrushColor = this._majorLineColor;
      this._majorPenGlue.CbBrushColor2 = null;
      this._majorPenGlue.CbBrushHatchStyle = null;
      this._majorPenGlue.CbBrushType = null;
      this._majorPenGlue.CbDashCap = null;
      this._majorPenGlue.CbDashStyle = null;
      this._majorPenGlue.CbEndCap = null;
      this._majorPenGlue.CbEndCapSize = null;
      this._majorPenGlue.CbLineJoin = null;
      this._majorPenGlue.CbLineThickness = this._lineMajorThickness;
      this._majorPenGlue.CbMiterLimit = null;
      this._majorPenGlue.CbStartCap = null;
      this._majorPenGlue.CbStartCapSize = null;
      this._majorPenGlue.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      // 
      // _minorPenGlue
      // 
      this._minorPenGlue.CbBrushColor = this._minorLineColor;
      this._minorPenGlue.CbBrushColor2 = null;
      this._minorPenGlue.CbBrushHatchStyle = null;
      this._minorPenGlue.CbBrushType = null;
      this._minorPenGlue.CbDashCap = null;
      this._minorPenGlue.CbDashStyle = null;
      this._minorPenGlue.CbEndCap = null;
      this._minorPenGlue.CbEndCapSize = null;
      this._minorPenGlue.CbLineJoin = null;
      this._minorPenGlue.CbLineThickness = this._lineMinorThickness;
      this._minorPenGlue.CbMiterLimit = null;
      this._minorPenGlue.CbStartCap = null;
      this._minorPenGlue.CbStartCapSize = null;
      this._minorPenGlue.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      // 
      // AxisLineStyleControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this._verticalPanel);
      this.Name = "AxisLineStyleControl";
      this.Size = new System.Drawing.Size(409, 223);
      this._verticalPanel.ResumeLayout(false);
      this._verticalPanel.PerformLayout();
      this._lineGroupBox.ResumeLayout(false);
      this._lineGroupBox.PerformLayout();
      this._lineLayoutTable.ResumeLayout(false);
      this._lineLayoutTable.PerformLayout();
      this._majorGroupBox.ResumeLayout(false);
      this._majorGroupBox.PerformLayout();
      this._majorLayoutTable.ResumeLayout(false);
      this._majorLayoutTable.PerformLayout();
      this._majorWhichTicksLayout.ResumeLayout(false);
      this._majorWhichTicksLayout.PerformLayout();
      this._minorGroupBox.ResumeLayout(false);
      this._minorGroupBox.PerformLayout();
      this._minorLayoutTable.ResumeLayout(false);
      this._minorLayoutTable.PerformLayout();
      this._minorWhichTicksLayout.ResumeLayout(false);
      this._minorWhichTicksLayout.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel _verticalPanel;
    private System.Windows.Forms.GroupBox _lineGroupBox;
    private System.Windows.Forms.GroupBox _majorGroupBox;
    private System.Windows.Forms.TableLayoutPanel _lineLayoutTable;
    private System.Windows.Forms.GroupBox _minorGroupBox;
    private Altaxo.Gui.Common.Drawing.PenControlsGlue _linePenGlue;
    private Altaxo.Gui.Common.Drawing.PenControlsGlue _majorPenGlue;
    private Altaxo.Gui.Common.Drawing.PenControlsGlue _minorPenGlue;
    private Altaxo.Gui.Common.Drawing.ColorComboBox _lineBrushColor;
    private Altaxo.Gui.Common.Drawing.LineThicknessComboBox _lineLineThickness;
    private System.Windows.Forms.Label _lblLineColor;
    private System.Windows.Forms.Label _lblLineThickness;
    private System.Windows.Forms.TableLayoutPanel _majorLayoutTable;
    private System.Windows.Forms.CheckBox _chkCustomMajorColor;
    private System.Windows.Forms.CheckBox _chkCustomMajorThickness;
    private System.Windows.Forms.Label _lblMajorLength;
    private Altaxo.Gui.Common.Drawing.ColorComboBox _majorLineColor;
    private Altaxo.Gui.Common.Drawing.LineThicknessComboBox _lineMajorThickness;
    private Altaxo.Gui.Common.Drawing.LineThicknessComboBox _lineMajorLength;
    private System.Windows.Forms.TableLayoutPanel _minorLayoutTable;
    private System.Windows.Forms.CheckBox _chkCustomMinorColor;
    private System.Windows.Forms.CheckBox _chkCustomMinorThickness;
    private System.Windows.Forms.Label _lblMinorLength;
    private Altaxo.Gui.Common.Drawing.ColorComboBox _minorLineColor;
    private Altaxo.Gui.Common.Drawing.LineThicknessComboBox _lineMinorThickness;
    private Altaxo.Gui.Common.Drawing.LineThicknessComboBox _lineMinorLength;
    private System.Windows.Forms.FlowLayoutPanel _majorWhichTicksLayout;
    private System.Windows.Forms.CheckBox _chkWhichMajor1;
    private System.Windows.Forms.CheckBox _chkWhichMajor2;
    private System.Windows.Forms.FlowLayoutPanel _minorWhichTicksLayout;
    private System.Windows.Forms.CheckBox _chkWhichMinor1;
    private System.Windows.Forms.CheckBox _chkWhichMinor2;
    private System.Windows.Forms.CheckBox _chkEnableLine;
  }
}
