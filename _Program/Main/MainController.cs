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
using System.Data;
using System.Reflection;
using Altaxo.Serialization;
using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo
{
	#region MVC Interface definitions
	
	
	/// <summary>
	/// This interface has to be implemented by all forms that are the Mdi parent form of Mdi childs.
	/// </summary>
	public interface IMdiActivationEventSource
	{
		/// <summary>
		/// Event is fired if OnMdiChildActivate is called, as first of four events, and before the base class OnMdiChildActivate is called.
		/// </summary>
		/// <remarks>The event is fired to all registed objects, regardless they was activated before or not! 
		/// The receiver is responsible for tracking of activation/deactivation.</remarks>
		event EventHandler MdiChildDeactivateBefore;
		
		/// <summary>
		/// Event is fired if OnMdiChildActivate is called, as second of four events, and before the base class OnMdiChildActivate is called.
		/// </summary>
		/// <remarks>The event is fired to all registed objects, regardless they was activated before or not! 
		/// The receiver is responsible for tracking of activation/deactivation.</remarks>
		event EventHandler MdiChildActivateBefore;

		/// <summary>
		/// Event is fired if OnMdiChildActivate is called, as third of four events, and after the base class OnMdiChildActivate is called.
		/// </summary>
		/// <remarks>The event is fired to all registed objects, regardless they was activated before or not! 
		/// The receiver is responsible for tracking of activation/deactivation.</remarks>
		event EventHandler MdiChildDeactivateAfter;

		/// <summary>
		/// Event is fired if OnMdiChildActivate is called, as fourth of four events, and after the base class OnMdiChildActivate is called.
		/// </summary>
		/// <remarks>The event is fired to all registed objects, regardless they was activated before or not! 
		/// The receiver is responsible for tracking of activation/deactivation.</remarks>
		event EventHandler MdiChildActivateAfter;
	}

	/// <summary>
	/// This interface has to be implemented by all forms that are controlled by a <see cref="IMainController"/>.
	/// </summary>
	public interface IMainView
	{
		/// <summary>
		/// Returns the Windows forms (i.e. in almost all cases - itself).
		/// </summary>
		System.Windows.Forms.Form Form { get; }
	

		/// <summary>
		/// Sets the contoller for this view.
		/// </summary>
		Altaxo.IMainController Controller { set; }

		/// <summary>
		/// Sets the main menu for the main window
		/// </summary>
		System.Windows.Forms.MainMenu MainViewMenu {	set; }
	}

	#endregion
	/// <summary>
	/// This interface has to be implemented by all Controllers that are able to controll a IMainView.
	/// </summary>
	public interface IMainController
	{
		/// <summary>
		/// The document visualized by the controller.
		/// </summary>
		Altaxo.AltaxoDocument Doc { get; }

		/// <summary>
		/// The view that is controlled by the controller.
		/// </summary>
		IMainView View { get; }


		/// <summary>
		/// Called if the view is about to be closed.
		/// </summary>
		/// <param name="e">CancelEventArgs</param>
		void EhView_Closing(System.ComponentModel.CancelEventArgs e);

		/// <summary>
		/// Called if the view is closed now.
		/// </summary>
		/// <param name="e">EventArgs</param>
		void EhView_Closed(System.EventArgs e);

		/// <summary>
		/// This is called if the Close message or shutdown is captured from the view
		/// </summary>
		void EhView_CloseMessage();


	}

	/// <summary>
	/// The class that controls the main window, i.e. the MDI parent window, of the application.
	/// </summary>
	public class MainController : IMainController
	{
		/// <summary>
		/// Das Wichtigste - das eigentliche Dokument
		/// </summary>
		public static AltaxoDocument m_Doc=null;

		public IMainView m_View;

		protected System.Collections.ArrayList m_WorkbenchViews = new System.Collections.ArrayList();

		MainMenu m_MainMenu;

		private System.Windows.Forms.PageSetupDialog m_PageSetupDialog;

		private System.Drawing.Printing.PrintDocument m_PrintDocument;

		private System.Windows.Forms.PrintDialog m_PrintDialog;


		/// <summary>
		/// Flag that indicates that the Application is about to be closed.
		/// </summary>
		private bool m_ApplicationIsClosing;

		#region Serialization
		
		public class MainControllerMemento
		{
			public MainControllerMemento(MainController ctrl)
			{
			}
			public MainControllerMemento()
			{
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MainControllerMemento),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				MainControllerMemento s = (MainControllerMemento)obj;
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				
				MainControllerMemento s = null!=o ? (MainControllerMemento)o : new MainControllerMemento();
				return s;
			}
		}

		#endregion
	
		public MainController(IMainView view, AltaxoDocument doc)
		{
			m_View = view;
			m_View.Controller = this;
			
			// we construct the main document
			if(null==m_Doc)
				m_Doc = new AltaxoDocument();
			else
				m_Doc = doc;

			// we initialize the printer variables
			m_PrintDocument = new System.Drawing.Printing.PrintDocument();
			// we set the print document default orientation to landscape
			m_PrintDocument.DefaultPageSettings.Landscape=true;
			m_PageSetupDialog = new System.Windows.Forms.PageSetupDialog();
			m_PageSetupDialog.Document = m_PrintDocument;
			m_PrintDialog = new System.Windows.Forms.PrintDialog();
			m_PrintDialog.Document = m_PrintDocument;

			// we create the menu and assign it to the view
			this.InitializeMenu();
			View.MainViewMenu = this.m_MainMenu;


			// wir konstruieren zu jeder Tabelle im Dokument ein GrafTabView
			CreateNewWorksheet();

			// we construct a empty graph by default
			CreateNewGraph(null);
		}

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
			// ******************************************************************
			// ******************************************************************
			// File Menu
			// ******************************************************************
			// ******************************************************************
			mi = new MenuItem("&File");
			mi.MergeOrder=0;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;

			// ------------------------------------------------------------------
			// File - New (Popup)
			// ------------------------------------------------------------------
			mi = new MenuItem("New");
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
			index2 = m_MainMenu.MenuItems[index].MenuItems.Count-1;

			// File - New - Worksheet 
			mi = new MenuItem("Worksheet");
			mi.Click += new EventHandler(EhMenuFileNewWorksheet_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

			// File - New - Graph 
			mi = new MenuItem("Graph");
			mi.Click += new EventHandler(EhMenuFileNewGraph_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

			// File - New -WorksheetFromFile 
			mi = new MenuItem("Worksheet/Graph from file");
			mi.Click += new EventHandler(EhMenuFileNewObjectFromFile_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

			// ------------------------------------------------------------------

			// File - Open
			mi = new MenuItem("Open..");
			mi.Click += new EventHandler(EhMenuFileOpen_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// File - Save
			mi = new MenuItem("Save");
			mi.Click += new EventHandler(EhMenuFileSave_OnClick);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// File - SaveAs
			mi = new MenuItem("SaveAs..");
			mi.Click += new EventHandler(EhMenuFileSaveAs_OnClick);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// File - Exit
			mi = new MenuItem("Exit");
			mi.MergeOrder = 100; // Exit should be the most last item in the menu
			mi.Click += new EventHandler(EhMenuFileExit_OnClick);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// ******************************************************************
			// ******************************************************************
			// Edit (Popup)
			// ******************************************************************
			// ****************************************************************** 
			mi = new MenuItem("Edit");
			mi.MergeOrder=1;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			//mi.Popup += new System.EventHandler(this.EhMenuEdit_OnPopup);
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;

		

			// ******************************************************************
			// ******************************************************************
			// Window (Popup)
			// ******************************************************************
			// ******************************************************************
			mi = new MenuItem("Window");
			//mi.Index=99; // the Window menu is the last but one menu item 
			mi.MergeOrder=99;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			mi.MdiList = true; // used to list the windows
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;

			// Window - Cascade
			mi = new MenuItem("Cascade");
			mi.Click += new EventHandler(EhMenuWindowCascade_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Window - Tile Horizontally
			mi = new MenuItem("Tile horizontally");
			mi.Click += new EventHandler(EhMenuWindowTileHorizontally_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Window - Tile vertically
			mi = new MenuItem("Tile vertically");
			mi.Click += new EventHandler(EhMenuWindowTileVertically_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Window - ArrangeIcons
			mi = new MenuItem("Arrange icons");
			mi.Click += new EventHandler(EhMenuWindowArrangeIcons_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Window - MinimizeAll
			mi = new MenuItem("Minimize all");
			mi.Click += new EventHandler(EhMenuWindowMinimizeAll_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Window - Maximize All
			mi = new MenuItem("Maximize all");
			mi.Click += new EventHandler(EhMenuWindowMaximizeAll_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			
			// ******************************************************************
			// ******************************************************************
			// Help (Popup)
			// ******************************************************************
			// ******************************************************************
			mi = new MenuItem("Help");
			//mi.Index=100;
			mi.MergeOrder=100;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;

			// Help - About Altaxo
			mi = new MenuItem("About Altaxo");
			mi.Click += new EventHandler(EhMenuHelpAboutAltaxo_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
		}

		#endregion // Menu definition

		#region Menu handlers

		// ******************************************************************
		// ******************************************************************
		// File Menu
		// ******************************************************************
		// ******************************************************************

		private void EhMenuFileNewWorksheet_OnClick(object sender, System.EventArgs e)
		{
			CreateNewWorksheet();
		}

		private void EhMenuFileNewGraph_OnClick(object sender, System.EventArgs e)
		{
			CreateNewGraph(null);
		}

	

		protected void EhMenuFileNewObjectFromFile_OnClick(object sender, System.EventArgs e)
		{
			System.IO.Stream myStream ;
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
 
			openFileDialog1.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*"  ;
			openFileDialog1.FilterIndex = 1 ;
			openFileDialog1.RestoreDirectory = true ;
 
			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((myStream = openFileDialog1.OpenFile()) != null)
				{
					Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
					info.BeginReading(myStream);
					object deserObject = info.GetValue("Table",null);
					info.EndReading();
					myStream.Close();

					// if it is a table, add it to the TableCollection
					if(deserObject is Altaxo.Data.DataTable)
					{
						Altaxo.Data.DataTable table = deserObject as Altaxo.Data.DataTable;
						if(table.Name==null || table.Name==string.Empty)
							table.TableName = this.Doc.TableCollection.FindNewTableName();
						else if( this.Doc.TableCollection.ContainsTable(table.Name))
							table.TableName = this.Doc.TableCollection.FindNewTableName(table.Name);

						this.Doc.TableCollection.Add(table);
						info.AnnounceDeserializationEnd(this.Doc); // fire the event to resolve path references
						
						CreateNewWorksheet(table);
					}
					else if(deserObject is Altaxo.Graph.GraphDocument)
					{
						Altaxo.Graph.GraphDocument graph = deserObject as Altaxo.Graph.GraphDocument;
						if(graph.Name==null || graph.Name==string.Empty)
							graph.Name = this.Doc.GraphDocumentCollection.FindNewName();
						else if( this.Doc.GraphDocumentCollection.Contains(graph.Name))
							graph.Name = this.Doc.GraphDocumentCollection.FindNewName(graph.Name);

						this.Doc.GraphDocumentCollection.Add(graph);
						info.AnnounceDeserializationEnd(this.Doc); // fire the event to resolve path references in the graph

						CreateNewGraph(graph);
					}

				}
			}
		}

		private void EhMenuFileOpen_OnClick(object sender, System.EventArgs e)
		{
			System.IO.Stream myStream;
			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.InitialDirectory = "c:\\temp\\" ;
			openFileDialog1.Filter = "txt files (*.axo)|*.axo|Altaxo zip files (*.zip)|*.zip|All files (*.*)|*.*" ;
			openFileDialog1.FilterIndex = 2 ;
			openFileDialog1.RestoreDirectory = true ;

			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((myStream = openFileDialog1.OpenFile())!= null)
				{
					if(openFileDialog1.FilterIndex==1)
					{
						//					System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
						System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

						System.Runtime.Serialization.SurrogateSelector ss = new System.Runtime.Serialization.SurrogateSelector();
						AltaxoStreamingContext additionalContext = new AltaxoStreamingContext();
						additionalContext.m_SurrogateSelector = ss;
						System.Runtime.Serialization.StreamingContext context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.All,additionalContext);
						formatter.Context = context;

						System.Collections.Hashtable versionList = (System.Collections.Hashtable)(formatter.Deserialize(myStream));

						System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
						foreach(Assembly assembly in assemblies)
						{
							// test if the assembly supports Serialization
							Attribute suppVersioning = Attribute.GetCustomAttribute(assembly,typeof(Altaxo.Serialization.SupportsSerializationVersioningAttribute));
							if(null==suppVersioning)
								continue; // this assembly don't support this, so skip it

							Type[] definedtypes = assembly.GetTypes();
							foreach(Type definedtype in definedtypes)
							{
								Attribute[] attributes = Attribute.GetCustomAttributes(definedtype,typeof(SerializationSurrogateAttribute));
								// compare with assembly version and search for the serialization
								// surrogate with the highest version where the version is lower than the
								// file version
								SerializationSurrogateAttribute bestattribute=null;
								int bestversion=-1;
								object hashversion = versionList[definedtype.FullName];
								int objversion = null==hashversion ? 0 : (int)hashversion;
								foreach(SerializationSurrogateAttribute att in attributes)
								{
									if(att.Version<=objversion && att.Version>bestversion)
									{
										bestattribute = att;
										bestversion = att.Version;
									}
								}
								if(null!=bestattribute)
								{
									ss.AddSurrogate(definedtype, formatter.Context, bestattribute.Surrogate);
								}
							}
						}

			
						/*
						AltaxoAdditionalContext additionalContext = new AltaxoAdditionalContext();
						additionalContext.m_SurrogateSelector = ss;
						additionalContext.m_FormatterType = formatter.GetType();
				*/
						
					
						formatter.SurrogateSelector=ss;
					
						object obj = formatter.Deserialize(myStream);
						m_Doc = (AltaxoDocument)obj;
						m_Doc.OnDeserialization(new DeserializationFinisher(this));
						System.Diagnostics.Trace.WriteLine("Deserialization of AltaxoDocument now completely finished.");
						// document.RestoreWindowsAfterDeserialization();
						// Code to write the stream goes here.

						myStream.Close();
					}
					else if(openFileDialog1.FilterIndex==2)
					{
						ZipFile zipFile = new ZipFile(myStream);
						Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
						AltaxoDocument newdocument = new AltaxoDocument();
						newdocument.RestoreFromZippedFile(zipFile,info);

						m_Doc = newdocument;
						this.RemoveAllWorkbenchViews();
						this.RestoreWindowStateFromZippedFile(zipFile,info,m_Doc);
						
						myStream.Close();
					} // Filterindex=2
				}
			}
		}


		private void EhMenuFileSave_OnClick(object sender, System.EventArgs e)
		{
			EhMenuFileSaveAs_OnClick(sender, e);
		}

		private void EhMenuFileSaveAs_OnClick(object sender, System.EventArgs e)
		{
			this.ShowSaveAsDialog();
		} // end method


		private void EhMenuFileExit_OnClick(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Application.Exit();
		}
	
		// ******************************************************************
		// ******************************************************************
		// Edit (Popup)
		// ******************************************************************
		// ****************************************************************** 


		// ******************************************************************
		// ******************************************************************
		// Window (Popup)
		// ******************************************************************
		// ******************************************************************

		private void EhMenuWindowCascade_OnClick(object sender, System.EventArgs e)
		{
			View.Form.LayoutMdi(MdiLayout.Cascade);
		}

		private void EhMenuWindowTileHorizontally_OnClick(object sender, System.EventArgs e)
		{
			View.Form.LayoutMdi(MdiLayout.TileHorizontal);
		}

		private void EhMenuWindowTileVertically_OnClick(object sender, System.EventArgs e)
		{
			View.Form.LayoutMdi(MdiLayout.TileVertical);
		}

		private void EhMenuWindowArrangeIcons_OnClick(object sender, System.EventArgs e)
		{
			View.Form.LayoutMdi(MdiLayout.ArrangeIcons);
		}

		private void EhMenuWindowMinimizeAll_OnClick(object sender, System.EventArgs e)
		{
			//Gets forms that represent the MDI child forms 
			//that are parented to this form in an array 
			Form[] charr= View.Form.MdiChildren; 
     
			//For each child form set the window state to Maximized 
			foreach (Form chform in charr) 
				chform.WindowState=FormWindowState.Minimized;
		}

		private void EhMenuWindowMaximizeAll_OnClick(object sender, System.EventArgs e)
		{
			//Gets forms that represent the MDI child forms 
			//that are parented to this form in an array 
			Form[] charr= View.Form.MdiChildren; 
     
			//For each child form set the window state to Maximized 
			foreach (Form chform in charr) 
				chform.WindowState=FormWindowState.Maximized;
		}

		// ******************************************************************
		// ******************************************************************
		// Help (Popup)
		// ******************************************************************
		// ******************************************************************

		private void EhMenuHelpAboutAltaxo_OnClick(object sender, System.EventArgs e)
		{
			Altaxo.Main.AboutDialog dlg = new Altaxo.Main.AboutDialog();
			dlg.ShowDialog(View.Form);
		}

		#endregion

		#region Properties
		public  System.Windows.Forms.PageSetupDialog PageSetupDialog
		{
			get { return m_PageSetupDialog; }
		}

		public  System.Drawing.Printing.PrintDocument PrintDocument
		{
			get { return m_PrintDocument; }
		}


		public System.Windows.Forms.PrintDialog PrintDialog
		{
			get { return m_PrintDialog; }
		}


		/// <summary>
		/// Indicates if true that the Application is about to be closed. Can be used by child forms to prevent the confirmation dialog that 
		/// normally appears also during close of the application, since the child windows also receive the closing message in this case.
		/// </summary>
		public bool IsClosing
		{
			get { return this.m_ApplicationIsClosing; }
		}

		#endregion

		#region IMainController members
		/// <summary>
		/// The document which is visualized by the controller, contains all data tables, graph, worksheet views and graph views
		/// </summary>
		public AltaxoDocument Doc
		{
			get	{	return m_Doc; }
		}

		/// <summary>
		/// The view that is controlled by this controller.
		/// </summary>
		public IMainView View
		{
			get { return m_View; }
		}


		public void EhView_Closing(System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true; // in doubt cancel the closing

			if(this.Doc.IsDirty)
			{
				System.Windows.Forms.DialogResult dlgres = System.Windows.Forms.MessageBox.Show(this.View.Form,"Do you want to save your document?","Attention",System.Windows.Forms.MessageBoxButtons.YesNoCancel);
				if(dlgres==System.Windows.Forms.DialogResult.Yes)
				{
					if(!this.ShowSaveAsDialog())
						e.Cancel = false;
				}
				else if(dlgres==System.Windows.Forms.DialogResult.No)
				{
					e.Cancel = false;
				}
				else if(dlgres==System.Windows.Forms.DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
			else // the document is not dirty
			{
				e.Cancel = false;
			}



			// update the closing flag - if e.Cancel is true, the application is not longer in the closing state
			this.m_ApplicationIsClosing = (false==e.Cancel);
		}

		public void EhView_Closed(System.EventArgs e)
		{
			View.Controller=null; // we are no longer the controller
		}

		public void EhView_CloseMessage()
		{
			this.m_ApplicationIsClosing = true;
		}


		#endregion

		protected bool ShowSaveAsDialog()
		{
			bool bRet = true;
			SaveFileDialog dlg = this.GetSaveAsDialog();
			if(dlg.ShowDialog(this.View.Form) == DialogResult.OK)
			{
				System.IO.Stream myStream;
				if((myStream = dlg.OpenFile()) != null)
				{
					try
					{
						if(dlg.FilterIndex==1)
							this.SaveDocument(myStream);
						else
						{
							Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
							ZipOutputStream zippedStream = new ZipOutputStream(myStream);
							this.Doc.SaveToZippedFile(zippedStream, info);
							this.SaveWindowStateToZippedFile(zippedStream, info);
							zippedStream.Close();
						}
						bRet = false;; // now saving was successfull, we can close the form
					}
					catch(Exception exc)
					{
						System.Windows.Forms.MessageBox.Show(this.View.Form,"An error occured saving the document, details see below:\n" + exc.ToString(),"Error",System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
					}
					finally
					{
						myStream.Close();
					}
				}
			}
			return bRet;
		}

		protected SaveFileDialog GetSaveAsDialog()
		{
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
			saveFileDialog1.Filter = "Altaxo files (*.axo)|*.axo|Altaxo zip files (*.axo.zip)|*.axo.zip|All files (*.*)|*.*"  ;
			saveFileDialog1.FilterIndex = 2 ;
			saveFileDialog1.RestoreDirectory = true ;
 	

			return saveFileDialog1;
		}


		public void SaveWindowStateToZippedFile(ZipOutputStream zippedStream, Altaxo.Serialization.Xml.XmlStreamSerializationInfo info)
		{
		
		{
			// first, we save our own state 
			ZipEntry ZipEntry = new ZipEntry("Workbench/MainWindow.xml");
			zippedStream.PutNextEntry(ZipEntry);
			zippedStream.SetLevel(0);
			info.BeginWriting(zippedStream);
			info.AddValue("MainWindow",new MainControllerMemento(this));
			info.EndWriting();
		}

			// second, we save all workbench windows into the Workbench/Views 
			int i=0;
			foreach(Main.GUI.IWorkbenchWindowController ctrl in this.m_WorkbenchViews)
			{
				i++;
				ZipEntry ZipEntry = new ZipEntry("Workbench/Views/View"+i.ToString()+".xml");
				zippedStream.PutNextEntry(ZipEntry);
				zippedStream.SetLevel(0);
				info.BeginWriting(zippedStream);
				info.AddValue("WorkbenchViewContent",ctrl.Content);
				info.EndWriting();
			}
		}


		public void RestoreWindowStateFromZippedFile(ZipFile zipFile, Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info, AltaxoDocument restoredDoc)
		{
			System.Collections.ArrayList restoredControllers = new System.Collections.ArrayList();
			foreach(ZipEntry zipEntry in zipFile)
			{
				if(!zipEntry.IsDirectory && zipEntry.Name.StartsWith("Workbench/Views/"))
				{
					System.IO.Stream zipinpstream = zipFile.GetInputStream(zipEntry);
					info.BeginReading(zipinpstream);
					object readedobject = info.GetValue("Table",this);
					if(readedobject is Main.GUI.IWorkbenchContentController)
						restoredControllers.Add(readedobject);
					info.EndReading();
				}
			}

			info.AnnounceDeserializationEnd(restoredDoc);
			info.AnnounceDeserializationEnd(this);

			// now give all restored controllers a view and show them in the Main view

			foreach(Main.GUI.IWorkbenchContentController ctrl in restoredControllers)
			{
				ctrl.CreateView();
				if(ctrl.View != null)
				{
					this.ShowContentInNewWorkbenchWindow(ctrl);
				}
			}

		}

		protected void SaveDocument(System.IO.Stream myStream)
		{
			System.Collections.Hashtable versionList = new System.Collections.Hashtable();
			//					System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
			System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			System.Runtime.Serialization.SurrogateSelector ss = new System.Runtime.Serialization.SurrogateSelector();
			AltaxoStreamingContext additionalContext = new AltaxoStreamingContext();
			additionalContext.m_SurrogateSelector = ss;
			System.Runtime.Serialization.StreamingContext context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.All,additionalContext);
			formatter.Context = context;


			System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			foreach(Assembly assembly in assemblies)
			{
				// test if the assembly supports Serialization
				Attribute suppVersioning = Attribute.GetCustomAttribute(assembly,typeof(Altaxo.Serialization.SupportsSerializationVersioningAttribute));
				if(null==suppVersioning)
					continue; // this assembly don't support this, so skip it
				
				Type[] definedtypes = assembly.GetTypes();
				foreach(Type definedtype in definedtypes)
				{
					SerializationVersionAttribute versionattribute = (SerializationVersionAttribute)Attribute.GetCustomAttribute(definedtype,typeof(SerializationVersionAttribute));
							
					if(null!=versionattribute)
						versionList.Add(definedtype.FullName,versionattribute.Version);
					
					Attribute[] surrogateattributes = Attribute.GetCustomAttributes(definedtype,typeof(SerializationSurrogateAttribute));
					// compare with assembly version and search for the serialization
					// surrogate with the highest version where the version is lower than the
					// file version
					SerializationSurrogateAttribute bestattribute=null;
					int bestversion=-1;
					int objversion = null==versionattribute ? 0 : versionattribute.Version;
					foreach(SerializationSurrogateAttribute att in surrogateattributes)
					{
						if(att.Version<=objversion && att.Version>bestversion)
						{
							bestattribute = att;
							bestversion = att.Version;
						}
					}
					if(null!=bestattribute)
					{
						ss.AddSurrogate(definedtype,formatter.Context, bestattribute.Surrogate);
					}
				} // end foreach type
			} // end foreach assembly 
					
							
			formatter.SurrogateSelector=ss;
			formatter.Serialize(myStream,versionList);
			formatter.Serialize(myStream, m_Doc);
			// Code to write the stream goes here.
			myStream.Close();
		} // end method


		public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(string worksheetName, bool bCreateDefaultColumns)
		{
			
			Altaxo.Data.DataTable dt1 = this.Doc.CreateNewTable(worksheetName, bCreateDefaultColumns);
			return CreateNewWorksheet(dt1);
		}
	
		public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(bool bCreateDefaultColumns)
		{
			return CreateNewWorksheet(this.Doc.TableCollection.FindNewTableName(),bCreateDefaultColumns);
		}

		public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet()
		{
			return CreateNewWorksheet(this.Doc.TableCollection.FindNewTableName(),false);
		}


		public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table)
		{
			Altaxo.Main.GUI.IWorkbenchWindowController wbv_controller = new Altaxo.Main.GUI.WorkbenchWindowController();
			Altaxo.Main.GUI.WorkbenchForm wbvform = new Altaxo.Main.GUI.WorkbenchForm(this.View.Form);
			wbv_controller.View = wbvform;

			Altaxo.Worksheet.GUI.WorksheetController ctrl = new Altaxo.Worksheet.GUI.WorksheetController(this.Doc.CreateNewTableLayout(table));
			Altaxo.Worksheet.GUI.WorksheetView view = new Altaxo.Worksheet.GUI.WorksheetView();
			ctrl.View = view;

			wbv_controller.Content = ctrl;
			
			this.m_WorkbenchViews.Add(wbv_controller);
			wbvform.Show();
			return ctrl;
		}

	

		public Altaxo.Graph.GUI.IGraphController CreateNewGraph()
		{
			return CreateNewGraph(Doc.CreateNewGraphDocument());
		}

	

		public Altaxo.Graph.GUI.IGraphController CreateNewGraph(Altaxo.Graph.GraphDocument graph)
		{
			Altaxo.Main.GUI.IWorkbenchWindowController wbv_controller = new Altaxo.Main.GUI.WorkbenchWindowController();
			Altaxo.Main.GUI.WorkbenchForm wbvform = new Altaxo.Main.GUI.WorkbenchForm(this.View.Form);
			wbv_controller.View = wbvform;

			if(graph==null)
				graph = this.Doc.CreateNewGraphDocument();

			Altaxo.Graph.GUI.GraphController ctrl = new Altaxo.Graph.GUI.GraphController(graph);
			Altaxo.Graph.GUI.GraphView view = new Altaxo.Graph.GUI.GraphView();
			ctrl.View = view;

			
			wbv_controller.Content = ctrl;

			this.m_WorkbenchViews.Add(wbv_controller);
			wbvform.Show();
			return ctrl;
		}


		public void ShowContentInNewWorkbenchWindow(Main.GUI.IWorkbenchContentController contentController)
		{
			Altaxo.Main.GUI.IWorkbenchWindowController wbv_controller = new Altaxo.Main.GUI.WorkbenchWindowController();
			Altaxo.Main.GUI.WorkbenchForm wbvform = new Altaxo.Main.GUI.WorkbenchForm(this.View.Form);
			wbv_controller.View = wbvform;

				
			wbv_controller.Content = contentController;
			this.m_WorkbenchViews.Add(wbv_controller);
			wbvform.Show();
		}

		/// <summary>This will remove the GraphController <paramref>ctrl</paramref> from the graph forms collection.</summary>
		/// <param name="ctrl">The GraphController to remove.</param>
		/// <remarks>No exception is thrown if the Form frm is not a member of the graph forms collection.</remarks>
		public void RemoveGraph(Altaxo.Graph.GUI.GraphController ctrl)
		{
			if(this.m_WorkbenchViews.Contains(ctrl))
				this.m_WorkbenchViews.Remove(ctrl);
			else if(ctrl.ParentWorkbenchWindowController !=null && this.m_WorkbenchViews.Contains(ctrl.ParentWorkbenchWindowController))
				this.m_WorkbenchViews.Remove(ctrl.ParentWorkbenchWindowController);
		}

		/// <summary>This will remove the Worksheet <paramref>ctrl</paramref> from the corresponding forms collection.</summary>
		/// <param name="ctrl">The Worksheet to remove.</param>
		/// <remarks>No exception is thrown if the Form frm is not a member of the worksheet forms collection.</remarks>
		public void RemoveWorksheet(Altaxo.Worksheet.GUI.WorksheetController ctrl)
		{
			if(this.m_WorkbenchViews.Contains(ctrl))
				this.m_WorkbenchViews.Remove(ctrl);
			else if(ctrl.ParentWorkbenchWindowController !=null && this.m_WorkbenchViews.Contains(ctrl.ParentWorkbenchWindowController))
				this.m_WorkbenchViews.Remove(ctrl.ParentWorkbenchWindowController);
		}

		public void RemoveAllWorkbenchViews()
		{
			foreach(Main.GUI.IWorkbenchWindowController ctrl in this.m_WorkbenchViews)
				ctrl.CloseView();

			this.m_WorkbenchViews.Clear();
		}
	}



	class App
	{
		// public static System.Runtime.Serialization.SurrogateSelector m_SurrogateSelector;


		private static MainController sm_theApplication;

		public static MainController Current
		{
			get { return sm_theApplication; }
		}


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			sm_theApplication = new MainController(new MainView(), new AltaxoDocument());

			try
			{
				System.Windows.Forms.Application.Run(Current.View.Form);
			}
			catch(Exception e)
			{
				System.Windows.Forms.MessageBox.Show(e.ToString());
			}
		}
	}
}
