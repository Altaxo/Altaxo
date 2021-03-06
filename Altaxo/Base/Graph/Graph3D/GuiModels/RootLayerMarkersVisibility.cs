﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Graph3D.GuiModels
{
  /// <summary>
  /// Designates the visibility of root layer markers.
  /// </summary>
  [Flags]
  public enum RootLayerMarkersVisibility
  {
    /// <summary>Definitely not show root layer markers.</summary>
    None = 0,

    /// <summary>Definitely not show root layer markers.</summary>
    Arrows = 1,

    /// <summary>Show root layer markers as lines.</summary>
    Lines = 2,

    /// <summary>Show root layer markers as lines with arrows.</summary>
    LinesWithArrows = 3
  }
}
