// Copyright (c) 2016-2017 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System.Windows;
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
        }
    }
}
