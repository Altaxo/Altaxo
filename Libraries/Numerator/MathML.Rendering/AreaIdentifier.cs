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

namespace MathML.Rendering
{
	/**
	 * The AreaIdentifer describes a relative path from  an area a 
	 * to either itself, or to one if its' descendent nodes. An 
	 * AreaIdentifer resembles the ChildSeq syntax in the XPointer 
	 * grammer. The AreaIdentifer path is a sequence of zero based
	 * offsets from one node to one if it's child nodes. If the path 
	 * is empty, it refers to the current area node. 
	 * Identifier paths are relative. A path from node a to node n is
	 * an path with lenght n at a. If b is the child node referneced 
	 * the first element of the path, the path at b now becomes lenght
	 * n - 1, the first element is removed, as that refered to the 
	 * offset at a to b.
	 * 
	 * The GetArea method on Area should interpret a path as :
	 * a.GetArea([k1,k2,...kn]) = a.children[k1].GetArea([k2,k3,...kn]).
	 * 
	 * An identifier from a node to itself is a  zero length, or 
	 * empty path. An identifier from a node to its' first child is a 
	 * path of length 1, with a single element with a value of 0, thus
	 * indicating the 0th child node.
	 * 
	 * Take for example the following area tree:
	 * [area_a]
	 *    |
	 * [area_b, area_c, area_d, area_e]
	 *    |               |
	 * [area_f]        [area_g]
	 *    |               |
	 *    |            [area_h]
	 *    |
	 * [area_i, area_j] 
	 * 
	 * Where we have area_a as the root node, this has a list  of 
	 * children:[area_b,area_c,area_d,area_e], area_b has a child of 
	 * area_f, and so forth. In this case, we have the
	 * following following paths:
	 * 
	 * a -> a = ""
	 * a -> b = "0"
	 * a -> c = "1"
	 * a -> d = "2"
	 * a -> e = "3"
	 * a -> f = a -> b -> f = "0,0"
	 * a -> g = a -> d -> g = "3,0"
	 * a -> i = a -> b -> f -> i = "0,0,0"
	 * a -> j = a -> b -> f -> j = "0,0,1"
	 * a -> h = a -> d -> g -> h = "2,0,0"
	 * 
	 * The methods on AreaIdentifer allow it to be used as a forward 
	 * only enumerator, so that it can be easily pased recursivly 
	 * through an area tree.
	 */
	public class AreaIdentifier
	{
		/**
		 * create a new empty identifer. An emtpy identifer
		 * is a path from a node to itself.
		 */
		public AreaIdentifier() { path = new int[0]; }

		/**
		 * create a new identifier from the path information 
		 * stored in a stack. As a stack is created from bottom to
		 * top, we need to reverse the entries in it, as a identifer
		 * path is from top to bottom.
		 */
		public AreaIdentifier(Stack path)
		{
			this.path = new int[path.Count];
			path.CopyTo(this.path, 0);
			Array.Reverse(this.path);
		}

		/**
		 * create a new identifier that is one larger than the given
		 * identifier, with the first element set to front.
		 */
		public AreaIdentifier(int front, AreaIdentifier back)
		{
			if(front < 0)
			{
				throw new ArgumentException("value must not be negative", "front");
			}
			path = new int[back.path.Length + 1];
			path[0] = front;
			Array.Copy(back.path, 0, path, 1, back.path.Length);
		}

		/**
		 * create a new identifier that is next identifier relative
		 * to the given identifier. this takes the given identifier and
		 * makes a new identifier that has a path length one less than
		 * the given identier, and copies the all items except the first
		 * one. So if the given identifier is [k1,k2,k3,...,kn], the
		 * new identifier will be [k2,k3,...,kn].
		 * If the previous identifer is allready empty, an 
		 * InvalidArgument exception is thrown.
		 */
		public AreaIdentifier(AreaIdentifier previous)
		{
			if(previous.path.Length > 0)
			{
				path = new int[previous.path.Length - 1];
				previous.path.CopyTo(path, 1);
			}
			else
			{
				throw new ArgumentException(
					"the previous identifier must not be empty", "previous");
			}
		}

		/**
		 * create a new identifier with a single path entry
		 */
		public AreaIdentifier(int path)
		{
			if(path < 0)
			{
				throw new ArgumentException("value must not be negative", "front");
			}
			this.path = new int[1];
			this.path[0] = path;
		}

		/**
		 * create a new identifier with a sequence of path entries, this 
		 * functions just like the ctor that accepts a stack.
		 */
		public AreaIdentifier(int[] path)
		{
			this.path = path;
		}

		/**
		 * gets the current value.
		 * throws InvalidOperationException if the current index
		 * is past the end of the array, or if the array is empty
		 */
		public int Current
		{
			get
			{
				if(current < path.Length)
				{
                    return path[current];
				}
				else
				{
					throw new InvalidOperationException(
						"Attempt to get current item with current index past the end");
				}
			}
		}

		/**
		 * Is this identifier at the end of its' path ?
		 * If an identifier is at the end, that means that it identifies
		 * the current node. An identifier with a zero lenght path is 
		 * allways at the end.
		 */
		public bool End { get { return current >= path.Length; } }

		/**
		 * move the identifier to the next position in its' path.
		 * if the identifier is at the end, MoveNext will have no
		 * affect. 
		 * returns true if there is a next postion, false otherwise
		 */
		public bool MoveNext() 
		{
			if(current < path.Length)
			{
				current++;
				return true;
			}
			else
			{
				return false;
			}
		}

		/**
		 * shortcut for MoveNext method, as this object is moved
		 * next frequently
		 * note, even if this object it at its' end, this operator is 
		 * still OK. 
		 */
		public static AreaIdentifier operator ++ (AreaIdentifier id)
		{
			id.MoveNext();
			return id;
		}

		/**
		 * the path from a root node to one if its' decendents.
		 */
		private int[]  path;

		/**
		 * the index of the current item.
		 */
		private int current = 0;
	}
}
