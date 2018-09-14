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
  /// State equations and constants of mixtures of CO2 and Argon.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the mixture file 'co2-argon.mix' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the mixture file):</para>
  /// <para>Info: Gernert (2013)</para>
  /// <para>Departure function (MXM):  Gernert, 2013  carbon dioxide - argon</para>
  /// </remarks>
  [CASRegistryNumber("124-38-9")]
  [CASRegistryNumber("7440-37-1")]
  public class Mixture_CO2_Argon : BinaryMixtureDefinitionBase
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Mixture_CO2_Argon Instance { get; } = new Mixture_CO2_Argon();

    #region Constants for the binary mixture of CO2 and Argon

    /// <summary>Gets the CAS registry number of component 1 (CO2).</summary>
    public override string CASRegistryNumber1 { get; } = "124-38-9";

    /// <summary>Gets the CAS registry number of component 2 (Argon).</summary>
    public override string CASRegistryNumber2 { get; } = "7440-37-1";

    #endregion Constants for the binary mixture of CO2 and Argon

    private Mixture_CO2_Argon()
    {
      #region  Mixture parameter

      _beta_T = 1.027147;
      _gamma_T = 0.968781;
      _beta_v = 1.001378;
      _gamma_v = 1.02971;
      _F = 1;

      _departureCoefficients_Polynomial = new (double ai, double ti, double di)[]
       {
        (          0.20483577,                2.194,                    2),
        (         -0.11465124,                1.743,                    3),
       };

      _departureCoefficients_Special = new (double n, double t, double d, double eta, double epsilon, double beta, double gamma)[]
      {
        (            -2.31517,                 2.47,                    1,                -0.25,                  0.5,                -0.75,                  0.5),
        (           2.3523617,                1.977,                    1,                -0.25,                  0.5,                   -1,                  0.5),
        (         -0.66835959,                1.207,                    1,                    0,                  0.5,                -1.05,                  0.5),
        (        -0.053016558,                0.796,                    2,                    0,                  0.5,                   -3,                  0.5),
      };
      #endregion

    }
  }
}
