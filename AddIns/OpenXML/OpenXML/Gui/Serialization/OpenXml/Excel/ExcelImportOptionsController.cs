#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Serialization.Ascii;
using Altaxo.Serialization.OpenXml.Excel;

namespace Altaxo.Gui.Serialization.OpenXml.Excel
{
  public interface IExcelImportOptionsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IExcelImportOptionsView))]
  [UserControllerForObject(typeof(ExcelImportOptions))]
  public class ExcelImportOptionsController : MVCANControllerEditImmutableDocBase<ExcelImportOptions, IExcelImportOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _useNeutralColumnName;

    public bool UseNeutralColumnName
    {
      get => _useNeutralColumnName;
      set
      {
        if (!(_useNeutralColumnName == value))
        {
          _useNeutralColumnName = value;
          OnPropertyChanged(nameof(UseNeutralColumnName));
        }
      }
    }

    private string _neutralColumnName;

    public string NeutralColumnName
    {
      get => _neutralColumnName;
      set
      {
        if (!(_neutralColumnName == value))
        {
          _neutralColumnName = value;
          OnPropertyChanged(nameof(NeutralColumnName));
        }
      }
    }

    private bool _includeFilePathAsProperty;

    public bool IncludeFilePathAsProperty
    {
      get => _includeFilePathAsProperty;
      set
      {
        if (!(_includeFilePathAsProperty == value))
        {
          _includeFilePathAsProperty = value;
          OnPropertyChanged(nameof(IncludeFilePathAsProperty));
        }
      }
    }

    private bool _includeSheetNameAsProperty;

    public bool IncludeSheetNameAsProperty
    {
      get => _includeSheetNameAsProperty;
      set
      {
        if (!(_includeSheetNameAsProperty == value))
        {
          _includeSheetNameAsProperty = value;
          OnPropertyChanged(nameof(IncludeSheetNameAsProperty));
        }
      }
    }

    private ItemsController<AsciiHeaderLinesDestination> _headerLinesDestination;

    public ItemsController<AsciiHeaderLinesDestination> HeaderLinesDestination
    {
      get => _headerLinesDestination;
      set
      {
        if (!(_headerLinesDestination == value))
        {
          _headerLinesDestination?.Dispose();
          _headerLinesDestination = value;
          OnPropertyChanged(nameof(HeaderLinesDestination));
        }
      }
    }

    public class IndexClass
    {
      public int Index { get; set; }


    }

    private ObservableCollection<IndexClass> _indicesOfImportedSheets;

    public ObservableCollection<IndexClass> IndicesOfImportedSheets
    {
      get => _indicesOfImportedSheets;
      set
      {
        if (!(_indicesOfImportedSheets == value))
        {
          _indicesOfImportedSheets = value;
          OnPropertyChanged(nameof(IndicesOfImportedSheets));
        }
      }
    }

    private bool _knownNumberOfMainHeaderLines;

    public bool KnownNumberOfMainHeaderLines
    {
      get => _knownNumberOfMainHeaderLines;
      set
      {
        if (!(_knownNumberOfMainHeaderLines == value))
        {
          _knownNumberOfMainHeaderLines = value;
          OnPropertyChanged(nameof(KnownNumberOfMainHeaderLines));
        }
      }
    }


    private int _numberOfMainHeaderLines;

    public int NumberOfMainHeaderLines
    {
      get => _numberOfMainHeaderLines;
      set
      {
        if (!(_numberOfMainHeaderLines == value))
        {
          _numberOfMainHeaderLines = value;
          OnPropertyChanged(nameof(NumberOfMainHeaderLines));
        }
      }
    }

    private bool _knownIndexOfCaptionLine;

    public bool KnownIndexOfCaptionLine
    {
      get => _knownIndexOfCaptionLine;
      set
      {
        if (!(_knownIndexOfCaptionLine == value))
        {
          _knownIndexOfCaptionLine = value;
          OnPropertyChanged(nameof(KnownIndexOfCaptionLine));
        }
      }
    }


    private int _indexOfCaptionLine;

    public int IndexOfCaptionLine
    {
      get => _indexOfCaptionLine;
      set
      {
        if (!(_indexOfCaptionLine == value))
        {
          _indexOfCaptionLine = value;
          OnPropertyChanged(nameof(IndexOfCaptionLine));
        }
      }
    }

    private bool _tableStructureIsKnown;

    public bool TableStructureIsKnown
    {
      get => _tableStructureIsKnown;
      set
      {
        if (!(_tableStructureIsKnown == value))
        {
          _tableStructureIsKnown = value;
          OnPropertyChanged(nameof(TableStructureIsKnown));
        }
      }
    }

    private ObservableCollection<Boxed<AsciiColumnType>> _tableStructure;

    public ObservableCollection<Boxed<AsciiColumnType>> TableStructure
    {
      get => _tableStructure;
      set
      {
        if (!(_tableStructure == value))
        {
          _tableStructure = value;
          OnPropertyChanged(nameof(TableStructure));
        }
      }
    }

    private ObservableCollection<AsciiColumnType> _tableStructureItems;

    public ObservableCollection<AsciiColumnType> TableStructureItems
    {
      get => _tableStructureItems;
      set
      {
        if (!(_tableStructureItems == value))
        {
          _tableStructureItems = value;
          OnPropertyChanged(nameof(TableStructureItems));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        UseNeutralColumnName = _doc.UseNeutralColumnName;
        NeutralColumnName = _doc.NeutralColumnName;
        IncludeFilePathAsProperty = _doc.IncludeFilePathAsProperty;
        IncludeSheetNameAsProperty = _doc.IncludeSheetNameAsProperty;
        HeaderLinesDestination = new ItemsController<AsciiHeaderLinesDestination>(new Collections.SelectableListNodeList(_doc.HeaderLinesDestination));
        IndicesOfImportedSheets = new ObservableCollection<IndexClass>(_doc.IndicesOfImportedSheets.Select(x => new IndexClass { Index = x }));
        KnownNumberOfMainHeaderLines = _doc.NumberOfMainHeaderLines.HasValue;
        NumberOfMainHeaderLines = _doc.NumberOfMainHeaderLines ?? 0;
        KnownIndexOfCaptionLine = _doc.IndexOfCaptionLine.HasValue;
        IndexOfCaptionLine = _doc.IndexOfCaptionLine ?? 0;
        TableStructureItems = new ObservableCollection<AsciiColumnType>(System.Enum.GetValues(typeof(AsciiColumnType)).Cast<AsciiColumnType>());
        TableStructureIsKnown = _doc.RecognizedStructure is not null;
        if (_doc.RecognizedStructure is not null)
        {
          TableStructure = new ObservableCollection<Boxed<AsciiColumnType>>(_doc.RecognizedStructure.Columns.Select(ct => new Boxed<AsciiColumnType>(ct.ColumnType)));
        }
        else
        {
          TableStructure = new ObservableCollection<Boxed<AsciiColumnType>>();
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        UseNeutralColumnName = UseNeutralColumnName,
        NeutralColumnName = string.IsNullOrEmpty(NeutralColumnName) ? "Y" : NeutralColumnName,
        IncludeFilePathAsProperty = IncludeFilePathAsProperty,
        IncludeSheetNameAsProperty = IncludeSheetNameAsProperty,
        HeaderLinesDestination = HeaderLinesDestination.SelectedValue,
        IndicesOfImportedSheets = IndicesOfImportedSheets.Select(x => x.Index).ToImmutableList(),
        NumberOfMainHeaderLines = KnownNumberOfMainHeaderLines ? NumberOfMainHeaderLines : null,
        IndexOfCaptionLine = KnownIndexOfCaptionLine && NumberOfMainHeaderLines > 0 ? IndexOfCaptionLine : null,
        RecognizedStructure = TableStructureIsKnown ? new AsciiLineComposition(TableStructure.Select(x => x.Value), TableStructure.Count) : null,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
