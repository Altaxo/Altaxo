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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Altaxo.Data;
using DocumentFormat.OpenXml.Packaging;

namespace Altaxo.Serialization.OpenXml.Excel
{
  public record ExcelImporter : DataFileImporterBase
  {
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".xlsx"], "Excel files (*.xlsx)");
    }

    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return importOptions as ExcelImportOptions ?? new ExcelImportOptions();
    }

    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new ExcelImportDataSource(fileNames, (ExcelImportOptions)importOptions);
    }


    public override double GetProbabilityForBeingThisFileFormat(string fileName)
    {
      double p = 0;
      var fe = GetFileExtensions();
      if (fe.FileExtensions.ToHashSet().Contains(Path.GetExtension(fileName).ToLowerInvariant()))
      {
        p += 0.5;
      }

      try
      {
        using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var doc = SpreadsheetDocument.Open(stream, false);
        if (doc.DocumentType == DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook)
        {
          p += 0.5;
        }
      }
      catch
      {
        p = 0;
      }

      return p;
    }

    public override string? Import(IReadOnlyList<string> fileNames, DataTable table, object importOptions, bool attachDataSource = true)
    {
      return "The Excel importer is not yet implemented";
    }
  }
}
