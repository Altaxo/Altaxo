// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Altaxo.AddInItems;
using Altaxo.Main.Services;
using Altaxo.Gui.AddInItems;
using Altaxo.Gui.Workbench;
using Altaxo.Gui;
using Altaxo.Workbench;

namespace Altaxo.Gui.Workbench.Commands
{
    public class NavigationHistoryMenuBuilder : IMenuItemBuilder
    {
        // TODO: refactor BuildSubmenu to add a choice between flat and perfile, eventually per class/method sorting of the list

        /*ToolStripItem[] BuildMenuFlat(ICollection<INavigationPoint> points, int additionalItems)
		{
			ToolStripItem[] items = new ToolStripItem[points.Count+additionalItems];
			MenuCommand cmd = null;
			INavigationPoint p = null;
			List<INavigationPoint> list = new List<INavigationPoint>(points);

			int n = points.Count-1; // the last point
			int i = 0;
			while (i<points.Count) {
				p = list[n-i];
				cmd = new MenuCommand(p.Description, new EventHandler(NavigateTo));
				cmd.Tag = p;
//					if (p == NavigationService.CurrentPosition) {
//						cmd.Text = "*** "+cmd.Text;
//					}
				items[i++] = cmd;
			}
			return items;
		}*/

        private List<FrameworkElement> BuildMenuByFile(ICollection<INavigationPoint> points)
        {
            List<FrameworkElement> items = new List<FrameworkElement>();
            Dictionary<string, List<INavigationPoint>> files =
                new Dictionary<string, List<INavigationPoint>>();
            List<string> fileNames = new List<string>();

            foreach (INavigationPoint p in points)
            {
                if (p.FileName == null)
                {
                    throw new ApplicationException("should not get here!");
                }
                if (!fileNames.Contains(p.FileName))
                {
                    fileNames.Add(p.FileName);
                    files.Add(p.FileName, new List<INavigationPoint>());
                }
                if (!files[p.FileName].Contains(p))
                {
                    files[p.FileName].Add(p);
                }
            }

            fileNames.Sort();

            MenuItem containerItem = null;
            MenuItem cmd = null;

            foreach (string fname in fileNames)
            {
                // create a menu bucket
                containerItem = new MenuItem();
                containerItem.Header = System.IO.Path.GetFileName(fname);
                containerItem.ToolTip = fname;

                // sort and populate the bucket's contents
                //				files[fname].Sort();
                foreach (INavigationPoint p in files[fname])
                {
                    cmd = new MenuItem();
                    cmd.Header = p.Description;
                    cmd.Click += NavigateTo;
                    cmd.Tag = p;
                    containerItem.Items.Add(cmd);
                }

                // if there's only one nested item, add it
                // to the result directly, ignoring the bucket
                //				if (containerItem.DropDownItems.Count==1) {
                //					items[i] = containerItem.DropDownItems[0];
                //					items[i].Text = ((INavigationPoint)items[i].Tag).FullDescription;
                //					i++;
                //				} else {
                //					// add the bucket to the result
                //					items[i++] = containerItem;
                //				}
                // add the bucket to the result
                items.Add(containerItem);
            }

            return items;
        }

        public IEnumerable<object> BuildItems(Codon codon, object owner)
        {
            if (NavigationService.CanNavigateBack || NavigationService.CanNavigateForwards)
            {
                ICollection<INavigationPoint> points = NavigationService.Points;

                //ToolStripItem[] items = BuildMenuFlat(points, numberOfAdditionalItems);
                var result = BuildMenuByFile(points);

                // additional item 1
                result.Add(new Separator());

                // additional item 2
                MenuItem clearHistory = new MenuItem();
                clearHistory.Header = StringParser.Parse("${res:XML.MainMenu.Navigation.ClearHistory}");
                clearHistory.Click += delegate { NavigationService.ClearHistory(); };
                result.Add(clearHistory);

                return result;
            }

            // default is to disable the dropdown feature...
            return null;
        }

        public void NavigateTo(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            NavigationService.Go((INavigationPoint)item.Tag);
        }
    }

    public class OpenContentsMenuBuilder : IMenuItemBuilder
    {
        public IEnumerable<object> BuildItems(Codon codon, object owner)
        {
            var workbench = Altaxo.Current.GetService<IWorkbench>();

            int windowCount = workbench.ViewContentCollection.Count;
            if (windowCount == 0)
            {
                return new object[] { };
            }
            var items = new object[windowCount + 1];
            items[0] = new System.Windows.Controls.Separator();
            int i = 1;
            foreach (var window in workbench.ViewContentCollection)
            {
                var item = new System.Windows.Controls.MenuItem()
                {
                    IsChecked = workbench.ActiveViewContent == window,
                    IsCheckable = true,
                    Header = StringParser.Parse(window.Title).Replace("_", "__")
                };
                item.Click += delegate
                {
                    window.IsSelected = true;
                };
                items[i++] = item;
            }
            return items;
        }
    }

    //	public class IncludeFilesBuilder : ISubmenuBuilder
    //	{
    //		public ProjectBrowserView browser;
    //
    //		MyMenuItem includeInCompileItem;
    //		MyMenuItem includeInDeployItem;
    //
    //		class MyMenuItem : MenuCheckBox
    //		{
    //			IncludeFilesBuilder builder;
    //
    //			public MyMenuItem(IncludeFilesBuilder builder, string name, EventHandler handler) : base(null, null, name)
    //			{
    //				base.Click += handler;
    //				this.builder = builder;
    //			}
    //
    //			public override void UpdateStatus()
    //			{
    //				base.UpdateStatus();
    //				if (builder == null) {
    //					return;
    //				}
    //				AbstractBrowserNode node = builder.browser.SelectedNode as AbstractBrowserNode;
    //
    //				if (node == null) {
    //					return;
    //				}
    //
    //				ProjectFile finfo = node.UserData as ProjectFile;
    //				if (finfo == null) {
    //					builder.includeInCompileItem.Enabled = builder.includeInCompileItem.Enabled = false;
    //				} else {
    //					if (!builder.includeInCompileItem.Enabled) {
    //						builder.includeInCompileItem.Enabled = builder.includeInCompileItem.Enabled = true;
    //					}
    //					builder.includeInCompileItem.Checked = finfo.BuildAction == BuildAction.Compile;
    //					builder.includeInDeployItem.Checked  = !node.Project.DeployInformation.IsFileExcluded(finfo.Name);
    //				}
    //			}
    //		}
    //
    //		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
    //		{
    //			browser = (ProjectBrowserView)owner;
    //			includeInCompileItem = new MyMenuItem(this, "${res:ProjectComponent.ContextMenu.IncludeMenu.InCompile}", new EventHandler(ChangeCompileInclude));
    //			includeInDeployItem  = new MyMenuItem(this, "${res:ProjectComponent.ContextMenu.IncludeMenu.InDeploy}",  new EventHandler(ChangeDeployInclude));
    //
    //			return new ToolStripItem[] {
    //				includeInCompileItem,
    //				includeInDeployItem
    //			};
    //
    //		}
    //		void ChangeCompileInclude(object sender, EventArgs e)
    //		{
    //			AbstractBrowserNode node = browser.SelectedNode as AbstractBrowserNode;
    //
    //			if (node == null) {
    //				return;
    //			}
    //
    //			ProjectFile finfo = node.UserData as ProjectFile;
    //			if (finfo != null) {
    //				if (finfo.BuildAction == BuildAction.Compile) {
    //					finfo.BuildAction = BuildAction.Nothing;
    //				} else {
    //					finfo.BuildAction = BuildAction.Compile;
    //				}
    //			}
    //
    //			ProjectService.SaveCombine();
    //		}
    //
    //		void ChangeDeployInclude(object sender, EventArgs e)
    //		{
    //			AbstractBrowserNode node = browser.SelectedNode as AbstractBrowserNode;
    //
    //			if (node == null) {
    //				return;
    //			}
    //
    //			ProjectFile finfo = node.UserData as ProjectFile;
    //			if (finfo != null) {
    //				if (node.Project.DeployInformation.IsFileExcluded(finfo.Name)) {
    //					node.Project.DeployInformation.RemoveExcludedFile(finfo.Name);
    //				} else {
    //					node.Project.DeployInformation.AddExcludedFile(finfo.Name);
    //				}
    //			}
    //
    //			ProjectService.SaveCombine();
    //		}
    //	}

    public class ToolsViewMenuBuilder : ViewMenuBuilder
    {
        protected override string Category
        {
            get
            {
                return "Tools";
            }
        }
    }

    public class MainViewMenuBuilder : ViewMenuBuilder
    {
        protected override string Category
        {
            get
            {
                return "Main";
            }
        }
    }

    /// <summary>
    /// Responsible to build the view menu for tool pads of a distinct category.
    /// </summary>
    /// <seealso cref="Altaxo.AddInItems.IMenuItemBuilder" />
    public abstract class ViewMenuBuilder : IMenuItemBuilder
    {
        private class BringPadToFrontCommand : System.Windows.Input.ICommand
        {
            private PadDescriptor padDescriptor;

            public BringPadToFrontCommand(PadDescriptor padDescriptor)
            {
                this.padDescriptor = padDescriptor;
            }

            public event EventHandler CanExecuteChanged { add { } remove { } }

            public void Execute(object parameter)
            {
                padDescriptor.BringPadToFront();
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }
        }

        protected abstract string Category
        {
            get;
        }

        public IEnumerable<object> BuildItems(Codon codon, object owner)
        {
            var workbench = Altaxo.Current.GetService<IWorkbench>();

            List<object> list = new List<object>();
            foreach (IPadContent padContent in workbench.PadContentCollection)
            {
                if (padContent.Category == Category && null != padContent.PadDescriptor)
                {
                    var item = new System.Windows.Controls.MenuItem();
                    item.Header = MenuService.ConvertLabel(StringParser.Parse(padContent.Title));
                    if (!string.IsNullOrEmpty(padContent.IconSource))
                    {
                        item.Icon = Altaxo.Current.ResourceService.GetImage(padContent.IconSource).CreateImage();
                    }
                    item.Command = new BringPadToFrontCommand(padContent.PadDescriptor);
                    if (!string.IsNullOrEmpty(padContent.Shortcut))
                    {
                        var kg = MenuService.ParseShortcut(padContent.Shortcut);
                        ((System.Windows.Window)workbench.ViewObject).InputBindings.Add(
                            new System.Windows.Input.InputBinding(item.Command, kg)
                        );
                        item.InputGestureText = MenuService.GetDisplayStringForShortcut(kg);
                    }

                    list.Add(item);
                }
            }
            return list;
        }
    }
}