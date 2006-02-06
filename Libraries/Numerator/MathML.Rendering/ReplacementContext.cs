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
//	/**
//	 * Stores a list of area substitutions, these are area / identifier
//	 * pairs. This class is used to identify which area to replace in
//	 * the replace method
//	 */
//	internal class ReplacementContext
//	{
//		/**
//		 * create an empty replacement context
//		 */
//		public ReplacementContext() 
//		{
//			substitutions = new ArrayList();
//		}
//
//		/**
//		 * create a replacement context with a single entry
//		 */
//		public ReplacementContext(AreaIdentifier id, Area area)
//		{
//			substitutions = new ArrayList();
//			substitutions.Add(new Pair(id, area));
//		}
//
//		/**
//		 * add each entry in the given context's substitutions list
//		 * whose identifier.head value is equal to the given head
//		 * value.
//		 * the 
//		 */
//		public ReplacementContext(ReplacementContext context, int front)
//		{
//			substitutions = new ArrayList();
//			foreach(Pair p in context.substitutions)
//			{
//				if(!p.id.End && p.id.Current == front)
//				{
//					// create a new identifier path that is the found
//					// path with the first item removed, this new path 
//					// is relative the child node that this new context
//					// is given to.
//					substitutions.Add(new Pair(new AreaIdentifier(p.id), p.area));
//				}
//			}
//		}
//
//		/**
//		 * add an identified area to the list
//		 * throw an exception if this is a duplicate entry
//		 */
//		public void AddArea(AreaIdentifier id, Area area)
//		{
//			// check to make sure we do not have this id
//			foreach(Pair p in substitutions)
//			{ 
//				// TODO make real exception
//				if(p.id.Equals(id)) throw new Exception();
//			}
//			substitutions.Add(new Pair(id, area));
//		}
//
//		/**
//		 * get the first area who's path is empty, indicating that it is the
//		 * current area node
//		 */
//		public Area Area
//		{
//			get
//			{
//				foreach(Pair pair in substitutions)
//				{
//					if (pair.id.End)
//					{
//						return pair.area;
//					}
//				}
//				return null;
//			}
//		}
//
//		/**
//		 * A class to map a stored identifier path to an area
//		 * a new replacement context is recursivly pased to each
//		 * child node, and new RC is created with a subset of paths
//		 * from the previous RC. 
//		 * 
//		 * note, this was implemented as a class as apposed to a struct
//		 * because it is stored in an array list. If it were a struct 
//		 * (value type), it would have to be boxed in order to be stored, 
//		 * this adding further overhead.
//		 */
//		private class Pair
//		{
//			public Pair(AreaIdentifier id, Area area)
//			{
//				this.id = id;
//				this.area = area;
//			}
//
//			public AreaIdentifier id;
//			public Area area;
//		}
//
//		/**
//		 * a mapping of areas to be substituted
//		 */
//		private ArrayList substitutions;    
//	}
}
