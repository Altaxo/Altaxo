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
  /// State equations and constants of mixtures of Nitrogen and CO2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the mixture file 'Nitrogen-CO2.mix' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the mixture file):</para>
  /// <para>Info: Kunz and Wagner (2006) - original GERG-2004 mixture model used in EOS-CG!</para>
  /// <para>Departure function (MXM): Kunz, O., Klimeck, R., Wagner, W., Jaeschke, M. The GERG-2004 Wide-Range Equation of State for Natural Gases and Other Mixtures. GERG Technical Monograph 15. Fortschr.-Ber. VDI, VDI-Verlag, D�sseldorf, 2007.</para>
  /// </remarks>
  [CASRegistryNumber("7727-37-9")]
  [CASRegistryNumber("124-38-9")]
  public class Mixture_Nitrogen_CO2 : BinaryMixtureDefinitionBase
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Mixture_Nitrogen_CO2 Instance { get; } = new Mixture_Nitrogen_CO2();

    #region Constants for the binary mixture of Nitrogen and CO2

    /// <summary>Gets the CAS registry number of component 1 (Nitrogen).</summary>
    public override string CASRegistryNumber1 { get; } = "7727-37-9";

    /// <summary>Gets the CAS registry number of component 2 (CO2).</summary>
    public override string CASRegistryNumber2 { get; } = "124-38-9";

    #endregion Constants for the binary mixture of Nitrogen and CO2

    private Mixture_Nitrogen_CO2()
    {
      #region  Mixture parameter

      _beta_T = 1.005894529;
      _gamma_T = 1.107654104;
      _beta_v = 0.977794634;
      _gamma_v = 1.047578256;
      _F = 1;

      _departureCoefficients_Polynomial = new (double ai, double ti, double di)[]
       {
        (    0.28661625028399,                 1.85,                    2),
        (   -0.10919833861247,                  1.4,                    3),
       };

      _departureCoefficients_Special = new (double n, double t, double d, double eta, double epsilon, double beta, double gamma)[]
      {
        (     -1.137403208227,                  3.2,                    1,                -0.25,                  0.5,                -0.75,                  0.5),
        (    0.76580544237358,                  2.5,                    1,                -0.25,                  0.5,                   -1,                  0.5),
        (  0.0042638000926819,                    8,                    1,                    0,                  0.5,                   -2,                  0.5),
        (    0.17673538204534,                 3.75,                    2,                    0,                  0.5,                   -3,                  0.5),
      };
      #endregion

    }
  }
}
