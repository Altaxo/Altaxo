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
	/// An mspace empty element represents a blank space of any desired size, as set by its attributes. 
	/// It can also be used to make linebreaking suggestions to a visual renderer. Note that the 
	/// default values for attributes have been chosen so that they typically will have no effect on 
	/// rendering. Thus, the mspace element is generally used with one or more attribute values 
	/// explicitly specified.
	/// </summary>
	public class MathMLSpaceElement : MathMLPresentationElement
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLSpaceElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

        /// <summary>
        /// the 'width' attribute
        /// </summary>
		public Length Width
		{
			get { return Utility.ParseLength(GetAttribute("width"), new Length(LengthType.Ex)); }
			set { SetAttribute("width", value.ToString()); }
		}

        /// <summary>
        /// the 'height' attribute
        /// </summary>
		public Length Height
		{
			get { return Utility.ParseLength(GetAttribute("height"), new Length(LengthType.Ex)); }
			set { SetAttribute("height", value.ToString()); }
		}

        /// <summary>
        /// the 'depth' attribute
        /// </summary>
		public Length Depth
		{
			get { return Utility.ParseLength(GetAttribute("depth"), new Length(LengthType.Ex)); }
			set { SetAttribute("depth", value.ToString()); }
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
