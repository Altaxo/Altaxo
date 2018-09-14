#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{

  /// <summary>
  /// State equations and constants of mixtures of Methane and Nitrogen.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the mixture file 'methane-nitrogen.mix' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the mixture file):</para>
  /// <para>Info: Kunz and Wagner (2007)</para>
  /// <para>Departure function (MXM): Kunz, O., Klimeck, R., Wagner, W., Jaeschke, M. The GERG-2004 Wide-Range Equation of State for Natural Gases and Other Mixtures. GERG Technical Monograph 15. Fortschr.-Ber. VDI, VDI-Verlag, D�sseldorf, 2007.</para>
  /// </remarks>
  [CASRegistryNumber("74-82-8")]
  [CASRegistryNumber("7727-37-9")]
  public class Mixture_Methane_Nitrogen : BinaryMixtureDefinitionBase
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Mixture_Methane_Nitrogen Instance { get; } = new Mixture_Methane_Nitrogen();

    #region Constants for the binary mixture of Methane and Nitrogen

    /// <summary>Gets the CAS registry number of component 1 (Methane).</summary>
    public override string CASRegistryNumber1 { get; } = "74-82-8";

    /// <summary>Gets the CAS registry number of component 2 (Nitrogen).</summary>
    public override string CASRegistryNumber2 { get; } = "7727-37-9";

    #endregion Constants for the binary mixture of Methane and Nitrogen

    private Mixture_Methane_Nitrogen()
    {
      #region  Mixture parameter

      _beta_T = 0.99809883;
      _gamma_T = 0.979273013;
      _beta_v = 0.998721377;
      _gamma_v = 1.013950311;
      _F = 1;

      _departureCoefficients_Polynomial = new (double ai, double ti, double di)[]
       {
        ( -0.0098038985517335,                    0,                    1),
        ( 0.00042487270143005,                 1.85,                    4),
       };

      _departureCoefficients_Special = new (double n, double t, double d, double eta, double epsilon, double beta, double gamma)[]
      {
        (  -0.034800214576142,                 7.85,                    1,                   -1,                  0.5,                   -1,                  0.5),
        (   -0.13333813013896,                  5.4,                    2,                   -1,                  0.5,                   -1,                  0.5),
        (  -0.011993694974627,                    0,                    2,                -0.25,                  0.5,                 -2.5,                  0.5),
        (   0.069243379775168,                 0.75,                    2,                    0,                  0.5,                   -3,                  0.5),
        (   -0.31022508148249,                  2.8,                    2,                    0,                  0.5,                   -3,                  0.5),
        (    0.24495491753226,                 4.45,                    2,                    0,                  0.5,                   -3,                  0.5),
        (    0.22369816716981,                 4.25,                    3,                    0,                  0.5,                   -3,                  0.5),
      };
      #endregion

    }
  }
}
