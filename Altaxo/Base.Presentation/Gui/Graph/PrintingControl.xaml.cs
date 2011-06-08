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
		public event Action ShowPrintPreview;

		GdiToWpfBitmap _previewBitmap;
		System.Drawing.Printing.PreviewPageInfo[] _previewData;

		public PrintingControl()
		{
			InitializeComponent();
		}


		#region IPrintingView

		public void InitializeAvailablePrinters(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbAvailablePrinters, list);
		}

		public void InitializeDocumentPrintOptionsView(object view)
		{
			_documentPrintOptionsViewHost.Child = view as UIElement;
		}

		public void SetPrintPreview(System.Drawing.Printing.PreviewPageInfo[] preview)
		{
			_previewData = preview;
			UpdatePreview();
		}

		void UpdatePreview()
		{
			if (null == _previewBitmap || null==_previewData || _previewData.Length == 0)
				return;

			double ow = _previewData[0].PhysicalSize.Width;
			double oh = _previewData[0].PhysicalSize.Height;

			double dw = _previewBitmap.GdiRectangle.Width;
			double dh = _previewBitmap.GdiRectangle.Height;




			System.Drawing.Rectangle destRect;

			if (oh / ow > dh / dw) // if the original image is more portrait than the destination rectangle
			{
				// use the full height, but restrict the destination with
				var rw = dh * ow / oh;
				destRect = new System.Drawing.Rectangle((int)(0.5 * (dw - rw)), 0, (int)rw, (int)dh);
			}
			else // if the original image is more landscape than the destination rectangle
			{
				var rh = dw * oh / ow;
				destRect = new System.Drawing.Rectangle(0, (int)(0.5*(dh - rh)), (int)dw, (int)rh);
			}

			_previewBitmap.GdiGraphics.FillRectangle(System.Drawing.Brushes.White, _previewBitmap.GdiRectangle);
			_previewBitmap.GdiGraphics.DrawRectangle(System.Drawing.Pens.Black, destRect);
			_previewBitmap.GdiGraphics.DrawImage(_previewData[0].Image, destRect);
				_previewBitmap.WpfBitmap.Invalidate();

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
		}

		private void EhShowPrintPreview(object sender, RoutedEventArgs e)
		{
			if (null != ShowPrintPreview)
				ShowPrintPreview();
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


		
	}
}

