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
			App.InitializeMainController(new ICSharpCode.SharpDevelop.Gui.BeautyWorkbench(new ICSharpCode.SharpDevelop.Gui.BeautyWorkbenchWindow(), new AltaxoDocument()));

			App.Main();
		}
	}
}
