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
