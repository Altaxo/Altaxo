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

namespace Altaxo.Gui.Common
{
  partial class TypeAndInstanceControl
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
      this._verticalPanel = new System.Windows.Forms.FlowLayoutPanel();
      this._horzizontalPanel = new System.Windows.Forms.FlowLayoutPanel();
      this._lblCSType = new System.Windows.Forms.Label();
      this._cbTypeChoice = new System.Windows.Forms.ComboBox();
      this._panelForSubControl = new System.Windows.Forms.Panel();
      this._verticalPanel.SuspendLayout();
      this._horzizontalPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // _verticalPanel
      // 
      this._verticalPanel.AutoSize = true;
      this._verticalPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._verticalPanel.Controls.Add(this._horzizontalPanel);
      this._verticalPanel.Controls.Add(this._panelForSubControl);
      this._verticalPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this._verticalPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this._verticalPanel.Location = new System.Drawing.Point(0, 0);
      this._verticalPanel.Name = "_verticalPanel";
      this._verticalPanel.Size = new System.Drawing.Size(242, 39);
      this._verticalPanel.TabIndex = 0;
      // 
      // _horzizontalPanel
      // 
      this._horzizontalPanel.AutoSize = true;
      this._horzizontalPanel.Controls.Add(this._lblCSType);
      this._horzizontalPanel.Controls.Add(this._cbTypeChoice);
      this._horzizontalPanel.Location = new System.Drawing.Point(3, 3);
      this._horzizontalPanel.Name = "_horzizontalPanel";
      this._horzizontalPanel.Size = new System.Drawing.Size(236, 27);
      this._horzizontalPanel.TabIndex = 0;
      // 
      // _lblCSType
      // 
      this._lblCSType.AutoSize = true;
      this._lblCSType.Location = new System.Drawing.Point(3, 0);
      this._lblCSType.Name = "_lblCSType";
      this._lblCSType.Size = new System.Drawing.Size(34, 13);
      this._lblCSType.TabIndex = 0;
      this._lblCSType.Text = "Type:";
      // 
      // _cbTypeChoice
      // 
      this._cbTypeChoice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._cbTypeChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbTypeChoice.FormattingEnabled = true;
      this._cbTypeChoice.Location = new System.Drawing.Point(43, 3);
      this._cbTypeChoice.Name = "_cbTypeChoice";
      this._cbTypeChoice.Size = new System.Drawing.Size(190, 21);
      this._cbTypeChoice.TabIndex = 1;
      this._cbTypeChoice.SelectionChangeCommitted += new System.EventHandler(this.EhSelectionChangeCommitted);
      // 
      // _panelForSubControl
      // 
      this._panelForSubControl.AutoSize = true;
      this._panelForSubControl.Location = new System.Drawing.Point(3, 36);
      this._panelForSubControl.Name = "_panelForSubControl";
      this._panelForSubControl.Size = new System.Drawing.Size(0, 0);
      this._panelForSubControl.TabIndex = 1;
      // 
      // TypeAndInstanceControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this._verticalPanel);
      this.Name = "TypeAndInstanceControl";
      this.Size = new System.Drawing.Size(242, 39);
      this._verticalPanel.ResumeLayout(false);
      this._verticalPanel.PerformLayout();
      this._horzizontalPanel.ResumeLayout(false);
      this._horzizontalPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel _verticalPanel;
    private System.Windows.Forms.FlowLayoutPanel _horzizontalPanel;
    private System.Windows.Forms.Label _lblCSType;
    private System.Windows.Forms.ComboBox _cbTypeChoice;
    private System.Windows.Forms.Panel _panelForSubControl;
  }
}
