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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{
  /// <summary>
  /// Represents a mixture of fluids.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Thermodynamics.Fluids.HelmholtzEquationOfState" />
  public class MixtureOfFluids : HelmholtzEquationOfState
  {
    private static Dictionary<(string cas1, string cas2), Type> _binaryMixtureDefinitions = new Dictionary<(string cas1, string cas2), Type>();

    private static Dictionary<string, Type> _fluidDefinitions = new Dictionary<string, Type>();

    private (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl pureFluid, double moleFraction)[] _fluidsAndMoleFractions;

    private (BinaryMixtureDefinitionBase definition, bool reverse)[,] _mixtureDefinitions;

    private double _reducingMoleDensity;
    private double _reducingTemperature;
    protected double _molecularWeight;
    protected double _workingUniversalGasConstant;

    static MixtureOfFluids()
    {
      CollectAllBinaryMixDefinitions();
      CollectAllFluidDefinitions();
    }

    public static MixtureOfFluids FromCASRegistryNumbersAndMoleFractions(IEnumerable<(string casNumber, double moleFraction)> casNumbersAndMoleFractions)
    {
      var list = new List<(HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid, double moleFraction)>();

      foreach (var (casNumber, moleFraction) in casNumbersAndMoleFractions)
      {
        if (_fluidDefinitions.TryGetValue(casNumber, out var fluidType))
        {
          var pd = fluidType.GetProperty("Instance")?.GetGetMethod() ?? throw new InvalidOperationException($"Fluid type {fluidType} doesn't seem to have an instance property");
          var definition = (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl)(pd.Invoke(null, null) ?? throw new InvalidOperationException($"Instance property of fluid type {fluidType} returned null."));
          list.Add((definition, moleFraction));
        }
        else
        {
          throw new ArgumentException(string.Format("A fluid with CAS registry number {0} could not be found!", casNumber));
        }
      }

      return new MixtureOfFluids(list);
    }

    public static MixtureOfFluids FromCASRegistryNumbersAndMassFractions(IEnumerable<(string casNumber, double massFraction)> casNumbersAndMassFractions)
    {
      var list = new List<(HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid, double moleFraction)>();

      // in order to calculate mole fractions, we need the molecular weight of the fluids

      foreach (var (casNumber, massFraction) in casNumbersAndMassFractions)
      {
        if (_fluidDefinitions.TryGetValue(casNumber, out var fluidType))
        {
          var pd = fluidType.GetProperty("Instance")?.GetGetMethod() ?? throw new InvalidOperationException($"Fluid type {fluidType} doesn't seem to have an instance property");
          var definition = (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl)(pd.Invoke(null, null) ?? throw new InvalidOperationException($"Fluid type {fluidType} doesn't seem to have an instance property"));
          list.Add((definition, massFraction));
        }
        else
        {
          throw new ArgumentException(string.Format("A fluid with CAS registry number {0} could not be found!", casNumber));
        }
      }

      // now that we have all fluids, we can calculate mole fractions
      double sum = 0;
      foreach (var (fluid, massFraction) in list)
      {
        sum += massFraction / fluid.MolecularWeight;
      }

      for (int i = 0; i < list.Count; ++i)
      {
        var (fluid, massFraction) = list[i];
        list[i] = (fluid, (massFraction / fluid.MolecularWeight) / sum);
      }

      return new MixtureOfFluids(list);
    }


    public MixtureOfFluids(
      HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid1, double moleFraction1,
      HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid2, double moleFraction2,
      bool checkForSumEqualsOne = true)
      : this(new (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl, double)[]
      {
        (fluid1, moleFraction1),
        (fluid2, moleFraction2),
      }, checkForSumEqualsOne)
    {
    }

    public MixtureOfFluids(
      HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid1, double moleFraction1,
      HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid2, double moleFraction2,
      HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid3, double moleFraction3,
      bool checkForSumEqualsOne = true)
  : this(new (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl, double)[]
  {
        (fluid1, moleFraction1),
        (fluid2, moleFraction2),
        (fluid3, moleFraction3),
  }, checkForSumEqualsOne)
    {
    }

    public MixtureOfFluids(
    HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid1, double moleFraction1,
    HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid2, double moleFraction2,
    HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid3, double moleFraction3,
    HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid4, double moleFraction4,
    bool checkForSumEqualsOne = true)
: this(new (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl, double)[]
{
        (fluid1, moleFraction1),
        (fluid2, moleFraction2),
        (fluid3, moleFraction3),
        (fluid4, moleFraction4),
}, checkForSumEqualsOne)
    {
    }

    public MixtureOfFluids(
      HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid1, double moleFraction1,
      HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid2, double moleFraction2,
      HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid3, double moleFraction3,
      HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid4, double moleFraction4,
      HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid5, double moleFraction5,
      bool checkForSumEqualsOne = true)
    : this(new (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl, double)[]
{
        (fluid1, moleFraction1),
        (fluid2, moleFraction2),
        (fluid3, moleFraction3),
        (fluid4, moleFraction4),
        (fluid5, moleFraction5),
}, checkForSumEqualsOne)
    {
    }

    public MixtureOfFluids(IReadOnlyList<(HelmholtzEquationOfStateOfPureFluidsBySpanEtAl pureFluid, double moleFraction)> fluidsAndMoleFractions, bool checkForSumEqualToOne = true)
    {
      // Parameter check and copy of the provided array
      _fluidsAndMoleFractions = new (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl pureFluid, double moleFraction)[fluidsAndMoleFractions.Count];

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        if (fluidsAndMoleFractions[i].pureFluid is null)
          throw new ArgumentNullException(string.Format("Fluid at index {0} is null!", i));

        if (!(0 <= fluidsAndMoleFractions[i].moleFraction && fluidsAndMoleFractions[i].moleFraction <= 1))
          throw new ArgumentOutOfRangeException(string.Format("Mole fraction at index {0} is not in the interval [0, 1]", i));

        _fluidsAndMoleFractions[i] = fluidsAndMoleFractions[i];
      }

      // Binary mixture definitions
      _mixtureDefinitions = new (BinaryMixtureDefinitionBase definition, bool reverse)[_fluidsAndMoleFractions.Length, _fluidsAndMoleFractions.Length];

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        for (int j = i; j < _fluidsAndMoleFractions.Length; ++j)
        {
          if (j == i)
          {
#nullable disable
            _mixtureDefinitions[i, j] = (null, false);
#nullable enable
          }
          else
          {
            if (_binaryMixtureDefinitions.TryGetValue((_fluidsAndMoleFractions[i].pureFluid.CASRegistryNumber, _fluidsAndMoleFractions[j].pureFluid.CASRegistryNumber), out var definitionType))
            {
              var pd = definitionType.GetProperty("Instance")?.GetGetMethod() ?? throw new InvalidOperationException($"Fluid type {definitionType} doesn't seem to have an instance property");
              var definition = (BinaryMixtureDefinitionBase)(pd.Invoke(null, null) ?? throw new InvalidOperationException($"Fluid type {definitionType} doesn't seem to have an instance property"));
              bool reverse = _fluidsAndMoleFractions[i].pureFluid.CASRegistryNumber != definition.CASRegistryNumber1;
              _mixtureDefinitions[i, j] = (definition, reverse);
              _mixtureDefinitions[j, i] = (definition, !reverse);
            }
          }
        }
      }

      // Working universal gas constant
      CalculateWorkingUniversalGasConstant();
      // Reducing density and temperature
      CalculateReducingDensityAndTemperature();
      // Molecular weight
      CalculateMolecularWeight();
    }

    /// <summary>
    /// Returns the same mixture of fluids, but with other mole fractions.
    /// </summary>
    /// <param name="moleFractions">The mole fractions of the components.</param>
    /// <returns>A mixture with the same components, but different mole fractions. The sum of mole fractions has to be equal to 1.</returns>
    public MixtureOfFluids WithMoleFractions(params double[] moleFractions)
    {
      return WithMoleFractions(moleFractions, true);
    }

    /// <summary>
    /// Returns the same mixture of fluids, but with other mass fractions.
    /// </summary>
    /// <param name="massFractions">The mass fractions of the components.</param>
    /// <returns>A mixture with the same components, but different mass fractions. The sum of mass fractions has to be equal to 1.</returns>
    public MixtureOfFluids WithMassFractions(params double[] massFractions)
    {
      double sum = 0;
      double checksum = 0;

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        sum += massFractions[i] / _fluidsAndMoleFractions[i].pureFluid.MolecularWeight;
        checksum += massFractions[i];
      }

      if (Math.Abs(checksum - 1) > 1E-7)
        throw new ArgumentOutOfRangeException(string.Format("The provided mass fractions do not sum up to 1, but to a value of {0}.", checksum));


      var moleFractions = massFractions.Select((massFraction, i) => (massFraction / _fluidsAndMoleFractions[i].pureFluid.MolecularWeight) / sum);

      return WithMoleFractions(moleFractions, true);
    }



    /// <summary>
    /// Returns the same mixture of fluids, but with other mole fractions.
    /// </summary>
    /// <param name="moleFractions">The mole fractions of the components.</param>
    /// <param name="checkForSumEqualToOne">If set to <c>true</c>, it is checked that the sum of mole fractions equals to 1. If this is not the case, a <see cref="ArgumentOutOfRangeException"/> is thrown.</param>
    /// <returns></returns>
    public MixtureOfFluids WithMoleFractions(IEnumerable<double> moleFractions, bool checkForSumEqualToOne = true)
    {
      var result = (MixtureOfFluids)MemberwiseClone();

      int index = 0;
      double sum = 0;
      foreach (var moleFraction in moleFractions)
      {
        result._fluidsAndMoleFractions[index] = (result._fluidsAndMoleFractions[index].pureFluid, moleFraction);
        sum += moleFraction;
        ++index;
      }

      if (checkForSumEqualToOne && Math.Abs(sum - 1) > 1E-7)
        throw new ArgumentOutOfRangeException(string.Format("The provided mole fractions do not sum up to 1, but to a value of {0}.", sum));

      // Working universal gas constant
      result.CalculateWorkingUniversalGasConstant();
      // Reducing density and temperature
      result.CalculateReducingDensityAndTemperature();
      // Molecular weight
      result.CalculateMolecularWeight();

      return result;
    }

    protected void CalculateReducingDensityAndTemperature()
    {
      double sumD = 0;
      double sumT = 0;

      for (int i = 0; i < _fluidsAndMoleFractions.Length - 1; ++i)
      {
        var (fluidi, xi) = _fluidsAndMoleFractions[i];
        for (int j = i + 1; j < _fluidsAndMoleFractions.Length; ++j)
        {
          var (fluidj, xj) = _fluidsAndMoleFractions[j];
          var (mixture, isReverse) = _mixtureDefinitions[i, j];

          if (isReverse)
          {
            sumD += 0.25 * xi * xj * mixture.Beta_v * mixture.Gamma_v * (xi + xj) / (Pow2(mixture.Beta_v) * xj + xi) * Pow3(Math.Pow(fluidi.CriticalPointMoleDensity, -1 / 3.0) + Math.Pow(fluidj.CriticalPointMoleDensity, -1 / 3.0));
            sumT += 2 * xi * xj * mixture.Beta_T * mixture.Gamma_T * (xi + xj) / (Pow2(mixture.Beta_T) * xj + xi) * Math.Sqrt(fluidi.CriticalPointTemperature * fluidj.CriticalPointTemperature);
          }
          else
          {
            sumD += 0.25 * xi * xj * mixture.Beta_v * mixture.Gamma_v * (xi + xj) / (Pow2(mixture.Beta_v) * xi + xj) * Pow3(Math.Pow(fluidi.CriticalPointMoleDensity, -1 / 3.0) + Math.Pow(fluidj.CriticalPointMoleDensity, -1 / 3.0));
            sumT += 2 * xi * xj * mixture.Beta_T * mixture.Gamma_T * (xi + xj) / (Pow2(mixture.Beta_T) * xi + xj) * Math.Sqrt(fluidi.CriticalPointTemperature * fluidj.CriticalPointTemperature);
          }
        }
      }

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        var (fluid, xi) = _fluidsAndMoleFractions[i];
        sumD += xi * xi / fluid.CriticalPointMoleDensity;
        sumT += xi * xi * fluid.CriticalPointTemperature;
      }

      _reducingMoleDensity = 1 / sumD;
      _reducingTemperature = sumT;
    }

    protected void CalculateMolecularWeight()
    {
      double sum = 0;
      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        var (fluid, xi) = _fluidsAndMoleFractions[i];
        sum += xi * fluid.MolecularWeight;
      }
      _molecularWeight = sum;
    }

    protected void CalculateWorkingUniversalGasConstant()
    {
      double sum = 0;
      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        var (fluid, xi) = _fluidsAndMoleFractions[i];
        sum += xi * fluid.WorkingUniversalGasConstant;
      }
      _workingUniversalGasConstant = sum;
    }

    protected static void CollectAllBinaryMixDefinitions()
    {
      var ass = Assembly.GetCallingAssembly();

      var baseType = typeof(BinaryMixtureDefinitionBase);
      foreach (var type in ass.DefinedTypes)
      {
        if (!baseType.IsAssignableFrom(type))
          continue;

        var casNumberAttributes = type.GetCustomAttributes(typeof(CASRegistryNumberAttribute)).ToArray();
        if (2 != casNumberAttributes.Length)
          continue;

        string casNumber1 = ((CASRegistryNumberAttribute)casNumberAttributes[0]).CASRegistryNumber;
        string casNumber2 = ((CASRegistryNumberAttribute)casNumberAttributes[1]).CASRegistryNumber;

        _binaryMixtureDefinitions[(casNumber1, casNumber2)] = type;
        _binaryMixtureDefinitions[(casNumber2, casNumber1)] = type;
      }
    }

    protected static void CollectAllFluidDefinitions()
    {
      var ass = Assembly.GetCallingAssembly();

      var baseType = typeof(HelmholtzEquationOfStateOfPureFluidsBySpanEtAl);
      foreach (var type in ass.DefinedTypes)
      {
        if (!baseType.IsAssignableFrom(type))
          continue;

        var casNumberAttributes = type.GetCustomAttributes(typeof(CASRegistryNumberAttribute)).ToArray();
        if (1 != casNumberAttributes.Length)
          continue;

        string casNumber = ((CASRegistryNumberAttribute)casNumberAttributes[0]).CASRegistryNumber;

        _fluidDefinitions[casNumber] = type;
      }
    }

    #region Helmholtz equation of state

    /// <inheritdoc/>
    public override double ReducingTemperature => _reducingTemperature;

    /// <inheritdoc/>
    public override double ReducingMoleDensity => _reducingMoleDensity;

    public override double WorkingUniversalGasConstant => _workingUniversalGasConstant;

    public override double MolecularWeight => _molecularWeight;

    /// <inheritdoc/>
    public override double Phi0_OfReducedVariables(double delta, double tau)
    {
      double moleDensity = delta * _reducingMoleDensity;
      double temperature = _reducingTemperature / tau;

      double sum = 0;

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        var (fluid, xi) = _fluidsAndMoleFractions[i];
        if (xi <= 0)
          continue;

        sum += xi * (Math.Log(xi) + fluid.Phi0_OfReducedVariables(moleDensity / fluid.ReducingMoleDensity, fluid.ReducingTemperature / temperature));
      }

      return sum;
    }

    /// <inheritdoc/>
    public override double Phi0_tau_OfReducedVariables(double delta, double tau)
    {
      double moleDensity = delta * _reducingMoleDensity;
      double temperature = _reducingTemperature / tau;

      double sum = 0;

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        var (fluid, xi) = _fluidsAndMoleFractions[i];
        if (xi <= 0)
          continue;

        sum += xi * (fluid.ReducingTemperature / _reducingTemperature) * fluid.Phi0_tau_OfReducedVariables(moleDensity / fluid.ReducingMoleDensity, fluid.ReducingTemperature / temperature);
      }
      return sum;
    }

    /// <inheritdoc/>
    public override double Phi0_tautau_OfReducedVariables(double delta, double tau)
    {
      double moleDensity = delta * _reducingMoleDensity;
      double temperature = _reducingTemperature / tau;

      double sum = 0;

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        var (fluid, xi) = _fluidsAndMoleFractions[i];
        if (xi <= 0)
          continue;

        sum += xi * Pow2(fluid.ReducingTemperature / _reducingTemperature) * fluid.Phi0_tautau_OfReducedVariables(moleDensity / fluid.ReducingMoleDensity, fluid.ReducingTemperature / temperature);
      }
      return sum;
    }

    /// <inheritdoc/>
    public override double PhiR_OfReducedVariables(double delta, double tau)
    {
      double sum = 0;

      for (int i = 0; i < _fluidsAndMoleFractions.Length - 1; ++i)
      {
        var (_, xi) = _fluidsAndMoleFractions[i];
        for (int j = i + 1; j < _fluidsAndMoleFractions.Length; ++j)
        {
          var (_, xj) = _fluidsAndMoleFractions[j];
          var (mixture, isReverse) = _mixtureDefinitions[i, j];
          if (mixture.F != 0)
            sum += xi * xj * mixture.F * mixture.DepartureFunction_OfReducedVariables(delta, tau);
        }
      }

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        var (fluid, xi) = _fluidsAndMoleFractions[i];
        if (xi <= 0)
          continue;

        sum += xi * fluid.PhiR_OfReducedVariables(delta, tau);
      }

      return sum;
    }

    public override double PhiR_delta_OfReducedVariables(double delta, double tau)
    {
      double sum = 0;

      for (int i = 0; i < _fluidsAndMoleFractions.Length - 1; ++i)
      {
        var (_, xi) = _fluidsAndMoleFractions[i];
        for (int j = i + 1; j < _fluidsAndMoleFractions.Length; ++j)
        {
          var (_, xj) = _fluidsAndMoleFractions[j];
          var (mixture, isReverse) = _mixtureDefinitions[i, j];
          if (mixture.F != 0)
            sum += xi * xj * mixture.F * mixture.DepartureFunction_delta_OfReducedVariables(delta, tau);
        }
      }

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        var (fluid, xi) = _fluidsAndMoleFractions[i];
        if (xi <= 0)
          continue;

        sum += xi * fluid.PhiR_delta_OfReducedVariables(delta, tau);
      }

      return sum;
    }

    public override double PhiR_deltadelta_OfReducedVariables(double delta, double tau)
    {
      double sum = 0;

      for (int i = 0; i < _fluidsAndMoleFractions.Length - 1; ++i)
      {
        var (_, xi) = _fluidsAndMoleFractions[i];
        for (int j = i + 1; j < _fluidsAndMoleFractions.Length; ++j)
        {
          var (_, xj) = _fluidsAndMoleFractions[j];
          var (mixture, isReverse) = _mixtureDefinitions[i, j];
          if (mixture.F != 0)
            sum += xi * xj * mixture.F * mixture.DepartureFunction_deltadelta_OfReducedVariables(delta, tau);
        }
      }

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        var (fluid, xi) = _fluidsAndMoleFractions[i];
        if (xi <= 0)
          continue;

        sum += xi * fluid.PhiR_deltadelta_OfReducedVariables(delta, tau);
      }

      return sum;
    }

    public override double PhiR_tau_OfReducedVariables(double delta, double tau)
    {
      double sum = 0;

      for (int i = 0; i < _fluidsAndMoleFractions.Length - 1; ++i)
      {
        var (_, xi) = _fluidsAndMoleFractions[i];
        for (int j = i + 1; j < _fluidsAndMoleFractions.Length; ++j)
        {
          var (_, xj) = _fluidsAndMoleFractions[j];
          var (mixture, isReverse) = _mixtureDefinitions[i, j];
          if (mixture.F != 0)
            sum += xi * xj * mixture.F * mixture.DepartureFunction_tau_OfReducedVariables(delta, tau);
        }
      }

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        var (fluid, xi) = _fluidsAndMoleFractions[i];
        if (xi <= 0)
          continue;

        sum += xi * fluid.PhiR_tau_OfReducedVariables(delta, tau);
      }

      return sum;
    }

    public override double PhiR_tautau_OfReducedVariables(double delta, double tau)
    {
      double sum = 0;

      for (int i = 0; i < _fluidsAndMoleFractions.Length - 1; ++i)
      {
        var (_, xi) = _fluidsAndMoleFractions[i];
        for (int j = i + 1; j < _fluidsAndMoleFractions.Length; ++j)
        {
          var (_, xj) = _fluidsAndMoleFractions[j];
          var (mixture, isReverse) = _mixtureDefinitions[i, j];
          if (mixture.F != 0)
            sum += xi * xj * mixture.F * mixture.DepartureFunction_tautau_OfReducedVariables(delta, tau);
        }
      }

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        var (fluid, xi) = _fluidsAndMoleFractions[i];
        if (xi <= 0)
          continue;

        sum += xi * fluid.PhiR_tautau_OfReducedVariables(delta, tau);
      }

      return sum;
    }

    public override double PhiR_deltatau_OfReducedVariables(double delta, double tau)
    {
      double sum = 0;

      for (int i = 0; i < _fluidsAndMoleFractions.Length - 1; ++i)
      {
        var (_, xi) = _fluidsAndMoleFractions[i];
        for (int j = i + 1; j < _fluidsAndMoleFractions.Length; ++j)
        {
          var (_, xj) = _fluidsAndMoleFractions[j];
          var (mixture, isReverse) = _mixtureDefinitions[i, j];
          if (mixture.F != 0)
            sum += xi * xj * mixture.F * mixture.DepartureFunction_deltatau_OfReducedVariables(delta, tau);
        }
      }

      for (int i = 0; i < _fluidsAndMoleFractions.Length; ++i)
      {
        var (fluid, xi) = _fluidsAndMoleFractions[i];
        if (xi <= 0)
          continue;

        sum += xi * fluid.PhiR_deltatau_OfReducedVariables(delta, tau);
      }

      return sum;
    }

    #endregion Helmholtz equation of state

    #region Derived quantities

    public override IEnumerable<double> MoleDensityEstimates_FromPressureAndTemperature(double pressure, double temperature)
    {
      foreach (var fluidAndMF in _fluidsAndMoleFractions)
      {
        foreach (var moleDensityGuess in fluidAndMF.pureFluid.MoleDensityEstimates_FromPressureAndTemperature(pressure, temperature))
          yield return moleDensityGuess;

      }
    }



    #endregion

    #region Mixture definitions and fluid definitions going public

    /// <summary>
    /// Gets the known fluids.
    /// </summary>
    /// <value>
    /// The known fluids as enumeration of tuples, containing the CASRegistryNumber of the fluid, its type, and a function to get an instance of the fluid.
    /// </value>
    public static IEnumerable<(string casNumber, Type type, Func<HelmholtzEquationOfStateOfPureFluidsBySpanEtAl> instanceGetter)> KnownFluids
    {
      get
      {
        foreach (var entry in _fluidDefinitions)
        {
          yield return (
                      entry.Key,
                      entry.Value,
                      () =>
                      {
                        var pd = entry.Value.GetProperty("Instance")?.GetGetMethod() ?? throw new InvalidOperationException($"Fluid type {entry.Value} doesn't seem to have an instance property");
                        var definition = (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl)(pd.Invoke(null, null) ?? throw new InvalidOperationException($"Instance property of fluid type {entry.Value} returned null."));
                        return definition;
                      }
          );
        }
      }
    }

    /// <summary>
    /// Tries  to the get fluid from its CAS registry number.
    /// </summary>
    /// <param name="casRegistryNumber">The CAS registry number of the fluid.</param>
    /// <param name="fluid">If the return value is true, an instance of the fluid; otherwise null.</param>
    /// <returns>True if an instance of a fluid with the given CAS registry number could be found; otherwise false.</returns>
    public static bool TryGetFluidFromCasRegistryNumber(string casRegistryNumber, [MaybeNullWhen(false)] out HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid)
    {
      if (_fluidDefinitions.TryGetValue(casRegistryNumber, out var type))
      {
        var pd = type.GetProperty("Instance")?.GetGetMethod() ?? throw new InvalidOperationException($"Fluid type {type} doesn't seem to have an instance property");
        fluid = (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl)(pd.Invoke(null, null) ?? throw new InvalidOperationException($"Instance property of fluid type {type} returned null."));
        return true;
      }

      fluid = null;
      return false;
    }

    /// <summary>
    /// Gets the known binary mixture definitions.
    /// </summary>
    /// <value>
    /// The known mixtures as enumeration of tuples, containing the CASRegistryNumber of the fluid1 and fluid2, the type of the mixture definition, and a function to get an instance of the binary mixture.
    /// </value>
    public static IEnumerable<(string casNumber1, string casNumber2, Type type, Func<BinaryMixtureDefinitionBase> instanceGetter)> KnownBinaryMixtures
    {
      get
      {
        foreach (var entry in _binaryMixtureDefinitions)
        {
          yield return (
                      entry.Key.cas1,
                      entry.Key.cas2,
                      entry.Value,
                      () =>
                      {
                        var pd = entry.Value.GetProperty("Instance")?.GetGetMethod() ?? throw new InvalidOperationException($"Fluid type {entry.Value} doesn't seem to have an instance property");
                        var definition = (BinaryMixtureDefinitionBase)(pd.Invoke(null, null) ?? throw new InvalidOperationException($"Instance property of fluid type {entry.Value} returned null."));
                        return definition;
                      }
          );
        }
      }
    }


    #endregion

    protected static double Pow3(double x) => x * x * x;


    #region Static helpers

    #region Mass fractions to mole fractions

    /// <summary>
    /// Gets the mole fractions from the fluids and the mass fractions of the fluids.
    /// </summary>
    /// <param name="list">The list of fluids and mass fractions of the fluids.</param>
    /// <returns>An array with the mole fractions of the fluids.</returns>
    public static double[] GetMoleFractionsFromFluidsAndMassFractions(params (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid, double MassFraction)[] list)
    {
      return GetMoleFractionsFromMolecularWeightsAndMassFractions(list.Select(entry => (entry.fluid.MolecularWeight, entry.MassFraction)));
    }


    /// <summary>
    /// Gets the mole fractions from the fluids and the mass fractions of the fluids.
    /// </summary>
    /// <param name="list">The list of fluids and mass fractions of the fluids.</param>
    /// <returns>An array with the mole fractions of the fluids.</returns>
    public static double[] GetMoleFractionsFromFluidsAndMassFractions(IEnumerable<(HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid, double MassFraction)> list)
    {
      return GetMoleFractionsFromMolecularWeightsAndMassFractions(list.Select(entry => (entry.fluid.MolecularWeight, entry.MassFraction)));
    }

    /// <summary>
    /// Gets the mole fractions from the molecular weights and mass fractions of the fluids.
    /// </summary>
    /// <param name="list">The list of molecular weights and mass fractions of the fluids.</param>
    /// <returns>An array with the mole fractions of the fluids.</returns>
    public static double[] GetMoleFractionsFromMolecularWeightsAndMassFractions(params (double MolecularWeight, double MassFraction)[] list)
    {
      return GetMoleFractionsFromMolecularWeightsAndMassFractions((IEnumerable<(double MolecularWeight, double MassFraction)>)list);
    }

    /// <summary>
    /// Gets the mole fractions from the molecular weights and mass fractions of the fluids.
    /// </summary>
    /// <param name="list">The list of molecular weights and mass fractions of the fluids.</param>
    /// <returns>An array with the mole fractions of the fluids.</returns>
    public static double[] GetMoleFractionsFromMolecularWeightsAndMassFractions(IEnumerable<(double MolecularWeight, double MassFraction)> list)
    {
      var result = new double[list.Count()];

      // now that we have all fluids, we can calculate mole fractions
      double sum = 0;
      foreach (var (MolecularWeight, massFraction) in list)
      {
        sum += massFraction / MolecularWeight;
      }

      int i = 0;
      foreach (var (MolecularWeight, massFraction) in list)
      {
        result[i++] = (massFraction / MolecularWeight) / sum;
      }
      return result;
    }

    #endregion

    #region Mass fractions to mole fractions

    /// <summary>
    /// Gets the mass fractions from the fluids and the mole fractions of the fluids.
    /// </summary>
    /// <param name="list">The list of fluids and mole fractions of the fluids.</param>
    /// <returns>An array with the mass fractions of the fluids.</returns>
    public static double[] GetMassFractionsFromFluidsAndMoleFractions(params (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid, double MoleFraction)[] list)
    {
      return GetMassFractionsFromMolecularWeightsAndMoleFractions(list.Select(entry => (entry.fluid.MolecularWeight, entry.MoleFraction)));
    }


    /// <summary>
    /// Gets the mass fractions from the fluids and the mole fractions of the fluids.
    /// </summary>
    /// <param name="list">The list of fluids and mole fractions of the fluids.</param>
    /// <returns>An array with the mass fractions of the fluids.</returns>
    public static double[] GetMassFractionsFromFluidsAndMoleFractions(IEnumerable<(HelmholtzEquationOfStateOfPureFluidsBySpanEtAl fluid, double MoleFraction)> list)
    {
      return GetMassFractionsFromMolecularWeightsAndMoleFractions(list.Select(entry => (entry.fluid.MolecularWeight, entry.MoleFraction)));
    }

    /// <summary>
    /// Gets the mass fractions from the molecular weights and mole fractions of the fluids.
    /// </summary>
    /// <param name="list">The list of molecular weights and mole fractions of the fluids.</param>
    /// <returns>An array with the mass fractions of the fluids.</returns>
    public static double[] GetMassFractionsFromMolecularWeightsAndMoleFractions(params (double MolecularWeight, double MoleFraction)[] list)
    {
      return GetMassFractionsFromMolecularWeightsAndMoleFractions((IEnumerable<(double MolecularWeight, double MoleFraction)>)list);
    }

    /// <summary>
    /// Gets the mass fractions from the molecular weights and mole fractions of the fluids.
    /// </summary>
    /// <param name="list">The list of molecular weights and mole fractions of the fluids.</param>
    /// <returns>An array with the mass fractions of the fluids.</returns>
    public static double[] GetMassFractionsFromMolecularWeightsAndMoleFractions(IEnumerable<(double MolecularWeight, double MoleFraction)> list)
    {
      var result = new double[list.Count()];

      // now that we have all fluids, we can calculate mole fractions
      double sum = 0;
      foreach (var (MolecularWeight, moleFraction) in list)
      {
        sum += moleFraction * MolecularWeight;
      }

      int i = 0;
      foreach (var (MolecularWeight, moleFraction) in list)
      {
        result[i++] = (moleFraction * MolecularWeight) / sum;
      }
      return result;
    }

    #endregion

    #endregion
  }
}
