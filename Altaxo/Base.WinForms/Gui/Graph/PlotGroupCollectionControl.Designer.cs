#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

namespace Altaxo.Gui.Graph
{
  partial class PlotGroupCollectionControl
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
      this._lbGroupStylesAvailable = new System.Windows.Forms.ListBox();
      this._lbGroupStyles = new System.Windows.Forms.CheckedListBox();
      this._btRemoveNormalGroupStyle = new System.Windows.Forms.Button();
      this._btAddNormalGroupStyle = new System.Windows.Forms.Button();
      this._btDown = new System.Windows.Forms.Button();
      this._btUp = new System.Windows.Forms.Button();
      this._btUnindent = new System.Windows.Forms.Button();
      this._btIndent = new System.Windows.Forms.Button();
      this._cbCoordTransfoStyle = new System.Windows.Forms.ComboBox();
      this._cbGroupStrictness = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this._btEditCSTransfoStyle = new System.Windows.Forms.Button();
      this._chkDistributeToSubGroups = new System.Windows.Forms.CheckBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this._chkUpdateFromParentGroups = new System.Windows.Forms.CheckBox();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // _lbGroupStylesAvailable
      // 
      this._lbGroupStylesAvailable.FormattingEnabled = true;
      this._lbGroupStylesAvailable.Location = new System.Drawing.Point(3, 28);
      this._lbGroupStylesAvailable.Name = "_lbGroupStylesAvailable";
      this._lbGroupStylesAvailable.Size = new System.Drawing.Size(138, 173);
      this._lbGroupStylesAvailable.TabIndex = 1;
      // 
      // _lbGroupStyles
      // 
      this._lbGroupStyles.FormattingEnabled = true;
      this._lbGroupStyles.Location = new System.Drawing.Point(170, 28);
      this._lbGroupStyles.Name = "_lbGroupStyles";
      this._lbGroupStyles.Size = new System.Drawing.Size(182, 169);
      this._lbGroupStyles.TabIndex = 3;
      // 
      // _btRemoveNormalGroupStyle
      // 
      this._btRemoveNormalGroupStyle.Location = new System.Drawing.Point(144, 28);
      this._btRemoveNormalGroupStyle.Name = "_btRemoveNormalGroupStyle";
      this._btRemoveNormalGroupStyle.Size = new System.Drawing.Size(20, 20);
      this._btRemoveNormalGroupStyle.TabIndex = 6;
      this._btRemoveNormalGroupStyle.Text = "<";
      this._btRemoveNormalGroupStyle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this._btRemoveNormalGroupStyle.UseVisualStyleBackColor = true;
      this._btRemoveNormalGroupStyle.Click += new System.EventHandler(this._btRemoveNormalGroupStyle_Click);
      // 
      // _btAddNormalGroupStyle
      // 
      this._btAddNormalGroupStyle.Location = new System.Drawing.Point(144, 54);
      this._btAddNormalGroupStyle.Name = "_btAddNormalGroupStyle";
      this._btAddNormalGroupStyle.Size = new System.Drawing.Size(20, 20);
      this._btAddNormalGroupStyle.TabIndex = 7;
      this._btAddNormalGroupStyle.Text = ">";
      this._btAddNormalGroupStyle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this._btAddNormalGroupStyle.UseVisualStyleBackColor = true;
      this._btAddNormalGroupStyle.Click += new System.EventHandler(this._btAddNormalGroupStyle_Click);
      // 
      // _btDown
      // 
      this._btDown.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
      this._btDown.Location = new System.Drawing.Point(358, 106);
      this._btDown.Name = "_btDown";
      this._btDown.Size = new System.Drawing.Size(20, 20);
      this._btDown.TabIndex = 8;
      this._btDown.Text = "â";
      this._btDown.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this._btDown.UseVisualStyleBackColor = true;
      this._btDown.Click += new System.EventHandler(this._btDown_Click);
      // 
      // _btUp
      // 
      this._btUp.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
      this._btUp.Location = new System.Drawing.Point(358, 80);
      this._btUp.Name = "_btUp";
      this._btUp.Size = new System.Drawing.Size(20, 20);
      this._btUp.TabIndex = 9;
      this._btUp.Text = "á";
      this._btUp.UseVisualStyleBackColor = true;
      this._btUp.Click += new System.EventHandler(this._btUp_Click);
      // 
      // _btUnindent
      // 
      this._btUnindent.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
      this._btUnindent.Location = new System.Drawing.Point(358, 54);
      this._btUnindent.Name = "_btUnindent";
      this._btUnindent.Size = new System.Drawing.Size(20, 20);
      this._btUnindent.TabIndex = 10;
      this._btUnindent.Text = "ß";
      this._btUnindent.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this._btUnindent.UseVisualStyleBackColor = true;
      this._btUnindent.Click += new System.EventHandler(this._btUnindent_Click);
      // 
      // _btIndent
      // 
      this._btIndent.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
      this._btIndent.Location = new System.Drawing.Point(358, 29);
      this._btIndent.Name = "_btIndent";
      this._btIndent.Size = new System.Drawing.Size(20, 20);
      this._btIndent.TabIndex = 11;
      this._btIndent.Text = "à";
      this._btIndent.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this._btIndent.UseVisualStyleBackColor = true;
      this._btIndent.Click += new System.EventHandler(this._btIndent_Click);
      // 
      // _cbCoordTransfoStyle
      // 
      this._cbCoordTransfoStyle.FormattingEnabled = true;
      this._cbCoordTransfoStyle.Location = new System.Drawing.Point(170, 1);
      this._cbCoordTransfoStyle.Name = "_cbCoordTransfoStyle";
      this._cbCoordTransfoStyle.Size = new System.Drawing.Size(179, 21);
      this._cbCoordTransfoStyle.TabIndex = 12;
      this._cbCoordTransfoStyle.SelectionChangeCommitted += new System.EventHandler(this._cbCoordTransfoStyle_SelectionChangeCommitted);
      // 
      // _cbGroupStrictness
      // 
      this._cbGroupStrictness.FormattingEnabled = true;
      this._cbGroupStrictness.Location = new System.Drawing.Point(6, 19);
      this._cbGroupStrictness.Name = "_cbGroupStrictness";
      this._cbGroupStrictness.Size = new System.Drawing.Size(132, 21);
      this._cbGroupStrictness.TabIndex = 13;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(19, 4);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(145, 13);
      this.label1.TabIndex = 15;
      this.label1.Text = "Coordinate transforming style:";
      // 
      // _btEditCSTransfoStyle
      // 
      this._btEditCSTransfoStyle.Location = new System.Drawing.Point(358, 1);
      this._btEditCSTransfoStyle.Name = "_btEditCSTransfoStyle";
      this._btEditCSTransfoStyle.Size = new System.Drawing.Size(20, 20);
      this._btEditCSTransfoStyle.TabIndex = 16;
      this._btEditCSTransfoStyle.Text = "..";
      this._btEditCSTransfoStyle.UseVisualStyleBackColor = true;
      this._btEditCSTransfoStyle.Click += new System.EventHandler(this._btEditCSTransfoStyle_Click);
      // 
      // _chkDistributeToSubGroups
      // 
      this._chkDistributeToSubGroups.AutoSize = true;
      this._chkDistributeToSubGroups.Location = new System.Drawing.Point(167, 32);
      this._chkDistributeToSubGroups.Name = "_chkDistributeToSubGroups";
      this._chkDistributeToSubGroups.Size = new System.Drawing.Size(140, 17);
      this._chkDistributeToSubGroups.TabIndex = 17;
      this._chkDistributeToSubGroups.Text = "distribute to child groups";
      this._chkDistributeToSubGroups.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this._chkUpdateFromParentGroups);
      this.groupBox1.Controls.Add(this._cbGroupStrictness);
      this.groupBox1.Controls.Add(this._chkDistributeToSubGroups);
      this.groupBox1.Location = new System.Drawing.Point(3, 207);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(349, 54);
      this.groupBox1.TabIndex = 18;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Update";
      // 
      // _chkUpdateFromParentGroups
      // 
      this._chkUpdateFromParentGroups.AutoSize = true;
      this._chkUpdateFromParentGroups.Location = new System.Drawing.Point(167, 9);
      this._chkUpdateFromParentGroups.Name = "_chkUpdateFromParentGroups";
      this._chkUpdateFromParentGroups.Size = new System.Drawing.Size(145, 17);
      this._chkUpdateFromParentGroups.TabIndex = 18;
      this._chkUpdateFromParentGroups.Text = "inherit from parent groups";
      this._chkUpdateFromParentGroups.UseVisualStyleBackColor = true;
      // 
      // PlotGroupCollectionControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this._btEditCSTransfoStyle);
      this.Controls.Add(this.label1);
      this.Controls.Add(this._cbCoordTransfoStyle);
      this.Controls.Add(this._btIndent);
      this.Controls.Add(this._btUnindent);
      this.Controls.Add(this._btUp);
      this.Controls.Add(this._btDown);
      this.Controls.Add(this._btAddNormalGroupStyle);
      this.Controls.Add(this._btRemoveNormalGroupStyle);
      this.Controls.Add(this._lbGroupStyles);
      this.Controls.Add(this._lbGroupStylesAvailable);
      this.Name = "PlotGroupCollectionControl";
      this.Size = new System.Drawing.Size(385, 267);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox _lbGroupStylesAvailable;
    private System.Windows.Forms.CheckedListBox _lbGroupStyles;
    private System.Windows.Forms.Button _btRemoveNormalGroupStyle;
    private System.Windows.Forms.Button _btAddNormalGroupStyle;
    private System.Windows.Forms.Button _btDown;
    private System.Windows.Forms.Button _btUp;
    private System.Windows.Forms.Button _btUnindent;
    private System.Windows.Forms.Button _btIndent;
    private System.Windows.Forms.ComboBox _cbCoordTransfoStyle;
    private System.Windows.Forms.ComboBox _cbGroupStrictness;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button _btEditCSTransfoStyle;
    private System.Windows.Forms.CheckBox _chkDistributeToSubGroups;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.CheckBox _chkUpdateFromParentGroups;
  }
}
