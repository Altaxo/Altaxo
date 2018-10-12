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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Text
{
  /// <summary>
  /// Designates an error during processing of a Markdown file.
  /// </summary>
  public class MarkdownError
  {
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the line number.
    /// </summary>
    public int LineNumber { get; set; }

    public int ColumnNumber { get; set; }

    /// <summary>
    /// Gets or sets the (text) document name in which the error occured.
    /// </summary>
    public string AltaxoDocumentName { get; set; }

    public override string ToString()
    {
      var stb = new StringBuilder();

      if (LineNumber != 0)
      {
        stb.AppendFormat("Line {0}, Column {1}", LineNumber, ColumnNumber);
      }

      if (!string.IsNullOrEmpty(AltaxoDocumentName))
      {
        if (stb.Length != 0)
        {
          stb.Append(" ");
        }
        stb.Append(AltaxoDocumentName);

      }

      if (!string.IsNullOrEmpty(ErrorMessage))
      {
        if (stb.Length != 0)
        {
          stb.Append(": ");
        }
        stb.Append(ErrorMessage);

      }
      return stb.ToString();
    }
  }
}
