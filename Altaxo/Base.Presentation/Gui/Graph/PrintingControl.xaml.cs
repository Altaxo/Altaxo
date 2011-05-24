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

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for PrintingControl.xaml
	/// </summary>
	public partial class PrintingControl : UserControl, IPrintingView
	{
		public event Action SelectedPrinterChanged;
		public event Action EditPrinterProperties;

		public PrintingControl()
		{
			InitializeComponent();
		}

	
		#region IPrintingView

		public void InitializeAvailablePrinters(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbAvailablePrinters, list);
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
	}
}
