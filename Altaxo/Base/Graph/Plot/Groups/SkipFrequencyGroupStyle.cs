#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  using Gdi.Plot.Groups;

  /// <summary>
  /// This group style is intended to publish the symbol size to all interested
  /// plot styles.
  /// </summary>
  public class SkipFrequencyGroupStyle : IPlotGroupStyle
  {
    bool _isInitialized;
    int _skipFrequency;
    readonly static Type MyType = typeof(SkipFrequencyGroupStyle);

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SkipFrequencyGroupStyle), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SkipFrequencyGroupStyle s = (SkipFrequencyGroupStyle)obj;
      }


      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        SkipFrequencyGroupStyle s = null != o ? (SkipFrequencyGroupStyle)o : new SkipFrequencyGroupStyle();
        return s;
      }
    }

    #endregion

    #region Constructors

    public SkipFrequencyGroupStyle()
    {
    }

    public SkipFrequencyGroupStyle(SkipFrequencyGroupStyle from)
    {
      this._isInitialized = from._isInitialized;
      this._skipFrequency = from._skipFrequency;
    }

    #endregion

    #region ICloneable Members

    public SkipFrequencyGroupStyle Clone()
    {
      return new SkipFrequencyGroupStyle(this);
    }

    object ICloneable.Clone()
    {
      return new SkipFrequencyGroupStyle(this);
    }


    #endregion

    #region IGroupStyle Members

    public void TransferFrom(IPlotGroupStyle fromb)
    {
      SkipFrequencyGroupStyle from = (SkipFrequencyGroupStyle)fromb;
      this._isInitialized = from._isInitialized;
      _skipFrequency = from._skipFrequency;
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

    public bool CanHaveChilds()
    {
      return false;
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

    #endregion

    #region Other members

    public bool IsInitialized
    {
      get
      {
        return _isInitialized;
      }
    }
    public void Initialize(int skipFrequency)
    {
      _isInitialized = true;
      _skipFrequency = skipFrequency;
    }
    public int SkipFrequency
    {
      get
      {
        return _skipFrequency;
      }
    }
    #endregion

    #region Static helpers

    public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
    {
      // this group style is local only, so no addition is made here
    }


    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(SkipFrequencyGroupStyle)))
        localGroups.Add(new SkipFrequencyGroupStyle());
    }

    public delegate int Int32FunctionValueGetter();
    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Int32FunctionValueGetter getter)
    {
      if (!externalGroups.ContainsType(typeof(SkipFrequencyGroupStyle))
        && null != localGroups
        && !localGroups.ContainsType(typeof(SkipFrequencyGroupStyle)))
      {
        localGroups.Add(new SkipFrequencyGroupStyle());
      }


      SkipFrequencyGroupStyle grpStyle = null;
      if (externalGroups.ContainsType(typeof(SkipFrequencyGroupStyle)))
        grpStyle = (SkipFrequencyGroupStyle)externalGroups.GetPlotGroupStyle(typeof(SkipFrequencyGroupStyle));
      else if (localGroups != null)
        grpStyle = (SkipFrequencyGroupStyle)localGroups.GetPlotGroupStyle(typeof(SkipFrequencyGroupStyle));

      if (grpStyle != null && getter != null && !grpStyle.IsInitialized)
        grpStyle.Initialize(getter());
    }


    public delegate void Int32ValueSetter(int c);
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
      Int32ValueSetter setter)
    {
      SkipFrequencyGroupStyle grpStyle = null;
      IPlotGroupStyleCollection grpColl = null;
      if (externalGroups.ContainsType(typeof(SkipFrequencyGroupStyle)))
        grpColl = externalGroups;
      else if (localGroups != null && localGroups.ContainsType(typeof(SkipFrequencyGroupStyle)))
        grpColl = localGroups;

      if (null != grpColl)
      {
        grpStyle = (SkipFrequencyGroupStyle)grpColl.GetPlotGroupStyle(typeof(SkipFrequencyGroupStyle));
        grpColl.OnBeforeApplication(typeof(SkipFrequencyGroupStyle));
        setter(grpStyle.SkipFrequency);
        return true;
      }
      else
      {
        return false;
      }


    }




    #endregion
  }
}
