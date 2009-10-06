using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.GUI
{
	class GraphControllerActions
	{
	}

#if false
	class GraphControllerMenuActions
	{
		#region Menu Definition


		/// <summary>
		/// Creates the default menu of a graph view.
		/// </summary>
		/// <remarks>In case there is already a menu here, the old menu is overwritten.</remarks>
		public void InitializeMenu()
		{
			int index = 0, index2 = 0;
			MenuItem mi;

			m_MainMenu = new MainMenu();

			// File Menu
			// **********************************************************
			mi = new MenuItem("&File");
			mi.Index = 0;
			mi.MergeOrder = 0;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count - 1;

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

			// File - Save Graph As
			mi = new MenuItem("Save Graph As..");
			mi.Click += new EventHandler(EhMenuFileSaveGraphAs_OnClick);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// File - Export (Popup)
			// ------------------------------------------------------------------
			mi = new MenuItem("Export");
			//mi.Popup += new EventHandler(MenuFileExport_OnPopup);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
			index2 = m_MainMenu.MenuItems[index].MenuItems.Count - 1;

			// File - Export - Metafile 
			mi = new MenuItem("Metafile");
			mi.Click += new EventHandler(EhMenuFileExportMetafile_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);


			// Edit (Popup)
			// ****************************************************************** 
			mi = new MenuItem("Edit");
			mi.Index = 1;
			mi.MergeOrder = 1;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count - 1;

			// Edit - NewLayer (Popup)
			// ------------------------------------------------------------------
			mi = new MenuItem("New layer(axes)");
			//mi.Popup += new EventHandler(MenuFileExport_OnPopup);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
			index2 = m_MainMenu.MenuItems[index].MenuItems.Count - 1;

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
			mi.Index = 3;
			mi.MergeOrder = 3;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MenuDataPopup = mi; // store this for later manimpulation
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count - 1;


			// Graph (Popup)
			// ****************************************************************** 
			mi = new MenuItem("Graph");
			mi.Index = 4;
			mi.MergeOrder = 4;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count - 1;

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

			// Graph - XYPlotLayer control
			mi = new MenuItem("XYPlotLayer control");
			mi.Click += new EventHandler(EhMenuGraphLayer_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Graph - Add curve plot
			mi = new MenuItem("Add Curve Plot");
			mi.Click += new EventHandler(EhMenuGraphAddCurvePlot_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
		}


		/// <summary>
		/// Updates a special menu item, the data item, with the currently available plot names. The active plot is marked with a
		/// check.
		/// </summary>
		public void UpdateDataPopup()
		{
			if (null == this.m_MenuDataPopup)
				return; // as long there is no menu, we cannot do it

			// first delete old menuitems
			this.m_MenuDataPopup.MenuItems.Clear();


			// check there is at least one layer
			if (m_Graph.Layers.Count == 0)
				return; // there is no layer, we can not have items in the data menu

			// now it is save to get the active layer
			int actLayerNum = this.CurrentLayerNumber;
			XYPlotLayer actLayer = this.Layers[actLayerNum];

			// then append the plot associations of the actual layer

			int actPA = CurrentPlotNumber;
			int len = actLayer.PlotItems.Flattened.Length;
			for (int i = 0; i < len; i++)
			{
				IGPlotItem pa = actLayer.PlotItems.Flattened[i];
				DataMenuItem mi = new DataMenuItem(pa.ToString(), new EventHandler(EhMenuData_Data));
				mi.Checked = (i == actPA);
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
				Current.Gui.ShowPageSetupDialog();
			}
			catch (Exception exc)
			{
				MessageBox.Show(exc.ToString(), "Exception occured!");
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
				if (Current.Gui.ShowPrintDialog())
				{
					Current.PrintingService.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
					Current.PrintingService.PrintDocument.Print();
				}
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(this.m_View.Window, ex.ToString());
			}
			finally
			{
				Current.PrintingService.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
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
				Current.PrintingService.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
				dlg.Document = Current.PrintingService.PrintDocument;
				dlg.ShowDialog(this.m_View.Window);
				dlg.Dispose();
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(this.m_View.Window, ex.ToString());
			}
			finally
			{
				Current.PrintingService.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
			}
		}


		protected void EhMenuFileSaveGraphAs_OnClick(object sender, System.EventArgs e)
		{
			System.IO.Stream myStream;
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();

			saveFileDialog1.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";
			saveFileDialog1.FilterIndex = 1;
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((myStream = saveFileDialog1.OpenFile()) != null)
				{
					Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
					info.BeginWriting(myStream);
					info.AddValue("Graph", this.Doc);
					info.EndWriting();
					myStream.Close();
				}
			}
		}

		/// <summary>
		/// Handler for the menu item "File" - "Export Metafile".
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">Not used.</param>
		private void EhMenuFileExportMetafile_OnClick(object sender, System.EventArgs e)
		{
			System.IO.Stream myStream;
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();

			saveFileDialog1.Filter = "Windows Metafiles (*.emf)|*.emf|All files (*.*)|*.*";
			saveFileDialog1.FilterIndex = 2;
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((myStream = saveFileDialog1.OpenFile()) != null)
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
			GraphDocument newDoc = new GraphDocument(this.Doc);
			Current.ProjectService.CreateNewGraph(newDoc);
		}

		private void EhMenuGraphLayer_OnClick(object sender, System.EventArgs e)
		{
			EnsureValidityOfCurrentLayerNumber();
			if (null != this.ActiveLayer)
				LayerController.ShowDialog(this.ActiveLayer);
		}

		private void EhMenuGraphAddCurvePlot_OnClick(object sender, System.EventArgs e)
		{
			EnsureValidityOfCurrentLayerNumber();
			this.Doc.Layers[this.CurrentLayerNumber].PlotItems.Add(new XYFunctionPlotItem(new XYFunctionPlotData(new PolynomialFunction(new double[] { 0, 0, 1 })), new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line)));
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
		/// it shows the LineScatterPlotStyleControl into a dialog box.
		/// </remarks>
		private void EhMenuData_Data(object sender, System.EventArgs e)
		{
			DataMenuItem dmi = (DataMenuItem)sender;

			if (!dmi.Checked)
			{
				// if the menu item was not checked before, check it now
				// by making the plot association shown by the menu item
				// the actual plot association
				int actLayerNum = this.CurrentLayerNumber;
				XYPlotLayer actLayer = this.Layers[actLayerNum];
				if (null != actLayer && dmi.PlotItemNumber < actLayer.PlotItems.Flattened.Length)
				{
					dmi.Checked = true;
					CurrentPlotNumber = dmi.PlotItemNumber;
				}
			}
			else
			{
				// if it was checked before, then bring up the plot style dialog
				// of the plot association represented by this menu item
				int actLayerNum = this.CurrentLayerNumber;
				XYPlotLayer actLayer = this.Layers[actLayerNum];
				IGPlotItem pa = actLayer.PlotItems.Flattened[CurrentPlotNumber];

				Current.Gui.ShowDialog(new object[] { pa }, string.Format("#{0}: {1}", pa.Name, pa.ToString()), true);
			}



		}



		#endregion // Menu event handlers

	}
#endif
}
