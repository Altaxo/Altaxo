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
using System.Xml;
using MathML;
using System.Diagnostics;
using System.Collections;
using System.Drawing;

namespace MathML.Rendering
{
    /// <summary>
    /// walks the mathml dom tree, and creates an area tree
	/// 
	/// The general concept here is that the mathml dom is a tree
	/// structure, and the most natural way of walking a tree is
	/// through recursion, where we use the stack space of the various
	/// functions to hold some state. We have a similar process here, 
	/// where each function calls other functions recursivly, but as
	/// the visitor interface is allready defined, we can not pass
	/// paramerters via function calls. Instead, we have pointers
	/// to the current area and context being formated. Each function
	/// call that modifies them needs to save them locally, set the pointer
	/// to an updated context or area, and then recursivly call
	/// one of the other methods. As the stack unwinds, the functions
	/// are returned to, and they need to restore the current pointers.
	/// </summary>
	/// <remarks>
	/// Note about stretchy-ness:
	/// Stretchyness has a definite scope, an operator can be stretched to 
	/// fit the imediate encompasing row. A row can not contain another
	/// row and expect the contained row's operators to stretch to fit the 
	/// outer row. So, stretchyness is scoped to the current row. A table
	/// cell element can contain operators and these operators will be expected
	/// to stretch to fit the cell. A cell's dimensions is only known after the
	/// table is formatted, so creating areas for a table is very similar to
	/// creating child areas for a row. First all the elemnents are created
	/// in a non-streatched state, then the column and row widths and heights
	/// are calculated, then the first level child operators will be asked to 
	/// stretch to fit the cell.
	/// 
	/// Vertical Stretching Rules:
	/// * If a stretchy operator is a direct sub-expression of an mrow element, 
	///	  or is the sole direct sub-expression of an mtd element in some row of a table, 
	///   then it should stretch to cover the height and depth (above and below the axis) 
	///   of the non-stretchy direct sub-expressions in the mrow element or table row, 
	///   unless stretching is constrained by minsize or maxsize attributes.
	/// * In the case of an embellished stretchy operator, the preceding rule applies 
	///   to the stretchy operator at its core.
	/// * If symmetric="true", then the maximum of the height and depth is used to 
	///   determine the size, before application of the minsize or maxsize attributes.
	/// * The preceding rules also apply in situations where the mrow element is inferred.
	/// 
	/// Horizontal Stretching Rules:
	/// * If a stretchy operator, or an embellished stretchy operator, is a direct 
	///   sub-expression of an munder, mover, or munderover element, or if it is the sole 
	///   direct sub-expression of an mtd element in some column of a table (see mtable), 
	///   then it, or the mo element at its core, should stretch to cover the width of
	///   the other direct sub-expressions in the given element (or in the same table column), 
	///   given the constraints	mentioned above.
	/// * If a stretchy operator is a direct sub-expression of an munder, mover, 
	///   or munderover element, or if it is the sole direct sub-expression of an mtd element 
	///   in some column of a table, then it should stretch to cover the width of the other 
	///   direct sub-expressions in the given element (or in the same table column),
	///   given the constraints mentioned above.
	/// * In the case of an embellished stretchy operator, the preceding rule applies to the 
	///   stretchy operator at its core.
	/// </remarks>
	public class MathMLFormatter : MathMLVisitor
	{
		// measure size of sub-areas
		private MathMLMeasurer measurer;

		// cache of layout areas
		// this is a map of MathMLElements to layout areas.
		// every time a node is changed or updated, it and it's parent
		// hierarchy are removed from this cache.
		// if a document is re-loaded this cache is cleared.
		// private Hashtable cachedAreas = new Hashtable();

		public MathMLFormatter() 
		{
			measurer = new MathMLMeasurer(this);
		}

		/// <summary>
		/// create an an area tree from root mathml element
		/// adds a default font area to the root of the area 
		/// tree to set the default font.
		/// </summary>
		public Area Format(MathMLMathElement element, IFormattingContext context)
		{
			return (Area)element.Accept(this, context);
		}

        /// <summary>
        /// format an apply element
        /// </summary>
        /// <param name="e"></param>
        /// <param name="args"></param>
        /// <returns></returns>
		public object Visit(MathMLApplyElement e, object args) 
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

            // document to create tmp elements
			MathMLDocument document = new MathMLDocument();
			Area area = null;
			IFormattingContext ctx = ((IFormattingContext)args).Clone();			

			if(e.Operator is MathMLPredefinedSymbol)
			{
				MathMLPredefinedSymbol predefSymbol = (MathMLPredefinedSymbol)e.Operator;
				PredefinedSymbolInfo info = PredefinedSymbolInfo.Get(predefSymbol.Name);
				if(info != null)
				{
					switch(info.Type)
					{
						case PredefinedSymbolType.Function:
						{
							MathMLNodeList arguments = e.Arguments;
							ArrayList argList = new ArrayList(arguments.Count);
							foreach(Object o in arguments)
							{
								argList.Add(o);
							}
							Area[] areas = new Area[3];
							areas[0] = AreaFactory.String(ctx, info.Value);
							areas[1] = GlyphFactory.GetGlyph(ctx, ctx.Size, PredefinedSymbolInfo.ApplyFunction[0]);
							ctx.Parens = false;
							areas[2] = FormatFencedContainer(e, ctx, argList, new Char[] {','}, "(", ")");
							area = AreaFactory.Horizontal(areas);							
						} break;
						case PredefinedSymbolType.Fraction:
						{
							bool parens = ctx.Parens;
							ctx.Parens = false;
							IEnumerator arguments = e.Arguments.GetEnumerator();
							arguments.MoveNext();
							MathMLElement n = (MathMLElement)arguments.Current;
							Area numerator = (Area)n.Accept(this, ctx);
							// idea is we can have lots of arguments, so keep dividing the numerator
							// by the denominator
							while(arguments.MoveNext())
							{
								MathMLElement d = (MathMLElement)arguments.Current;
								Area denominator = (Area)d.Accept(this, ctx);
								numerator = AreaFactory.Fraction(ctx, numerator, denominator, 5);
							}
							area = numerator;

							if(parens)
							{
								area = FenceArea(ctx, area, ctx.Size, '(', ')');
							}
							
						} break;
						case PredefinedSymbolType.Infix:
						{							
							// make a MathMLOperatorElement for the operator symbol
							MathMLOperatorElement opElement = (MathMLOperatorElement)document.CreateElement("mo");
							XmlNode text = opElement.OwnerDocument.CreateTextNode(info.Value);
							opElement.AppendChild(text);

							int argCount = e.ArgumentCount;

							// count of items + operators
							int itemCount = 2 * argCount - 1;
							
							// list of items to format
							ArrayList items = new ArrayList(itemCount + (info.Parens && ctx.Parens ? 2 : 0));

							// only one arg, so make it a prefix op
							if(argCount == 1)
							{
								items.Add(opElement);
								IEnumerator en = e.Arguments.GetEnumerator();
								en.MoveNext();
								items.Add(en.Current);
							}
							else
							{
								foreach(MathMLElement arg in e.Arguments)
								{
									items.Add(arg);
									if(items.Count + 1 < itemCount)
									{
										items.Add(opElement);
									}
								}
							}

							// add open and close parens if we want them
							if(info.Parens && ctx.Parens)
							{
								MathMLOperatorElement open = (MathMLOperatorElement)document.CreateElement("mo");
								MathMLOperatorElement close = (MathMLOperatorElement)document.CreateElement("mo");
								open.AppendChild(document.CreateTextNode("("));
								close.AppendChild(document.CreateTextNode(")"));
								items.Insert(0, open);
								items.Add(close);
							}

							ctx.Parens = info.ChildParens;						
							area = FormatContainer(e, ctx, items);
						
						} break;
						case PredefinedSymbolType.Postfix:
						{							
							// make a MathMLOperatorElement for the operator symbol
							MathMLOperatorElement opElement = (MathMLOperatorElement)document.CreateElement("mo");
							XmlNode text = opElement.OwnerDocument.CreateTextNode(info.Value);
							opElement.AppendChild(text);
							
							// list of items to format
							ArrayList items = new ArrayList(2 + (info.Parens && ctx.Parens ? 2 : 0));

							// a postfix element only expect one arg							
							IEnumerator en = e.Arguments.GetEnumerator();
							en.MoveNext();
							items.Add(en.Current);
							items.Add(opElement);
							

							// add open and close parens if we want them
							if(ctx.Parens && info.Parens)
							{
								MathMLOperatorElement open = (MathMLOperatorElement)document.CreateElement("mo");
								MathMLOperatorElement close = (MathMLOperatorElement)document.CreateElement("mo");
								open.AppendChild(document.CreateTextNode("("));
								close.AppendChild(document.CreateTextNode(")"));
								items.Insert(0, open);
								items.Add(close);
							}

							ctx.Parens = true;
							area = FormatContainer(e, ctx, items);
						
						} break;
						case PredefinedSymbolType.Root:
						{
							MathMLContentContainer indexElement = null;
							MathMLElement radicandElement = null;
							foreach(XmlNode node in e.ChildNodes)
							{
								if(node == e.Operator)
								{
									continue;
								}
								if(indexElement ==  null && node is MathMLContentContainer && node.Name == "degree")
								{
									indexElement = (MathMLContentContainer)node;
									continue;
								}

								if(radicandElement == null && node is MathMLElement)
								{
									radicandElement = (MathMLElement)node;
								}
							}
							if(radicandElement != null)
							{
                IFormattingContext indexCtx = ctx.Clone();
								indexCtx.ScriptLevel++;
								Area index = (Area)indexElement.Accept(this, indexCtx);
								area = AreaFactory.Radical(ctx, (Area)radicandElement.Accept(this, ctx), index);
							}
						} break;
						case PredefinedSymbolType.Power:
						{
							MathMLNodeList arguments = e.Arguments;
							int argCount = arguments.Count;
							
							Area bseArea = null, scriptArea = null;

							if(argCount > 0) 
							{
                IFormattingContext bseCtx = ctx.Clone();
								bseCtx.Parens = true;
								MathMLElement bse = (MathMLElement)arguments.Item(0);
								bseArea = (Area)bse.Accept(this, bseCtx);
							}

							if(argCount > 1)
							{
                IFormattingContext scriptCtx = ctx.Clone();
								scriptCtx.ScriptLevel++;
								scriptCtx.Parens = true;
								MathMLElement script = (MathMLElement)arguments.Item(1);
								scriptArea = (Area)script.Accept(this, scriptCtx);
							}
                            
							if(bseArea != null)
							{
								area = AreaFactory.Script(ctx, bseArea, null, new Length(LengthType.Undefined), 
									scriptArea, new Length(LengthType.Undefined));
							}
							else
							{
								// TODO error msg
							}
						} break;
						case PredefinedSymbolType.Exp:
						{
							MathMLNodeList arguments = e.Arguments;
							int argCount = arguments.Count;
							
							Area bseArea = null, scriptArea = null;

							MathMLElement baseElm = (MathMLElement)document.CreateElement("mi");
							baseElm.AppendChild(document.CreateTextNode(info.Value));
							bseArea = (Area)baseElm.Accept(this, ctx);

							if(argCount > 0)
							{
                IFormattingContext scriptCtx = ctx.Clone();
								scriptCtx.ScriptLevel++;
								scriptCtx.Parens = true;
								MathMLElement script = (MathMLElement)arguments.Item(0);
								scriptArea = (Area)script.Accept(this, scriptCtx);
							}
                            
							area = AreaFactory.Script(ctx, bseArea, null, new Length(LengthType.Undefined), 
									scriptArea, new Length(LengthType.Undefined));
				
						} break;
						case PredefinedSymbolType.Fenced:
						{
							MathMLNodeList arguments = e.Arguments;
							if(arguments.Count > 0)
							{
								MathMLElement argument = (MathMLElement)arguments.Item(0);
								ArrayList argList = new ArrayList(1);
								argList.Add(argument);
								area = FormatFencedContainer(argument, ctx, argList, new char[0], 
									info.Value[0].ToString(), info.Value[1].ToString());
							}

						} break;
						case PredefinedSymbolType.Log:
						{
							MathMLElement arg = null;
							ArrayList argList = new ArrayList(1);
							MathMLElement logbase = null;
							Area bseArea = AreaFactory.String(ctx, info.Value);
							Area logbaseArea = null;
							foreach(XmlNode n in e.ChildNodes)
							{
								if(n == e.Operator)
									continue;
								else if(logbase == null && n is MathMLElement && n.Name == "logbase")
								{
									logbase = (MathMLElement)n;
                  IFormattingContext scriptCtx = ctx.Clone();
									scriptCtx.ScriptLevel++;
									logbaseArea = (Area)logbase.Accept(this, scriptCtx);
									continue;
								}
								else if(arg == null && n is MathMLElement)
								{
                                    arg = (MathMLElement)n; 
                                    argList.Add(arg);    
								}
							}
							area = AreaFactory.Script(ctx, bseArea, logbaseArea, new Length(LengthType.Undefined), 
								null, new Length(LengthType.Undefined));
							Area[] areas = new Area[3];
							areas[0] = area;
							areas[1] = GlyphFactory.GetGlyph(ctx, ctx.Size, PredefinedSymbolInfo.ApplyFunction[0]);
							ctx.Parens = false;
							areas[2] = FormatFencedContainer(e, ctx, argList, new Char[] {','}, "(", ")");
							area = AreaFactory.Horizontal(areas);	


						} break;
					}
				}				
			}
			else if(e.Operator is MathMLApplyElement)
			{
				MathMLNodeList arguments = e.Arguments;
				ArrayList argList = new ArrayList(arguments.Count);
				foreach(Object o in arguments)
				{
					argList.Add(o);
				}
				Area[] areas = new Area[3];

				ctx.Parens = true;
				areas[0] = (Area)e.Operator.Accept(this, ctx);
				areas[1] = GlyphFactory.GetGlyph(ctx, ctx.Size, PredefinedSymbolInfo.ApplyFunction[0]);
				ctx.Parens = false;
				areas[2] = FormatFencedContainer(e, ctx, argList, new Char[] {','}, "(", ")");
				area = AreaFactory.Horizontal(areas);	
			}

			if(area == null)
			{
				area = AreaFactory.String((IFormattingContext)args, "?");
			}

			return CompleteArea(ctx, e, area);
		}

		public object Visit(MathMLBvarElement e, object args)  
		{
			return AreaFactory.String((IFormattingContext)args, "?");
		}

		public object Visit(MathMLFnElement e, object args)  
		{
			return AreaFactory.String((IFormattingContext)args, "?");
		}

		public object Visit(MathMLLambdaElement e, object args) 
		{
			return AreaFactory.String((IFormattingContext)args, "?");
		}

		public object Visit(MathMLListElement e, object args)  
		{
			return AreaFactory.String((IFormattingContext)args, "?");
		}

		public object Visit(MathMLSetElement e, object args)  
		{
			return AreaFactory.String((IFormattingContext)args, "?");
		}

		/// <summary>
		/// process the root mathml math element.
		/// a "math" element may accoring to the spec contain arbitrary 
		/// many child elements, so we create an implicit mrow out of
		/// all the child elements.
		/// </summary>
		public object Visit(MathMLMathElement e, object args)
		{
			IFormattingContext context = (IFormattingContext)args;
			context.DisplayStyle = e.Display;
			int i = 0;

			Area[] areas = new Area[e.Arguments.Count];

			foreach(MathMLElement m in e.Arguments)
			{
				areas[i++] = m != null ? (Area)m.Accept(this, context) : AreaFactory.String(context, "");
			}

			return CompleteArea(context, e, AreaFactory.Horizontal(areas));
		} 

		/// <summary>
		/// action element is faked out for now.
		/// currently we only grab the last child (activated state)
		/// and render it
		/// </summary>
		public object Visit(MathMLActionElement e, object args) 
		{			
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			IFormattingContext ctx = (IFormattingContext)args;

			try
			{
				MathMLElement selectedElement = (MathMLElement)e.ChildNodes[e.Selection];
				Area area = (Area)selectedElement.Accept(this, ctx);

				// check to see if we are colored
				Object userData = e.GetUserData("mouseenter");
				if(userData != null && userData is bool && (bool)userData)
				{
					Color background = e.Background;
					Color color = e.Color;

					if(!color.IsEmpty && !background.IsEmpty)
					{
						area = AreaFactory.Color(color, area);
						area = AreaFactory.Background(background, area);
					}
					else if(!color.IsEmpty) 
					{
						area = AreaFactory.Color(color, area);
					}
					else if(!background.IsEmpty)
					{
						area = AreaFactory.Background(background, area);
					}
				
				}
				return CompleteArea(ctx, e, area);
			}
			catch(Exception ex)
			{
				return Error(ctx, e, ex.Message);
			}
		}

		public object Visit(MathMLEncloseElement e, object args) 
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "enclose"));
		}

		/// <summary>
		/// format a fenced element. We assume that the fenced opening and closing
		/// items are stretchy items. We pretty much just format this like a container, 
		/// except we put the separators in place
		/// </summary>
		public object Visit(MathMLFencedElement e, object args)  
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			IFormattingContext context = ((IFormattingContext)args).Clone();
			char[] separators = e.Separators.ToCharArray();
			string open = e.Open;
			string close = e.Close;
			MathMLNodeList arguments = e.Arguments;
			ArrayList argList = new ArrayList(arguments.Count);
			foreach(object o in e.Arguments)
			{
				argList.Add(o);
			}

			Area area = FormatFencedContainer(e, context, argList, separators, open, close);

			return CompleteArea(context, e, area);
		}

		private Area FormatFencedContainer(MathMLElement e, IFormattingContext context, ArrayList arguments, 
			char[] separators, String open, String close)
		{			
			BoundingBox extent = BoundingBox.New();
			int argCount = arguments.Count;
			int openCount = open.Length > 0 ? 1 : 0;
			int closeCount = close.Length > 0 ? 1 : 0;
			// list to hold fences, childs and separators
			Area[] areas = new Area[openCount + closeCount + argCount + (argCount > 1 ? argCount - 1 : 0)];

			// process all nodes that are not stretchy operators, get thier total 
			// extents
			for(int i = 0, j = 0; i < argCount; i++)
			{
				MathMLElement element = (MathMLElement)arguments[i];
				MathMLOperatorElement op = element as MathMLOperatorElement;

				if(op == null || op.Stretchy == false)
				{
					areas[i + j + openCount] = (Area)element.Accept(this, context);
					extent.Append(areas[i + j + openCount].BoundingBox);
				}		

                // insert a separator		
				if(i + 1 < argCount)
				{
					// at the next child node, but i has not been bumped yet
					areas[i + j + openCount + 1] = 
						GlyphFactory.GetGlyph(context, 
						context.Size, separators[j < separators.Length ? j : separators.Length - 1]);
					j++;
				}
			}	
	
			if(!context.Stretch.Defined)
			{
				// avail width is epsilon because stretchy glyphs were not counted in the
				// width calculation
				context.Stretch = BoundingBox.New(Single.Epsilon, extent.Height, extent.Depth);
			}
			else
			{
				// calculate availible width
				context.StretchWidth = context.StretchWidth - extent.Width;
				if(context.Stretch.Width < 0) context.StretchWidth = 0;
			}

			// process all areas that need to be stretched			
			for(int i = 0, j = 0; i < argCount; i++)
			{
				MathMLOperatorElement op = arguments[i] as MathMLOperatorElement;
				if(op != null && op.Stretchy)
				{
					areas[i + openCount + j] = (Area)op.Accept(this, context);
					context.StretchWidth -= areas[i + openCount + j].BoundingBox.Width;
					if(context.Stretch.Width < 0) context.StretchWidth = 0;

					// adjust sep index
					if (i + 1 < argCount) j++;
				}
			}		

			// add fenced elements, these only stretchy vertically
			context.StretchWidth = 0;
			if(openCount > 0)
			{
				areas[0] = open.Length > 1 ? AreaFactory.String(context, open) : 
					GlyphFactory.GetStretchyGlyph(context, context.Size, open[0], context.Stretch);
			}
			if(closeCount > 0)
			{
				areas[areas.Length - 1] = close.Length > 1 ? AreaFactory.String(context, close) :
					GlyphFactory.GetStretchyGlyph(context, context.Size, close[0], context.Stretch);
			}		
	
			return AreaFactory.Horizontal(areas);
		}

        /// <summary>
        /// fence an area with a set of glyphs. The glyphs will stretch if they can
        /// to fit the vertical extent of the area.
        /// </summary>
    private Area FenceArea(IFormattingContext context, Area area, float ptSize, char open, char close)
		{
			Area[] areas = new Area[1 + (open != 0 ? 1 : 0) + (close != 0 ? 1 : 0)];
			int i = 0;
			BoundingBox stretch = area.BoundingBox;

			// add fenced elements, these only stretchy vertically
			stretch.Width = 0;

            if(open != 0)
                areas[i++] = GlyphFactory.GetStretchyGlyph(context, ptSize, open, stretch);

			areas[i++] = area;

			if(close != 0)
                areas[i++] = GlyphFactory.GetStretchyGlyph(context, ptSize, close, stretch);

			return AreaFactory.Horizontal(areas);			
		}


		public object Visit(MathMLPaddedElement e, object args)  
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			return CompleteArea((IFormattingContext)args, e, (Area)Visit((MathMLPresentationContainer)e, args));
		}

		/// <summary>
		/// format a style element.
		/// re-set the formatting context to the values of the style
		/// element, and format the sub-tree
		/// </summary>
		public object Visit(MathMLStyleElement e, object args)  
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			IFormattingContext ctx = ((IFormattingContext)args).Clone();		

			// format the child tree
			Area area = (Area)Visit((MathMLPresentationContainer)e, ctx);

			// make sure that we only set a mathml element for the outermost area
			Color color = e.Color;
			Color background = e.Background;

			if(!color.IsEmpty && !background.IsEmpty)
			{
				area = AreaFactory.Color(color, area);
				area = AreaFactory.Background(background, area);
			}
			else if(!color.IsEmpty) 
			{
				area = AreaFactory.Color(color, area);
			}
			else if(!background.IsEmpty)
			{
				area = AreaFactory.Background(background, area);
			}

			return CompleteArea(ctx, e,area);
		}

		/// <summary>
		/// this is really just a presentation container, the formatting context
		/// is allready updated with any special formatting needs here.
		/// 
		/// table cells area not cached, if a table is required to be re-flowed, 
		/// the cells allways have to be re-layed out
		/// </summary>
		public object Visit(MathMLTableCellElement e, object args)  
		{
			// format the internal area, this is just a mrow, later we orient the row
			// in the cell
			// FormattingContext ctx = (FormattingContext)args;
			Area area = (Area)Visit((MathMLPresentationContainer)e, args);

			area = AreaFactory.Box(area.BoundingBox, area);

			return area;
		}
		
		public object Visit(MathMLDocument e, object args)  
		{
			return AreaFactory.String((IFormattingContext)args, "?");
		}
			
		public object Visit(MathMLAnnotationElement e, object args)  
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
				
		public object Visit(MathMLCaseElement e, object args)  
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
					
		public object Visit(MathMLConditionElement e, object args)  
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
		
		/// <summary>
		/// format a content identifier
		/// </summary>
		/// <param name="e"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public object Visit(MathMLCiElement e, object args) 
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			Area area = null;
			IFormattingContext ctx = ((IFormattingContext)args).Clone();		

			XmlNode text = e.FirstChild;
			if(text != null && text.NodeType == XmlNodeType.Text)
			{
				if(text.Value.Length == 1)
				{
					ctx.MathVariant = MathVariant.Italic;
				}
				area = AreaFactory.String(ctx, text.Value);
			}
			else
			{
				area = AreaFactory.String((IFormattingContext)args, "?");
			}
			return CompleteArea((IFormattingContext)args, e, area);
		}
						
		public object Visit(MathMLCnElement e, object args)  
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			Area area = null;
			IFormattingContext ctx = (IFormattingContext)args;		

			XmlNode text = e.FirstChild;
			if(text != null && text.NodeType == XmlNodeType.Text)
			{
				area = AreaFactory.String(ctx, text.Value);
			}
			else
			{
				area = AreaFactory.String((IFormattingContext)args, "?");
			}
			return CompleteArea((IFormattingContext)args, e, area);
		}
						
		public object Visit(MathMLCsymbolElement e, object args)  
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
				
		public object Visit(MathMLDeclareElement e, object args)  
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
				
		public object Visit(MathMLIntervalElement e, object args) 
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
		
		public object Visit(MathMLMatrixElement e, object args) 
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
					
		public object Visit(MathMLMatrixRowElement e, object args)  
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
					
		public object Visit(MathMLPiecewiseElement e, object args) 
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
					
		public object Visit(MathMLPredefinedSymbol e, object args)  
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;
			IFormattingContext ctx = (IFormattingContext)args;

			Area area = null;
			PredefinedSymbolInfo info = PredefinedSymbolInfo.Get(e.Name);

			if(info != null)
			{
				area = AreaFactory.String(ctx, info.Value);
			}
			else
			{
				area = AreaFactory.String(ctx, "?");
			}
			return CompleteArea((IFormattingContext)args, e, area);
		}
					
		public object Visit(MathMLVectorElement e, object args)  
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
		
		public object Visit(MathMLAlignGroupElement e, object args) 
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
			
		public object Visit(MathMLAlignMarkElement e, object args)  
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
		
		/// <summary>
		/// format a fraction
		/// </summary>		
		public object Visit(MathMLFractionElement e, object args)
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			IFormattingContext context = (IFormattingContext)args;			
			Area numerator = (Area)e.Numerator.Accept(this, context);			
			Area denominator = (Area)e.Denominator.Accept(this, context);

			return CompleteArea(context, e, AreaFactory.Fraction(context, numerator, denominator, 5*context.OnePixel));
		} 		
		
		/// <summary>
		/// format a glyph.
		/// This is a terminal node, and has no context info
		/// </summary>
		public object Visit(MathMLGlyphElement e, object args)
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			return CompleteArea((IFormattingContext)args, e, AreaFactory.Glyph((IFormattingContext)args, e.FontFamily, e.Alt, e.Index));
		}
 	
        /// <summary>
        /// MathMLMultiScriptsElement not supported yet
        /// </summary>					
		public object Visit(MathMLMultiScriptsElement e, object args) 
		{
			return AreaFactory.String((IFormattingContext)args, "MathMLMultiScriptsElement not supported yet");
		}

		/// <summary>
		/// Format an operator
		/// An operator should have only one child node, and it should be of
		/// type text or glyph. An operator 
		/// </summary>				
		public object Visit(MathMLOperatorElement e, object args)
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			Area op = null;
			IFormattingContext context = (IFormattingContext)args;	

			// copy of stretch size, this will be changed, no sense copy whole context
			BoundingBox stretch = context.Stretch;

			// get the left and right spaces for an operator.
			Area lspace = AreaFactory.HorizontalSpace(context.Evaluate(e.LSpace));
			Area rspace = AreaFactory.HorizontalSpace(context.Evaluate(e.RSpace));

			// get the area(s) for the center of the operator
			if(e.Contents.Count > 0)
			{
				MathMLGlyphElement glyph = e.Contents.Item(0) as MathMLGlyphElement;
				XmlNode text = e.Contents.Item(0) as XmlNode;

				if(e.Stretchy)
				{
					// this really should have a text node, but no rule says
					// that you can not specify a operator with a glyph
					if(text != null && text.Value.Length > 0)
					{
						Debug.WriteLine(String.Format(
							"creating stretchy filler area from operator with text node, op is\"{0}\"", 
							text.Value));

						// trim the spaces from the requested size, these sizes area added
						// to the total width of the operator when formatted
						stretch.Width -= lspace.BoundingBox.Width;
						stretch.Width -= rspace.BoundingBox.Width;
						if(stretch.Width < 0) stretch.Width = 0;
	
						op = GlyphFactory.GetStretchyGlyph(context, context.Size, text.Value[0], stretch);
					}
					else if(glyph != null)
					{
						Debug.WriteLine(String.Format(
							"creating stretchy filler area from operator with glyph, index is {0}", 
							glyph.Index));

						op = GlyphFactory.GetStretchyGlyph(context, context.Size, (char)glyph.Index, context.Stretch);
					}
				}
				else
				{					
					if(text != null)
					{	
						Debug.WriteLine(String.Format("operator \"{0}\" is not stretchy", text.Value));					
						op = AreaFactory.String(context, text.Value);
					}
					else if(glyph != null)
					{
						Debug.WriteLine(String.Format("operator \"{0}\" is not stretchy", (char)glyph.Index));
						op = AreaFactory.Glyph(context, glyph.FontFamily, glyph.Alt, glyph.Index);
					}
				}
			}

			// if there was no suitable glyph, create a bogus character.
			if(op == null)
			{
				Debug.WriteLine("Warning, no area was created for an operator, defaulting to \"?\"");
				op = AreaFactory.String(context, "?");
			}			

            return CompleteArea((IFormattingContext)args, e, Colorize(e, AreaFactory.Horizontal(new Area[]{lspace, op, rspace})));
		} 	
					
		public object Visit(MathMLStringLitElement e, object args)  
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			XmlNode xmlNode = e.FirstChild;
			string text = xmlNode.NodeType == XmlNodeType.Text ? xmlNode.Value : String.Empty;
			return CompleteArea((IFormattingContext)args, 
				e, Colorize(e, AreaFactory.String((IFormattingContext)args, e.LQuote + text + e.RQuote)));
		}
		
		/// <summary>
		/// format a radical element
		/// </summary>		
		public object Visit(MathMLRadicalElement e, object args) 
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			IFormattingContext ctx = (IFormattingContext)args;
      IFormattingContext indexCtx = ctx.Clone();
			indexCtx.ScriptLevel++;
			Area radicand = (Area)e.Radicand.Accept(this, ctx);
			Area index = e.Index != null ? (Area)e.Index.Accept(this, indexCtx) : null;
			return CompleteArea(ctx, e, AreaFactory.Radical(ctx, radicand, index));
		} 
		
		/// <summary>
		/// format a script element.
		/// </summary>		
		public object Visit(MathMLScriptElement e, object args)	
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			Area baseArea = null, subArea = null, superArea = null;
			IFormattingContext ctx = (IFormattingContext)args;
			IFormattingContext nctx = ctx.Clone();

            // format the base area, we better well have one...
			baseArea = (Area)e.Base.Accept(this, ctx);

			// bump script level for subscript
			nctx.ScriptLevel++;

			// sub script is optional
			if(e.SubScript != null) {
				subArea = (Area)e.SubScript.Accept(this, nctx);
			}
						
			// super script is also optional
			if(e.SuperScript != null) {
				superArea = (Area)e.SuperScript.Accept(this, nctx);
			}

			return CompleteArea(ctx, e, AreaFactory.Script(ctx, baseArea, subArea, e.SubScriptShift, 
				superArea, e.SuperScriptShift));
		} 

		public object Visit(MathMLSpaceElement e, object args)  
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			IFormattingContext ctx = (IFormattingContext)args;
			float width = ctx.Evaluate(e.Width);
			float height = ctx.Evaluate(e.Height);
			float depth = ctx.Evaluate(e.Depth);
			return CompleteArea(ctx, e, AreaFactory.Space(BoundingBox.New(width, height, depth)));
		}

		public object Visit(MathMLTableRowElement e, object args) 
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
			
		public object Visit(MathMLTableElement e, object args)  
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			IFormattingContext ctx = ((IFormattingContext)args).Clone();
			MathMLTableSizer sizer = new MathMLTableSizer(ctx, measurer, e);
			ArrayList cells = new ArrayList();

			Debug.WriteLine("Formatting table, extent will be: " + sizer.BoundingBox);

			for(int i = 0; i < sizer.Cells.Length; i++)
			{
				for(int j = 0; j < sizer.Cells[i].Length; j++)
				{
					if(sizer.Cells[i][j] != null)
					{
						Debug.WriteLine("Formatting cell at [" + i + "][" + j + "] to size of " + sizer.CellSizes[i][j] + 
							", shift of " + sizer.CellShifts[i][j]);

						// format an area, try to stretch the area to fit the cell
						ctx.Stretch = sizer.CellSizes[i][j];
						Area cell = (Area)sizer.Cells[i][j].Accept(this, ctx);

						// get attributes of cell
						BoundingBox cellSize = sizer.CellSizes[i][j];
						PointF cellShift = sizer.CellShifts[i][j];

						// adjust the cell shift so the area is aligned properly
						PointF areaShift = GetCellAlignment(sizer.Cells[i][j], cell, ctx, cellSize, cellShift);

						cells.Add(new TableCellArea(sizer.Cells[i][j], cell, cellSize, cellShift, areaShift));
					}
					Debug.WriteLineIf(sizer.Cells[i][j] == null, "Cell at [" + i + "][" + j + "] is null");
				}
			}

			return new TableArea(e, (Area[])cells.ToArray(typeof(Area)), sizer.BoundingBox, sizer.SolidLines, 
				sizer.DashedLines);
		}
			
		public object Visit(MathMLLabeledRowElement e, object args) 
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
		
		public object Visit(MathMLUnderOverElement e, object args) 
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;
			IFormattingContext ctx = (IFormattingContext)args;
      IFormattingContext baseCtx = ctx.Clone();
      IFormattingContext overCtx = ctx.Clone();
      IFormattingContext underCtx = ctx.Clone();
			MathMLElement baseElement = e.Base;
			MathMLElement overElement = e.OverScript;
			MathMLElement underElement = e.UnderScript;
			Area overArea = null;
			Area underArea = null;
			Area result = null;
			BoundingBox baseBox = BoundingBox.New();
			BoundingBox underBox = BoundingBox.New();
			BoundingBox overBox = BoundingBox.New();
			BoundingBox extent = BoundingBox.New();	
			float ex = baseCtx.Evaluate(new Length(LengthType.Ex, 1.0f));
			float overSpace = e.Accent ? ex / 8.0f : ex / 4.0f;
			float underSpace = e.AccentUnder ? ex / 8.0f : ex / 4.0f;

			// first get the initial un-stretched areas to compute the
			// stretch extent
			baseCtx.Stretch = BoundingBox.New();
			Area baseArea = (Area)baseElement.Accept(this, baseCtx);
			baseBox = baseArea.BoundingBox;
			extent.Overlap(baseBox);

			if(overElement != null)
			{
				overCtx.cacheArea = false;
				overCtx.Stretch = BoundingBox.New();
				overCtx.DisplayStyle = Display.Block;
				if(e.Accent == false) overCtx.ScriptLevel++;
				overArea = (Area)overElement.Accept(this, overCtx);
				overBox = overArea.BoundingBox;
				extent.Overlap(overBox);
			}

			if(underElement != null)
			{
				underCtx.cacheArea = false;
				underCtx.Stretch = BoundingBox.New();
				underCtx.DisplayStyle = Display.Block;
				if(e.AccentUnder == false) underCtx.ScriptLevel++;
				underArea = (Area)underElement.Accept(this, underCtx);
				underBox = underArea.BoundingBox;
				extent.Overlap(underBox);
			}

			// now that we have the stretch extent, re-format the areas with 
			// the correct stretch distance.
			baseCtx.Stretch = BoundingBox.New(extent.Width, baseBox.Height, baseBox.Depth);
			baseArea = (Area)baseElement.Accept(this, baseCtx);

			if(overElement != null)
			{
				overCtx.cacheArea = ctx.cacheArea;
				overCtx.Stretch = BoundingBox.New(extent.Width, overBox.Height, overBox.Depth);
				overArea = (Area)overElement.Accept(this, overCtx);
			}

			if(underElement != null)
			{
				underCtx.cacheArea = ctx.cacheArea;
				underCtx.Stretch = BoundingBox.New(extent.Width, underBox.Height, underBox.Depth);
				underArea = (Area)underElement.Accept(this, underCtx);
			}

			if(overArea != null && underArea != null)
			{
				Area[] areas = new Area[5];
				areas[0] = AreaFactory.HorizontalCenter(underArea);
				areas[1] = AreaFactory.VerticalSpace(underSpace, underSpace); 
				areas[2] = AreaFactory.HorizontalCenter(baseArea);
				areas[3] = AreaFactory.VerticalSpace(overSpace, overSpace); 
				areas[4] = AreaFactory.HorizontalCenter(overArea);
				result = AreaFactory.Vertical(areas, 2);
			}
			else if(overArea != null)
			{
				Area[] areas = new Area[3];
				areas[0] = AreaFactory.HorizontalCenter(baseArea);
				areas[1] = AreaFactory.VerticalSpace(overSpace, overSpace);
				areas[2] = AreaFactory.HorizontalCenter(overArea);
				result = AreaFactory.Vertical(areas, 0);
			}
			else if(underArea != null)
			{
				Area[] areas = new Area[3];
				areas[0] = AreaFactory.HorizontalCenter(underArea);
				areas[1] = AreaFactory.VerticalSpace(underSpace, underSpace);
				areas[2] = AreaFactory.HorizontalCenter(baseArea);
				result = AreaFactory.Vertical(areas, 2);
			}
			else
			{ 
				// strange, but could happen, no under or over areas
				result = baseArea;
			}

			return CompleteArea(ctx, e, result);
		}
			
		public object Visit(MathMLSemanticsElement e, object args) 
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
			
		public object Visit(MathMLXMLAnnotationElement e, object args)  
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}
		
		/// <summary>
		///  process a presentation token. This can be either a "mi", 
		///  "mn", or "mtext" element. These nodes can only have content
		///  of text or glyph nodes. This method process all of the child
		///  nodes of the element, verifies that they are indeed text nodes,
		///  or glyph elements, and depending on the content, creates either
		///  an array of glyphs in a horizontal array if the content is text
		///  nodes interspersed with glyph elements, or a string element
		///  if the content is a single text node 
		/// </summary>
		public object Visit(MathMLPresentationToken e, object args)
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			Area area = null;

			// make an updated context
			IFormattingContext context = ((IFormattingContext)args).Clone();
			XmlNode node = null;
			MathMLElement element = null;
			MathMLNodeList contents = e.Contents;
			
			// update the font size, pres-tokens can specify font size
			context.Size = context.Evaluate(e.MathSize);			

			MathVariant variant = e.MathVariant;
			if(variant != MathVariant.Unknown)
			{
				context.MathVariant = variant;
			}

            if(contents.Count == 1) // create a single string area
			{
				if((node = e.FirstChild) != null && node.NodeType == XmlNodeType.Text)
				{
					area = AreaFactory.String(context, node.Value);
				}
				else if((element = e.FirstChild as MathMLElement) != null) 
				{
					// sets area to a new glyph area
					area = (Area)element.Accept(this, context);
				}
				else
				{
					// TODO this is bad, need error handler
				}
			}
			else // create a sequence of areas
			{
				Area[] areas = new Area[contents.Count];
				int i = 0;
				foreach(XmlNode n in contents)
				{
					if(n.NodeType == XmlNodeType.Text)
					{
						area = AreaFactory.String(context, n.Value);
					}
					else if((element = n as MathMLElement) != null)
					{
						area = (Area)element.Accept(this, context);
					}
					areas[i++] = area;
				}
				area = AreaFactory.Horizontal(areas);
			}

			return CompleteArea((IFormattingContext)args, e, Colorize(e, area));
		}	

		/// <summary>
		/// separators are ignored
		/// </summary>
		public object Visit(MathMLSeparator e, object args) 
		{
			return CompleteArea((IFormattingContext)args, e, AreaFactory.String((IFormattingContext)args, "?"));
		}

		/// <summary>
		/// process a mrow element.
		/// a mrow is a horizontal array of areas. many of these areas may need
		/// to be stretched, but we need to know what size to stretch them first, 
		/// so, we first create areas for all child nodes of this node that are 
		/// not stretchable glyphs, and calculate the extents of these areas 
		/// to find the min and max sizes, then we go back and create areas
		/// for the stretchable glyphs.
		/// </summary>
		/// <remarks>
		/// There is no way of knowing exactly how far to stretch glyphs in the
		/// width direction, so we calculate the avalilble width (total extent -
		/// width of non stretchy areas), and for each stretchy operator we create, 
		/// we subtract the new op's width from the avail width to get the new avail
		/// width for the next op. This does have the nasty side effect of stretching
		/// the first horizontally stretchy glyph to fill up all of the horizontal
		/// space.
		/// </remarks>		
		public object Visit(MathMLPresentationContainer e, object args)
		{
			Area cache = Area.GetArea(e);
			if(cache != null) return cache;

			// moved logic to FormatContainer so that it can be shared with apply element
			MathMLNodeList arguments = e.Arguments;
			ArrayList argList = new ArrayList(arguments.Count);
			foreach(object o in e.Arguments)
			{
				argList.Add(o);
			}
			Area area = FormatContainer(e, (IFormattingContext)args, argList);

			// table cell areas elemetns area a special case, they get 'Completed'
			// in the MathMLTableCellElement method
			return e is MathMLTableCellElement ? area : CompleteArea((IFormattingContext)args, e, area);
		}

        /// <summary>
        /// Format a row of elements. This can be either a presentation container or a mrow
        /// </summary>
		private Area FormatContainer(MathMLElement e, IFormattingContext ctx, ArrayList arguments)
		{
      IFormattingContext context = ctx.Clone();
			BoundingBox extent = BoundingBox.New();
			Area[] areas = new Area[arguments.Count];
			int stretchCount = 0;

			// save the stretch size because stretch scope can not extend into another 
			// level of nesting
			BoundingBox stretch = context.Stretch;
			context.Stretch = BoundingBox.New();

			// process all nodes that are not stretchy operators, get thier total 
			// extents
			for(int i = 0; i < arguments.Count; i++)
			{
				MathMLElement element = (MathMLElement)arguments[i];
				MathMLOperatorElement op = element as MathMLOperatorElement;

				if(op == null || op.Stretchy == false)
				{
					areas[i] = (Area)element.Accept(this, context);
					extent.Append(areas[i].BoundingBox);
				}	
				if(op != null && op.Stretchy)
				{
					stretchCount++;
				}
			}	

			// if we have any elements that can be stretched, stretch them
			if(stretchCount > 0)
			{	
				if(!stretch.Defined)
				{
					// avail width is epsilon because stretchy glyphs were not counted in the
					// width calculation
					context.Stretch = BoundingBox.New(Single.Epsilon, extent.Height, extent.Depth);
				}
				else
				{
					// set the stretch size back to stretch the child elements
					context.Stretch = stretch;

					// calculate availible width
					context.StretchWidth = context.Stretch.Width - extent.Width;
					if(context.Stretch.Width < 0) context.StretchWidth = 0;

					// size to stretch each width equally
					context.StretchWidth = context.Stretch.Width / (float)stretchCount;
				}            

				// process all areas that need to be stretched			
				for(int i = 0; i < arguments.Count; i++)
				{
					MathMLOperatorElement op = arguments[i] as MathMLOperatorElement;
					if(op != null && op.Stretchy)
					{
						areas[i] = (Area)op.Accept(this, context);
					}
				}
			}

			Area area = AreaFactory.Horizontal(areas);

			// hide the areas if it is a phantom
			if(e.Name == "mphantom")
			{
				area = AreaFactory.Hide(area);
			}
			else if(e.Name == "merror")
			{
				area = AreaFactory.Color(Color.Red, area);
			}

			// table cell areas elemetns area a special case, they get 'Completed'
			// in the MathMLTableCellElement method
			return area;
		}

		/// <summary>
		/// format a placeholder element
		/// this creates the hollow dashed rectangle to indicate that a user
		/// can type in this area.
		/// </summary>
		public object Visit(MathMLPlaceholderElement e, object args)
		{
			IFormattingContext ctx = (IFormattingContext)args;
			Area glyph = GlyphFactory.GetGlyph(ctx, ctx.Size, '\xfffc');
			return CompleteArea(ctx, e, glyph);
		}

		public object Visit(MathMLContentContainer e, object args)
		{
			IFormattingContext ctx = (IFormattingContext)args;
			// only deal with first child for now
			MathMLElement elm = e.FirstChild as MathMLElement;
			return elm != null ? elm.Accept(this, ctx) : AreaFactory.String(ctx, "?");
		}

		/// <summary>
		/// set the foreground and background colors for an area derived from a 
		/// token area. This takes a formatted area tree, and if a color exists 
		/// for a token, this color is set to the area subtree, same for background
		/// color.
		/// </summary>
		private static Area Colorize(MathMLPresentationToken e, Area a)
		{
			Color color = e.MathColor;
			Color backColor = e.MathBackground;

			// make sure we only colorize outermost area

			if(!color.IsEmpty && !backColor.IsEmpty)
			{
				a = AreaFactory.Color(color, a);
				a = AreaFactory.Background(backColor, a);
			}
			else if(!color.IsEmpty)
			{
				a = AreaFactory.Color(color, a);
			}
			else if(!backColor.IsEmpty)
			{
				a = AreaFactory.Background(backColor, a);
			}
			return a;
		}

		private static PointF GetCellAlignment(MathMLTableCellElement e, Area area, IFormattingContext ctx,
			BoundingBox cellSize, PointF cellShift)
		{
			BoundingBox areaSize = area.BoundingBox;
			float vShift = 0;
			float hShift = 0;

			// orient the row in the horizontal direction
			switch(e.ColumnAlign)
			{
				case Align.Left:
				{
					hShift = 0;
				} break;
				case Align.Right:
				{
					hShift = cellSize.Width - areaSize.Width;
				} break;
				default:
				{
					// default is center
					hShift = (cellSize.Width - areaSize.Width) / 2.0f;
				} break;
			}
			
			// orient the row in the vertical direction
			switch(e.RowAlign)
			{
				case Align.Top:
				{
					vShift = areaSize.Height - cellSize.Height;
				} break;
				case Align.Bottom:
				{
					vShift = cellSize.Depth - areaSize.Depth;
				} break;
				case Align.Center:
				{
					// equal space above and below area
					float filler = (cellSize.VerticalExtent - areaSize.VerticalExtent) / 2.0f;
					vShift = areaSize.Height + filler - cellSize.Height;
				} break;
				case Align.Axis:
				{
					// shift to the axis (shift up in the negative direction)
					vShift = -ctx.Axis;
				} break;
				default: 
				{	
					// baseline, leave the area alone
				} break;
			}

			// might need to re-adjust in case of exteme spanning cells
			if(areaSize.VerticalExtent > cellSize.VerticalExtent)
			{
				Debug.WriteLine("WARNING!, area will exceed cell size, shifting to fit, need to fix this!!!");
				vShift = areaSize.Height - cellSize.Height;
			}

			return new PointF(cellShift.X + hShift, cellShift.Y + vShift);			
		}

		/**
		 * wrap the completed area with a MathMLWrapper area and set the
		 * elements area field to point to the newly completed area
		 */
		private Area CompleteArea(IFormattingContext ctx, MathMLElement e, Area area)
		{
			if(ctx.cacheArea)
			{
				area = new MathMLWrapperArea(area, e);
				Area.SetArea(e, area);
			}
			return area;
		}

		/**
		 * There was an error processing a node, create an error node.
		 * wrap the completed area with a MathMLWrapper area and set the
		 * elements area field to point to the newly completed area
		 */
		private Area Error(IFormattingContext ctx, MathMLElement e, String errorMsg)
		{
			Debug.WriteLine("Error Formatting " + e.GetType().Name + " element, " + errorMsg);
			Area area = AreaFactory.String(ctx, "?");
			area = AreaFactory.Color(Color.Red, area);
            area = new MathMLWrapperArea(area, e);
			Area.SetArea(e, area);
			return area;
		}
	}
}
