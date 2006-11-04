using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Graph.PlotGroups;
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
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PlotGroupStyleCollection s = (PlotGroupStyleCollection)obj;

        info.AddValue("TransformingStyle", s._coordinateTransformingStyle);

        int savedStyles = 0; // for test of consistency
        info.CreateArray("Styles", s.Count);

        foreach (System.Type t in s._typeToInstance.Keys)
        {
          if (s._typeToInfo[t].ParentGroupType != null)
            continue;

          info.AddValue("Style", s._typeToInstance[t]);
          info.AddValue("HasChild", null != s._typeToInfo[t].ChildGroupType);
          savedStyles++;

          System.Type childtype = t;
          while (null != (childtype = s._typeToInfo[childtype].ChildGroupType))
          {
            info.AddValue("Style", s._typeToInstance[childtype]);
            info.AddValue("HasChild", null != s._typeToInfo[childtype].ChildGroupType);
            savedStyles++;
          }
        }

        info.CommitArray();

        if (s.Count != savedStyles)
          throw new ApplicationException("Inconsistency in parent-child relationship in this PlotGroupStyleCollection. Please inform the author");

      }


      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        PlotGroupStyleCollection s = null != o ? (PlotGroupStyleCollection)o : new PlotGroupStyleCollection();

        s._coordinateTransformingStyle = (ICoordinateTransformingGroupStyle)info.GetValue("TransformingStyle", s);

        Type parentStyleType = null;
        int count = info.OpenArray();
        for (int i = 0; i < count; i++)
        {
          IPlotGroupStyle style = (IPlotGroupStyle)info.GetValue("Style", s);
          bool hasChild = info.GetBoolean("HasChild");
          s.Add(style, parentStyleType);
          parentStyleType = hasChild ? style.GetType() : null;
        }
        info.CloseArray(count);

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
