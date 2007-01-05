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
  partial class LineGraphicControl
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
      this._edPositionX = new System.Windows.Forms.TextBox();
      this._edPositionY = new System.Windows.Forms.TextBox();
      this._edSizeX = new System.Windows.Forms.TextBox();
      this._edSizeY = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this._lblColor = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this._cbColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._cbRotation = new Altaxo.Gui.Common.Drawing.RotationComboBox();
      this._cbDashStyle = new Altaxo.Gui.Common.Drawing.DashStyleComboBox();
      this._cbLineThickness = new Altaxo.Gui.Common.Drawing.LineThicknessComboBox();
      this._cbStartCap = new Altaxo.Gui.Common.Drawing.LineCapComboBox();
      this._cbEndCap = new Altaxo.Gui.Common.Drawing.LineCapComboBox();
      this._cbStartCapSize = new Altaxo.Gui.Common.Drawing.LineCapSizeComboBox();
      this._cbEndCapSize = new Altaxo.Gui.Common.Drawing.LineCapSizeComboBox();
      this._penGlue = new Altaxo.Gui.Common.Drawing.PenControlsGlue();
      this._positionSizeGlue = new Altaxo.Gui.Graph.ObjectPositionAndSizeGlue();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // _edPositionX
      // 
      this._edPositionX.Location = new System.Drawing.Point(68, 158);
      this._edPositionX.Name = "_edPositionX";
      this._edPositionX.Size = new System.Drawing.Size(100, 20);
      this._edPositionX.TabIndex = 7;
      this._edPositionX.Text = "0 pt";
      // 
      // _edPositionY
      // 
      this._edPositionY.Location = new System.Drawing.Point(68, 184);
      this._edPositionY.Name = "_edPositionY";
      this._edPositionY.Size = new System.Drawing.Size(100, 20);
      this._edPositionY.TabIndex = 8;
      this._edPositionY.Text = "0 pt";
      // 
      // _edSizeX
      // 
      this._edSizeX.Location = new System.Drawing.Point(251, 158);
      this._edSizeX.Name = "_edSizeX";
      this._edSizeX.Size = new System.Drawing.Size(100, 20);
      this._edSizeX.TabIndex = 10;
      this._edSizeX.Text = "0 pt";
      // 
      // _edSizeY
      // 
      this._edSizeY.Location = new System.Drawing.Point(251, 184);
      this._edSizeY.Name = "_edSizeY";
      this._edSizeY.Size = new System.Drawing.Size(100, 20);
      this._edSizeY.TabIndex = 11;
      this._edSizeY.Text = "0 pt";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this._cbColor, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this._edPositionX, 1, 6);
      this.tableLayoutPanel1.Controls.Add(this._edPositionY, 1, 7);
      this.tableLayoutPanel1.Controls.Add(this._edSizeX, 3, 6);
      this.tableLayoutPanel1.Controls.Add(this._edSizeY, 3, 7);
      this.tableLayoutPanel1.Controls.Add(this._cbRotation, 1, 8);
      this.tableLayoutPanel1.Controls.Add(this._cbDashStyle, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this._cbLineThickness, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this._cbStartCap, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this._cbEndCap, 1, 4);
      this.tableLayoutPanel1.Controls.Add(this._cbStartCapSize, 3, 3);
      this.tableLayoutPanel1.Controls.Add(this._cbEndCapSize, 3, 4);
      this.tableLayoutPanel1.Controls.Add(this._lblColor, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this.label5, 0, 6);
      this.tableLayoutPanel1.Controls.Add(this.label6, 0, 7);
      this.tableLayoutPanel1.Controls.Add(this.label7, 2, 6);
      this.tableLayoutPanel1.Controls.Add(this.label8, 2, 7);
      this.tableLayoutPanel1.Controls.Add(this.label9, 0, 8);
      this.tableLayoutPanel1.Controls.Add(this.label10, 2, 3);
      this.tableLayoutPanel1.Controls.Add(this.label11, 2, 4);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 9;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(375, 234);
      this.tableLayoutPanel1.TabIndex = 13;
      // 
      // _lblColor
      // 
      this._lblColor.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblColor.AutoSize = true;
      this._lblColor.Location = new System.Drawing.Point(28, 7);
      this._lblColor.Name = "_lblColor";
      this._lblColor.Size = new System.Drawing.Size(34, 13);
      this._lblColor.TabIndex = 13;
      this._lblColor.Text = "Color:";
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(29, 34);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(33, 13);
      this.label1.TabIndex = 14;
      this.label1.Text = "Style:";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 61);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(59, 13);
      this.label2.TabIndex = 15;
      this.label2.Text = "Thickness:";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(11, 88);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(51, 13);
      this.label3.TabIndex = 16;
      this.label3.Text = "StartCap:";
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(14, 115);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(48, 13);
      this.label4.TabIndex = 17;
      this.label4.Text = "EndCap:";
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(24, 161);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(38, 13);
      this.label5.TabIndex = 18;
      this.label5.Text = "Pos.X:";
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(24, 187);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(38, 13);
      this.label6.TabIndex = 19;
      this.label6.Text = "Pos.Y:";
      // 
      // label7
      // 
      this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(207, 161);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(38, 13);
      this.label7.TabIndex = 20;
      this.label7.Text = "Width:";
      // 
      // label8
      // 
      this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(204, 187);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(41, 13);
      this.label8.TabIndex = 21;
      this.label8.Text = "Height:";
      // 
      // label9
      // 
      this.label9.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(12, 214);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(50, 13);
      this.label9.TabIndex = 22;
      this.label9.Text = "Rotation:";
      // 
      // label10
      // 
      this.label10.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(195, 88);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(50, 13);
      this.label10.TabIndex = 23;
      this.label10.Text = "Min.Size:";
      // 
      // label11
      // 
      this.label11.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(195, 115);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(50, 13);
      this.label11.TabIndex = 24;
      this.label11.Text = "Min.Size:";
      // 
      // _cbColor
      // 
      this._cbColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._cbColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbColor.FormattingEnabled = true;
      this._cbColor.ItemHeight = 15;
  
      this._cbColor.Location = new System.Drawing.Point(68, 3);
      this._cbColor.Name = "_cbColor";
      this._cbColor.Size = new System.Drawing.Size(121, 21);
      this._cbColor.TabIndex = 1;
      // 
      // _cbRotation
      // 
      this._cbRotation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbRotation.FormattingEnabled = true;
      this._cbRotation.ItemHeight = 15;
      this._cbRotation.Items.AddRange(new object[] {
            0F,
            45F,
            90F,
            135F,
            180F,
            225F,
            270F,
            315F});
      this._cbRotation.Location = new System.Drawing.Point(68, 210);
      this._cbRotation.Name = "_cbRotation";
      this._cbRotation.Size = new System.Drawing.Size(121, 21);
      this._cbRotation.TabIndex = 12;
      // 
      // _cbDashStyle
      // 
      this._cbDashStyle.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbDashStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbDashStyle.FormattingEnabled = true;
      this._cbDashStyle.ItemHeight = 15;
      this._cbDashStyle.Location = new System.Drawing.Point(68, 30);
      this._cbDashStyle.Name = "_cbDashStyle";
      this._cbDashStyle.Size = new System.Drawing.Size(121, 21);
      this._cbDashStyle.TabIndex = 0;
      // 
      // _cbLineThickness
      // 
      this._cbLineThickness.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbLineThickness.FormattingEnabled = true;
      this._cbLineThickness.ItemHeight = 15;
     
      this._cbLineThickness.Location = new System.Drawing.Point(68, 57);
      this._cbLineThickness.Name = "_cbLineThickness";
      this._cbLineThickness.Size = new System.Drawing.Size(121, 21);
      this._cbLineThickness.TabIndex = 2;
      // 
      // _cbStartCap
      // 
      this._cbStartCap.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbStartCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbStartCap.FormattingEnabled = true;
      this._cbStartCap.ItemHeight = 15;
      this._cbStartCap.Location = new System.Drawing.Point(68, 84);
      this._cbStartCap.Name = "_cbStartCap";
      this._cbStartCap.Size = new System.Drawing.Size(121, 21);
      this._cbStartCap.TabIndex = 3;
      // 
      // _cbEndCap
      // 
      this._cbEndCap.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbEndCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbEndCap.FormattingEnabled = true;
      this._cbEndCap.ItemHeight = 15;
      this._cbEndCap.Location = new System.Drawing.Point(68, 111);
      this._cbEndCap.Name = "_cbEndCap";
      this._cbEndCap.Size = new System.Drawing.Size(121, 21);
      this._cbEndCap.TabIndex = 4;
      // 
      // _cbStartCapSize
      // 
      this._cbStartCapSize.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbStartCapSize.FormattingEnabled = true;
      this._cbStartCapSize.ItemHeight = 15;
     
      this._cbStartCapSize.Location = new System.Drawing.Point(251, 84);
      this._cbStartCapSize.Name = "_cbStartCapSize";
      this._cbStartCapSize.Size = new System.Drawing.Size(121, 21);
      this._cbStartCapSize.TabIndex = 5;
      this._cbStartCapSize.Thickness = 8F;
      // 
      // _cbEndCapSize
      // 
      this._cbEndCapSize.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbEndCapSize.FormattingEnabled = true;
      this._cbEndCapSize.ItemHeight = 15;
    
      this._cbEndCapSize.Location = new System.Drawing.Point(251, 111);
      this._cbEndCapSize.Name = "_cbEndCapSize";
      this._cbEndCapSize.Size = new System.Drawing.Size(121, 21);
      this._cbEndCapSize.TabIndex = 6;
      this._cbEndCapSize.Thickness = 8F;
      // 
      // _penGlue
      // 
      this._penGlue.CbBrushColor = this._cbColor;
      this._penGlue.CbBrushColor2 = null;
      this._penGlue.CbBrushHatchStyle = null;
      this._penGlue.CbBrushType = null;
      this._penGlue.CbDashCap = null;
      this._penGlue.CbDashStyle = this._cbDashStyle;
      this._penGlue.CbEndCap = this._cbEndCap;
      this._penGlue.CbEndCapSize = this._cbEndCapSize;
      this._penGlue.CbLineJoin = null;
      this._penGlue.CbLineThickness = this._cbLineThickness;
      this._penGlue.CbMiterLimit = null;
      this._penGlue.CbStartCap = this._cbStartCap;
      this._penGlue.CbStartCapSize = this._cbStartCapSize;
      this._penGlue.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      // 
      // _positionSizeGlue
      // 
      this._positionSizeGlue.CbRotation = this._cbRotation;
      this._positionSizeGlue.EdPositionX = this._edPositionX;
      this._positionSizeGlue.EdPositionY = this._edPositionY;
      this._positionSizeGlue.EdSizeX = this._edSizeX;
      this._positionSizeGlue.EdSizeY = this._edSizeY;
      // 
      // LineGraphicControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "LineGraphicControl";
      this.Size = new System.Drawing.Size(378, 237);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Altaxo.Gui.Common.Drawing.PenControlsGlue _penGlue;
    private Altaxo.Gui.Common.Drawing.DashStyleComboBox _cbDashStyle;
    private Altaxo.Gui.Common.Drawing.ColorComboBox _cbColor;
    private Altaxo.Gui.Common.Drawing.LineThicknessComboBox _cbLineThickness;
    private Altaxo.Gui.Common.Drawing.LineCapComboBox _cbStartCap;
    private Altaxo.Gui.Common.Drawing.LineCapComboBox _cbEndCap;
    private Altaxo.Gui.Common.Drawing.LineCapSizeComboBox _cbStartCapSize;
    private Altaxo.Gui.Common.Drawing.LineCapSizeComboBox _cbEndCapSize;
    private System.Windows.Forms.TextBox _edPositionX;
    private System.Windows.Forms.TextBox _edPositionY;
    private System.Windows.Forms.TextBox _edSizeX;
    private ObjectPositionAndSizeGlue _positionSizeGlue;
    private System.Windows.Forms.TextBox _edSizeY;
    private Altaxo.Gui.Common.Drawing.RotationComboBox _cbRotation;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label _lblColor;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label label11;
  }
}
