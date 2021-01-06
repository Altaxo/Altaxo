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

namespace Altaxo.Data
{
  public interface IOperatable
  {
    // Note: unfortunately (and maybe also undocumented) we can not use
    // the names op_Addition, op_Subtraction and so one, because these
    // names seems to be used by the compiler for the operators itself
    // so we use here vopAddition and so on (the v from virtual)

    bool vop_Addition(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Addition_Rev(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Subtraction(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Subtraction_Rev(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Multiplication(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Multiplication_Rev(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Division(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Division_Rev(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Modulo(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Modulo_Rev(object a, [MaybeNullWhen(false)] out object b);

    bool vop_And(object a, [MaybeNullWhen(false)] out object b);

    bool vop_And_Rev(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Or(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Or_Rev(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Xor(object a, [MaybeNullWhen(false)] out object b);

    bool vop_Xor_Rev(object a, [MaybeNullWhen(false)] out object b);

    bool vop_ShiftLeft(object a, [MaybeNullWhen(false)] out object b);

    bool vop_ShiftLeft_Rev(object a, [MaybeNullWhen(false)] out object b);

    bool vop_ShiftRight(object a, [MaybeNullWhen(false)] out object b);

    bool vop_ShiftRight_Rev(object a, [MaybeNullWhen(false)] out object b);

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

    bool vop_Plus([MaybeNullWhen(false)] out object a);

    bool vop_Minus([MaybeNullWhen(false)] out object a);

    bool vop_Not([MaybeNullWhen(false)] out object a);

    bool vop_Complement([MaybeNullWhen(false)] out object a);

    bool vop_Increment([MaybeNullWhen(false)] out object a);

    bool vop_Decrement([MaybeNullWhen(false)] out object a);

    bool vop_True(out bool a);

    bool vop_False(out bool a);
  }

  /// <summary>
  /// AltaxoVariant is the universal datatype used to return the value of a data column,
  /// it is necessary because the type of the column can be text, date, or double
  /// I decided to use struct which only holds the object because of its efficiency
  /// AltaxoVariant is never used to store the data in the array, for this purpose
  /// the native data types are used
  /// </summary>
  public struct AltaxoVariant : IComparable, IFormattable
  {
    public enum Content { VNull, VDouble, VDateTime, VString, VOperatable, VObject, VDateTimeOffset }

    private Content _typeOfContent;
    private double _double;
    private object? _object;

    #region Serialization

    #region Version 0

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AltaxoVariant), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AltaxoVariant)obj;
        info.AddEnum("Content", s._typeOfContent);
        switch (s._typeOfContent)
        {
          case Content.VNull:
            break;

          case Content.VDouble:
            info.AddValue("Value", s._double);
            break;

          case Content.VDateTime:
            info.AddValue("Value", (DateTime)s._object!);
            break;

          case Content.VString:
            info.AddValue("Value", s._object ?? string.Empty);
            break;

          default:
            info.AddValue("Value", s._object!);
            break;
        }
      }

      protected virtual AltaxoVariant SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AltaxoVariant?)o ?? new AltaxoVariant();
        var c = (Content)info.GetEnum("Content", typeof(Content));
        s._typeOfContent = c;

        switch (c)
        {
          case Content.VNull:
            break;

          case Content.VDouble:
            s._double = info.GetDouble("Value");
            break;

          case Content.VDateTime:
            s._object = info.GetDateTime("Value");
            break;

          case Content.VString:
            s._object = info.GetString("Value");
            break;

          default:
            s._object = info.GetValue("Value", s);
            break;
        }

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        AltaxoVariant s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    public AltaxoVariant(AltaxoVariant a)
    {
      _typeOfContent = a._typeOfContent;
      _double = a._double;
      _object = a._object;
    }

    public AltaxoVariant(double d)
    {
      _typeOfContent = Content.VDouble;
      _double = d;
      _object = null;
    }

    public AltaxoVariant(DateTime f)
    {
      _typeOfContent = Content.VDateTime;
      _object = f;
      _double = 0;
    }

    public AltaxoVariant(DateTimeOffset f)
    {
      _typeOfContent = Content.VDateTimeOffset;
      _object = f;
      _double = 0;
    }

    public AltaxoVariant(string? s)
    {
      _typeOfContent = Content.VString;
      _object = s;
      _double = 0;
    }

    public AltaxoVariant(object? k)
    {
      switch(k)
      {
        case null:
          _typeOfContent = Content.VNull;
          _double = 0;
          _object = null;
          break;
        case double dd:
          _typeOfContent = Content.VDouble;
          _double = dd;
          _object = null;
          break;
        case DateTime:
          _typeOfContent = Content.VDateTime;
          _double = 0;
          _object = k;
          break;
        case string:
          _typeOfContent = Content.VString;
          _double = 0;
          _object = k;
          break;
        case IOperatable:
          _typeOfContent = Content.VOperatable;
          _double = 0;
          _object = k;
          break;
        case AltaxoVariant av:
          _typeOfContent = av._typeOfContent;
          _double = av._double;
          _object = av._object; // is critical, because the object is not cloned, so be warned for the first time
          break;
        case DateTimeOffset:
          _typeOfContent = Content.VDateTime;
          _double = 0;
          _object = k;
          break;
        default:
          _typeOfContent = Content.VObject;
          _double = 0;
          _object = k;
          break;
      }
    }

    public bool IsType(Content c)
    {
      return _typeOfContent == c;
    }

    public bool IsTypeOrNull(Content c)
    {
      return _typeOfContent == c || _typeOfContent == Content.VNull;
    }

    public bool CanConvertedToDouble
    {
      get
      {
        if (_typeOfContent == Content.VDouble || _typeOfContent == Content.VDateTime || _typeOfContent==Content.VDateTimeOffset)
          return true; // we can create a double from a double (trivial) and from DateTime
        if (_typeOfContent == Content.VString) // if the content is a string, we have to look if it is possible to convert
          return Altaxo.Serialization.NumberConversion.IsNumeric((string?)_object);
        else if (_object is not null)
          return Altaxo.Serialization.NumberConversion.IsNumeric(_object.ToString());
        else
          return false; // it is not possible to convert the contents to a double
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance holds a native numeric value,
    /// as for instance of type double or DateTime.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is native numeric; otherwise, <c>false</c>.
    /// </value>
    public bool IsNativeNumeric
    {
      get
      {
        if (_typeOfContent == Content.VDouble || _typeOfContent == Content.VDateTime || _typeOfContent == Content.VDateTimeOffset)
          return true; // we can create a double from a double (trivial) and from DateTime
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
      return _typeOfContent switch
      {
        Content.VDouble => _double,
        Content.VDateTime => ((DateTime)_object!).Ticks / 10000000.0,
        Content.VDateTimeOffset => ((DateTimeOffset)_object!).Ticks / 10000000.0,
        Content.VString => System.Convert.ToDouble((string?)_object),
        _ => _object is not null ? System.Convert.ToDouble(_object.ToString()) : throw new ApplicationException("Unable to convert the contents of this variant to a number, the contents is: " + ToString())
      };
    }

    /// <summary>
    /// Converts the variant to a nullable boolean value.
    /// </summary>
    /// <returns>Nullable boolean value.</returns>
    /// <exception cref="ApplicationException">Unable to convert the contents of this variant to a number, the contents is: " + ToString()</exception>
    public bool? ToNullableBoolean()
    {
      if (_typeOfContent == Content.VDouble)
      {
        return Double.IsNaN(_double) ? null : (bool?)(!(_double == 0));
      }
      else if (_typeOfContent == Content.VString)
      {
        return bool.TryParse(_object as string ?? string.Empty, out var result) ? (bool?)result : null;
      }
      else if (_object is not null)
      {
        return bool.TryParse(_object.ToString(), out var result) ? (bool?)result : null;
      }
      else
      {
        throw new ApplicationException("Unable to convert the contents of this variant to a number, the contents is: " + ToString());
      }

    }

    /// <summary>
    /// Converts the content to a double, if possible. The structure remains unchanged.
    /// </summary>
    /// <returns>The content converted to a double.</returns>
    /// <remarks>No exception is thrown if the conversion is not possible. Instead, NaN is returned.</remarks>
    public double ToDoubleOrNaN()
    {
      if (_typeOfContent == Content.VDouble)
        return _double;
      else if (_typeOfContent == Content.VDateTime)
        return ((DateTime)_object!).Ticks / 10000000.0;
      else if (_typeOfContent == Content.VString)
      {
        if (double.TryParse((string?)_object, System.Globalization.NumberStyles.Float, Altaxo.Settings.GuiCulture.Instance, out double result))
          return result;
      }
      else if (_object is not null)
      {
        try
        {
          return System.Convert.ToDouble(_object.ToString());
        }
        catch (Exception)
        {
        }
      }

      return double.NaN;
    }

    /// <summary>
    /// Converts the content to a DateTime if possible. The structure remains unchanged.
    /// </summary>
    /// <returns>The contents converted to a DateTime.</returns>
    /// <remarks>An exception is thrown if the conversion fails. </remarks>
    public DateTime ToDateTime()
    {
      if (_typeOfContent == Content.VDouble)
        return new DateTime((long)(_double * 10000000.0));
      else if (_typeOfContent == Content.VDateTime)
        return (DateTime)_object!;
      else if (_typeOfContent == Content.VString)
        return System.Convert.ToDateTime((string?)_object);
      else if (_object is not null)
        return System.Convert.ToDateTime(_object.ToString());
      else
        throw new ApplicationException("Unable to convert the contents of this variant to a DateTime, the contents is: " + ToString());
    }

    /// <summary>
    /// Converts the content to a DateTime if possible. The structure remains unchanged.
    /// </summary>
    /// <returns>The contents converted to a DateTime.</returns>
    /// <remarks>An exception is thrown if the conversion fails. </remarks>
    public DateTimeOffset ToDateTimeOffset()
    {
      if (_typeOfContent == Content.VDouble)
        return new DateTimeOffset((long)(_double * 10000000.0), TimeSpan.Zero);
      else if (_typeOfContent == Content.VDateTime)
        return new DateTimeOffset((DateTime)_object!);
      else if (_typeOfContent == Content.VDateTimeOffset)
        return (DateTimeOffset)_object!;
      else
      {
        var s = _typeOfContent == Content.VString ? (string?)_object : _object?.ToString();
        if (s is not null && DateTime.TryParse(s, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dto))
          return dto;
        else
          throw new ApplicationException($"Unable to convert the contents of this variant to a DateTimeOffset, the contents is: {ToString()}");
      }
    }

    public override string ToString()
    {
      if (_typeOfContent == Content.VNull)
        return "(null)";
      else if (_typeOfContent == Content.VDouble)
        return _double.ToString();
      else if (_object is not null)
        return _object.ToString() ?? string.Empty;
      else // everything is null
        return "";
    }

    public string ToString(string? formatString, IFormatProvider? provider)
    {
      try
      {
        if (_typeOfContent == Content.VNull)
          return "(null)";
        else if (_typeOfContent == Content.VDouble)
          return _double.ToString(formatString, provider);
        else if (_typeOfContent == Content.VDateTime)
          return ((DateTime)_object!).ToString(formatString, provider);
        else if (_typeOfContent == Content.VDateTimeOffset)
          return ((DateTimeOffset)_object!).ToString(formatString, provider);
        else if (_typeOfContent == Content.VString)
          return ((string?)_object)?.ToString(provider) ?? string.Empty;
        else if (_object is not null)
          return _object.ToString() ?? string.Empty;
        else // everything is null
          return "";
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    /// <summary>
    /// Converts the content to an object. This conversion is always possible. The structure remains unchanged.
    /// </summary>
    /// <returns>The contents converted to an object.</returns>
    public object ToObject()
    {
      if (_typeOfContent == Content.VDouble)
        return _double;
      else
        return _object!;
    }

    public override bool Equals(object? k)
    {
      if (k is AltaxoVariant av)
        return this == av;
      else
        return this == new AltaxoVariant(k);
    }

    public override int GetHashCode()
    {
      if (_typeOfContent == Content.VNull)
        return _typeOfContent.GetHashCode();
      else if (_typeOfContent == Content.VDouble)
        return _double.GetHashCode();
      else
        return _object!.GetHashCode();
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
      if (f._typeOfContent == Content.VDouble)
        return f._double;
      throw new ApplicationException("Variant contains " + f._typeOfContent.ToString() + ", but expecting type Double");
    }

    public static implicit operator AltaxoVariant(double f)
    {
      return new AltaxoVariant(f);
    }

    public static implicit operator DateTime(AltaxoVariant f)
    {
      if (f._typeOfContent == Content.VDateTime)
        return (DateTime)f._object!;
      throw new ApplicationException("Variant contains " + f._typeOfContent.ToString() + ", but expecting type DateTime");
    }

    public static implicit operator DateTimeOffset(AltaxoVariant f)
    {
      if (f._typeOfContent == Content.VDateTimeOffset)
        return (DateTimeOffset)f._object!;
      throw new ApplicationException("Variant contains " + f._typeOfContent.ToString() + ", but expecting type DateTimeOffset");
    }

    public static implicit operator AltaxoVariant(DateTime f)
    {
      return new AltaxoVariant(f);
    }

    public static implicit operator AltaxoVariant(DateTimeOffset f)
    {
      return new AltaxoVariant(f);
    }

    public static implicit operator string?(AltaxoVariant f)
    {
      if (f._typeOfContent == Content.VString)
        return (string?)f._object;
      throw new ApplicationException("Variant contains " + f._typeOfContent.ToString() + ", but expecting type string");
    }

    public static implicit operator AltaxoVariant(string? f)
    {
      return new AltaxoVariant(f);
    }

    public static AltaxoVariant operator +(AltaxoVariant a, AltaxoVariant b)
    {

      if (a._typeOfContent == Content.VDouble && b._typeOfContent == Content.VDouble)
        return new AltaxoVariant(a._double + b._double);
      else if (a._typeOfContent == Content.VString && b._typeOfContent == Content.VString)
        return new AltaxoVariant(((string?)a._object) + ((string?)b._object));
      else if (a._typeOfContent == Content.VDateTime && b._typeOfContent == Content.VDouble)
        return new AltaxoVariant(((DateTime)a._object!).AddSeconds(b._double));
      else if (a._typeOfContent == Content.VDateTimeOffset && b._typeOfContent == Content.VDouble)
        return new AltaxoVariant(((DateTimeOffset)a._object!).AddSeconds(b._double));
      else if (a._typeOfContent == Content.VDouble && b._typeOfContent == Content.VDateTime)
        return new AltaxoVariant(((DateTime)b._object!).AddSeconds(a._double));
      else if (a._typeOfContent == Content.VDouble && b._typeOfContent == Content.VDateTimeOffset)
        return new AltaxoVariant(((DateTimeOffset)b._object!).AddSeconds(a._double));
      else if (a._typeOfContent == Content.VString && b._typeOfContent == Content.VDouble)
        return new AltaxoVariant(((string?)a._object) + b._double.ToString());
      else if (a._typeOfContent == Content.VString && b._typeOfContent == Content.VDateTime)
        return new AltaxoVariant(((string?)a._object) + ((System.DateTime)b._object!).ToString());
      else if (a._typeOfContent == Content.VNull && b._typeOfContent == Content.VNull)
        return new AltaxoVariant();
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)a._object!).vop_Addition(b._typeOfContent == Content.VDouble ? b._double : b._object!, out var result))
        return new AltaxoVariant(result);
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Addition_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to add types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.GetType().ToString());
    }

    public static AltaxoVariant operator -(AltaxoVariant a, AltaxoVariant b)
    {

      if (a._typeOfContent == Content.VDouble && b._typeOfContent == Content.VDouble)
        return new AltaxoVariant(a._double - b._double);
      else if (a._typeOfContent == Content.VDateTime && a._typeOfContent == Content.VDouble)
        return new AltaxoVariant(((DateTime)a._object!).AddSeconds(-b._double));
      else if (a._typeOfContent == Content.VDateTimeOffset && a._typeOfContent == Content.VDouble)
        return new AltaxoVariant(((DateTimeOffset)a._object!).AddSeconds(-b._double));
      else if (a._typeOfContent == Content.VDateTime && b._typeOfContent == Content.VDateTime)
        return new AltaxoVariant((((DateTime)a._object!) - ((DateTime)b._object!)).TotalSeconds);
      else if (a._typeOfContent == Content.VDateTimeOffset && b._typeOfContent == Content.VDateTimeOffset)
        return new AltaxoVariant((((DateTimeOffset)a._object!) - ((DateTimeOffset)b._object!)).TotalSeconds);
      else if (a._typeOfContent == Content.VNull && b._typeOfContent == Content.VNull)
        return new AltaxoVariant();
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)a._object!).vop_Subtraction(b._typeOfContent == Content.VDouble ? b._double : b._object!, out var result))
        return new AltaxoVariant(result);
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Subtraction_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to subtract types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());
    }

    public static AltaxoVariant operator *(AltaxoVariant a, AltaxoVariant b)
    {

      if (a._typeOfContent == Content.VDouble && b._typeOfContent == Content.VDouble)
        return new AltaxoVariant(a._double * b._double);
      else if (a._typeOfContent == Content.VDouble && b._typeOfContent == Content.VDateTime)
        return new AltaxoVariant(DateTime.FromBinary((long)(a._double * ((DateTime)b._object!).Ticks)));
      else if (a._typeOfContent == Content.VDateTime && b._typeOfContent == Content.VDouble)
        return new AltaxoVariant(DateTime.FromBinary((long)(b._double * ((DateTime)a._object!).Ticks)));
      else if (a._typeOfContent == Content.VNull && b._typeOfContent == Content.VNull)
        return new AltaxoVariant();
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)a._object!).vop_Multiplication(b._typeOfContent == Content.VDouble ? b._double : b._object!, out var result))
        return new AltaxoVariant(result);
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Multiplication_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to multiply types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());
    }

    public static AltaxoVariant operator /(AltaxoVariant a, AltaxoVariant b)
    {
      if (a._typeOfContent == Content.VDouble && b._typeOfContent == Content.VDouble)
        return new AltaxoVariant(a._double / b._double);
      else if (a._typeOfContent == Content.VNull && b._typeOfContent == Content.VNull)
        return new AltaxoVariant();
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)a._object!).vop_Division(b._typeOfContent == Content.VDouble ? b._double : b._object!, out var result))
        return new AltaxoVariant(result);
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Division_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to divide types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());
    }

    public static AltaxoVariant operator %(AltaxoVariant a, AltaxoVariant b)
    {
      if (a._typeOfContent == Content.VDouble && b._typeOfContent == Content.VDouble)
        return new AltaxoVariant(a._double % b._double);
      else if (a._typeOfContent == Content.VNull && b._typeOfContent == Content.VNull)
        return new AltaxoVariant();
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)a._object!).vop_Modulo(b._typeOfContent == Content.VDouble ? b._double : b._object!, out var result))
        return new AltaxoVariant(result);
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Modulo_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to get remainder of types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());
    }

    public static AltaxoVariant operator &(AltaxoVariant a, AltaxoVariant b)
    {
      if (a._typeOfContent == Content.VDouble && b._typeOfContent == Content.VDouble)
        return new AltaxoVariant((double)((long)a._double & (long)b._double));
      else if (a._typeOfContent == Content.VNull && b._typeOfContent == Content.VNull)
        return new AltaxoVariant();
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)a._object!).vop_And(b._typeOfContent == Content.VDouble ? b._double : b._object!, out var result))
        return new AltaxoVariant(result);
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_And_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to apply operator and to types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());
    }

    public static AltaxoVariant operator |(AltaxoVariant a, AltaxoVariant b)
    {
      if (a._typeOfContent == Content.VDouble && b._typeOfContent == Content.VDouble)
        return new AltaxoVariant((double)((long)a._double | (long)b._double));
      else if (a._typeOfContent == Content.VNull && b._typeOfContent == Content.VNull)
        return new AltaxoVariant();
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)a._object!).vop_Or(b._typeOfContent == Content.VDouble ? b._double : b._object!, out var result))
        return new AltaxoVariant(result);
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Or_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to apply operator OR to types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());
    }

    public static AltaxoVariant operator ^(AltaxoVariant a, AltaxoVariant b)
    {
      if (a._typeOfContent == Content.VDouble && b._typeOfContent == Content.VDouble)
        return new AltaxoVariant((double)((long)a._double ^ (long)b._double));
      else if (a._typeOfContent == Content.VNull && b._typeOfContent == Content.VNull)
        return new AltaxoVariant();
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)a._object!).vop_Xor(b._typeOfContent == Content.VDouble ? b._double : b._object!, out var result))
        return new AltaxoVariant(result);
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Xor_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to apply operator XOR to types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());
    }

    public static AltaxoVariant operator <<(AltaxoVariant a, int b)
    {
      if (a._typeOfContent == Content.VDouble)
        return new AltaxoVariant((double)((long)a._double << b));
      else if (a._typeOfContent == Content.VNull)
        return new AltaxoVariant();
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)a._object!).vop_ShiftLeft(b, out var result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to apply operator << to types " + a._typeOfContent.ToString() + " and " + b.ToString());
    }

    public static AltaxoVariant operator >>(AltaxoVariant a, int b)
    {
      if (a._typeOfContent == Content.VDouble)
        return new AltaxoVariant((double)((long)a._double >> b));
      else if (a._typeOfContent == Content.VNull)
        return new AltaxoVariant();
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)a._object!).vop_ShiftRight(b, out var result))
        return new AltaxoVariant(result);
      else
        throw new AltaxoOperatorException("Error: Try to apply operator >> to types " + a._typeOfContent.ToString() + " and " + b.ToString());
    }

    public static bool operator ==(AltaxoVariant a, AltaxoVariant b)
    {

      if (a._typeOfContent != b._typeOfContent)
        return false;
      else if (a._typeOfContent == Content.VDouble)
        return (a._double == b._double);
      else if (a._typeOfContent == Content.VDateTime)
        return (((System.DateTime)a._object!) == ((System.DateTime)b._object!));
      else if (a._typeOfContent == Content.VDateTimeOffset)
        return (((System.DateTimeOffset)a._object!) == ((System.DateTimeOffset)b._object!));
      else if (a._typeOfContent == Content.VString)
        return 0 == string.Compare((string?)a._object, (string?)b._object);
      else if (a._typeOfContent == Content.VNull)
        return false;
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Equal(a._typeOfContent == Content.VDouble ? a._double : a._object!, out var result))
        return result;
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Equal_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return result;
      else
        return false;
    }

    public static bool operator !=(AltaxoVariant a, AltaxoVariant b)
    {
      return !(a == b);
    }

    public static bool operator <(AltaxoVariant a, AltaxoVariant b)
    {

      if (a._typeOfContent == Content.VNull || b._typeOfContent == Content.VNull)
        return false;
      else if (a._typeOfContent != b._typeOfContent)
        throw new AltaxoOperatorException("Error: Try to compare types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());

      if (a._typeOfContent == Content.VDouble)
        return (a._double < b._double);
      else if (a._typeOfContent == Content.VDateTime)
        return (((System.DateTime)a._object!) < ((System.DateTime)b._object!));
      else if (a._typeOfContent == Content.VDateTimeOffset)
        return (((System.DateTimeOffset)a._object!) < ((System.DateTimeOffset)b._object!));
      else if (a._typeOfContent == Content.VString)
        return 0 > string.Compare((string?)a._object, (string?)b._object);
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Lesser(a._typeOfContent == Content.VDouble ? a._double : a._object!, out var result))
        return result;
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Lesser_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return result;
      else
        throw new AltaxoOperatorException("Error: Try to compare types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());
    }

    public static bool operator >(AltaxoVariant a, AltaxoVariant b)
    {

      if (a._typeOfContent == Content.VNull || b._typeOfContent == Content.VNull)
        return false;
      else if (a._typeOfContent != b._typeOfContent)
        throw new AltaxoOperatorException("Error: Try to compare types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());

      if (a._typeOfContent == Content.VDouble)
        return (a._double > b._double);
      else if (a._typeOfContent == Content.VDateTime)
        return (((System.DateTime)a._object!) > ((System.DateTime)b._object!));
      else if (a._typeOfContent == Content.VDateTimeOffset)
        return (((System.DateTimeOffset)a._object!) > ((System.DateTimeOffset)b._object!));
      else if (a._typeOfContent == Content.VString)
        return 0 < string.Compare((string?)a._object, (string?)b._object);
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Greater(a._typeOfContent == Content.VDouble ? a._double : a._object!, out var result))
        return result;
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_Greater_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return result;
      else
        throw new AltaxoOperatorException("Error: Try to compare types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());
    }

    public static bool operator <=(AltaxoVariant a, AltaxoVariant b)
    {

      if (a._typeOfContent == Content.VNull || b._typeOfContent == Content.VNull)
        return false;
      else if (a._typeOfContent != b._typeOfContent)
        throw new AltaxoOperatorException("Error: Try to compare types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());

      if (a._typeOfContent == Content.VDouble)
        return (a._double <= b._double);
      else if (a._typeOfContent == Content.VDateTime)
        return (((System.DateTime)a._object!) <= ((System.DateTime)b._object!));
      else if (a._typeOfContent == Content.VDateTimeOffset)
        return (((System.DateTimeOffset)a._object!) <= ((System.DateTimeOffset)b._object!));
      else if (a._typeOfContent == Content.VString)
        return 0 >= string.Compare((string?)a._object, (string?)b._object);
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_LesserOrEqual(a._typeOfContent == Content.VDouble ? a._double : a._object!, out var result))
        return result;
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_LesserOrEqual_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return result;
      else
        throw new AltaxoOperatorException("Error: Try to compare types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());
    }

    public static bool operator >=(AltaxoVariant a, AltaxoVariant b)
    {

      if (a._typeOfContent == Content.VNull || b._typeOfContent == Content.VNull)
        return false;
      else if (a._typeOfContent != b._typeOfContent)
        throw new AltaxoOperatorException("Error: Try to compare types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());

      if (a._typeOfContent == Content.VDouble)
        return (a._double >= b._double);
      else if (a._typeOfContent == Content.VDateTime)
        return (((System.DateTime)a._object!) >= ((System.DateTime)b._object!));
      else if (a._typeOfContent == Content.VDateTimeOffset)
        return (((System.DateTimeOffset)a._object!) >= ((System.DateTimeOffset)b._object!));
      else if (a._typeOfContent == Content.VString)
        return 0 >= string.Compare((string?)a._object, (string?)b._object);
      else if (a._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_GreaterOrEqual(a._typeOfContent == Content.VDouble ? a._double : a._object!, out var result))
        return result;
      else if (b._typeOfContent == Content.VOperatable && ((IOperatable)b._object!).vop_GreaterOrEqual_Rev(a._typeOfContent == Content.VDouble ? a._double : a._object!, out result))
        return result;
      else
        throw new AltaxoOperatorException("Error: Try to compare types " + a._typeOfContent.ToString() + " and " + b._typeOfContent.ToString());
    }

    // Unary operators

    public static AltaxoVariant operator +(AltaxoVariant a)
    {
      switch (a._typeOfContent)
      {
        case Content.VNull:
        case Content.VDouble:
        case Content.VDateTime:
        case Content.VDateTimeOffset:
          return new AltaxoVariant(a);

        case Content.VOperatable:
          if (((IOperatable)a._object!).vop_Plus(out var result))
            return new AltaxoVariant(result);
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary plus operator to variant " + a.ToString());
    }

    public static AltaxoVariant operator -(AltaxoVariant a)
    {
      switch (a._typeOfContent)
      {
        case Content.VNull:
          return new AltaxoVariant();

        case Content.VDouble:
          return new AltaxoVariant(-a._double);

        case Content.VOperatable:
          if (((IOperatable)a._object!).vop_Minus(out var result))
            return new AltaxoVariant(result);
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary minus operator to variant " + a.ToString());
    }

    public static AltaxoVariant operator !(AltaxoVariant a)
    {
      switch (a._typeOfContent)
      {
        case Content.VNull:
          return new AltaxoVariant();

        case Content.VDouble:
          return new AltaxoVariant((double)(a._double == 0 ? 1 : 0));

        case Content.VOperatable:
          if (((IOperatable)a._object!).vop_Not(out var result))
            return new AltaxoVariant(result);
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary not operator to variant " + a.ToString());
    }

    public static AltaxoVariant operator ~(AltaxoVariant a)
    {
      switch (a._typeOfContent)
      {
        case Content.VNull:
          return new AltaxoVariant();

        case Content.VDouble:
          return new AltaxoVariant((double)~(long)a._double);

        case Content.VOperatable:
          if (((IOperatable)a._object!).vop_Complement(out var result))
            return new AltaxoVariant(result);
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary complement operator to variant " + a.ToString());
    }

    public static AltaxoVariant operator ++(AltaxoVariant a)
    {
      switch (a._typeOfContent)
      {
        case Content.VNull:
          return new AltaxoVariant();

        case Content.VDouble:
          return new AltaxoVariant(a._double + 1);

        case Content.VOperatable:
          if (((IOperatable)a._object!).vop_Increment(out var result))
            return new AltaxoVariant(result);
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary increment operator to variant " + a.ToString());
    }

    public static AltaxoVariant operator --(AltaxoVariant a)
    {
      switch (a._typeOfContent)
      {
        case Content.VNull:
          return new AltaxoVariant();

        case Content.VDouble:
          return new AltaxoVariant(a._double - 1);

        case Content.VOperatable:
          if (((IOperatable)a._object!).vop_Decrement(out var result))
            return new AltaxoVariant(result);
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary decrement operator to variant " + a.ToString());
    }

    public static bool operator true(AltaxoVariant a)
    {
      switch (a._typeOfContent)
      {
        case Content.VNull:
          return false;

        case Content.VDouble:
          return a._double != 0 ? true : false;

        case Content.VOperatable:
          if (((IOperatable)a._object!).vop_True(out var result))
            return result;
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary true operator to variant " + a.ToString());
    }

    public static bool operator false(AltaxoVariant a)
    {
      switch (a._typeOfContent)
      {
        case Content.VNull:
          return false;

        case Content.VDouble:
          return a._double == 0 ? true : false;

        case Content.VOperatable:
          if (((IOperatable)a._object!).vop_False(out var result))
            return result;
          break;
      }
      throw new AltaxoOperatorException("Error: Try to apply unary false operator to variant " + a.ToString());
    }

    #region IComparable Members

    int IComparable.CompareTo(object? obj)
    {
      if (!(obj is AltaxoVariant from))
        throw new Exception($"Can not compare AltaxoVariant to an object of type {obj?.GetType()}");

      if (_typeOfContent != from._typeOfContent)
        throw new Exception(string.Format("A variant of type {0} can not be compared to a variant of type {1}", _typeOfContent.ToString(), from._typeOfContent.ToString()));

      // both have the same content
      switch (_typeOfContent)
      {
        case Content.VNull:
          return 0;

        case Content.VDouble:
          return _double.CompareTo(from._double);

        case Content.VDateTime:
          return ((DateTime)_object!).CompareTo(from._object);

        case Content.VDateTimeOffset:
          return ((IComparable)_object!).CompareTo(from._object);

        case Content.VString:
          return string.Compare((string?)_object, (string?)from._object);

        default:
          if (_object is IComparable)
            return ((IComparable)_object).CompareTo(from._object);
          else
            throw new Exception($"The inner object of this AltaxoVariant (of typeof: {_object?.GetType()}) does not implement IComparable");
      }
    }

    #endregion IComparable Members
  } // end of AltaxoVariant
} // end of namespace
