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
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Graph;
using Altaxo.Serialization;

namespace Altaxo.Graph
{
	/// <summary>
	/// GraphController is our default implementation to control a graph view.
	/// </summary>
	[SerializationSurrogate(0,typeof(GraphController.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class GraphController : IGraphController,  System.Runtime.Serialization.IDeserializationCallback
	{

		#region Member variables

		// following default unit is point (1/72 inch)
		/// <summary>For the graph elements all the units are in points. One point is 1/72 inch.</summary>
		protected const float UnitPerInch = 72;


		/// <summary>Holds the Graph document (the place were the layers, plots, graph elements... are stored).</summary>
		protected Altaxo.Graph.GraphDocument m_Graph;

		/// <summary>Holds the view (the window where the graph is visualized).</summary>
		protected IGraphView m_View;
		
		/// <summary>The main menu of this controller.</summary>
		protected System.Windows.Forms.MainMenu m_MainMenu; 
		
		/// <summary>Special menu item to show the currently available plots.</summary>
		protected System.Windows.Forms.MenuItem m_MenuDataPopup;

		/// <summary>
		/// Color for the area of the view, where there is no page.
		/// </summary>
		protected Color m_NonPageAreaColor;

		/// <summary>
		/// Brush to fill the page ground. Since the printable area is filled with another brush, in effect
		/// this brush fills only the non printable margins of the page. 
		/// </summary>
		protected BrushHolder m_PageGroundBrush;

		/// <summary>
		/// Brush to fill the printable area of the graph.
		/// </summary>
		protected BrushHolder m_PrintableAreaBrush;

		/// <summary>Current horizontal resolution of the paint method.</summary>
		protected float m_HorizRes;
		
		/// <summary>Current vertical resolution of the paint method.</summary>
		protected float m_VertRes;

		/// <summary>Current zoom factor. If AutoZoom is on, this factor is calculated automatically.</summary>
		protected float m_Zoom;
		
		/// <summary>If true, the view is zoomed so that the page fits exactly into the viewing area.</summary>
		protected bool  m_AutoZoom; // if true, the sheet is zoomed as big as possible to fit into window
		
		/// <summary>Number of the currently selected layer (or -1 if no layer is present).</summary>
		protected int m_CurrentLayerNumber;

		/// <summary>Number of the currently selected plot (or -1 if no plot is present on the layer).</summary>
		protected int m_CurrentPlotNumber;
		
		/// <summary>Currently selected GraphTool.</summary>
		protected GraphTools m_CurrentGraphTool;
		
		/// <summary>A instance of a mouse handler class that currently handles the mouse events..</summary>
		protected MouseStateHandler m_MouseState;

		/// <summary>
		/// The hashtable of the selected objects. The key is the selected object itself,
		/// the data is a int object, which stores the layer number the object belongs to.
		/// </summary>
		protected System.Collections.Hashtable m_SelectedObjects;

		/// <summary>
		/// This holds a frozen image of the graph during the moving time
		/// </summary>
		protected Bitmap m_FrozenGraph;

		/// <summary>
		/// Necessary to determine if deserialization finisher already has finished serialization.
		/// </summary>
		private object m_DeserializationSurrogate;

		#endregion Member variables

		#region Serialization
		/// <summary>Used to serialize the GraphController Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes the GraphController (version 0).
			/// </summary>
			/// <param name="obj">The GraphController to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				GraphController s = (GraphController)obj;
				info.AddValue("AutoZoom",s.m_AutoZoom);
				info.AddValue("Zoom",s.m_Zoom);
				info.AddValue("Graph",s.m_Graph);
			}
			/// <summary>
			/// Deserializes the GraphController (version 0).
			/// </summary>
			/// <param name="obj">The empty GraphController object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized GraphController.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				GraphController s = (GraphController)obj;
				s.SetMemberVariablesToDefault();

				s.m_AutoZoom = info.GetBoolean("AutoZoom");
				s.m_Zoom = info.GetSingle("Zoom");
				s.m_Graph = (GraphDocument)info.GetValue("Graph",typeof(GraphDocument));

				s.m_DeserializationSurrogate = this;
				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
			if(null!=this.m_DeserializationSurrogate && obj is DeserializationFinisher)
			{
				m_DeserializationSurrogate=null;

				// first finish the document
				DeserializationFinisher finisher = new DeserializationFinisher(this);
				
				m_Graph.OnDeserialization(finisher);


				// create the menu
				this.InitializeMenu();

				// set the menu of this class
				m_View.GraphMenu = this.m_MainMenu;


				// restore event chain to GraphDocument
				m_Graph.Changed += new EventHandler(this.EhGraph_Changed);
				m_Graph.LayerCollectionChanged += new EventHandler(this.EhGraph_LayerCollectionChanged);


				// Ensure the current layer and plot numbers are valid
				this.EnsureValidityOfCurrentLayerNumber();
				this.EnsureValidityOfCurrentPlotNumber();
			}
		}
		#endregion

		#region Constructors


		/// <summary>
		/// Set the member variables to default values. Intended only for use in constructors and deserialization code.
		/// </summary>
		protected virtual void SetMemberVariablesToDefault()
		{
			m_NonPageAreaColor = Color.Gray;
		
			m_PageGroundBrush = new BrushHolder(Color.LightGray);

			m_PrintableAreaBrush = new BrushHolder(Color.Snow);

			m_HorizRes  = 300;
		
			m_VertRes = 300;

			m_Zoom  = 0.4f;
		
			// If true, the view is zoomed so that the page fits exactly into the viewing area.</summary>
			m_AutoZoom = true; // if true, the sheet is zoomed as big as possible to fit into window
		
		
			// Number of the currently selected layer (or -1 if no layer is present).</summary>
			m_CurrentLayerNumber = -1;
		
			// Number of the currently selected plot (or -1 if no plot is present on the layer).</summary>
			m_CurrentPlotNumber = -1;
		
			// Currently selected GraphTool.</summary>
			m_CurrentGraphTool = GraphTools.ObjectPointer;
		
			// A instance of a mouse handler class that currently handles the mouse events..</summary>
			m_MouseState= new ObjectPointerMouseHandler();

			// The hashtable of the selected objects. The key is the selected object itself,
			// the data is a int object, which stores the layer number the object belongs to.
			m_SelectedObjects = new System.Collections.Hashtable();

			// This holds a frozen image of the graph during the moving time
			m_FrozenGraph=null;
		}


		/// <summary>
		/// Creates a GraphController for control of the View <paramref	name="view"/>. 
		/// Also creates a default <see cref="GraphDocument"/> for use by this controller.
		/// </summary>
		/// <param name="view">The view this controller has to control.</param>
		public GraphController(IGraphView view)
			: this(view, null)
		{
		}

		/// <summary>
		/// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/> into the 
		/// View <paramref name="view"/>.
		/// </summary>
		/// <param name="view">The view to show the graph into.</param>
		/// <param name="graphdoc">The graph which holds the graphical elements.</param>
		public GraphController(IGraphView view, GraphDocument graphdoc)
		{
			SetMemberVariablesToDefault();

			
			m_View = view;
			m_View.Controller = this;

			if(null!=graphdoc)
				this.m_Graph = graphdoc;
			else
				this.m_Graph = new GraphDocument();

			this.InitializeMenu();

			
			// Adjust the zoom level just so, that area fits into control
			Graphics grfx = m_View.CreateGraphGraphics();
			this.m_HorizRes = grfx.DpiX;
			this.m_VertRes = grfx.DpiY;
			grfx.Dispose();

			if(null!=App.Current) // if we are at design time, this is null and we use the default values above
			{
				System.Drawing.Printing.PrintDocument doc = App.Current.PrintDocument;
			
				// Test whether or not a printer is installed
				System.Drawing.Printing.PrinterSettings prnset = new System.Drawing.Printing.PrinterSettings();
				RectangleF pageBounds;
				System.Drawing.Printing.Margins ma;
				if(prnset.IsValid)
				{
					pageBounds = doc.DefaultPageSettings.Bounds;
					ma = doc.DefaultPageSettings.Margins;
				}
				else // obviously no printer installed, use A4 size (sorry, this is european size)
				{
					pageBounds = new RectangleF(0,0,1169,826);
					ma = new System.Drawing.Printing.Margins(50,50,50,50);
				}
				// since Bounds are in 100th inch, we have to adjust them to points (72th inch)
				pageBounds.X *= UnitPerInch/100;
				pageBounds.Y *= UnitPerInch/100;
				pageBounds.Width *= UnitPerInch/100;
				pageBounds.Height *= UnitPerInch/100;

				RectangleF printableBounds = new RectangleF();
				printableBounds.X			= ma.Left * UnitPerInch/100;
				printableBounds.Y			= ma.Top * UnitPerInch/100;
				printableBounds.Width	= pageBounds.Width - ((ma.Left+ma.Right)*UnitPerInch/100);
				printableBounds.Height = pageBounds.Height - ((ma.Top+ma.Bottom)*UnitPerInch/100);
			
				m_Graph.Changed += new EventHandler(this.EhGraph_Changed);
				m_Graph.LayerCollectionChanged += new EventHandler(this.EhGraph_LayerCollectionChanged);
				m_Graph.PageBounds = pageBounds;
				m_Graph.PrintableBounds = printableBounds;
			}

			if(0==m_Graph.Layers.Count)
				m_Graph.CreateNewLayerNormalBottomXLeftY();


			// Calculate the zoom if Autozoom is on - simulate a SizeChanged event of the view to force calculation of new zoom factor
			this.EhView_GraphPanelSizeChanged(new EventArgs());

			// set the menu of this class
			m_View.GraphMenu = this.m_MainMenu;
			m_View.NumberOfLayers = m_Graph.Layers.Count; // tell the view how many layers we have
		}

		#endregion // Constructors

		#region Menu Definition


		/// <summary>
		/// Creates the default menu of a graph view.
		/// </summary>
		/// <remarks>In case there is already a menu here, the old menu is overwritten.</remarks>
		public void InitializeMenu()
		{
			int index=0, index2=0;
			MenuItem mi;

			m_MainMenu = new MainMenu();

			// File Menu
			// **********************************************************
			mi = new MenuItem("&File");
			mi.Index=0;
			mi.MergeOrder=0;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;

			// File - Page Setup
			mi = new MenuItem("Page Setup..");
			mi.Click += new EventHandler(EhMenuFilePageSetup_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// File - Print Preview
			mi = new MenuItem("Print Preview..");
			mi.Click += new EventHandler(EhMenuFilePrintPreview_OnClick);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// File - Print 
			mi = new MenuItem("Print..");
			mi.Click += new EventHandler(EhMenuFilePrint_OnClick);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// File - Export (Popup)
			// ------------------------------------------------------------------
			mi = new MenuItem("Export");
			//mi.Popup += new EventHandler(MenuFileExport_OnPopup);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
			index2 = m_MainMenu.MenuItems[index].MenuItems.Count-1;

			// File - Export - Metafile 
			mi = new MenuItem("Metafile");
			mi.Click += new EventHandler(EhMenuFileExportMetafile_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);


			// Edit (Popup)
			// ****************************************************************** 
			mi = new MenuItem("Edit");
			mi.Index=1;
			mi.MergeOrder=1;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;

			// Edit - NewLayer (Popup)
			// ------------------------------------------------------------------
			mi = new MenuItem("New layer(axes)");
			//mi.Popup += new EventHandler(MenuFileExport_OnPopup);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
			index2 = m_MainMenu.MenuItems[index].MenuItems.Count-1;

			// Edit - NewLayer - Normal:Bottom X and Left Y 
			mi = new MenuItem("(Normal): Bottom X + Left Y ");
			mi.Click += new EventHandler(EhMenuEditNewlayerNormalBottomXLeftY_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

			// Edit - NewLayer - "(Linked: Top X + Right Y" 
			mi = new MenuItem("(Linked: Top X + Right Y");
			mi.Click += new EventHandler(EhMenuEditNewlayerLinkedTopXRightY_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

			// Edit - NewLayer - "(Linked): Top X" 
			mi = new MenuItem("(Linked): Top X");
			mi.Click += new EventHandler(EhMenuEditNewlayerLinkedTopX_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

			// Edit - NewLayer - "(Linked): Right Y" 
			mi = new MenuItem("(Linked): Right Y");
			mi.Click += new EventHandler(EhMenuEditNewlayerLinkedRightY_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

			// Edit - NewLayer - "(Linked): Top X + Right Y + X Axis Straight" 
			mi = new MenuItem("(Linked): Top X + Right Y + X Axis Straight");
			mi.Click += new EventHandler(EhMenuEditNewlayerLinkedTopXRightYXAxisStraight_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);


			// Data (Popup)
			// ****************************************************************** 
			mi = new MenuItem("Data");
			mi.Index=3;
			mi.MergeOrder=3;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MenuDataPopup = mi; // store this for later manimpulation
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;


			// Graph (Popup)
			// ****************************************************************** 
			mi = new MenuItem("Graph");
			mi.Index=4;
			mi.MergeOrder=4;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;

			// Graph - Duplicate
			mi = new MenuItem("Duplicate Graph");
			mi.Click += new EventHandler(EhMenuGraphDuplicate_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Graph - NewLayerLegend
			mi = new MenuItem("New layer legend");
			mi.Click += new EventHandler(EhMenuGraphNewLayerLegend_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
		}


		/// <summary>
		/// Updates a special menu item, the data item, with the currently available plot names. The active plot is marked with a
		/// check.
		/// </summary>
		public void UpdateDataPopup()
		{
			if(null==this.m_MenuDataPopup)
				return; // as long there is no menu, we cannot do it

			// first delete old menuitems
			this.m_MenuDataPopup.MenuItems.Clear();


			// check there is at least one layer
			if(m_Graph.Layers.Count==0)
				return; // there is no layer, we can not have items in the data menu

			// now it is save to get the active layer
			int actLayerNum = this.CurrentLayerNumber;
			Layer actLayer = this.Layers[actLayerNum];

			// then append the plot associations of the actual layer

			int actPA = CurrentPlotNumber;
			int len = actLayer.PlotItems.Count;
			for(int i = 0; i<len; i++)
			{
				PlotItem pa = actLayer.PlotItems[i];
				DataMenuItem mi = new DataMenuItem(pa.ToString(), new EventHandler(EhMenuData_Data));
				mi.Checked = (i==actPA);
				mi.PlotItemNumber = i;
				this.m_MenuDataPopup.MenuItems.Add(mi);
			}
		}




		#endregion // Menu definition

		#region Menu event handlers

		/// <summary>
		/// Handler for the menu item "File" - "Setup Page".
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">Not used.</param>
		private void EhMenuFilePageSetup_OnClick(object sender, System.EventArgs e)
		{
			try
			{
				App.Current.PageSetupDialog.ShowDialog(this.m_View.Window);
			}
			catch(Exception exc)
			{
				MessageBox.Show(exc.ToString(),"Exception occured!");
			}
		}

		/// <summary>
		/// Handler for the menu item "File" - "Print".
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">Not used.</param>
		private void EhMenuFilePrint_OnClick(object sender, System.EventArgs e)
		{
			try
			{
				if(DialogResult.OK==App.Current.PrintDialog.ShowDialog(this.m_View.Window))
				{
					App.Current.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
					App.Current.PrintDocument.Print();
				}
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(this.m_View.Window,ex.ToString());
			}
			finally
			{
				App.Current.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
			}
		}
	

		/// <summary>
		/// Handler for the menu item "File" - "Print Preview".
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">Not used.</param>
		private void EhMenuFilePrintPreview_OnClick(object sender, System.EventArgs e)
		{
			try
			{
				System.Windows.Forms.PrintPreviewDialog dlg = new System.Windows.Forms.PrintPreviewDialog();
				App.Current.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
				dlg.Document = App.Current.PrintDocument;
				dlg.ShowDialog(this.m_View.Window);
				dlg.Dispose();
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(this.m_View.Window,ex.ToString());
			}
			finally
			{
				App.Current.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
			}
		}


		/// <summary>
		/// Handler for the menu item "File" - "Export Metafile".
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">Not used.</param>
		private void EhMenuFileExportMetafile_OnClick(object sender, System.EventArgs e)
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
					this.SaveAsMetafile(myStream);
					myStream.Close();
				} // end openfile ok
			} // end dlgresult ok

		}


		/// <summary>
		/// Handler for the menu item "Edit" - "New layer(axes)" - "Normal: Bottom X Left Y".
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">Not used.</param>
		private void EhMenuEditNewlayerNormalBottomXLeftY_OnClick(object sender, System.EventArgs e)
		{
			m_Graph.CreateNewLayerNormalBottomXLeftY();
		}

		/// <summary>
		/// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X Right Y".
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">Not used.</param>
		private void EhMenuEditNewlayerLinkedTopXRightY_OnClick(object sender, System.EventArgs e)
		{
			m_Graph.CreateNewLayerLinkedTopXRightY(CurrentLayerNumber);
		}

		/// <summary>
		/// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X".
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">Not used.</param>
		private void EhMenuEditNewlayerLinkedTopX_OnClick(object sender, System.EventArgs e)
		{
		
		}

		/// <summary>
		/// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Right Y".
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">Not used.</param>
		private void EhMenuEditNewlayerLinkedRightY_OnClick(object sender, System.EventArgs e)
		{
		
		}



		/// <summary>
		/// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X Right Y, X axis straight ".
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">Not used.</param>
		private void EhMenuEditNewlayerLinkedTopXRightYXAxisStraight_OnClick(object sender, System.EventArgs e)
		{
			m_Graph.CreateNewLayerLinkedTopXRightY_XAxisStraight(CurrentLayerNumber);
		}


		/// <summary>
		/// Duplicates the Graph and the Graph view to a new one.
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">Not used.</param>
		private void EhMenuGraphDuplicate_OnClick(object sender, System.EventArgs e)
		{
			GraphView newView = new GraphView(View.Form.ParentForm,null);
			GraphDocument newDoc = new GraphDocument(this.Doc);
			GraphController newCtrl = new GraphController(newView,newDoc);
			App.Current.Doc.AddGraph(newView);
		}



		/// <summary>
		/// Handler for the menu item "Graph" - "New layer legend.
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">Not used.</param>
		private void EhMenuGraphNewLayerLegend_OnClick(object sender, System.EventArgs e)
		{
			m_Graph.Layers[CurrentLayerNumber].CreateNewLayerLegend();
		}


		/// <summary>
		/// Handler for all submenu items of the data popup.".
		/// </summary>
		/// <param name="sender">The menuitem, must be of type <see cref="DataMenuItem"/>.</param>
		/// <param name="e">Not used.</param>
		/// <remarks>The handler either checks the menuitem, if it was unchecked. If it was already checked,
		/// it shows the <see cref="PlotStyleDialog"/> dialog box.
		/// </remarks>
		private void EhMenuData_Data(object sender, System.EventArgs e)
		{
			DataMenuItem dmi = (DataMenuItem)sender;

			if(!dmi.Checked)
			{
				// if the menu item was not checked before, check it now
				// by making the plot association shown by the menu item
				// the actual plot association
				int actLayerNum = this.CurrentLayerNumber;
				Layer actLayer = this.Layers[actLayerNum];
				if(null!=actLayer && dmi.PlotItemNumber<actLayer.PlotItems.Count)
				{
					dmi.Checked=true;
					CurrentPlotNumber = dmi.PlotItemNumber;
				}
			}
			else
			{
				// if it was checked before, then bring up the plot style dialog
				// of the plot association represented by this menu item
				int actLayerNum = this.CurrentLayerNumber;
				Layer actLayer = this.Layers[actLayerNum];
				PlotItem pa = actLayer.PlotItems[CurrentPlotNumber];


				// get plot group
				PlotGroup plotGroup = actLayer.PlotGroups.GetPlotGroupOf(pa);
				PlotStyleDialog dlg = new PlotStyleDialog((PlotStyle)pa.Style,plotGroup);
				DialogResult dr = dlg.ShowDialog(this.m_View.Window);
				if(dr==DialogResult.OK)
				{
					if(null!=plotGroup)
					{
						plotGroup.Style = dlg.PlotGroupStyle;
						if(plotGroup.IsIndependent)
							pa.Style = dlg.PlotStyle;
						else
						{
							plotGroup.MasterItem.Style = dlg.PlotStyle;
							plotGroup.UpdateMembers();
						}
					}
					else // pa was not member of a plot group
					{
						pa.Style = dlg.PlotStyle;
					}

					// this.InvalidateGraph(); // renew the picture
				}
			}

		}


		
		#endregion // Menu event handlers

		#region IGraphController interface definitions

		/// <summary>
		/// This returns the GraphDocument that is managed by this controller.
		/// </summary>
		public GraphDocument Doc
		{
			get { return m_Graph; }
		}

		/// <summary>
		/// Returns the view that this controller controls.
		/// </summary>
		/// <remarks>Setting the view is only neccessary on deserialization, so the controller
		/// can restrict setting the view only the own view is still null.</remarks>
		public IGraphView View
		{
			get { return m_View; }
			set
			{
				if(value==null)
					throw new ArgumentNullException("The view to be set must not be null!");

				IGraphView oldView = m_View;
				m_View = value;

				if(null!=oldView)
				{
					oldView.GraphMenu = null; // don't let the old view have the menu
					oldView.Controller = null; // no longer the controller of this view
				}

				m_View.Controller = this;
				m_View.GraphMenu = m_MainMenu;
				m_View.NumberOfLayers = m_Graph.Layers.Count;
				m_View.CurrentLayer = this.CurrentLayerNumber;
				m_View.CurrentGraphTool = this.CurrentGraphTool;
			}
		}

		/// <summary>
		/// Handles the selection of the current layer by the <b>user</b>.
		/// </summary>
		/// <param name="currLayer">The current layer number as selected by the user.</param>
		/// <param name="bAlternative">Normally false, can be set to true if the user clicked for instance with the right mouse button on the layer button.</param>
		public virtual void EhView_CurrentLayerChoosen(int currLayer, bool bAlternative)
		{
			int oldCurrLayer = this.CurrentLayerNumber;
			this.CurrentLayerNumber = currLayer;


			// if we have clicked the button already down then open the layer dialog
			if(null!=ActiveLayer && currLayer==oldCurrLayer && false==bAlternative)
			{
				LayerDialog dlg = new LayerDialog(ActiveLayer,LayerDialog.Tab.Scale,EdgeType.Bottom);
				dlg.ShowDialog(this.m_View.Window);
			}
		}

		/// <summary>
		/// The controller should show a data context menu (contains all plots of the currentLayer).
		/// </summary>
		/// <param name="currLayer">The layer number. The controller has to make this number the CurrentLayerNumber.</param>
		/// <param name="parent">The parent control which is the parent of the context menu.</param>
		/// <param name="pt">The location where the context menu should be shown.</param>
		public virtual void EhView_ShowDataContextMenu(int currLayer, System.Windows.Forms.Form parent, Point pt)
		{
			int oldCurrLayer = this.CurrentLayerNumber;
			this.CurrentLayerNumber = currLayer;


			if(null!=this.ActiveLayer)
			{
				// then append the plot associations of the actual layer
				ContextMenu contextMenu = new ContextMenu();

				int actPA = CurrentPlotNumber;
				int len = ActiveLayer.PlotItems.Count;
				for(int i = 0; i<len; i++)
				{
					PlotItem pa = ActiveLayer.PlotItems[i];
					DataMenuItem mi = new DataMenuItem(pa.ToString(), new EventHandler(EhMenuData_Data));
					mi.Checked = (i==actPA);
					mi.PlotItemNumber = i;
					contextMenu.MenuItems.Add(mi);
						
				}
				contextMenu.Show(parent,pt);
			}
		}

		/// <summary>
		/// This function is called if the user changed the GraphTool.
		/// </summary>
		/// <param name="currGraphTool">The new selected GraphTool.</param>
		/// <remarks>The view should not reflect the newly selected graph tool. This should only be done if the view
		/// receives the currently selected graphtool by setting its <see cref="IGraphView.CurrentGraphTool"/> property.
		/// In case radio buttons are used, they should not push itself (autopush or similar should be disabled).</remarks>
		public virtual void EhView_CurrentGraphToolChoosen(GraphTools currGraphTool)
		{
			this.CurrentGraphTool = currGraphTool;
		}

		/// <summary>
		/// Handles the event when the graph view is about to be closed.
		/// </summary>
		/// <param name="e">CancelEventArgs.</param>
		public virtual void EhView_Closing(System.ComponentModel.CancelEventArgs e)
		{
			System.Windows.Forms.DialogResult dlgres = System.Windows.Forms.MessageBox.Show(this.m_View.Window,"Do you really want to close this graph?","Attention",System.Windows.Forms.MessageBoxButtons.YesNo);

			if(dlgres==System.Windows.Forms.DialogResult.No)
			{
				e.Cancel = true;
			}
		}

		/// <summary>
		/// Handles the event when the graph view is closed.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		public virtual void EhView_Closed(System.EventArgs e)
		{
			App.Current.Doc.RemoveGraph(this.m_View.Form);
		}


		/// <summary>
		/// Handles the event when the size of the graph area is changed.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		public virtual void EhView_GraphPanelSizeChanged(EventArgs e)
		{
			if(m_AutoZoom)
			{
				this.m_Zoom = CalculateAutoZoom();
		
				// System.Console.WriteLine("h={0}, v={1} {3} {4} {5}",zoomh,zoomv,UnitPerInch,this.ClientSize.Width,this.m_HorizRes, this.m_PageBounds.Width);
				// System.Console.WriteLine("SizeX = {0}, zoom = {1}, dpix={2},in={3}",this.ClientSize.Width,this.m_Zoom,this.m_HorizRes,this.ClientSize.Width/(this.m_HorizRes*this.m_Zoom));
			
				m_View.GraphScrollSize= new Size(0,0);
			}
			else
			{
				double pixelh = System.Math.Ceiling(m_Graph.PageBounds.Width*this.m_HorizRes*this.m_Zoom/(UnitPerInch));
				double pixelv = System.Math.Ceiling(m_Graph.PageBounds.Height*this.m_VertRes*this.m_Zoom/(UnitPerInch));
				m_View.GraphScrollSize = new Size((int)pixelh,(int)pixelv);
			}

		}


		/// <summary>
		/// Handles the mouse up event onto the graph in the controller class.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		public virtual void EhView_GraphPanelMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			m_MouseState = m_MouseState.OnMouseUp(this,e);
		}

		/// <summary>
		/// Handles the mouse down event onto the graph in the controller class.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		public virtual void EhView_GraphPanelMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			m_MouseState = m_MouseState.OnMouseDown(this,e);
		}

		/// <summary>
		/// Handles the mouse move event onto the graph in the controller class.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		public virtual void EhView_GraphPanelMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			m_MouseState = this.m_MouseState.OnMouseMove(this,e);
		}

		/// <summary>
		/// Handles the click onto the graph event in the controller class.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		public virtual void EhView_GraphPanelMouseClick(System.EventArgs e)
		{
			m_MouseState = m_MouseState.OnClick(this,e);
		}

		/// <summary>
		/// Handles the double click onto the graph event in the controller class.
		/// </summary>
		/// <param name="e"></param>
		public virtual void EhView_GraphPanelMouseDoubleClick(System.EventArgs e)
		{
			m_MouseState = m_MouseState.OnDoubleClick(this,e);
		}

		/// <summary>
		/// Handles the paint event of that area, where the graph is shown.
		/// </summary>
		/// <param name="e">The paint event args.</param>
		public virtual void EhView_GraphPanelPaint(System.Windows.Forms.PaintEventArgs e)
		{
			this.DoPaint(e.Graphics,false);
		}

		#endregion // IGraphView interface definitions

		#region GraphDocument event handlers

		/// <summary>
		/// Handler of the event LayerCollectionChanged of the graph document. Forces to
		/// check the LayerButtonBar to keep track that the number of buttons match the number of layers.</summary>
		/// <param name="sender">The sender of the event (the GraphDocument).</param>
		/// <param name="e">The event arguments.</param>
		protected void EhGraph_LayerCollectionChanged(object sender, System.EventArgs e)
		{
			int oldActiveLayer = this.m_CurrentLayerNumber;

			// Ensure that the current layer and current plot are valid anymore
			EnsureValidityOfCurrentLayerNumber();

			if(oldActiveLayer!=this.m_CurrentLayerNumber)
				m_View.CurrentLayer = this.m_CurrentLayerNumber;

			// even if the active layer number not changed, it can be that the layer itself has changed from
			// one to another, so make sure that the current plot number is valid also
			EnsureValidityOfCurrentPlotNumber();

			// make sure the view knows about when the number of layers changed
			m_View.NumberOfLayers = m_Graph.Layers.Count;
		}


		/// <summary>
		/// Called if something in the <see cref="GraphDocument"/> changed.
		/// </summary>
		/// <param name="sender">Not used (always the GraphDocument).</param>
		/// <param name="e">The EventArgs.</param>
		protected void EhGraph_Changed(object sender, System.EventArgs e)
		{
			// if something changed on the graph, make sure that the layer and plot number reflect this changed
			this.EnsureValidityOfCurrentLayerNumber();
			this.EnsureValidityOfCurrentPlotNumber();
			this.m_View.InvalidateGraph();
		}

		#endregion // GraphDocument event handlers

		#region Other event handlers
		
		/// <summary>
		/// Called from the system to print out a page.
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="ppea">PrintPageEventArgs used to retrieve the graphic context for printing.</param>
		public void EhPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs ppea)
		{
			Graphics g = ppea.Graphics;
			DoPaint(g,true);
		}
		
		#endregion

		#region Methods

		/// <summary>
		/// Saves the graph as an enhanced windows metafile into the stream <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream to save the metafile into.</param>
		/// <returns>Null if successfull, error description otherwise.</returns>
		public string SaveAsMetafile(System.IO.Stream stream)
		{
			// Code to write the stream goes here.
			Graphics grfx = m_View.CreateGraphGraphics();
			IntPtr ipHdc = grfx.GetHdc();
			System.Drawing.Imaging.Metafile mf = new System.Drawing.Imaging.Metafile(stream,ipHdc);
			grfx.ReleaseHdc(ipHdc);
			grfx.Dispose();
			grfx = Graphics.FromImage(mf);
					
			this.m_Graph.DoPaint(grfx,true);

			grfx.Dispose();
			
			return null;
		}

		/// <summary>
		/// Central routine for painting the graph. The painting can either be on the screen (bForPrinting=false), or
		/// on a printer or file (bForPrinting=true).
		/// </summary>
		/// <param name="g">The graphics context painting to.</param>
		/// <param name="bForPrinting">If true, margins and background are not painted, as is usefull for printing.
		/// Also, if true, the scale is temporarely set to 1.</param>
		private void DoPaint(Graphics g, bool bForPrinting)
		{
			try
			{
				// g.SmoothingMode = SmoothingMode.AntiAlias;
				// get the dpi settings of the graphics context,
				// for example; 96dpi on screen, 600dpi for the printer
				// used to adjust grid and margin sizing.
				this.m_HorizRes = g.DpiX;
				this.m_VertRes = g.DpiY;

				g.PageUnit = GraphicsUnit.Point;
				
				if(bForPrinting)
					g.PageScale=1;
				else
					g.PageScale = this.m_Zoom;

				float pointsh = UnitPerInch*m_View.GraphScrollPosition.X/(this.m_HorizRes*this.m_Zoom);
				float pointsv = UnitPerInch*m_View.GraphScrollPosition.Y/(this.m_VertRes*this.m_Zoom);
				g.TranslateTransform(pointsh,pointsv);

				if(!bForPrinting)
				{
					g.Clear(this.m_NonPageAreaColor);
					// Fill the page with its own color
					g.FillRectangle(m_PageGroundBrush,m_Graph.PageBounds);
					g.FillRectangle(m_PrintableAreaBrush,m_Graph.PrintableBounds);
					// DrawMargins(g);
				}

				System.Console.WriteLine("Paint with zoom {0}",this.m_Zoom);
				// handle the possibility that the viewport is scrolled,
				// adjust my origin coordintates to compensate
				Point pt = m_View.GraphScrollPosition;
				

				// Paint the graph now
				g.TranslateTransform(m_Graph.PrintableBounds.X,m_Graph.PrintableBounds.Y); // translate the painting to the printable area
				m_Graph.DoPaint(g,bForPrinting);

				// finally, mark the selected objects
				if(!bForPrinting && m_SelectedObjects.Count>0)
				{
					foreach(GraphObject graphObject in m_SelectedObjects.Keys)
					{
						int nLayer = (int)m_SelectedObjects[graphObject];
						g.DrawPath(Pens.Blue,Layers[nLayer].LayerToGraphCoordinates(graphObject.GetSelectionPath()));
					}
				}
			}
			catch(System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(this.m_View.Window,ex.ToString());
			}

		}


		#endregion // Methods

		#region Properties
		/// <summary>
		/// Get / sets the currently active GraphTool.
		/// </summary>
		public GraphTools CurrentGraphTool
		{
			get
			{
				return m_CurrentGraphTool; 
			}
			set 
			{
				m_CurrentGraphTool = value;

				// select the appropriate mouse handler
				switch(m_CurrentGraphTool)
				{
					case GraphTools.ObjectPointer:
						if(!(m_MouseState is ObjectPointerMouseHandler))
							m_MouseState = new ObjectPointerMouseHandler();
						break;
					case GraphTools.Text:
						if(!(m_MouseState is TextToolMouseHandler))
							m_MouseState = new TextToolMouseHandler();
						break;
				}

				// we set the current graph tool at the view at the very end, since in the meantime (by the mousehandler)
				// the tool can have changed and is no longer <value>
				m_View.CurrentGraphTool = m_CurrentGraphTool;
			}
		}

		/// <summary>
		/// Returns the layer collection. Is the same as m_GraphDocument.Layers.
		/// </summary>
		public Layer.LayerCollection Layers
		{
			get { return m_Graph.Layers; }
		}


		/// <summary>
		/// Returns the currently active layer, or null if there is no active layer.
		/// </summary>
		public Layer ActiveLayer
		{
			get
			{
				return this.m_CurrentLayerNumber<0 ? null : m_Graph.Layers[this.m_CurrentLayerNumber]; 
			}			
		}

		/// <summary>
		/// check the validity of the CurrentLayerNumber and correct it
		/// </summary>
		public void EnsureValidityOfCurrentLayerNumber()
		{
			if(m_Graph.Layers.Count>0) // if at least one layer is present
			{
				if(m_CurrentLayerNumber<0)
					CurrentLayerNumber=0;
				else if(m_CurrentLayerNumber>=m_Graph.Layers.Count)
					CurrentLayerNumber=m_Graph.Layers.Count-1;
			}
			else // no layers present
			{
				if(-1!=m_CurrentLayerNumber)
					CurrentLayerNumber=-1;
			}
		}

		/// <summary>
		/// Get / sets the currently active layer by number.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CurrentLayerNumber
		{
			get
			{
				EnsureValidityOfCurrentLayerNumber();
				return m_CurrentLayerNumber;
			}
			set
			{
				int oldValue = this.m_CurrentLayerNumber;

				// negative values are only accepted if there is no layer
				if(value<0 && m_Graph.Layers.Count>0)
					throw new ArgumentOutOfRangeException("CurrentLayerNumber",value,"Accepted values must be >=0 if there is at least one layer in the graph!");

				if(value>=m_Graph.Layers.Count)
					throw new ArgumentOutOfRangeException("CurrentLayerNumber",value,"Accepted values must be less than the number of layers in the graph(currently " + m_Graph.Layers.Count.ToString() + ")!");

				m_CurrentLayerNumber = value<0 ? -1 : value;

				// if something changed
				if(oldValue!=this.m_CurrentLayerNumber)
				{
					// reflect the change in layer number in the layer tool bar
					m_View.CurrentLayer = this.m_CurrentLayerNumber;

					// since the layer changed, also the plots changed, so the menu has
					// to reflect the new plots
					this.UpdateDataPopup();
				}
			}
		}


		/// <summary>
		/// This ensures that the current plot number is valid. If there is no plot on the currently active layer,
		/// the current plot number is set to -1.
		/// </summary>
		public void EnsureValidityOfCurrentPlotNumber()
		{
			EnsureValidityOfCurrentLayerNumber();

			// if Layer don't exist anymore, correct CurrentLayerNumber and ActualPlotAssocitation
			if(null!=ActiveLayer) // if the ActiveLayer exists
			{
				// if the PlotAssociation don't exist anymore, correct it
				if(ActiveLayer.PlotItems.Count>0) // if at least one plotitem exists
				{
					if(m_CurrentPlotNumber<0)
						CurrentPlotNumber=0;
					else if(m_CurrentPlotNumber>ActiveLayer.PlotItems.Count)
						CurrentPlotNumber = 0;
				}
				else
				{
					if(-1!=m_CurrentPlotNumber)
						CurrentPlotNumber=-1;
				}
			}
			else // if no layer anymore
			{
				if(-1!=m_CurrentPlotNumber)
					CurrentPlotNumber=-1;
			}
		}

		/// <summary>
		/// Get / sets the currently active plot by number.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CurrentPlotNumber 
		{
			get 
			{
				return m_CurrentPlotNumber;
			}
			set
			{
				if(CurrentLayerNumber>=0 && 0!=this.m_Graph[CurrentLayerNumber].PlotItems.Count && value<0)
					throw new ArgumentOutOfRangeException("CurrentPlotNumber",value,"CurrentPlotNumber has to be greater or equal than zero");

				if(CurrentLayerNumber>=0 && value>=m_Graph[CurrentLayerNumber].PlotItems.Count)
					throw new ArgumentOutOfRangeException("CurrentPlotNumber",value,"CurrentPlotNumber has to  be lesser than actual count: " + m_Graph[CurrentLayerNumber].PlotItems.Count.ToString());

				m_CurrentPlotNumber = value<0 ? -1 : value;

				this.UpdateDataPopup();
			}
		}

		#endregion // Properties

		#region Scaling and Positioning

		/// <summary>
		/// Zoom value of the graph view.
		/// </summary>
		public float Zoom
		{
			get
			{
				return m_Zoom;
			}
			set
			{
				if( value > 0.05 )
					m_Zoom = value;
				else
					m_Zoom = 0.05f;
			} 
		}

		/// <summary>
		/// Enables / disable the autozoom feature.
		/// </summary>
		/// <remarks>If autozoom is enables, the zoom factor is calculated depending on the size of the
		/// graph view so that the graph fits best possible inside the view.</remarks>
		public bool AutoZoom
		{
			get
			{
				return this.m_AutoZoom;
			}
			set
			{
				this.m_AutoZoom = value;
				if(this.m_AutoZoom)
				{
					this.m_Zoom = CalculateAutoZoom();
					m_View.GraphScrollSize = new Size(0,0);
					m_View.InvalidateGraph();
				}
			}
		}

		/// <summary>
		/// Factor for horizontal conversion of page units (points=1/72 inch) to pixel.
		/// The resolution used for this is <see cref="m_HorizRes"/>.
		/// </summary>
		/// <returns>The factor described above.</returns>
		public float HorizFactorPageToPixel()
		{
			return this.m_HorizRes*this.m_Zoom/UnitPerInch;
		}

		/// <summary>
		/// Factor for vertical conversion of page units (points=1/72 inch) to pixel.
		/// The resolution used for this is <see cref="m_VertRes"/>.
		/// </summary>
		/// <returns>The factor described above.</returns>
		public float VertFactorPageToPixel()
		{
			return this.m_VertRes*this.m_Zoom/UnitPerInch;
		}

		/// <summary>
		/// Converts page coordinates (in points=1/72 inch) to pixel units. Uses the resolutions <see cref="m_HorizRes"/>
		/// and <see cref="m_VertRes"/> for calculation-
		/// </summary>
		/// <param name="pagec">The page coordinates to convert.</param>
		/// <returns>The coordinates as pixel coordinates.</returns>
		public PointF PageCoordinatesToPixel(PointF pagec)
		{
			return new PointF(pagec.X*HorizFactorPageToPixel(),pagec.Y*VertFactorPageToPixel());
		}

		/// <summary>
		/// Converts pixel coordinates to page coordinates (in points=1/72 inch). Uses the resolutions <see cref="m_HorizRes"/>
		/// and <see cref="m_VertRes"/> for calculation-
		/// </summary>
		/// <param name="pixelc">The pixel coordinates to convert.</param>
		/// <returns>The coordinates as page coordinates (points=1/72 inch).</returns>
		public PointF PixelToPageCoordinates(PointF pixelc)
		{
			return new PointF(pixelc.X/HorizFactorPageToPixel(),pixelc.Y/VertFactorPageToPixel());
		}

		/// <summary>
		/// Converts x,y differences in pixels to the corresponding
		/// differences in page coordinates
		/// </summary>
		/// <param name="pixeldiff">X,Y differences in pixel units</param>
		/// <returns>X,Y differences in page coordinates</returns>
		public PointF PixelToPageDifferences(PointF pixeldiff)
		{
			return new PointF(pixeldiff.X/HorizFactorPageToPixel(),pixeldiff.Y/VertFactorPageToPixel());
		}

		/// <summary>
		/// converts from pixel to printable area coordinates
		/// </summary>
		/// <param name="pixelc">pixel coordinates as returned by MouseEvents</param>
		/// <returns>coordinates of the printable area in 1/72 inch</returns>
		public PointF PixelToPrintableAreaCoordinates(PointF pixelc)
		{
			PointF r = PixelToPageCoordinates(pixelc);
			r.X -= m_Graph.PrintableBounds.X;
			r.Y -= m_Graph.PrintableBounds.Y;
			return r;
		}

		/// <summary>
		/// This calculates the zoom factor using the size of the graph view, so that the page fits
		/// best into the view.
		/// </summary>
		/// <returns>The calculated zoom factor.</returns>
		protected virtual float CalculateAutoZoom()
		{
			float zoomh = (UnitPerInch*m_View.GraphSize.Width/this.m_HorizRes)/m_Graph.PageBounds.Width;
			float zoomv = (UnitPerInch*m_View.GraphSize.Height/this.m_VertRes)/m_Graph.PageBounds.Height;
			return System.Math.Min(zoomh,zoomv);

		}
		/// <summary>
		/// Translates the graphics properties to graph (printable area), so that the beginning of printable area now is (0,0) and the units are in Points (1/72 inch)
		/// </summary>
		/// <param name="g">Graphics to translate</param>
		void TranslateGraphicsToGraphUnits(Graphics g)
		{
			g.PageUnit = GraphicsUnit.Point;
			g.PageScale = this.m_Zoom;

			// the graphics path was returned in printable area ("graph") coordinates
			// thats why we have to shift our coordinate system to printable area coordinates also
			float pointsh = UnitPerInch*m_View.GraphScrollPosition.X/(this.m_HorizRes*this.m_Zoom);
			float pointsv = UnitPerInch*m_View.GraphScrollPosition.Y/(this.m_VertRes*this.m_Zoom);
			pointsh += m_Graph.PrintableBounds.X;
			pointsv += m_Graph.PrintableBounds.Y; 

			// shift the coordinates to page coordinates
			g.TranslateTransform(pointsh,pointsv);
		}

		#endregion // Scaling, Converting


		/// <summary>
		/// Clears the selection list and repaints the graph if neccessary
		/// </summary>
		public void ClearSelections()
		{
			bool bRepaint = (m_SelectedObjects.Count>0); // is a repaint neccessary
			m_SelectedObjects.Clear();
			
			
			this.m_MouseState = new ObjectPointerMouseHandler();

			if(bRepaint)
				m_View.InvalidateGraph(); 
		}

		/// <summary>
		/// Determines whether or not the pixel position in <paramref name="pixelPos"/> is on a already selected object
		/// </summary>
		/// <param name="pixelPos">The pixel position to test (on the graph panel)</param>
		/// <param name="foundObject">Returns the object the position <paramref name="pixelPos"/> is pointing to, else null</param>
		/// <returns>True when the pixel position is upon a selected object, else false</returns>
		public bool IsPixelPositionOnAlreadySelectedObject(PointF pixelPos, out GraphObject foundObject)
		{
			PointF graphXY = this.PixelToPrintableAreaCoordinates(pixelPos); // Graph area coordinates

			// have we clicked on one of the already selected objects
			foreach(GraphObject graphObject in m_SelectedObjects.Keys)
			{
				int nLayer = (int)m_SelectedObjects[graphObject];
				if(null!=graphObject.HitTest(Layers[nLayer].GraphToLayerCoordinates(graphXY)))
				{
					foundObject = graphObject;
					return true;
				}
			}
			foundObject = null;
			return false;
		}



		/// <summary>
		/// Looks for a graph object at pixel position <paramref name="pixelPos"/> and returns true if one is found.
		/// </summary>
		/// <param name="pixelPos">The pixel coordinates (graph panel coordinates)</param>
		/// <param name="foundObject">Found object if there is one found, else null</param>
		/// <param name="foundInLayerNumber">The layer the found object belongs to, otherwise 0</param>
		/// <returns>True if a object was found at the pixel coordinates <paramref name="pixelPos"/>, else false.</returns>
		public bool FindGraphObjectAtPixelPosition(PointF pixelPos, out GraphObject foundObject, out int foundInLayerNumber)
		{
			// search for a object first
			PointF mousePT = PixelToPrintableAreaCoordinates(pixelPos);
			GraphicsPath gp;

			for(int nLayer=0;nLayer<Layers.Count;nLayer++)
			{
				Altaxo.Graph.Layer layer = Layers[nLayer];
				foundObject = layer.HitTest(mousePT, out gp);
				if(null!=foundObject)
				{
					foundInLayerNumber = nLayer;
					return true;
				}
			}
			foundObject=null;
			foundInLayerNumber=0;
			return false;
		}

		/// <summary>
		/// Draws immediately a selection rectangle around the object <paramref name="graphObject"/>.
		/// </summary>
		/// <param name="graphObject">The graph object for which a selection rectangle has to be drawn.</param>
		/// <param name="nLayer">The layer number the <paramref name="graphObject"/> belongs to.</param>
		public void DrawSelectionRectangleImmediately(GraphObject graphObject, int nLayer)
		{
			using(Graphics g = m_View.CreateGraphGraphics())
			{
				// now translate the graphics to graph units and paint all selection path
				this.TranslateGraphicsToGraphUnits(g);
				g.DrawPath(Pens.Blue,Layers[nLayer].LayerToGraphCoordinates(graphObject.GetSelectionPath())); // draw the selection path
			}		
		}


		#region Inner Classes

		#region Mouse Handler Classes

		#region abstract mouse state handler
		/// <summary>
		/// The abstract base class of all MouseStateHandlers.
		/// </summary>
		/// <remarks>The mouse state handler are used to handle the mouse events of the graph view in different contexts,
		/// depending on which GraphTool is choosen by the user.</remarks>
		public abstract class MouseStateHandler
		{
			/// <summary>Stores the mouse position of the last mouse up event.</summary>
			protected PointF m_LastMouseUp;
			/// <summary>Stores the mouse position of the last mouse down event.</summary>
			protected PointF m_LastMouseDown;

			/// <summary>
			/// Handles the mouse move event.
			/// </summary>
			/// <param name="sender">The GraphController that sends this event.</param>
			/// <param name="e">MouseEventArgs as provided by the view.</param>
			/// <returns>The next mouse state handler that should handle mouse events.</returns>
			public virtual MouseStateHandler OnMouseMove(GraphController sender, System.Windows.Forms.MouseEventArgs e)
			{
				return this;
			}

			/// <summary>
			/// Handles the mouse up event. Stores the position of the mouse into <see cref="m_LastMouseUp"/>.
			/// </summary>
			/// <param name="sender">The GraphController that sends this event.</param>
			/// <param name="e">MouseEventArgs as provided by the view.</param>
			/// <returns>The next mouse state handler that should handle mouse events.</returns>
			public virtual MouseStateHandler OnMouseUp(GraphController sender, System.Windows.Forms.MouseEventArgs e)
			{
				m_LastMouseUp = new Point(e.X,e.Y);
				return this;
			}

			/// <summary>
			/// Handles the mouse down event. Stores the position of the mouse into <see cref="m_LastMouseDown"/>.
			/// </summary>
			/// <param name="sender">The GraphController that sends this event.</param>
			/// <param name="e">MouseEventArgs as provided by the view.</param>
			/// <returns>The next mouse state handler that should handle mouse events.</returns>
			public virtual MouseStateHandler OnMouseDown(GraphController sender, System.Windows.Forms.MouseEventArgs e)
			{
				m_LastMouseDown = new Point(e.X,e.Y);
				return this;
			}
			
			/// <summary>
			/// Handles the mouse click event.
			/// </summary>
			/// <param name="sender">The GraphController that sends this event.</param>
			/// <param name="e">EventArgs as provided by the view.</param>
			/// <returns>The next mouse state handler that should handle mouse events.</returns>
			public virtual MouseStateHandler OnClick(GraphController sender, System.EventArgs e)
			{
				return this;
			}
			
			/// <summary>
			/// Handles the mouse doubleclick event.
			/// </summary>
			/// <param name="sender">The GraphController that sends this event.</param>
			/// <param name="e">EventArgs as provided by the view.</param>
			/// <returns>The next mouse state handler that should handle mouse events.</returns>
			public virtual MouseStateHandler OnDoubleClick(GraphController sender, System.EventArgs e)
			{
				return this;
			}
		}
		#endregion // abstract mouse state handler

		#region Object Pointer Mouse Handler
		/// <summary>
		/// Handles the mouse events when the <see cref="GraphTools.ObjectPointer"/> tools is selected.
		/// </summary>
		public class ObjectPointerMouseHandler : MouseStateHandler
		{
			/// <summary>
			/// If true, the selected objects where moved when a MouseMove event is fired
			/// </summary>
			protected bool m_bMoveObjectsOnMouseMove=false;
			/// <summary>Stores the mouse position of the last point to where the selected objects where moved</summary>
			protected PointF m_MoveObjectsLastMovePoint;
			/// <summary>If objects where really moved during the moving mode, this value become true</summary>
			protected bool m_bObjectsWereMoved=false;


			/// <summary>
			/// Handles the mouse move event.
			/// </summary>
			/// <param name="grac">The GraphController that sends this event.</param>
			/// <param name="e">MouseEventArgs as provided by the view.</param>
			/// <returns>The next mouse state handler that should handle mouse events.</returns>
			public override MouseStateHandler OnMouseMove(GraphController grac, System.Windows.Forms.MouseEventArgs e)
			{
				base.OnMouseMove(grac,e);

				System.Console.WriteLine("MouseMove {0},{1}",e.X,e.Y);

				PointF mouseDiff = new PointF(e.X - m_MoveObjectsLastMovePoint.X, e.Y - m_MoveObjectsLastMovePoint.Y);
				if(m_bMoveObjectsOnMouseMove && 0!= grac.m_SelectedObjects.Count && (mouseDiff.X!=0 || mouseDiff.Y!=0))
				{
					// move all the selected objects to the new position
					// first update the position of the selected objects to reflect the new position
					m_MoveObjectsLastMovePoint.X = e.X;
					m_MoveObjectsLastMovePoint.Y = e.Y;

					// indicate the objects has moved now
					m_bObjectsWereMoved = true;


					// this difference, which is in mouse coordinates, must first be 
					// converted to Graph coordinates (1/72"), and then transformed for
					// each object to the layer coordinate differences of the layer
		
					PointF graphDiff = grac.PixelToPageDifferences(mouseDiff); // calulate the moving distance in page units = graph units

					foreach(GraphObject graphObject in grac.m_SelectedObjects.Keys)
					{
						// get the layer number the graphObject belongs to
						int nLayer = (int)grac.m_SelectedObjects[graphObject];
						PointF layerDiff = grac.Layers[nLayer].GraphToLayerDifferences(graphDiff); // calculate the moving distance in layer units
						graphObject.X += layerDiff.X;
						graphObject.Y += layerDiff.Y;
						// Console.WriteLine("Moving mdiff={0}, gdiff={1}, ldiff={2}", mouseDiff,graphDiff,layerDiff);
					}
					// now paint the objects on the new position
					if(null!=grac.m_FrozenGraph)
					{
						Graphics g = grac.m_View.CreateGraphGraphics();
						// first paint the frozen graph, and upon that, paint all selection rectangles
						g.DrawImageUnscaled(grac.m_FrozenGraph,0,0,grac.m_View.GraphSize.Width,grac.m_View.GraphSize.Height);

						// now translate the graphics to graph units and paint all selection path
						grac.TranslateGraphicsToGraphUnits(g);
						foreach(GraphObject graphObject in grac.m_SelectedObjects.Keys)
						{
							int nLayer = (int)grac.m_SelectedObjects[graphObject]; 						// get the layer number the graphObject belongs to
							g.DrawPath(Pens.Blue,grac.Layers[nLayer].LayerToGraphCoordinates(graphObject.GetSelectionPath())); // draw the selection path
						}
					}
					else  // if the graph was not frozen before - what reasons ever
					{
						grac.m_View.InvalidateGraph(); // rise a normal paint event
					}

				}
				return this;
			}

			/// <summary>
			/// Handles the MouseDown event when the object pointer tool is selected
			/// </summary>
			/// <param name="grac">The sender of the event.</param>
			/// <param name="e">The mouse event args</param>
			/// <remarks>
			/// The strategy to handle the mousedown event is as following:
			/// 
			/// Have we clicked on already selected objects?
			///		if yes (we have clicked on already selected objects) and the shift or control key was pressed -> deselect the object and repaint
			///		if yes (we have clicked on already selected objects) and none shift nor control key was pressed-> activate the object moving  mode
			///		if no (we have not clicked on already selected objects) and shift or control key was pressed -> search for the object and add it to the selected objects, then aktivate moving mode
			///		if no (we have not clicked on already selected objects) and no shift or control key pressed -> if a object was found add it to the selected objects and activate moving mode
			///		                                                                                               if no object was found clear the selection list, deactivate moving mode
			/// </remarks>
			public override MouseStateHandler OnMouseDown(GraphController grac, System.Windows.Forms.MouseEventArgs e)
			{
				base.OnMouseDown(grac, e);

				if(e.Button != MouseButtons.Left)
					return this; // then there is nothing to do here

				// first, if we have a mousedown without shift key and the
				// position has changed with respect to the last mousedown
				// we have to deselect all objects
				PointF mouseXY = new PointF(e.X,e.Y);
				bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
				bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
				PointF graphXY = grac.PixelToPrintableAreaCoordinates(mouseXY); // Graph area coordinates

	
				// have we clicked on one of the already selected objects
				GraphObject clickedSelectedObject=null;
				bool bClickedOnAlreadySelectedObjects=grac.IsPixelPositionOnAlreadySelectedObject(mouseXY, out clickedSelectedObject);

				if(bClickedOnAlreadySelectedObjects)
				{
					if(bShiftKey || bControlKey) // if shift or control is pressed, remove the selection
					{
						grac.m_SelectedObjects.Remove(clickedSelectedObject);
						grac.m_View.InvalidateGraph(); // repaint the graph
					}
					else // not shift or control pressed -> so activate the object moving mode
					{
						StartMovingObjects(grac,mouseXY);
					}
				} // end if bClickedOnAlreadySelectedObjects
				else // not clicked on a already selected object
				{
					// search for a object first
					GraphObject clickedObject;
					int clickedLayerNumber=0;
					grac.FindGraphObjectAtPixelPosition(mouseXY, out clickedObject, out clickedLayerNumber);

					if(bShiftKey || bControlKey) // if shift or control are pressed, we add the object to the selection list and start moving mode
					{
						if(null!=clickedObject)
						{
							grac.m_SelectedObjects.Add(clickedObject,clickedLayerNumber);
							grac.DrawSelectionRectangleImmediately(clickedObject,clickedLayerNumber);
							StartMovingObjects(grac,mouseXY);
						}
					}
					else // no shift or control key pressed
					{
						if(null!=clickedObject)
						{
							ClearSelections(grac);
							grac.m_SelectedObjects.Add(clickedObject,clickedLayerNumber);
							grac.DrawSelectionRectangleImmediately(clickedObject,clickedLayerNumber);
							StartMovingObjects(grac,mouseXY);
						}
						else // if clicked to nothing 
						{
							ClearSelections(grac); // clear the selection list
						}
					} // end else no shift or control

				} // end else (not cklicked on already selected object)
				return this;
			} // end of function

			/// <summary>
			/// Handles the mouse up event.
			/// </summary>
			/// <param name="grac">The GraphController that sends this event.</param>
			/// <param name="e">MouseEventArgs as provided by the view.</param>
			/// <returns>The next mouse state handler that should handle mouse events.</returns>
			public override MouseStateHandler OnMouseUp(GraphController grac, System.Windows.Forms.MouseEventArgs e)
			{
				base.OnMouseUp(grac,e);

				System.Console.WriteLine("MouseUp {0},{1}",e.X,e.Y);
				EndMovingObjects(grac);
				return this;
			}

			/// <summary>
			/// Handles the mouse doubleclick event.
			/// </summary>
			/// <param name="grac">The GraphController that sends this event.</param>
			/// <param name="e">EventArgs as provided by the view.</param>
			/// <returns>The next mouse state handler that should handle mouse events.</returns>
			public override MouseStateHandler OnDoubleClick(GraphController grac, System.EventArgs e)
			{
				base.OnDoubleClick(grac,e);

				System.Console.WriteLine("DoubleClick!");

				// if there is exactly one object selected, try to open the corresponding configuration dialog
				if(grac.m_SelectedObjects.Count==1)
				{
					IEnumerator graphEnum = grac.m_SelectedObjects.Keys.GetEnumerator(); // get the enumerator
					graphEnum.MoveNext(); // set the enumerator to the first item
					GraphObject graphObject = (GraphObject)graphEnum.Current;
					int nLayer = (int)grac.m_SelectedObjects[graphObject];
					if(graphObject is Graph.ExtendedTextGraphObject)
					{
						TextControlDialog dlg = new TextControlDialog(grac.Layers[nLayer],(ExtendedTextGraphObject)graphObject);
						if(DialogResult.OK==dlg.ShowDialog(grac.m_View.Window))
						{
							if(!dlg.TextGraphObject.Empty)
							{
								((ExtendedTextGraphObject)graphObject).CopyFrom(dlg.TextGraphObject);
							}
							else // item is empty, so must be deleted in the layer and in the selectedObjects
							{
								grac.m_SelectedObjects.Remove(graphObject);
								grac.Layers[nLayer].Remove(graphObject);
							}

							grac.m_View.InvalidateGraph(); // repaint the graph
						}
					}
				}
				return this;
			}


			/// <summary>
			/// Handles the mouse click event.
			/// </summary>
			/// <param name="grac">The GraphController that sends this event.</param>
			/// <param name="e">EventArgs as provided by the view.</param>
			/// <returns>The next mouse state handler that should handle mouse events.</returns>
			public override MouseStateHandler OnClick(GraphController grac, System.EventArgs e)
			{
				base.OnClick(grac,e);

				System.Console.WriteLine("Click");

				return this;
			}


			/// <summary>
			/// Actions neccessary to start the dragging of graph objects
			/// </summary>
			/// <param name="currentMousePosition">the current mouse position in pixel</param>
			protected void StartMovingObjects(GraphController grac, PointF currentMousePosition)
			{
				if(!m_bMoveObjectsOnMouseMove)
				{
					m_bMoveObjectsOnMouseMove=true;
					m_bObjectsWereMoved=false; // up to now no objects were really moved
					m_MoveObjectsLastMovePoint = currentMousePosition;

					// create a frozen bitmap of the graph
					Graphics g = grac.m_View.CreateGraphGraphics(); // do not translate the graphics here!
					grac.m_FrozenGraph = new Bitmap(grac.m_View.GraphSize.Width,grac.m_View.GraphSize.Height,g);
					Graphics gbmp = Graphics.FromImage(grac.m_FrozenGraph);
					grac.DoPaint(gbmp,false);
					gbmp.Dispose();
				}
			}

			/// <summary>
			/// Actions neccessary to end the dragging of graph objects
			/// </summary>
			protected void EndMovingObjects(GraphController grac)
			{
				bool bRepaint = m_bObjectsWereMoved; // repaint the graph when objects were really moved

				m_bMoveObjectsOnMouseMove = false;
				m_bObjectsWereMoved=false;
				m_MoveObjectsLastMovePoint = new Point(0,0); // this is not neccessary, but only for "order"
				if(null!=grac.m_FrozenGraph) 
				{
					grac.m_FrozenGraph.Dispose(); grac.m_FrozenGraph=null;
				}
			
				if(bRepaint)
					grac.m_View.InvalidateGraph(); // redraw the contents

			}

			/// <summary>
			/// Clears the selection list and repaints the graph if neccessary
			/// </summary>
			public void ClearSelections(GraphController grac)
			{
				bool bRepaint = (grac.m_SelectedObjects.Count>0); // is a repaint neccessary
				grac.m_SelectedObjects.Clear();
				EndMovingObjects(grac);

				if(bRepaint)
					grac.m_View.InvalidateGraph(); 
			}


		} // end of class

		#endregion // object pointer mouse handler

		#region Text tool mouse handler

		/// <summary>
		/// This class handles the mouse events in case the text tool is selected.
		/// </summary>
		public class TextToolMouseHandler : MouseStateHandler
		{
			/// <summary>
			/// Handles the click event by opening the text tool dialog.
			/// </summary>
			/// <param name="grac">The graph control.</param>
			/// <param name="e">EventArgs.</param>
			/// <returns>The mouse state handler for handling the next mouse events.</returns>
			public override MouseStateHandler OnClick(GraphController grac, System.EventArgs e)
			{
				base.OnClick(grac,e);

				// get the page coordinates (in Point (1/72") units)
				PointF printAreaCoord = grac.PixelToPrintableAreaCoordinates(m_LastMouseDown);
				// with knowledge of the current active layer, calculate the layer coordinates from them
				PointF layerCoord = grac.Layers[grac.CurrentLayerNumber].GraphToLayerCoordinates(printAreaCoord);

				ExtendedTextGraphObject tgo = new ExtendedTextGraphObject();
				tgo.Position = layerCoord;

				// deselect the text tool
				grac.CurrentGraphTool = GraphTools.ObjectPointer;

				TextControlDialog dlg = new TextControlDialog(grac.Layers[grac.CurrentLayerNumber],tgo);
				if(DialogResult.OK==dlg.ShowDialog(grac.m_View.Window))
				{
					// add the resulting textgraphobject to the layer
					if(!dlg.TextGraphObject.Empty)
					{
						grac.Layers[grac.CurrentLayerNumber].GraphObjects.Add(dlg.TextGraphObject);
						grac.m_View.InvalidateGraph();
					}
				}
				return new ObjectPointerMouseHandler();
			}
		}

		#endregion // Text Tool Mouse Handler

		#endregion // Mouse Handlers


		/// <summary>
		/// Used as menu items in the Data menu popup. Stores the plot number the menu item represents.
		/// </summary>
		public class DataMenuItem : MenuItem
		{
			/// <summary>The plot number this menu item represents.</summary>
			public int PlotItemNumber=0;

			/// <summary>Creates the default menu item (PlotItemNumber is 0).</summary>
			public DataMenuItem() {}

			/// <summary>Creates a menuitem with text and a handler.</summary>
			/// <param name="t">The text the menuitem shows.</param>
			/// <param name="e">The handler in case the menu item is clicked.</param>
			public DataMenuItem(string t, EventHandler e) : base(t,e) {}
		}

		#endregion // Inner Classes

	}
}
