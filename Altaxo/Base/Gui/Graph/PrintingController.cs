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

using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph
{
  public interface IPrintingView
  {
    void InitializeAvailablePrinters(SelectableListNodeList list);

    void InitializeDocumentPrintOptionsView(object view);

    void InitializeAvailablePaperSizes(Collections.SelectableListNodeList list);

    void InitializeAvailablePaperSources(Collections.SelectableListNodeList list);

    void InitializePaperOrientationLandscape(bool isLandScape);

    void InitializePaperMarginsInHundrethInch(double left, double right, double top, double bottom);

    void InitializePrintPreview(System.Drawing.Printing.PreviewPageInfo[] preview);

    void InitializeNumberOfCopies(int val);

    void InitializeCollateCopies(bool val);

    void ShowPrinterPropertiesDialog(PrinterSettings currentSettings);

    event Action SelectedPrinterChanged;

    event Action EditPrinterProperties;

    event Action<bool> PaperOrientationLandscapeChanged;

    event Action PaperSizeChanged;

    event Action PaperSourceChanged;

    event Action<double> MarginLeftChanged;

    event Action<double> MarginRightChanged;

    event Action<double> MarginTopChanged;

    event Action<double> MarginBottomChanged;

    event Action<int> NumberOfCopiesChanged;

    event Action<bool> CollateCopiesChanged;
  }

  [ExpectedTypeOfView(typeof(IPrintingView))]
  public class PrintingController : IMVCANController
  {
    private IPrintingView _view;
    private SelectableListNodeList _installedPrinters;
    private Altaxo.Graph.Gdi.GraphDocument _doc;

    private SingleGraphPrintOptionsController _documentPrintOptionsController;

    private SelectableListNodeList _currentPaperSources;
    private SelectableListNodeList _currentPaperSizes;

    private void Initialize(bool initData)
    {
      if (initData)
      {
        _documentPrintOptionsController = new SingleGraphPrintOptionsController() { UseDocumentCopy = UseDocument.Directly };
        if (null == _doc.PrintOptions)
          _doc.PrintOptions = new SingleGraphPrintOptions();
        _documentPrintOptionsController.InitializeDocument(_doc.PrintOptions);
        Current.Gui.FindAndAttachControlTo(_documentPrintOptionsController);
        _doc.PrintOptions.PropertyChanged += new Altaxo.WeakPropertyChangedEventHandler(this.EhDocumentPrintOptionsChanged, x => _doc.PrintOptions.PropertyChanged -= x);

        InitAvailablePaperSizes(true);
        InitAvailablePaperSources(true);
      }

      if (null != _view)
      {
        var currentPrinterSettings = Current.PrintingService.PrintDocument.PrinterSettings;
        _installedPrinters = new SelectableListNodeList();

        foreach (string printer in PrinterSettings.InstalledPrinters)
          _installedPrinters.Add(new SelectableListNode(printer, printer, currentPrinterSettings.PrinterName == printer));

        _view.InitializeAvailablePrinters(_installedPrinters);

        _view.InitializeDocumentPrintOptionsView(_documentPrintOptionsController.ViewObject);

        InitAvailablePaperSources(false);
        InitAvailablePaperSizes(false);
        InitPaperOrientation();
        InitPaperMargins();
        _view.InitializeNumberOfCopies(Current.PrintingService.PrintDocument.PrinterSettings.Copies);
        _view.InitializeCollateCopies(Current.PrintingService.PrintDocument.PrinterSettings.Collate);

        RequestPreview();
      }
    }

    private void EhSelectedPrinterChanged()
    {
      if (null != _installedPrinters.FirstSelectedNode)
        Current.PrintingService.PrintDocument.PrinterSettings.PrinterName = (string)_installedPrinters.FirstSelectedNode.Tag;

      InitAvailablePaperSizes(true);
      InitAvailablePaperSources(true);
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

      InitAvailablePaperSizes(true);
      InitAvailablePaperSources(true);
      InitPaperOrientation();
      InitPaperMargins();
      RequestPreview();
    }

    private void EhPaperOrientationLandscapeChanged(bool isLandscape)
    {
      var oldValue = Current.PrintingService.PrintDocument.DefaultPageSettings.Landscape;
      Current.PrintingService.PrintDocument.DefaultPageSettings.Landscape = isLandscape;

      if (oldValue != isLandscape)
        RequestPreview();
    }

    private void EhPaperSourceChanged()
    {
      var sel = _currentPaperSources.FirstSelectedNode;
      if (null != sel)
        Current.PrintingService.PrintDocument.DefaultPageSettings.PaperSource = (PaperSource)(sel.Tag);
    }

    private void EhPaperSizeChanged()
    {
      var sel = _currentPaperSizes.FirstSelectedNode;
      if (null != sel)
      {
        Current.PrintingService.PrintDocument.DefaultPageSettings.PaperSize = (PaperSize)(sel.Tag);
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

    private void InitAvailablePaperSources(bool initData)
    {
      if (initData)
      {
        var sources = Current.PrintingService.PrintDocument.PrinterSettings.PaperSources;
        var currSource = Current.PrintingService.PrintDocument.DefaultPageSettings.PaperSource;

        _currentPaperSources = new SelectableListNodeList();
        foreach (PaperSource paper in sources)
        {
          _currentPaperSources.Add(new SelectableListNode(paper.SourceName, paper, paper.SourceName == currSource.SourceName));
        }
      }

      if (null != _view)
      {
        _view.InitializeAvailablePaperSources(_currentPaperSources);
      }
    }

    private void InitAvailablePaperSizes(bool initData)
    {
      if (initData)
      {
        var sizes = Current.PrintingService.PrintDocument.PrinterSettings.PaperSizes;
        var currSize = Current.PrintingService.PrintDocument.DefaultPageSettings.PaperSize;

        _currentPaperSizes = new SelectableListNodeList();
        foreach (PaperSize paper in sizes)
        {
          _currentPaperSizes.Add(new SelectableListNode(paper.PaperName, paper, paper.PaperName == currSize.PaperName));
        }
      }

      if (null != _view)
      {
        _view.InitializeAvailablePaperSizes(_currentPaperSizes);
      }
    }

    private void InitPaperOrientation()
    {
      if (null != _view)
        _view.InitializePaperOrientationLandscape(Current.PrintingService.PrintDocument.DefaultPageSettings.Landscape);
    }

    private void InitPaperMargins()
    {
      if (null != _view)
      {
        var m = Current.PrintingService.PrintDocument.DefaultPageSettings.Margins;
        _view.InitializePaperMarginsInHundrethInch(m.Left, m.Right, m.Top, m.Bottom);
      }
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
      RequestPreview();
    }

    private void UpdatePrinterProperties()
    {
    }

    private PrintDocument GetCloneOfPrintDocument(PrintDocument source)
    {
      var result = new PrintDocument();
      result.PrinterSettings = (PrinterSettings)source.PrinterSettings.Clone();
      result.DefaultPageSettings = (PageSettings)source.DefaultPageSettings.Clone();
      result.OriginAtMargins = source.OriginAtMargins;
      return result;
    }

    #region Preview

    private bool _previewRequested;
    private Task<PreviewPageInfo[]> _previewTask;

    public void RequestPreview()
    {
      _previewRequested = true;
      if (null == _previewTask)
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
      CancellationTokenSource tokenSource = new CancellationTokenSource();
      CancellationToken token = tokenSource.Token;

      _previewTask = Task.Factory.StartNew<PreviewPageInfo[]>(CreatePreviewPageInfo);
      _previewTask.ContinueWith(EhSetPrintPreview, token, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

      //Console.WriteLine("End InitiatePreview");
    }

    private PreviewPageInfo[] CreatePreviewPageInfo()
    {
      //Console.WriteLine("Begin CreatePreviewPageInfo");

      // use not the print document directly, but a clone - since we evaluate the print preview in a separate task
      var printDocument = GetCloneOfPrintDocument(Current.PrintingService.PrintDocument);

      GraphDocumentPrintTask printTask = new GraphDocumentPrintTask(_doc);
      printTask.IsPrintPreview = true;
      System.Drawing.Printing.PreviewPrintController _previewController = new System.Drawing.Printing.PreviewPrintController();

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

    private void EhSetPrintPreview(Task<PreviewPageInfo[]> t)
    {
      if (null != _view)
        _view.InitializePrintPreview(t.Result);

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

    #endregion Preview

    #region IMVCANController

    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0 || !(args[0] is Altaxo.Graph.Gdi.GraphDocument))
        return false;

      _doc = (Altaxo.Graph.Gdi.GraphDocument)args[0];

      Initialize(true);

      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { }
    }

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (null != _view)
        {
          _view.SelectedPrinterChanged -= EhSelectedPrinterChanged;
          _view.EditPrinterProperties -= EhEditPrinterProperties;
          _view.PaperOrientationLandscapeChanged -= this.EhPaperOrientationLandscapeChanged;
          _view.PaperSizeChanged -= this.EhPaperSizeChanged;
          _view.PaperSourceChanged -= this.EhPaperSourceChanged;
          _view.MarginLeftChanged -= this.EhMarginLeftChanged;
          _view.MarginRightChanged -= this.EhMarginRightChanged;
          _view.MarginTopChanged -= this.EhMarginTopChanged;
          _view.MarginBottomChanged -= this.EhMarginBottomChanged;
          _view.NumberOfCopiesChanged -= this.EhNumberOfCopiesChanged;
          _view.CollateCopiesChanged -= this.EhCollateCopiesChanged;
        }

        _view = value as IPrintingView;

        if (null != _view)
        {
          Initialize(false);
          _view.SelectedPrinterChanged += EhSelectedPrinterChanged;
          _view.EditPrinterProperties += EhEditPrinterProperties;
          _view.PaperOrientationLandscapeChanged += this.EhPaperOrientationLandscapeChanged;
          _view.PaperSizeChanged += this.EhPaperSizeChanged;
          _view.PaperSourceChanged += this.EhPaperSourceChanged;
          _view.MarginLeftChanged += this.EhMarginLeftChanged;
          _view.MarginRightChanged += this.EhMarginRightChanged;
          _view.MarginTopChanged += this.EhMarginTopChanged;
          _view.MarginBottomChanged += this.EhMarginBottomChanged;
          _view.NumberOfCopiesChanged += this.EhNumberOfCopiesChanged;
          _view.CollateCopiesChanged += this.EhCollateCopiesChanged;
        }
      }
    }

    public object ModelObject
    {
      get { return _doc; }
    }

    public void Dispose()
    {
    }

    public bool Apply(bool disposeController)
    {
      var previewTask = _previewTask;
      if (null != previewTask)
        previewTask.Wait();

      Current.PrintingService.PrintDocument.DocumentName = _doc.Name;
      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IMVCANController
  }
}
