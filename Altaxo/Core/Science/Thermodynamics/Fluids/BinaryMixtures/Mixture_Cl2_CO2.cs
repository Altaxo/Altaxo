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
  /// State equations and constants of mixtures of Cl2 and CO2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the mixture file 'chlorine-co2.mix' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the mixture file):</para>
  /// <para>Info: Herrig (2015)</para>
  /// </remarks>
  [CASRegistryNumber("7782-50-5")]
  [CASRegistryNumber("124-38-9")]
  public class Mixture_Cl2_CO2 : BinaryMixtureDefinitionBase
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Mixture_Cl2_CO2 Instance { get; } = new Mixture_Cl2_CO2();

    #region Constants for the binary mixture of Cl2 and CO2

    /// <summary>Gets the CAS registry number of component 1 (Cl2).</summary>
    public override string CASRegistryNumber1 { get; } = "7782-50-5";

    /// <summary>Gets the CAS registry number of component 2 (CO2).</summary>
    public override string CASRegistryNumber2 { get; } = "124-38-9";

    #endregion Constants for the binary mixture of Cl2 and CO2

    private Mixture_Cl2_CO2()
    {
      #region  Mixture parameter

      _beta_T = 1;
      _gamma_T = 1;
      _beta_v = 1;
      _gamma_v = 1;
      _F = 0;
      #endregion

    }
  }
}
