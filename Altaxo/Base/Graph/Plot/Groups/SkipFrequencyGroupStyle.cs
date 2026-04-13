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

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// This group style is intended to publish the skip frequency to all interested
  /// plot styles.
  /// </summary>
  public class SkipFrequencyGroupStyle
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IPlotGroupStyle
  {
    private bool _isInitialized;
    private int _skipFrequency;
    private static readonly Type MyType = typeof(SkipFrequencyGroupStyle);

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SkipFrequencyGroupStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SkipFrequencyGroupStyle)o;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (SkipFrequencyGroupStyle?)o ?? new SkipFrequencyGroupStyle();
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SkipFrequencyGroupStyle"/> class.
    /// </summary>
    public SkipFrequencyGroupStyle()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SkipFrequencyGroupStyle"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public SkipFrequencyGroupStyle(SkipFrequencyGroupStyle from)
    {
      _isInitialized = from._isInitialized;
      _skipFrequency = from._skipFrequency;
    }

    #endregion Constructors

    #region ICloneable Members

    /// <summary>
    /// Creates a copy of this style.
    /// </summary>
    /// <returns>A copied style instance.</returns>
    public SkipFrequencyGroupStyle Clone()
    {
      return new SkipFrequencyGroupStyle(this);
    }

    /// <inheritdoc />
    object ICloneable.Clone()
    {
      return new SkipFrequencyGroupStyle(this);
    }

    #endregion ICloneable Members

    #region IGroupStyle Members

    /// <inheritdoc/>
    public void TransferFrom(IPlotGroupStyle from)
    {
      var fromX = (SkipFrequencyGroupStyle)from;
      _isInitialized = fromX._isInitialized;
      _skipFrequency = fromX._skipFrequency;
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
    /// Initializes the skip frequency.
    /// </summary>
    /// <param name="skipFrequency">The skip frequency.</param>
    public void Initialize(int skipFrequency)
    {
      _isInitialized = true;
      _skipFrequency = skipFrequency;
    }

    /// <summary>
    /// Gets the skip frequency.
    /// </summary>
    public int SkipFrequency
    {
      get
      {
        return _skipFrequency;
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
    /// Adds the local group style when applicable.
    /// </summary>
    /// <param name="externalGroups">The external group styles.</param>
    /// <param name="localGroups">The local group styles.</param>
    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(SkipFrequencyGroupStyle)))
        localGroups.Add(new SkipFrequencyGroupStyle());
    }

    /// <summary>
    /// Represents a delegate that returns an integer function value.
    /// </summary>
    /// <returns>The integer value returned by the delegate.</returns>
    public delegate int Int32FunctionValueGetter();

    /// <summary>
    /// Prepares the skip-frequency style.
    /// </summary>
    /// <param name="externalGroups">The external group-style collection.</param>
    /// <param name="localGroups">The local group-style collection.</param>
    /// <param name="getter">The delegate that supplies the skip frequency.</param>
    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Int32FunctionValueGetter getter)
    {
      if (!externalGroups.ContainsType(typeof(SkipFrequencyGroupStyle))
        && localGroups is not null
        && !localGroups.ContainsType(typeof(SkipFrequencyGroupStyle)))
      {
        localGroups.Add(new SkipFrequencyGroupStyle());
      }

      SkipFrequencyGroupStyle? grpStyle = null;
      if (externalGroups.ContainsType(typeof(SkipFrequencyGroupStyle)))
        grpStyle = (SkipFrequencyGroupStyle)externalGroups.GetPlotGroupStyle(typeof(SkipFrequencyGroupStyle));
      else if (localGroups is not null)
        grpStyle = (SkipFrequencyGroupStyle)localGroups.GetPlotGroupStyle(typeof(SkipFrequencyGroupStyle));

      if (grpStyle is not null && getter is not null && !grpStyle.IsInitialized)
        grpStyle.Initialize(getter());
    }

    /// <summary>
    /// Represents a delegate that stores an integer value.
    /// </summary>
    /// <param name="c">The value to store.</param>
    public delegate void Int32ValueSetter(int c);

    /// <summary>
    /// Tries to apply the skip-frequency group style.
    /// </summary>
    /// <param name="externalGroups">The external group-style collection.</param>
    /// <param name="localGroups">The local group-style collection.</param>
    /// <param name="setter">The delegate that applies the skip frequency.</param>
    /// <returns><c>true</c> if successfully applied; otherwise, <c>false</c>.</returns>
    public static bool ApplyStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Int32ValueSetter setter)
    {
      IPlotGroupStyleCollection? grpColl = null;
      if (externalGroups.ContainsType(typeof(SkipFrequencyGroupStyle)))
        grpColl = externalGroups;
      else if (localGroups is not null && localGroups.ContainsType(typeof(SkipFrequencyGroupStyle)))
        grpColl = localGroups;

      if (grpColl is not null)
      {
        var grpStyle = (SkipFrequencyGroupStyle)grpColl.GetPlotGroupStyle(typeof(SkipFrequencyGroupStyle));
        grpColl.OnBeforeApplication(typeof(SkipFrequencyGroupStyle));
        setter(grpStyle.SkipFrequency);
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
