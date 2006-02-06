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
	/// This is an abstract interface containing functionality required by MathML elements that 
	/// may contain arbitrarily many child elements. No elements are directly supported
	/// by this interface; all instances are instances of either MathMLPresentationContainer, 
	/// MathMLContentContainer, or MathMLMathElement.
	/// </summary>
	public interface MathMLContainer
	{
		/// <summary>
		/// The number of child elements of this element which represent arguments of the element, 
		/// as opposed to qualifiers or declare elements. Thus for a MathMLContentContainer 
		/// it does not contain elements representing bound variables, conditions, separators, 
		/// degrees, or upper or lower limits (bvar, condition, sep, degree, lowlimit, or uplimit).
		/// </summary>
		int ArgumentCount
		{
			get;
		}

		/// <summary>
		/// This attribute accesses the child MathMLElements of this element which are arguments 
		/// of it, as a MathMLNodeList. Note that this list does not contain any MathMLElements 
		/// representing qualifier elements or declare elements.
		/// </summary>
		MathMLNodeList Arguments
		{
			get;
		}

		/// <summary>
		/// Provides access to the declare elements which are children of this element, 
		/// in a MathMLNodeList. All Nodes in this list must be MathMLDeclareElements.
		/// </summary>
		MathMLNodeList Declarations
		{
			get;
		}

		/// <summary>
		/// This method returns the indexth child argument element of this element. 
		/// This frequently differs from the value of Node::childNodes().item(index),
		/// as qualifier elements and declare elements are not counted.
		/// </summary>
		MathMLElement GetArgument(int index);

		/// <summary>
		/// This method sets newArgument as the index-th argument of this element. 
		/// If there is currently an index-th argument, it is replaced by newArgument.
		/// This frequently differs from setting the node at Node::childNodes().item(index), 
		/// as qualifier elements and declare elements are not counted.
		/// </summary>
		MathMLElement SetArgument(MathMLElement newArgument, int index);

		/// <summary>
		/// This method inserts newArgument before the current index-th argument of this 
		/// element. If index is 0, or if index is one more than the current number
		/// of arguments, newArgument is appended as the last argument. This frequently 
		/// differs from setting the node at Node::childNodes().item(index),
		/// as qualifier elements and declare elements are not counted.
		/// </summary>
		MathMLElement InsertArgument(MathMLElement newArgument, int index);

		/// <summary>
		/// This method deletes the index-th child element that is an argument of this element. 
		/// Note that child elements which are qualifier elements or declare
		/// elements are not counted in determining the index-th argument.
		/// </summary>
		void DeleteArgument(int index);

		/// <summary>
		/// This method deletes the index-th child element that is an argument of this element, 
		/// and returns it to the caller. Note that child elements that are qualifier
		/// elements or declare elements are not counted in determining the index-th argument.
		/// </summary>
		MathMLElement RemoveArgument(int index);

        /// <summary>
        /// This method retrieves the index-th child declare element of this element.
        /// </summary>
		MathMLDeclareElement GetDeclaration(int index);

		/// <summary>
		/// This method inserts newDeclaration as the index-th child declaration of this element. 
		/// If there is already an index-th declare child element, it is replaced by newDeclaration.
		/// </summary>
		MathMLDeclareElement SetDeclaration(MathMLDeclareElement newDeclaration, int index);

		/// <summary>
		/// This method inserts newDeclaration before the current index-th child declare element 
		/// of this element. If index is 0, newDeclaration is appended as the last child declare element.
		/// </summary>
		MathMLDeclareElement InsertDeclaration(MathMLDeclareElement newDeclaration, int index);

		/// <summary>
		/// This method removes the MathMLDeclareElement representing the index-th declare child 
		/// element of this element, and returns it to the caller. Note that index is the position 
		/// in the list of declare element children, as opposed to the position in the list of all 
		/// child Nodes.
		/// </summary>
		MathMLDeclareElement RemoveDeclaration(int index);

		/// <summary>
		/// This method deletes the MathMLDeclareElement representing the index-th declare child 
		/// element of this element. Note that index is the position in the list of declare 
		/// element children, as opposed to the position in the list of all child Nodes.
		/// </summary>
		void DeleteDeclaration(int index);		
	}
}
