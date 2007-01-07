#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Drawing;


namespace Altaxo.Graph.Plot.Data
{
  using Scales;
  using Gdi.Plot.Data;

  #region XYFunctionPlotData
  /// <summary>
  /// Summary description for XYFunctionPlotData.
  /// </summary>
  [Serializable]
  public class XYFunctionPlotData : ICloneable, Calc.IScalarFunctionDD, Main.IChangedEventSource, Main.IDocumentNode
  {
    Altaxo.Calc.IScalarFunctionDD _function;

    [field:NonSerialized]
    public event System.EventHandler Changed;

    [NonSerialized]
    object _parent;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYFunctionPlotData", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYFunctionPlotData), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYFunctionPlotData s = (XYFunctionPlotData)obj;

        info.AddValue("Function", s._function);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYFunctionPlotData s = null != o ? (XYFunctionPlotData)o : new XYFunctionPlotData();

        s.Function = (Altaxo.Calc.IScalarFunctionDD)info.GetValue("Function", parent);

        return s;
      }
    }

    #endregion

    /// <summary>
    /// Only for deserialization purposes.
    /// </summary>
    protected XYFunctionPlotData()
    {
    }

    public XYFunctionPlotData(Altaxo.Calc.IScalarFunctionDD function)
    {
      this.Function = function;


    }

    public XYFunctionPlotData(XYFunctionPlotData from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(XYFunctionPlotData from)
    {
      if (from._function is ICloneable)
        this.Function = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)from._function).Clone();
      else
        this.Function = from._function;
    }

    public override string ToString()
    {
      if (_function != null)
        return "Function: " + _function.ToString();
      else
        return base.ToString();
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
      set
      {
        Altaxo.Calc.IScalarFunctionDD oldValue = _function;
        _function = value;

        if (oldValue is Main.IChangedEventSource)
          ((Main.IChangedEventSource)oldValue).Changed -= new EventHandler(EhFunctionChanged);

        if (_function != null && _function is Main.IChangedEventSource)
          ((Main.IChangedEventSource)_function).Changed += new EventHandler(EhFunctionChanged);

        if (!object.ReferenceEquals(oldValue, value))
          OnChanged();
      }
    }

    #region ICloneable Members

    public object Clone()
    {
      return new XYFunctionPlotData(this);
    }

    #endregion

    #region IScalarFunctionDD Members

    public double Evaluate(double x)
    {
      return _function == null ? 0 : _function.Evaluate(x);
    }

    #endregion

    #region IChangedEventSource Members

    /// <summary>
    /// EventHandler called if the underlying function has changed.
    /// </summary>
    /// <param name="sender">Sender of this event.</param>
    /// <param name="e">EventArgs (not used here).</param>
    void EhFunctionChanged(object sender, EventArgs e)
    {
      OnChanged();
    }

    /// <summary>
    /// Fires the Changed event.
    /// </summary>
    protected virtual void OnChanged()
    {
      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);
      
      if (Changed != null)
        Changed(this, EventArgs.Empty);
    }


    #endregion


    class MyPlotData
    {
      public double[] _xPhysical;
      public double[] _yPhysical;

      public Altaxo.Data.AltaxoVariant GetXPhysical(int originalRowIndex)
      {
        return _xPhysical[originalRowIndex];
      }
      public Altaxo.Data.AltaxoVariant GetYPhysical(int originalRowIndex)
      {
        return _yPhysical[originalRowIndex];
      }
    }

    /// <summary>
    /// This will create a point list out of the data, which can be used to plot the data. In order to create this list,
    /// the function must have knowledge how to calculate the points out of the data. This will be done
    /// by a function provided by the calling function.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    /// <returns>An array of plot points in layer coordinates.</returns>
    public Processed2DPlotData GetRangesAndPoints(
      Gdi.IPlotArea layer)
    {
      const int functionPoints = 1000;
      const double MaxRelativeValue = 1E6;


      // allocate an array PointF to hold the line points
      PointF[] ptArray = new PointF[functionPoints];
      Processed2DPlotData result = new Processed2DPlotData();
      MyPlotData pdata = new MyPlotData();
      result.PlotPointsInAbsoluteLayerCoordinates = ptArray;
      double[] xPhysArray = new double[functionPoints];
      double[] yPhysArray = new double[functionPoints];
      pdata._xPhysical = xPhysArray;
      pdata._yPhysical = yPhysArray;
      result.XPhysicalAccessor = new IndexedPhysicalValueAccessor(pdata.GetXPhysical);
      result.YPhysicalAccessor = new IndexedPhysicalValueAccessor(pdata.GetYPhysical);

      // double xorg = layer.XAxis.Org;
      // double xend = layer.XAxis.End;
      // Fill the array with values
      // only the points where x and y are not NaNs are plotted!

      int i, j;

      bool bInPlotSpace = true;
      int rangeStart = 0;
      PlotRangeList rangeList = new PlotRangeList();
      result.RangeList = rangeList;
      Gdi.G2DCoordinateSystem coordsys = layer.CoordinateSystem;

      NumericalScale xaxis = layer.XAxis as NumericalScale;
      NumericalScale yaxis = layer.YAxis as NumericalScale;
      if (xaxis == null || yaxis == null)
        return null;

      for (i = 0, j = 0; i < functionPoints; i++)
      {
        double x_rel = ((double)i) / (functionPoints - 1);
        double x = xaxis.NormalToPhysical(x_rel);
        double y = Evaluate(x);

        if (Double.IsNaN(x) || Double.IsNaN(y))
        {
          if (!bInPlotSpace)
          {
            bInPlotSpace = true;
            rangeList.Add(new PlotRange(rangeStart, j));
          }
          continue;
        }


        // double x_rel = layer.XAxis.PhysicalToNormal(x);
        double y_rel = yaxis.PhysicalToNormal(y);

        // chop relative values to an range of about -+ 10^6
        if (y_rel > MaxRelativeValue)
          y_rel = MaxRelativeValue;
        if (y_rel < -MaxRelativeValue)
          y_rel = -MaxRelativeValue;

        // after the conversion to relative coordinates it is possible
        // that with the choosen axis the point is undefined 
        // (for instance negative values on a logarithmic axis)
        // in this case the returned value is NaN
        double xcoord, ycoord;
        if (coordsys.LogicalToLayerCoordinates(new Logical3D(x_rel, y_rel), out xcoord, out ycoord))
        {
          if (bInPlotSpace)
          {
            bInPlotSpace = false;
            rangeStart = j;
          }
          xPhysArray[j] = x;
          yPhysArray[j] = y;
          ptArray[j].X = (float)xcoord;
          ptArray[j].Y = (float)ycoord;
          j++;
        }
        else
        {
          if (!bInPlotSpace)
          {
            bInPlotSpace = true;
            rangeList.Add(new PlotRange(rangeStart, j));
          }
        }
      } // end for
      if (!bInPlotSpace)
      {
        bInPlotSpace = true;
        rangeList.Add(new PlotRange(rangeStart, j)); // add the last range
      }
      return result;
    }



    #region IDocumentNode Members

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public string Name
    {
      get { return "FunctionPlotData"; }
    }

    #endregion
  }
  #endregion

  #region PolynomialFunction

  /// <summary>
  /// <para>Evaluates a polynomial a0 + a1*x + a2*x^2 ...</para>
  /// <para>Special serializable version for plotting purposes.</para>
  /// </summary>
  [Serializable]
  public class PolynomialFunction : Altaxo.Calc.IScalarFunctionDD, ICloneable, Main.IChangedEventSource
  {
    /// <summary>
    /// Coefficient array used to evaluate the polynomial.
    /// </summary>
    double[] _coefficients;

    /// <summary>
    /// Event fired when the coefficients of the polynomial changed.
    /// </summary>
    [field:NonSerialized]
    public event System.EventHandler Changed;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PolynomialFunction", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PolynomialFunction), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PolynomialFunction s = (PolynomialFunction)obj;

        info.AddArray("Coefficients", s._coefficients, s._coefficients.Length);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        PolynomialFunction s = null != o ? (PolynomialFunction)o : new PolynomialFunction();

        info.GetArray("Coefficients", out s._coefficients);


        return s;
      }
    }

    #endregion

    /// <summary>
    /// Only for deserialization purposes.
    /// </summary>
    protected PolynomialFunction()
    {
    }



    /// <summary>
    /// Constructor by providing the array of coefficients (a0 is the first element of the array).
    /// </summary>
    /// <param name="coefficients">The coefficient array, starting with coefficient a0.</param>
    public PolynomialFunction(double[] coefficients)
    {
      if (coefficients != null)
        _coefficients = (double[])coefficients.Clone();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Another polynomial function to clone from.</param>
    public PolynomialFunction(PolynomialFunction from)
    {
      if (from._coefficients != null)
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
        if (value != null)
        {
          _coefficients = (double[])value.Clone();
          OnChanged();
        }
      }
    }

    public int Order
    {
      get
      {
        return _coefficients == null ? 0 : _coefficients.Length - 1;
      }
    }

    public override string ToString()
    {
      return "Polynomial (order " + Order.ToString() + ")";
    }


    #region IScalarFunctionDD Members

    /// <summary>
    /// Evaluates the polynomial.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The value of the polynomial, a0+a1*x+a2*x^2+...</returns>
    public double Evaluate(double x)
    {
      if (null == _coefficients)
        return 0;

      double result = 0;
      for (int i = _coefficients.Length - 1; i >= 0; i--)
      {
        result *= x;
        result += _coefficients[i];
      }
      return result;
    }

    #endregion

    #region ICloneable Members

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A new, cloned instance of this object.</returns>
    public object Clone()
    {
      return new PolynomialFunction(this);
    }

    #endregion

    #region IChangedEventSource Members

    /// <summary>
    /// Fires the Changed event.
    /// </summary>
    protected virtual void OnChanged()
    {
      if (Changed != null)
      {
        Changed(this, EventArgs.Empty);
      }
    }

    

    #endregion
  }


  #endregion

  #region SquareRootFunction

  /// <summary>
  /// <para>Evaluates the square root of another function</para>
  /// <para>Special serializable version for plotting purposes.</para>
  /// </summary>
  [Serializable]
  public class SquareRootFunction : Altaxo.Calc.IScalarFunctionDD, ICloneable, Main.IChangedEventSource
  {
    /// <summary>
    /// Function from which to evaluate the square root.
    /// </summary>
    Altaxo.Calc.IScalarFunctionDD _baseFunction;


    /// <summary>
    /// Event fired when the coefficients of the polynomial changed.
    /// </summary>
    [field:NonSerialized]
    public event System.EventHandler Changed;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.SquareRootFunction", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SquareRootFunction), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SquareRootFunction s = (SquareRootFunction)obj;

        info.AddValue("BaseFunction", s._baseFunction);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        SquareRootFunction s = null != o ? (SquareRootFunction)o : new SquareRootFunction();

        s._baseFunction = (Altaxo.Calc.IScalarFunctionDD)info.GetValue("BaseFunction");


        return s;
      }
    }

    #endregion

    /// <summary>
    /// Only for deserialization purposes.
    /// </summary>
    protected SquareRootFunction()
    {
    }



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
        this._baseFunction = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)from._baseFunction).Clone();
      else
        this._baseFunction = from._baseFunction;
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
        if (value != null)
        {
          if (value is ICloneable)
            _baseFunction = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)value).Clone();
          else
            _baseFunction = value;

          OnChanged();
        }
      }
    }



    public override string ToString()
    {
      if (_baseFunction != null)
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

    #endregion

    #region ICloneable Members

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A new, cloned instance of this object.</returns>
    public object Clone()
    {
      return new SquareRootFunction(this);
    }

    #endregion

    #region IChangedEventSource Members

    /// <summary>
    /// Fires the Changed event.
    /// </summary>
    protected virtual void OnChanged()
    {
      if (Changed != null)
      {
        Changed(this, EventArgs.Empty);
      }
    }

   

    #endregion
  }


  #endregion

  #region ScaledSumFunction

  /// <summary>
  /// <para>Evaluates a scaled sum of other functions f(x) = a1*f1(x)+ a2*f2(x)+...</para>
  /// <para>Special serializable version for plotting purposes.</para>
  /// </summary>
  [Serializable]
  public class ScaledSumFunction : Altaxo.Calc.IScalarFunctionDD, ICloneable, Main.IChangedEventSource
  {
    /// <summary>
    /// Coefficient array used to evaluate the polynomial.
    /// </summary>
    double[] _coefficients;
    Altaxo.Calc.IScalarFunctionDD[] _functions;

    /// <summary>
    /// Event fired when the coefficients of the polynomial changed.
    /// </summary>
    [field:NonSerialized]
    public event System.EventHandler Changed;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ScaledSumFunction", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScaledSumFunction), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ScaledSumFunction s = (ScaledSumFunction)obj;

        info.AddArray("Coefficients", s._coefficients, s._coefficients.Length);
        info.CreateArray("Functions", s._functions.Length);
        for (int i = 0; i < s._functions.Length; i++)
          info.AddValue("e", s._functions[i]);
        info.CommitArray();
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ScaledSumFunction s = null != o ? (ScaledSumFunction)o : new ScaledSumFunction();

        info.GetArray("Coefficients", out s._coefficients);

        int cnt = info.OpenArray();
        s._functions = new Altaxo.Calc.IScalarFunctionDD[cnt];
        for (int i = 0; i < cnt; i++)
          s._functions[i] = (Altaxo.Calc.IScalarFunctionDD)info.GetValue("e", parent);

        info.CloseArray(cnt);

        return s;
      }
    }

    #endregion

    /// <summary>
    /// Only for deserialization purposes.
    /// </summary>
    protected ScaledSumFunction()
    {
    }



    /// <summary>
    /// Constructor by providing the array of coefficients (a0 is the first element of the array).
    /// </summary>
    /// <param name="coefficients">The coefficient array, starting with coefficient a0.</param>
    /// <param name="functions">The array of functions to sum up.</param>
    public ScaledSumFunction(double[] coefficients, Altaxo.Calc.IScalarFunctionDD[] functions)
    {
      if (coefficients != null)
        _coefficients = (double[])coefficients.Clone();

      if (functions != null)
      {
        _functions = new Altaxo.Calc.IScalarFunctionDD[functions.Length];
        for (int i = 0; i < functions.Length; i++)
        {
          if (functions[i] is ICloneable)
            _functions[i] = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)functions[i]).Clone();
          else
            _functions[i] = functions[i];
        }
      }


    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Another polynomial function to clone from.</param>
    public ScaledSumFunction(ScaledSumFunction from)
    {
      if (from._coefficients != null)
        _coefficients = (double[])from._coefficients.Clone();
      else
        _coefficients = null;

      if (from._functions != null)
      {
        _functions = new Altaxo.Calc.IScalarFunctionDD[from._functions.Length];
        for (int i = 0; i < from._functions.Length; i++)
        {
          if (from._functions[i] is ICloneable)
            _functions[i] = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)from._functions[i]).Clone();
          else
            _functions[i] = from._functions[i];
        }
      }
      else
      {
        _functions = null;
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
        if (value != null)
        {
          _coefficients = (double[])value.Clone();
          OnChanged();
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
      if (null == _coefficients)
        return 0;

      double result = 0;
      double end = Math.Min(_coefficients.Length, _functions.Length);
      for (int i = 0; i < end; i++)
      {
        result += _coefficients[i] * _functions[i].Evaluate(x);
      }
      return result;
    }

    #endregion

    #region ICloneable Members

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A new, cloned instance of this object.</returns>
    public object Clone()
    {
      return new ScaledSumFunction(this);
    }

    #endregion

    #region IChangedEventSource Members

    /// <summary>
    /// Fires the Changed event.
    /// </summary>
    protected virtual void OnChanged()
    {
      if (Changed != null)
      {
        Changed(this, EventArgs.Empty);
      }
    }

   

    #endregion
  }


  #endregion

  #region ProductFunction

  /// <summary>
  /// <para>Evaluates the product of other functions f(x) = f1(x)^a1*f2(x)^a2*...</para>
  /// <para>Special serializable version for plotting purposes.</para>
  /// </summary>
  [Serializable]
  public class ProductFunction : Altaxo.Calc.IScalarFunctionDD, ICloneable, Main.IChangedEventSource
  {
    /// <summary>
    /// Coefficient array (the power).
    /// </summary>
    double[] _coefficients;
    Altaxo.Calc.IScalarFunctionDD[] _functions;

    /// <summary>
    /// Event fired when the coefficients of the polynomial changed.
    /// </summary>
    [field:NonSerialized]
    public event System.EventHandler Changed;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ProductFunction", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ProductFunction), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ProductFunction s = (ProductFunction)obj;

        info.AddArray("Coefficients", s._coefficients, s._coefficients.Length);
        info.CreateArray("Functions", s._functions.Length);
        for (int i = 0; i < s._functions.Length; i++)
          info.AddValue("e", s._functions[i]);
        info.CommitArray();
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ProductFunction s = null != o ? (ProductFunction)o : new ProductFunction();

        info.GetArray("Coefficients", out s._coefficients);

        int cnt = info.OpenArray();
        s._functions = new Altaxo.Calc.IScalarFunctionDD[cnt];
        for (int i = 0; i < cnt; i++)
          s._functions[i] = (Altaxo.Calc.IScalarFunctionDD)info.GetValue("e", parent);

        info.CloseArray(cnt);

        return s;
      }
    }

    #endregion

    /// <summary>
    /// Only for deserialization purposes.
    /// </summary>
    protected ProductFunction()
    {
    }



    /// <summary>
    /// Constructor by providing the array of coefficients (a0 is the first element of the array).
    /// </summary>
    /// <param name="coefficients">The coefficient array, starting with coefficient a0.</param>
    /// <param name="functions">The array of functions to sum up.</param>
    public ProductFunction(double[] coefficients, Altaxo.Calc.IScalarFunctionDD[] functions)
    {
      if (coefficients != null)
        _coefficients = (double[])coefficients.Clone();

      if (functions != null)
      {
        _functions = new Altaxo.Calc.IScalarFunctionDD[functions.Length];
        for (int i = 0; i < functions.Length; i++)
        {
          if (functions[i] is ICloneable)
            _functions[i] = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)functions[i]).Clone();
          else
            _functions[i] = functions[i];
        }
      }


    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Another polynomial function to clone from.</param>
    public ProductFunction(ProductFunction from)
    {
      if (from._coefficients != null)
        _coefficients = (double[])from._coefficients.Clone();
      else
        _coefficients = null;

      if (from._functions != null)
      {
        _functions = new Altaxo.Calc.IScalarFunctionDD[from._functions.Length];
        for (int i = 0; i < from._functions.Length; i++)
        {
          if (from._functions[i] is ICloneable)
            _functions[i] = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)from._functions[i]).Clone();
          else
            _functions[i] = from._functions[i];
        }
      }
      else
      {
        _functions = null;
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
        if (value != null)
        {
          _coefficients = (double[])value.Clone();
          OnChanged();
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
      if (null == _coefficients)
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

    #endregion

    #region ICloneable Members

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A new, cloned instance of this object.</returns>
    public object Clone()
    {
      return new ProductFunction(this);
    }

    #endregion

    #region IChangedEventSource Members

    /// <summary>
    /// Fires the Changed event.
    /// </summary>
    protected virtual void OnChanged()
    {
      if (Changed != null)
      {
        Changed(this, EventArgs.Empty);
      }
    }

   

    #endregion
  }


  #endregion

}
