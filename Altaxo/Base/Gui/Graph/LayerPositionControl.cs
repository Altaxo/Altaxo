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

using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for LayerPositionControl.
  /// </summary>
  public class LayerPositionControl : System.Windows.Forms.UserControl, ILayerPositionView
  {
    private ILayerPositionViewEventSink m_Controller;

    private System.Windows.Forms.ComboBox m_Layer_cbHeightType;
    private System.Windows.Forms.ComboBox m_Layer_cbWidthType;
    private System.Windows.Forms.ComboBox m_Layer_cbTopType;
    private System.Windows.Forms.ComboBox m_Layer_cbLeftType;
    private System.Windows.Forms.ComboBox m_Layer_cbLinkedLayer;
    private System.Windows.Forms.Label label21;
    private System.Windows.Forms.TextBox m_Layer_edScale;
    private System.Windows.Forms.Label label20;
    private System.Windows.Forms.Label label19;
    private System.Windows.Forms.TextBox m_Layer_edHeight;
    private System.Windows.Forms.Label label18;
    private System.Windows.Forms.TextBox m_Layer_edWidth;
    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.TextBox m_Layer_edTopPosition;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.TextBox m_Layer_edLeftPosition;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.GroupBox m_groupLinkXAxis;
    private AxisLinkControl m_ctrlLinkXAxis;
    private System.Windows.Forms.GroupBox m_groupLinkYAxis;
    private AxisLinkControl m_ctrlLinkYAxis;
    private System.Windows.Forms.CheckBox m_Layer_ClipDataToFrame;
    private Altaxo.Gui.Common.Drawing.RotationComboBox m_Layer_edRotation;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public LayerPositionControl()
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

    public static void InitComboBox(System.Windows.Forms.ComboBox box, string[] names, string name)
    {
      box.Items.Clear();
      box.Items.AddRange(names);
      box.SelectedItem = name;
    }
    public static void InitComboBox(System.Windows.Forms.ComboBox box,SelectableListNodeList names)
    {
      box.BeginUpdate();
      box.Items.Clear();
      foreach (SelectableListNode node in names)
      {
        box.Items.Add(node);
      }
      foreach (SelectableListNode node in names)
      {
        if (node.Selected)
        {
          box.SelectedItem = node;
          break;
        }
      }
      box.EndUpdate();
    }



    #region Component Designer generated code
    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.m_Layer_cbHeightType = new System.Windows.Forms.ComboBox();
      this.m_Layer_cbWidthType = new System.Windows.Forms.ComboBox();
      this.m_Layer_cbTopType = new System.Windows.Forms.ComboBox();
      this.m_Layer_cbLeftType = new System.Windows.Forms.ComboBox();
      this.m_Layer_cbLinkedLayer = new System.Windows.Forms.ComboBox();
      this.label21 = new System.Windows.Forms.Label();
      this.m_Layer_edScale = new System.Windows.Forms.TextBox();
      this.label20 = new System.Windows.Forms.Label();
      this.label19 = new System.Windows.Forms.Label();
      this.m_Layer_edHeight = new System.Windows.Forms.TextBox();
      this.label18 = new System.Windows.Forms.Label();
      this.m_Layer_edWidth = new System.Windows.Forms.TextBox();
      this.label17 = new System.Windows.Forms.Label();
      this.m_Layer_edTopPosition = new System.Windows.Forms.TextBox();
      this.label16 = new System.Windows.Forms.Label();
      this.m_Layer_edLeftPosition = new System.Windows.Forms.TextBox();
      this.label15 = new System.Windows.Forms.Label();
      this.m_groupLinkXAxis = new System.Windows.Forms.GroupBox();
      this.m_groupLinkYAxis = new System.Windows.Forms.GroupBox();
      this.m_Layer_ClipDataToFrame = new System.Windows.Forms.CheckBox();
      this.m_Layer_edRotation = new Altaxo.Gui.Common.Drawing.RotationComboBox();
      this.m_ctrlLinkYAxis = new Altaxo.Gui.Graph.AxisLinkControl();
      this.m_ctrlLinkXAxis = new Altaxo.Gui.Graph.AxisLinkControl();
      this.m_groupLinkXAxis.SuspendLayout();
      this.m_groupLinkYAxis.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_Layer_cbHeightType
      // 
      this.m_Layer_cbHeightType.Location = new System.Drawing.Point(115, 160);
      this.m_Layer_cbHeightType.Name = "m_Layer_cbHeightType";
      this.m_Layer_cbHeightType.Size = new System.Drawing.Size(157, 21);
      this.m_Layer_cbHeightType.TabIndex = 35;
      this.m_Layer_cbHeightType.Text = "comboBox1";
      this.m_Layer_cbHeightType.SelectionChangeCommitted += new System.EventHandler(this.EhHeightType_SelectionChangeCommitted);
      // 
      // m_Layer_cbWidthType
      // 
      this.m_Layer_cbWidthType.Location = new System.Drawing.Point(115, 136);
      this.m_Layer_cbWidthType.Name = "m_Layer_cbWidthType";
      this.m_Layer_cbWidthType.Size = new System.Drawing.Size(157, 21);
      this.m_Layer_cbWidthType.TabIndex = 34;
      this.m_Layer_cbWidthType.Text = "comboBox1";
      this.m_Layer_cbWidthType.SelectionChangeCommitted += new System.EventHandler(this.EhWidthType_SelectionChangeCommitted);
      // 
      // m_Layer_cbTopType
      // 
      this.m_Layer_cbTopType.Location = new System.Drawing.Point(115, 104);
      this.m_Layer_cbTopType.Name = "m_Layer_cbTopType";
      this.m_Layer_cbTopType.Size = new System.Drawing.Size(157, 21);
      this.m_Layer_cbTopType.TabIndex = 33;
      this.m_Layer_cbTopType.Text = "comboBox1";
      this.m_Layer_cbTopType.SelectionChangeCommitted += new System.EventHandler(this.EhTopType_SelectionChangeCommitted);
      // 
      // m_Layer_cbLeftType
      // 
      this.m_Layer_cbLeftType.Location = new System.Drawing.Point(115, 80);
      this.m_Layer_cbLeftType.Name = "m_Layer_cbLeftType";
      this.m_Layer_cbLeftType.Size = new System.Drawing.Size(157, 21);
      this.m_Layer_cbLeftType.TabIndex = 32;
      this.m_Layer_cbLeftType.Text = "comboBox1";
      this.m_Layer_cbLeftType.SelectionChangeCommitted += new System.EventHandler(this.EhLeftType_SelectionChangeCommitted);
      // 
      // m_Layer_cbLinkedLayer
      // 
      this.m_Layer_cbLinkedLayer.Location = new System.Drawing.Point(104, 8);
      this.m_Layer_cbLinkedLayer.Name = "m_Layer_cbLinkedLayer";
      this.m_Layer_cbLinkedLayer.Size = new System.Drawing.Size(168, 21);
      this.m_Layer_cbLinkedLayer.TabIndex = 31;
      this.m_Layer_cbLinkedLayer.Text = "None";
      this.m_Layer_cbLinkedLayer.SelectionChangeCommitted += new System.EventHandler(this.EhLinkedLayer_SelectionChangeCommitted);
      // 
      // label21
      // 
      this.label21.Location = new System.Drawing.Point(16, 8);
      this.label21.Name = "label21";
      this.label21.Size = new System.Drawing.Size(88, 16);
      this.label21.TabIndex = 30;
      this.label21.Text = "Linked to layer:";
      // 
      // m_Layer_edScale
      // 
      this.m_Layer_edScale.Location = new System.Drawing.Point(166, 223);
      this.m_Layer_edScale.Name = "m_Layer_edScale";
      this.m_Layer_edScale.Size = new System.Drawing.Size(106, 20);
      this.m_Layer_edScale.TabIndex = 29;
      this.m_Layer_edScale.Validating += new System.ComponentModel.CancelEventHandler(this.EhScale_Validating);
      // 
      // label20
      // 
      this.label20.Location = new System.Drawing.Point(112, 224);
      this.label20.Name = "label20";
      this.label20.Size = new System.Drawing.Size(48, 16);
      this.label20.TabIndex = 28;
      this.label20.Text = "Scale";
      this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label19
      // 
      this.label19.Location = new System.Drawing.Point(112, 200);
      this.label19.Name = "label19";
      this.label19.Size = new System.Drawing.Size(48, 16);
      this.label19.TabIndex = 26;
      this.label19.Text = "Rotation";
      this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // m_Layer_edHeight
      // 
      this.m_Layer_edHeight.Location = new System.Drawing.Point(48, 160);
      this.m_Layer_edHeight.Name = "m_Layer_edHeight";
      this.m_Layer_edHeight.Size = new System.Drawing.Size(61, 20);
      this.m_Layer_edHeight.TabIndex = 25;
      this.m_Layer_edHeight.Validating += new System.ComponentModel.CancelEventHandler(this.EhHeight_Validating);
      // 
      // label18
      // 
      this.label18.Location = new System.Drawing.Point(0, 160);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(48, 16);
      this.label18.TabIndex = 24;
      this.label18.Text = "Height";
      this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_Layer_edWidth
      // 
      this.m_Layer_edWidth.Location = new System.Drawing.Point(48, 136);
      this.m_Layer_edWidth.Name = "m_Layer_edWidth";
      this.m_Layer_edWidth.Size = new System.Drawing.Size(61, 20);
      this.m_Layer_edWidth.TabIndex = 23;
      this.m_Layer_edWidth.Validating += new System.ComponentModel.CancelEventHandler(this.EhWidth_Validating);
      // 
      // label17
      // 
      this.label17.Location = new System.Drawing.Point(3, 136);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(45, 16);
      this.label17.TabIndex = 22;
      this.label17.Text = "Width";
      this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_Layer_edTopPosition
      // 
      this.m_Layer_edTopPosition.Location = new System.Drawing.Point(48, 104);
      this.m_Layer_edTopPosition.Name = "m_Layer_edTopPosition";
      this.m_Layer_edTopPosition.Size = new System.Drawing.Size(61, 20);
      this.m_Layer_edTopPosition.TabIndex = 21;
      this.m_Layer_edTopPosition.Validating += new System.ComponentModel.CancelEventHandler(this.EhTop_Validating);
      // 
      // label16
      // 
      this.label16.Location = new System.Drawing.Point(0, 104);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(48, 16);
      this.label16.TabIndex = 20;
      this.label16.Text = "Top";
      this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_Layer_edLeftPosition
      // 
      this.m_Layer_edLeftPosition.Location = new System.Drawing.Point(48, 80);
      this.m_Layer_edLeftPosition.Name = "m_Layer_edLeftPosition";
      this.m_Layer_edLeftPosition.Size = new System.Drawing.Size(61, 20);
      this.m_Layer_edLeftPosition.TabIndex = 19;
      this.m_Layer_edLeftPosition.Validating += new System.ComponentModel.CancelEventHandler(this.EhLeft_Validating);
      // 
      // label15
      // 
      this.label15.Location = new System.Drawing.Point(0, 80);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(48, 16);
      this.label15.TabIndex = 18;
      this.label15.Text = "Left";
      this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_groupLinkXAxis
      // 
      this.m_groupLinkXAxis.Controls.Add(this.m_ctrlLinkXAxis);
      this.m_groupLinkXAxis.Location = new System.Drawing.Point(280, 0);
      this.m_groupLinkXAxis.Name = "m_groupLinkXAxis";
      this.m_groupLinkXAxis.Size = new System.Drawing.Size(144, 136);
      this.m_groupLinkXAxis.TabIndex = 37;
      this.m_groupLinkXAxis.TabStop = false;
      this.m_groupLinkXAxis.Text = "Link X scale";
      // 
      // m_groupLinkYAxis
      // 
      this.m_groupLinkYAxis.Controls.Add(this.m_ctrlLinkYAxis);
      this.m_groupLinkYAxis.Location = new System.Drawing.Point(280, 144);
      this.m_groupLinkYAxis.Name = "m_groupLinkYAxis";
      this.m_groupLinkYAxis.Size = new System.Drawing.Size(144, 136);
      this.m_groupLinkYAxis.TabIndex = 38;
      this.m_groupLinkYAxis.TabStop = false;
      this.m_groupLinkYAxis.Text = "Link Y scale";
      // 
      // m_Layer_ClipDataToFrame
      // 
      this.m_Layer_ClipDataToFrame.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.m_Layer_ClipDataToFrame.Location = new System.Drawing.Point(59, 249);
      this.m_Layer_ClipDataToFrame.Name = "m_Layer_ClipDataToFrame";
      this.m_Layer_ClipDataToFrame.Size = new System.Drawing.Size(120, 16);
      this.m_Layer_ClipDataToFrame.TabIndex = 41;
      this.m_Layer_ClipDataToFrame.Text = "Clip data to frame";
      this.m_Layer_ClipDataToFrame.Validating += new System.ComponentModel.CancelEventHandler(this.EhClipDataToFrame_Validating);
      // 
      // m_Layer_edRotation
      // 
      this.m_Layer_edRotation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.m_Layer_edRotation.FormattingEnabled = true;
      this.m_Layer_edRotation.ItemHeight = 13;
      this.m_Layer_edRotation.Location = new System.Drawing.Point(166, 199);
      this.m_Layer_edRotation.Name = "m_Layer_edRotation";
      this.m_Layer_edRotation.Size = new System.Drawing.Size(106, 19);
      this.m_Layer_edRotation.TabIndex = 42;
      this.m_Layer_edRotation.RotationChanged += new System.EventHandler(this.EhRotation_Changed);
      // 
      // m_ctrlLinkYAxis
      // 
      this.m_ctrlLinkYAxis.Controller = null;
      this.m_ctrlLinkYAxis.ControllerObject = null;
      this.m_ctrlLinkYAxis.Location = new System.Drawing.Point(8, 16);
      this.m_ctrlLinkYAxis.Name = "m_ctrlLinkYAxis";
      this.m_ctrlLinkYAxis.Size = new System.Drawing.Size(128, 120);
      this.m_ctrlLinkYAxis.TabIndex = 0;
      // 
      // m_ctrlLinkXAxis
      // 
      this.m_ctrlLinkXAxis.Controller = null;
      this.m_ctrlLinkXAxis.ControllerObject = null;
      this.m_ctrlLinkXAxis.Location = new System.Drawing.Point(8, 16);
      this.m_ctrlLinkXAxis.Name = "m_ctrlLinkXAxis";
      this.m_ctrlLinkXAxis.Size = new System.Drawing.Size(128, 120);
      this.m_ctrlLinkXAxis.TabIndex = 36;
      // 
      // LayerPositionControl
      // 
      this.Controls.Add(this.m_Layer_edRotation);
      this.Controls.Add(this.m_Layer_ClipDataToFrame);
      this.Controls.Add(this.m_groupLinkYAxis);
      this.Controls.Add(this.m_groupLinkXAxis);
      this.Controls.Add(this.m_Layer_cbHeightType);
      this.Controls.Add(this.m_Layer_cbWidthType);
      this.Controls.Add(this.m_Layer_cbTopType);
      this.Controls.Add(this.m_Layer_cbLeftType);
      this.Controls.Add(this.m_Layer_cbLinkedLayer);
      this.Controls.Add(this.label21);
      this.Controls.Add(this.m_Layer_edScale);
      this.Controls.Add(this.label20);
      this.Controls.Add(this.label19);
      this.Controls.Add(this.m_Layer_edHeight);
      this.Controls.Add(this.label18);
      this.Controls.Add(this.m_Layer_edWidth);
      this.Controls.Add(this.label17);
      this.Controls.Add(this.m_Layer_edTopPosition);
      this.Controls.Add(this.label16);
      this.Controls.Add(this.m_Layer_edLeftPosition);
      this.Controls.Add(this.label15);
      this.Name = "LayerPositionControl";
      this.Size = new System.Drawing.Size(432, 288);
      this.m_groupLinkXAxis.ResumeLayout(false);
      this.m_groupLinkYAxis.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    #region ILayerPositionView Members

    public ILayerPositionViewEventSink Controller
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

   

    #endregion

  

    #region ILayerPositionView Members

    public void InitializeLeft(string txt)
    {
      this.m_Layer_edLeftPosition.Text = txt;
    }

    public void InitializeTop(string txt)
    {
      this.m_Layer_edTopPosition.Text = txt;
    }

    public void InitializeHeight(string txt)
    {
      this.m_Layer_edHeight.Text = txt;
    }

    public void InitializeWidth(string txt)
    {
      this.m_Layer_edWidth.Text = txt;
    }

    public void InitializeRotation(float rotationValue)
    {
      this.m_Layer_edRotation.Rotation = rotationValue;
    }

    public void InitializeScale(string txt)
    {
      this.m_Layer_edScale.Text = txt;
    }

    public void InitializeClipDataToFrame(bool value)
    {
      this.m_Layer_ClipDataToFrame.Checked = value;
    }

    public void InitializeLeftType(SelectableListNodeList names)
    {
      InitComboBox(this.m_Layer_cbLeftType,names);
    }

    public void InitializeTopType(SelectableListNodeList names)
    {
      InitComboBox(this.m_Layer_cbTopType,names);
    }

    public void InitializeHeightType(SelectableListNodeList names)
    {
      InitComboBox(this.m_Layer_cbHeightType,names);
    }

    public void InitializeWidthType(SelectableListNodeList names)
    {
      InitComboBox(this.m_Layer_cbWidthType,names);
    }

    public void InitializeLinkedLayer(SelectableListNodeList names)
    {
      InitComboBox(this.m_Layer_cbLinkedLayer,names);
    }

    public IAxisLinkView GetXAxisLink() { return this.m_ctrlLinkXAxis; }
    public IAxisLinkView GetYAxisLink() { return this.m_ctrlLinkYAxis; }


    #endregion

    private void EhLinkedLayer_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_LinkedLayerChanged((SelectableListNode)this.m_Layer_cbLinkedLayer.SelectedItem);
    }

    private void EhLeftType_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_LeftTypeChanged((SelectableListNode)this.m_Layer_cbLeftType.SelectedItem);
    }

    private void EhTopType_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_TopTypeChanged((SelectableListNode)this.m_Layer_cbTopType.SelectedItem);
    }

    private void EhWidthType_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_WidthTypeChanged((SelectableListNode)this.m_Layer_cbWidthType.SelectedItem);
    
    }

    private void EhHeightType_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_HeightTypeChanged((SelectableListNode)this.m_Layer_cbHeightType.SelectedItem);
    
    }

    private void EhLeft_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
      {
        bool bCancel = e.Cancel;
        Controller.EhView_LeftChanged(((TextBox)sender).Text,ref bCancel);
        e.Cancel = bCancel;
      }
    }

    private void EhTop_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
      {
        bool bCancel = e.Cancel;
        Controller.EhView_TopChanged(((TextBox)sender).Text,ref bCancel);
        e.Cancel = bCancel;
      }
    }

    private void EhWidth_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
      {
        bool bCancel = e.Cancel;
        Controller.EhView_WidthChanged(((TextBox)sender).Text,ref bCancel);
        e.Cancel = bCancel;
      }
    }

    private void EhHeight_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
      {
        bool bCancel = e.Cancel;
        Controller.EhView_HeightChanged(((TextBox)sender).Text,ref bCancel);
        e.Cancel = bCancel;
      }
    
    }

    private void EhRotation_Changed(object sender, EventArgs e)
    {
      if(null!=Controller)
      {
        Controller.EhView_RotationChanged(m_Layer_edRotation.Rotation);
      }
    
    }

    private void EhScale_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
      {
        bool bCancel = e.Cancel;
        Controller.EhView_ScaleChanged(((TextBox)sender).Text,ref bCancel);
        e.Cancel = bCancel;
      }
    
    }

    private void EhClipDataToFrame_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
      {
        Controller.EhView_ClipDataToFrameChanged(this.m_Layer_ClipDataToFrame.Checked);
      }
    }
  }
}
