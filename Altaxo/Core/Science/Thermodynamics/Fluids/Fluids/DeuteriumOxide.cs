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
  /// State equations and constants of deuterium oxide.
  /// Short name: heavy water.
  /// Synomym: deuterium oxide.
  /// Chemical formula: D2O.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'd2o.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Herrig, S., Span, R., Harvey, A.H., and Lemmon, E.W.to be submitted to J. Phys. Chem. Ref. Data, 2017.</para>
  /// <para>HeatCapacity (CPP): see EOS of Herrig</para>
  /// <para>Saturated vapor pressure: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("7789-20-0")]
  public class DeuteriumOxide : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static DeuteriumOxide Instance { get; } = new DeuteriumOxide();

    #region Constants for deuterium oxide

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "deuterium oxide";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "heavy water";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "deuterium oxide";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "D2O";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "7789-20-0";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.020027508; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 643.847;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 21671000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 17775.55;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 276.969;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 662.04;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 55199.869149007;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.287579010064657;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 374.547704935307;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.364;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.9;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 276.969;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 825;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 65000;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 1200000000;

    #endregion Constants for deuterium oxide

    private DeuteriumOxide()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -8.67397100406045;
      _alpha0_n_tau = 6.96117555312965;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (             0.00863,    0.425566943699357),
          (             0.97454,     2.60931556720774),
          (              2.0646,     6.01851060888689),
          (             0.23528,     11.3380974051289),
          (             0.29555,      29.510116533897),
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
          (           0.0105835,                    1,                    4),
          (          0.99127253,                0.463,                    1),
          (           -1.224122,                 1.29,                    1),
          (            1.710643,                1.307,                    2),
          (           -2.189443,               1.2165,                    2),
          (           0.1145315,                0.587,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (         -0.89875532,                 2.95,                    1,                   -1,                    1),
          (           -1.597051,                1.713,                    1,                   -1,                    2),
          (           -2.804509,                1.929,                    3,                   -1,                    2),
          (          0.33016885,                 0.94,                    2,                   -1,                    1),
          (           -3.396526,                3.033,                    2,                   -1,                    2),
          (           -0.001881,                0.765,                    8,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (         -0.70355957,                1.504,                    1,               -0.982,               -0.907,                2.263,                2.272),
          (         -0.20345481,                 2.85,                    2,                -1.34,                -0.48,                2.343,                1.375),
          (         -0.70691398,                 1.96,                    3,               -1.658,               -1.223,                0.929,                0.648),
          (            2.094255,                0.969,                    1,              -1.6235,                -2.61,                    1,               0.8925),
          (            3.042546,                2.576,                    3,                 -1.4,               -4.283,                1.383,                0.145),
          (           0.8010728,                 2.79,                    1,               -2.206,                 -1.4,                0.968,                0.291),
          (            0.213384,                3.581,                    1,                -0.84,               -0.735,                1.695,                 2.01),
          (          0.32335789,                 3.67,                    2,               -1.535,                -0.24,                 2.23,                 1.08),
          (          -0.0245055,                  1.7,                    2,               -11.33,                -1067,                 1.07,                 0.96),
          (           0.7380677,                    1,                    2,                -3.86,               -13.27,                1.297,                0.181),
          (         -0.21484089,                  4.1,                    1,                -7.56,                -1.48,                 2.41,                0.529),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              2.6406,               0.3678),
          (               9.709,                  1.9),
          (             -18.058,                  2.2),
          (              8.7202,                 2.63),
          (             -7.4487,                  7.3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.7651,                0.409),
          (             -38.673,                1.766),
          (              73.024,                 2.24),
          (             -132.51,                 3.04),
          (              75.235,                 3.42),
          (             -70.412,                  6.9),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -8.0236,                    1),
          (              2.3957,                  1.5),
          (             -42.639,                 2.75),
          (              99.569,                    3),
          (             -62.135,                  3.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}
