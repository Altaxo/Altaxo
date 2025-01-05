#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using Altaxo.Data;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;

namespace Altaxo.Graph.Plot.Data
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Data.Selections;
  using Gdi.Plot.Data;

  /// <summary>
  /// Summary description for XYColumnPlotData.
  /// </summary>
  public class XYColumnPlotData
    :
    XAndYColumn,
    IColumnPlotData
  {
    /// <summary>This is here only for backward deserialization compatibility. Do not use it.</summary>
    private Altaxo.Data.IReadableColumn? _deprecatedLabelColumn; // the label column

    // cached or temporary data
    protected IPhysicalBoundaries? _xBoundaries;

    protected IPhysicalBoundaries? _yBoundaries;

    /// <summary>List of plot points that is allocated once per thread (as thread local storage variable).</summary>
    [ThreadStatic]
    [NonSerialized]
    protected static List<PointF>? _tlsBufferedPlotData;

    /// <summary>
    /// One more that the index to the last valid pair of plot data.
    /// </summary>
    protected int _pointCount;

    protected bool _isCachedDataValidX = false;
    protected bool _isCachedDataValidY = false;

    #region Serialization

    #region Xml 0 und 1

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 1)] // by mistake the data of version 0 and 1 are identical
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ApplicationException("Calling a deprecated serialization handler for XYColumnPlotData");
        /*
                XYColumnPlotData s = (XYColumnPlotData)obj;

                if(s.m_xColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_xColumn).ParentObject))
                {
                    info.AddValue("XColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_xColumn));
                }
                else
                {
                    info.AddValue("XColumn",s.m_xColumn);
                }

                if(s.m_yColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_yColumn).ParentObject))
                {
                    info.AddValue("YColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_yColumn));
                }
                else
                {
                    info.AddValue("YColumn",s.m_yColumn);
                }

                info.AddValue("XBoundaries",s.m_xBoundaries);
                info.AddValue("YBoundaries",s.m_yBoundaries);
                */
      }

      protected Main.AbsoluteDocumentPath? _xColumn;
      protected Main.AbsoluteDocumentPath? _yColumn;

      protected XYColumnPlotData? _plotAssociation;

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        bool bNeedsCallback = false;
        XYColumnPlotData s = (XYColumnPlotData?)o ?? new XYColumnPlotData(info, -1);

        var xColumn = info.GetValueOrNull("XColumn", s) ?? new IndexerColumn();
        var yColumn = info.GetValueOrNull("YColumn", s) ?? new IndexerColumn();

        if (xColumn is Altaxo.Data.IReadableColumn xReadColumn)
          s.XColumn = xReadColumn;
        else if (xColumn is Main.AbsoluteDocumentPath)
          bNeedsCallback = true;

        if (yColumn is Altaxo.Data.IReadableColumn yReadColumn)
          s.YColumn = yReadColumn;
        else if (yColumn is Main.AbsoluteDocumentPath)
          bNeedsCallback = true;

        s.ChildSetMember(ref s._xBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("XBoundaries", s));
        s.ChildSetMember(ref s._yBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("YBoundaries", s));

        if (bNeedsCallback)
        {
          var surr = new XmlSerializationSurrogate0
          {
            _xColumn = xColumn as Main.AbsoluteDocumentPath,
            _yColumn = yColumn as Main.AbsoluteDocumentPath,
            _plotAssociation = s
          };

          info.DeserializationFinished += surr.EhDeserializationFinished;
        }
        return s;
      }

      private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinallyCall)
      {
        bool bAllResolved = true;

        if (_plotAssociation is not null)
        {
          if (_xColumn is not null)
          {
            var xColumn = Main.AbsoluteDocumentPath.GetObject(_xColumn, _plotAssociation, (Main.IDocumentNode)documentRoot);
            bAllResolved &= (xColumn is not null);
            if (xColumn is Altaxo.Data.IReadableColumn readableColum)
              _plotAssociation.XColumn = readableColum;
          }

          if (_yColumn is not null)
          {
            var yColumn = Main.AbsoluteDocumentPath.GetObject(_yColumn, _plotAssociation, (Main.IDocumentNode)documentRoot);
            bAllResolved &= (yColumn is not null);
            if (yColumn is Altaxo.Data.IReadableColumn readableColum)
              _plotAssociation.YColumn = readableColum;
          }


          if (isFinallyCall && (_plotAssociation._independentVariables[0] is null || _plotAssociation._dependentVariables[0] is null))
          {
            if (_plotAssociation._independentVariables[0] is null)
              _plotAssociation.XColumn = new IndexerColumn();
            if (_plotAssociation._dependentVariables[0] is null)
              _plotAssociation.YColumn = new IndexerColumn();
          }
        }

        if (bAllResolved)
          info.DeserializationFinished -= EhDeserializationFinished;
      }
    }

    #endregion Xml 0 und 1

    #region Xml2

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 2)]
    private class XmlSerializationSurrogate2 : XmlSerializationSurrogate0
    {
      public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ApplicationException("Calling a deprecated serialization handler for XYColumnPlotData");
        /*
                XYColumnPlotData s = (XYColumnPlotData)obj;
                base.Serialize(obj,info);

                // -----------------------Added in version 2 ------------------------

                // the rest of the plot data is stored in kind of a array
                // so it should be easy to add more data here, and only data that are valid
                // are been serialized
                int nElements = s.LabelColumn==null ? 0 : 1;
                info.CreateArray("OptionalData",nElements);
                if(null!=s.LabelColumn)
                {
                    if(s.m_LabelColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_LabelColumn).ParentObject))
                        info.AddValue("LabelColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_LabelColumn));
                    else
                        info.AddValue("LabelColumn",s.m_LabelColumn);
                }
                info.CommitArray(); // end of array OptionalData
            */
      }

      public override object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (XYColumnPlotData?)o ?? new XYColumnPlotData(info, -1);
        base.Deserialize(s, info, parent);

        bool bNeedsCallback = false;

        object? labelColumn = null;

        int nOptionalData = info.OpenArray();
        {
          if (nOptionalData == 1)
          {
            string keystring = info.GetNodeName();
            labelColumn = info.GetValue("LabelColumn", s);

            if (labelColumn is Altaxo.Data.IReadableColumn)
              s._deprecatedLabelColumn = (Altaxo.Data.IReadableColumn)labelColumn;
            else if (labelColumn is Main.AbsoluteDocumentPath)
              bNeedsCallback = true;
          }
        }
        info.CloseArray(nOptionalData);

        if (bNeedsCallback)
        {
          var surr = new XmlSerializationSurrogate2
          {
            _labelColumn = labelColumn as Main.AbsoluteDocumentPath,
            _plotAssociation = s
          };

          info.DeserializationFinished += surr.EhDeserializationFinished2;
        }

        return s;
      }

      private Main.AbsoluteDocumentPath? _labelColumn;

      private void EhDeserializationFinished2(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinallyCall)
      {
        bool bAllResolved = true;

        if (_plotAssociation is not null)
        {
          if (_labelColumn is not null)
          {
            var labelColumn = Main.AbsoluteDocumentPath.GetObject(_labelColumn, _plotAssociation, (Main.IDocumentNode)documentRoot);
            bAllResolved &= (labelColumn is not null);
            if (labelColumn is Altaxo.Data.IReadableColumn)
              _plotAssociation._deprecatedLabelColumn = (Altaxo.Data.IReadableColumn)labelColumn;
          }


          if (isFinallyCall && (_plotAssociation._independentVariables[0] is null || _plotAssociation._dependentVariables[0] is null))
          {
            if (_plotAssociation._independentVariables[0] is null)
              _plotAssociation.XColumn = new IndexerColumn();
            if (_plotAssociation._dependentVariables[0] is null)
              _plotAssociation.YColumn = new IndexerColumn();
          }
        }

        if (bAllResolved)
          info.DeserializationFinished -= EhDeserializationFinished2;
      }
    }

    #endregion Xml2

    #region Xml 3

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version is not allowed");
        /*

                XYColumnPlotData s = (XYColumnPlotData)obj;

                info.AddValue("XColumn", s._xColumn);
                info.AddValue("YColumn", s._yColumn);

                info.AddValue("XBoundaries", s._xBoundaries);
                info.AddValue("YBoundaries", s._yBoundaries);

                */
      }

      public virtual XYColumnPlotData SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (XYColumnPlotData?)o ?? new XYColumnPlotData(info, -1);

        s.ChildSetMember(ref s._independentVariables[0], info.GetValueOrNull<IReadableColumnProxy>("XColumn", s) ?? ReadableColumnProxyForStandaloneColumns.FromColumn(new IndexerColumn()));
        s.ChildSetMember(ref s._dependentVariables[0], info.GetValueOrNull<IReadableColumnProxy>("YColumn", s) ?? ReadableColumnProxyForStandaloneColumns.FromColumn(new IndexerColumn()));

        s.ChildSetMember(ref s._xBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("XBoundaries", s));
        s.ChildSetMember(ref s._yBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("YBoundaries", s));

        return s;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Xml 3

    #region Xml 4 und 5

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 4)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Plot.Data.XYColumnPlotData", 5)]
    private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version is not allowed");

        /*
                XYColumnPlotData s = (XYColumnPlotData)obj;

                info.AddValue("XColumn", s._xColumn);
                info.AddValue("YColumn", s._yColumn);

                info.AddValue("XBoundaries", s._xBoundaries);
                info.AddValue("YBoundaries", s._yBoundaries);

                info.AddValue("RangeStart", s._plotRangeStart);
                info.AddValue("RangeLength", s._plotRangeLength);
                */
      }

      public virtual XYColumnPlotData SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (XYColumnPlotData?)o ?? new XYColumnPlotData(info, -1);

        s.ChildSetMember(ref s._independentVariables[0], info.GetValueOrNull<IReadableColumnProxy>("XColumn", s) ?? ReadableColumnProxyForStandaloneColumns.FromColumn(new IndexerColumn()));
        s.ChildSetMember(ref s._dependentVariables[0], info.GetValueOrNull<IReadableColumnProxy>("YColumn", s) ?? ReadableColumnProxyForStandaloneColumns.FromColumn(new IndexerColumn()));
        s.ChildSetMember(ref s._xBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("XBoundaries", s));
        s.ChildSetMember(ref s._yBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("YBoundaries", s));


        int rangeStart = info.GetInt32("RangeStart");
        int rangeLength = info.GetInt32("RangeLength");

        if (rangeStart < 0 || rangeLength != int.MaxValue)
          s.ChildSetMember(ref s._rangeOfRows, RangeOfRowIndices.FromStartAndCount(rangeStart, rangeLength));

        return s;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }


    }

    #endregion Xml 4 und 5

    #region Xml 6

    /// <summary>
    /// 2016-09-25 Added DataTable and GroupNumber. Changed from RangeStart and RangeLength to RowSelection
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Plot.Data.XYColumnPlotData", 6)]
    private class XmlSerializationSurrogate6 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYColumnPlotData)obj;

        info.AddValueOrNull("DataTable", s._dataTable);
        info.AddValue("GroupNumber", s._groupNumber);
        info.AddValue("RowSelection", s._rangeOfRows);

        info.AddValue("XColumn", s._independentVariables[0]);
        info.AddValue("YColumn", s._dependentVariables[0]);

        info.AddValueOrNull("XBoundaries", s._xBoundaries);
        info.AddValueOrNull("YBoundaries", s._yBoundaries);
      }

      public virtual XYColumnPlotData SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (XYColumnPlotData?)o ?? new XYColumnPlotData(info, -1);

        s.ChildSetMember(ref s._dataTable, info.GetValueOrNull<DataTableProxy>("DataTable", s));

        s._groupNumber = info.GetInt32("GroupNumber");

        s.ChildSetMember(ref s._rangeOfRows, (IRowSelection)info.GetValue("RowSelection", s));
        s.ChildSetMember(ref s._independentVariables[0], (IReadableColumnProxy)info.GetValue("XColumn", s));
        s.ChildSetMember(ref s._dependentVariables[0], (IReadableColumnProxy)info.GetValue("YColumn", s));
        s.ChildSetMember(ref s._xBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("XBoundaries", s));
        s.ChildSetMember(ref s._yBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("YBoundaries", s));

        return s;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Xml 6

    #region Xml 7

    /// <summary>
    /// 2024-12-26 XYColumnPlotData derived from XAndYColumn
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYColumnPlotData), 7)]
    private class XmlSerializationSurrogate7 : IndependentAndDependentColumns.XmlSerializationSurrogate0
    {
      public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYColumnPlotData)obj;
        base.Serialize(s, info);
        info.AddValueOrNull("XBoundaries", s._xBoundaries);
        info.AddValueOrNull("YBoundaries", s._yBoundaries);
      }

      public override object? Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parentobject)
      {
        if (o is XYColumnPlotData s)
          s.DeserializeSurrogate0(info);
        else
          s = new XYColumnPlotData(info, 0);

        s.ChildSetMember(ref s._xBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("XBoundaries", s));
        s.ChildSetMember(ref s._yBoundaries, info.GetValueOrNull<IPhysicalBoundaries>("YBoundaries", s));

        return s;
      }
    }

    #endregion Xml 7

    /// <summary>
    /// Deserialization constructor. Initializes a new instance of the <see cref="XYZColumnPlotData"/> class without any member initialization.
    /// </summary>
    /// <param name="info">The information.</param>
    /// <param name="version">The serialization version. Pass -1 in order to not read any data from the XML stream.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected XYColumnPlotData(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
      : base(info, version)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    #endregion Serialization

    public XYColumnPlotData(Altaxo.Data.DataTable dataTable, int groupNumber, Altaxo.Data.IReadableColumn xColumn, Altaxo.Data.IReadableColumn yColumn)
      : base(dataTable, groupNumber, xColumn, yColumn)
    {
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The object to copy from.</param>
    /// <remarks>Only clones the references to the data columns, not the columns itself.</remarks>
    public XYColumnPlotData(XYColumnPlotData from)
      : base(from)
    {
      // cached or temporary data

      if (from._xBoundaries is not null)
        ChildCopyToMember(ref _xBoundaries, from._xBoundaries);

      if (from._yBoundaries is not null)
        ChildCopyToMember(ref _yBoundaries, from._yBoundaries);

      _pointCount = from._pointCount;
      _isCachedDataValidX = from._isCachedDataValidX;
      _isCachedDataValidY = from._isCachedDataValidY;
    }

    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      foreach (var item in base.GetDocumentNodeChildrenWithName())
      {
        yield return item;
      }

      if (_xBoundaries is not null)
        yield return new Main.DocumentNodeAndName(_xBoundaries, "XBoundaries");

      if (_yBoundaries is not null)
        yield return new Main.DocumentNodeAndName(_yBoundaries, "YBoundaries");
    }

    /// <summary>
    /// Creates a cloned copy of this object.
    /// </summary>
    /// <returns>The cloned copy of this object.</returns>
    /// <remarks>The data columns refered by this object are <b>not</b> cloned, only the reference is cloned here.</remarks>
    public override object Clone()
    {
      return new XYColumnPlotData(this);
    }

    [MaybeNull]
    public DataTable DataTable
    {
      get
      {
        var resultTable = _dataTable?.Document;

        if (resultTable is not null)
          return resultTable;
        IReadableColumnExtensions.GetCommonDataTableAndGroupNumberFromColumns(GetAllColumns(), out var nonUniformTables, out resultTable, out var nonUniformGroup, out var resultGroup);

        if (resultTable is not null)
          DataTable = resultTable;
        if (resultGroup is not null)
          GroupNumber = resultGroup.Value;

        return resultTable;
      }
      [MemberNotNull(nameof(_dataTable))]
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (object.ReferenceEquals(_dataTable?.Document, value))
          return;

        if (ChildSetMember(ref _dataTable, new DataTableProxy(value)))
        {
          EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
        }
      }
    }

    private IEnumerable<IReadableColumn> GetAllColumns()
    {
      if (XColumn is { } xc)
        yield return xc;
      if (YColumn is { } yc)
        yield return yc;
    }

    public int GroupNumber
    {
      get
      {
        return _groupNumber;
      }
      set
      {
        if (!(_groupNumber == value))
        {
          _groupNumber = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// The selection of data rows to be plotted.
    /// </summary>
    public IRowSelection DataRowSelection
    {
      get
      {
        return _rangeOfRows;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (!_rangeOfRows.Equals(value))
        {
          ChildSetMember(ref _rangeOfRows, value);
          _isCachedDataValidX = _isCachedDataValidY = false;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      if (_xBoundaries is null || pb.GetType() != _xBoundaries.GetType())
        SetXBoundsFromTemplate(pb);

      if (!_isCachedDataValidX)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CalculateCachedData();
        }
      }
      pb.Add(_xBoundaries);
    }

    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      if (_yBoundaries is null || pb.GetType() != _yBoundaries.GetType())
        SetYBoundsFromTemplate(pb);

      if (!_isCachedDataValidY)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CalculateCachedData();
        }
      }
      pb.Add(_yBoundaries);
    }

    /// <summary>
    /// This sets the x boundary object to a object of the same type as val. The inner data of the boundary, if present,
    /// are copied into the new x boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    [MemberNotNull(nameof(_xBoundaries))]
    protected void SetXBoundsFromTemplate(IPhysicalBoundaries val)
    {
      if (_xBoundaries is null || val.GetType() != _xBoundaries.GetType())
      {
        if (ChildCopyToMember(ref _xBoundaries, val))
        {
          _isCachedDataValidX = false;

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// This sets the y boundary object to a object of the same type as val. The inner data of the boundary, if present,
    /// are copied into the new y boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    [MemberNotNull(nameof(_yBoundaries))]
    protected void SetYBoundsFromTemplate(IPhysicalBoundaries val)
    {
      if (_yBoundaries is null || val.GetType() != _yBoundaries.GetType())
      {
        if (ChildCopyToMember(ref _yBoundaries, val))
        {
          _isCachedDataValidY = false;

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
    /// </summary>
    /// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
    /// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
    public override IEnumerable<GroupOfColumnsInformation> GetAdditionallyUsedColumns()
    {
      yield return new GroupOfColumnsInformation("#0: X-Y-Data", GetColumns());
    }

    /// <summary>
    /// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
    /// </summary>
    /// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
    /// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
    private IEnumerable<ColumnInformation> GetColumns()
    {
      yield return new ColumnInformation("X", XColumn, _independentVariables[0]?.DocumentPath()?.LastPartOrDefault, (col, table, group) => { XColumn = col; if (table is not null) { DataTable = table; GroupNumber = group; } });
      yield return new ColumnInformation("Y", YColumn, _dependentVariables[0]?.DocumentPath()?.LastPartOrDefault, (col, table, group) => { YColumn = col; if (table is not null) { DataTable = table; GroupNumber = group; } });
    }

    public override Altaxo.Data.IReadableColumn? XColumn
    {
      get
      {
        return GetIndependentVariable(0);
      }
      set
      {
        SetIndependentVariable(0, value, () =>
        {
          _isCachedDataValidX = _isCachedDataValidY = false; // this influences both x and y boundaries
        });
      }
    }

    public string XColumnName
    {
      get
      {
        return _independentVariables[0]?.DocumentPath()?.LastPartOrDefault ?? string.Empty;
      }
    }

    public override Altaxo.Data.IReadableColumn? YColumn
    {
      get
      {
        return GetDependentVariable(0);
      }
      set
      {
        SetDependentVariable(0, value, () =>
        {
          _isCachedDataValidX = _isCachedDataValidY = false; // this influences both x and y boundaries
        });
      }
    }

    public string YColumnName
    {
      get
      {
        return _dependentVariables[0]?.DocumentPath()?.LastPartOrDefault ?? string.Empty;
      }
    }

    /// <summary>
    /// For compatibility with older deserialization versions. Do not use it!
    /// </summary>
    public Altaxo.Data.IReadableColumn? LabelColumn
    {
      get
      {
        return _deprecatedLabelColumn;
      }
    }

    public void CalculateCachedData(IPhysicalBoundaries xBounds, IPhysicalBoundaries yBounds)
    {
      if (IsDisposeInProgress)
        return;

      if (_xBoundaries is null || (xBounds is not null && _xBoundaries.GetType() != xBounds.GetType()))
      {
        _isCachedDataValidX = false;
        SetXBoundsFromTemplate(xBounds);
      }

      if (_yBoundaries is null || (yBounds is not null && _yBoundaries.GetType() != yBounds.GetType()))
      {
        _isCachedDataValidY = false;
        SetYBoundsFromTemplate(yBounds);
      }

      if (!_isCachedDataValidX || !_isCachedDataValidY)
        CalculateCachedData();
    }

    /// <summary>
    /// Gets the maximum row index that can be deduced from the data columns. The calculation does <b>not</b> include the DataRowSelection.
    /// </summary>
    /// <returns>The maximum row index that can be deduced from the data columns.</returns>
    public int GetMaximumRowIndexFromDataColumns()
    {
      var xColumn = XColumn;
      var yColumn = YColumn;

      int maxRowIndex;

      if (xColumn is null || yColumn is null)
      {
        maxRowIndex = 0;
      }
      else
      {
        maxRowIndex = int.MaxValue;

        if (xColumn.Count.HasValue)
          maxRowIndex = System.Math.Min(maxRowIndex, xColumn.Count.Value);
        if (yColumn.Count.HasValue)
          maxRowIndex = System.Math.Min(maxRowIndex, yColumn.Count.Value);

        // if both columns are indefinite long, we set the length to zero
        if (maxRowIndex == int.MaxValue || maxRowIndex < 0)
          maxRowIndex = 0;
      }

      return maxRowIndex;
    }

    public void CalculateCachedData()
    {
      if (IsDisposeInProgress)
        return;

      // we can calulate the bounds only if they are set before
      if (_xBoundaries is null && _yBoundaries is null)
        return;

      var suspendTokenX = _xBoundaries?.SuspendGetToken();
      var suspendTokenY = _yBoundaries?.SuspendGetToken();

      try
      {
        _xBoundaries?.Reset();
        _yBoundaries?.Reset();

        _pointCount = GetMaximumRowIndexFromDataColumns();

        if (XColumn is { } xColumn && YColumn is { } yColumn && DataTable is { } dataTable)
        {
          foreach (var segment in _rangeOfRows.GetSelectedRowIndexSegmentsFromTo(0, _pointCount, dataTable.DataColumns, _pointCount))
          {
            for (int rowIdx = segment.start; rowIdx < segment.endExclusive; ++rowIdx)
            {
              if (!xColumn.IsElementEmpty(rowIdx) && !yColumn.IsElementEmpty(rowIdx))
              {
                _xBoundaries?.Add(xColumn, rowIdx);
                _yBoundaries?.Add(yColumn, rowIdx);
              }
            }
          }
        }

        // now the cached data are valid
        _isCachedDataValidX = _xBoundaries is not null;
        _isCachedDataValidY = _yBoundaries is not null;

        // now when the cached data are valid, we can reenable the events
      }
      finally
      {
        suspendTokenX?.Resume();
        suspendTokenY?.Resume();
      }
    }

    private class MyPlotData
    {
      private IReadableColumn _xColumn;
      private IReadableColumn _yColumm;

      public MyPlotData(IReadableColumn xc, IReadableColumn yc)
      {
        _xColumn = xc;
        _yColumm = yc;
      }

      public AltaxoVariant GetXPhysical(int originalRowIndex)
      {
        return _xColumn[originalRowIndex];
      }

      public AltaxoVariant GetYPhysical(int originalRowIndex)
      {
        return _yColumm[originalRowIndex];
      }
    }

    /// <summary>
    /// This will create a point list out of the data, which can be used to plot the data. In order to create this list,
    /// the function must have knowledge how to calculate the points out of the data. This will be done
    /// by a function provided by the calling function.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    /// <returns>An array of plot points in layer coordinates.</returns>
    public Processed2DPlotData GetRangesAndPoints(
      Gdi.IPlotArea layer)
    {
      const double MaxRelativeValue = 1E2;

      var xColumn = XColumn ?? EmptyDoubleColumn.Instance;
      var yColumn = YColumn ?? EmptyDoubleColumn.Instance;


      var result = new Processed2DPlotData();
      var myPlotData = new MyPlotData(xColumn, yColumn);
      result.XPhysicalAccessor = new IndexedPhysicalValueAccessor(myPlotData.GetXPhysical);
      result.YPhysicalAccessor = new IndexedPhysicalValueAccessor(myPlotData.GetYPhysical);
      PlotRangeList? rangeList = null;

      // allocate an array PointF to hold the line points
      // _tlsBufferedPlotData is a static buffer that is allocated per thread
      // and thus is only used temporary here in this routine
      if (_tlsBufferedPlotData is null)
        _tlsBufferedPlotData = new List<PointF>();
      else
        _tlsBufferedPlotData.Clear();

      // Fill the array with values
      // only the points where x and y are not NaNs are plotted!

      bool weAreInsideSegment = false;
      int rangeStart = 0;
      int rangeOffset = 0;
      rangeList = new PlotRangeList();
      result.RangeList = rangeList;

      Scale xAxis = layer.XAxis;
      Scale yAxis = layer.YAxis;
      Gdi.G2DCoordinateSystem coordsys = layer.CoordinateSystem;

      int maxRowIndex = GetMaximumRowIndexFromDataColumns();

      int plotArrayIdx = 0;

      if (DataTable is { } dataTable)
      {
        foreach ((int start, int endExclusive) in _rangeOfRows.GetSelectedRowIndexSegmentsFromTo(0, maxRowIndex, dataTable.DataColumns, maxRowIndex))
        {
          for (int dataRowIdx = start; dataRowIdx < endExclusive; ++dataRowIdx)
          {
            if (xColumn.IsElementEmpty(dataRowIdx) || yColumn.IsElementEmpty(dataRowIdx))
            {
              if (weAreInsideSegment)
              {
                weAreInsideSegment = false;
                rangeList.Add(new PlotRange(rangeStart, plotArrayIdx, rangeOffset));
              }
              continue;
            }

            double x_rel, y_rel;
            x_rel = xAxis.PhysicalVariantToNormal(xColumn[dataRowIdx]);
            y_rel = yAxis.PhysicalVariantToNormal(yColumn[dataRowIdx]);

            // chop relative values to an range of about -+ 10^6
            if (x_rel > MaxRelativeValue)
              x_rel = MaxRelativeValue;
            if (x_rel < -MaxRelativeValue)
              x_rel = -MaxRelativeValue;
            if (y_rel > MaxRelativeValue)
              y_rel = MaxRelativeValue;
            if (y_rel < -MaxRelativeValue)
              y_rel = -MaxRelativeValue;

            // after the conversion to relative coordinates it is possible
            // that with the choosen axis the point is undefined
            // (for instance negative values on a logarithmic axis)
            // in this case the returned value is NaN
            if (coordsys.LogicalToLayerCoordinates(new Logical3D(x_rel, y_rel), out var xcoord, out var ycoord))
            {
              if (!weAreInsideSegment)
              {
                weAreInsideSegment = true;
                rangeStart = plotArrayIdx;
                rangeOffset = dataRowIdx - plotArrayIdx;
              }
              _tlsBufferedPlotData.Add(new PointF((float)xcoord, (float)ycoord));
              plotArrayIdx++;
            }
            else
            {
              if (weAreInsideSegment)
              {
                weAreInsideSegment = false;
                rangeList.Add(new PlotRange(rangeStart, plotArrayIdx, rangeOffset));
              }
            }
          } // end for
          if (weAreInsideSegment)
          {
            weAreInsideSegment = false;
            rangeList.Add(new PlotRange(rangeStart, plotArrayIdx, rangeOffset));
          }
        } // end foreach
      }

      result.PlotPointsInAbsoluteLayerCoordinates = _tlsBufferedPlotData.ToArray();

      return result;
    }

    #region Change event handling

    /*
        /// <summary>
        /// Used by the data proxies to report changes
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void EhColumnDataChangedEventHandler(object sender, EventArgs e)
        {
            // !!!todo!!! : special case if only data added to a column should
            // be handeld separately to save computing time
            this._isCachedDataValid = false;

            EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
        }
        */

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (object.ReferenceEquals(sender, _independentVariables[0]) || object.ReferenceEquals(sender, _dependentVariables[0]))
        _isCachedDataValidX = _isCachedDataValidY = false;

      // If it is BoundaryChangedEventArgs, we have to set a flag for which boundary is affected
      var eAsBCEA = e as BoundariesChangedEventArgs;
      if (eAsBCEA is not null)
      {
        if (object.ReferenceEquals(sender, _xBoundaries))
        {
          eAsBCEA.SetXBoundaryChangedFlag();
        }
        else if (object.ReferenceEquals(sender, _yBoundaries))
        {
          eAsBCEA.SetYBoundaryChangedFlag();
        }
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    /*
        /// <summary>
        /// Looks whether one of data data columns have changed their data. If this is the case, we must recalculate the boundaries,
        /// and trigger the boundary changed event if one of the boundaries have changed.
        /// </summary>
        /// <param name="sender">The sender of the event args, usually a child of this object.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data. On return, you can provided transformed event args by this parameter.</param>
        /// <returns>
        /// The return value of the base handling function
        /// </returns>
        protected override bool HandleLowPriorityChildChangeCases(object sender, ref EventArgs e)
        {
            if (object.ReferenceEquals(sender, _xColumn) || object.ReferenceEquals(sender, _yColumn))
                _isCachedDataValid = false;

            if (!_isCachedDataValid)
                CalculateCachedData(); // Calculates cached data -> If boundaries changed, this will trigger a boundary changed event

            return base.HandleLowPriorityChildChangeCases(sender, ref e);
        }
        */

    /// <summary>
    /// Looks whether one of data data columns have changed their data. If this is the case, we must recalculate the boundaries,
    /// and trigger the boundary changed event if one of the boundaries have changed.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data. On return, you can provided transformed event args by this parameter.</param>
    /// <returns>
    /// The return value of the base handling function
    /// </returns>
    protected override void OnChanged(EventArgs e)
    {
      /* 2019-09-12 Outcommented for new data deserialization: the next lines will cause the XColumn and YColumn to be instantiated
       * from the corresponding proxies,
       * this is unwanted, because it will require to load the table. It should happen only if the graph is really needed

            if (!_isCachedDataValidX || !_isCachedDataValidY)
              CalculateCachedData(); // Calculates cached data -> If boundaries changed, this will trigger a boundary changed event

        */

      base.OnChanged(e);
    }

    #endregion Change event handling
  }
}
