using System;
using System.Windows.Forms;
using ICSharpCode.Core.AddIns.Codons;
using Altaxo;
using Altaxo.Main;
using Altaxo.Worksheet;
using Altaxo.Worksheet.GUI;
using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo.Worksheet.Commands
{
	/// <summary>
	/// Summary description for OpenScriptDialog.
	/// </summary>
	public class OpenScriptDialog : AbstractMenuCommand
	{
		public override void Run()
		{
			Form form = new Form();
		

			ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBindingWrapper ctrl =
				new ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBindingWrapper();

			ctrl.TextEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			form.Controls.Add(ctrl.TextEditorControl);
			ctrl.TextEditorControl.FileName = @"blablatest01.CS";
			ctrl.ContentName = @"blablatest01.CS";


			ICSharpCode.SharpDevelop.Services.DefaultParserService parserService = (ICSharpCode.SharpDevelop.Services.DefaultParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ICSharpCode.SharpDevelop.Services.DefaultParserService));

			if(parserService!=null)
			{
				parserService.ShowDialog(form,Altaxo.Current.MainWindow,ctrl);
			}
			else
			{
				form.ShowDialog(Altaxo.Current.MainWindow);
			}
		}

	}
}
