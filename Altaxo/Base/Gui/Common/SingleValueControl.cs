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

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Summary description for SingleValueControl.
  /// </summary>
  public class SingleValueControl : System.Windows.Forms.UserControl, ISingleValueView
  {
    private System.Windows.Forms.TextBox m_edEdit;
    private System.Windows.Forms.Label m_Label1;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public SingleValueControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if(components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

    #region Component Designer generated code
    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.m_edEdit = new System.Windows.Forms.TextBox();
      this.m_Label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // m_edEdit
      // 
      this.m_edEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.m_edEdit.Location = new System.Drawing.Point(8, 32);
      this.m_edEdit.Name = "m_edEdit";
      this.m_edEdit.Size = new System.Drawing.Size(248, 20);
      this.m_edEdit.TabIndex = 3;
      this.m_edEdit.Text = "";
      this.m_edEdit.Validating += new System.ComponentModel.CancelEventHandler(this.m_edEdit_Validating);
      // 
      // m_Label1
      // 
      this.m_Label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.m_Label1.Location = new System.Drawing.Point(8, 8);
      this.m_Label1.Name = "m_Label1";
      this.m_Label1.Size = new System.Drawing.Size(240, 16);
      this.m_Label1.TabIndex = 2;
      this.m_Label1.Text = "Please enter :";
      this.m_Label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      // 
      // SingleValueControl
      // 
      this.Controls.Add(this.m_edEdit);
      this.Controls.Add(this.m_Label1);
      this.Name = "SingleValueControl";
      this.Size = new System.Drawing.Size(264, 56);
      this.ResumeLayout(false);

    }
    #endregion

    #region ISingleValueView Members

    public string DescriptionText
    {
      set
      {
        SizeF size1, size2;
        using (System.Drawing.Graphics grfx = this.CreateGraphics())
        {
          size1 = grfx.MeasureString(value, m_Label1.Font);
          size2 = grfx.MeasureString(value, m_Label1.Font, m_Label1.ClientSize.Width);
        }
        m_Label1.Size = new Size(m_edEdit.Size.Width, (int)(m_Label1.PreferredHeight * Math.Ceiling(size2.Height / size1.Height)));
        this.m_Label1.Text = value;

        this.m_edEdit.Location = new Point(m_Label1.Location.X, m_Label1.Bounds.Bottom + m_edEdit.Size.Height / 2);
        this.ClientSize = new Size(this.ClientSize.Width, m_edEdit.Bounds.Bottom);
      }
    }

    public string ValueText
    {
      get
      {
        return this.m_edEdit.Text;
      }
      set
      {
        this.m_edEdit.Text = value;
      }
    }

    #endregion

    public event System.ComponentModel.CancelEventHandler ValueText_Validating;
    private void m_edEdit_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (null != ValueText_Validating)
        ValueText_Validating(this, e);
    }
  }
}
