#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Collections
{
  /// <summary>
  /// Extensions to the IEnumerable interface.
  /// </summary>
  public static class EnumerableExtensions
  {
    /// <summary>
    /// Converts a recursive data structure into a flat list. The root element is enumerated before its corresponding child element(s).
    /// </summary>
    /// <typeparam name="T">Type of the elements.</typeparam>
    /// <param name="root">The root element of the recursive data structure.</param>
    /// <param name="recursion">The function that gets the children of an element. If no children of an element exist, the function is allowed to return null.</param>
    /// <returns>Iterator that enumerates the tree structure in preorder.</returns>
    public static IEnumerable<T> FlattenFromRootToLeaves<T>(T root, Func<T, IEnumerable<T>?> recursion)
    {
      return FlattenFromRootToLeaves(new T[] { root }, recursion);
    }

    /// <summary>
    /// Converts a recursive data structure into a flat list. The root element is enumerated before its corresponding child element(s).
    /// </summary>
    /// <typeparam name="T">Type of the elements.</typeparam>
    /// <param name="input">The root elements of the recursive data structure.</param>
    /// <param name="recursion">The function that gets the children of an element. If no children of an element exist, the function is allowed to return null.</param>
    /// <returns>Iterator that enumerates the tree structure in preorder.</returns>
    public static IEnumerable<T> FlattenFromRootToLeaves<T>(this IEnumerable<T> input, Func<T, IEnumerable<T>?> recursion)
    {
      var stack = new Stack<IEnumerator<T>>();
      try
      {
        stack.Push(input.GetEnumerator());
        while (stack.Count > 0)
        {
          while (stack.Peek().MoveNext())
          {
            T element = stack.Peek().Current;
            yield return element;
            if (recursion(element) is { } children)
            {
              stack.Push(children.GetEnumerator());
            }
          }
          stack.Pop().Dispose();
        }
      }
      finally
      {
        while (stack.Count > 0)
        {
          stack.Pop().Dispose();
        }
      }
    }

    /// <summary>
    /// Returns the first value of the enumeration, or, if the enumeration is empty, the other value provided in the arguments.
    /// </summary>
    /// <typeparam name="T">Type of enumerable value.</typeparam>
    /// <param name="org">The enumeration.</param>
    /// <param name="otherValue">The other value.</param>
    /// <returns>First value of the enumeration, or, if the enumeration is empty, the other value provided in the arguments.</returns>
    public static T FirstOr<T>(this IEnumerable<T> org, T otherValue)
    {
      if (org is null)
        throw new ArgumentNullException(nameof(org));

      using (var it = org.GetEnumerator())
      {
        if (it.MoveNext())
          return it.Current;
        else
          return otherValue;
      }
    }

    /// <summary>
    /// Returns the last value of the enumeration, or, if the enumeration is empty, the other value provided in the arguments.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="org">The enumeration.</param>
    /// <param name="otherValue">The other value.</param>
    /// <returns>Last value of the enumeration, or, if the enumeration is empty, the other value provided in the arguments.</returns>
    public static T LastOr<T>(this IEnumerable<T> org, T otherValue)
    {
      if (org is null)
        throw new ArgumentNullException(nameof(org));

      T result = otherValue;

      using (var it = org.GetEnumerator())
      {
        while (it.MoveNext())
          result = it.Current;
      }

      return result;
    }

    /// <summary>
    /// Returns true and the first and last value of the enumeration, or, if the enumeration is empty, returns false.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="org">The enumeration.</param>
    /// <param name="first">if successful, the first value of the enumeration.</param>
    /// <param name="last">If successful, the last value of the enumeration.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public static bool TryGetFirstAndLast<T>(this IEnumerable<T> org, [MaybeNullWhen(false)] out T first, [MaybeNullWhen(false)] out T last)
    {
      if (org is null)
        throw new ArgumentNullException(nameof(org));

      using (var it = org.GetEnumerator())
      {
        if (!it.MoveNext())
        {
          first = last = default(T);
          return false;
        }

        first = it.Current;
        last = it.Current;

        while (it.MoveNext())
          last = it.Current;

        return true;
      }
    }

    /// <summary>
    /// Takes all elements of the enumeration except the last element.
    /// </summary>
    /// <typeparam name="T">Type of the elements of the enumeration</typeparam>
    /// <param name="org">The original enumeration.</param>
    /// <returns>An enumeration that has all elements of the original enumeration, except the last one.</returns>
    /// <exception cref="System.ArgumentNullException">The original enumeration was <c>null</c>.</exception>
    public static IEnumerable<T> TakeAllButLast<T>(this IEnumerable<T> org)
    {
      if (org is null)
        throw new ArgumentNullException("org");

      using (var it = org.GetEnumerator())
      {
        if (it.MoveNext())
        {
          var p = it.Current;
          while (it.MoveNext())
          {
            yield return p;
            p = it.Current;
          }
        }
      }
    }

    /// <summary>
    /// Returns the indices of the elements which fullfill a given condition, given by the element's value.
    /// </summary>
    /// <typeparam name="T">The type of element to consider</typeparam>
    /// <param name="org">The enumeration of elements.</param>
    /// <param name="condition">The condition. The argument is the element's value.</param>
    /// <returns>The indices of all elements in the enumeration for which the condition is fulfilled, i.e. returns true.</returns>
    public static IEnumerable<int> IndicesInt32Where<T>(this IEnumerable<T> org, Func<T, bool> condition)
    {
      if (org is null)
        throw new ArgumentNullException(nameof(org));
      if (condition is null)
        throw new ArgumentNullException(nameof(condition));

      using (var it = org.GetEnumerator())
      {
        int index = 0;
        while (it.MoveNext())
        {
          if (condition(it.Current))
          {
            yield return index;
          }
          ++index;
        }
      }
    }

    /// <summary>
    /// Returns the indices of the elements which fullfill a given condition, given by the element's value and its index.
    /// </summary>
    /// <typeparam name="T">The type of element to consider</typeparam>
    /// <param name="org">The enumeration of elements.</param>
    /// <param name="condition">The condition. The first argument is the element's index, the second argument is the element's value.</param>
    /// <returns>The indices of all elements in the enumeration for which the condition is fulfilled, i.e. returns true.</returns>
    public static IEnumerable<int> IndicesInt32Where<T>(this IEnumerable<T> org, Func<int, T, bool> condition)
    {
      if (org is null)
        throw new ArgumentNullException(nameof(org));
      if (condition is null)
        throw new ArgumentNullException(nameof(condition));

      using (var it = org.GetEnumerator())
      {
        int index = 0;
        while (it.MoveNext())
        {
          if (condition(index, it.Current))
          {
            yield return index;
          }
          ++index;
        }
      }
    }

    /// <summary>
    /// Returns tuples of index and element of all elements in an enumeration which fullfill a given condition, given by the element's value.
    /// </summary>
    /// <typeparam name="T">The type of element to consider</typeparam>
    /// <param name="org">The enumeration of elements.</param>
    /// <param name="condition">The condition. The argument is the element's value</param>
    /// <returns>Tuples of (index and element value) of all elements in the enumeration for which the condition is fulfilled, i.e. returns true.</returns>
    public static IEnumerable<(int Index, T Value)> IndicesInt32AndValuesWhere<T>(this IEnumerable<T> org, Func<T, bool> condition)
    {
      if (org is null)
        throw new ArgumentNullException(nameof(org));
      if (condition is null)
        throw new ArgumentNullException(nameof(condition));

      using (var it = org.GetEnumerator())
      {
        int index = 0;
        while (it.MoveNext())
        {
          var v = it.Current;
          if (condition(v))
          {
            yield return (index, v);
          }
          ++index;
        }
      }
    }

    /// <summary>
    /// Returns tuples of index and element of all elements in an enumeration which fullfill a given condition, given by the element's value and its index.
    /// </summary>
    /// <typeparam name="T">The type of element to consider</typeparam>
    /// <param name="org">The enumeration of elements.</param>
    /// <param name="condition">The condition. First argument is the element's index, second argument is the element's value.</param>
    /// <returns>Tuples of (index and element value) of all elements in the enumeration for which the condition is fulfilled, i.e. returns true.</returns>
    public static IEnumerable<(int Index, T Value)> IndicesInt32AndValuesWhere<T>(this IEnumerable<T> org, Func<int, T, bool> condition)
    {
      if (org is null)
        throw new ArgumentNullException(nameof(org));
      if (condition is null)
        throw new ArgumentNullException(nameof(condition));

      using (var it = org.GetEnumerator())
      {
        int index = 0;
        while (it.MoveNext())
        {
          var v = it.Current;
          if (condition(index, v))
          {
            yield return (index, v);
          }
          ++index;
        }
      }
    }

    /// <summary>
    /// Determines whether the specified enumeration is empty.
    /// </summary>
    /// <typeparam name="T">Type of the elements of the enumeration.</typeparam>
    /// <param name="org">The enumeration to test.</param>
    /// <returns>
    ///   <c>true</c> if the specified org is empty; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">The enumeration to test is <c>null</c>.</exception>
    public static bool IsEmpty<T>(this IEnumerable<T> org)
    {
      if (org is null)
        throw new ArgumentNullException("org");

      bool result;
      using (var it = org.GetEnumerator())
      {
        result = !it.MoveNext();
      }
      return result;
    }

    /// <summary>
    /// Returns either the provided enumeration, or if it is null, an empty enumeration.
    /// </summary>
    /// <typeparam name="T">Type of elements of the enumeration.</typeparam>
    /// <param name="org">The orginal enumeration (may be null)</param>
    /// <returns>Either the original enumeration (if it is not null); otherwise, an empty enumeration.</returns>
    public static IEnumerable<T> ThisOrEmpty<T>(this IEnumerable<T>? org)
    {
      return org ?? Empty<T>();
    }

    /// <summary>
    /// Returns an empty enumeration of T.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <returns>Empty enumeration of T.</returns>
    public static IEnumerable<T> Empty<T>()
    {
      yield break;
    }

    /// <summary>
    /// Presents the argument as IEnumerable with one element.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="element">The element to present.</param>
    /// <returns></returns>
    public static IEnumerable<T> AsEnumerable<T>(T element)
    {
      yield return element;
    }

    /// <summary>
    /// Determines whether the specified enumeration has exactly one element.
    /// </summary>
    /// <typeparam name="T">Type of the elements of the enumeration.</typeparam>
    /// <param name="org">The enumeration to test.</param>
    /// <returns>
    ///   <c>true</c> if the specified enumeration contains excactly one element; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">The enumeration to test is <c>null</c>.</exception>
    public static bool HasSingleElement<T>(this IEnumerable<T> org)
    {
      if (org is null)
        throw new ArgumentNullException(nameof(org));

      using (var it = org.GetEnumerator())
      {
        return it.MoveNext() && !it.MoveNext();

      }
    }

    /// <summary>
    /// Try to get the one and only element of the collection.
    /// </summary>
    /// <typeparam name="T">Type of the elements of the enumeration.</typeparam>
    /// <param name="org">The enumeration.</param>
    /// <param name="singleElement">If the enumeration contains exactly one element, this parameter contains the element. Otherwise, the parameter contains the default for this type.</param>
    /// <returns> <c>true</c> if the specified enumeration contains excactly one element; otherwise, <c>false</c>.</returns>
    /// <exception cref="System.ArgumentNullException">org</exception>
    public static bool TryGetSingleElement<T>(this IEnumerable<T> org, out T singleElement)
    {
      if (org is null)
        throw new ArgumentNullException(nameof(org));

      using (var it = org.GetEnumerator())
      {
        if (!it.MoveNext())
        {
          singleElement = default!;
          return false;
        }
        var ele = it.Current;
        if (it.MoveNext())
        {
          singleElement = default!;
          return false;
        }
        else
        {
          singleElement = ele;
          return true;
        }
      }
    }

    /// <summary>
    /// Gets the element of a IEnumerabe that evaluates by means of a conversion function to the maximal value.
    /// This is different from Select(x => conversion(x)).Max() insofar as it not returns the maximum value, but the original element x which converts to the maximum value.
    /// </summary>
    /// <typeparam name="T">Type of the elements of the enumeration.</typeparam>
    /// <typeparam name="M">Type of the value that is used to compare the elements of the sequence.</typeparam>
    /// <param name="org">The enumeration to consider.</param>
    /// <param name="conversion">Conversion function that converts each element (type: <typeparamref name="T"/>) of the sequence to a value (of type <typeparamref name="M"/> that is comparable.</param>
    /// <returns>This element of the sequence, which by the provided conversion function evaluates to the maximum value.</returns>
    /// <exception cref="System.InvalidOperationException">The provided enumeration is empty. Thus it is not possible to evaluate the maximum.</exception>
    public static T MaxElement<T, M>(this IEnumerable<T> org, Func<T, M> conversion) where M : IComparable<M>
    {
      using (var en = org.GetEnumerator())
      {
        if (!en.MoveNext())
          throw new InvalidOperationException("The provided enumeration is empty. Thus it is not possible to evaluate the maximum.");
        var maxEle = en.Current;
        var max = conversion(maxEle);

        while (en.MoveNext())
        {
          var ot = conversion(en.Current);
          if (max.CompareTo(ot) < 0)
          {
            maxEle = en.Current;
            max = ot;
          }
        }

        return maxEle;
      }
    }

    /// <summary>
    /// Evaluates the minimum of a enumeration of elements, or returns a default value if the series is empty.
    /// </summary>
    /// <typeparam name="T">Type of element</typeparam>
    /// <param name="seq">The enumeration.</param>
    /// <param name="defaultValue">The default value that is returned if the enumeration is empty.</param>
    /// <returns>The minimum of of all elements, or the <paramref name="defaultValue"/> if the series is empty.</returns>
    public static T MinOrDefault<T>(this IEnumerable<T> seq, T defaultValue) where T : IComparable<T>
    {
      using (var en = seq.GetEnumerator())
      {
        if (!en.MoveNext())
          return defaultValue;

        var min = en.Current;
        while (en.MoveNext())
        {
          if (min.CompareTo(en.Current) > 0)
          {
            min = en.Current;
          }
        }
        return min;
      }
    }

    /// <summary>Return the index of the element with the minimum value in an enumerable.
    /// If multiple elements with the same minimal value exist, the index of the first element in the sequence is returned.</summary>
		/// <param name="elements">The input elements.</param>
    /// <param name="transformer">The function that transforms the elements to a numerical value.</param>
		/// <returns>The index of the last element with the minimum value.
    /// Returns -1 if the element enumeration is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMin<T>(this IEnumerable<T> elements, Func<T, double> transformer)
    {
      int index = -1;
      int i = -1;
      double min = double.PositiveInfinity;
      foreach (var element in elements)
      {
        ++i;
        var test = transformer(element);
        if (test < min) // less than ensures that only the first element with that minimum is tagged
        {
          index = i;
          min = test;
        }
      }
      return index;
    }

    /// <summary>Return the index of the element with the maximum value in an enumerable.
    /// If multiple elements with the same minimal value exist, the index of the first element in the sequence is returned.</summary>
		/// <param name="elements">The input elements.</param>
    /// <param name="transformer">The function that transforms the elements to a numerical value.</param>
		/// <returns>The index of the last element with the minimum value.
    /// Returns -1 if the element enumeration is empty or contains only nonvalid elements (NaN).</returns>
		public static int IndexOfMax<T>(this IEnumerable<T> elements, Func<T, double> transformer)
    {
      int index = -1;
      int i = -1;
      double min = double.NegativeInfinity;
      foreach (var element in elements)
      {
        ++i;
        var test = transformer(element);
        if (test > min) // greater than ensures that only the first element with that maximum is tagged
        {
          index = i;
          min = test;
        }
      }
      return index;
    }

    /// <summary>Return the index of the element with the minimum value in an enumerable.
    /// If multiple elements with the same minimal value exist, the index of the first element in the sequence is returned.</summary>
    /// <param name="elements">The input elements.</param>
    /// <param name="transformer">The function that transforms the elements to a numerical value.</param>
    /// <returns>The index of the last element with the minimum value.
    /// Returns the indeces of the minimal value and the maximal value, and -1 if the enumeration is empty or the elements are transformed to only nonvalid numbers (NaN).</returns>
    public static (int IndexOfMin, int IndexOfMax) IndicesOfMinMax<T>(this IEnumerable<T> elements, Func<T, double> transformer)
    {
      int indexMin = -1;
      int indexMax = -1;
      int i = -1;
      double min = double.PositiveInfinity;
      double max = double.NegativeInfinity;
      foreach (var element in elements)
      {
        ++i;
        var test = transformer(element);
        if (test < min) // less than ensures that only the first element with that minimum is tagged
        {
          indexMin = i;
          min = test;
        }
        if (test > max)
        {
          indexMax = i;
          max = test;
        }
      }
      return (indexMin, indexMax);
    }

    /// <summary>
    /// Evaluates the maximum of a enumeration of elements, or returns a default value if the series is empty.
    /// </summary>
    /// <typeparam name="T">Type of element</typeparam>
    /// <param name="seq">The enumeration.</param>
    /// <param name="defaultValue">The default value that is returned if the enumeration is empty.</param>
    /// <returns>The maximum of of all elements, or the <paramref name="defaultValue"/> if the series is empty.</returns>
    public static T MaxOrDefault<T>(this IEnumerable<T> seq, T defaultValue) where T : IComparable<T>
    {
      using (var en = seq.GetEnumerator())
      {
        if (!en.MoveNext())
          return defaultValue;

        var max = en.Current;
        while (en.MoveNext())
        {
          if (max.CompareTo(en.Current) < 0)
          {
            max = en.Current;
          }
        }
        return max;
      }
    }

    /// <summary>
    /// Evaluates the maximum of a enumeration of elements, or returns a default value if the series is empty.
    /// </summary>
    /// <typeparam name="T">Type of element</typeparam>
    /// <typeparam name="M">Type of comparison value that results from the element.</typeparam>
    /// <param name="seq">The enumeration of elements.</param>
    /// <param name="conversion">Conversion function which converts the type <typeparamref name="T"/> of the original sequence into a type <typeparamref name="M"/> which is used to determine the maximum value.</param>
    /// <param name="defaultValue">The default value that is returned if the enumeration is empty.</param>
    /// <returns>The maximum of of all comparison values of the elements, or the <paramref name="defaultValue"/> if the series is empty.</returns>
    public static M MaxOrDefault<T, M>(this IEnumerable<T> seq, Func<T, M> conversion, M defaultValue) where M : IComparable<M>
    {
      using (var en = seq.GetEnumerator())
      {
        if (!en.MoveNext())
          return defaultValue;

        var max = conversion(en.Current);
        while (en.MoveNext())
        {
          var curVal = conversion(en.Current);
          if (max.CompareTo(curVal) < 0)
          {
            max = curVal;
          }
        }
        return max;
      }
    }

    /// <summary>
    /// Executes an action for each element of the sequence.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    /// <param name="seq">The element sequence.</param>
    /// <param name="action">The action to execute for each element.</param>
    public static void ForEachDo<T>(this IEnumerable<T> seq, Action<T> action)
    {
      foreach (var element in seq)
        action(element);
    }

    /// <summary>
    /// Takes all elements of a list, starting from index <paramref name="upperIndexInclusive"/> down to the index <paramref name="lowerIndexInclusive"/>.
    /// </summary>
    /// <typeparam name="T">Type of list elements</typeparam>
    /// <param name="list">The list of elements.</param>
    /// <param name="upperIndexInclusive">The upper index (inclusive).</param>
    /// <param name="lowerIndexInclusive">The lower index (inclusive).</param>
    /// <returns></returns>
    public static IEnumerable<T> TakeFromUpperIndexInclusiveDownToLowerIndexInclusive<T>(this IList<T> list, int upperIndexInclusive, int lowerIndexInclusive)
    {
      if (list is null)
        throw new ArgumentNullException("list");
      if (!(upperIndexInclusive >= lowerIndexInclusive))
        throw new ArgumentException("upperIndexInclusive should be >= lowerIndexInclusive");

      for (int i = upperIndexInclusive; i >= lowerIndexInclusive; --i)
        yield return list[i];
    }

    /// <summary>
    /// Takes all elements of a list, starting from index <paramref name="upperIndexExclusive"/> - 1 down to the index <paramref name="lowerIndexInclusive"/>.
    /// </summary>
    /// <typeparam name="T">Type of list elements</typeparam>
    /// <param name="list">The list of elements.</param>
    /// <param name="upperIndexExclusive">The upper index (exclusive).</param>
    /// <param name="lowerIndexInclusive">The lower index (inclusive).</param>
    /// <returns>All elements of a list, starting from index <paramref name="upperIndexExclusive"/> - 1 down to the index <paramref name="lowerIndexInclusive"/>.</returns>
    public static IEnumerable<T> TakeFromUpperIndexExclusiveDownToLowerIndexInclusive<T>(this IList<T> list, int upperIndexExclusive, int lowerIndexInclusive)
    {
      if (list is null)
        throw new ArgumentNullException(nameof(list));
      if (lowerIndexInclusive < 0)
        throw new ArgumentOutOfRangeException(nameof(lowerIndexInclusive) + " should be >= 0");
      if (!(upperIndexExclusive >= lowerIndexInclusive))
        throw new ArgumentException(nameof(upperIndexExclusive) + " should be >= " + nameof(lowerIndexInclusive));

      for (int i = upperIndexExclusive - 1; i >= lowerIndexInclusive; --i)
        yield return list[i];
    }

    /// <summary>
    /// Disposes all elements of the enumeration.
    /// </summary>
    /// <param name="sequence">The sequence of elements to dispose.</param>
    public static void DisposeElements(this IEnumerable<IDisposable?>? sequence)
    {
      if (sequence is not null)
      {
        foreach (var ele in sequence)
        {
          ele?.Dispose();
        }
      }
    }

    /// <summary>
    /// Takes a join of two sequences, but only takes into account those pair, which fulfill a given condition.
    /// </summary>
    /// <typeparam name="T1">The element type of the 1st sequence.</typeparam>
    /// <typeparam name="T2">The element type of the 2nd sequence.</typeparam>
    /// <param name="seq1">The first sequence.</param>
    /// <param name="seq2">The second sequence.</param>
    /// <param name="Condition">A condition that is evaluated for each pair of (T1, T2). Only if the return value is true, the pair (T1, T2) is put into the output sequence.</param>
    /// <returns>The resulting sequence with elements of type (T1, T2).</returns>
    public static IEnumerable<(T1, T2)> JoinConditional<T1, T2>(this IEnumerable<T1> seq1, IEnumerable<T2> seq2, Func<T1, T2, bool> Condition)
    {
      foreach (T1 t1 in seq1)
        foreach (T2 t2 in seq2)
          if (Condition(t1, t2))
            yield return (t1, t2);
    }

    /// <summary>
    /// Takes a join of two sequences, but only takes into account those pair, which fulfill a given condition.
    /// </summary>
    /// <typeparam name="T1">The element type of the 1st sequence.</typeparam>
    /// <typeparam name="T2">The element type of the 2nd sequence.</typeparam>
    /// <typeparam name="TResult">The element type of the resulting sequence.</typeparam>
    /// <param name="seq1">The first sequence.</param>
    /// <param name="seq2">The second sequence.</param>
    /// <param name="Condition">A condition that is evaluated for each pair of (T1, T2). Only if the return value is true, the pair (T1, T2) is put into the output sequence.</param>
    /// <param name="CreateResult">A function that takes a pair of T1 and T2 as parameters, and returns the result.</param>
    /// <returns>The resulting sequence of with elements of type TResult.</returns>
    public static IEnumerable<TResult> JoinConditional<T1, T2, TResult>(this IEnumerable<T1> seq1, IEnumerable<T2> seq2, Func<T1, T2, bool> Condition, Func<T1, T2, TResult> CreateResult)
    {
      foreach (T1 t1 in seq1)
        foreach (T2 t2 in seq2)
          if (Condition(t1, t2))
            yield return CreateResult(t1, t2);
    }

    /// <summary>
    /// Determines whether two enumerations are structural equivalent. They are structurally equivalent if i) both enumerations are null, ii) both enumerations are empty,
    /// or c) both enumerations have the same number of elements and contain the same elements in the same order.
    /// Please not that if one enumeration is null and the other is empty, they are not considered equivalent.
    /// </summary>
    /// <typeparam name="T">Type of the elements in the enumeration</typeparam>
    /// <param name="e1">The first enumeration.</param>
    /// <param name="e2">The second enumeration.</param>
    /// <returns><c>true</c> if the two enumerations are structural equivalent; otherwise, <c>false</c>.
    /// </returns>
    public static bool AreStructurallyEqual<T>(IEnumerable<T> e1, IEnumerable<T> e2) where T : IEquatable<T>
    {
      return AreStructurallyEqual(e1, e2, EqualityComparer<T>.Default);
    }

    /// <summary>
    /// Determines whether two enumerations are structural equivalent. They are structural equivalent if i) both enumerations are null, ii) both enumerations are empty,
    /// or c) both enumerations have the same number of elements and contain the same elements in the same order.
    /// Please not that if one enumeration is null and the other is empty, they are not considered equivalent.
    /// </summary>
    /// <typeparam name="T">Type of the elements in the enumeration</typeparam>
    /// <param name="e1">The first enumeration.</param>
    /// <param name="e2">The second enumeration.</param>
    /// <param name="equalityComparer">The equality comparer to compare the elements of the enumeration.</param>
    /// <returns><c>true</c> if the two enumerations are structural equivalent; otherwise, <c>false</c>.
    /// </returns>
    public static bool AreStructurallyEqual<T>(IEnumerable<T> e1, IEnumerable<T> e2, IEqualityComparer<T> equalityComparer)
    {
      if (e1 is null && e2 is null)
        return true;
      if ((e1 is null) || (e2 is null))
        return false;

      // both e1 and e2 are != null
      if (e1 is System.Collections.ICollection c1 && e2 is System.Collections.ICollection c2 && c1.Count != c2.Count)
        return false;

      using (var it1 = e1.GetEnumerator())
      {
        using (var it2 = e2.GetEnumerator())
        {
          bool b1, b2;
          b1 = it1.MoveNext();
          b2 = it2.MoveNext();
          if (b1 ^ b2)
            return false; // one of the enumerations is empty
          if (!b1 && !b2)
            return true; // both enumerations empty

          do
          {
            if (!equalityComparer.Equals(it1.Current, it2.Current))
              return false; // one element is different from the other element

            b1 = it1.MoveNext();
            b2 = it2.MoveNext();
          } while (b1 && b2);

          return !b1 && !b2; // both enumerations must be at the end in order for both enumerations to be equal
        }
      }
    }
    /// <summary>
    /// Enumerates the range starting from <paramref name="start"/> with count elements, i.e. yields start, start+1, ..., start + count - 1, and returns the elements as double.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="count">The count.</param>
    /// <returns>Elements from start .. start + count -1 as double values.</returns>
    public static IEnumerable<double> RangeDouble(double start, int count)
    {
      for (int i = 0; i < count; ++i)
      {
        yield return start + i;
      }
    }

    /// <summary>
    /// Gets an enumeration where the elements are given by start + i * step, i=[0, count-1].
    /// </summary>
    /// <param name="start">The start value.</param>
    /// <param name="step">The step value.</param>
    /// <param name="count">The number of values in the enumeration.</param>
    /// <returns></returns>
    public static IEnumerable<double> EquallySpacedByStartStepCount(double start, double step, int count)
    {
      for (int i = 0; i < count; ++i)
      {
        yield return start + i * step;
      }
    }

    /// <summary>
    /// Gets the differences x[i+1] - x[i], for i = 0 .. x.Count-2.
    /// </summary>
    /// <param name="x">The x enumeration.</param>
    /// <returns>The differences x[i+1] - x[i], for i = 0 .. x.Count-2.</returns>
    public static IEnumerable<double> GetDifferences(this IEnumerable<double> x)
    {
      double? xprev = null;
      foreach (var xnext in x)
      {
        if (xprev.HasValue)
        {
          yield return xnext - xprev.Value;
        }
        xprev = xnext;
      }
    }

    /// <summary>
    /// Gets the differences x[i+1] - x[i], for i = 0 .. x.Count-2.
    /// </summary>
    /// <param name="x">The x enumeration.</param>
    /// <returns>The differences x[i+1] - x[i], for i = 0 .. x.Count-2.</returns>
    public static IEnumerable<double> GetDifferences(this ReadOnlyMemory<double> x)
    {
      for (int i = 1; i < x.Length; ++i)
      {
        yield return x.Span[i] - x.Span[i - 1];
      }
    }

    /// <summary>
    /// Gets the same element if all elements of the enumeration (after a transformation) are the same. If the elements in the enumeration are different, the return value is null or default.
    /// </summary>
    /// <typeparam name="T">Type of the enumeration.</typeparam>
    /// <typeparam name="S">Type of the comparable value. Type T is transformed to type S before comparison.</typeparam>
    /// <param name="seq">The enumeration.</param>
    /// <param name="transform">The transformation function that converts the elements of the enumeration to a comparable value.</param>
    /// <returns>The element if all elements of the enumeration are the same. If the elements in the enumeration are different, the return value is null or default.</returns>
    [return: MaybeNull]
    public static S GetSameOrDefault<T, S>(this IEnumerable<T> seq, Func<T, S> transform)
    {
      var it = seq.GetEnumerator();
      if (it.MoveNext())
      {
        var first = transform(it.Current);
        if (first is null)
        {
          while (it.MoveNext())
          {
            if (transform(it.Current) is not null)
              return default;
          }
          return first;
        }
        else
        {
          while (it.MoveNext())
          {
            if (!first.Equals(transform(it.Current)))
              return default;
          }
          return first;
        }
      }
      else
      {
        return default;
      }
    }
  }
}
