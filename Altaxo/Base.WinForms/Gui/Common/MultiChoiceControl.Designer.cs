namespace Altaxo.Gui.Common
{
	partial class MultiChoiceControl
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
			this._edDescription = new System.Windows.Forms.TextBox();
			this._lvItems = new System.Windows.Forms.ListView();
			this.SuspendLayout();
			// 
			// _edDescription
			// 
			this._edDescription.Dock = System.Windows.Forms.DockStyle.Top;
			this._edDescription.Location = new System.Drawing.Point(0, 0);
			this._edDescription.Multiline = true;
			this._edDescription.Name = "_edDescription";
			this._edDescription.ReadOnly = true;
			this._edDescription.Size = new System.Drawing.Size(377, 40);
			this._edDescription.TabIndex = 0;
			this._edDescription.Text = "Items:";
			// 
			// _lvItems
			// 
			this._lvItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lvItems.Location = new System.Drawing.Point(0, 40);
			this._lvItems.Name = "_lvItems";
			this._lvItems.Size = new System.Drawing.Size(377, 405);
			this._lvItems.TabIndex = 1;
			this._lvItems.UseCompatibleStateImageBehavior = false;
			this._lvItems.View = System.Windows.Forms.View.Details;
			this._lvItems.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.EhList_IndexSelectionChanged);
			// 
			// MultiChoiceControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._lvItems);
			this.Controls.Add(this._edDescription);
			this.Name = "MultiChoiceControl";
			this.Size = new System.Drawing.Size(377, 445);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _edDescription;
		private System.Windows.Forms.ListView _lvItems;
	}
}
