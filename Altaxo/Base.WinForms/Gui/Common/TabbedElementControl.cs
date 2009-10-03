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
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;


namespace Altaxo.Gui.Common
{
  /// <summary>
  /// This view is intended to be used as Dialog. It hosts an arbitrary UserControl in its client area,
  /// which provides the user interaction.
  /// The only elements it itself is shown are the 3 buttons OK, Cancel, and Apply.
  /// </summary>
  [UserControlForController(typeof(ITabbedElementViewEventSink))]
  public class TabbedElementControl : System.Windows.Forms.UserControl, ITabbedElementView
  {
    private ITabbedElementViewEventSink _controller;
    private System.Windows.Forms.Panel m_ButtonPanel;
    private System.Windows.Forms.TabControl m_TabControl;


    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public TabbedElementControl()
    {
      //
      // Required for Windows Form Designer support
      //
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

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.m_ButtonPanel = new System.Windows.Forms.Panel();
      this.m_TabControl = new System.Windows.Forms.TabControl();
      this.SuspendLayout();
      // 
      // m_ButtonPanel
      // 
      this.m_ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_ButtonPanel.Location = new System.Drawing.Point(0, 218);
      this.m_ButtonPanel.Name = "m_ButtonPanel";
      this.m_ButtonPanel.Size = new System.Drawing.Size(272, 40);
      this.m_ButtonPanel.TabIndex = 0;
      // 
      // m_TabControl
      // 
      this.m_TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.m_TabControl.Location = new System.Drawing.Point(4, 8);
      this.m_TabControl.Name = "m_TabControl";
      this.m_TabControl.SelectedIndex = 0;
      this.m_TabControl.Size = new System.Drawing.Size(264, 200);
      this.m_TabControl.TabIndex = 1;
      // 
      // TabbedDialogView
      // 
      this.Controls.Add(this.m_TabControl);
      this.Controls.Add(this.m_ButtonPanel);
      this.Name = "TabbedDialogView";
      this.Size = new System.Drawing.Size(272, 258);
      this.Load += new System.EventHandler(this.EhView_Load);
      this.ResumeLayout(false);

    }
    #endregion

  
  

    private void EhView_Load(object sender, System.EventArgs e)
    {
      // this.ActiveControl = m_HostedControl;
    }
    #region ITabbedDialogView Members

   

    public ITabbedElementViewEventSink Controller
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

    public void ClearTabs()
    {
      m_TabControl.TabPages.Clear();
    }

    public void AddTab(string title, object view)
    {

      // look if the size of the tab control is enough, otherwise make the dialog bigger
      /*
            this.ResumeLayout(true);
            this.m_HostedControl.ResumeLayout(true);
            this.ClientSize = new System.Drawing.Size(m_HostedControl.Size.Width, m_HostedControl.Size.Height + this.m_ButtonPanel.Size.Height);
            this.Controls.Add(hostedControl);
            this.m_HostedControl.ResumeLayout(false);
            this.ResumeLayout(false);
      */
      System.Windows.Forms.TabPage tab = new System.Windows.Forms.TabPage(title);
      

      System.Windows.Forms.Control cc = (System.Windows.Forms.Control)view;
      


      tab.Controls.Add(cc);
      m_TabControl.TabPages.Add(tab);

      if (null != cc)
      {
        cc.Enter += EhChildControl_Entered;
        cc.Validated += EhChildControl_Validated;
        int diffx = Math.Max(0, cc.Width - m_TabControl.TabPages[0].ClientSize.Width);
        int diffy = Math.Max(0, cc.Height - m_TabControl.TabPages[0].ClientSize.Height);

        if (diffx > 0 || diffy > 0)
        {
          this.Size = new Size(this.Size.Width + diffx, this.Size.Height + diffy);
        }
      }
    }

    public void BringTabToFront(int index)
    {
      m_TabControl.SelectedIndex = index;
      m_TabControl.TabPages[index].Focus();
    }

    public event EventHandler ChildControl_Entered;
    void EhChildControl_Entered(object sender, EventArgs e)
    {
      if (ChildControl_Entered != null)
      {
        ChildControl_Entered(sender, e);
      }
    }
    public event EventHandler ChildControl_Validated;
    void EhChildControl_Validated(object sender, EventArgs e)
    {
      if (ChildControl_Validated != null)
      {
        ChildControl_Validated(sender, e);
      }
    }
    #endregion
  }
}
