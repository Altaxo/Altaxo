namespace Altaxo.Gui.Graph.Scales
{
  partial class AngularScaleControl
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
      this._grpUnit = new System.Windows.Forms.GroupBox();
      this._rbRadian = new System.Windows.Forms.RadioButton();
      this._rbDegree = new System.Windows.Forms.RadioButton();
      this._lblOrigin = new System.Windows.Forms.Label();
      this._cbOrigin = new System.Windows.Forms.ComboBox();
      this._lblMajorTicks = new System.Windows.Forms.Label();
      this._cbMajorTicks = new System.Windows.Forms.ComboBox();
      this._lblMinorTicks = new System.Windows.Forms.Label();
      this._cbMinorTicks = new System.Windows.Forms.ComboBox();
      this._chkPosNegValues = new System.Windows.Forms.CheckBox();
      this._grpUnit.SuspendLayout();
      this.SuspendLayout();
      // 
      // _grpUnit
      // 
      this._grpUnit.Controls.Add(this._rbRadian);
      this._grpUnit.Controls.Add(this._rbDegree);
      this._grpUnit.Location = new System.Drawing.Point(3, 3);
      this._grpUnit.Name = "_grpUnit";
      this._grpUnit.Size = new System.Drawing.Size(100, 47);
      this._grpUnit.TabIndex = 0;
      this._grpUnit.TabStop = false;
      this._grpUnit.Text = "Unit";
      // 
      // _rbRadian
      // 
      this._rbRadian.AutoSize = true;
      this._rbRadian.Location = new System.Drawing.Point(55, 19);
      this._rbRadian.Name = "_rbRadian";
      this._rbRadian.Size = new System.Drawing.Size(40, 17);
      this._rbRadian.TabIndex = 1;
      this._rbRadian.TabStop = true;
      this._rbRadian.Text = "rad";
      this._rbRadian.UseVisualStyleBackColor = true;
      // 
      // _rbDegree
      // 
      this._rbDegree.AutoSize = true;
      this._rbDegree.Location = new System.Drawing.Point(6, 19);
      this._rbDegree.Name = "_rbDegree";
      this._rbDegree.Size = new System.Drawing.Size(43, 17);
      this._rbDegree.TabIndex = 0;
      this._rbDegree.TabStop = true;
      this._rbDegree.Text = "deg";
      this._rbDegree.UseVisualStyleBackColor = true;
      // 
      // _lblOrigin
      // 
      this._lblOrigin.AutoSize = true;
      this._lblOrigin.Location = new System.Drawing.Point(32, 56);
      this._lblOrigin.Name = "_lblOrigin";
      this._lblOrigin.Size = new System.Drawing.Size(37, 13);
      this._lblOrigin.TabIndex = 1;
      this._lblOrigin.Text = "Origin:";
      // 
      // _cbOrigin
      // 
      this._cbOrigin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbOrigin.FormattingEnabled = true;
      this._cbOrigin.Location = new System.Drawing.Point(71, 53);
      this._cbOrigin.Name = "_cbOrigin";
      this._cbOrigin.Size = new System.Drawing.Size(121, 21);
      this._cbOrigin.TabIndex = 2;
      this._cbOrigin.SelectionChangeCommitted += new System.EventHandler(this._cbOrigin_SelectionChangeCommitted);
      // 
      // _lblMajorTicks
      // 
      this._lblMajorTicks.AutoSize = true;
      this._lblMajorTicks.Location = new System.Drawing.Point(8, 83);
      this._lblMajorTicks.Name = "_lblMajorTicks";
      this._lblMajorTicks.Size = new System.Drawing.Size(61, 13);
      this._lblMajorTicks.TabIndex = 3;
      this._lblMajorTicks.Text = "Major ticks:";
      // 
      // _cbMajorTicks
      // 
      this._cbMajorTicks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbMajorTicks.FormattingEnabled = true;
      this._cbMajorTicks.Location = new System.Drawing.Point(71, 80);
      this._cbMajorTicks.Name = "_cbMajorTicks";
      this._cbMajorTicks.Size = new System.Drawing.Size(121, 21);
      this._cbMajorTicks.TabIndex = 4;
      this._cbMajorTicks.SelectedIndexChanged += new System.EventHandler(this._cbMajorTicks_SelectedIndexChanged);
      // 
      // _lblMinorTicks
      // 
      this._lblMinorTicks.AutoSize = true;
      this._lblMinorTicks.Location = new System.Drawing.Point(8, 110);
      this._lblMinorTicks.Name = "_lblMinorTicks";
      this._lblMinorTicks.Size = new System.Drawing.Size(61, 13);
      this._lblMinorTicks.TabIndex = 5;
      this._lblMinorTicks.Text = "Minor ticks:";
      // 
      // _cbMinorTicks
      // 
      this._cbMinorTicks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbMinorTicks.FormattingEnabled = true;
      this._cbMinorTicks.Location = new System.Drawing.Point(71, 107);
      this._cbMinorTicks.Name = "_cbMinorTicks";
      this._cbMinorTicks.Size = new System.Drawing.Size(121, 21);
      this._cbMinorTicks.TabIndex = 6;
      this._cbMinorTicks.SelectedIndexChanged += new System.EventHandler(this._cbMinorTicks_SelectedIndexChanged);
      // 
      // _chkPosNegValues
      // 
      this._chkPosNegValues.AutoSize = true;
      this._chkPosNegValues.Location = new System.Drawing.Point(109, 22);
      this._chkPosNegValues.Name = "_chkPosNegValues";
      this._chkPosNegValues.Size = new System.Drawing.Size(106, 17);
      this._chkPosNegValues.TabIndex = 7;
      this._chkPosNegValues.Text = "pos./neg. values";
      this._chkPosNegValues.UseVisualStyleBackColor = true;
      // 
      // AngularScaleControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._chkPosNegValues);
      this.Controls.Add(this._cbMinorTicks);
      this.Controls.Add(this._lblMinorTicks);
      this.Controls.Add(this._cbMajorTicks);
      this.Controls.Add(this._lblMajorTicks);
      this.Controls.Add(this._cbOrigin);
      this.Controls.Add(this._lblOrigin);
      this.Controls.Add(this._grpUnit);
      this.Name = "AngularScaleControl";
      this.Size = new System.Drawing.Size(220, 134);
      this._grpUnit.ResumeLayout(false);
      this._grpUnit.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox _grpUnit;
    private System.Windows.Forms.RadioButton _rbRadian;
    private System.Windows.Forms.RadioButton _rbDegree;
    private System.Windows.Forms.Label _lblOrigin;
    private System.Windows.Forms.ComboBox _cbOrigin;
    private System.Windows.Forms.Label _lblMajorTicks;
    private System.Windows.Forms.ComboBox _cbMajorTicks;
    private System.Windows.Forms.Label _lblMinorTicks;
    private System.Windows.Forms.ComboBox _cbMinorTicks;
    private System.Windows.Forms.CheckBox _chkPosNegValues;
  }
}
