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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.AddInItems;
using Altaxo.Main.Services;

namespace Altaxo.Main.Commands
{
  /// <summary>
  /// Taken from Commands.MenuItemBuilders. See last line for change.
  /// </summary>
  public class RecentProjectsMenuBuilder : IMenuItemBuilder
  {
    public IEnumerable<object> BuildItems(Codon codon, object owner)
    {
      var recentOpen = Current.GetRequiredService<IRecentOpen>();

      if (recentOpen.RecentProjects.Count > 0)
      {
        var items = new System.Windows.Controls.MenuItem[recentOpen.RecentProjects.Count];

        for (int i = 0; i < recentOpen.RecentProjects.Count; ++i)
        {
          // variable inside loop, so that anonymous method refers to correct recent file
          string recentProjectFile = recentOpen.RecentProjects[i];
          string acceleratorKeyPrefix = i < 10 ? "_" + ((i + 1) % 10) + " " : "";
          items[i] = new System.Windows.Controls.MenuItem()
          {
            Header = acceleratorKeyPrefix + recentProjectFile
          };
          items[i].Click += delegate
          {
            Current.IProjectService.OpenProject(recentProjectFile, false);
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
