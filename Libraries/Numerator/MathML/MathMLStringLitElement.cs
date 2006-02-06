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
	/// This interface extends the MathMLPresentationToken interface for the MathML 
	/// string literal element ms.
	/// </summary> 
	/// <remarks>
	/// The ms element is used to represent ‘string literals’ in expressions meant to be interpreted 
	/// by computer algebra systems or other systems containing ‘programming languages’. By default, 
	/// string literals are displayed surrounded by double quotes. As explained in Section 3.2.6, 
	/// ordinary text embedded in a mathematical expression	should be marked up with mtext, or in 
	/// some cases mo or mi, but never with ms.	Note that the string literals encoded by ms are 
	/// ‘Unicode strings’ rather than ‘ASCII strings’. In practice, non-ASCII characters will 
	/// typically be represented by entity 	references. For example, <c>&lt;ms&gt;&amp;amp&lt;/ms&gt;</c> 
	/// represents a string literal containing a single character, &amp;, and <c>&lt;ms&gt;&amp;amp;amp;&lt;/ms&gt;</c> 
	/// represents a string literal containing 5 characters, the first one of which is &amp;. Like all token elements, 
	/// ms does trim and collapse whitespace in its content according to the rules of Section 2.4.6, 
	/// so whitespace intended to remain in the content should be encoded as described in that section.
	/// </remarks>
	public class MathMLStringLitElement : MathMLPresentationToken
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLStringLitElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		/// A string giving the opening delimiter for the string literal; 
		/// represents the lquote attribute for the ms element, if specified.
		/// </summary>
		public String LQuote
		{
			get { return HasAttribute("lquote") ? GetAttribute("lquote") : "\""; }
			set { SetAttribute("lquote", value); }
		}

		/// <summary>
		/// A string giving the closing delimiter for the string literal; 
		/// represents the rquote attribute for the ms element, if specified.
		/// </summary>
		public String RQuote
		{
			get { return HasAttribute("rquote") ? GetAttribute("rquote") : "\""; }
			set { SetAttribute("rquote", value); }
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
