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
	/// This interface extends the MathMLPresentationElement interface for the 
	/// MathML subscript, superscript and subscript-superscript pair elements
	/// msub, msup, and	msubsup.
	/// </summary>
	public class MathMLScriptElement : MathMLPresentationElement
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLScriptElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}


		/// <summary>
		/// A string representing the minimum amount to shift the baseline of the 
		/// subscript down, if specified; this is the element’s subscriptshift attribute. 
		/// This must return null for an msup.
		/// </summary>
		public Length SubScriptShift {
			get {
				return Utility.ParseLength(GetAttribute("subscriptshift"), new Length(LengthType.Undefined));
			}
			set {
				SetAttribute("subscriptshift", value.ToString());
			}
		}

		/// <summary>
		/// The superscriptshift attribute specifies the minimum amount to shift the 
		/// baseline of superscript up. v-unit represents a unit of vertical length 
		/// (see Section 2.4.4.2). The msup element increments scriptlevel by 1, and sets 
		/// displaystyle to false, within superscript, but leaves both attributes unchanged 
		/// within base. (Theseattributes are inherited by every element through its 
		/// rendering environment, but can be set explicitly only on mstyle; see Section 3.3.4.)
		/// </summary>
		public Length SuperScriptShift {
			get {
				return Utility.ParseLength(GetAttribute("superscriptshift"), new Length(LengthType.Undefined));
			}
			set {
				SetAttribute("superscriptshift", value.ToString());
			}
		}

			/// <summary>
			/// A MathMLElement representing the base of the script. This is the first 
			/// child of the element.
			/// </summary>
			public MathMLElement Base {
			get { return (MathMLElement)FirstChild; }
			set 
			{ 
                ReplaceChild(value, FirstChild);				
			}
		}

		/// <summary>
		/// A MathMLElement representing the subscript of the script. This is the 
		/// second child of a msub or msubsup; retrieval must return null for an msup.
		/// </summary>
		public MathMLElement SubScript
		{
			get 
			{
				if(Name == "msup")
				{
					return null;
				}
				else
				{
					return (MathMLElement)FirstChild.NextSibling;
				}
			}

			set
			{
				if(Name == "msup")
				{
					// TODO this is bad, need exception
				}
				else
				{
					this.ReplaceChild(value, FirstChild.NextSibling);
				}
			}
		}

		/// <summary>
		/// A MathMLElement representing the superscript of the script. This is the 
		/// second child of a msup, or the third child of a msubsup; retrieval must 
		/// return null for an msub.										
		/// </summary>
		public MathMLElement SuperScript
		{
			get
			{
				if(Name == "msup")
				{
					return (MathMLElement)FirstChild.NextSibling;
				}
				else if(Name == "msubsup")
				{
					return (MathMLElement)FirstChild.NextSibling.NextSibling;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if(Name == "msup")
				{
					ReplaceChild(value, FirstChild.NextSibling);
				}
				else if(Name == "msubsup")
				{
					ReplaceChild(value, FirstChild.NextSibling.NextSibling);
				}
				else
				{
					// TODO need exception, this is bad
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

		/// <summary>
		/// Implement rule 2 of the definition of an embelished operator
		/// <see cref="MathMLElement"/>
		/// </summary>
		public override MathMLOperatorElement EmbelishedOperator
		{
			get
			{
				MathMLElement firstChild = FirstChild as MathMLElement;
				return firstChild != null ? firstChild.EmbelishedOperator : null;
			}
		}
	}
}
