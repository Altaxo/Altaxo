#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Serialization.Ascii;

namespace Altaxo.Gui.Serialization.Ascii
{
  /// <summary>
  /// Provides the view contract for <see cref="AsciiImportOptionsController"/>.
  /// </summary>
  public interface IAsciiImportOptionsView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Supports getting a stream of analysis data. Any function using the <see cref="AsciiImportOptionsController"/> can create an object that implements
  /// this interface and provide this object as second argument to the <c>InitializeDocument</c> function of the controller.
  /// </summary>
  public interface IAsciiImportOptionsAnalysisDataProvider
  {
    /// <summary>
    /// Gets a stream for analysis of the ASCII file. If the stream could not be opened (file unavailable), this function returns <see langword="null"/> without throwing an exception.
    /// </summary>
    /// <returns>The stream for analysis, or <see langword="null"/>.</returns>
    public System.IO.Stream GetStreamForAnalysis();
  }

  /// <summary>
  /// Controller for <see cref="AsciiImportOptions"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IAsciiImportOptionsView))]
  [UserControllerForObject(typeof(AsciiImportOptions))]
  public class AsciiImportOptionsController : MVCANControllerEditImmutableDocBase<AsciiImportOptions, IAsciiImportOptionsView>
  {
    private System.IO.Stream _asciiStreamData;
    private IAsciiImportOptionsAnalysisDataProvider _asciiStreamDataProvider;
    private Dictionary<Type, IAsciiSeparationStrategy> _separationStrategyInstances = new Dictionary<Type, IAsciiSeparationStrategy>();

    /// <summary>
    /// Temporary storage for the analysis options. This is needed because the analysis options may be passed to this controller.
    /// </summary>
    private AsciiDocumentAnalysisOptions _analysisOptions;

    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(SeparationStrategyInstanceController, () => SeparationStrategyInstanceController = null);
      yield return new ControllerAndSetNullMethod(AsciiDocumentAnalysisOptionsController, () => AsciiDocumentAnalysisOptionsController = null);
    }

    #region Bindings


    /// <summary>
    /// Gets the command do analyze. Occurs when analysis of the ASCII input is requested.
    /// </summary>
    public ICommand CmdDoAnalyze { get => field ??= new RelayCommand(EhCmdDoAsciiAnalysis); }

    /// <summary>
    /// Gets or sets a value indicating whether the number of main header lines is known for the ASCII import.
    /// </summary>
    public bool IsNumberOfMainHeaderLinesKnown
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IsNumberOfMainHeaderLinesKnown));
        }
      }
    }


    /// <summary>
    /// Gets or sets the number of main header lines for the ASCII import.
    /// </summary>
    public int NumberOfMainHeaderLines
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(NumberOfMainHeaderLines));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the index of the caption line is known.
    /// </summary>
    public bool IsIndexOfCaptionLineKnown
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IsIndexOfCaptionLineKnown));
        }
      }
    }


    /// <summary>
    /// Gets or sets the index of the caption line within the input.
    /// </summary>
    public int IndexOfCaptionLine
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IndexOfCaptionLine));
        }
      }
    }


    /// <summary>
    /// Gets or sets the controller providing available separation strategies.
    /// </summary>
    public ItemsController<Type> SeparationStrategy
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(SeparationStrategy));
        }
      }
    }



    /// <summary>
    /// Gets or sets a value indicating whether the separation strategy is known.
    /// </summary>
    public bool IsSeparationStrategyKnown
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IsSeparationStrategyKnown));
        }
      }
    }


    /// <summary>
    /// Gets or sets the controller for selecting the number-format culture.
    /// </summary>
    public ItemsController<CultureInfo> NumberFormatCulture
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(NumberFormatCulture));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the number-format culture is known.
    /// </summary>
    public bool IsNumberFormatCultureKnown
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IsNumberFormatCultureKnown));
        }
      }
    }


    /// <summary>
    /// Gets or sets the controller for selecting the date/time-format culture.
    /// </summary>
    public ItemsController<CultureInfo> DateTimeFormatCulture
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(DateTimeFormatCulture));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the date/time-format culture is known.
    /// </summary>
    public bool IsDateTimeFormatCultureKnown
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IsDateTimeFormatCultureKnown));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the table structure has been detected and is known.
    /// </summary>
    public bool IsTableStructureKnown
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IsTableStructureKnown));
        }
      }
    }


    /// <summary>
    /// Gets or sets the detected table structure as a collection of column type descriptors.
    /// </summary>
    public ObservableCollection<Boxed<AsciiColumnType>> TableStructure
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(TableStructure));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether columns should be renamed using header names.
    /// </summary>
    public bool RenameColumnsWithHeaderNames
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(RenameColumnsWithHeaderNames));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the worksheet should be renamed from the file name.
    /// </summary>
    public bool RenameWorksheetWithFileName
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(RenameWorksheetWithFileName));
        }
      }
    }


    /// <summary>
    /// Gets or sets the controller for selecting destinations for header lines.
    /// </summary>
    public ItemsController<AsciiHeaderLinesDestination> HeaderLinesDestination
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(HeaderLinesDestination));
        }
      }
    }


    /// <summary>
    /// Gets or sets the controller that manages the separation-strategy detail view.
    /// </summary>
    public IMVCANController SeparationStrategyInstanceController
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(SeparationStrategyInstanceController));
        }
      }
    }


    /// <summary>
    /// Gets or sets the controller for the ASCII document analysis options view.
    /// </summary>
    public IMVCANController AsciiDocumentAnalysisOptionsController
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(AsciiDocumentAnalysisOptionsController));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether multiple ASCII streams are imported vertically.
    /// </summary>
    public bool ImportMultipleAsciiVertically
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(ImportMultipleAsciiVertically));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the encoding is detected from byte-order marks.
    /// </summary>
    public bool DetectEncodingFromByteOrderMarks
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(DetectEncodingFromByteOrderMarks));
        }
      }
    }

    /// <summary>
    /// Gets or sets the controller for selecting the code page used to decode the ASCII file.
    /// </summary>
    public ItemsController<int> CodePage
    {
      get => field;

      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(CodePage));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to reuse the column names when the destination table already exist.
    /// This property is ignored if the destination table is empty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the existing column names should be reused; otherwise, <c>false</c>.
    /// </value>
    public bool ReuseColumnNames
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(ReuseColumnNames));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to reuse column kinds and groups when the destination table already exists.
    /// This property is ignored if the destination table is empty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the existing column kinds and groups should be reused; otherwise, <c>false</c>.
    /// </value>
    public bool ReuseGroupNumbers
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(ReuseGroupNumbers));
        }
      }
    }




    #endregion

    /// <inheritdoc />
    public override bool InitializeDocument(params object[] args)
    {
      if (args is not null && args.Length >= 2 && args[1] is System.IO.Stream)
        _asciiStreamData = args[1] as System.IO.Stream;
      if (args is not null && args.Length >= 2 && args[1] is IAsciiImportOptionsAnalysisDataProvider)
        _asciiStreamDataProvider = args[1] as IAsciiImportOptionsAnalysisDataProvider;

      if (args is not null && args.Length >= 3 && args[2] is AsciiDocumentAnalysisOptions)
        _analysisOptions = (AsciiDocumentAnalysisOptions)args[2];
      else
        _analysisOptions = Current.PropertyService.GetValue(AsciiImporterImpl.PropertyKeyAsciiDocumentAnalysisOptions, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin);

      return base.InitializeDocument(args);
    }

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        DetectEncodingFromByteOrderMarks = _doc.DetectEncodingFromByteOrderMarks;


        var encodingsAvailable = new List<(int CodePage, string DisplayString)>();
        encodingsAvailable.Add((0, "Default (0)"));
        encodingsAvailable.AddRange(System.Text.Encoding.GetEncodings().Select(e => (e.CodePage, $"{e.DisplayName} ({e.CodePage})")));
        encodingsAvailable.Sort((x, y) => Comparer<int>.Default.Compare(x.CodePage, y.CodePage));
        var listEncodings = new SelectableListNodeList(encodingsAvailable.Select(e => new SelectableListNode(e.DisplayString, e.CodePage, e.CodePage == _doc.CodePage)));

        CodePage = new ItemsController<int>(listEncodings);
        CodePage.SelectedValue = _doc.CodePage;

        AsciiDocumentAnalysisOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _analysisOptions }, typeof(IMVCANController), UseDocument.Directly);

        _separationStrategyInstances.Clear();

        if (_doc.SeparationStrategy is not null)
          _separationStrategyInstances.Add(_doc.SeparationStrategy.GetType(), _doc.SeparationStrategy);

        IsSeparationStrategyKnown = _doc.SeparationStrategy is not null;
        var separationStrategyList = new SelectableListNodeList();
        var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Serialization.Ascii.IAsciiSeparationStrategy));
        foreach (var t in types)
          separationStrategyList.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), t, _doc.SeparationStrategy is null ? false : t == _doc.SeparationStrategy.GetType()));
        SeparationStrategy = new ItemsController<Type>(separationStrategyList, EhSeparationStrategyChanged);
        if (_doc.SeparationStrategy is not null)
        {
          SeparationStrategy.SelectedValue = _doc.SeparationStrategy.GetType();
          EhSeparationStrategyChanged(_doc.SeparationStrategy.GetType());
        }

        IsNumberFormatCultureKnown = _doc.NumberFormatCulture is not null;
        NumberFormatCulture = new ItemsController<CultureInfo>(GetAvailableCultures());
        NumberFormatCulture.SelectedValue = _doc.NumberFormatCulture;

        IsDateTimeFormatCultureKnown = _doc.DateTimeFormatCulture is not null;
        DateTimeFormatCulture = new ItemsController<CultureInfo>(GetAvailableCultures());
        DateTimeFormatCulture.SelectedValue = _doc.DateTimeFormatCulture;

        HeaderLinesDestination = new ItemsController<AsciiHeaderLinesDestination>(new SelectableListNodeList(_doc.HeaderLinesDestination));

        IsTableStructureKnown = _doc.RecognizedStructure is not null;
        if (_doc.RecognizedStructure is not null)
        {
          TableStructure = new((Boxed<AsciiColumnType>.ToBoxedItems(_doc.RecognizedStructure.Columns.Select(x => x.ColumnType))));
        }
        else
        {
          TableStructure = [];
        }


        IsNumberOfMainHeaderLinesKnown = _doc.NumberOfMainHeaderLines.HasValue;
        NumberOfMainHeaderLines = _doc.NumberOfMainHeaderLines ?? 0;

        IsIndexOfCaptionLineKnown = _doc.IndexOfCaptionLine.HasValue;
        IndexOfCaptionLine = _doc.IndexOfCaptionLine + 1 ?? 0; // we translate from zero-based index to user-friendly one-based index for the view

        RenameColumnsWithHeaderNames = _doc.RenameColumns;
        RenameWorksheetWithFileName = _doc.RenameWorksheet;


        HeaderLinesDestination = new ItemsController<AsciiHeaderLinesDestination>(new SelectableListNodeList(_doc.HeaderLinesDestination));

        ImportMultipleAsciiVertically = _doc.ImportMultipleStreamsVertically;

        ReuseColumnNames = _doc.ReuseColumnNames;
        ReuseGroupNumbers = _doc.ReuseGroupNumbers;
      }
    }



    private bool ApplyWithoutClosing()
    {
      IAsciiSeparationStrategy? newSeparationStrategy = null;
      if (IsSeparationStrategyKnown)
      {
        if (SeparationStrategyInstanceController is not null)
        {
          if (SeparationStrategyInstanceController.Apply(false))
            newSeparationStrategy = (IAsciiSeparationStrategy)SeparationStrategyInstanceController.ModelObject;
          else
            return false;
        }
        else
        {
          newSeparationStrategy = (IAsciiSeparationStrategy)Activator.CreateInstance(SeparationStrategy.SelectedValue);
        }
        _separationStrategyInstances[newSeparationStrategy.GetType()] = newSeparationStrategy;
      }

      AsciiLineComposition? recognizedStructure = null;
      if (IsTableStructureKnown && TableStructure is not null)
      {
        recognizedStructure = new AsciiLineComposition(TableStructure.Select(x => x.Value), TableStructure.Count);
      }

      _doc = _doc with
      {
        DetectEncodingFromByteOrderMarks = DetectEncodingFromByteOrderMarks,
        CodePage = CodePage.SelectedValue,
        NumberOfMainHeaderLines = IsNumberOfMainHeaderLinesKnown ? NumberOfMainHeaderLines : null,
        IndexOfCaptionLine = IsIndexOfCaptionLineKnown ? IndexOfCaptionLine - 1 : null, // translate back from user-friendly one-based index to zero-based index for the document
        RenameColumns = RenameColumnsWithHeaderNames,
        RenameWorksheet = RenameWorksheetWithFileName,
        ImportMultipleStreamsVertically = ImportMultipleAsciiVertically,
        NumberFormatCulture = IsNumberFormatCultureKnown ? NumberFormatCulture.SelectedValue : null,
        DateTimeFormatCulture = IsDateTimeFormatCultureKnown ? DateTimeFormatCulture.SelectedValue : null,
        SeparationStrategy = newSeparationStrategy,
        RecognizedStructure = IsTableStructureKnown ? (recognizedStructure.Count == 0 ? null : recognizedStructure) : null,
        HeaderLinesDestination = HeaderLinesDestination.SelectedValue,
        ReuseColumnNames = ReuseColumnNames,
        ReuseGroupNumbers = ReuseGroupNumbers,
      };

      return true;
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      if (!ApplyWithoutClosing())
        return false;

      if (!_doc.IsFullySpecified)
      {
        ReadAnalysisOptionsAndAnalyze();
      }

      if (!_doc.IsFullySpecified)
      {
        Current.Gui.InfoMessageBox("The analysis of the document was unable to determine some of the import options. You have to specify them manually.", "Attention");
        return false;
      }

      return ApplyEnd(true, disposeController);
    }

    private int CompareCultures(CultureInfo x, CultureInfo y)
    {
      return string.Compare(x.DisplayName, y.DisplayName);
    }

    private SelectableListNodeList GetAvailableCultures()
    {
      var list = new SelectableListNodeList();
      var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
      Array.Sort(cultures, CompareCultures);

      var invCult = System.Globalization.CultureInfo.InvariantCulture;
      AddCulture(list, invCult, false);

      foreach (var cult in cultures)
        AddCulture(list, cult, false);

      if (list.FirstSelectedNode is null)
        list[0].IsSelected = true;

      return list;
    }

    private void AddCulture(SelectableListNodeList cultureList, CultureInfo cult, bool isSelected)
    {
      cultureList.Add(new SelectableListNode(cult.DisplayName, cult, isSelected));
    }

    private void EhCmdDoAsciiAnalysis()
    {
      ApplyWithoutClosing(); // getting _doc filled with user choices

      if (_doc.IsFullySpecified)
      {
        Current.Gui.InfoMessageBox("The import options are fully specified. There is nothing left for analysis.");
        return;
      }

      ReadAnalysisOptionsAndAnalyze();
    }

    private void ReadAnalysisOptionsAndAnalyze()
    {
      if (!AsciiDocumentAnalysisOptionsController.Apply(false))
        return;

      _analysisOptions = (AsciiDocumentAnalysisOptions)AsciiDocumentAnalysisOptionsController.ModelObject;

      if (_asciiStreamData is not null)
      {
        _asciiStreamData.Seek(0, System.IO.SeekOrigin.Begin);
        _doc = AsciiDocumentAnalysis.Analyze(_doc, _asciiStreamData, _analysisOptions);
        _asciiStreamData.Seek(0, System.IO.SeekOrigin.Begin);
        Initialize(true); // getting Gui elements filled with the result of the analysis
      }
      else if (_asciiStreamDataProvider is not null)
      {
        using (var str = _asciiStreamDataProvider.GetStreamForAnalysis())
        {
          if (str is not null)
          {
            str.Seek(0, System.IO.SeekOrigin.Begin);
            _doc = AsciiDocumentAnalysis.Analyze(_doc, str, _analysisOptions);
            Initialize(true); // getting Gui elements filled with the result of the analysis
          }
        }
      }
    }


    private void EhSeparationStrategyChanged(Type sepType)
    {
      if (_doc.SeparationStrategy is not null && _doc.SeparationStrategy.GetType() == sepType && SeparationStrategyInstanceController is not null)
        return;

      if (SeparationStrategyInstanceController is not null)
      {
        if (SeparationStrategyInstanceController.Apply(false))
        {
          var oldSep = (IAsciiSeparationStrategy)SeparationStrategyInstanceController.ModelObject;
          _separationStrategyInstances[oldSep.GetType()] = oldSep;
        }
      }

      IAsciiSeparationStrategy sep;
      if (_separationStrategyInstances.ContainsKey(sepType))
      {
        sep = _separationStrategyInstances[sepType];
      }
      else
      {
        sep = (IAsciiSeparationStrategy)System.Activator.CreateInstance(sepType);
        _separationStrategyInstances.Add(sep.GetType(), sep);
      }

      _doc = _doc with { SeparationStrategy = sep };

      SeparationStrategyInstanceController = (IMVCANController)Current.Gui.GetController(new object[] { sep }, typeof(IMVCANController));
      if (SeparationStrategyInstanceController is not null)
      {
        Current.Gui.FindAndAttachControlTo(SeparationStrategyInstanceController);
      }
    }
  }
}
