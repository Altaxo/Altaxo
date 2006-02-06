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
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace MathML
{
	/// <summary>
	/// A Reader specific to mathml documents. This is a non-validating reader, 
	/// that automatically resolves entity references without any external DTD 
	/// or schema. mathml entities are either resolved to a standard printing character, 
	/// or to a character in a special reserved block of unicode. If a char is one
	/// of these reserved values, it is the job of the renderer to determine how to
	/// display it.
	/// 
	/// As this reader is very specific to mathml, it is tuned to be very fast, much 
	/// faster than the validating reader, even without any external dtd.
	/// </summary>
	public class MathMLReader : XmlTextReader
	{
		/// <summary>
		/// set whenever a mathml entity is encountered, and unset
		/// after the next node is read.
		/// </summary>
		private string entity = null;

		/// <summary>
		/// pass all standard construction to base class
		/// </summary>
		public MathMLReader() : base() {}

		/// <summary>
		/// pass all standard construction to base class
		/// </summary>
		public MathMLReader(string url, XmlNameTable nt) : base(url, nt) {}

		/// <summary>
		/// pass all standard construction to base class
		/// </summary>
		public MathMLReader(Stream input) : base(input) {}

		/// <summary>
		/// pass all standard construction to base class
		/// </summary>
		public MathMLReader(TextReader input) : base(input) {}
		 

		/// <summary>
		/// if the resolved entity is not, a 'Read' is supposed to bring forth the next
		/// node. The next node was allready brought forth in the NodeType method where
		/// the mathml entity was resolved
		/// </summary>
		/// <returns></returns>

		public override bool Read()
		{
			bool ret;
			if(entity != null)
			{
				// we allready called a 'Read()' in the NodeType property, so if we
				// had a entity, it has allready been read, and the next node is
				// really the current one.
				ret = true;
				entity = null;
			}
			else
			{
				ret = base.Read();
			}
			return ret;
		}

        /// <summary>
        /// skip
        /// </summary>
		public override void Skip()
		{
			if(entity != null)
			{
				Debug.WriteLine("skipping mathml entity");
				entity = null;
			}
			else
			{
				base.Skip();
			}
		}

        /// <summary>
        /// the current node type
        /// </summary>
		public override XmlNodeType NodeType
		{
			get
			{
				// get the real node type
				XmlNodeType type = base.NodeType;

				if(entity == null)
				{
					// only perform special action if we really do have
					// an entity reference, otherwise leave node type allone
					if(type == XmlNodeType.EntityReference)
					{
						// get the entity name
						string name = base.Name;

						// read past the current real entity in the source xml, 
						// this brings the next node as the 'real' active node.
						base.Read();
						
						// set the resolved entity
						entity = EntityDictionary.GetValue(name);
						Debug.WriteLine(String.Format(
							"entity \"{0}\" resolved to \"{1}\", hex value:{2:x}", 
							name, entity, entity.Length > 1 ? entity[0] : 0));
						
						// set node type to text, as the entity was 'resolved' to a 
						// text node.
						type = XmlNodeType.Text;
					}
				}
				else
				{
					type = XmlNodeType.Text;
				}

				return type;
			}
		}

		/// <summary>
		/// entitities are resolved to text nodes, so return the entity
		/// value if it is set.
		/// </summary>
		public override string Value
		{
			get
			{
				return entity != null ? entity : base.Value;
			}
		}

		/// <summary>
		/// we resolve entities to text nodes, so a xml reader should return
		/// an empty string if it is at a text node.
		/// </summary>
		public override string Name
		{
			get
			{
				return entity != null ? String.Empty : base.Name;
			}
		}

		/// <summary> 
		/// same as Name
		/// </summary>
		public override string LocalName
		{
			get
			{
				return entity != null ? String.Empty : base.Name;
			}
		}
	}
}
