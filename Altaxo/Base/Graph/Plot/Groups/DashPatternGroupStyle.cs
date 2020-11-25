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

namespace Altaxo.Graph.Plot.Groups
{
  using Collections;
  using Drawing;
  using Drawing.DashPatternManagement;

  public class DashPatternGroupStyle
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IPlotGroupStyle
  {
    private bool _isInitialized;
    private IDashPattern _value;

    /// <summary>True if step enabled (only if used as external group style with symbol grouping).</summary>
    private bool _isStepEnabled;

    /// <summary>The list of dash pattern to switch through.</summary>
    private IStyleList<IDashPattern> _listOfValues;

    #region Serialization

    /// <summary>
    /// Deserializes the old Altaxo.Graph.Gdi.Plot.Groups.LineStyleGroupStyle (Version 0).
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Groups.LineStyleGroupStyle", 0)]
    private class XmlSerializationSurrogateForLineStyleGroupStyleVersion0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
                LineStyleGroupStyle s = (LineStyleGroupStyle)obj;
                info.AddValue("StepEnabled", s._isStepEnabled);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DashPatternGroupStyle?)o ?? new DashPatternGroupStyle();
        s._isStepEnabled = info.GetBoolean("StepEnabled");

        s._listOfValues = DashPatternListManager.Instance.BuiltinDefault;
        s.SetValueCoercedToGroup(DashPatternListManager.Instance.BuiltinDefault[0]);

        return s;
      }
    }

    /// <summary>
    /// 2016-08-24 Initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DashPatternGroupStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DashPatternGroupStyle)obj;
        info.AddValue("StepEnabled", s._isStepEnabled);

        info.AddValue("Value", s._value);

        info.AddValue("ListOfValues", s._listOfValues);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        DashPatternGroupStyle s = o is not null ? (DashPatternGroupStyle)o : new DashPatternGroupStyle();
        s._isStepEnabled = info.GetBoolean("StepEnabled");

        var value = (IDashPattern)info.GetValue("Value", s);

        var listOfValues = (DashPatternList)info.GetValue("ListOfValues", s);
        DashPatternListManager.Instance.TryRegisterList(listOfValues, Main.ItemDefinitionLevel.Project, out var registeredList);
        s._listOfValues = registeredList;
        s.SetValueCoercedToGroup(value);

        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    public DashPatternGroupStyle()
    {
      _listOfValues = DashPatternListManager.Instance.BuiltinDefault;
      _value = _listOfValues[0];
    }

    public DashPatternGroupStyle(DashPatternGroupStyle from)
    {
      _isStepEnabled = from._isStepEnabled;
      _value = from._value;
      _listOfValues = from._listOfValues;
    }

    public void TransferFrom(IPlotGroupStyle fromb)
    {
      var from = (DashPatternGroupStyle)fromb;
      _value = from._value;
      _listOfValues = from._listOfValues;
    }

    #endregion Constructors

    #region ICloneable Members

    public DashPatternGroupStyle Clone()
    {
      return new DashPatternGroupStyle(this);
    }

    object ICloneable.Clone()
    {
      return new DashPatternGroupStyle(this);
    }

    #endregion ICloneable Members

    #region IGroupStyle Members

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

      int currentIdx = Math.Max(0, _listOfValues.IndexOf(_value));

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
    /// The list of values to switch through
    /// </summary>
    public IStyleList<IDashPattern> ListOfValues
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

    private void SetValueCoercedToGroup(IDashPattern value)
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

    public void Initialize(IDashPattern value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      _isInitialized = true;

      var parentList = DashPatternListManager.Instance.GetParentList(value);
      if (parentList is not null)
      {
        _listOfValues = parentList;
      }

      SetValueCoercedToGroup(value);
    }

    public IDashPattern DashStyle
    {
      get
      {
        return _value;
      }
    }

    #endregion Other members

    #region Static helpers

    public static void AddLocalGroupStyle(
   IPlotGroupStyleCollection externalGroups,
   IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(DashPatternGroupStyle)))
        localGroups.Add(new DashPatternGroupStyle());
    }

    public delegate IDashPattern Getter();

    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Getter getter)
    {
      if (!externalGroups.ContainsType(typeof(DashPatternGroupStyle))
        && localGroups is not null
        && !localGroups.ContainsType(typeof(DashPatternGroupStyle)))
      {
        localGroups.Add(new DashPatternGroupStyle());
      }

      DashPatternGroupStyle? grpStyle = null;
      if (externalGroups.ContainsType(typeof(DashPatternGroupStyle)))
        grpStyle = (DashPatternGroupStyle)externalGroups.GetPlotGroupStyle(typeof(DashPatternGroupStyle));
      else if (localGroups is not null)
        grpStyle = (DashPatternGroupStyle)localGroups.GetPlotGroupStyle(typeof(DashPatternGroupStyle));

      if (grpStyle is not null && getter is not null && !grpStyle.IsInitialized)
        grpStyle.Initialize(getter());
    }

    public delegate void Setter(IDashPattern c);

    public static void ApplyStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Setter setter)
    {
      IPlotGroupStyleCollection? grpColl = null;
      if (externalGroups.ContainsType(typeof(DashPatternGroupStyle)))
        grpColl = externalGroups;
      else if (localGroups is not null && localGroups.ContainsType(typeof(DashPatternGroupStyle)))
        grpColl = localGroups;

      if (grpColl is not null)
      {
        var grpStyle = (DashPatternGroupStyle)grpColl.GetPlotGroupStyle(typeof(DashPatternGroupStyle));
        grpColl.OnBeforeApplication(typeof(DashPatternGroupStyle));
        setter(grpStyle.DashStyle);
      }
    }

    #endregion Static helpers
  }
}
