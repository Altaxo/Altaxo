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
using System.Collections;

namespace MathML
{
	/// <summary>
	/// This interface is provided as a specialization of the NodeList interface. 
	/// The child Nodes of this NodeList must be MathMLElements or Text nodes. Note that
	/// MathMLNodeLists are frequently used in the DOM as values of readonly attributes, 
	/// encapsulating, for instance, various collections of child elements. When
	/// used in this way, these objects are always understood to be live, in the sense 
	/// that changes to the document are immediately reflected in them.
	/// </summary>
	internal class MathMLTableNodeList : MathMLNodeList
	{
		// real node list of parent node
		private XmlNodeList childNodes;

		// starting index of the child node to start this list at
		private int firstChild;

		/// <summary>
		/// create a node list, attached to the given parent, and stating at the
		/// first child node
		/// </summary>
		internal MathMLTableNodeList(XmlNodeList childNodes)
		{
			this.childNodes = childNodes;
			this.firstChild = 0;
		}

		internal MathMLTableNodeList(XmlNodeList childNodes, int firstChild)
		{
			this.childNodes = childNodes;
			this.firstChild = firstChild;
		}


		public override int Count
		{
			get
			{
				int count = childNodes.Count - firstChild;
				return count >= 0 ? count : 0;
			}
		}

		public override XmlNode Item(int index)
		{
			return childNodes.Item(index + firstChild);
		}

		public override IEnumerator GetEnumerator ()
		{
			IEnumerator e = childNodes.GetEnumerator();
			for(int i = 0; i < firstChild; i++) e.MoveNext();
			return e;
		}
	}
}
