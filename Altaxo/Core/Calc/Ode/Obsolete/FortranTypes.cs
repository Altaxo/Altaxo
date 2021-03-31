﻿#region Copyright © 2009 Jose Antonio De Santiago-Castillo.

//Copyright © 2009 Jose Antonio De Santiago-Castillo
//E-mail:JAntonioDeSantiago@gmail.com
//Web: www.DotNumerics.com
//

#endregion Copyright © 2009 Jose Antonio De Santiago-Castillo.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.Ode.Obsolete
{
  public class Odouble
  {
    private double _v = 0;

    public double v
    {
      get { return _v; }
      set { _v = value; }
    }
  }

  public class Oint
  {
    private int _v = 0;

    public int v
    {
      get { return _v; }
      set { _v = value; }
    }
  }

  public class CommonBlock
  {
    #region Fields

    //private string MeName = "";

#nullable disable
    private Odouble[] MeDoubleData;
    private Oint[] MeIntData;
#nullable enable

    #endregion Fields

    #region Constructor

    public CommonBlock(int DimensionDouble, int DimensionInt, int DimensionSingle, int DimensionComplex)
    {
      //this.MeName = TheName;
      if (DimensionDouble > 0)
      {
        MeDoubleData = new Odouble[DimensionDouble];

        for (int i = 0; i < DimensionDouble; i++)
        {
          MeDoubleData[i] = new Odouble();
        }
      }
      if (DimensionInt > 0)
      {
        MeIntData = new Oint[DimensionInt];

        for (int i = 0; i < DimensionInt; i++)
        {
          MeIntData[i] = new Oint();
        }
      }
    }

    #endregion Constructor

    #region Properties

    public Odouble[] doubleData
    {
      get { return MeDoubleData; }
      set { MeDoubleData = value; }
    }

    public Oint[] intData
    {
      get { return MeIntData; }
      set { MeIntData = value; }
    }

    #endregion Properties
  }

  public class Characters
  {
    #region Borrar

    //#region Fields

    //private char[] _CharArray = new char[0];
    //private int _Offset = 1;
    //private int _Length = 0;

    //#endregion

    //#region Constructor

    //public Characters(int length)
    //{
    //    if (length < 0) length = 0;
    //    this._Length = length;
    //    this._CharArray = new char[length];

    //    for (int i = 0; i < this._CharArray.Length; i++)
    //    {
    //        this._CharArray[i] = ' ';
    //    }
    //}

    //public Characters(char[] sourceChars)
    //{
    //    if (sourceChars == null)
    //    {
    //        return;
    //    }
    //    this._CharArray = sourceChars;
    //    this._Length = sourceChars.Length;
    //}

    //public Characters(char[] sourceChars, int offset)
    //{
    //    if (sourceChars == null)
    //    {
    //        return;
    //    }
    //    this._CharArray = sourceChars;
    //    this._Length = sourceChars.Length - offset + 1;
    //    this._Offset = offset;
    //}

    //public Characters(char[] sourceChars, int offset, int length)
    //{
    //    if (sourceChars == null)
    //    {
    //        return;
    //    }
    //    this._CharArray = sourceChars;
    //    this._Length = length;
    //    this._Offset = offset;
    //}

    //#endregion

    //#region Properties

    //public char[] CharArray
    //{
    //    get { return this._CharArray; }
    //}

    //public int Offset
    //{
    //    get { return _Offset; }
    //    set { _Offset = value; }
    //}

    //public int Length
    //{
    //    get { return _Length; }
    //    set { _Length = value; }
    //}

    //public char this[int index]
    //{
    //    get
    //    {
    //        return this._CharArray[index - this._Offset];
    //    }
    //    set
    //    {
    //        this._CharArray[index - this._Offset] = value;
    //    }
    //}

    //#endregion

    #endregion Borrar

    #region Fields

    private char[] _CharArray = new char[0];

    #endregion Fields

    #region Constructor

    public Characters(int length)
    {
      if (length < 0)
        length = 0;
      _CharArray = new char[length];

      for (int i = 0; i < _CharArray.Length; i++)
      {
        _CharArray[i] = ' ';
      }
    }

    public Characters(char[] sourceChars, bool copy)
    {
      if (sourceChars is null)
      {
        return;
      }
      if (copy == true)
      {
        _CharArray = new char[sourceChars.Length];
        Array.Copy(sourceChars, _CharArray, _CharArray.Length);
      }
      else
      {
        _CharArray = sourceChars;
      }
    }

    public Characters(string s) : this(s.ToCharArray(), false)
    {
    }

    public Characters(string s, int length)
        : this(length)
    {
      Copy(s);
    }

    #endregion Constructor

    #region Properties

    public char[] CharArray
    {
      get { return _CharArray; }
    }

    public int Length
    {
      get { return _CharArray.Length; }
    }

    #endregion Properties

    #region Methods

    public void Copy(int startIndex, int lastIndex, char[] sourceCharArray)
    {
      if (lastIndex > Length)
        lastIndex = Length;
      int length = lastIndex - startIndex + 1;
      length = Math.Min(length, sourceCharArray.Length);

      startIndex--; //C# index

      Array.Copy(sourceCharArray, 0, _CharArray, startIndex, length);

      //for (int i = 0; i < length; i++)
      //{
      //    this._CharArray[startIndex + i] = sourceCharArray[i];
      //}
    }

    public void Copy(int startIndex, char[] sourceCharArray)
    {
      Copy(startIndex, Length, sourceCharArray);
    }

    public void Copy(char[] sourceCharArray)
    {
      Copy(1, sourceCharArray);
    }

    public void Copy(int startIndex, int lastIndex, string sourceString)
    {
      Copy(startIndex, lastIndex, sourceString.ToCharArray());
    }

    public void Copy(int startIndex, string sourceString)
    {
      Copy(startIndex, sourceString.ToCharArray());
    }

    public void Copy(string sourceString)
    {
      Copy(sourceString.ToCharArray());
    }

    public void Replace(char[] source)
    {
      ToBlanks();
      Copy(source);
    }

    public void Replace(Characters source)
    {
      Replace(source.CharArray);
    }

    public void Replace(string source)
    {
      Replace(source.ToCharArray());
    }

    public Characters Substring(int startIndex, int lastIndex)
    {
      int length = lastIndex - startIndex + 1;
      var sub = new Characters(length);

      startIndex--; //C# index
      Array.Copy(_CharArray, startIndex, sub.CharArray, 0, length);

      return sub;
    }

    public Characters Substring(int startIndex)
    {
      return Substring(startIndex, Length);
    }

    public void ToUpper()
    {
      for (int i = 0; i < _CharArray.Length; i++)
      {
        _CharArray[i] = char.ToUpper(_CharArray[i]);
      }
    }

    public void ToLower()
    {
      for (int i = 0; i < _CharArray.Length; i++)
      {
        _CharArray[i] = char.ToLower(_CharArray[i]);
      }
    }

    public void ToBlanks()
    {
      for (int i = 0; i < _CharArray.Length; i++)
      {
        _CharArray[i] = ' ';
      }
    }

    public void ToBlanks(int start, int last)
    {
      start--;
      int max = Math.Min(last, _CharArray.Length);
      for (int i = start; i < max; i++)
      {
        _CharArray[i] = ' ';
      }
    }

    public void ToBlanks(int length)
    {
      ToBlanks(1, length + 1);
    }

    public int ToInt32()
    {
      if (LenTrim() == 0)
        return 0;
      string s = new string(_CharArray);
      int val = Convert.ToInt32(s);
      return val;
    }

    public string Trim()
    {
      string s = new string(_CharArray);
      return s.TrimEnd(new char[] { ' ' });
    }

    public void AdjustLeft()
    {
      int numLeftBlanks = 0;
      for (int i = 0; i < _CharArray.Length; i++)
      {
        if (_CharArray[i] == ' ')
        {
          numLeftBlanks++;
        }
        else
        {
          break;
        }
      }

      if (numLeftBlanks == 0)
        return;
      else
      {
        int lastIndex = _CharArray.Length - numLeftBlanks;
        for (int i = 0; i < _CharArray.Length; i++)
        {
          if (i < lastIndex)
          {
            _CharArray[i] = _CharArray[numLeftBlanks + i];
          }
          else
          {
            _CharArray[i] = ' ';
          }
        }
      }
    }

    public void AdjustRight()
    {
      Array.Reverse(_CharArray);
      AdjustLeft();
      Array.Reverse(_CharArray);
    }

    public int LenTrim()
    {
      int lentrim = _CharArray.Length;
      char blank = ' ';

      for (int i = _CharArray.Length - 1; i > -1; i--)
      {
        if (_CharArray[i] == blank)
        {
          lentrim--;
        }
        else
        {
          break;
        }
      }
      return lentrim;
    }

    #endregion Methods

    #region Operators

    #region operator +

    public static Characters operator +(Characters c1, Characters c2)
    {
      return Characters.Add(c1.CharArray, c2.CharArray);
    }

    public static Characters operator +(string s, Characters c2)
    {
      return Characters.Add(s.ToCharArray(), c2.CharArray);
    }

    public static Characters operator +(Characters c1, string s)
    {
      return Characters.Add(c1.CharArray, s.ToCharArray());
    }

    public static Characters Add(char[] c1, char[] c2)
    {
      var newCharacters = new Characters(c1.Length + c2.Length);

      Array.Copy(c1, newCharacters.CharArray, c1.Length);

      Array.Copy(c2, 0, newCharacters.CharArray, c1.Length, c2.Length);

      return newCharacters;
    }

    #endregion operator +

    #region Operators == and !=

    public static bool operator ==(Characters c1, Characters c2)
    {
      return Characters.AreEqual(c1.CharArray, c2.CharArray);
    }

    public static bool operator ==(string s, Characters c2)
    {
      return Characters.AreEqual(s.ToCharArray(), c2.CharArray);
    }

    public static bool operator ==(Characters c1, string s)
    {
      return Characters.AreEqual(c1.CharArray, s.ToCharArray());
    }

    public static bool operator !=(Characters c1, Characters c2)
    {
      return !Characters.AreEqual(c1.CharArray, c2.CharArray);
    }

    public static bool operator !=(string s, Characters c2)
    {
      return !Characters.AreEqual(s.ToCharArray(), c2.CharArray);
    }

    public static bool operator !=(Characters c1, string s)
    {
      return !Characters.AreEqual(c1.CharArray, s.ToCharArray());
    }

    public static bool AreEqual(char[] left, char[] right)
    {
      int maxLength = Math.Max(left.Length, right.Length);
      char[] c1 = Characters.GetCharArray(left, maxLength);
      char[] c2 = Characters.GetCharArray(right, maxLength);

      if (c1.Length != c2.Length)
        return false;

      bool areEqual = true;

      for (int i = 0; i < c1.Length; i++)
      {
        if (c1[i] != c2[i])
        {
          areEqual = false;
          break;
        }
      }
      return areEqual;
    }

    public override bool Equals(object? obj)
    {
      // If parameter is null return false.
      if (obj is null)
      {
        return false;
      }

      // If parameter cannot be cast to Characters return false.
      if (!(obj is Characters c))
      {
        return false;
      }

      // Return true if the fields match:
      return Characters.AreEqual(c.CharArray, CharArray);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    #endregion Operators == and !=

    #region operator Large Than (> ,>=)

    public static bool operator >(Characters c1, Characters c2)
    {
      return Characters.LargeThan(c1.CharArray, c2.CharArray);
    }

    public static bool operator >(string s, Characters c2)
    {
      return Characters.LargeThan(s.ToCharArray(), c2.CharArray);
    }

    public static bool operator >(Characters c1, string s)
    {
      return Characters.LargeThan(c1.CharArray, s.ToCharArray());
    }

    public static bool operator >=(Characters c1, Characters c2)
    {
      bool isLE = Characters.AreEqual(c1.CharArray, c2.CharArray) || Characters.LargeThan(c1.CharArray, c2.CharArray);
      return isLE;
    }

    public static bool operator >=(string s, Characters c2)
    {
      bool isLE = Characters.AreEqual(s.ToCharArray(), c2.CharArray) || Characters.LargeThan(s.ToCharArray(), c2.CharArray);
      return isLE;
    }

    public static bool operator >=(Characters c1, string s)
    {
      bool isLE = Characters.AreEqual(c1.CharArray, s.ToCharArray()) || Characters.LargeThan(c1.CharArray, s.ToCharArray());
      return isLE;
    }

    public static bool LargeThan(char[] left, char[] right)
    {
      //For all relational operators, the collating sequence is used to interpret a
      //character relational expression. The character expression whose value is lower
      //in the collating sequence is less than the other expression. The character
      //expressions are evaluated one character at a time from left to right. You can
      //also use the intrinsic functions (LGE, LLT, and LLT) to compare character
      //strings in the order specified by the ASCII collating sequence. For all
      //relational operators, if the operands are of unequal length, the shorter is
      //extended on the right with blanks. If both char_expr1 and char_expr2 are of
      //zero length, they are evaluated as equal.

      int maxLength = Math.Max(left.Length, right.Length);
      char[] leftCharArray = Characters.GetCharArray(left, maxLength);
      char[] rightCharArray = Characters.GetCharArray(right, maxLength);

      bool isLargeThan = false;

      for (int i = 0; i < leftCharArray.Length; i++)
      {
        if (leftCharArray[i] != rightCharArray[i])
        {
          if (leftCharArray[i] > rightCharArray[i])
          {
            isLargeThan = true;
            break;
          }
          else if (leftCharArray[i] < rightCharArray[i])
          {
            isLargeThan = false;
            break;
          }
        }
      }
      return isLargeThan;
    }

    #endregion operator Large Than (> ,>=)

    #region operator Large Than (< ,<=)

    public static bool operator <(Characters c1, Characters c2)
    {
      return Characters.LeastThan(c1.CharArray, c2.CharArray);
    }

    public static bool operator <(string s, Characters c2)
    {
      return Characters.LeastThan(s.ToCharArray(), c2.CharArray);
    }

    public static bool operator <(Characters c1, string s)
    {
      return Characters.LeastThan(c1.CharArray, s.ToCharArray());
    }

    public static bool operator <=(Characters c1, Characters c2)
    {
      bool isLE = Characters.AreEqual(c1.CharArray, c2.CharArray) || Characters.LeastThan(c1.CharArray, c2.CharArray);
      return isLE;
    }

    public static bool operator <=(string s, Characters c2)
    {
      bool isLE = Characters.AreEqual(s.ToCharArray(), c2.CharArray) || Characters.LeastThan(s.ToCharArray(), c2.CharArray);
      return isLE;
    }

    public static bool operator <=(Characters c1, string s)
    {
      bool isLE = Characters.AreEqual(c1.CharArray, s.ToCharArray()) || Characters.LeastThan(c1.CharArray, s.ToCharArray());
      return isLE;
    }

    public static bool LeastThan(char[] left, char[] right)
    {
      //For all relational operators, the collating sequence is used to interpret a
      //character relational expression. The character expression whose value is lower
      //in the collating sequence is less than the other expression. The character
      //expressions are evaluated one character at a time from left to right. You can
      //also use the intrinsic functions (LGE, LLT, and LLT) to compare character
      //strings in the order specified by the ASCII collating sequence. For all
      //relational operators, if the operands are of unequal length, the shorter is
      //extended on the right with blanks. If both char_expr1 and char_expr2 are of
      //zero length, they are evaluated as equal.

      int maxLength = Math.Max(left.Length, right.Length);
      char[] leftCharArray = Characters.GetCharArray(left, maxLength);
      char[] rightCharArray = Characters.GetCharArray(right, maxLength);

      bool isLeastThan = false;

      for (int i = 0; i < leftCharArray.Length; i++)
      {
        if (leftCharArray[i] != rightCharArray[i])
        {
          if (leftCharArray[i] < rightCharArray[i])
          {
            isLeastThan = true;
            break;
          }
          else if (leftCharArray[i] > rightCharArray[i])
          {
            isLeastThan = false;
            break;
          }
        }
      }
      return isLeastThan;
    }

    #endregion operator Large Than (< ,<=)

    private static char[] GetCharArray(char[] source, int length)
    {
      if (source.Length == length)
        return source;
      char[] newCharArray = new char[length];
      int sourceLength = Math.Min(source.Length, length);

      for (int i = 0; i < sourceLength; i++)
      {
        newCharArray[i] = source[i];
      }

      for (int i = sourceLength; i < newCharArray.Length; i++)
      {
        newCharArray[i] = ' ';
      }

      return newCharArray;
    }

    //public static void NormalizeSize(ref char[] c1, ref char[] c2)
    //{
    //    if (c1.Length != c2.Length)
    //    {
    //        if (c1.Length > c2.Length)
    //        {
    //            char[] c = new char[c1.Length];
    //            Array.Copy(c2, c, c2.Length);
    //            for (int i = c2.Length; i < c.Length; i++)
    //            {
    //                c[i] = ' ';
    //            }
    //            c2 = c;
    //        }
    //        else if (c1.Length < c2.Length)
    //        {
    //            char[] c = new char[c2.Length];
    //            Array.Copy(c1, c, c1.Length);
    //            for (int i = c1.Length; i < c.Length; i++)
    //            {
    //                c[i] = ' ';
    //            }
    //            c1 = c;
    //        }
    //    }

    //}

    #region implicit Operators

    public static implicit operator Characters(string s)  // implicit string to Characters conversion operator
    {
      var TheChararacters = new Characters(s);
      return TheChararacters;
    }

    #endregion implicit Operators

    #endregion Operators

    public override string ToString()
    {
      return new string(_CharArray);
    }
  }
}
