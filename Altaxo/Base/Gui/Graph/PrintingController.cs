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
using System.Drawing.Printing;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common;
using Altaxo.Units;

namespace Altaxo.Gui.Graph
{
  public interface IPrintingView : IDataContextAwareView
  {
    void ShowPrinterPropertiesDialog(PrinterSettings currentSettings);
  }

  [ExpectedTypeOfView(typeof(IPrintingView))]
  public class PrintingController : MVCANControllerEditImmutableDocBase<GraphDocument, IPrintingView>
  {
    /// <summary>Number of the page that is currently previewed.</summary>
    private int _previewPageNumber;
    private System.Drawing.Printing.PreviewPageInfo[] _previewData;
    private System.Threading.CancellationToken _printerStatusCancellationToken;
    private System.Threading.CancellationTokenSource _printerStatusCancellationTokenSource;

    public PrintingController()
    {
      CmdShowPrinterProperties = new RelayCommand(EhEditPrinterProperties);
      CmdPreviewFirstPage = new RelayCommand(EhPreviewFirstPage);
      CmdPreviewPreviousPage = new RelayCommand(EhPreviewPreviousPage);
      CmdPreviewNextPage = new RelayCommand(EhPreviewNextPage);
      CmdPreviewLastPage = new RelayCommand(EhPreviewLastPage);


      _printerStatusCancellationTokenSource = new System.Threading.CancellationTokenSource();
      _printerStatusCancellationToken = _printerStatusCancellationTokenSource.Token;
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_documentPrintOptionsController, () => DocumentPrintOptionsController = null);
    }

    public override void Dispose(bool isDisposing)
    {
      _printerStatusCancellationTokenSource.Cancel();
      base.Dispose(isDisposing);
    }

    #region Bindings

    public ICommand CmdShowPrinterProperties { get; }

    public ICommand CmdPreviewFirstPage { get; }
    public ICommand CmdPreviewPreviousPage { get; }

    public ICommand CmdPreviewNextPage { get; }
    public ICommand CmdPreviewLastPage { get; }


    private SingleGraphPrintOptionsController _documentPrintOptionsController;


    public SingleGraphPrintOptionsController DocumentPrintOptionsController
    {
      get => _documentPrintOptionsController;
      set
      {
        if (!(_documentPrintOptionsController == value))
        {
          _documentPrintOptionsController?.Dispose();
          _documentPrintOptionsController = value;

          if (_documentPrintOptionsController is { } newC)
            newC.PropertyChanged += new Altaxo.WeakPropertyChangedEventHandler(EhDocumentPrintOptionsChanged, _doc.PrintOptions, nameof(_doc.PrintOptions.PropertyChanged));


          OnPropertyChanged(nameof(DocumentPrintOptionsController));
        }
      }
    }

    private ItemsController<string> _availablePrinters;

    public ItemsController<string> AvailablePrinters
    {
      get => _availablePrinters;
      set
      {
        if (!(_availablePrinters == value))
        {
          _availablePrinters?.Dispose();
          _availablePrinters = value;
          OnPropertyChanged(nameof(AvailablePrinters));
        }
      }
    }


    private ItemsController<PaperSize> _availablePaperSizes;

    public ItemsController<PaperSize> AvailablePaperSizes
    {
      get => _availablePaperSizes;
      set
      {
        if (!(_availablePaperSizes == value))
        {
          _availablePaperSizes?.Dispose();
          _availablePaperSizes = value;
          OnPropertyChanged(nameof(AvailablePaperSizes));
        }
      }
    }

    private ItemsController<PaperSource> _availablePaperSources;

    public ItemsController<PaperSource> AvailablePaperSources
    {
      get => _availablePaperSources;
      set
      {
        if (!(_availablePaperSources == value))
        {
          _availablePaperSources?.Dispose();
          _availablePaperSources = value;
          OnPropertyChanged(nameof(AvailablePaperSources));
        }
      }
    }

    private bool _isPaperOrientationLandscape;

    public bool IsPaperOrientationLandscape
    {
      get => _isPaperOrientationLandscape;
      set
      {
        if (!(_isPaperOrientationLandscape == value))
        {
          _isPaperOrientationLandscape = value;
          OnPropertyChanged(nameof(IsPaperOrientationLandscape));
          EhPaperOrientationLandscapeChanged(value);
        }
      }
    }


    public bool IsPaperOrientationPortrait
    {
      get => !IsPaperOrientationLandscape;
      set => IsPaperOrientationLandscape = !value;
    }

    public QuantityWithUnitGuiEnvironment MarginEnvironment => PaperMarginEnvironment.Instance;

    private DimensionfulQuantity _marginLeft;

    public DimensionfulQuantity MarginLeft
    {
      get => _marginLeft;
      set
      {
        if (!(_marginLeft == value))
        {
          _marginLeft = value;
          OnPropertyChanged(nameof(MarginLeft));
          EhMarginLeftChanged(value.AsValueIn(SIPrefix.Centi, Altaxo.Units.Length.Inch.Instance));
        }
      }
    }
    private DimensionfulQuantity _marginRight;

    public DimensionfulQuantity MarginRight
    {
      get => _marginRight;
      set
      {
        if (!(_marginRight == value))
        {
          _marginRight = value;
          OnPropertyChanged(nameof(MarginRight));
          EhMarginRightChanged(value.AsValueIn(SIPrefix.Centi, Altaxo.Units.Length.Inch.Instance));
        }
      }
    }

    private DimensionfulQuantity _marginTop;

    public DimensionfulQuantity MarginTop
    {
      get => _marginTop;
      set
      {
        if (!(_marginTop == value))
        {
          _marginTop = value;
          OnPropertyChanged(nameof(MarginTop));
          EhMarginTopChanged(value.AsValueIn(SIPrefix.Centi, Altaxo.Units.Length.Inch.Instance));
        }
      }
    }
    private DimensionfulQuantity _marginBottom;

    public DimensionfulQuantity MarginBottom
    {
      get => _marginBottom;
      set
      {
        if (!(_marginBottom == value))
        {
          _marginBottom = value;
          OnPropertyChanged(nameof(MarginBottom));
          EhMarginBottomChanged(value.AsValueIn(SIPrefix.Centi, Altaxo.Units.Length.Inch.Instance));
        }
      }
    }


    private int _numberOfCopies;

    public int NumberOfCopies
    {
      get => _numberOfCopies;
      set
      {
        if (!(_numberOfCopies == value))
        {
          _numberOfCopies = value;
          OnPropertyChanged(nameof(NumberOfCopies));
          EhNumberOfCopiesChanged(value);
        }
      }
    }


    private bool _collateCopies;

    public bool CollateCopies
    {
      get => _collateCopies;
      set
      {
        if (!(_collateCopies == value))
        {
          _collateCopies = value;
          OnPropertyChanged(nameof(CollateCopies));
          EhCollateCopiesChanged(value);
        }
      }
    }

    private string _printerStatus = "Ready";

    public string PrinterStatus
    {
      get => _printerStatus;
      set
      {
        if (!(_printerStatus == value))
        {
          _printerStatus = value;
          OnPropertyChanged(nameof(PrinterStatus));
        }
      }
    }
    private string _printerLocation = "Unknown";

    public string PrinterLocation
    {
      get => _printerLocation;
      set
      {
        if (!(_printerLocation == value))
        {
          _printerLocation = value;
          OnPropertyChanged(nameof(PrinterLocation));
        }
      }
    }
    private string _printerComment = "Unknown";

    public string PrinterComment
    {
      get => _printerComment;
      set
      {
        if (!(_printerComment == value))
        {
          _printerComment = value;
          OnPropertyChanged(nameof(PrinterComment));
        }
      }
    }


    private string _previewPageNumberText;

    public string PreviewPageNumberText
    {
      get => _previewPageNumberText;
      set
      {
        if (!(_previewPageNumberText == value))
        {
          _previewPageNumberText = value;
          OnPropertyChanged(nameof(PreviewPageNumberText));
        }
      }
    }

    public PreviewPageInfo CurrentPreviewData
    {
      get => _previewData is not null && _previewPageNumber < _previewData.Length ? _previewData[_previewPageNumber] : null;
    }


    private double _previewHeight;

    public double PreviewHeight
    {
      get => _previewHeight;
      set
      {
        if (!(_previewHeight == value))
        {
          _previewHeight = value;
          OnPropertyChanged(nameof(PreviewHeight));
        }
      }
    }
    private double _previewWidth;

    public double PreviewWidth
    {
      get => _previewWidth;
      set
      {
        if (!(_previewWidth == value))
        {
          _previewWidth = value;
          OnPropertyChanged(nameof(PreviewWidth));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      if (initData)
      {
        var documentPrintOptionsController = new SingleGraphPrintOptionsController() { UseDocumentCopy = UseDocument.Directly };
        if (_doc.PrintOptions is null)
          _doc.PrintOptions = new SingleGraphPrintOptions();
        documentPrintOptionsController.InitializeDocument(_doc.PrintOptions);
        Current.Gui.FindAndAttachControlTo(documentPrintOptionsController);
        DocumentPrintOptionsController = documentPrintOptionsController;


        InitAvailablePaperSizes();
        InitAvailablePaperSources();

        var currentPrinterSettings = Current.PrintingService.PrintDocument.PrinterSettings;
        var installedPrinters = new SelectableListNodeList();

        foreach (string printer in PrinterSettings.InstalledPrinters)
          installedPrinters.Add(new SelectableListNode(printer, printer, currentPrinterSettings.PrinterName == printer));

        AvailablePrinters = new ItemsController<string>(installedPrinters, EhSelectedPrinterChanged);
        InitAvailablePaperSources();
        InitAvailablePaperSizes();
        InitPaperOrientation();
        InitPaperMargins();
        NumberOfCopies = Current.PrintingService.PrintDocument.PrinterSettings.Copies;
        CollateCopies = Current.PrintingService.PrintDocument.PrinterSettings.Collate;

        RequestPreview();

        System.Threading.Tasks.Task.Factory.StartNew(UpdatePrinterStatusGuiElements, _printerStatusCancellationToken);
      }
    }

    private void EhSelectedPrinterChanged(string newPrinter)
    {
      if (newPrinter is not null)
        Current.PrintingService.PrintDocument.PrinterSettings.PrinterName = newPrinter;

      InitAvailablePaperSizes();
      InitAvailablePaperSources();
      InitPaperOrientation();
      InitPaperMargins();
      RequestPreview();
    }

    private void EhEditPrinterProperties()
    {
      CopyPageSettings(Current.PrintingService.PrintDocument.DefaultPageSettings, Current.PrintingService.PrintDocument.PrinterSettings.DefaultPageSettings);

      _view.ShowPrinterPropertiesDialog(Current.PrintingService.PrintDocument.PrinterSettings);

      // We take the default page settings of the printer now for the print document
      Current.PrintingService.PrintDocument.DefaultPageSettings = Current.PrintingService.PrintDocument.PrinterSettings.DefaultPageSettings;

      InitAvailablePaperSizes();
      InitAvailablePaperSources();
      InitPaperOrientation();
      InitPaperMargins();
      RequestPreview();
    }

    private void EhPaperOrientationLandscapeChanged(bool isLandscape)
    {
      Current.PrintingService.PrintDocument.DefaultPageSettings.Landscape = isLandscape;
      RequestPreview();
    }

    private void EhPaperSourceChanged(PaperSource newValue)
    {
      if (newValue is not null)
      {
        Current.PrintingService.PrintDocument.DefaultPageSettings.PaperSource = newValue;
      }
    }

    private void EhPaperSizeChanged(PaperSize newValue)
    {
      if (newValue is not null)
      {
        Current.PrintingService.PrintDocument.DefaultPageSettings.PaperSize = newValue;
        RequestPreview();
      }
    }

    private void EhMarginLeftChanged(double val)
    {
      Current.PrintingService.PrintDocument.DefaultPageSettings.Margins.Left = (int)val;
      RequestPreview();
    }

    private void EhMarginRightChanged(double val)
    {
      Current.PrintingService.PrintDocument.DefaultPageSettings.Margins.Right = (int)val;
      RequestPreview();
    }

    private void EhMarginTopChanged(double val)
    {
      Current.PrintingService.PrintDocument.DefaultPageSettings.Margins.Top = (int)val;
      RequestPreview();
    }

    private void EhMarginBottomChanged(double val)
    {
      Current.PrintingService.PrintDocument.DefaultPageSettings.Margins.Bottom = (int)val;
      RequestPreview();
    }

    private void EhNumberOfCopiesChanged(int val)
    {
      Current.PrintingService.PrintDocument.PrinterSettings.Copies = (short)val;
    }

    private void EhCollateCopiesChanged(bool val)
    {
      Current.PrintingService.PrintDocument.PrinterSettings.Collate = val;
    }

    private void EhPreviewFirstPage()
    {
      _previewPageNumber = 0;
      UpdatePreviewPageAndText();
    }

    private void EhPreviewPreviousPage()
    {
      _previewPageNumber = Math.Max(0, _previewPageNumber - 1);
      UpdatePreviewPageAndText();
    }

    private void EhPreviewNextPage()
    {
      _previewPageNumber = Math.Min(_previewPageNumber + 1, _previewData is not null ? _previewData.Length - 1 : 0);
      UpdatePreviewPageAndText();
    }

    private void EhPreviewLastPage()
    {
      _previewPageNumber = _previewData is not null ? _previewData.Length - 1 : 0;
      UpdatePreviewPageAndText();
    }

    private void UpdatePreviewPageAndText()
    {
      if (_previewData is null)
      {
        PreviewPageNumberText = string.Empty;
      }
      else
      {
        _previewPageNumber = Math.Max(0, Math.Min(_previewPageNumber, _previewData.Length - 1));
        RequestPreview();
      }
    }

    private void InitAvailablePaperSources()
    {
      var sources = Current.PrintingService.PrintDocument.PrinterSettings.PaperSources;
      var currSource = Current.PrintingService.PrintDocument.DefaultPageSettings.PaperSource;

      var currentPaperSources = new SelectableListNodeList();
      foreach (PaperSource paper in sources)
      {
        currentPaperSources.Add(new SelectableListNode(paper.SourceName, paper, paper.SourceName == currSource.SourceName));
      }

      AvailablePaperSources = new ItemsController<PaperSource>(currentPaperSources, EhPaperSourceChanged);
    }

    private void InitAvailablePaperSizes()
    {

      var sizes = Current.PrintingService.PrintDocument.PrinterSettings.PaperSizes;
      var currSize = Current.PrintingService.PrintDocument.DefaultPageSettings.PaperSize;

      var currentPaperSizes = new SelectableListNodeList();
      foreach (PaperSize paper in sizes)
      {
        currentPaperSizes.Add(new SelectableListNode(paper.PaperName, paper, paper.PaperName == currSize.PaperName));
      }

      AvailablePaperSizes = new ItemsController<PaperSize>(currentPaperSizes, EhPaperSizeChanged);
    }

    private void InitPaperOrientation()
    {
      IsPaperOrientationLandscape = Current.PrintingService.PrintDocument.DefaultPageSettings.Landscape;
    }

    private void InitPaperMargins()
    {
      var m = Current.PrintingService.PrintDocument.DefaultPageSettings.Margins;
      MarginLeft = new DimensionfulQuantity(m.Left, SIPrefix.Centi, Altaxo.Units.Length.Inch.Instance).AsQuantityIn(MarginEnvironment.DefaultUnit);
      MarginRight = new DimensionfulQuantity(m.Right, SIPrefix.Centi, Altaxo.Units.Length.Inch.Instance).AsQuantityIn(MarginEnvironment.DefaultUnit);
      MarginTop = new DimensionfulQuantity(m.Top, SIPrefix.Centi, Altaxo.Units.Length.Inch.Instance).AsQuantityIn(MarginEnvironment.DefaultUnit);
      MarginBottom = new DimensionfulQuantity(m.Bottom, SIPrefix.Centi, Altaxo.Units.Length.Inch.Instance).AsQuantityIn(MarginEnvironment.DefaultUnit);
    }

    private void CopyPageSettings(PageSettings source, PageSettings dest)
    {
      try
      {
        dest.Color = source.Color;
        dest.Landscape = source.Landscape;
        dest.PaperSize = source.PaperSize;
        dest.PaperSource = source.PaperSource;
      }
      catch (Exception)
      {
      }
    }

    private void EhDocumentPrintOptionsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      DocumentPrintOptionsController.Apply(false);
      RequestPreview();
    }

    private PrintDocument GetCloneOfPrintDocument(PrintDocument source)
    {
      var result = new PrintDocument
      {
        PrinterSettings = (PrinterSettings)source.PrinterSettings.Clone(),
        DefaultPageSettings = (PageSettings)source.DefaultPageSettings.Clone(),
        OriginAtMargins = source.OriginAtMargins
      };
      return result;
    }

    #region Preview

    private bool _previewRequested;
    private Task<PreviewPageInfo[]> _previewTask;



    private PreviewPageInfo[] CreatePreviewPageInfo()
    {
      //Console.WriteLine("Begin CreatePreviewPageInfo");

      // use not the print document directly, but a clone - since we evaluate the print preview in a separate task
      var printDocument = GetCloneOfPrintDocument(Current.PrintingService.PrintDocument);

      var printTask = new GraphDocumentPrintTask(_doc)
      {
        IsPrintPreview = true
      };
      var _previewController = new System.Drawing.Printing.PreviewPrintController();

      printDocument.PrintController = _previewController;
      printDocument.PrintPage += printTask.EhPrintPage;
      printDocument.QueryPageSettings += printTask.EhQueryPageSettings;

      printDocument.Print();

      printDocument.PrintPage -= printTask.EhPrintPage;
      printDocument.QueryPageSettings -= printTask.EhQueryPageSettings;
      printDocument.PrintController = null;

      //Console.WriteLine("End CreatePreviewPageInfo");
      return _previewController.GetPreviewPageInfo();
    }

    public void RequestPreview()
    {
      _previewRequested = true;
      if (_previewTask is null)
      {
        _previewRequested = false;
        InitiatePreview();
      }
    }

    private void InitiatePreview()
    {
      _previewRequested = false;
      //Console.WriteLine("Begin InitiatePreview");
      //create CancellationTokenSource, so we can use the overload of
      //the Task.Factory that allows us to pass in a SynchronizationContext
      var tokenSource = new CancellationTokenSource();
      CancellationToken token = tokenSource.Token;

      _previewTask = Task.Factory.StartNew<PreviewPageInfo[]>(CreatePreviewPageInfo);
      _previewTask.ContinueWith(EhSetPrintPreview, token, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

      //Console.WriteLine("End InitiatePreview");
    }

    private void EhSetPrintPreview(Task<PreviewPageInfo[]> t)
    {
      _previewData = t.Result;
      OnPropertyChanged(nameof(CurrentPreviewData));
      PreviewPageNumberText = _previewData is not null ? $"{_previewPageNumber + 1} of {_previewData.Length}" : String.Empty;


      if (_previewRequested)
      {
        //Console.WriteLine("EhSetPrintPreview next Initiate");
        InitiatePreview();
      }
      else
      {
        _previewTask = null;
        //Console.WriteLine("End EhSetPrintPreview null");
      }

      //Console.WriteLine("End EhSetPrintPreview");
    }

    private void UpdatePrinterStatusGuiElements()
    {
      for (; ; )
      {
        if (_printerStatusCancellationToken.IsCancellationRequested)
          break;

        string printerName = AvailablePrinters.SelectedValue;
        if (string.IsNullOrEmpty(printerName))
        {
          System.Threading.Thread.Sleep(100);
          continue;
        }

        string comment = string.Empty;
        string status = string.Empty;
        string location = string.Empty;
        bool? isOffline = null;

        string escapedPrinterName = printerName.Replace("\\", "\\\\");
        //string query = string.Format("SELECT * from Win32_Printer WHERE Name LIKE '%{0}'", escapedPrinterName);
        string query = string.Format("SELECT * from Win32_Printer WHERE Name = '{0}'", escapedPrinterName);
        var searcher = new System.Management.ManagementObjectSearcher(query);
        ManagementObjectCollection coll = searcher.Get();
        foreach (ManagementObject printer in coll)
        {
          status = (string)printer.GetPropertyValue("Status");
          comment = (string)printer.GetPropertyValue("Comment");
          location = (string)printer.GetPropertyValue("Location");
          isOffline = (bool)printer.GetPropertyValue("WorkOffline");
          break;
        }

        if (true == isOffline)
          status = "Offline";

        if (_printerStatusCancellationToken.IsCancellationRequested)
          break;

        Current.Dispatcher.InvokeIfRequired(() =>
        {
          PrinterStatus = status;
          PrinterComment = comment;
          PrinterLocation = location;
        });
        System.Threading.Thread.Sleep(100);
      }
    }

    #endregion Preview

    public override bool Apply(bool disposeController)
    {
      var previewTask = _previewTask;
      if (previewTask is not null)
        previewTask.Wait();

      Current.PrintingService.PrintDocument.DocumentName = _doc.Name;
      return true;
    }
  }
}
