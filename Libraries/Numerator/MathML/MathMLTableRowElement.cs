using System;
using System.Xml;


namespace MathML
{
	/// <summary>
	/// This interface extends the MathMLPresentationElement interface 
	/// for the MathML table or matrix row element mtr.
	/// </summary>
	public class MathMLTableRowElement : MathMLPresentationElement
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLTableRowElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

        // set up be parent table element, these are the default values inherited
		// from the parent table
		internal Align rowAlign = Align.Baseline;
		internal Align groupAlign = Align.Left;

		/// <summary>
		/// rowalign of type DOMString A string representing an override of the row alignment specified 
		/// in the containing mtable. Allowed values are top, bottom, center, baseline, and axis.
		/// </summary>
		public Align RowAlign
		{
			get { return Utility.ParseAlign(GetAttribute("rowalign"), rowAlign); }
			set { SetAttribute("rowalign", Utility.UnparseAlign(value)); }
		}

		/// <summary>
		/// columnalign of type DOMString A string representing an override of the column alignment 
		/// specified in the containing mtable. Allowed values are left, center, and right.
		/// </summary>
		public Align[] ColumnAlign
		{
			get { return Utility.ParseAligns(GetAttribute("columnalign"), ((MathMLTableElement)ParentNode).ColumnAlign); }
			set { SetAttribute("columnalign", Utility.UnparseAligns(value)); }
		}

		/// <summary>
		/// groupalign of type DOMString [To be changed?]
		/// </summary>
		public Align GroupAlign
		{
			get { return Utility.ParseAlign(GetAttribute("groupalign"), groupAlign); }
			set { SetAttribute("groupalign", Utility.UnparseAlign(value)); }
		}

		/// <summary>
		/// cells of type MathMLNodeList, readonly A MathMLNodeList consisting of the cells of the row. 
		/// Note that this does not include the label if this is a MathMLLabeledRowElement!
		/// </summary>
		public MathMLNodeList Cells
		{
			get
			{
				int firstChild = Name == "mlabeledtr" ? 1 : 0;
				MathMLTableNodeList childNodes = new MathMLTableNodeList(ChildNodes, firstChild);
				Align[] columnAlign = ColumnAlign;

				for(int i = 0; i < childNodes.Count; i++)
				{
					MathMLTableCellElement cell = childNodes[i] as  MathMLTableCellElement;
					if(cell == null)
					{
						XmlNode oldChild = childNodes[i];
						cell = (MathMLTableCellElement)OwnerDocument.CreateElement("mtd");
						ReplaceChild(cell, childNodes[i]);
						cell.AppendChild(oldChild);
					}
					cell.columnAlign = i < columnAlign.Length ? columnAlign[i] : columnAlign[columnAlign.Length - 1];
				}
				return childNodes;
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
