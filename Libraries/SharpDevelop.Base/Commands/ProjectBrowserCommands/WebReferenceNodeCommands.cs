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
using System.Web.Services.Description;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser;

namespace ICSharpCode.SharpDevelop.Commands.ProjectBrowser
{
	public class RefreshWebReference : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			WebReferenceNode node = browser.SelectedNode as WebReferenceNode;
			if (node != null) {
				CloseOpenFiles(node);
				Refresh(node);
			}
		}
		
		void Refresh(WebReferenceNode node)
		{
      ServiceDescription desc = ICSharpCode.SharpDevelop.Gui.Dialogs.WebReference.ReadServiceDescription(node.Uri);
			if (desc != null) {
				Refresh(node, desc);
			} else {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));	
				messageService.ShowErrorFormatted(stringParserService.Parse("${res:ICSharpCode.SharpDevelop.Commands.ProjectBrowser.RefreshWebReference.ReadServiceDescriptionError}"), node.Uri);
			}
		}
		
		void Refresh(WebReferenceNode node, ServiceDescription desc)
		{			
			// Regenerate the proxy
      ICSharpCode.SharpDevelop.Gui.Dialogs.WebReference.GenerateWebProxyCode(node.ProxyNamespace, node.ProxyFileName, desc);
			
			// Save the wsdl.
			desc.Write(node.WsdlFileName);
			
			// Add new code to code completion.
			IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			parserService.ParseFile(node.ProxyFileName);		
		}
		
		/// <summary>
		/// Closes any files that are currently on display that will
		/// be regenerated when refreshing the proxy code.
		/// </summary>
		void CloseOpenFiles(WebReferenceNode node)
		{
			CloseView(node.ProxyFileName);
			CloseView(node.WsdlFileName);
		}
		
		/// <summary>
		/// Closes the file if it is currently being viewed.
		/// </summary>
		void CloseView(string fileName)
		{
			IWorkbench workbench = ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench;
			foreach (IViewContent viewContent in workbench.ViewContentCollection) {
				if (viewContent.FileName.ToLower() == fileName.ToLower()) {
					IWorkbenchWindow window = viewContent.WorkbenchWindow;
					window.CloseWindow(false);
					break;
				}
			}
		}
	}
}
