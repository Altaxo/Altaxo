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
  /// State equations and constants of hexane.
  /// Short name: hexane.
  /// Synomym: n-hexane.
  /// Chemical formula: CH3-4(CH2)-CH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'hexane.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Span, R. and Wagner, W. "Equations of State for Technical Applications. II. Results for Nonpolar Fluids," Int. J. Thermophys., 24(1):41-109, 2003.</para>
  /// <para>HeatCapacity (CPP): Jaeschke, M. and Schley, P. "Ideal-Gas Thermodynamic Properties for Natural-Gas Applications," Int. J. Thermophys., 16(6):1381-1392, 1995.</para>
  /// <para>Saturated vapor pressure: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("110-54-3")]
  public class Hexane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Hexane Instance { get; } = new Hexane();

    #region Constants for hexane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "hexane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "hexane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "n-hexane";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH3-4(CH2)-CH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "n-alkane";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "110-54-3";

    private int[] _unNumbers = new int[] { 1208, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31451;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.08617536; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 507.82;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3034000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 2705.8779;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 177.83;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 1.277;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 8839.37936678993;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.000863774629636664;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 341.864512243112;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.299;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.07;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 177.83;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 600;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 8850;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 100000000;

    #endregion Constants for hexane

    private Hexane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 24.6896273187263;
      _alpha0_n_tau = -98.4721866398513;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
      };

      _alpha0_Cosh = new (double ni, double thetai)[]
      {
          (   -26.8142058438565,      1.6919518727108),
      };

      _alpha0_Sinh = new (double ni, double thetai)[]
      {
          (    11.6977013880442,    0.359036666535387),
          (    38.6164134414525,     3.59692410696704),
      };
      #endregion Ideal part of dimensionless Helmholtz energy and derivatives

      #region Residual part(s) of dimensionless Helmholtz energy and derivatives

      _alphaR_Poly = new (double ni, double ti, int di)[]
      {
          (           1.0553238,                 0.25,                    1),
          (          -2.6120616,                1.125,                    1),
          (          0.76613883,                  1.5,                    1),
          (         -0.29770321,                1.375,                    2),
          (          0.11879908,                 0.25,                    3),
          (       0.00027922861,                0.875,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           0.4634759,                0.625,                    2,                   -1,                    1),
          (         0.011433197,                 1.75,                    5,                   -1,                    1),
          (         -0.48256969,                3.625,                    1,                   -1,                    2),
          (        -0.093750559,                3.625,                    4,                   -1,                    2),
          (       -0.0067273247,                 14.5,                    3,                   -1,                    3),
          (       -0.0051141584,                   12,                    4,                   -1,                    3),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              146.86,                 0.75),
          (             -265.85,                 0.81),
          (                 122,                 0.88),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -0.13309,                0.107),
          (             -5.0653,                0.553),
          (             -11.602,                2.006),
          (              -28.53,                 4.46),
          (             -51.731,                    8),
          (             -134.82,                   16),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.4172,                    1),
          (              1.2897,                  1.5),
          (             -3.2544,                  3.1),
          (             -1.4609,                  5.3),
          (            0.081765,                  5.6),
      };

      #endregion Saturated densities and pressure

    }
  }
}
