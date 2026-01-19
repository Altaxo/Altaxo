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
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Altaxo.Calc.Regression.Multivariate;
using Altaxo.Data;

namespace Altaxo.Gui.Analysis.Multivariate
{
  public interface IDimensionReductionByAggregationView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="DimensionReductionByAggregation"/>
  /// </summary>
  [ExpectedTypeOfView(typeof(IDimensionReductionByAggregationView))]
  [UserControllerForObject(typeof(DimensionReductionByAggregation))]

  public class DimensionReductionByAggregationController : MVCANControllerEditImmutableDocBase<DimensionReductionByAggregation, IDimensionReductionByAggregationView>
  {

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings


    /// <summary>
    /// Gets or sets the aggregation kinds that can be applied.
    /// </summary>
    public ObservableCollection<NotifyChangedValue<KindOfAggregation>> AggregationKinds
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(AggregationKinds));
        }
      }
    }

    /// <summary>
    /// Gets the list of available aggregation kinds.
    /// </summary>
    public static KindOfAggregation[] AvailableAggregationKinds => field ??= (KindOfAggregation[])System.Enum.GetValues(typeof(KindOfAggregation));


    #endregion Bindings



    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        AggregationKinds = new ObservableCollection<NotifyChangedValue<KindOfAggregation>>(_doc.AggregationKinds.Select(x => new NotifyChangedValue<KindOfAggregation>(x)));
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        AggregationKinds = AggregationKinds.Select(x => x.Value).Distinct().ToImmutableList(),
      };

      return ApplyEnd(true, disposeController);
    }
  }
}
