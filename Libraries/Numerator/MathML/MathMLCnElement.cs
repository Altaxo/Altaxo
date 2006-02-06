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
	/// The cn element is used to specify actual numeric constants.
	/// </summary>
	public class MathMLCnElement : MathMLContentToken
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLCnElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		/// Values include, but are not restricted to, e-notation, integer, rational, real, float, complex,
		/// complex-polar, complexcartesian, and constant.
		/// </summary>
		public string Type
		{
			get
			{
				string s = GetAttribute("type");
				return s.Length > 0 ? s : "real";
			}
			set
			{
				SetAttribute("type", value);
			}
		}

		/// <summary>
		/// base of type DOMString A string representing an integer between 2 and 36; 
		/// the base of the numerical representation.
		/// </summary>
		public string Base
		{
			get
			{
				string s = GetAttribute("base");
				return s.Length > 0 ? s : "10";
			}
			set
			{
				SetAttribute("base", value);
			}
		}

		/// <summary>
		/// nargs of type unsigned long, readonly The number of sep-separated arguments.
		/// </summary>
		public int NumArgs
		{
			get
			{
				return arguments.Count;
			}
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
