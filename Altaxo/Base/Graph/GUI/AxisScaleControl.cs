#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.GUI
{
  /// <summary>
  /// Summary description for AxisScaleControl.
  /// </summary>
  public class AxisScaleControl : System.Windows.Forms.UserControl, IAxisScaleView
  {
    private System.Windows.Forms.ComboBox m_Scale_cbRescale;
    private System.Windows.Forms.ComboBox m_Scale_cbType;
    private System.Windows.Forms.TextBox m_Scale_edTo;
    private System.Windows.Forms.TextBox m_Scale_edFrom;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;

    private IAxisScaleController m_Ctrl;

    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public AxisScaleControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      // TODO: Add any initialization after the InitializeComponent call

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
      this.m_Scale_cbRescale = new System.Windows.Forms.ComboBox();
      this.m_Scale_cbType = new System.Windows.Forms.ComboBox();
      this.m_Scale_edTo = new System.Windows.Forms.TextBox();
      this.m_Scale_edFrom = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // m_Scale_cbRescale
      // 
      this.m_Scale_cbRescale.Location = new System.Drawing.Point(80, 112);
      this.m_Scale_cbRescale.Name = "m_Scale_cbRescale";
      this.m_Scale_cbRescale.Size = new System.Drawing.Size(121, 21);
      this.m_Scale_cbRescale.TabIndex = 17;
      this.m_Scale_cbRescale.Text = "comboBox2";
      this.m_Scale_cbRescale.SelectionChangeCommitted += new System.EventHandler(this.EhAxisRescale_SelectionChangeCommit);
      // 
      // m_Scale_cbType
      // 
      this.m_Scale_cbType.Location = new System.Drawing.Point(80, 8);
      this.m_Scale_cbType.Name = "m_Scale_cbType";
      this.m_Scale_cbType.Size = new System.Drawing.Size(121, 21);
      this.m_Scale_cbType.TabIndex = 16;
      this.m_Scale_cbType.Text = "comboBox1";
      this.m_Scale_cbType.SelectionChangeCommitted += new System.EventHandler(this.EhAxisType_SelectionChangeCommit);
      // 
      // m_Scale_edTo
      // 
      this.m_Scale_edTo.Location = new System.Drawing.Point(80, 48);
      this.m_Scale_edTo.Name = "m_Scale_edTo";
      this.m_Scale_edTo.Size = new System.Drawing.Size(120, 20);
      this.m_Scale_edTo.TabIndex = 15;
      this.m_Scale_edTo.Text = "textBox2";
      this.m_Scale_edTo.TextChanged += new System.EventHandler(this.EhAxisEnd_Changed);
      // 
      // m_Scale_edFrom
      // 
      this.m_Scale_edFrom.Location = new System.Drawing.Point(80, 80);
      this.m_Scale_edFrom.Name = "m_Scale_edFrom";
      this.m_Scale_edFrom.Size = new System.Drawing.Size(120, 20);
      this.m_Scale_edFrom.TabIndex = 14;
      this.m_Scale_edFrom.Text = "textBox1";
      this.m_Scale_edFrom.TextChanged += new System.EventHandler(this.EhAxisOrg_Changed);
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(16, 112);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(56, 16);
      this.label5.TabIndex = 13;
      this.label5.Text = "Rescale";
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(16, 8);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(32, 16);
      this.label4.TabIndex = 12;
      this.label4.Text = "Type";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(16, 48);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(24, 16);
      this.label3.TabIndex = 11;
      this.label3.Text = "To";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(16, 80);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(40, 16);
      this.label2.TabIndex = 10;
      this.label2.Text = "From";
      // 
      // AxisScaleControl
      // 
      this.Controls.Add(this.m_Scale_cbRescale);
      this.Controls.Add(this.m_Scale_cbType);
      this.Controls.Add(this.m_Scale_edTo);
      this.Controls.Add(this.m_Scale_edFrom);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Name = "AxisScaleControl";
      this.Size = new System.Drawing.Size(232, 160);
      this.ResumeLayout(false);

    }
    #endregion

    public static void InitComboBox(System.Windows.Forms.ComboBox box, string[] names, string name)
    {
      box.Items.Clear();
      box.Items.AddRange(names);
      box.SelectedItem = name;
    }

    #region IAxisScaleView Members

    public IAxisScaleController Controller
    {
      get { return m_Ctrl; }
      set { m_Ctrl = value; }
    }
    public object ControllerObject
    {
      get { return Controller; }
      set { Controller = (IAxisScaleController)value; }
    }


    public System.Windows.Forms.Form Form
    {
      get { return this.ParentForm; }
    }

    public void InitializeAxisOrg(string org)
    {
      this.m_Scale_edFrom.Text = org;
    }

    public void InitializeAxisEnd(string end)
    {
      this.m_Scale_edTo.Text = end;
    }

    public void InitializeAxisType(string[] arr, string sel)
    {
      InitComboBox(this.m_Scale_cbType,arr,sel);
    }

    public void InitializeAxisRescale(string[] arr, string sel)
    {
      InitComboBox(this.m_Scale_cbRescale,arr,sel);
    }

    public void SetBoundaryGUIObject(object guiobject)
    {
      UserControl ctrl = (UserControl)guiobject;
      
      // find a good place for this object
      // right below the type
      this.Controls.Add(ctrl);
      ctrl.Location = new Point(0,this.m_Scale_cbType.Bounds.Bottom);
    }

    #endregion

    private void EhAxisOrg_Changed(object sender, System.EventArgs e)
    {
      if(null!=m_Ctrl)
        m_Ctrl.EhView_AxisOrgChanged(this.m_Scale_edFrom.Text);
    }

    private void EhAxisEnd_Changed(object sender, System.EventArgs e)
    {
      if(null!=m_Ctrl)
        m_Ctrl.EhView_AxisEndChanged(this.m_Scale_edTo.Text);
    }

    private void EhAxisType_SelectionChangeCommit(object sender, System.EventArgs e)
    {
      if(null!=m_Ctrl)
        m_Ctrl.EhView_AxisTypeChanged((string)this.m_Scale_cbType.SelectedItem);
    }

    private void EhAxisRescale_SelectionChangeCommit(object sender, System.EventArgs e)
    {
      if(null!=m_Ctrl)
        m_Ctrl.EhView_AxisRescaleChanged((string)this.m_Scale_cbType.SelectedItem);
    }

  }
}
