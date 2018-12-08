#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using md = Markdig.Extensions.Tables;

namespace Altaxo.Text.Renderers.OpenXML.Extensions
{
  /// <summary>
  /// Maml renderer for a <see cref="Table"/>.
  /// </summary>
  /// <seealso cref="MamlObjectRenderer{Table}" />
  public class TableRenderer : OpenXMLObjectRenderer<md.Table>
  {
    protected override void Write(OpenXMLRenderer renderer, md.Table mdTable)
    {
      // Create an empty table.
      var table = new Table();

      // Create a TableProperties object and specify its border information.
      var tblProp = new TableProperties(
          new TableBorders(
              new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Dashed), Size = 24 },
              new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Dashed), Size = 24 },
              new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Dashed), Size = 24 },
              new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Dashed), Size = 24 },
              new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Dashed), Size = 24 },
              new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Dashed), Size = 24 }
          )
      );
      // Append the TableProperties object to the empty table.
      table.AppendChild<TableProperties>(tblProp);




      foreach (md.TableRow mdRow in mdTable)
      {

        // Create a row.
        var wRow = new TableRow();

        foreach (var mdCell in mdRow)
        {
          // Create a cell.
          var wTableCell = new TableCell();

          // Specify the width property of the table cell.
          wTableCell.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Auto }));

          // Specify the table cell content.
          wTableCell.Append(renderer.Paragraph = new Paragraph());
          renderer.Run = null;
          renderer.Write(mdCell);

          // Append the table cell to the table row.
          wRow.Append(wTableCell);
        }

        table.Append(wRow);
      }

      renderer._wordDocument.MainDocumentPart.Document.Body.Append(table);
      renderer.Paragraph = new Paragraph();
      renderer.Run = null;

    }
  }
}

