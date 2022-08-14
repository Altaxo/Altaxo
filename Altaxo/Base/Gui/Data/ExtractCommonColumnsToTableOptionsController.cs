using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.Interpolation;
using Altaxo.Data;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Data
{
  public interface IExtractCommonColumnsToTableOptionsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IExtractCommonColumnsToTableOptionsView))]
  [UserControllerForObject(typeof(ExtractCommonColumnsToTableOptions))]
  public class ExtractCommonColumnsToTableOptionsController : MVCANControllerEditImmutableDocBase<ExtractCommonColumnsToTableOptions, IExtractCommonColumnsToTableOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_interpolationDetails, () => InterpolationDetails = null);
    }

    #region Bindings

    private bool _intersectXValues;

    public bool IntersectXValues
    {
      get => _intersectXValues;
      set
      {
        if (!(_intersectXValues == value))
        {
          _intersectXValues = value;
          OnPropertyChanged(nameof(IntersectXValues));
        }
      }
    }

    private bool _useUserDefinedNameForXColumn;

    public bool UseUserDefinedNameForXColumn
    {
      get => _useUserDefinedNameForXColumn;
      set
      {
        if (!(_useUserDefinedNameForXColumn == value))
        {
          _useUserDefinedNameForXColumn = value;
          OnPropertyChanged(nameof(UseUserDefinedNameForXColumn));
        }
      }
    }

    private string _userDefinedNameForXColumn;

    public string UserDefinedNameForXColumn
    {
      get => _userDefinedNameForXColumn;
      set
      {
        if (!(_userDefinedNameForXColumn == value))
        {
          _userDefinedNameForXColumn = value;
          OnPropertyChanged(nameof(UserDefinedNameForXColumn));
        }
      }
    }

    private bool _useUserDefinedNamesForYColumns;

    public bool UseUserDefinedNamesForYColumns
    {
      get => _useUserDefinedNamesForYColumns;
      set
      {
        if (!(_useUserDefinedNamesForYColumns == value))
        {
          _useUserDefinedNamesForYColumns = value;
          OnPropertyChanged(nameof(UseUserDefinedNamesForYColumns));
        }
      }
    }

    private string _userDefinedNamesForYColumns;

    public string UserDefinedNamesForYColumns
    {
      get => _userDefinedNamesForYColumns;
      set
      {
        if (!(_userDefinedNamesForYColumns == value))
        {
          _userDefinedNamesForYColumns = value;
          OnPropertyChanged(nameof(UserDefinedNamesForYColumns));
        }
      }
    }

    private bool _placeMultipleYColumnsAdjacentInDestinationTable;

    public bool PlaceMultipleYColumnsAdjacentInDestinationTable
    {
      get => _placeMultipleYColumnsAdjacentInDestinationTable;
      set
      {
        if (!(_placeMultipleYColumnsAdjacentInDestinationTable == value))
        {
          _placeMultipleYColumnsAdjacentInDestinationTable = value;
          OnPropertyChanged(nameof(PlaceMultipleYColumnsAdjacentInDestinationTable));
        }
      }
    }

    private bool _createPropertyColumnWithSourceTableName;

    public bool CreatePropertyColumnWithSourceTableName
    {
      get => _createPropertyColumnWithSourceTableName;
      set
      {
        if (!(_createPropertyColumnWithSourceTableName == value))
        {
          _createPropertyColumnWithSourceTableName = value;
          OnPropertyChanged(nameof(CreatePropertyColumnWithSourceTableName));
        }
      }
    }

    private bool _copyColumnProperties;

    public bool CopyColumnProperties
    {
      get => _copyColumnProperties;
      set
      {
        if (!(_copyColumnProperties == value))
        {
          _copyColumnProperties = value;
          OnPropertyChanged(nameof(CopyColumnProperties));
        }
      }
    }

    private bool _useResampling;

    public bool UseResampling
    {
      get => _useResampling;
      set
      {
        if (!(_useResampling == value))
        {
          _useResampling = value;
          OnPropertyChanged(nameof(UseResampling));
        }
      }
    }

    private ItemsController<Type> _interpolationFunction;

    public ItemsController<Type> InterpolationFunction
    {
      get => _interpolationFunction;
      set
      {
        if (!(_interpolationFunction == value))
        {
          _interpolationFunction = value;
          OnPropertyChanged(nameof(InterpolationFunction));
        }
      }
    }

    private double _interpolationInterval;

    public double InterpolationInterval
    {
      get => _interpolationInterval;
      set
      {
        if (!(_interpolationInterval == value))
        {
          _interpolationInterval = value;
          OnPropertyChanged(nameof(InterpolationInterval));
        }
      }
    }

    private bool _useUserDefinedInterpolationRangeStart;

    public bool UseUserDefinedInterpolationRangeStart
    {
      get => _useUserDefinedInterpolationRangeStart;
      set
      {
        if (!(_useUserDefinedInterpolationRangeStart == value))
        {
          _useUserDefinedInterpolationRangeStart = value;
          OnPropertyChanged(nameof(UseUserDefinedInterpolationRangeStart));
        }
      }
    }

    private double? _interpolationRangeStart;

    public double? InterpolationRangeStart
    {
      get => _interpolationRangeStart;
      set
      {
        if (!(_interpolationRangeStart == value))
        {
          _interpolationRangeStart = value;
          OnPropertyChanged(nameof(InterpolationRangeStart));
        }
      }
    }


    private bool _useUserDefinedInterpolationRangeEnd;

    public bool UseUserDefinedInterpolationRangeEnd
    {
      get => _useUserDefinedInterpolationRangeEnd;
      set
      {
        if (!(_useUserDefinedInterpolationRangeEnd == value))
        {
          _useUserDefinedInterpolationRangeEnd = value;
          OnPropertyChanged(nameof(UseUserDefinedInterpolationRangeEnd));
        }
      }
    }

    private double?  _interpolationRangeEnd;

    public double?  InterpolationRangeEnd
    {
      get => _interpolationRangeEnd;
      set
      {
        if (!(_interpolationRangeEnd == value))
        {
          _interpolationRangeEnd = value;
          OnPropertyChanged(nameof(InterpolationRangeEnd));
        }
      }
    }

    private IMVCANController _interpolationDetails;

    public IMVCANController InterpolationDetails
    {
      get => _interpolationDetails;
      set
      {
        if (!(_interpolationDetails == value))
        {
          _interpolationDetails?.Dispose();
          _interpolationDetails = value;
          OnPropertyChanged(nameof(InterpolationDetails));
        }
      }
    }

    private IInterpolationFunctionOptions? _lastInterpolationFunction;

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        IntersectXValues = _doc.IntersectXValues;
        UseUserDefinedNameForXColumn = !string.IsNullOrEmpty(_doc.UserDefinedNameForXColumn);
        UserDefinedNameForXColumn = _doc.UserDefinedNameForXColumn;
        UseUserDefinedNamesForYColumns = _doc.UserDefinedNamesForYColumns.Length > 0;
        UserDefinedNamesForYColumns = string.Join("\r\n", _doc.UserDefinedNamesForYColumns);
        PlaceMultipleYColumnsAdjacentInDestinationTable = _doc.PlaceMultipleYColumnsAdjacentInDestinationTable;
        CreatePropertyColumnWithSourceTableName = _doc.CreatePropertyColumnWithSourceTableName;
        CopyColumnProperties =  _doc.CopyColumnProperties;
        UseResampling = _doc.UseResampling;


        InterpolationInterval = _doc.InterpolationInterval;

        UseUserDefinedInterpolationRangeStart = _doc.UserSpecifiedInterpolationStart.HasValue;
        InterpolationRangeStart = _doc.UserSpecifiedInterpolationStart;
        UseUserDefinedInterpolationRangeEnd = _doc.UserSpecifiedInterpolationEnd.HasValue;
        InterpolationRangeEnd = _doc.UserSpecifiedInterpolationEnd;


        InterpolationFunction = new ItemsController<Type>(
          new Collections.SelectableListNodeList(
            Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IInterpolationFunctionOptions))
            .Select(t => new Collections.SelectableListNode(t.Name, t, false))
          ),
          EhInterpolationFunctionChanged
          );
        if (_doc.Interpolation is not null)
          InterpolationFunction.SelectedValue = _doc.Interpolation.GetType();
        else
          InterpolationFunction.SelectedValue = (Type)InterpolationFunction.Items[0].Tag;
      }
    }

    private void EhInterpolationFunctionChanged(Type type)
    {
      if(type is not null && type != _lastInterpolationFunction?.GetType())
      {
        _lastInterpolationFunction = (IInterpolationFunctionOptions)Activator.CreateInstance(type);

        if (_lastInterpolationFunction is not null)
        {
          InterpolationDetails = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _lastInterpolationFunction }, typeof(IMVCANController));
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      var userDefinedY = UserDefinedNamesForYColumns.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
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
        Interpolation = null,
      };

      if(UseResampling)
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
