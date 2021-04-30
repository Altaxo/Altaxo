#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Threading.Tasks;

namespace Altaxo.Gui.Common.PropertyGrid
{
  /// <summary>
  /// Designates a property in the Gui system.
  /// </summary>
  public interface ICategoryNameView
  {
    /// <summary>
    /// Gets the category the property belongs to.
    /// </summary>
    string Category { get; }
    /// <summary>
    /// Gets the name of the property.
    /// </summary>
   
    string Name { get; }
    /// <summary>
    /// Gets the view (Gui object) that allows to show or edit the property value.
    /// </summary>
    object View { get; }
  }
}
