namespace Altaxo.Gui.Graph
{
  partial class BackgroundStyleControl
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
      this._cbStyles = new System.Windows.Forms.ComboBox();
      this._cbColors = new Altaxo.Gui.Common.Drawing.BrushColorComboBox();
      this.SuspendLayout();
      // 
      // _cbStyles
      // 
      this._cbStyles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbStyles.FormattingEnabled = true;
      this._cbStyles.ItemHeight = 13;
      this._cbStyles.Location = new System.Drawing.Point(0, 0);
      this._cbStyles.Margin = new System.Windows.Forms.Padding(0);
      this._cbStyles.Name = "_cbStyles";
      this._cbStyles.Size = new System.Drawing.Size(121, 21);
      this._cbStyles.TabIndex = 0;
      this._cbStyles.SelectionChangeCommitted += new System.EventHandler(this._cbBackgroundStyle_SelectionChangeCommitted);
      // 
      // _cbColors
      // 
      this._cbColors.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._cbColors.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbColors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbColors.FormattingEnabled = true;
      this._cbColors.ItemHeight = 15;
      this._cbColors.Location = new System.Drawing.Point(133, 0);
      this._cbColors.Margin = new System.Windows.Forms.Padding(0);
      this._cbColors.Name = "_cbColors";
      this._cbColors.Size = new System.Drawing.Size(129, 21);
      this._cbColors.TabIndex = 2;
      this._cbColors.SelectionChangeCommitted += new System.EventHandler(this.EhBackgroundColor_SelectionChangeCommitted);
      // 
      // BackgroundStyleControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.Controls.Add(this._cbColors);
      this.Controls.Add(this._cbStyles);
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "BackgroundStyleControl";
      this.Size = new System.Drawing.Size(262, 21);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox _cbStyles;
    private Altaxo.Gui.Common.Drawing.BrushColorComboBox _cbColors;
  }
}
