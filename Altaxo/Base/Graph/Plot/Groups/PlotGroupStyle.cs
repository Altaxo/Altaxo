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
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// Provides helper methods for working with plot group styles from local and external collections.
  /// </summary>
  public static class PlotGroupStyle
  {
    /// <summary>
    /// Determines whether an external group style of the specified type should be added.
    /// </summary>
    /// <param name="externalGroups">The external group styles.</param>
    /// <param name="type">The group style type.</param>
    /// <returns><c>true</c> if the type is not already present; otherwise, <c>false</c>.</returns>
    public static bool ShouldAddExternalGroupStyle(
  IPlotGroupStyleCollection externalGroups,
  System.Type type)
    {
      return !externalGroups.ContainsType(type);
    }

    /// <summary>
    /// Determines whether a local group style of the specified type should be added.
    /// </summary>
    /// <param name="externalGroups">The external group styles.</param>
    /// <param name="localGroups">The local group styles.</param>
    /// <param name="type">The group style type.</param>
    /// <returns><c>true</c> if the style should be added locally; otherwise, <c>false</c>.</returns>
    public static bool ShouldAddLocalGroupStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      System.Type type)
    {
      bool found = false;
      if (externalGroups is not null && externalGroups.ContainsType(type))
        found = true;
      if (!found && localGroups is not null && localGroups.ContainsType(type))
        found = true;

      return (!found && localGroups is not null);
    }
    /// <summary>
    /// Gets a style instance of type <typeparamref name="T"/> that still needs initialization.
    /// </summary>
    /// <typeparam name="T">The group style type.</typeparam>
    /// <param name="externalGroups">The external group styles.</param>
    /// <param name="localGroups">The local group styles.</param>
    /// <returns>The style to initialize, or <c>null</c> if no uninitialized style is available.</returns>
    [return: MaybeNull]
    public static T GetStyleToInitialize<T>(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups
      ) where T : IPlotGroupStyle, new()
    {
      if (!externalGroups.ContainsType(typeof(T))
        && localGroups is not null
        && !localGroups.ContainsType(typeof(T)))
      {
        localGroups.Add(new T());
      }

      var grpStyle = default(T);
      if (externalGroups.ContainsType(typeof(T)))
        grpStyle = (T)externalGroups.GetPlotGroupStyle(typeof(T));
      else if (localGroups is not null)
        grpStyle = (T)localGroups.GetPlotGroupStyle(typeof(T));

      if (grpStyle is not null && !grpStyle.IsInitialized)
        return grpStyle;
      else
        return default(T);
    }

    /// <summary>
    /// Looks first in externalGroups, then in localGroups for the type of PlotGroupStyle to apply.
    /// If an instance of this type is found, this instance is returned. If found, the containig collection
    /// is informed that this group style will be applied now by calling OnBeforeApplication.
    /// </summary>
    /// <typeparam name="T">Type of PlotGroupStyle to look for.</typeparam>
    /// <param name="externalGroups">First collection to look for the group style.</param>
    /// <param name="localGroups">Second collection to look for the group style.</param>
    /// <returns>The instance of the plot group style (if found), or null otherwise.</returns>
    [return: MaybeNull]
    public static T GetStyleToApply<T>(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups
     ) where T : IPlotGroupStyle
    {
      var grpStyle = default(T);
      IPlotGroupStyleCollection? grpColl = null;
      if (externalGroups.ContainsType(typeof(T)))
        grpColl = externalGroups;
      else if (localGroups is not null && localGroups.ContainsType(typeof(T)))
        grpColl = localGroups;

      if (grpColl is not null)
      {
        grpStyle = (T)grpColl.GetPlotGroupStyle(typeof(T));
        grpColl.OnBeforeApplication(typeof(T));
      }

      return grpStyle;
    }

    /// <summary>
    /// Looks first in externalGroups, then in localGroups for the type of PlotGroupStyle to apply.
    /// In contrast to <see cref="GetStyleToApply{T}(IPlotGroupStyleCollection, IPlotGroupStyleCollection)"/>, we are searching here only for an interface,
    /// and return the first plot group style found that implements that interface.
    /// If an instance with this interface is found, this instance is returned. If found, the containing collection
    /// is informed that this group style will be applied now by calling OnBeforeApplication.
    /// </summary>
    /// <typeparam name="T">Type of the interface to look for.</typeparam>
    /// <param name="externalGroups">First collection to look for the group style.</param>
    /// <param name="localGroups">Second collection to look for the group style.</param>
    /// <returns>The instance of the plot group style that implements the interface (if found), or null otherwise.</returns>
    [return: MaybeNull]
    public static T GetFirstStyleToApplyImplementingInterface<T>(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups
     )
    {
      IPlotGroupStyle? grpStyle = null;
      IPlotGroupStyleCollection grpColl = externalGroups;
      grpStyle = grpColl.FirstOrDefault(style => typeof(T).IsAssignableFrom(style.GetType()));
      if (grpStyle is null)
      {
        grpColl = localGroups;
        grpStyle = grpColl.FirstOrDefault(style => typeof(T).IsAssignableFrom(style.GetType()));
      }

      if (grpStyle is not null)
      {
        grpColl.OnBeforeApplication(grpStyle.GetType());
      }

      return (T)grpStyle;
    }
  }
}
