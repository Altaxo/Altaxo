/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Graph;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for AltaxoGraph.
	/// </summary>
	public class GraphForm : System.Windows.Forms.Form
	{
		private AltaxoDocument parentDocument =null;

		private Altaxo.Graph.GraphControl m_GraphControl;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuDataPopup;
		private System.Windows.Forms.MenuItem menuDataSeparator;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuFile_PageSetup;
		private System.Windows.Forms.MenuItem menuFile_Print;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuNewLayer_NormalBottomXLeftY;
		private System.Windows.Forms.MenuItem menuNewLayer_LinkedTopXRightY;
		private System.Windows.Forms.MenuItem menuNewLayer_LinkedTopX;
		private System.Windows.Forms.MenuItem menuNewLayer_LinkedRightY;
		private System.Windows.Forms.MenuItem menuNewLayer_TopXRightY_XAxisStraight;
		private System.Windows.Forms.MenuItem menuFile_PrintPreview;

	

		protected void OnAMdiChildActivate(object sender, EventArgs e)
		{
			if(((System.Windows.Forms.Form)sender).ActiveMdiChild==this)
			{
				this.m_GraphControl.OnActivationOfParentForm();
			
			
					
			}
		}

		protected void OnAMdiChildDeactivate(object sender, EventArgs e)
		{
			if(((System.Windows.Forms.Form)sender).ActiveMdiChild!=this)
			{
				this.m_GraphControl.OnDeactivationOfParentForm();
			}
		}


		public GraphForm(System.Windows.Forms.Form parent, AltaxoDocument doc)
		: this(parent,doc,null)
		{
		}		

		public GraphForm(System.Windows.Forms.Form parent, AltaxoDocument doc, PlotAssociation[] pas)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();


			parentDocument = doc;
			this.MdiParent=parent;

			// register event so to be informed when activated
			if(parent is Altaxo.App)
			{
				((Altaxo.App)parent).MdiChildDeactivateBefore += new EventHandler(this.OnAMdiChildDeactivate);
				((Altaxo.App)parent).MdiChildActivateAfter += new EventHandler(this.OnAMdiChildActivate);
			}
			else
			{
				parent.MdiChildActivate += new EventHandler(this.OnAMdiChildActivate);
				parent.MdiChildActivate += new EventHandler(this.OnAMdiChildDeactivate);
			}

			this.Show();
		}


		public Altaxo.Graph.GraphControl GraphControl
		{
			get { return m_GraphControl; }
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GraphForm));
			this.m_GraphControl = new Altaxo.Graph.GraphControl();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuFile_PageSetup = new System.Windows.Forms.MenuItem();
			this.menuFile_Print = new System.Windows.Forms.MenuItem();
			this.menuFile_PrintPreview = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuNewLayer_NormalBottomXLeftY = new System.Windows.Forms.MenuItem();
			this.menuNewLayer_LinkedTopXRightY = new System.Windows.Forms.MenuItem();
			this.menuNewLayer_LinkedTopX = new System.Windows.Forms.MenuItem();
			this.menuNewLayer_LinkedRightY = new System.Windows.Forms.MenuItem();
			this.menuDataPopup = new System.Windows.Forms.MenuItem();
			this.menuDataSeparator = new System.Windows.Forms.MenuItem();
			this.menuNewLayer_TopXRightY_XAxisStraight = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// m_GraphControl
			// 
			this.m_GraphControl.AutoZoom = true;
			this.m_GraphControl.CurrentGraphTool = Altaxo.Graph.GraphControl.GraphTools.ObjectPointer;
			this.m_GraphControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_GraphControl.Name = "m_GraphControl";
			this.m_GraphControl.PageBounds = ((System.Drawing.RectangleF)(resources.GetObject("m_GraphControl.PageBounds")));
			this.m_GraphControl.PrintableBounds = ((System.Drawing.RectangleF)(resources.GetObject("m_GraphControl.PrintableBounds")));
			this.m_GraphControl.Size = new System.Drawing.Size(304, 266);
			this.m_GraphControl.TabIndex = 0;
			this.m_GraphControl.Zoom = 0.270784F;
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuItem1,
																																							this.menuItem2,
																																							this.menuDataPopup});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuFile_PageSetup,
																																							this.menuFile_Print,
																																							this.menuFile_PrintPreview});
			this.menuItem1.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuItem1.Text = "File";
			// 
			// menuFile_PageSetup
			// 
			this.menuFile_PageSetup.Index = 0;
			this.menuFile_PageSetup.Text = "Page Setup...";
			this.menuFile_PageSetup.Click += new System.EventHandler(this.menuFile_PageSetup_Click);
			// 
			// menuFile_Print
			// 
			this.menuFile_Print.Index = 1;
			this.menuFile_Print.Text = "Print...";
			this.menuFile_Print.Click += new System.EventHandler(this.menuFile_Print_Click);
			// 
			// menuFile_PrintPreview
			// 
			this.menuFile_PrintPreview.Index = 2;
			this.menuFile_PrintPreview.Text = "Print Prewiew..";
			this.menuFile_PrintPreview.Click += new System.EventHandler(this.menuFile_PrintPreview_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuItem3});
			this.menuItem2.Text = "Edit";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 0;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuNewLayer_NormalBottomXLeftY,
																																							this.menuNewLayer_LinkedTopXRightY,
																																							this.menuNewLayer_LinkedTopX,
																																							this.menuNewLayer_LinkedRightY,
																																							this.menuNewLayer_TopXRightY_XAxisStraight});
			this.menuItem3.Text = "New Layer(Axes)";
			// 
			// menuNewLayer_NormalBottomXLeftY
			// 
			this.menuNewLayer_NormalBottomXLeftY.Index = 0;
			this.menuNewLayer_NormalBottomXLeftY.Text = "(Normal): Bottom X + Left Y ";
			this.menuNewLayer_NormalBottomXLeftY.Click += new System.EventHandler(this.menuNewLayer_NormalBottomXLeftY_Click);
			// 
			// menuNewLayer_LinkedTopXRightY
			// 
			this.menuNewLayer_LinkedTopXRightY.Index = 1;
			this.menuNewLayer_LinkedTopXRightY.Text = "(Linked: Top X + Right Y";
			this.menuNewLayer_LinkedTopXRightY.Click += new System.EventHandler(this.menuNewLayer_LinkedTopXRightY_Click);
			// 
			// menuNewLayer_LinkedTopX
			// 
			this.menuNewLayer_LinkedTopX.Index = 2;
			this.menuNewLayer_LinkedTopX.Text = "(Linked): Top X";
			this.menuNewLayer_LinkedTopX.Click += new System.EventHandler(this.menuNewLayer_LinkedTopX_Click);
			// 
			// menuNewLayer_LinkedRightY
			// 
			this.menuNewLayer_LinkedRightY.Index = 3;
			this.menuNewLayer_LinkedRightY.Text = "(Linked): Right Y";
			this.menuNewLayer_LinkedRightY.Click += new System.EventHandler(this.menuNewLayer_LinkedRightY_Click);
			// 
			// menuDataPopup
			// 
			this.menuDataPopup.Index = 2;
			this.menuDataPopup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																									this.menuDataSeparator});
			this.menuDataPopup.Text = "Data";
			this.menuDataPopup.Popup += new System.EventHandler(this.menuDataPopup_Popup);
			this.menuDataPopup.Click += new System.EventHandler(this.menuDataPopup_Click);
			this.menuDataPopup.Select += new System.EventHandler(this.menuDataPopup_Select);
			// 
			// menuDataSeparator
			// 
			this.menuDataSeparator.Index = 0;
			this.menuDataSeparator.Text = "-";
			// 
			// menuNewLayer_TopXRightY_XAxisStraight
			// 
			this.menuNewLayer_TopXRightY_XAxisStraight.Index = 4;
			this.menuNewLayer_TopXRightY_XAxisStraight.Text = "(Linked): Top X + Right Y + X Axis Straight";
			this.menuNewLayer_TopXRightY_XAxisStraight.Click += new System.EventHandler(this.menuNewLayer_LinkedTopXRightY_XAxisStraight_Click);
			// 
			// GraphForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(304, 266);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.m_GraphControl});
			this.Menu = this.mainMenu1;
			this.Name = "GraphForm";
			this.Text = "AltaxoGraph";
			this.ResumeLayout(false);

		}
		#endregion

		private void menuData_Data(object sender, System.EventArgs e)
		{
			DataMenuItem dmi = (DataMenuItem)sender;

			if(!dmi.Checked)
			{
				// if the menu item was not checked before, check it now
				// by making the plot association shown by the menu item
				// the actual plot association
				int actLayerNum = this.m_GraphControl.CurrentLayerNumber;
				Layer actLayer = this.m_GraphControl.Layers[actLayerNum];
				if(null!=actLayer && dmi.tagValue<actLayer.PlotAssociations.Count)
				{
					dmi.Checked=true;
					m_GraphControl.CurrentPlotNumber = dmi.tagValue;
				}
			}
			else
			{
				// if it was checked before, then bring up the plot style dialog
				// of the plot association represented by this menu item
				int actLayerNum = this.m_GraphControl.CurrentLayerNumber;
				Layer actLayer = this.m_GraphControl.Layers[actLayerNum];
				PlotAssociation pa = actLayer.PlotAssociations[m_GraphControl.CurrentPlotNumber];


				// get plot group
				PlotGroup plotGroup = actLayer.PlotGroups.GetPlotGroupOf(pa);
				PlotStyleDialog dlg = new PlotStyleDialog(pa.PlotStyle,plotGroup);
				DialogResult dr = dlg.ShowDialog(this);
				if(dr==DialogResult.OK)
				{
					if(null!=plotGroup)
					{
						plotGroup.Style = dlg.PlotGroupStyle;
						if(plotGroup.IsIndependent)
							pa.PlotStyle = dlg.PlotStyle;
						else
						{
							plotGroup.MasterItem.PlotStyle = dlg.PlotStyle;
							plotGroup.UpdateMembers();
						}
					}
					else // pa was not member of a plot group
					{
						pa.PlotStyle = dlg.PlotStyle;
					}

						this.m_GraphControl.Invalidate(); // renew the picture
				}
			}

		}


		private void menuDataPopup_Popup(object sender, System.EventArgs e)
		{
			int actLayerNum = this.m_GraphControl.CurrentLayerNumber;
			Layer actLayer = this.m_GraphControl.Layers[actLayerNum];
			if(null==actLayer)
				return;


			// first delete old menuitems
			// then append the plot associations of the actual layer
			menuDataPopup.MenuItems.Clear();


			menuDataPopup.MenuItems.Add(this.menuDataSeparator);
			int actPA = m_GraphControl.CurrentPlotNumber;
			int len = actLayer.PlotAssociations.Count;
			for(int i = 0; i<len; i++)
			{
				PlotAssociation pa = actLayer.PlotAssociations[i];
				DataMenuItem mi = new DataMenuItem(pa.ToString(), new EventHandler(menuData_Data));
				mi.Checked = (i==actPA);
				mi.tagValue = i;
				menuDataPopup.MenuItems.Add(mi);
			}


		}

		private void menuDataPopup_Select(object sender, System.EventArgs e)
		{
		}

		private void menuDataPopup_Click(object sender, System.EventArgs e)
		{
		
		}

		private void menuFile_PageSetup_Click(object sender, System.EventArgs e)
		{
		App.CurrentApplication.PageSetupDialog.ShowDialog(this);
		}

		private void menuFile_Print_Click(object sender, System.EventArgs e)
		{
			if(DialogResult.OK==App.CurrentApplication.PrintDialog.ShowDialog(this))
			{
				try
				{
					App.CurrentApplication.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.m_GraphControl.OnPrintPage);
					App.CurrentApplication.PrintDocument.Print();
				}
				catch(Exception ex)
				{
					System.Windows.Forms.MessageBox.Show(this,ex.ToString());
				}
				finally
				{
					App.CurrentApplication.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(this.m_GraphControl.OnPrintPage);
				}
			}
		}

		private void menuFile_PrintPreview_Click(object sender, System.EventArgs e)
		{
			try
			{
				System.Windows.Forms.PrintPreviewDialog dlg = new System.Windows.Forms.PrintPreviewDialog();
				App.CurrentApplication.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.m_GraphControl.OnPrintPage);
				dlg.Document = App.CurrentApplication.PrintDocument;
				dlg.ShowDialog(this);
				dlg.Dispose();
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(this,ex.ToString());
			}
			finally
			{
				App.CurrentApplication.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(this.m_GraphControl.OnPrintPage);
			}
			}

		private void menuNewLayer_NormalBottomXLeftY_Click(object sender, System.EventArgs e)
		{
		m_GraphControl.menuNewLayer_NormalBottomXLeftY_Click(sender,e);
		}

		private void menuNewLayer_LinkedTopXRightY_Click(object sender, System.EventArgs e)
		{
			m_GraphControl.menuNewLayer_LinkedTopXRightY_Click(sender,e);
		}

		private void menuNewLayer_LinkedTopX_Click(object sender, System.EventArgs e)
		{
		
		}

		private void menuNewLayer_LinkedRightY_Click(object sender, System.EventArgs e)
		{
		
		}

		private void menuNewLayer_LinkedTopXRightY_XAxisStraight_Click(object sender, System.EventArgs e)
		{
			m_GraphControl.menuNewLayer_LinkedTopXRightY_XAxisStraight_Click(sender, e);
		}

		public class DataMenuItem : MenuItem
		{
			public int tagValue=0;

			public DataMenuItem() {}
			public DataMenuItem(string t, EventHandler e) : base(t,e) {}
		}
	}
}
