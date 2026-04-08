#region Copyright

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Altaxo.Gui.Graph.Plot.Data
{
  /// <summary>
  /// Provides default colors for different severity levels.
  /// </summary>
  public static class DefaultSeverityColumnColors
  {
    /// <summary>
    /// Gets or sets the brush used for normal severity.
    /// </summary>
    public static Brush NormalColor { get; set; } = Brushes.White;

    /// <summary>
    /// Gets or sets the brush used for warning severity.
    /// </summary>
    public static Brush WarningColor { get; set; } = Brushes.Yellow;

    /// <summary>
    /// Gets or sets the brush used for error severity.
    /// </summary>
    public static Brush ErrorColor { get; set; } = Brushes.LightPink;

    /// <summary>
    /// Gets the brush for the specified severity.
    /// </summary>
    /// <param name="severity">The severity level.</param>
    /// <returns>The corresponding brush.</returns>
    public static Brush GetSeverityColor(int severity)
    {
      switch (severity)
      {
        case 0:
          return NormalColor;

        case 1:
          return WarningColor;

        case 2:
          return ErrorColor;
      }

      return NormalColor;
    }
  }
}
