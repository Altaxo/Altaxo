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
  public interface IOperatable
  {
    // Note: unfortunately (and maybe also undocumented) we can not use
    // the names op_Addition, op_Subtraction and so one, because these
    // names seems to be used by the compiler for the operators itself
    // so we use here vopAddition and so on (the v from virtual)


    bool vop_Addition(object a, out object b);
    bool vop_Addition_Rev(object a, out object b);

    bool vop_Subtraction(object a, out object b);
    bool vop_Subtraction_Rev(object a, out object b);

    bool vop_Multiplication(object a, out object b);
    bool vop_Multiplication_Rev(object a, out object b);

    bool vop_Division(object a, out object b);
    bool vop_Division_Rev(object a, out object b);

    bool vop_Modulo(object a, out object b);
    bool vop_Modulo_Rev(object a, out object b);

    bool vop_And(object a, out object b);
    bool vop_And_Rev(object a, out object b);

    bool vop_Or(object a, out object b);
    bool vop_Or_Rev(object a, out object b);

    bool vop_Xor(object a, out object b);
    bool vop_Xor_Rev(object a, out object b);

    bool vop_ShiftLeft(object a, out object b);
    bool vop_ShiftLeft_Rev(object a, out object b);

    bool vop_ShiftRight(object a, out object b);
    bool vop_ShiftRight_Rev(object a, out object b);

    bool vop_Equal(object a, out bool b);
    bool vop_Equal_Rev(object a, out bool b);

    bool vop_NotEqual(object a, out bool b);
    bool vop_NotEqual_Rev(object a, out bool b);

    bool vop_Lesser(object a, out bool b);
    bool vop_Lesser_Rev(object a, out bool b);

    bool vop_Greater(object a, out bool b);
    bool vop_Greater_Rev(object a, out bool b);

    bool vop_LesserOrEqual(object a, out bool b);
    bool vop_LesserOrEqual_Rev(object a, out bool b);

    bool vop_GreaterOrEqual(object a, out bool b);
    bool vop_GreaterOrEqual_Rev(object a, out bool b);

    // Unary operators

    bool vop_Plus(out object a);
    bool vop_Minus(out object a);
    bool vop_Not(out object a);
    bool vop_Complement(out object a);
    bool vop_Increment(out object a);
    bool vop_Decrement(out object a);
    bool        vop_True(out bool a);
    bool        vop_False(out bool a);


  }

  /// <summary>
  /// AltaxoVariant is the universal datatype used to return the value of a data column,
  /// it is necessary because the type of the column can be text, date, or double
  /// I decided to use struct which only holds the object because of its efficiency
  /// AltaxoVariant is never used to store the data in the array, for this purpose
  /// the native data types are used
  /// </summary>
  public struct AltaxoVariant : IComparable
  {
    public enum Content { VNull, VDouble, VDateTime, VString, VOperatable, VObject }
 
    public Content m_Content;
    public double m_Double;
    public object m_Object;


    #region Serialization
    #region Version 0
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AltaxoVariant), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        AltaxoVariant s = (AltaxoVariant)obj;
        info.AddEnum("Content", s.m_Content);
        switch (s.m_Content)
        {
          case Content.VNull:
            break;
          case Content.VDouble:
            info.AddValue("Value", s.m_Double);
            break;
          case Content.VDateTime:
            info.AddValue("Value", (DateTime)s.m_Object);
            break;
          case Content.VString:
            info.AddValue("Value", (string)s.m_Object);
            break;
          default:
            info.AddValue("Value", s.m_Object);
            break;
        }
      }

      protected virtual AltaxoVariant SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AltaxoVariant s = (o == null ? new AltaxoVariant() : (AltaxoVariant)o);
        Content c = (Content)info.GetEnum("Content", typeof(Content));
        s.m_Content = c;

        switch(c)
        {
          case Content.VNull:
            break;
          case Content.VDouble:
            s.m_Double = info.GetDouble("Value");
            break;
          case Content.VDateTime:
            s.m_Object = info.GetDateTime("Value");
            break;
          case Content.VString:
            s.m_Object = info.GetString("Value");
            break;
          default:
            s.m_Object = info.GetValue("Value",s);
            break;
        }

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        AltaxoVariant s = SDeserialize(o, info, parent);
        return s;
      }
    }
    #endregion
    #endregion

    

    public AltaxoVariant(AltaxoVariant a)
    {
      m_Content = a.m_Content;
      m_Double = a.m_Double;
      m_Object = a.m_Object;
    }
    

    public AltaxoVariant(double d)
    {
      m_Content = Content.VDouble;
      m_Double = d;
      m_Object = null;
    }
    public AltaxoVariant(DateTime f)
    {
      m_Content = Content.VDateTime;
      m_Object = f;
      m_Double = 0;
    } 
    public AltaxoVariant(string s)
    {
      m_Content = Content.VString;
      m_Object=s;
      m_Double = 0;
    }
    
    public AltaxoVariant(object k)
    {
      if(k == null)
      {
        m_Content = Content.VNull;
        m_Double = 0;
        m_Object = null;
      }
      else if(k is Double)
      {
        m_Content = Content.VDouble;
        m_Double = (double)k;
        m_Object = null;
      }
      else if( k is DateTime)
      {
        m_Content = Content.VDateTime;
        m_Double = 0;
        m_Object = k;
      }
      else if (k is string)
      {
        m_Content = Content.VString;
        m_Double = 0;
        m_Object = k;
      }
      else if (k is IOperatable)
      {
        m_Content = Content.VOperatable;
        m_Double = 0;
        m_Object = k;
      }
      else if (k is AltaxoVariant)
      {
        m_Content = ((AltaxoVariant)k).m_Content;
        m_Double  = ((AltaxoVariant)k).m_Double;
        m_Object  = ((AltaxoVariant)k).m_Object; // is critical, because the object is not cloned, so be warned for the first time
      }
      else 
      {
        m_Content = Content.VObject;
        m_Double = 0;
        m_Object = k;
      }
    }

    public bool IsType(Content c)
    {
      return this.m_Content==c;
    }


    public bool IsTypeOrNull(Content c)
    {
      return this.m_Content==c || this.m_Content==Content.VNull;
    }

    public bool CanConvertedToDouble
    {
      get 
      {
        if(m_Content==Content.VDouble || m_Content==Content.VDateTime)
          return true; // we can create a double from a double (trivial) and from DateTime
        if(m_Content==Content.VString) // if the content is a string, we have to look if it is possible to convert
          return Altaxo.Serialization.NumberConversion.IsNumeric((string)m_Object);
        else if(m_Object!=null)
          return Altaxo.Serialization.NumberConversion.IsNumeric(m_Object.ToString());
        else
          return false; // it is not possible to convert the contents to a double
      }
    }

    /// <summary>
    /// Converts the content to a double if possible. The structure remains unchanged.
    /// </summary>
    /// <returns>The contents converted to a double.</returns>
    /// <remarks>An exception is thrown if the conversion fails. You have to use <see cref="CanConvertedToDouble"/> for testing if the contents can be converted to a double.</remarks>
    public double ToDouble()
    {
      if(m_Content==Content.VDouble)
        return m_Double;
      else if(m_Content==Content.VDateTime)
        return ((DateTime)m_Object).Ticks/10000000.0;
      else if(m_Content==Content.VString)
        return System.Convert.ToDouble((string)m_Object);
      else if(m_Object!=null)
        return System.Convert.ToDouble(m_Object.ToString());
      else
        throw new ApplicationException("Unable to convert the contents of this variant to a number, the contents is: " + this.ToString());
    }

    public override string ToString()
    {
      if(this.m_Content == Content.VNull)
        return "(null)";
      else if(this.m_Content == Content.VDouble)
        return this.m_Double.ToString();
      else if(null!=m_Object)
        return this.m_Object.ToString();
      else // everything is null
        return "";
    }

    public override bool Equals(object k)
    {
      if(k is AltaxoVariant)
        return this == ((AltaxoVariant)k);
      else
        return this == new AltaxoVariant(k);
    }

    public override int GetHashCode()
    {
      if(this.m_Content == Content.VNull)
        return this.m_Content.GetHashCode();
      else if(this.m_Content == Content.VDouble)
        return m_Double.GetHashCode();
      else
        return m_Object.GetHashCode();
    }

    /*
    public static explicit operator double(AltaxoVariant f) 
    {
      if(f.m_Content==Content.VDouble)
        return f.m_Double;
      else 
        return f.ToDouble();
    }
    */

    public static implicit operator double(AltaxoVariant f) 
    {
      if(f.m_Content==Content.VDouble)
        return f.m_Double;
      throw new ApplicationException("Variant contains " + f.m_Content.ToString() + ", but expecting type Double");
    }
  

  
    public static implicit operator AltaxoVariant(double f) 
    {
      return new AltaxoVariant(f);
    }
      
    public static implicit operator DateTime(AltaxoVariant f) 
    {
      if(f.m_Content==Content.VDateTime)
        return (DateTime)f.m_Object;
      throw new ApplicationException("Variant contains " + f.m_Content.ToString() + ", but expecting type DateTime");
    }
  
    public static implicit operator AltaxoVariant(DateTime f) 
    {
      return new AltaxoVariant(f);
    }
    
    public static implicit operator string(AltaxoVariant f) 
    {
      if(f.m_Content==Content.VString)
        return (string)f.m_Object;
      throw new ApplicationException("Variant contains " + f.m_Content.ToString() + ", but expecting type string");
    }
  
    public static implicit operator AltaxoVariant(string f) 
    {
      return new AltaxoVariant(f);
    }

    public static AltaxoVariant operator + (AltaxoVariant a, AltaxoVariant b)
    {
      object result;

      if(a.m_Content==Content.VDouble && b.m_Content==Content.VDouble)
        return new AltaxoVariant(a.m_Double + b.m_Double);
      else if(a.m_Content==Content.VString && b.m_Content==Content.VString)
        return new AltaxoVariant(((string)a.m_Object)+((string)b.m_Object));
      else if(a.m_Content==Content.VDateTime && b.m_Content==Content.VDouble)
        return new AltaxoVariant(((DateTime)a.m_Object).AddSeconds(b.m_Double));
      else if (a.m_Content == Content.VDouble && b.m_Content == Content.VDateTime)
        return new AltaxoVariant(((DateTime)b.m_Object).AddSeconds(a.m_Double));
      else if(a.m_Content==Content.VString && b.m_Content==Content.VDouble)
        return new AltaxoVariant(((string)a.m_Object)+((double)b.m_Double).ToString());
      else if(a.m_Content==Content.VString && b.m_Content==Content.VDateTime)
        return new AltaxoVariant(((string)a.m_Object)+((System.DateTime)b.m_Object).ToString());
      else if(a.m_Content==Content.VNull && b.m_Content==Content.VNull)
        return new AltaxoVariant();
      else if(a.m_Content==Content.VOperatable && ((IOperatable)a.m_Object).vop_Addition(b.m_Content==Content.VDouble ? b.m_Double : b.m_Object, out result))
        return new AltaxoVariant(result);
      else if(b.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_Addition_Rev(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to add types " + a.m_Content.ToString() + " and " + b.m_Content.GetType().ToString());  
    }

    public static AltaxoVariant operator - (AltaxoVariant a, AltaxoVariant b)
    {
      object result;

      if(a.m_Content==Content.VDouble && b.m_Content==Content.VDouble)
        return new AltaxoVariant(a.m_Double - b.m_Double);
      else if(a.m_Content==Content.VDateTime && a.m_Content==Content.VDouble)
        return new AltaxoVariant(((DateTime)a.m_Object).AddSeconds(-b.m_Double));
      else if(a.m_Content==Content.VDateTime && b.m_Content==Content.VDateTime)
        return new AltaxoVariant((((DateTime)a.m_Object)-((DateTime)b.m_Object)).TotalSeconds);
      else if(a.m_Content==Content.VNull && b.m_Content==Content.VNull)
        return new AltaxoVariant();
      else if(a.m_Content==Content.VOperatable && ((IOperatable)a.m_Object).vop_Subtraction(b.m_Content==Content.VDouble ? b.m_Double : b.m_Object, out result))
        return new AltaxoVariant(result);
      else if(b.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_Subtraction_Rev(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to subtract types " + a.m_Content.ToString() + " and " + b.m_Content.ToString()); 
    }
    
    public static AltaxoVariant operator * (AltaxoVariant a, AltaxoVariant b)
    {
      object result;

      if (a.m_Content == Content.VDouble && b.m_Content == Content.VDouble)
        return new AltaxoVariant(a.m_Double * b.m_Double);
      else if (a.m_Content == Content.VDouble && b.m_Content == Content.VDateTime)
        return new AltaxoVariant(DateTime.FromBinary((long)(a.m_Double * ((DateTime)b.m_Object).Ticks)));
      else if (a.m_Content == Content.VDateTime && b.m_Content == Content.VDouble)
        return new AltaxoVariant(DateTime.FromBinary((long)(b.m_Double * ((DateTime)a.m_Object).Ticks)));
      else if (a.m_Content == Content.VNull && b.m_Content == Content.VNull)
        return new AltaxoVariant();
      else if (a.m_Content == Content.VOperatable && ((IOperatable)a.m_Object).vop_Multiplication(b.m_Content == Content.VDouble ? b.m_Double : b.m_Object, out result))
        return new AltaxoVariant(result);
      else if (b.m_Content == Content.VOperatable && ((IOperatable)b.m_Object).vop_Multiplication_Rev(a.m_Content == Content.VDouble ? a.m_Double : a.m_Object, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to multiply types " + a.m_Content.ToString() + " and " + b.m_Content.ToString()); 
    }

    public static AltaxoVariant operator / (AltaxoVariant a, AltaxoVariant b)
    {
      object result;
      if(a.m_Content==Content.VDouble && b.m_Content==Content.VDouble)
        return new AltaxoVariant(a.m_Double / b.m_Double);
      else if(a.m_Content==Content.VNull && b.m_Content==Content.VNull)
        return new AltaxoVariant();
      else if(a.m_Content==Content.VOperatable && ((IOperatable)a.m_Object).vop_Division(b.m_Content==Content.VDouble ? b.m_Double : b.m_Object, out result))
        return new AltaxoVariant(result);
      else if(b.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_Division_Rev(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to divide types " + a.m_Content.ToString() + " and " + b.m_Content.ToString()); 
    }

    public static AltaxoVariant operator % (AltaxoVariant a, AltaxoVariant b)
    {
      object result;
      if(a.m_Content==Content.VDouble && b.m_Content==Content.VDouble)
        return new AltaxoVariant(a.m_Double % b.m_Double);
      else if(a.m_Content==Content.VNull && b.m_Content==Content.VNull)
        return new AltaxoVariant();
      else if(a.m_Content==Content.VOperatable && ((IOperatable)a.m_Object).vop_Modulo(b.m_Content==Content.VDouble ? b.m_Double : b.m_Object, out result))
        return new AltaxoVariant(result);
      else if(b.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_Modulo_Rev(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to get remainder of types " + a.m_Content.ToString() + " and " + b.m_Content.ToString()); 
    }

    public static AltaxoVariant operator & (AltaxoVariant a, AltaxoVariant b)
    {
      object result;
      if(a.m_Content==Content.VDouble && b.m_Content==Content.VDouble)
        return new AltaxoVariant((double)((long)a.m_Double & (long)b.m_Double));
      else if(a.m_Content==Content.VNull && b.m_Content==Content.VNull)
        return new AltaxoVariant();
      else if(a.m_Content==Content.VOperatable && ((IOperatable)a.m_Object).vop_And(b.m_Content==Content.VDouble ? b.m_Double : b.m_Object, out result))
        return new AltaxoVariant(result);
      else if(b.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_And_Rev(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to apply operator and to types " + a.m_Content.ToString() + " and " + b.m_Content.ToString());  
    }

    public static AltaxoVariant operator | (AltaxoVariant a, AltaxoVariant b)
    {
      object result;
      if(a.m_Content==Content.VDouble && b.m_Content==Content.VDouble)
        return new AltaxoVariant((double)((long)a.m_Double | (long)b.m_Double));
      else if(a.m_Content==Content.VNull && b.m_Content==Content.VNull)
        return new AltaxoVariant();
      else if(a.m_Content==Content.VOperatable && ((IOperatable)a.m_Object).vop_Or(b.m_Content==Content.VDouble ? b.m_Double : b.m_Object, out result))
        return new AltaxoVariant(result);
      else if(b.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_Or_Rev(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to apply operator OR to types " + a.m_Content.ToString() + " and " + b.m_Content.ToString()); 
    }

    public static AltaxoVariant operator ^ (AltaxoVariant a, AltaxoVariant b)
    {
      object result;
      if(a.m_Content==Content.VDouble && b.m_Content==Content.VDouble)
        return new AltaxoVariant((double)((long)a.m_Double ^ (long)b.m_Double));
      else if(a.m_Content==Content.VNull && b.m_Content==Content.VNull)
        return new AltaxoVariant();
      else if(a.m_Content==Content.VOperatable && ((IOperatable)a.m_Object).vop_Xor(b.m_Content==Content.VDouble ? b.m_Double : b.m_Object, out result))
        return new AltaxoVariant(result);
      else if(b.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_Xor_Rev(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to apply operator XOR to types " + a.m_Content.ToString() + " and " + b.m_Content.ToString());  
    }

    public static AltaxoVariant operator << (AltaxoVariant a, int b)
    {
      object result;
      if(a.m_Content==Content.VDouble)
        return new AltaxoVariant((double)((long)a.m_Double << b));
      else if(a.m_Content==Content.VNull)
        return new AltaxoVariant();
      else if(a.m_Content==Content.VOperatable && ((IOperatable)a.m_Object).vop_ShiftLeft(b, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to apply operator << to types " + a.m_Content.ToString() + " and " + b.ToString()); 
    }

    public static AltaxoVariant operator >> (AltaxoVariant a, int b)
    {
      object result;
      if(a.m_Content==Content.VDouble)
        return new AltaxoVariant((double)((long)a.m_Double >> b));
      else if(a.m_Content==Content.VNull)
        return new AltaxoVariant();
      else if(a.m_Content==Content.VOperatable && ((IOperatable)a.m_Object).vop_ShiftRight(b, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to apply operator >> to types " + a.m_Content.ToString() + " and " + b.ToString()); 
    }




    public static bool operator == (AltaxoVariant a, AltaxoVariant b)
    {
      bool result;

      if(a.m_Content != b.m_Content)
        return false;
      else if(a.m_Content==Content.VDouble)
        return (a.m_Double == b.m_Double);
      else if(a.m_Content == Content.VDateTime)
        return (((System.DateTime)a.m_Object) == ((System.DateTime)b.m_Object));
      else if(a.m_Content == Content.VString)
        return 0==(((String)a.m_Object).CompareTo((string)b.m_Object));
      else if(a.m_Content==Content.VNull)
        return false;
      else if(a.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_Equal(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return result;
      else if(b.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_Equal_Rev(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return result;
      else return false;  
    }

    public static bool operator != (AltaxoVariant a, AltaxoVariant b)
    {
      return !(a==b);
    }

    public static bool operator < (AltaxoVariant a, AltaxoVariant b)
    {
      bool result;

      if(a.m_Content == Content.VNull || b.m_Content==Content.VNull)
        return false;
      else if(a.m_Content != b.m_Content)
        throw new AltaxoOperatorException("Error: Try to compare types " + a.m_Content.ToString() + " and " + b.m_Content.ToString());
      
      if(a.m_Content == Content.VDouble)
        return (a.m_Double < b.m_Double);
      else if(a.m_Content == Content.VDateTime)
        return (((System.DateTime)a.m_Object) < ((System.DateTime)b.m_Object));
      else if(a.m_Content== Content.VString)
        return 0>(((String)a.m_Object).CompareTo((string)b.m_Object));
      else if(a.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_Lesser(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return result;
      else if(b.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_Lesser_Rev(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return result;
      else throw new AltaxoOperatorException("Error: Try to compare types " + a.m_Content.ToString() + " and " + b.m_Content.ToString());
    }

    public static bool operator > (AltaxoVariant a, AltaxoVariant b)
    {
      bool result;

      if(a.m_Content == Content.VNull || b.m_Content==Content.VNull)
        return false;
      else if(a.m_Content != b.m_Content)
        throw new AltaxoOperatorException("Error: Try to compare types " + a.m_Content.ToString() + " and " + b.m_Content.ToString());
      
      if(a.m_Content == Content.VDouble)
        return (a.m_Double > b.m_Double);
      else if(a.m_Content == Content.VDateTime)
        return (((System.DateTime)a.m_Object) > ((System.DateTime)b.m_Object));
      else if(a.m_Content== Content.VString)
        return 0<(((String)a.m_Object).CompareTo((string)b.m_Object));
      else if(a.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_Greater(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return result;
      else if(b.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_Greater_Rev(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return result;
      else throw new AltaxoOperatorException("Error: Try to compare types " + a.m_Content.ToString() + " and " + b.m_Content.ToString());
    }


    public static bool operator <= (AltaxoVariant a, AltaxoVariant b)
    {
      bool result;

      if(a.m_Content == Content.VNull || b.m_Content==Content.VNull)
        return false;
      else if(a.m_Content != b.m_Content)
        throw new AltaxoOperatorException("Error: Try to compare types " + a.m_Content.ToString() + " and " + b.m_Content.ToString());
      
      if(a.m_Content == Content.VDouble)
        return (a.m_Double <= b.m_Double);
      else if(a.m_Content == Content.VDateTime)
        return (((System.DateTime)a.m_Object) <= ((System.DateTime)b.m_Object));
      else if(a.m_Content== Content.VString)
        return 0>=(((String)a.m_Object).CompareTo((string)b.m_Object));
      else if(a.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_LesserOrEqual(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return result;
      else if(b.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_LesserOrEqual_Rev(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return result;
      else throw new AltaxoOperatorException("Error: Try to compare types " + a.m_Content.ToString() + " and " + b.m_Content.ToString());
    }

    public static bool operator >= (AltaxoVariant a, AltaxoVariant b)
    {
      bool result;

      if(a.m_Content == Content.VNull || b.m_Content==Content.VNull)
        return false;
      else if(a.m_Content != b.m_Content)
        throw new AltaxoOperatorException("Error: Try to compare types " + a.m_Content.ToString() + " and " + b.m_Content.ToString());
      
      if(a.m_Content == Content.VDouble)
        return (a.m_Double >= b.m_Double);
      else if(a.m_Content == Content.VDateTime)
        return (((System.DateTime)a.m_Object) >= ((System.DateTime)b.m_Object));
      else if(a.m_Content== Content.VString)
        return 0>=(((String)a.m_Object).CompareTo((string)b.m_Object));
      else if(a.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_GreaterOrEqual(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return result;
      else if(b.m_Content==Content.VOperatable && ((IOperatable)b.m_Object).vop_GreaterOrEqual_Rev(a.m_Content==Content.VDouble ? a.m_Double : a.m_Object, out result))
        return result;
      else throw new AltaxoOperatorException("Error: Try to compare types " + a.m_Content.ToString() + " and " + b.m_Content.ToString());
    }


    // Unary operators

    public static AltaxoVariant operator + (AltaxoVariant a)
    {
      object result;
      switch(a.m_Content)
      {
        case Content.VNull:
        case Content.VDouble:
        case Content.VDateTime:
          return new AltaxoVariant(a);
          
        case Content.VOperatable:
          if(((IOperatable)a.m_Object).vop_Plus(out result))
            return new AltaxoVariant(result);
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary plus operator to variant " + a.ToString());
    }


    public static AltaxoVariant operator - (AltaxoVariant a)
    {
      object result;
      switch(a.m_Content)
      {
        case Content.VNull:
          return new AltaxoVariant();
        case Content.VDouble:
          return new AltaxoVariant(-a.m_Double);
        case Content.VOperatable:
          if(((IOperatable)a.m_Object).vop_Minus(out result))
            return new AltaxoVariant(result);
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary minus operator to variant " + a.ToString());
    }

    public static AltaxoVariant operator ! (AltaxoVariant a)
    {
      object result;
      switch(a.m_Content)
      {
        case Content.VNull:
          return new AltaxoVariant();
        case Content.VDouble:
          return new AltaxoVariant((double)(a.m_Double == 0 ? 1 : 0));
        case Content.VOperatable:
          if(((IOperatable)a.m_Object).vop_Not(out result))
            return new AltaxoVariant(result);
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary not operator to variant " + a.ToString());
    }

    public static AltaxoVariant operator ~ (AltaxoVariant a)
    {
      object result;
      switch(a.m_Content)
      {
        case Content.VNull:
          return new AltaxoVariant();
        case Content.VDouble:
          return new AltaxoVariant((double)~(long)a.m_Double);
        case Content.VOperatable:
          if(((IOperatable)a.m_Object).vop_Complement(out result))
            return new AltaxoVariant(result);
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary complement operator to variant " + a.ToString());
    }

    public static AltaxoVariant operator ++ (AltaxoVariant a)
    {
      object result;
      switch(a.m_Content)
      {
        case Content.VNull:
          return new AltaxoVariant();
        case Content.VDouble:
          return new AltaxoVariant(a.m_Double+1);
        case Content.VOperatable:
          if(((IOperatable)a.m_Object).vop_Increment(out result))
            return new AltaxoVariant(result);
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary increment operator to variant " + a.ToString());
    }

    public static AltaxoVariant operator -- (AltaxoVariant a)
    {
      object result;
      switch(a.m_Content)
      {
        case Content.VNull:
          return new AltaxoVariant();
        case Content.VDouble:
          return new AltaxoVariant(a.m_Double-1);
        case Content.VOperatable:
          if(((IOperatable)a.m_Object).vop_Decrement(out result))
            return new AltaxoVariant(result);
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary decrement operator to variant " + a.ToString());
    }

    public static bool operator true (AltaxoVariant a)
    {
      bool result;
      switch(a.m_Content)
      {
        case Content.VNull:
          return false;
        case Content.VDouble:
          return a.m_Double!=0 ? true : false;
        case Content.VOperatable:
          if(((IOperatable)a.m_Object).vop_True(out result))
            return result;
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary true operator to variant " + a.ToString());
    }

    public static bool operator false (AltaxoVariant a)
    {
      bool result;
      switch(a.m_Content)
      {
        case Content.VNull:
          return false;
        case Content.VDouble:
          return a.m_Double==0 ? true : false;
        case Content.VOperatable:
          if(((IOperatable)a.m_Object).vop_False(out result))
            return result;
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary false operator to variant " + a.ToString());
    }


    #region IComparable Members

    int IComparable.CompareTo(object obj)
    {
      if (!(obj is AltaxoVariant))
        throw new Exception("Can not compare AltaxoVariant to an object of type " + obj.GetType().ToString());
      
      AltaxoVariant from = (AltaxoVariant)obj;

      if (this.m_Content != from.m_Content)
        throw new Exception(string.Format("A variant of type {0} can not be compared to a variant of type {1}", this.m_Content.ToString(), from.m_Content.ToString()));

      // both have the same content
      switch (m_Content)
      {
        case Content.VNull:
          return 0;
        case Content.VDouble:
          return m_Double.CompareTo(from.m_Double);
        case Content.VDateTime:
          return ((DateTime)m_Object).CompareTo(from.m_Object);
        case Content.VString:
          return ((string)m_Object).CompareTo(from.m_Object);
        default:
          if (this.m_Object is IComparable)
            return ((IComparable)this).CompareTo(from.m_Object);
          else
            throw new Exception(string.Format("The inner object of this AltaxoVariant (of typeof: {0}) does not implement IComparable",this.m_Object.GetType().ToString()));
      }
    }

    #endregion
  } // end of AltaxoVariant
} // end of namespace
