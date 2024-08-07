﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using Altaxo.Drawing;

namespace Altaxo.Gui.Drawing.DashPatternManagement
{
  public interface IDashPatternView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IDashPatternView))]
  [UserControllerForObject(typeof(IDashPattern))]
  public class DashPatternController : MVCANControllerEditImmutableDocBase<IDashPattern, IDashPatternView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private IDashPattern _selectedItem;

    public IDashPattern SelectedItem
    {
      get => _selectedItem;
      set
      {
        if (!(_selectedItem == value))
        {
          _selectedItem = value;
          OnPropertyChanged(nameof(SelectedItem));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (_view is not null)
      {
        SelectedItem = _doc;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = SelectedItem;

      return ApplyEnd(true, disposeController);
    }

   
  }
}
