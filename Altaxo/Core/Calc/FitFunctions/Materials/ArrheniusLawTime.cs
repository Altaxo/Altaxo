#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Science;

namespace Altaxo.Calc.FitFunctions.Materials
{
  /// <summary>
  /// Represents the Arrhenius law for times. Describes the temperature dependence of relaxation time, viscosity, etc., i.e. quantities which decrease with increasing temperature.
  /// </summary>
  [FitFunctionClass]
  public class ArrheniusLawTime : IFitFunction, IFitFunctionWithDerivative, Main.IImmutable
  {
    private TemperatureRepresentation _temperatureUnitOfX;
    private EnergyRepresentation _paramEnergyUnit;

    /// <summary>
    /// Initializes a new default instance using Kelvin and Joule per mole.
    /// </summary>
    public ArrheniusLawTime()
    {
      _temperatureUnitOfX = TemperatureRepresentation.Kelvin;
      _paramEnergyUnit = EnergyRepresentation.JoulePerMole;
    }

    /// <summary>
    /// Initializes a new instance with specified units.
    /// </summary>
    /// <param name="temperatureUnitOfX">Temperature unit for the independent variable.</param>
    /// <param name="paramEnergyUnit">Energy unit for the energy parameter.</param>
    public ArrheniusLawTime(TemperatureRepresentation temperatureUnitOfX, EnergyRepresentation paramEnergyUnit)
    {
      _temperatureUnitOfX = temperatureUnitOfX;
      _paramEnergyUnit = paramEnergyUnit;
    }





    [Category("OptionsForParameters")]
    public EnergyRepresentation ParameterEnergyRepresentation => _paramEnergyUnit;

    [Category("OptionsForParameters")]
    public ArrheniusLawTime WithParameterEnergyRepresentation(EnergyRepresentation value)
    {
      if (_paramEnergyUnit == value)
      {
        return this;
      }
      else
      {
        var result = (ArrheniusLawTime)MemberwiseClone();
        result._paramEnergyUnit = value;
        return result;
      }
    }


    [Category("OptionsForIndependentVariables")]
    public TemperatureRepresentation IndependentVariableRepresentation => _temperatureUnitOfX;


    [Category("OptionsForIndependentVariables")]
    public ArrheniusLawTime WithIndependentVariableRepresentation(TemperatureRepresentation value)
    {
      if (_temperatureUnitOfX == value)
      {
        return this;
      }
      else
      {
        var result = (ArrheniusLawTime)MemberwiseClone();
        result._temperatureUnitOfX = value;
        return result;
      }
    }

    #region Serialization

    /// <summary>
    /// 2021-05-23 initial version
    /// V1: 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Materials.ArrheniusLawTime", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ArrheniusLawTime), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ArrheniusLawTime)obj;
        info.AddEnum("IndependentVariableUnit", s._temperatureUnitOfX);
        info.AddEnum("ParamEnergyUnit", s._paramEnergyUnit);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        ArrheniusLawTime s = (ArrheniusLawTime?)o ?? new ArrheniusLawTime();

        s._temperatureUnitOfX = (TemperatureRepresentation)info.GetEnum("IndependentVariableUnit", typeof(TemperatureRepresentation));
        s._paramEnergyUnit = (EnergyRepresentation)info.GetEnum("ParamEnergyUnit", typeof(EnergyRepresentation));

        return s;
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public override string ToString()
    {
      return "ArrheniusLaw (Time)";
    }

    /// <summary>
    /// Factory used by discovery to create the default Arrhenius time fit function.
    /// </summary>
    /// <returns>A new instance of <see cref="ArrheniusLawTime"/> with default units.</returns>
    [FitFunctionCreator("Arrhenius law (time)", "Materials", 1, 1, 2)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Materials.ArrheniusLawTime}")]
    public static IFitFunction CreateDefault()
    {
      return new ArrheniusLawTime();
    }

    #region IFitFunction Members

    /// <inheritdoc/>
    public int NumberOfIndependentVariables
    {
      get
      {
        return 1;
      }
    }

    /// <inheritdoc/>
    public int NumberOfDependentVariables
    {
      get
      {
        return 1;
      }
    }

    /// <inheritdoc/>
    public int NumberOfParameters
    {
      get
      {
        return 2;
      }
    }

    /// <inheritdoc/>
    public string IndependentVariableName(int i)
    {
      return "T_" + _temperatureUnitOfX.ToString();
    }

    /// <inheritdoc/>
    public virtual string DependentVariableName(int i)
    {
      return "y";
    }

    /// <inheritdoc/>
    public string ParameterName(int i)
    {
      switch (i)
      {
        case 0:
          return "y0";

        case 1:
          return "E_" + _paramEnergyUnit.ToString();

        default:
          throw new ArgumentOutOfRangeException(nameof(i));
      }
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      switch (i)
      {
        case 0:
          return 1;

        case 1:
          var en = new Energy(80000, EnergyRepresentation.JoulePerMole);
          return en.ConvertTo(_paramEnergyUnit).Value;

        default:
          throw new ArgumentOutOfRangeException(nameof(i));
      }
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }



    /// <summary>
    /// Not used (instance is immutable).
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion Change event

    /// <summary>
    /// Evaluates the Arrhenius time law for a single X value and parameter list.
    /// </summary>
    /// <param name="X">The independent variable value (temperature).</param>
    /// <param name="P">Parameters array, where P[0] is prefactor and P[1] is activation energy.</param>
    /// <returns>The evaluated value.</returns>
    public virtual double Evaluate(double X, IReadOnlyList<double> P)
    {
      double temperature = Temperature.ToKelvin(X, _temperatureUnitOfX);
      double energyAsTemperature = Energy.ToTemperatureSI(P[1], _paramEnergyUnit);
      return Math.Exp(Math.Log(P[0]) + energyAsTemperature / temperature);
    }

    /// <inheritdoc/>
    public virtual void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = Evaluate(X[0], P);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        FV[r] = Evaluate(x, P);
      }
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return (null, null);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      double factorParam1ToJoule = Energy.ToJouleFactor(_paramEnergyUnit) / SIConstants.BOLTZMANN;
      double energyAsTemperature = Energy.ToTemperatureSI(parameters[1], _paramEnergyUnit);

      for (int r = 0; r < independent.RowCount; ++r)
      {
        var x = independent[r, 0];
        double temperature = Temperature.ToKelvin(x, _temperatureUnitOfX);
        var arg = energyAsTemperature / temperature;
        DF[r, 0] = Math.Exp(arg);
        DF[r, 1] = Math.Exp(Math.Log(parameters[0]) + arg) * (factorParam1ToJoule / temperature);
      }
    }
  }
}
