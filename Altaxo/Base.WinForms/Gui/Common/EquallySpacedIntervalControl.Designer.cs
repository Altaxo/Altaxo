namespace Altaxo.Gui.Common
{
	partial class EquallySpacedIntervalControl
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
			this._lblStart = new System.Windows.Forms.Label();
			this._lblEnd = new System.Windows.Forms.Label();
			this._edStart = new System.Windows.Forms.TextBox();
			this._edEnd = new System.Windows.Forms.TextBox();
			this._rbStartCountInterval = new System.Windows.Forms.RadioButton();
			this._rbEndCountIntv = new System.Windows.Forms.RadioButton();
			this._rbStartEndCount = new System.Windows.Forms.RadioButton();
			this._rbStartEndIntv = new System.Windows.Forms.RadioButton();
			this._edCount = new System.Windows.Forms.TextBox();
			this._edIntv = new System.Windows.Forms.TextBox();
			this._lblCount = new System.Windows.Forms.Label();
			this._lblIntv = new System.Windows.Forms.Label();
			this._tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			//
			// _tableLayoutPanel
			//
			this._tableLayoutPanel.AutoSize = true;
			this._tableLayoutPanel.ColumnCount = 2;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.Controls.Add(this._lblStart, 0, 2);
			this._tableLayoutPanel.Controls.Add(this._lblEnd, 0, 3);
			this._tableLayoutPanel.Controls.Add(this._edStart, 1, 2);
			this._tableLayoutPanel.Controls.Add(this._edEnd, 1, 3);
			this._tableLayoutPanel.Controls.Add(this._rbStartCountInterval, 1, 0);
			this._tableLayoutPanel.Controls.Add(this._rbEndCountIntv, 1, 1);
			this._tableLayoutPanel.Controls.Add(this._rbStartEndCount, 0, 1);
			this._tableLayoutPanel.Controls.Add(this._rbStartEndIntv, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._edCount, 1, 5);
			this._tableLayoutPanel.Controls.Add(this._edIntv, 1, 4);
			this._tableLayoutPanel.Controls.Add(this._lblCount, 0, 5);
			this._tableLayoutPanel.Controls.Add(this._lblIntv, 0, 4);
			this._tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 6;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.Size = new System.Drawing.Size(226, 150);
			this._tableLayoutPanel.TabIndex = 0;
			//
			// _lblStart
			//
			this._lblStart.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblStart.AutoSize = true;
			this._lblStart.Location = new System.Drawing.Point(77, 52);
			this._lblStart.Name = "_lblStart";
			this._lblStart.Size = new System.Drawing.Size(32, 13);
			this._lblStart.TabIndex = 0;
			this._lblStart.Text = "Start:";
			//
			// _lblEnd
			//
			this._lblEnd.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblEnd.AutoSize = true;
			this._lblEnd.Location = new System.Drawing.Point(80, 78);
			this._lblEnd.Name = "_lblEnd";
			this._lblEnd.Size = new System.Drawing.Size(29, 13);
			this._lblEnd.TabIndex = 1;
			this._lblEnd.Text = "End:";
			//
			// _edStart
			//
			this._edStart.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._edStart.Location = new System.Drawing.Point(115, 49);
			this._edStart.Name = "_edStart";
			this._edStart.Size = new System.Drawing.Size(108, 20);
			this._edStart.TabIndex = 4;
			this._edStart.TextChanged += new System.EventHandler(this._edStart_TextChanged);
			//
			// _edEnd
			//
			this._edEnd.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._edEnd.Location = new System.Drawing.Point(115, 75);
			this._edEnd.Name = "_edEnd";
			this._edEnd.Size = new System.Drawing.Size(108, 20);
			this._edEnd.TabIndex = 5;
			this._edEnd.TextChanged += new System.EventHandler(this._edEnd_TextChanged);
			//
			// _rbStartCountInterval
			//
			this._rbStartCountInterval.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._rbStartCountInterval.AutoSize = true;
			this._rbStartCountInterval.Location = new System.Drawing.Point(115, 3);
			this._rbStartCountInterval.Name = "_rbStartCountInterval";
			this._rbStartCountInterval.Size = new System.Drawing.Size(108, 17);
			this._rbStartCountInterval.TabIndex = 9;
			this._rbStartCountInterval.TabStop = true;
			this._rbStartCountInterval.Text = "Start, Count, Intv.";
			this._rbStartCountInterval.UseVisualStyleBackColor = true;
			this._rbStartCountInterval.CheckedChanged += new System.EventHandler(this._rbStartEndCount_CheckedChanged);
			//
			// _rbEndCountIntv
			//
			this._rbEndCountIntv.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._rbEndCountIntv.AutoSize = true;
			this._rbEndCountIntv.Location = new System.Drawing.Point(115, 26);
			this._rbEndCountIntv.Name = "_rbEndCountIntv";
			this._rbEndCountIntv.Size = new System.Drawing.Size(105, 17);
			this._rbEndCountIntv.TabIndex = 11;
			this._rbEndCountIntv.TabStop = true;
			this._rbEndCountIntv.Text = "End, Count, Intv.";
			this._rbEndCountIntv.UseVisualStyleBackColor = true;
			this._rbEndCountIntv.CheckedChanged += new System.EventHandler(this._rbStartEndCount_CheckedChanged);
			//
			// _rbStartEndCount
			//
			this._rbStartEndCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._rbStartEndCount.AutoSize = true;
			this._rbStartEndCount.Location = new System.Drawing.Point(3, 26);
			this._rbStartEndCount.Name = "_rbStartEndCount";
			this._rbStartEndCount.Size = new System.Drawing.Size(106, 17);
			this._rbStartEndCount.TabIndex = 8;
			this._rbStartEndCount.TabStop = true;
			this._rbStartEndCount.Text = "Start, End, Count";
			this._rbStartEndCount.UseVisualStyleBackColor = true;
			this._rbStartEndCount.CheckedChanged += new System.EventHandler(this._rbStartEndCount_CheckedChanged);
			//
			// _rbStartEndIntv
			//
			this._rbStartEndIntv.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._rbStartEndIntv.AutoSize = true;
			this._rbStartEndIntv.Location = new System.Drawing.Point(3, 3);
			this._rbStartEndIntv.Name = "_rbStartEndIntv";
			this._rbStartEndIntv.Size = new System.Drawing.Size(99, 17);
			this._rbStartEndIntv.TabIndex = 10;
			this._rbStartEndIntv.TabStop = true;
			this._rbStartEndIntv.Text = "Start, End, Intv.";
			this._rbStartEndIntv.UseVisualStyleBackColor = true;
			this._rbStartEndIntv.CheckedChanged += new System.EventHandler(this._rbStartEndCount_CheckedChanged);
			//
			// _edCount
			//
			this._edCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._edCount.Location = new System.Drawing.Point(115, 127);
			this._edCount.Name = "_edCount";
			this._edCount.Size = new System.Drawing.Size(108, 20);
			this._edCount.TabIndex = 6;
			this._edCount.TextChanged += new System.EventHandler(this._edCount_TextChanged);
			this._edCount.Validating += new System.ComponentModel.CancelEventHandler(this._edCount_Validating);
			//
			// _edIntv
			//
			this._edIntv.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._edIntv.Location = new System.Drawing.Point(115, 101);
			this._edIntv.Name = "_edIntv";
			this._edIntv.Size = new System.Drawing.Size(108, 20);
			this._edIntv.TabIndex = 7;
			this._edIntv.TextChanged += new System.EventHandler(this._edIntv_TextChanged);
			this._edIntv.Validating += new System.ComponentModel.CancelEventHandler(this._edIntv_Validating);
			//
			// _lblCount
			//
			this._lblCount.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblCount.AutoSize = true;
			this._lblCount.Location = new System.Drawing.Point(71, 130);
			this._lblCount.Name = "_lblCount";
			this._lblCount.Size = new System.Drawing.Size(38, 13);
			this._lblCount.TabIndex = 2;
			this._lblCount.Text = "Count:";
			//
			// _lblIntv
			//
			this._lblIntv.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblIntv.AutoSize = true;
			this._lblIntv.Location = new System.Drawing.Point(64, 104);
			this._lblIntv.Name = "_lblIntv";
			this._lblIntv.Size = new System.Drawing.Size(45, 13);
			this._lblIntv.TabIndex = 3;
			this._lblIntv.Text = "Interval:";
			//
			// EquallySpacedIntervalControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "EquallySpacedIntervalControl";
			this.Size = new System.Drawing.Size(232, 156);
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private System.Windows.Forms.Label _lblStart;
		private System.Windows.Forms.Label _lblEnd;
		private System.Windows.Forms.Label _lblCount;
		private System.Windows.Forms.Label _lblIntv;
		private System.Windows.Forms.TextBox _edStart;
		private System.Windows.Forms.TextBox _edEnd;
		private System.Windows.Forms.TextBox _edCount;
		private System.Windows.Forms.TextBox _edIntv;
		private System.Windows.Forms.RadioButton _rbStartEndCount;
		private System.Windows.Forms.RadioButton _rbStartCountInterval;
		private System.Windows.Forms.RadioButton _rbStartEndIntv;
		private System.Windows.Forms.RadioButton _rbEndCountIntv;
	}
}
