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
    public List<XAndYColumn?[]> CurveData { get; } = [];

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
