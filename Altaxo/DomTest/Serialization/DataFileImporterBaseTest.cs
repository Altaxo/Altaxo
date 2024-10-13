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
using System.IO;
using System.Linq;
using Altaxo.Serialization.PrincetonInstruments;
using Xunit;

namespace Altaxo.Serialization
{
  public class DataFileImporterBaseTest
  {
    public DirectoryInfo TestFileBase
    {
      get
      {
        var appDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        var dir = appDir;
        do
        {
          dir = dir.Parent;
        } while (dir.Name != "Altaxo");
        return dir;
      }
    }

    public IEnumerable<FileInfo> GetTestFilesWithExtension(string extension)
    {
      var dir = TestFileBase;
      var testFileDirs = dir.GetDirectories("TestFiles", SearchOption.AllDirectories);
      var dict = new Dictionary<(string Name, long length), FileInfo>();
      foreach (var testFileDir in testFileDirs)
      {
        var files = testFileDir.GetFiles("*" + extension);
        foreach (var file in files)
          dict[(file.Name, file.Length)] = file;
      }

      return dict.Values;
    }


    [Fact]
    public void TestProbabilitySPE()
    {
      var dummy = new Altaxo.Serialization.HDF5.Chada.ChadaImporter(); // make sure that AltaxoDom was loaded
      var importers = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IDataFileImporter))
                      .Select(t => (IDataFileImporter)Activator.CreateInstance(t)).ToArray();

      var files = GetTestFilesWithExtension("*.spe").ToList();

      Assert.NotEmpty(files);
      Assert.NotEmpty(importers);

      foreach (var importer in importers)
      {
        foreach (var file in files)
        {
          var p = importer.GetProbabilityForBeingThisFileFormat(file.FullName);

          var expectedP = importer is PrincetonInstrumentsSPEImporter ? 1.0 : 0.0;
          Assert.Equal(expectedP, p);
        }
      }
    }

    /// <summary>
    /// Tests for all Princeton Instruments SPE files, that the PrincetonInstrumentsSPEImporter is choosen.
    /// </summary>
    [Fact]
    public void TestChoiceSPE()
    {
      var dummy = new Altaxo.Serialization.HDF5.Chada.ChadaImporter(); // make sure that AltaxoDom was loaded
      var importers = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IDataFileImporter))
                      .Select(t => (IDataFileImporter)(Activator.CreateInstance(t)))
                      .ToList(); ;

      var files = GetTestFilesWithExtension("*.spe").ToList();

      Assert.NotEmpty(files);
      Assert.NotEmpty(importers);

      foreach (var file in files)
      {
        var importer = DataFileImporterBase.GetDataFileImporterForFile(file.FullName, importers);
        Assert.True(importer is PrincetonInstrumentsSPEImporter);
      }
    }
  }
}
