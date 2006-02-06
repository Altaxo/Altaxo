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
	/// This interface extends the MathMLPresentationElement interface for the MathML 
	/// underscript, overscript and overscript-underscript pair elements munder, mover
	/// and munderover.
	/// </summary>
	public class MathMLUnderOverElement : MathMLPresentationElement
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLUnderOverElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		/// Either true or false if present; a string controlling whether underscript is drawn as an 
		/// ‘accent’ or as a ‘limit’, if specified; this is the element’s accentunder attribute. 
		/// This must return null for an mover.
		/// </summary>
		/// <remarks>
		/// The accentunder attribute controls whether underscript is drawn as an ‘accent’ or as a limit. 
		/// The main difference between an accent and a limit is that the limit is reduced in size whereas 
		/// an accent is the same size as the base. A second difference is that the accent is drawn closer 
		/// to the base. The default value of accentunder is false, unless underscript is an mo element or 
		/// an embellished operator (see Section 3.2.5). If underscript is an mo element, the value of its 
		/// accent attribute is used as the default value of accentunder. If underscript is an embellished 
		/// operator, the accent attribute of the mo element at its core is used as the default value. 
		/// As with all attributes, an explicitly given value overrides the default. Here is an example 
		/// (accent versus underscript): x+y+z| {z } versus x+y+z | {z }. The MathML representation for this 
		/// example is shown below. If the base is an operator with movablelimits=true (or an embellished 
		/// operator whose mo element core has movablelimits=true), and displaystyle=false, then underscript 
		/// is drawn in a subscript position. In this case, the accentunder attribute is ignored. This is 
		/// often used for limits on symbols such as &amp;sum;. Within underscript, munder always sets 
		/// displaystyle to false, but increments scriptlevel by 1 only when accentunder is false. Within 
		/// base, it always leaves both attributes unchanged. (These attributes are inherited by every element 
		/// through its rendering environment, but can be set explicitly only on mstyle; see Section 3.3.4.)
		/// </remarks>
		public bool AccentUnder
		{
			get
			{
				bool result = false;
				string s = GetAttribute("accentunder");
				if(s.Length > 0)
				{
					result = Utility.ParseBool(s, false);
				}
				else
				{
					MathMLElement u = UnderScript;
					MathMLOperatorElement op = u != null ? u.EmbelishedOperator : null;
					result = op != null ? op.Accent : false;
				}
				return result;
			}
			set { SetAttribute("accentunder", value.ToString()); }
		}

		/// <summary>
		/// Either true or false if present; a string controlling whether overscript is drawn as an 
		/// ‘accent’ or as a ‘limit’, if specified; this is the element’s accent attribute. This must 
		/// return null for an munder.
		/// </summary>
		/// <remarks>
		/// The accent attribute controls whether overscript is drawn as an ‘accent’ (diacritical mark) 
		/// or as a limit. The main difference between an accent and a limit is that the limit is reduced 
		/// in size whereas an accent is the same size as the base. A second difference is that the 
		/// accent is drawn closer to the base. This is shown below (accent versus limit): ˆ x versus ˆx.
		/// These differences also apply to ‘mathematical accents’ such as bars over expressions: z }| 
		/// { x+y+z versus z }|{x+y+z. The MathML representation for each of these examples is shown below.
		/// The default value of accent is false, unless overscript is an mo element or an embellished 
		/// operator (see Section 3.2.5). If overscript is an mo element, the value of its accent attribute 
		/// is used as the default value of accent for mover. If overscript is an embellished operator, 
		/// the accent attribute of the mo element at its core is used as the default value. If the base is 
		/// an operator with movablelimits=true (or an embellished operator whose mo element core has 
		/// movablelimits=true), and displaystyle=false, then overscript is drawn in a superscript position. 
		/// In this case, the accent attribute is ignored. This is often used for limits on symbols such as 
		/// &amp;sum;. Within overscript, mover always sets displaystyle to false, but increments scriptlevel 
		/// by 1 only when accent is false. Within base, it always leaves both attributes unchanged. (These 
		/// attributes are inherited by every element through its rendering environment, but can be set 
		/// explicitly only on mstyle; see Section 3.3.4.)
		/// </remarks>
		public bool Accent
		{
			get
			{
				bool result = false;
				string s = GetAttribute("accent");
				if(s.Length > 0)
				{
					result = Utility.ParseBool(s, false);
				}
				else
				{
					MathMLElement o = OverScript;
					MathMLOperatorElement op = o != null ? o.EmbelishedOperator : null;
					result = op != null ? op.Accent : false;
				}
				return result;
			}
			set { SetAttribute("accent", value.ToString()); }
		}

		/// <summary>
		/// A MathMLElement representing the base of the script. This is the first child of the element.
		/// </summary>
		public MathMLElement Base
		{
			get 
			{ 
				XmlNode firstChild = FirstChild;
				return firstChild != null ? (MathMLElement)firstChild : null;	
			}
			set 
			{ 
				XmlNode firstChild = FirstChild;
				if(firstChild != null) ReplaceChild(value, firstChild);
				else AppendChild(value);
			}
		}

		/// <summary>
		/// MathMLElement A MathMLElement representing the underscript of the script. This is the second 
		/// child of a munder or munderover; retrieval must return null for an mover. 
		/// throws DOMException HIERARCHY_REQUEST_ERR: Raised when the element is a mover.
		/// </summary>
		public MathMLElement UnderScript
		{
			get 
			{ 
				MathMLElement result = null;
				if(Name != "mover") result = ChildNodes[1] as MathMLElement;
				return result;
			}
		}

		/// <summary>
		/// A MathMLElement representing the overscript of the script. This is the second child of a mover 
		/// or the third child of a munderover; retrieval must return null for an munder.
		/// throws DOMException HIERARCHY_REQUEST_ERR: Raised when the element is a munder.
		/// </summary>
		public MathMLElement OverScript
		{
			get
			{
				MathMLElement result = null;
				if(Name == "mover") result = ChildNodes[1] as MathMLElement;
				else if(Name == "munderover") result = ChildNodes[2] as MathMLElement;
				return result;
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
