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
using System.Collections.Generic;
using Altaxo.Data.Selections;

namespace Altaxo.Gui.Data.Selections
{
  public interface IUnionOfRowSelectionView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(UnionOfRowSelections), 100)]
  [ExpectedTypeOfView(typeof(IUnionOfRowSelectionView))]
  public class UnionOfRowSelectionsController : MVCANControllerEditImmutableDocBase<UnionOfRowSelections, IUnionOfRowSelectionView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _mergeAdjoiningSegments;

    public bool MergeAdjoiningSegments
    {
      get => _mergeAdjoiningSegments;
      set
      {
        if (!(_mergeAdjoiningSegments == value))
        {
          _mergeAdjoiningSegments = value;
          OnPropertyChanged(nameof(MergeAdjoiningSegments));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        MergeAdjoiningSegments = _doc.MergeAdjoinigSegments;
      }
    }

    public override bool Apply(bool disposeController)
    {
      var mergeAdjoiningSegments = MergeAdjoiningSegments;

      _doc = _doc.WithMergeAdjoiningSegments(mergeAdjoiningSegments);
      _originalDoc = _doc;

      return ApplyEnd(true, disposeController);
    }
  }
}
