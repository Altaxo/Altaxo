#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  using Gdi.Plot.Groups;

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


    public static T GetStyleToInitialize<T>(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups
      ) where T: IPlotGroupStyle, new()
    {
      if (!externalGroups.ContainsType(typeof(T))
        && null != localGroups
        && !localGroups.ContainsType(typeof(T)))
      {
        localGroups.Add(new T());
      }

      T grpStyle = default(T);
      if (externalGroups.ContainsType(typeof(T)))
        grpStyle = (T)externalGroups.GetPlotGroupStyle(typeof(T));
      else if (localGroups != null)
        grpStyle = (T)localGroups.GetPlotGroupStyle(typeof(T));

      if (grpStyle != null && !grpStyle.IsInitialized)
        return grpStyle;
      else
        return default(T);
    }


    public static T GetStyleToApply<T>(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups
     ) where T : IPlotGroupStyle
    {
      T grpStyle = default(T);
      IPlotGroupStyleCollection grpColl = null;
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
  }
}
