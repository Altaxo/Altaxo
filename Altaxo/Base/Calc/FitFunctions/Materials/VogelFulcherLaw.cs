#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.ComponentModel;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Science;

namespace Altaxo.Calc.FitFunctions.Materials
{
  /// <summary>
  /// Represents the Vogel-Fulcher law to describe the temperature dependence of rates in glass forming substances.
  /// </summary>
  [FitFunctionClass]
  public class VogelFulcherLaw : IFitFunction
  {
    public enum OutputType { Direct = 0, NaturalLogarithm = 1, DecadicLogarithm = 2 };

    private TransformedValueRepresentation _dependentVariableTransform;
    private TemperatureRepresentation _temperatureUnitOfX;
    private TemperatureRepresentation _temperatureUnitOfT0;
    private TemperatureRepresentation _temperatureUnitOfB;

    [Category("OptionsForDependentVariables")]
    public TransformedValueRepresentation DependentVariableRepresentation
    {
      get { return _dependentVariableTransform; }
      set { _dependentVariableTransform = value; }
    }

    [Category("OptionsForIndependentVariables")]
    public TemperatureRepresentation IndependentVariableRepresentation
    {
      get { return _temperatureUnitOfX; }
      set { _temperatureUnitOfX = value; }
    }

    [Category("OptionsForParameters")]
    public TemperatureRepresentation ParameterT0Representation
    {
      get { return _temperatureUnitOfT0; }
      set { _temperatureUnitOfT0 = value; }
    }

    [Category("OptionsForParameters")]
    public TemperatureRepresentation ParameterBRepresentation
    {
      get { return _temperatureUnitOfB; }
      set { _temperatureUnitOfB = value; }
    }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VogelFulcherLaw), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VogelFulcherLaw)obj;
        info.AddEnum("IndependentVariableUnit", s._temperatureUnitOfX);
        info.AddEnum("DependentVariableTransform", s._dependentVariableTransform);
        info.AddEnum("ParamBUnit", s._temperatureUnitOfB);
        info.AddEnum("ParamT0Unit", s._temperatureUnitOfT0);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        VogelFulcherLaw s = (VogelFulcherLaw?)o ?? new VogelFulcherLaw();

        s._temperatureUnitOfX = (TemperatureRepresentation)info.GetEnum("IndependentVariableUnit", typeof(TemperatureRepresentation));
        s._dependentVariableTransform = (TransformedValueRepresentation)info.GetEnum("DependentVariableTransform", typeof(TransformedValueRepresentation));
        s._temperatureUnitOfB = (TemperatureRepresentation)info.GetEnum("ParamBUnit", typeof(TemperatureRepresentation));
        s._temperatureUnitOfT0 = (TemperatureRepresentation)info.GetEnum("ParamT0Unit", typeof(TemperatureRepresentation));

        return s;
      }
    }

    #endregion Serialization

    public VogelFulcherLaw()
    {
    }

    public override string ToString()
    {
      return "VogelFulcherLaw";
    }

    [FitFunctionCreator("VogelFulcherLaw", "Materials", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Materials.VogelFulcherLaw}")]
    public static IFitFunction CreateDefault()
    {
      return new VogelFulcherLaw();
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

    public string DependentVariableName(int i)
    {
      return TransformedValue.GetFormula("y", _dependentVariableTransform);
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
          return 1;

        case 1:
          return 1000;

        case 2:
          return 0;
      }

      return 0;
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double temperature = Temperature.ToKelvin(X[0], _temperatureUnitOfX);
      double B = Temperature.ToKelvin(P[1], _temperatureUnitOfB);
      double T0 = Temperature.ToKelvin(P[2], _temperatureUnitOfT0);
      double yraw = P[0] * Math.Exp(B / (temperature - T0));
      Y[0] = TransformedValue.BaseValueToTransformedValue(yraw, _dependentVariableTransform);
    }

    /// <summary>
    /// Not used here since this fit function never changed.
    /// </summary>
    public event EventHandler? Changed;

    protected virtual void OnChanged()
    {
      Changed?.Invoke(this, EventArgs.Empty);
    }

    #endregion IFitFunction Members
  }
}
