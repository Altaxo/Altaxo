#region Copyright

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Science.Spectroscopy.SpikeRemoval;

namespace Altaxo.Gui.Analysis.Spectroscopy.SpikeRemoval
{
  public interface ISpikeRemovalByPeakEliminationView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(SpikeRemovalByPeakElimination))]
  [ExpectedTypeOfView(typeof(ISpikeRemovalByPeakEliminationView))]
  public class SpikeRemovalByPeakEliminationController : MVCANControllerEditImmutableDocBase<SpikeRemovalByPeakElimination, ISpikeRemovalByPeakEliminationView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _maximalWidth;

    public int MaximalWidth
    {
      get => _maximalWidth;
      set
      {
        if (!(_maximalWidth == value))
        {
          _maximalWidth = value;
          OnPropertyChanged(nameof(MaximalWidth));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        MaximalWidth = _doc.MaximalWidth;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with { MaximalWidth = MaximalWidth };

      return ApplyEnd(true, disposeController);
    }

   
  }
}
