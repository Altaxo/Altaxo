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
using System.Diagnostics.CodeAnalysis;
using Altaxo.Data;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Wraps the fit function, storing its parameters, so that it can be used as a plot function.
  /// </summary>
  public class FitFunctionToScalarFunctionDDWrapper
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IScalarFunctionDD
  {
    private IFitFunction? _fitFunction;

    private double[] _y;
    private double[] _x;
    private double[] _parameter;
    private int _independentVariable;
    private IVariantToVariantTransformation? _independentVariableTransformation;
    private int _dependentVariable;
    private IVariantToVariantTransformation? _dependentVariableTransformation;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Nonlinear.FitFunctionToScalarFunctionDDWrapper", 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old versions not allowed.");
        /*
                FitFunctionToScalarFunctionDDWrapper s = (FitFunctionToScalarFunctionDDWrapper)obj;

                info.AddValue("IndependentVariable", s._independentVariable);
                info.AddValue("DependentVariable", s._dependentVariable);
                info.AddArray("ParameterValues", s._parameter, s._parameter.Length);

                if (s._fitFunction == null || info.IsSerializable(s._fitFunction))
                    info.AddValue("FitFunction", s._fitFunction);
                else
                    info.AddValue("FitFunction", new Altaxo.Serialization.Xml.AssemblyAndTypeSurrogate(s._fitFunction));
                    */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        int independentVariable = info.GetInt32("IndependentVariable");
        int dependentVariable = info.GetInt32("DependentVariable");
        info.GetArray("ParameterValues", out double[] parameter);

        object? fo = info.GetValueOrNull("FitFunction", null);

        if (fo is Altaxo.Serialization.Xml.AssemblyAndTypeSurrogate ats)
          fo = ats.CreateInstance();

        FitFunctionToScalarFunctionDDWrapper s;
        if (o is null)
        {
          s = new FitFunctionToScalarFunctionDDWrapper(fo as IFitFunction, dependentVariable, independentVariable, parameter);
        }
        else
        {
          s = (FitFunctionToScalarFunctionDDWrapper)o;
          s._independentVariable = independentVariable;
          s._dependentVariable = dependentVariable;
          s._parameter = parameter;
          s._fitFunction = fo as IFitFunction;

          if (s._fitFunction is Main.IDocumentLeafNode && !(s._fitFunction is Altaxo.Scripting.FitFunctionScript))
            ((Main.IDocumentLeafNode)s._fitFunction).ParentObject = s;
        }

        return s;
      }
    }

    /// <summary>
    /// 2017-01-05 Added: _independentVariableTransformation and _dependentVariableTransformation
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitFunctionToScalarFunctionDDWrapper), 1)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FitFunctionToScalarFunctionDDWrapper)obj;

        info.AddValue("IndependentVariable", s._independentVariable);
        info.AddValueOrNull("IndependentVariableTransformation", s._independentVariableTransformation);
        info.AddValue("DependentVariable", s._dependentVariable);
        info.AddValueOrNull("DependentVariableTransformation", s._dependentVariableTransformation);
        info.AddArray("ParameterValues", s._parameter, s._parameter.Length);

        if (s._fitFunction is null || info.IsSerializable(s._fitFunction))
          info.AddValueOrNull("FitFunction", s._fitFunction);
        else
          info.AddValue("FitFunction", new Altaxo.Serialization.Xml.AssemblyAndTypeSurrogate(s._fitFunction));
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        int independentVariable = info.GetInt32("IndependentVariable");
        var independentVariableTransformation = (IVariantToVariantTransformation?)info.GetValueOrNull("IndependentVariableTransformation", null);
        int dependentVariable = info.GetInt32("DependentVariable");
        var dependentVariableTransformation = (IVariantToVariantTransformation?)info.GetValueOrNull("DependentVariableTransformation", null);

        info.GetArray("ParameterValues", out double[] parameter);

        object? fo = info.GetValueOrNull("FitFunction", null);

        if (fo is Altaxo.Serialization.Xml.AssemblyAndTypeSurrogate ats)
          fo = ats.CreateInstance();

        FitFunctionToScalarFunctionDDWrapper s;
        if (o is null)
        {
          s = new FitFunctionToScalarFunctionDDWrapper(fo as IFitFunction, dependentVariable, dependentVariableTransformation, independentVariable, independentVariableTransformation, parameter);
        }
        else
        {
          s = (FitFunctionToScalarFunctionDDWrapper)o;
          s._independentVariable = independentVariable;
          s._independentVariableTransformation = independentVariableTransformation;
          s._dependentVariable = dependentVariable;
          s._dependentVariableTransformation = dependentVariableTransformation;
          s._parameter = parameter;
          s._fitFunction = fo as IFitFunction;

          if (s._fitFunction is Main.IDocumentLeafNode && !(s._fitFunction is Altaxo.Scripting.FitFunctionScript))
            ((Main.IDocumentLeafNode)s._fitFunction).ParentObject = s;
        }

        return s;
      }
    }

    #endregion Serialization

    public FitFunctionToScalarFunctionDDWrapper(IFitFunction? fitFunction, int dependentVariable, IVariantToVariantTransformation? dependentVariableTransformation, int independentVariable, IVariantToVariantTransformation? independentVariableTransformation, double[] parameter)
    {
      Initialize(fitFunction, dependentVariable, dependentVariableTransformation, independentVariable, independentVariableTransformation, parameter);
    }

    public FitFunctionToScalarFunctionDDWrapper(IFitFunction? fitFunction, int dependentVariable, int independentVariable, double[] parameter)
    {
      Initialize(fitFunction, dependentVariable, null, independentVariable, null, parameter);
    }

    public FitFunctionToScalarFunctionDDWrapper(IFitFunction? fitFunction, int dependentVariable, IVariantToVariantTransformation? dependentVariableTransformation, double[] parameter)
    {
      Initialize(fitFunction, dependentVariable, dependentVariableTransformation, 0, null, parameter);
    }

    public FitFunctionToScalarFunctionDDWrapper(IFitFunction? fitFunction, int dependentVariable, double[] parameter)
    {
      Initialize(fitFunction, dependentVariable, null, 0, null, parameter);
    }

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_fitFunction is Main.IDocumentLeafNode && object.ReferenceEquals(((Main.IDocumentLeafNode)_fitFunction).ParentObject, this))
        yield return new Main.DocumentNodeAndName((Main.IDocumentLeafNode)_fitFunction, () => _fitFunction = null, "WrappedFitFunction");
    }

    [MemberNotNull(nameof(_x), nameof(_y), nameof(_parameter))]
    public void Initialize(IFitFunction? fitFunction, int dependentVariable, IVariantToVariantTransformation? dependentVariableTransformation, int independentVariable, IVariantToVariantTransformation? independentVariableTransformation, double[] parameter)
    {
      _fitFunction = fitFunction;

      if (_fitFunction is not null)
      {
        _x = new double[_fitFunction.NumberOfIndependentVariables];
        _y = new double[_fitFunction.NumberOfDependentVariables];
        _parameter = new double[Math.Max(_fitFunction.NumberOfParameters, parameter.Length)];

        if (_fitFunction is Main.IDocumentLeafNode && !(_fitFunction is Altaxo.Scripting.FitFunctionScript))
          ((Main.IDocumentLeafNode)_fitFunction).ParentObject = this;
      }
      else
      {
        _x = new double[1];
        _y = new double[1];
        _parameter = new double[parameter.Length];
      }

      _dependentVariable = dependentVariable;
      _dependentVariableTransformation = dependentVariableTransformation;

      _independentVariable = independentVariable;
      _independentVariableTransformation = independentVariableTransformation;

      int len = Math.Min(_parameter.Length, parameter.Length);
      for (int i = 0; i < len; i++)
        _parameter[i] = parameter[i];
    }

    public IFitFunction? FitFunction => _fitFunction;

    public double[] Parameter
    {
      get
      {
        return _parameter;
      }
    }

    public double[] X
    {
      get
      {
        return _x;
      }
    }

    public int DependentVariable
    {
      get
      {
        return _dependentVariable;
      }
      set
      {
        _dependentVariable = value;
      }
    }

    public IVariantToVariantTransformation? DependentVariableTransformation
    {
      get
      {
        return _dependentVariableTransformation;
      }
    }

    public int IndependentVariable
    {
      get
      {
        return _independentVariable;
      }
      set
      {
        _independentVariable = value;
      }
    }

    public IVariantToVariantTransformation? IndependentVariableTransformation
    {
      get
      {
        return _independentVariableTransformation;
      }
    }

    #region IScalarFunctionDD Members

    public double Evaluate(double x)
    {
      if (_fitFunction is not null)
      {
        if (_independentVariableTransformation is not null)
          _x[_independentVariable] = _independentVariableTransformation.Transform(x);
        else
          _x[_independentVariable] = x;

        _fitFunction.Evaluate(_x, _parameter, _y);

        if (_dependentVariableTransformation is not null)
          return _dependentVariableTransformation.Transform(_y[_dependentVariable]);
        else
          return _y[_dependentVariable];
      }
      else
      {
        return double.NaN;
      }
    }

    #endregion IScalarFunctionDD Members
  }
}
