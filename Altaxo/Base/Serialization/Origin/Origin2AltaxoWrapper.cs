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
using System.Text;
using Altaxo.Collections;
using Altaxo.Data;

namespace Altaxo.Serialization.Origin
{
  /// <summary>
  /// Provides methods to translate the parsed Origin .opj files to Altaxo's infrastructure.
  /// </summary>
  public class Origin2AltaxoWrapper
  {
    public OriginAnyParser Parser { get; }

    public string FileName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Origin2AltaxoWrapper"/> class.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="fileName">The file name (only to track the origin of the opj project)</param>
    /// <exception cref="Markdig.Helpers.ThrowHelper.ArgumentNullException(System.String)">parser</exception>
    public Origin2AltaxoWrapper(OriginAnyParser parser, string fileName)
    {
      Parser = parser ?? throw new ArgumentNullException(nameof(parser));
      FileName = fileName ?? string.Empty;
    }


    /// <summary>
    /// Enumerates all spread sheets together with their full name.
    /// </summary>
    /// <returns>Enumeration of tuples of full name of the spreadsheet and the spreadsheet.</returns>
    public IEnumerable<(string fullName, SpreadSheet spreadsheet)> EnumerateAllSpreadSheets(bool considerSpreadsheetsInExcelsToo)
    {
      for (int i = 0; i < Parser.SpreadSheets.Count; i++)
      {
        var spreadsheet = Parser.SpreadSheets[i];
        yield return (GetFullNameOfOriginObject(spreadsheet, spreadsheet.Name), spreadsheet);
      }
      if (considerSpreadsheetsInExcelsToo)
      {
        foreach (var entry in EnumerateAllSpreadsheetsInExcels())
        {
          yield return (entry.fullName, entry.spreadSheet);
        }
      }
    }

    /// <summary>
    /// Enumerates all matrices together with their full name.
    /// </summary>
    /// <returns>Enumeration of tuples of full name of the matrix and the matrix.</returns>
    public IEnumerable<(string fullName, Matrix)> EnumerateAllMatrices()
    {
      for (int i = 0; i < Parser.Matrixes.Count; i++)
      {
        var matrix = Parser.Matrixes[i];
        yield return (GetFullNameOfOriginObject(matrix, matrix.Name), matrix);
      }
    }

    /// <summary>
    /// Enumerates all matrix sheets located in the matrices together with their full name.
    /// </summary>
    /// <returns>Enumeration of triples of full name of the matrix sheet, the matrix sheet, and the parent matrix.</returns>
    public IEnumerable<(string fullName, MatrixSheet matrixSheet, Matrix matrix)> EnumerateAllMatrixSheets()
    {
      for (int i = 0; i < Parser.Matrixes.Count; i++)
      {
        var matrix = Parser.Matrixes[i];

        for (int j = 0; j < matrix.Sheets.Count; j++)
        {
          var matrixSheet = matrix.Sheets[j];
          yield return (GetFullNameOfOriginObject(matrix, matrix.Name) + "\\" + matrixSheet.Name, matrixSheet, matrix);
        }
      }
    }

    /// <summary>
    /// Enumerates all excels together with their full name.
    /// </summary>
    /// <returns>Enumeration of tuples of full name of the excel and the excel.</returns>
    public IEnumerable<(string fullName, Excel)> EnumerateAllExcels()
    {
      for (int i = 0; i < Parser.Excels.Count; i++)
      {
        var excel = Parser.Excels[i];
        yield return (GetFullNameOfOriginObject(excel, excel.Name), excel);
      }
    }

    /// <summary>
    /// Enumerates all spreadsheets located in excels together with their full name.
    /// </summary>
    /// <returns>Enumeration of triples of full name of the spreadsheet, the spreadsheet, and the parent excel.</returns>
    public IEnumerable<(string fullName, SpreadSheet spreadSheet, Excel excel)> EnumerateAllSpreadsheetsInExcels()
    {
      for (int i = 0; i < Parser.Excels.Count; i++)
      {
        var excel = Parser.Excels[i];
        for (int j = 0; j < excel.Sheets.Count; j++)
        {
          var spreadsheet = excel.Sheets[j];
          yield return (GetFullNameOfOriginObject(excel, excel.Name) + "\\" + spreadsheet.Name, spreadsheet, excel);
        }
      }
    }

    /// <summary>
    /// Gets the full name of an origin object (any object that could be part of the project tree).
    /// </summary>
    /// <param name="originObject">The origin object.</param>
    /// <param name="name">The short name of the object. If there is no project tree, then this name will be returned.</param>
    /// <returns>If the project tree exists and the object is part of that tree, the full name (parts separated by backlashes) is returned.
    /// Otherwise, the name given in the argument is returned.</returns>
    public string GetFullNameOfOriginObject(object originObject, string name)
    {
      if (Parser.ProjectTree is not null)
      {
        var projectNode = Parser.ProjectTree.AnyBetweenHereAndLeaves(n => object.ReferenceEquals(originObject, n.ValueAsObject));
        if (projectNode is not null)
          name = FullPathOfProjectNode(projectNode);
      }
      return name;
    }

    /// <summary>
    /// Gets the full Altaxo path of a Origin project node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>The full Altaxo path. Parts of the part are separated by backlashes. That also means that if the node is a folder node,
    /// then the path ends with a backlash.
    /// </returns>
    public static string FullPathOfProjectNode(ProjectNode node)
    {
      var stb = new StringBuilder(128);
      var current = node;
      while (current.ParentNode is not null)
      {
        if (object.ReferenceEquals(current, node))
          stb.Insert(0, current.Name);
        else
          stb.Insert(0, current.Name + "\\");

        current = current.ParentNode;
      }

      if (node.NodeType == ProjectNodeType.Folder)
      {
        stb.Append('\\');
      }

      return stb.ToString();
    }


    /// <summary>
    /// Translates Origin's column type to the equivalent in Altaxo
    /// </summary>
    /// <param name="ctype">Origin's column type.</param>
    /// <returns>Altaxo's column type.</returns>
    public static ColumnKind OriginColumnTypeToAltaxoColumnType(SpreadColumnType ctype)
    {
      return ctype switch
      {
        SpreadColumnType.X => ColumnKind.X,
        SpreadColumnType.Y => ColumnKind.V,
        SpreadColumnType.Z => ColumnKind.Z,
        SpreadColumnType.XErr => ColumnKind.Err,
        SpreadColumnType.YErr => ColumnKind.Err,
        SpreadColumnType.Label => ColumnKind.Label,
        SpreadColumnType.Ignore => ColumnKind.V,
        SpreadColumnType.Group => ColumnKind.V,
        SpreadColumnType.Subject => ColumnKind.V,
        _ => throw new System.NotImplementedException($"SpreadColumnType {ctype} (0x{(int)ctype:X2}) is not implemented!"),
      };
    }

    /// <summary>
    /// Creates an Altaxo column using a Origin spread column.
    /// </summary>
    /// <param name="spreadColumn">Origin spread column.</param>
    /// <returns>An Altaxo column. The exact type depends on the type of Origin's spread column.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public static (string name, DataColumn dataColumn, ColumnKind kind) OriginColumnToAltaxoColumn(SpreadColumn spreadColumn)
    {
      DataColumn dataColumn;
      switch (spreadColumn.ValueType)
      {
        case ValueType.Day:
        case ValueType.Month:
        case ValueType.Date:
        case ValueType.Time:
          dataColumn = new DateTimeColumn();
          break;
        case ValueType.Numeric:
          dataColumn = new DoubleColumn();
          break;
        case ValueType.Text:
          dataColumn = new TextColumn();
          break;
        case ValueType.TextNumeric:
          dataColumn = new TextColumn();
          break;
        default:
          throw new System.NotImplementedException();
      };

      for (int i = spreadColumn.BeginRow; i < spreadColumn.EndRow; i++)
      {
        var v = spreadColumn.Data[i];
        if (dataColumn is DoubleColumn dc)
        {
          if (v.IsDouble)
          {
            dc[i] = v.AsDouble();
          }
          else
          {
            if (double.TryParse(v.AsString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var vv))
              dc[i] = vv;
          }
        }
        else if (dataColumn is DateTimeColumn dtc)
        {
          if (v.IsDouble)
          {
            switch (spreadColumn.ValueType)
            {
              case ValueType.Day:
                dtc[i] = DateTime.MinValue + TimeSpan.FromDays(v.AsDouble());
                break;
              case ValueType.Month:
                dtc[i] = new DateTime(1, (int)v.AsDouble(), 1);
                break;
              case ValueType.Time:
                dtc[i] = DateTime.MinValue + TimeSpan.FromDays(v.AsDouble());
                break;
              default:
                dtc[i] = OriginAnyParser.DoubleToPosixTime(v.AsDouble());
                break;
            }
          }
          else
          {
            if (DateTime.TryParse(v.AsString(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out var vt))
              dtc[i] = vt;
          }
        }
        else
        {
          if (v.IsDouble)
          {
            dataColumn[i] = v.AsDouble().ToString(System.Globalization.CultureInfo.InvariantCulture);
          }
          else
          {
            dataColumn[i] = v.AsString();
          }
        }
      }
      return (spreadColumn.Name, dataColumn, OriginColumnTypeToAltaxoColumnType(spreadColumn.ColumnType));
    }
  }
}
