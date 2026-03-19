#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using Altaxo.Units;

namespace Altaxo.Gui.Science.Spectroscopy.SpikeRemoval
{
  public interface ISpikeRemovalByEnsembleStatisticsView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(SpikeRemovalByEnsembleStatistics))]
  [ExpectedTypeOfView(typeof(ISpikeRemovalByEnsembleStatisticsView))]
  public class SpikeRemovalByEnsembleStatisticsController : MVCANControllerEditImmutableDocBase<SpikeRemovalByEnsembleStatistics, ISpikeRemovalByEnsembleStatisticsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings


    public double MaximalWidth
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(MaximalWidth));
        }
      }
    }




    public bool EliminateNegativeSpikes
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(EliminateNegativeSpikes));
        }
      }
    }


    public double NumberOfSigmas
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(NumberOfSigmas));
        }
      }
    }



    public DimensionfulQuantity MinimalSignalToNoiseRatio
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(MinimalSignalToNoiseRatio));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment UnitEnvironmentRelation => RelationEnvironment.Instance;




    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        MaximalWidth = _doc.MaximalWidth;
        EliminateNegativeSpikes = _doc.EliminateNegativeSpikes;
        MinimalSignalToNoiseRatio = new DimensionfulQuantity(_doc.MinimalSignalToNoiseRatio, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(UnitEnvironmentRelation.DefaultUnit);
        NumberOfSigmas = _doc.NumberOfSigmas;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        MaximalWidth = MaximalWidth,
        EliminateNegativeSpikes = EliminateNegativeSpikes,
        MinimalSignalToNoiseRatio = MinimalSignalToNoiseRatio.AsValueInSIUnits,
        NumberOfSigmas = NumberOfSigmas,
      };

      return ApplyEnd(true, disposeController);
    }


  }
}
