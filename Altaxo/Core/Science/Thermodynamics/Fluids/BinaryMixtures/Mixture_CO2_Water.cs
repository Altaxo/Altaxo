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
  /// State equations and constants of mixtures of CO2 and Water.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the mixture file 'co2-water.mix' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the mixture file):</para>
  /// <para>Info: Gernert (2013)</para>
  /// </remarks>
  [CASRegistryNumber("124-38-9")]
  [CASRegistryNumber("7732-18-5")]
  public class Mixture_CO2_Water : BinaryMixtureDefinitionBase
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Mixture_CO2_Water Instance { get; } = new Mixture_CO2_Water();

    #region Constants for the binary mixture of CO2 and Water

    /// <summary>Gets the CAS registry number of component 1 (CO2).</summary>
    public override string CASRegistryNumber1 { get; } = "124-38-9";

    /// <summary>Gets the CAS registry number of component 2 (Water).</summary>
    public override string CASRegistryNumber2 { get; } = "7732-18-5";

    #endregion Constants for the binary mixture of CO2 and Water

    private Mixture_CO2_Water()
    {
      #region  Mixture parameter

      _beta_T = 1.030538;
      _gamma_T = 0.828472;
      _beta_v = 1.021392;
      _gamma_v = 0.895156;
      _F = 1;

      _departureCoefficients_Polynomial = new (double ai, double ti, double di)[]
       {
        (          0.39440467,                 0.88,                    1),
        (          -1.7634732,                2.932,                    1),
        (          0.14620755,                2.433,                    3),
       };

      _departureCoefficients_Exponential = new (double ai, double ti, double di, double li)[]
      {
        (         0.008752232,                 1.33,                    0,                    1),
        (           2.0349398,                4.416,                    2,                    1),
        (         -0.09035025,                5.514,                    3,                    1),
        (         -0.21638854,                5.203,                    1,                    2),
        (          0.03961217,                    1,                    5,                    2),
      };
      #endregion

    }
  }
}
