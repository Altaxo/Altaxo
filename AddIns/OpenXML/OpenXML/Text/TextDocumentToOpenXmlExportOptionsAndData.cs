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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Text
{
  public class TextDocumentToOpenXmlExportOptionsAndData : TextDocumentToOpenXmlExportOptions
  {

    #region "Serialization"

    /// <summary>
    /// 2018-12-07: Created
    /// 2020-01-29: Name change from OpenXMLExportOptions to TextDocumentToOpenXmlExportOptionsAndData
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("OpenXMLAddin", "Altaxo.Text.OpenXMLExportOptions", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextDocumentToOpenXmlExportOptionsAndData), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TextDocumentToOpenXmlExportOptionsAndData)obj;

        info.AddValue("ExpandChildDocuments", s.ExpandChildDocuments);
        info.AddValue("MaxImageWidth", s.MaximumImageWidth);
        info.AddValue("MaxImageHeight", s.MaximumImageHeight);
        info.AddValue("ImageResolution", s.ImageResolutionDpi);

        info.AddValue("ThemeName", s.ThemeName);
        info.AddValue("RemoveOldContent", s.RemoveOldContentsOfTemplateFile);
        info.AddValue("OpenApplication", s.OpenApplication);
        info.AddValue("OutputFileName", s.OutputFileName);

        info.AddValue("RenumerateFigures", s.RenumerateFigures);
        info.AddValue("UseAutomaticFigureNumbering", s.UseAutomaticFigureNumbering);
        info.AddValue("DoNotFormatFigureLinksAsHyperlinks", s.DoNotFormatFigureLinksAsHyperlinks);
        info.AddValue("ShiftSolitaryHeader1ToTitle", s.ShiftSolitaryHeader1ToTitle);
      }

      public void Deserialize(TextDocumentToOpenXmlExportOptionsAndData s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        s.ExpandChildDocuments = info.GetBoolean("ExpandChildDocuments");
        s.MaximumImageWidth = (Altaxo.Units.DimensionfulQuantity?)info.GetValue("MaxImageWidth", s);
        s.MaximumImageHeight = (Altaxo.Units.DimensionfulQuantity?)info.GetValue("MaxImageHeight", s);
        //if (info.CurrentElementName == "ImageResolution")
        s.ImageResolutionDpi = info.GetInt32("ImageResolution");

        s.ThemeName = info.GetString("ThemeName");
        //if (info.CurrentElementName == "RemoveOldContent")
        s.RemoveOldContentsOfTemplateFile = info.GetBoolean("RemoveOldContent");
        s.OpenApplication = info.GetBoolean("OpenApplication");
        s.OutputFileName = info.GetString("OutputFileName");

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
        var s = (TextDocumentToOpenXmlExportOptionsAndData)o ?? new TextDocumentToOpenXmlExportOptionsAndData();
        Deserialize(s, info, parent);
        return s;
      }
    }

    #endregion "Serialization"

    public override object Clone()
    {
      return this.MemberwiseClone();
    }

    /// <summary>
    /// Gets or sets the output file. This is preferably a Sandcastle help file builder project file, but can also be a layout content file (.content) or a Maml file (.aml).
    /// </summary>
    public string OutputFileName { get; set; }


    /// <summary>
    /// If true, the application that is linked to .docx format will be opened.
    /// </summary>
    public bool OpenApplication { get; set; } = true;

    public static readonly Main.Properties.PropertyKey<TextDocumentToOpenXmlExportOptionsAndData> PropertyKeyTextDocumentToOpenXMLExportOptionsAndData =
      new Main.Properties.PropertyKey<TextDocumentToOpenXmlExportOptionsAndData>(
        "5BB2EED3-9FCD-4E63-9113-CCB2A2066462",
        "Text\\OpenXMLExportOptionsAndData",
        Main.Properties.PropertyLevel.Document,
        typeof(TextDocument),
        () => new TextDocumentToOpenXmlExportOptionsAndData());

  }
}
