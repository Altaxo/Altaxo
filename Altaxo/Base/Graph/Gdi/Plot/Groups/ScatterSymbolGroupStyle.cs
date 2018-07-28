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

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Groups
{
  using Collections;
  using Drawing;
  using Graph.Plot.Groups;
  using Graph2D.Plot.Groups;
  using Graph2D.Plot.Styles;

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

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Groups.SymbolShapeStyleGroupStyle", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old versions not allowed.");
        /*
				SymbolShapeStyleGroupStyle s = (SymbolShapeStyleGroupStyle)obj;
				info.AddValue("StepEnabled", s._isStepEnabled);
				*/
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ScatterSymbolGroupStyle s = null != o ? (ScatterSymbolGroupStyle)o : new ScatterSymbolGroupStyle();
        s._isStepEnabled = info.GetBoolean("StepEnabled");
        s._listOfValues = ScatterSymbolListManager.Instance.OldSolid;
        s._value = s._listOfValues[0];

        return s;
      }
    }

    /// <summary>
    /// 2016-08-24 Initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScatterSymbolGroupStyle), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ScatterSymbolGroupStyle s = (ScatterSymbolGroupStyle)obj;
        info.AddValue("StepEnabled", s._isStepEnabled);

        info.AddValue("Value", s._value);

        info.AddValue("ListOfValues", s._listOfValues);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ScatterSymbolGroupStyle s = null != o ? (ScatterSymbolGroupStyle)o : new ScatterSymbolGroupStyle();
        s._isStepEnabled = info.GetBoolean("StepEnabled");

        var value = (IScatterSymbol)info.GetValue("Value", s);

        var listOfValues = (ScatterSymbolList)info.GetValue("ListOfValues", s);
        ScatterSymbolList registeredList;
        ScatterSymbolListManager.Instance.TryRegisterList(listOfValues, Main.ItemDefinitionLevel.Project, out registeredList);
        s._listOfValues = registeredList;

        s.SetValueCoercedToGroup(value);

        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    public ScatterSymbolGroupStyle()
    {
      _listOfValues = ScatterSymbolListManager.Instance.BuiltinDefault;
      _value = _listOfValues[0];
    }

    public ScatterSymbolGroupStyle(ScatterSymbolGroupStyle from)
    {
      this._isStepEnabled = from._isStepEnabled;
      this._value = from._value;
      this._listOfValues = from._listOfValues;
    }

    #endregion Constructors

    #region ICloneable Members

    public ScatterSymbolGroupStyle Clone()
    {
      return new ScatterSymbolGroupStyle(this);
    }

    object ICloneable.Clone()
    {
      return new ScatterSymbolGroupStyle(this);
    }

    #endregion ICloneable Members

    #region IGroupStyle Members

    public void TransferFrom(IPlotGroupStyle fromb)
    {
      ScatterSymbolGroupStyle from = (ScatterSymbolGroupStyle)fromb;
      this._value = from._value;
      this._listOfValues = from._listOfValues;
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
        return true;
      }
    }

    public bool CanStep
    {
      get
      {
        return true;
      }
    }

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
        if (null == value)
          throw new ArgumentNullException(nameof(value));

        if (!object.ReferenceEquals(_listOfValues, value))
        {
          _listOfValues = value;
          SetValueCoercedToGroup(_value);

          EhSelfChanged();
        }
      }
    }

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

    public bool IsInitialized
    {
      get
      {
        return _isInitialized;
      }
    }

    public void Initialize(IScatterSymbol value)
    {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      _isInitialized = true;

      var parentList = ScatterSymbolListManager.Instance.GetParentList(value);
      if (null != parentList)
      {
        _listOfValues = parentList;
      }

      SetValueCoercedToGroup(value);
    }

    public IScatterSymbol ShapeAndStyle
    {
      get
      {
        return _value;
      }
    }

    #endregion Other members

    #region Static helpers

    public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
    {
      if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(ScatterSymbolGroupStyle)))
      {
        ScatterSymbolGroupStyle gstyle = new ScatterSymbolGroupStyle();
        gstyle.IsStepEnabled = true;
        externalGroups.Add(gstyle);
      }
    }

    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(ScatterSymbolGroupStyle)))
        localGroups.Add(new ScatterSymbolGroupStyle());
    }

    public delegate IScatterSymbol Getter();

    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Getter getter)
    {
      if (!externalGroups.ContainsType(typeof(ScatterSymbolGroupStyle))
        && null != localGroups
        && !localGroups.ContainsType(typeof(ScatterSymbolGroupStyle)))
      {
        localGroups.Add(new ScatterSymbolGroupStyle());
      }

      ScatterSymbolGroupStyle grpStyle = null;
      if (externalGroups.ContainsType(typeof(ScatterSymbolGroupStyle)))
        grpStyle = (ScatterSymbolGroupStyle)externalGroups.GetPlotGroupStyle(typeof(ScatterSymbolGroupStyle));
      else if (localGroups != null)
        grpStyle = (ScatterSymbolGroupStyle)localGroups.GetPlotGroupStyle(typeof(ScatterSymbolGroupStyle));

      if (grpStyle != null && getter != null && !grpStyle.IsInitialized)
        grpStyle.Initialize(getter());
    }

    public delegate void Setter(IScatterSymbol val);

    public static void ApplyStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Setter setter)
    {
      ScatterSymbolGroupStyle grpStyle = null;
      IPlotGroupStyleCollection grpColl = null;
      if (externalGroups.ContainsType(typeof(ScatterSymbolGroupStyle)))
        grpColl = externalGroups;
      else if (localGroups != null && localGroups.ContainsType(typeof(ScatterSymbolGroupStyle)))
        grpColl = localGroups;

      if (null != grpColl)
      {
        grpStyle = (ScatterSymbolGroupStyle)grpColl.GetPlotGroupStyle(typeof(ScatterSymbolGroupStyle));
        grpColl.OnBeforeApplication(typeof(ScatterSymbolGroupStyle));
        setter(grpStyle.ShapeAndStyle);
      }
    }

    #endregion Static helpers
  }
}
