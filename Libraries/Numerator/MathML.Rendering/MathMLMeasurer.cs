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
using System.Collections;
using System.Xml;

namespace MathML.Rendering
{
	/// <summary>
	/// Summary description for MathMLMinimumSizeFinder.
	/// </summary>
	internal class MathMLMeasurer : MathML.MathMLVisitor
	{
		// temporary untill we fill this class out
		// private MathMLFormatter formatter = new MathMLFormatter();
		private MathMLFormatter formatter;

		// cache measured ares
		private Hashtable cache;

		/// <summary>
		/// this is essentially a stateless object, so nothing done here
		/// </summary>
		public MathMLMeasurer(MathMLFormatter formatter)
		{
			this.formatter = formatter;
			this.cache = new Hashtable();
		}

		/// <summary>
		/// Get the minimum formatted area size for the given element using the
		/// state of the given formatting context.
		/// 
		/// Currently this just formats the element using the this as the formatter, 
		/// in the future, this will be optimized so that a visitor only calculates
		/// the min size instead of creating an entire sub tree of areas.
		/// </summary>
		public BoundingBox MeasureElement(IFormattingContext ctx, MathMLElement e)
		{
			Debug.Assert(ctx.cacheArea == false);
			if(e != null)
			{
				Area a = Area.GetArea(e);
				if(a != null) return a.BoundingBox;

				if(cache.Contains(e))
				{
					return (BoundingBox)cache[e];
				}
				else
				{
					if(e is MathMLTableElement)
					{
						Debug.WriteLine("table element");
					}
					BoundingBox box = (BoundingBox)e.Accept(this, ctx);
					cache.Add(e, box);
					return box;
				}
			}
			else
			{
				return BoundingBox.New();
			}
		}

		public BoundingBox[] MeasureElements(IFormattingContext ctx, MathMLElement[] elements)
		{
			BoundingBox[] boxes = new BoundingBox[elements.Length];
			for(int i = 0; i < elements.Length; i++)
			{
				if(elements[i] != null)
				{
					boxes[i] = MeasureElement(ctx, elements[i]);
				}
				else
				{
					boxes[i] = BoundingBox.New();
				}
			}
			return boxes;
		}

		public BoundingBox[][] MeasureElements(IFormattingContext ctx, MathMLElement[][] elements)
		{
			BoundingBox[][] boxes = new BoundingBox[elements.Length][];
			for(int i = 0; i < elements.Length; i++)
			{
				boxes[i] = MeasureElements(ctx, elements[i]);
			}
			return boxes;
		}

		public object Visit(MathMLPlaceholderElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLSeparator e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLPresentationContainer e, object args)
		{
			IFormattingContext context = ((IFormattingContext)args).Clone();
			MathMLNodeList arguments = e.Arguments;
			BoundingBox extent = BoundingBox.New();
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
					//areas[i] = (Area)element.Accept(this, context);
					extent.Append((BoundingBox)element.Accept(this, context));
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
					context.StretchWidth = context.StretchWidth - extent.Width;
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
						//areas[i] = (Area)op.Accept(this, context);
						extent.Append((BoundingBox)op.Accept(this, context));
					}
				}
			}

            return extent;			
		}

		object MathML.MathMLVisitor.Visit(MathMLPresentationToken e, object args)
		{
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
					area = (Area)element.Accept(formatter, context);
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
						area = (Area)element.Accept(formatter, context);
					}
					areas[i++] = area;
				}
				area = AreaFactory.Horizontal(areas);
			}

			return area.BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLXMLAnnotationElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLSemanticsElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLUnderOverElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLLabeledRowElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLTableElement e, object args)
		{
			MathMLTableSizer sizer = new MathMLTableSizer((IFormattingContext)args, this, e);
			return sizer.BoundingBox;			
		}

		object MathML.MathMLVisitor.Visit(MathMLTableRowElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLSpaceElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLScriptElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLRadicalElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLStringLitElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLOperatorElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLMultiScriptsElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLGlyphElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLFractionElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLAlignMarkElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLAlignGroupElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLVectorElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLPredefinedSymbol e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLPiecewiseElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLMatrixRowElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLMatrixElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLIntervalElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLDeclareElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLCsymbolElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLCnElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLCiElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLConditionElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLCaseElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLAnnotationElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLDocument e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLTableCellElement e, object args)
		{
			MathMLVisitor p = this;
			return p.Visit((MathMLPresentationContainer)e, args);
		}

		object MathML.MathMLVisitor.Visit(MathMLStyleElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLPaddedElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLFencedElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLEncloseElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLActionElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLMathElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLSetElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLListElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLLambdaElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLFnElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLBvarElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLApplyElement e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}

		object MathML.MathMLVisitor.Visit(MathMLContentContainer e, object args)
		{
			return ((Area)formatter.Visit(e, args)).BoundingBox;
		}
	}
}
