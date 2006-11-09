using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.PlotGroups
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
