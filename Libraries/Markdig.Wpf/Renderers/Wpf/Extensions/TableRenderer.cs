// Copyright (c) 2016-2017 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Markdig.Annotations;
using Markdig.Extensions.Tables;
using Markdig.Wpf;
using WpfTable = System.Windows.Documents.Table;
using WpfTableCell = System.Windows.Documents.TableCell;
using WpfTableColumn = System.Windows.Documents.TableColumn;
using WpfTableRow = System.Windows.Documents.TableRow;
using WpfTableRowGroup = System.Windows.Documents.TableRowGroup;

namespace Markdig.Renderers.Wpf.Extensions
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Wpf.WpfObjectRenderer{Markdig.Extensions.Tables.Table}" />
    /// <remarks>
    /// A flowdocument table does not support auto width's, see <see href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/98348085-a1cb-414f-b082-5a9342ed174c/flowdocument-table-columns-autowidth?forum=wpf"/>.
    /// Thus, the table width must be measured manually.
    /// </remarks>
    public class TableRenderer : WpfObjectRenderer<Table>
    {
        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] Table table)
        {
            var wpfTable = new WpfTable() { Tag = table };
            renderer.Styles.ApplyTableStyle(wpfTable);

            foreach (var tableColumnDefinition in table.ColumnDefinitions)
            {
                wpfTable.Columns.Add(new WpfTableColumn
                {
                    Width = (tableColumnDefinition?.Width ?? 0) != 0 ?
                        new GridLength(tableColumnDefinition.Width, GridUnitType.Star) :
                        GridLength.Auto,
                    Tag = tableColumnDefinition,
                });
            }

            var wpfRowGroup = new WpfTableRowGroup();

            renderer.Push(wpfTable);
            renderer.Push(wpfRowGroup);

            foreach (var rowObj in table)
            {
                var row = (TableRow)rowObj;
                var wpfRow = new WpfTableRow() { Tag = row };

                renderer.Push(wpfRow);

                if (row.IsHeader)
                {
                    renderer.Styles.ApplyTableHeaderStyle(wpfRow);
                }

                for (var i = 0; i < row.Count; i++)
                {
                    var cellObj = row[i];
                    var cell = (TableCell)cellObj;
                    var wpfCell = new WpfTableCell
                    {
                        ColumnSpan = cell.ColumnSpan,
                        RowSpan = cell.RowSpan,
                        Tag = cell,
                    };

                    renderer.Styles.ApplyTableCellStyle(wpfCell);

                    renderer.Push(wpfCell);
                    renderer.Write(cell);
                    renderer.Pop();

                    if (table.ColumnDefinitions != null)
                    {
                        var columnIndex = cell.ColumnIndex < 0 || cell.ColumnIndex >= table.ColumnDefinitions.Count
                            ? i
                            : cell.ColumnIndex;
                        columnIndex = columnIndex >= table.ColumnDefinitions.Count ? table.ColumnDefinitions.Count - 1 : columnIndex;
                        var alignment = table.ColumnDefinitions[columnIndex].Alignment;
                        if (alignment.HasValue)
                        {
                            switch (alignment)
                            {
                                case TableColumnAlign.Center:
                                    wpfCell.TextAlignment = TextAlignment.Center;
                                    break;

                                case TableColumnAlign.Right:
                                    wpfCell.TextAlignment = TextAlignment.Right;
                                    break;

                                case TableColumnAlign.Left:
                                    wpfCell.TextAlignment = TextAlignment.Left;
                                    break;
                            }
                        }
                    }
                }

                renderer.Pop();
            }

            renderer.Pop();
            renderer.Pop();

            // TryOptimizeColumnWidths(wpfTable);
        }

        private void TryOptimizeColumnWidths(WpfTable table)
        {
            const double lineWidthOfTableFrame = 2;

            var numColumns = table.Columns.Count;
            var numRowGroups = table.RowGroups.Count;

            if (1 != numRowGroups)
                return;

            var rows = table.RowGroups[0].Rows;
            var numRows = rows.Count;

            double[] columnWidths = new double[numColumns];
            double totalWidthOfAllColumns = 0;

            double maxPaddingLeft = 0, maxPaddingRight = 0;

            for (int colIdx = 0; colIdx < numColumns; ++colIdx)
            {
                var col = table.Columns[colIdx];
                columnWidths[colIdx] = 0;

                foreach (var row in rows)
                {
                    if (colIdx >= row.Cells.Count) // it seems that table has one more columns defined than there are 'real' columns, thus we have to check it
                        continue;

                    var cell = row.Cells[colIdx];
                    if (cell.RowSpan != 1)
                        continue;

                    if (cell.Blocks.Count != 1)
                        continue;

                    double? width = null;
                    if (cell.Blocks.FirstBlock is System.Windows.Documents.Paragraph block)
                    {
                        width = Measure(block.Inlines);
                    }

                    if (width.HasValue)
                    {
                        columnWidths[colIdx] = Math.Max(columnWidths[colIdx], width.Value);
                    }
                    else
                    {
                        return; // if there is only one row that can not be measured, we can not optimize column widths
                    }

                    maxPaddingLeft = Math.Max(maxPaddingLeft, cell.Padding.Left);
                    maxPaddingRight = Math.Max(maxPaddingRight, cell.Padding.Right);
                }

                totalWidthOfAllColumns += columnWidths[colIdx];
            }

            // it seems that there is one column more defined in the table than that is really there
            if (0 == columnWidths[columnWidths.Length - 1])
                numColumns -= 1;

            totalWidthOfAllColumns += numColumns * (maxPaddingLeft + maxPaddingRight + lineWidthOfTableFrame);

            // now, it seems that we have all column widths at hand
            // but there is one catch: if the sum of column widths (plus some extra for the table frame lines) exceed
            // the current width of the flow document, then we can not optimize the table widths

            for (int colIdx = 0; colIdx < numColumns; ++colIdx)
            {
                table.Columns[colIdx].Width = new GridLength(columnWidths[colIdx] + maxPaddingLeft + maxPaddingRight + lineWidthOfTableFrame);
            }
        }

        private double? Measure(System.Windows.Documents.InlineCollection inlines)
        {
            double totalWidth = 0;
            foreach (var inline in inlines)
            {
                var width = Measure(inline);

                if (width.HasValue)
                {
                    totalWidth = Math.Max(totalWidth, width.Value);
                }
                else
                {
                    return null;
                }
            }
            return totalWidth;
        }

        private double? Measure(System.Windows.Documents.Inline inline)
        {
            if (inline is System.Windows.Documents.Run run)
            {
                var ft = new FormattedText(run.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(run.FontFamily, run.FontStyle, run.FontWeight, run.FontStretch), run.FontSize, Brushes.Black);
                return ft.Width;
            }
            else if (inline is System.Windows.Documents.Span span)
            {
                return Measure(span.Inlines);
            }

            return null;
        }
    }
}
