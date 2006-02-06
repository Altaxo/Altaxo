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
using System.Collections;
using System.Xml;

namespace MathML
{
	/// <summary>
	/// 
	/// </summary>
	internal class MathMLArgumentsEnumerator : IEnumerator
	{
		private MathMLElement parent = null;
		private XmlNode current = null;
		private bool reset = true;

		/// <summary>
		/// create the enumerator. the enumerator is initially created with the
		/// current element set before the collection
		/// </summary>
		/// <param name="p">the parent node</param>
		internal MathMLArgumentsEnumerator(MathMLElement p)
		{
			parent = p;
		}

		public void Reset()
		{
			// grab the first child node
			current = parent is MathMLApplyElement ? parent.FirstChild.NextSibling : parent.FirstChild;

			// move the node the the first VALID position
			while (current != null && InvalidType(current))
			{
				current = current.NextSibling; 
			} 
			
		}

		public object Current
		{
			get { return current; }
		}

		/// <summary>
		/// move the the next valid type, skipping over any invalid nodes
		/// </summary>
		/// <returns>true if there is a next node, false otherwise</returns>
		public bool MoveNext()
		{
			if(reset) 
			{ 
				MoveFirst();
			}
			else if(current != null)
			{
				do 
				{ 
					current = current.NextSibling; 
				} 
				while (current != null && InvalidType(current));
			}
			return current != null;
		}

		/// <summary>
		/// tests if a node is a valid type or not, as defined in the mathml spec
		/// </summary>
		/// <param name="n">a node to test</param>
		/// <returns>true if the node is invalid, false if the node is valid</returns>
		internal static bool InvalidType(XmlNode n)
		{
			return (!(n is MathMLElement) ||
					(n is MathMLSeparator		|| 
				     n is MathMLBvarElement		||
				     n is MathMLConditionElement ||
				     n is MathMLDeclareElement	||
				     (n is MathMLContentContainer && (n.Name == "degree"	||
					 								  n.Name == "lowlimit"	||
													  n.Name == "uplimit"))));
		}

		/// <summary>
		/// move the current node to the first position
		/// </summary>
		private void MoveFirst()
		{
			// grab the first child node
			current = parent is MathMLApplyElement ? parent.FirstChild.NextSibling : parent.FirstChild;

			// move the node the the first VALID position
			while (current != null && InvalidType(current))
			{
				current = current.NextSibling; 
			} 
			reset = false;			
		}
	}
}
