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
using Altaxo.Serialization;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for AltaxoGraph.
	/// </summary>
	[SerializationSurrogate(0,typeof(GraphForm.SerializationSurrogate0))]
	[SerializationVersion(0,"Initial version.")]
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
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuFileExportPageWMF;
		private System.Windows.Forms.MenuItem menuGraphPopup;
		private System.Windows.Forms.MenuItem menuGraph_NewLayerLegend;
		private System.Windows.Forms.MenuItem menuFile_PrintPreview;

	

		#region Serialization
		public class SerializationSurrogate0 : IDeserializationSubstitute, System.Runtime.Serialization.ISerializationSurrogate, System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback
		{
			protected Point		m_Location;
			protected Size		m_Size;
			protected object	m_GraphControl=null;

			// we need a empty constructor
			public SerializationSurrogate0() {}

			// not used for deserialization, since the ISerializable constructor is used for that
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector){return obj;}
			// not used for serialization, instead the ISerializationSurrogate is used for that
			public void GetObjectData(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)	{}

			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				info.SetType(this.GetType());
				GraphForm s = (GraphForm)obj;
				info.AddValue("Location",s.Location);
				info.AddValue("Size",s.Size);
				info.AddValue("GraphControl",s.m_GraphControl);
				
			}

			public SerializationSurrogate0(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context)
			{
				m_Location = (Point)info.GetValue("Location",typeof(Point));
				m_Size     = (Size)info.GetValue("Size",typeof(Size));
				m_GraphControl = info.GetValue("GraphControl",typeof(object));
			}

			public void OnDeserialization(object o)
			{
			}

			public object GetRealObject(object parent)
			{
				// Create a new worksheet, parent window is the application window
				GraphForm frm = new GraphForm(App.CurrentApplication,null,null);
				frm.Location = m_Location;
				frm.Size = m_Size;

				// Change the IDeserializationSurrogate of the data grid control to the real object
				// parent of the data grid is the worksheet created above
				m_GraphControl = ((IDeserializationSubstitute)m_GraphControl).GetRealObject(frm);
				frm.GraphControl = m_GraphControl as GraphControl;
				frm.GraphControl.Size = frm.ClientSize;
				// frm.Text = frm.GraphControl.DataTable.TableName;
				m_GraphControl = null;
				return frm;
			}
		}
		#endregion




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


			// store the menu item for the data popup in the graph control so that the graph control can manage it
			this.m_GraphControl.menuDataPopup = this.menuDataPopup;
			this.m_GraphControl.UpdateDataPopup();

			this.Show();
		}


		public Altaxo.Graph.GraphControl GraphControl
		{
			get { return m_GraphControl; }
			set
			{
				if(null!=m_GraphControl)
				{
					m_GraphControl.Hide();
					m_GraphControl.Dispose();
				}
				m_GraphControl = value;
				m_GraphControl.Parent=this;
				this.m_GraphControl.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
					| System.Windows.Forms.AnchorStyles.Left) 
					| System.Windows.Forms.AnchorStyles.Right);
				this.m_GraphControl.BackColor = System.Drawing.SystemColors.AppWorkspace;
				this.m_GraphControl.TabIndex = 0;
				m_GraphControl.Size = this.Size;
				m_GraphControl.Show();
			}
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
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuFileExportPageWMF = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuNewLayer_NormalBottomXLeftY = new System.Windows.Forms.MenuItem();
			this.menuNewLayer_LinkedTopXRightY = new System.Windows.Forms.MenuItem();
			this.menuNewLayer_LinkedTopX = new System.Windows.Forms.MenuItem();
			this.menuNewLayer_LinkedRightY = new System.Windows.Forms.MenuItem();
			this.menuNewLayer_TopXRightY_XAxisStraight = new System.Windows.Forms.MenuItem();
			this.menuGraphPopup = new System.Windows.Forms.MenuItem();
			this.menuGraph_NewLayerLegend = new System.Windows.Forms.MenuItem();
			this.menuDataPopup = new System.Windows.Forms.MenuItem();
			this.menuDataSeparator = new System.Windows.Forms.MenuItem();
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
																																							this.menuGraphPopup,
																																							this.menuDataPopup});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuFile_PageSetup,
																																							this.menuFile_Print,
																																							this.menuFile_PrintPreview,
																																							this.menuItem4});
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
			// menuItem4
			// 
			this.menuItem4.Index = 3;
			this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuFileExportPageWMF});
			this.menuItem4.Text = "Export Page";
			// 
			// menuFileExportPageWMF
			// 
			this.menuFileExportPageWMF.Index = 0;
			this.menuFileExportPageWMF.Text = "Windows Metafile";
			this.menuFileExportPageWMF.Click += new System.EventHandler(this.menuFileExportPageWMF_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuItem3});
			this.menuItem2.MergeOrder = 1;
			this.menuItem2.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
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
			// menuNewLayer_TopXRightY_XAxisStraight
			// 
			this.menuNewLayer_TopXRightY_XAxisStraight.Index = 4;
			this.menuNewLayer_TopXRightY_XAxisStraight.Text = "(Linked): Top X + Right Y + X Axis Straight";
			this.menuNewLayer_TopXRightY_XAxisStraight.Click += new System.EventHandler(this.menuNewLayer_LinkedTopXRightY_XAxisStraight_Click);
			// 
			// menuGraphPopup
			// 
			this.menuGraphPopup.Index = 2;
			this.menuGraphPopup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																									 this.menuGraph_NewLayerLegend});
			this.menuGraphPopup.MergeOrder = 2;
			this.menuGraphPopup.Text = "Graph";
			// 
			// menuGraph_NewLayerLegend
			// 
			this.menuGraph_NewLayerLegend.Index = 0;
			this.menuGraph_NewLayerLegend.Text = "New Layer Legend";
			this.menuGraph_NewLayerLegend.Click += new System.EventHandler(this.menuGraph_NewLayerLegend_Click);
			// 
			// menuDataPopup
			// 
			this.menuDataPopup.Index = 3;
			this.menuDataPopup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																									this.menuDataSeparator});
			this.menuDataPopup.MergeOrder = 3;
			this.menuDataPopup.Text = "Data";

			// 
			// menuDataSeparator
			// 
			this.menuDataSeparator.Index = 0;
			this.menuDataSeparator.Text = "-";
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
			this.Closing += new System.ComponentModel.CancelEventHandler(this.GraphForm_Closing);
			this.Closed += new System.EventHandler(this.GraphForm_Closed);
			this.ResumeLayout(false);

		}
		#endregion

	

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

		private void menuFileExportPageWMF_Click(object sender, System.EventArgs e)
		{
			System.IO.Stream myStream ;
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
			saveFileDialog1.Filter = "Windows Metafiles (*.emf)|*.emf|All files (*.*)|*.*"  ;
			saveFileDialog1.FilterIndex = 2 ;
			saveFileDialog1.RestoreDirectory = true ;
 
			if(saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((myStream = saveFileDialog1.OpenFile()) != null)
				{
					this.m_GraphControl.SaveAsMetafile(myStream);
					myStream.Close();
				} // end openfile ok
			} // end dlgresult ok

		}

		private void menuGraph_NewLayerLegend_Click(object sender, System.EventArgs e)
		{
			this.m_GraphControl.menuGraph_NewLayerLegend();
		}

		private void GraphForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			System.Windows.Forms.DialogResult dlgres = System.Windows.Forms.MessageBox.Show(this,"Do you really want to close this graph?","Attention",System.Windows.Forms.MessageBoxButtons.YesNo);

			if(dlgres==System.Windows.Forms.DialogResult.No)
			{
				e.Cancel = true;
			}
		}

		private void GraphForm_Closed(object sender, System.EventArgs e)
		{
			App.document.RemoveGraph(this);
		}

	
	}
}
