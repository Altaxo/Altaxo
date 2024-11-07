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
using System.IO;
using Xunit;

namespace Altaxo.Serialization.Renishaw
{
  public class WdfFileReaderTest
  {
    public string TestFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Serialization\\Renishaw\\TestFiles");

    public FileStream GetFileStream(string fileName)
    {
      return new FileStream(Path.Combine(TestFilePath, fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public WdfFileReader GetReader(string fileName)
    {
      using var str = GetFileStream(fileName);
      return new WdfFileReader(str);
    }

    [Fact]
    public void Test_AllFilesReadable()
    {
      var speFiles = new DirectoryInfo(TestFilePath).GetFiles("*.wdf");
      Assert.NotEmpty(speFiles);

      foreach (var file in speFiles)
      {
        using var str = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        var reader = new WdfFileReader(str);
      }
    }

    [Fact]
    public void Test_ReadSyncroScan()
    {
      const int ExpectedNumberOfPoints = 4270;
      var r = GetReader("si-synchroscan.wdf");
      Assert.Equal(ExpectedNumberOfPoints, r.NumberOfPointsPerSpectrum);

      // x-axis
      Assert.NotEmpty(r.XData);
      Assert.Equal(ExpectedNumberOfPoints, r.XData.Length);
      Assert.True(r.XData[0] > 0 && r.XData[0] < 5000);
      Assert.True(r.XData[^1] > 0 && r.XData[^1] < 5000);

      // y-axis
      Assert.Equal(1, r.Count);
      Assert.Single(r.Spectra);
      Assert.NotEmpty(r.Spectra[0]);
      Assert.NotEmpty(r.GetSpectrum(0));
      Assert.Equal(ExpectedNumberOfPoints, r.Spectra[0].Length);
      Assert.Equal(ExpectedNumberOfPoints, r.GetSpectrum(0).Length);
    }

    [Fact]
    public void Test_Static()
    {
      const int ExpectedNumberOfPoints = 1015;
      var r = GetReader("si-static.wdf");
      Assert.Equal(ExpectedNumberOfPoints, r.NumberOfPointsPerSpectrum);

      // x-axis
      Assert.NotEmpty(r.XData);
      Assert.Equal(ExpectedNumberOfPoints, r.XData.Length);
      Assert.True(r.XData[0] > 0 && r.XData[0] < 5000);
      Assert.True(r.XData[^1] > 0 && r.XData[^1] < 5000);

      // y-axis
      Assert.Equal(1, r.Count);
      Assert.Single(r.Spectra);
      Assert.NotEmpty(r.Spectra[0]);
      Assert.NotEmpty(r.GetSpectrum(0));
      Assert.Equal(ExpectedNumberOfPoints, r.Spectra[0].Length);
      Assert.Equal(ExpectedNumberOfPoints, r.GetSpectrum(0).Length);
    }


  }
}
