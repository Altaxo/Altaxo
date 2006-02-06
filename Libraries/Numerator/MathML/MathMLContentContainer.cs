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
	/// This interface supports the MathML Content elements that may contain 
	/// child Content elements. The elements directly supported by 
	/// MathMLContentContainer include: reln (deprecated), lambda, lowlimit, 
	/// uplimit, degree, domainofapplication, and momentabout. Interfaces 
	/// derived from MathMLContentContainer support the elements apply, fn, 
	/// interval, condition, declare, bvar, set, list, vector, matrix, and matrixrow.
	/// </summary>
	public class MathMLContentContainer : MathMLContentElement, MathMLContainer
	{
		/// <summary>
		/// implementation of the MathMLContainer interface
		/// </summary>
		private MathMLContainerImpl container;

		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLContentContainer(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
			container = new MathMLContainerImpl(this);
		}		

		/// <summary>
		/// nBoundVariables of type unsigned long, readonly The number of bvar child 
		/// elements of this element.
		/// </summary>
		public int BoundVariables
		{
			get
			{
				int count = 0;
				for(XmlNode n = FirstChild; n != null; n = n.NextSibling)
				{
					if(n is MathMLBvarElement) 
					{
						count++;
					}
				}
				return count;
			}
		}


		/// <summary>
		/// This attribute represents the condition child element of this node. 
		/// DOMException HIERARCHY_REQUEST_ERR: Raised if this element does not permit a 
		/// child condition element. In particular, raised if this element	
		/// is not a apply, set, or list.
		/// </summary>
		public MathMLConditionElement Condition
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// This attribute represents the degree child element of this node. 
		/// This expresses, for instance, the degree of differentiation if this
		/// element is a bvar child of an apply element whose first child is a 
		/// diff or partialdiff. If this is an apply element whose first child 
		/// is a partialdiff, the OpDegree attribute, if present, represents 
		/// the total degree of differentiation. See Section 4.2.3.2.
		/// DOMException HIERARCHY_REQUEST_ERR: Raised if this element does not 
		/// permit a child degree element. In particular, raised if this element is not
		/// a bvar or apply
		/// </summary>
		public MathMLElement OpDegree
		{
			get
			{
				return null;
			}
		}
		
		/// <summary>
		/// This attribute represents the domainofapplication child element of this node, 
		/// if present. This may express, for instance, the domain of integration if 
		/// this element is an apply element whose first child is an integral operator 
		/// (int). See Section 4.2.3.2. DOMException HIERARCHY_REQUEST_ERR: Raised if 
		/// this element does not permit a child domainofapplication element.
		/// </summary>
		public virtual MathMLElement DomainOfApplication
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// This attribute represents the momentabout child element of this node, 
		/// if present. This typically expresses the point about which a statistical 
		/// moment is to be calculated, if this element is an apply element whose 
		/// first child is a moment. See Section 4.2.3.2. DOMException HIERARCHY_REQUEST_ERR: 
		/// Raised if this element does not permit a child momentabout element. 
		/// In particular, raised if this element is not an apply whose first child is a moment.
		/// </summary>
		public MathMLElement MomentAbout
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// This method retrieves the index-th MathMLBvarElement child of the MathMLElement. 
		/// Note that only bvar child elements are counted in determining the index-th bound variable.
		/// Parameters
		/// int index The one-based index into the bound variable children of this element of the 
		/// MathMLBvarElement to be retrieved.
		/// Return value MathMLBvarElement The MathMLBvarElement representing the index-th bvar 
		/// child of this element.
		/// This method raises no exceptions.
		/// </summary>
		MathMLBvarElement GetBoundVariable(int index)
		{
			return null;
		}


        /// <summary>
		/// This method inserts a MathMLBvarElement as a child node before the current 
		/// index-th bound variable child of this MathMLElement. If index is 0,
		/// newBVar is appended as the last bound variable child. This has the effect 
		/// of adding a bound variable to the expression this element represents. Note 
		/// that the new bound variable is inserted as the index-th bvar child node, 
		/// not necessarily as the index-th child node. The point of the method is to allow
		/// insertion of bound variables without requiring the caller to calculate the 
		/// exact order of child qualifier elements.
        /// </summary>
		MathMLBvarElement InsertBoundVariable(MathMLBvarElement newBVar, int index)
		{
			return null;
		}

		/// <summary>
		/// This method sets the index-th bound variable child of this MathMLElement 
		/// to newBVar. This has the effect of setting a bound variable in the expression
		/// this element represents. Note that the new bound variable is inserted as the 
		/// index-th bvar child node, not necessarily as the index-th child node. The
		/// point of the method is to allow insertion of bound variables without 
		/// requiring the caller to calculate the exact order of child qualifier elements. 
		/// If there is already a bvar at the index-th position, it is replaced by newBVar.
		/// </summary>
		MathMLBvarElement SetBoundVariable(MathMLBvarElement newBVar, int index)
		{
			return null;
		}

		/// <summary>
		/// This method deletes the index-th MathMLBvarElement child of the MathMLElement. 
		/// This has the effect of removing this bound variable from the list of
		/// qualifiers affecting the element this represents.
		/// </summary>
		/// <param name="index">The one-based index into the bound variable children of this 
		/// element of the MathMLBvarElement to be removed.</param>
		void DeleteBoundVariable(int index)
		{
		}

		/// <summary>
		/// This method removes the index-th MathMLBvarElement child of the MathMLElement 
		/// and returns it to the caller. This has the effect of removing this
		/// bound variable from the list of qualifiers affecting the element this represents.
		/// </summary>
		MathMLBvarElement RemoveBoundVariable(int index)
		{
			return null;
		}

		/// <summary>
		/// The number of child elements of this element which represent arguments of the element, 
		/// as opposed to qualifiers or declare elements. Thus for a MathMLContentContainer 
		/// it does not contain elements representing bound variables, conditions, separators, 
		/// degrees, or upper or lower limits (bvar, condition, sep, degree, lowlimit, or uplimit).
		/// </summary>
		public int ArgumentCount
		{
			get { return container.ArgumentCount; }
		}

		/// <summary>
		/// This attribute accesses the child MathMLElements of this element which are arguments 
		/// of it, as a MathMLNodeList. Note that this list does not contain any MathMLElements 
		/// representing qualifier elements or declare elements. 
		/// </summary>
		public MathMLNodeList Arguments
		{
			get { return container.Arguments; }
		}

		/// <summary>
		/// Provides access to the declare elements which are children of this element, 
		/// in a MathMLNodeList. All Nodes in this list must be MathMLDeclareElements.
		/// </summary>
		public MathMLNodeList Declarations
		{
			get {return container.Declarations;}
		}

		/// <summary>
		/// This method returns the indexth child argument element of this element. 
		/// This frequently differs from the value of Node::childNodes().item(index),
		/// as qualifier elements and declare elements are not counted.
		/// </summary>
		public MathMLElement GetArgument(int index)
		{
			return container.GetArgument(index);
		}

		/// <summary>
		/// This method sets newArgument as the index-th argument of this element. 
		/// If there is currently an index-th argument, it is replaced by newArgument.
		/// This frequently differs from setting the node at Node::childNodes().item(index), 
		/// as qualifier elements and declare elements are not counted.
		/// </summary>
		public MathMLElement SetArgument(MathMLElement newArgument, int index)
		{
			return container.SetArgument(newArgument, index);
		}

		/// <summary>
		/// This method inserts newArgument before the current index-th argument of this 
		/// element. If index is 0, or if index is one more than the current number
		/// of arguments, newArgument is appended as the last argument. This frequently 
		/// differs from setting the node at Node::childNodes().item(index),
		/// as qualifier elements and declare elements are not counted.
		/// </summary>
		public MathMLElement InsertArgument(MathMLElement newArgument, int index)
		{
			return container.InsertArgument(newArgument, index);
		}

		/// <summary>
		/// This method deletes the index-th child element that is an argument of this element. 
		/// Note that child elements which are qualifier elements or declare
		/// elements are not counted in determining the index-th argument.
		/// </summary>
		public void DeleteArgument(int index)
		{
			container.DeleteArgument(index);
		}

		/// <summary>
		/// This method deletes the index-th child element that is an argument of this element, 
		/// and returns it to the caller. Note that child elements that are qualifier
		/// elements or declare elements are not counted in determining the index-th argument.
		/// </summary>
		public MathMLElement RemoveArgument(int index)
		{
			return container.RemoveArgument(index);
		}

		/// <summary>
		/// This method retrieves the index-th child declare element of this element.
		/// </summary>
		public MathMLDeclareElement GetDeclaration(int index)
		{
			return container.GetDeclaration(index);
		}

		/// <summary>
		/// This method inserts newDeclaration as the index-th child declaration of this element. 
		/// If there is already an index-th declare child element, it is replaced by newDeclaration.
		/// </summary>
		public MathMLDeclareElement SetDeclaration(MathMLDeclareElement newDeclaration, int index)
		{
			return container.SetDeclaration(newDeclaration, index);
		}

		/// <summary>
		/// This method inserts newDeclaration before the current index-th child declare element 
		/// of this element. If index is 0, newDeclaration is appended as the last child declare element.
		/// </summary>
		public MathMLDeclareElement InsertDeclaration(MathMLDeclareElement newDeclaration, int index)
		{
			return container.InsertDeclaration(newDeclaration, index);
		}

		/// <summary>
		/// This method removes the MathMLDeclareElement representing the index-th declare child 
		/// element of this element, and returns it to the caller. Note that index is the position 
		/// in the list of declare element children, as opposed to the position in the list of all 
		/// child Nodes.
		/// </summary>
		public MathMLDeclareElement RemoveDeclaration(int index)
		{
			return container.RemoveDeclaration(index);
		}

		/// <summary>
		/// This method deletes the MathMLDeclareElement representing the index-th declare child 
		/// element of this element. Note that index is the position in the list of declare 
		/// element children, as opposed to the position in the list of all child Nodes.
		/// </summary>
		public void DeleteDeclaration(int index)
		{
			container.DeleteDeclaration(index);
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
