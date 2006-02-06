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
	/// This interface extends the MathMLPresentationElement interface to include 
	/// access for attributes specific to text presentation. It serves as the 
	/// base class for all MathML presentation token elements. Access to the body 
	/// of the element is via the Value attribute inherited from Node. Elements 
	/// that expose only the core presentation token attributes are directly 
	/// supported by this object. These elements are:
	/// 
	/// mi identifier element
	/// mn number element
	/// mtext text element
	/// </summary>
	public class MathMLPresentationToken : MathMLPresentationElement
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLPresentationToken(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		/// The mathvariant attribute for the element, if specified. One of the values 
		/// "normal", "bold", "italic", "bold-italic", "double-struck", "bold-fraktur", 
		/// "script", "bold-script", "fraktur", "sans-serif", "bold-sans-serif", 
		/// "sans-serif-italic", "sans-serif-bold-italic", or "monospace". 
		/// 
		/// if no attribute exists, the default is evaluated per the mathml spec (mi element)
		/// </summary>
		public MathVariant MathVariant
		{
			get 
			{
				String attr = GetAttribute("mathvariant"); 
				if(attr.Length == 0)
				{
					if(Name == "mi" && InnerText.Length == 1)
					{
                        attr = "italic";
					}
					else
					{
						attr = "normal";
					}
				}
				return StringToVariant(attr);
			}
			set { SetAttribute("mathvariant", variantMap[(int)value]); }
		}

		/// <summary> 
		/// The mathsize attribute for the element, if specified. Either "small", 
		/// "normal" or "big", or of the form "number v-unit".
		/// </summary>
		public Length MathSize
		{
			get { return Utility.ParseLength(GetAttribute("mathsize"), new Length(LengthType.Normal)); }
			set { SetAttribute("mathsize", value.ToString()); }
		}

		/// <summary>
		/// The mathcolor attribute for the element, if specified. The String returned 
		/// should be in one of the forms "#rgb" or "#rrggbb", or should be an html-color-name, 
		/// as specified in Section 3.2.2.2.
		/// </summary>
		public Color MathColor
		{
			get 
			{
				string color = HasAttribute("mathcolor") ? GetAttribute("mathcolor") : GetAttribute("color");
				return Utility.ParseColor(color); 
			}
			set { SetAttribute("mathcolor", value.ToString()); }
		}

		/// <summary>
		/// The mathbackground attribute for the element, if specified. 
		/// The String returned should be in one of the forms "#rgb" or "#rrggbb", 
		/// or should be an html-color-name, as specified in Section 3.2.2.2.
		/// </summary>
		public Color MathBackground
		{
			get 
			{
				string color = HasAttribute("mathbackground") ? GetAttribute("mathbackground") :
					GetAttribute("background");
				return Utility.ParseColor(color); 
			}
			set { SetAttribute("mathbackground", value.ToString()); }
		}

		/// <summary>
		/// Returns the child Nodes of the element. These should consist only of 
		/// Text nodes, MathMLGlyphElements, and MathMLAlignMarkElements. 
		/// Should behave the same as the base class’s Node::childNodes
		/// attribute; however, it is provided here for clarity.
		/// </summary>

		public MathMLNodeList Contents
		{
			get { return new MathMLPresentationTokenNodeList(this); }
		}	
		
		/// <summary>
		/// map a math variant enum to its' text representation
		/// </summary>		
		private static readonly String[] variantMap = 
		{
			"normal", "bold", "italic", "bold-italic", "double-struck", "bold-fraktur",
			"script", "bold-script", "fraktur", "sans-serif", "bold-sans-serif",
			"sans-serif-italic", "sans-serif-bold-italic", "monospace"
		};

		private static MathVariant StringToVariant(String str)
		{
			for(int i = 0; i < variantMap.Length; i++)
			{
				if(str == variantMap[i]) return (MathVariant)i;
			}
			return MathVariant.Unknown;
		}

		/// <summary>
		/// finds the style element for this node if one exists, 
		/// returns null otherwise.
		/// 
		/// note, it is possible to override a method in MathMLStyleElement
		/// to set some paramenters whenever a new child node is added to it, 
		/// but this method is internal to System.Xml, and we can not count
		/// on it being there in non-ms .net runtimes. Besides, searching 
		/// throught the parent list should not be that much slower, and it
		/// is easier to implement.
		/// </summary>
		internal MathMLStyleElement Style
		{
			get
			{
				XmlNode parent = this;
				while((parent = parent.ParentNode) != null)
				{
					MathMLStyleElement style = parent as MathMLStyleElement;
					if(style != null) return style;
				}
				return null;
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
