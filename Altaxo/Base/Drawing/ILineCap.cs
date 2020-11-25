#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using Altaxo.Graph.Gdi.LineCaps;

namespace Altaxo.Drawing
{
  /// <summary>
  /// Interface to a start or end line cap of a line.
  /// </summary>
  public interface ILineCap : IEquatable<ILineCap>, Main.IImmutable
  {
    /// <summary>
    /// Gets the minimum absolute size of the line cap in points (1/72 inch).
    /// </summary>
    /// <value>
    /// The minimum absolute size of the line cap in points (1/72 inch).
    /// </value>
    double MinimumAbsoluteSizePt { get; }

    /// <summary>
    /// Gets the minimum relative size (relative to the line thickness) of the line cap. Example: if this value is 2, and the line thickness is 10 points, then the line cap size is 20 points.
    /// </summary>
    /// <value>
    /// The minimum relative size of the line cap.
    /// </value>
    double MinimumRelativeSize { get; }

    /// <summary>
    /// Gets the name of the line cap.
    /// </summary>
    /// <value>
    /// The name of the line cap.
    /// </value>
    string Name { get; }

    /// <summary>
    /// Gets a new instance of the line cap with the designated minimum absolute and relative sizes. Note that not all line cap types support one or both values; in this case, those values are ignored.
    /// </summary>
    /// <param name="minimumAbsoluteSizePt">The minimum absolute size pt.</param>
    /// <param name="minimumRelativeSize">Minimum size of the relative.</param>
    /// <returns>A new instance of the line cap with the designated minimum absolute and relative sizes.</returns>
    ILineCap WithMinimumAbsoluteAndRelativeSize(double minimumAbsoluteSizePt, double minimumRelativeSize);
  }
}
