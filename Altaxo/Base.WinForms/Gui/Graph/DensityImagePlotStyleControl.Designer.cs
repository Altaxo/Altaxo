namespace Altaxo.Gui.Graph
{
	partial class DensityImagePlotStyleControl
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
			this._flowLayoutVerticalMain = new System.Windows.Forms.FlowLayoutPanel();
			this._grpScale = new System.Windows.Forms.GroupBox();
			this._grpColorProvider = new System.Windows.Forms.GroupBox();
			this._grpOtherProperties = new System.Windows.Forms.GroupBox();
			this._chkClipToLayer = new System.Windows.Forms.CheckBox();
			this._ctrlDensityScale = new Altaxo.Gui.Graph.DensityScaleControl();
			this._ctrlColorProvider = new Altaxo.Gui.Graph.ColorProviderControl();
			this._flowLayoutVerticalMain.SuspendLayout();
			this._grpScale.SuspendLayout();
			this._grpColorProvider.SuspendLayout();
			this._grpOtherProperties.SuspendLayout();
			this.SuspendLayout();
			// 
			// _flowLayoutVerticalMain
			// 
			this._flowLayoutVerticalMain.AutoSize = true;
			this._flowLayoutVerticalMain.Controls.Add(this._grpScale);
			this._flowLayoutVerticalMain.Controls.Add(this._grpColorProvider);
			this._flowLayoutVerticalMain.Controls.Add(this._grpOtherProperties);
			this._flowLayoutVerticalMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this._flowLayoutVerticalMain.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._flowLayoutVerticalMain.Location = new System.Drawing.Point(0, 0);
			this._flowLayoutVerticalMain.Name = "_flowLayoutVerticalMain";
			this._flowLayoutVerticalMain.Size = new System.Drawing.Size(340, 484);
			this._flowLayoutVerticalMain.TabIndex = 0;
			// 
			// _grpScale
			// 
			this._grpScale.AutoSize = true;
			this._grpScale.Controls.Add(this._ctrlDensityScale);
			this._grpScale.Location = new System.Drawing.Point(3, 3);
			this._grpScale.Name = "_grpScale";
			this._grpScale.Size = new System.Drawing.Size(334, 325);
			this._grpScale.TabIndex = 0;
			this._grpScale.TabStop = false;
			this._grpScale.Text = "Scale:";
			// 
			// _grpColorProvider
			// 
			this._grpColorProvider.AutoSize = true;
			this._grpColorProvider.Controls.Add(this._ctrlColorProvider);
			this._grpColorProvider.Location = new System.Drawing.Point(3, 334);
			this._grpColorProvider.Name = "_grpColorProvider";
			this._grpColorProvider.Size = new System.Drawing.Size(236, 92);
			this._grpColorProvider.TabIndex = 1;
			this._grpColorProvider.TabStop = false;
			this._grpColorProvider.Text = "Colorization:";
			// 
			// _grpOtherProperties
			// 
			this._grpOtherProperties.Controls.Add(this._chkClipToLayer);
			this._grpOtherProperties.Location = new System.Drawing.Point(3, 432);
			this._grpOtherProperties.Name = "_grpOtherProperties";
			this._grpOtherProperties.Size = new System.Drawing.Size(256, 49);
			this._grpOtherProperties.TabIndex = 2;
			this._grpOtherProperties.TabStop = false;
			this._grpOtherProperties.Text = "Other:";
			// 
			// _chkClipToLayer
			// 
			this._chkClipToLayer.AutoSize = true;
			this._chkClipToLayer.Location = new System.Drawing.Point(6, 19);
			this._chkClipToLayer.Name = "_chkClipToLayer";
			this._chkClipToLayer.Size = new System.Drawing.Size(80, 17);
			this._chkClipToLayer.TabIndex = 0;
			this._chkClipToLayer.Text = "Clip to layer";
			this._chkClipToLayer.UseVisualStyleBackColor = true;
			// 
			// _ctrlDensityScale
			// 
			this._ctrlDensityScale.AutoSize = true;
			this._ctrlDensityScale.Location = new System.Drawing.Point(6, 19);
			this._ctrlDensityScale.Name = "_ctrlDensityScale";
			this._ctrlDensityScale.Size = new System.Drawing.Size(322, 287);
			this._ctrlDensityScale.TabIndex = 0;
			// 
			// _ctrlColorProvider
			// 
			this._ctrlColorProvider.AutoSize = true;
			this._ctrlColorProvider.Location = new System.Drawing.Point(6, 19);
			this._ctrlColorProvider.Name = "_ctrlColorProvider";
			this._ctrlColorProvider.Size = new System.Drawing.Size(224, 54);
			this._ctrlColorProvider.TabIndex = 0;
			// 
			// DensityImagePlotStyleControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._flowLayoutVerticalMain);
			this.Name = "DensityImagePlotStyleControl";
			this.Size = new System.Drawing.Size(340, 484);
			this._flowLayoutVerticalMain.ResumeLayout(false);
			this._flowLayoutVerticalMain.PerformLayout();
			this._grpScale.ResumeLayout(false);
			this._grpScale.PerformLayout();
			this._grpColorProvider.ResumeLayout(false);
			this._grpColorProvider.PerformLayout();
			this._grpOtherProperties.ResumeLayout(false);
			this._grpOtherProperties.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel _flowLayoutVerticalMain;
		private System.Windows.Forms.GroupBox _grpScale;
		private System.Windows.Forms.GroupBox _grpColorProvider;
		private System.Windows.Forms.GroupBox _grpOtherProperties;
		private System.Windows.Forms.CheckBox _chkClipToLayer;
		private DensityScaleControl _ctrlDensityScale;
		private ColorProviderControl _ctrlColorProvider;
	}
}
