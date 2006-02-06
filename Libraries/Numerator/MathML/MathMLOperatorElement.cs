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
	/// This interface extends the MathMLPresentationToken interface for the 
	/// MathML operator element mo.
	/// </summary>
	/// <remarks>
	/// An mo element represents an operator or anything that should be rendered as an operator. 
	/// In general, the notational conventions for mathematical operators are quite complicated, 
	/// and therefore MathML provides a relatively sophisticated mechanism for specifying the 
	/// rendering behavior of an mo element. As a consequence, in MathML the list of things that 
	/// should ‘render as an operator’ includes a number of notations that are not mathematical 
	/// operators in the ordinary sense. Besides ordinary operators with infix, prefix, or postfix 
	/// forms, these include fence characters such as braces, parentheses, and ‘absolute value’ 
	/// bars, separators such as comma and semicolon, and mathematical accents such as a bar or 
	/// tilde over a symbol. The term ‘operator’ as used in the present chapter means any symbol 
	/// or notation that should render as an operator, and that is therefore representable by an mo
	/// element. That is, the term ‘operator’ includes any ordinary operator, fence, separator, 
	/// or accent unless otherwise specified or clear from the context. All such symbols are represented 
	/// in MathML with mo elements since they are subject to essentially the same rendering attributes 
	/// and rules; subtle distinctions in the rendering of these classes of symbols, when they exist, 
	/// are supported using the boolean attributes fence, separator and accent, which can be used to 
	/// distinguish these cases. A key feature of the mo element is that its default attribute values 
	/// are set on a case-by-case basis from an ‘operator dictionary’ as explained below. In particular,
	/// default values for fence, separator and accent can usually be found in the operator dictionary 
	/// and therefore need not be specified on each mo element. Note that some mathematical operators 
	/// are represented not by mo elements alone, but by mo elements ‘embellished’ with (for example) 
	/// surrounding superscripts; this is further described below. Conversely, as presentation elements, 
	/// mo elements can contain arbitrary text, even when that text has no standard interpretation as
	/// an operator; for an example, see the discussion ‘Mixing text and mathematics’ in Section 3.2.6. 
	/// See also Chapter 4 for definitions of MathML content elements that are guaranteed to have the 
	/// semantics of specific mathematical operators.
	/// </remarks>
	public class MathMLOperatorElement : MathMLPresentationToken
	{
		private bool dirty = true;
		private Operator op;
		private Form form;

		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLOperatorElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{}

		/// <summary>
		/// The form attribute ("prefix", "infix" or "postfix") for the mo element, if specified.
		/// </summary>
		/// <remarks> 
		/// The form attribute does not usually have to be specified explicitly, since there 
		/// are effective heuristic rules for inferring the value of the form attribute from 
		/// the context. If it is not specified, and there is more than one possible form in 
		/// the dictionary for an mo element with given content, the renderer should choose 
		/// which form to use as follows (but see the exception for embellished operators, 
		/// described later):
		/// If the operator is the first argument in an mrow of length (i.e. number of arguments) 
		/// greater than one (ignoring all space-like arguments (see Section 3.2.7) in the 
		/// determination of both the length and the first argument), the prefix form is used;
		/// 
		/// if it is the last argument in an mrow of length greater than one (ignoring all 
		/// space-like arguments), the postfix form is used;
		///   
		/// in all other cases, including when the operator is not part of an mrow, the infix 
		/// form is used.
		/// 
		/// Note that these rules make reference to the mrow in which the mo 
		/// element lies. In some situations, this mrow might be an inferred mrow implicitly 
		/// present around the arguments of an element such as msqrt or mtd. Opening (left) 
		/// fences should have form="prefix", and closing (right) fences should have 
		/// form="postfix"; separators are usually ‘infix’, but not always, depending on 
		/// their surroundings. As with ordinary operators, these values do not usually 
		/// need to be specified explicitly.
		/// 
		/// If the operator does not occur in the dictionary with the specified form, the 
		/// renderer should use one of the forms that is available there, in the order of 
		/// preference: infix, postfix, prefix; if no forms are available for the given mo 
		/// element content, the renderer should use the defaults given in parentheses in 
		/// the table of attributes for mo.
		/// </remarks>
		public Form Form
		{
			get 
			{
				if(dirty) GetAttributes();
				return op.Form;
			}
			set 
			{
				SetAttribute("form", Utility.UnparseForm(value)); 
			}
		}

		/// <summary>
		/// The fence attribute ("true" or "false") for the mo element, if specified.
		/// </summary>
		public bool Fence
		{
			get 
			{ 
				if(dirty) GetAttributes();
				return Utility.ParseBool(GetAttribute("fence"), false); 
			}
			set 
			{ 
				SetAttribute("fence", value.ToString()); 
			}
		}

		/// <summary>
		/// The separator attribute ("true" or "false") for the mo element, if specified.
		/// </summary>
		public bool Separator
		{
			get 
			{ 
				if(dirty) GetAttributes();
				return Utility.ParseBool(GetAttribute("separator"), false); 
			}
			set { SetAttribute("separator", value.ToString()); }
		}

		/// <summary>
		/// The lspace attribute (spacing to left) of the mo element, if specified.
		/// </summary>
		public Length LSpace
		{
			get 
			{
				if(dirty) GetAttributes();
				string s = GetAttribute("lspace");
				return s.Length > 0 ? Utility.ParseLength(s, op.LSpace) : op.LSpace;
			}
			set 
			{ 
				SetAttribute("lspace", value.ToString()); 
			}
		}

		/// <summary>
		/// The rspace attribute (spacing to right) of the mo element, if specified.
		/// </summary>
		public Length RSpace
		{
			get 
			{
				if(dirty) GetAttributes();
				string s = GetAttribute("rspace");
				return s.Length > 0 ? Utility.ParseLength(s, op.RSpace) : op.RSpace;
			}
			set 
			{ 
				SetAttribute("rspace", value.ToString()); 
			}
		}

		/// <summary>
		/// The stretchy attribute ("true" or "false") for the mo element, if specified.
		/// </summary>
		public bool Stretchy
		{
			get 
			{
				if(dirty) GetAttributes();
				String s = GetAttribute("stretchy");
				return s.Length > 0 ? Utility.ParseBool(s, op.Stretchy) : op.Stretchy;
			}
			set { SetAttribute("stretchy", value.ToString()); }
		}

		/// <summary>
		/// The symmetric attribute ("true" or "false") for the mo element, if specified.
		/// </summary>
		public bool Symmetric
		{
			get 
			{ 
				if(dirty) GetAttributes();
				String s = GetAttribute("symmetric");
				return s.Length > 0 ? Utility.ParseBool(s, op.Symmetric) : op.Symmetric;
			}
			set { SetAttribute("symmetric", value.ToString()); }
		}

		/// <summary>
		/// The maxsize attribute for the mo element, if specified.
		/// </summary>
		public Length MaxSize
		{
			get 
			{
				if(dirty) GetAttributes();
				String s = GetAttribute("maxsize"); 
				return s.Length > 0 ? Utility.ParseLength(s, op.MaxSize) : op.MaxSize;
			}
			set { SetAttribute("maxsize", value.ToString()); }
		}

		/// <summary>
		/// The minsize attribute for the mo element, if specified.
		/// </summary>
		public String MinSize
		{
			get { return GetAttribute("minsize"); }
			set { SetAttribute("minsize", value); }
		}

		/// <summary>
		/// The largeop attribute for the mo element, if specified.
		/// </summary>
		public String LargeOp
		{
			get { return GetAttribute("largeop"); }
			set { SetAttribute("largeop", value); }
		}

		/// <summary>
		/// The movablelimits ("true" or "false") attribute for the mo element, if specified.
		/// </summary>
		/// <remarks>
		/// The movablelimits attribute specifies whether underscripts and overscripts attached 
		/// to this mo element should be drawn as subscripts and superscripts when displaystyle=false. 
		/// movablelimits=false means that underscripts and overscripts should never be drawn as 
		/// subscripts and superscripts. In general, displaystyle is true for displayed mathematics 
		/// and false for inline mathematics. Also, displaystyle is false by default within tables, 
		/// scripts and fractions, and a few other exceptional situations detailed in Section 3.3.4. 
		/// Thus, operators with movablelimits=true will display with limits (i.e. underscripts 
		/// and overscripts) in displayed mathematics, and with subscripts and superscripts in 
		/// inline mathematics, tables, scripts and so on. Examples of operators that typically have
		/// movablelimits=true are sum, prod, and lim. 
		/// </remarks>
		public bool MovableLimits
		{
			get 
			{ 
				if(dirty) GetAttributes();
				string s = GetAttribute("movablelimits");
				return s.Length > 0 ? Utility.ParseBool(s, op.MoveableLimits) : op.MoveableLimits;
			}
			set { SetAttribute("movablelimits", value.ToString()); }
		}

		/// <summary>
		/// The accent attribute ("true" or "false") for the mo element, if specified.
		/// The accent attribute determines whether this operator should be treated by 
		/// default as an accent (diacritical mark) when used as an underscript or overscript; see
		/// munder, mover, and munderover
		/// </summary>
		public bool Accent
		{
			get 
			{ 
				if(dirty) GetAttributes();
				string s = GetAttribute("accent");
				return s.Length > 0 ? Utility.ParseBool(s, op.Accent) : op.Accent;
			}
			set { SetAttribute("accent", value.ToString()); }
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
		/// grab a set of operator attributes from the operator dictionary.
		/// this needs to be done after the object is created as we need to
		/// know if this node is prefix, infix, or postfix, and the prev and
		/// next siblings are not set yet in the ctor
		/// </summary>
		private void GetAttributes()
		{
			if(PreviousSibling != null)
			{
				if(NextSibling != null)
				{
					form = Form.Infix;
				}
				else
				{
					form = Form.Postfix;
				}
			}
			else
			{
				if(NextSibling != null)
				{
					form = Form.Prefix;
				}
				else
				{
					form = Form.Infix;
				}
			}

			if(FirstChild != null)
			{
				MathMLGlyphElement glyph = null;
				if(FirstChild.NodeType == XmlNodeType.Text)
				{
					op = OperatorDictionary.GetValue(FirstChild.Value, form);
				}
				else if((glyph = FirstChild as MathMLGlyphElement) != null)
				{
					string s = new string((char)glyph.Index, 1);
					op = OperatorDictionary.GetValue(s, form);
				}
				else 
				{
					op = Operator.Default(form);
				}
			}
			else
			{
				op = Operator.Default(form);
			}			
			dirty = false;
		}

		/// <summary>
		/// a mathml operator is a 'embelished operator' by rule 1 of the 
		/// definition of an embelished operator.
		/// <see cref="MathMLElement"/>
		/// </summary>
		public override MathMLOperatorElement EmbelishedOperator
		{
			get { return this; }
		}
	}
}
