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
  /// State equations and constants of benzene.
  /// Short name: benzene.
  /// Synomym: benzene.
  /// Chemical formula: C6H6.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'benzene.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Thol, M., Lemmon, E.W., Span, R. "Equation of state for benzene for temperatures from the melting line up to 725 K with pressures up to 500 MPa," High Temp. High Press., 41:81-97, 2012.</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("71-43-2")]
  public class Benzene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Benzene Instance { get; } = new Benzene();

    #region Constants for benzene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "benzene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "benzene";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "benzene";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C6H6";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "aromatic";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "71-43-2";

    private int[] _unNumbers = new int[] { 1114, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.07811184; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 562.02;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4907277;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3901;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 278.674;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 4785;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 11444.8069128735;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 2.07192033069777;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 353.218732756008;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.211;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 278.674;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 725;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 11450;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 500000000;

    #endregion Constants for benzene

    private Benzene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -0.673586759677081;
      _alpha0_n_tau = 2.55555672777722;
      _alpha0_n_lntau = 2.94645;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (             7.36374,     7.32358279064802),
          (              18.649,      2.6885164229031),
          (             4.01834,     1.12095654958898),
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
          (          0.03512459,                    1,                    4),
          (              2.2338,                 0.29,                    1),
          (         -3.10542612,                0.696,                    1),
          (           -0.577233,                1.212,                    2),
          (             0.25101,                0.595,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -0.705518,                 2.51,                    1,                   -1,                    2),
          (           -0.139648,                 3.96,                    3,                   -1,                    2),
          (             0.83494,                 1.24,                    2,                   -1,                    1),
          (           -0.331456,                 1.83,                    2,                   -1,                    2),
          (          -0.0279953,                 0.82,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           0.7099766,                 0.57,                    1,               -1.032,               -1.864,                1.118,                0.729),
          (          -0.3732185,                 2.04,                    1,               -1.423,               -1.766,                0.639,                0.907),
          (          -0.0629985,                  3.2,                    3,               -1.071,               -1.825,                0.654,                0.765),
          (           -0.803041,                 0.78,                    3,                -14.2,               -297.9,                1.164,                 0.87),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (               18.16,                0.534),
          (             -56.879,                0.686),
          (              87.478,                 0.84),
          (             -64.365,                    1),
          (                18.5,                  1.2),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.1147,                0.419),
          (             -4.6689,                 1.12),
          (             -16.161,                  2.8),
          (              -146.5,                  7.3),
          (              518.87,                   10),
          (             -827.72,                   12),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.1661,                    1),
          (              2.1551,                  1.5),
          (             -2.0297,                  2.2),
          (             -4.0668,                  4.8),
          (             0.38092,                  6.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}
