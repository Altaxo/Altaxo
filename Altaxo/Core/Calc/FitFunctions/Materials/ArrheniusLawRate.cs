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
  /// Represents the Arrhenius law for rates. Describes the temperature dependence of reaction rates or similar quantities that increase with temperature.
  /// </summary>
  [FitFunctionClass]
  public class ArrheniusLawRate : IFitFunction, Main.IImmutable
  {
    private TemperatureRepresentation _temperatureUnitOfX;
    private EnergyRepresentation _paramEnergyUnit;

    /// <summary>
    /// Initializes a new default instance using Kelvin and Joule per mole.
    /// </summary>
    public ArrheniusLawRate()
    {
      _temperatureUnitOfX = TemperatureRepresentation.Kelvin;
      _paramEnergyUnit = EnergyRepresentation.JoulePerMole;
    }

    private ArrheniusLawRate(TemperatureRepresentation temperatureUnitOfX, TransformedValueRepresentation dependentVariableTransform, EnergyRepresentation paramEnergyUnit)
    {
      _temperatureUnitOfX = temperatureUnitOfX;
      _paramEnergyUnit = paramEnergyUnit;
    }

    /// <summary>
    /// Initializes a new instance with the specified units for the independent variable and energy parameter.
    /// </summary>
    /// <param name="temperatureUnitOfX">The temperature unit used for the independent variable.</param>
    /// <param name="paramEnergyUnit">The energy unit used for the energy parameter.</param>
    public ArrheniusLawRate(TemperatureRepresentation temperatureUnitOfX, EnergyRepresentation paramEnergyUnit)
    {
      _temperatureUnitOfX = temperatureUnitOfX;
      _paramEnergyUnit = paramEnergyUnit;
    }


    /// <summary>
    /// Gets the energy unit used for the parameter representing activation energy.
    /// </summary>
    [Category("OptionsForParameters")]
    public EnergyRepresentation ParameterEnergyRepresentation => _paramEnergyUnit;

    /// <summary>
    /// Returns a copy of this instance with the provided energy unit for the parameter.
    /// </summary>
    /// <param name="value">The new energy unit.</param>
    [Category("OptionsForParameters")]
    public ArrheniusLawRate WithParameterEnergyRepresentation(EnergyRepresentation value)
    {
      if (!(_paramEnergyUnit == value))
      {
        return new ArrheniusLawRate(_temperatureUnitOfX, value);
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the representation used for the independent temperature variable.
    /// </summary>
    [Category("OptionsForIndependentVariables")]
    public TemperatureRepresentation IndependentVariableRepresentation => _temperatureUnitOfX;

    /// <summary>
    /// Returns a copy of this instance with the provided temperature unit for the independent variable.
    /// </summary>
    /// <param name="value">The new temperature unit.</param>
    [Category("OptionsForIndependentVariables")]
    public ArrheniusLawRate WithIndependentVariableRepresentation(TemperatureRepresentation value)
    {
      if (!(_temperatureUnitOfX == value))
      {
        return new ArrheniusLawRate(value, _paramEnergyUnit);
      }
      else
      {
        return this;
      }
    }

    #region Serialization

    /// <summary>
    /// Initial version 2021-05-10.
    /// V1: 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Materials.ArrheniusLawRate", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ArrheniusLawRate), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ArrheniusLawRate)obj;
        info.AddEnum("IndependentVariableUnit", s._temperatureUnitOfX);
        info.AddEnum("ParamEnergyUnit", s._paramEnergyUnit);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        ArrheniusLawRate s = (ArrheniusLawRate?)o ?? new ArrheniusLawRate();

        var temperatureUnitOfX = (TemperatureRepresentation)info.GetEnum("IndependentVariableUnit", typeof(TemperatureRepresentation));
        var paramEnergyUnit = (EnergyRepresentation)info.GetEnum("ParamEnergyUnit", typeof(EnergyRepresentation));

        return new ArrheniusLawRate(temperatureUnitOfX, paramEnergyUnit);
      }
    }



    #endregion Serialization

    /// <inheritdoc/>
    public override string ToString()
    {
      return "ArrheniusLaw(Rate)";
    }

    /// <summary>
    /// Factory used by discovery to create the default Arrhenius rate fit function.
    /// </summary>
    /// <returns>A new instance of <see cref="ArrheniusLawRate"/> with default units.</returns>
    [FitFunctionCreator("Arrhenius law (rate)", "Materials", 1, 1, 2)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Materials.ArrheniusLawRate}")]
    public static IFitFunction CreateDefault()
    {
      return new ArrheniusLawRate();
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
    public string DependentVariableName(int i)
    {
      return "y";
    }

    /// <inheritdoc/>
    public string ParameterName(int i)
    {
      return i switch
      {
        0 => "y0",
        1 => "E_" + _paramEnergyUnit.ToString(),
        _ => throw new ArgumentOutOfRangeException(nameof(i)),
      };
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

    /// <inheritdoc/>
    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double temperature = Temperature.ToKelvin(X[0], _temperatureUnitOfX);
      double energyAsTemperature = Energy.ToTemperatureSI(P[1], _paramEnergyUnit);
      Y[0] = P[0] * Math.Exp(-energyAsTemperature / temperature);
    }
    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        double temperature = Temperature.ToKelvin(x, _temperatureUnitOfX);
        double energyAsTemperature = Energy.ToTemperatureSI(P[1], _paramEnergyUnit);
        FV[r] = P[0] * Math.Exp(-energyAsTemperature / temperature);
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

  }
}
