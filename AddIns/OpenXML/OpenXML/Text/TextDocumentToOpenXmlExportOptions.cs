#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Altaxo.Graph;
using Altaxo.Gui;
using Altaxo.Text.Renderers;
using Markdig;

namespace Altaxo.Text
{
  /// <summary>
  /// Options to export a <see cref="TextDocument"/> into a OpenXML (MS Word) file, including all the referenced graphs and local images.
  /// </summary>
  public class TextDocumentToOpenXmlExportOptions : ICloneable
  {

    #region "Serialization"

    /// <summary>
    /// 2020-01-29: Created
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextDocumentToOpenXmlExportOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TextDocumentToOpenXmlExportOptions)obj;

        info.AddValue("ExpandChildDocuments", s.ExpandChildDocuments);
        info.AddValue("MaxImageWidth", s.MaximumImageWidth);
        info.AddValue("MaxImageHeight", s.MaximumImageHeight);
        info.AddValue("ImageResolution", s.ImageResolutionDpi);

        info.AddValue("ThemeName", s.ThemeName);
        info.AddValue("RemoveOldContent", s.RemoveOldContentsOfTemplateFile);

        info.AddValue("RenumerateFigures", s.RenumerateFigures);
        info.AddValue("UseAutomaticFigureNumbering", s.UseAutomaticFigureNumbering);
        info.AddValue("DoNotFormatFigureLinksAsHyperlinks", s.DoNotFormatFigureLinksAsHyperlinks);
        info.AddValue("ShiftSolitaryHeader1ToTitle", s.ShiftSolitaryHeader1ToTitle);
      }

      public void Deserialize(TextDocumentToOpenXmlExportOptions s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        s.ExpandChildDocuments = info.GetBoolean("ExpandChildDocuments");
        s.MaximumImageWidth = (Altaxo.Units.DimensionfulQuantity?)info.GetValue("MaxImageWidth", s);
        s.MaximumImageHeight = (Altaxo.Units.DimensionfulQuantity?)info.GetValue("MaxImageHeight", s);
        //if (info.CurrentElementName == "ImageResolution")
        s.ImageResolutionDpi = info.GetInt32("ImageResolution");

        s.ThemeName = info.GetString("ThemeName");
        //if (info.CurrentElementName == "RemoveOldContent")
        s.RemoveOldContentsOfTemplateFile = info.GetBoolean("RemoveOldContent");

        if (info.CurrentElementName == "RenumerateFigures")
        {
          s.RenumerateFigures = info.GetBoolean("RenumerateFigures");
          s.UseAutomaticFigureNumbering = info.GetBoolean("UseAutomaticFigureNumbering");
          s.DoNotFormatFigureLinksAsHyperlinks = info.GetBoolean("DoNotFormatFigureLinksAsHyperlinks");
        }

        if (info.CurrentElementName == "ShiftSolitaryHeader1ToTitle")
        {
          s.ShiftSolitaryHeader1ToTitle = info.GetBoolean("ShiftSolitaryHeader1ToTitle");
        }
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (TextDocumentToOpenXmlExportOptions)o ?? new TextDocumentToOpenXmlExportOptions();
        Deserialize(s, info, parent);
        return s;
      }
    }

    #endregion "Serialization"

    public virtual object Clone()
    {
      return MemberwiseClone();
    }

    #region Properties

    public string ThemeName { get; set; } = "GitHub";

    /// <summary>
    /// Gets or sets a value indicating whether the old contents of the .docx file used as style template should be removed.
    /// If set to false, the content is kept, and the new content is appended to the end of the document.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the old contents of the template file should be removed; otherwise, <c>false</c>.
    /// </value>
    public bool RemoveOldContentsOfTemplateFile { get; set; } = true;

    /// <summary>
    /// Gets or sets the image resolution of the graphs that must be rendered in Dpi.
    /// </summary>
    /// <value>
    /// The image resolution in dpi.
    /// </value>
    public int ImageResolutionDpi { get; set; } = 600;


    /// <summary>
    /// If true, included child documents are expanded before the markdown document is processed.
    /// </summary>
    public bool ExpandChildDocuments { get; set; } = true;

    /// <summary>
    /// If true, figures are renumerated and the links to those figures updated.
    /// </summary>
    public bool RenumerateFigures { get; set; } = true;

    /// <summary>
    /// If true, in the exported OpenXML document, the figures are numbered automatically, and the links will be replaced by fields.
    /// </summary>
    public bool UseAutomaticFigureNumbering { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether links to figures are formatted as hyperlinks or not.
    /// Explanation: in MS Word, it seems not possible to have a reference to a text marker hyperlink formatted.
    /// But, for automatic figure numbering and referencing we need references to text markers.
    /// </summary>
    /// <value>
    /// <c>true</c> if links to figures should not be hyperlink formatted.; otherwise, <c>false</c>.
    /// </value>
    public bool DoNotFormatFigureLinksAsHyperlinks { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a solitary Header1 should be shifted to the title, an header2..header7 should be shifted one level upwards.
    /// </summary>
    /// <value>
    ///   <c>true</c> if a solitary Header1 should be shifted to the title, an header2..header7 should be shifted one level upwards.
    /// </value>
    public bool ShiftSolitaryHeader1ToTitle { get; set; }




    /// <summary>
    /// Gets or sets the maximum width of exported images in points (1/72nd inch).
    /// </summary>
    /// <value>
    /// The maximum width of exported images (1/72nd inch).
    /// </value>
    public Altaxo.Units.DimensionfulQuantity? MaximumImageWidth { get; set; } // = new Units.DimensionfulQuantity(16, new Units.PrefixedUnit(Units.SIPrefix.Centi, Units.Length.Meter.Instance));

    /// <summary>
    /// Gets or sets the maximum height of exported images in points (1/72nd inch).
    /// </summary>
    /// <value>
    /// The maximum height of exported images (1/72nd inch).
    /// </value>
    public Altaxo.Units.DimensionfulQuantity? MaximumImageHeight { get; set; } // = new Units.DimensionfulQuantity(9.5, new Units.PrefixedUnit(Units.SIPrefix.Centi, Units.Length.Meter.Instance));

    #endregion Properties




    public static readonly Main.Properties.PropertyKey<TextDocumentToOpenXmlExportOptions> PropertyKeyTextDocumentToOpenXmlExportOptions =
      new Main.Properties.PropertyKey<TextDocumentToOpenXmlExportOptions>(
        "D4B1436F-CA4A-4D11-8A4C-869746997338",
        "Text\\OpenXMLExportOptions",
        Main.Properties.PropertyLevel.AllUpToFolder,
        typeof(TextDocument),
        () => new TextDocumentToOpenXmlExportOptions());

  }
}
