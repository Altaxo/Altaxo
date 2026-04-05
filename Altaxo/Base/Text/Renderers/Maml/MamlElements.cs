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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Text.Renderers.Maml
{
  /// <summary>
  /// Describes a supported MAML element.
  /// </summary>
  public class MamlElement
  {
    /// <summary>
    /// Gets a value indicating whether the element is an inline element.
    /// </summary>
    public bool IsInlineElement { get; private set; }

    /// <summary>
    /// Gets the element name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MamlElement"/> class.
    /// </summary>
    /// <param name="name">The element name.</param>
    /// <param name="isInlineElement">Whether the element is inline.</param>
    public MamlElement(string name, bool isInlineElement)
    {
      Name = name;
      IsInlineElement = isInlineElement;
    }
  }

  /// <summary>
  /// Provides known MAML elements used by the renderer.
  /// </summary>
  public static class MamlElements
  {
    /// <summary>Gets the <c>a</c> element.</summary>
    public static MamlElement a { get; private set; } = new MamlElement("a", true);
    /// <summary>Gets the <c>code</c> element.</summary>
    public static MamlElement code { get; private set; } = new MamlElement("code", false);
    /// <summary>Gets the <c>codeInline</c> element.</summary>
    public static MamlElement codeInline { get; private set; } = new MamlElement("codeInline", true);
    /// <summary>Gets the <c>content</c> element.</summary>
    public static MamlElement content { get; private set; } = new MamlElement("content", false);
    /// <summary>Gets the <c>developerConceptualDocument</c> element.</summary>
    public static MamlElement developerConceptualDocument { get; private set; } = new MamlElement("developerConceptualDocument", false);
    /// <summary>Gets the <c>entry</c> element.</summary>
    public static MamlElement entry { get; private set; } = new MamlElement("entry", true);
    /// <summary>Gets the <c>externalLink</c> element.</summary>
    public static MamlElement externalLink { get; private set; } = new MamlElement("externalLink", true);
    /// <summary>Gets the <c>img</c> element.</summary>
    public static MamlElement img { get; private set; } = new MamlElement("img", true);
    /// <summary>Gets the <c>image</c> element.</summary>
    public static MamlElement image { get; private set; } = new MamlElement("image", true);
    /// <summary>Gets the <c>introduction</c> element.</summary>
    public static MamlElement introduction { get; private set; } = new MamlElement("introduction", false);
    /// <summary>Gets the <c>legacyBold</c> element.</summary>
    public static MamlElement legacyBold { get; private set; } = new MamlElement("legacyBold", true);
    /// <summary>Gets the <c>legacyItalic</c> element.</summary>
    public static MamlElement legacyItalic { get; private set; } = new MamlElement("legacyItalic", true);
    /// <summary>Gets the <c>legacyStrikethrough</c> element.</summary>
    public static MamlElement legacyStrikethrough { get; private set; } = new MamlElement("legacyStrikethrough", true);
    /// <summary>Gets the <c>legacyUnderline</c> element.</summary>
    public static MamlElement legacyUnderline { get; private set; } = new MamlElement("legacyUnderline", true);
    /// <summary>Gets the <c>link</c> element.</summary>
    public static MamlElement link { get; private set; } = new MamlElement("link", true);
    /// <summary>Gets the <c>linkText</c> element.</summary>
    public static MamlElement linkText { get; private set; } = new MamlElement("linkText", true);
    /// <summary>Gets the <c>linkUri</c> element.</summary>
    public static MamlElement linkUri { get; private set; } = new MamlElement("linkUri", true);
    /// <summary>Gets the <c>list</c> element.</summary>
    public static MamlElement list { get; private set; } = new MamlElement("list", false);
    /// <summary>Gets the <c>listItem</c> element.</summary>
    public static MamlElement listItem { get; private set; } = new MamlElement("listItem", false);
    /// <summary>Gets the <c>markup</c> element.</summary>
    public static MamlElement markup { get; private set; } = new MamlElement("markup", true);
    /// <summary>Gets the <c>mediaLinkInline</c> element.</summary>
    public static MamlElement mediaLinkInline { get; private set; } = new MamlElement("mediaLinkInline", true);
    /// <summary>Gets the <c>para</c> element.</summary>
    public static MamlElement para { get; private set; } = new MamlElement("para", false);
    /// <summary>Gets the <c>quote</c> element.</summary>
    public static MamlElement quote { get; private set; } = new MamlElement("quote", false);
    /// <summary>Gets the <c>row</c> element.</summary>
    public static MamlElement row { get; private set; } = new MamlElement("row", false);
    /// <summary>Gets the <c>section</c> element.</summary>
    public static MamlElement section { get; private set; } = new MamlElement("section", false);
    /// <summary>Gets the <c>sections</c> element.</summary>
    public static MamlElement sections { get; private set; } = new MamlElement("sections", false);
    /// <summary>Gets the <c>subscript</c> element.</summary>
    public static MamlElement subscript { get; private set; } = new MamlElement("subscript", true);
    /// <summary>Gets the <c>superscript</c> element.</summary>
    public static MamlElement superscript { get; private set; } = new MamlElement("superscript", true);
    /// <summary>Gets the <c>table</c> element.</summary>
    public static MamlElement table { get; private set; } = new MamlElement("table", false);
    /// <summary>Gets the <c>tableHeader</c> element.</summary>
    public static MamlElement tableHeader { get; private set; } = new MamlElement("tableHeader", false);
    /// <summary>Gets the <c>title</c> element.</summary>
    public static MamlElement title { get; private set; } = new MamlElement("title", true);
    /// <summary>Gets the <c>topic</c> element.</summary>
    public static MamlElement topic { get; private set; } = new MamlElement("topic", false);
  }
}
