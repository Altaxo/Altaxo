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
using Altaxo.Main.GUI;


namespace Altaxo
{
	/// <summary>
	/// This class holds the application instance.
	/// </summary>
	public class Current
	{
	
		
		//private static object sm_theApplication;

		private static IWorkbench sm_theWorkbench;

		private static Altaxo.Main.IProjectService sm_theProjectService;

		private static Altaxo.Main.IPrintingService sm_thePrintingService;

		private static bool sm_theApplicationIsClosing;

		public static IWorkbench Workbench
		{
			get { return sm_theWorkbench; }
		}

		public static Altaxo.Main.IProjectService ProjectService
		{
			get { return sm_theProjectService; }
		}

		public static Altaxo.AltaxoDocument Project
		{
			get { return sm_theProjectService.CurrentOpenProject; }
		}

		public static Altaxo.Main.IPrintingService PrintingService
		{
			get { return sm_thePrintingService; }
		}

		public static bool ApplicationIsClosing
		{
			get { return sm_theApplicationIsClosing; }
			set { sm_theApplicationIsClosing = value; }
		}

		/// <summary>
		/// Returns the main windows form.
		/// </summary>
		public static Form MainWindow
		{
			get 
			{
				if(Current.Workbench is Form)
					return (Form)Current.Workbench;
				else
					return (Form)Current.Workbench.ViewObject;				
			}
		}
	

		public static void SetWorkbench(IWorkbench workbench)
		{
			if(null==sm_theWorkbench)
				sm_theWorkbench = workbench; 
			else
				throw new ApplicationException("The workbench can not be re-set to another value, only initialized for the first time!");

		}

		public static void SetProjectService(Altaxo.Main.IProjectService projectservice)
		{
			if(null==sm_theProjectService)
				sm_theProjectService = projectservice; 
			else
				throw new ApplicationException("The project service can not be re-set to another value, only initialized for the first time!");

		}

		public static void SetPrintingService(Altaxo.Main.IPrintingService printingservice)
		{
			if(null==sm_thePrintingService)
				sm_thePrintingService = printingservice; 
			else
				throw new ApplicationException("The printing service can not be re-set to another value, only initialized for the first time!");

		}

#if FormerGuiState
		/// <summary>
		/// The main entry point for the application. This function has to be called to
		/// run the application.
		/// </summary>
		[STAThread]
		public static void Main() 
		{
			if(null==sm_theProjectService)
			{
				sm_theProjectService  = new Altaxo.Main.ProjectService();

				sm_thePrintingService = new Altaxo.Main.PrintingService();

				// we construct the main document
				sm_theProjectService.CurrentOpenProject = new AltaxoDocument();

				MainController ctrl = new MainController();

				sm_theWorkbench = new AltaxoWorkbench(new MainView());
				
				ctrl.SetMenuToMainWindow();

				// InitializeMainController(ctrl);

			}
			try
			{
				System.Windows.Forms.Application.Run(Current.MainWindow);
			}
			catch(Exception e)
			{
				System.Windows.Forms.MessageBox.Show(e.ToString());
			}
		}
#endif
	}
}
