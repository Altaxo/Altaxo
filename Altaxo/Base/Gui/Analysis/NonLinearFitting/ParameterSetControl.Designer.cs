namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  partial class ParameterSetControl
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
      this._grid = new System.Windows.Forms.DataGridView();
      this.NameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ValueCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.VaryCol = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.VarianceCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
      this.SuspendLayout();
      // 
      // _grid
      // 
      this._grid.AllowUserToAddRows = false;
      this._grid.AllowUserToDeleteRows = false;
      this._grid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this._grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameCol,
            this.ValueCol,
            this.VaryCol,
            this.VarianceCol});
      this._grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
      this._grid.Location = new System.Drawing.Point(0, 0);
      this._grid.Name = "_grid";
      this._grid.RowHeadersVisible = false;
      this._grid.Size = new System.Drawing.Size(422, 274);
      this._grid.TabIndex = 0;
      // 
      // NameCol
      // 
      this.NameCol.HeaderText = "Parameter";
      this.NameCol.Name = "NameCol";
      this.NameCol.ReadOnly = true;
      this.NameCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.NameCol.ToolTipText = "Name of the parameter";
      // 
      // ValueCol
      // 
      this.ValueCol.HeaderText = "Value";
      this.ValueCol.Name = "ValueCol";
      this.ValueCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.ValueCol.ToolTipText = "Value of the parameter";
      this.ValueCol.Width = 130;
      // 
      // VaryCol
      // 
      this.VaryCol.HeaderText = "Vary";
      this.VaryCol.Name = "VaryCol";
      this.VaryCol.ToolTipText = "Indicates if the parameter should be varied";
      this.VaryCol.Width = 40;
      // 
      // VarianceCol
      // 
      this.VarianceCol.HeaderText = "Variance";
      this.VarianceCol.Name = "VarianceCol";
      this.VarianceCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.VarianceCol.ToolTipText = "Variance of the parameter value";
      this.VarianceCol.Width = 130;
      // 
      // ParameterSetControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._grid);
      this.Name = "ParameterSetControl";
      this.Size = new System.Drawing.Size(425, 274);
      ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView _grid;
    private System.Windows.Forms.DataGridViewTextBoxColumn _nameCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn _valueCol;
    private System.Windows.Forms.DataGridViewCheckBoxColumn _varyCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn _varianceCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn NameCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn ValueCol;
    private System.Windows.Forms.DataGridViewCheckBoxColumn VaryCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn VarianceCol;
  }
}
