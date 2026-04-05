#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Altaxo.Drawing.LineCaps
{
  /// <summary>
  /// Draws an open circular line cap whose center lies at the line end.
  /// </summary>
  public class CircleOLineCap : LineCapBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CircleOLineCap"/> class.
    /// </summary>
    public CircleOLineCap()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CircleOLineCap"/> class.
    /// </summary>
    /// <param name="minimumAbsoluteSizePt">The minimum absolute size in points.</param>
    /// <param name="minimumRelativeSize">The minimum relative size.</param>
    public CircleOLineCap(double minimumAbsoluteSizePt, double minimumRelativeSize)
      : base(minimumAbsoluteSizePt, minimumRelativeSize)
    {
    }

    /// <inheritdoc/>
    public override string Name { get { return "CircleO"; } }

    /// <inheritdoc/>
    public override double DefaultMinimumAbsoluteSizePt { get { return 8; } }

    /// <inheritdoc/>
    public override double DefaultMinimumRelativeSize { get { return 4; } }
  }

  /// <summary>
  /// Draws a filled circular line cap whose center lies at the line end.
  /// </summary>
  public class CircleFLineCap : LineCapBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CircleFLineCap"/> class.
    /// </summary>
    public CircleFLineCap()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CircleFLineCap"/> class.
    /// </summary>
    /// <param name="minimumAbsoluteSizePt">The minimum absolute size in points.</param>
    /// <param name="minimumRelativeSize">The minimum relative size.</param>
    public CircleFLineCap(double minimumAbsoluteSizePt, double minimumRelativeSize)
      : base(minimumAbsoluteSizePt, minimumRelativeSize)
    {
    }

    /// <inheritdoc/>
    public override string Name { get { return "CircleF"; } }

    /// <inheritdoc/>
    public override double DefaultMinimumAbsoluteSizePt { get { return 8; } }

    /// <inheritdoc/>
    public override double DefaultMinimumRelativeSize
    {
      get { return 4; }
    }
  }
}
