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
  /// Summary description for AxisScaleControl.
  /// </summary>
  public class AxisScaleControl : System.Windows.Forms.UserControl, IAxisScaleView
  {
    private System.Windows.Forms.ComboBox m_Scale_cbType;
    private System.Windows.Forms.Label label4;

    private IAxisScaleController m_Ctrl;

    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public AxisScaleControl()
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
      this.m_Scale_cbType = new System.Windows.Forms.ComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.SuspendLayout();
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
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(8, 8);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(32, 16);
      this.label4.TabIndex = 12;
      this.label4.Text = "Type:";
      // 
      // AxisScaleControl
      // 
      this.Controls.Add(this.m_Scale_cbType);
      this.Controls.Add(this.label4);
      this.Name = "AxisScaleControl";
      this.Size = new System.Drawing.Size(232, 48);
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
   

    public void InitializeAxisType(string[] arr, string sel)
    {
      InitComboBox(this.m_Scale_cbType,arr,sel);
    }


    [BrowsableAttribute(false)]
    private UserControl _boundaryControl=null;
    public void SetBoundaryView(object guiobject)
    {
      if(null!=_boundaryControl)
        this.Controls.Remove(_boundaryControl);

      _boundaryControl = guiobject as UserControl;
      
      if(_boundaryControl!=null)
      {
        // find a good place for this object
        // right below the type

        if(_scaleControl==null)
        _boundaryControl.Location = new Point(0,this.m_Scale_cbType.Bounds.Bottom + this.m_Scale_cbType.Bounds.Height);
        else
        _boundaryControl.Location = new Point(0, this._scaleControl.Bounds.Bottom + this.m_Scale_cbType.Bounds.Height);

      SetMySize();
        this.Controls.Add(_boundaryControl);
      }
    }

    [BrowsableAttribute(false)]
    private Control _scaleControl = null;
    public void SetScaleView(object guiobject)
    {
      if (null != _scaleControl)
        this.Controls.Remove(_scaleControl);

      _scaleControl = guiobject as Control;

      if (_scaleControl != null)
      {
        // find a good place for this object
        // right below the type
       _scaleControl.Location = new Point(0, this.m_Scale_cbType.Bounds.Bottom + this.m_Scale_cbType.Bounds.Height);
       SetMySize();
       this.Controls.Add(_scaleControl);
      }
    }

    void SetMySize()
    {
      int x = this.m_Scale_cbType.Bounds.Right;
      int y = this.m_Scale_cbType.Bounds.Bottom;

      if (null != _scaleControl)
      {
        x = Math.Max(x, _scaleControl.Bounds.Right);
        y = Math.Max(y, _scaleControl.Bounds.Bottom);
      }
      if (null != _boundaryControl)
      {
        x = Math.Max(x, _boundaryControl.Bounds.Right);
        y = Math.Max(y, _boundaryControl.Bounds.Bottom);
      }
      this.Size = new Size(x,y);
    }


    #endregion

   
    private void EhAxisType_SelectionChangeCommit(object sender, System.EventArgs e)
    {
      if(null!=m_Ctrl)
        m_Ctrl.EhView_AxisTypeChanged((string)this.m_Scale_cbType.SelectedItem);
    }

  

  }
}
