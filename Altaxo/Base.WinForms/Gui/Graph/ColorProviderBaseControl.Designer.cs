namespace Altaxo.Gui.Graph
{
	partial class ColorProviderBaseControl
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
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._lblColorBelow = new System.Windows.Forms.Label();
			this._lblColorAbove = new System.Windows.Forms.Label();
			this._lblColorInvalid = new System.Windows.Forms.Label();
			this._lblTransparency = new System.Windows.Forms.Label();
			this._edTransparency = new System.Windows.Forms.NumericUpDown();
			this._cbColorBelow = new Altaxo.Gui.Common.Drawing.ColorComboBox();
			this._cbColorAbove = new Altaxo.Gui.Common.Drawing.ColorComboBox();
			this._cbInvalid = new Altaxo.Gui.Common.Drawing.ColorComboBox();
			this._tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._edTransparency)).BeginInit();
			this.SuspendLayout();
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.AutoSize = true;
			this._tableLayoutPanel.ColumnCount = 2;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.Controls.Add(this._lblColorBelow, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._lblColorAbove, 0, 1);
			this._tableLayoutPanel.Controls.Add(this._lblColorInvalid, 0, 2);
			this._tableLayoutPanel.Controls.Add(this._lblTransparency, 0, 3);
			this._tableLayoutPanel.Controls.Add(this._edTransparency, 1, 3);
			this._tableLayoutPanel.Controls.Add(this._cbColorBelow, 1, 0);
			this._tableLayoutPanel.Controls.Add(this._cbColorAbove, 1, 1);
			this._tableLayoutPanel.Controls.Add(this._cbInvalid, 1, 2);
			this._tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 4;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.Size = new System.Drawing.Size(238, 101);
			this._tableLayoutPanel.TabIndex = 0;
			// 
			// _lblColorBelow
			// 
			this._lblColorBelow.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblColorBelow.AutoSize = true;
			this._lblColorBelow.Location = new System.Drawing.Point(43, 6);
			this._lblColorBelow.Name = "_lblColorBelow";
			this._lblColorBelow.Size = new System.Drawing.Size(65, 13);
			this._lblColorBelow.TabIndex = 0;
			this._lblColorBelow.Text = "Color below:";
			// 
			// _lblColorAbove
			// 
			this._lblColorAbove.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblColorAbove.AutoSize = true;
			this._lblColorAbove.Location = new System.Drawing.Point(41, 31);
			this._lblColorAbove.Name = "_lblColorAbove";
			this._lblColorAbove.Size = new System.Drawing.Size(67, 13);
			this._lblColorAbove.TabIndex = 1;
			this._lblColorAbove.Text = "Color above:";
			// 
			// _lblColorInvalid
			// 
			this._lblColorInvalid.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblColorInvalid.AutoSize = true;
			this._lblColorInvalid.Location = new System.Drawing.Point(3, 56);
			this._lblColorInvalid.Name = "_lblColorInvalid";
			this._lblColorInvalid.Size = new System.Drawing.Size(105, 13);
			this._lblColorInvalid.TabIndex = 2;
			this._lblColorInvalid.Text = "Color of invalid point:";
			// 
			// _lblTransparency
			// 
			this._lblTransparency.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblTransparency.AutoSize = true;
			this._lblTransparency.Location = new System.Drawing.Point(16, 81);
			this._lblTransparency.Name = "_lblTransparency";
			this._lblTransparency.Size = new System.Drawing.Size(92, 13);
			this._lblTransparency.TabIndex = 3;
			this._lblTransparency.Text = "Transparency (%):";
			// 
			// _edTransparency
			// 
			this._edTransparency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._edTransparency.Location = new System.Drawing.Point(114, 78);
			this._edTransparency.Name = "_edTransparency";
			this._edTransparency.Size = new System.Drawing.Size(121, 20);
			this._edTransparency.TabIndex = 4;
			// 
			// _cbColorBelow
			// 
			this._cbColorBelow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._cbColorBelow.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
			this._cbColorBelow.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._cbColorBelow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cbColorBelow.FormattingEnabled = true;
			this._cbColorBelow.ItemHeight = 13;
			this._cbColorBelow.Location = new System.Drawing.Point(114, 3);
			this._cbColorBelow.Name = "_cbColorBelow";
			this._cbColorBelow.Size = new System.Drawing.Size(121, 19);
			this._cbColorBelow.TabIndex = 5;
			// 
			// _cbColorAbove
			// 
			this._cbColorAbove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._cbColorAbove.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
			this._cbColorAbove.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._cbColorAbove.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cbColorAbove.FormattingEnabled = true;
			this._cbColorAbove.ItemHeight = 13;
			this._cbColorAbove.Location = new System.Drawing.Point(114, 28);
			this._cbColorAbove.Name = "_cbColorAbove";
			this._cbColorAbove.Size = new System.Drawing.Size(121, 19);
			this._cbColorAbove.TabIndex = 6;
			// 
			// _cbInvalid
			// 
			this._cbInvalid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._cbInvalid.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
			this._cbInvalid.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._cbInvalid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cbInvalid.FormattingEnabled = true;
			this._cbInvalid.ItemHeight = 13;
			this._cbInvalid.Location = new System.Drawing.Point(114, 53);
			this._cbInvalid.Name = "_cbInvalid";
			this._cbInvalid.Size = new System.Drawing.Size(121, 19);
			this._cbInvalid.TabIndex = 7;
			// 
			// ColorProviderBaseControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "ColorProviderBaseControl";
			this.Size = new System.Drawing.Size(244, 107);
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._edTransparency)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private System.Windows.Forms.Label _lblColorBelow;
		private System.Windows.Forms.Label _lblColorAbove;
		private System.Windows.Forms.Label _lblColorInvalid;
		private System.Windows.Forms.Label _lblTransparency;
		private System.Windows.Forms.NumericUpDown _edTransparency;
		private Altaxo.Gui.Common.Drawing.ColorComboBox _cbColorBelow;
		private Altaxo.Gui.Common.Drawing.ColorComboBox _cbColorAbove;
		private Altaxo.Gui.Common.Drawing.ColorComboBox _cbInvalid;
	}
}
