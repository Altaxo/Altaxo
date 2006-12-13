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
      this.SuspendLayout();
      // 
      // _chkIndependentColor
      // 
      this._chkIndependentColor.AutoSize = true;
      this._chkIndependentColor.Location = new System.Drawing.Point(3, 3);
      this._chkIndependentColor.Name = "_chkIndependentColor";
      this._chkIndependentColor.Size = new System.Drawing.Size(82, 17);
      this._chkIndependentColor.TabIndex = 14;
      this._chkIndependentColor.Text = "Indep. color";
      this._chkIndependentColor.UseVisualStyleBackColor = true;
      // 
      // _cbSymbolSize
      // 
      this._cbSymbolSize.Location = new System.Drawing.Point(93, 137);
      this._cbSymbolSize.Name = "_cbSymbolSize";
      this._cbSymbolSize.Size = new System.Drawing.Size(112, 21);
      this._cbSymbolSize.TabIndex = 15;
      this._cbSymbolSize.Text = "comboBox1";
      this._cbSymbolSize.Validating += new System.ComponentModel.CancelEventHandler(this._cbSymbolSize_Validating);
      // 
      // _chkIndependentSize
      // 
      this._chkIndependentSize.Location = new System.Drawing.Point(7, 137);
      this._chkIndependentSize.Name = "_chkIndependentSize";
      this._chkIndependentSize.Size = new System.Drawing.Size(80, 24);
      this._chkIndependentSize.TabIndex = 28;
      this._chkIndependentSize.Text = "indep. Size";
      // 
      // _chkLineSymbolGap
      // 
      this._chkLineSymbolGap.Location = new System.Drawing.Point(7, 167);
      this._chkLineSymbolGap.Name = "_chkLineSymbolGap";
      this._chkLineSymbolGap.Size = new System.Drawing.Size(112, 24);
      this._chkLineSymbolGap.TabIndex = 29;
      this._chkLineSymbolGap.Text = "Line/Symbol Gap";
      // 
      // _lblPosErrorColumn
      // 
      this._lblPosErrorColumn.Location = new System.Drawing.Point(8, 216);
      this._lblPosErrorColumn.Name = "_lblPosErrorColumn";
      this._lblPosErrorColumn.Size = new System.Drawing.Size(112, 20);
      this._lblPosErrorColumn.TabIndex = 41;
      this._lblPosErrorColumn.Text = "Error column:";
      this._lblPosErrorColumn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // _btSelectErrorColumn
      // 
      this._btSelectErrorColumn.Location = new System.Drawing.Point(376, 216);
      this._btSelectErrorColumn.Name = "_btSelectErrorColumn";
      this._btSelectErrorColumn.Size = new System.Drawing.Size(56, 20);
      this._btSelectErrorColumn.TabIndex = 40;
      this._btSelectErrorColumn.Text = "Select ..";
      this._btSelectErrorColumn.Click += new System.EventHandler(this._btSelectErrorColumn_Click);
      // 
      // _edErrorColumn
      // 
      this._edErrorColumn.Location = new System.Drawing.Point(128, 216);
      this._edErrorColumn.Name = "_edErrorColumn";
      this._edErrorColumn.ReadOnly = true;
      this._edErrorColumn.Size = new System.Drawing.Size(240, 20);
      this._edErrorColumn.TabIndex = 39;
      this._edErrorColumn.Text = "textBox1";
      // 
      // _btSelectNegErrorColumn
      // 
      this._btSelectNegErrorColumn.Location = new System.Drawing.Point(376, 242);
      this._btSelectNegErrorColumn.Name = "_btSelectNegErrorColumn";
      this._btSelectNegErrorColumn.Size = new System.Drawing.Size(56, 20);
      this._btSelectNegErrorColumn.TabIndex = 43;
      this._btSelectNegErrorColumn.Text = "Select ..";
      this._btSelectNegErrorColumn.Click += new System.EventHandler(this._btSelectNegErrorColumn_Click);
      // 
      // _edNegErrorColumn
      // 
      this._edNegErrorColumn.Location = new System.Drawing.Point(128, 242);
      this._edNegErrorColumn.Name = "_edNegErrorColumn";
      this._edNegErrorColumn.ReadOnly = true;
      this._edNegErrorColumn.Size = new System.Drawing.Size(240, 20);
      this._edNegErrorColumn.TabIndex = 42;
      this._edNegErrorColumn.Text = "textBox1";
      // 
      // _chkIndepNegErrorColumn
      // 
      this._chkIndepNegErrorColumn.AutoSize = true;
      this._chkIndepNegErrorColumn.Location = new System.Drawing.Point(16, 245);
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
      this._chkShowEndBars.Location = new System.Drawing.Point(128, 171);
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
      this._cbDashStyle.ItemHeight = 13;
      this._cbDashStyle.Location = new System.Drawing.Point(84, 76);
      this._cbDashStyle.Name = "_cbDashStyle";
      this._cbDashStyle.Size = new System.Drawing.Size(121, 19);
      this._cbDashStyle.TabIndex = 32;
      // 
      // _cbThickness
      // 
      this._cbThickness.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbThickness.FormattingEnabled = true;
      this._cbThickness.ItemHeight = 13;
      this._cbThickness.Location = new System.Drawing.Point(84, 51);
      this._cbThickness.Name = "_cbThickness";
      this._cbThickness.Size = new System.Drawing.Size(121, 19);
      this._cbThickness.TabIndex = 31;
      // 
      // _cbPenColor
      // 
      this._cbPenColor.ColorType = Altaxo.Graph.ColorType.PlotColor;
      this._cbPenColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbPenColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbPenColor.FormattingEnabled = true;
      this._cbPenColor.ItemHeight = 13;
      this._cbPenColor.Items.AddRange(new object[] {
            System.Drawing.Color.Black});
      this._cbPenColor.Location = new System.Drawing.Point(84, 26);
      this._cbPenColor.Name = "_cbPenColor";
      this._cbPenColor.Size = new System.Drawing.Size(121, 19);
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
      this._chkDoNotShift.Location = new System.Drawing.Point(231, 167);
      this._chkDoNotShift.Name = "_chkDoNotShift";
      this._chkDoNotShift.Size = new System.Drawing.Size(137, 24);
      this._chkDoNotShift.TabIndex = 47;
      this._chkDoNotShift.Text = "Not shift indep. var.";
      // 
      // _chkIsHorizontal
      // 
      this._chkIsHorizontal.Location = new System.Drawing.Point(84, 107);
      this._chkIsHorizontal.Name = "_chkIsHorizontal";
      this._chkIsHorizontal.Size = new System.Drawing.Size(137, 24);
      this._chkIsHorizontal.TabIndex = 48;
      this._chkIsHorizontal.Text = "Horizontal error bars";
      // 
      // ErrorBarPlotStyleControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._chkIsHorizontal);
      this.Controls.Add(this._chkDoNotShift);
      this.Controls.Add(this._chkShowEndBars);
      this.Controls.Add(this._chkIndepNegErrorColumn);
      this.Controls.Add(this._btSelectNegErrorColumn);
      this.Controls.Add(this._edNegErrorColumn);
      this.Controls.Add(this._lblPosErrorColumn);
      this.Controls.Add(this._btSelectErrorColumn);
      this.Controls.Add(this._edErrorColumn);
      this.Controls.Add(this._cbDashStyle);
      this.Controls.Add(this._cbThickness);
      this.Controls.Add(this._cbPenColor);
      this.Controls.Add(this._chkLineSymbolGap);
      this.Controls.Add(this._chkIndependentSize);
      this.Controls.Add(this._cbSymbolSize);
      this.Controls.Add(this._chkIndependentColor);
      this.Name = "ErrorBarPlotStyleControl";
      this.Size = new System.Drawing.Size(439, 317);
      this.ResumeLayout(false);
      this.PerformLayout();

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
  }
}
