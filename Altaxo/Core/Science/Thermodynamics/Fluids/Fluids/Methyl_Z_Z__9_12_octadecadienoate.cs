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
  /// State equations and constants of methyl (Z,Z)-9,12-octadecadienoate.
  /// Short name: methyl linoleate.
  /// Synomym: methyl ester(Z,Z)-9,12-octadecadienoic acid.
  /// Chemical formula: C19H34O2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'mlinolea.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS):  Huber, M.L., Lemmon, E.W., Kazakov, A., Ott, L.S., and Bruno, T.J. "Model for the Thermodynamic Properties of a Biodiesel Fuel," Energy &amp; Fuels, 23:3790-3797, 2009.</para>
  /// <para>HeatCapacity (CPP):  TDE 3.0 internal version, March 2008, Planck-Einstein form based on estimation from Joback method, uncertainty 10%</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("112-63-0")]
  public class Methyl_Z_Z__9_12_octadecadienoate : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Methyl_Z_Z__9_12_octadecadienoate Instance { get; } = new Methyl_Z_Z__9_12_octadecadienoate();

    #region Constants for methyl (Z,Z)-9,12-octadecadienoate

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "methyl (Z,Z)-9,12-octadecadienoate";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "methyl linoleate";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "methyl ester(Z,Z)-9,12-octadecadienoic acid";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C19H34O2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "112-63-0";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.29447206; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 799;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1341000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 808.4;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 238.1;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 7.719E-09;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = double.NaN;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = double.NaN;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 628.840566215343;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.805;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.79;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 238.1;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 1000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 3160;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 50000000;

    #endregion Constants for methyl (Z,Z)-9,12-octadecadienoate

    private Methyl_Z_Z__9_12_octadecadienoate()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 1330.36241976484;
      _alpha0_n_tau = -33.3956015545631;
      _alpha0_n_lntau = -1;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (   -1275.01239484272,            -0.020213),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (    52.6035808407317,     3.81991239048811),
          (    34.5448273804999,    0.934456821026283),
          (    38.7223626467201,     2.03295369211514),
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
          (          0.03183187,                    1,                    4),
          (            1.927286,                  0.2,                    1),
          (           -3.685053,                  1.2,                    1),
          (          0.08449312,                    1,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          -0.9766643,                  2.2,                    1,                   -1,                    2),
          (          -0.4323178,                  2.5,                    3,                   -1,                    2),
          (             2.00047,                  1.8,                    2,                   -1,                    1),
          (            -1.75203,                 1.92,                    2,                   -1,                    2),
          (         -0.01726895,                 1.47,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            2.116515,                  1.7,                    1,                 -1.1,                 -0.9,                 1.14,                 0.79),
          (          -0.7884271,                  2.3,                    1,                 -1.6,                -0.65,                 0.65,                  0.9),
          (          -0.3811699,                  2.1,                    3,                 -1.1,                -0.75,                 0.77,                 0.76),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              227.05,                 0.83),
          (             -667.63,                 0.98),
          (              723.23,                 1.17),
          (             -492.44,                  1.5),
          (              213.91,                  1.7),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              -8.588,                0.568),
          (              14.766,                 1.08),
          (             -24.195,                  1.4),
          (             -374.74,                  4.8),
          (              326.89,                    5),
          (             -191.25,                    9),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -10.946,                    1),
          (              4.8849,                  1.5),
          (             -4.6773,                 2.22),
          (             -8.0201,                  3.6),
          (             -8.9572,                    8),
      };

      #endregion Saturated densities and pressure

    }
  }
}
