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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using Altaxo.Drawing;

namespace Altaxo.Drawing.LineCaps
{
  /// <summary>
  /// Designates a flat line cap (i.e. no cap at all). If possible, use the static <see cref="Instance"/> function to get a flat cap.
  /// </summary>
  /// <seealso cref="Altaxo.Drawing.ILineCap" />
  public class FlatCap : ILineCap
  {
    public static FlatCap Instance { get; } = new FlatCap();

    public double MinimumAbsoluteSizePt => 0;

    public double MinimumRelativeSize => 0;

    public string Name => "Flat";

    public bool Equals(ILineCap other)
    {
      return other is FlatCap;
    }

    public ILineCap WithMinimumAbsoluteAndRelativeSize(double minimumAbsoluteSizePt, double minimumRelativeSize)
    {
      return Instance;
    }
  }
}
