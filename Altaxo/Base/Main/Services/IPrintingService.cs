#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main.Services
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
    System.Drawing.Printing.PrintDocument PrintDocument { get; }

    System.Drawing.Printing.Margins PrintingMargins { get; }

    System.Drawing.RectangleF PrintingBounds { get; }

    /// <summary>
    /// Update the default bounds and margins after the printer settings changed.
    /// This function must be called manually after a page setup dialog.
    /// </summary>
    void UpdateBoundsAndMargins();
  }
}
