#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

namespace Altaxo.Graph
{
  #region XYFunctionPlotData
  /// <summary>
  /// Summary description for XYFunctionPlotData.
  /// </summary>
  public class XYFunctionPlotData : ICloneable, Calc.IScalarFunctionDD, Main.IChangedEventSource
  {
    Altaxo.Calc.IScalarFunctionDD _function;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYFunctionPlotData),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYFunctionPlotData s = (XYFunctionPlotData)obj;
        
        info.AddValue("Function",s._function);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYFunctionPlotData s = null!=o ? (XYFunctionPlotData)o : new XYFunctionPlotData();

        s.Function = (Altaxo.Calc.IScalarFunctionDD)info.GetValue("Function",parent);
        
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
      this.Function = (Altaxo.Calc.IScalarFunctionDD)((ICloneable)_function).Clone();
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

        if(oldValue is Main.IChangedEventSource)
          ((Main.IChangedEventSource)oldValue).Changed -= new EventHandler(EhFunctionChanged);

        if(_function!=null && _function is Main.IChangedEventSource)
          ((Main.IChangedEventSource)_function).Changed += new EventHandler(EhFunctionChanged);

        if(!object.ReferenceEquals(oldValue,value))
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
      return _function==null ? 0 : _function.Evaluate(x);
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
      if(Changed!=null)
      {
        Changed(this,EventArgs.Empty);
      }
    }

    public event System.EventHandler Changed;

    #endregion
  }

  #endregion

  /// <summary>
  /// <para>Evaluates a polynomial a0 + a1*x + a2*x^2 ...</para>
  /// <para>Special serializable version for plotting purposes.</para>
  /// </summary>
  public class PolynomialFunction : Altaxo.Calc.IScalarFunctionDD, ICloneable, Main.IChangedEventSource
  {
    /// <summary>
    /// Coefficient array used to evaluate the polynomial.
    /// </summary>
    double[] _coefficients;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PolynomialFunction),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PolynomialFunction s = (PolynomialFunction)obj;
        
        info.AddArray("Coefficients",s._coefficients,s._coefficients.Length);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        PolynomialFunction s = null!=o ? (PolynomialFunction)o : new PolynomialFunction();
       
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
      if(coefficients!=null)
        _coefficients = (double[])coefficients.Clone();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Another polynomial function to clone from.</param>
    public PolynomialFunction(PolynomialFunction from)
    {
      if(from._coefficients!=null)  
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
        if(value!=null)
        {
          _coefficients = (double[])value.Clone();
          OnChanged();
        }
      }
    }

    #region IScalarFunctionDD Members

    /// <summary>
    /// Evaluates the polynomial.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The value of the polynomial, a0+a1*x+a2*x^2+...</returns>
    public double Evaluate(double x)
    {
      if(null==_coefficients)
        return 0;
    
      double result=0;
      for(int i=_coefficients.Length-1;i>=0;i--)
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
      if(Changed!=null)
      {
        Changed(this,EventArgs.Empty);
      }
    }

    /// <summary>
    /// Event fired when the coefficients of the polynomial changed.
    /// </summary>
    public event System.EventHandler Changed;

    #endregion
  }

}
