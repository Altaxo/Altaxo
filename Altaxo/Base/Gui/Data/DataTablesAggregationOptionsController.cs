#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using Altaxo.Data;

namespace Altaxo.Gui.Data
{
  /// <summary>
  /// Represents the view for editing <see cref="DataTablesAggregationOptions"/>.
  /// </summary>
  public interface IDataTablesAggregationOptionsView : IDataContextAwareView { }

  /// <summary>
  /// Controller for editing <see cref="DataTablesAggregationOptions"/> in a GUI.
  /// </summary>
  [ExpectedTypeOfView(typeof(IDataTablesAggregationOptionsView))]
  [UserControllerForObject(typeof(DataTablesAggregationOptions))]
  public class DataTablesAggregationOptionsController : MVCANControllerEditImmutableDocBase<DataTablesAggregationOptions, IDataTablesAggregationOptionsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Binding


    /// <summary>
    /// Gets or sets the collection of clustered property names.
    /// </summary>
    public ObservableCollection<NotifyChangedValue<string>> ClusteredPropertyNames
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(ClusteredPropertyNames));
        }
      }
    }


    /// <summary>
    /// Gets or sets the collection of column names that should be aggregated.
    /// </summary>
    public ObservableCollection<NotifyChangedValue<string>> AggregatedColumnNames
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(AggregatedColumnNames));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the aggregated column names are treated as property names instead of data column names.
    /// </summary>
    public bool AggregatedColumnNamesArePropertyNames
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(AggregatedColumnNamesArePropertyNames));
        }
      }
    }



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

    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        ClusteredPropertyNames = new ObservableCollection<NotifyChangedValue<string>>(_doc.ClusteredPropertiesNames.Select(e => new NotifyChangedValue<string>(e)));
        AggregatedColumnNames = new ObservableCollection<NotifyChangedValue<string>>(_doc.AggregatedColumnNames.Select(e => new NotifyChangedValue<string>(e)));
        AggregatedColumnNamesArePropertyNames = _doc.AggregatedColumnNamesArePropertyNames;
        AggregationKinds = new ObservableCollection<NotifyChangedValue<KindOfAggregation>>(_doc.AggregationKinds.Select(x => new NotifyChangedValue<KindOfAggregation>(x)));
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        ClusteredPropertiesNames = ClusteredPropertyNames.Select(e => e.Value).Where(s => !string.IsNullOrEmpty(s)).ToImmutableList(),
        AggregatedColumnNames = AggregatedColumnNames.Select(e => e.Value).Where(s => !string.IsNullOrEmpty(s)).ToImmutableList(),
        AggregatedColumnNamesArePropertyNames = AggregatedColumnNamesArePropertyNames,
        AggregationKinds = AggregationKinds.Select(x => x.Value).Distinct().ToImmutableList(),
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
