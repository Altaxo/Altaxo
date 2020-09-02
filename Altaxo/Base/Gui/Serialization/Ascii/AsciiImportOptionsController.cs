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
using System.Globalization;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Serialization.Ascii;

namespace Altaxo.Gui.Serialization.Ascii
{
  public interface IAsciiImportOptionsView
  {
    event Action DoAnalyze;

    event Action SeparationStrategyChanged;

    int? NumberOfMainHeaderLines { get; set; }

    int? IndexOfCaptionLine { get; set; }

    void SetGuiSeparationStrategy(SelectableListNodeList list);

    bool GuiSeparationStrategyIsKnown { get; set; }

    void SetNumberFormatCulture(SelectableListNodeList list);

    bool NumberFormatCultureIsKnowm { get; set; }

    void SetDateTimeFormatCulture(SelectableListNodeList list);

    bool DateTimeFormatCultureIsKnown { get; set; }

    bool TableStructureIsKnown { get; set; }

    System.Collections.ObjectModel.ObservableCollection<Boxed<AsciiColumnType>> TableStructure { set; }

    bool RenameColumnsWithHeaderNames { get; set; }

    bool RenameWorksheetWithFileName { get; set; }

    SelectableListNodeList HeaderLinesDestination { set; }

    object AsciiSeparationStrategyDetailView { set; }

    object AsciiDocumentAnalysisOptionsView { get; }

    bool ImportMultipleAsciiVertically { get; set; }
  }

  /// <summary>
  /// Supports getting a stream of analysis data. Any function using the <see cref="AsciiImportOptionsController"/> can create an object which implemnts
  /// this interface, and providing this object as second argument to the InitializeDocument function of the controller.
  /// </summary>
  public interface IAsciiImportOptionsAnalysisDataProvider
  {
    /// <summary>
    /// Gets a stream for analysis of the ASCII file. If the stream could not be opened (file unavailable), this function will return null without throwinga an exception.
    /// </summary>
    /// <returns></returns>
    System.IO.Stream GetStreamForAnalysis();
  }

  [ExpectedTypeOfView(typeof(IAsciiImportOptionsView))]
  [UserControllerForObject(typeof(AsciiImportOptions))]
  public class AsciiImportOptionsController : MVCANControllerEditOriginalDocBase<AsciiImportOptions, IAsciiImportOptionsView>
  {
    private System.IO.Stream _asciiStreamData;
    private IAsciiImportOptionsAnalysisDataProvider _asciiStreamDataProvider;

    private SelectableListNodeList _separationStrategyList;
    private SelectableListNodeList _numberFormatList;
    private SelectableListNodeList _dateTimeFormatList;
    private SelectableListNodeList _headerLinesDestination;
    private System.Collections.ObjectModel.ObservableCollection<Boxed<AsciiColumnType>> _tableStructure;

    private IMVCANController _separationStrategyInstanceController;
    private IMVCANController _asciiDocumentAnalysisOptionsController;

    private Dictionary<Type, IAsciiSeparationStrategy> _separationStrategyInstances = new Dictionary<Type, IAsciiSeparationStrategy>();

    private AsciiDocumentAnalysisOptions _analysisOptions;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_separationStrategyInstanceController, () => _separationStrategyInstanceController = null);
      yield return new ControllerAndSetNullMethod(_asciiDocumentAnalysisOptionsController, () => _asciiDocumentAnalysisOptionsController = null);
    }

    public override bool InitializeDocument(params object[] args)
    {
      if (args is not null && args.Length >= 2 && args[1] is System.IO.Stream)
        _asciiStreamData = args[1] as System.IO.Stream;
      if (args is not null && args.Length >= 2 && args[1] is IAsciiImportOptionsAnalysisDataProvider)
        _asciiStreamDataProvider = args[1] as IAsciiImportOptionsAnalysisDataProvider;

      if (args is not null && args.Length >= 3 && args[2] is AsciiDocumentAnalysisOptions)
        _analysisOptions = (AsciiDocumentAnalysisOptions)args[2];
      else
        _analysisOptions = Current.PropertyService.GetValue(AsciiDocumentAnalysisOptions.PropertyKeyAsciiDocumentAnalysisOptions, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin);

      return base.InitializeDocument(args);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _asciiDocumentAnalysisOptionsController = (IMVCANController)Current.Gui.GetController(new object[] { _analysisOptions }, typeof(IMVCANController), UseDocument.Directly);

        _separationStrategyInstances.Clear();

        if (_doc.SeparationStrategy is not null)
          _separationStrategyInstances.Add(_doc.SeparationStrategy.GetType(), _doc.SeparationStrategy);

        _separationStrategyList = new SelectableListNodeList();
        var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Serialization.Ascii.IAsciiSeparationStrategy));

        foreach (var t in types)
          _separationStrategyList.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), t, _doc.SeparationStrategy is null ? false : t == _doc.SeparationStrategy.GetType()));

        GetAvailableCultures(ref _numberFormatList, _doc.NumberFormatCulture);
        GetAvailableCultures(ref _dateTimeFormatList, _doc.DateTimeFormatCulture);

        _headerLinesDestination = new SelectableListNodeList(_doc.HeaderLinesDestination);

        if (_doc.RecognizedStructure is not null)
        {
          _tableStructure = new System.Collections.ObjectModel.ObservableCollection<Boxed<AsciiColumnType>>(Boxed<AsciiColumnType>.ToBoxedItems(_doc.RecognizedStructure.ColumnTypes));
        }
        else
        {
          _tableStructure = new System.Collections.ObjectModel.ObservableCollection<Boxed<AsciiColumnType>>();
        }
      }

      if (_view is not null)
      {
        _asciiDocumentAnalysisOptionsController.ViewObject = _view.AsciiDocumentAnalysisOptionsView;

        _view.NumberOfMainHeaderLines = _doc.NumberOfMainHeaderLines;
        _view.IndexOfCaptionLine = _doc.IndexOfCaptionLine;

        _view.RenameColumnsWithHeaderNames = _doc.RenameColumns;
        _view.RenameWorksheetWithFileName = _doc.RenameWorksheet;

        _view.GuiSeparationStrategyIsKnown = _doc.SeparationStrategy is not null;
        _view.SetGuiSeparationStrategy(_separationStrategyList);
        EhSeparationStrategyChanged();

        _view.NumberFormatCultureIsKnowm = _doc.NumberFormatCulture is not null;
        _view.DateTimeFormatCultureIsKnown = _doc.DateTimeFormatCulture is not null;
        _view.TableStructureIsKnown = _doc.RecognizedStructure is not null;

        _view.SetNumberFormatCulture(_numberFormatList);
        _view.SetDateTimeFormatCulture(_dateTimeFormatList);

        _view.TableStructure = _tableStructure;

        _view.HeaderLinesDestination = _headerLinesDestination;

        _view.ImportMultipleAsciiVertically = _doc.ImportMultipleStreamsVertically;
      }
    }

    private bool ApplyWithoutClosing()
    {
      if (_separationStrategyInstanceController is not null)
        if (_separationStrategyInstanceController.Apply(false))
          _doc.SeparationStrategy = (IAsciiSeparationStrategy)_separationStrategyInstanceController.ModelObject;
        else
          return false;

      _doc.NumberOfMainHeaderLines = _view.NumberOfMainHeaderLines;
      _doc.IndexOfCaptionLine = _view.IndexOfCaptionLine;

      _doc.RenameColumns = _view.RenameColumnsWithHeaderNames;
      _doc.RenameWorksheet = _view.RenameWorksheetWithFileName;
      _doc.ImportMultipleStreamsVertically = _view.ImportMultipleAsciiVertically;

      if (_view.NumberFormatCultureIsKnowm)
      {
        _doc.NumberFormatCulture = (CultureInfo)_numberFormatList.FirstSelectedNode.Tag;
      }
      else
      {
        _doc.NumberFormatCulture = null;
      }

      if (_view.DateTimeFormatCultureIsKnown)
      {
        _doc.DateTimeFormatCulture = (CultureInfo)_dateTimeFormatList.FirstSelectedNode.Tag;
      }
      else
      {
        _doc.DateTimeFormatCulture = null;
      }

      if (_view.GuiSeparationStrategyIsKnown)
      {
        // this case was already handled above
      }
      else
      {
        _doc.SeparationStrategy = null;
      }

      if (_view.TableStructureIsKnown)
      {
        _doc.RecognizedStructure.Clear();
        Boxed<AsciiColumnType>.AddRange(_doc.RecognizedStructure.ColumnTypes, _tableStructure);
        if (_doc.RecognizedStructure.Count == 0)
          _doc.RecognizedStructure = null;
      }
      else
      {
        _doc.RecognizedStructure = null;
      }

      _doc.HeaderLinesDestination = (AsciiHeaderLinesDestination)_headerLinesDestination.FirstSelectedNode.Tag;

      return true;
    }

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

    protected override void AttachView()
    {
      base.AttachView();

      _view.DoAnalyze += EhDoAsciiAnalysis;
      _view.SeparationStrategyChanged += EhSeparationStrategyChanged;
    }

    protected override void DetachView()
    {
      _view.DoAnalyze -= EhDoAsciiAnalysis;
      _view.SeparationStrategyChanged -= EhSeparationStrategyChanged;

      base.DetachView();
    }

    private int CompareCultures(CultureInfo x, CultureInfo y)
    {
      return string.Compare(x.DisplayName, y.DisplayName);
    }

    private void GetAvailableCultures(ref SelectableListNodeList list, CultureInfo currentlySelectedCulture)
    {
      list = new SelectableListNodeList();
      var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
      Array.Sort(cultures, CompareCultures);

      var invCult = System.Globalization.CultureInfo.InvariantCulture;
      AddCulture(list, invCult, currentlySelectedCulture is not null && invCult.ThreeLetterISOLanguageName == currentlySelectedCulture.ThreeLetterISOLanguageName);

      foreach (var cult in cultures)
        AddCulture(list, cult, currentlySelectedCulture is not null && cult.Name == currentlySelectedCulture.Name);

      if (list.FirstSelectedNode is null)
        list[0].IsSelected = true;
    }

    private void AddCulture(SelectableListNodeList cultureList, CultureInfo cult, bool isSelected)
    {
      cultureList.Add(new SelectableListNode(cult.DisplayName, cult, isSelected));
    }

    private void EhDoAsciiAnalysis()
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
      if (!_asciiDocumentAnalysisOptionsController.Apply(false))
        return;

      _analysisOptions = (AsciiDocumentAnalysisOptions)_asciiDocumentAnalysisOptionsController.ModelObject;

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

    private void EhSeparationStrategyChanged()
    {
      var selNode = _separationStrategyList.FirstSelectedNode;
      if (selNode is null)
        return;

      var sepType = (Type)selNode.Tag;
      if (_doc.SeparationStrategy is not null && _doc.SeparationStrategy.GetType() == sepType && _separationStrategyInstanceController is not null)
        return;

      if (_separationStrategyInstanceController is not null)
      {
        if (_separationStrategyInstanceController.Apply(false))
        {
          var oldSep = (IAsciiSeparationStrategy)_separationStrategyInstanceController.ModelObject;
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
        sep = (IAsciiSeparationStrategy)System.Activator.CreateInstance((Type)selNode.Tag);
        _separationStrategyInstances.Add(sep.GetType(), sep);
      }

      _doc.SeparationStrategy = sep;

      _separationStrategyInstanceController = (IMVCANController)Current.Gui.GetController(new object[] { sep }, typeof(IMVCANController));
      object view = null;
      if (_separationStrategyInstanceController is not null)
      {
        Current.Gui.FindAndAttachControlTo(_separationStrategyInstanceController);
        view = _separationStrategyInstanceController.ViewObject;
      }
      if (_view is not null)
        _view.AsciiSeparationStrategyDetailView = view;
    }
  }
}
