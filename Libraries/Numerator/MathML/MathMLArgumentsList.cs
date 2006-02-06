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
	/// An list that enumerates all child nodes of an element that are not
	/// of type x
	/// 
	/// The number of child elements of this element which represent arguments of the element,
	/// as opposed to qualifiers or declare elements. Thus for a MathMLContentContainer it does 
	/// not contain elements representing bound variables, conditions, separators, degrees, or 
	/// upper or lower limits (bvar, condition, sep, degree, lowlimit, or uplimit). Note that 
	/// presentation elements should not contain any of these types anyway ???
	/// 
	/// node types that will have a arguments property:
	/// 1: direct classes that implent MathMLContainer:
	///		MathMLPresentationContainer, 
	///		MathMLContentContainer, 
	///		MathMLMathElement
	///	2: concrete classes derived from these:
	///	
	///	MathMLPresentationContainer
	///	 |-> MathMLActionElement
	///	 |-> MathMLEncloseElement
	///	 |-> MathMLFencedElement
	///	 |-> MathMLPaddedElement
	///	 |-> MathMLStyleElement
	///	  -> MathMLTableCellElement
	///	 
	///	 MathMLContentContainer
	///	 |-> MathMLApplyElement
	///	 |-> MathMLBvarElement
	///	 |-> MathMLFnElement
	///	 |-> MathMLLambdaElement
	///	 |-> MathMLListElement
	///	  -> MathMLSetElement
	///		
	///	In the case of an apply element, the first child of that type is the opperator, 
	///	and is not considered an argument. 
	/// </summary>
	internal class MathMLArgumentsList : MathMLNodeList
	{
		private MathMLElement parent;

		internal MathMLArgumentsList(MathMLElement p)
		{
			parent = p;
		}

		public override int Count
		{
			get
			{
				int count = 0;
				XmlNode node = parent is MathMLApplyElement ? parent.FirstChild.NextSibling : parent.FirstChild;
				
				while(node != null)
				{
					if(!MathMLArgumentsEnumerator.InvalidType(node))
					{
						count++;
					}
					node = node.NextSibling;
				}
				return count;
			}
		}

		public override XmlNode Item(int index)
		{
			int i = 0;
			XmlNode node = parent is MathMLApplyElement ? parent.FirstChild.NextSibling : parent.FirstChild;

			while(node != null && i < index)
			{
				if(!MathMLArgumentsEnumerator.InvalidType(node))
				{
					i++;
				}				
				node = node.NextSibling;
			}
			return node;
		}

		public override IEnumerator GetEnumerator ()
		{
			return new MathMLArgumentsEnumerator(parent);
		}
	}
}
