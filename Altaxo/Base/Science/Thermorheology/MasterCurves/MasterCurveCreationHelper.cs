using System;
using System.Collections.Generic;
using Altaxo.Data;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  public class MasterCurveCreationHelper
  {
    public static ShiftCurveCollections GetShiftCurveCollections(List<List<DoubleColumn>> data)
    {
      var multipleShiftDataList = new List<ShiftCurveCollection>();
      foreach (var listOfColumns in data)
      {
        var shiftDataList = new List<ShiftCurve>();
        foreach (var yCol in listOfColumns)
        {
          var table = DataColumnCollection.GetParentDataColumnCollectionOf(yCol) ?? throw new InvalidOperationException($"Column {yCol.Name} has no parent data table!");
          var xCol = (DoubleColumn)(table.FindXColumnOf(yCol) ?? throw new InvalidOperationException($"Can't find corresponding x-column for column {yCol.Name}"));
          var len = Math.Min(xCol.Count, yCol.Count);

          var x = new double[len];
          var y = new double[len];

          for (int i = 0; i < len; i++)
          {
            x[i] = xCol[i];
            y[i] = yCol[i];
          }
          var shiftData = new ShiftCurve(x, y);
          shiftDataList.Add(shiftData);
        }
        var shiftDataCollection = new ShiftCurveCollection(shiftDataList);

        multipleShiftDataList.Add(shiftDataCollection);
      }

      var shiftCurveCollections = new ShiftCurveCollections(multipleShiftDataList);
      return shiftCurveCollections;
    }

  }
}
