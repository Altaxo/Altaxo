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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Main;

namespace Altaxo.Text.GuiModels
{
  public class TextDocumentViewOptions : IProjectItemPresentationModel, ICloneable
  {
    /// <summary>
    /// Gets the Markdown document this presentation data is based on.
    /// </summary>
    public TextDocument Document { get; protected set; }

    /// <summary>
    /// Gets or sets the window configuration, i.e. if the editor and the viewer windows are located left, top, right, bottom or in a tab control
    /// </summary>
    public ViewerConfiguration WindowConfiguration { get; set; }

    /// <summary>
    /// Gets or sets the last focus location. If true, the viewer window was selected last, if false, the editor window was selected.
    /// </summary>
    public bool IsViewerSelected { get; set; }

    /// <summary>
    /// Indicates if word wrapping  is enabled in the editor window. If null, the default global value of Altaxo is used.
    /// </summary>
    public bool? IsWordWrappingEnabled { get; set; }

    /// <summary>
    /// Indicates if line numbering is enabled in the editor window. If null, the default global value of Altaxo is used.
    /// </summary>
    public bool? IsLineNumberingEnabled { get; set; }

    /// <summary>
    /// Indicates if spell checking is enabled in the viewer window. If null, the default global value of Altaxo is used.
    /// </summary>
    public bool? IsSpellCheckingEnabled { get; set; }

    /// <summary>
    /// Indicates if folding marks are enabled in the editor window. If null, the default global value of Altaxo is used.
    /// </summary>
    public bool? IsFoldingEnabled { get; set; }

    /// <summary>
    /// Indicates the highlighting style of the editor window. If null, the default global highlighting style of Altaxo is used.
    /// </summary>
    public string HighlightingStyle { get; set; }

    /// <summary>
    /// The fraction of the width (when shown in left-right configuration) or height (when shown in top-bottom configuration) of the source editor window in relation to the available width/height.
    /// </summary>
    private double _fractionOfSourceEditorWindowVisible = 0.5;


    /// <summary>
    /// Gets or sets a value indicating whether the outline window is visible.
    /// </summary>
    /// <value>
    /// A value indicating whether the outline window is visible.
    /// </value>
    public bool? IsOutlineWindowVisible { get; set; }

    /// <summary>
    /// Gets or sets the relative width of the outline window. A value of <see cref="double.NaN"/>
    /// is indicating that the width of the outline window is set automatically.
    /// </summary>
    /// <value>
    /// The width of the outline window relative.
    /// </value>
    public double? OutlineWindowRelativeWidth { get; set; }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextDocumentViewOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      private AbsoluteDocumentPath _pathToDocument;
      private TextDocumentViewOptions _deserializedInstance;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TextDocumentViewOptions)obj;
        info.AddValue("Document", AbsoluteDocumentPath.GetAbsolutePath(s.Document));
        info.AddEnum("WindowConfiguration", s.WindowConfiguration);
        info.AddValue("IsViewerSelected", s.IsViewerSelected);
        info.AddValue("FractionSourceEditor", s._fractionOfSourceEditorWindowVisible);
        info.AddValue("IsWordWrappingEnabled", s.IsWordWrappingEnabled);
        info.AddValue("IsLineNumberingEnabled", s.IsLineNumberingEnabled);
        info.AddValue("IsSpellCheckingEnabled", s.IsSpellCheckingEnabled);
        info.AddValue("IsFoldingEnabled", s.IsFoldingEnabled);
        info.AddValue("HighlightingStyle", s.HighlightingStyle);
        info.AddValue("IsOutlineWindowVisible", s.IsOutlineWindowVisible);
        info.AddValue("OutlineWindowRelativeWidth", s.OutlineWindowRelativeWidth);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = null != o ? (TextDocumentViewOptions)o : new TextDocumentViewOptions(info);

        var pathToDocument = (AbsoluteDocumentPath)info.GetValue("Document", s);
        s.WindowConfiguration = (ViewerConfiguration)info.GetEnum("WindowConfiguration", typeof(ViewerConfiguration));
        s.IsViewerSelected = info.GetBoolean("IsViewerSelected");
        s._fractionOfSourceEditorWindowVisible = info.GetDouble("FractionSourceEditor");
        s.IsWordWrappingEnabled = info.GetNullableBoolean("IsWordWrappingEnabled");
        s.IsLineNumberingEnabled = info.GetNullableBoolean("IsLineNumberingEnabled");
        s.IsSpellCheckingEnabled = info.GetNullableBoolean("IsSpellCheckingEnabled");
        s.IsFoldingEnabled = info.GetNullableBoolean("IsFoldingEnabled");
        s.HighlightingStyle = info.GetString("HighlightingStyle");
        if (info.CurrentElementName == "IsOutlineWindowVisible")
        {
          s.IsOutlineWindowVisible = info.GetNullableBoolean("IsOutlineWindowVisible");
          s.OutlineWindowRelativeWidth = info.GetNullableDouble("OutlineWindowRelativeWidth");
        }

        var surr = new XmlSerializationSurrogate0
        {
          _deserializedInstance = s,
          _pathToDocument = pathToDocument,
        };

        info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);

        return s;
      }

      private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, Main.IDocumentNode documentRoot, bool isFinallyCall)
      {
        object o = AbsoluteDocumentPath.GetObject(_pathToDocument, documentRoot);
        if (o is TextDocument textDoc)
        {
          _deserializedInstance.Document = textDoc;
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(EhDeserializationFinished);
        }
      }
    }

    #endregion Serialization

    protected TextDocumentViewOptions(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }

    public TextDocumentViewOptions(TextDocument doc)
    {
      Document = doc ?? throw new ArgumentNullException(nameof(doc));
    }

    public object Clone()
    {
      var result = (TextDocumentViewOptions)MemberwiseClone();
      return result;
    }

    /// <summary>
    /// The fraction of the width (when shown in left-right configuration) or height (when shown in top-bottom configuration) of the source editor window in relation to the available width/height.
    /// </summary>
    public double FractionOfSourceEditorWindowVisible
    {
      get
      {
        return _fractionOfSourceEditorWindowVisible;
      }
      set
      {
        if (!(value >= 0 && value <= 1))
          throw new ArgumentOutOfRangeException(nameof(value), "Must be >=0 and <=1");
        if (!(_fractionOfSourceEditorWindowVisible == value))
        {
          _fractionOfSourceEditorWindowVisible = value;
        }
      }
    }

    IProjectItem IProjectItemPresentationModel.Document => Document;

    #region Property keys

    public static readonly Main.Properties.PropertyKey<bool> PropertyKeyIsWordWrappingEnabled =
      new Main.Properties.PropertyKey<bool>(
        "FA2B3AF4-ED5D-4A26-B467-D9C4AE7C394B",
        "Text\\IsWordWrappingEnabled",
        Main.Properties.PropertyLevel.All,
        typeof(TextDocument),
        () => true);

    public static readonly Main.Properties.PropertyKey<bool> PropertyKeyIsLineNumberingEnabled =
      new Main.Properties.PropertyKey<bool>(
        "648D76D1-401F-4955-881A-716FFB4C6106",
        "Text\\IsLineNumberingEnabled",
        Main.Properties.PropertyLevel.All,
        typeof(TextDocument),
        () => false);

    public static readonly Main.Properties.PropertyKey<bool> PropertyKeyIsSpellCheckingEnabled =
      new Main.Properties.PropertyKey<bool>(
      "0F4B3D74-51BB-4FE3-BF30-465E02305593",
      "Text\\IsSpellCheckingEnabled",
      Main.Properties.PropertyLevel.All,
      typeof(TextDocument),
      () => false);

    public static readonly Main.Properties.PropertyKey<bool> PropertyKeyIsHyphenationEnabled =
      new Main.Properties.PropertyKey<bool>(
      "1A61D4A8-06E8-4A98-AD60-E89C0B47D77F",
      "Text\\IsHyphenationEnabled",
      Main.Properties.PropertyLevel.All,
      typeof(TextDocument),
      () => false);

    public static readonly Main.Properties.PropertyKey<bool> PropertyKeyIsFoldingEnabled =
      new Main.Properties.PropertyKey<bool>(
      "8FBF38A4-E9ED-4C17-B6F0-F6E5E285FD33",
      "Text\\IsFoldingEnabled",
      Main.Properties.PropertyLevel.All,
      typeof(TextDocument),
      () => false);

    public static readonly Main.Properties.PropertyKey<string> PropertyKeyHighlightingStyle =
    new Main.Properties.PropertyKey<string>(
    "60B8A644-DB17-4D7A-A6EC-DD076B6E3A7A",
    "Text\\HighlightingStyle",
    Main.Properties.PropertyLevel.All,
    typeof(TextDocument),
    () => "default");

    public static readonly Main.Properties.PropertyKey<bool> PropertyKeyOutlineVisible =
      new Main.Properties.PropertyKey<bool>(
        "208895E4-47CA-4A54-A19E-E09E1A2376CD",
        "Text\\OutlineWindowVisible",
        Main.Properties.PropertyLevel.All,
        typeof(TextDocument),
        () => false);

    public static readonly Main.Properties.PropertyKey<double> PropertyKeyOutlineWindowRelativeWidth =
      new Main.Properties.PropertyKey<double>(
        "6E55BC19-E1B5-4838-AD20-9A354E3C1C16",
        "Text\\OutlineWindowRelativeWidth",
        Main.Properties.PropertyLevel.All,
        typeof(TextDocument),
        () => double.NaN);


    #endregion Property keys
  }

  /// <summary>
  /// Designates the viewers configuration.
  /// </summary>
  public enum ViewerConfiguration
  {
    /// <summary>The editor window is left, the viewer window is right.</summary>
    EditorLeftViewerRight = 0,

    /// <summary>The editor window is top, the viewer window is bottom.</summary>
    EditorTopViewerBottom = 1,

    /// <summary>The editor window is right, the viewer window is left.</summary>
    EditorRightViewerLeft = 2,

    /// <summary>The editor window is bottom, the viewer window is top.</summary>
    EditorBottomViewerTop = 3,

    /// <summary>The editor window and the viewer window are inside a tabbed control.</summary>
    Tabbed = 4
  }
}
