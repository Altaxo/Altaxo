namespace Altaxo.Gui.Graph
{
	partial class FillToCurvePlotStyleControl
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._cbFillColor = new Altaxo.Gui.Common.Drawing.BrushColorComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this._chkFillPrevious = new System.Windows.Forms.CheckBox();
			this._chkFillNext = new System.Windows.Forms.CheckBox();
			this._tableLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayout
			// 
			this._tableLayout.AutoSize = true;
			this._tableLayout.ColumnCount = 2;
			this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayout.Controls.Add(this.label3, 0, 1);
			this._tableLayout.Controls.Add(this.label2, 0, 0);
			this._tableLayout.Controls.Add(this._chkFillPrevious, 1, 0);
			this._tableLayout.Controls.Add(this._chkFillNext, 1, 1);
			this._tableLayout.Controls.Add(this.label1, 0, 2);
			this._tableLayout.Controls.Add(this._cbFillColor, 1, 2);
			this._tableLayout.Location = new System.Drawing.Point(3, 3);
			this._tableLayout.Name = "_tableLayout";
			this._tableLayout.RowCount = 3;
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayout.Size = new System.Drawing.Size(206, 65);
			this._tableLayout.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(32, 46);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Fill color:";
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Fill to previous:";
			// 
			// _cbFillColor
			// 
			this._cbFillColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._cbFillColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
			this._cbFillColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._cbFillColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cbFillColor.FormattingEnabled = true;
			this._cbFillColor.ItemHeight = 13;
			this._cbFillColor.Location = new System.Drawing.Point(86, 43);
			this._cbFillColor.Name = "_cbFillColor";
			this._cbFillColor.Size = new System.Drawing.Size(117, 19);
			this._cbFillColor.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(23, 23);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(57, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Fill to next:";
			// 
			// _chkFillPrevious
			// 
			this._chkFillPrevious.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._chkFillPrevious.AutoSize = true;
			this._chkFillPrevious.Location = new System.Drawing.Point(86, 3);
			this._chkFillPrevious.Name = "_chkFillPrevious";
			this._chkFillPrevious.Size = new System.Drawing.Size(15, 14);
			this._chkFillPrevious.TabIndex = 4;
			this._chkFillPrevious.UseVisualStyleBackColor = true;
			// 
			// _chkFillNext
			// 
			this._chkFillNext.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._chkFillNext.AutoSize = true;
			this._chkFillNext.Location = new System.Drawing.Point(86, 23);
			this._chkFillNext.Name = "_chkFillNext";
			this._chkFillNext.Size = new System.Drawing.Size(15, 14);
			this._chkFillNext.TabIndex = 5;
			this._chkFillNext.UseVisualStyleBackColor = true;
			// 
			// FillToCurvePlotStyleControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._tableLayout);
			this.Name = "FillToCurvePlotStyleControl";
			this.Size = new System.Drawing.Size(212, 71);
			this._tableLayout.ResumeLayout(false);
			this._tableLayout.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayout;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private Altaxo.Gui.Common.Drawing.BrushColorComboBox _cbFillColor;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox _chkFillPrevious;
		private System.Windows.Forms.CheckBox _chkFillNext;
	}
}
