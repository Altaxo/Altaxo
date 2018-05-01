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

namespace Altaxo.Text.Renderers.Maml
{
	public class MamlElement
	{
		public bool IsInlineElement { get; private set; }
		public string Name { get; private set; }

		public MamlElement(string name, bool isInlineElement)
		{
			Name = name;
			IsInlineElement = isInlineElement;
		}
	}

	/// <summary>
	/// Designate valid Maml elements
	/// </summary>
	public static class MamlElements
	{
		public static MamlElement code { get; private set; } = new MamlElement("code", false);
		public static MamlElement codeInline { get; private set; } = new MamlElement("codeInline", true);
		public static MamlElement content { get; private set; } = new MamlElement("content", false);
		public static MamlElement legacyBold { get; private set; } = new MamlElement("legacyBold", true);
		public static MamlElement legacyItalic { get; private set; } = new MamlElement("legacyItalic", true);
		public static MamlElement legacyStrikethrough { get; private set; } = new MamlElement("legacyStrikethrough", true);
		public static MamlElement legacyUnderline { get; private set; } = new MamlElement("legacyUnderline", true);
		public static MamlElement list { get; private set; } = new MamlElement("list", false);
		public static MamlElement listItem { get; private set; } = new MamlElement("listItem", false);
		public static MamlElement para { get; private set; } = new MamlElement("para", false);
		public static MamlElement quote { get; private set; } = new MamlElement("quote", false);
		public static MamlElement section { get; private set; } = new MamlElement("section", false);
		public static MamlElement sections { get; private set; } = new MamlElement("sections", false);
		public static MamlElement subscript { get; private set; } = new MamlElement("subscript", true);
		public static MamlElement superscript { get; private set; } = new MamlElement("superscript", true);
		public static MamlElement title { get; private set; } = new MamlElement("title", true);
	}
}
