using Altaxo.AddInItems;
using Altaxo.Gui.Workbench;
using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Altaxo.Gui.MenuCommands
{
	/// <summary>
	/// Taken from Commands.MenuItemBuilders. See last line for change.
	/// </summary>
	public class RecentProjectsMenuBuilder : IMenuItemBuilder
	{
		public IEnumerable<object> BuildItems(Codon codon, object owner)
		{
			// search either as part of IFileService or directly as service IRecentOpen
			var recentOpen = Altaxo.Current.GetService<IFileService>()?.RecentOpen ?? Altaxo.Current.GetService<IRecentOpen>();

			if (null != recentOpen && recentOpen.RecentProjects.Count > 0)
			{
				var items = new System.Windows.Controls.MenuItem[recentOpen.RecentProjects.Count];

				for (int i = 0; i < recentOpen.RecentProjects.Count; ++i)
				{
					// variable inside loop, so that anonymous method refers to correct recent file
					string recentProjectFile = recentOpen.RecentProjects[i];
					string acceleratorKeyPrefix = i < 10 ? "_" + ((i + 1) % 10) + " " : "";
					items[i] = new System.Windows.Controls.MenuItem()
					{
						Header = acceleratorKeyPrefix + recentProjectFile,
						Tag = recentProjectFile
					};

					items[i].Click += EhHandleClick;
				}
				return items;
			}
			else
			{
				return new[] { new System.Windows.Controls.MenuItem {
												Header = StringParser.Parse("${res:Dialog.Componnents.RichMenuItem.NoRecentFilesString}"),
												IsEnabled = false
										} };
			}
		}

		private static void EhHandleClick(object sender, RoutedEventArgs e)
		{
			if (sender is FrameworkElement ele && ele.Tag is string recentProjectFile)
			{
				Current.IProjectService.OpenProject(recentProjectFile, false);
			}
		}
	}
}