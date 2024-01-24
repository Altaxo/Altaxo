#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Linq;
using Altaxo.Data;
using Altaxo.Main;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  public class MasterCurveData : Main.SuspendableDocumentNodeWithSetOfEventArgs, IHasDocumentReferences, ICloneable
  {

    /// <summary>
    /// The data of the curves to master. The data belonging to the first master curve are _curves[0][0], _curves[1][0], _curves[2][0], .. _curves[n-1][0].
    /// The data belonging to a second master curve with the same shift factors (if there is any, e.g. the imaginary part), are _curves[0][1], _curves[1][1], .. _curves[n-1][1].
    /// </summary>
    public List<XAndYColumn?[]> CurveData { get; set; } = [];

    #region Serialization

    /// <summary>
    /// V0: 2024-01-24 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MasterCurveData), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MasterCurveData)obj;

        info.CreateArray("Groups", s.CurveData.Count);
        {
          foreach (var group in s.CurveData)
          {
            info.CreateArray("Group", group.Length);
            {
              foreach (var curve in group)
              {
                info.AddValueOrNull("e", curve);
              }
            }
            info.CommitArray(); // Group
          }
        }
        info.CommitArray(); // Groups
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o as MasterCurveData ?? new MasterCurveData();

        int numberOfGroups = info.OpenArray("Groups");
        var curveData = new List<XAndYColumn?[]>(numberOfGroups);
        {
          int numberOfCurves = info.OpenArray("Group");
          {
            var curves = new XAndYColumn?[numberOfCurves];
            for (int i = 0; i < numberOfCurves; ++i)
            {
              var curve = info.GetValueOrNull<XAndYColumn>("e", null);
              if (curve is not null)
                curve.ParentObject = s;
              curves[i] = curve;
            }
            curveData.Add(curves);
          }
          info.CloseArray(numberOfCurves);
        }
        info.CloseArray(numberOfGroups);

        return s;
      }
    }

    #endregion


    public void SetCurveData(DataTable dataTable, IReadOnlyList<IReadOnlyList<DataColumn?>> dataGroups)
    {
      var maxCount = dataGroups.Max(g => g.Count);

      var curveData = new List<XAndYColumn?[]>();

      foreach (var group in dataGroups)
      {
        var arr = new XAndYColumn?[maxCount];
        for (int i = 0; i < group.Count; ++i)
        {
          var yCol = group[i];
          if (yCol is not null)
          {
            var xCol = dataTable.DataColumns.FindXColumnOf(yCol);
            if (xCol is not null)
            {
              arr[i] = new XAndYColumn(dataTable, dataTable.DataColumns.GetColumnGroup(yCol)) { XColumn = xCol, YColumn = yCol };
            }
          }
        }

        curveData.Add(arr);
      }

      CurveData = curveData;
    }

    public object Clone()
    {
      var clone = new MasterCurveData();
      for (int i = 0; i < CurveData.Count; i++)
      {
        clone.CurveData.Add(new XAndYColumn[CurveData[i].Length]);
        for (int j = 0; j < CurveData[i].Length; j++)
        {
          clone.CurveData[i][j] = CurveData[i][j] is { } curve ? (XAndYColumn)curve.Clone() : null;
        }
      }
      return clone;
    }

    public void VisitDocumentReferences(DocNodeProxyReporter ReportProxies)
    {
      for (int i = 0; i < CurveData.Count; i++)
      {
        var curves = CurveData[i];
        for (int j = 0; j < curves.Length; ++j)
        {
          if (curves[j] is { } curve_j)
          {
            curve_j.VisitDocumentReferences(ReportProxies);
          }
        }
      }
    }

    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      for (int i = 0; i < CurveData.Count; i++)
      {
        var curves = CurveData[i];
        for (int j = 0; j < curves.Length; ++j)
        {
          if (curves[j] is not null)
          {
            yield return new DocumentNodeAndName(curves[j], () => curves[j] = null, $"Curves[{i},{j}]");
          }
        }
      }
    }
  }
}
