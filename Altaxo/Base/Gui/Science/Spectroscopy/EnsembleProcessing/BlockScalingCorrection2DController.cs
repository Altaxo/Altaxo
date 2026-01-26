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

using System.Collections.Generic;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;

namespace Altaxo.Gui.Science.Spectroscopy.EnsembleProcessing
{
  public interface IBlockScalingCorrection2DView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(BlockScalingCorrection2D))]
  [ExpectedTypeOfView(typeof(IBlockScalingCorrection2DView))]
  public class BlockScalingCorrection2DController : MVCANControllerEditImmutableDocBase<BlockScalingCorrection2D, IBlockScalingCorrection2DView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings


    public int SizeOfDimension0
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(SizeOfDimension0));
        }
      }
    }


    public int SizeOfDimension1
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(SizeOfDimension1));
        }
      }
    }

    public bool SizeOfDimension1IsKnown
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(SizeOfDimension1IsKnown));
        }
      }
    }

    public bool DimensionToAverageIsZero
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(DimensionToAverageIsZero));
          OnPropertyChanged(nameof(DimensionToAverageIsOne));
        }
      }
    }


    public bool DimensionToAverageIsOne
    {
      get => !DimensionToAverageIsZero;
      set
      {
        DimensionToAverageIsZero = !value;
      }
    }


    public double MinimumX
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(MinimumX));
        }
      }
    }


    public double MaximumX
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(MaximumX));
        }
      }
    }


    public bool XIsInSpectralUnits
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(XIsInSpectralUnits));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        SizeOfDimension0 = _doc.SizeOfDimension0;
        SizeOfDimension1IsKnown = _doc.SizeOfDimension1.HasValue;
        SizeOfDimension1 = _doc.SizeOfDimension1 ?? 1;
        DimensionToAverageIsZero = _doc.IndexOfDimensionToAverage == 0;
        MinimumX = _doc.MinimumX;
        MaximumX = _doc.MaximumX;
        XIsInSpectralUnits = _doc.XIsInSpectralUnits;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        SizeOfDimension0 = SizeOfDimension0,
        SizeOfDimension1 = SizeOfDimension1IsKnown ? SizeOfDimension1 : null,
        IndexOfDimensionToAverage = DimensionToAverageIsZero ? 0 : 1,
        MinimumX = MinimumX,
        MaximumX = MaximumX,
        XIsInSpectralUnits = XIsInSpectralUnits,
      };

      return ApplyEnd(true, disposeController);
    }
  }
}
