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
  partial class ErrorBarPlotStyleControl
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
      this._chkIndependentColor = new System.Windows.Forms.CheckBox();
      this._cbSymbolSize = new System.Windows.Forms.ComboBox();
      this._chkIndependentSize = new System.Windows.Forms.CheckBox();
      this._chkLineSymbolGap = new System.Windows.Forms.CheckBox();
      this._lblPosErrorColumn = new System.Windows.Forms.Label();
      this._btSelectErrorColumn = new System.Windows.Forms.Button();
      this._edErrorColumn = new System.Windows.Forms.TextBox();
      this._btSelectNegErrorColumn = new System.Windows.Forms.Button();
      this._edNegErrorColumn = new System.Windows.Forms.TextBox();
      this._chkIndepNegErrorColumn = new System.Windows.Forms.CheckBox();
      this._chkShowEndBars = new System.Windows.Forms.CheckBox();
      this._cbDashStyle = new Altaxo.Gui.Common.Drawing.DashStyleComboBox();
      this._cbThickness = new Altaxo.Gui.Common.Drawing.LineThicknessComboBox();
      this._cbPenColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._strokePenGlue = new Altaxo.Gui.Common.Drawing.PenControlsGlue();
      this._chkDoNotShift = new System.Windows.Forms.CheckBox();
      this._chkIsHorizontal = new System.Windows.Forms.CheckBox();
      this._dataGroupBox = new System.Windows.Forms.GroupBox();
      this._btClearNegError = new System.Windows.Forms.Button();
      this._btClearPosError = new System.Windows.Forms.Button();
      this._edSkipFrequency = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this._styleGroupBox = new System.Windows.Forms.GroupBox();
      this._dataGroupBox.SuspendLayout();
      this._styleGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // _chkIndependentColor
      // 
      this._chkIndependentColor.AutoSize = true;
      this._chkIndependentColor.Location = new System.Drawing.Point(6, 15);
      this._chkIndependentColor.Name = "_chkIndependentColor";
      this._chkIndependentColor.Size = new System.Drawing.Size(73, 17);
      this._chkIndependentColor.TabIndex = 14;
      this._chkIndependentColor.Text = "Ind. color:";
      this._chkIndependentColor.UseVisualStyleBackColor = true;
      // 
      // _cbSymbolSize
      // 
      this._cbSymbolSize.ItemHeight = 13;
      this._cbSymbolSize.Location = new System.Drawing.Point(125, 126);
      this._cbSymbolSize.Name = "_cbSymbolSize";
      this._cbSymbolSize.Size = new System.Drawing.Size(121, 21);
      this._cbSymbolSize.TabIndex = 15;
      this._cbSymbolSize.Text = "comboBox1";
      this._cbSymbolSize.Validating += new System.ComponentModel.CancelEventHandler(this._cbSymbolSize_Validating);
      // 
      // _chkIndependentSize
      // 
      this._chkIndependentSize.Location = new System.Drawing.Point(6, 126);
      this._chkIndependentSize.Name = "_chkIndependentSize";
      this._chkIndependentSize.Size = new System.Drawing.Size(80, 24);
      this._chkIndependentSize.TabIndex = 28;
      this._chkIndependentSize.Text = "indep. Size";
      // 
      // _chkLineSymbolGap
      // 
      this._chkLineSymbolGap.Location = new System.Drawing.Point(5, 156);
      this._chkLineSymbolGap.Name = "_chkLineSymbolGap";
      this._chkLineSymbolGap.Size = new System.Drawing.Size(112, 24);
      this._chkLineSymbolGap.TabIndex = 29;
      this._chkLineSymbolGap.Text = "Line/Symbol Gap";
      // 
      // _lblPosErrorColumn
      // 
      this._lblPosErrorColumn.Location = new System.Drawing.Point(5, 19);
      this._lblPosErrorColumn.Name = "_lblPosErrorColumn";
      this._lblPosErrorColumn.Size = new System.Drawing.Size(112, 20);
      this._lblPosErrorColumn.TabIndex = 41;
      this._lblPosErrorColumn.Text = "Error column:";
      this._lblPosErrorColumn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // _btSelectErrorColumn
      // 
      this._btSelectErrorColumn.Location = new System.Drawing.Point(373, 19);
      this._btSelectErrorColumn.Name = "_btSelectErrorColumn";
      this._btSelectErrorColumn.Size = new System.Drawing.Size(56, 20);
      this._btSelectErrorColumn.TabIndex = 40;
      this._btSelectErrorColumn.Text = "Select ..";
      this._btSelectErrorColumn.Click += new System.EventHandler(this._btSelectErrorColumn_Click);
      // 
      // _edErrorColumn
      // 
      this._edErrorColumn.Location = new System.Drawing.Point(125, 19);
      this._edErrorColumn.Name = "_edErrorColumn";
      this._edErrorColumn.ReadOnly = true;
      this._edErrorColumn.Size = new System.Drawing.Size(240, 20);
      this._edErrorColumn.TabIndex = 39;
      this._edErrorColumn.Text = "textBox1";
      // 
      // _btSelectNegErrorColumn
      // 
      this._btSelectNegErrorColumn.Location = new System.Drawing.Point(373, 45);
      this._btSelectNegErrorColumn.Name = "_btSelectNegErrorColumn";
      this._btSelectNegErrorColumn.Size = new System.Drawing.Size(56, 20);
      this._btSelectNegErrorColumn.TabIndex = 43;
      this._btSelectNegErrorColumn.Text = "Select ..";
      this._btSelectNegErrorColumn.Click += new System.EventHandler(this._btSelectNegErrorColumn_Click);
      // 
      // _edNegErrorColumn
      // 
      this._edNegErrorColumn.Location = new System.Drawing.Point(125, 45);
      this._edNegErrorColumn.Name = "_edNegErrorColumn";
      this._edNegErrorColumn.ReadOnly = true;
      this._edNegErrorColumn.Size = new System.Drawing.Size(240, 20);
      this._edNegErrorColumn.TabIndex = 42;
      this._edNegErrorColumn.Text = "textBox1";
      // 
      // _chkIndepNegErrorColumn
      // 
      this._chkIndepNegErrorColumn.AutoSize = true;
      this._chkIndepNegErrorColumn.Location = new System.Drawing.Point(13, 48);
      this._chkIndepNegErrorColumn.Name = "_chkIndepNegErrorColumn";
      this._chkIndepNegErrorColumn.Size = new System.Drawing.Size(107, 17);
      this._chkIndepNegErrorColumn.TabIndex = 45;
      this._chkIndepNegErrorColumn.Text = "Indep. neg. error:";
      this._chkIndepNegErrorColumn.UseVisualStyleBackColor = true;
      this._chkIndepNegErrorColumn.CheckedChanged += new System.EventHandler(this._chkIndepNegErrorColumn_CheckedChanged);
      // 
      // _chkShowEndBars
      // 
      this._chkShowEndBars.AutoSize = true;
      this._chkShowEndBars.Location = new System.Drawing.Point(125, 160);
      this._chkShowEndBars.Name = "_chkShowEndBars";
      this._chkShowEndBars.Size = new System.Drawing.Size(97, 17);
      this._chkShowEndBars.TabIndex = 46;
      this._chkShowEndBars.Text = "Show end bars";
      this._chkShowEndBars.UseVisualStyleBackColor = true;
      // 
      // _cbDashStyle
      // 
      this._cbDashStyle.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbDashStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbDashStyle.FormattingEnabled = true;
      this._cbDashStyle.ItemHeight = 15;
      this._cbDashStyle.Location = new System.Drawing.Point(125, 69);
      this._cbDashStyle.Name = "_cbDashStyle";
      this._cbDashStyle.Size = new System.Drawing.Size(121, 21);
      this._cbDashStyle.TabIndex = 32;
      // 
      // _cbThickness
      // 
      this._cbThickness.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbThickness.FormattingEnabled = true;
      this._cbThickness.ItemHeight = 15;
      this._cbThickness.Location = new System.Drawing.Point(125, 42);
      this._cbThickness.Name = "_cbThickness";
      this._cbThickness.Size = new System.Drawing.Size(121, 21);
      this._cbThickness.TabIndex = 31;
      // 
      // _cbPenColor
      // 
      this._cbPenColor.ColorType = Altaxo.Graph.ColorType.PlotColor;
      this._cbPenColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbPenColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbPenColor.FormattingEnabled = true;
      this._cbPenColor.ItemHeight = 15;
      this._cbPenColor.Items.AddRange(new object[] {
            System.Drawing.Color.Black});
      this._cbPenColor.Location = new System.Drawing.Point(125, 15);
      this._cbPenColor.Name = "_cbPenColor";
      this._cbPenColor.Size = new System.Drawing.Size(121, 21);
      this._cbPenColor.TabIndex = 30;
      // 
      // _strokePenGlue
      // 
      this._strokePenGlue.CbBrushColor = this._cbPenColor;
      this._strokePenGlue.CbBrushColor2 = null;
      this._strokePenGlue.CbBrushHatchStyle = null;
      this._strokePenGlue.CbBrushType = null;
      this._strokePenGlue.CbDashCap = null;
      this._strokePenGlue.CbDashStyle = this._cbDashStyle;
      this._strokePenGlue.CbEndCap = null;
      this._strokePenGlue.CbEndCapSize = null;
      this._strokePenGlue.CbLineJoin = null;
      this._strokePenGlue.CbLineThickness = this._cbThickness;
      this._strokePenGlue.CbMiterLimit = null;
      this._strokePenGlue.CbStartCap = null;
      this._strokePenGlue.CbStartCapSize = null;
      this._strokePenGlue.ColorType = Altaxo.Graph.ColorType.PlotColor;
      // 
      // _chkDoNotShift
      // 
      this._chkDoNotShift.Location = new System.Drawing.Point(228, 156);
      this._chkDoNotShift.Name = "_chkDoNotShift";
      this._chkDoNotShift.Size = new System.Drawing.Size(137, 24);
      this._chkDoNotShift.TabIndex = 47;
      this._chkDoNotShift.Text = "Not shift indep. var.";
      // 
      // _chkIsHorizontal
      // 
      this._chkIsHorizontal.Location = new System.Drawing.Point(6, 96);
      this._chkIsHorizontal.Name = "_chkIsHorizontal";
      this._chkIsHorizontal.Size = new System.Drawing.Size(137, 24);
      this._chkIsHorizontal.TabIndex = 48;
      this._chkIsHorizontal.Text = "Horizontal error bars";
      // 
      // _dataGroupBox
      // 
      this._dataGroupBox.Controls.Add(this._btClearNegError);
      this._dataGroupBox.Controls.Add(this._btClearPosError);
      this._dataGroupBox.Controls.Add(this._edErrorColumn);
      this._dataGroupBox.Controls.Add(this._btSelectErrorColumn);
      this._dataGroupBox.Controls.Add(this._lblPosErrorColumn);
      this._dataGroupBox.Controls.Add(this._edNegErrorColumn);
      this._dataGroupBox.Controls.Add(this._chkIndepNegErrorColumn);
      this._dataGroupBox.Controls.Add(this._btSelectNegErrorColumn);
      this._dataGroupBox.Location = new System.Drawing.Point(3, 3);
      this._dataGroupBox.Name = "_dataGroupBox";
      this._dataGroupBox.Size = new System.Drawing.Size(490, 89);
      this._dataGroupBox.TabIndex = 49;
      this._dataGroupBox.TabStop = false;
      this._dataGroupBox.Text = "Data";
      // 
      // _btClearNegError
      // 
      this._btClearNegError.Location = new System.Drawing.Point(435, 45);
      this._btClearNegError.Name = "_btClearNegError";
      this._btClearNegError.Size = new System.Drawing.Size(49, 20);
      this._btClearNegError.TabIndex = 55;
      this._btClearNegError.Text = "Clear!";
      this._btClearNegError.UseVisualStyleBackColor = true;
      this._btClearNegError.Click += new System.EventHandler(this._btClearNegError_Click);
      // 
      // _btClearPosError
      // 
      this._btClearPosError.Location = new System.Drawing.Point(435, 19);
      this._btClearPosError.Name = "_btClearPosError";
      this._btClearPosError.Size = new System.Drawing.Size(49, 20);
      this._btClearPosError.TabIndex = 54;
      this._btClearPosError.Text = "Clear!";
      this._btClearPosError.UseVisualStyleBackColor = true;
      this._btClearPosError.Click += new System.EventHandler(this._btClearPosError_Click);
      // 
      // _edSkipFrequency
      // 
      this._edSkipFrequency.Location = new System.Drawing.Point(125, 183);
      this._edSkipFrequency.Name = "_edSkipFrequency";
      this._edSkipFrequency.Size = new System.Drawing.Size(54, 20);
      this._edSkipFrequency.TabIndex = 51;
      this._edSkipFrequency.Validating += new System.ComponentModel.CancelEventHandler(this._edSkipFrequency_Validating);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 190);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(84, 13);
      this.label1.TabIndex = 52;
      this.label1.Text = "Skip frequency.:";
      // 
      // _styleGroupBox
      // 
      this._styleGroupBox.Controls.Add(this._chkIsHorizontal);
      this._styleGroupBox.Controls.Add(this.label1);
      this._styleGroupBox.Controls.Add(this._chkIndependentColor);
      this._styleGroupBox.Controls.Add(this._edSkipFrequency);
      this._styleGroupBox.Controls.Add(this._cbSymbolSize);
      this._styleGroupBox.Controls.Add(this._chkIndependentSize);
      this._styleGroupBox.Controls.Add(this._chkLineSymbolGap);
      this._styleGroupBox.Controls.Add(this._chkDoNotShift);
      this._styleGroupBox.Controls.Add(this._cbPenColor);
      this._styleGroupBox.Controls.Add(this._chkShowEndBars);
      this._styleGroupBox.Controls.Add(this._cbThickness);
      this._styleGroupBox.Controls.Add(this._cbDashStyle);
      this._styleGroupBox.Location = new System.Drawing.Point(3, 98);
      this._styleGroupBox.Name = "_styleGroupBox";
      this._styleGroupBox.Size = new System.Drawing.Size(490, 217);
      this._styleGroupBox.TabIndex = 53;
      this._styleGroupBox.TabStop = false;
      this._styleGroupBox.Text = "Style";
      // 
      // ErrorBarPlotStyleControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._styleGroupBox);
      this.Controls.Add(this._dataGroupBox);
      this.Name = "ErrorBarPlotStyleControl";
      this.Size = new System.Drawing.Size(497, 323);
      this._dataGroupBox.ResumeLayout(false);
      this._dataGroupBox.PerformLayout();
      this._styleGroupBox.ResumeLayout(false);
      this._styleGroupBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.CheckBox _chkIndependentColor;
    private System.Windows.Forms.ComboBox _cbSymbolSize;
    private System.Windows.Forms.CheckBox _chkIndependentSize;
    private System.Windows.Forms.CheckBox _chkLineSymbolGap;
    private Altaxo.Gui.Common.Drawing.PenControlsGlue _strokePenGlue;
    private Altaxo.Gui.Common.Drawing.ColorComboBox _cbPenColor;
    private Altaxo.Gui.Common.Drawing.LineThicknessComboBox _cbThickness;
    private Altaxo.Gui.Common.Drawing.DashStyleComboBox _cbDashStyle;
    private System.Windows.Forms.Label _lblPosErrorColumn;
    private System.Windows.Forms.Button _btSelectErrorColumn;
    private System.Windows.Forms.TextBox _edErrorColumn;
    private System.Windows.Forms.Button _btSelectNegErrorColumn;
    private System.Windows.Forms.TextBox _edNegErrorColumn;
    private System.Windows.Forms.CheckBox _chkIndepNegErrorColumn;
    private System.Windows.Forms.CheckBox _chkShowEndBars;
    private System.Windows.Forms.CheckBox _chkDoNotShift;
    private System.Windows.Forms.CheckBox _chkIsHorizontal;
    private System.Windows.Forms.GroupBox _dataGroupBox;
    private System.Windows.Forms.TextBox _edSkipFrequency;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox _styleGroupBox;
    private System.Windows.Forms.Button _btClearNegError;
    private System.Windows.Forms.Button _btClearPosError;
  }
}
