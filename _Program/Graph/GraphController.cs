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
	/// Summary description for GraphController.
	/// </summary>
	public class GraphController
	{

		protected System.Windows.Forms.MainMenu m_MainMenu; // the Menu of this control to be merged
		protected System.Windows.Forms.MenuItem m_MenuDataPopup;



		
		public GraphController()
		{
			//
			// TODO: Add constructor logic here
			//
		}


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

		#region Menu event handlers

		private void EhMenuFilePageSetup_OnClick(object sender, System.EventArgs e)
		{
			App.CurrentApplication.PageSetupDialog.ShowDialog(this);
		}

		private void EhMenuFilePrint_OnClick(object sender, System.EventArgs e)
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

		private void EhMenuFilePrintPreview_OnClick(object sender, System.EventArgs e)
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

		private void EhMenuEditNewlayerNormalBottomXLeftY_OnClick(object sender, System.EventArgs e)
		{
			m_GraphControl.menuNewLayer_NormalBottomXLeftY_Click(sender,e);
		}

		private void EhMenuEditNewlayerLinkedTopXRightY_OnClick(object sender, System.EventArgs e)
		{
			m_GraphControl.menuNewLayer_LinkedTopXRightY_Click(sender,e);
		}

		private void EhMenuEditNewlayerLinkedTopX_OnClick(object sender, System.EventArgs e)
		{
		
		}

		private void EhMenuEditNewlayerLinkedRightY_OnClick(object sender, System.EventArgs e)
		{
		
		}

		private void EhMenuEditNewlayerLinkedTopXRightYXAxisStraight_OnClick(object sender, System.EventArgs e)
		{
			m_GraphControl.menuNewLayer_LinkedTopXRightY_XAxisStraight_Click(sender, e);
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
					this.m_GraphControl.SaveAsMetafile(myStream);
					myStream.Close();
				} // end openfile ok
			} // end dlgresult ok

		}

		private void EhMenuGraphNewLayerLegend_OnClick(object sender, System.EventArgs e)
		{
			this.m_GraphControl.menuGraph_NewLayerLegend();
		}


		
		#endregion // Menu event handlers


		#region Other event handlers

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


		#endregion // Other event handlers
	}
}
