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
  partial class FunctionPlotDataControl
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
      this._edText = new System.Windows.Forms.TextBox();
      this._btEditText = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // _edText
      // 
      this._edText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._edText.Location = new System.Drawing.Point(3, 3);
      this._edText.Multiline = true;
      this._edText.Name = "_edText";
      this._edText.ReadOnly = true;
      this._edText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this._edText.Size = new System.Drawing.Size(320, 299);
      this._edText.TabIndex = 0;
      this._edText.WordWrap = false;
      // 
      // _btEditText
      // 
      this._btEditText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btEditText.Location = new System.Drawing.Point(3, 308);
      this._btEditText.Name = "_btEditText";
      this._btEditText.Size = new System.Drawing.Size(75, 23);
      this._btEditText.TabIndex = 1;
      this._btEditText.Text = "Edit ..";
      this._btEditText.UseVisualStyleBackColor = true;
      this._btEditText.Click += new System.EventHandler(this.EhEditText_Click);
      // 
      // FunctionPlotDataControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._btEditText);
      this.Controls.Add(this._edText);
      this.Name = "FunctionPlotDataControl";
      this.Size = new System.Drawing.Size(326, 334);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox _edText;
    private System.Windows.Forms.Button _btEditText;
  }
}
