using Altaxo.AddInItems;
using Altaxo.Gui.Workbench;
using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.MenuCommands
{
  public class RecentFilesMenuBuilder : IMenuItemBuilder
  {
    public IEnumerable<object> BuildItems(Codon codon, object owner)
    {
      // search either as part of IFileService or directly as service IRecentOpen
      var recentOpen = Altaxo.Current.GetService<IFileService>()?.RecentOpen ?? Altaxo.Current.GetService<IRecentOpen>();

      if (null != recentOpen && recentOpen.RecentFiles.Count > 0)
      {
        var items = new System.Windows.Controls.MenuItem[recentOpen.RecentFiles.Count];

        for (int i = 0; i < recentOpen.RecentFiles.Count; ++i)
        {
          // variable inside loop, so that anonymous method refers to correct recent file
          string recentFile = recentOpen.RecentFiles[i];
          string accelaratorKeyPrefix = i < 10 ? "_" + ((i + 1) % 10) + " " : "";
          items[i] = new System.Windows.Controls.MenuItem()
          {
            Header = accelaratorKeyPrefix + recentFile
          };
          items[i].Click += delegate
          {
            Altaxo.Current.GetRequiredService<IFileService>().OpenFile(new FileName(recentFile));
          };
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
  }
}
