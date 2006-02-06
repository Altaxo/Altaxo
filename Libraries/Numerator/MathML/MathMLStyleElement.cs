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
using System.Drawing;
using System.Xml;


namespace MathML
{
	/// <summary>
	/// This interface extends the MathMLElement interface for the MathML 
	/// style element mstyle. While the mstyle element may contain any 
	/// attributes allowable on any MathML presentation element, only 
	/// attributes specific to the mstyle element are included in the 
	/// interface below. Other attributes should be accessed using
	/// the methods on the base Element class, particularly the 
	/// Element::getAttribute and Element::setAttribute methods, or 
	/// even the Node::attributes attribute to access all of them at 
	/// once. Not only does this obviate a lengthy list below, but 
	/// it seems likely that most implementations will find this a 
	/// considerably more useful interface to a MathMLStyleElement.
	/// </summary>
	public class MathMLStyleElement : MathMLPresentationContainer
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLStyleElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		/// scriptlevel A string of the form ‘+/- unsigned integer ’; 
		/// represents the scriptlevel attribute for the mstyle element,
		/// if specified. See also the discussion of this attribute.
		/// </summary>
		public int ScriptLevel
		{
			get { return Utility.ParseInt(GetAttribute("scriptlevel"), 0); }
			set { SetAttribute("scriptlevel", value.ToString()); }
		}


		/// <summary>
		/// displaystyle Either "true" or "false"; a string representing the displaystyle 
		/// attribute for the mstyle element, if specified. See also the 
		/// discussion of this attribute.
		/// </summary>
		public bool DisplayStyle
		{
			get { return Utility.ParseBool(GetAttribute("displaystyle"), false); }
			set { SetAttribute("displaystyle", value.ToString()); }
		}

		/// <summary> 
		/// A string of the form ‘number’; represents the scriptsizemultiplier 
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public int ScriptSizeMultiplier
		{
			get { return Utility.ParseInt(GetAttribute("scriptsizemultiplier"), 0); }
			set { SetAttribute("scriptsizemultiplier", value.ToString()); }
		}

		/// <summary> 
		/// A string of the form ‘number v-unit’; represents the scriptminsize 
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute
		/// </summary>
		public int ScriptMinSize
		{
			get { return Utility.ParseInt(GetAttribute("scriptminsize"), 0); }
			set { SetAttribute("scriptminsize", value.ToString()); }
		}
		
		/// <summary>
		/// A string representation of a color; represents the color attribute 
		/// for the mstyle element, if specified. See also the discussion of this attribute.
		/// </summary>
		public Color Color
		{
			get 
			{
				string color = HasAttribute("color") ? GetAttribute("color") : GetAttribute("mathcolor");
				return Utility.ParseColor(color); 
			}
			set { SetAttribute("color", value.ToString()); }
		}

		/// <summary>	
		/// A string representation of a color or the string "transparent"; 
		/// represents the background attribute for the mstyle element, 
		/// if specified. See also the discussion of this attribute.
		/// </summary>
		public Color Background
		{
			get 
			{
				string color = HasAttribute("background") ? GetAttribute("background") : GetAttribute("mathbackground");
				return Utility.ParseColor(color); 
			}
			set { SetAttribute("background", value.ToString()); }
		}
		
		/// <summary> 
		/// A string of the form ‘number h-unit’; represents the veryverythinmathspace 
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public String VeryVeryThinMathspace
		{
			get { return GetAttribute("veryverythinmathspace"); }
			set { SetAttribute("veryverythinmathspace", value); }
		}

		/// <summary> 
		/// A string of the form ‘number h-unit’; represents the verythinmathspace 
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public String VeryThinMathspace
		{
			get { return GetAttribute("verythinmathspace"); }
			set { SetAttribute("verythinmathspace", value); }
		}

		/// <summary> A string of the form ‘number h-unit’; 
		/// represents the thinmathspace attribute for the mstyle element,
		/// if specified. See also the discussion of this attribute.
		/// </summary>
		public String ThinMathspace
		{
			get { return GetAttribute("thinmathspace"); }
			set { SetAttribute("thinmathspace", value); }
		}

		/// <summary>
		/// A string of the form ‘number h-unit’; represents the mediummathspace 
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public String MediumMathspace
		{
			get { return GetAttribute("mediummathspace"); }
			set { SetAttribute("mediummathspace", value); }
		}

		/// <summary>
		/// A string of the form ‘number h-unit’; represents the thickmathspace 
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public String ThickMathspace
		{
			get { return GetAttribute("thickmathspace"); }
			set { SetAttribute("thickmathspace", value); }
		}

		/// <summary>
		/// A string of the form ‘number h-unit’; represents the verythickmathspace 
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public String VeryThickMathspace
		{
			get { return GetAttribute("verythickmathspace"); }
			set { SetAttribute("verythickmathspace", value); }
		}

		/// <summary> 
		/// A string of the form ‘number h-unit’; represents the veryverythickmathspace 
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public String VeryVeryThickMathspace
		{
			get { return GetAttribute("veryverythickmathspace"); }
			set { SetAttribute("veryverythickmathspace", value); }
		}

		/// <summary> 
		/// A string of the form ‘number h-unit’; represents the negativeveryverythinmathspace 
		/// attribute for the mstyle element, if specified. See also the discussion of 
		/// this attribute.
		/// </summary>
		public String NegatieVeryVeryThinMathspace
		{
			get { return GetAttribute("negativeveryverythinmathspace"); }
			set { SetAttribute("negativeveryverythinmathspace", value); }
		}

		/// <summary>
		/// A string of the form ‘number h-unit’; represents the negativeverythinmathspace
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public String NegativeVeryThinMathspace
		{
			get { return GetAttribute("negativeverythinmathspace"); }
			set { SetAttribute("negativeverythinmathspace", value); }
		}

		/// <summary> 
		/// A string of the form ‘number h-unit’; represents the negativethinmathspace 
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public String NegativeThinMathspace
		{
			get { return GetAttribute("negativethinmathspace"); }
			set { SetAttribute("negativethinmathspace", value); }
		}

		/// <summary>
		/// A string of the form ‘number h-unit’; represents the negativemediummathspace
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public String NegativeMediumMathspace
		{
			get { return GetAttribute("negativemediummathspace"); }
			set { SetAttribute("negativemediummathspace", value); }
		}

		/// <summary>
		/// A string of the form ‘number h-unit’; represents the negativethickmathspace 
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public String NegativeThickMathspace
		{
			get { return GetAttribute("negativethickmathspace"); }
			set { SetAttribute("negativethickmathspace", value); }
		}

		/// <summary> 
		/// A string of the form ‘number h-unit’; represents the negativeverythickmathspace 
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public String NegativeVeryThickMathspace
		{
			get { return GetAttribute("negativeverythickmathspace"); }
			set { SetAttribute("negativeverythickmathspace", value); }
		}

		/// <summary>
		/// A string of the form ‘number h-unit’; represents the negativeveryverythickmathspace 
		/// attribute for the mstyle element, if specified. See also the discussion 
		/// of this attribute.
		/// </summary>
		public String NegativeVeryVeryThickMathspace
		{
			get { return GetAttribute("negativeveryverythickmathspace"); }
			set { SetAttribute("negativeveryverythickmathspace", value); }
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
