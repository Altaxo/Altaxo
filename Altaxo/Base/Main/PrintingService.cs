#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
    
    
    /// <summary>
    /// Returns the current printing page setup dialog for this instance of the application.
    /// </summary>
    System.Windows.Forms.PageSetupDialog PageSetupDialog  { get; }

    /// <summary>
    /// Returns the print dialog for the current instance of this application.
    /// </summary>
    System.Windows.Forms.PrintDialog PrintDialog { get; }
  }


  /// <summary>
  /// Summary description for PrintingService.
  /// </summary>
  public class PrintingService :  ICSharpCode.Core.Services.AbstractService, IPrintingService
  {
  
    private System.Windows.Forms.PageSetupDialog m_PageSetupDialog;

    private System.Drawing.Printing.PrintDocument m_PrintDocument;

    private System.Windows.Forms.PrintDialog m_PrintDialog;

    public PrintingService()
    {
  

      // we initialize the printer variables
      m_PrintDocument = new System.Drawing.Printing.PrintDocument();
      // we set the print document default orientation to landscape
      m_PrintDocument.DefaultPageSettings.Landscape=true;
      m_PageSetupDialog = new System.Windows.Forms.PageSetupDialog();
      m_PageSetupDialog.Document = m_PrintDocument;
      m_PrintDialog = new System.Windows.Forms.PrintDialog();
      m_PrintDialog.Document = m_PrintDocument;
    }

    #region Properties


    

    public  System.Windows.Forms.PageSetupDialog PageSetupDialog
    {
      get { return m_PageSetupDialog; }
    }

    public  System.Drawing.Printing.PrintDocument PrintDocument
    {
      get { return m_PrintDocument; }
    }


    public System.Windows.Forms.PrintDialog PrintDialog
    {
      get { return m_PrintDialog; }
    }


  

    #endregion


  }
}
