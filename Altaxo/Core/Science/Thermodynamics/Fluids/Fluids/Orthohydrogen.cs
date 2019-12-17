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
  /// State equations and constants of orthohydrogen.
  /// Short name: orthohydrogen.
  /// Synomym: R-702.
  /// Chemical formula: H2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'orthohyd.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Leachman, J.W., Jacobsen, R.T, Penoncello, S.G., Lemmon, E.W."Fundamental Equations of State for Parahydrogen, Normal Hydrogen, andOrthohydrogen,"J. Phys. Chem. Ref. Data, 38(3):721-748, 2009.</para>
  /// <para>HeatCapacity (CPP): Leachman, J.W., Jacobsen, R.T, Penoncello, S.G., Lemmon, E.W."Fundamental Equations of State for Parahydrogen, Normal Hydrogen, andOrthohydrogen,"J. Phys. Chem. Ref. Data, 38(3):721-748, 2009.</para>
  /// <para>Saturated vapor pressure: Leachman, J.W., Jacobsen, R.T, Penoncello, S.G., Lemmon, E.W."Fundamental Equations of State for Parahydrogen, Normal Hydrogen, andOrthohydrogen,"J. Phys. Chem. Ref. Data, 38(3):721-748, 2009.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("1333-74-0o")]
  public class Orthohydrogen : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Orthohydrogen Instance { get; } = new Orthohydrogen();

    #region Constants for orthohydrogen

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "orthohydrogen";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "orthohydrogen";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-702";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "H2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "1333-74-0o";

    private int[] _unNumbers = new int[] { 1049, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.00201594; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 33.22;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1310650;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 15445;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 14.008;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 7560;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 38200.3216488919;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 65.8416415727076;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 20.379968058644;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = -0.218;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 14.008;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 1000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 38200;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 2000000000;

    #endregion Constants for orthohydrogen

    private Orthohydrogen()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -1.46754423359213;
      _alpha0_n_tau = 1.88450688616282;
      _alpha0_n_lntau = 1.5;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (             2.54151,     25.7676098735701),
          (             -2.3661,      43.467790487658),
          (             1.00365,     66.0445514750151),
          (             1.22447,     209.753160746538),
      };

      _alpha0_Cosh = new (double ni, double thetai)[]
      {
      };

      _alpha0_Sinh = new (double ni, double thetai)[]
      {
      };
      #endregion Ideal part of dimensionless Helmholtz energy and derivatives

      #region Residual part(s) of dimensionless Helmholtz energy and derivatives

      _alphaR_Poly = new (double ni, double ti, int di)[]
      {
          (            -6.83148,               0.7333,                    1),
          (                0.01,                    1,                    4),
          (             2.11505,               1.1372,                    1),
          (             4.38353,               0.5136,                    1),
          (            0.211292,               0.5638,                    2),
          (            -1.00939,               1.6248,                    2),
          (            0.142086,                1.829,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            -0.87696,                2.404,                    1,                   -1,                    1),
          (            0.804927,                2.105,                    3,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           -0.710775,                  4.1,                    2,               -1.169,              -0.4555,               1.5444,               0.6366),
          (           0.0639688,                7.658,                    1,               -0.894,              -0.4046,               0.6627,               0.3876),
          (           0.0710858,                1.259,                    3,                -0.04,              -0.0869,                0.763,               0.9437),
          (           -0.087654,                7.589,                    1,               -2.072,              -0.4415,               0.6587,               0.3976),
          (            0.647088,                3.946,                    1,               -1.306,              -0.5743,               1.4327,               0.9626),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              4.3911,                 0.53),
          (             -7.5872,                 0.93),
          (              10.402,                 1.35),
          (             -7.2651,                  1.8),
          (              1.8302,                  2.4),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.1463,                0.491),
          (             -16.183,                  2.1),
          (              31.803,                  2.9),
          (             -219.61,                  4.4),
          (              431.23,                    5),
          (             -255.91,                  5.5),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (            -4.88684,                    1),
          (              1.0531,                  1.5),
          (            0.856947,                  2.7),
          (           -0.185355,                  6.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}
