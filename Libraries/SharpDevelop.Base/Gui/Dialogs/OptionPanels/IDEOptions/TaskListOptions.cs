// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class TaskListOptions : AbstractOptionPanel
	{
		const string taskListView        = "taskListView";
		const string nameTextBox         = "nameTextBox";
		const string changeButton        = "changeButton";
		const string removeButton        = "removeButton";
		const string addButton           = "addButton";
		
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\TaskListOptions.xfrm"));
			string tasklisttokens = PropertyService.GetProperty("SharpDevelop.TaskListTokens", "HACK;TODO;UNDONE;FIXME");
			string[] tokens = tasklisttokens.Split(';');
			((ListView)ControlDictionary[taskListView]).BeginUpdate();
			foreach (string token in tokens) {
				((ListView)ControlDictionary[taskListView]).Items.Add(new ListViewItem(token));
			}
			((ListView)ControlDictionary[taskListView]).EndUpdate();
			((ListView)ControlDictionary[taskListView]).SelectedIndexChanged += new EventHandler(TaskListViewSelectedIndexChanged);
			
			ControlDictionary[changeButton].Click += new EventHandler(ChangeButtonClick);
			ControlDictionary[removeButton].Click += new EventHandler(RemoveButtonClick);
			ControlDictionary[addButton].Click    += new EventHandler(AddButtonClick);
			
			TaskListViewSelectedIndexChanged(this, EventArgs.Empty);
		}
		
		public override bool StorePanelContents()
		{
			ArrayList tokens = new ArrayList();
			foreach (ListViewItem item in ((ListView)ControlDictionary[taskListView]).Items) {
				tokens.Add(item.Text);
			}
			
			PropertyService.SetProperty("SharpDevelop.TaskListTokens", String.Join(";", (string[])tokens.ToArray(typeof(string))));
			
			return true;
		}
		
		void AddButtonClick(object sender, EventArgs e)
		{
			string newItemText = ControlDictionary[nameTextBox].Text;
			foreach (ListViewItem item in ((ListView)ControlDictionary[taskListView]).Items) {
				if (item.Text == newItemText) {
					return;
				}
			}
			((ListView)ControlDictionary[taskListView]).Items.Add(new ListViewItem(newItemText));
		}
		
		void ChangeButtonClick(object sender, EventArgs e)
		{
			((ListView)ControlDictionary[taskListView]).SelectedItems[0].Text = ControlDictionary[nameTextBox].Text;
		}
		void RemoveButtonClick(object sender, EventArgs e)
		{
			((ListView)ControlDictionary[taskListView]).Items.Remove(((ListView)ControlDictionary[taskListView]).SelectedItems[0]);
		}
		
		void TaskListViewSelectedIndexChanged(object sender, EventArgs e)
		{
			if (((ListView)ControlDictionary[taskListView]).SelectedItems.Count > 0) {
				ControlDictionary[nameTextBox].Text = ((ListView)ControlDictionary[taskListView]).SelectedItems[0].Text;
				ControlDictionary[changeButton].Enabled = true;
				ControlDictionary[removeButton].Enabled = true;
			} else {
				ControlDictionary[nameTextBox].Text = String.Empty;
				ControlDictionary[changeButton].Enabled = false;
				ControlDictionary[removeButton].Enabled = false;
			}
		}
		
		public TaskListOptions()
		{
		}
	}
}
