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
  [UserControlForController(typeof(ISingleChoiceViewEventSink))]
  public class SingleChoiceComboBoxControl : System.Windows.Forms.UserControl, ISingleChoiceView
  {
    private System.Windows.Forms.Label m_Label1;
    private System.Windows.Forms.ComboBox _cbChoice;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public SingleChoiceComboBoxControl()
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
      this.m_Label1 = new System.Windows.Forms.Label();
      this._cbChoice = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
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
      // _cbChoice
      // 
      this._cbChoice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this._cbChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbChoice.Location = new System.Drawing.Point(8, 32);
      this._cbChoice.Name = "_cbChoice";
      this._cbChoice.Size = new System.Drawing.Size(248, 21);
      this._cbChoice.TabIndex = 3;
      this._cbChoice.SelectionChangeCommitted += new System.EventHandler(this._cbChoice_SelectionChangeCommitted);
      // 
      // SingleChoiceComboBoxControl
      // 
      this.Controls.Add(this._cbChoice);
      this.Controls.Add(this.m_Label1);
      this.Name = "SingleChoiceComboBoxControl";
      this.Size = new System.Drawing.Size(264, 56);
      this.ResumeLayout(false);

    }
    #endregion

    #region ISingleValueView Members

    ISingleChoiceViewEventSink _controller;
    public ISingleChoiceViewEventSink Controller
    {
      get
      {
        return _controller;
      }
      set
      {
        _controller = value;
      }
    }

    public void InitializeDescription(string value)
    {
      SizeF size1, size2;
      using(System.Drawing.Graphics grfx = this.CreateGraphics())
      {
        size1 = grfx.MeasureString(value,m_Label1.Font);
        size2 = grfx.MeasureString(value,m_Label1.Font,m_Label1.ClientSize.Width);
      }
      m_Label1.Size = new Size(_cbChoice.Size.Width,(int)(m_Label1.PreferredHeight*Math.Ceiling(size2.Height/size1.Height)));
      this.m_Label1.Text = value;

      this._cbChoice.Location = new Point(m_Label1.Location.X, m_Label1.Bounds.Bottom + _cbChoice.Size.Height/2);
      this.ClientSize = new Size(this.ClientSize.Width, _cbChoice.Bounds.Bottom);
    }

    public void InitializeChoice(string[] values, int initialselection)
    {
      this._cbChoice.Items.Clear();
      this._cbChoice.Items.AddRange(values);
      this._cbChoice.SelectedIndex = initialselection;
    }

    #endregion
   

    private void _cbChoice_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=_controller)
        _controller.EhChoiceChanged(this._cbChoice.SelectedIndex);
    
    }
  }
}
