using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Graph.PlotGroups;
namespace Altaxo.Graph.Gdi.Plot.Groups
{
  public class G2DPlotGroupStyleCollection 
    :
    PlotGroupStyleCollection,
    ICloneable // is already implemented in base but is hidden because of inheritance
  {
    IG2DCoordinateTransformingGroupStyle _coordinateTransformingStyle;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DPlotGroupStyleCollection), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        G2DPlotGroupStyleCollection s = (G2DPlotGroupStyleCollection)obj;

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

        G2DPlotGroupStyleCollection s = null != o ? (G2DPlotGroupStyleCollection)o : new G2DPlotGroupStyleCollection();

        s._coordinateTransformingStyle = (IG2DCoordinateTransformingGroupStyle)info.GetValue("TransformingStyle", s);

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
    public G2DPlotGroupStyleCollection()
    {
    }

    public G2DPlotGroupStyleCollection(G2DPlotGroupStyleCollection from)
    {
      CopyFrom(from);
    }

    public override void CopyFrom(PlotGroupStyleCollection fromb)
    {
      base.CopyFrom(fromb);

      if (fromb is G2DPlotGroupStyleCollection)
      {
        G2DPlotGroupStyleCollection from = (G2DPlotGroupStyleCollection)fromb;

        _coordinateTransformingStyle = null == from._coordinateTransformingStyle ? null : (IG2DCoordinateTransformingGroupStyle)from._coordinateTransformingStyle.Clone();

      }
    }

    #endregion

    #region ICloneable Members

    public new G2DPlotGroupStyleCollection Clone()
    {
      return new G2DPlotGroupStyleCollection(this);
    }

    object ICloneable.Clone()
    {
      return new G2DPlotGroupStyleCollection(this);
    }

    #endregion





    public override void Clear()
    {
      base.Clear();
      _coordinateTransformingStyle = null;
    }

   

    public IG2DCoordinateTransformingGroupStyle CoordinateTransformingStyle
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
