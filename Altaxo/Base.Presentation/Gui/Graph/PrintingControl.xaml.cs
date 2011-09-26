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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Management;

using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for PrintingControl.xaml
	/// </summary>
	public partial class PrintingControl : UserControl, IPrintingView
	{
		public event Action SelectedPrinterChanged;
		public event Action EditPrinterProperties;
		public event Action<bool> PaperOrientationLandscapeChanged;
		public event Action PaperSizeChanged;
		public event Action PaperSourceChanged;
		public event Action<double> MarginLeftChanged;
		public event Action<double> MarginRightChanged;
		public event Action<double> MarginTopChanged;
		public event Action<double> MarginBottomChanged;
		public event Action<int> NumberOfCopiesChanged;
		public event Action<bool> CollateCopiesChanged;


		GdiToWpfBitmap _previewBitmap;
		System.Drawing.Printing.PreviewPageInfo[] _previewData;

		System.Threading.CancellationToken _printerStatusCancellationToken;
		System.Threading.CancellationTokenSource _printerStatusCancellationTokenSource;
		string _printerName;

		/// <summary>Number of the page that is currently previewed.</summary>
		int _previewPageNumber;

		public PrintingControl()
		{
			InitializeComponent();
			_guiMarginLeft.UnitEnvironment = Altaxo.Gui.PaperMarginEnvironment.Instance;
			_guiMarginRight.UnitEnvironment = Altaxo.Gui.PaperMarginEnvironment.Instance;
			_guiMarginTop.UnitEnvironment = Altaxo.Gui.PaperMarginEnvironment.Instance;
			_guiMarginBottom.UnitEnvironment = Altaxo.Gui.PaperMarginEnvironment.Instance;
		}


		private void EhLoaded(object sender, RoutedEventArgs e)
		{
			this._printerStatusCancellationTokenSource = new System.Threading.CancellationTokenSource();
			this._printerStatusCancellationToken = _printerStatusCancellationTokenSource.Token;

			System.Threading.Tasks.Task.Factory.StartNew(UpdatePrinterStatusGuiElements, _printerStatusCancellationToken);
		}

		private void EhUnloaded(object sender, RoutedEventArgs e)
		{
			_printerStatusCancellationTokenSource.Cancel();
		}




		#region IPrintingView

		public void InitializeAvailablePrinters(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbAvailablePrinters, list);
		}

		public void InitializeDocumentPrintOptionsView(object view)
		{
			_documentPrintOptionsViewHost.Content = view as UIElement;
		}

		public void InitializeAvailablePaperSizes(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiPaperSize, list);
		}

		public void InitializeAvailablePaperSources(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiPaperSource, list);
		}

		public void InitializePaperOrientationLandscape(bool isLandScape)
		{
			_guiPaperOrientationLandscape.IsChecked = isLandScape;
			_guiPaperOrientationPortrait.IsChecked = !isLandScape;
		}

		public void InitializePaperMarginsInHundrethInch(double left, double right, double top, double bottom)
		{
			_guiMarginLeft.SelectedQuantity = new Science.DimensionfulQuantity(left, Science.SIPrefix.Centi, Science.LengthUnitInch.Instance).AsQuantityIn(_guiMarginLeft.UnitEnvironment.DefaultUnit);
			_guiMarginRight.SelectedQuantity = new Science.DimensionfulQuantity(right, Science.SIPrefix.Centi, Science.LengthUnitInch.Instance).AsQuantityIn(_guiMarginRight.UnitEnvironment.DefaultUnit);
			_guiMarginTop.SelectedQuantity = new Science.DimensionfulQuantity(top, Science.SIPrefix.Centi, Science.LengthUnitInch.Instance).AsQuantityIn(_guiMarginTop.UnitEnvironment.DefaultUnit);
			_guiMarginBottom.SelectedQuantity = new Science.DimensionfulQuantity(bottom, Science.SIPrefix.Centi, Science.LengthUnitInch.Instance).AsQuantityIn(_guiMarginBottom.UnitEnvironment.DefaultUnit);
		}

		public void InitializeNumberOfCopies(int val)
		{
			_guiNumberOfCopies.Value = val;
		}

		public void InitializeCollateCopies(bool val)
		{
			_guiCollateCopies.IsChecked = val;
		}

		public void InitializePrintPreview(System.Drawing.Printing.PreviewPageInfo[] preview)
		{
			_previewData = preview;
			UpdatePreviewPageAndText();
		}


		private void UpdatePreviewPageAndText()
		{
			if (null == _previewData)
			{
				_edPreviewPageOfPages.Content = null;
			}
			else
			{
				_previewPageNumber = Math.Max(0, Math.Min(_previewPageNumber, _previewData.Length - 1));
				string txt = string.Format("{0} of {1}", _previewPageNumber + 1, _previewData.Length);
				_edPreviewPageOfPages.Content = txt;
				UpdatePreview();
			}
		}

		void UpdatePreview()
		{
			if (null == _previewBitmap || null == _previewData || _previewData.Length == 0)
				return;

			// original sizes
			double ow = _previewData[0].PhysicalSize.Width;
			double oh = _previewData[0].PhysicalSize.Height;

			// destination sizes and locations
			double dl = Math.Min(4, _previewBitmap.GdiRectangle.Width / 8.0); // left
			double dt = Math.Min(4, _previewBitmap.GdiRectangle.Height / 8.0); // top
			double dw = _previewBitmap.GdiRectangle.Width - 2 * dl; // width
			double dh = _previewBitmap.GdiRectangle.Height - 2 * dt; // height




			System.Drawing.Rectangle destRect;

			if (oh / ow > dh / dw) // if the original image is more portrait than the destination rectangle
			{
				// use the full height, but restrict the destination with
				var rw = dh * ow / oh;
				destRect = new System.Drawing.Rectangle((int)(0.5 * (dw - rw) + dl), (int)dt, (int)rw, (int)dh);
			}
			else // if the original image is more landscape than the destination rectangle
			{
				var rh = dw * oh / ow;
				destRect = new System.Drawing.Rectangle((int)dl, (int)(0.5 * (dh - rh) + dt), (int)dw, (int)rh);
			}

			_previewBitmap.GdiGraphics.FillRectangle(System.Drawing.Brushes.White, _previewBitmap.GdiRectangle);
			_previewBitmap.GdiGraphics.DrawRectangle(System.Drawing.Pens.Black, destRect);

			_previewPageNumber = Math.Max(0, Math.Min(_previewPageNumber, _previewData.Length - 1));
			_previewBitmap.GdiGraphics.DrawImage(_previewData[_previewPageNumber].Image, destRect);
			_previewBitmap.WpfBitmap.Invalidate();

		}


		void UpdatePrinterStatusGuiElements()
		{
			for (; ; )
			{
				if (_printerStatusCancellationToken.IsCancellationRequested)
					break;

				string printerName = _printerName;
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
				ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
				ManagementObjectCollection coll = searcher.Get();
				foreach (ManagementObject printer in coll)
				{
					//Console.WriteLine("------ Printer: {0} ---------", printer.Path);
					//Console.WriteLine("Property Status = {0}", printer.GetPropertyValue("Status"));

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

				Current.Gui.Execute(() =>
					{
						this._guiPrinterStatus.Content = status;
						this._guiPrinterComment.Content = comment;
						this._guiPrinterLocation.Content = location;
					});
				System.Threading.Thread.Sleep(100);
			}
		}


		#endregion

		#region Interop for printer properties

		class UnmanagedPrinterPropertiesDialogHelper
		{
			// see http://www.pinvoke.net/default.aspx/winspool/documentproperties.html

			public const int DM_UPDATE = 1;
			public const int DM_COPY = 2;
			public const int DM_PROMPT = 4;
			public const int DM_MODIFY = 8;

			public const int DM_IN_BUFFER = DM_MODIFY;
			public const int DM_IN_PROMPT = DM_PROMPT;
			public const int DM_OUT_BUFFER = DM_COPY;
			public const int DM_OUT_DEFAULT = DM_UPDATE;

			[DllImport("winspool.Drv", EntryPoint = "DocumentPropertiesW", SetLastError = true,
				 ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
			static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter,
							[MarshalAs(UnmanagedType.LPWStr)] string pDeviceName,
							IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode);

			[DllImport("kernel32.dll")]
			static extern IntPtr GlobalLock(IntPtr hMem);
			[DllImport("kernel32.dll")]
			static extern bool GlobalUnlock(IntPtr hMem);
			[DllImport("kernel32.dll")]
			static extern bool GlobalFree(IntPtr hMem);


			public static void OpenPrinterPropertiesDialog(PrinterSettings printerSettings, IntPtr handle)
			{
				IntPtr hDevMode = printerSettings.GetHdevmode(printerSettings.DefaultPageSettings);
				IntPtr pDevMode = GlobalLock(hDevMode);
				int sizeNeeded = DocumentProperties(handle, IntPtr.Zero, printerSettings.PrinterName, IntPtr.Zero, pDevMode, 0);
				if (sizeNeeded < 0)
				{
					GlobalUnlock(hDevMode);
					return;
				}
				IntPtr devModeData = Marshal.AllocHGlobal(sizeNeeded);
				DocumentProperties(handle, IntPtr.Zero, printerSettings.PrinterName, devModeData, pDevMode, DM_IN_BUFFER | DM_OUT_BUFFER | DM_PROMPT);
				GlobalUnlock(hDevMode);
				printerSettings.SetHdevmode(devModeData);
				printerSettings.DefaultPageSettings.SetHdevmode(devModeData);
				GlobalFree(hDevMode);
				Marshal.FreeHGlobal(devModeData);
			}
		}

		public void ShowPrinterPropertiesDialog(PrinterSettings psSettings)
		{
			IntPtr handle = Current.Gui.MainWindowHandle;
			UnmanagedPrinterPropertiesDialogHelper.OpenPrinterPropertiesDialog(psSettings, handle);
		}



		#endregion

		private void EhShowPrinterProperties(object sender, RoutedEventArgs e)
		{
			if (null != EditPrinterProperties)
				EditPrinterProperties();
		}

		private void EhPrinterSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbAvailablePrinters);
			if (null != SelectedPrinterChanged)
				SelectedPrinterChanged();

			var node = (Collections.SelectableListNode)_cbAvailablePrinters.SelectedItem;

			_printerName = node.Text;

			//UpdatePrinterStatusGuiElements(printerName);


		}



		private void EhPreviewImageSizeChanged(object sender, SizeChangedEventArgs e)
		{
			_previewImage.Width = e.NewSize.Width;
			_previewImage.Height = e.NewSize.Height;
			if (null == _previewBitmap)
			{
				_previewBitmap = new GdiToWpfBitmap((int)e.NewSize.Width, (int)e.NewSize.Height);
				_previewImage.Source = _previewBitmap.WpfBitmap;
			}
			else
			{
				_previewBitmap.Resize((int)e.NewSize.Width, (int)e.NewSize.Height);
				_previewImage.Source = _previewBitmap.WpfBitmap;
			}
			UpdatePreview();
		}





		private void EhPreviewFirstPage(object sender, RoutedEventArgs e)
		{
			_previewPageNumber = 0;
			UpdatePreviewPageAndText();
		}

		private void EhPreviewPreviousPage(object sender, RoutedEventArgs e)
		{
			_previewPageNumber = Math.Max(0, _previewPageNumber - 1);
			UpdatePreviewPageAndText();

		}

		private void EhPreviewNextPage(object sender, RoutedEventArgs e)
		{
			_previewPageNumber = Math.Min(_previewPageNumber + 1, null != _previewData ? _previewData.Length - 1 : 0);
			UpdatePreviewPageAndText();
		}

		private void EhPreviewLastPage(object sender, RoutedEventArgs e)
		{
			_previewPageNumber = null != _previewData ? _previewData.Length - 1 : 0;
			UpdatePreviewPageAndText();
		}

		private void EhPaperOrientationPortrait(object sender, RoutedEventArgs e)
		{
			if (null != PaperOrientationLandscapeChanged)
				PaperOrientationLandscapeChanged(false);
		}

		private void EhPaperOrientationLandscape(object sender, RoutedEventArgs e)
		{
			if (null != PaperOrientationLandscapeChanged)
				PaperOrientationLandscapeChanged(true);
		}

		private void EhPaperSizeChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiPaperSize);
			if (null != PaperSizeChanged)
			{
				PaperSizeChanged();
			}
		}

		private void EhPaperSourceChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiPaperSource);
			if (null != PaperSourceChanged)
			{
				PaperSourceChanged();
			}
		}

		private void EhMarginLeftChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var val = _guiMarginLeft.SelectedQuantity.AsQuantityIn(Science.SIPrefix.Centi, Science.LengthUnitInch.Instance).Value;
			if (null != MarginLeftChanged)
				MarginLeftChanged(val);
		}

		private void EhMarginRightChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var val = _guiMarginRight.SelectedQuantity.AsQuantityIn(Science.SIPrefix.Centi, Science.LengthUnitInch.Instance).Value;
			if (null != MarginRightChanged)
				MarginRightChanged(val);

		}

		private void EhMarginTopChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var val = _guiMarginTop.SelectedQuantity.AsQuantityIn(Science.SIPrefix.Centi, Science.LengthUnitInch.Instance).Value;
			if (null != MarginTopChanged)
				MarginTopChanged(val);

		}

		private void EhMarginBottomChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var val = _guiMarginBottom.SelectedQuantity.AsQuantityIn(Science.SIPrefix.Centi, Science.LengthUnitInch.Instance).Value;
			if (null != MarginBottomChanged)
				MarginBottomChanged(val);

		}

		private void EhNoOfCopiesChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (null != NumberOfCopiesChanged)
				NumberOfCopiesChanged(_guiNumberOfCopies.Value);
		}

		private void EhCollateCopiesChanged(object sender, RoutedEventArgs e)
		{
			if (null != CollateCopiesChanged)
				CollateCopiesChanged(_guiCollateCopies.IsChecked == true);

		}







	}
}

