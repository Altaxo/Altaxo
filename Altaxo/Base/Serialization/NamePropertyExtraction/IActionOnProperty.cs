#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

namespace Altaxo.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Defines an interface for actions that operate on a specific property. Implementing classes must provide the name of the property they act upon.
  /// </summary>
  public interface IActionOnProperty : Main.IImmutable
  {
    /// <summary>
    /// Gets the name of the property that the action operates on.
    /// This property is required and must be initialized when implementing classes are instantiated.
    /// </summary>
    public string PropertyName { get; init; }
    /// <summary>
    /// Gets or sets the description of the action.
    /// </summary>
    public string Description { get; }
  }
}
