﻿#region Copyright

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
using System.Drawing;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// This group style is used to distribute a symbol size function to all local plot styles.
  /// The symbol size function maps an index i of the actual plot point to a symbol size.
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
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VariableColorGroupStyle)obj;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (VariableColorGroupStyle?)o ?? new VariableColorGroupStyle();
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    public VariableColorGroupStyle()
    {
      _colorForIndex = (i) => Color.Black;
    }

    public VariableColorGroupStyle(VariableColorGroupStyle from)
    {
      _isInitialized = from._isInitialized;
      _colorForIndex = from._colorForIndex;
    }

    #endregion Constructors

    #region ICloneable Members

    public VariableColorGroupStyle Clone()
    {
      return new VariableColorGroupStyle(this);
    }

    object ICloneable.Clone()
    {
      return new VariableColorGroupStyle(this);
    }

    #endregion ICloneable Members

    #region IGroupStyle Members

    public void TransferFrom(IPlotGroupStyle fromb)
    {
      var from = (VariableColorGroupStyle)fromb;
      _isInitialized = from._isInitialized;
      _colorForIndex = from._colorForIndex;
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

    public void Initialize(Func<int, Color> colorForIndex)
    {
      _isInitialized = true;
      _colorForIndex = colorForIndex;
    }

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
    /// If neccessary, adds this group style to localGroups.
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
    /// Try to apply the symbol size group style. Returns true if successfull applied.
    /// </summary>
    /// <param name="externalGroups"></param>
    /// <param name="localGroups"></param>
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
