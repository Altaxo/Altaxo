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
	/// The apply element allows a function or operator to be applied to its arguments.
	/// </summary>
	public class MathMLApplyElement : MathMLContentContainer
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLApplyElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		/// The MathML element representing the function or operator that is applied to the list of arguments.
		/// </summary>
		public MathMLElement Operator
		{
			get
			{
				return (MathMLElement)FirstChild;
			}
			set
			{
				ReplaceChild(value, FirstChild);
			}
		}
		
		/// <summary>
		/// This attribute represents the domainofapplication child element of this node (if any). 
		/// This expresses, for instance, the domain of integration if this is an apply element 
		/// whose first child is a int. See Section 4.2.3.2. DOMException HIERARCHY_REQUEST_ERR: 
		/// Raised if this element does not permit a child domainofapplication element.
		/// </summary>
		public override MathMLElement DomainOfApplication
		{
			get
			{
				return (MathMLElement)SelectChildNode("domainofapplication");
			}
			set
			{
				XmlNode node = SelectChildNode("domainofapplication");

				if(node != null)
				{
					ReplaceChild(value, node);
				}
				else
				{
					InsertAfter(value, FirstChild);
				}
			}
		}

		/// <summary>
		/// This attribute represents the lowlimit child element of this node (if any). 
		/// This expresses, for instance, the lower limit of integration if this is an apply element 
		/// whose first child is a int. See Section 4.2.3.2. DOMException HIERARCHY_REQUEST_ERR: 
		/// Raised if this element does not permit a child lowlimit element. In particular, 
		/// raised if this element is not an apply element whose first child is an int, sum, product, 
		/// or limit element.
		/// </summary>
		public MathMLElement LowLimit
		{
			get
			{
				return (MathMLElement)SelectChildNode("lowlimit");
			}
			set
			{
				XmlNode node = SelectChildNode("lowlimit");
				if(node != null)
				{
					ReplaceChild(value, node);
				}
				else
				{
					AppendChild(value);
				}
			}
		}


		/// <summary>
		/// This attribute represents the uplimit child element of this node (if any). This expresses, 
		/// for instance, the upper limit of integration if this is an apply element whose first child 
		/// is a int. See Section 4.2.3.2. DOMException HIERARCHY_REQUEST_ERR: Raised if this element 
		/// does not permit a child uplimit element. In particular, raised if this element is not an 
		/// apply element whose first child is an int, sum, or product element.
		/// </summary>
		public MathMLElement UpLimit
		{
			get
			{
				return (MathMLElement)SelectChildNode("uplimit");
			}
			set
			{
				XmlNode node = SelectChildNode("uplimit");
				if(node != null)
				{
					ReplaceChild(value, node);
				}
				else
				{
					AppendChild(value);
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
