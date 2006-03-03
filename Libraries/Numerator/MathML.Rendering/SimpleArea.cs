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
	 * Simple area is a terminal area, it will have no child areas
	 */
	internal class SimpleArea : Area
	{
		/**
		 * make sure only derived class can create one of these
		 */
		protected SimpleArea() {}
		
        /**
		 * simple areas are terminal nodes, so if the given area
		 * matches this one, add a 0 indicating this is the end of the
		 * path and return true, otherwise return false indicating
		 * there is no path here.
		 */	
		public override bool GetPath(Area area, Stack path)
		{
			if(area == this)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        /**
		 * simple areas are terminsl nodes, so make sure the identifer
		 * is empty at this point, and return this node, otherwise
		 * throw an invalid id exception
		 */
		public override Area GetArea(AreaIdentifier identifier) 
		{
			if(identifier.End) { return this; }
			else throw new InvalidIdentifier();
		}

		/**
		 * TODO, figure out why we do nothing here
		 */
		public override void Render(IGraphicDevice device, float x, float y) {}

		/**
		 * if the id id empty (at a terminal node), return 0, 
		 * otherwise throw an invalid id exception
		 * TODO figure out why we do this
		 */
		public override Scaled Origin(AreaIdentifier id)
		{
			if(id.End) { return 0; }
			else throw new InvalidIdentifier();
		}

		/**
		 * not supported on simple nodes
		 * TODO find out why
		 */
		public override Scaled LeftSide(AreaIdentifier id) 
		{ 
			throw new InvalidOperation();
		}

		/**
		 * not supported on simple nodes
		 * TODO find out why
		 */
		public override Scaled RightSide(AreaIdentifier id) 
		{ 
			throw new InvalidOperation();
		}

		/**
		 * simple areas have zero strength in all directions
		 * TODO find out why
		 */
		public override Strength Strength 
		{ 
			get { return new Strength(0, 0, 0); } 
		}

		/**
		 * cloning a simple area makes no sense as cloning occurs only
		 * when fitting an area, and simple areas fit into themselves.
		 */
		public override Object Clone() 
		{
			throw new InvalidOperation();
		}
	}
}
