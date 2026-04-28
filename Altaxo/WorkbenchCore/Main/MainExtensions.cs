// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Main.Services;

namespace Altaxo.Main
{
  /// <summary>
  /// Some extension methods.
  /// </summary>
  public static class MainExtensions
  {
    #region RaiseEvent

    /// <summary>
    /// Raises the event.
    /// Does nothing if eventHandler is null.
    /// Because the event handler is passed as parameter, it is only fetched from the event field one time.
    /// This makes
    /// <code>MyEvent.RaiseEvent(x,y);</code>
    /// thread-safe
    /// whereas
    /// <code>if (MyEvent != null) MyEvent(x,y);</code>
    /// would not be safe.
    /// </summary>
    /// <param name="eventHandler">The event handler to raise.</param>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    /// <remarks>Using this method is only thread-safe under the Microsoft .NET memory model,
    /// not under the less strict memory model in the CLI specification.</remarks>
    [Obsolete("Use 'event EventHandler MyEvent = delegate{};' instead")]
    public static void RaiseEvent(this EventHandler eventHandler, object sender, EventArgs e)
    {
      if (eventHandler is not null)
      {
        eventHandler(sender, e);
      }
    }

    /// <summary>
    /// Raises the event.
    /// Does nothing if eventHandler is null.
    /// Because the event handler is passed as parameter, it is only fetched from the event field one time.
    /// This makes
    /// <code>MyEvent.RaiseEvent(x,y);</code>
    /// thread-safe
    /// whereas
    /// <code>if (MyEvent != null) MyEvent(x,y);</code>
    /// would not be safe.
    /// </summary>
    /// <typeparam name="T">The event argument type.</typeparam>
    /// <param name="eventHandler">The event handler to raise.</param>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    [Obsolete("Use 'event EventHandler MyEvent = delegate{};' instead")]
    public static void RaiseEvent<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
    {
      if (eventHandler is not null)
      {
        eventHandler(sender, e);
      }
    }

    #endregion RaiseEvent

    #region Task Extensions

    /// <summary>
    /// If the task throws an exception, notifies the message service.
    /// Call this method on asynchronous tasks if you do not care about the result, but do not want
    /// unhandled exceptions to go unnoticed.
    /// </summary>
    /// <param name="task">The task to observe.</param>
    public static void FireAndForget(this Task task)
    {
      task.ContinueWith(
        t =>
        {
          if (t.Exception is not null)
          {
            if (t.Exception.InnerExceptions.Count == 1)
              MessageService.ShowException(t.Exception.InnerExceptions[0]);
            else
              MessageService.ShowException(t.Exception);
          }
        }, TaskContinuationOptions.OnlyOnFaulted);
    }

    #endregion Task Extensions

    #region CoerceValue

    /// <summary>
    /// Forces the value to stay between minimum and maximum.
    /// </summary>
    /// <param name="value">The value to constrain.</param>
    /// <param name="minimum">The lower bound.</param>
    /// <param name="maximum">The upper bound.</param>
    /// <returns>minimum, if value is less than minimum.
    /// Maximum, if value is greater than maximum.
    /// Otherwise, value.</returns>
    public static double CoerceValue(this double value, double minimum, double maximum)
    {
      return Math.Max(Math.Min(value, maximum), minimum);
    }

    /// <summary>
    /// Forces the value to stay between minimum and maximum.
    /// </summary>
    /// <param name="value">The value to constrain.</param>
    /// <param name="minimum">The lower bound.</param>
    /// <param name="maximum">The upper bound.</param>
    /// <returns>minimum, if value is less than minimum.
    /// Maximum, if value is greater than maximum.
    /// Otherwise, value.</returns>
    public static int CoerceValue(this int value, int minimum, int maximum)
    {
      return Math.Max(Math.Min(value, maximum), minimum);
    }

    #endregion CoerceValue

    #region Collections

    /// <summary>
    /// Obsolete. Please use a regular foreach loop instead. ForEach() is executed for its side-effects, and side-effects mix poorly with a functional programming style.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="input">The sequence to enumerate.</param>
    /// <param name="action">The action to run for each element.</param>
    //[Obsolete("Please use a regular foreach loop instead. ForEach() is executed for its side-effects, and side-effects mix poorly with a functional programming style.")]
    public static void ForEach<T>(this IEnumerable<T> input, Action<T> action)
    {
      if (input is null)
        throw new ArgumentNullException("input");
      foreach (T element in input)
      {
        action(element);
      }
    }

    /// <summary>
    /// Adds all <paramref name="elements"/> to <paramref name="list"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="list">The collection to add to.</param>
    /// <param name="elements">The elements to add.</param>
    public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> elements)
    {
      foreach (T o in elements)
        list.Add(o);
    }

    /// <summary>
    /// Wraps the list in a read-only collection.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="arr">The list to wrap.</param>
    /// <returns>A read-only view of <paramref name="arr"/>.</returns>
    public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> arr)
    {
      return new ReadOnlyCollection<T>(arr);
    }

    /// <summary>
    /// Gets the value for the specified key or the default value when the key is absent.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="V">The value type.</typeparam>
    /// <param name="dict">The dictionary to query.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>The stored value, or the default value of <typeparamref name="V"/> when the key is missing.</returns>
    [return: MaybeNull]
    public static V GetOrDefault<K, V>(this IReadOnlyDictionary<K, V> dict, K key) where K : notnull
    {
      dict.TryGetValue(key, out var ret);
      return ret;
    }

    /// <summary>
    /// Searches a sorted list
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="K">The key type.</typeparam>
    /// <param name="list">The list to search in</param>
    /// <param name="key">The key to search for</param>
    /// <param name="keySelector">Function that maps list items to their sort key</param>
    /// <param name="keyComparer">Comparer used for the sort</param>
    /// <returns>Returns the index of the element with the specified key.
    /// If no such element is found, this method returns a negative number that is the bitwise complement of the
    /// index where the element could be inserted while maintaining the order.</returns>
    public static int BinarySearch<T, K>(this IList<T> list, K key, Func<T, K> keySelector, IComparer<K>? keyComparer = null)
    {
      return BinarySearch(list, 0, list.Count, key, keySelector, keyComparer);
    }

    /// <summary>
    /// Searches a sorted list
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="K">The key type.</typeparam>
    /// <param name="list">The list to search in</param>
    /// <param name="index">Starting index of the range to search</param>
    /// <param name="length">Length of the range to search</param>
    /// <param name="key">The key to search for</param>
    /// <param name="keySelector">Function that maps list items to their sort key</param>
    /// <param name="keyComparer">Comparer used for the sort</param>
    /// <returns>Returns the index of the element with the specified key.
    /// If no such element is found in the specified range, this method returns a negative number that is the bitwise complement of the
    /// index where the element could be inserted while maintaining the order.</returns>
    public static int BinarySearch<T, K>(this IList<T> list, int index, int length, K key, Func<T, K> keySelector, IComparer<K>? keyComparer = null)
    {
      if (keyComparer is null)
        keyComparer = Comparer<K>.Default;
      int low = index;
      int high = index + length - 1;
      while (low <= high)
      {
        int mid = low + (high - low >> 1);
        int r = keyComparer.Compare(keySelector(list[mid]), key);
        if (r == 0)
        {
          return mid;
        }
        else if (r < 0)
        {
          low = mid + 1;
        }
        else
        {
          high = mid - 1;
        }
      }
      return ~low;
    }

    /// <summary>
    /// Inserts an item into a sorted list.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="list">The list to insert into.</param>
    /// <param name="item">The item to insert.</param>
    /// <param name="comparer">The comparer used to keep the list sorted.</param>
    public static void OrderedInsert<T>(this IList<T> list, T item, IComparer<T> comparer)
    {
      int pos = BinarySearch(list, item, x => x, comparer);
      if (pos < 0)
        pos = ~pos;
      list.Insert(pos, item);
    }

    /// <summary>
    /// Sorts the enumerable using the given comparer.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="input">The sequence to sort.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <returns>The sorted sequence.</returns>
    public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> input, IComparer<T> comparer)
    {
      return Enumerable.OrderBy(input, e => e, comparer);
    }

    /// <summary>
    /// Converts a recursive data structure into a flat list.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="input">The root elements of the recursive data structure.</param>
    /// <param name="recursion">The function that gets the children of an element.</param>
    /// <returns>Iterator that enumerates the tree structure in preorder.</returns>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<T> input, Func<T, IEnumerable<T>> recursion)
    {
      throw new NotImplementedException();
      //return ICSharpCode.NRefactory.Utils.TreeTraversal.PreOrder(input, recursion);
    }

    /// <summary>
    /// Creates an array containing a part of the array (similar to string.Substring).
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="array">The source array.</param>
    /// <param name="startIndex">The starting index.</param>
    /// <returns>An array containing the slice from <paramref name="startIndex"/> to the end of the source array.</returns>
    public static T[] Splice<T>(this T[] array, int startIndex)
    {
      if (array is null)
        throw new ArgumentNullException("array");
      return Splice(array, startIndex, array.Length - startIndex);
    }

    /// <summary>
    /// Creates an array containing a part of the array (similar to string.Substring).
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="array">The source array.</param>
    /// <param name="startIndex">The starting index.</param>
    /// <param name="length">The number of elements to copy.</param>
    /// <returns>An array containing the requested slice of the source array.</returns>
    public static T[] Splice<T>(this T[] array, int startIndex, int length)
    {
      if (array is null)
        throw new ArgumentNullException("array");
      if (startIndex < 0 || startIndex > array.Length)
        throw new ArgumentOutOfRangeException("startIndex", startIndex, "Value must be between 0 and " + array.Length);
      if (length < 0 || length > array.Length - startIndex)
        throw new ArgumentOutOfRangeException("length", length, "Value must be between 0 and " + (array.Length - startIndex));
      var result = new T[length];
      Array.Copy(array, startIndex, result, 0, length);
      return result;
    }

    /// <summary>
    /// Returns distinct elements by comparing a projected key.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="K">The key type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>A sequence containing only the first element for each key.</returns>
    public static IEnumerable<T> DistinctBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector) where K : IEquatable<K>
    {
      // Don't just use .Distinct(KeyComparer.Create(keySelector)) - that would evaluate the keySelector multiple times.
      var hashSet = new HashSet<K>();
      foreach (var element in source)
      {
        if (hashSet.Add(keySelector(element)))
        {
          yield return element;
        }
      }
    }

    /// <summary>
    /// Returns the minimum element.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="K">The key type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>The minimum element.</returns>
    /// <exception cref="InvalidOperationException">The input sequence is empty</exception>
    public static T MinBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector) where K : IComparable<K>
    {
      return source.MinBy(keySelector, Comparer<K>.Default);
    }

    /// <summary>
    /// Returns the minimum element.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="K">The key type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="keyComparer">The comparer used to compare keys.</param>
    /// <returns>The minimum element.</returns>
    /// <exception cref="InvalidOperationException">The input sequence is empty</exception>
    public static T MinBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, IComparer<K> keyComparer)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (keySelector is null)
        throw new ArgumentNullException(nameof(keySelector));
      if (keyComparer is null)
        keyComparer = Comparer<K>.Default;
      using (var enumerator = source.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          throw new InvalidOperationException("Sequence contains no elements");
        T minElement = enumerator.Current;
        K minKey = keySelector(minElement);
        while (enumerator.MoveNext())
        {
          T element = enumerator.Current;
          K key = keySelector(element);
          if (keyComparer.Compare(key, minKey) < 0)
          {
            minElement = element;
            minKey = key;
          }
        }
        return minElement;
      }
    }

    /// <summary>
    /// Returns the maximum element.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="K">The key type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>The maximum element.</returns>
    /// <exception cref="InvalidOperationException">The input sequence is empty</exception>
    public static T MaxBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector) where K : IComparable<K>
    {
      return source.MaxBy(keySelector, Comparer<K>.Default);
    }

    /// <summary>
    /// Returns the maximum element.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="K">The key type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="keyComparer">The comparer used to compare keys.</param>
    /// <returns>The maximum element.</returns>
    /// <exception cref="InvalidOperationException">The input sequence is empty</exception>
    public static T MaxBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, IComparer<K> keyComparer)
    {
      if (source is null)
        throw new ArgumentNullException("source");
      if (keySelector is null)
        throw new ArgumentNullException("selector");
      if (keyComparer is null)
        keyComparer = Comparer<K>.Default;
      using (var enumerator = source.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          throw new InvalidOperationException("Sequence contains no elements");
        T maxElement = enumerator.Current;
        K maxKey = keySelector(maxElement);
        while (enumerator.MoveNext())
        {
          T element = enumerator.Current;
          K key = keySelector(element);
          if (keyComparer.Compare(key, maxKey) > 0)
          {
            maxElement = element;
            maxKey = key;
          }
        }
        return maxElement;
      }
    }

    /// <summary>
    /// Returns the index of the first element for which <paramref name="predicate"/> returns true.
    /// If none of the items in the list fits the <paramref name="predicate"/>, -1 is returned.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="list">The list to search.</param>
    /// <param name="predicate">The predicate used to determine a match.</param>
    /// <returns>The index of the first matching element, or -1 if no match is found.</returns>
    public static int FindIndex<T>(this IList<T> list, Func<T, bool> predicate)
    {
      for (int i = 0; i < list.Count; i++)
      {
        if (predicate(list[i]))
          return i;
      }

      return -1;
    }

    /// <summary>
    /// Returns the index of the first element for which <paramref name="predicate"/> returns true.
    /// If none of the items in the list fits the <paramref name="predicate"/>, -1 is returned.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="list">The list to search.</param>
    /// <param name="predicate">The predicate used to determine a match.</param>
    /// <returns>The index of the first matching element, or -1 if no match is found.</returns>
    public static int FindIndex<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
    {
      for (int i = 0; i < list.Count; i++)
      {
        if (predicate(list[i]))
          return i;
      }

      return -1;
    }

    /// <summary>
    /// Adds item to the list if the item is not null.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="list">The list to modify.</param>
    /// <param name="itemToAdd">The item to add if it is not null.</param>
    public static void AddIfNotNull<T>(this IList<T> list, T itemToAdd) where T : class
    {
      if (itemToAdd is not null)
        list.Add(itemToAdd);
    }

    /// <summary>
    /// Removes all items that match the specified predicate.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="list">The list to modify.</param>
    /// <param name="condition">The predicate used to decide which items to remove.</param>
    public static void RemoveAll<T>(this IList<T> list, Predicate<T> condition)
    {
      if (list is null)
        throw new ArgumentNullException("list");
      int i = 0;
      while (i < list.Count)
      {
        if (condition(list[i]))
          list.RemoveAt(i);
        else
          i++;
      }
    }

    #endregion Collections

    #region String extensions

    /// <summary>
    /// Removes <paramref name="stringToRemove"/> from the start of this string.
    /// Throws ArgumentException if this string does not start with <paramref name="stringToRemove"/>.
    /// </summary>
    /// <param name="s">String from which we want to remove another string at the start.</param>
    /// <param name="stringToRemove">The string to remove.</param>
    /// <returns>The string <paramref name="s"/> without string <paramref name="stringToRemove"/> at the start.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="s"/> does not start with <paramref name="stringToRemove"/>.</exception>
    [return: MaybeNull]
    [return: NotNullIfNotNull("s")]
    public static string RemoveFromStart(this string? s, string stringToRemove)
    {
      if (s is null)
        return null;
      if (string.IsNullOrEmpty(stringToRemove))
        return s;
      if (!s.StartsWith(stringToRemove))
        throw new ArgumentException(string.Format("{0} does not start with {1}", s, stringToRemove));
      return s.Substring(stringToRemove.Length);
    }

    /// <summary>
    /// Removes <paramref name="stringToRemove"/> from the end of this string.
    /// Throws ArgumentException if this string does not end with <paramref name="stringToRemove"/>.
    /// </summary>
    /// <param name="s">String from which we want to remove another string at the end.</param>
    /// <param name="stringToRemove">The string to remove.</param>
    /// <returns>The string <paramref name="s"/> without string <paramref name="stringToRemove"/> at the end.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="s"/> does not end with <paramref name="stringToRemove"/>.</exception>
    [return: MaybeNull]
    [return: NotNullIfNotNull("s")]
    public static string RemoveFromEnd(this string? s, string stringToRemove)
    {
      if (s is null)
        return null;
      if (string.IsNullOrEmpty(stringToRemove))
        return s;
      if (!s.EndsWith(stringToRemove))
        throw new ArgumentException(string.Format("{0} does not end with {1}", s, stringToRemove));
      return s.Substring(0, s.Length - stringToRemove.Length);
    }

    /// <summary>
    /// Trims the string from the first occurence of <paramref name="cutoffStart" /> to the end, including <paramref name="cutoffStart" />.
    /// If the string does not contain <paramref name="cutoffStart" />, just returns the original string.
    /// </summary>
    /// <param name="s">The source string.</param>
    /// <param name="cutoffStart">The marker at which to cut off the string.</param>
    /// <returns>The string up to the first occurrence of <paramref name="cutoffStart"/>, or the original string if the marker is not found.</returns>
    [return: MaybeNull]
    [return: NotNullIfNotNull("s")]
    public static string CutoffEnd(this string? s, string cutoffStart)
    {
      if (s is null)
        return null;
      int pos = s.IndexOf(cutoffStart);
      if (pos != -1)
      {
        return s.Substring(0, pos);
      }
      else
      {
        return s;
      }
    }

    /// <summary>
    /// Takes at most <paramref name="length" /> first characters from string <paramref name="s"/>.
    /// String can be null.
    /// </summary>
    /// <param name="s">The string to take from.</param>
    /// <param name="length">The number of characters taken.</param>
    /// <returns>The first <paramref name="length"/> characters of <paramref name="s"/>.</returns>
    public static string TakeStart(this string s, int length)
    {
      if (string.IsNullOrEmpty(s) || length >= s.Length)
        return s;
      return s.Substring(0, length);
    }

    /// <summary>
    /// Takes at most <paramref name="length" /> first characters from string, and appends '...' if string is longer.
    /// String can be null.
    /// </summary>
    /// <param name="s">The string to take from.</param>
    /// <param name="length">The number of characters taken at most.</param>
    /// <returns>The first <paramref name="length"/> characters of <paramref name="s"/> at most, and an ellipsis if the string was longer.</returns>
    public static string TakeStartEllipsis(this string s, int length)
    {
      if (string.IsNullOrEmpty(s) || length >= s.Length)
        return s;
      return s.Substring(0, length) + "...";
    }

    /// <summary>
    /// Removes any character given in the array from the given string.
    /// </summary>
    /// <param name="s">The source string.</param>
    /// <param name="chars">The characters to remove.</param>
    /// <returns>The string with the specified characters removed.</returns>
    public static string RemoveAny(this string s, params char[] chars)
    {
      if (string.IsNullOrEmpty(s))
        return s;
      var b = new StringBuilder(s);
      foreach (char ch in chars)
      {
        b.Replace(ch.ToString(), "");
      }
      return b.ToString();
    }

    /// <summary>
    /// Replaces all occurrences of a pattern using the specified string comparison.
    /// </summary>
    /// <param name="original">The source string.</param>
    /// <param name="pattern">The pattern to replace.</param>
    /// <param name="replacement">The replacement text.</param>
    /// <param name="comparisonType">The comparison type to use.</param>
    /// <returns>The resulting string.</returns>
    public static string Replace(this string original, string pattern, string replacement, StringComparison comparisonType)
    {
      if (original is null)
        throw new ArgumentNullException("original");
      if (pattern is null)
        throw new ArgumentNullException("pattern");
      if (pattern.Length == 0)
        throw new ArgumentException("String cannot be of zero length.", "pattern");
      if (comparisonType != StringComparison.Ordinal && comparisonType != StringComparison.OrdinalIgnoreCase)
        throw new NotSupportedException("Currently only ordinal comparisons are implemented.");

      var result = new StringBuilder(original.Length);
      int currentPos = 0;
      int nextMatch = original.IndexOf(pattern, comparisonType);
      while (nextMatch >= 0)
      {
        result.Append(original, currentPos, nextMatch - currentPos);
        // The following line restricts this method to ordinal comparisons:
        // for non-ordinal comparisons, the match length might be different than the pattern length.
        currentPos = nextMatch + pattern.Length;
        result.Append(replacement);

        nextMatch = original.IndexOf(pattern, currentPos, comparisonType);
      }

      result.Append(original, currentPos, original.Length - currentPos);
      return result.ToString();
    }

    /// <summary>
    /// Encodes the specified text and prefixes it with the encoding preamble when present.
    /// </summary>
    /// <param name="encoding">The encoding to use.</param>
    /// <param name="text">The text to encode.</param>
    /// <returns>The encoded bytes, including the preamble when available.</returns>
    public static byte[] GetBytesWithPreamble(this Encoding encoding, string text)
    {
      byte[] encodedText = encoding.GetBytes(text);
      byte[] bom = encoding.GetPreamble();
      if (bom is not null && bom.Length > 0)
      {
        byte[] result = new byte[bom.Length + encodedText.Length];
        bom.CopyTo(result, 0);
        encodedText.CopyTo(result, bom.Length);
        return result;
      }
      else
      {
        return encodedText;
      }
    }

    /// <summary>
    /// Finds the earliest occurrence of any candidate string.
    /// </summary>
    /// <param name="haystack">The string to search.</param>
    /// <param name="needles">The candidate strings.</param>
    /// <param name="startIndex">The start index for the search.</param>
    /// <param name="matchLength">Receives the length of the matched string.</param>
    /// <returns>The index of the first match, or <c>-1</c> when no match exists.</returns>
    public static int IndexOfAny(this string haystack, IEnumerable<string> needles, int startIndex, out int matchLength)
    {
      if (haystack is null)
        throw new ArgumentNullException("haystack");
      if (needles is null)
        throw new ArgumentNullException("needles");
      int index = -1;
      matchLength = 0;
      foreach (var needle in needles)
      {
        int i = haystack.IndexOf(needle, startIndex, StringComparison.Ordinal);
        if (i != -1 && (index == -1 || index > i))
        {
          index = i;
          matchLength = needle.Length;
        }
      }
      return index;
    }

    /// <summary>
    /// Determines whether the string contains any of the candidate strings.
    /// </summary>
    /// <param name="haystack">The string to search.</param>
    /// <param name="needles">The candidate strings.</param>
    /// <param name="startIndex">The start index for the search.</param>
    /// <param name="match">Receives the matching string.</param>
    /// <returns><see langword="true"/> if a match was found; otherwise, <see langword="false"/>.</returns>
    public static bool ContainsAny(this string haystack, IEnumerable<string> needles, int startIndex, [MaybeNullWhen(false)] out string match)
    {
      if (haystack is null)
        throw new ArgumentNullException("haystack");
      if (needles is null)
        throw new ArgumentNullException("needles");
      int index = -1;
      match = null;
      foreach (var needle in needles)
      {
        int i = haystack.IndexOf(needle, startIndex, StringComparison.Ordinal);
        if (i != -1 && (index == -1 || index > i))
        {
          index = i;
          match = needle;
        }
      }
      return index > -1 && match is not null;
    }

    /// <summary>
    /// Retrieves a hash code for the specified string that is stable across
    /// multiple runs of the application and .NET upgrades.
    ///
    /// Use this method instead of the normal <c>string.GetHashCode</c> if the hash code
    /// is persisted to disk.
    /// </summary>
    /// <param name="text">The text to hash.</param>
    /// <returns>A stable hash code for <paramref name="text"/>.</returns>
    public static int GetStableHashCode(this string text)
    {
      unchecked
      {
        int h = 0;
        foreach (char c in text)
        {
          h = (h << 5) - h + c;
        }
        return h;
      }
    }

    /// <summary>
    /// Asynchronously copies all text from a reader to a writer.
    /// </summary>
    /// <param name="reader">The source reader.</param>
    /// <param name="writer">The target writer.</param>
    /// <returns>A task that completes when the copy operation finishes.</returns>
    public static async Task CopyToAsync(this TextReader reader, TextWriter writer)
    {
      char[] buffer = new char[2048];
      int read;
      while ((read = await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
      {
        writer.Write(buffer, 0, read);
      }
    }

    #endregion String extensions

    #region Service Provider Extensions

    /// <summary>
    /// Retrieves the service of type <c>T</c> from the provider.
    /// If the service cannot be found, this method returns <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="provider">The service provider.</param>
    /// <returns>The requested service, or <c>null</c> if it is not available.</returns>
    public static T? GetService<T>(this IServiceProvider provider) where T : class
    {
      return (T?)provider.GetService(typeof(T));
    }

    /// <summary>
    /// Retrieves the service of type <c>T</c> from the provider.
    /// If the service cannot be found, a <see cref="ServiceNotFoundException"/> will be thrown.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="provider">The service provider.</param>
    /// <returns>The requested service.</returns>
    public static T GetRequiredService<T>(this IServiceProvider provider) where T : class
    {
      return (T)GetRequiredService(provider, typeof(T));
    }

    /// <summary>
    /// Retrieves the service of type <paramref name="serviceType"/> from the provider.
    /// If the service cannot be found, a <see cref="ServiceNotFoundException"/> will be thrown.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <param name="serviceType">The service type.</param>
    /// <returns>The requested service.</returns>
    public static object GetRequiredService(this IServiceProvider provider, Type serviceType)
    {
      object? service = provider.GetService(serviceType);
      if (service is null)
        throw new ServiceNotFoundException(serviceType);
      return service;
    }

    #endregion Service Provider Extensions
  }
}
