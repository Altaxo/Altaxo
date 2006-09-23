using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Graph.PlotGroups;
namespace Altaxo.Graph.G2D.Plot.Groups
{
  public class PlotGroupStyleCollection
    :
    IEnumerable<IPlotGroupStyle>,
    ICloneable
  {
    #region Internal Class
    private class GroupInfo : ICloneable
    {
      public bool WasApplied;
      public System.Type ChildGroupType;
      public System.Type ParentGroupType;

      public GroupInfo()
      {
      }

      public GroupInfo(GroupInfo from)
      {
        this.WasApplied = false;
        this.ChildGroupType = from.ChildGroupType;
      }

      #region ICloneable Members

      object ICloneable.Clone()
      {
        return new GroupInfo(this);
      }
      public GroupInfo Clone()
      {
        return new GroupInfo(this);
      }

      #endregion
    }
    #endregion

    ICoordinateTransformingGroupStyle _coordinateTransformingStyle;
    Dictionary<System.Type, IPlotGroupStyle> _typeToInstance;
    Dictionary<System.Type, GroupInfo> _typeToInfo;

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
      _typeToInstance = new Dictionary<Type, IPlotGroupStyle>();
      _typeToInfo = new Dictionary<Type, GroupInfo>();
    }

    public PlotGroupStyleCollection(PlotGroupStyleCollection from)
    {
      _coordinateTransformingStyle = null == from._coordinateTransformingStyle ? null : (ICoordinateTransformingGroupStyle)from._coordinateTransformingStyle.Clone();
      _typeToInstance = new Dictionary<Type, IPlotGroupStyle>();
      _typeToInfo = new Dictionary<Type, GroupInfo>();

      foreach (KeyValuePair<System.Type, IPlotGroupStyle> entry in from._typeToInstance)
        this._typeToInstance.Add(entry.Key, (IPlotGroupStyle)entry.Value.Clone());

      foreach (KeyValuePair<System.Type, GroupInfo> entry in from._typeToInfo)
        this._typeToInfo.Add(entry.Key, entry.Value.Clone());

    }
    #endregion

    #region ICloneable Members

    public PlotGroupStyleCollection Clone()
    {
      return new PlotGroupStyleCollection(this);
    }

    object ICloneable.Clone()
    {
      return new PlotGroupStyleCollection(this);
    }

    #endregion

    public bool ContainsType(System.Type groupStyleType)
    {
      return _typeToInstance.ContainsKey(groupStyleType);
    }
    public IPlotGroupStyle GetPlotGroupStyle(System.Type groupStyleType)
    {
      return _typeToInstance[groupStyleType];
    }

    public System.Type GetChildTypeOf(System.Type groupStyleType)
    {
      return _typeToInfo[groupStyleType].ChildGroupType;
    }

    public int Count
    {
      get
      {
        return _typeToInstance.Count;
      }
    }

    public void Add(IPlotGroupStyle groupStyle)
    {
      Add(groupStyle, null);
    }


    public void Add(IPlotGroupStyle groupStyle, System.Type parentGroupStyleType)
    {
      if (groupStyle == null)
        throw new ArgumentNullException("Try to add a null value to this group style collection");

      if (_typeToInstance.ContainsKey(groupStyle.GetType()))
        throw new ArgumentException(string.Format("The group style type <<{0}>> is already present in this group style collection", groupStyle.GetType()));

      if ((groupStyle is ICoordinateTransformingGroupStyle) && _coordinateTransformingStyle != null)
        throw new ArgumentException("A coordinate transforming group style type is already present in this group style collection");

      if (parentGroupStyleType != null && !_typeToInstance.ContainsKey(parentGroupStyleType))
        throw new ArgumentException(string.Format("The parent group style (of type: {0}) can not be found in this collection", parentGroupStyleType));

      _typeToInstance.Add(groupStyle.GetType(), groupStyle);
      GroupInfo groupInfo = new GroupInfo();
      groupInfo.ParentGroupType = parentGroupStyleType;
      _typeToInfo.Add(groupStyle.GetType(), groupInfo);

      if (groupStyle is ICoordinateTransformingGroupStyle)
        _coordinateTransformingStyle = (ICoordinateTransformingGroupStyle)groupStyle;

      if (parentGroupStyleType != null)
      {
        System.Type oldChildType = _typeToInfo[parentGroupStyleType].ChildGroupType;
        _typeToInfo[parentGroupStyleType].ChildGroupType = groupStyle.GetType();
        groupInfo.ChildGroupType = oldChildType;
        if (oldChildType != null)
          _typeToInfo[oldChildType].ParentGroupType = groupStyle.GetType();
      }
    }

    public void RemoveType(System.Type groupType)
    {
      if (!_typeToInstance.ContainsKey(groupType))
        return;

      GroupInfo groupInfo = _typeToInfo[groupType];
      System.Type parentGroupType = groupInfo.ParentGroupType;
      System.Type childGroupType = groupInfo.ChildGroupType;

      if (parentGroupType != null) // then append my current child directly to the parent
      {
        GroupInfo parentGroupInfo = _typeToInfo[parentGroupType];
        parentGroupInfo.ChildGroupType = groupInfo.ChildGroupType;
        if (parentGroupInfo.ChildGroupType != null)
          _typeToInfo[parentGroupInfo.ChildGroupType].ParentGroupType = groupInfo.ParentGroupType;
      }
      else if (childGroupType != null) // then set the parent of the child to null
      {
        _typeToInfo[childGroupType].ParentGroupType = null;
      }

      _typeToInstance.Remove(groupType);
      _typeToInfo.Remove(groupType);


    }

    public void Clear()
    {
      _typeToInfo.Clear();
      _typeToInstance.Clear();
      _coordinateTransformingStyle = null;
    }

    public void BeginPrepare()
    {
      foreach (IPlotGroupStyle pgs in this)
        pgs.BeginPrepare();
    }
    public void EndPrepare()
    {
      foreach (IPlotGroupStyle pgs in this)
        pgs.EndPrepare();
    }
    public void PrepareStep()
    {
      foreach (IPlotGroupStyle pgs in this)
        pgs.PrepareStep();
    }

    public void BeginApply()
    {
      foreach (GroupInfo info in _typeToInfo.Values)
        info.WasApplied = false;
    }

    public void EndApply()
    {
      foreach (GroupInfo info in _typeToInfo.Values)
        info.WasApplied = false;
    }

    public void OnBeforeApplication(System.Type groupStyleType)
    {
      _typeToInfo[groupStyleType].WasApplied = true;
    }

    public void Step(int step)
    {
      foreach (KeyValuePair<Type, IPlotGroupStyle> entry in _typeToInstance)
      {
        Type groupType = entry.Key;
        IPlotGroupStyle groupStyle = entry.Value;
        GroupInfo groupInfo = _typeToInfo[groupType];

        if (groupInfo.ParentGroupType == null
          && groupInfo.WasApplied
          && groupStyle.IsStepEnabled
          )
        {
          step = groupStyle.Step(step);
          Type subGroupType = groupType;
          for (groupType = groupInfo.ChildGroupType; groupType != null && step != 0; groupType = groupInfo.ChildGroupType)
          {
            groupInfo = _typeToInfo[groupType];
            groupStyle = _typeToInstance[groupType];
            step = groupStyle.IsStepEnabled ? groupStyle.Step(step) : 0;
          }
        }
        groupInfo.WasApplied = false;
      }
    }

    public ICoordinateTransformingGroupStyle GetCoordinateTransformingStyle()
    {
      return _coordinateTransformingStyle;
    }



    #region IEnumerable<IPlotGroup> Members

    public IEnumerator<IPlotGroupStyle> GetEnumerator()
    {
      return _typeToInstance.Values.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _typeToInstance.Values.GetEnumerator();
    }

    #endregion


  }
}
