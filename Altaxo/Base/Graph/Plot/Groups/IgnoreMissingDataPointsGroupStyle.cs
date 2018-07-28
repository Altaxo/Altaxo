#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Graph.Gdi.Plot.Styles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// This group style is intended to make sure that all substyles have the same line connection.
  /// Thus it is only intended for local use (only amound substyles of a single plot item).
  /// plot styles.
  /// </summary>
  public class IgnoreMissingDataPointsGroupStyle
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IPlotGroupStyle
  {
    private bool _isInitialized;
    private bool _ignoreMissingDataPoints;
    private static readonly Type MyType = typeof(IgnoreMissingDataPointsGroupStyle);

    #region Serialization

    /// <summary>
    /// 2016-11-17 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(IgnoreMissingDataPointsGroupStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        IgnoreMissingDataPointsGroupStyle s = (IgnoreMissingDataPointsGroupStyle)obj;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        IgnoreMissingDataPointsGroupStyle s = null != o ? (IgnoreMissingDataPointsGroupStyle)o : new IgnoreMissingDataPointsGroupStyle();
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    public IgnoreMissingDataPointsGroupStyle()
    {
    }

    public IgnoreMissingDataPointsGroupStyle(IgnoreMissingDataPointsGroupStyle from)
    {
      this._isInitialized = from._isInitialized;
      this._ignoreMissingDataPoints = from._ignoreMissingDataPoints;
    }

    #endregion Constructors

    #region ICloneable Members

    public IgnoreMissingDataPointsGroupStyle Clone()
    {
      return new IgnoreMissingDataPointsGroupStyle(this);
    }

    object ICloneable.Clone()
    {
      return new IgnoreMissingDataPointsGroupStyle(this);
    }

    #endregion ICloneable Members

    #region IGroupStyle Members

    public void TransferFrom(IPlotGroupStyle fromb)
    {
      IgnoreMissingDataPointsGroupStyle from = (IgnoreMissingDataPointsGroupStyle)fromb;
      this._isInitialized = from._isInitialized;
      _ignoreMissingDataPoints = from._ignoreMissingDataPoints;
    }

    public void BeginPrepare()
    {
      _isInitialized = false;
    }

    public void PrepareStep()
    {
    }

    public void EndPrepare()
    {
    }

    public bool CanCarryOver
    {
      get
      {
        return false;
      }
    }

    public bool CanStep
    {
      get
      {
        return false;
      }
    }

    public int Step(int step)
    {
      return 0;
    }

    /// <summary>
    /// Get/sets whether or not stepping is allowed.
    /// </summary>
    public bool IsStepEnabled
    {
      get
      {
        return false;
      }
      set
      {
      }
    }

    #endregion IGroupStyle Members

    #region Other members

    public bool IsInitialized
    {
      get
      {
        return _isInitialized;
      }
    }

    public void Initialize(bool ignoreMissingDataPoints)
    {
      _isInitialized = true;
      _ignoreMissingDataPoints = ignoreMissingDataPoints;
    }

    public bool IgnoreMissingDataPoints
    {
      get
      {
        return _ignoreMissingDataPoints;
      }
    }

    #endregion Other members

    #region Static helpers

    public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
    {
      // this group style is local only, so no addition is made here
    }

    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(IgnoreMissingDataPointsGroupStyle)))
        localGroups.Add(new IgnoreMissingDataPointsGroupStyle());
    }

    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Func<bool> getter)
    {
      if (!externalGroups.ContainsType(typeof(IgnoreMissingDataPointsGroupStyle))
        && null != localGroups
        && !localGroups.ContainsType(typeof(IgnoreMissingDataPointsGroupStyle)))
      {
        localGroups.Add(new IgnoreMissingDataPointsGroupStyle());
      }

      IgnoreMissingDataPointsGroupStyle grpStyle = null;
      if (externalGroups.ContainsType(typeof(IgnoreMissingDataPointsGroupStyle)))
        grpStyle = (IgnoreMissingDataPointsGroupStyle)externalGroups.GetPlotGroupStyle(typeof(IgnoreMissingDataPointsGroupStyle));
      else if (localGroups != null)
        grpStyle = (IgnoreMissingDataPointsGroupStyle)localGroups.GetPlotGroupStyle(typeof(IgnoreMissingDataPointsGroupStyle));

      if (grpStyle != null && getter != null && !grpStyle.IsInitialized)
        grpStyle.Initialize(getter());
    }

    /// <summary>
    /// Try to apply the symbol size group style. Returns true if successfull applied.
    /// </summary>
    /// <param name="externalGroups"></param>
    /// <param name="localGroups"></param>
    /// <param name="setter"></param>
    /// <returns></returns>
    public static bool ApplyStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Action<bool> setter)
    {
      IgnoreMissingDataPointsGroupStyle grpStyle = null;
      IPlotGroupStyleCollection grpColl = null;
      if (externalGroups.ContainsType(typeof(IgnoreMissingDataPointsGroupStyle)))
        grpColl = externalGroups;
      else if (localGroups != null && localGroups.ContainsType(typeof(IgnoreMissingDataPointsGroupStyle)))
        grpColl = localGroups;

      if (null != grpColl)
      {
        grpStyle = (IgnoreMissingDataPointsGroupStyle)grpColl.GetPlotGroupStyle(typeof(IgnoreMissingDataPointsGroupStyle));
        grpColl.OnBeforeApplication(typeof(IgnoreMissingDataPointsGroupStyle));
        setter(grpStyle.IgnoreMissingDataPoints);
        return true;
      }
      else
      {
        return false;
      }
    }

    #endregion Static helpers
  }
}
