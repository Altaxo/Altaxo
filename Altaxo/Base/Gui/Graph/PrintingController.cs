using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using Altaxo.Collections;

using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Graph
{
	public interface IPrintingView
	{
		void InitializeAvailablePrinters(SelectableListNodeList list);
		void InitializeDocumentPrintOptionsView(object view);
		void ShowPrinterPropertiesDialog(PrinterSettings currentSettings);
		void SetPrintPreview(System.Drawing.Printing.PreviewPageInfo[] preview);
		event Action SelectedPrinterChanged;
		event Action EditPrinterProperties;
		event Action ShowPrintPreview;
	}

	[ExpectedTypeOfView(typeof(IPrintingView))]
	public class PrintingController : IMVCANController
	{
		IPrintingView _view;
		SelectableListNodeList _installedPrinters;
		Altaxo.Graph.Gdi.GraphDocument _doc;

		SingleGraphPrintOptionsController _documentPrintOptionsController;

		void Initialize(bool initData)
		{
			if (initData)
			{
				_documentPrintOptionsController = new SingleGraphPrintOptionsController() { UseDocumentCopy = UseDocument.Directly };
				if (null == _doc.PrintOptions)
					_doc.PrintOptions = new Altaxo.Graph.SingleGraphPrintOptions();
				_documentPrintOptionsController.InitializeDocument(_doc.PrintOptions);
				Current.Gui.FindAndAttachControlTo(_documentPrintOptionsController);
				_doc.PrintOptions.PropertyChanged += new Altaxo.WeakPropertyChangedEventHandler(this.EhDocumentPrintOptionsChanged, x => _doc.PrintOptions.PropertyChanged -= x);
			}

			if(null!=_view)
			{
				var currentPrinterSettings = Current.PrintingService.PrintDocument.PrinterSettings;
				_installedPrinters = new SelectableListNodeList();

				foreach(string printer in PrinterSettings.InstalledPrinters)
					_installedPrinters.Add(new SelectableListNode(printer,printer, currentPrinterSettings.PrinterName==printer));

				_view.InitializeAvailablePrinters(_installedPrinters);

				_view.InitializeDocumentPrintOptionsView(_documentPrintOptionsController.ViewObject);
			}
		}

		void EhSelectedPrinterChanged()
		{
			if (null != _installedPrinters.FirstSelectedNode)
				Current.PrintingService.PrintDocument.PrinterSettings.PrinterName = (string)_installedPrinters.FirstSelectedNode.Tag;
		}

		void CopyPageSettings(PageSettings source, PageSettings dest)
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


		void EhEditPrinterProperties()
		{
			CopyPageSettings(Current.PrintingService.PrintDocument.DefaultPageSettings, Current.PrintingService.PrintDocument.PrinterSettings.DefaultPageSettings);

			_view.ShowPrinterPropertiesDialog(Current.PrintingService.PrintDocument.PrinterSettings);

			// We take the default page settings of the printer now for the print document
			Current.PrintingService.PrintDocument.DefaultPageSettings = Current.PrintingService.PrintDocument.PrinterSettings.DefaultPageSettings;
		}

		void EhDocumentPrintOptionsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
		}

		void UpdatePrinterProperties()
		{
		}


		PrintDocument GetCloneOfPrintDocument(PrintDocument source)
		{
			var result = new PrintDocument();
			result.PrinterSettings = (PrinterSettings)source.PrinterSettings.Clone();
			result.DefaultPageSettings = (PageSettings)source.DefaultPageSettings.Clone();
			result.OriginAtMargins = source.OriginAtMargins;
			return result;
		}

		public void EhShowPreview()
		{
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

			var preview = _previewController.GetPreviewPageInfo();
				_view.SetPrintPreview(preview);
		}


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
			set {  }
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
					_view.ShowPrintPreview -= EhShowPreview;
				}

				_view = value as IPrintingView;

				if (null != _view)
				{
					Initialize(false);
					_view.SelectedPrinterChanged += EhSelectedPrinterChanged;
					_view.EditPrinterProperties += EhEditPrinterProperties;
					_view.ShowPrintPreview += EhShowPreview;
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		public bool Apply()
		{
			return true;
		}

		#endregion
	}
}
