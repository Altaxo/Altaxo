namespace Altaxo.Gui.Graph
{
  partial class WaterfallTransformControl
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
			this._edYScale = new System.Windows.Forms.TextBox();
			this._lblXScale = new System.Windows.Forms.Label();
			this._lblYScale = new System.Windows.Forms.Label();
			this._lblClip = new System.Windows.Forms.Label();
			this._edXScale = new System.Windows.Forms.TextBox();
			this._chkClipValues = new System.Windows.Forms.CheckBox();
			this._tableLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayout
			// 
			this._tableLayout.AutoSize = true;
			this._tableLayout.ColumnCount = 2;
			this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayout.Controls.Add(this._edYScale, 1, 1);
			this._tableLayout.Controls.Add(this._lblXScale, 0, 0);
			this._tableLayout.Controls.Add(this._lblYScale, 0, 1);
			this._tableLayout.Controls.Add(this._lblClip, 0, 2);
			this._tableLayout.Controls.Add(this._edXScale, 1, 0);
			this._tableLayout.Controls.Add(this._chkClipValues, 1, 2);
			this._tableLayout.Location = new System.Drawing.Point(3, 3);
			this._tableLayout.Name = "_tableLayout";
			this._tableLayout.RowCount = 3;
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayout.Size = new System.Drawing.Size(151, 72);
			this._tableLayout.TabIndex = 0;
			// 
			// _edYScale
			// 
			this._edYScale.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._edYScale.Location = new System.Drawing.Point(54, 29);
			this._edYScale.Name = "_edYScale";
			this._edYScale.Size = new System.Drawing.Size(94, 20);
			this._edYScale.TabIndex = 4;
			// 
			// _lblXScale
			// 
			this._lblXScale.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblXScale.AutoSize = true;
			this._lblXScale.Location = new System.Drawing.Point(3, 6);
			this._lblXScale.Name = "_lblXScale";
			this._lblXScale.Size = new System.Drawing.Size(45, 13);
			this._lblXScale.TabIndex = 0;
			this._lblXScale.Text = "X-scale:";
			// 
			// _lblYScale
			// 
			this._lblYScale.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblYScale.AutoSize = true;
			this._lblYScale.Location = new System.Drawing.Point(3, 32);
			this._lblYScale.Name = "_lblYScale";
			this._lblYScale.Size = new System.Drawing.Size(45, 13);
			this._lblYScale.TabIndex = 1;
			this._lblYScale.Text = "Y-scale:";
			// 
			// _lblClip
			// 
			this._lblClip.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblClip.AutoSize = true;
			this._lblClip.Location = new System.Drawing.Point(21, 55);
			this._lblClip.Name = "_lblClip";
			this._lblClip.Size = new System.Drawing.Size(27, 13);
			this._lblClip.TabIndex = 2;
			this._lblClip.Text = "Clip:";
			// 
			// _edXScale
			// 
			this._edXScale.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._edXScale.Location = new System.Drawing.Point(54, 3);
			this._edXScale.Name = "_edXScale";
			this._edXScale.Size = new System.Drawing.Size(94, 20);
			this._edXScale.TabIndex = 3;
			// 
			// _chkClipValues
			// 
			this._chkClipValues.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._chkClipValues.AutoSize = true;
			this._chkClipValues.Location = new System.Drawing.Point(54, 55);
			this._chkClipValues.Name = "_chkClipValues";
			this._chkClipValues.Size = new System.Drawing.Size(15, 14);
			this._chkClipValues.TabIndex = 5;
			this._chkClipValues.UseVisualStyleBackColor = true;
			// 
			// WaterfallTransformControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._tableLayout);
			this.Name = "WaterfallTransformControl";
			this.Size = new System.Drawing.Size(157, 78);
			this._tableLayout.ResumeLayout(false);
			this._tableLayout.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel _tableLayout;
    private System.Windows.Forms.TextBox _edYScale;
    private System.Windows.Forms.Label _lblXScale;
    private System.Windows.Forms.Label _lblYScale;
    private System.Windows.Forms.Label _lblClip;
    private System.Windows.Forms.TextBox _edXScale;
    private System.Windows.Forms.CheckBox _chkClipValues;
  }
}
