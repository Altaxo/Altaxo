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

		MainMenu m_MainMenu;

		private System.Windows.Forms.PageSetupDialog m_PageSetupDialog;

		private System.Drawing.Printing.PrintDocument m_PrintDocument;

		private System.Windows.Forms.PrintDialog m_PrintDialog;


		/// <summary>
		/// Flag that indicates that the Application is about to be closed.
		/// </summary>
		private bool m_ApplicationIsClosing;

	
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
			Doc.CreateNewWorksheet(View.Form);

			// we construct a empty graph by default
			Doc.CreateNewGraph(View.Form);
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
			m_Doc.CreateNewWorksheet(View.Form);
		}

		private void EhMenuFileNewGraph_OnClick(object sender, System.EventArgs e)
		{
			m_Doc.CreateNewGraph(View.Form);
		}

		private void EhMenuFileOpen_OnClick(object sender, System.EventArgs e)
		{
			System.IO.Stream myStream;
			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.InitialDirectory = "c:\\temp\\" ;
			openFileDialog1.Filter = "txt files (*.axo)|*.axo|All files (*.*)|*.*" ;
			openFileDialog1.FilterIndex = 2 ;
			openFileDialog1.RestoreDirectory = true ;

			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((myStream = openFileDialog1.OpenFile())!= null)
				{
					//					System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
					System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
					System.Collections.Hashtable versionList = (System.Collections.Hashtable)(formatter.Deserialize(myStream));
					System.Runtime.Serialization.SurrogateSelector ss = new System.Runtime.Serialization.SurrogateSelector();

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
								ss.AddSurrogate(definedtype,formatter.Context, bestattribute.Surrogate);
							}
						}
					}

			
					/*
					AltaxoAdditionalContext additionalContext = new AltaxoAdditionalContext();
					additionalContext.m_SurrogateSelector = ss;
					additionalContext.m_FormatterType = formatter.GetType();
			
					System.Runtime.Serialization.StreamingContext context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.All,additionalContext);
					formatter.Context = context;
					*/
					
					formatter.SurrogateSelector=ss;

					App.m_SurrogateSelector = ss;
					object obj = formatter.Deserialize(myStream);
					App.m_SurrogateSelector = null;	
					m_Doc = (AltaxoDocument)obj;
					m_Doc.OnDeserialization(new DeserializationFinisher(this));
					System.Diagnostics.Trace.WriteLine("Deserialization of AltaxoDocument now completely finished.");
					// document.RestoreWindowsAfterDeserialization();
					// Code to write the stream goes here.

					myStream.Close();
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
						this.SaveDocument(myStream);
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
 
			saveFileDialog1.Filter = "Altaxo files (*.axo)|*.axo|All files (*.*)|*.*"  ;
			saveFileDialog1.FilterIndex = 2 ;
			saveFileDialog1.RestoreDirectory = true ;
 	

			return saveFileDialog1;
		}


		protected void SaveDocument(System.IO.Stream myStream)
		{
					System.Collections.Hashtable versionList = new System.Collections.Hashtable();
					//					System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
					System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
					System.Runtime.Serialization.SurrogateSelector ss = new System.Runtime.Serialization.SurrogateSelector();
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
					App.m_SurrogateSelector = ss;
					formatter.Serialize(myStream, m_Doc);
					App.m_SurrogateSelector=null;
					// Code to write the stream goes here.
					myStream.Close();
		} // end method



		public Altaxo.Worksheet.ITableView CreateNewWorksheet(bool bCreateDefault)
		{
			return Doc.CreateNewWorksheet(View.Form, bCreateDefault);
		}
		public Altaxo.Worksheet.ITableView CreateNewWorksheet(Altaxo.Data.DataTable table)
		{
			return Doc.CreateNewWorksheet(View.Form, table);
		}

		public Altaxo.Graph.IGraphView CreateNewGraph()
		{
			return Doc.CreateNewGraph(View.Form);
		}


	}


	class App
	{
		public static System.Runtime.Serialization.SurrogateSelector m_SurrogateSelector;


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
