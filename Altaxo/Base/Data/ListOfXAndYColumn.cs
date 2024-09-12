#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using Altaxo.Main;

namespace Altaxo.Data
{
  public class ListOfXAndYColumn : Main.SuspendableDocumentNodeWithSetOfEventArgs, IHasDocumentReferences, ICloneable
  {
    /// <summary>
    /// The data of the curves to master. The data belonging to the first master curve are _curves[0][0], _curves[1][0], _curves[2][0], .. _curves[n-1][0].
    /// The data belonging to a second master curve with the same shift factors (if there is any, e.g. the imaginary part), are _curves[0][1], _curves[1][1], .. _curves[n-1][1].
    /// </summary>
    public List<XAndYColumn> CurveData { get; set; } = [];

    #region Serialization

    /// <summary>
    /// V0: 2024-01-24 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ListOfXAndYColumn), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ListOfXAndYColumn)obj;

        info.CreateArray("Curves", s.CurveData.Count);
        {
          foreach (var curve in s.CurveData)
          {
            info.AddValueOrNull("e", curve);
          }
        }
        info.CommitArray(); // Group
      }



      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o as ListOfXAndYColumn ?? new ListOfXAndYColumn();

        int numberOfCurves = info.OpenArray("Curves");
        var curves = new List<XAndYColumn>(numberOfCurves);
        {
          for (int i = 0; i < numberOfCurves; ++i)
          {
            var curve = info.GetValue<XAndYColumn>("e", null);
            if (curve is not null)
              curve.ParentObject = s;
            curves.Add(curve);
          }
        }
        info.CloseArray(numberOfCurves);


        s.CurveData = curves;

        return s;
      }
    }

    #endregion


    public void SetCurveData(DataTable dataTable, IReadOnlyList<DataColumn> dataGroups)
    {
      var curveData = new List<XAndYColumn>();


      var arr = new List<XAndYColumn>();
      for (int i = 0; i < dataGroups.Count; ++i)
      {
        var yCol = dataGroups[i];
        if (yCol is not null)
        {
          var xCol = dataTable.DataColumns.FindXColumnOf(yCol);
          if (xCol is not null)
          {
            arr.Add(new XAndYColumn(dataTable, dataTable.DataColumns.GetColumnGroup(yCol)) { XColumn = xCol, YColumn = yCol });
          }
        }


      }

      CurveData = curveData;
    }

    public void SetCurveData(IEnumerable<(DataColumn x, DataColumn y)> data)
    {
      var curveData = new List<XAndYColumn>();

      foreach (var pair in data)
      {
        var dataTable = DataTable.GetParentDataTableOf(pair.y);
        var columnGroup = dataTable.DataColumns.GetColumnGroup(pair.y);
        curveData.Add(new XAndYColumn(dataTable, columnGroup) { XColumn = pair.x, YColumn = pair.y });
      }
      CurveData = curveData;
    }

    public object Clone()
    {
      var clone = new ListOfXAndYColumn();
      for (int j = 0; j < CurveData.Count; j++)
      {
        clone.CurveData.Add(CurveData[j] is { } curve ? (XAndYColumn)curve.Clone() : null);
      }
      return clone;
    }

    public void VisitDocumentReferences(DocNodeProxyReporter ReportProxies)
    {
      var curves = CurveData;
      for (int j = 0; j < curves.Count; ++j)
      {
        if (curves[j] is { } curve_j)
        {
          curve_j.VisitDocumentReferences(ReportProxies);
        }
      }
    }

    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      var curves = CurveData;
      for (int j = 0; j < curves.Count; ++j)
      {
        if (curves[j] is not null)
        {
          yield return new DocumentNodeAndName(curves[j], () => curves[j] = null, $"Curves[{j}]");
        }
      }
    }
  }
}
