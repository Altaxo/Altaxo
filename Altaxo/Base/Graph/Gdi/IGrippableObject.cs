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
using System.Drawing;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Provides an interface for all objects that are grippable, i.e. that show special areas by which
  /// those objects can be manipulated.
  /// </summary>
  public interface IGrippableObject
  {

    /// <summary>
    /// Shows the grips, i.e. the special areas for manipulation of the object.
    /// </summary>
    /// <param name="g">The graphic context.</param>
		/// <returns>Grip manipulation handles that are used to show the grips and to manipulate the object.</returns>
    IGripManipulationHandle[] ShowGrips(System.Drawing.Graphics g);
  }
}
