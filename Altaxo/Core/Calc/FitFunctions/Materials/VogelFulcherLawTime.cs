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
  /// Represents the Vogel-Fulcher law to describe the temperature dependence of times, viscosities, etc. in glass forming substances,
  /// i.e. quantities which decrease with increasing temperature.
  /// </summary>
  [FitFunctionClass]
  public class VogelFulcherLawTime
        : IFitFunction, Main.IImmutable
  {
    private TemperatureRepresentation _temperatureUnitOfX;
    private TemperatureRepresentation _temperatureUnitOfT0;
    private TemperatureRepresentation _temperatureUnitOfB;

    /// <summary>
    /// Initializes a new instance of the <see cref="VogelFulcherLawTime"/> class with default Kelvin units.
    /// </summary>
    public VogelFulcherLawTime()
    {
      _temperatureUnitOfX = TemperatureRepresentation.Kelvin;
      _temperatureUnitOfT0 = TemperatureRepresentation.Kelvin;
      _temperatureUnitOfB = TemperatureRepresentation.Kelvin;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VogelFulcherLawTime"/> class with the provided temperature unit representations.
    /// </summary>
    /// <param name="temperatureUnitOfX">Unit representation for the independent variable.</param>
    /// <param name="temperatureUnitOfT0">Unit representation for parameter T0.</param>
    /// <param name="temperatureUnitOfB">Unit representation for parameter B.</param>
    public VogelFulcherLawTime(
    TemperatureRepresentation temperatureUnitOfX,
    TemperatureRepresentation temperatureUnitOfT0,
    TemperatureRepresentation temperatureUnitOfB)
    {
      _temperatureUnitOfX = temperatureUnitOfX;
      _temperatureUnitOfT0 = temperatureUnitOfT0;
      _temperatureUnitOfB = temperatureUnitOfB;
    }



    /// <summary>
    /// Gets or sets the temperature unit representation used for the independent variable (temperature).
    /// </summary>
    [Category("OptionsForIndependentVariables")]
    public TemperatureRepresentation IndependentVariableRepresentation
    {
      get { return _temperatureUnitOfX; }
      set { _temperatureUnitOfX = value; }
    }

    /// <summary>
    /// Returns a new instance with the provided independent variable temperature representation.
    /// </summary>
    /// <param name="value">Temperature representation for the independent variable.</param>
    /// <returns>A new <see cref="VogelFulcherLawTime"/> instance or the same instance if unchanged.</returns>
    public VogelFulcherLawTime WithIndependentVariableRepresentation(TemperatureRepresentation value)
    {
      if (value == _temperatureUnitOfX)
      {
        return this;
      }
      else
      {
        var result = (VogelFulcherLawTime)MemberwiseClone();
        result._temperatureUnitOfX = value;
        return result;
      }
    }

    /// <summary>
    /// Gets the temperature unit representation used for the parameter T0.
    /// </summary>
    [Category("OptionsForParameters")]
    public TemperatureRepresentation ParameterT0Representation
    {
      get { return _temperatureUnitOfT0; }
    }

    /// <summary>
    /// Returns a new instance with the provided T0 parameter temperature representation.
    /// </summary>
    /// <param name="value">Temperature representation for parameter T0.</param>
    /// <returns>A new <see cref="VogelFulcherLawTime"/> instance or the same instance if unchanged.</returns>
    public VogelFulcherLawTime WithParameterT0Representation(TemperatureRepresentation value)
    {
      if (value == _temperatureUnitOfT0)
      {
        return this;
      }
      else
      {
        var result = (VogelFulcherLawTime)MemberwiseClone();
        result._temperatureUnitOfT0 = value;
        return result;
      }
    }

    /// <summary>
    /// Gets the temperature unit representation used for the parameter B.
    /// </summary>
    [Category("OptionsForParameters")]
    public TemperatureRepresentation ParameterBRepresentation
    {
      get { return _temperatureUnitOfB; }
    }

    /// <summary>
    /// Returns a new instance with the provided B parameter temperature representation.
    /// </summary>
    /// <param name="value">Temperature representation for parameter B. Celsius and Fahrenheit are not allowed for B.</param>
    /// <returns>A new <see cref="VogelFulcherLawTime"/> instance or the same instance if unchanged.</returns>
    public VogelFulcherLawTime WithParameterBRepresentation(TemperatureRepresentation value)
    {
      if (value == _temperatureUnitOfB)
      {
        return this;
      }
      else
      {
        if (value == TemperatureRepresentation.DegreeCelsius || value == TemperatureRepresentation.DegreeFahrenheit)
          throw new InvalidOperationException("Celsius and Fahrenheit are units with offset, which is not allowed for parameter B");

        var result = (VogelFulcherLawTime)MemberwiseClone();
        result._temperatureUnitOfB = value;
        return result;
      }
    }

    #region Serialization

    /// <summary>
    /// 2021-05-23 Initial version
    /// V1: 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Materials.VogelFulcherLawTime", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VogelFulcherLawTime), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VogelFulcherLawTime)obj;
        info.AddEnum("IndependentVariableUnit", s._temperatureUnitOfX);
        info.AddEnum("ParamBUnit", s._temperatureUnitOfB);
        info.AddEnum("ParamT0Unit", s._temperatureUnitOfT0);
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        VogelFulcherLawTime s = (VogelFulcherLawTime?)o ?? new VogelFulcherLawTime();

        s._temperatureUnitOfX = (TemperatureRepresentation)info.GetEnum("IndependentVariableUnit", typeof(TemperatureRepresentation));
        s._temperatureUnitOfB = (TemperatureRepresentation)info.GetEnum("ParamBUnit", typeof(TemperatureRepresentation));
        s._temperatureUnitOfT0 = (TemperatureRepresentation)info.GetEnum("ParamT0Unit", typeof(TemperatureRepresentation));

        return s;
      }
    }


    #endregion Serialization

    /// <summary>
    /// Returns a string that represents the current fit function.
    /// </summary>
    /// <returns>Always returns "VogelFulcherLaw".</returns>
    public override string ToString()
    {
      return "VogelFulcherLaw";
    }

    /// <summary>
    /// Factory method used by discovery to create a default instance of this fit function.
    /// </summary>
    /// <returns>A new <see cref="VogelFulcherLawTime"/> instance.</returns>
    [FitFunctionCreator("Vogel-Fulcher law (time)", "Materials", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Materials.VogelFulcherLawTime}")]
    public static IFitFunction CreateDefault()
    {
      return new VogelFulcherLawTime();
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
        return 3;
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
          return "B_" + _temperatureUnitOfB.ToString();
          ;
        case 2:
          return "T0_" + _temperatureUnitOfT0.ToString();

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
          return 1; // y0

        case 1: // B
          var te = new Temperature(1000, TemperatureRepresentation.Kelvin);
          return te.ConvertTo(_temperatureUnitOfB).Value;

        case 2: // T0
          return 0;

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
    /// Evaluates the Vogel-Fulcher law for time-like quantities at the specified temperature.
    /// </summary>
    /// <param name="x">The temperature value in the unit represented by <see cref="IndependentVariableRepresentation"/>.</param>
    /// <param name="P">Parameter array where P[0]=y0, P[1]=B and P[2]=T0.</param>
    /// <returns>The evaluated function value.</returns>
    public virtual double Evaluate(double x, IReadOnlyList<double> P)
    {
      double temperature = Temperature.ToKelvin(x, _temperatureUnitOfX);
      double B = Temperature.ToKelvin(P[1], _temperatureUnitOfB);
      double T0 = Temperature.ToKelvin(P[2], _temperatureUnitOfT0);
      return P[0] * Math.Exp(B / (temperature - T0));
    }

    /// <inheritdoc/>
    public virtual void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = Evaluate(X[0], P);
    }

    /// <inheritdoc/>
    public virtual void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        double temperature = Temperature.ToKelvin(x, _temperatureUnitOfX);
        double B = Temperature.ToKelvin(P[1], _temperatureUnitOfB);
        double T0 = Temperature.ToKelvin(P[2], _temperatureUnitOfT0);
        FV[r] = P[0] * Math.Exp(B / (temperature - T0));
      }
    }

    /// <summary>
    /// Not used (instance is immutable).
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }



    #endregion IFitFunction Members

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
