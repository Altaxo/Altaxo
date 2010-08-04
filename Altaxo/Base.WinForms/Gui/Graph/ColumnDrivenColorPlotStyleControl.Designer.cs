namespace Altaxo.Gui.Graph
{
	partial class ColumnDrivenColorPlotStyleControl
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
			this._grpScale = new System.Windows.Forms.GroupBox();
			this._ctrlScale = new Altaxo.Gui.Graph.DensityScaleControl();
			this._btClearDataColumn = new System.Windows.Forms.Button();
			this._edDataColumn = new System.Windows.Forms.TextBox();
			this._btSelectDataColumn = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._colorProviderControl = new Altaxo.Gui.Graph.ColorProviderControl();
			this._grpScale.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _grpScale
			// 
			this._grpScale.AutoSize = true;
			this._grpScale.Controls.Add(this._ctrlScale);
			this._grpScale.Location = new System.Drawing.Point(3, 3);
			this._grpScale.Name = "_grpScale";
			this._grpScale.Size = new System.Drawing.Size(334, 88);
			this._grpScale.TabIndex = 1;
			this._grpScale.TabStop = false;
			this._grpScale.Text = "Scale:";
			// 
			// _ctrlScale
			// 
			this._ctrlScale.AutoSize = true;
			this._ctrlScale.Location = new System.Drawing.Point(6, 19);
			this._ctrlScale.Name = "_ctrlScale";
			this._ctrlScale.Size = new System.Drawing.Size(322, 50);
			this._ctrlScale.TabIndex = 0;
			// 
			// _btClearDataColumn
			// 
			this._btClearDataColumn.Location = new System.Drawing.Point(281, 45);
			this._btClearDataColumn.Name = "_btClearDataColumn";
			this._btClearDataColumn.Size = new System.Drawing.Size(49, 20);
			this._btClearDataColumn.TabIndex = 58;
			this._btClearDataColumn.Text = "Clear!";
			this._btClearDataColumn.UseVisualStyleBackColor = true;
			this._btClearDataColumn.Click += new System.EventHandler(this._btClearDataColumn_Click);
			// 
			// _edDataColumn
			// 
			this._edDataColumn.Location = new System.Drawing.Point(8, 19);
			this._edDataColumn.Name = "_edDataColumn";
			this._edDataColumn.ReadOnly = true;
			this._edDataColumn.Size = new System.Drawing.Size(322, 20);
			this._edDataColumn.TabIndex = 55;
			this._edDataColumn.Text = "textBox1";
			// 
			// _btSelectDataColumn
			// 
			this._btSelectDataColumn.Location = new System.Drawing.Point(8, 45);
			this._btSelectDataColumn.Name = "_btSelectDataColumn";
			this._btSelectDataColumn.Size = new System.Drawing.Size(56, 20);
			this._btSelectDataColumn.TabIndex = 56;
			this._btSelectDataColumn.Text = "Select ..";
			this._btSelectDataColumn.Click += new System.EventHandler(this._btSelectDataColumn_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._edDataColumn);
			this.groupBox1.Controls.Add(this._btSelectDataColumn);
			this.groupBox1.Controls.Add(this._btClearDataColumn);
			this.groupBox1.Location = new System.Drawing.Point(3, 97);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(370, 76);
			this.groupBox1.TabIndex = 60;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data column:";
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this._grpScale);
			this.flowLayoutPanel1.Controls.Add(this.groupBox1);
			this.flowLayoutPanel1.Controls.Add(this._colorProviderControl);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(446, 497);
			this.flowLayoutPanel1.TabIndex = 61;
			// 
			// _colorProviderControl
			// 
			this._colorProviderControl.AutoSize = true;
			this._colorProviderControl.Location = new System.Drawing.Point(3, 179);
			this._colorProviderControl.Name = "_colorProviderControl";
			this._colorProviderControl.Size = new System.Drawing.Size(224, 70);
			this._colorProviderControl.TabIndex = 61;
			// 
			// ColumnDrivenColorPlotStyleControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "ColumnDrivenColorPlotStyleControl";
			this.Size = new System.Drawing.Size(446, 497);
			this._grpScale.ResumeLayout(false);
			this._grpScale.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private DensityScaleControl _ctrlScale;
		private System.Windows.Forms.GroupBox _grpScale;
		private System.Windows.Forms.Button _btClearDataColumn;
		private System.Windows.Forms.TextBox _edDataColumn;
		private System.Windows.Forms.Button _btSelectDataColumn;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private ColorProviderControl _colorProviderControl;
	}
}
