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
	/// Interface to be implemented by a form or a control to be able to show a graph. This can either be a control or a form.
	/// </summary>
	public interface IGraphView
	{
		/// <summary>Returns the windows of this view. In case the view is a Form, it returns the form. But if the view is only a control
		/// on a form, it returns the control window.
		/// </summary>
		System.Windows.Forms.Control Window { get; }
		/// <summary>
		/// Returns the form of this view. In case the view is a Form, it returns that form itself. In case the view is a control on a form,
		/// it returns not the control but the hosting form of this control.
		/// </summary>
		System.Windows.Forms.Form    Form   { get; }


		/// <summary>
		/// Get / sets the AutoScroll size property 
		/// </summary>
		Size AutoScrollMinSize { get; set; }


		/// <summary>
		/// Get /sets the scroll position of the graph
		/// </summary>
		Point AutoScrollPosition { get; set; }


		/// <summary>
		/// This creates a graphics context for the graph.
		/// </summary>
		/// <returns>The graphics context.</returns>
		Graphics CreateGraphics();


		/// <summary>
		/// This forces redrawing of the entire graph window.
		/// </summary>
		void InvalidateGraph();

		/// <summary>
		/// Returns the size (in pixel) of the area, wherein the graph is painted.
		/// </summary>
		Size GraphSize { get; }

	}


	/// <summary>
	/// Summary description for GraphController.
	/// </summary>
	public class GraphController
	{

		protected System.Windows.Forms.MainMenu m_MainMenu; // the Menu of this control to be merged
		protected System.Windows.Forms.MenuItem m_MenuDataPopup;

		protected Altaxo.Graph.GraphDocument m_Graph;
		protected IGraphView m_View;

		
		/// <summary>
		/// This enumeration declares the current choosen tool for the GraphControl
		/// The numeric values have to match the icon positions in the corresponding toolbar
		/// </summary>
		public enum GraphTools 
		{
			/// <summary>The tool to click to the objects, dragging them, and open the object dialogs by doubleclicking on them</summary>
			ObjectPointer=0,
			/// <summary>The tool to write text, i.e. to create ExtendedTextGraphObjects</summary>
			Text=1
		}


		private const double MinimumGridSize = 20;
		private const float UnitPerInch = 72;

		// following default unit is point (1/72 inch)
		
	
		/// <summary>
		/// Brush to fill the page ground. Since the printable area is filled with another brush, in effect
		/// this brush fills only the non printable margins of the page. 
		/// </summary>
		private BrushHolder m_PageGroundBrush = new BrushHolder(Color.LightGray);

		/// <summary>
		/// Brush to fill the printable area of the graph.
		/// </summary>
		private BrushHolder m_PrintableAreaBrush = new BrushHolder(Color.Snow);

		/// <summary>Current horizontal resolution of the paint method.</summary>
		private float m_HorizRes  = 300;
		private float m_VertRes = 300;
		private Color m_NonPrintingAreaColor = Color.Gray;
		private int m_MarginLineWidth = 1;
		private Color m_MarginColor = Color.Green;
		private float m_Zoom  = 0.4f;
		private bool  m_AutoZoom = true; // if true, the sheet is zoomed as big as possible to fit into window
		/// <summary>Number of the currently selected layer.</summary>
		protected int m_CurrentLayerNumber = 0;
		/// <summary>Number of the currently selected plot.</summary>
		protected int m_CurrentPlotNumber=0;
		protected GraphTools m_CurrentGraphTool = GraphTools.ObjectPointer;
		private MouseStateHandler m_MouseState= new ObjectPointerMouseHandler();

		/// <summary>
		/// The hashtable of the selected objects. The key is the selected object itself,
		/// the data is a int object, which stores the layer number the object belongs to.
		/// </summary>
		protected System.Collections.Hashtable m_SelectedObjects = new System.Collections.Hashtable();



		public GraphController()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region Menu Definition

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


			// Graph - NewLayerLegend
			mi = new MenuItem("New layer legend");
			mi.Click += new EventHandler(EhMenuGraphNewLayerLegend_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
		}


		public void UpdateDataPopup()
		{
			int actLayerNum = this.CurrentLayerNumber;
			Layer actLayer = this.Layers[actLayerNum];
			
			if(null==this.m_MenuDataPopup)
				return;

			// first delete old menuitems
			this.m_MenuDataPopup.MenuItems.Clear();

			
			if(null==actLayer)
				return;


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

		private void EhMenuFilePageSetup_OnClick(object sender, System.EventArgs e)
		{
			App.CurrentApplication.PageSetupDialog.ShowDialog(this.m_View.Window);
		}

		private void EhMenuFilePrint_OnClick(object sender, System.EventArgs e)
		{
			if(DialogResult.OK==App.CurrentApplication.PrintDialog.ShowDialog(this.m_View.Window))
			{
				try
				{
					App.CurrentApplication.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
					App.CurrentApplication.PrintDocument.Print();
				}
				catch(Exception ex)
				{
					System.Windows.Forms.MessageBox.Show(this.m_View.Window,ex.ToString());
				}
				finally
				{
					App.CurrentApplication.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
				}
			}
		}

		private void EhMenuFilePrintPreview_OnClick(object sender, System.EventArgs e)
		{
			try
			{
				System.Windows.Forms.PrintPreviewDialog dlg = new System.Windows.Forms.PrintPreviewDialog();
				App.CurrentApplication.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
				dlg.Document = App.CurrentApplication.PrintDocument;
				dlg.ShowDialog(this.m_View.Window);
				dlg.Dispose();
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(this.m_View.Window,ex.ToString());
			}
			finally
			{
				App.CurrentApplication.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
			}
		}

		private void EhMenuEditNewlayerNormalBottomXLeftY_OnClick(object sender, System.EventArgs e)
		{
			m_Graph.CreateNewLayerNormalBottomXLeftY();
		}

		private void EhMenuEditNewlayerLinkedTopXRightY_OnClick(object sender, System.EventArgs e)
		{
			m_Graph.CreateNewLayerLinkedTopXRightY(CurrentLayerNumber);
		}

		private void EhMenuEditNewlayerLinkedTopX_OnClick(object sender, System.EventArgs e)
		{
		
		}

		private void EhMenuEditNewlayerLinkedRightY_OnClick(object sender, System.EventArgs e)
		{
		
		}

		private void EhMenuEditNewlayerLinkedTopXRightYXAxisStraight_OnClick(object sender, System.EventArgs e)
		{
			m_Graph.CreateNewLayerLinkedTopXRightY_XAxisStraight(CurrentLayerNumber);
		}

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

		private void EhMenuGraphNewLayerLegend_OnClick(object sender, System.EventArgs e)
		{
			m_Graph.Layers[CurrentLayerNumber].CreateNewLayerLegend();
		}


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


		#region Other event handlers

		private void GraphForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			System.Windows.Forms.DialogResult dlgres = System.Windows.Forms.MessageBox.Show(this.m_View.Window,"Do you really want to close this graph?","Attention",System.Windows.Forms.MessageBoxButtons.YesNo);

			if(dlgres==System.Windows.Forms.DialogResult.No)
			{
				e.Cancel = true;
			}
		}

		private void GraphForm_Closed(object sender, System.EventArgs e)
		{
			App.document.RemoveGraph(this.m_View.Form);
		}


		public void EhPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs ppea)
		{
			Graphics g = ppea.Graphics;
			DoPaint(g,true);
		}

		protected void EhSizeChanged(EventArgs e)
		{
			if(m_AutoZoom)
			{
				this.m_Zoom = CalculateAutoZoom();
		
				// System.Console.WriteLine("h={0}, v={1} {3} {4} {5}",zoomh,zoomv,UnitPerInch,this.ClientSize.Width,this.m_HorizRes, this.m_PageBounds.Width);
				// System.Console.WriteLine("SizeX = {0}, zoom = {1}, dpix={2},in={3}",this.ClientSize.Width,this.m_Zoom,this.m_HorizRes,this.ClientSize.Width/(this.m_HorizRes*this.m_Zoom));
			
				m_View.AutoScrollMinSize= new Size(0,0);
			}
			else
			{
				double pixelh = System.Math.Ceiling(m_Graph.PageBounds.Width*this.m_HorizRes*this.m_Zoom/(UnitPerInch));
				double pixelv = System.Math.Ceiling(m_Graph.PageBounds.Height*this.m_VertRes*this.m_Zoom/(UnitPerInch));
				m_View.AutoScrollMinSize = new Size((int)pixelh,(int)pixelv);
			}

		}


		/// <summary>
		/// Handler of the event LayerCollectionChanged of the graph document. Forces to
		/// check the LayerButtonBar to keep track that the number of buttons match the number of layers.</summary>
		/// <param name="sender">The sender of the event (the GraphDocument).</param>
		/// <param name="e">The event arguments.</param>
		protected void EhGraphDocument_LayerCollectionChanged(object sender, System.EventArgs e)
		{
			
			// Ensure that the current layer and current plot are valid anymore
			EnsureValidityOfCurrentLayerNumber();
			EnsureValidityOfCurrentPlotNumber();
			// firstly, check if the CurrentLayerNumber or CurrentPlotNumber are valid
			// anymore by using them
			int nCurrLayerNum = this.CurrentLayerNumber;
			int nCurrPlotNum  = this.CurrentPlotNumber;

			MatchLayerToolbarButtons();

		}


		private void OnGraphPanel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_MouseState = m_MouseState.OnMouseUp(this,e);
		}

		private void OnGraphPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_MouseState = m_MouseState.OnMouseDown(this,e);
		}

		private void OnGraphPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_MouseState = this.m_MouseState.OnMouseMove(this,e);
		}

		private void OnGraphPanel_Click(object sender, System.EventArgs e)
		{
			m_MouseState = m_MouseState.OnClick(this,e);
		}

		private void OnGraphPanel_DoubleClick(object sender, System.EventArgs e)
		{
			m_MouseState = m_MouseState.OnDoubleClick(this,e);
		}



		#endregion // Other event handlers


		public string SaveAsMetafile(System.IO.Stream stream)
		{
			// Code to write the stream goes here.
			Graphics grfx = m_View.CreateGraphics();
			IntPtr ipHdc = grfx.GetHdc();
			System.Drawing.Imaging.Metafile mf = new System.Drawing.Imaging.Metafile(stream,ipHdc);
			grfx.ReleaseHdc(ipHdc);
			grfx.Dispose();
			grfx = Graphics.FromImage(mf);
					
			this.m_Graph.DoPaint(grfx,true);

			grfx.Dispose();
			
			return null;
		}

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

				float pointsh = UnitPerInch*m_View.AutoScrollPosition.X/(this.m_HorizRes*this.m_Zoom);
				float pointsv = UnitPerInch*m_View.AutoScrollPosition.Y/(this.m_VertRes*this.m_Zoom);
				g.TranslateTransform(pointsh,pointsv);

				if(!bForPrinting)
				{
					g.Clear(this.m_NonPrintingAreaColor);
					// Fill the page with its own color
					g.FillRectangle(m_PageGroundBrush,m_Graph.PageBounds);
					g.FillRectangle(m_PrintableAreaBrush,m_Graph.PrintableBounds);
					// DrawMargins(g);
				}

				System.Console.WriteLine("Paint with zoom {0}",this.m_Zoom);
				// handle the possibility that the viewport is scrolled,
				// adjust my origin coordintates to compensate
				Point pt = m_View.AutoScrollPosition;
				

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



		public GraphTools CurrentGraphTool
		{
			get { return m_CurrentGraphTool; }
			set 
			{
				m_CurrentGraphTool = value;
				
				if(null!=this.m_GraphToolsToolBar)
				{
					for(int i=0;i<m_GraphToolsToolBar.Buttons.Count;i++)
						m_GraphToolsToolBar.Buttons[i].Pushed = (i==(int)value);
				}

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

			}
		}

		public Layer.LayerCollection Layers
		{
			get { return m_Graph.Layers; }
		}


		public void EnsureValidityOfCurrentLayerNumber()
		{

			// check the validity of the CurrentLayerNumber
			if(0==m_Graph.Layers.Count)
			{
				CurrentLayerNumber=-1;
			}
			else if(m_CurrentLayerNumber>=m_Graph.Layers.Count)
			{
				CurrentLayerNumber=0;
			}
		}

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
				// negative values are only accepted if there is no layer
				if(value<0 && m_Graph.Layers.Count>0)
					throw new ArgumentOutOfRangeException("CurrentLayerNumber",value,"Accepted values must be >=0 if there is at least one layer in the graph!");

				if(value>=m_Graph.Layers.Count)
					throw new ArgumentOutOfRangeException("CurrentLayerNumber",value,"Accepted values must be less than the number of layers in the graph(currently " + m_Graph.Layers.Count.ToString() + ")!");

				m_CurrentLayerNumber = value<0 ? -1 : value;

				// reflect the change in layer number in the layer tool bar
				this.PushCurrentlyActiveLayerToolbarButton();

				this.UpdateDataPopup();
			}
		}


		public void EnsureValidityOfCurrentPlotNumber()
		{
			EnsureValidityOfCurrentLayerNumber();

			// if Layer don't exist anymore, correct CurrentLayerNumber and ActualPlotAssocitation
			if(m_CurrentLayerNumber<0)
			{
				CurrentPlotNumber=-1;
			}
			else // if at least one Layer exists
			{
				// if the PlotAssociation don't exist anymore, correct it
				if(0==this.m_Graph[CurrentLayerNumber].PlotItems.Count)
					CurrentPlotNumber = -1;
				if(m_CurrentPlotNumber>=this.m_Graph[CurrentLayerNumber].PlotItems.Count)
					CurrentPlotNumber = 0;
			}	
		}

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
					m_View.AutoScrollMinSize = new Size(0,0);
					m_View.InvalidateGraph();
				}
			}
		}

		
		public float HorizFactorPageToPixel()
		{
			return this.m_HorizRes*this.m_Zoom/UnitPerInch;
		}
		public float VertFactorPageToPixel()
		{
			return this.m_HorizRes*this.m_Zoom/UnitPerInch;
		}

		public PointF PageCoordinatesToPixel(PointF pagec)
		{
			return new PointF(pagec.X*HorizFactorPageToPixel(),pagec.Y*VertFactorPageToPixel());
		}

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


		protected virtual void DrawMargins(Graphics g)
		{
			//Rectangle margins = ZoomRectangle(ConvertToPixels(this.m_PrintableBounds));
			RectangleF margins = m_Graph.PrintableBounds;
			Pen marginPen = new Pen(m_MarginColor);
			marginPen.DashStyle = DashStyle.Dash;
			marginPen.Width = m_MarginLineWidth;
			g.DrawRectangle(marginPen, margins.X,margins.Y,margins.Width,margins.Height);
				
			float infx = -margins.Width/20;
			float infy = -margins.Height/20;
			for(int i=0;i<8;i++)
			{
				margins.Inflate(infx,infy);
				g.DrawRectangle(marginPen, margins.X,margins.Y,margins.Width,margins.Height);
			}	


			System.Console.WriteLine("margins {0},{1}",margins.Width,margins.Height);
		}

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
			float pointsh = UnitPerInch*m_View.AutoScrollPosition.X/(this.m_HorizRes*this.m_Zoom);
			float pointsv = UnitPerInch*m_View.AutoScrollPosition.Y/(this.m_VertRes*this.m_Zoom);
			pointsh += m_Graph.PrintableBounds.X;
			pointsv += m_Graph.PrintableBounds.Y; 

			// shift the coordinates to page coordinates
			g.TranslateTransform(pointsh,pointsv);
		}


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
			using(Graphics g = m_View.CreateGraphics())
			{
				// now translate the graphics to graph units and paint all selection path
				this.TranslateGraphicsToGraphUnits(g);
				g.DrawPath(Pens.Blue,Layers[nLayer].LayerToGraphCoordinates(graphObject.GetSelectionPath())); // draw the selection path
			}		
		}




		#region Mouse Handler Classes

		#region abstract mouse state handler
		/// <summary>
		/// The abstract base class of all MouseStateHandlers.
		/// </summary>
		public abstract class MouseStateHandler
		{
			protected PointF m_LastMouseUp;
			protected PointF m_LastMouseDown;

			public virtual MouseStateHandler OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				return this;
			}
			public virtual MouseStateHandler OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				m_LastMouseUp = new Point(e.X,e.Y);
				return this;
			}
			public virtual MouseStateHandler OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				m_LastMouseDown = new Point(e.X,e.Y);
				return this;
			}
			public virtual MouseStateHandler OnClick(object sender, System.EventArgs e)
			{
				return this;
			}
			public virtual MouseStateHandler OnDoubleClick(object sender, System.EventArgs e)
			{
				return this;
			}
		}
		#endregion // abstract mouse state handler

		#region Object Pointer Mouse Handler
		/// <summary>
		/// Handles the mouse events when the ObjectPointer tools is selected.
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


			public override MouseStateHandler OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				base.OnMouseMove(sender,e);

				GraphControl grac = sender as GraphControl;

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
						Graphics g = grac.m_GraphPanel.CreateGraphics();
						// first paint the frozen graph, and upon that, paint all selection rectangles
						g.DrawImageUnscaled(grac.m_FrozenGraph,0,0,grac.m_GraphPanel.Width,grac.m_GraphPanel.Height);

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
						grac.m_GraphPanel.Invalidate(); // rise a normal paint event
					}

				}
				return this;
			}

			/// <summary>
			/// Handles the MouseDown event when the object pointer tool is selected
			/// </summary>
			/// <param name="sender">The sender of the event</param>
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
			public override MouseStateHandler OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				base.OnMouseDown(sender, e);

				GraphControl grac = sender as GraphControl;

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
						grac.InvalidateGraph(); // repaint the graph
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

			public override MouseStateHandler OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				GraphControl grac = sender as GraphControl;
				System.Console.WriteLine("MouseUp {0},{1}",e.X,e.Y);
				EndMovingObjects(grac);
				return this;
			}

			public override MouseStateHandler OnDoubleClick(object sender, System.EventArgs e)
			{
				base.OnDoubleClick(sender,e);
				GraphControl grac = sender as GraphControl;

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
						if(DialogResult.OK==dlg.ShowDialog(grac))
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

							grac.InvalidateGraph(); // repaint the graph
						}
					}
				}
				return this;
			}


			public override MouseStateHandler OnClick(object sender, System.EventArgs e)
			{

				System.Console.WriteLine("Click");

				return this;
			}


			/// <summary>
			/// Actions neccessary to start the dragging of graph objects
			/// </summary>
			/// <param name="currentMousePosition">the current mouse position in pixel</param>
			protected void StartMovingObjects(GraphControl grac, PointF currentMousePosition)
			{
				if(!m_bMoveObjectsOnMouseMove)
				{
					m_bMoveObjectsOnMouseMove=true;
					m_bObjectsWereMoved=false; // up to now no objects were really moved
					m_MoveObjectsLastMovePoint = currentMousePosition;

					// create a frozen bitmap of the graph
					Graphics g = grac.m_GraphPanel.CreateGraphics(); // do not translate the graphics here!
					grac.m_FrozenGraph = new Bitmap(grac.m_GraphPanel.Width,grac.m_GraphPanel.Height,g);
					Graphics gbmp = Graphics.FromImage(grac.m_FrozenGraph);
					grac.DoPaint(gbmp,false);
					gbmp.Dispose();
				}
			}

			/// <summary>
			/// Actions neccessary to end the dragging of graph objects
			/// </summary>
			protected void EndMovingObjects(GraphControl grac)
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
					grac.InvalidateGraph(); // redraw the contents

			}

			/// <summary>
			/// Clears the selection list and repaints the graph if neccessary
			/// </summary>
			public void ClearSelections(GraphControl grac)
			{
				bool bRepaint = (grac.m_SelectedObjects.Count>0); // is a repaint neccessary
				grac.m_SelectedObjects.Clear();
				EndMovingObjects(grac);

				if(bRepaint)
					grac.InvalidateGraph(); 
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
			/// <param name="sender">The graph control.</param>
			/// <param name="e">EventArgs.</param>
			/// <returns>The mouse state handler for handling the next mouse events.</returns>
			public override MouseStateHandler OnClick(object sender, System.EventArgs e)
			{
				base.OnClick(sender,e);

				GraphControl grac = sender as GraphControl;

				// get the page coordinates (in Point (1/72") units)
				PointF printAreaCoord = grac.PixelToPrintableAreaCoordinates(m_LastMouseDown);
				// with knowledge of the current active layer, calculate the layer coordinates from them
				PointF layerCoord = grac.Layers[grac.CurrentLayerNumber].GraphToLayerCoordinates(printAreaCoord);

				ExtendedTextGraphObject tgo = new ExtendedTextGraphObject();
				tgo.Position = layerCoord;

				// deselect the text tool
				grac.CurrentGraphTool = GraphTools.ObjectPointer;

				TextControlDialog dlg = new TextControlDialog(grac.Layers[grac.CurrentLayerNumber],tgo);
				if(DialogResult.OK==dlg.ShowDialog(grac))
				{
					// add the resulting textgraphobject to the layer
					if(!dlg.TextGraphObject.Empty)
					{
						grac.Layers[grac.CurrentLayerNumber].GraphObjects.Add(dlg.TextGraphObject);
						grac.m_GraphPanel.Invalidate();
					}
				}
				return new ObjectPointerMouseHandler();
			}
		}

		#endregion // Text Tool Mouse Handler

		#endregion // Mouse Handlers


		public class DataMenuItem : MenuItem
		{
			public int PlotItemNumber=0;

			public DataMenuItem() {}
			public DataMenuItem(string t, EventHandler e) : base(t,e) {}
		}



	}
}
