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

namespace Altaxo.Calc
{
  /// <summary>
  /// This structure holds a value, which is either absolute or relative to another value.
  /// </summary>
  [Serializable]
  public struct RelativeOrAbsoluteValue
  {
    /// <summary>True if the value m_Value is relative, false if m_Value is absolute.</summary>
    private bool m_bIsRelative; // per default, m_bRelative is false, so the value is interpreted as absolute
    /// <summary>
    /// The value to hold, either absolute, or relative. If relative, the value 1 means the same value
    /// as the value it is relative to.
    /// </summary>
    private double m_Value;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RelativeOrAbsoluteValue),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        RelativeOrAbsoluteValue s = (RelativeOrAbsoluteValue)obj;
          
        info.AddValue("IsRelative",s.m_bIsRelative);
        info.AddValue("Value",s.m_Value);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        bool rel = info.GetBoolean("IsRelative");
        double val = info.GetDouble("Value");
        return new RelativeOrAbsoluteValue(val,rel);
      }
    }
    #endregion


    /// <summary>
    /// This creates the structure with the absolute value absval.
    /// </summary>
    /// <param name="absval"></param>
    public RelativeOrAbsoluteValue(double absval)
    {
      m_bIsRelative=false;
      m_Value = absval;
    }

    /// <summary>
    /// Constructs the structure with provided value and the information,
    /// if it is a absolute or a relative value.
    /// </summary>
    /// <param name="val">The value, either absolute or relative.</param>
    /// <param name="isRelative">True if the value is relative, else false.</param>
    public RelativeOrAbsoluteValue(double val, bool isRelative)
    {
      m_bIsRelative= isRelative;
      m_Value = val;
    }


    /// <summary>
    /// Get / set the information, if the value is relative or absolute.
    /// </summary>
    public bool IsRelative
    {
      get { return m_bIsRelative; }
      set { m_bIsRelative = value; }
    }

    /// <summary>
    ///  Get / set the raw value. Careful! the value you get is not relative to another, even
    ///  in the case that the sructure holds a relative value, it is the raw value in m_Value instead. 
    /// </summary>
    public double Value
    {
      get { return m_Value; }
      set { Value = value; }
    }

    /// <summary>
    /// This is the function to get out the value. In case it is a absolute value, simply
    /// Value is returned, regardless of the argument <paramref name="r"/>. In case it
    /// is a absolute value, the product of r with the value is returned.
    /// </summary>
    /// <param name="r">The value to which this value is relative to.</param>
    /// <returns>If absolute, the stored value; if relative, the product of the stored value with <paramref name="r"/></returns>
    public double GetValueRelativeTo(double r)
    {
      return m_bIsRelative ? r*m_Value : m_Value;
    }

    public static bool operator==(RelativeOrAbsoluteValue a, RelativeOrAbsoluteValue b)
    {
      return a.m_bIsRelative==b.m_bIsRelative && a.m_Value==b.m_Value;
    }

    public static bool operator!=(RelativeOrAbsoluteValue a, RelativeOrAbsoluteValue b)
    {
      return !(a==b);
    }

    public override bool Equals(object o)
    {
      if(!(o is RelativeOrAbsoluteValue))
        return false;
      else
        return ((RelativeOrAbsoluteValue)o)==this;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode ();
    }


  }
}
