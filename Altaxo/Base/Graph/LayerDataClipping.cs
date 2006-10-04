using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{
  public enum LayerDataClipping
  {
    /// <summary>No data clipping.</summary>
    None,

    /// <summary>All plots are strictly clipped to the coordinate system plane.</summary>
    StrictToCS,


    /// <summary>All plot lines are strictly clipped to the coordinate system plane.
    /// The scatter styles can be drawn outside the CS plane as long as the centre of the scatter point is inside the CS plane.</summary>
    LazyToCS
  }

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LayerDataClipping), 0)]
  internal class BrushTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      info.SetNodeContent(obj.ToString());
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      string val = info.GetNodeContent();
      return System.Enum.Parse(typeof(LayerDataClipping), val, true);
    }
  }

}
