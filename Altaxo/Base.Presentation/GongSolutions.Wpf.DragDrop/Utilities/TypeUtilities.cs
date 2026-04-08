#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
// BSD 3-Clause License
//
// Copyright(c) 2015-16, Jan Karger(Steven Kirk)
//
// All rights reserved.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable disable warnings
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
  /// <summary>
  /// Provides helper methods for working with runtime types used during drag and drop.
  /// </summary>
  public static class TypeUtilities
  {
#if NET35
    /// <summary>
    /// Check to see if a flags enumeration has a specific flag set.
    /// </summary>
    /// <param name="variable">Flags enumeration to check</param>
    /// <param name="flag"></param>
    public static bool HasFlag(this Enum variable, Enum flag)
    {
      if (variable == null) {
        return false;
      }

      if (flag == null) {
        throw new ArgumentNullException("flag");
      }

      if (flag.GetType() != variable.GetType()) {
        throw new ArgumentException(string.Format("Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.", flag.GetType(), variable.GetType()));
      }

      var uFlag = Convert.ToUInt64(flag);
      var uVar = Convert.ToUInt64(variable);
      return ((uVar & uFlag) == uFlag);
    }
#endif

    /// <summary>
    /// Creates a generic list whose element type is the common base type of the provided source elements.
    /// </summary>
    /// <param name="source">The source elements.</param>
    /// <returns>A dynamically typed list containing the source elements.</returns>
    public static IEnumerable CreateDynamicallyTypedList(IEnumerable source)
    {
      var type = GetCommonBaseClass(source);
      var listType = typeof(List<>).MakeGenericType(type);
      var addMethod = listType.GetMethod("Add");
      var list = listType.GetConstructor(Type.EmptyTypes).Invoke(null);

      foreach (var o in source)
      {
        addMethod.Invoke(list, new[] { o });
      }

      return (IEnumerable)list;
    }

    /// <summary>
    /// Gets the common base type of all elements in the specified sequence.
    /// </summary>
    /// <param name="e">The sequence to inspect.</param>
    /// <returns>The common base type.</returns>
    public static Type GetCommonBaseClass(IEnumerable e)
    {
      var types = e.Cast<object>().Select(o => o.GetType()).ToArray<Type>();
      return GetCommonBaseClass(types);
    }

    /// <summary>
    /// Gets the common base type of the specified types.
    /// </summary>
    /// <param name="types">The types to inspect.</param>
    /// <returns>The common base type.</returns>
    public static Type GetCommonBaseClass(Type[] types)
    {
      if (types.Length == 0)
      {
        return typeof(object);
      }

      var ret = types[0];

      for (var i = 1; i < types.Length; ++i)
      {
        if (types[i].IsAssignableFrom(ret))
        {
          ret = types[i];
        }
        else
        {
          // This will always terminate when ret == typeof(object)
          while (!ret.IsAssignableFrom(types[i]))
          {
            ret = ret.BaseType;
          }
        }
      }

      return ret;
    }
  }
}
