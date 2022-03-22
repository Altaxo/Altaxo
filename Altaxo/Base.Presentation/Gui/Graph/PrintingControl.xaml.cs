#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

#nullable disable warnings
using System;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Interaction logic for PrintingControl.xaml
  /// </summary>
  public partial class PrintingControl : UserControl, IPrintingView
  {
    public PrintingControl()
    {
      InitializeComponent();
    }

    #region Interop for printer properties

    private class UnmanagedPrinterPropertiesDialogHelper
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
      private static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter,
              [MarshalAs(UnmanagedType.LPWStr)] string pDeviceName,
              IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode);

      [DllImport("kernel32.dll")]
      private static extern IntPtr GlobalLock(IntPtr hMem);

      [DllImport("kernel32.dll")]
      private static extern bool GlobalUnlock(IntPtr hMem);

      [DllImport("kernel32.dll")]
      private static extern bool GlobalFree(IntPtr hMem);

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

    #endregion Interop for printer properties
  }
}
