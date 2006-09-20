using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.PlotGroups
{
  public class ColorGroupStyle : IPlotGroupStyle
  {
    bool _isInitialized;
    PlotColor _color;
    bool _isStepEnabled = true;


    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorGroupStyle), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
    }

    public ColorGroupStyle(ColorGroupStyle from)
    {
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
    }
    public PlotColor Color
    {
      get
      {
        return _color;
      }
    }
    #endregion

    #region Static helpers

    public static void AddLocalGroupStyle(
      PlotGroupStyleCollection externalGroups,
      PlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(ColorGroupStyle)))
        localGroups.Add(new ColorGroupStyle());
    }

    public delegate PlotColor Getter();
    public static void PrepareStyle(
      PlotGroupStyleCollection externalGroups,
      PlotGroupStyleCollection localGroups,
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
      PlotGroupStyleCollection externalGroups,
      PlotGroupStyleCollection localGroups,
      Setter setter)
    {
      ColorGroupStyle grpStyle = null;
      PlotGroupStyleCollection grpColl = null;
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
