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
  partial class ImageGraphicControl
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._grpbPosition = new System.Windows.Forms.GroupBox();
			this._ctrlPosSize = new Altaxo.Gui.Graph.PositionSizeRotationScaleControl();
			this.flowLayoutPanel1.SuspendLayout();
			this._grpbPosition.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._grpbPosition);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(356, 129);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// _grpbPosition
			// 
			this._grpbPosition.AutoSize = true;
			this._grpbPosition.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._grpbPosition.Controls.Add(this._ctrlPosSize);
			this._grpbPosition.Location = new System.Drawing.Point(3, 3);
			this._grpbPosition.Name = "_grpbPosition";
			this._grpbPosition.Size = new System.Drawing.Size(350, 123);
			this._grpbPosition.TabIndex = 2;
			this._grpbPosition.TabStop = false;
			this._grpbPosition.Text = "Position/Size";
			// 
			// _ctrlPosSize
			// 
			this._ctrlPosSize.AutoSize = true;
			this._ctrlPosSize.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._ctrlPosSize.Location = new System.Drawing.Point(6, 19);
			this._ctrlPosSize.Name = "_ctrlPosSize";
			this._ctrlPosSize.Size = new System.Drawing.Size(338, 85);
			this._ctrlPosSize.TabIndex = 0;
			// 
			// ImageGraphicControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "ImageGraphicControl";
			this.Size = new System.Drawing.Size(362, 135);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this._grpbPosition.ResumeLayout(false);
			this._grpbPosition.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.GroupBox _grpbPosition;
    private PositionSizeRotationScaleControl _ctrlPosSize;
  }
}
