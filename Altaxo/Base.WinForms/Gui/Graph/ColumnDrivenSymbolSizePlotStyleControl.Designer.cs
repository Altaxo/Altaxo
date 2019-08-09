namespace Altaxo.Gui.Graph
{
	partial class ColumnDrivenSymbolSizePlotStyleControl
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
			this._btClearDataColumn = new System.Windows.Forms.Button();
			this._edDataColumn = new System.Windows.Forms.TextBox();
			this._btSelectDataColumn = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this._edNumberOfSteps = new System.Windows.Forms.NumericUpDown();
			this._cbSymbolSizeAt0 = new Altaxo.Gui.Common.Drawing.SingleSizeComboBox();
			this._cbSymbolSizeAt1 = new Altaxo.Gui.Common.Drawing.SingleSizeComboBox();
			this._cbSymbolSizeBelow = new Altaxo.Gui.Common.Drawing.SingleSizeComboBox();
			this._cbSymbolSizeAbove = new Altaxo.Gui.Common.Drawing.SingleSizeComboBox();
			this._cbSymbolSizeInvalid = new Altaxo.Gui.Common.Drawing.SingleSizeComboBox();
			this._ctrlScale = new Altaxo.Gui.Graph.DensityScaleControl();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._grpScale.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._edNumberOfSteps)).BeginInit();
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
			// tableLayoutPanel1
			//
			this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.label6, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this._edNumberOfSteps, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this._cbSymbolSizeAt0, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this._cbSymbolSizeAt1, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this._cbSymbolSizeBelow, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this._cbSymbolSizeAbove, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this._cbSymbolSizeInvalid, 1, 4);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 179);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 6;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(324, 166);
			this.tableLayoutPanel1.TabIndex = 59;
			//
			// label1
			//
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Symbol size at 0:";
			//
			// label2
			//
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(86, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Symbol size at 1:";
			//
			// label3
			//
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(5, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Symbol size below:";
			//
			// label4
			//
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 81);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(98, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Symbol size above:";
			//
			// label5
			//
			this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 106);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(98, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "Symbol size invalid:";
			//
			// label6
			//
			this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(14, 139);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(87, 13);
			this.label6.TabIndex = 5;
			this.label6.Text = "Number of steps:";
			//
			// _edNumberOfSteps
			//
			this._edNumberOfSteps.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._edNumberOfSteps.Location = new System.Drawing.Point(107, 135);
			this._edNumberOfSteps.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this._edNumberOfSteps.Name = "_edNumberOfSteps";
			this._edNumberOfSteps.Size = new System.Drawing.Size(121, 20);
			this._edNumberOfSteps.TabIndex = 6;
			//
			// _cbSymbolSizeAt0
			//
			this._cbSymbolSizeAt0.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._cbSymbolSizeAt0.FormattingEnabled = true;
			this._cbSymbolSizeAt0.ItemHeight = 13;
			this._cbSymbolSizeAt0.Location = new System.Drawing.Point(107, 3);
			this._cbSymbolSizeAt0.Name = "_cbSymbolSizeAt0";
			this._cbSymbolSizeAt0.Size = new System.Drawing.Size(121, 19);
			this._cbSymbolSizeAt0.TabIndex = 12;
			//
			// _cbSymbolSizeAt1
			//
			this._cbSymbolSizeAt1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._cbSymbolSizeAt1.FormattingEnabled = true;
			this._cbSymbolSizeAt1.ItemHeight = 13;
			this._cbSymbolSizeAt1.Location = new System.Drawing.Point(107, 28);
			this._cbSymbolSizeAt1.Name = "_cbSymbolSizeAt1";
			this._cbSymbolSizeAt1.Size = new System.Drawing.Size(121, 19);
			this._cbSymbolSizeAt1.TabIndex = 13;
			//
			// _cbSymbolSizeBelow
			//
			this._cbSymbolSizeBelow.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._cbSymbolSizeBelow.FormattingEnabled = true;
			this._cbSymbolSizeBelow.ItemHeight = 13;
			this._cbSymbolSizeBelow.Location = new System.Drawing.Point(107, 53);
			this._cbSymbolSizeBelow.Name = "_cbSymbolSizeBelow";
			this._cbSymbolSizeBelow.Size = new System.Drawing.Size(121, 19);
			this._cbSymbolSizeBelow.TabIndex = 14;
			//
			// _cbSymbolSizeAbove
			//
			this._cbSymbolSizeAbove.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._cbSymbolSizeAbove.FormattingEnabled = true;
			this._cbSymbolSizeAbove.ItemHeight = 13;
			this._cbSymbolSizeAbove.Location = new System.Drawing.Point(107, 78);
			this._cbSymbolSizeAbove.Name = "_cbSymbolSizeAbove";
			this._cbSymbolSizeAbove.Size = new System.Drawing.Size(121, 19);
			this._cbSymbolSizeAbove.TabIndex = 15;
			//
			// _cbSymbolSizeInvalid
			//
			this._cbSymbolSizeInvalid.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._cbSymbolSizeInvalid.FormattingEnabled = true;
			this._cbSymbolSizeInvalid.ItemHeight = 13;
			this._cbSymbolSizeInvalid.Location = new System.Drawing.Point(107, 103);
			this._cbSymbolSizeInvalid.Name = "_cbSymbolSizeInvalid";
			this._cbSymbolSizeInvalid.Size = new System.Drawing.Size(121, 19);
			this._cbSymbolSizeInvalid.TabIndex = 16;
			//
			// _ctrlScale
			//
			this._ctrlScale.AutoSize = true;
			this._ctrlScale.Location = new System.Drawing.Point(6, 19);
			this._ctrlScale.Name = "_ctrlScale";
			this._ctrlScale.Size = new System.Drawing.Size(322, 50);
			this._ctrlScale.TabIndex = 0;
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
			this.flowLayoutPanel1.Controls.Add(this.tableLayoutPanel1);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(446, 391);
			this.flowLayoutPanel1.TabIndex = 61;
			//
			// ColumnDrivenSymbolSizePlotStyleControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "ColumnDrivenSymbolSizePlotStyleControl";
			this.Size = new System.Drawing.Size(446, 391);
			this._grpScale.ResumeLayout(false);
			this._grpScale.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._edNumberOfSteps)).EndInit();
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
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown _edNumberOfSteps;
		private Common.Drawing.SingleSizeComboBox _cbSymbolSizeAt0;
		private Common.Drawing.SingleSizeComboBox _cbSymbolSizeAt1;
		private Common.Drawing.SingleSizeComboBox _cbSymbolSizeBelow;
		private Common.Drawing.SingleSizeComboBox _cbSymbolSizeAbove;
		private Common.Drawing.SingleSizeComboBox _cbSymbolSizeInvalid;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
	}
}
