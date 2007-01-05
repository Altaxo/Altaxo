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

namespace Altaxo.Data
{
  /// <summary>
  /// The EquallySpacedColumn is a simple readable numeric column. The value of an element is 
  /// calculated from y = a+b*i. This means the value of the first element is a, the values are equally spaced by b.
  /// </summary>
  [Serializable]
  public class EquallySpacedColumn : INumericColumn, IReadableColumn, ICloneable
  {
    /// <summary>The start value, i.e. the value at index 0.</summary>
    protected double m_Start=0;
    /// <summary>The spacing value between consecutive elements.</summary>
    protected double m_Increment=1;


    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EquallySpacedColumn),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        EquallySpacedColumn s = (EquallySpacedColumn)obj;
        info.AddValue("StartValue",s.m_Start);
        info.AddValue("Increment",s.m_Increment);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        EquallySpacedColumn s = null!=o ? (EquallySpacedColumn)o : new EquallySpacedColumn(0,1);
        
        s.m_Start = info.GetDouble("StartValue");
        s.m_Increment = info.GetDouble("Increment");
        return s;
      }
    }
    #endregion

    /// <summary>
    /// Creates a EquallySpacedColumn with starting value start and spacing increment.
    /// </summary>
    /// <param name="start">The starting value.</param>
    /// <param name="increment">The increment value (spacing value between consecutive elements).</param>
    public EquallySpacedColumn(double start, double increment)
    {
      m_Start = start;
      m_Increment = increment;
    }

    /// <summary>
    /// Creates a cloned instance of this object.
    /// </summary>
    /// <returns>The cloned instance of this object.</returns>
    public object Clone()
    {
      return new EquallySpacedColumn(m_Start, m_Increment);
    }


    /// <summary>
    /// Simply returns the value i.
    /// </summary>
    /// <param name="i">The index i.</param>
    /// <returns>The index i.</returns>
    public double this[int i]
    {
      get
      {
        return m_Start + i * m_Increment;
      }
    }
    
    /// <summary>
    /// This returns always true.
    /// </summary>
    /// <param name="i">The index i.</param>
    /// <returns>Always true.</returns>
    public bool IsElementEmpty(int i)
    {
      return false;
    }

    /// <summary>
    /// Returns the index i as AltaxoVariant.
    /// </summary>
    AltaxoVariant Altaxo.Data.IReadableColumn.this[int i] 
    {
      get 
      {
        return new AltaxoVariant((double)(m_Start+i*m_Increment));
      }
    } 

    /// <summary>
    /// The full name of a indexer column is "EquallySpacedColumn(start,increment)".
    /// </summary>
    public string FullName
    {
      get { return "EquallySpacedColumn("+m_Start.ToString()+","+m_Increment.ToString()+")"; }
    }

  }

}
