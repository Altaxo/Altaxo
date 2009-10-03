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
  partial class G2DCartesicCSControl
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
      this._chkExchangeXY = new System.Windows.Forms.CheckBox();
      this._verticalPanel = new System.Windows.Forms.FlowLayoutPanel();
      this._chkXReverse = new System.Windows.Forms.CheckBox();
      this._chkYReverse = new System.Windows.Forms.CheckBox();
      this._verticalPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // _chkExchangeXY
      // 
      this._chkExchangeXY.AutoSize = true;
      this._chkExchangeXY.Location = new System.Drawing.Point(3, 3);
      this._chkExchangeXY.Name = "_chkExchangeXY";
      this._chkExchangeXY.Size = new System.Drawing.Size(157, 17);
      this._chkExchangeXY.TabIndex = 0;
      this._chkExchangeXY.Text = "Exchange X-Y (X is vertical)";
      this._chkExchangeXY.UseVisualStyleBackColor = true;
      // 
      // _verticalPanel
      // 
      this._verticalPanel.AutoSize = true;
      this._verticalPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._verticalPanel.Controls.Add(this._chkExchangeXY);
      this._verticalPanel.Controls.Add(this._chkXReverse);
      this._verticalPanel.Controls.Add(this._chkYReverse);
      this._verticalPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this._verticalPanel.Location = new System.Drawing.Point(3, 3);
      this._verticalPanel.Name = "_verticalPanel";
      this._verticalPanel.Size = new System.Drawing.Size(163, 69);
      this._verticalPanel.TabIndex = 1;
      // 
      // _chkXReverse
      // 
      this._chkXReverse.AutoSize = true;
      this._chkXReverse.Location = new System.Drawing.Point(3, 26);
      this._chkXReverse.Name = "_chkXReverse";
      this._chkXReverse.Size = new System.Drawing.Size(76, 17);
      this._chkXReverse.TabIndex = 1;
      this._chkXReverse.Text = "Reverse X";
      this._chkXReverse.UseVisualStyleBackColor = true;
      // 
      // _chkYReverse
      // 
      this._chkYReverse.AutoSize = true;
      this._chkYReverse.Location = new System.Drawing.Point(3, 49);
      this._chkYReverse.Name = "_chkYReverse";
      this._chkYReverse.Size = new System.Drawing.Size(76, 17);
      this._chkYReverse.TabIndex = 2;
      this._chkYReverse.Text = "Reverse Y";
      this._chkYReverse.UseVisualStyleBackColor = true;
      // 
      // G2DCartesicCSControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this._verticalPanel);
      this.Name = "G2DCartesicCSControl";
      this.Size = new System.Drawing.Size(169, 75);
      this._verticalPanel.ResumeLayout(false);
      this._verticalPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox _chkExchangeXY;
    private System.Windows.Forms.FlowLayoutPanel _verticalPanel;
    private System.Windows.Forms.CheckBox _chkXReverse;
    private System.Windows.Forms.CheckBox _chkYReverse;
  }
}
