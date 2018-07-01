using NUnit.Framework;

using Altaxo.Science.Thermodynamics.Fluids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{
	public class FluidTestBase
	{
		protected HelmholtzEquationOfStateOfPureFluidsByWagnerEtAl _fluid;

		/// <summary>
		// TestData contains:
		// 0. Temperature (Kelvin)
		// 1. Pressure (Pa)
		// 2. Saturated liquid mole density (mol/m³)
		// 3. Saturated vapor mole density (mol/m³)
		/// </summary>
		protected double[][] _testDataSaturatedProperties;

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
		protected double[][] _testDataEquationOfState;

		public virtual void SaturatedVaporPressure_TestMonotony()
		{
			const double relativePressureErrorAllowed = 5E-2;
			double pressure;

			pressure = _fluid.SaturatedVaporPressureEstimate_FromTemperature(_fluid.TriplePointTemperature);
			Assert.Less(GetRelativeErrorBetween(pressure, _fluid.TriplePointPressure), relativePressureErrorAllowed, "Deviation of triple point pressure of {0}", _fluid.GetType().ToString());

			pressure = _fluid.SaturatedVaporPressureEstimate_FromTemperature(_fluid.CriticalPointTemperature);
			Assert.Less(GetRelativeErrorBetween(pressure, _fluid.CriticalPointPressure), relativePressureErrorAllowed, "Deviation of critical point pressure of {0}", _fluid.GetType().ToString());

			// now test monotony

			double startTemperature = Math.Ceiling(_fluid.TriplePointTemperature);
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

			double startTemperature = Math.Ceiling(_fluid.TriplePointTemperature);
			double endTemperature = Math.Floor(_fluid.CriticalPointTemperature);

			for (double temperature = startTemperature; temperature <= endTemperature; temperature += 0.25)
			{
				var pressure = _fluid.SaturatedVaporPressureEstimate_FromTemperature(temperature);

				var temperatureCalcBack = _fluid.SaturatedVaporTemperature_FromPressure(pressure, 1E-6);

				Assert.Less(GetRelativeErrorBetween(temperature, temperatureCalcBack), relativeTemperatureErrorAllowed, "Calculation of vapor pressure from temperature faild for fluid {0}", _fluid.GetType().ToString());
			}
		}

		public virtual void SaturatedData_Test()
		{
			for (int i = 0; i < _testDataSaturatedProperties.Length; ++i)
			{
				var item = _testDataSaturatedProperties[i];

				var temperature = item[0];
				var pressure = item[1];
				var satLiquidMoleDensity = item[2];
				var satVaporMoleDensity = item[3];

				var pressureCalc = _fluid.SaturatedVaporPressureEstimate_FromTemperature(temperature);
				var satLiquidMoleDensityCalc = _fluid.SaturatedLiquidMoleDensityEstimate_FromTemperature(temperature);
				var satVaporMoleDensityCalc = _fluid.SaturatedVaporMoleDensityEstimate_FromTemperature(temperature);

				Assert.IsTrue(IsInToleranceLevel(pressure, pressureCalc, 5E-2, 0));
				Assert.IsTrue(IsInToleranceLevel(satLiquidMoleDensity, satLiquidMoleDensityCalc, 5E-2, 0));
				Assert.IsTrue(IsInToleranceLevel(satVaporMoleDensity, satVaporMoleDensityCalc, 5E-2, 0));
			}
		}

		public virtual void SublimationLineData_Test()
		{
		}

		public virtual void MeltingLineData_Test()
		{
		}

		public virtual void EquationOfState_Test()
		{
			var material = _fluid;

			var methods = new(string colName, Func<double, double, double> call, int index, double relTol, double absTol)[]
			{
								("Pressure", material.Pressure_FromMoleDensityAndTemperature, 2, 1E-6, 1E-3),
								("InternalEnergy", material.MoleSpecificInternalEnergy_FromMoleDensityAndTemperature, 3, 1E-7, 1E-5),
								("Enthalpy", material.MoleSpecificEnthalpy_FromMoleDensityAndTemperature,4, 1E-7, 1E-7 ),
								("Entropy", material.MoleSpecificEntropy_FromMoleDensityAndTemperature, 5, 1E-7, 1E-7),
								("Cv", material.MoleSpecificIsochoricHeatCapacity_FromMoleDensityAndTemperature, 6, 1E-7, 1E-7),
								("Cp", material.MoleSpecificIsobaricHeatCapacity_FromMoleDensityAndTemperature, 7, 1E-7, 1E-7),
								("SpeedOfSound", material.SpeedOfSound_FromMoleDensityAndTemperature,  8, 1E-7, 0.01),
		};

			for (int i = 0; i < _testDataEquationOfState.Length; ++i)
			{
				var item = _testDataEquationOfState[i];
				var temperature = item[0];
				var moleDensity = item[1];

				Assert.IsFalse(!(temperature > 0 && temperature <= _fluid.UpperTemperatureLimit));
				Assert.IsFalse(!(moleDensity > 0 && moleDensity <= _fluid.UpperMoleDensityLimit));
				// var densityCalculatedBack = material.MoleDensity_FromPressureAndTemperature(pressure, temperature, 1E-7, density);
				// Assert.IsTrue(IsInToleranceLevel(density, densityCalculatedBack, 1E-6, 0), "Density deviation, expected: {0} but was {1}", density, densityCalculatedBack);

				foreach (var (colName, call, index, relTol, absTol) in methods)
				{
					double valueCalculated = call(moleDensity, temperature);
					var valueStored = item[index];

					Assert.IsFalse(double.IsNaN(valueStored), "Row[{0}] : {1} defect", i, colName);
					Assert.IsTrue(IsInToleranceLevel(valueStored, valueCalculated, relTol, absTol), "Row[{0}, T={1} K, d={2} mol/m³]: {3} deviation, expected {4}, current {5}", i, temperature, moleDensity, colName, valueStored, valueCalculated);
				}
			}
		}

		public static bool IsInToleranceLevel(double expected, double actual, double relativeError, double absoluteError)
		{
			var diff = Math.Abs(expected * relativeError) + Math.Abs(absoluteError);
			return Math.Abs(expected - actual) <= diff;
		}

		public static double GetRelativeErrorBetween(double x, double y)
		{
			var min = Math.Min(Math.Abs(x), Math.Abs(y));

			if (double.IsNaN(min) || double.IsInfinity(min))
				return double.PositiveInfinity;
			else if (min == 0)
				return x == y ? 0 : double.PositiveInfinity;
			else
				return Math.Abs(x - y) / min;
		}
	}
}
