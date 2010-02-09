namespace Altaxo.Gui.Graph
{
	partial class ColorProviderControl
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
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._lblColorProvider = new System.Windows.Forms.Label();
			this._cbColorProvider = new System.Windows.Forms.ComboBox();
			this._lblPreview = new System.Windows.Forms.Label();
			this._previewPanel = new System.Windows.Forms.PictureBox();
			this._tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._previewPanel)).BeginInit();
			this.SuspendLayout();
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.AutoSize = true;
			this._tableLayoutPanel.ColumnCount = 2;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.Controls.Add(this._lblColorProvider, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._cbColorProvider, 1, 0);
			this._tableLayoutPanel.Controls.Add(this._lblPreview, 0, 2);
			this._tableLayoutPanel.Controls.Add(this._previewPanel, 1, 2);
			this._tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 3;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.Size = new System.Drawing.Size(218, 64);
			this._tableLayoutPanel.TabIndex = 0;
			// 
			// _lblColorProvider
			// 
			this._lblColorProvider.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblColorProvider.AutoSize = true;
			this._lblColorProvider.Location = new System.Drawing.Point(3, 7);
			this._lblColorProvider.Name = "_lblColorProvider";
			this._lblColorProvider.Size = new System.Drawing.Size(64, 13);
			this._lblColorProvider.TabIndex = 0;
			this._lblColorProvider.Text = "Colorization:";
			// 
			// _cbColorProvider
			// 
			this._cbColorProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._cbColorProvider.FormattingEnabled = true;
			this._cbColorProvider.Location = new System.Drawing.Point(73, 3);
			this._cbColorProvider.Name = "_cbColorProvider";
			this._cbColorProvider.Size = new System.Drawing.Size(142, 21);
			this._cbColorProvider.TabIndex = 1;
			this._cbColorProvider.SelectionChangeCommitted += new System.EventHandler(this.EhColorProviderChanged);
			// 
			// _lblPreview
			// 
			this._lblPreview.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblPreview.AutoSize = true;
			this._lblPreview.Location = new System.Drawing.Point(19, 39);
			this._lblPreview.Name = "_lblPreview";
			this._lblPreview.Size = new System.Drawing.Size(48, 13);
			this._lblPreview.TabIndex = 2;
			this._lblPreview.Text = "Preview:";
			// 
			// _previewPanel
			// 
			this._previewPanel.Location = new System.Drawing.Point(73, 30);
			this._previewPanel.Name = "_previewPanel";
			this._previewPanel.Size = new System.Drawing.Size(142, 31);
			this._previewPanel.TabIndex = 3;
			this._previewPanel.TabStop = false;
			// 
			// ColorProviderControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "ColorProviderControl";
			this.Size = new System.Drawing.Size(224, 70);
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._previewPanel)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private System.Windows.Forms.Label _lblColorProvider;
		private System.Windows.Forms.ComboBox _cbColorProvider;
		private System.Windows.Forms.Label _lblPreview;
		private System.Windows.Forms.PictureBox _previewPanel;
	}
}
