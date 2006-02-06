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
using System.Drawing;

namespace MathML
{
	/// <summary>
	/// This interface extends the MathMLPresentationContainer interface for 
	/// the MathML enlivening expression element maction.
	/// 
	/// There are many ways in which it might be desirable to make mathematical 
	/// content active. Adding a link to a MathML sub-expression is one basic kind 
	/// of interactivity. See Section 7.1.4. However, many other kinds of 
	/// interactivity cannot be easily accommodated by generic linking mechanisms. 
	/// For example, in lengthy mathematical expressions, the ability to ‘fold’ 
	/// xpressions might be provided, i.e. a renderer might allow a reader to 
	/// toggle between an ellipsis and a much longer expression that it	represents.
	/// 
	/// To provide a mechanism for binding actions to expressions, MathML provides 
	/// the maction element. This element accepts any number of sub-expressions as
	/// arguments.
	/// </summary>
	/// <remarks>
	/// <para>A suggested list of actiontypes and their associated actions is given below. 
	/// Keep in mind, however, that this list is mainly for illustration, and recognized 
	/// values and behaviors will vary from application to application.</para> 
	/// <para><c>&lt;maction actiontype="toggle" selection="positive-integer"&gt;
	/// (first expression) (second expression)... &lt;/maction&gt;</c>
	/// For this action type, a renderer would alternately display the given expressions, 
	/// cycling through them when a reader clicked on the active expression, starting
	/// with the selected expression and updating the selection attribute value as described above. 
	/// Typical uses would be for exercises in education, ellipses in long computer algebra output, 
	/// or to illustrate alternate notations. Note that the expressions may be of significantly 
	/// different size, so that size negotiation with the browser may be desirable. 
	/// If size negotiation is not available, scrolling, elision, panning, or some other method 
	/// may be necessary to allow full viewing.</para>
	/// <para><c>&lt;maction actiontype="statusline"&gt; (expression) (message) &lt;/maction&gt;</c>
	/// In this case, the renderer would display the expression in context on the screen. 
	/// When a reader clicked on the expression or moved the mouse over it, the renderer would send 
	/// a rendering of the message to the browser statusline. Since most browsers in the foreseeable 
	/// future are likely to be limited to 	displaying text on their statusline, authors would 
	/// presumably use plain text in an mtext element for the message in most circumstances. For 
	/// non-mtext messages, renderers might provide a natural language translation of the markup, 
	/// but this is not required.</para>
	/// <para><c>&lt;maction actiontype="tooltip"&gt; (expression) (message) &lt;/maction&gt;</c>
	/// Here the renderer would also display the expression in context on the screen. 
	/// When the mouse pauses over the expression for a long enough delay time,
	/// the renderer displays a rendering of the message in a pop-up ‘tooltip’ box near the expression. 
	/// These message boxes are also sometimes called ‘balloon help’ boxes. Presumably authors would 
	/// use plain text in an mtext element for the message in most circumstances. For non-mtext messages, 
	/// renderers may provide a natural language translation of the markup if full MathML rendering 
	/// is not practical, but this is not required.</para>
	/// <para><c>&lt;maction actiontype="highlight" my:color="red" my:background="yellow"&gt; expression 
	/// &lt;/maction&gt;</c>
	/// In this case, a renderer might highlight the enclosed expression on a ‘mouse-over’ event. In the 
	/// example given above, non-standard attributes from another namespace are being used to pass additional 
	/// information to renderers that support them, without violating the MathML DTD (see Section 7.2.3). The
	/// my:color attribute changes the color of the characters in the presentation, while the my:background 
	/// attribute changes the color of the background behind the characters.</para>
	/// <para><c>&lt;maction actiontype="menu" selection="1" &gt; (menu item 1) (menu item 2) ... &lt;/maction&gt;</c>
	/// This action type instructs a renderer to provide a pop up menu. This allows a one-to-many 
	/// linking capability. Note that the menu items may be other 
	/// <c>&lt;action actiontype="menu"&gt;...&lt;/maction&gt;</c> expressions, thereby allowing nested menus. 
	/// It is assumed that the user choosing a menu item would invoke some kind of action associated 
	/// with that item. Such action might be completely handled by the renderer itself or it might trigger 
	/// some kind of event within the browser that could be linked to other programming logic.</para>
	/// </remarks>
	public class MathMLActionElement : MathMLPresentationContainer
	{
		/// <summary>
		/// creates a new MathMLActionElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLActionElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		/// A string specifying the action. Possible values include toggle, 
		/// statusline, tooltip, highlight, and menu.
		/// </summary>
		public String ActionType
		{
			get { return GetAttribute("actiontype"); }
			set { SetAttribute("actiontype", value); }
		}

		/// <summary>
		/// A string specifying an integer that selects the current subject of the action.
		/// </summary>
		public int Selection
		{
			get { return Utility.ParseInt(GetAttribute("selection"), 0); }
			set { SetAttribute("selection", value.ToString()); }
		}

		/// <summary>
		/// not part of w3c standard, but most renderers, especially design sciences use this attribute
		/// </summary>
		public Color Color
		{
			get 
			{
				// get the dsi color attribue 
				// namespaces suck!!!
				String color = GetAttribute("color");
				if(color == null || color.Length == 0) 
					color = GetAttribute("dsi:color");
				if(color == null || color.Length == 0) 
					color = GetAttribute("dsi:dsi:color");
				return Utility.ParseColor(color); 
			}
			set { SetAttribute("color", value.ToString()); }
		}

		/// <summary>
		/// not part of w3c standard, but most renderers, especially design sciences use this attribute
		/// </summary>
		public Color Background
		{
			get 
			{
				// get the dsi background attribue 
				// namespaces suck!!!
				String color = GetAttribute("background");
				if(color == null || color.Length == 0) 
					color = GetAttribute("dsi:background");
				if(color == null || color.Length == 0) 
					color = GetAttribute("dsi:dsi:background");
				return Utility.ParseColor(color); 
			}
			set { SetAttribute("background", value.ToString()); }
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
