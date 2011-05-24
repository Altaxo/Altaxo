using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Printing;

using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
	public interface IPrintingView
	{
		void InitializeAvailablePrinters(SelectableListNodeList list);
		void ShowPrinterPropertiesDialog(PrinterSettings currentSettings);
		event Action SelectedPrinterChanged;
		event Action EditPrinterProperties;
	}

	[ExpectedTypeOfView(typeof(IPrintingView))]
	public class PrintingController : IMVCANController
	{
		IPrintingView _view;
		SelectableListNodeList _installedPrinters;
		PrinterSettings _currentPrinterSettings =  new PrinterSettings();

		void Initialize(bool initData)
		{
			if (initData)
			{
			}

			if(null!=_view)
			{
				_installedPrinters = new SelectableListNodeList();
				foreach(string printer in PrinterSettings.InstalledPrinters)
					_installedPrinters.Add(new SelectableListNode(printer,printer,_currentPrinterSettings.PrinterName==printer));

				_view.InitializeAvailablePrinters(_installedPrinters);
			}
		}

		void EhSelectedPrinterChanged()
		{
			if (null != _installedPrinters.FirstSelectedNode)
				_currentPrinterSettings.PrinterName = (string)_installedPrinters.FirstSelectedNode.Tag;
		}

		void EhEditPrinterProperties()
		{
			_view.ShowPrinterPropertiesDialog(_currentPrinterSettings);
		}

		void UpdatePrinterProperties()
		{
		}

		#region IMVCANController
		public bool InitializeDocument(params object[] args)
		{
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
				}

				_view = value as IPrintingView;

				if (null != _view)
				{
					Initialize(false);
					_view.SelectedPrinterChanged += EhSelectedPrinterChanged;
					_view.EditPrinterProperties += EhEditPrinterProperties;
				}
			}
		}

		public object ModelObject
		{
			get { throw new NotImplementedException(); }
		}

		public bool Apply()
		{
			return true;
		}

		#endregion
	}
}
