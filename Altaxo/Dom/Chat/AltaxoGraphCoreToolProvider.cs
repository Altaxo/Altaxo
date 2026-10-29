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
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.Json;
using Altaxo.Collections;
using ModelContextProtocol.Protocol;

namespace Altaxo.Chat
{
  /// <summary>
  /// Provides the basic tools from the Altaxo graph namespace (2D graphs), like creating graphs, adding data series, and rendering graphs as images.
  /// </summary>
  /// <remarks>Remember: The tool provider is not discovered by reflection, instead each tool provider must be loaded via the .addin file of Altaxo.</remarks>
  public partial class AltaxoGraphCoreToolProvider : AltaxoToolProviderBase
  {
    /// <inheritdoc/>
    public override string Id => "altaxo.graph.core";

    [Description("Renders a graph document of the current project as a PNG image and returns it as image content, " +
                 "so that the caller can look at the graph. The project is not modified."), ReadOnly(true)]
    private CallToolResult RenderGraphAsImage(
      [Description("Name of the graph, including folder if any, e.g. 'Folder1\\Graph1'.")] string graphName,
      [Description("Width of the image in pixels (100..2000). The height follows from the aspect ratio of the graph. Default: 800.")] int widthPixels = 800)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(graphName))
          return ErrorResult("graphName must not be empty.");

        // limit the width to a reasonable range, so that the caller cannot request a huge image that would consume too much memory.
        widthPixels = Math.Max(100, Math.Min(2000, widthPixels));

        // Get the graph
        if (!Current.Project.GraphDocumentCollection.TryGetValue(graphName, out var graph))
          return ErrorResult($"Graph '{graphName}' not found.");

        // Graph size is in points (1/72 inch); choose the dpi so that the bitmap gets the requested width.
        double dpi = widthPixels * 72.0 / graph.Size.X;

        byte[]? png = null;

        using var ms = new MemoryStream();
        Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderAsBitmapToStream(graph, ms, ImageFormat.Png, dpi);
        png = ms.ToArray();

        return new CallToolResult
        {
          Content = new ContentBlock[]
          {
            ImageContentBlock.FromBytes(png, "image/png"),
            new TextContentBlock { Text = $"Graph '{graphName}', {widthPixels} px wide, PNG, {png.Length} bytes." }
          }
        };
      }
      catch (Exception ex)
      {
        return ErrorResult(ex.Message);
      }
    }

    [Description("Creates a new graph that plots one or more Y columns of a data table against an X column " +
             "(line plot, scatter plot, or both) and returns the name of the new graph. " +
             "Side effect: the X column is marked as ColumnKind.X and the Y columns as ColumnKind.V.")]
    [ReadOnly(false)]
    private string CreateXYPlot(
  [Description("Name of the data table, including folder if any.")] string tableName,
  [Description("Name of the column used for the X axis.")] string xColumnName,
  [Description("Names of the columns plotted against X. Each becomes one plot item.")] string[] yColumnNames,
  [Description("'line', 'scatter' or 'line_and_scatter'. Default: 'line'.")] string plotKind = "line",
  [Description("Optional preferred graph name. If empty or already in use, Altaxo chooses a unique name.")] string? graphName = null,
  [Description("Whether the graph window is opened in the GUI. Default: true.")] bool openGraph = true)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(tableName))
          return Error("tableName must not be empty.");
        if (string.IsNullOrWhiteSpace(xColumnName))
          return Error("xColumnName must not be empty.");
        if (yColumnNames == null || yColumnNames.Length == 0)
          return Error("yColumnNames must not be empty.");
        // Get the table
        if (!Current.Project.DataTableCollection.TryGetValue(tableName, out var table))
          return Error($"Table '{tableName}' not found.");

        var xColumn = table.DataColumns.TryGetColumn(xColumnName);
        if (xColumn is null)
          return Error($"X column '{xColumnName}' not found in table '{tableName}'.");
        var yColumns = yColumnNames.Select(name => table.DataColumns.TryGetColumn(name)).Where(c => c is not null).ToArray();
        if (yColumns.Length == 0)
          return Error($"Y column '{yColumnNames[0]}' not found in table '{tableName}'.");

        // test if both columns belong to the same group
        var xGroup = table.DataColumns.GetColumnGroup(xColumn);
        foreach (var yColumn in yColumns)
        {
          if (table.DataColumns.GetColumnGroup(yColumn) != xGroup)
            return Error($"X column '{xColumnName}' and Y column '{yColumn.Name}' do not belong to the same column group in table '{tableName}'.");
        }

        var currentXColumn = table.DataColumns.FindXColumnOfGroup(xGroup);
        var currentKind = table.DataColumns.GetColumnKind(xColumn);

        // now set the x column to kind x
        bool mustSetBackColumnKinds = false;
        if (currentXColumn != xColumn || currentKind != Altaxo.Data.ColumnKind.X)
        {
          table.DataColumns.SetColumnKind(xColumn, Altaxo.Data.ColumnKind.X);
          mustSetBackColumnKinds = true;
        }

        var selectedColumns = new AscendingIntegerCollection();
        foreach (var yColumn in yColumns)
        {
          selectedColumns.Add(table.DataColumns.GetColumnNumber(yColumn));
        }

        // Create the graph
        var (graph, controller) = Altaxo.Worksheet.Commands.PlotCommands.PlotLine(table, selectedColumns, plotKind.Contains("line"), plotKind.Contains("scatter"), graphName, openGraph);

        // now set back the column kinds of the y columns to kind v
        if (mustSetBackColumnKinds)
        {
          table.DataColumns.SetColumnKind(xColumn, currentKind);
          if (currentXColumn is not null)
            table.DataColumns.SetColumnKind(currentXColumn, Altaxo.Data.ColumnKind.X);
        }

        return JsonSerializer.Serialize(new
        {
          Message = $"Graph '{graph.Name}' created with {yColumns.Length} plot items from table '{tableName}'.",
        });
      }
      catch (Exception ex)
      {
        return Error(ex.Message);
      }
    }

    private static CallToolResult ErrorResult(string message) => new CallToolResult
    {
      IsError = true,
      Content = new ContentBlock[] { new TextContentBlock { Text = message } }
    };

  }
}
