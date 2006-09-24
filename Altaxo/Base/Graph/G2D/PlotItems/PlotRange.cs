using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Data
{
  /// <summary>
  /// PlotRange represents a range of plotting points from index 
  /// lowerBound to (upperBound-1)
  /// I use a class instead of a struct because it is intended to use with
  /// <see cref="System.Collections.ArrayList" />.
  /// </summary>
  /// <remarks>For use in a list, the UpperBound property is somewhat useless, since it should be equal
  /// to the LowerBound property of the next item.</remarks>
  [Serializable]
  public class PlotRange
  {
    int m_LowerBound;
    int m_UpperBound;
    int m_OffsetToOriginal;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotRange), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PlotRange s = (PlotRange)obj;
        info.AddValue("LowerBound", s.m_LowerBound);
        info.AddValue("UpperBound", s.m_UpperBound);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        PlotRange s = null != o ? (PlotRange)o : new PlotRange(0, 0);

        s.m_LowerBound = info.GetInt32("LowerBound");
        s.m_UpperBound = info.GetInt32("UpperBound");

        return s;
      }
    }
    #endregion



    public PlotRange(int lower, int upper)
    {
      m_LowerBound = lower;
      m_UpperBound = upper;
      m_OffsetToOriginal = 0;
    }

    public PlotRange(int lower, int upper, int offset)
    {
      m_LowerBound = lower;
      m_UpperBound = upper;
      m_OffsetToOriginal = offset;
    }

    public PlotRange(PlotRange a)
    {
      m_LowerBound = a.m_LowerBound;
      m_UpperBound = a.m_UpperBound;
      m_OffsetToOriginal = a.m_OffsetToOriginal;
    }

    /// <summary>
    /// Number of points in this plot range.
    /// </summary>
    public int Length
    {
      get { return m_UpperBound - m_LowerBound; }
    }

    /// <summary>
    /// First index in the plot point array, that appears as a plot range.
    /// </summary>
    public int LowerBound
    {
      get { return m_LowerBound; }
      set { m_LowerBound = value; }
    }

    /// <summary>
    /// One more than the last index in the plot poin array, that appeas as a plot range.
    /// </summary>
    public int UpperBound
    {
      get { return m_UpperBound; }
      set { m_UpperBound = value; }
    }

    /// <summary>
    /// This gives the offset to the original row index.
    /// </summary>
    /// <example>If the LowerBound=4 and UpperBound==6, this means points 4 to 6 in the array of plot point locations (!)
    /// will be a contiguous range of plot points, and for this, they are connected by a line etc.
    /// If OffsetToOriginal is 5, this means the point at index 4 in the plot point location array was created
    /// from row index 4+5=9, i.e. from row index 9 in the DataColumn.</example>
    public int OffsetToOriginal
    {
      get { return m_OffsetToOriginal; }
      set { m_OffsetToOriginal = value; }
    }


    public int OriginalFirstPoint
    {
      get { return m_LowerBound + m_OffsetToOriginal; }
    }
    public int OriginalLastPoint
    {
      get { return m_UpperBound + m_OffsetToOriginal -1; }
    }

  }

  /// <summary>
  /// Holds a list of plot ranges. The list is not sorted automatically, but is assumed to be sorted.
  /// </summary>
  [Serializable]
  public class PlotRangeList 
  {
    List<PlotRange> InnerList = new List<PlotRange>();
    /// <summary>
    /// Getter to a plot range at index i.
    /// </summary>
    public PlotRange this[int i]
    {
      get { return InnerList[i]; }
    }

    public int Count { get { return InnerList.Count; } }

    /// <summary>
    /// Adds a plot range. This plot range should be above the previous added plot range.
    /// </summary>
    /// <param name="a"></param>
    public void Add(PlotRange a)
    {
      // test the argument
      if (a.UpperBound <= a.LowerBound)
        throw new ArgumentException("UpperBound is <= LowerBound");
      if (Count == 0 && a.LowerBound != 0)
        throw new ArgumentException("First item must have a LowerBound of 0");
      if (Count != 0 && a.LowerBound != this[Count - 1].UpperBound)
        throw new ArgumentException("This item must have a LowerBound equal to the UpperBound of the previous item");

      InnerList.Add(a);
    }


    /// <summary>
    /// This will get the row index into the data row belonging to a given plot index.
    /// </summary>
    /// <param name="idx">Index of point in the plot array.</param>
    /// <returns>Index into the original data.</returns>
    /// <remarks>Returns -1 if the point is not found.</remarks>
    public int GetRowIndexForPlotIndex(int idx)
    {
      for (int i = 0; i < Count; i++)
      {
        if (this[i].LowerBound <= idx && idx < this[i].UpperBound)
          return idx + this[i].OffsetToOriginal;
      }
      return -1;
    }

    /// <summary>
    /// Returns the total number of plot points.
    /// </summary>
    public int PlotPointCount
    {
      get
      {
        return Count == 0 ? 0 : InnerList[InnerList.Count - 1].UpperBound;
      }
    }

    /// <summary>
    /// By enumerating throw this, you will get the original row indices (indices into the DataColumns).
    /// The number of items returned is equal to <see cref="PlotPointCount" />.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<int> OriginalRowIndices()
    {
      for (int i = 0; i < InnerList.Count; i++)
      {
        PlotRange r = InnerList[i];
        for(int j = r.LowerBound;j<r.UpperBound;j++)
          yield return j+r.OffsetToOriginal;
      }
    }
  }

}
