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
	using Altaxo.Collections;
  /// <summary>
  /// Summary description for AxisScaleControl.
  /// </summary>
  public class AxisScaleControl : System.Windows.Forms.UserControl, IAxisScaleView
  {
    private System.Windows.Forms.ComboBox m_Scale_cbType;
    private System.Windows.Forms.Label label4;

    private IAxisScaleController m_Ctrl;
		private TableLayoutPanel _tlp_Main;
		private FlowLayoutPanel _flp_ScaleType;
		private FlowLayoutPanel _flp_LinkType;
		private CheckBox _chkLinkScale;
		private ComboBox _cbLinkTarget;
		private FlowLayoutPanel _flp_TickSpacingType;
		private Label label1;
		private ComboBox _cbTickSpacingType;

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
      this._tlp_Main = new System.Windows.Forms.TableLayoutPanel();
      this._flp_ScaleType = new System.Windows.Forms.FlowLayoutPanel();
      this._flp_LinkType = new System.Windows.Forms.FlowLayoutPanel();
      this._chkLinkScale = new System.Windows.Forms.CheckBox();
      this._cbLinkTarget = new System.Windows.Forms.ComboBox();
      this._flp_TickSpacingType = new System.Windows.Forms.FlowLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this._cbTickSpacingType = new System.Windows.Forms.ComboBox();
      this._tlp_Main.SuspendLayout();
      this._flp_ScaleType.SuspendLayout();
      this._flp_LinkType.SuspendLayout();
      this._flp_TickSpacingType.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_Scale_cbType
      // 
      this.m_Scale_cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_Scale_cbType.Location = new System.Drawing.Point(69, 3);
      this.m_Scale_cbType.Name = "m_Scale_cbType";
      this.m_Scale_cbType.Size = new System.Drawing.Size(121, 21);
      this.m_Scale_cbType.TabIndex = 16;
      this.m_Scale_cbType.SelectionChangeCommitted += new System.EventHandler(this.EhAxisType_SelectionChangeCommit);
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 7);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(60, 13);
      this.label4.TabIndex = 12;
      this.label4.Text = "Scale type:";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // _tlp_Main
      // 
      this._tlp_Main.AutoSize = true;
      this._tlp_Main.ColumnCount = 1;
      this._tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._tlp_Main.Controls.Add(this._flp_ScaleType, 0, 0);
      this._tlp_Main.Controls.Add(this._flp_LinkType, 0, 1);
      this._tlp_Main.Controls.Add(this._flp_TickSpacingType, 0, 4);
      this._tlp_Main.Location = new System.Drawing.Point(3, 3);
      this._tlp_Main.Name = "_tlp_Main";
      this._tlp_Main.RowCount = 6;
      this._tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tlp_Main.Size = new System.Drawing.Size(308, 294);
      this._tlp_Main.TabIndex = 17;
      // 
      // _flp_ScaleType
      // 
      this._flp_ScaleType.AutoSize = true;
      this._flp_ScaleType.Controls.Add(this.label4);
      this._flp_ScaleType.Controls.Add(this.m_Scale_cbType);
      this._flp_ScaleType.Location = new System.Drawing.Point(3, 3);
      this._flp_ScaleType.Name = "_flp_ScaleType";
      this._flp_ScaleType.Size = new System.Drawing.Size(193, 27);
      this._flp_ScaleType.TabIndex = 0;
      // 
      // _flp_LinkType
      // 
      this._flp_LinkType.AutoSize = true;
      this._flp_LinkType.Controls.Add(this._chkLinkScale);
      this._flp_LinkType.Controls.Add(this._cbLinkTarget);
      this._flp_LinkType.Location = new System.Drawing.Point(3, 36);
      this._flp_LinkType.Name = "_flp_LinkType";
      this._flp_LinkType.Size = new System.Drawing.Size(222, 27);
      this._flp_LinkType.TabIndex = 1;
      // 
      // _chkLinkScale
      // 
      this._chkLinkScale.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._chkLinkScale.AutoSize = true;
      this._chkLinkScale.Location = new System.Drawing.Point(3, 5);
      this._chkLinkScale.Name = "_chkLinkScale";
      this._chkLinkScale.Size = new System.Drawing.Size(89, 17);
      this._chkLinkScale.TabIndex = 18;
      this._chkLinkScale.Text = "Link scale to:";
      this._chkLinkScale.UseVisualStyleBackColor = true;
      this._chkLinkScale.CheckedChanged += new System.EventHandler(this.EhLinked_CheckedChanged);
      // 
      // _cbLinkTarget
      // 
      this._cbLinkTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbLinkTarget.Location = new System.Drawing.Point(98, 3);
      this._cbLinkTarget.Name = "_cbLinkTarget";
      this._cbLinkTarget.Size = new System.Drawing.Size(121, 21);
      this._cbLinkTarget.TabIndex = 17;
      // 
      // _flp_TickSpacingType
      // 
      this._flp_TickSpacingType.AutoSize = true;
      this._flp_TickSpacingType.Controls.Add(this.label1);
      this._flp_TickSpacingType.Controls.Add(this._cbTickSpacingType);
      this._flp_TickSpacingType.Location = new System.Drawing.Point(3, 69);
      this._flp_TickSpacingType.Name = "_flp_TickSpacingType";
      this._flp_TickSpacingType.Size = new System.Drawing.Size(204, 27);
      this._flp_TickSpacingType.TabIndex = 2;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(71, 13);
      this.label1.TabIndex = 13;
      this.label1.Text = "Tick spacing:";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // _cbTickSpacingType
      // 
      this._cbTickSpacingType.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbTickSpacingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbTickSpacingType.Location = new System.Drawing.Point(80, 3);
      this._cbTickSpacingType.Name = "_cbTickSpacingType";
      this._cbTickSpacingType.Size = new System.Drawing.Size(121, 21);
      this._cbTickSpacingType.TabIndex = 17;
      this._cbTickSpacingType.SelectionChangeCommitted += new System.EventHandler(this.EhTickSpacingType_SelectionChangeCommitted);
      // 
      // AxisScaleControl
      // 
      this.AutoScroll = true;
      this.AutoSize = true;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this._tlp_Main);
      this.Name = "AxisScaleControl";
      this.Size = new System.Drawing.Size(314, 300);
      this._tlp_Main.ResumeLayout(false);
      this._tlp_Main.PerformLayout();
      this._flp_ScaleType.ResumeLayout(false);
      this._flp_ScaleType.PerformLayout();
      this._flp_LinkType.ResumeLayout(false);
      this._flp_LinkType.PerformLayout();
      this._flp_TickSpacingType.ResumeLayout(false);
      this._flp_TickSpacingType.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

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
   

    public void InitializeAxisType(SelectableListNodeList names)
    {
			GuiHelper.UpdateList(this.m_Scale_cbType, names);
    }

		public void InitializeTickSpacingType(SelectableListNodeList names)
		{
			GuiHelper.UpdateList(_cbTickSpacingType, names);
		}

		public void InitializeLinkTargets(SelectableListNodeList names)
		{
			GuiHelper.UpdateList(this._cbLinkTarget, names);
		}

		public bool ScaleIsLinked
		{
			get
			{
				return _chkLinkScale.Checked;
			}
			set
			{
				_chkLinkScale.Checked = value;
			}

		}

    [BrowsableAttribute(false)]
    private UserControl _boundaryControl=null;
    public void SetBoundaryView(object guiobject)
    {
			if (null != _boundaryControl)
				_tlp_Main.Controls.Remove(_boundaryControl);

			_boundaryControl = guiobject as UserControl;


			if(_boundaryControl!=null)
      {
				_tlp_Main.Controls.Add(_boundaryControl);
				_tlp_Main.SetCellPosition(_boundaryControl, new TableLayoutPanelCellPosition(0, 3));
			}
    }

    [BrowsableAttribute(false)]
    private Control _scaleControl = null;
    public void SetScaleView(object guiobject)
    {
			if (null != _scaleControl)
				_tlp_Main.Controls.Remove(_scaleControl);
			
			_scaleControl = guiobject as Control;

      if (_scaleControl != null)
      {
				_tlp_Main.Controls.Add(_scaleControl);
				_tlp_Main.SetCellPosition(_scaleControl, new TableLayoutPanelCellPosition(0, 2));
			}
    }

		[BrowsableAttribute(false)]
		private Control _tickSpacingControl = null;
		public void SetTickSpacingView(object guiobject)
		{
			if (null != _tickSpacingControl)
				_tlp_Main.Controls.Remove(_tickSpacingControl);

			_tickSpacingControl = guiobject as Control;

			if (_tickSpacingControl != null)
			{
				_tlp_Main.Controls.Add(_tickSpacingControl);
				_tlp_Main.SetCellPosition(_tickSpacingControl, new TableLayoutPanelCellPosition(0, 5));
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
			if (null != m_Ctrl)
			{
				GuiHelper.SynchronizeSelectionFromGui(this.m_Scale_cbType);
				m_Ctrl.EhView_AxisTypeChanged();
			}
    }

		private void EhTickSpacingType_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (null != m_Ctrl)
			{
				GuiHelper.SynchronizeSelectionFromGui(_cbTickSpacingType);
				m_Ctrl.EhView_TickSpacingTypeChanged();
			}
		}

		private void EhLinkTarget_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (null != m_Ctrl)
			{
				GuiHelper.SynchronizeSelectionFromGui(this._cbLinkTarget);
				m_Ctrl.EhView_LinkTargetChanged();
			}
		}


		private void EhLinked_CheckedChanged(object sender, EventArgs e)
		{
			if (null != m_Ctrl)
			{
				m_Ctrl.EhView_LinkChanged(_chkLinkScale.Checked);
			}
		}


  

  }
}
