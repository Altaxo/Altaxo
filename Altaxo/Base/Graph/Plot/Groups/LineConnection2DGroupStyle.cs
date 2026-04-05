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
using System.Collections.Generic;
using System.Text;
using Altaxo.Graph.Gdi.Plot.Styles;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// This group style is intended to make sure that all substyles have the same line connection.
   /// Thus it is only intended for local use (only among substyles of a single plot item).
  /// plot styles.
  /// </summary>
  public class LineConnection2DGroupStyle
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IPlotGroupStyle
  {
    private bool _isInitialized;

    private ILineConnectionStyle _lineConnectionStyle;
    private bool _connectCircular;

    private static readonly Type MyType = typeof(LineConnection2DGroupStyle);

    #region Serialization

    /// <summary>
    /// 2016-11-17 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LineConnection2DGroupStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
    /// <inheritdoc />
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LineConnection2DGroupStyle)obj;
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LineConnection2DGroupStyle?)o ?? new LineConnection2DGroupStyle();
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="LineConnection2DGroupStyle"/> class.
    /// </summary>
    public LineConnection2DGroupStyle()
    {
      _lineConnectionStyle = Gdi.Plot.Styles.LineConnectionStyles.NoConnection.Instance;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineConnection2DGroupStyle"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public LineConnection2DGroupStyle(LineConnection2DGroupStyle from)
    {
      _isInitialized = from._isInitialized;
      _lineConnectionStyle = from._lineConnectionStyle;
    }

    #endregion Constructors

    #region ICloneable Members

    /// <inheritdoc />
    public LineConnection2DGroupStyle Clone()
    {
      return new LineConnection2DGroupStyle(this);
    }

    /// <inheritdoc />
    object ICloneable.Clone()
    {
      return new LineConnection2DGroupStyle(this);
    }

    #endregion ICloneable Members

    #region IGroupStyle Members

    /// <inheritdoc />
    public void TransferFrom(IPlotGroupStyle fromb)
    {
      var from = (LineConnection2DGroupStyle)fromb;
      _isInitialized = from._isInitialized;
      _lineConnectionStyle = from._lineConnectionStyle;
    }

    /// <inheritdoc />
    public void BeginPrepare()
    {
      _isInitialized = false;
    }

    /// <inheritdoc />
    public void PrepareStep()
    {
    }

    /// <inheritdoc />
    public void EndPrepare()
    {
    }

    /// <inheritdoc />
    public bool CanCarryOver
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc />
    public bool CanStep
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc />
    public int Step(int step)
    {
      return 0;
    }

    /// <summary>
    /// <inheritdoc />
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
    /// Gets a value indicating whether this group style has already been initialized.
    /// </summary>
    public bool IsInitialized
    {
      get
      {
        return _isInitialized;
      }
    }

    /// <summary>
    /// Initializes the group style with the specified line connection settings.
    /// </summary>
    /// <param name="lineConnectionStyle">The line connection style to use.</param>
    /// <param name="connectCircular">If set to <see langword="true"/>, the line connection is circular.</param>
    public void Initialize(ILineConnectionStyle lineConnectionStyle, bool connectCircular)
    {
      _isInitialized = true;
      _lineConnectionStyle = lineConnectionStyle;
      _connectCircular = connectCircular;
    }

    /// <summary>
    /// Gets the line connection style stored in this group style.
    /// </summary>
    public ILineConnectionStyle LineConnectionStyle
    {
      get
      {
        return _lineConnectionStyle;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the line connection is circular.
    /// </summary>
    public bool ConnectCircular
    {
      get
      {
        return _connectCircular;
      }
    }

    #endregion Other members

    #region Static helpers

    /// <summary>
    /// Adds the external group style if required.
    /// </summary>
    /// <param name="externalGroups">The external group style collection.</param>
    public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
    {
      // this group style is local only, so no addition is made here
    }

    /// <summary>
    /// Adds the local group style if required.
    /// </summary>
    /// <param name="externalGroups">The external group style collection.</param>
    /// <param name="localGroups">The local group style collection.</param>
    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(LineConnection2DGroupStyle)))
        localGroups.Add(new LineConnection2DGroupStyle());
    }

    /// <summary>
    /// Prepares the style.
    /// </summary>
    /// <param name="externalGroups">The external groups.</param>
    /// <param name="localGroups">The local groups.</param>
    /// <param name="getter">The getter function. Item1 of the tuple is the line connection style, Item2 of the tuple is the ConnectCircular flag.</param>
    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Func<Tuple<ILineConnectionStyle, bool>> getter)
    {
      if (!externalGroups.ContainsType(typeof(LineConnection2DGroupStyle))
        && localGroups is not null
        && !localGroups.ContainsType(typeof(LineConnection2DGroupStyle)))
      {
        localGroups.Add(new LineConnection2DGroupStyle());
      }

      LineConnection2DGroupStyle? grpStyle = null;
      if (externalGroups.ContainsType(typeof(LineConnection2DGroupStyle)))
        grpStyle = (LineConnection2DGroupStyle)externalGroups.GetPlotGroupStyle(typeof(LineConnection2DGroupStyle));
      else if (localGroups is not null)
        grpStyle = (LineConnection2DGroupStyle)localGroups.GetPlotGroupStyle(typeof(LineConnection2DGroupStyle));

      if (grpStyle is not null && getter is not null && !grpStyle.IsInitialized)
      {
        var data = getter();
        grpStyle.Initialize(data.Item1, data.Item2);
      }
    }

    /// <summary>
    /// Tries to apply the line connection group style.
    /// </summary>
    /// <param name="externalGroups">The external group style collection.</param>
    /// <param name="localGroups">The local group style collection.</param>
    /// <param name="setter">The callback that applies the extracted settings.</param>
    /// <returns><see langword="true"/> if the style was applied; otherwise, <see langword="false"/>.</returns>
    public static bool ApplyStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Action<ILineConnectionStyle, bool> setter)
    {
      IPlotGroupStyleCollection? grpColl = null;
      if (externalGroups.ContainsType(typeof(LineConnection2DGroupStyle)))
        grpColl = externalGroups;
      else if (localGroups is not null && localGroups.ContainsType(typeof(LineConnection2DGroupStyle)))
        grpColl = localGroups;

      if (grpColl is not null)
      {
        var grpStyle = (LineConnection2DGroupStyle)grpColl.GetPlotGroupStyle(typeof(LineConnection2DGroupStyle));
        grpColl.OnBeforeApplication(typeof(LineConnection2DGroupStyle));
        setter(grpStyle.LineConnectionStyle, grpStyle._connectCircular);
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
