using System;
using System.Windows.Forms;
using ICSharpCode.Core.AddIns.Codons;
using Altaxo;
using Altaxo.Main;
using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo.Worksheet.Commands
{
	public class PlotLine : AbstractMenuCommand
	{
		public override void Run()
		{
			Altaxo.Worksheet.GUI.WorksheetController ctrl 
				= App.Current.ActiveWorkbenchWindow.ActiveViewContent 
				as Altaxo.Worksheet.GUI.WorksheetController;
			
			DataGridOperations.PlotLine(ctrl, true, false);
		}
	}
}
