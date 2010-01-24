namespace Altaxo.Gui.Pads.ProjectBrowser
{
  partial class ProjectBrowseControl
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
			this._treeView = new System.Windows.Forms.TreeView();
			this._listView = new System.Windows.Forms.ListView();
			this._colName = new System.Windows.Forms.ColumnHeader();
			this._splitContainer = new System.Windows.Forms.SplitContainer();
			this._splitContainer.Panel1.SuspendLayout();
			this._splitContainer.Panel2.SuspendLayout();
			this._splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// _treeView
			// 
			this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._treeView.HideSelection = false;
			this._treeView.Location = new System.Drawing.Point(0, 0);
			this._treeView.Name = "_treeView";
			this._treeView.Size = new System.Drawing.Size(352, 204);
			this._treeView.TabIndex = 0;
			// 
			// _listView
			// 
			this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._colName});
			this._listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listView.Location = new System.Drawing.Point(0, 0);
			this._listView.Name = "_listView";
			this._listView.ShowItemToolTips = true;
			this._listView.Size = new System.Drawing.Size(352, 253);
			this._listView.TabIndex = 1;
			this._listView.UseCompatibleStateImageBehavior = false;
			this._listView.View = System.Windows.Forms.View.Details;
			this._listView.DoubleClick += new System.EventHandler(this.EhListViewItemDoubleClick);
			this._listView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.EhListViewItemSelecctionChanged);
			// 
			// _colName
			// 
			this._colName.Text = "Name";
			this._colName.Width = 141;
			// 
			// _splitContainer
			// 
			this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitContainer.Location = new System.Drawing.Point(0, 0);
			this._splitContainer.Name = "_splitContainer";
			this._splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _splitContainer.Panel1
			// 
			this._splitContainer.Panel1.Controls.Add(this._treeView);
			// 
			// _splitContainer.Panel2
			// 
			this._splitContainer.Panel2.Controls.Add(this._listView);
			this._splitContainer.Size = new System.Drawing.Size(352, 461);
			this._splitContainer.SplitterDistance = 204;
			this._splitContainer.TabIndex = 2;
			// 
			// ProjectBrowseControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._splitContainer);
			this.Name = "ProjectBrowseControl";
			this.Size = new System.Drawing.Size(352, 461);
			this._splitContainer.Panel1.ResumeLayout(false);
			this._splitContainer.Panel2.ResumeLayout(false);
			this._splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TreeView _treeView;
    private System.Windows.Forms.ListView _listView;
    private System.Windows.Forms.SplitContainer _splitContainer;
    private System.Windows.Forms.ColumnHeader _colName;
  }
}
