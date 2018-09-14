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
  /// State equations and constants of mixtures of Nitrogen and Ethane.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the mixture file 'nitrogen-ethane.mix' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the mixture file):</para>
  /// <para>Info: Kunz and Wagner (2007)</para>
  /// <para>Departure function (MXM): Kunz, O., Klimeck, R., Wagner, W., Jaeschke, M. The GERG-2004 Wide-Range Equation of State for Natural Gases and Other Mixtures. GERG Technical Monograph 15. Fortschr.-Ber. VDI, VDI-Verlag, D�sseldorf, 2007.</para>
  /// </remarks>
  [CASRegistryNumber("7727-37-9")]
  [CASRegistryNumber("74-84-0")]
  public class Mixture_Nitrogen_Ethane : BinaryMixtureDefinitionBase
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Mixture_Nitrogen_Ethane Instance { get; } = new Mixture_Nitrogen_Ethane();

    #region Constants for the binary mixture of Nitrogen and Ethane

    /// <summary>Gets the CAS registry number of component 1 (Nitrogen).</summary>
    public override string CASRegistryNumber1 { get; } = "7727-37-9";

    /// <summary>Gets the CAS registry number of component 2 (Ethane).</summary>
    public override string CASRegistryNumber2 { get; } = "74-84-0";

    #endregion Constants for the binary mixture of Nitrogen and Ethane

    private Mixture_Nitrogen_Ethane()
    {
      #region  Mixture parameter

      _beta_T = 1.007671428;
      _gamma_T = 1.098650964;
      _beta_v = 0.978880168;
      _gamma_v = 1.042352891;
      _F = 1;

      _departureCoefficients_Polynomial = new (double ai, double ti, double di)[]
       {
        (   -0.47376518126608,                    0,                    2),
        (    0.48961193461001,                 0.05,                    2),
        ( -0.0057011062090535,                    0,                    3),
       };

      _departureCoefficients_Special = new (double n, double t, double d, double eta, double epsilon, double beta, double gamma)[]
      {
        (    -0.1996682004132,                 3.65,                    1,                   -1,                  0.5,                   -1,                  0.5),
        (   -0.69411103101723,                  4.9,                    2,                   -1,                  0.5,                   -1,                  0.5),
        (    0.69226192739021,                 4.45,                    2,               -0.875,                  0.5,                -1.25,                  0.5),
      };
      #endregion

    }
  }
}
