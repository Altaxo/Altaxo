namespace Altaxo.Gui.Graph
{
  partial class BarGraphPlotStyleControl
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
      this._lblInnerGap = new System.Windows.Forms.Label();
      this._lblOuterGap = new System.Windows.Forms.Label();
      this._chkFrameBar = new System.Windows.Forms.CheckBox();
      this._lblBaseValue = new System.Windows.Forms.Label();
      this._edInnerGap = new System.Windows.Forms.TextBox();
      this._edOuterGap = new System.Windows.Forms.TextBox();
      this._edBaseValue = new System.Windows.Forms.TextBox();
      this._edYGap = new System.Windows.Forms.TextBox();
      this._chkUsePreviousItem = new System.Windows.Forms.CheckBox();
      this._lblYGap = new System.Windows.Forms.Label();
      this._lblFillColor = new System.Windows.Forms.Label();
      this._cbPenColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._cbFillColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._fillBrushGlue = new Altaxo.Gui.Common.Drawing.BrushControlsGlue();
      this._framePenGlue = new Altaxo.Gui.Common.Drawing.PenControlsGlue();
      this._chkIndependentColor = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // _lblInnerGap
      // 
      this._lblInnerGap.AutoSize = true;
      this._lblInnerGap.Location = new System.Drawing.Point(137, 77);
      this._lblInnerGap.Name = "_lblInnerGap";
      this._lblInnerGap.Size = new System.Drawing.Size(75, 13);
      this._lblInnerGap.TabIndex = 2;
      this._lblInnerGap.Text = "Inner gap (%) :";
      // 
      // _lblOuterGap
      // 
      this._lblOuterGap.AutoSize = true;
      this._lblOuterGap.Location = new System.Drawing.Point(137, 103);
      this._lblOuterGap.Name = "_lblOuterGap";
      this._lblOuterGap.Size = new System.Drawing.Size(77, 13);
      this._lblOuterGap.TabIndex = 3;
      this._lblOuterGap.Text = "Outer gap (%) :";
      // 
      // _chkFrameBar
      // 
      this._chkFrameBar.AutoSize = true;
      this._chkFrameBar.Location = new System.Drawing.Point(66, 28);
      this._chkFrameBar.Name = "_chkFrameBar";
      this._chkFrameBar.Size = new System.Drawing.Size(87, 17);
      this._chkFrameBar.TabIndex = 4;
      this._chkFrameBar.Text = "Frame color :";
      this._chkFrameBar.UseVisualStyleBackColor = true;
      this._chkFrameBar.CheckedChanged += new System.EventHandler(this._chkFrameBar_CheckedChanged);
      // 
      // _lblBaseValue
      // 
      this._lblBaseValue.AutoSize = true;
      this._lblBaseValue.Location = new System.Drawing.Point(131, 129);
      this._lblBaseValue.Name = "_lblBaseValue";
      this._lblBaseValue.Size = new System.Drawing.Size(83, 13);
      this._lblBaseValue.TabIndex = 5;
      this._lblBaseValue.Text = "Base value (%) :";
      // 
      // _edInnerGap
      // 
      this._edInnerGap.Location = new System.Drawing.Point(220, 74);
      this._edInnerGap.Name = "_edInnerGap";
      this._edInnerGap.Size = new System.Drawing.Size(60, 20);
      this._edInnerGap.TabIndex = 6;
      // 
      // _edOuterGap
      // 
      this._edOuterGap.Location = new System.Drawing.Point(220, 100);
      this._edOuterGap.Name = "_edOuterGap";
      this._edOuterGap.Size = new System.Drawing.Size(60, 20);
      this._edOuterGap.TabIndex = 7;
      // 
      // _edBaseValue
      // 
      this._edBaseValue.Location = new System.Drawing.Point(220, 126);
      this._edBaseValue.Name = "_edBaseValue";
      this._edBaseValue.Size = new System.Drawing.Size(60, 20);
      this._edBaseValue.TabIndex = 8;
      // 
      // _edYGap
      // 
      this._edYGap.Location = new System.Drawing.Point(220, 152);
      this._edYGap.Name = "_edYGap";
      this._edYGap.Size = new System.Drawing.Size(60, 20);
      this._edYGap.TabIndex = 9;
      // 
      // _chkUsePreviousItem
      // 
      this._chkUsePreviousItem.AutoSize = true;
      this._chkUsePreviousItem.Location = new System.Drawing.Point(28, 154);
      this._chkUsePreviousItem.Name = "_chkUsePreviousItem";
      this._chkUsePreviousItem.Size = new System.Drawing.Size(109, 17);
      this._chkUsePreviousItem.TabIndex = 10;
      this._chkUsePreviousItem.Text = "Start at prev. item";
      this._chkUsePreviousItem.UseVisualStyleBackColor = true;
      this._chkUsePreviousItem.CheckedChanged += new System.EventHandler(this._chkUsePreviousItem_CheckedChanged);
      // 
      // _lblYGap
      // 
      this._lblYGap.AutoSize = true;
      this._lblYGap.Location = new System.Drawing.Point(158, 155);
      this._lblYGap.Name = "_lblYGap";
      this._lblYGap.Size = new System.Drawing.Size(58, 13);
      this._lblYGap.TabIndex = 11;
      this._lblYGap.Text = "Y-gap (%) :";
      // 
      // _lblFillColor
      // 
      this._lblFillColor.AutoSize = true;
      this._lblFillColor.Location = new System.Drawing.Point(102, 6);
      this._lblFillColor.Name = "_lblFillColor";
      this._lblFillColor.Size = new System.Drawing.Size(51, 13);
      this._lblFillColor.TabIndex = 12;
      this._lblFillColor.Text = "Fill color :";
      // 
      // _cbPenColor
      // 
      this._cbPenColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._cbPenColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbPenColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbPenColor.FormattingEnabled = true;
      this._cbPenColor.ItemHeight = 13;
      this._cbPenColor.Location = new System.Drawing.Point(159, 28);
      this._cbPenColor.Name = "_cbPenColor";
      this._cbPenColor.Size = new System.Drawing.Size(121, 19);
      this._cbPenColor.TabIndex = 1;
      // 
      // _cbFillColor
      // 
      this._cbFillColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._cbFillColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbFillColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbFillColor.FormattingEnabled = true;
      this._cbFillColor.ItemHeight = 13;
      this._cbFillColor.Location = new System.Drawing.Point(159, 3);
      this._cbFillColor.Name = "_cbFillColor";
      this._cbFillColor.Size = new System.Drawing.Size(121, 19);
      this._cbFillColor.TabIndex = 0;
      // 
      // _fillBrushGlue
      // 
      this._fillBrushGlue.CbBrushType = null;
      this._fillBrushGlue.CbColor1 = this._cbFillColor;
      this._fillBrushGlue.CbColor2 = null;
      this._fillBrushGlue.CbGradientFocus = null;
      this._fillBrushGlue.CbGradientMode = null;
      this._fillBrushGlue.CbGradientScale = null;
      this._fillBrushGlue.CbGradientShape = null;
      this._fillBrushGlue.CbHatchStyle = null;
      this._fillBrushGlue.CbTextureImage = null;
      this._fillBrushGlue.CbTextureScale = null;
      this._fillBrushGlue.CbWrapMode = null;
      this._fillBrushGlue.ChkExchangeColors = null;
      this._fillBrushGlue.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._fillBrushGlue.LabelColor2 = null;
      this._fillBrushGlue.LabelExchangeColors = null;
      this._fillBrushGlue.LabelGradientFocus = null;
      this._fillBrushGlue.LabelGradientMode = null;
      this._fillBrushGlue.LabelGradientScale = null;
      this._fillBrushGlue.LabelGradientShape = null;
      this._fillBrushGlue.LabelHatchStyle = null;
      this._fillBrushGlue.LabelTextureImage = null;
      this._fillBrushGlue.LabelTextureScale = null;
      this._fillBrushGlue.LabelWrapMode = null;
      // 
      // _framePenGlue
      // 
      this._framePenGlue.CbBrushColor = this._cbPenColor;
      this._framePenGlue.CbBrushColor2 = null;
      this._framePenGlue.CbBrushHatchStyle = null;
      this._framePenGlue.CbBrushType = null;
      this._framePenGlue.CbDashCap = null;
      this._framePenGlue.CbDashStyle = null;
      this._framePenGlue.CbEndCap = null;
      this._framePenGlue.CbEndCapSize = null;
      this._framePenGlue.CbLineJoin = null;
      this._framePenGlue.CbLineThickness = null;
      this._framePenGlue.CbMiterLimit = null;
      this._framePenGlue.CbStartCap = null;
      this._framePenGlue.CbStartCapSize = null;
      this._framePenGlue.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      // 
      // _chkIndependentColor
      // 
      this._chkIndependentColor.AutoSize = true;
      this._chkIndependentColor.Location = new System.Drawing.Point(3, 6);
      this._chkIndependentColor.Name = "_chkIndependentColor";
      this._chkIndependentColor.Size = new System.Drawing.Size(82, 17);
      this._chkIndependentColor.TabIndex = 13;
      this._chkIndependentColor.Text = "Indep. color";
      this._chkIndependentColor.UseVisualStyleBackColor = true;
      // 
      // BarGraphPlotStyleControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._chkIndependentColor);
      this.Controls.Add(this._lblFillColor);
      this.Controls.Add(this._lblYGap);
      this.Controls.Add(this._chkUsePreviousItem);
      this.Controls.Add(this._edYGap);
      this.Controls.Add(this._edBaseValue);
      this.Controls.Add(this._edOuterGap);
      this.Controls.Add(this._edInnerGap);
      this.Controls.Add(this._lblBaseValue);
      this.Controls.Add(this._chkFrameBar);
      this.Controls.Add(this._lblOuterGap);
      this.Controls.Add(this._lblInnerGap);
      this.Controls.Add(this._cbPenColor);
      this.Controls.Add(this._cbFillColor);
      this.Name = "BarGraphPlotStyleControl";
      this.Size = new System.Drawing.Size(283, 179);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Altaxo.Gui.Common.Drawing.BrushControlsGlue _fillBrushGlue;
    private Altaxo.Gui.Common.Drawing.ColorComboBox _cbFillColor;
    private Altaxo.Gui.Common.Drawing.PenControlsGlue _framePenGlue;
    private Altaxo.Gui.Common.Drawing.ColorComboBox _cbPenColor;
    private System.Windows.Forms.Label _lblInnerGap;
    private System.Windows.Forms.Label _lblOuterGap;
    private System.Windows.Forms.CheckBox _chkFrameBar;
    private System.Windows.Forms.Label _lblBaseValue;
    private System.Windows.Forms.TextBox _edInnerGap;
    private System.Windows.Forms.TextBox _edOuterGap;
    private System.Windows.Forms.TextBox _edBaseValue;
    private System.Windows.Forms.TextBox _edYGap;
    private System.Windows.Forms.CheckBox _chkUsePreviousItem;
    private System.Windows.Forms.Label _lblYGap;
    private System.Windows.Forms.Label _lblFillColor;
    private System.Windows.Forms.CheckBox _chkIndependentColor;
  }
}
