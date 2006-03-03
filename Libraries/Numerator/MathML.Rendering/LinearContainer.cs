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
using System.Collections;
using Scaled = System.Single;

namespace MathML.Rendering
{
    /**
	 * provide base functionality to all container types
	 * (Horizontal and Vertical)
	 */
	internal class LinearContainer : ContainerArea
	{
		/**
		 * When this area is fited, this is the source area
		 */
		protected readonly Area source; 

		/**
		 * initialize the content.
		 */
		internal LinearContainer(Area[] c, Area source) 
		{ 
			content = c; 
			this.source = source;
		}
		
		/**
		 * render each child area with an un-updated rendering 
		 * context. Most containers need to update the rendering
		 * context to reflect a different positions for each 
		 * child area
		 */
		public override void Render(IGraphicDevice device, float x, float y)
		{
			for(int i = 0; i < content.Length; i++)
			{
				content[i].Render(device, x, y);
			}
		}

		/**
		 * find a path from this node to a child node  if area is this node, 
		 * append a 0 to the path indicating this is node that there is a 
		 * path to, otherwise iterate through each child node, adding the 
		 * index of the node the path, and see if there is a path to that node, 
		 * if no path is found, that index is poped from the stack, and the
		 * next node is tried. If there is no path to any node, false is
		 * returned
		 */
		public override bool GetPath(Area area, Stack path)
		{
			if(area == this)
			{
				return true;
			}
			else
			{
				for(int i = 0; i < content.Length; i++)
				{
					// push the offset of the current child node
					path.Push(i); 
					if(content[i].GetPath(area, path)) 
					{
                        // found a path
						return true;
					}
					else
					{
						// no path, pop the offset, and try the next one
						path.Pop();
					}					
				}
				return false;
			}
		}

		/**
		 * get a child area that has a path to this node
		 * if the identifier is at its's last item, this final step
		 * in the path, so return this node, otherwise, if the current step
		 * is valid (less than the size of the child node list, the child
		 * area at the current position will get the path moved to the next 
		 * position. If the id is not at it's end, and the current item is 
		 * out of range, an invalid id exception is thrown.
		 */
		public override Area GetArea(AreaIdentifier id)
		{
			if(id.End)
			{
				return this;
			}
			else if(id.Current < content.Length)
			{
				Area current = content[id.Current];
				id.MoveNext();
				return current.GetArea(id);
			}
			else
			{
				throw new InvalidIdentifier();
			}
		}

		/**
		 * if the id is at it's end (identifies this node), return 0, otherwise
		 * return the child node that it identifies' origin
		 */
		public override Scaled Origin(AreaIdentifier id)
		{
			if(id.End)
			{
				return 0;
			}
			else if(id.Current < content.Length)
			{
				return content[id.Current].Origin(id++);
			}
			else 
			{
				throw new InvalidIdentifier();
			}
		}

		/**
		 * find the smallest left side value of the child nodes
		 * if there are no child nodes, return 0
		 */
		public override Scaled LeftEdge
		{
			get
			{
				if(content.Length > 0)
				{
					Scaled edge = Scaled.MaxValue;
					foreach(Area a in content)
					{
						if(a.LeftEdge < edge) edge = a.LeftEdge;
					}
					return edge;
				}
				else
				{
					return 0;
				}
			}
		}

		/** 
		 * find the largest right edge in the content list.
		 * if the list is empty, return 0
		 */
		public override Scaled RightEdge
		{
			get 
			{
				if(content.Length > 0)
				{
					Scaled edge = Scaled.Epsilon;
					foreach(Area a in content)
					{
						if(a.RightEdge > edge) edge = a.RightEdge;
					}
					return edge;
				}
				else
				{
					return 0; 
				}
			}
		}

		/**
		 * if the id is for this node, return 0. otherwise if the id is for
		 * a child node, return that node's left side.
		 */
		public override Scaled LeftSide(AreaIdentifier id)
		{
			if(id.End)
			{
				return 0;
			}
			else if(id.Current < content.Length)
			{
				return content[id.Current].LeftSide(id++);
			}
			else
			{
				throw new InvalidIdentifier();
			}
		}

		/**
		 * if the id is for this node, return 0. otherwise if the id is for
		 * a child node, return that node's left side.
		 */
		public override Scaled RightSide(AreaIdentifier id)
		{
			if(id.End)
			{
				return 0;
			}
			else if(id.Current < content.Length)
			{
				return content[id.Current].RightSide(id++);
			}
			else
			{
				throw new InvalidIdentifier();
			}
		}

		/**
		 * Replaces the child node oldChild with newChild node.
		 * @param newChild The new node to put in the child list. 
		 * @param oldChild The node being replaced in the list. 
		 * @return The node replaced if found, null otherwise.
		 */
		public override Area ReplaceChild(Area newChild, Area oldChild)
		{
			ContainerArea container = null;
			for(int i = 0; i < content.Length; i++)
			{
				if(content[i] == oldChild)
				{
					content[i] = newChild;
					return oldChild;
				}
				else if((container = content[i] as ContainerArea) != null &&
					container.ReplaceChild(newChild, oldChild) != null)
				{
					return oldChild;
				}
			}
			return null;
		}

		/**
		 * the child areas of a container
		 */
		protected Area[] content;
	}
}
