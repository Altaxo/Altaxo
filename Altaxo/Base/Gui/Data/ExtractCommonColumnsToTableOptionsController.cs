using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Altaxo.Calc.Interpolation;
using Altaxo.Data;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Data
{
  /// <summary>
  /// View contract for editing options when extracting common columns into a table.
  /// </summary>
  public interface IExtractCommonColumnsToTableOptionsView : IDataContextAwareView { }

  /// <summary>
  /// Controller for editing options when extracting common columns into a table.
  /// </summary>
  [ExpectedTypeOfView(typeof(IExtractCommonColumnsToTableOptionsView))]
  [UserControllerForObject(typeof(ExtractCommonColumnsToTableOptions))]
  public class ExtractCommonColumnsToTableOptionsController : MVCANControllerEditImmutableDocBase<ExtractCommonColumnsToTableOptions, IExtractCommonColumnsToTableOptionsView>
  {
    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(InterpolationDetails, () => InterpolationDetails = null!);
    }

    #region Bindings

    /// <summary>
    /// Gets or sets a value indicating whether X values should be intersected.
    /// </summary>
    public bool IntersectXValues
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IntersectXValues));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether a user-defined name should be used for the X column.
    /// </summary>
    public bool UseUserDefinedNameForXColumn
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(UseUserDefinedNameForXColumn));
        }
      }
    }

    /// <summary>
    /// Gets or sets the user-defined name for the X column.
    /// </summary>
    public string UserDefinedNameForXColumn
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(UserDefinedNameForXColumn));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether user-defined names should be used for the Y columns.
    /// </summary>
    public bool UseUserDefinedNamesForYColumns
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(UseUserDefinedNamesForYColumns));
        }
      }
    }

    /// <summary>
    /// Gets or sets the user-defined names for the Y columns.
    /// </summary>
    public string UserDefinedNamesForYColumns
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(UserDefinedNamesForYColumns));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether multiple Y columns should be placed adjacently in the destination table.
    /// </summary>
    public bool PlaceMultipleYColumnsAdjacentInDestinationTable
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(PlaceMultipleYColumnsAdjacentInDestinationTable));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether a property column with the source table name should be created.
    /// </summary>
    public bool CreatePropertyColumnWithSourceTableName
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(CreatePropertyColumnWithSourceTableName));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether column properties should be copied to the destination table.
    /// </summary>
    public bool CopyColumnProperties
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(CopyColumnProperties));
        }
      }
    }

    /// <summary>
    /// Contains the names of properties of the source table that should be copied to the destination table.
    /// </summary>
    public string TablePropertyNames
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(TablePropertyNames));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether resampling should be used.
    /// </summary>
    public bool UseResampling
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(UseResampling));
        }
      }
    }

    /// <summary>
    /// Gets or sets the interpolation function controller item list.
    /// </summary>
    public ItemsController<Type> InterpolationFunction
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(InterpolationFunction));
        }
      }
    }


    /// <summary>
    /// Gets or sets the interpolation interval.
    /// </summary>
    public double InterpolationInterval
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(InterpolationInterval));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether a user-defined interpolation range start should be used.
    /// </summary>
    public bool UseUserDefinedInterpolationRangeStart
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(UseUserDefinedInterpolationRangeStart));
        }
      }
    }

    /// <summary>
    /// Gets or sets the user-defined interpolation range start.
    /// </summary>
    public double? InterpolationRangeStart
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(InterpolationRangeStart));
        }
      }
    }



    /// <summary>
    /// Gets or sets a value indicating whether a user-defined interpolation range end should be used.
    /// </summary>
    public bool UseUserDefinedInterpolationRangeEnd
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(UseUserDefinedInterpolationRangeEnd));
        }
      }
    }

    /// <summary>
    /// Gets or sets the user-defined interpolation range end.
    /// </summary>
    public double? InterpolationRangeEnd
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(InterpolationRangeEnd));
        }
      }
    }

    /// <summary>
    /// Gets or sets the controller for editing interpolation-specific options.
    /// </summary>
    public IMVCANController InterpolationDetails
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(InterpolationDetails));
        }
      }
    }

    private IInterpolationFunctionOptions? _lastInterpolationFunction;

    #endregion

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        IntersectXValues = _doc.IntersectXValues;
        UseUserDefinedNameForXColumn = !string.IsNullOrEmpty(_doc.UserDefinedNameForXColumn);
        UserDefinedNameForXColumn = _doc.UserDefinedNameForXColumn;
        UseUserDefinedNamesForYColumns = _doc.UserDefinedNamesForYColumns.Length > 0;
        UserDefinedNamesForYColumns = string.Join("\r\n", _doc.UserDefinedNamesForYColumns);
        PlaceMultipleYColumnsAdjacentInDestinationTable = _doc.PlaceMultipleYColumnsAdjacentInDestinationTable;
        CreatePropertyColumnWithSourceTableName = _doc.CreatePropertyColumnWithSourceTableName;
        CopyColumnProperties = _doc.CopyColumnProperties;
        TablePropertyNames = string.Join("\r\n", _doc.TablePropertyNames);
        UseResampling = _doc.UseResampling;


        InterpolationInterval = _doc.InterpolationInterval;

        UseUserDefinedInterpolationRangeStart = _doc.UserSpecifiedInterpolationStart.HasValue;
        InterpolationRangeStart = _doc.UserSpecifiedInterpolationStart;
        UseUserDefinedInterpolationRangeEnd = _doc.UserSpecifiedInterpolationEnd.HasValue;
        InterpolationRangeEnd = _doc.UserSpecifiedInterpolationEnd;


        InterpolationFunction = new ItemsController<Type>(
          new Collections.SelectableListNodeList(
            Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IInterpolationFunctionOptions))
            .Select(t => new Collections.SelectableListNode(TrimOptionsFromName(t.Name), t, false))
          ),
          EhInterpolationFunctionChanged
          );
        if (_doc.Interpolation is not null)
          InterpolationFunction.SelectedValue = _doc.Interpolation.GetType();
        else
          InterpolationFunction.SelectedValue = (Type)InterpolationFunction.Items[0].Tag;
      }
    }

    /// <summary>
    /// Trims a trailing `Options` suffix from a type name.
    /// </summary>
    /// <param name="name">The name to trim.</param>
    /// <returns>The trimmed name.</returns>
    static string TrimOptionsFromName(string name)
    {
      if (name.EndsWith("Options"))
        return name.Substring(0, name.Length - 7);
      else
        return name;
    }


    private void EhInterpolationFunctionChanged(Type type)
    {
      if (type is not null && type != _lastInterpolationFunction?.GetType())
      {
        _lastInterpolationFunction = (IInterpolationFunctionOptions)Activator.CreateInstance(type);

        if (_lastInterpolationFunction is not null)
        {
          InterpolationDetails = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _lastInterpolationFunction }, typeof(IMVCANController));
        }
      }
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      var userDefinedY = UserDefinedNamesForYColumns.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                        .Where(s => !string.IsNullOrEmpty(s.Trim()))
                        .ToImmutableArray();


      _doc = _doc with
      {
        IntersectXValues = IntersectXValues,
        UserDefinedNameForXColumn = UseUserDefinedNameForXColumn ? UserDefinedNameForXColumn.Trim() : String.Empty,
        UserDefinedNamesForYColumns = UseUserDefinedNamesForYColumns ? userDefinedY : ImmutableArray<string>.Empty,
        PlaceMultipleYColumnsAdjacentInDestinationTable = PlaceMultipleYColumnsAdjacentInDestinationTable,
        CreatePropertyColumnWithSourceTableName = CreatePropertyColumnWithSourceTableName,
        CopyColumnProperties = CopyColumnProperties,
        TablePropertyNames = TablePropertyNames.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                             .Where(s => !string.IsNullOrEmpty(s.Trim()))
                             .ToImmutableArray(),
        Interpolation = null,
      };

      if (UseResampling)
      {
        if (InterpolationDetails is not null)
        {
          if (false == InterpolationDetails.Apply(disposeController))
            return ApplyEnd(false, disposeController);

          _lastInterpolationFunction = (IInterpolationFunctionOptions)InterpolationDetails.ModelObject;
        }


        _doc = _doc with
        {
          Interpolation = _lastInterpolationFunction,
          InterpolationInterval = InterpolationInterval,
          UserSpecifiedInterpolationStart = InterpolationRangeStart,
          UserSpecifiedInterpolationEnd = InterpolationRangeEnd,
        };
      }

      return ApplyEnd(true, disposeController);
    }


  }

}
