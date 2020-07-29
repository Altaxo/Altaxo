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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Altaxo
{
  /// <summary>
  /// Helps to copy instances, preferably by using <see cref="Altaxo.Main.ICopyFrom"/>, or by using <see cref="ICloneable"/> interface.
  /// </summary>
  public static class CopyHelper
  {
    /// <summary>Copies an instance of an immutable class. This can be done simply by assigning <paramref name="from"/> to <paramref name="to"/>.</summary>
    /// <typeparam name="T">The type of the instance to copy.</typeparam>
    /// <param name="to">The variable to copy to.</param>
    /// <param name="from">The instance that was copied.</param>
    public static void CopyImmutable<T>([AllowNull][MaybeNull][NotNullIfNotNull("from")] ref T to, [AllowNull] T from) where T : Main.IImmutable
    {
      to = from;
    }

    /// <summary>
    /// Try to copy one object to another object.
    /// </summary>
    /// <typeparam name="T">The type of the object to copy.</typeparam>
    /// <param name="to">Reference where to copy to.</param>
    /// <param name="from">Object to copy from.</param>
    /// <returns></returns>
    public static bool TryCopy<T>([MaybeNull] ref T to, [MaybeNull] T from)
    {
      var fromC = from as ICloneable;

      if (object.ReferenceEquals(to, from))
      {
        return true;
      }
      else if (from is Main.IImmutable)
      {
        to = from;
        return true;
      }
      else if (from is null)
      {
        to = default;
        return true;
      }
      else if (to is null && fromC is { } _)
      {
        to = (T)fromC.Clone();
        return true;
      }
      else if (to is Main.ICopyFrom toc && to.GetType() == from.GetType())
      {
        toc.CopyFrom(from);
        return true;
      }
      else if (fromC is { } _)
      {
        to = (T)fromC.Clone();
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>Copies an instance.</summary>
    /// <typeparam name="T">The type of the instance to copy.</typeparam>
    /// <param name="to">The variable to copy to.</param>
    /// <param name="from">The instance that was copied.</param>
    public static void Copy<T>([NotNullIfNotNull("from")][AllowNull] ref T to, [MaybeNull] T from) where T : ICloneable?
    {
      if (object.ReferenceEquals(to, from))
      {
      }
      else if (from is null)
      {
#pragma warning disable CS8601 // Possible null reference assignment.
        to = default;
#pragma warning restore CS8601 // Possible null reference assignment.
      }
      else if (to is null)
      {
        to = (T)from.Clone();
      }
      else if (to is Main.ICopyFrom tocf && to.GetType() == from.GetType())
      {
        tocf.CopyFrom(from);
      }
      else
      {
        to = (T)from.Clone();
      }
    }

    /// <summary>Copies an instance.</summary>
    /// <typeparam name="T">The type of the instance to copy.</typeparam>
    /// <param name="to">The variable to copy to.</param>
    /// <param name="from">The instance that was copied.</param>
    public static void CopyN<T>([NotNullIfNotNull("from")] ref T to, [MaybeNull] T from) where T : ICloneable
    {
      if (object.ReferenceEquals(to, from))
      {
      }
      else if (from is null)
      {
#pragma warning disable CS8601 // Possible null reference assignment.
        to = default;
#pragma warning restore CS8601 // Possible null reference assignment.
      }
      else if (to is null)
      {
        to = (T)from.Clone();
      }
      else if (to is Main.ICopyFrom tocf && to.GetType() == from.GetType())
      {
        tocf.CopyFrom(from);
      }
      else
      {
        to = (T)from.Clone();
      }
    }


    /// <summary>Gets a copy of an instance, either by using <see cref="Altaxo.Main.ICopyFrom"/> or <see cref="ICloneable"/> interface.</summary>
    /// <typeparam name="T">The type of the instance to copy.</typeparam>
    /// <param name="to">The value of the variable to copy to.</param>
    /// <param name="from">The instance to copy from.</param>
    /// <returns>The copied instance. It might be the same instance as provided in <paramref name="to"/>, if the interface <see cref="Altaxo.Main.ICopyFrom"/> was used for copying.
    /// If the <see cref="ICloneable"/> interface was used for copying, the returned instance is different from <paramref name="to"/>.</returns>
    [return: NotNullIfNotNull("from")]
    public static T GetCopy<T>(T to, [MaybeNull] T from) where T : ICloneable
    {
      Copy(ref to, from);
      return to;
    }

    /// <summary>
    /// Gets the members of the input enumeration cloned as output enumeration.
    /// </summary>
    /// <typeparam name="T">Type of the enumeration members.</typeparam>
    /// <param name="toClone">Input enumeration.</param>
    /// <returns>Output enumeration with cloned members of the input enumeration.</returns>
    public static IEnumerable<T?> GetEnumerationMembersCloned<T>(IEnumerable<T?> toClone) where T : class, ICloneable
    {
      foreach (var e in toClone)
      {
        if (e is null)
          yield return default;
        else
          yield return (T)e.Clone();
      }
    }

    /// <summary>
    /// Gets the members of the input enumeration cloned as output enumeration.
    /// Here, only those entries of the input enumeration, which are not null, are cloned and returned in the output enumeration.
    /// </summary>
    /// <typeparam name="T">Type of the enumeration members.</typeparam>
    /// <param name="toClone">Input enumeration.</param>
    /// <returns>Output enumeration with cloned members of the input enumeration.</returns>
    public static IEnumerable<T> GetEnumerationMembersNotNullCloned<T>(IEnumerable<T?> toClone) where T : class, ICloneable
    {
      foreach (var e in toClone)
      {
        if (!(e is null))

          yield return (T)e.Clone();
      }
    }
  }
}
