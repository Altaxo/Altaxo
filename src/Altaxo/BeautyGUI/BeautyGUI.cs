using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using Altaxo;
using Altaxo.Main;


namespace BeautyGUI
{
	class Startup
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			App.InitializeMainController(new ICSharpCode.SharpDevelop.Gui.DefaultWorkbench(new ICSharpCode.SharpDevelop.Gui.DefaultWorkbenchWindow(), new AltaxoDocument()));

			App.Main();
		}
	}
}
