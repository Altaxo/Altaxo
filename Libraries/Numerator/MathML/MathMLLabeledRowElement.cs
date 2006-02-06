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
	/// This interface extends the MathMLTableRowElement interface to represent the 
	/// mlabeledtr element Section 3.5.3. Note that the presence of a label causes the
	/// indexth child node to differ from the index-th cell!
	/// </summary>
	/// <remarks>
	/// The mlabeledtr element represents a labeled row of a table and can be used for 
	/// numbered equations. The first child of mlabeledtr is the label. A label is
	/// somewhat special in that it is not considered an expression in the matrix and 
	/// is not counted when determining the number of columns in that row.
	/// </remarks>
	public class MathMLLabeledRowElement : MathMLTableRowElement
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLLabeledRowElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		/// A MathMLElement representing the label of this row. Note that retrieving this should 
		/// have the same effect as a call to Node::getfirstChild(), while setting it should 
		/// have the same effect as Node::replaceChild(Node::getfirstChild()). 
		/// DOMException NO_MODIFICATION_ALLOWED_ERR: Raised if this MathMLElement or the new 
		/// MathMLElement is read-only.
		/// </summary>
		public MathMLElement Label
		{
			get { return (MathMLElement)FirstChild; }
			set { ReplaceChild(value, FirstChild); }
		}

		/// <summray>
		/// accept a visitor.
		/// return the return value of the visitor's visit method
		/// </summray>
		public override object Accept(MathMLVisitor v, object args)
		{
			return v.Visit(this, args);
		}
	}
}
