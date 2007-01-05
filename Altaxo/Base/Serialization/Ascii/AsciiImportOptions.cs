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
  public class AsciiImportOptions
  {
    public bool bRenameColumns; /// rename the columns if 1st line contain  the column names
    public bool bRenameWorksheet; // rename the worksheet to the data file name

    public int nMainHeaderLines; // lines to skip (the main header)
    public bool bDelimited;      // true if delimited by a single char
    public char cDelimiter;      // the delimiter char

    public int m_DecimalSeparatorDotCount=0;
    public int m_DecimalSeparatorCommaCount=0;


    public AsciiLineStructure recognizedStructure=null;




    public AsciiImportOptions Clone()
    {
      return (AsciiImportOptions)MemberwiseClone();
    }

  }


}
