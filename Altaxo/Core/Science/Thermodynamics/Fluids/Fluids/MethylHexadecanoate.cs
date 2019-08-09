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
  /// State equations and constants of methyl hexadecanoate.
  /// Short name: methyl palmitate.
  /// Synomym: methyl ester palmitic acid.
  /// Chemical formula: C17H34O2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'mpalmita.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS):  Huber, M.L., Lemmon, E.W., Kazakov, A., Ott, L.S., and Bruno, T.J. "Model for the Thermodynamic Properties of a Biodiesel Fuel," Energy &amp; Fuels, 23:3790-3797, 2009.</para>
  /// <para>HeatCapacity (CPP):  TDE 3.0 internal version, March 2008, Planck-Einstein form based on estimation from Joback method, uncertainty 10%</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("112-39-0")]
  public class MethylHexadecanoate : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static MethylHexadecanoate Instance { get; } = new MethylHexadecanoate();

    #region Constants for methyl hexadecanoate

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "methyl hexadecanoate";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "methyl palmitate";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "methyl ester palmitic acid";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C17H34O2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "112-39-0";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.27045066; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 755;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1350000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 897;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 242;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 8.149E-07;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = double.NaN;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = double.NaN;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 602.268912155815;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.91;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.54;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 302.71;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 1000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 3360;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 50000000;

    #endregion Constants for methyl hexadecanoate

    private MethylHexadecanoate()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 335.273174078933;
      _alpha0_n_tau = -29.8059358897566;
      _alpha0_n_lntau = -1;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (   -284.773362753129,           -0.0801627),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (    41.5684844449533,      3.9104238410596),
          (    34.7632417308038,    0.973050331125828),
          (    36.2787919665855,     2.11066225165563),
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
          (          0.04282821,                    1,                    4),
          (            2.443162,                 0.36,                    1),
          (            -3.75754,                 1.22,                    1),
          (          -0.1588526,                 1.45,                    2),
          (           0.0405599,                  0.7,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            -1.52409,                    3,                    1,                   -1,                    2),
          (          -0.7686167,                  3.9,                    3,                   -1,                    2),
          (             1.79995,                  2.2,                    2,                   -1,                    1),
          (           -1.590967,                  2.9,                    2,                   -1,                    2),
          (         -0.01267681,                 1.25,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            2.198347,                  2.6,                    1,                 -1.1,                 -0.9,                 1.14,                 0.79),
          (          -0.7737211,                    3,                    1,                 -1.6,                -0.65,                 0.65,                  0.9),
          (           -0.431452,                  3.2,                    3,                 -1.1,                -0.75,                 0.77,                 0.76),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -0.54203,                 0.18),
          (              13.191,                  0.5),
          (             -20.107,                  0.7),
          (              11.328,                  0.9),
          (            -0.60761,                  1.5),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -11.612,                 0.65),
          (                 163,                 1.78),
          (             -479.13,                 2.15),
          (              729.86,                  2.7),
          (             -482.02,                  3.1),
          (             -181.98,                  9.8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -13.378,                    1),
          (              12.258,                  1.5),
          (             -12.205,                 2.04),
          (             -5.8118,                  4.3),
          (             -2.5451,                    8),
      };

      #endregion Saturated densities and pressure

    }
  }
}
