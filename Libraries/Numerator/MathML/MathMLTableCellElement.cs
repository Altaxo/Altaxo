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
	/// This interface extends the MathMLPresentationContainer interface for the 
	/// MathML table or matrix cell element mtd.
	/// </summary>
	public class MathMLTableCellElement : MathMLPresentationContainer
	{
		// cache rowspan and columnspan as these are call A LOT
		private int rowspan = -1;
		private int columnspan = -1;

		// set when a list of these is returned from the parent row
		internal Align columnAlign = Align.Center;

		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>        
		public MathMLTableCellElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{			
		}

		/// <summary>
		/// rowspan of type DOMString A string representing a positive integer that specifies the number 
		/// of rows spanned by this cell. The default is 1.
		/// </summary>
		public int RowSpan
		{
			get 
			{
				if(rowspan < 0) rowspan = Utility.ParseInt(GetAttribute("rowspan"), 1);
				return rowspan; 
			}
			set 
			{
				rowspan = value;
				SetAttribute("rowspan", value.ToString()); 
			}
		}

		/// <summary>
		/// columnspan of type DOMString A string representing a positive integer that specifies the number 
		/// of columns spanned by this cell. The default is 1.
		/// </summary>
		public int ColumnSpan
		{
			get 
			{
				if(columnspan < 0) columnspan = Utility.ParseInt(GetAttribute("columnspan"), 1);  
				return columnspan; 
			}
			set 
			{
				columnspan = value;
				SetAttribute("columnspan", value.ToString()); 
			}
		}

		/// <summary>
		/// rowalign of type DOMString A string specifying an override of the inherited vertical 
		/// alignment of this cell within the table row. Allowed values are top, bottom,
		/// center, baseline, and axis.
		/// </summary>
		public Align RowAlign
		{
			get 
			{ 
				Align rowAlign = ((MathMLTableRowElement)ParentNode).RowAlign;
				return Utility.ParseAlign(GetAttribute("rowalign"), rowAlign);
			}
		}

		/// <summary>
		/// columnalign of type DOMString A string specifying an override of the inherited horizontal 
		/// alignment of this cell within the table column. Allowed values are left, center, and right.	
		/// </summary>
		public Align ColumnAlign
		{
			get
			{
				return Utility.ParseAlign(GetAttribute("columnalign"), columnAlign);
			}
			set { SetAttribute("columnalign", Utility.UnparseAlign(value)); }
		}


		/// <summary>
		/// groupalign of type DOMString A string specifying how the alignment groups within the cell are 
		/// to be aligned with those in cells above or below this cell. The string consists of a 
		/// space-separated sequence of specifiers, each of which can have the following values: left, 
		/// right, center, or decimalpoint.
		/// </summary>
		public Align GroupAlign
		{
			get
			{
				Align groupAlign = ((MathMLTableRowElement)ParentNode).GroupAlign;
				return Utility.ParseAlign(GetAttribute("groupalign"), groupAlign);
			}
			set { SetAttribute("groupalign", Utility.UnparseAlign(value)); }
		}

		/// <summary>
		/// hasaligngroups of type boolean, readonly A string with the values true or false indicating 
		/// whether the cell contains align groups.
		/// </summary>
		public bool HasAlignGroups
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// cellindex of type DOMString, readonly A string representing the integer index (1-based?) 
		/// of the cell in its containing row. [What about spanning cells? How do these affect this value?]
		/// </summary>
		public int CellIndex
		{
			get
			{
				return 0;
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
