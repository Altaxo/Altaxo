#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Drawing.DashPatternManagement;
using Altaxo.Graph.Graph3D.Plot.Groups;

namespace Altaxo.Gui.Drawing.DashPatternManagement
{
  public interface IDashPatternListView : IStyleListView
  {
  }

  [ExpectedTypeOfView(typeof(IDashPatternListView))]
  [UserControllerForObject(typeof(DashPatternList))]
  public class DashPatternListController : StyleListController<DashPatternListManager, DashPatternList, IDashPattern>
  {
    public DashPatternListController()
      : base(DashPatternListManager.Instance)
    {
      CmdAddCustomDashPatternToList = new RelayCommand(EhUserRequest_AddCustomDashPatternToList);
    }

    #region Bindings

    public ICommand CmdAddCustomDashPatternToList { get; }

    private IDashPattern _customDashPattern = new Altaxo.Drawing.DashPatterns.Solid();

    public IDashPattern CustomDashPattern
    {
      get => _customDashPattern;
      set
      {
        if (!(_customDashPattern == value))
        {
          _customDashPattern = value;
          OnPropertyChanged(nameof(CustomDashPattern));
        }
      }
    }


    #endregion

    private void EhUserRequest_AddCustomDashPatternToList()
    {
      var dashPattern = CustomDashPattern;
      CurrentItems.Add(new SelectableListNode(ToDisplayName(dashPattern), dashPattern, false));
      SetListDirty();
    }
  }
}
