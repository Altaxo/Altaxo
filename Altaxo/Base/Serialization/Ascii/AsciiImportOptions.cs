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

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Denotes options about how to import data from an ascii text file.
  /// </summary>
  public class AsciiImportOptions
  {
    /// <summary>If true, rename the columns if 1st line contain  the column names.</summary>
    public bool RenameColumns; 
    /// <summary>If true, rename the worksheet to the data file name.</summary>
    public bool RenameWorksheet;

    /// <summary>Number of lines to skip (the main header).</summary>
    public int NumberOfMainHeaderLines;


    /// <summary>Number of dots that act as decimal separator in numeric strings.</summary>
    public int DecimalSeparatorDotCount;
    /// <summary>Number of commas that act as decimal separator in numeric strings.</summary>
    public int DecimalSeparatorCommaCount;

    /// <summary>Method to separate the tokens in each line of ascii text.</summary>
    public IAsciiSeparationStrategy SeparationStrategy;      

    /// <summary>Structur of the file (which data type is placed in which column).</summary>
    public AsciiLineStructure RecognizedStructure;




    /// <summary>Clones the options. This is a shallow copy, so the class members are not cloned.</summary>
    public AsciiImportOptions Clone()
    {
      return (AsciiImportOptions)MemberwiseClone();
    }
  }
}
