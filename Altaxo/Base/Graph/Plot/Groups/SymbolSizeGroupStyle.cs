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
  public class SymbolSizeGroupStyle : IPlotGroupStyle
  {
    bool _isInitialized;
    float _symbolSize;
    readonly static Type MyType = typeof(SymbolSizeGroupStyle);

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SymbolSizeGroupStyle), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SymbolSizeGroupStyle s = (SymbolSizeGroupStyle)obj;
      }


      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        SymbolSizeGroupStyle s = null != o ? (SymbolSizeGroupStyle)o : new SymbolSizeGroupStyle();
        return s;
      }
    }

    #endregion


    #region Constructors

    public SymbolSizeGroupStyle()
    {
    }

    public SymbolSizeGroupStyle(SymbolSizeGroupStyle from)
    {
      this._isInitialized = from._isInitialized;
      this._symbolSize = from._symbolSize;
    }

    #endregion

    #region ICloneable Members

    public SymbolSizeGroupStyle Clone()
    {
      return new SymbolSizeGroupStyle(this);
    }

    object ICloneable.Clone()
    {
      return new SymbolSizeGroupStyle(this);
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
    public void Initialize(float symbolSize)
    {
      _isInitialized = true;
      _symbolSize = symbolSize;
    }
    public float SymbolSize
    {
      get
      {
        return _symbolSize;
      }
    }
    #endregion

    #region Static helpers

    public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
    {
      if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(SymbolSizeGroupStyle)))
      {
        SymbolSizeGroupStyle gstyle = new SymbolSizeGroupStyle();
        gstyle.IsStepEnabled = true;
        externalGroups.Add(gstyle);
      }
    }


    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(SymbolSizeGroupStyle)))
        localGroups.Add(new SymbolSizeGroupStyle());
    }

    public delegate float SymbolSizeGetter();
    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      SymbolSizeGetter getter)
    {
      if (!externalGroups.ContainsType(typeof(SymbolSizeGroupStyle))
        && null != localGroups
        && !localGroups.ContainsType(typeof(SymbolSizeGroupStyle)))
      {
        localGroups.Add(new SymbolSizeGroupStyle());
      }


      SymbolSizeGroupStyle grpStyle = null;
      if (externalGroups.ContainsType(typeof(SymbolSizeGroupStyle)))
        grpStyle = (SymbolSizeGroupStyle)externalGroups.GetPlotGroupStyle(typeof(SymbolSizeGroupStyle));
      else if (localGroups != null)
        grpStyle = (SymbolSizeGroupStyle)localGroups.GetPlotGroupStyle(typeof(SymbolSizeGroupStyle));

      if (grpStyle != null && getter != null && !grpStyle.IsInitialized)
        grpStyle.Initialize(getter());
    }


    public delegate void SymbolSizeSetter(float c);
    public static void ApplyStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      SymbolSizeSetter setter)
    {
      SymbolSizeGroupStyle grpStyle = null;
      IPlotGroupStyleCollection grpColl = null;
      if (externalGroups.ContainsType(typeof(SymbolSizeGroupStyle)))
        grpColl = externalGroups;
      else if (localGroups != null && localGroups.ContainsType(typeof(SymbolSizeGroupStyle)))
        grpColl = localGroups;

      if (null != grpColl)
      {
        grpStyle = (SymbolSizeGroupStyle)grpColl.GetPlotGroupStyle(typeof(SymbolSizeGroupStyle));
        grpColl.OnBeforeApplication(typeof(SymbolSizeGroupStyle));
        setter(grpStyle.SymbolSize);
      }


    }




    #endregion
  }
}
