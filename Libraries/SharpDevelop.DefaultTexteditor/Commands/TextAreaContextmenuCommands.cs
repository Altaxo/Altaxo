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
using System.Text;

using Reflector.UserInterface;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Conditions;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class ShowBufferOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textarea = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			TabbedOptions o = new TabbedOptions(resourceService.GetString("Dialog.Options.BufferOptions"),
			                                    ((IProperties)propertyService.GetProperty("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new DefaultProperties())),
			                                    AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/ViewContent/DefaultTextEditor/OptionsDialog"));
			o.Width  = 450;
			o.Height = 425;
			o.FormBorderStyle = FormBorderStyle.FixedDialog;
			o.ShowDialog();
			o.Dispose();
			textarea.OptionsChanged();
		}
	}
	
	
	public class HighlightingTypeBuilder : ISubmenuBuilder
	{
		TextEditorControl  control      = null;
		CommandBarItem[] menuCommands = null;
		
		public CommandBarItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			control = (TextEditorControl)owner;
			
			ArrayList menuItems = new ArrayList();
			
			foreach (DictionaryEntry entry in HighlightingManager.Manager.HighlightingDefinitions) {
				SdMenuCheckBox item = new SdMenuCheckBox(null, null, entry.Key.ToString());
				item.Click    += new EventHandler(ChangeSyntax);
				item.IsChecked = control.Document.HighlightingStrategy.Name == entry.Key.ToString();
				menuItems.Add(item);
			}
			menuCommands = (CommandBarItem[])menuItems.ToArray(typeof(CommandBarItem));
			return menuCommands;
		}
		
		void ChangeSyntax(object sender, EventArgs e)
		{
			if (control != null) {
				SdMenuCheckBox item = (SdMenuCheckBox)sender;
				foreach (SdMenuCheckBox i in menuCommands) {
					i.IsChecked = false;
				}
				item.IsChecked = true;
				IHighlightingStrategy strat = HighlightingStrategyFactory.CreateHighlightingStrategy(item.Text);
				if (strat == null) {
					throw new Exception("Strategy can't be null");
				}
				control.Document.HighlightingStrategy = strat;
				control.Refresh();
			}
		}
	}	
}
