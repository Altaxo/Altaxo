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
  public class DialogShellView : System.Windows.Forms.Form, IDialogShellView
  {
    private IDialogShellController m_Controller;
    private System.Windows.Forms.UserControl m_HostedControl;
    private System.Windows.Forms.Panel m_ButtonPanel;
    private System.Windows.Forms.Button m_btOK;
    private System.Windows.Forms.Button m_btCancel;
    private System.Windows.Forms.Button m_btApply;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public DialogShellView(System.Windows.Forms.UserControl hostedControl)
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      m_HostedControl = hostedControl;

      this.SuspendLayout();;
      this.m_HostedControl.SuspendLayout();
      this.ClientSize = new System.Drawing.Size(m_HostedControl.Size.Width, m_HostedControl.Size.Height + this.m_ButtonPanel.Size.Height);
      this.Controls.Add(hostedControl);
      hostedControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
      this.m_HostedControl.ResumeLayout(false);
      this.ResumeLayout(false);

      this.m_HostedControl.SizeChanged += new EventHandler(m_HostedControl_SizeChanged);

    }

    private void m_HostedControl_SizeChanged(object sender, EventArgs e)
    {
      this.ClientSize = new System.Drawing.Size(m_HostedControl.Size.Width, m_HostedControl.Size.Height + this.m_ButtonPanel.Size.Height);
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
      this.m_btApply = new System.Windows.Forms.Button();
      this.m_btCancel = new System.Windows.Forms.Button();
      this.m_btOK = new System.Windows.Forms.Button();
      this.m_ButtonPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_ButtonPanel
      // 
      this.m_ButtonPanel.Controls.Add(this.m_btApply);
      this.m_ButtonPanel.Controls.Add(this.m_btCancel);
      this.m_ButtonPanel.Controls.Add(this.m_btOK);
      this.m_ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_ButtonPanel.Location = new System.Drawing.Point(0, 226);
      this.m_ButtonPanel.Name = "m_ButtonPanel";
      this.m_ButtonPanel.Size = new System.Drawing.Size(292, 40);
      this.m_ButtonPanel.TabIndex = 0;
      // 
      // m_btApply
      // 
      this.m_btApply.Location = new System.Drawing.Point(192, 8);
      this.m_btApply.Name = "m_btApply";
      this.m_btApply.Size = new System.Drawing.Size(59, 24);
      this.m_btApply.TabIndex = 2;
      this.m_btApply.Text = "Apply";
      this.m_btApply.Click += new System.EventHandler(this.EhButtonApply_Click);
      // 
      // m_btCancel
      // 
      this.m_btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_btCancel.Location = new System.Drawing.Point(104, 8);
      this.m_btCancel.Name = "m_btCancel";
      this.m_btCancel.Size = new System.Drawing.Size(59, 24);
      this.m_btCancel.TabIndex = 1;
      this.m_btCancel.Text = "Cancel";
      this.m_btCancel.Click += new System.EventHandler(this.EhButtonCancel_Click);
      // 
      // m_btOK
      // 
      this.m_btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_btOK.Location = new System.Drawing.Point(16, 8);
      this.m_btOK.Name = "m_btOK";
      this.m_btOK.Size = new System.Drawing.Size(59, 24);
      this.m_btOK.TabIndex = 0;
      this.m_btOK.Text = "OK";
      this.m_btOK.Click += new System.EventHandler(this.EhButtonOK_Click);
      // 
      // DialogShellView
      // 
      this.AcceptButton = this.m_btOK;
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.CancelButton = this.m_btCancel;
      this.ClientSize = new System.Drawing.Size(292, 266);
      this.Controls.Add(this.m_ButtonPanel);
      this.Name = "DialogShellView";
      this.Text = "DialogShellView";
      this.Load += new System.EventHandler(this.EhView_Load);
      this.m_ButtonPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion

  
    private void EhButtonOK_Click(object sender, System.EventArgs e)
    {
      if(null!=m_Controller)
        m_Controller.EhOK();
    }

    private void EhButtonCancel_Click(object sender, System.EventArgs e)
    {
      if(null!=m_Controller)
        m_Controller.EhCancel();
    }

    private void EhButtonApply_Click(object sender, System.EventArgs e)
    {
      if(null!=m_Controller)
        m_Controller.EhApply();

    }

    private void EhView_Load(object sender, System.EventArgs e)
    {
      this.ActiveControl = m_HostedControl;
    }
    #region IDialogShellView Members

    public Form Form
    {
      get
      {
        return this;
      }
    }

    public IDialogShellController Controller
    {
      get
      {
        return m_Controller;
      }
      set
      {
        m_Controller = value;
      }
    }

    public bool ApplyVisible
    {
      set { this.m_btApply.Visible = value; }
    }

    public string Title
    {
      set { this.Text = value; }
    }

    #endregion

  }
}
