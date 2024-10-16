﻿#region Copyright

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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Geometry;

namespace Altaxo.Graph.Graph2D
{
  /// <summary>
  /// Collects options to resize a graph.
  /// </summary>
  /// <seealso cref="System.ICloneable" />
  public class ResizeGraphOptions : ICloneable
  {
    // Old Values

    /// <summary>
    /// Gets or sets the size of the root layer (before resizing).
    /// </summary>
    /// <value>
    /// The old size of the root layer (before resizing).
    /// </value>
    public PointD2D OldRootLayerSize { get; set; }

    /// <summary>
    /// Gets or sets the standard font family (before resizing).
    /// </summary>
    /// <value>
    /// The standard font family (before resizing).
    /// </value>
    public string? OldStandardFontFamily { get; set; }

    /// <summary>
    /// Gets or sets the standard font size (before resizing).
    /// </summary>
    /// <value>
    /// The standard font size  (before resizing).
    /// </value>
    public double? OldStandardFontSize { get; set; }

    /// <summary>
    /// Gets or sets the standard line thickness  (before resizing).
    /// </summary>
    /// <value>
    /// The standard line thickness (before resizing).
    /// </value>
    public double? OldLineThickness { get; set; }

    /// <summary>
    /// Gets or sets the standard length of the major tick (before resizing).
    /// </summary>
    /// <value>
    /// The standard length of the major tick (before resizing).
    /// </value>
    public double? OldMajorTickLength { get; set; }

    /// <summary>
    /// Initializes the values associated with the graph before resizing.
    /// </summary>
    /// <param name="doc">The document.</param>
    public void InitializeOldValues(Altaxo.Graph.Gdi.GraphDocument doc)
    {
      OldRootLayerSize = doc.RootLayer.Size;
      var oldFont = Altaxo.Graph.Gdi.GraphDocument.GetDefaultFont(doc.GetPropertyContext());
      OldStandardFontFamily = oldFont.FontFamilyName;
      OldStandardFontSize = oldFont.Size;
      OldLineThickness = Altaxo.Graph.Gdi.GraphDocument.GetDefaultPenWidth(doc.GetPropertyContext());
      OldMajorTickLength = Altaxo.Graph.Gdi.GraphDocument.GetDefaultMajorTickLength(doc.GetPropertyContext());
    }

    /// <summary>
    /// Gets or sets the new size of the root layer (after resizing).
    /// </summary>
    /// <value>
    /// The new size of the root layer (after resizing).
    /// </value>
    public PointD2D? NewRootLayerSize { get; set; }

    /// <summary>
    /// Gets or sets the new standard font family (after resizing).
    /// </summary>
    /// <value>
    /// The new standard font family (after resizing).
    /// </value>
    public string? NewStandardFontFamily { get; set; }

    /// <summary>
    /// Gets or sets the new standard font size (after resizing).
    /// </summary>
    /// <value>
    /// The new standard font size(after resizing).
    /// </value>
    public double? NewStandardFontSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether all fonts should be reset to the standard font family.
    /// </summary>
    /// <value>
    ///   <c>true</c> if all fonts should be re-set to the standard font family; otherwise, <c>false</c>.
    /// </value>
    public bool OptionResetAllFontsToStandardFont { get; set; }

    /// <summary>
    /// Actions concerning a scalar size value of the graph.
    /// </summary>
    public enum ScalarSizeActions
    {
      /// <summary>No action. The scalar size value is not modified.</summary>
      None,

      /// <summary>The scalar size value is set to its standard value.</summary>
      ResetAllToStandardValue,

      /// <summary>The scalar size value is scaled by the ratio of its new standard value by its old standard value.</summary>
      RescaleByRatioNewToOldValue
    }

    /// <summary>
    /// Gets or sets the action concerning the font size.
    /// </summary>
    /// <value>
    /// The action concerning the font size of the elements of the graph.
    /// </value>
    public ScalarSizeActions ActionForFontSize { get; set; } = ScalarSizeActions.RescaleByRatioNewToOldValue;

    /// <summary>
    /// Gets or sets the action concerning the symbol size.
    /// </summary>
    /// <value>
    /// The action concerning the font size of the elements of the graph.
    /// </value>
    public ScalarSizeActions ActionForSymbolSize { get; set; } = ScalarSizeActions.RescaleByRatioNewToOldValue;

    /// <summary>
    /// Gets or sets the action concerning the line thickness.
    /// </summary>
    /// <value>
    /// The action concerning the line thickness of elements of the graph that have lines.
    /// </value>
    public ScalarSizeActions ActionForLineThickness { get; set; } = ScalarSizeActions.RescaleByRatioNewToOldValue;

    /// <summary>
    /// Gets or sets the user defined line thickness. If this value is set, it will override the standard line thickness that is calculated from the font family and font size.
    /// </summary>
    /// <value>
    /// The user defined line thickness.
    /// </value>
    public double? UserDefinedLineThickness { get; set; }

    /// <summary>
    /// Gets or sets the length of the action concerning the length of scale ticks.
    /// </summary>
    /// <value>
    /// The action concerning the length of scale ticks.
    /// </value>
    public ScalarSizeActions ActionForTickLength { get; set; } = ScalarSizeActions.RescaleByRatioNewToOldValue;

    /// <summary>
    /// Gets or sets the user defined major tick length. If this value is set, it will override the standard major tick length that is calculated from the font family and font size.
    /// </summary>
    /// <value>
    /// The user defined line thickness.
    /// </value>
    public double? UserDefinedMajorTickLength { get; set; }

    public object Clone()
    {
      return MemberwiseClone();
    }

    /// <summary>
    /// Resizes a graph according to the options stored in this instance.
    /// </summary>
    /// <param name="doc">The graph to resize..</param>
    public void ResizeGraph(Altaxo.Graph.Gdi.GraphDocument doc)
    {
      var context = doc.GetPropertyHierarchy();

      var oldFont = context.GetValue(Altaxo.Graph.Gdi.GraphDocument.PropertyKeyDefaultFont) ?? throw new InvalidProgramException(); ;

      // calculate new line thickness
      var oldLineThickness = Altaxo.Graph.Gdi.GraphDocument.GetDefaultPenWidth(doc.GetPropertyHierarchy());
      var oldMajorTickLength = Altaxo.Graph.Gdi.GraphDocument.GetDefaultMajorTickLength(doc.GetPropertyHierarchy());

      var newFont = oldFont;

      if (!string.IsNullOrEmpty(NewStandardFontFamily))
        newFont = newFont.WithFamily(NewStandardFontFamily);
      if (NewStandardFontSize.HasValue)
        newFont = newFont.WithSize(NewStandardFontSize.Value);

      using (var token = doc.SuspendGetToken())
      {
        if (newFont != oldFont)
          doc.PropertyBagNotNull.SetValue(Altaxo.Graph.Gdi.GraphDocument.PropertyKeyDefaultFont, newFont);

        // calculate new line thickness
        var newLineThickness = Altaxo.Graph.Gdi.GraphDocument.GetDefaultPenWidth(doc.GetPropertyHierarchy());
        if (UserDefinedLineThickness.HasValue)
          newLineThickness = UserDefinedLineThickness.Value;
        // new major tick length
        var newMajorTickLength = Altaxo.Graph.Gdi.GraphDocument.GetDefaultMajorTickLength(doc.GetPropertyHierarchy());
        if (UserDefinedMajorTickLength.HasValue)
          newMajorTickLength = UserDefinedMajorTickLength.Value;

        if (NewRootLayerSize.HasValue)
        {
          doc.RootLayer.Size = NewRootLayerSize.Value;
        }

        var alreadyProcessedItems = new HashSet<object>(EqualityComparer<object>.Default); // because Gdi graphs have objects in the normal hierarchy as well as cached objects in the GraphObjects collection, we have to keep track of already processed items

        foreach (var node in doc.EnumerateFromHereToLeaves().OfType<Altaxo.Graph.IRoutedPropertyReceiver>())
        {
          if (alreadyProcessedItems.Contains(node))
            continue; // already processed
          else
            alreadyProcessedItems.Add(node);

          // Font family
          if (!string.IsNullOrEmpty(NewStandardFontFamily))
          {
            if (OptionResetAllFontsToStandardFont) // change all fonts to new font family
            {
              foreach (var prop in node.GetRoutedProperties("FontFamily"))
                prop.PropertySetter(NewStandardFontFamily);
            }
            else // change FontFamily only if font was old standard font
            {
              foreach (var prop in node.GetRoutedProperties("FontFamily"))
              {
                if (!string.IsNullOrEmpty((string)prop.PropertyValue) && ((string)prop.PropertyValue) == OldStandardFontFamily)
                {
                  prop.PropertySetter(NewStandardFontFamily);
                }
              }
            }
          }

          // Font size
          if (NewStandardFontSize.HasValue && ActionForFontSize != ScalarSizeActions.None)
          {
            if (ActionForFontSize == ScalarSizeActions.ResetAllToStandardValue)
            {
              foreach (var prop in node.GetRoutedProperties("FontSize"))
                prop.PropertySetter(NewStandardFontSize.Value);
            }
            else if (ActionForFontSize == ScalarSizeActions.RescaleByRatioNewToOldValue)
            {
              foreach (var prop in node.GetRoutedProperties("FontSize"))
              {
                if (newFont.Size != oldFont.Size)
                {
                  var oldValue = (double)prop.PropertyValue;
                  var newValue = oldValue * newFont.Size / oldFont.Size;
                  prop.PropertySetter(newValue);
                }
              }
            }
            else
            {
              throw new NotImplementedException();
            }
          }

          // Symbol size (the value to scale symbol size is used from the font size)
          if (NewStandardFontSize.HasValue && ActionForSymbolSize != ScalarSizeActions.None)
          {
            if (ActionForSymbolSize == ScalarSizeActions.ResetAllToStandardValue)
            {
              foreach (var prop in node.GetRoutedProperties("SymbolSize"))
                prop.PropertySetter(NewStandardFontSize.Value);
            }
            else if (ActionForSymbolSize == ScalarSizeActions.RescaleByRatioNewToOldValue)
            {
              foreach (var prop in node.GetRoutedProperties("SymbolSize"))
              {
                if (newFont.Size != oldFont.Size)
                {
                  var oldValue = (double)prop.PropertyValue;
                  var newValue = oldValue * newFont.Size / oldFont.Size;
                  prop.PropertySetter(newValue);
                }
              }
            }
            else
            {
              throw new NotImplementedException();
            }
          }

          // Line thickness

          if (newLineThickness != oldLineThickness && ActionForLineThickness != ScalarSizeActions.None)
          {
            if (ActionForLineThickness == ScalarSizeActions.ResetAllToStandardValue)
            {
              foreach (var prop in node.GetRoutedProperties("StrokeWidth"))
                prop.PropertySetter(newLineThickness);
            }
            else if (ActionForLineThickness == ScalarSizeActions.RescaleByRatioNewToOldValue)
            {
              if (newLineThickness != oldLineThickness)
              {
                foreach (var prop in node.GetRoutedProperties("StrokeWidth"))
                {
                  var oldValue = (double)prop.PropertyValue;
                  var newValue = oldValue * newLineThickness / oldLineThickness;
                  prop.PropertySetter(newValue);
                }
              }
            }
            else
            {
              throw new NotImplementedException();
            }
          }

          // Major tick length
          if (newMajorTickLength != oldMajorTickLength && ActionForTickLength != ScalarSizeActions.None)
          {
            if (ActionForTickLength == ScalarSizeActions.ResetAllToStandardValue)
            {
              foreach (var prop in node.GetRoutedProperties("MajorTickLength"))
                prop.PropertySetter(newMajorTickLength);
            }
            else if (ActionForTickLength == ScalarSizeActions.RescaleByRatioNewToOldValue)
            {
              if (newMajorTickLength != oldMajorTickLength)
              {
                foreach (var prop in node.GetRoutedProperties("MajorTickLength"))
                {
                  var oldValue = (double)prop.PropertyValue;
                  var newValue = oldValue * newMajorTickLength / oldMajorTickLength;
                  prop.PropertySetter(newValue);
                }
              }
            }
            else
            {
              throw new NotImplementedException();
            }
          }
        } // foreach node
      } // Resume
    }
  }
}
