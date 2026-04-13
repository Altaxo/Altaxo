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
using System.Drawing;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// This group style is used to distribute a color function to all local plot styles.
  /// The color function maps an index of the actual plot point to a color.
  /// </summary>
  public class VariableColorGroupStyle
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IPlotGroupStyle
  {
    /// <summary>True if this group style was initialized.</summary>
    private bool _isInitialized;

    /// <summary>The function which maps an index i to a color. </summary>
    private Func<int, Color> _colorForIndex;

    /// <summary>Helper.</summary>
    private static readonly Type MyType = typeof(VariableColorGroupStyle);

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VariableColorGroupStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VariableColorGroupStyle)o;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (VariableColorGroupStyle?)o ?? new VariableColorGroupStyle();
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableColorGroupStyle"/> class.
    /// </summary>
    public VariableColorGroupStyle()
    {
      _colorForIndex = (i) => Color.Black;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableColorGroupStyle"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public VariableColorGroupStyle(VariableColorGroupStyle from)
    {
      _isInitialized = from._isInitialized;
      _colorForIndex = from._colorForIndex;
    }

    #endregion Constructors

    #region ICloneable Members

    /// <summary>
    /// Creates a copy of this style.
    /// </summary>
    /// <returns>A copied style instance.</returns>
    public VariableColorGroupStyle Clone()
    {
      return new VariableColorGroupStyle(this);
    }

    /// <inheritdoc />
    object ICloneable.Clone()
    {
      return new VariableColorGroupStyle(this);
    }

    #endregion ICloneable Members

    #region IGroupStyle Members

    /// <inheritdoc/>
    public void TransferFrom(IPlotGroupStyle from)
    {
      var fromX = (VariableColorGroupStyle)from;
      _isInitialized = fromX._isInitialized;
      _colorForIndex = fromX._colorForIndex;
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
    /// Initializes the color function.
    /// </summary>
    /// <param name="colorForIndex">The color function.</param>
    public void Initialize(Func<int, Color> colorForIndex)
    {
      _isInitialized = true;
      _colorForIndex = colorForIndex;
    }

    /// <summary>
    /// Gets the color function.
    /// </summary>
    public Func<int, Color> ColorForIndex
    {
      get
      {
        return _colorForIndex;
      }
    }

    #endregion Other members

    #region Static helpers

    /// <summary>
    /// If necessary, adds this group style to local groups.
    /// </summary>
    /// <param name="externalGroups">External group styles.</param>
    /// <param name="localGroups">Local group styles.</param>
    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, MyType))
        localGroups.Add(new VariableColorGroupStyle());
    }

    /// <summary>
    /// Prepares the variable-color style.
    /// </summary>
    /// <param name="externalGroups">The external group-style collection.</param>
    /// <param name="localGroups">The local group-style collection.</param>
    /// <param name="getter">The delegate that supplies the color function.</param>
    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Func<int, Color> getter)
    {
      if (!externalGroups.ContainsType(MyType)
        && localGroups is not null
        && !localGroups.ContainsType(MyType))
      {
        localGroups.Add(new VariableColorGroupStyle());
      }

      VariableColorGroupStyle? grpStyle = null;
      if (externalGroups.ContainsType(typeof(SymbolSizeGroupStyle)))
        grpStyle = (VariableColorGroupStyle)externalGroups.GetPlotGroupStyle(MyType);
      else if (localGroups is not null)
        grpStyle = (VariableColorGroupStyle)localGroups.GetPlotGroupStyle(MyType);

      if (grpStyle is not null && getter is not null && !grpStyle.IsInitialized)
        grpStyle.Initialize(getter);
    }

    /// <summary>
    /// Tries to apply the variable-color group style.
    /// </summary>
    /// <param name="externalGroups">The external group-style collection.</param>
    /// <param name="localGroups">The local group-style collection.</param>
    /// <param name="setter">A function of the plot style that takes the symbol size evaluation function.</param>
    /// <returns>True if successfully applied, false otherwise.</returns>
    public static bool ApplyStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Action<Func<int, Color>> setter)
    {
      IPlotGroupStyleCollection? grpColl = null;
      if (externalGroups.ContainsType(MyType))
        grpColl = externalGroups;
      else if (localGroups is not null && localGroups.ContainsType(MyType))
        grpColl = localGroups;

      if (grpColl is not null)
      {
        var grpStyle = (VariableColorGroupStyle)grpColl.GetPlotGroupStyle(MyType);
        grpColl.OnBeforeApplication(MyType);
        setter(grpStyle._colorForIndex);
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
