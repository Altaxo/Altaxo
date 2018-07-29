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
using Altaxo.Drawing.ColorManagement;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Altaxo.Gui.Autostart
{
  public class StartColorSetManager : ICommand
  {
    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      var colorSets = AddInTree.GetTreeNode("/Altaxo/ApplicationColorSets").BuildChildItems<Tuple<IColorSet, bool>>(this);
      foreach (var entry in colorSets)
      {
        IColorSet storedList;
        ColorSetManager.Instance.TryRegisterList(entry.Item1, ItemDefinitionLevel.Application, out storedList);
        if (entry.Item2)
          ColorSetManager.Instance.DeclareAsPlotColorList(storedList);
      }
    }
  }
}
