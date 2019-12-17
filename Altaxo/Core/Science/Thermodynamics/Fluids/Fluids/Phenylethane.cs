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
  /// State equations and constants of phenylethane.
  /// Short name: ethylbenzene.
  /// Synomym: benzene, ethyl-.
  /// Chemical formula: C8H10.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'ebenzene.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Zhou, Y., Lemmon, E.W., and Wu, J."Thermodynamic Properties of o-Xylene, m-Xylene, p-Xylene, and Ethylbenzene"J. Phys. Chem. Ref. Data, 41(023103):1-26, 2012.</para>
  /// <para>HeatCapacity (CPP): see EOS for reference</para>
  /// <para>Saturated vapor pressure: Herrig, S., 2013.</para>
  /// <para>Saturated liquid density: Herrig, S., 2013.</para>
  /// <para>Saturated vapor density: Herrig, S., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("100-41-4")]
  public class Phenylethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Phenylethane Instance { get; } = new Phenylethane();

    #region Constants for phenylethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "phenylethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "ethylbenzene";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "benzene, ethyl-";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C8H10";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "100-41-4";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.106165; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 617.12;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3622400;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 2741.016;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 178.2;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.004002;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 9123.24596049696;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 2.70171190981265E-06;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 409.314171158545;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.305;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.6;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 178.2;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 700;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 9124;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 60000000;

    #endregion Constants for phenylethane

    private Phenylethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 5.70409004241246;
      _alpha0_n_tau = -0.524143527514908;
      _alpha0_n_lntau = 4.2557889;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (           9.7329909,    0.947951775991703),
          (           11.201832,     7.16230230749287),
          (           25.440749,     2.71098003629764),
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
          (        0.0018109418,                    1,                    5),
          (        -0.076824284,                    1,                    1),
          (         0.041823789,                 0.92,                    4),
          (           1.5059649,                 0.27,                    1),
          (          -2.4122441,                0.962,                    1),
          (         -0.47788846,                1.033,                    2),
          (          0.18814732,                0.513,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          -1.0657412,                 2.31,                    1,                   -1,                    2),
          (         -0.20797007,                 3.21,                    3,                   -1,                    2),
          (           1.1222031,                 1.26,                    2,                   -1,                    1),
          (         -0.99300799,                 2.29,                    2,                   -1,                    2),
          (        -0.027300984,                    1,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           1.3757894,                  0.6,                    1,               -1.178,               -2.437,               1.2667,               0.5494),
          (         -0.44477155,                  3.6,                    1,                -1.07,               -1.488,               0.4237,               0.7235),
          (         -0.07769742,                  2.1,                    3,               -1.775,                   -4,               0.8573,                0.493),
          (            -2.16719,                  0.5,                    3,               -15.45,               -418.6,                 1.15,               0.8566),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              3.5146,                 0.43),
          (             -3.7537,                 0.83),
          (               5.476,                  1.3),
          (             -3.4724,                  1.9),
          (              1.2141,                  3.1),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.2877,                 0.42),
          (             -3.6071,                 0.98),
          (             -15.878,                 2.48),
          (             -53.363,                  5.9),
          (             -128.57,                 13.4),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.8411,                    1),
          (              2.5921,                  1.5),
          (              -3.502,                  2.5),
          (             -2.7613,                  5.4),
      };

      #endregion Saturated densities and pressure

    }
  }
}
