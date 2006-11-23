namespace Altaxo.Gui.Graph
{
  partial class PlotGroupCollectionControl
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
      this._lbCoordTransfoStylesAvailable = new System.Windows.Forms.ListBox();
      this._lbGroupStylesAvailable = new System.Windows.Forms.ListBox();
      this._edCoordTransfoStyle = new System.Windows.Forms.TextBox();
      this._lbGroupStyles = new System.Windows.Forms.CheckedListBox();
      this._btAddCoordTransfoGroupStyle = new System.Windows.Forms.Button();
      this._btRemoveCoordTransfoGroupStyle = new System.Windows.Forms.Button();
      this._btRemoveNormalGroupStyle = new System.Windows.Forms.Button();
      this._btAddNormalGroupStyle = new System.Windows.Forms.Button();
      this._btDown = new System.Windows.Forms.Button();
      this._btUp = new System.Windows.Forms.Button();
      this._btUnindent = new System.Windows.Forms.Button();
      this._btIndent = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // _lbCoordTransfoStylesAvailable
      // 
      this._lbCoordTransfoStylesAvailable.FormattingEnabled = true;
      this._lbCoordTransfoStylesAvailable.Location = new System.Drawing.Point(0, 0);
      this._lbCoordTransfoStylesAvailable.Name = "_lbCoordTransfoStylesAvailable";
      this._lbCoordTransfoStylesAvailable.Size = new System.Drawing.Size(138, 108);
      this._lbCoordTransfoStylesAvailable.TabIndex = 0;
      // 
      // _lbGroupStylesAvailable
      // 
      this._lbGroupStylesAvailable.FormattingEnabled = true;
      this._lbGroupStylesAvailable.Location = new System.Drawing.Point(0, 114);
      this._lbGroupStylesAvailable.Name = "_lbGroupStylesAvailable";
      this._lbGroupStylesAvailable.Size = new System.Drawing.Size(138, 173);
      this._lbGroupStylesAvailable.TabIndex = 1;
      // 
      // _edCoordTransfoStyle
      // 
      this._edCoordTransfoStyle.Enabled = false;
      this._edCoordTransfoStyle.Location = new System.Drawing.Point(196, 0);
      this._edCoordTransfoStyle.Name = "_edCoordTransfoStyle";
      this._edCoordTransfoStyle.Size = new System.Drawing.Size(182, 20);
      this._edCoordTransfoStyle.TabIndex = 2;
      // 
      // _lbGroupStyles
      // 
      this._lbGroupStyles.FormattingEnabled = true;
      this._lbGroupStyles.Location = new System.Drawing.Point(196, 28);
      this._lbGroupStyles.Name = "_lbGroupStyles";
      this._lbGroupStyles.Size = new System.Drawing.Size(182, 259);
      this._lbGroupStyles.TabIndex = 3;
      // 
      // _btAddCoordTransfoGroupStyle
      // 
      this._btAddCoordTransfoGroupStyle.Location = new System.Drawing.Point(170, 0);
      this._btAddCoordTransfoGroupStyle.Name = "_btAddCoordTransfoGroupStyle";
      this._btAddCoordTransfoGroupStyle.Size = new System.Drawing.Size(20, 20);
      this._btAddCoordTransfoGroupStyle.TabIndex = 4;
      this._btAddCoordTransfoGroupStyle.Text = ">";
      this._btAddCoordTransfoGroupStyle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this._btAddCoordTransfoGroupStyle.UseVisualStyleBackColor = true;
      this._btAddCoordTransfoGroupStyle.Click += new System.EventHandler(this._btAddCoordTransfoGroupStyle_Click);
      // 
      // _btRemoveCoordTransfoGroupStyle
      // 
      this._btRemoveCoordTransfoGroupStyle.Location = new System.Drawing.Point(144, 0);
      this._btRemoveCoordTransfoGroupStyle.Name = "_btRemoveCoordTransfoGroupStyle";
      this._btRemoveCoordTransfoGroupStyle.Size = new System.Drawing.Size(20, 20);
      this._btRemoveCoordTransfoGroupStyle.TabIndex = 5;
      this._btRemoveCoordTransfoGroupStyle.Text = "<";
      this._btRemoveCoordTransfoGroupStyle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this._btRemoveCoordTransfoGroupStyle.UseVisualStyleBackColor = true;
      this._btRemoveCoordTransfoGroupStyle.Click += new System.EventHandler(this._btRemoveCoordTransfoGroupStyle_Click);
      // 
      // _btRemoveNormalGroupStyle
      // 
      this._btRemoveNormalGroupStyle.Location = new System.Drawing.Point(144, 114);
      this._btRemoveNormalGroupStyle.Name = "_btRemoveNormalGroupStyle";
      this._btRemoveNormalGroupStyle.Size = new System.Drawing.Size(20, 20);
      this._btRemoveNormalGroupStyle.TabIndex = 6;
      this._btRemoveNormalGroupStyle.Text = "<";
      this._btRemoveNormalGroupStyle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this._btRemoveNormalGroupStyle.UseVisualStyleBackColor = true;
      this._btRemoveNormalGroupStyle.Click += new System.EventHandler(this._btRemoveNormalGroupStyle_Click);
      // 
      // _btAddNormalGroupStyle
      // 
      this._btAddNormalGroupStyle.Location = new System.Drawing.Point(172, 114);
      this._btAddNormalGroupStyle.Name = "_btAddNormalGroupStyle";
      this._btAddNormalGroupStyle.Size = new System.Drawing.Size(20, 20);
      this._btAddNormalGroupStyle.TabIndex = 7;
      this._btAddNormalGroupStyle.Text = ">";
      this._btAddNormalGroupStyle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this._btAddNormalGroupStyle.UseVisualStyleBackColor = true;
      this._btAddNormalGroupStyle.Click += new System.EventHandler(this._btAddNormalGroupStyle_Click);
      // 
      // _btDown
      // 
      this._btDown.Location = new System.Drawing.Point(172, 267);
      this._btDown.Name = "_btDown";
      this._btDown.Size = new System.Drawing.Size(20, 20);
      this._btDown.TabIndex = 8;
      this._btDown.Text = "V";
      this._btDown.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this._btDown.UseVisualStyleBackColor = true;
      this._btDown.Click += new System.EventHandler(this._btDown_Click);
      // 
      // _btUp
      // 
      this._btUp.Location = new System.Drawing.Point(172, 241);
      this._btUp.Name = "_btUp";
      this._btUp.Size = new System.Drawing.Size(20, 20);
      this._btUp.TabIndex = 9;
      this._btUp.Text = "?";
      this._btUp.UseVisualStyleBackColor = true;
      this._btUp.Click += new System.EventHandler(this._btUp_Click);
      // 
      // _btUnindent
      // 
      this._btUnindent.Location = new System.Drawing.Point(172, 215);
      this._btUnindent.Name = "_btUnindent";
      this._btUnindent.Size = new System.Drawing.Size(20, 20);
      this._btUnindent.TabIndex = 10;
      this._btUnindent.Text = "<";
      this._btUnindent.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this._btUnindent.UseVisualStyleBackColor = true;
      this._btUnindent.Click += new System.EventHandler(this._btUnindent_Click);
      // 
      // _btIndent
      // 
      this._btIndent.Location = new System.Drawing.Point(172, 189);
      this._btIndent.Name = "_btIndent";
      this._btIndent.Size = new System.Drawing.Size(20, 20);
      this._btIndent.TabIndex = 11;
      this._btIndent.Text = ">";
      this._btIndent.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this._btIndent.UseVisualStyleBackColor = true;
      this._btIndent.Click += new System.EventHandler(this._btIndent_Click);
      // 
      // PlotGroupCollectionControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._btIndent);
      this.Controls.Add(this._btUnindent);
      this.Controls.Add(this._btUp);
      this.Controls.Add(this._btDown);
      this.Controls.Add(this._btAddNormalGroupStyle);
      this.Controls.Add(this._btRemoveNormalGroupStyle);
      this.Controls.Add(this._btRemoveCoordTransfoGroupStyle);
      this.Controls.Add(this._btAddCoordTransfoGroupStyle);
      this.Controls.Add(this._lbGroupStyles);
      this.Controls.Add(this._edCoordTransfoStyle);
      this.Controls.Add(this._lbGroupStylesAvailable);
      this.Controls.Add(this._lbCoordTransfoStylesAvailable);
      this.Name = "PlotGroupCollectionControl";
      this.Size = new System.Drawing.Size(378, 289);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox _lbCoordTransfoStylesAvailable;
    private System.Windows.Forms.ListBox _lbGroupStylesAvailable;
    private System.Windows.Forms.TextBox _edCoordTransfoStyle;
    private System.Windows.Forms.CheckedListBox _lbGroupStyles;
    private System.Windows.Forms.Button _btAddCoordTransfoGroupStyle;
    private System.Windows.Forms.Button _btRemoveCoordTransfoGroupStyle;
    private System.Windows.Forms.Button _btRemoveNormalGroupStyle;
    private System.Windows.Forms.Button _btAddNormalGroupStyle;
    private System.Windows.Forms.Button _btDown;
    private System.Windows.Forms.Button _btUp;
    private System.Windows.Forms.Button _btUnindent;
    private System.Windows.Forms.Button _btIndent;
  }
}
