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

#endregion Copyright

#nullable enable
namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Designates what to do with the main header lines of an ASCII file.
  /// </summary>
  public enum AsciiHeaderLinesDestination
  {
    /// <summary>Ignore the main header lines (throw them away).</summary>
    Ignore,

    /// <summary>Try to import the items in the header lines as property columns.</summary>
    ImportToProperties,

    /// <summary>Try to import the items in the header line(s) as properties. If the number of items doesn't match with that of the table, those header line is imported into the notes of the worksheet.</summary>
    ImportToPropertiesOrNotes,

    /// <summary>Store the main header lines as notes in the worksheet.</summary>
    ImportToNotes,

    /// <summary>Try to import the items in the header lines as property columns. Additionally, those lines are added to the notes of the table.</summary>
    ImportToPropertiesAndNotes
  }
}
