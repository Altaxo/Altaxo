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
	/// Summary description for LineScatterLayerContentsControl.
	/// </summary>
	public class LineScatterLayerContentsControl : System.Windows.Forms.UserControl, ILineScatterLayerContentsView
	{
		private ILineScatterLayerContentsController m_Ctrl;
		private System.Windows.Forms.CheckBox m_Contents_chkShowRange;
		private System.Windows.Forms.Button m_Contents_btEditRange;
		private System.Windows.Forms.Button m_Contents_btUngroup;
		private System.Windows.Forms.Button m_Contents_btGroup;
		private System.Windows.Forms.Button m_Contents_btPlotAssociations;
		private System.Windows.Forms.Button m_Contents_btListSelDown;
		private System.Windows.Forms.Button m_Contents_btListSelUp;
		private System.Windows.Forms.ListBox m_Contents_lbContents;
		private System.Windows.Forms.Button m_Contents_btPullData;
		private System.Windows.Forms.Button m_Contents_btPutData;
		private System.Windows.Forms.Label label14;
		//private System.Windows.Forms.TreeView m_Content_tvDataAvail;
		private MWControls.MWTreeView m_Content_tvDataAvail;
		private System.Windows.Forms.Label label13;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LineScatterLayerContentsControl()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LineScatterLayerContentsControl));
			this.m_Contents_chkShowRange = new System.Windows.Forms.CheckBox();
			this.m_Contents_btEditRange = new System.Windows.Forms.Button();
			this.m_Contents_btUngroup = new System.Windows.Forms.Button();
			this.m_Contents_btGroup = new System.Windows.Forms.Button();
			this.m_Contents_btPlotAssociations = new System.Windows.Forms.Button();
			this.m_Contents_btListSelDown = new System.Windows.Forms.Button();
			this.m_Contents_btListSelUp = new System.Windows.Forms.Button();
			this.m_Contents_lbContents = new System.Windows.Forms.ListBox();
			this.m_Contents_btPullData = new System.Windows.Forms.Button();
			this.m_Contents_btPutData = new System.Windows.Forms.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.m_Content_tvDataAvail = new MWControls.MWTreeView();
			this.label13 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// m_Contents_chkShowRange
			// 
			this.m_Contents_chkShowRange.Location = new System.Drawing.Point(352, 240);
			this.m_Contents_chkShowRange.Name = "m_Contents_chkShowRange";
			this.m_Contents_chkShowRange.TabIndex = 25;
			this.m_Contents_chkShowRange.Text = "Show Range";
			this.m_Contents_chkShowRange.CheckedChanged += new System.EventHandler(this.EhShowRange_CheckedChanged);
			// 
			// m_Contents_btEditRange
			// 
			this.m_Contents_btEditRange.Location = new System.Drawing.Point(352, 208);
			this.m_Contents_btEditRange.Name = "m_Contents_btEditRange";
			this.m_Contents_btEditRange.Size = new System.Drawing.Size(104, 24);
			this.m_Contents_btEditRange.TabIndex = 24;
			this.m_Contents_btEditRange.Text = "Edit Range...";
			this.m_Contents_btEditRange.Click += new System.EventHandler(this.EhEditRange_Click);
			// 
			// m_Contents_btUngroup
			// 
			this.m_Contents_btUngroup.Location = new System.Drawing.Point(352, 176);
			this.m_Contents_btUngroup.Name = "m_Contents_btUngroup";
			this.m_Contents_btUngroup.Size = new System.Drawing.Size(104, 24);
			this.m_Contents_btUngroup.TabIndex = 23;
			this.m_Contents_btUngroup.Text = "Ungroup";
			this.m_Contents_btUngroup.Click += new System.EventHandler(this.EhUngroup_Click);
			// 
			// m_Contents_btGroup
			// 
			this.m_Contents_btGroup.Location = new System.Drawing.Point(352, 144);
			this.m_Contents_btGroup.Name = "m_Contents_btGroup";
			this.m_Contents_btGroup.Size = new System.Drawing.Size(104, 24);
			this.m_Contents_btGroup.TabIndex = 22;
			this.m_Contents_btGroup.Text = "Group";
			this.m_Contents_btGroup.Click += new System.EventHandler(this.EhGroup_Click);
			// 
			// m_Contents_btPlotAssociations
			// 
			this.m_Contents_btPlotAssociations.Location = new System.Drawing.Point(352, 112);
			this.m_Contents_btPlotAssociations.Name = "m_Contents_btPlotAssociations";
			this.m_Contents_btPlotAssociations.Size = new System.Drawing.Size(104, 24);
			this.m_Contents_btPlotAssociations.TabIndex = 21;
			this.m_Contents_btPlotAssociations.Text = "PlotAssociations...";
			this.m_Contents_btPlotAssociations.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.m_Contents_btPlotAssociations.Click += new System.EventHandler(this.EhPlotAssociations_Click);
			// 
			// m_Contents_btListSelDown
			// 
			this.m_Contents_btListSelDown.Image = ((System.Drawing.Image)(resources.GetObject("m_Contents_btListSelDown.Image")));
			this.m_Contents_btListSelDown.Location = new System.Drawing.Point(352, 72);
			this.m_Contents_btListSelDown.Name = "m_Contents_btListSelDown";
			this.m_Contents_btListSelDown.Size = new System.Drawing.Size(32, 32);
			this.m_Contents_btListSelDown.TabIndex = 20;
			this.m_Contents_btListSelDown.Click += new System.EventHandler(this.EhListSelDown_Click);
			// 
			// m_Contents_btListSelUp
			// 
			this.m_Contents_btListSelUp.Image = ((System.Drawing.Image)(resources.GetObject("m_Contents_btListSelUp.Image")));
			this.m_Contents_btListSelUp.Location = new System.Drawing.Point(352, 32);
			this.m_Contents_btListSelUp.Name = "m_Contents_btListSelUp";
			this.m_Contents_btListSelUp.Size = new System.Drawing.Size(32, 32);
			this.m_Contents_btListSelUp.TabIndex = 19;
			this.m_Contents_btListSelUp.Click += new System.EventHandler(this.EhListSelUp_Click);
			// 
			// m_Contents_lbContents
			// 
			this.m_Contents_lbContents.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.m_Contents_lbContents.Location = new System.Drawing.Point(208, 24);
			this.m_Contents_lbContents.Name = "m_Contents_lbContents";
			this.m_Contents_lbContents.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.m_Contents_lbContents.Size = new System.Drawing.Size(136, 238);
			this.m_Contents_lbContents.TabIndex = 18;
			this.m_Contents_lbContents.DoubleClick += new System.EventHandler(this.EhContents_DoubleClick);
			this.m_Contents_lbContents.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.EhContents_MeasureItem);
			this.m_Contents_lbContents.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.EhContents_DrawItem);
			this.m_Contents_lbContents.SelectedIndexChanged += new System.EventHandler(this.EhContents_SelectedIndexChanged);
			// 
			// m_Contents_btPullData
			// 
			this.m_Contents_btPullData.Image = ((System.Drawing.Image)(resources.GetObject("m_Contents_btPullData.Image")));
			this.m_Contents_btPullData.Location = new System.Drawing.Point(168, 72);
			this.m_Contents_btPullData.Name = "m_Contents_btPullData";
			this.m_Contents_btPullData.Size = new System.Drawing.Size(32, 32);
			this.m_Contents_btPullData.TabIndex = 17;
			this.m_Contents_btPullData.Click += new System.EventHandler(this.EhPullData_Click);
			// 
			// m_Contents_btPutData
			// 
			this.m_Contents_btPutData.Image = ((System.Drawing.Image)(resources.GetObject("m_Contents_btPutData.Image")));
			this.m_Contents_btPutData.Location = new System.Drawing.Point(168, 32);
			this.m_Contents_btPutData.Name = "m_Contents_btPutData";
			this.m_Contents_btPutData.Size = new System.Drawing.Size(32, 32);
			this.m_Contents_btPutData.TabIndex = 16;
			this.m_Contents_btPutData.Click += new System.EventHandler(this.EhPutData_Click);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(208, 8);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(88, 16);
			this.label14.TabIndex = 15;
			this.label14.Text = "XYPlotLayer Contents";
			// 
			// m_Content_tvDataAvail
			// 
			this.m_Content_tvDataAvail.ImageIndex = -1;
			this.m_Content_tvDataAvail.Location = new System.Drawing.Point(8, 24);
			this.m_Content_tvDataAvail.Name = "m_Content_tvDataAvail";
			this.m_Content_tvDataAvail.SelectedImageIndex = -1;
			this.m_Content_tvDataAvail.Size = new System.Drawing.Size(152, 240);
			this.m_Content_tvDataAvail.TabIndex = 14;
			this.m_Content_tvDataAvail.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.EhDataAvailable_BeforeExpand);
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(8, 8);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(80, 16);
			this.label13.TabIndex = 13;
			this.label13.Text = "Available data";
			// 
			// LineScatterLayerContentsControl
			// 
			this.Controls.Add(this.m_Contents_chkShowRange);
			this.Controls.Add(this.m_Contents_btEditRange);
			this.Controls.Add(this.m_Contents_btUngroup);
			this.Controls.Add(this.m_Contents_btGroup);
			this.Controls.Add(this.m_Contents_btPlotAssociations);
			this.Controls.Add(this.m_Contents_btListSelDown);
			this.Controls.Add(this.m_Contents_btListSelUp);
			this.Controls.Add(this.m_Contents_lbContents);
			this.Controls.Add(this.m_Contents_btPullData);
			this.Controls.Add(this.m_Contents_btPutData);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.m_Content_tvDataAvail);
			this.Controls.Add(this.label13);
			this.Name = "LineScatterLayerContentsControl";
			this.Size = new System.Drawing.Size(464, 272);
			this.ResumeLayout(false);

		}
		#endregion

		#region ILineScatterLayerContentsView Members

		public ILineScatterLayerContentsController Controller
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

		public Form Form
		{
			get
			{
				return this.ParentForm;
			}
		}


		public void DataAvailable_Initialize(TreeNode[] nodes)
		{
			this.m_Content_tvDataAvail.BeginUpdate();
			// Clear the TreeView each time the method is called.
			this.m_Content_tvDataAvail.Nodes.Clear();

			this.m_Content_tvDataAvail.Nodes.AddRange(nodes);

			this.m_Content_tvDataAvail.EndUpdate();
		}

		public void DataAvailable_ClearSelection()
		{
			this.m_Content_tvDataAvail.ClearSelNodes();
		}
	
		public void Contents_SetItemCount(int newCount)
		{
			// please note:
			// every time we change the count, we have to remove all items and add them again
			// this is because the group items have another heigth, but the MeasureItem routine is
			// only called once when the item is added into the list

			this.m_Contents_lbContents.BeginUpdate();
			this.m_Contents_lbContents.Items.Clear();
			for(int i=0;i<newCount;i++)
					this.m_Contents_lbContents.Items.Add(i);

			this.m_Contents_lbContents.EndUpdate();
		}

		public void Contents_SetSelected(int idx, bool bSelect)
	{
		this.m_Contents_lbContents.SetSelected(idx,bSelect);
	}

		public void Contents_InvalidateItems(int idx1, int idx2)
		{
			this.m_Contents_lbContents.Items[idx1] = idx1;
			this.m_Contents_lbContents.Items[idx2] = idx2;

		}

		#endregion

		private int[] SelectedContents
		{
			get
			{
				int[] selidxs = new int[m_Contents_lbContents.SelectedIndices.Count];
				this.m_Contents_lbContents.SelectedIndices.CopyTo(selidxs,0);
				return selidxs;
			}
		}

		private void EhDataAvailable_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			if(null!=Controller)
			{
				this.m_Content_tvDataAvail.BeginUpdate();
				Controller.EhView_DataAvailableBeforeExpand(e.Node);
				this.m_Content_tvDataAvail.EndUpdate();
			}
		}

		private void EhContents_DoubleClick(object sender, System.EventArgs e)
		{
			if(null!=Controller)
			{
				if(this.m_Contents_lbContents.SelectedItems.Count==1)
				{
					int selidx = this.m_Contents_lbContents.SelectedIndices[0];
					Controller.EhView_ContentsDoubleClick(selidx);
				}
			}
		}

			private void EhContents_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
			{
				if(null!=Controller)
					Controller.EhView_ContentsMeasureItem(sender, e);
			}

		private void EhContents_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			if(null!=Controller)
				Controller.EhView_ContentsDrawItem(sender, e);
		}


		private void EhContents_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private void EhPutData_Click(object sender, System.EventArgs e)
		{
			if(null!=Controller)
			{
				Controller.EhView_PutData(this.m_Content_tvDataAvail.SelNodes);
			}
		}

		private void EhPullData_Click(object sender, System.EventArgs e)
		{
			if(null!=Controller)
				Controller.EhView_PullDataClick(SelectedContents);
		}

			private void EhListSelUp_Click(object sender, System.EventArgs e)
			{
		if(null!=Controller)
			Controller.EhView_ListSelUpClick(SelectedContents);
			}

		private void EhListSelDown_Click(object sender, System.EventArgs e)
		{
		if(null!=Controller)
			Controller.EhView_SelDownClick(SelectedContents);
		}

		private void EhPlotAssociations_Click(object sender, System.EventArgs e)
		{
			if(null!=Controller)
				Controller.EhView_PlotAssociationsClick(SelectedContents);
		}

		private void EhGroup_Click(object sender, System.EventArgs e)
		{
			if(null!=Controller)
				Controller.EhView_GroupClick(SelectedContents);
		
		}

		private void EhUngroup_Click(object sender, System.EventArgs e)
		{
		if(null!=Controller)
			Controller.EhView_UngroupClick(SelectedContents);
		}

		private void EhEditRange_Click(object sender, System.EventArgs e)
		{
			if(null!=Controller)
				Controller.EhView_EditRangeClick(SelectedContents);
		}

		private void EhShowRange_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		#region IMVCView Members

		public object ControllerObject
		{
			get
			{
				return Controller;
			}
			set
			{
				Controller = value as ILineScatterLayerContentsController;
			}
		}


		#endregion
	}
}
