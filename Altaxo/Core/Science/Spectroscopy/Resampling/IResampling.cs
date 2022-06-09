#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

namespace Altaxo.Science.Spectroscopy.Resampling
{
  /// <summary>Resamples the spectrum, i.e. sample the spectrum at different x-values, probably equidistant.</summary>
  public interface IResampling
  {
    /// <summary>
    /// Executes the resampler.
    /// </summary>
    /// <param name="x">The x-values of the spectrum.</param>
    /// <param name="y">The y-values of the spectrum.</param>
    /// <returns>X and y-values of the resampled spectrum.</returns>
    (double[] x, double[] y) Execute(double[] x, double[] y);
  }
}
