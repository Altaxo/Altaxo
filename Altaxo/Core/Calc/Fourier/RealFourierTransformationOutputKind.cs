#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.Fourier
{
  /// <summary>
  /// Designates different possible kinds of output of the real fourier transformation.
  /// </summary>
  public enum RealFourierTransformationOutputKind
  {
    /// <summary>Output is the real part of the fourier transformation.</summary>
    RealPart,

    /// <summary>Output is the imaginary part of the fourier transformation.</summary>
    ImaginaryPart,

    /// <summary>Output is the amplitude of the fourier transformation.</summary>
    Amplitude,

    /// <summary>Output is the phase (-Pi..+Pi) of the fourier transformation.</summary>
    Phase,

    /// <summary>Output is the power (square of the amplitude) of the fourier transformation.</summary>
    Power,
  }
}
