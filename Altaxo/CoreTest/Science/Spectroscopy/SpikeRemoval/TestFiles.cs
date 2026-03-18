#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.IO;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Science.Spectroscopy.SpikeRemoval
{
  public class TestFiles
  {
    public static string TestFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Science\Spectroscopy\SpikeRemoval\TestFiles");

    public static FileStream GetFileStream(string fileName)
    {
      return new FileStream(Path.Combine(TestFilePath, fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public static (double[] x, double[] y) ReadTestFileXY(string simpleName)
    {
      var testFile = Path.Combine(TestFilePath, simpleName);

      if (!File.Exists(testFile))
        Assert.Fail($"Test file not found: {testFile}");

      var reader = new StreamReader(testFile);
      string line;
      var xl = new List<double>();
      var yl = new List<double>();
      while ((line = reader.ReadLine()) is not null)
      {
        var parts = line.Split('\t');
        var x = double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
        var y = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
        xl.Add(x);
        yl.Add(y);
      }

      return (xl.ToArray(), yl.ToArray());
    }

    public static (double[] x, Matrix<double> y) ReadTestFileXYMatrix(string simpleName)
    {
      var testFile = Path.Combine(TestFilePath, simpleName);

      if (!File.Exists(testFile))
        Assert.Fail($"Test file not found: {testFile}");

      var reader = new StreamReader(testFile);
      string line;
      var xl = new List<double>();
      var yl = new List<double>[0];
      while ((line = reader.ReadLine()) is not null)
      {
        var parts = line.Split('\t');
        if (yl.Length == 0)
          yl = Enumerable.Range(0, parts.Length - 1).Select((i) => new List<double>()).ToArray();

        xl.Add(double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture));
        for (int i = 0; i < yl.Length; ++i)
          yl[i].Add(double.Parse(parts[i + 1], System.Globalization.CultureInfo.InvariantCulture));
      }

      var ym = CreateMatrix.Dense<double>(yl.Length, xl.Count, (i, j) => yl[i][j]);

      return (xl.ToArray(), ym);
    }

    public static IEnumerable<(string name, int firstPoint, int lastPoint)> GetTestFileInformation()
    {
      yield return ("NoSpikes.txt", 0, -1);
      yield return ("SpikeAt0298.txt", 297, 298);
      yield return ("SpikeAt1322.txt", 1320, 1324);
    }

    public static IEnumerable<(string name, int firstPoint, int lastPoint, double[] x, double[] y)> GetTestFiles()
    {
      foreach (var (name, firstPoint, lastPoint) in GetTestFileInformation())
      {
        var (x, y) = ReadTestFileXY(name);
        yield return (name, firstPoint, lastPoint, x, y);
      }
    }
  }
}
