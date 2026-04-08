#region Copyright © 2009 Jose Antonio De Santiago-Castillo.

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
  /// <summary>
  /// Represents a double-precision floating-point number.
  /// </summary>
  public class Odouble
  {
    private double _v = 0;

    /// <summary>
    /// Gets or sets the value of the Odouble instance.
    /// </summary>
    public double v
    {
      get { return _v; }
      set { _v = value; }
    }
  }

  /// <summary>
  /// Represents a 32-bit signed integer.
  /// </summary>
  public class Oint
  {
    private int _v = 0;

    /// <summary>
    /// Gets or sets the value of the Oint instance.
    /// </summary>
    public int v
    {
      get { return _v; }
      set { _v = value; }
    }
  }

  /// <summary>
  /// Represents a common block that can contain double-precision, single-precision, and integer data.
  /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the CommonBlock class with the specified dimensions for double, integer, single, and complex data.
    /// </summary>
    /// <param name="DimensionDouble">The dimension of the double-precision data.</param>
    /// <param name="DimensionInt">The dimension of the integer data.</param>
    /// <param name="DimensionSingle">The dimension of the single-precision data.</param>
    /// <param name="DimensionComplex">The dimension of the complex data.</param>
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

    /// <summary>
    /// Gets or sets the double-precision data of the CommonBlock.
    /// </summary>
    public Odouble[] doubleData
    {
      get { return MeDoubleData; }
      set { MeDoubleData = value; }
    }

    /// <summary>
    /// Gets or sets the integer data of the CommonBlock.
    /// </summary>
    public Oint[] intData
    {
      get { return MeIntData; }
      set { MeIntData = value; }
    }

    #endregion Properties
  }

  /// <summary>
  /// Represents a character array and provides methods for manipulating and converting character data.
  /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the Characters class with the specified length.
    /// </summary>
    /// <param name="length">The length of the character array.</param>
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

    /// <summary>
    /// Initializes a new instance of the Characters class with the specified character array.
    /// </summary>
    /// <param name="sourceChars">The source character array.</param>
    /// <param name="copy">If set to true, the contents of the source character array are copied; otherwise, the reference is used.</param>
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

    /// <summary>
    /// Initializes a new instance of the Characters class with the specified string.
    /// </summary>
    /// <param name="s">The source string.</param>
    public Characters(string s) : this(s.ToCharArray(), false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Characters class with the specified string and length.
    /// </summary>
    /// <param name="s">The source string.</param>
    /// <param name="length">The length of the character array.</param>
    public Characters(string s, int length)
        : this(length)
    {
      Copy(s);
    }

    #endregion Constructor

    #region Properties

    /// <summary>
    /// Gets the underlying character array.
    /// </summary>
    public char[] CharArray
    {
      get { return _CharArray; }
    }

    /// <summary>
    /// Gets the length of the character array.
    /// </summary>
    public int Length
    {
      get { return _CharArray.Length; }
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Copies a range of characters from the specified source character array to this instance.
    /// </summary>
    /// <param name="startIndex">The starting index (1-based) in the destination array.</param>
    /// <param name="lastIndex">The ending index (1-based) in the destination array.</param>
    /// <param name="sourceCharArray">The source character array.</param>
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

    /// <summary>
    /// Copies characters from the specified source character array to this instance.
    /// </summary>
    /// <param name="startIndex">The starting index (1-based) in the destination array.</param>
    /// <param name="sourceCharArray">The source character array.</param>
    public void Copy(int startIndex, char[] sourceCharArray)
    {
      Copy(startIndex, Length, sourceCharArray);
    }

    /// <summary>
    /// Copies characters from the specified source character array to this instance.
    /// </summary>
    /// <param name="sourceCharArray">The source character array.</param>
    public void Copy(char[] sourceCharArray)
    {
      Copy(1, sourceCharArray);
    }

    /// <summary>
    /// Copies a range of characters from the specified source string to this instance.
    /// </summary>
    /// <param name="startIndex">The starting index (1-based) in the destination array.</param>
    /// <param name="lastIndex">The ending index (1-based) in the destination array.</param>
    /// <param name="sourceString">The source string.</param>
    public void Copy(int startIndex, int lastIndex, string sourceString)
    {
      Copy(startIndex, lastIndex, sourceString.ToCharArray());
    }

    /// <summary>
    /// Copies characters from the specified source string to this instance.
    /// </summary>
    /// <param name="startIndex">The starting index (1-based) in the destination array.</param>
    /// <param name="sourceString">The source string.</param>
    public void Copy(int startIndex, string sourceString)
    {
      Copy(startIndex, sourceString.ToCharArray());
    }

    /// <summary>
    /// Copies characters from the specified source string to this instance.
    /// </summary>
    /// <param name="sourceString">The source string.</param>
    public void Copy(string sourceString)
    {
      Copy(sourceString.ToCharArray());
    }

    /// <summary>
    /// Replaces the contents of this instance with the specified character array.
    /// </summary>
    /// <param name="source">The source character array.</param>
    public void Replace(char[] source)
    {
      ToBlanks();
      Copy(source);
    }

    /// <summary>
    /// Replaces the contents of this instance with the specified Characters instance.
    /// </summary>
    /// <param name="source">The source Characters instance.</param>
    public void Replace(Characters source)
    {
      Replace(source.CharArray);
    }

    /// <summary>
    /// Replaces the contents of this instance with the specified string.
    /// </summary>
    /// <param name="source">The source string.</param>
    public void Replace(string source)
    {
      Replace(source.ToCharArray());
    }

    /// <summary>
    /// Creates and returns a new Characters object that is a substring of this instance.
    /// </summary>
    /// <param name="startIndex">The starting index (1-based) of the substring.</param>
    /// <param name="lastIndex">The ending index (1-based) of the substring.</param>
    /// <returns>A new Characters object that represents the substring.</returns>
    public Characters Substring(int startIndex, int lastIndex)
    {
      int length = lastIndex - startIndex + 1;
      var sub = new Characters(length);

      startIndex--; //C# index
      Array.Copy(_CharArray, startIndex, sub.CharArray, 0, length);

      return sub;
    }

    /// <summary>
    /// Creates and returns a new Characters object that is a substring of this instance.
    /// </summary>
    /// <param name="startIndex">The starting index (1-based) of the substring.</param>
    /// <returns>A new Characters object that represents the substring.</returns>
    public Characters Substring(int startIndex)
    {
      return Substring(startIndex, Length);
    }

    /// <summary>
    /// Converts all characters in the array to uppercase.
    /// </summary>
    public void ToUpper()
    {
      for (int i = 0; i < _CharArray.Length; i++)
      {
        _CharArray[i] = char.ToUpper(_CharArray[i]);
      }
    }

    /// <summary>
    /// Converts all characters in the array to lowercase.
    /// </summary>
    public void ToLower()
    {
      for (int i = 0; i < _CharArray.Length; i++)
      {
        _CharArray[i] = char.ToLower(_CharArray[i]);
      }
    }

    /// <summary>
    /// Fills the entire character array with blank spaces.
    /// </summary>
    public void ToBlanks()
    {
      for (int i = 0; i < _CharArray.Length; i++)
      {
        _CharArray[i] = ' ';
      }
    }

    /// <summary>
    /// Fills a specified range of the character array with blank spaces.
    /// </summary>
    /// <param name="start">The starting index (1-based) of the range.</param>
    /// <param name="last">The ending index (1-based) of the range.</param>
    public void ToBlanks(int start, int last)
    {
      start--;
      int max = Math.Min(last, _CharArray.Length);
      for (int i = start; i < max; i++)
      {
        _CharArray[i] = ' ';
      }
    }

    /// <summary>
    /// Fills the specified number of characters in the array with blank spaces.
    /// </summary>
    /// <param name="length">The number of characters to fill with blanks.</param>
    public void ToBlanks(int length)
    {
      ToBlanks(1, length + 1);
    }

    /// <summary>
    /// Converts the character data to a 32-bit signed integer.
    /// </summary>
    /// <returns>The integer value of the character data.</returns>
    public int ToInt32()
    {
      if (LenTrim() == 0)
        return 0;
      string s = new string(_CharArray);
      int val = Convert.ToInt32(s);
      return val;
    }

    /// <summary>
    /// Trims trailing blank spaces and returns the resulting string.
    /// </summary>
    /// <returns>A string that represents the trimmed character data.</returns>
    public string Trim()
    {
      string s = new string(_CharArray);
      return s.TrimEnd(new char[] { ' ' });
    }

    /// <summary>
    /// Adjusts the characters in the array to the left by removing leading blank spaces.
    /// </summary>
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

    /// <summary>
    /// Adjusts the characters in the array to the right by removing trailing blank spaces.
    /// </summary>
    public void AdjustRight()
    {
      Array.Reverse(_CharArray);
      AdjustLeft();
      Array.Reverse(_CharArray);
    }

    /// <summary>
    /// Gets the length of the character data after trimming trailing blank spaces.
    /// </summary>
    /// <returns>The length of the trimmed character data.</returns>
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

    /// <summary>
    /// Concatenates two Characters instances.
    /// </summary>
    /// <param name="c1">The first Characters instance.</param>
    /// <param name="c2">The second Characters instance.</param>
    /// <returns>A new Characters instance that represents the concatenation of the two instances.</returns>
    public static Characters operator +(Characters c1, Characters c2)
    {
      return Characters.Add(c1.CharArray, c2.CharArray);
    }

    /// <summary>
    /// Concatenates a string and a Characters instance.
    /// </summary>
    /// <param name="s">The string to concatenate.</param>
    /// <param name="c2">The Characters instance to concatenate.</param>
    /// <returns>A new Characters instance that represents the concatenation of the string and the instance.</returns>
    public static Characters operator +(string s, Characters c2)
    {
      return Characters.Add(s.ToCharArray(), c2.CharArray);
    }

    /// <summary>
    /// Concatenates a Characters instance and a string.
    /// </summary>
    /// <param name="c1">The Characters instance to concatenate.</param>
    /// <param name="s">The string to concatenate.</param>
    /// <returns>A new Characters instance that represents the concatenation of the instance and the string.</returns>
    public static Characters operator +(Characters c1, string s)
    {
      return Characters.Add(c1.CharArray, s.ToCharArray());
    }

    /// <summary>
    /// Concatenates two character arrays and returns a new Characters instance.
    /// </summary>
    /// <param name="c1">The first character array.</param>
    /// <param name="c2">The second character array.</param>
    /// <returns>A new Characters instance that represents the concatenation of the two character arrays.</returns>
    public static Characters Add(char[] c1, char[] c2)
    {
      var newCharacters = new Characters(c1.Length + c2.Length);

      Array.Copy(c1, newCharacters.CharArray, c1.Length);

      Array.Copy(c2, 0, newCharacters.CharArray, c1.Length, c2.Length);

      return newCharacters;
    }

    #endregion operator +

    #region Operators == and !=

    /// <summary>
    /// Checks if two Characters instances are equal.
    /// </summary>
    /// <param name="c1">The first Characters instance.</param>
    /// <param name="c2">The second Characters instance.</param>
    /// <returns>true if the two instances are equal; otherwise, false.</returns>
    public static bool operator ==(Characters c1, Characters c2)
    {
      return Characters.AreEqual(c1.CharArray, c2.CharArray);
    }

    /// <summary>
    /// Checks if a string and a Characters instance are equal.
    /// </summary>
    /// <param name="s">The string to compare.</param>
    /// <param name="c2">The Characters instance to compare.</param>
    /// <returns>true if the string and the instance are equal; otherwise, false.</returns>
    public static bool operator ==(string s, Characters c2)
    {
      return Characters.AreEqual(s.ToCharArray(), c2.CharArray);
    }

    /// <summary>
    /// Checks if a Characters instance and a string are equal.
    /// </summary>
    /// <param name="c1">The Characters instance to compare.</param>
    /// <param name="s">The string to compare.</param>
    /// <returns>true if the instance and the string are equal; otherwise, false.</returns>
    public static bool operator ==(Characters c1, string s)
    {
      return Characters.AreEqual(c1.CharArray, s.ToCharArray());
    }

    /// <summary>
    /// Checks if two Characters instances are not equal.
    /// </summary>
    /// <param name="c1">The first Characters instance.</param>
    /// <param name="c2">The second Characters instance.</param>
    /// <returns>true if the two instances are not equal; otherwise, false.</returns>
    public static bool operator !=(Characters c1, Characters c2)
    {
      return !Characters.AreEqual(c1.CharArray, c2.CharArray);
    }

    /// <summary>
    /// Checks if a string and a Characters instance are not equal.
    /// </summary>
    /// <param name="s">The string to compare.</param>
    /// <param name="c2">The Characters instance to compare.</param>
    /// <returns>true if the string and the instance are not equal; otherwise, false.</returns>
    public static bool operator !=(string s, Characters c2)
    {
      return !Characters.AreEqual(s.ToCharArray(), c2.CharArray);
    }

    /// <summary>
    /// Checks if a Characters instance and a string are not equal.
    /// </summary>
    /// <param name="c1">The Characters instance to compare.</param>
    /// <param name="s">The string to compare.</param>
    /// <returns>true if the instance and the string are not equal; otherwise, false.</returns>
    public static bool operator !=(Characters c1, string s)
    {
      return !Characters.AreEqual(c1.CharArray, s.ToCharArray());
    }

    /// <summary>
    /// Determines whether two character arrays are equal.
    /// </summary>
    /// <param name="left">The first character array.</param>
    /// <param name="right">The second character array.</param>
    /// <returns>true if the character arrays are equal; otherwise, false.</returns>
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

    /// <summary>
    /// Determines whether the specified object is equal to the current Characters instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>true if the specified object is equal to the current instance; otherwise, false.</returns>
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

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A hash code for the current Characters instance.</returns>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    #endregion Operators == and !=

    #region operator Large Than (> ,>=)

    /// <summary>
    /// Checks if the first Characters instance is greater than the second.
    /// </summary>
    /// <param name="c1">The first Characters instance.</param>
    /// <param name="c2">The second Characters instance.</param>
    /// <returns>true if the first instance is greater; otherwise, false.</returns>
    public static bool operator >(Characters c1, Characters c2)
    {
      return Characters.LargeThan(c1.CharArray, c2.CharArray);
    }

    /// <summary>
    /// Checks if a string is greater than a Characters instance.
    /// </summary>
    /// <param name="s">The string to compare.</param>
    /// <param name="c2">The Characters instance to compare.</param>
    /// <returns>true if the string is greater; otherwise, false.</returns>
    public static bool operator >(string s, Characters c2)
    {
      return Characters.LargeThan(s.ToCharArray(), c2.CharArray);
    }

    /// <summary>
    /// Checks if a Characters instance is greater than a string.
    /// </summary>
    /// <param name="c1">The Characters instance to compare.</param>
    /// <param name="s">The string to compare.</param>
    /// <returns>true if the Characters instance is greater; otherwise, false.</returns>
    public static bool operator >(Characters c1, string s)
    {
      return Characters.LargeThan(c1.CharArray, s.ToCharArray());
    }

    /// <summary>
    /// Checks if the first Characters instance is greater than or equal to the second.
    /// </summary>
    /// <param name="c1">The first Characters instance.</param>
    /// <param name="c2">The second Characters instance.</param>
    /// <returns>true if the first instance is greater than or equal to the second; otherwise, false.</returns>
    public static bool operator >=(Characters c1, Characters c2)
    {
      bool isLE = Characters.AreEqual(c1.CharArray, c2.CharArray) || Characters.LargeThan(c1.CharArray, c2.CharArray);
      return isLE;
    }

    /// <summary>
    /// Checks if a string is greater than or equal to a Characters instance.
    /// </summary>
    /// <param name="s">The string to compare.</param>
    /// <param name="c2">The Characters instance to compare.</param>
    /// <returns>true if the string is greater than or equal to the Characters instance; otherwise, false.</returns>
    public static bool operator >=(string s, Characters c2)
    {
      bool isLE = Characters.AreEqual(s.ToCharArray(), c2.CharArray) || Characters.LargeThan(s.ToCharArray(), c2.CharArray);
      return isLE;
    }

    /// <summary>
    /// Checks if a Characters instance is greater than or equal to a string.
    /// </summary>
    /// <param name="c1">The Characters instance to compare.</param>
    /// <param name="s">The string to compare.</param>
    /// <returns>true if the Characters instance is greater than or equal to the string; otherwise, false.</returns>
    public static bool operator >=(Characters c1, string s)
    {
      bool isLE = Characters.AreEqual(c1.CharArray, s.ToCharArray()) || Characters.LargeThan(c1.CharArray, s.ToCharArray());
      return isLE;
    }

    /// <summary>
    /// Determines whether the first character array is larger than the second.
    /// </summary>
    /// <param name="left">The first character array.</param>
    /// <param name="right">The second character array.</param>
    /// <returns>true if the first character array is larger; otherwise, false.</returns>
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

    /// <summary>
    /// Checks if the first Characters instance is less than the second.
    /// </summary>
    /// <param name="c1">The first Characters instance.</param>
    /// <param name="c2">The second Characters instance.</param>
    /// <returns>true if the first instance is less; otherwise, false.</returns>
    public static bool operator <(Characters c1, Characters c2)
    {
      return Characters.LeastThan(c1.CharArray, c2.CharArray);
    }

    /// <summary>
    /// Checks if a string is less than a Characters instance.
    /// </summary>
    /// <param name="s">The string to compare.</param>
    /// <param name="c2">The Characters instance to compare.</param>
    /// <returns>true if the string is less; otherwise, false.</returns>
    public static bool operator <(string s, Characters c2)
    {
      return Characters.LeastThan(s.ToCharArray(), c2.CharArray);
    }

    /// <summary>
    /// Checks if a Characters instance is less than a string.
    /// </summary>
    /// <param name="c1">The Characters instance to compare.</param>
    /// <param name="s">The string to compare.</param>
    /// <returns>true if the Characters instance is less; otherwise, false.</returns>
    public static bool operator <(Characters c1, string s)
    {
      return Characters.LeastThan(c1.CharArray, s.ToCharArray());
    }

    /// <summary>
    /// Checks if the first Characters instance is less than or equal to the second.
    /// </summary>
    /// <param name="c1">The first Characters instance.</param>
    /// <param name="c2">The second Characters instance.</param>
    /// <returns>true if the first instance is less than or equal to the second; otherwise, false.</returns>
    public static bool operator <=(Characters c1, Characters c2)
    {
      bool isLE = Characters.AreEqual(c1.CharArray, c2.CharArray) || Characters.LeastThan(c1.CharArray, c2.CharArray);
      return isLE;
    }

    /// <summary>
    /// Checks if a string is less than or equal to a Characters instance.
    /// </summary>
    /// <param name="s">The string to compare.</param>
    /// <param name="c2">The Characters instance to compare.</param>
    /// <returns>true if the string is less than or equal to the Characters instance; otherwise, false.</returns>
    public static bool operator <=(string s, Characters c2)
    {
      bool isLE = Characters.AreEqual(s.ToCharArray(), c2.CharArray) || Characters.LeastThan(s.ToCharArray(), c2.CharArray);
      return isLE;
    }

    /// <summary>
    /// Checks if a Characters instance is less than or equal to a string.
    /// </summary>
    /// <param name="c1">The Characters instance to compare.</param>
    /// <param name="s">The string to compare.</param>
    /// <returns>true if the Characters instance is less than or equal to the string; otherwise, false.</returns>
    public static bool operator <=(Characters c1, string s)
    {
      bool isLE = Characters.AreEqual(c1.CharArray, s.ToCharArray()) || Characters.LeastThan(c1.CharArray, s.ToCharArray());
      return isLE;
    }

    /// <summary>
    /// Determines whether the first character array is less than the second.
    /// </summary>
    /// <param name="left">The first character array.</param>
    /// <param name="right">The second character array.</param>
    /// <returns>true if the first character array is less; otherwise, false.</returns>
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

    /// <summary>
    /// Implicitly converts a string to a Characters instance.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>A new Characters instance that represents the string.</returns>
    public static implicit operator Characters(string s)  // implicit string to Characters conversion operator
    {
      var TheChararacters = new Characters(s);
      return TheChararacters;
    }

    #endregion implicit Operators

    #endregion Operators

    /// <summary>
    /// Returns a string that represents the current Characters instance.
    /// </summary>
    /// <returns>A string that represents the current Characters instance.</returns>
    public override string ToString()
    {
      return new string(_CharArray);
    }
  }
}
