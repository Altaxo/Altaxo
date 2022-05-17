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
using System.ComponentModel;
using Altaxo.Science;

namespace Altaxo.Calc.FitFunctions.Materials
{
  [Obsolete("Please use ArrheniusLawTime or ArrheniusLawRate")]
  public class ArrheniusLaw : ArrheniusLawTime
  {
    private TransformedValueRepresentation _dependentVariableTransform;

    private ArrheniusLaw(TemperatureRepresentation temperatureUnitOfX, TransformedValueRepresentation dependentVariableTransform, EnergyRepresentation paramEnergyUnit)
      : base(temperatureUnitOfX, paramEnergyUnit)
    {
      _dependentVariableTransform = dependentVariableTransform;
    }

    [Category("OptionsForDependentVariables")]
    public TransformedValueRepresentation DependentVariableRepresentation => _dependentVariableTransform;
    [Category("OptionsForDependentVariables")]
    public ArrheniusLawTime WithDependentVariableRepresentation(TransformedValueRepresentation value)
    {
      throw new NotImplementedException("Setting the dependent variable transformation is obsolete. Instead, please set the transformation in the fit function view.");
    }

    public override string DependentVariableName(int i)
    {
      return TransformedValue.GetFormula("y", _dependentVariableTransform);
    }

    public override void Evaluate(double[] X, double[] P, double[] Y)
    {
      base.Evaluate(X, P, Y);
      Y[0] = TransformedValue.BaseValueToTransformedValue(Y[0], _dependentVariableTransform);
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ArrheniusLaw), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ArrheniusLaw)obj;
        info.AddEnum("IndependentVariableUnit", s.IndependentVariableRepresentation);
        info.AddEnum("DependentVariableTransform", s.DependentVariableRepresentation);
        info.AddEnum("ParamEnergyUnit", s.ParameterEnergyRepresentation);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var temperatureUnitOfX = (TemperatureRepresentation)info.GetEnum("IndependentVariableUnit", typeof(TemperatureRepresentation));
        var dependentVariableTransform = (TransformedValueRepresentation)info.GetEnum("DependentVariableTransform", typeof(TransformedValueRepresentation));
        var paramEnergyUnit = (EnergyRepresentation)info.GetEnum("ParamEnergyUnit", typeof(EnergyRepresentation));

        return new ArrheniusLaw(temperatureUnitOfX, dependentVariableTransform, paramEnergyUnit);
      }
    }
  }
}
