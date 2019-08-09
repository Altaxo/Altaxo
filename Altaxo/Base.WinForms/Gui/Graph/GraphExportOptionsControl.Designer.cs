namespace Altaxo.Gui.Graph
{
	partial class GraphExportOptionsControl
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
      this._tableLayout = new System.Windows.Forms.TableLayoutPanel();
      this._lblClipboardFormat = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this._cbImageFormat = new System.Windows.Forms.ComboBox();
      this._cbPixelFormat = new System.Windows.Forms.ComboBox();
      this._cbExportArea = new System.Windows.Forms.ComboBox();
      this._cbSourceResolution = new System.Windows.Forms.ComboBox();
      this._cbDestinationResolution = new System.Windows.Forms.ComboBox();
      this.label6 = new System.Windows.Forms.Label();
      this._cbClipboardFormat = new System.Windows.Forms.ComboBox();
      this._cbBackgroundBrush = new Altaxo.Gui.Common.Drawing.BrushColorComboBox();
      this._tableLayout.SuspendLayout();
      this.SuspendLayout();
      //
      // _tableLayout
      //
      this._tableLayout.AutoSize = true;
      this._tableLayout.ColumnCount = 2;
      this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._tableLayout.Controls.Add(this._lblClipboardFormat, 0, 6);
      this._tableLayout.Controls.Add(this.label1, 0, 0);
      this._tableLayout.Controls.Add(this.label2, 0, 1);
      this._tableLayout.Controls.Add(this.label3, 0, 2);
      this._tableLayout.Controls.Add(this.label4, 0, 3);
      this._tableLayout.Controls.Add(this.label5, 0, 4);
      this._tableLayout.Controls.Add(this._cbImageFormat, 1, 0);
      this._tableLayout.Controls.Add(this._cbPixelFormat, 1, 1);
      this._tableLayout.Controls.Add(this._cbExportArea, 1, 2);
      this._tableLayout.Controls.Add(this._cbSourceResolution, 1, 3);
      this._tableLayout.Controls.Add(this._cbDestinationResolution, 1, 4);
      this._tableLayout.Controls.Add(this.label6, 0, 5);
      this._tableLayout.Controls.Add(this._cbClipboardFormat, 1, 6);
      this._tableLayout.Controls.Add(this._cbBackgroundBrush, 1, 5);
      this._tableLayout.Location = new System.Drawing.Point(3, 3);
      this._tableLayout.Name = "_tableLayout";
      this._tableLayout.RowCount = 7;
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.Size = new System.Drawing.Size(372, 187);
      this._tableLayout.TabIndex = 0;
      //
      // _lblClipboardFormat
      //
      this._lblClipboardFormat.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblClipboardFormat.AutoSize = true;
      this._lblClipboardFormat.Location = new System.Drawing.Point(45, 167);
      this._lblClipboardFormat.Name = "_lblClipboardFormat";
      this._lblClipboardFormat.Size = new System.Drawing.Size(86, 13);
      this._lblClipboardFormat.TabIndex = 11;
      this._lblClipboardFormat.Text = "Clipboard format:";
      //
      // label1
      //
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(60, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(71, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Image format:";
      //
      // label2
      //
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(67, 34);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(64, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Pixel format:";
      //
      // label3
      //
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(55, 61);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(76, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Area to export:";
      //
      // label4
      //
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(22, 88);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(109, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "Source dpi resolution:";
      //
      // label5
      //
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 115);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(128, 13);
      this.label5.TabIndex = 4;
      this.label5.Text = "Destination dpi resolution:";
      //
      // _cbImageFormat
      //
      this._cbImageFormat.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbImageFormat.FormattingEnabled = true;
      this._cbImageFormat.Location = new System.Drawing.Point(137, 3);
      this._cbImageFormat.Name = "_cbImageFormat";
      this._cbImageFormat.Size = new System.Drawing.Size(232, 21);
      this._cbImageFormat.TabIndex = 5;
      this._cbImageFormat.SelectionChangeCommitted += new System.EventHandler(this.EhImageFormatSelected);
      //
      // _cbPixelFormat
      //
      this._cbPixelFormat.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbPixelFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbPixelFormat.FormattingEnabled = true;
      this._cbPixelFormat.Location = new System.Drawing.Point(137, 30);
      this._cbPixelFormat.Name = "_cbPixelFormat";
      this._cbPixelFormat.Size = new System.Drawing.Size(232, 21);
      this._cbPixelFormat.TabIndex = 6;
      this._cbPixelFormat.SelectionChangeCommitted += new System.EventHandler(this.EhPixelFormatSelected);
      //
      // _cbExportArea
      //
      this._cbExportArea.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbExportArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbExportArea.FormattingEnabled = true;
      this._cbExportArea.Location = new System.Drawing.Point(137, 57);
      this._cbExportArea.Name = "_cbExportArea";
      this._cbExportArea.Size = new System.Drawing.Size(232, 21);
      this._cbExportArea.TabIndex = 7;
      this._cbExportArea.SelectionChangeCommitted += new System.EventHandler(this.EhExportAreaSelected);
      //
      // _cbSourceResolution
      //
      this._cbSourceResolution.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbSourceResolution.FormattingEnabled = true;
      this._cbSourceResolution.Location = new System.Drawing.Point(137, 84);
      this._cbSourceResolution.Name = "_cbSourceResolution";
      this._cbSourceResolution.Size = new System.Drawing.Size(232, 21);
      this._cbSourceResolution.TabIndex = 8;
      //
      // _cbDestinationResolution
      //
      this._cbDestinationResolution.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbDestinationResolution.FormattingEnabled = true;
      this._cbDestinationResolution.Location = new System.Drawing.Point(137, 111);
      this._cbDestinationResolution.Name = "_cbDestinationResolution";
      this._cbDestinationResolution.Size = new System.Drawing.Size(232, 21);
      this._cbDestinationResolution.TabIndex = 9;
      //
      // label6
      //
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(63, 141);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(68, 13);
      this.label6.TabIndex = 10;
      this.label6.Text = "Background:";
      //
      // _cbClipboardFormat
      //
      this._cbClipboardFormat.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbClipboardFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbClipboardFormat.FormattingEnabled = true;
      this._cbClipboardFormat.Location = new System.Drawing.Point(137, 163);
      this._cbClipboardFormat.Name = "_cbClipboardFormat";
      this._cbClipboardFormat.Size = new System.Drawing.Size(232, 21);
      this._cbClipboardFormat.TabIndex = 12;
      this._cbClipboardFormat.SelectionChangeCommitted += new System.EventHandler(this.EhClipboardFormatSelected);
      //
      // _cbBackgroundBrush
      //
      this._cbBackgroundBrush.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._cbBackgroundBrush.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbBackgroundBrush.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbBackgroundBrush.FormattingEnabled = true;
      this._cbBackgroundBrush.ItemHeight = 13;
      this._cbBackgroundBrush.Location = new System.Drawing.Point(137, 138);
      this._cbBackgroundBrush.Name = "_cbBackgroundBrush";
      this._cbBackgroundBrush.Size = new System.Drawing.Size(232, 19);
      this._cbBackgroundBrush.TabIndex = 13;
      //
      // GraphExportOptionsControl
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.Controls.Add(this._tableLayout);
      this.Name = "GraphExportOptionsControl";
      this.Size = new System.Drawing.Size(378, 193);
      this._tableLayout.ResumeLayout(false);
      this._tableLayout.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayout;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox _cbImageFormat;
		private System.Windows.Forms.ComboBox _cbPixelFormat;
		private System.Windows.Forms.ComboBox _cbExportArea;
		private System.Windows.Forms.ComboBox _cbSourceResolution;
		private System.Windows.Forms.ComboBox _cbDestinationResolution;
    private System.Windows.Forms.Label _lblClipboardFormat;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.ComboBox _cbClipboardFormat;
    private Altaxo.Gui.Common.Drawing.BrushColorComboBox _cbBackgroundBrush;
	}
}
