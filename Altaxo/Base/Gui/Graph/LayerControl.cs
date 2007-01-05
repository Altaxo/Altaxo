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

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for LayerControl.
  /// </summary>
  public class LayerControl : System.Windows.Forms.UserControl, ILayerView
  {
    private System.Windows.Forms.ListBox m_lbEdges;
    private System.Windows.Forms.TabControl m_TabCtrl;
    private ILayerController m_Ctrl;
    private int m_SuppressEvents=0;

    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public LayerControl()
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
      this.m_lbEdges = new System.Windows.Forms.ListBox();
      this.m_TabCtrl = new System.Windows.Forms.TabControl();
      this.SuspendLayout();
      // 
      // m_lbEdges
      // 
      this.m_lbEdges.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left)));
      this.m_lbEdges.Location = new System.Drawing.Point(8, 8);
      this.m_lbEdges.Name = "m_lbEdges";
      this.m_lbEdges.Size = new System.Drawing.Size(64, 420);
      this.m_lbEdges.TabIndex = 2;
      this.m_lbEdges.SelectedIndexChanged += new System.EventHandler(this.EhSecondChoice_SelChanged);
      // 
      // m_TabCtrl
      // 
      this.m_TabCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.m_TabCtrl.Location = new System.Drawing.Point(80, 8);
      this.m_TabCtrl.Name = "m_TabCtrl";
      this.m_TabCtrl.SelectedIndex = 0;
      this.m_TabCtrl.Size = new System.Drawing.Size(552, 420);
      this.m_TabCtrl.TabIndex = 3;
      this.m_TabCtrl.SelectedIndexChanged += new System.EventHandler(this.EhTabCtrl_SelectedIndexChanged);
      // 
      // LayerControl
      // 
      this.Controls.Add(this.m_TabCtrl);
      this.Controls.Add(this.m_lbEdges);
      this.Name = "LayerControl";
      this.Size = new System.Drawing.Size(640, 440);
      this.ResumeLayout(false);

    }
    #endregion

    #region ILayerView Members

    public ILayerController Controller
    {
      get
      {
        return m_Ctrl;
      }
      set
      {
        m_Ctrl = value;
      }
    }

   

    public void AddTab(string name, string text)
    {
      System.Windows.Forms.TabPage tc = new System.Windows.Forms.TabPage();
      tc.Name = name;
      tc.Text = text;
      tc.CausesValidation = true;
      tc.Validating += EhTabControl_Validating;
      
      this.m_TabCtrl.Controls.Add( tc );
    }

    public event CancelEventHandler TabValidating;
    void EhTabControl_Validating(object sender, CancelEventArgs e)
    {
      if (TabValidating != null)
        TabValidating(this, e);
    }

    public void SelectTab(string name)
    {
      foreach(TabPage page in this.m_TabCtrl.Controls)
      {
        if (page.Name == name)
        {
          this.m_TabCtrl.SelectedTab = page;
          break;
        }
      }
    }

    private void EhTabCtrl_SelectedIndexChanged(object sender, System.EventArgs e)
    {
      if(null!=m_Ctrl)
      {
        int sel = m_TabCtrl.SelectedIndex;
        System.Windows.Forms.TabPage tp = m_TabCtrl.TabPages[sel];

        m_Ctrl.EhView_PageChanged(tp.Name);
      }
    }

    public object CurrentContent
    {
      get
      {
        int sel = m_TabCtrl.SelectedIndex;
        System.Windows.Forms.TabPage tp = m_TabCtrl.TabPages[sel];
        return tp.Controls[0];
      }
      set
      {
        int sel = m_TabCtrl.SelectedIndex;
        System.Windows.Forms.TabPage tp = m_TabCtrl.TabPages[sel];
        if(tp.Controls.Count>0)
          tp.Controls.Clear();

        Control ctrl = value as Control;
        if (ctrl != null)
        {
          ctrl.Location = new Point(0, 0);
          ctrl.Dock = DockStyle.Fill;
          tp.Controls.Add(ctrl);
          ctrl.CausesValidation = true;
          
        }
        
      }
    }

    public void SetCurrentContentWithEnable(object guielement, bool enable, string title)
    {
      int sel = m_TabCtrl.SelectedIndex;
      System.Windows.Forms.TabPage tp = m_TabCtrl.TabPages[sel];
      if(tp.Controls.Count>0)
        tp.Controls.Clear();

      _chkPageEnable = new CheckBox();
      _chkPageEnable.Checked = enable;
      _chkPageEnable.Text = title;
      _chkPageEnable.CheckedChanged += new EventHandler(EhControlEnable_CheckedChanged);
      _chkPageEnable.Size = new Size(this.ClientSize.Width,_chkPageEnable.Height);

      Control value = guielement as Control;
      if (value!=null)
      {
        value.Enabled = enable;
        value.Location = new Point(0, _chkPageEnable.Height);
        value.Size = new Size(tp.ClientSize.Width, tp.ClientSize.Height - _chkPageEnable.Height);
        value.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tp.Controls.Add(value);
      }
      tp.Controls.Add(_chkPageEnable);
    }

    public bool IsPageEnabled
    {
      get
      {
        if(this.m_TabCtrl.SelectedTab!=null && this.m_TabCtrl.SelectedTab.Controls.Count>=2 && this.m_TabCtrl.SelectedTab.Controls[1] is CheckBox)
          return ((CheckBox)this.m_TabCtrl.SelectedTab.Controls[1]).Checked;
        else
          return true;
      }
      set
      {
        if(this.m_TabCtrl.SelectedTab!=null && this.m_TabCtrl.SelectedTab.Controls.Count>=2 && this.m_TabCtrl.SelectedTab.Controls[1] is CheckBox)
        {
          ((CheckBox)this.m_TabCtrl.SelectedTab.Controls[1]).Checked = value;
          this.m_TabCtrl.Controls[0].Enabled=value;
        }
      }
    }

    CheckBox _chkPageEnable;
    bool _pageEnabled = true;
    void EhControlEnable_CheckedChanged(object sender, EventArgs args)
    {
      _pageEnabled = _chkPageEnable.Checked;
      m_TabCtrl.SelectedTab.Controls[0].Enabled = _pageEnabled;

      if(null!=m_Ctrl && m_SuppressEvents==0)
        m_Ctrl.EhView_PageEnabledChanged(_pageEnabled);
    }

    public void InitializeSecondaryChoice(string[] names, string name)
    {
      ++m_SuppressEvents;
      this.m_lbEdges.Items.Clear();
      this.m_lbEdges.Items.AddRange(names);
      this.m_lbEdges.SelectedItem = name;
      --m_SuppressEvents;
    }

    #endregion

    private void EhSecondChoice_SelChanged(object sender, System.EventArgs e)
    {
      if(null!=m_Ctrl && m_SuppressEvents==0)
        m_Ctrl.EhView_SecondChoiceChanged(this.m_lbEdges.SelectedIndex, (string)this.m_lbEdges.SelectedItem);
    }
  }
}
