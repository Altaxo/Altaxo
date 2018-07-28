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

using NUnit.Framework;

using Altaxo.Science.Thermodynamics.Fluids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{
  public class FluidTestBase : TestBase
  {
    protected HelmholtzEquationOfStateOfPureFluidsBySpanEtAl _fluid;

    /// <summary>
    // TestData contains:
    // 0. Temperature (Kelvin)
    // 1. Pressure (Pa)
    // 2. Saturated liquid mole density (mol/m³)
    // 3. Saturated vapor mole density (mol/m³)
    /// </summary>
    protected (double temperature, double pressure, double saturatedLiquidMoleDensity, double saturatedVaporMoleDensity)[] _testDataSaturatedProperties;

    protected (double temperature, double pressure)[] _testDataSublimationLine;

    protected (double temperature, double pressure)[] _testDataMeltingLine;

    /// <summary>
    /// TestData contains:
    /// 0. Temperature (Kelvin)
    /// 1. Mole density (mol/m³)
    /// 2. Pressure (Pa)
    /// 3. Internal energy (J/mol)
    /// 4. Enthalpy (J/mol)
    /// 5. Entropy (J/mol K)
    /// 6. Isochoric heat capacity (J/(mol K))
    /// 7. Isobaric heat capacity (J/(mol K))
    /// 8. Speed of sound (m/s)
    /// </summary>
    protected (double temperature, double moleDensity, double pressure, double internalEnergy, double enthalpy, double entropy, double isochoricHeatCapacity, double isobaricHeatCapacity, double speedOfSound)[] _testDataEquationOfState;

    public virtual void CASNumberAttribute_Test()
    {
      var casNumber = _fluid.CASRegistryNumber;
      Assert.IsFalse(string.IsNullOrEmpty(casNumber));

      var type = _fluid.GetType();
      var attr = type.GetCustomAttributes(typeof(CASRegistryNumberAttribute), false);
      Assert.AreEqual(1, attr.Length, "Exactly one CASAttribute must be assigned");
      Assert.IsTrue(attr[0] is CASRegistryNumberAttribute);
      var casNumberAttribute = attr[0] as CASRegistryNumberAttribute;
      Assert.AreEqual(casNumber, casNumberAttribute.CASRegistryNumber, "The CAS registry number as returned by the instance must be equal to that stored inside the CASRegistryNumberAttribute");
    }

    public virtual void SaturatedVaporPressure_TestMonotony()
    {
      const double relativePressureErrorAllowed = 5E-2;
      double pressure;

      if (_fluid.TriplePointTemperature >= _fluid.LowerTemperatureLimit)
      {
        pressure = _fluid.SaturatedVaporPressureEstimate_FromTemperature(_fluid.TriplePointTemperature);
        Assert.AreEqual(_fluid.TriplePointPressure, pressure, GetAllowedError(_fluid.TriplePointPressure, relativePressureErrorAllowed, 1e-6), "Deviation of triple point pressure of {0}", _fluid.GetType().ToString());
      }

      if (_fluid.LowerTemperatureLimit <= _fluid.CriticalPointTemperature && _fluid.CriticalPointTemperature <= _fluid.UpperTemperatureLimit)
      {
        pressure = _fluid.SaturatedVaporPressureEstimate_FromTemperature(_fluid.CriticalPointTemperature);
        Assert.AreEqual(_fluid.CriticalPointPressure, pressure, GetAllowedError(_fluid.CriticalPointPressure, relativePressureErrorAllowed, 0), "Deviation of critical point pressure of {0}", _fluid.GetType().ToString());
      }

      // now test monotony

      double startTemperature = Math.Ceiling(Math.Max(_fluid.TriplePointTemperature, _fluid.LowerTemperatureLimit));
      double endTemperature = Math.Floor(_fluid.CriticalPointTemperature);
      double pressureDerivative;

      double previousTemperature = _fluid.TriplePointTemperature;
      double previousPressure = _fluid.SaturatedVaporPressureEstimate_FromTemperature(previousTemperature);
      for (double temperature = startTemperature; temperature <= endTemperature; temperature += 0.25)
      {
        pressure = _fluid.SaturatedVaporPressureEstimate_FromTemperature(temperature);

        if (previousTemperature < temperature - 0.1)
        {
          Assert.Greater(pressure, previousPressure, "Monotony of saturated vapor pressure curve not given for {0}", _fluid.GetType().ToString());
        }

        previousTemperature = temperature;
        previousPressure = pressure;
      }

      // Test derivative of pressure wrt temperature
      startTemperature = Math.Ceiling((0.75 * _fluid.TriplePointTemperature + 0.25 * _fluid.CriticalPointTemperature) * 16) / 16;
      endTemperature = Math.Floor(_fluid.CriticalPointTemperature * 16) / 16;
      double temperatureStep = 1.0 / 16;
      previousTemperature = _fluid.TriplePointTemperature;
      previousPressure = _fluid.SaturatedVaporPressureEstimate_FromTemperature(previousTemperature);
      for (double temperature = startTemperature; temperature <= endTemperature; temperature += temperatureStep)
      {
        (pressure, pressureDerivative) = _fluid.SaturatedVaporPressureEstimateAndDerivativeWrtTemperature_FromTemperature(temperature);

        if ((temperature - previousTemperature) <= temperatureStep)
        {
          Assert.Greater(pressure, previousPressure, "Monotony of saturated vapor pressure curve not given for {0}", _fluid.GetType().ToString());

          var derivEstimate = (pressure - previousPressure) / (temperature - previousTemperature);

          Assert.Less(GetRelativeErrorBetween(derivEstimate, pressureDerivative), 1, "Great deviation between pressure derivative and difference estimation of derivative for fluid {0}", _fluid.GetType().ToString());
        }

        previousTemperature = temperature;
        previousPressure = pressure;
      }
    }

    public virtual void SaturatedVaporPressure_TestInverseIteration()
    {
      const double relativeTemperatureErrorAllowed = 1E-6;

      // for each point between triple point and critical point,
      // the iteration of pressure must work!

      double startTemperature = Math.Max(Math.Ceiling(_fluid.TriplePointTemperature + 0.25), _fluid.LowerTemperatureLimit);
      double endTemperature = Math.Min(Math.Floor(_fluid.CriticalPointTemperature), _fluid.UpperTemperatureLimit);

      for (double temperature = startTemperature; temperature <= endTemperature; temperature += 0.25)
      {
        var pressure = _fluid.SaturatedVaporPressureEstimate_FromTemperature(temperature);

        var temperatureCalcBack = _fluid.SaturatedVaporTemperature_FromPressure(pressure, 1E-6);

        Assert.AreEqual(temperature, temperatureCalcBack, GetAllowedError(temperature, relativeTemperatureErrorAllowed, 0), "Calculation of temperature from pressure p={1} Pa failed for fluid {0}", _fluid.GetType().ToString(), pressure);
      }
    }

    public virtual void SaturatedVaporProperties_TestData()
    {
      for (int i = 0; i < _testDataSaturatedProperties.Length; ++i)
      {
        var item = _testDataSaturatedProperties[i];

        var temperature = item.temperature;
        var pressure = item.pressure;
        var satLiquidMoleDensity = item.saturatedLiquidMoleDensity;
        var satVaporMoleDensity = item.saturatedVaporMoleDensity;

        var pressureCalc = _fluid.SaturatedVaporPressureEstimate_FromTemperature(temperature);
        var satLiquidMoleDensityCalc = _fluid.SaturatedLiquidMoleDensityEstimate_FromTemperature(temperature);
        var satVaporMoleDensityCalc = _fluid.SaturatedVaporMoleDensityEstimate_FromTemperature(temperature);

        Assert.AreEqual(pressure, pressureCalc, GetAllowedError(pressure, 5E-2, 1e-5));
        Assert.AreEqual(satLiquidMoleDensity, satLiquidMoleDensityCalc, GetAllowedError(satLiquidMoleDensity, 5E-2, 0));
        Assert.AreEqual(satVaporMoleDensity, satVaporMoleDensityCalc, GetAllowedError(satVaporMoleDensity, 5E-2, 0));
      }
    }

    public virtual void SublimationPressure_TestImplemented()
    {
      Assert.AreEqual(_testDataIsSublimationCurveImplemented, _fluid.IsSublimationPressureCurveImplemented);
    }

    public virtual void SublimationLineData_Test()
    {
      if (null == _testDataSublimationLine)
      {
        Assert.IsFalse(_fluid.IsSublimationPressureCurveImplemented);
        return;
      }

      SublimationPressure_TestMonotony();

      SublimationPressure_TestDerivative();

      SublimationPressure_TestData();

      SublimationTemperature_TestData();
    }

    public virtual void SublimationPressure_TestMonotony()
    {
      const double relativePressureErrorAllowed = 5E-2;
      double pressure;

      pressure = _fluid.SublimationPressureEstimate_FromTemperature(_fluid.TriplePointTemperature);
      Assert.AreEqual(_fluid.TriplePointPressure, pressure, GetAllowedError(_fluid.TriplePointPressure, relativePressureErrorAllowed, 0), "Deviation of triple point pressure of {0}", _fluid.GetType().ToString());

      // now test monotony

      double startTemperature = Math.Floor(_fluid.TriplePointTemperature);
      double endTemperature = Math.Ceiling(_fluid.TriplePointTemperature / 2);
      double pressureDerivative;

      double previousTemperature = _fluid.TriplePointTemperature;
      double previousPressure = _fluid.SublimationPressureEstimate_FromTemperature(previousTemperature);
      for (double temperature = startTemperature; temperature >= endTemperature; temperature -= 0.125)
      {
        pressure = _fluid.SublimationPressureEstimate_FromTemperature(temperature);

        if (temperature < previousTemperature - 0.1)
        {
          Assert.Less(pressure, previousPressure, "Monotony of sublimation pressure curve not given for {0} at temperature={1}, previous temperature={2}", _fluid.GetType().ToString(), temperature, previousTemperature);
        }

        previousTemperature = temperature;
        previousPressure = pressure;
      }

      // Test derivative of pressure wrt temperature
      startTemperature = Math.Ceiling((0.75 * _fluid.TriplePointTemperature + 0.25 * _fluid.CriticalPointTemperature) * 16) / 16;
      endTemperature = Math.Floor(_fluid.CriticalPointTemperature * 16) / 16;
      double temperatureStep = 1.0 / 16;
      previousTemperature = _fluid.TriplePointTemperature;
      previousPressure = _fluid.SaturatedVaporPressureEstimate_FromTemperature(previousTemperature);
      for (double temperature = startTemperature; temperature <= endTemperature; temperature += temperatureStep)
      {
        (pressure, pressureDerivative) = _fluid.SaturatedVaporPressureEstimateAndDerivativeWrtTemperature_FromTemperature(temperature);

        if ((temperature - previousTemperature) <= temperatureStep)
        {
          Assert.Greater(pressure, previousPressure, "Monotony of saturated vapor pressure curve not given for {0}", _fluid.GetType().ToString());

          var derivEstimate = (pressure - previousPressure) / (temperature - previousTemperature);

          Assert.AreEqual(derivEstimate, pressureDerivative, GetAllowedError(derivEstimate, 1, 0), "Great deviation between pressure derivative and difference estimation of derivative for fluid {0}", _fluid.GetType().ToString());
        }

        previousTemperature = temperature;
        previousPressure = pressure;
      }
    }

    public virtual void SublimationPressure_TestDerivative()
    {
      const double relativeErrorAllowed = 5E-1;
      const double absErrorAllowed = 0;
      double pressure;

      // now test derivative

      double startTemperature = Math.Floor(_fluid.TriplePointTemperature);
      double endTemperature = Math.Ceiling(_fluid.TriplePointTemperature / 2);
      double temperatureStep = 1 / 8.0;
      double pressureDerivative;

      double previousTemperature = _fluid.TriplePointTemperature;
      double previousPressure = _fluid.SublimationPressureEstimate_FromTemperature(previousTemperature);
      for (double temperature = startTemperature; temperature >= endTemperature; temperature -= temperatureStep)
      {
        (pressure, pressureDerivative) = _fluid.SublimationPressureEstimateAndDerivativeWrtTemperature_FromTemperature(temperature);

        if ((temperature - previousTemperature) <= temperatureStep)
        {
          var derivEstimate = (pressure - previousPressure) / (temperature - previousTemperature);

          Assert.AreEqual(derivEstimate, pressureDerivative, GetAllowedError(derivEstimate, relativeErrorAllowed, absErrorAllowed), "Great deviation between pressure derivative and difference estimation of derivative for fluid {0}", _fluid.GetType().ToString());

          previousTemperature = temperature;
          previousPressure = pressure;
        }
      }
    }

    public virtual void SublimationPressure_TestData()
    {
      const double relativeDeviation = 5E-2;
      const double absoluteDeviation = 100; // Pa

      foreach (var (temperature, pressureExpected) in _testDataSublimationLine.Reverse())
      {
        var pressureHere = _fluid.SublimationPressureEstimate_FromTemperature(temperature);
        Assert.AreEqual(pressureExpected, pressureHere, GetAllowedError(pressureExpected, relativeDeviation, absoluteDeviation), "Temperature: {0}", temperature);
      }
    }

    public virtual void SublimationTemperature_TestData()
    {
      const double relativeDeviation = 0;
      double absoluteDeviation = 1; // Kelvin

      foreach (var (temperatureExpected, pressure) in _testDataSublimationLine.Reverse())
      {
        if (temperatureExpected < _fluid.TriplePointTemperature / 2)
          absoluteDeviation = 2; // for very low temperatures, allow more deviation

        var temperatureHere = _fluid.SublimationTemperatureEstimate_FromPressure(pressure);
        Assert.AreEqual(temperatureExpected, temperatureHere, GetAllowedError(temperatureExpected, relativeDeviation, absoluteDeviation), "At a pressure of {0} Pa:", pressure);
      }
    }

    public virtual void MeltingLineData_Test()
    {
      MeltingPressure_TestMonotony();
      MeltingPressure_TestDerivative();
      MeltingPressure_TestData();
      MeltingTemperature_TestData();
    }

    public virtual void MeltingPressure_TestImplemented()
    {
      Assert.AreEqual(_testDataIsMeltingCurveImplemented, _fluid.IsMeltingPressureCurveImplemented);
    }

    public virtual void MeltingPressure_TestMonotony()
    {
      const double relativePressureErrorAllowed = 5E-2;

      if (_fluid.CASRegistryNumber == "7732-18-5") // for water don't test monotony
        return;

      // 2018-07-21: Since some melting pressure models (e.g. for CO) are unable to model the steep increase of pressure
      // in the vicinity of the triple point, I commented-out the following test at the triple point temperature
      // var trpressure = _fluid.MeltingPressureEstimate_FromTemperature(_fluid.TriplePointTemperature);
      // Assert.AreEqual(_fluid.TriplePointPressure, trpressure, GetAllowedError(_fluid.TriplePointPressure, relativePressureErrorAllowed, 0), "Deviation of triple point pressure of {0}", _fluid.GetType().ToString());

      // now test monotony
      double startTemperature = Math.Ceiling(_fluid.TriplePointTemperature);
      double endTemperature = Math.Floor(_fluid.CriticalPointTemperature);
      double temperatureStep = 1 / 8.0;

      double previousTemperature = _fluid.TriplePointTemperature;
      double previousPressure = _fluid.MeltingPressureEstimate_FromTemperature(previousTemperature);
      for (double temperature = startTemperature; temperature <= endTemperature; temperature += temperatureStep)
      {
        var pressure = _fluid.MeltingPressureEstimate_FromTemperature(temperature);

        if (temperature > previousTemperature + 0.1)
        {
          Assert.Greater(pressure, previousPressure, "Monotony of melting pressure curve not given for {0} at temperature={1}, previous temperature={2}", _fluid.GetType().ToString(), temperature, previousTemperature);
        }

        previousTemperature = temperature;
        previousPressure = pressure;
      }
    }

    public virtual void MeltingPressure_TestDerivative()
    {
      const double relativeErrorAllowed = 5E-1;
      const double absErrorAllowed = 0;

      if (_fluid.CASRegistryNumber == "7732-18-5") // for water don't test derivative
        return;

      // now test derivative

      double startTemperature = Math.Ceiling(_fluid.TriplePointTemperature);
      double endTemperature = Math.Floor(_fluid.CriticalPointTemperature);
      double temperatureStep = 1 / 8.0;
      double pressure, pressureDerivative;

      double previousTemperature = _fluid.TriplePointTemperature;
      double previousPressure = _fluid.MeltingPressureEstimate_FromTemperature(previousTemperature);
      for (double temperature = startTemperature; temperature <= endTemperature; temperature += temperatureStep)
      {
        (pressure, pressureDerivative) = _fluid.MeltingPressureEstimateAndDerivativeWrtTemperature_FromTemperature(temperature);

        if (temperature >= previousTemperature + temperatureStep)
        {
          var derivEstimate = (pressure - previousPressure) / (temperature - previousTemperature);

          Assert.AreEqual(derivEstimate, pressureDerivative, GetAllowedError(derivEstimate, relativeErrorAllowed, absErrorAllowed), "Great deviation between pressure derivative and difference estimation of derivative for fluid {0}", _fluid.GetType().ToString());

          previousTemperature = temperature;
          previousPressure = pressure;
        }
      }
    }

    public virtual void MeltingPressure_TestData()
    {
      const double relativeDeviation = 5E-2;
      const double absoluteDeviation = 100; // Pa

      foreach (var (temperature, pressureExpected) in _testDataMeltingLine)
      {
        var pressureHere = _fluid.MeltingPressureEstimate_FromTemperature(temperature);
        Assert.AreEqual(pressureExpected, pressureHere, GetAllowedError(pressureExpected, relativeDeviation, absoluteDeviation), "Temperature: {0}", temperature);
      }
    }

    public virtual void MeltingTemperature_TestData()
    {
      const double relativeDeviation = 0;
      const double absoluteDeviation = 0.5; // Kelvin

      foreach (var (temperatureExpected, pressure) in _testDataMeltingLine)
      {
        var temperatureHere = _fluid.MeltingTemperatureEstimate_FromPressure(pressure);
        Assert.AreEqual(temperatureExpected, temperatureHere, GetAllowedError(temperatureExpected, relativeDeviation, absoluteDeviation), "Pressure: {0}", pressure);
      }
    }

    public virtual void EquationOfState_Test()
    {
      var material = _fluid;

      var methods = new(string colName, Func<double, double, double> call, int index, double relTol, double absTol)[]
      {
                ("Pressure", material.Pressure_FromMoleDensityAndTemperature, 0, 1E-6, 1E-3),
                ("Cv", material.MoleSpecificIsochoricHeatCapacity_FromMoleDensityAndTemperature, 4, 1E-7, 1E-7),
                ("SpeedOfSound", material.SpeedOfSound_FromMoleDensityAndTemperature,  6, 1E-7, 0.01),
                ("InternalEnergy", material.MoleSpecificInternalEnergy_FromMoleDensityAndTemperature, 1, 5E-7, 1E-3),
                ("Enthalpy", material.MoleSpecificEnthalpy_FromMoleDensityAndTemperature, 2, 5E-7, 1E-3 ),
                ("Entropy", material.MoleSpecificEntropy_FromMoleDensityAndTemperature, 3, 5E-7, 1E-4),
                ("Cp", material.MoleSpecificIsobaricHeatCapacity_FromMoleDensityAndTemperature, 5, 1E-7, 1E-7),
    };

      for (int methodIndex = 0; methodIndex < methods.Length; ++methodIndex)
      {
        var method = methods[methodIndex];
        for (int i = 0; i < _testDataEquationOfState.Length; ++i)
        {
          var item = _testDataEquationOfState[i];
          var temperature = item.temperature;
          var moleDensity = item.moleDensity;
          Assert.Greater(temperature, 0, "Temperature has to be positive");
          Assert.LessOrEqual(temperature, _fluid.UpperTemperatureLimit, "Temperature has to be less than or equal to upper temperature limit");
          Assert.Greater(moleDensity, 0, "MoleDensity has to be positive");
          Assert.LessOrEqual(moleDensity, _fluid.UpperMoleDensityLimit, "MoleDensity has to be less than or equal to upper temperature limit");
          var testValues = new[] { item.pressure, item.internalEnergy, item.enthalpy, item.entropy, item.isochoricHeatCapacity, item.isobaricHeatCapacity, item.speedOfSound };
          double valueStored = testValues[method.index];
          double valueCalculated = method.call(moleDensity, temperature);
          Assert.IsFalse(double.IsNaN(valueStored), "Test value row[{0}] : {1} defect (contains NaN)", i, method.colName);
          Assert.AreEqual(valueStored, valueCalculated, GetAllowedError(valueStored, method.relTol, method.absTol), "{3} in row[{0}], T={1} K, d={2} mol/m³", i, temperature, moleDensity, method.colName);
        }
      }
    }

    #region Fluid constants

    protected double _testDataMolecularWeight;
    protected double _testDataTriplePointTemperature;
    protected double _testDataTriplePointPressure;
    protected double _testDataTriplePointLiquidMoleDensity;
    protected double _testDataTriplePointVaporMoleDensity;

    protected double _testDataCriticalPointTemperature;
    protected double _testDataCriticalPointPressure;
    protected double _testDataCriticalPointMoleDensity;

    protected double? _testDataNormalBoilingPointTemperature;
    protected double? _testDataNormalSublimationPointTemperature;

    protected bool _testDataIsMeltingCurveImplemented;

    protected bool _testDataIsSublimationCurveImplemented;

    public virtual void ConstantsAndCharacteristicPoints_Test()
    {
      Assert.IsFalse(string.IsNullOrEmpty(_fluid.CASRegistryNumber));
      Assert.IsFalse(string.IsNullOrEmpty(_fluid.ShortName));
      Assert.IsFalse(string.IsNullOrEmpty(_fluid.FullName));

      Assert.GreaterOrEqual(_fluid.LowerTemperatureLimit, 0);
      Assert.Greater(_fluid.UpperTemperatureLimit, _fluid.LowerTemperatureLimit);
      Assert.Greater(_fluid.UpperPressureLimit, 0);
      Assert.Greater(_fluid.UpperMoleDensityLimit, 0);

      Assert.AreEqual(_testDataMolecularWeight, _fluid.MolecularWeight, GetAllowedError(_testDataMolecularWeight, 1E-14, 0), "MolecularWeight");
      Assert.AreEqual(_testDataTriplePointTemperature, _fluid.TriplePointTemperature, GetAllowedError(_testDataTriplePointTemperature, 1E-4, 0.01), "TriplePointTemperature");
      Assert.AreEqual(_testDataTriplePointPressure, _fluid.TriplePointPressure, GetAllowedError(_testDataTriplePointPressure, 1E-4, 0), "TriplePointPressure");
      Assert.AreEqual(_testDataTriplePointLiquidMoleDensity, _fluid.TriplePointSaturatedLiquidMoleDensity, GetAllowedError(_testDataTriplePointLiquidMoleDensity, 1E-4, 0), "TriplePointSaturatedLiquidMoleDensity");
      Assert.AreEqual(_testDataTriplePointVaporMoleDensity, _fluid.TriplePointSaturatedVaporMoleDensity, GetAllowedError(_testDataTriplePointVaporMoleDensity, 1E-4, 0), "TriplePointSaturatedVaporMoleDensity");
      Assert.AreEqual(_testDataCriticalPointTemperature, _fluid.CriticalPointTemperature, GetAllowedError(_testDataCriticalPointTemperature, 1E-4, 0.01), "CriticalPointTemperature");
      Assert.AreEqual(_testDataCriticalPointPressure, _fluid.CriticalPointPressure, GetAllowedError(_testDataCriticalPointPressure, 1E-2, 0), "CriticalPointPressure");
      Assert.AreEqual(_testDataCriticalPointMoleDensity, _fluid.CriticalPointMoleDensity, GetAllowedError(_testDataCriticalPointMoleDensity, 1E-2, 0), "CriticalPointLiquidMoleDensity");

      if (_testDataNormalBoilingPointTemperature.HasValue)
        Assert.AreEqual(_testDataNormalBoilingPointTemperature.Value, _fluid.NormalBoilingPointTemperature.Value, GetAllowedError(_testDataNormalBoilingPointTemperature.Value, 1E-4, 0.1), "NormalBoilingPointTemperature");
      else
        Assert.IsTrue(_fluid.NormalBoilingPointTemperature is null, "NormalBoilingPointTemperature");

      if (_testDataNormalSublimationPointTemperature.HasValue)
        Assert.AreEqual(_testDataNormalSublimationPointTemperature.Value, _fluid.NormalSublimationPointTemperature.Value, GetAllowedError(_testDataNormalSublimationPointTemperature.Value, 1E-4, 0.1), "NormalSublimationPointTemperature");
      else
        Assert.IsTrue(_fluid.NormalSublimationPointTemperature is null, "NormalSublimationPointTemperature");
    }

    #endregion Fluid constants
  }
}
