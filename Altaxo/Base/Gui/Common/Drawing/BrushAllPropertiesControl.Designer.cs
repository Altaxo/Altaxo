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
      this._cbGradientMode = new Altaxo.Gui.Common.Drawing.LinearGradientModeExComboBox();
      this._lblGradientMode = new System.Windows.Forms.Label();
      this._cbGradientShape = new Altaxo.Gui.Common.Drawing.LinearGradientShapeComboBox();
      this._lblGradientShape = new System.Windows.Forms.Label();
      this._cbGradientFocus = new Altaxo.Gui.Common.Drawing.GradientFocusComboBox();
      this._cbGradientScale = new Altaxo.Gui.Common.Drawing.GradientScaleComboBox();
      this._brushPreviewPanel = new System.Windows.Forms.Panel();
      this._brushGlue = new Altaxo.Gui.Common.Drawing.BrushControlsGlue();
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
      this.flowLayoutPanel1.Size = new System.Drawing.Size(402, 316);
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
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 5;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(396, 135);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // _lblGradientScale
      // 
      this._lblGradientScale.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblGradientScale.AutoSize = true;
      this._lblGradientScale.Location = new System.Drawing.Point(29, 115);
      this._lblGradientScale.Name = "_lblGradientScale";
      this._lblGradientScale.Size = new System.Drawing.Size(37, 13);
      this._lblGradientScale.TabIndex = 17;
      this._lblGradientScale.Text = "Scale:";
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
      // _brushPreviewPanel
      // 
      this._brushPreviewPanel.Location = new System.Drawing.Point(3, 144);
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
      this._brushGlue.CbWrapMode = this._cbWrapMode;
      this._brushGlue.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._brushGlue.LabelColor2 = this._lblBackColor;
      this._brushGlue.LabelGradientFocus = this._lblGradientFocus;
      this._brushGlue.LabelGradientMode = this._lblGradientMode;
      this._brushGlue.LabelGradientScale = this._lblGradientScale;
      this._brushGlue.LabelGradientShape = this._lblGradientShape;
      this._brushGlue.LabelHatchStyle = this._lblHatchStyle;
      this._brushGlue.LabelWrapMode = this._lblWrapMode;
      this._brushGlue.BrushChanged += new System.EventHandler(this.EhBrushChanged);
      // 
      // BrushAllPropertiesControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.Controls.Add(this.flowLayoutPanel1);
      this.Name = "BrushAllPropertiesControl";
      this.Size = new System.Drawing.Size(405, 319);
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
    private LinearGradientModeExComboBox _cbGradientMode;
    private System.Windows.Forms.Label _lblGradientMode;
    private LinearGradientShapeComboBox _cbGradientShape;
    private System.Windows.Forms.Label _lblGradientShape;
    private System.Windows.Forms.Label _lblGradientFocus;
    private GradientFocusComboBox _cbGradientFocus;
    private System.Windows.Forms.Label _lblGradientScale;
    private GradientScaleComboBox _cbGradientScale;
  }
}
