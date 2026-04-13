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

#nullable enable
using System;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// This group style is intended to make sure that all substyles have the same line connection.
  /// Thus it is only intended for local use, only among substyles of a single plot item.
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
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (IgnoreMissingDataPointsGroupStyle)o;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (IgnoreMissingDataPointsGroupStyle?)o ?? new IgnoreMissingDataPointsGroupStyle();
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="IgnoreMissingDataPointsGroupStyle"/> class.
    /// </summary>
    public IgnoreMissingDataPointsGroupStyle()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IgnoreMissingDataPointsGroupStyle"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    /// <summary>
    /// Initializes a new instance of the <see cref="IgnoreMissingDataPointsGroupStyle"/> class by copying another instance.
    /// </summary>
    public IgnoreMissingDataPointsGroupStyle(IgnoreMissingDataPointsGroupStyle from)
    {
      _isInitialized = from._isInitialized;
      _ignoreMissingDataPoints = from._ignoreMissingDataPoints;
    }

    #endregion Constructors

    #region ICloneable Members

    /// <summary>
    /// Creates a copy of this style.
    /// </summary>
    /// <returns>A copied style instance.</returns>
    public IgnoreMissingDataPointsGroupStyle Clone()
    {
      return new IgnoreMissingDataPointsGroupStyle(this);
    }

    /// <inheritdoc />
    object ICloneable.Clone()
    {
      return new IgnoreMissingDataPointsGroupStyle(this);
    }

    #endregion ICloneable Members

    #region IGroupStyle Members

    /// <inheritdoc/>
    public void TransferFrom(IPlotGroupStyle from)
    {
      var fromX = (IgnoreMissingDataPointsGroupStyle)from;
      _isInitialized = fromX._isInitialized;
      _ignoreMissingDataPoints = fromX._ignoreMissingDataPoints;
    }

    /// <inheritdoc/>
    public void BeginPrepare()
    {
      _isInitialized = false;
    }

    /// <inheritdoc/>
    public void PrepareStep()
    {
    }

    /// <inheritdoc/>
    public void EndPrepare()
    {
    }

    /// <inheritdoc/>
    public bool CanCarryOver
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public bool CanStep
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
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

    /// <summary>
    /// Gets a value indicating whether this style was initialized.
    /// </summary>
    public bool IsInitialized
    {
      get
      {
        return _isInitialized;
      }
    }

    /// <summary>
    /// Initializes the style.
    /// </summary>
    /// <param name="ignoreMissingDataPoints">If set to <c>true</c>, missing data points are ignored.</param>
    public void Initialize(bool ignoreMissingDataPoints)
    {
      _isInitialized = true;
      _ignoreMissingDataPoints = ignoreMissingDataPoints;
    }

    /// <summary>
    /// Gets a value indicating whether missing data points are ignored.
    /// </summary>
    public bool IgnoreMissingDataPoints
    {
      get
      {
        return _ignoreMissingDataPoints;
      }
    }

    #endregion Other members

    #region Static helpers

    /// <summary>
    /// Adds the external group style when applicable.
    /// </summary>
    /// <param name="externalGroups">The external group-style collection.</param>
    public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
    {
      // this group style is local only, so no addition is made here
    }

    /// <summary>
    /// Adds the ignore-missing-data-points group style to the local collection when required.
    /// </summary>
    /// <param name="externalGroups">The external group-style collection.</param>
    /// <param name="localGroups">The local group-style collection.</param>
    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(IgnoreMissingDataPointsGroupStyle)))
        localGroups.Add(new IgnoreMissingDataPointsGroupStyle());
    }

    /// <summary>
    /// Prepares an ignore-missing-data-points group style for later application.
    /// </summary>
    /// <param name="externalGroups">The external group-style collection.</param>
    /// <param name="localGroups">The local group-style collection.</param>
    /// <param name="getter">The delegate that supplies the current setting.</param>
    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Func<bool> getter)
    {
      if (!externalGroups.ContainsType(typeof(IgnoreMissingDataPointsGroupStyle))
        && localGroups is not null
        && !localGroups.ContainsType(typeof(IgnoreMissingDataPointsGroupStyle)))
      {
        localGroups.Add(new IgnoreMissingDataPointsGroupStyle());
      }

      IgnoreMissingDataPointsGroupStyle? grpStyle = null;
      if (externalGroups.ContainsType(typeof(IgnoreMissingDataPointsGroupStyle)))
        grpStyle = (IgnoreMissingDataPointsGroupStyle)externalGroups.GetPlotGroupStyle(typeof(IgnoreMissingDataPointsGroupStyle));
      else if (localGroups is not null)
        grpStyle = (IgnoreMissingDataPointsGroupStyle)localGroups.GetPlotGroupStyle(typeof(IgnoreMissingDataPointsGroupStyle));

      if (grpStyle is not null && getter is not null && !grpStyle.IsInitialized)
        grpStyle.Initialize(getter());
    }

    /// <summary>
    /// Tries to apply the ignore-missing-data-points group style.
    /// </summary>
    /// <param name="externalGroups">The external group-style collection.</param>
    /// <param name="localGroups">The local group-style collection.</param>
    /// <param name="setter">The delegate that applies the ignore-missing-data-points setting.</param>
    /// <returns><c>true</c> if the style was applied; otherwise, <c>false</c>.</returns>
    public static bool ApplyStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Action<bool> setter)
    {
      IPlotGroupStyleCollection? grpColl = null;
      if (externalGroups.ContainsType(typeof(IgnoreMissingDataPointsGroupStyle)))
        grpColl = externalGroups;
      else if (localGroups is not null && localGroups.ContainsType(typeof(IgnoreMissingDataPointsGroupStyle)))
        grpColl = localGroups;

      if (grpColl is not null)
      {
        var grpStyle = (IgnoreMissingDataPointsGroupStyle)grpColl.GetPlotGroupStyle(typeof(IgnoreMissingDataPointsGroupStyle));
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
