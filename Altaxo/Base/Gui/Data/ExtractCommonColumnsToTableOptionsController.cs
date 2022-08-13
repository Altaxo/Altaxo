using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Data;

namespace Altaxo.Gui.Data
{
  public interface IExtractCommonColumnsToTableOptionsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IExtractCommonColumnsToTableOptionsView))]
  [UserControllerForObject(typeof(ExtractCommonColumnsToTableOptions))]
  public class ExtractCommonColumnsToTableOptionsController : MVCANControllerEditImmutableDocBase<ExtractCommonColumnsToTableOptions, IExtractCommonColumnsToTableOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
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
      };

      return ApplyEnd(true, disposeController);
    }

   
  }

}
