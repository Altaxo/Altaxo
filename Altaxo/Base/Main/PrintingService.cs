#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

namespace Altaxo.Main
{

  /// <summary>
  /// This interface provides access to printers and forms.
  /// </summary>
  public interface IPrintingService 
  {
    /// <summary>
    /// Returns the current print document for this instance of the application.
    /// This contains settings that store the current printer, paper size, orientation and so on.
    /// </summary>
    System.Drawing.Printing.PrintDocument PrintDocument { get ; }

    System.Drawing.Printing.Margins PrintingMargins { get; }

    System.Drawing.RectangleF PrintingBounds { get; }

        /// <summary>
    /// Update the default bounds and margins after the printer settings changed.
    /// This function must be called manually after a page setup dialog.
    /// </summary>
    void UpdateBoundsAndMargins();

  
  }


  /// <summary>
  /// Summary description for PrintingService.
  /// </summary>
  public class PrintingService :  IPrintingService
  {
    private System.Drawing.Printing.PrintDocument _printDocument;
    System.Drawing.Printing.PrinterSettings _printerSettings;
    System.Drawing.Printing.Margins _printerMargins;
    System.Drawing.RectangleF _printerPageBounds;

    public PrintingService()
    {
      // we initialize the printer variables
      _printDocument = new System.Drawing.Printing.PrintDocument();
      // we set the print document default orientation to landscape
      _printDocument.DefaultPageSettings.Landscape=true;
      Current.Gui.SetPrintDocument(_printDocument);
      _printerSettings = new System.Drawing.Printing.PrinterSettings();
      UpdateBoundsAndMargins();
    }

    /// <summary>
    /// Update the default bounds and margins after the printer settings changed.
    /// This function must be called manually after a page setup dialog.
    /// </summary>
    public void UpdateBoundsAndMargins()
    {
      if (_printerSettings.IsValid)
      {
        _printerPageBounds = _printDocument.DefaultPageSettings.Bounds;
        _printerMargins = _printDocument.DefaultPageSettings.Margins;
      }
      else // obviously no printer installed, use A4 size (sorry, this is european size)
      {
        _printerPageBounds = new System.Drawing.RectangleF(0, 0, 1169, 826);
        _printerMargins = new System.Drawing.Printing.Margins(50, 50, 50, 50);
      }

    }

   

    #region Properties

    public System.Drawing.Printing.Margins PrintingMargins 
    {
      get { return _printerMargins; }
    }

    public System.Drawing.RectangleF PrintingBounds 
    {
      get { return _printerPageBounds; }
    }


    public  System.Drawing.Printing.PrintDocument PrintDocument
    {
      get { return _printDocument; }
    }

    #endregion


  }
}
