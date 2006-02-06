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
	/// This is the interface from which the interfaces representing the 
	/// MathML Content token elements (ci, cn and csymbol) are derived. 
	/// These elements may contain MathML Presentation elements, Text nodes, 
	/// or a combination of both. Thus the getArgument and insertArgument 
	/// methods have been provided to deal with this distinction between 
	/// these elements and other MathML Content elements.
	/// </summary>
	public class MathMLContentToken : MathMLContentElement
	{
		/// <summary>
		/// keep an argument list arround for the lifetime of this class
		/// it is probably more efficient to keep one of these around instead
		/// of new'ing one each time it is needed, as this was, we create a new instance
		/// only once, and the garbage collector only needs to keep track of it, as apposed
		/// to creating a new one each time.
		/// </summary>
		internal MathMLArgumentsList arguments;

		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		internal MathMLContentToken(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
			arguments = new MathMLArgumentsList(this);
		}

		/// <summary>
		/// The arguments of this element, returned as a MathMLNodeList. Note that this is 
		/// not necessarily the same as Node::childNodes, particularly in the case of the cn 
		/// element. The reason is that the sep elements that are used to separate the 
		/// arguments of a cn are not returned.
		/// </summary>
		public MathMLNodeList Arguments
		{
			get { return arguments; }
		}

		/// <summary>
		/// A URI pointing to a semantic definition for this content element. Note that there 
		/// is no stipulation about the form this definition may take!
		/// </summary>
		public string DefinitionURL
		{
			get { return GetAttribute("definitionURL"); }
			set { SetAttribute("definitionURL", value); }
		}

		/// <summary>
		/// A string describing the syntax in which the definition located at definitionURL is given.
		/// </summary>
		public string Encoding
		{
			get { return GetAttribute("encoding"); }
			set { SetAttribute("encoding", value); }
		}

		/// <summary>
		/// A convenience method to retrieve the child argument at the position referenced by 
		/// index. Note that this is not necessarily the same as the index-th child
		/// Node of this Element; in particular, sep elements will not be counted.
		/// </summary>
		public XmlNode GetArgument(int index)
		{
			return arguments.Item(index);
		}

		/// <summary>
		/// A convenience method to insert newArgument before the current index-th argument 
		/// child of this element. If index is 0, newArgument is appended as the last argument.
		/// </summary>
		public XmlNode InsertArgument(int index, XmlNode newArgument)
		{
			if(index == 0)
			{
				AppendChild(newArgument);
			}
			else
			{
				InsertBefore(newArgument, arguments.Item(index));
			}
			return newArgument;
		}

		/// <summary>
		/// A convenience method to set an argument child at the position referenced by index. 
		/// If there is currently an argument at this position, it is replaced by
		/// newArgument.
		/// </summary>
		public XmlNode SetArgument(int index, XmlNode newArgument)
		{
			XmlNode node = arguments.Item(index);

			if(node != null)
			{
				node = ReplaceChild(newArgument, node);
			}
			else if(index == arguments.Count + 1)
			{
				node = AppendChild(newArgument);
			}
			else
			{
				// this is bad, should throw some sort of exceptionb
			}
			return node;
		}

		/// <summary>
		/// A convenience method to delete the argument child located at the position 
		/// referenced by index.
		/// </summary>
		public void DeleteArgument(int index)
		{
			RemoveChild(arguments.Item(index));
		}

		/// <summary>
		/// A convenience method to delete the argument child located at the position .
		/// referenced by index, and to return it to the caller.
		/// </summary>
		public XmlNode RemoveArgument(int index)
		{
			return RemoveChild(arguments.Item(index));
		}
	}
}
