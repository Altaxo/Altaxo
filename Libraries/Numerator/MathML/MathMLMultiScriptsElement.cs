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

namespace MathML
{
	/// <summary>
	/// This interface extends the MathMLPresentationElement interface for the 
	/// MathML multiscripts (including prescripts or tensors) element mmultiscripts.
	/// </summary>
	public class MathMLMultiScriptsElement : MathMLPresentationElement
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLMultiScriptsElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		/// accept a visitor.
		/// return the return value of the visitor's visit method
		/// </summary>
		public override object Accept(MathMLVisitor v, object args)
		{
			return v.Visit(this, args);
		}

		/// <summary>
		/// Implement rule 2 of the definition of an embelished operator
		/// <see cref="MathMLElement"/>
		/// </summary>
		public override MathMLOperatorElement EmbelishedOperator
		{
			get
			{
				MathMLElement firstChild = FirstChild as MathMLElement;
				return firstChild is not null ? firstChild.EmbelishedOperator : null;
			}
		}
	}
}
