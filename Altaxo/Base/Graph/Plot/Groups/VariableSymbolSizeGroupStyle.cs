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

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// This group style is used to distribute a symbol size function to all local plot styles.
  /// The symbol size function maps an index i of the actual plot point to a symbol size.
  /// </summary>
  public class VariableSymbolSizeGroupStyle
    :
        Main.SuspendableDocumentLeafNodeWithEventArgs,
      IPlotGroupStyle
  {
    /// <summary>True if this group style was initialized.</summary>
    private bool _isInitialized;

    /// <summary>The symbol size function which maps an index i to a symbol size. </summary>
    private Func<int, double> _symbolSizeForIndex;

    /// <summary>Helper.</summary>
    private static readonly Type MyType = typeof(VariableSymbolSizeGroupStyle);

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VariableSymbolSizeGroupStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VariableSymbolSizeGroupStyle)obj;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = null != o ? (VariableSymbolSizeGroupStyle)o : new VariableSymbolSizeGroupStyle();
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    public VariableSymbolSizeGroupStyle()
    {
    }

    public VariableSymbolSizeGroupStyle(VariableSymbolSizeGroupStyle from)
    {
      _isInitialized = from._isInitialized;
      _symbolSizeForIndex = from._symbolSizeForIndex;
    }

    #endregion Constructors

    #region ICloneable Members

    public VariableSymbolSizeGroupStyle Clone()
    {
      return new VariableSymbolSizeGroupStyle(this);
    }

    object ICloneable.Clone()
    {
      return new VariableSymbolSizeGroupStyle(this);
    }

    #endregion ICloneable Members

    #region IGroupStyle Members

    public void TransferFrom(IPlotGroupStyle fromb)
    {
      var from = (VariableSymbolSizeGroupStyle)fromb;
      _isInitialized = from._isInitialized;
      _symbolSizeForIndex = from._symbolSizeForIndex;
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

    public void Initialize(Func<int, double> symbolSizeForIndex)
    {
      _isInitialized = true;
      _symbolSizeForIndex = symbolSizeForIndex;
    }

    public Func<int, double> SymbolSizeForIndex
    {
      get
      {
        return _symbolSizeForIndex;
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
        localGroups.Add(new VariableSymbolSizeGroupStyle());
    }

    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Func<int, double> getter)
    {
      if (!externalGroups.ContainsType(MyType)
        && null != localGroups
        && !localGroups.ContainsType(MyType))
      {
        localGroups.Add(new VariableSymbolSizeGroupStyle());
      }

      VariableSymbolSizeGroupStyle grpStyle = null;
      if (externalGroups.ContainsType(typeof(SymbolSizeGroupStyle)))
        grpStyle = (VariableSymbolSizeGroupStyle)externalGroups.GetPlotGroupStyle(MyType);
      else if (localGroups != null)
        grpStyle = (VariableSymbolSizeGroupStyle)localGroups.GetPlotGroupStyle(MyType);

      if (grpStyle != null && getter != null && !grpStyle.IsInitialized)
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
      Action<Func<int, double>> setter)
    {
      VariableSymbolSizeGroupStyle grpStyle = null;
      IPlotGroupStyleCollection grpColl = null;
      if (externalGroups.ContainsType(MyType))
        grpColl = externalGroups;
      else if (localGroups != null && localGroups.ContainsType(MyType))
        grpColl = localGroups;

      if (null != grpColl)
      {
        grpStyle = (VariableSymbolSizeGroupStyle)grpColl.GetPlotGroupStyle(MyType);
        grpColl.OnBeforeApplication(MyType);
        setter(grpStyle._symbolSizeForIndex);
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
