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

    /// <summary>
    /// Initializes a new instance of the <see cref="VogelFulcherLaw"/> class with the specified
    /// dependent variable transformation and temperature unit representations.
    /// </summary>
    /// <param name="dependentVariableTransform">Representation of the dependent variable transformation.</param>
    /// <param name="temperatureUnitOfX">Temperature unit representation for the independent variable.</param>
    /// <param name="temperatureUnitOfT0">Temperature unit representation for parameter T0.</param>
    /// <param name="temperatureUnitOfB">Temperature unit representation for parameter B.</param>
    private VogelFulcherLaw(
      TransformedValueRepresentation dependentVariableTransform,
    TemperatureRepresentation temperatureUnitOfX,
    TemperatureRepresentation temperatureUnitOfT0,
    TemperatureRepresentation temperatureUnitOfB)
      : base(temperatureUnitOfX, temperatureUnitOfT0, temperatureUnitOfB)
    {
      _dependentVariableTransform = dependentVariableTransform;
    }

    /// <summary>
    /// Gets the representation used to transform the dependent variable for output.
    /// Setting this property is obsolete and will always throw a <see cref="NotImplementedException"/>.
    /// Use the output transformation in the fit function manager instead.
    /// </summary>
    [Category("OptionsForDependentVariables")]
    public TransformedValueRepresentation DependentVariableRepresentation
    {
      get { return _dependentVariableTransform; }
      set { throw new NotImplementedException("Obsolete. Instead, please use the output transformation in the fit function manager"); }
    }

    /// <inheritdoc/>
    public override string DependentVariableName(int i)
    {
      return TransformedValue.GetFormula("y", _dependentVariableTransform);
    }

    /// <inheritdoc/>
    public override void Evaluate(double[] X, double[] P, double[] Y)
    {
      var y = base.Evaluate(X[0], P);
      Y[0] = TransformedValue.BaseValueToTransformedValue(y, _dependentVariableTransform);
    }

    /// <summary>
    /// Evaluates the fit function for multiple rows of independent variables and stores the transformed results in <paramref name="FV"/>.
    /// </summary>
    /// <param name="independent">Matrix of independent variable values (rows x columns), expected single-column.</param>
    /// <param name="P">Parameter list where P[0]=y0, P[1]=B and P[2]=T0.</param>
    /// <param name="independentVariableChoice">Not used; retained for compatibility.</param>
    /// <param name="FV">Vector that receives the transformed function values for each row.</param>
    public void EvaluateMultiple(IROMatrix<double> independent, IReadOnlyList<double> P, IReadOnlyList<bool>? independentVariableChoice, IVector<double> FV)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        var y = base.Evaluate(x, P);
        FV[r] = TransformedValue.BaseValueToTransformedValue(y, _dependentVariableTransform);
      }
    }

    /// <summary>
    /// Initial version.
    /// 2021-05-23 renamed to VogelFulcherLawTimeA
    /// V1: 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Materials.VogelFulcherLaw", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VogelFulcherLaw), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VogelFulcherLaw)obj;
        info.AddEnum("IndependentVariableUnit", s.IndependentVariableRepresentation);
        info.AddEnum("DependentVariableTransform", s.DependentVariableRepresentation);
        info.AddEnum("ParamBUnit", s.ParameterBRepresentation);
        info.AddEnum("ParamT0Unit", s.ParameterT0Representation);
      }

      /// <inheritdoc/>
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
