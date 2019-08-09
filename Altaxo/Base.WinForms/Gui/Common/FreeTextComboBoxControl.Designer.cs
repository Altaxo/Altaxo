namespace Altaxo.Gui.Common
{
  partial class FreeTextComboBoxControl
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
			this._mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._lblDescription = new System.Windows.Forms.Label();
			this._cbChoice = new System.Windows.Forms.ComboBox();
			this._mainLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			//
			// _mainLayoutPanel
			//
			this._mainLayoutPanel.AutoSize = true;
			this._mainLayoutPanel.ColumnCount = 1;
			this._mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._mainLayoutPanel.Controls.Add(this._lblDescription, 0, 0);
			this._mainLayoutPanel.Controls.Add(this._cbChoice, 0, 1);
			this._mainLayoutPanel.Location = new System.Drawing.Point(3, 3);
			this._mainLayoutPanel.Name = "_mainLayoutPanel";
			this._mainLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this._mainLayoutPanel.RowCount = 2;
			this._mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._mainLayoutPanel.Size = new System.Drawing.Size(213, 55);
			this._mainLayoutPanel.TabIndex = 0;
			//
			// _lblDescription
			//
			this._lblDescription.AutoSize = true;
			this._lblDescription.Location = new System.Drawing.Point(3, 0);
			this._lblDescription.Name = "_lblDescription";
			this._lblDescription.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
			this._lblDescription.Size = new System.Drawing.Size(119, 23);
			this._lblDescription.TabIndex = 0;
			this._lblDescription.Text = "Please choose or enter:";
			//
			// _cbChoice
			//
			this._cbChoice.FormattingEnabled = true;
			this._cbChoice.Location = new System.Drawing.Point(3, 26);
			this._cbChoice.Name = "_cbChoice";
			this._cbChoice.Size = new System.Drawing.Size(207, 21);
			this._cbChoice.TabIndex = 1;
			this._cbChoice.SelectionChangeCommitted += new System.EventHandler(this.EhSelectionChangeCommitted);
			this._cbChoice.Validating += new System.ComponentModel.CancelEventHandler(this.EhValidating);
			//
			// FreeTextComboBoxControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._mainLayoutPanel);
			this.Name = "FreeTextComboBoxControl";
			this.Size = new System.Drawing.Size(219, 61);
			this._mainLayoutPanel.ResumeLayout(false);
			this._mainLayoutPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel _mainLayoutPanel;
    private System.Windows.Forms.Label _lblDescription;
    private System.Windows.Forms.ComboBox _cbChoice;
  }
}
