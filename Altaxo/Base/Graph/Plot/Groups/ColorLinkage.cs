#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// Designates the usage of a color in plot styles.
  /// </summary>
  public enum ColorLinkage
  {
    /// <summary>
    /// Fully dependent color. Must be a member of a plot color set. Act both as color provider and color receiver.
    /// </summary>
    Dependent = 0,

    /// <summary>
    /// Fully independent color. Is neither a provider of the color nor a receiver.
    /// </summary>
    Independent = 1,

    /// <summary>
    /// Dependent color. Can not be a provider. When receiving the color from other providers, the alpha value of the original color is preserved.
    /// This means that therefrom resulting color is probably not a plot color, and has no parent color set (this is the reason that the color can not act as provider).
    /// </summary>
    PreserveAlpha
  }
}
