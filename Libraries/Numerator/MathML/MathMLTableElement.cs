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

namespace MathML
{
	/// <summary>
	/// Hold the parsed values of a 'mtable' element's 'align' attribute.
	/// </summary>
	public struct TableAlign
	{
		/// <summary>
		/// create a TableAlign with the following values
		/// </summary>
		/// <param name="align"></param>
		/// <param name="rownumber"></param>
		public TableAlign(Align align, int rownumber)
		{ 
			Align = align;
			RowNumber = rownumber;
		}

		/// <summary>
		/// the align attribute
		/// </summary>
		public Align Align;

		/// <summary>
		/// the row number if specified
		/// </summary>
		public int RowNumber;
	}

	/// <summary>
	/// This interface extends the MathMLPresentationElement interface 
	/// for the MathML table or matrix element mtable.
	/// </summary>
	public class MathMLTableElement : MathMLPresentationElement
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLTableElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
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
		/// The vertical alignment of the table with the adjacent text. Allowed values are 
		/// (top | bottom | center | baseline | axis)[rownumber], where rownumber is between 1 
		/// and n (for a table with n rows) or -1 and -n.
		/// </summary>
		/// <remarks>
		/// The align attribute specifies where to align the table with respect to its environment. 
		/// axis means to align the center of the table on the environment’s axis. (The axis of an 
		/// equation is an alignment line used by typesetters. It is the line on which a minus sign 
		/// typically lies. The center of the table is the midpoint of the table’s vertical extent.) 
		/// center and baseline both mean to align the center of the table on the environment’s 
		/// baseline. top or bottom aligns the top or bottom of the table on the environment’s 
		/// baseline. If the align attribute value ends with a rownumber between 1 and n (for a 
		/// table with n rows), the specified row is aligned in the way described above, rather than
		/// the table as a whole; the top (first) row is numbered 1, and the bottom (last) row is 
		/// numbered n. The same is true if the row number is negative, between -1 and -n, except 
		/// that the bottom row is referred to as -1 and the top row as -n. Other values of rownumber 
		/// are illegal.
		/// </remarks>
		public TableAlign Align
		{
			get 
			{
				string s = GetAttribute("align");
				TableAlign result = new TableAlign(MathML.Align.Axis, 0);
				try
				{					
					if(s.StartsWith("top"))
					{
						result = new TableAlign(MathML.Align.Top, Int32.Parse(s.Substring(3)));
					}
					else if(s.StartsWith("bottom"))
					{
						result = new TableAlign(MathML.Align.Bottom, Int32.Parse(s.Substring(6)));
					}
					else if(s.StartsWith("center"))
					{
						result = new TableAlign(MathML.Align.Center, Int32.Parse(s.Substring(6)));
					}
					else if(s.StartsWith("baseline"))
					{
						result = new TableAlign(MathML.Align.Baseline, Int32.Parse(s.Substring(8)));
					}
				} 
				catch(Exception) 
				{
					Debug.WriteLine("Warning, \"" + s + "\" is not a valid value for align");
				}

				return result;
			}
			set 
			{ 
				SetAttribute("align", Utility.UnparseAlign(value.Align) + value.RowNumber.ToString()); 
			}
		}

		/// <summary>
		/// A string representing the alignment of entries in each row, 
		/// consisting of a space-separated sequence of alignment specifiers, each of which can have the 
		/// following values: top, bottom, center, baseline, or axis.
		/// This method returns a list of align elements, one for each align element in
		/// the rowalign attribute. If there is no rowalign attribute, a list of length one
		/// containing the default value (baseline) is returned.
		/// </summary>
		/// <remarks>
		/// The rowalign attribute specifies how the entries in each row should be aligned. For example, 
		/// top means that the tops of each entry in each row should be aligned with the tops of the other 
		/// entries in that row. The columnalign attribute specifies how the entries in each column 
		/// should be aligned. 
		/// If there are more entries than are necessary (e.g. more entries than columns for columnalign),
		/// then only the first entries will be used. If there are fewer entries, then the last entry is 
		/// repeated as often as necessary. For example, if columnalign="right center" and the table has 
		/// three columns, the first column will be right aligned and the second and third columns will 
		/// be centered.
		/// </remarks>
		public Align[] RowAlign
		{
			get { return Utility.ParseAligns(GetAttribute("rowalign"), new MathML.Align[] {MathML.Align.Baseline}); }
			set { SetAttribute("rowalign", Utility.UnparseAligns(value)); }
		}

		/// <summary>
		/// columnalign of type DOMString A string representing the alignment of entries in each column, 
		/// consisting of a space-separated sequence of alignment specifiers, each of which can have the 
		/// following values: left, center, or right. The columnalign attribute specifies how the entries 
		/// in each column should be aligned.
		/// This method returns a list of align elements, one for each align element in
		/// the rowalign attribute. If there is no rowalign attribute, a list of length one
		/// containing the default value (baseline) is returned.
		/// </summary>
		/// <remarks>
		/// If there are more entries than are necessary (e.g. more entries than columns for columnalign),
		/// then only the first entries will be used. If there are fewer entries, then the last entry is 
		/// repeated as often as necessary. For example, if columnalign="right center" and the table has 
		/// three columns, the first column will be right aligned and the second and third columns will 
		/// be centered.
		/// </remarks>
		public Align[] ColumnAlign
		{
			get { return Utility.ParseAligns(GetAttribute("columnalign"), new MathML.Align[] {MathML.Align.Center}); }
			set { SetAttribute("columnalign", Utility.UnparseAligns(value)); }
		}

		/// <summary>
		/// A string specifying how the alignment groups within the cells of each row are to be aligned with 
		/// the corresponding items above or below them in the same column. The string consists of a 
		/// sequence of braced group alignment lists. Each group alignment list is a space-separated 
		/// sequence, each of which can have the following values: left, right, center, or decimalpoint.
		/// This method returns a list of align elements, one for each align element in
		/// the rowalign attribute. If there is no rowalign attribute, a list of length one
		/// containing the default value (baseline) is returned.
		/// </summary>
		/// <remarks>
		/// If there are more entries than are necessary (e.g. more entries than columns for columnalign),
		/// then only the first entries will be used. If there are fewer entries, then the last entry is 
		/// repeated as often as necessary. For example, if columnalign="right center" and the table has 
		/// three columns, the first column will be right aligned and the second and third columns will 
		/// be centered.
		/// </remarks>		
		public Align[] GroupAlign
		{
			get { return Utility.ParseAligns(GetAttribute("groupalign"), new MathML.Align[] {MathML.Align.Left}); }
			set { SetAttribute("groupalign", Utility.UnparseAligns(value)); }
		}

		/// <summary>
		/// A string consisting of the values true or false indicating, for each column, whether it can 
		/// be used as an alignment scope.
		/// </summary>
		/// <remarks>
		/// If there are more entries than are necessary (e.g. more entries than columns for columnalign),
		/// then only the first entries will be used. If there are fewer entries, then the last entry is 
		/// repeated as often as necessary. For example, if columnalign="right center" and the table has 
		/// three columns, the first column will be right aligned and the second and third columns will 
		/// be centered.
		/// </remarks>
		public bool[] AlignmentScope
		{
			get { return Utility.ParseBoolList(GetAttribute("alignmentscope"), true); }
			set { SetAttribute("alignmentscope", Utility.UnparseBoolList(value)); }
		}

		/// <summary>
		/// A string consisting of a space-separated sequence of specifiers, 
		/// each of which can have one of the following forms: auto, number h-unit, namedspace, or fit. 
		/// (A value of the form namedspace is one of veryverythinmathspace, verythinmathspace, 
		/// thinmathspace, mediummathspace, thickmathspace, verythickmathspace, or veryverythickmathspace.) 
		/// This represents the element’s columnwidth attribute. 
		/// </summary>
		/// <remarks>
		/// The columnwidth attribute specifies how wide a column should be. The "auto" value means 
		/// that the column should be as wide as needed, which is the default. If an explicit value 
		/// is given, then the column is exactly that wide and the contents of that column are made 
		/// to fit in that width. The contents are linewrapped or clipped at the discretion of the 
		/// renderer. If "fit" is given as a value, the remaining page width after subtracting the 
		/// widths for columns specified as "auto" and/or specific widths is divided equally among 
		/// the "fit" columns and this value is used for the column width. If insufficient room 
		/// remains to hold the contents of the "fit" columns, renderers may linewrap or clip the 
		/// contents of the "fit" columns. When the columnwidth is specified as a percentage, the 
		/// value is relative to the width of the table. That is, a renderer should try to adjust the 
		/// width of the column so that it covers the specified percentage of the entire table width.
		/// </remarks>
		public Length[] ColumnWidth
		{
			get 
			{ 
				return Utility.ParseLengths(GetAttribute("columnwidth"), 
					new Length[] {new Length(LengthType.Auto)});
			}
			set 
			{ 
				SetAttribute("columnwidth", Utility.UnparseLengths(value));
			}
		}

		/// <summary>
		/// width of type DOMString A string that is either of the form number h-unit or is the string auto. 
		/// This represents the element’s width attribute.
		/// </summary>
		public Length Width
		{
			get 
			{ 
				return Utility.ParseLength(GetAttribute("width"), new Length(LengthType.Auto)); 
			}
			set 
			{ 
				SetAttribute("width", value.ToString());
			}
		}

		/// <summary>
		/// rowspacing of type DOMString A string consisting of a space-separated sequence of specifiers of the 
		/// form number v-unit representing the space to be added between rows.
		/// </summary>
		public Length[] RowSpacing 
		{
			get 
			{ 
				return Utility.ParseLengths(GetAttribute("rowspacing"), 
					new Length[] {new Length(LengthType.Ex, 1.0f)});
			}
			set 
			{ 
				SetAttribute("rowspacing", Utility.UnparseLengths(value));
			}
		}

		/// <summary>
		/// columnspacing of type DOMString A string consisting of a space-separated sequence of 
		/// specifiers of the form number h-unit representing the space to be added between columns.
		/// </summary>
		public Length[] ColumnSpacing
		{
			get 
			{ 
				return Utility.ParseLengths(GetAttribute("columnspacing"), 
					new Length[] {new Length(LengthType.Em, 0.8f)});
			}
			set 
			{ 
				SetAttribute("columnspacing", Utility.UnparseLengths(value));
			}
		}

		/// <summary>
		/// rowlines of type DOMString A string specifying whether and what kind of lines 
		/// should be added between each row. The string consists of a space-separated
		/// sequence of specifiers, each of which can have the following values: none, 
		/// solid, or dashed.
		/// </summary>
		public LineStyle[] RowLines
		{
			get 
			{
				return Utility.ParseLinestyles(GetAttribute("rowlines"), 
					new LineStyle[] { LineStyle.None } ); 
			}
			set
			{
				SetAttribute("rowlines", Utility.UnparseLinestyles(value)); 
			}
		}

		/// <summary>
		/// columnlines of type DOMString A string specifying whether and what kind of 
		/// lines should be added between each column. The string consists of a spaceseparated
		/// sequence of specifiers, each of which can have the following values: none, 
		/// solid, or dashed.
		/// </summary>
		public LineStyle[] ColumnLines
		{
			get 
			{
				return Utility.ParseLinestyles(GetAttribute("columnlines"), 
					new LineStyle[] { LineStyle.None } ); 
			}
			set
			{
				SetAttribute("columnlines", Utility.UnparseLinestyles(value)); 
			}
		}

		/// <summary>
		/// frame of type DOMString A string specifying a frame around the table. 
		/// Allowed values are (none | solid | dashed).
		/// </summary>
		public LineStyle Frame
		{
			get { return Utility.ParseLinestyle(GetAttribute("frame"), LineStyle.None); }
			set { SetAttribute("frame", Utility.UnparseLinestyle(value)); }

		}

		/// <summary>
		/// A string of the form number h-unit number v-unit specifying the spacing between 
		/// table and its frame. This property returns 2 Length types, in the order that
		/// they were specified in the original attribute string. If the attribute string
		/// contains more than 2 elements, all of them are returned, it is up to the 
		/// renderer to only accept the first 2.
		/// </summary>
		public Length[] FrameSpacing
		{
			get 
			{ 
				return Utility.ParseLengths(GetAttribute("framespacing"), 
					new Length[] {new Length(LengthType.Em, 0.4f), new Length(LengthType.Ex, 0.5f)});
			}
			set { SetAttribute("framespacing", Utility.UnparseLengths(value));}
		}

		/// <summary>
		/// equalrows of type DOMString A string with the values true or false.
		/// </summary>
		public bool EqualRows
		{
			get { return Utility.ParseBool(GetAttribute("equalrows"), false); }
			set { SetAttribute("equalrows", Utility.UnparseBool(value)); }
		}

		/// <summary>
		/// equalcolumns of type DOMString A string with the values true or false.
		/// </summary>
		public bool EqualColumns
		{
			get { return Utility.ParseBool(GetAttribute("equalcolumns"), false); }
			set { SetAttribute("equalcolumns", Utility.UnparseBool(value)); }
		}

		/// <summary>
		/// displaystyle of type DOMString A string with the values true or false.
		/// </summary>
		public bool DisplayStyle
		{
			get { return Utility.ParseBool(GetAttribute("displaystyle"), false); }
			set { SetAttribute("displaystyle", Utility.UnparseBool(value)); }
		}

		/// <summary>
		/// side of type DOMString A string with the values left, right, leftoverlap, 
		/// or rightoverlap.
		/// </summary>
		public string Side
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// minlabelspacing of type DOMString A string of the form number h-unit, specifying 
		/// the minimum space between a label and the adjacent entry in the labeled																																						 row.
		/// </summary>
		public Length MinLabelSpacing
		{
			get { return Utility.ParseLength(GetAttribute("minlabelspacing"), new Length(LengthType.Em, 0.8f));}
			set { SetAttribute("minlabelspacing", value.ToString());}
		}

		/// <summary>
		/// rows of type MathMLNodeList, readonly A MathMLNodeList consisting of MathMLTableRowElements 
		/// and MathMLLabeledRowElements representing the rows of the table. This is a live object.
		/// </summary>
		public MathMLNodeList Rows
		{
			get
			{
				MathMLNodeList childNodes =  new MathMLTableNodeList(ChildNodes);
				Align[] rowalign = RowAlign;
				Align[] groupalign = GroupAlign;

				for(int i = 0; i < childNodes.Count; i++)
				{
					MathMLTableRowElement row = (MathMLTableRowElement)childNodes[i];
					row.rowAlign = i < rowalign.Length ? rowalign[i] : rowalign[rowalign.Length - 1];
					row.groupAlign = i < groupalign.Length ? groupalign[i] : groupalign[groupalign.Length - 1];
				}
				return childNodes;
			}
		}
	}
}
