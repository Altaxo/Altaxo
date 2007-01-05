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

namespace Altaxo.Gui
{
  /// <summary>
  /// Implemented by class attributes, where one class (that has the attribute assigned to) is responsible for another type of class (the destination class type).
  /// </summary>
  public interface IClassForClassAttribute
  {

    /// <summary>
    /// The destination class type.
    /// </summary>
    System.Type TargetType { get; }

    /// <summary>
    /// The priority. Attributes with higher priority are preferred over such attributes with lower priority.
    /// </summary>
    int Priority { get; }
  }
}
