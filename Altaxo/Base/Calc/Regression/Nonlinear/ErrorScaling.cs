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

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Interface to how to scale the differences between real quantities (dependent variables) and fitted values.
  /// </summary>
  public interface IVarianceScaling : ICloneable
  {
    /// <summary>
    /// Gets the weight in dependence of the real data (roughly spoken: inverse of variance).
    /// </summary>
    /// <param name="yreal">The real (measured) data.</param>
    /// <param name="i">The index of the measured data point in the table.</param>
    /// <returns>The weight used to scale the fit difference (yreal-yfit). In case a variance is given for the current data,
    /// you should return (1/variance). </returns>
    double GetWeight(double yreal, int i);

    /// <summary>
    /// Returns a short name for the scaling method. Used to display this short name in
    /// the fit function dialog box.
    /// </summary>
    string ShortName { get; }
  }

  /// <summary>
  /// The default scaling variance returns always 1.
  /// </summary>
  public class ConstantVarianceScaling : IVarianceScaling
  {
    double _scaling = 1;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ConstantVarianceScaling), 0)]
      public  class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ConstantVarianceScaling s = (ConstantVarianceScaling)obj;

        info.AddValue("ScalingFactor", s._scaling);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ConstantVarianceScaling s = o != null ? (ConstantVarianceScaling)o : new ConstantVarianceScaling();
        
        s._scaling = info.GetDouble("ScalingFactor");

        return s;
      }
    }

    #endregion

    /// <summary>
    /// Returns true when the scaling factor is 1 (one).
    /// </summary>
    public bool IsDefault
    {
      get
      {
        return _scaling == 1;
      }
    }

    public double GetWeight(double yr, int i)
    {
      return _scaling;
    }

    public string ShortName
    {
      get { return "N2"; }
    }

    public object Clone()
    {
      ConstantVarianceScaling result = new ConstantVarianceScaling();
      result._scaling = this._scaling;
      return result;
    }
  }

  /// <summary>
  /// This is a variance which scales linearly with the measured value. Useful for
  /// functions with a broad range of y-values. Make sure that no y-value is zero.
  /// </summary>
  public class RelativeVarianceScaling : IVarianceScaling
  {
    double _scaling = 1;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RelativeVarianceScaling), 0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        RelativeVarianceScaling s = (RelativeVarianceScaling)obj;

        info.AddValue("ScalingFactor", s._scaling);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        RelativeVarianceScaling s = o != null ? (RelativeVarianceScaling)o : new RelativeVarianceScaling();

        s._scaling = info.GetDouble("ScalingFactor");

        return s;
      }
    }

    #endregion

    public double GetWeight(double yr, int i)
    {
      if (yr == 0)
        return _scaling;
      else
        return _scaling/Math.Abs(yr);
    }

    public string ShortName
    {
      get { return "N1"; }
    }

    public object Clone()
    {
      RelativeVarianceScaling result = new RelativeVarianceScaling();
      result._scaling = this._scaling;
      return result;
    }

  }


}
