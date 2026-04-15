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
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Data
{
  /// <summary>
  /// Base class for objects that expose virtual hooks for arithmetic, logical, and comparison operators.
  /// </summary>
  public abstract class OperatableObject
  {
    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    // Note: unfortunately (and maybe also undocumented) we can not use
    // the names op_Addition, op_Subtraction and so one, because these
    // names seems to be used by the compiler for the operators itself
    // so we use here vopAddition and so on (the v from virtual)

    /// <summary>
    /// Tries to apply addition with the specified right-hand operand.
    /// </summary>
    /// <param name="a">The right-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Addition(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply addition with the specified left-hand operand.
    /// </summary>
    /// <param name="a">The left-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Addition_Rev(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply subtraction with the specified right-hand operand.
    /// </summary>
    /// <param name="a">The right-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Subtraction(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply subtraction with the specified left-hand operand.
    /// </summary>
    /// <param name="a">The left-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Subtraction_Rev(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply multiplication with the specified right-hand operand.
    /// </summary>
    /// <param name="a">The right-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Multiplication(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply multiplication with the specified left-hand operand.
    /// </summary>
    /// <param name="a">The left-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Multiplication_Rev(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply division with the specified right-hand operand.
    /// </summary>
    /// <param name="a">The right-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Division(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply division with the specified left-hand operand.
    /// </summary>
    /// <param name="a">The left-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Division_Rev(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply modulo with the specified right-hand operand.
    /// </summary>
    /// <param name="a">The right-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Modulo(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply modulo with the specified left-hand operand.
    /// </summary>
    /// <param name="a">The left-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Modulo_Rev(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply bitwise/logical AND with the specified right-hand operand.
    /// </summary>
    /// <param name="a">The right-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_And(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply bitwise/logical AND with the specified left-hand operand.
    /// </summary>
    /// <param name="a">The left-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_And_Rev(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply bitwise/logical OR with the specified right-hand operand.
    /// </summary>
    /// <param name="a">The right-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Or(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply bitwise/logical OR with the specified left-hand operand.
    /// </summary>
    /// <param name="a">The left-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Or_Rev(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply XOR with the specified right-hand operand.
    /// </summary>
    /// <param name="a">The right-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Xor(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply XOR with the specified left-hand operand.
    /// </summary>
    /// <param name="a">The left-hand operand.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Xor_Rev(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply a left shift with the specified operand.
    /// </summary>
    /// <param name="a">The operand that specifies the shift count.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_ShiftLeft(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply a reversed left shift with the specified operand.
    /// </summary>
    /// <param name="a">The operand that specifies the shift count.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_ShiftLeft_Rev(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply a right shift with the specified operand.
    /// </summary>
    /// <param name="a">The operand that specifies the shift count.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_ShiftRight(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply a reversed right shift with the specified operand.
    /// </summary>
    /// <param name="a">The operand that specifies the shift count.</param>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_ShiftRight_Rev(object a, [MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to compare for equality with the specified operand.
    /// </summary>
    /// <param name="a">The operand to compare with this instance.</param>
    /// <param name="b">The comparison result if the comparison is supported.</param>
    /// <returns><see langword="true"/> if the comparison was performed; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Equal(object a, out bool b)
    { b = Equals(a); return true; }

    /// <summary>
    /// Tries to compare for equality with the specified operand in reversed order.
    /// </summary>
    /// <param name="a">The operand to compare with this instance.</param>
    /// <param name="b">The comparison result if the comparison is supported.</param>
    /// <returns><see langword="true"/> if the comparison was performed; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Equal_Rev(object a, out bool b)
    { b = a.Equals(this); return true; }

    /// <summary>
    /// Tries to compare for inequality with the specified operand.
    /// </summary>
    /// <param name="a">The operand to compare with this instance.</param>
    /// <param name="b">The comparison result if the comparison is supported.</param>
    /// <returns><see langword="true"/> if the comparison was performed; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_NotEqual(object a, out bool b)
    { b = !Equals(a); return true; }

    /// <summary>
    /// Tries to compare for inequality with the specified operand in reversed order.
    /// </summary>
    /// <param name="a">The operand to compare with this instance.</param>
    /// <param name="b">The comparison result if the comparison is supported.</param>
    /// <returns><see langword="true"/> if the comparison was performed; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_NotEqual_Rev(object a, out bool b)
    { b = !a.Equals(this); return true; }

    /// <summary>
    /// Tries to compare whether this instance is less than the specified operand.
    /// </summary>
    /// <param name="a">The operand to compare with this instance.</param>
    /// <param name="b">The comparison result if the comparison is supported.</param>
    /// <returns><see langword="true"/> if the comparison was performed; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Lesser(object a, out bool b)
    { b = false; return false; }

    /// <summary>
    /// Tries to compare whether this instance is greater than the specified operand in reversed order.
    /// </summary>
    /// <param name="a">The operand to compare with this instance.</param>
    /// <param name="b">The comparison result if the comparison is supported.</param>
    /// <returns><see langword="true"/> if the comparison was performed; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Lesser_Rev(object a, out bool b)
    { b = false; return false; }

    /// <summary>
    /// Tries to compare whether this instance is greater than the specified operand.
    /// </summary>
    /// <param name="a">The operand to compare with this instance.</param>
    /// <param name="b">The comparison result if the comparison is supported.</param>
    /// <returns><see langword="true"/> if the comparison was performed; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Greater(object a, out bool b)
    { b = false; return false; }

    /// <summary>
    /// Tries to compare whether this instance is less than the specified operand in reversed order.
    /// </summary>
    /// <param name="a">The operand to compare with this instance.</param>
    /// <param name="b">The comparison result if the comparison is supported.</param>
    /// <returns><see langword="true"/> if the comparison was performed; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Greater_Rev(object a, out bool b)
    { b = false; return false; }

    /// <summary>
    /// Tries to compare whether this instance is less than or equal to the specified operand.
    /// </summary>
    /// <param name="a">The operand to compare with this instance.</param>
    /// <param name="b">The comparison result if the comparison is supported.</param>
    /// <returns><see langword="true"/> if the comparison was performed; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_LesserOrEqual(object a, out bool b)
    { b = false; return false; }

    /// <summary>
    /// Tries to compare whether this instance is greater than or equal to the specified operand in reversed order.
    /// </summary>
    /// <param name="a">The operand to compare with this instance.</param>
    /// <param name="b">The comparison result if the comparison is supported.</param>
    /// <returns><see langword="true"/> if the comparison was performed; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_LesserOrEqual_Rev(object a, out bool b)
    { b = false; return false; }

    /// <summary>
    /// Tries to compare whether this instance is greater than or equal to the specified operand.
    /// </summary>
    /// <param name="a">The operand to compare with this instance.</param>
    /// <param name="b">The comparison result if the comparison is supported.</param>
    /// <returns><see langword="true"/> if the comparison was performed; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_GreaterOrEqual(object a, out bool b)
    { b = false; return false; }

    /// <summary>
    /// Tries to compare whether this instance is less than or equal to the specified operand in reversed order.
    /// </summary>
    /// <param name="a">The operand to compare with this instance.</param>
    /// <param name="b">The comparison result if the comparison is supported.</param>
    /// <returns><see langword="true"/> if the comparison was performed; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_GreaterOrEqual_Rev(object a, out bool b)
    { b = false; return false; }

    // Unary operators

    /// <summary>
    /// Tries to apply the unary plus operator.
    /// </summary>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Plus([MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply the unary minus operator.
    /// </summary>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Minus([MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply the logical negation operator.
    /// </summary>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Not([MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply the bitwise complement operator.
    /// </summary>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Complement([MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply the increment operator.
    /// </summary>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Increment([MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to apply the decrement operator.
    /// </summary>
    /// <param name="b">The operation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_Decrement([MaybeNullWhen(false)] out OperatableObject b)
    { b = null; return false; }

    /// <summary>
    /// Tries to evaluate the instance for the <c>true</c> operator.
    /// </summary>
    /// <param name="b">The boolean evaluation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_True(out bool b)
    { b = false; return false; }

    /// <summary>
    /// Tries to evaluate the instance for the <c>false</c> operator.
    /// </summary>
    /// <param name="b">The boolean evaluation result if the operation succeeds.</param>
    /// <returns><see langword="true"/> if the operation was applied; otherwise, <see langword="false"/>.</returns>
    public virtual bool vop_False(out bool b)
    { b = false; return false; }

    /// <summary>
    /// Applies the binary addition operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns>The result of the addition.</returns>
    public static OperatableObject operator +(OperatableObject c1, object c2)
    {

      if (c1.vop_Addition(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_Addition_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to add " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the binary addition operator with operands in reversed order.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns>The result of the addition.</returns>
    public static OperatableObject operator +(object c1, OperatableObject c2)
    {

      if (c2.vop_Addition_Rev(c1, out var c3))
        return c3;
      if (c1 is OperatableObject c1o && c1o.vop_Addition(c2, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to add " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the binary subtraction operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns>The result of the subtraction.</returns>
    public static OperatableObject operator -(OperatableObject c1, object c2)
    {

      if (c1.vop_Subtraction(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_Subtraction_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to subtract " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the binary multiplication operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns>The result of the multiplication.</returns>
    public static OperatableObject operator *(OperatableObject c1, object c2)
    {

      if (c1.vop_Multiplication(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_Multiplication_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to multiply " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the binary division operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns>The result of the division.</returns>
    public static OperatableObject operator /(OperatableObject c1, object c2)
    {

      if (c1.vop_Division(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_Division_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to divide " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the binary modulo operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns>The result of the modulo operation.</returns>
    public static OperatableObject operator %(OperatableObject c1, object c2)
    {

      if (c1.vop_Modulo(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_Modulo_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to take modulus of " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the binary AND operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns>The result of the AND operation.</returns>
    public static OperatableObject operator &(OperatableObject c1, object c2)
    {

      if (c1.vop_And(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_And_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply and operator to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the binary OR operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns>The result of the OR operation.</returns>
    public static OperatableObject operator |(OperatableObject c1, object c2)
    {

      if (c1.vop_Or(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_Or_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply or operator to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the binary XOR operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns>The result of the XOR operation.</returns>
    public static OperatableObject operator ^(OperatableObject c1, object c2)
    {

      if (c1.vop_Xor(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_Xor_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply xor operator to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the left-shift operator.
    /// </summary>
    /// <param name="c1">The value to shift.</param>
    /// <param name="c2">The shift count.</param>
    /// <returns>The shifted value.</returns>
    public static OperatableObject operator <<(OperatableObject c1, int c2)
    {

      if (c1.vop_ShiftLeft(c2, out var c3))
        return c3;
      throw new AltaxoOperatorException("Error: Try to shift left " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the right-shift operator.
    /// </summary>
    /// <param name="c1">The value to shift.</param>
    /// <param name="c2">The shift count.</param>
    /// <returns>The shifted value.</returns>
    public static OperatableObject operator >>(OperatableObject c1, int c2)
    {

      if (c1.vop_ShiftRight(c2, out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to shift right " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the equality operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns><see langword="true"/> if both operands are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(OperatableObject c1, object c2)
    {
      if (c1 is null && c2 is null)
        return true;
      if (c1 is null || c2 is null)
        return false;
      if (c1.vop_Equal(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_Equal_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator equal to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the inequality operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns><see langword="true"/> if the operands are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(OperatableObject c1, object c2)
    {

      if (c1 is null && c2 is null)
        return false;
      if (c1 is null || c2 is null)
        return true;
      if (c1.vop_NotEqual(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_NotEqual_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator notequal to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the less-than operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns><see langword="true"/> if <paramref name="c1"/> is less than <paramref name="c2"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator <(OperatableObject c1, object c2)
    {

      if (c1.vop_Lesser(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_Lesser_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator lesser to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the greater-than operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns><see langword="true"/> if <paramref name="c1"/> is greater than <paramref name="c2"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator >(OperatableObject c1, object c2)
    {

      if (c1.vop_Greater(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_Greater_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator greater to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the less-than-or-equal operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns><see langword="true"/> if <paramref name="c1"/> is less than or equal to <paramref name="c2"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator <=(OperatableObject c1, object c2)
    {

      if (c1.vop_LesserOrEqual(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_LesserOrEqual_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator LesserOrEqual to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies the greater-than-or-equal operator.
    /// </summary>
    /// <param name="c1">The left-hand operand.</param>
    /// <param name="c2">The right-hand operand.</param>
    /// <returns><see langword="true"/> if <paramref name="c1"/> is greater than or equal to <paramref name="c2"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator >=(OperatableObject c1, object c2)
    {

      if (c1.vop_GreaterOrEqual(c2, out var c3))
        return c3;
      if (c2 is OperatableObject c2o && c2o.vop_GreaterOrEqual_Rev(c1, out c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator GreaterOrEqual to " + c1.ToString() + " (" + c1.GetType() + ")" + " and " + c2.ToString() + " (" + c2.GetType() + ")");
    }

    /// <summary>
    /// Applies unary plus.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns>The result of applying unary plus.</returns>
    public static OperatableObject operator +(OperatableObject c1)
    {

      if (c1.vop_Plus(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator plus to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    /// <summary>
    /// Applies unary minus.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns>The result of applying unary minus.</returns>
    public static OperatableObject operator -(OperatableObject c1)
    {

      if (c1.vop_Minus(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator minus to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    /// <summary>
    /// Applies logical negation.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns>The result of applying logical negation.</returns>
    public static OperatableObject operator !(OperatableObject c1)
    {

      if (c1.vop_Not(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator not to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    /// <summary>
    /// Applies bitwise complement.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns>The result of applying bitwise complement.</returns>
    public static OperatableObject operator ~(OperatableObject c1)
    {

      if (c1.vop_Complement(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator complement to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    /// <summary>
    /// Applies increment.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns>The incremented value.</returns>
    public static OperatableObject operator ++(OperatableObject c1)
    {

      if (c1.vop_Increment(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator increment to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    /// <summary>
    /// Applies decrement.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns>The decremented value.</returns>
    public static OperatableObject operator --(OperatableObject c1)
    {

      if (c1.vop_Decrement(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator decrement to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    /// <summary>
    /// Evaluates the instance for boolean <c>true</c>.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns><see langword="true"/> if the operand evaluates to <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator true(OperatableObject c1)
    {

      if (c1.vop_True(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator TRUE to " + c1.ToString() + " (" + c1.GetType() + ")");
    }

    /// <summary>
    /// Evaluates the instance for boolean <c>false</c>.
    /// </summary>
    /// <param name="c1">The operand.</param>
    /// <returns><see langword="true"/> if the operand evaluates to <see langword="false"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator false(OperatableObject c1)
    {

      if (c1.vop_False(out var c3))
        return c3;

      throw new AltaxoOperatorException("Error: Try to apply operator FALSE to " + c1.ToString() + " (" + c1.GetType() + ")");
    }
  }
}
