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
  partial class PositionSizeRotationScaleControl
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
      this._tableLayout = new System.Windows.Forms.TableLayoutPanel();
      this._cbRotation = new Altaxo.Gui.Common.Drawing.RotationComboBox();
      this._lblRotation = new System.Windows.Forms.Label();
      this._lblPosX = new System.Windows.Forms.Label();
      this._lblPosY = new System.Windows.Forms.Label();
      this._lblWidth = new System.Windows.Forms.Label();
      this._lblHeight = new System.Windows.Forms.Label();
      this._edPosX = new System.Windows.Forms.TextBox();
      this._edPosY = new System.Windows.Forms.TextBox();
      this._edWidth = new System.Windows.Forms.TextBox();
      this._edHeight = new System.Windows.Forms.TextBox();
      this._positionSizeGlue = new Altaxo.Gui.Graph.ObjectPositionAndSizeGlue();
      this._tableLayout.SuspendLayout();
      this.SuspendLayout();
      // 
      // _tableLayout
      // 
      this._tableLayout.AutoSize = true;
      this._tableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._tableLayout.ColumnCount = 4;
      this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._tableLayout.Controls.Add(this._cbRotation, 1, 2);
      this._tableLayout.Controls.Add(this._lblRotation, 0, 2);
      this._tableLayout.Controls.Add(this._lblPosX, 0, 0);
      this._tableLayout.Controls.Add(this._lblPosY, 0, 1);
      this._tableLayout.Controls.Add(this._lblWidth, 2, 0);
      this._tableLayout.Controls.Add(this._lblHeight, 2, 1);
      this._tableLayout.Controls.Add(this._edPosY, 1, 1);
      this._tableLayout.Controls.Add(this._edWidth, 3, 0);
      this._tableLayout.Controls.Add(this._edHeight, 3, 1);
      this._tableLayout.Controls.Add(this._edPosX, 1, 0);
      this._tableLayout.Location = new System.Drawing.Point(0, 3);
      this._tableLayout.Name = "_tableLayout";
      this._tableLayout.RowCount = 3;
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.Size = new System.Drawing.Size(335, 79);
      this._tableLayout.TabIndex = 0;
      // 
      // _cbRotation
      // 
      this._cbRotation.Anchor = System.Windows.Forms.AnchorStyles.Left;
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
      this._cbRotation.Location = new System.Drawing.Point(59, 55);
      this._cbRotation.Name = "_cbRotation";
      this._cbRotation.Size = new System.Drawing.Size(120, 21);
      this._cbRotation.TabIndex = 0;
      // 
      // _lblRotation
      // 
      this._lblRotation.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblRotation.AutoSize = true;
      this._lblRotation.Location = new System.Drawing.Point(3, 59);
      this._lblRotation.Name = "_lblRotation";
      this._lblRotation.Size = new System.Drawing.Size(50, 13);
      this._lblRotation.TabIndex = 1;
      this._lblRotation.Text = "Rotation:";
      // 
      // _lblPosX
      // 
      this._lblPosX.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblPosX.AutoSize = true;
      this._lblPosX.Location = new System.Drawing.Point(15, 6);
      this._lblPosX.Name = "_lblPosX";
      this._lblPosX.Size = new System.Drawing.Size(38, 13);
      this._lblPosX.TabIndex = 2;
      this._lblPosX.Text = "Pos.X:";
      // 
      // _lblPosY
      // 
      this._lblPosY.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblPosY.AutoSize = true;
      this._lblPosY.Location = new System.Drawing.Point(15, 32);
      this._lblPosY.Name = "_lblPosY";
      this._lblPosY.Size = new System.Drawing.Size(38, 13);
      this._lblPosY.TabIndex = 3;
      this._lblPosY.Text = "Pos.Y:";
      // 
      // _lblWidth
      // 
      this._lblWidth.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblWidth.AutoSize = true;
      this._lblWidth.Location = new System.Drawing.Point(188, 6);
      this._lblWidth.Name = "_lblWidth";
      this._lblWidth.Size = new System.Drawing.Size(38, 13);
      this._lblWidth.TabIndex = 4;
      this._lblWidth.Text = "Width:";
      // 
      // _lblHeight
      // 
      this._lblHeight.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblHeight.AutoSize = true;
      this._lblHeight.Location = new System.Drawing.Point(185, 32);
      this._lblHeight.Name = "_lblHeight";
      this._lblHeight.Size = new System.Drawing.Size(41, 13);
      this._lblHeight.TabIndex = 5;
      this._lblHeight.Text = "Height:";
      // 
      // _edPosX
      // 
      this._edPosX.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._edPosX.Location = new System.Drawing.Point(59, 3);
      this._edPosX.Name = "_edPosX";
      this._edPosX.Size = new System.Drawing.Size(100, 20);
      this._edPosX.TabIndex = 7;
      this._edPosX.Text = "0 pt";
      // 
      // _edPosY
      // 
      this._edPosY.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._edPosY.Location = new System.Drawing.Point(59, 29);
      this._edPosY.Name = "_edPosY";
      this._edPosY.Size = new System.Drawing.Size(100, 20);
      this._edPosY.TabIndex = 8;
      this._edPosY.Text = "0 pt";
      // 
      // _edWidth
      // 
      this._edWidth.Location = new System.Drawing.Point(232, 3);
      this._edWidth.Name = "_edWidth";
      this._edWidth.Size = new System.Drawing.Size(100, 20);
      this._edWidth.TabIndex = 9;
      this._edWidth.Text = "0 pt";
      // 
      // _edHeight
      // 
      this._edHeight.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._edHeight.Location = new System.Drawing.Point(232, 29);
      this._edHeight.Name = "_edHeight";
      this._edHeight.Size = new System.Drawing.Size(100, 20);
      this._edHeight.TabIndex = 10;
      this._edHeight.Text = "0 pt";
      // 
      // _positionSizeGlue
      // 
      this._positionSizeGlue.CbRotation = this._cbRotation;
      this._positionSizeGlue.EdPositionX = this._edPosX;
      this._positionSizeGlue.EdPositionY = this._edPosY;
      this._positionSizeGlue.EdSizeX = this._edWidth;
      this._positionSizeGlue.EdSizeY = this._edHeight;
      // 
      // PositionSizeRotationScaleControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this._tableLayout);
      this.Name = "PositionSizeRotationScaleControl";
      this.Size = new System.Drawing.Size(338, 85);
      this._tableLayout.ResumeLayout(false);
      this._tableLayout.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel _tableLayout;
    private Altaxo.Gui.Common.Drawing.RotationComboBox _cbRotation;
    private System.Windows.Forms.Label _lblRotation;
    private System.Windows.Forms.Label _lblPosX;
    private System.Windows.Forms.Label _lblPosY;
    private System.Windows.Forms.Label _lblWidth;
    private System.Windows.Forms.Label _lblHeight;
    private System.Windows.Forms.TextBox _edPosX;
    private System.Windows.Forms.TextBox _edPosY;
    private System.Windows.Forms.TextBox _edWidth;
    private System.Windows.Forms.TextBox _edHeight;
    private ObjectPositionAndSizeGlue _positionSizeGlue;
  }
}
