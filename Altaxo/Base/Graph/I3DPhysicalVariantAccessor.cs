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
using System.Text;
using Altaxo.Data;

namespace Altaxo.Graph
{
  /// <summary>
  /// Provides indexed access to physical x, y, and z values.
  /// </summary>
  public interface I3DPhysicalVariantAccessor
  {
    /// <summary>
    /// Gets the physical x value for the specified original row index.
    /// </summary>
    /// <param name="originalRowIndex">The original row index.</param>
    /// <returns>The physical x value.</returns>
    AltaxoVariant GetXPhysical(int originalRowIndex);

    /// <summary>
    /// Gets the physical y value for the specified original row index.
    /// </summary>
    /// <param name="originalRowIndex">The original row index.</param>
    /// <returns>The physical y value.</returns>
    AltaxoVariant GetYPhysical(int originalRowIndex);

    /// <summary>
    /// Gets the physical z value for the specified original row index.
    /// </summary>
    /// <param name="originalRowIndex">The original row index.</param>
    /// <returns>The physical z value.</returns>
    AltaxoVariant GetZPhysical(int originalRowIndex);
  }

  /// <summary>
  /// Represents a delegate that returns a physical value for a given index.
  /// </summary>
  /// <param name="i">The index.</param>
  /// <returns>The physical value.</returns>
  public delegate AltaxoVariant IndexedPhysicalValueAccessor(int i);
}
