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

    public VogelFulcherLawTime()
    {
      _temperatureUnitOfX = TemperatureRepresentation.Kelvin;
      _temperatureUnitOfT0 = TemperatureRepresentation.Kelvin;
      _temperatureUnitOfB = TemperatureRepresentation.Kelvin;
    }

    public VogelFulcherLawTime(
    TemperatureRepresentation temperatureUnitOfX,
    TemperatureRepresentation temperatureUnitOfT0,
    TemperatureRepresentation temperatureUnitOfB)
    {
      _temperatureUnitOfX = temperatureUnitOfX;
      _temperatureUnitOfT0 = temperatureUnitOfT0;
      _temperatureUnitOfB = temperatureUnitOfB;
    }



    [Category("OptionsForIndependentVariables")]
    public TemperatureRepresentation IndependentVariableRepresentation
    {
      get { return _temperatureUnitOfX; }
      set { _temperatureUnitOfX = value; }
    }

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

    [Category("OptionsForParameters")]
    public TemperatureRepresentation ParameterT0Representation
    {
      get { return _temperatureUnitOfT0; }
    }

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

    [Category("OptionsForParameters")]
    public TemperatureRepresentation ParameterBRepresentation
    {
      get { return _temperatureUnitOfB; }
    }

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
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VogelFulcherLawTime)obj;
        info.AddEnum("IndependentVariableUnit", s._temperatureUnitOfX);
        info.AddEnum("ParamBUnit", s._temperatureUnitOfB);
        info.AddEnum("ParamT0Unit", s._temperatureUnitOfT0);
      }

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

    public override string ToString()
    {
      return "VogelFulcherLaw";
    }

    [FitFunctionCreator("Vogel-Fulcher law (time)", "Materials", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Materials.VogelFulcherLawTime}")]
    public static IFitFunction CreateDefault()
    {
      return new VogelFulcherLawTime();
    }

    #region IFitFunction Members

    public int NumberOfIndependentVariables
    {
      get
      {
        return 1;
      }
    }

    public int NumberOfDependentVariables
    {
      get
      {
        return 1;
      }
    }

    public int NumberOfParameters
    {
      get
      {
        return 3;
      }
    }

    public string IndependentVariableName(int i)
    {
      return "T_" + _temperatureUnitOfX.ToString();
    }

    public virtual string DependentVariableName(int i)
    {
      return "y";
    }

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
          throw new ArgumentOutOfRangeException("i");
      }
    }

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
      }

      return 0;
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    public virtual double Evaluate(double x, IReadOnlyList<double> P)
    {
      double temperature = Temperature.ToKelvin(x, _temperatureUnitOfX);
      double B = Temperature.ToKelvin(P[1], _temperatureUnitOfB);
      double T0 = Temperature.ToKelvin(P[2], _temperatureUnitOfT0);
      return P[0] * Math.Exp(B / (temperature - T0));
    }

    public virtual void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = Evaluate(X[0], P);
    }

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
