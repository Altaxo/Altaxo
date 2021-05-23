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
  /// <summary>
  /// Obsolete VogelFulcherLaw class.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.FitFunctions.Materials.VogelFulcherLawTime" />
  [Obsolete("Please use VogelFulcherLawTime or VogelFulcherLawRate")]
  public class VogelFulcherLaw : VogelFulcherLawTime
  {
    private TransformedValueRepresentation _dependentVariableTransform;

    private VogelFulcherLaw(
      TransformedValueRepresentation dependentVariableTransform,
    TemperatureRepresentation temperatureUnitOfX,
    TemperatureRepresentation temperatureUnitOfT0,
    TemperatureRepresentation temperatureUnitOfB)
      : base(temperatureUnitOfX, temperatureUnitOfT0, temperatureUnitOfB)
    {
      _dependentVariableTransform = dependentVariableTransform;
    }

    [Category("OptionsForDependentVariables")]
    public TransformedValueRepresentation DependentVariableRepresentation
    {
      get { return _dependentVariableTransform; }
      set { throw new NotImplementedException("Obsolete. Instead, please use the output transformation in the fit function manager"); }
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

    /// <summary>
    /// Initial version.
    /// 2021-05-23 renamed to VogelFulcherLawTimeA
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VogelFulcherLaw), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VogelFulcherLaw)obj;
        info.AddEnum("IndependentVariableUnit", s.IndependentVariableRepresentation);
        info.AddEnum("DependentVariableTransform", s.DependentVariableRepresentation);
        info.AddEnum("ParamBUnit", s.ParameterBRepresentation);
        info.AddEnum("ParamT0Unit", s.ParameterT0Representation);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {

        var unitOfX = (TemperatureRepresentation)info.GetEnum("IndependentVariableUnit", typeof(TemperatureRepresentation));
         var dependentVariableTransform = (TransformedValueRepresentation)info.GetEnum("DependentVariableTransform", typeof(TransformedValueRepresentation));
        var temperatureUnitOfB = (TemperatureRepresentation)info.GetEnum("ParamBUnit", typeof(TemperatureRepresentation));
        var temperatureUnitOfT0 = (TemperatureRepresentation)info.GetEnum("ParamT0Unit", typeof(TemperatureRepresentation));

        return new VogelFulcherLaw(dependentVariableTransform, unitOfX, temperatureUnitOfT0, temperatureUnitOfB);
      }
    }
  }
}
