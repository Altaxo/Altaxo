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
  /// State equations and constants of methyl octadecanoate.
  /// Short name: methyl stearate.
  /// Synomym: methyl ester stearic acid.
  /// Chemical formula: C19H38O2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'mstearat.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS):  Huber, M.L., Lemmon, E.W., Kazakov, A., Ott, L.S., and Bruno, T.J. "Model for the Thermodynamic Properties of a Biodiesel Fuel," Energy &amp; Fuels, 23:3790-3797, 2009.</para>
  /// <para>HeatCapacity (CPP):  TDE 3.0 internal version, March 2008, Planck-Einstein form based on estimation from Joback method, uncertainty 10%</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("112-61-8")]
  public class MethylOctadecanoate : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static MethylOctadecanoate Instance { get; } = new MethylOctadecanoate();

    #region Constants for methyl octadecanoate

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "methyl octadecanoate";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "methyl stearate";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "methyl ester stearic acid";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C19H38O2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "112-61-8";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.29850382; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 775;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1239000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 794.3;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 311.84;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.006011;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 2851.43273206711;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 2.31833639285455E-06;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 629.557000659334;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 1.02;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.54;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 311.84;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 1000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 2860;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 50000000;

    #endregion Constants for methyl octadecanoate

    private MethylOctadecanoate()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -125.38273017903;
      _alpha0_n_tau = -36.0623159459988;
      _alpha0_n_lntau = -1;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (    193.998538022974,            0.0916606),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (    33.3081884213453,    0.717638709677419),
          (    49.1909768894525,     1.69270967741935),
          (     56.852918621892,     3.64607741935484),
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
          (          0.03959635,                    1,                    4),
          (            2.466654,                  0.3,                    1),
          (            -3.89595,                 1.25,                    1),
          (          -0.1167375,                 1.65,                    2),
          (          0.04127229,                  0.8,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -1.403734,                  3.1,                    1,                   -1,                    2),
          (          -0.6465264,                  3.4,                    3,                   -1,                    2),
          (            1.934675,                  2.3,                    2,                   -1,                    1),
          (           -1.608124,                  3.8,                    2,                   -1,                    2),
          (         -0.01113813,                  1.2,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            2.125325,                  3.2,                    1,                 -1.1,                 -0.9,                 1.14,                 0.79),
          (          -0.7772671,                  3.8,                    1,                 -1.6,                -0.65,                 0.65,                  0.9),
          (          -0.4183684,                  3.8,                    3,                 -1.1,                -0.75,                 0.77,                 0.76),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -11.202,                0.439),
          (              78.636,                 0.59),
          (             -125.54,                 0.73),
          (              72.942,                  0.9),
          (             -11.524,                  1.2),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -18.187,                 0.71),
          (              81.619,                  1.3),
          (              -90.21,                  1.5),
          (             -528.88,                    6),
          (                1127,                    7),
          (             -844.53,                    8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -14.597,                    1),
          (              13.836,                  1.5),
          (             -14.484,                 2.12),
          (             -5.1856,                  4.7),
          (             -2.7076,                    8),
      };

      #endregion Saturated densities and pressure

    }
  }
}
