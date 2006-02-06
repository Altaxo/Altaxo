//This file is part of the gNumerator MathML DOM library, a complete 
//implementation of the w3c mathml dom specification
//Copyright (C) 2003, Andy Somogyi
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//For details, see http://numerator.sourceforge.net, or send mail to
//andy@epsilon3.net

using System;
using System.Xml;

namespace MathML
{
	/// <summary>
	/// Unicode defines a large number of characters used in mathematics, and in most cases, 
	/// glyphs representing these characters are widely available in a variety of fonts. 
	/// Although these characters should meet almost all users needs, MathML recognizes 
	/// that mathematics is not static and that new characters are added when convenient. 
	/// Characters that become well accepted will likely be eventually incorporated
	/// by the Unicode Consortium or other standards bodies, but that is often a lengthy 
	/// process. In the meantime, a mechanism is necessary for accessing glyphs from 
	/// non-standard fonts representing these characters. The mglyph element is the 
	/// means by which users can directly access glyphs for characters that are not 
	/// defined by Unicode, or not known to the renderer. Similarly, the mglyph element 
	/// can also be used to select glyph variants for existing Unicode characters, 
	/// as might be desirable when a glyph variant has begun to differentiate itself 
	/// as a new character by taking on a distinguished mathematical meaning. The mglyph 
	/// element names a specific character glyph, and is valid inside any MathML leaf 
	/// content listed in Section 3.1.6 (mi, etc.) or Section 4.2.2 (ci, etc.) unless 
	/// otherwise restricted by an attribute (e.g. base=2 to &lt;cn&gt;). In order for a 
	/// visually-oriented renderer to render the character, the renderer must be told 
	/// what font to use and what index within that font to use.
	/// 
	/// Note, a future possible optimization, since most glyphs are created from
	/// entities mapped to glyphs, we could change the attributes as a pointer, 
	/// or index to a entity replacement???
	/// </summary>
	public class MathMLGlyphElement : MathMLPresentationElement
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLGlyphElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		/// A string giving an alternate name for the character. 
		/// Represents the mglyph’s alt attribute.
		/// </summary>
		public string Alt
		{
			get { return GetAttribute("alt"); }
			set { SetAttribute("alt", value); }
		}

		/// <summary>
		/// A string representing the font family.
		/// </summary>
		public String FontFamily
		{
			get { return GetAttribute("fontfamily"); }
			set { SetAttribute("fontfamily", value); }
		}

		/// <summary>
		/// An unsigned integer giving the glyph’s position within the font.
		/// </summary>
		public ushort Index
		{
			get { return Utility.ParseUnsignedShort(GetAttribute("index"), 0); }
			set { GetAttribute("index", value.ToString()); }
		}

		/// <summary>
		/// accept a visitor.
		/// return the return value of the visitor's visit method
		/// </summary>
		public override object Accept(MathMLVisitor v, object args)
		{
			return v.Visit(this, args);
		}
	}
}
