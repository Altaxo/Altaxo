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
using System.Drawing;

namespace Altaxo.Graph.Plot.Data
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Main;
  using Gdi.Plot.Data;

  #region XYFunctionPlotData

  /// <summary>
  /// Summary description for XYFunctionPlotData.
  /// </summary>
  [Serializable]
  public class XYFunctionPlotData : XYFunctionPlotDataBase

  {
    protected Altaxo.Calc.IScalarFunctionDD _function;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYFunctionPlotData", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYFunctionPlotData), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYFunctionPlotData)obj;

        info.AddValue("Function", s._function);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (XYFunctionPlotData?)o ?? new XYFunctionPlotData();

        s.Function = (Altaxo.Calc.IScalarFunctionDD)info.GetValue("Function", s);
        if (s.Function is Main.IDocumentLeafNode)
          ((Main.IDocumentLeafNode)s.Function).ParentObject = s;

        return s;
      }
    }

    #endregion Serialization

    #region Construction and Copying

    /// <summary>
    /// Only for derived classes and deserialization.
    /// </summary>
    protected XYFunctionPlotData()
    {
      _function = Calc.NullFunction.Instance;
    }

    public XYFunctionPlotData(Altaxo.Calc.IScalarFunctionDD function)
    {
      Function = function;
    }

    public XYFunctionPlotData(XYFunctionPlotData from)
    {
      if (from is null)
        throw new ArgumentNullException(nameof(from));

      CopyFrom(from);
    }

    [MemberNotNull(nameof(_function))]
    public void CopyFrom(XYFunctionPlotData from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      ChildCloneToMemberAlt(ref _function, from._function);
    }

    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (!base.CopyFrom(obj))
        return false;

      if (obj is XYFunctionPlotData from)
      {
        CopyFrom(from);

        return true;
      }
      return false;
    }

    public override object Clone()
    {
      return new XYFunctionPlotData(this);
    }

    #endregion Construction and Copying

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_function is not null && Function is Main.IDocumentLeafNode)
        yield return new Main.DocumentNodeAndName((Main.IDocumentLeafNode)_function, "Function");
    }



    public override string ToString()
    {
      if (_function is not null)
        return "Function: " + _function.ToString();
      else
        return base.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Get/sets the function used for evaluation. Must be serializable in order to store the graph to disk.
    /// </summary>
    /// <value>The function.</value>
    public Altaxo.Calc.IScalarFunctionDD Function
    {
      get
      {
        return _function;
      }
      [MemberNotNull(nameof(_function))]
      set
      {
        if (ChildSetMemberAlt(ref _function, value ?? throw new ArgumentNullException(nameof(Function))))
        {
          EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
        }
      }
    }

    #region IScalarFunctionDD Members

    public override double Evaluate(double x)
    {
      return _function is null ? 0 : _function.Evaluate(x);
    }

    #endregion IScalarFunctionDD Members
  }

  #endregion XYFunctionPlotData

  #region PolynomialFunction

  /// <summary>
  /// <para>Evaluates a polynomial a0 + a1*x + a2*x^2 ...</para>
  /// <para>Special serializable version for plotting purposes.</para>
  /// </summary>
  [Serializable]
  public class PolynomialFunction
      :
      Main.SuspendableDocumentLeafNodeWithSingleAccumulatedData<PlotItemDataChangedEventArgs>,
      Altaxo.Calc.IScalarFunctionDD, ICloneable
  {
    /// <summary>
    /// Coefficient array used to evaluate the polynomial.
    /// </summary>
    private double[] _coefficients;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PolynomialFunction", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PolynomialFunction), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PolynomialFunction)obj;

        info.AddArray("Coefficients", s._coefficients, s._coefficients.Length);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PolynomialFunction?)o ?? new PolynomialFunction();

        info.GetArray("Coefficients", out s._coefficients);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Only for deserialization purposes.
    /// </summary>
    protected PolynomialFunction()
    {
      _coefficients = new double[1];
    }

    /// <summary>
    /// Constructor by providing the array of coefficients (a0 is the first element of the array).
    /// </summary>
    /// <param name="coefficients">The coefficient array, starting with coefficient a0.</param>
    public PolynomialFunction(double[] coefficients)
    {
      if (coefficients is not null)
        _coefficients = (double[])coefficients.Clone();
      else
        _coefficients = new double[1];
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Another polynomial function to clone from.</param>
    public PolynomialFunction(PolynomialFunction from)
    {
      _coefficients = (double[])from._coefficients.Clone();
    }

    /// <summary>
    /// Get / set the coefficients of the polynomial.
    /// </summary>
    /// <value>The coefficient array of the polynomial, starting with a0.</value>
    public double[] Coefficients
    {
      get
      {
        return (double[])_coefficients.Clone();
      }
      set
      {
        if (value is not null)
        {
          _coefficients = (double[])value.Clone();
          EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
        }
      }
    }

    public int Order
    {
      get
      {
        return _coefficients is null ? 0 : _coefficients.Length - 1;
      }
    }

    public override string ToString()
    {
      var stb = new System.Text.StringBuilder();
      stb.AppendFormat("Polynomial (order {0})", Order);

      if (_coefficients is not null && _coefficients.Length > 0)
      {
        stb.Append(" [");

        for (int i = 0; i < _coefficients.Length; ++i)
        {
          stb.Append(_coefficients[i]);
          if ((i + 1) != _coefficients.Length)
            stb.Append(";");
          else
            stb.Append("]");
        }
      }

      return stb.ToString();
    }

    #region IScalarFunctionDD Members

    /// <summary>
    /// Evaluates the polynomial.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The value of the polynomial, a0+a1*x+a2*x^2+...</returns>
    public double Evaluate(double x)
    {
      if (_coefficients is null)
        return 0;

      double result = 0;
      for (int i = _coefficients.Length - 1; i >= 0; i--)
      {
        result *= x;
        result += _coefficients[i];
      }
      return result;
    }

    #endregion IScalarFunctionDD Members

    #region ICloneable Members

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A new, cloned instance of this object.</returns>
    public object Clone()
    {
      return new PolynomialFunction(this);
    }

    #endregion ICloneable Members
  }

  #endregion PolynomialFunction

  #region SquareRootFunction

  /// <summary>
  /// <para>Evaluates the square root of another function</para>
  /// <para>Special serializable version for plotting purposes.</para>
  /// </summary>
  [Serializable]
  public class SquareRootFunction
      :
      Main.SuspendableDocumentLeafNodeWithSingleAccumulatedData<PlotItemDataChangedEventArgs>,
      Altaxo.Calc.IScalarFunctionDD, ICloneable
  {
    /// <summary>
    /// Function from which to evaluate the square root.
    /// </summary>
    private Altaxo.Calc.IScalarFunctionDD _baseFunction;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.SquareRootFunction", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SquareRootFunction), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SquareRootFunction)obj;

        info.AddValue("BaseFunction", s._baseFunction);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (SquareRootFunction?)o ?? new SquareRootFunction(info);

        s._baseFunction = (Altaxo.Calc.IScalarFunctionDD)info.GetValue("BaseFunction", s);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Only for deserialization purposes.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected SquareRootFunction(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    /// <summary>
    /// Constructor by providing the array of coefficients (a0 is the first element of the array).
    /// </summary>
    /// <param name="baseFunction">The function whose square root is evaluated.</param>
    public SquareRootFunction(Altaxo.Calc.IScalarFunctionDD baseFunction)
    {
      if (baseFunction is ICloneable)
        _baseFunction = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)baseFunction).Clone();
      else
        _baseFunction = baseFunction;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Another polynomial function to clone from.</param>
    public SquareRootFunction(SquareRootFunction from)
    {
      if (from._baseFunction is ICloneable)
        _baseFunction = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)from._baseFunction).Clone();
      else
        _baseFunction = from._baseFunction;
    }

    /// <summary>
    /// Get / set the base function.
    /// </summary>
    /// <value>The base function, from which the square root is evaluated.</value>
    public Altaxo.Calc.IScalarFunctionDD BaseFunction
    {
      get
      {
        if (_baseFunction is ICloneable)
          return (Altaxo.Calc.IScalarFunctionDD)((ICloneable)_baseFunction).Clone();
        else
          return _baseFunction;
      }
      set
      {
        if (value is not null)
        {
          if (value is ICloneable)
            _baseFunction = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)value).Clone();
          else
            _baseFunction = value;

          EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
        }
      }
    }

    public override string ToString()
    {
      if (_baseFunction is not null)
        return "Sqrt(" + _baseFunction.ToString() + ")";
      else
        return "Sqrt(InvalidFunction)";
    }

    #region IScalarFunctionDD Members

    /// <summary>
    /// Evaluates the polynomial.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The value of the polynomial, a0+a1*x+a2*x^2+...</returns>
    public double Evaluate(double x)
    {
      return Math.Sqrt(_baseFunction.Evaluate(x));
    }

    #endregion IScalarFunctionDD Members

    #region ICloneable Members

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A new, cloned instance of this object.</returns>
    public object Clone()
    {
      return new SquareRootFunction(this);
    }

    #endregion ICloneable Members
  }

  #endregion SquareRootFunction

  #region ScaledSumFunction

  /// <summary>
  /// <para>Evaluates a scaled sum of other functions f(x) = a1*f1(x)+ a2*f2(x)+...</para>
  /// <para>Special serializable version for plotting purposes.</para>
  /// </summary>
  [Serializable]
  public class ScaledSumFunction
      :
      Main.SuspendableDocumentLeafNodeWithSingleAccumulatedData<PlotItemDataChangedEventArgs>,
      Altaxo.Calc.IScalarFunctionDD, ICloneable
  {
    /// <summary>
    /// Coefficient array used to evaluate the polynomial.
    /// </summary>
    private double[] _coefficients;

    private Altaxo.Calc.IScalarFunctionDD[] _functions;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ScaledSumFunction", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScaledSumFunction), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ScaledSumFunction)obj;

        info.AddArray("Coefficients", s._coefficients, s._coefficients.Length);
        info.CreateArray("Functions", s._functions.Length);
        for (int i = 0; i < s._functions.Length; i++)
          info.AddValue("e", s._functions[i]);
        info.CommitArray();
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ScaledSumFunction?)o ?? new ScaledSumFunction(info);

        info.GetArray("Coefficients", out s._coefficients);

        int cnt = info.OpenArray();
        s._functions = new Altaxo.Calc.IScalarFunctionDD[cnt];
        for (int i = 0; i < cnt; i++)
          s._functions[i] = (Altaxo.Calc.IScalarFunctionDD)info.GetValue("e", s);

        info.CloseArray(cnt);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Only for deserialization purposes.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected ScaledSumFunction(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    /// <summary>
    /// Constructor by providing the array of coefficients (a0 is the first element of the array).
    /// </summary>
    /// <param name="coefficients">The coefficient array, starting with coefficient a0.</param>
    /// <param name="functions">The array of functions to sum up.</param>
    public ScaledSumFunction(double[] coefficients, Altaxo.Calc.IScalarFunctionDD[] functions)
    {
      _coefficients = (double[])coefficients.Clone();


      _functions = new Altaxo.Calc.IScalarFunctionDD[functions.Length];
      for (int i = 0; i < functions.Length; i++)
      {
        if (functions[i] is ICloneable)
          _functions[i] = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)functions[i]).Clone();
        else
          _functions[i] = functions[i];
      }
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Another polynomial function to clone from.</param>
    public ScaledSumFunction(ScaledSumFunction from)
    {
      _coefficients = (double[])from._coefficients.Clone();

      _functions = new Altaxo.Calc.IScalarFunctionDD[from._functions.Length];
      for (int i = 0; i < from._functions.Length; i++)
      {
        if (from._functions[i] is ICloneable)
          _functions[i] = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)from._functions[i]).Clone();
        else
          _functions[i] = from._functions[i];
      }
    }

    /// <summary>
    /// Get / set the coefficients of the polynomial.
    /// </summary>
    /// <value>The coefficient array of the polynomial, starting with a0.</value>
    public double[] Coefficients
    {
      get
      {
        return (double[])_coefficients.Clone();
      }
      set
      {
        if (value is not null)
        {
          _coefficients = (double[])value.Clone();
          EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
        }
      }
    }

    public override string ToString()
    {
      return "SumOfScaledFunctions";
    }

    #region IScalarFunctionDD Members

    /// <summary>
    /// Evaluates the polynomial.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The value of the polynomial, a0+a1*x+a2*x^2+...</returns>
    public double Evaluate(double x)
    {
      if (_coefficients is null)
        return 0;

      double result = 0;
      double end = Math.Min(_coefficients.Length, _functions.Length);
      for (int i = 0; i < end; i++)
      {
        result += _coefficients[i] * _functions[i].Evaluate(x);
      }
      return result;
    }

    #endregion IScalarFunctionDD Members

    #region ICloneable Members

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A new, cloned instance of this object.</returns>
    public object Clone()
    {
      return new ScaledSumFunction(this);
    }

    #endregion ICloneable Members
  }

  #endregion ScaledSumFunction

  #region ProductFunction

  /// <summary>
  /// <para>Evaluates the product of other functions f(x) = f1(x)^a1*f2(x)^a2*...</para>
  /// <para>Special serializable version for plotting purposes.</para>
  /// </summary>
  [Serializable]
  public class ProductFunction
      :
      Main.SuspendableDocumentLeafNodeWithSingleAccumulatedData<PlotItemDataChangedEventArgs>,
      Altaxo.Calc.IScalarFunctionDD, ICloneable
  {
    /// <summary>
    /// Coefficient array (the power).
    /// </summary>
    private double[] _coefficients;

    private Altaxo.Calc.IScalarFunctionDD[] _functions;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ProductFunction", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ProductFunction), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ProductFunction)obj;

        info.AddArray("Coefficients", s._coefficients, s._coefficients.Length);
        info.CreateArray("Functions", s._functions.Length);
        for (int i = 0; i < s._functions.Length; i++)
          info.AddValue("e", s._functions[i]);
        info.CommitArray();
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ProductFunction?)o ?? new ProductFunction(info);

        info.GetArray("Coefficients", out s._coefficients);

        int cnt = info.OpenArray();
        s._functions = new Altaxo.Calc.IScalarFunctionDD[cnt];
        for (int i = 0; i < cnt; i++)
          s._functions[i] = (Altaxo.Calc.IScalarFunctionDD)info.GetValue("e", s);

        info.CloseArray(cnt);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Only for deserialization purposes.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected ProductFunction(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    /// <summary>
    /// Constructor by providing the array of coefficients (a0 is the first element of the array).
    /// </summary>
    /// <param name="coefficients">The coefficient array, starting with coefficient a0.</param>
    /// <param name="functions">The array of functions to sum up.</param>
    public ProductFunction(double[] coefficients, Altaxo.Calc.IScalarFunctionDD[] functions)
    {
      _coefficients = (double[])coefficients.Clone();

      _functions = new Altaxo.Calc.IScalarFunctionDD[functions.Length];
      for (int i = 0; i < functions.Length; i++)
      {
        if (functions[i] is ICloneable)
          _functions[i] = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)functions[i]).Clone();
        else
          _functions[i] = functions[i];
      }
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Another polynomial function to clone from.</param>
    public ProductFunction(ProductFunction from)
    {
      _coefficients = (double[])from._coefficients.Clone();

      _functions = new Altaxo.Calc.IScalarFunctionDD[from._functions.Length];
      for (int i = 0; i < from._functions.Length; i++)
      {
        if (from._functions[i] is ICloneable)
          _functions[i] = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)from._functions[i]).Clone();
        else
          _functions[i] = from._functions[i];
      }
    }

    /// <summary>
    /// Get / set the coefficients of the polynomial.
    /// </summary>
    /// <value>The coefficient array of the polynomial, starting with a0.</value>
    public double[] Coefficients
    {
      get
      {
        return (double[])_coefficients.Clone();
      }
      set
      {
        if (value is not null)
        {
          _coefficients = (double[])value.Clone();
          EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
        }
      }
    }

    public override string ToString()
    {
      return "ProductOfFunctions";
    }

    #region IScalarFunctionDD Members

    /// <summary>
    /// Evaluates the polynomial.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The value of the polynomial, a0+a1*x+a2*x^2+...</returns>
    public double Evaluate(double x)
    {
      if (_coefficients is null)
        return 0;

      double result = 1;
      double term;
      double coeff;
      double end = Math.Min(_coefficients.Length, _functions.Length);
      for (int i = 0; i < end; i++)
      {
        coeff = _coefficients[i];
        if (coeff == 1)
          term = _functions[i].Evaluate(x);
        else if (coeff == 0)
          term = 1;
        else if (coeff == 0.5)
          term = Math.Sqrt(_functions[i].Evaluate(x));
        else
          term = Math.Pow(_functions[i].Evaluate(x), coeff);

        result *= term;
      }
      return result;
    }

    #endregion IScalarFunctionDD Members

    #region ICloneable Members

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A new, cloned instance of this object.</returns>
    public object Clone()
    {
      return new ProductFunction(this);
    }

    #endregion ICloneable Members
  }

  #endregion ProductFunction
}
