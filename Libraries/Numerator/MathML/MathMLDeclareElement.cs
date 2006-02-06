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
	/// The declare construct has two primary roles. The first is to change or set 
	/// the default attribute values for a specific mathematical object. The second 
	/// is to establish	an association between a ‘name’ and an object.
	/// </summary>
	public class MathMLDeclareElement : MathMLContentElement
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		internal MathMLDeclareElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc) {}

        /// <summary>
		/// A string indicating the type of the identifier. It must be compatible with the type of the 
		/// constructor, if a constructor is present. The type is inferred from the constructor if present, 
		/// otherwise it must be specified.
		/// </summary>
		public string Type
		{
			get { return GetAttribute("type"); }
			set { SetAttribute("type", value); }
		}

		/// <summary>
		/// If the identifier is a function, this attribute specifies the number of arguments the function 
		/// takes. This represents the declare element’s nargs attribute; see Section 4.4.2.8.
		/// </summary>
		public int NumArgs
		{
			// might need some exception handling here
			get { return int.Parse(GetAttribute("nargs")); }
			set { SetAttribute("nargs", value.ToString()); }
		}

		/// <summary>
		/// A string with the values prefix, infix, postfix, or function-model.
		/// </summary>
		public Occurrence Occurrence
		{
			get
			{
				string s = GetAttribute("occurrence");
				return Utility.ParseOccurrence(s, Occurrence.FunctionModel);
			}
			set
			{
				SetAttribute("occurrence", Utility.UnparseOccurrence(value));
			}
		}
		
		/// <summary>
		/// A URI specifying the detailed semantics of the element. 
		/// </summary>
		public string DefinitionURL
		{
			get { return GetAttribute("definitionURL"); }
			set { SetAttribute("definitionURL", value); }
		}

		/// <summary>
		/// A description of the syntax used in definitionURL.
		/// </summary>
		public string Encoding
		{
			get { return GetAttribute("encoding"); }
			set { SetAttribute("encoding", value); }
		}
		
		/// <summary>
		/// A MathMLCiElement representing the name being declared.
		/// this should allways be the first child node
		/// </summary>
		public MathMLCiElement Identifier
		{
			get { return FirstChild as MathMLCiElement; }
		}

		/// <summary>
		/// An optional MathMLElement providing an initial value for the object being declared.
		/// </summary>
		public MathMLElement Constructor
		{
			get { return (MathMLElement)ChildNodes.Item(1); }
			set 
			{
				int child_nodes = ChildNodes.Count;

				if(child_nodes == 1) 
				{
					AppendChild(value);
				}
				else if(child_nodes == 2)
				{
					ReplaceChild(value, ChildNodes.Item(1));
				}
				else
				{
					// this is bad
				}
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
