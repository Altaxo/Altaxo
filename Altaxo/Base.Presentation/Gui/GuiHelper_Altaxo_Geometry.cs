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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using Altaxo.Collections;
using Altaxo.Geometry;

namespace Altaxo.Gui
{
  /// <summary>
  /// Provides helper methods for converting Altaxo geometry types to and from WPF types.
  /// </summary>
  public static partial class GuiHelper
  {
    #region Graphics primitives

    #region Point

    /// <summary>
    /// Converts an Altaxo point to a WPF point.
    /// </summary>
    /// <param name="pt">The Altaxo point.</param>
    /// <returns>The converted WPF point.</returns>
    public static Point ToWpf(this Altaxo.Geometry.PointD2D pt)
    {
      return new Point(pt.X, pt.Y);
    }

    /// <summary>
    /// Converts a WPF point to an Altaxo point.
    /// </summary>
    /// <param name="pt">The WPF point.</param>
    /// <returns>The converted Altaxo point.</returns>
    public static PointD2D ToAltaxo(this Point pt)
    {
      return new PointD2D(pt.X, pt.Y);
    }

    #endregion Point

    #region Rectangle

    /// <summary>
    /// Converts an Altaxo rectangle to a WPF rectangle.
    /// </summary>
    /// <param name="rect">The Altaxo rectangle.</param>
    /// <returns>The converted WPF rectangle.</returns>
    public static Rect ToWpf(this RectangleD2D rect)
    {
      return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
    }

    /// <summary>
    /// Converts a WPF rectangle to an Altaxo rectangle.
    /// </summary>
    /// <param name="rect">The WPF rectangle.</param>
    /// <returns>The converted Altaxo rectangle.</returns>
    public static RectangleD2D ToAltaxo(this Rect rect)
    {
      return new RectangleD2D(rect.X, rect.Y, rect.Width, rect.Height);
    }

    /// <summary>
    /// Converts an Altaxo rectangle to a <see cref="System.Drawing.RectangleF"/>.
    /// </summary>
    /// <param name="rect">The Altaxo rectangle.</param>
    /// <returns>The converted drawing rectangle.</returns>
    public static System.Drawing.RectangleF ToSysDraw(this RectangleD2D rect)
    {
      return new System.Drawing.RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
    }

    #endregion Rectangle

    #endregion Graphics primitives
  }
}
