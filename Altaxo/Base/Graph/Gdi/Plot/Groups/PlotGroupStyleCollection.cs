using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Graph.Plot.Groups;
namespace Altaxo.Graph.Gdi.Plot.Groups
{
  public class PlotGroupStyleCollection 
    :
    PlotGroupStyleCollectionBase,
    ICloneable // is already implemented in base but is hidden because of inheritance
  {
    ICoordinateTransformingGroupStyle _coordinateTransformingStyle;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotGroupStyleCollection), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PlotGroupStyleCollection s = (PlotGroupStyleCollection)obj;
        info.AddBaseValueEmbedded(obj, obj.GetType().BaseType);

        info.AddValue("TransformingStyle", s._coordinateTransformingStyle);

      }


      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        PlotGroupStyleCollection s = null != o ? (PlotGroupStyleCollection)o : new PlotGroupStyleCollection();

        info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);

        s._coordinateTransformingStyle = (ICoordinateTransformingGroupStyle)info.GetValue("TransformingStyle", s);

        return s;
      }
    }

    #endregion

    #region Constructors
    public PlotGroupStyleCollection()
    {
    }

    public PlotGroupStyleCollection(PlotGroupStyleCollection from)
    {
      CopyFrom(from);
    }

    public override void CopyFrom(PlotGroupStyleCollectionBase fromb)
    {
      base.CopyFrom(fromb);

      if (fromb is PlotGroupStyleCollection)
      {
        PlotGroupStyleCollection from = (PlotGroupStyleCollection)fromb;

        _coordinateTransformingStyle = null == from._coordinateTransformingStyle ? null : (ICoordinateTransformingGroupStyle)from._coordinateTransformingStyle.Clone();

      }
    }

    #endregion

    #region ICloneable Members

    public new PlotGroupStyleCollection Clone()
    {
      return new PlotGroupStyleCollection(this);
    }

    object ICloneable.Clone()
    {
      return new PlotGroupStyleCollection(this);
    }

    #endregion

    public override void Clear()
    {
      base.Clear();
      _coordinateTransformingStyle = null;
    }

    public ICoordinateTransformingGroupStyle CoordinateTransformingStyle
    {
      get
      {
        return _coordinateTransformingStyle;
      }
      set
      {
        _coordinateTransformingStyle = value;
      }
    }



   

   

  }
}
