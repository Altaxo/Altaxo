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
  public static class PlotGroupStyle
  {
    public static bool ShouldAddExternalGroupStyle(
  IPlotGroupStyleCollection externalGroups,
  System.Type type)
    {
      return !externalGroups.ContainsType(type);
    }

    public static bool ShouldAddLocalGroupStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      System.Type type)
    {
      bool found = false;
      if (externalGroups != null && externalGroups.ContainsType(type))
        found = true;
      if (!found && localGroups != null && localGroups.ContainsType(type))
        found = true;

      return (!found && localGroups != null);
    }
    [return: MaybeNull]
    public static T GetStyleToInitialize<T>(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups
      ) where T : IPlotGroupStyle, new()
    {
      if (!externalGroups.ContainsType(typeof(T))
        && null != localGroups
        && !localGroups.ContainsType(typeof(T)))
      {
        localGroups.Add(new T());
      }

      var grpStyle = default(T);
      if (externalGroups.ContainsType(typeof(T)))
        grpStyle = (T)externalGroups.GetPlotGroupStyle(typeof(T));
      else if (localGroups != null)
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
      else if (localGroups != null && localGroups.ContainsType(typeof(T)))
        grpColl = localGroups;

      if (null != grpColl)
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
    /// If an instance with this interface found, this instance is returned. If found, the containig collection
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
      if (null == grpStyle)
      {
        grpColl = localGroups;
        grpStyle = grpColl.FirstOrDefault(style => typeof(T).IsAssignableFrom(style.GetType()));
      }

      if (null != grpStyle)
      {
        grpColl.OnBeforeApplication(grpStyle.GetType());
      }

      return (T)grpStyle;
    }
  }
}
