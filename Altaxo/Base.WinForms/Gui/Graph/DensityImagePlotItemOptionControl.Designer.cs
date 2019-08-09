namespace Altaxo.Gui.Graph
{
	partial class DensityImagePlotItemOptionControl
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
			this._btCopyImageToClipboard = new System.Windows.Forms.Button();
			this._btSaveImageToDisc = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			// _btCopyImageToClipboard
			//
			this._btCopyImageToClipboard.Location = new System.Drawing.Point(12, 13);
			this._btCopyImageToClipboard.Name = "_btCopyImageToClipboard";
			this._btCopyImageToClipboard.Size = new System.Drawing.Size(209, 23);
			this._btCopyImageToClipboard.TabIndex = 0;
			this._btCopyImageToClipboard.Text = "Copy image to clipboard";
			this._btCopyImageToClipboard.UseVisualStyleBackColor = true;
			this._btCopyImageToClipboard.Click += new System.EventHandler(this.EhCopyImageToClipboard);
			//
			// _btSaveImageToDisc
			//
			this._btSaveImageToDisc.Location = new System.Drawing.Point(12, 42);
			this._btSaveImageToDisc.Name = "_btSaveImageToDisc";
			this._btSaveImageToDisc.Size = new System.Drawing.Size(209, 23);
			this._btSaveImageToDisc.TabIndex = 1;
			this._btSaveImageToDisc.Text = "Save image to disc";
			this._btSaveImageToDisc.UseVisualStyleBackColor = true;
			this._btSaveImageToDisc.Click += new System.EventHandler(this.EhSaveImageToDisc);
			//
			// DensityImagePlotItemOptionControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._btSaveImageToDisc);
			this.Controls.Add(this._btCopyImageToClipboard);
			this.Name = "DensityImagePlotItemOptionControl";
			this.Size = new System.Drawing.Size(253, 85);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _btCopyImageToClipboard;
		private System.Windows.Forms.Button _btSaveImageToDisc;
	}
}
