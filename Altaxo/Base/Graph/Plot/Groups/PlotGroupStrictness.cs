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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// Enumerates the strictness of the coupling between plot items into a plot group.
  /// </summary>
  public enum PlotGroupStrictness
  {
    /// <summary>
    /// Only the properties are coupled by means of the plot group styles, like color and symbols.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// If the plot styles have the same substyles (for instance both have scatter styles), then the style's properties
    /// are set to the same values before the plot groups are applied.
    /// </summary>
    Exact = 1,

    /// <summary>
    /// The style of the master item is copyied exactly to the style of all other items in the plot group (including all substyles). Then
    /// the plot groups are applied.
    /// </summary>
    Strict = 2
  }
}
