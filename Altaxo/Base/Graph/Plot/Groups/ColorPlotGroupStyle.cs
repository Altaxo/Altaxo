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

  public class ColorGroupStyle : IPlotGroupStyle
  {
    bool _isInitialized;
    PlotColor _color;
    bool _isStepEnabled = true;


    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorGroupStyle), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ColorGroupStyle s = (ColorGroupStyle)obj;
        info.AddValue("StepEnabled", s._isStepEnabled);
      }


      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ColorGroupStyle s = null != o ? (ColorGroupStyle)o : new ColorGroupStyle();
        s._isStepEnabled = info.GetBoolean("StepEnabled");
        return s;
      }
    }

    #endregion

    #region Constructors

    public ColorGroupStyle()
    {
      _isStepEnabled = true;
    }

    public ColorGroupStyle(ColorGroupStyle from)
    {
      this._isStepEnabled = from._isStepEnabled;
      this._isInitialized = from._isInitialized;
      this._color = from._color;
    }

    #endregion

    #region ICloneable Members

    public ColorGroupStyle Clone()
    {
      return new ColorGroupStyle(this);
    }

    object ICloneable.Clone()
    {
      return new ColorGroupStyle(this);
    }


    #endregion

    #region IGroupStyle Members

    public void TransferFrom(IPlotGroupStyle fromb)
    {
      ColorGroupStyle from = (ColorGroupStyle)fromb;
      System.Diagnostics.Debug.WriteLine(string.Format("ColorTransfer: myIni={0}, myCol={1}, fromI={2}, fromC={3}", _isInitialized, (_color==null?null:_color.Color.ToString()), from._isInitialized, (from._color==null?null:from._color.Color.ToString())));
      this._isInitialized = from._isInitialized;
      this._color = from._color;
    }

    public void BeginPrepare()
    {
      _isInitialized = false;
      System.Diagnostics.Debug.WriteLine(string.Format("ColorGroupStyle.BeginPrepare"));
    }

    public void PrepareStep()
    {
    }

    public void EndPrepare()
    {
      System.Diagnostics.Debug.WriteLine(string.Format("ColorGroupStyle.EndPrepare, ini={0}, col={1}",_isInitialized,(_color==null ? null:_color.Color.ToString())));
    }

   


    public bool CanHaveChilds()
    {
      return true;
    }

    public int Step(int step)
    {
      int wraps;
      this._color = PlotColors.Colors.GetNextPlotColor(this._color, step, out wraps);
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
        _isStepEnabled = value;
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
    public void Initialize(PlotColor c)
    {
      _isInitialized = true;
      _color = c;
      //System.Diagnostics.Debug.WriteLine(string.Format("ColorGroup.Initialize, col={0}", _color.Color));
    }
    public PlotColor Color
    {
      get
      {
        if (null == _color) // then it seems that no color provider has given us a color, so we initialize with black
          _color = PlotColors.Colors.GetPlotColor(System.Drawing.Color.Black);
        return _color;
      }
    }
    #endregion

    #region Static helpers


    public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
    {
      if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(ColorGroupStyle)))
      {
        ColorGroupStyle gstyle = new ColorGroupStyle();
        gstyle.IsStepEnabled = true;
        externalGroups.Add(gstyle);
      }
    }


    public static void AddLocalGroupStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(ColorGroupStyle)))
        localGroups.Add(new ColorGroupStyle());
    }

    public delegate PlotColor Getter();
    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Getter getter)
    {
      if (!externalGroups.ContainsType(typeof(ColorGroupStyle))
        && null != localGroups
        && !localGroups.ContainsType(typeof(ColorGroupStyle)))
      {
        localGroups.Add(new ColorGroupStyle());
      }

      ColorGroupStyle grpStyle = null;
      if (externalGroups.ContainsType(typeof(ColorGroupStyle)))
        grpStyle = (ColorGroupStyle)externalGroups.GetPlotGroupStyle(typeof(ColorGroupStyle));
      else if (localGroups != null)
        grpStyle = (ColorGroupStyle)localGroups.GetPlotGroupStyle(typeof(ColorGroupStyle));

      if (grpStyle != null && getter != null && !grpStyle.IsInitialized)
        grpStyle.Initialize(getter());
    }

    public delegate void Setter(PlotColor c);
    public static void ApplyStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      Setter setter)
    {
      ColorGroupStyle grpStyle = null;
      IPlotGroupStyleCollection grpColl = null;
      if (externalGroups.ContainsType(typeof(ColorGroupStyle)))
        grpColl = externalGroups;
      else if (localGroups != null && localGroups.ContainsType(typeof(ColorGroupStyle)))
        grpColl = localGroups;

      if (null != grpColl)
      {
        grpStyle = (ColorGroupStyle)grpColl.GetPlotGroupStyle(typeof(ColorGroupStyle));
        grpColl.OnBeforeApplication(typeof(ColorGroupStyle));
        setter(grpStyle.Color);
      }
    }




    #endregion


  }
}
