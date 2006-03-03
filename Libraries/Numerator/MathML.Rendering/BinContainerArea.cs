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
using Scaled = System.Single;

namespace MathML.Rendering
{
	/**
	 * base class for all modifier areas, these are area that modify
	 * a single child area, such as Color, Ink, Box, etc...
	 */
	internal class BinContainerArea : ContainerArea
	{
		/**
		 * initialize the single child area
		 */
		protected BinContainerArea(Area child)
		{
			this.child = child;
		}		
		
		/**
		 * Render the Area. Most modifier area simply defer rendering
		 * to the child area.
		 */
		public override void Render(IGraphicDevice device, float x, float y)
		{
			child.Render(device, x, y);
		}

		/**
		 * return the child area's left edge
		 */
		public override Scaled LeftEdge { get { return child.LeftEdge; } }

		/**
		 * return the child area's right edge
		 */
		public override Scaled RightEdge { get { return child.RightEdge; } }

		/**
		 * TODO write this up
		 */
		public override Scaled Origin(AreaIdentifier id)
		{
			if(id.End) 
			{
				// get the origin for this node.
                return 0; 
			}
			else if( id.Current == 0)
			{
				// get the origin for the child node
				id.MoveNext();
				return child.Origin(id);
			}
			else
			{
				// bad monkey
                throw new InvalidIdentifier();
			}
		}

		/**
		 * Default behavior is for a container area not to change any 
		 * properties of the bounding box, and simply fit the child
		 * to this box.
		 */
		public override Area Fit(BoundingBox box)
		{
			BinContainerArea area = (BinContainerArea)Clone();
			Debug.Assert(area != null, "result of Clone() is null in BinContainerArea.Fit");
			area.child = child.Fit(box);
			area.source = this;
			return area;
		}

		/**
		 * create an identifier for a descendant area.
		 * as a BinContainerArea can only have one direct child area, 
		 * the only valie identifiers are ones that are at the end of 
		 * thier path, indicating that it identifies this this node, 
		 * or an identifier with its' current element being zero, indicating
		 * that the current leg of its's path identifies the zero'th and
		 * only child node.
		 */
		public override Area GetArea(AreaIdentifier id)
		{
			if(id.End) 
			{
				return this;
			}
			else if(id.Current == 0) 
			{ 
				id.MoveNext();
				return child.GetArea(id);
			}
			else
			{
				throw new InvalidIdentifier();
			}
		}

		/**
		 * TODO why do we do this
		 */
		public override Scaled LeftSide(AreaIdentifier id)
		{
			if(id.End) 
			{
				throw new InvalidOperation();
			}
			else if(id.Current == 0) 
			{ 
				id.MoveNext();
				return child.LeftSide(id);
			}
			else
			{
				throw new InvalidIdentifier();
			}
		}

		/**
		 * get the right side value.
		 * the identier is only valid if it is for the child
		 * node, or one of the child node's descendants. a 
		 * bin container can not have a right side value.
		 */
		public override Scaled RightSide(AreaIdentifier id)
		{
			if(id.End) 
			{
				throw new InvalidOperation();
			}
			else if(id.Current == 0) 
			{ 
				id.MoveNext();
				return child.RightSide(id);
			}
			else
			{
				throw new InvalidIdentifier();
			}
		}

		/**
		 * Find a path from an area a to either this node, 
		 * or its' child.
		 * returns true is a path is found, false otherwise
		 */
		public override bool GetPath(Area a, Stack path)
		{
			if(a == this)
			{
				return true;
			}
			else
			{
				path.Push(0);
				if(child.GetPath(a, path))
				{
					return true;
				}
				else
				{
					// a path was not found, so pop the item we added.
					path.Pop();
					return false;
				}
			}
		}

		/**
		 * just return the child's strength
		 */
		public override Strength Strength
		{
			get { return child.Strength; }
		}

        /**
		 * just get the child areas bounding box
		 */
		public override BoundingBox BoundingBox
		{
			get { return child.BoundingBox; }
		}

    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, Area area, int index)
		{
			if(area == source)
			{
				return new AreaRegion(this, x, y);
			}
			else
			{
				return child.GetRegion(context, x, y, area, index);
			}
		}

		public override AreaRegion GetRegion(float x, float y, float pointX, float pointY)
		{
			return child.GetRegion (x, y, pointX, pointY);
		}

    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, MathMLElement element, int index)
		{
			return child.GetRegion (context, x, y, element, index);
		}

		/// <summary>
		/// Get the child area's terminal node
		/// </summary>
    public override AreaRegion GetEditRegion(IFormattingContext context, float x, float y, int index)
		{
			return child.GetEditRegion (context, x, y, index);
		}




//		public override bool GetSelection(float renderX, float renderY, Selection selection)
//		{retur
//			// are we looking for element type
//			if (selection.Type == SelectionType.Area && selection.Area == source)
//			{
//				//selection.Element = element;
//				selection.Area = source;
//				selection.CaretX = renderX;
//				selection.CaretY = renderY - BoundingBox.Height;
//				selection.CaretHeight = BoundingBox.VerticalExtent;
//				return true;
//			}
//			return child.GetSelection(renderX, renderY, selection);
//		}

		/**
		 * Replaces the child node oldChild with newChild node.
		 * @param newChild The new node to put in the child list. 
		 * @param oldChild The node being replaced in the list. 
		 * @return The node replaced if found, null otherwise.
		 */
		public override Area ReplaceChild(Area newChild, Area oldChild)
		{
			if(child == oldChild)
			{
				child = newChild;
				return oldChild;
			}
			return null;
		}

		/**
		 * the single child area
		 */
		protected Area child;

		/**
		 * the source of this area from a fiting operation
		 */
		protected Area source;
	}
}
