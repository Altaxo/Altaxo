// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Services;

#if !ModifiedForAltaxo
using ICSharpCode.SharpRefactory.PrettyPrinter.VB;
using ICSharpCode.SharpRefactory.Parser.VB;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class CSharpConvertBuffer : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null && window.ViewContent is IEditable) {
				Parser p = new Parser();
				p.Parse(new Lexer(new ICSharpCode.SharpRefactory.Parser.VB.StringReader(((IEditable)window.ViewContent).Text)));
				
				if (p.Errors.count > 0) {
					Console.WriteLine(p.Errors.ErrorOutput);
					
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError("Correct source code errors first (only correct source code would convert).");
					return;
				}
				CSharpVisitor vbv = new CSharpVisitor();
				vbv.Visit(p.compilationUnit, null);
				
				IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				fileService.NewFile("Generated.CS", "C#", vbv.SourceText.ToString());
			}
		}
	}
}
#endif
