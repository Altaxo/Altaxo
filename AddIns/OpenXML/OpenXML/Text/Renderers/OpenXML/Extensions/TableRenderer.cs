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

using System.Linq;
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
      var table = renderer.Push(new Table());

      // Create a TableProperties object and specify its border information.
      var tblProp = new TableProperties(
          new TableBorders(
              new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
              new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
              new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
              new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
              new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
              new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
          )
      );
      // Append the TableProperties object to the empty table.
      table.AppendChild<TableProperties>(tblProp);




      foreach (md.TableRow mdRow in mdTable)
      {

        // Create a row.
        var wRow = renderer.Push(new TableRow());

        for (var i = 0; i < mdRow.Count; i++)
        {
          var cellObj = mdRow[i];
          var mdCell = (md.TableCell)cellObj;
          // Create a cell.
          var wTableCell = renderer.Push(new TableCell());
          wTableCell.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Auto })); // Specify the width property of the table cell.

          renderer.Write(mdCell);

          // Apply horizontal alignment
          var horizontalJustification = JustificationValues.Left;

          if (mdTable.ColumnDefinitions is not null)
          {
            var columnIndex = mdCell.ColumnIndex < 0 || mdCell.ColumnIndex >= mdTable.ColumnDefinitions.Count
                ? i
                : mdCell.ColumnIndex;
            columnIndex = columnIndex >= mdTable.ColumnDefinitions.Count ? mdTable.ColumnDefinitions.Count - 1 : columnIndex;
            var alignment = mdTable.ColumnDefinitions[columnIndex].Alignment;
            if (alignment.HasValue)
            {
              switch (alignment)
              {
                case md.TableColumnAlign.Center:
                  horizontalJustification = JustificationValues.Center;
                  break;

                case md.TableColumnAlign.Right:
                  horizontalJustification = JustificationValues.Right;
                  break;

                case md.TableColumnAlign.Left:
                  horizontalJustification = JustificationValues.Left;
                  break;
              }
            }
          }

          if (horizontalJustification != JustificationValues.Left)
          {
            foreach (var paragraph in wTableCell.ChildElements.OfType<Paragraph>())
            {
              var pp = paragraph.ChildElements.OfType<ParagraphProperties>().FirstOrDefault()
                        ?? paragraph.PrependChild(new ParagraphProperties());

              pp.AppendChild(new Justification { Val = horizontalJustification });
            }
          }

          renderer.PopTo(wTableCell);
        }
        renderer.PopTo(wRow);
      }
      renderer.PopTo(table);
    }
  }
}
