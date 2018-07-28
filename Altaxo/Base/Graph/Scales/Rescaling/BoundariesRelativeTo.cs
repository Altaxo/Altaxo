#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Scales.Rescaling
{
  /// <summary>
  /// Designates what the user provided boundaries represent.
  /// </summary>
  public enum BoundariesRelativeTo
  {
    /// <summary>
    /// User provided boundary value is absolute.
    /// </summary>
    Absolute = 0,

    /// <summary>
    /// User provided boundary is an offset relative to the origin of the data bounds.
    /// Example: the user provided value is 200, the origin of the data bounds is 1000, thus the resulting boundary value is 1200.
    /// </summary>
    RelativeToDataBoundsOrg = 1,

    /// <summary>
    /// User provided boundary is an offset relative to the end of the data bounds
    /// Example: the user provided value is 200, the end of the data bounds is 2000, thus the resulting boundary value is 2200.
    /// </summary>
    RelativeToDataBoundsEnd = 2,

    /// <summary>
    /// User provided boundary is an offset relative to the mean of the data bounds. Interpretation of 'mean' depends on the scale: it is the physical value at that point on the scale where the logical value is 0.5.
    /// Thus on a linear scale it is the arithmetic mean of org and end.
    /// Example: on a linear scale the user provided value is 200, the org of the data bounds is 1000, the end of the data bounds is 2000, thus the resulting boundary value is 1500 + 200 = 1700.
    /// </summary>
    RelativeToDataBoundsMean = 3
  }
}
