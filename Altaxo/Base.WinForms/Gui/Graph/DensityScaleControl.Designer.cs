namespace Altaxo.Gui.Graph
{
	partial class DensityScaleControl
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
			this._lblScale = new System.Windows.Forms.Label();
			this._cbScales = new System.Windows.Forms.ComboBox();
			this._tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.AutoSize = true;
			this._tableLayoutPanel.ColumnCount = 2;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.Controls.Add(this._lblScale, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._cbScales, 1, 0);
			this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 3;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.Size = new System.Drawing.Size(319, 27);
			this._tableLayoutPanel.TabIndex = 0;
			// 
			// _lblScale
			// 
			this._lblScale.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblScale.AutoSize = true;
			this._lblScale.Location = new System.Drawing.Point(3, 7);
			this._lblScale.Name = "_lblScale";
			this._lblScale.Size = new System.Drawing.Size(37, 13);
			this._lblScale.TabIndex = 0;
			this._lblScale.Text = "Scale:";
			this._lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _cbScales
			// 
			this._cbScales.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._cbScales.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cbScales.FormattingEnabled = true;
			this._cbScales.Location = new System.Drawing.Point(46, 3);
			this._cbScales.Name = "_cbScales";
			this._cbScales.Size = new System.Drawing.Size(270, 21);
			this._cbScales.TabIndex = 1;
			this._cbScales.SelectionChangeCommitted += new System.EventHandler(this.EhScaleSelectionChangeCommitted);
			// 
			// DensityScaleControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "DensityScaleControl";
			this.Size = new System.Drawing.Size(322, 30);
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private System.Windows.Forms.Label _lblScale;
		private System.Windows.Forms.ComboBox _cbScales;
	}
}
