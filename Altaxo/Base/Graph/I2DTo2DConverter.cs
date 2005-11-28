#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

namespace Altaxo.Graph
{
  /// <summary>
  /// Converts two values into two numerical values, for instance two physical values into a pair of x and y location values.
  /// </summary>
  public interface I2DTo2DConverter
  {
    /// <summary>
    /// Calculates from two physical values the coordinates of the point. Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="x">The physical x value.</param>
    /// <param name="y">The physical y value.</param>
    /// <param name="xout">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="yout">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    bool Convert(double x, double y, out double xout, out double yout);
  
  
    /// <summary>
    /// Fires when anything that influences the conversion has changed. Needed to clear cached values when
    /// the conversion routine has changed.
    /// </summary>
    event EventHandler Changed;
  }



  /// <summary>
  /// Converts two values into two numerical values, for instance two physical values into a pair of x and y location values.
  /// </summary>
  public interface I2DPhysicalTo2DConverter : I2DTo2DConverter
  {
    /// <summary>
    /// Calculates from two physical values the coordinates of the point. Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="x">The physical x value.</param>
    /// <param name="y">The physical y value.</param>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    bool Convert(Altaxo.Data.AltaxoVariant x, Altaxo.Data.AltaxoVariant y, out double xlocation, out double ylocation);
  }



  /// <summary>
  /// Converts two values into two physical values, for instance a x-y coordinate pair into two physical values.
  /// </summary>
  public interface I2DTo2DPhysicalConverter : I2DTo2DConverter
  {
    /// <summary>
    /// Calculates from two physical values the coordinates of the point. Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="y">The y value.</param>
    /// <param name="xlocation">On return, gives the physical x value.</param>
    /// <param name="ylocation">On return, gives the physical y value.</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    bool Convert(double x, double y, out Altaxo.Data.AltaxoVariant xlocation, out Altaxo.Data.AltaxoVariant ylocation);
  }
}
