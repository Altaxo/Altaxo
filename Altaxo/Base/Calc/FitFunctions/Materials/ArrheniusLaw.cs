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

using System;
using System.ComponentModel;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Science;

namespace Altaxo.Calc.FitFunctions.Materials
{
  /// <summary>
  /// Represents the Arrhenius law (formula for the temperature dependence of the rate).
  /// </summary>
  [FitFunctionClass]
  public class ArrheniusLaw : IFitFunction
  {
    private TransformedValueRepresentation _dependentVariableTransform;
    private TemperatureRepresentation _temperatureUnitOfX;
    private EnergyRepresentation _paramEnergyUnit;

    [Category("OptionsForParameters")]
    public EnergyRepresentation ParameterEnergyRepresentation
    {
      get { return _paramEnergyUnit; }
      set
      {
        var oldValue = _paramEnergyUnit;
        _paramEnergyUnit = value;
        if (oldValue != value)
          OnChanged();
      }
    }

    [Category("OptionsForDependentVariables")]
    public TransformedValueRepresentation DependentVariableRepresentation
    {
      get { return _dependentVariableTransform; }
      set
      {
        var oldValue = _dependentVariableTransform;
        _dependentVariableTransform = value;
        if (oldValue != value)
          OnChanged();
      }
    }

    [Category("OptionsForIndependentVariables")]
    public TemperatureRepresentation IndependentVariableRepresentation
    {
      get { return _temperatureUnitOfX; }
      set
      {
        var oldValue = _temperatureUnitOfX;
        _temperatureUnitOfX = value;
        if (oldValue != value)
          OnChanged();
      }
    }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ArrheniusLaw), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ArrheniusLaw)obj;
        info.AddEnum("IndependentVariableUnit", s._temperatureUnitOfX);
        info.AddEnum("DependentVariableTransform", s._dependentVariableTransform);
        info.AddEnum("ParamEnergyUnit", s._paramEnergyUnit);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ArrheniusLaw s = o != null ? (ArrheniusLaw)o : new ArrheniusLaw();

        s._temperatureUnitOfX = (TemperatureRepresentation)info.GetEnum("IndependentVariableUnit", typeof(TemperatureRepresentation));
        s._dependentVariableTransform = (TransformedValueRepresentation)info.GetEnum("DependentVariableTransform", typeof(TransformedValueRepresentation));
        s._paramEnergyUnit = (EnergyRepresentation)info.GetEnum("ParamEnergyUnit", typeof(EnergyRepresentation));

        return s;
      }
    }

    #endregion Serialization

    public ArrheniusLaw()
    {
    }

    public override string ToString()
    {
      return "ArrheniusLaw";
    }

    [FitFunctionCreator("ArrheniusLaw", "Materials", 1, 1, 2)]
    [System.ComponentModel.Description("Altaxo.Calc.FitFunctions.Materials.ArrheniusLaw.Description")]
    public static IFitFunction CreateDefault()
    {
      return new ArrheniusLaw();
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
        return 2;
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
          return "E_" + _paramEnergyUnit.ToString();

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
      }

      return 0;
    }

    public IVarianceScaling DefaultVarianceScaling(int i)
    {
      return null;
    }

    #region Change event

    /// <summary>
    /// Called when anything in this fit function has changed.
    /// </summary>
    protected virtual void OnChanged()
    {
      if (null != Changed)
        Changed(this, EventArgs.Empty);
    }

    /// <summary>
    /// Fired when the fit function changed.
    /// </summary>
    public event EventHandler Changed;

    #endregion Change event

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double temperature = Temperature.ToKelvin(X[0], _temperatureUnitOfX);
      double energyAsTemperature = Energy.ToTemperatureSI(P[1], _paramEnergyUnit);
      double ybase = P[0] * Math.Exp(energyAsTemperature / temperature);
      Y[0] = TransformedValue.BaseValueToTransformedValue(ybase, _dependentVariableTransform);
    }

    #endregion IFitFunction Members
  }
}
