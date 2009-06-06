namespace Altaxo.Gui.Graph
{
	partial class PrintableAreaSetupControl
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
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._edWidth = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this._edHeight = new System.Windows.Forms.TextBox();
			this._edXPos = new System.Windows.Forms.TextBox();
			this._edYPos = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._positionSizeGlue = new Altaxo.Gui.Graph.ObjectPositionAndSizeGlue();
			this._chkRescale = new System.Windows.Forms.CheckBox();
			this._tableLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayout
			// 
			this._tableLayout.AutoSize = true;
			this._tableLayout.ColumnCount = 2;
			this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayout.Controls.Add(this.label4, 0, 3);
			this._tableLayout.Controls.Add(this.label2, 0, 1);
			this._tableLayout.Controls.Add(this._edWidth, 1, 0);
			this._tableLayout.Controls.Add(this.label1, 0, 0);
			this._tableLayout.Controls.Add(this._edHeight, 1, 1);
			this._tableLayout.Controls.Add(this._edXPos, 1, 2);
			this._tableLayout.Controls.Add(this._edYPos, 1, 3);
			this._tableLayout.Controls.Add(this.label3, 0, 2);
			this._tableLayout.Controls.Add(this._chkRescale, 1, 4);
			this._tableLayout.Location = new System.Drawing.Point(3, 3);
			this._tableLayout.Name = "_tableLayout";
			this._tableLayout.RowCount = 5;
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayout.Size = new System.Drawing.Size(185, 127);
			this._tableLayout.TabIndex = 0;
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 84);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(38, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Y Pos:";
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Height:";
			// 
			// _edWidth
			// 
			this._edWidth.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._edWidth.Location = new System.Drawing.Point(50, 3);
			this._edWidth.Name = "_edWidth";
			this._edWidth.Size = new System.Drawing.Size(132, 20);
			this._edWidth.TabIndex = 0;
			this._edWidth.Text = "0 pt";
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Width:";
			// 
			// _edHeight
			// 
			this._edHeight.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._edHeight.Location = new System.Drawing.Point(50, 29);
			this._edHeight.Name = "_edHeight";
			this._edHeight.Size = new System.Drawing.Size(132, 20);
			this._edHeight.TabIndex = 5;
			this._edHeight.Text = "0 pt";
			// 
			// _edXPos
			// 
			this._edXPos.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._edXPos.Location = new System.Drawing.Point(50, 55);
			this._edXPos.Name = "_edXPos";
			this._edXPos.Size = new System.Drawing.Size(132, 20);
			this._edXPos.TabIndex = 6;
			this._edXPos.Text = "0 pt";
			// 
			// _edYPos
			// 
			this._edYPos.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._edYPos.Location = new System.Drawing.Point(50, 81);
			this._edYPos.Name = "_edYPos";
			this._edYPos.Size = new System.Drawing.Size(132, 20);
			this._edYPos.TabIndex = 7;
			this._edYPos.Text = "0 pt";
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 58);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(38, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "X Pos:";
			// 
			// _positionSizeGlue
			// 
			this._positionSizeGlue.CbRotation = null;
			this._positionSizeGlue.EdPositionX = this._edXPos;
			this._positionSizeGlue.EdPositionY = this._edYPos;
			this._positionSizeGlue.EdSizeX = this._edWidth;
			this._positionSizeGlue.EdSizeY = this._edHeight;
			// 
			// _chkRescale
			// 
			this._chkRescale.AutoSize = true;
			this._chkRescale.Location = new System.Drawing.Point(50, 107);
			this._chkRescale.Name = "_chkRescale";
			this._chkRescale.Size = new System.Drawing.Size(65, 17);
			this._chkRescale.TabIndex = 10;
			this._chkRescale.Text = "Rescale";
			this._chkRescale.UseVisualStyleBackColor = true;
			// 
			// PrintableAreaSetupControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._tableLayout);
			this.Name = "PrintableAreaSetupControl";
			this.Size = new System.Drawing.Size(191, 133);
			this._tableLayout.ResumeLayout(false);
			this._tableLayout.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ObjectPositionAndSizeGlue _positionSizeGlue;
		private System.Windows.Forms.TableLayoutPanel _tableLayout;
		private System.Windows.Forms.TextBox _edWidth;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _edHeight;
		private System.Windows.Forms.TextBox _edXPos;
		private System.Windows.Forms.TextBox _edYPos;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox _chkRescale;
	}
}
