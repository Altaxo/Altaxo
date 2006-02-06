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
	/// The base of all mathml elements.
	/// </summary>
	/// <remarks>
	/// In order to facilitate use with style sheet mechanisms such as [XSLT] and [CSS2] 
	/// all MathML elements accept class, style, and id attributes in addition to the
	/// attributes described specifically for each element. MathML renderers not supporting 
	/// CSS may ignore these attributes. MathML specifies these attribute values as
	/// general strings, even if style sheet mechanisms have more restrictive syntaxes for them. 
	/// That is, any value for them is valid in MathML.
	/// </remarks>
	public class MathMLElement : XmlElement
	{
		/// <summary>
		/// creates a new MathMLElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLElement(String prefix, String localName, String namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
			// intiialize the user data to be store an area
			userData[0].key = AreaGuid;
		}

		/// <summary>
		/// The class attribute of the element. See the discussion elsewhere in the mathml specificaiton document 
		/// of the class attribute; see also the HTML definition of this attribute. 
		/// </summary>
		public String ClassName
		{
			get {return GetAttribute("class"); }
			set {SetAttribute("class", value); }
		}

		/// <summary>
		/// A string identifying the element’s style attribute.
		/// </summary>
		public String MathElementStyle
		{
			get { return GetAttribute("style"); }
			set { SetAttribute("style", value); }
		}

		/// <summary>
		/// The element’s identifier. See the discussion elsewhere in the mathml specification document of 
		/// the id attribute; see also the HTML definition.
		/// </summary>
		public String Id
		{
			get {return GetAttribute("id");}
			set {SetAttribute("id", value);}
		}

		/// <summary>
		/// The xref attribute of the element. See the discussion elsewhere in mathml specification document 
		/// of the xref attribute.
		/// </summary>
		public String Xref
		{
			get {return GetAttribute("xref");}
			set {SetAttribute("xref", value);}
		}

		/// <summary>
		/// The xlink:href attribute of the element. See the discussion elsewhere in the mathml specification 
		/// document of the xlink:href attribute; see also the definition of this attribute in the XLink specification.
		/// </summary>
		public String Href
		{
			get {return GetAttribute("xlink:href");}
			set {SetAttribute("xlink:href", value);}
		}

		/// <summary>
		/// The MathMLMathElement corresponding to the nearest math element ancestor of this element.
		/// Should be null if this element is a top-level math element.
		/// </summary>
		public MathMLMathElement OwnerMathElement
		{
			get 
			{
				XmlNode parent = ParentNode;
				while(parent != null)
				{
					if(parent is MathMLMathElement) 
					{
						return (MathMLMathElement)parent;
					}
					else
					{
						parent = parent.ParentNode;
					}
				}
				return null;
			}
		}
	
		/// <summary>
		/// An extension to the w3c published DOM specification, this is NOT
		/// in the published spec. 
		/// 
		/// An mo element that is ‘embellished’ by one or more nested subscripts, superscripts, 
		/// surrounding text or whitespace, or style changes is an embellished operator. 
		/// The embellished operator as a whole (this is defined precisely, below) whose
		/// position in an mrow is examined by the above rules and whose surrounding spacing is 
		/// affected by its form, not the mo element at its core; however, the attributes
		/// influencing this surrounding spacing are taken from the mo element at the core 
		/// (or from that element’s dictionary entry). 
		/// 
		/// <pre>
		/// The precise definition of an ‘embellished operator’ is: 
		/// 1: an mo element;
		/// 2: one of the elements msub, msup, msubsup, munder, mover, munderover, mmultiscripts, 
		///    mfrac, or semantics (Section 4.2.6), whose first argument exists and is an embellished 
		///    operator;
		/// 3: one of the elements mstyle, mphantom, or mpadded, such that an mrow containing the 
		///    same arguments would be an embellished operator;
		/// 4: an maction element whose selected sub-expression exists and is an embellished operator;
		/// 5: an mrow whose arguments consist (in any order) of one embellished operator and zero or 
		///    more space-like elements.
		/// </pre>
		///   
		/// Note that this definition permits nested embellishment only when there are no intervening 
		/// enclosing elements not in the above list.
		/// 
		/// returns null if this is not an embelished operator, the operator that is being
		/// embelished otherwise.
		/// </summary>
		public virtual MathMLOperatorElement EmbelishedOperator
		{
			get {return null;}
		}

		/// <summary>
		/// A way for a node to process a visitor. The visitor pattern allows the development
		/// of a wide variety of actions (rendering, compiling, etc...) to be applied to a 
		/// node hierachy without adding code to the actuall MathMLElements, thus keeping the
		/// element hierarchy pure and conforming to the published specification.
		/// 
		/// This is an extension to the w3c published specification.
		/// The descision to include this method was not taken lightly, as one of the goals
		/// of this probject is to implement the offcial w3c spec as closely as possible.
		/// </summary>
		public virtual object Accept(MathMLVisitor v, object args) 
		{
			System.Diagnostics.Debug.WriteLine(string.Format("Warning, {0}.Accept() is not implemented", 
				GetType().ToString()));
			return null;
		}
		

		/// <summary>
		/// setUserData introduced in DOM Level 3
		/// Associate an object to a key on this node. The by calling getUserData with the same key.
		/// </summary>
		/// <param name="key">The key to associate the object to.</param>
		/// <param name="data">The object to associate to the given key, or null to remove 
		/// any existing association to that key.</param>
		/// <param name="handler">The handler to associate to that key, or null.</param>
		/// <returns>the user data previously associated to the given key on this node, or 
		/// null if there was none.</returns>
		public object SetUserData(string key, object data, UserDataHandler handler)
		{
			for(int i = 0; i < userData.Length; i++)
			{
				if(userData[i].key == key)
				{
					object result = userData[i].data;
					userData[i].data = data;
					userData[i].handler = handler;
					return result;					
				}
			}
			UserDataItem[] newUserData = new UserDataItem[userData.Length + 1];
			for(int i = 0; i < userData.Length; i++)
			{
				newUserData[i] = userData[i];
			}
			newUserData[newUserData.Length - 1].key = key;
			newUserData[newUserData.Length - 1].data = data;
			newUserData[newUserData.Length - 1].handler = handler;
			userData = newUserData;
			return data;
		}

		/// <summary>
		/// getUserData introduced in DOM Level 3
		/// Retrieves the object associated to a key on a this node. The object must first have 
		/// been set to this node by calling setUserData with the same key.
		/// </summary>
		/// <param name="key">The key the object is associated to.</param>
		/// <returns>the DOMUserData associated to the given key on this node, 
		/// or null if there was none.</returns>
		public object GetUserData(string key)
		{
			for(int i = 0; i < userData.Length; i++)
			{
				if(userData[i].key == key) return userData[i].data;
			}
			return null;
		}

		/// <summary>
		/// internal method to find a child node with the given tag name.
		/// this method should be faster than the SelectSingleNode method, as
		/// there is no xpath expression to parse. In MathML, most relationships
		/// are to first level child nodes, so this is an optimization.
		/// </summary>
		/// <param name="tagName">The tag name to search for</param>
		/// <returns>The first child node matching the tag name, null otherwise</returns>
		internal XmlNode SelectChildNode(string tagName)
		{
			for(XmlNode node = FirstChild; node != null; node = node.NextSibling)
			{
				if(node.Name == tagName)
				{
					return node;
				}
			}
			return null;
		}
		
		// string to identifiy an area in the MathMLElement node
		private const string AreaGuid = "DEF47EE-E23A-11D3-B4D0-8208CCE0C829";

		// struct to hold user data items.
		private struct UserDataItem
		{
			public string key;
			public object data;
			public UserDataHandler handler;
		}

		// array of items. start with length one to accomodata MathML.Rendering.Area
		private UserDataItem[] userData = new UserDataItem[1]; 
	}
}
