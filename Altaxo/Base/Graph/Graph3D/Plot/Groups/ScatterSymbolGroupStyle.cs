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

namespace Altaxo.Graph.Graph3D.Plot.Groups
{
  using Collections;
  using Drawing;
  using Graph.Plot.Groups;
  using Styles;

  /// <summary>
  /// Provides group-style handling for scatter symbols in three-dimensional plots.
  /// </summary>
  public class ScatterSymbolGroupStyle
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IPlotGroupStyle
  {
    private bool _isInitialized;

    private IScatterSymbol _value;

    /// <summary>True if step enabled (only if used as external group style with symbol grouping).</summary>
    private bool _isStepEnabled;

    /// <summary>The list of scatter symbols to switch through.</summary>
    private IStyleList<IScatterSymbol> _listOfValues;

    #region Serialization

    /// <summary>
    /// 2016-08-24 Initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    /// <summary>
    /// Serializes <see cref="ScatterSymbolGroupStyle"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScatterSymbolGroupStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ScatterSymbolGroupStyle)obj;
        info.AddValue("StepEnabled", s._isStepEnabled);

        info.AddValue("Value", s._value);

        info.AddValue("ListOfValues", s._listOfValues);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ScatterSymbolGroupStyle?)o ?? new ScatterSymbolGroupStyle();
        s._isStepEnabled = info.GetBoolean("StepEnabled");

        var value = (IScatterSymbol)info.GetValue("Value", s);

        var listOfValues = (ScatterSymbolList)info.GetValue("ListOfValues", s);
        ScatterSymbolListManager.Instance.TryRegisterList(listOfValues, Main.ItemDefinitionLevel.Project, out var registeredList);
        s._listOfValues = registeredList;

        s.SetValueCoercedToGroup(value);

        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterSymbolGroupStyle"/> class.
    /// </summary>
    public ScatterSymbolGroupStyle()
    {
      _listOfValues = ScatterSymbolListManager.Instance.BuiltinDefault;
      _value = _listOfValues[0];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterSymbolGroupStyle"/> class by copying from another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public ScatterSymbolGroupStyle(ScatterSymbolGroupStyle from)
    {
      _isStepEnabled = from._isStepEnabled;
      _value = from._value;
      _listOfValues = from._listOfValues;
    }

    #endregion Constructors

    #region ICloneable Members

    /// <summary>
    /// Creates a strongly typed clone of this group style.
    /// </summary>
    /// <returns>A cloned group style.</returns>
    public ScatterSymbolGroupStyle Clone()
    {
      return new ScatterSymbolGroupStyle(this);
    }

    /// <inheritdoc/>
    object ICloneable.Clone()
    {
      return new ScatterSymbolGroupStyle(this);
    }

    #endregion ICloneable Members

    #region IGroupStyle Members

    /// <inheritdoc/>
    public void TransferFrom(IPlotGroupStyle fromb)
    {
      var from = (ScatterSymbolGroupStyle)fromb;
      _value = from._value;
      _listOfValues = from._listOfValues;
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
        return true;
      }
    }

    /// <inheritdoc/>
    public bool CanStep
    {
      get
      {
        return true;
      }
    }

    /// <inheritdoc/>
    public int Step(int step)
    {
      if (0 == step)
        return 0; // nothing changed

      var list = _listOfValues;
      var listcount = list.Count;

      if (listcount == 0)
      {
        return 0;
      }

      var currentIdx = Math.Max(0, list.IndexOf(_value));

      var valueIndex = Calc.BasicFunctions.PMod(currentIdx + step, _listOfValues.Count);
      int wraps = Calc.BasicFunctions.NumberOfWraps(_listOfValues.Count, currentIdx, step);
      _value = _listOfValues[valueIndex];
      return wraps;
    }

    /// <summary>
    /// Get/sets whether or not stepping is allowed.
    /// </summary>
    public bool IsStepEnabled
    {
      get
      {
        return _isStepEnabled;
      }
      set
      {
        var oldValue = _isStepEnabled;
        _isStepEnabled = value;

        if (value != oldValue)
          SetValueCoercedToGroup(_value);
      }
    }

    /// <summary>
    /// The list of symbols to switch through
    /// </summary>
    public IStyleList<IScatterSymbol> ListOfValues
    {
      get
      {
        return _listOfValues;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (!object.ReferenceEquals(_listOfValues, value))
        {
          _listOfValues = value;
          SetValueCoercedToGroup(_value);

          EhSelfChanged();
        }
      }
    }

    /// <summary>
    /// Coerces the current value to the active symbol list if stepping is enabled.
    /// </summary>
    /// <param name="value">The candidate scatter symbol.</param>
    private void SetValueCoercedToGroup(IScatterSymbol value)
    {
      if (_isStepEnabled)
      {
        var idx = Math.Max(0, _listOfValues.IndexOf(value));
        _value = _listOfValues[idx];
      }
      else
      {
        _value = value;
      }
    }

    #endregion IGroupStyle Members

    #region Other members

    /// <summary>
    /// Gets a value indicating whether the style has been initialized with a symbol.
    /// </summary>
    public bool IsInitialized
    {
      get
      {
        return _isInitialized;
      }
    }

    /// <summary>
    /// Initializes the group style with the specified scatter symbol.
    /// </summary>
    /// <param name="value">The scatter symbol.</param>
    public void Initialize(IScatterSymbol value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      _isInitialized = true;

      var parentList = ScatterSymbolListManager.Instance.GetParentList(value);
      if (parentList is not null)
      {
        _listOfValues = parentList;
      }

      SetValueCoercedToGroup(value);
    }

    /// <summary>
    /// Gets the current scatter symbol shape and style.
    /// </summary>
    public IScatterSymbol ShapeAndStyle
    {
      get
      {
        return _value;
      }
    }

    #endregion Other members

    #region Static helpers

    /// <summary>
    /// Adds the external scatter-symbol group style when required.
    /// </summary>
    public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
    {
      if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(ScatterSymbolGroupStyle)))
      {
        var gstyle = new ScatterSymbolGroupStyle
        {
          IsStepEnabled = true
        };
        externalGroups.Add(gstyle);
      }
    }

    /// <summary>
    /// Adds the local scatter-symbol group style when required.
    /// </summary>
    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(ScatterSymbolGroupStyle)))
        localGroups.Add(new ScatterSymbolGroupStyle());
    }

    /// <summary>
    /// Represents a callback that reads a scatter symbol.
    /// </summary>
    public delegate IScatterSymbol Getter();

    /// <summary>
    /// Prepares the scatter-symbol group style using the supplied getter.
    /// </summary>
    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Getter getter)
    {
      if (!externalGroups.ContainsType(typeof(ScatterSymbolGroupStyle))
        && localGroups is not null
        && !localGroups.ContainsType(typeof(ScatterSymbolGroupStyle)))
      {
        localGroups.Add(new ScatterSymbolGroupStyle());
      }

      ScatterSymbolGroupStyle? grpStyle = null;
      if (externalGroups.ContainsType(typeof(ScatterSymbolGroupStyle)))
        grpStyle = (ScatterSymbolGroupStyle)externalGroups.GetPlotGroupStyle(typeof(ScatterSymbolGroupStyle));
      else if (localGroups is not null)
        grpStyle = (ScatterSymbolGroupStyle)localGroups.GetPlotGroupStyle(typeof(ScatterSymbolGroupStyle));

      if (grpStyle is not null && getter is not null && !grpStyle.IsInitialized)
        grpStyle.Initialize(getter());
    }

    /// <summary>
    /// Represents a callback that writes a scatter symbol.
    /// </summary>
    public delegate void Setter(IScatterSymbol val);

    /// <summary>
    /// Applies the scatter-symbol group style using the supplied setter.
    /// </summary>
    public static void ApplyStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Setter setter)
    {
      IPlotGroupStyleCollection? grpColl = null;
      if (externalGroups.ContainsType(typeof(ScatterSymbolGroupStyle)))
        grpColl = externalGroups;
      else if (localGroups is not null && localGroups.ContainsType(typeof(ScatterSymbolGroupStyle)))
        grpColl = localGroups;

      if (grpColl is not null)
      {
        var grpStyle = (ScatterSymbolGroupStyle)grpColl.GetPlotGroupStyle(typeof(ScatterSymbolGroupStyle));
        grpColl.OnBeforeApplication(typeof(ScatterSymbolGroupStyle));
        setter(grpStyle.ShapeAndStyle);
      }
    }

    #endregion Static helpers
  }
}
