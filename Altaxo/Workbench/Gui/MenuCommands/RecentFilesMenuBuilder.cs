#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

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
