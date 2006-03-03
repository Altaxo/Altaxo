//This file is part of MathML.Rendering, a library for displaying mathml
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
//(slightly obfuscated for spam mail harvesters)
//andy[at]epsilon3[dot]net

using System;
using System.Diagnostics;
using System.Xml;

namespace MathML.Rendering
{
	/// <summary>
	/// Summary description for MathMLElementFinder.
	/// </summary>
	public class MathMLElementFinder : MathMLVisitor
	{
		// mathml node selection type
		private enum SelectionType
		{
			// seek the previous node, args is the current child node, method should return the
			// node just before args
			Prev, 
			
			// seek the next nodd. args is the current child node, method should return the 
			// node just after args
			Next, 
			
			// get the start of the current node. args is null, method should return the first 
			// child node, or itself it there are no child nodes
			Start, 
			
			// get the end of the current node. args is null, method sould return the last child 
			// node, or itself if there are no child nodes.
			End
		}
		
		// get the main selection
		private SelectionType selection;

		// index of current token string character
		private int index;

		public MathMLElementFinder()
		{
		}

		/// <summary>
		/// get an mathml element with the given relationship to the current element
		/// </summary>
		public Selection GetNextSelection(MathMLElement element, int i)
		{
			Selection result = null;
			selection = SelectionType.Next;
			index = i;

			MathMLElement e = (MathMLElement)element.Accept(this, element);

			if(e != null)
			{
				result = new Selection();
				result.Element = e;
				result.CharIndex = index;
			}

			return result;
		}

		/// <summary>
		/// get an mathml element with the given relationship to the current element
		/// </summary>
		public Selection GetPrevSelection(MathMLElement element, int i)
		{
			Selection result = null;
			selection = SelectionType.Prev;
			index = i;

			MathMLElement e = (MathMLElement)element.Accept(this, element);

			if(e != null)
			{
				result = new Selection();
				result.Element = e;
				result.CharIndex = index;
			}

			return result;
		}

		object MathML.MathMLVisitor.Visit(MathMLSeparator e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLPresentationContainer e, object args)
		{
			MathMLElement c = (MathMLElement)args;
			MathMLElement elm = null;
			switch(selection)
			{
				case SelectionType.Prev:
				{
					if((elm = PreviousMathSibling(c)) != null)
					{
						selection = SelectionType.End;
						return elm.Accept(this, null);
					}
					else
					{
						MathMLElement p = e.ParentNode as MathMLElement;
						return p != null ? p.Accept(this, e) : null;
					}
				}
				case SelectionType.Next:
				{					
					if(e == c && (elm = e.FirstChild as MathMLElement) != null)
					{						
						return elm.Accept(this, e);
					}
					else if((elm = NextMathSibling(c)) != null)
					{
						selection = SelectionType.Start;
						return elm.Accept(this, null);
					}
					else
					{
						MathMLElement p = e.ParentNode as MathMLElement;
						return p != null ? p.Accept(this, e) : null;
					}
				}
				case SelectionType.Start:
				{
					elm = e.FirstChild as MathMLElement;
					return elm != null ? elm.Accept(this, null) : e;
				} 
				case SelectionType.End:
				{
					elm = e.LastChild as MathMLElement;
					return elm != null ? elm.Accept(this, null) : null;
				}
				default:
				{
					return null;
				}
			}
		}

		object MathML.MathMLVisitor.Visit(MathMLPresentationToken e, object args)
		{
			MathMLElement p = null;
			switch(selection)
			{
				case SelectionType.Prev:
				{
					if(index > 1)
					{
						index--;
						return e;
					}
					else if(index == 1)
					{
						if((e.ParentNode is MathMLPresentationContainer && !(PreviousMathSibling(e) is MathMLPresentationToken)) || 
							(!(e.ParentNode is MathMLPresentationContainer)))
						{
							index--;
							return e;
						}
						else
						{
							return ((MathMLElement)e.ParentNode).Accept(this, e);
						}
					}
					else if(e.ParentNode is MathMLPresentationContainer && (p = PreviousMathSibling(e) as MathMLPresentationToken) != null)
					{
						selection = SelectionType.End;
						return p.Accept(this, null);
					}
					else
					{
						return ((MathMLElement)e.ParentNode).Accept(this, e);
					}
				} 
				case SelectionType.Next:
				{
					if(e.ChildNodes.Count == 1 && e.FirstChild.NodeType == XmlNodeType.Text && index < e.FirstChild.Value.Length)
					{
						index++;
						return e;
					}
					else if((e.ParentNode is MathMLPresentationContainer || e.ParentNode is MathMLMathElement) 
						&& (p = NextMathSibling(e) as MathMLPresentationToken) != null)
					{
						selection = SelectionType.Start;
						index = 0;
						return p.Accept(this, null);
					}
					else
					{
						return ((MathMLElement)e.ParentNode).Accept(this, e);
					}
				}
				case SelectionType.End:
				{
					index = e.FirstChild.Value.Length;
					return e;
				}
				case SelectionType.Start:
				default: 
				{
					if(((e.ParentNode is MathMLPresentationContainer || e.ParentNode is MathMLMathElement) 
						&& !(PreviousMathSibling(e) is MathMLPresentationToken)) || 
						(!(e.ParentNode is MathMLPresentationContainer || e.ParentNode is MathMLMathElement)))
					{
						index = 0;
					}
					else
					{
						index = 1;
					}
					return e;
				}
			}
		}

		object MathML.MathMLVisitor.Visit(MathMLXMLAnnotationElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLSemanticsElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLUnderOverElement e, object args)
		{
			MathMLElement c = (MathMLElement)args;
			switch(selection)
			{
				case SelectionType.Prev:
				{
					if(c == e.UnderScript || c == e.OverScript)
					{
						selection = SelectionType.End;
						return e.Base.Accept(this, e);
					}
					else
					{
						MathMLElement p = e.ParentNode as MathMLElement;
						return p != null ? p.Accept(this, e) : null;
					}                
				} 
				case SelectionType.Next:
				{
					if(c == e.Base)
					{
						MathMLElement next = e.OverScript != null ? e.OverScript : e.UnderScript;
						selection = SelectionType.Start;
						return next != null ? next.Accept(this, null) : null;
					}
					else
					{
						MathMLElement p = e.ParentNode as MathMLElement;
						return p != null ? p.Accept(this, e) : null;
					}
				}
				case SelectionType.End:
				{
					MathMLElement end = e.OverScript != null ? e.OverScript : 
						(e.UnderScript != null ? e.UnderScript : e.Base);
					return end.Accept(this, null);
				}
				case SelectionType.Start:
				{
					return e.Base.Accept(this, null);
				}
				default:
				{
					return null;
				}
			}
		}

		object MathML.MathMLVisitor.Visit(MathMLLabeledRowElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLTableElement e, object args)
		{
			if(selection == SelectionType.Prev || selection == SelectionType.Next)
			{
				return ((MathMLElement)e.ParentNode).Accept(this, e);
			}
			else
			{
				MathMLElement elm = e.FirstChild as MathMLElement;
				return elm != null ? elm.Accept(this, args) : e;
			}
		}

		object MathML.MathMLVisitor.Visit(MathMLTableRowElement e, object args)
		{
			if(selection == SelectionType.Prev || selection == SelectionType.Next)
			{
				return ((MathMLElement)e.ParentNode).Accept(this, e);
			}
			else
			{
				MathMLElement elm = e.FirstChild as MathMLElement;
				return elm != null ? elm.Accept(this, args) : e;
			}
		}

		object MathML.MathMLVisitor.Visit(MathMLSpaceElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLScriptElement e, object args)
		{
			MathMLElement c = (MathMLElement)args;
			switch(selection)
			{
				case SelectionType.Prev:
				{
					if(c == e.SubScript || c == e.SuperScript)
					{
						selection = SelectionType.End;
						return e.Base.Accept(this, e);
					}
					else
					{
						MathMLElement p = e.ParentNode as MathMLElement;
						return p != null ? p.Accept(this, e) : null;
					}                
				} 
				case SelectionType.Next:
				{
					if(c == e.Base)
					{
						MathMLElement next = e.SuperScript != null ? e.SuperScript : e.SubScript;
						selection = SelectionType.Start;
						return next != null ? next.Accept(this, null) : null;
					}
					else
					{
						MathMLElement p = e.ParentNode as MathMLElement;
						return p != null ? p.Accept(this, e) : null;
					}
				}
				case SelectionType.End:
				{
					MathMLElement end = e.SuperScript != null ? e.SuperScript : 
						(e.SubScript != null ? e.SubScript : e.Base);
					return end.Accept(this, null);
				}
				case SelectionType.Start:
				{
					return e.Base.Accept(this, null);
				}
				default:
				{
					return null;
				}
			}
		}

		object MathML.MathMLVisitor.Visit(MathMLRadicalElement e, object args)
		{
			switch(selection)
			{
				case SelectionType.Prev:
				{
					if(args == e.Radicand && e.Index != null)
					{
						selection = SelectionType.End;
						return e.Index.Accept(this, null);
					}
					else
					{
						MathMLElement p = e.ParentNode as MathMLElement;
						return p != null ? p.Accept(this, e) : null;
					}
				}
				case SelectionType.Next:
				{
					if(args == e)
					{
						index = 0;
						selection = SelectionType.Start;
						MathMLElement start = e.Index != null ? e.Index : e.Radicand;
						return start != null ? start.Accept(this, null) : null;
					}
					if(args == e.Index) 
					{
						MathMLElement next = e.Radicand;
						selection = SelectionType.Start;
						return next != null ? next.Accept(this, null) : null;
					}
					else
					{
						MathMLElement p = e.ParentNode as MathMLElement;
						return p != null ? p.Accept(this, e) : null;
					}
				}
				case SelectionType.End:
				{
					MathMLElement end = e.Radicand != null ? e.Radicand : e.Index;
					return end != null ? end.Accept(this, null) : null;
				}
				case SelectionType.Start:
				{
                    return e;					
				}
				default:
				{
					return null;
				}
			}
		}

		object MathML.MathMLVisitor.Visit(MathMLStringLitElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLOperatorElement e, object args)
		{
			// not sure why we have to cast this???			
			return ((MathMLVisitor)this).Visit((MathMLPresentationToken)e, args);
		}

		object MathML.MathMLVisitor.Visit(MathMLMultiScriptsElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLGlyphElement e, object args)
		{		  
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLFractionElement e, object args)
		{
			switch(selection)
			{
				case SelectionType.Prev:
				{
					if(args == e.Denominator && e.Numerator != null)
					{
						selection = SelectionType.End;
						return e.Numerator.Accept(this, null);
					}
					else
					{
						MathMLElement p = e.ParentNode as MathMLElement;
						return p != null ? p.Accept(this, e) : null;
					}
				}
				case SelectionType.Next:
				{
					if(args == e)
					{
						index = 0;
						selection = SelectionType.Start;
                        return e.Numerator != null ? e.Numerator.Accept(this, e) : null;
					}
					else if(args == e.Numerator && e.Denominator != null) 
					{
						selection = SelectionType.Start;
						return e.Denominator.Accept(this, null);
					}
					else
					{
						MathMLElement p = e.ParentNode as MathMLElement;
						return p != null ? p.Accept(this, e) : null;
					}
				}
				case SelectionType.End:
				{
					return e.Denominator != null ? e.Denominator.Accept(this, null) : null;
				}
				case SelectionType.Start:
				{
					return e;
				}
				default:
				{
					return null;
				}
			}
		}

		object MathML.MathMLVisitor.Visit(MathMLAlignMarkElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLAlignGroupElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLVectorElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLPredefinedSymbol e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLPiecewiseElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLMatrixRowElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLMatrixElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLIntervalElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLDeclareElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLCsymbolElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLCnElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLCiElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLConditionElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLCaseElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLAnnotationElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLDocument e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLTableCellElement e, object args)
		{
			MathMLVisitor v = this;
			return v.Visit((MathMLPresentationContainer)e, args);
		}

		object MathML.MathMLVisitor.Visit(MathMLStyleElement e, object args)
		{
            return ((MathMLVisitor)this).Visit((MathMLPresentationContainer)e, args);
		}

		object MathML.MathMLVisitor.Visit(MathMLPaddedElement e, object args)
		{
			return ((MathMLVisitor)this).Visit((MathMLPresentationContainer)e, args);
		}

		object MathML.MathMLVisitor.Visit(MathMLFencedElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLEncloseElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLActionElement e, object args)
		{
			if(selection == SelectionType.Prev || selection == SelectionType.Next)
			{
				return ((MathMLElement)e.ParentNode).Accept(this, e);
			}
			else
			{
				MathMLElement elm = e.FirstChild as MathMLElement;
				return elm != null ? elm.Accept(this, args) : e;
			}
		}

		object MathML.MathMLVisitor.Visit(MathMLMathElement e, object args)
		{
			MathMLElement c = (MathMLElement)args;
			MathMLElement n = null;

			switch(selection)
			{
				case SelectionType.Prev:
				{
					if((n = PreviousMathSibling(c)) != null)
					{
						return n;
					}
					else
					{
						selection = SelectionType.Start;
						return c.Accept(this, null);
					}
				}
				case SelectionType.Next:
				{
					if((n = NextMathSibling(c)) != null)
					{
						selection = SelectionType.Start;
						index = 0;
						return n.Accept(this, null);
					}
					else
					{
						selection = SelectionType.End;
						return c.Accept(this, null);
					}
				}
				case SelectionType.Start:
				{
					n = e.FirstChild as MathMLElement;
					return n != null ? n.Accept(this, null) : null;
				} 
				case SelectionType.End:
				{
					n = e.LastChild as MathMLElement;
					return n != null ? n.Accept(this, null) : null;
				}
				default:
				{
					return null;
				}
			}
		}

		object MathML.MathMLVisitor.Visit(MathMLSetElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLListElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLLambdaElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLFnElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLBvarElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLApplyElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLPlaceholderElement e, object args)
		{
			return args;
		}

		object MathML.MathMLVisitor.Visit(MathMLContentContainer e, object args)
		{
			return args;
		}

		/// <summary>
		/// does the node have any child nodes that are mathml elements?
		/// </summary>
		private static bool HasMathChildNodes(MathMLElement e)
		{
			foreach(XmlNode n in e.ChildNodes)
			{
				if(n is MathMLElement) return true;
			}
			return false;
		}

		private static MathMLElement FirstMathChild(MathMLElement e)
		{
			XmlNodeList list = e.ChildNodes;
			for(int i = 0; i < list.Count; i++)
			{
				MathMLElement n = list.Item(i) as MathMLElement;
				if(n != null) return n;
			}
			return null;
		}

		private static MathMLElement LastMathChild(MathMLElement e)
		{
			XmlNodeList list = e.ChildNodes;
			for(int i = list.Count - 1; i >= 0; i--)
			{
				MathMLElement n = list.Item(i) as MathMLElement;
				if(n != null) return n;
			}
			return null;
		}

		private static MathMLElement PreviousMathSibling(MathMLElement e)
		{
			XmlNode s = e.PreviousSibling;
			while(s != null && !(s is MathMLElement))
			{
				s = s.PreviousSibling;
			}
			return s as MathMLElement;
		}

		private static MathMLElement NextMathSibling(MathMLElement e)
		{
			XmlNode s = e.NextSibling;
			while(s != null && !(s is MathMLElement))
			{
				s = s.NextSibling;
			}
			return s as MathMLElement;
		}
	}
}
