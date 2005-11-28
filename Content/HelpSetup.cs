#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using System.Configuration.Install;

namespace Altaxo
{
	/// <summary>
	/// Summary description for HelpSetup.
	/// </summary>
	[RunInstaller(true)]
	public class HelpSetup : System.Configuration.Install.Installer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public HelpSetup()
		{
			// This call is required by the Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		public override void Install(IDictionary stateSaver)
		{
			/*
			System.IO.StreamWriter streamwriter = new System.IO.StreamWriter(@"C:\TEMP\Content.log",false);

			streamwriter.WriteLine("Application Startup: " + System.Windows.Forms.Application.StartupPath); 
			streamwriter.WriteLine("Assembly location: " + System.Reflection.Assembly.GetExecutingAssembly().Location);
			streamwriter.WriteLine("Assembly directory: " + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

			string basePath = System.IO.Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)
				+ System.IO.Path.DirectorySeparatorChar +
				".." + System.IO.Path.DirectorySeparatorChar +
				"doc" + System.IO.Path.DirectorySeparatorChar +
				"help";

			streamwriter.WriteLine("Help file: " + basePath);



			streamwriter.Close();

			*/


			ICSharpCode.HelpConverter.HelpBrowserApp.Main(null) ;
			
			base.Install (stateSaver);

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
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
