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

namespace MathML.Rendering
{
	/**
	 * an empty class that just signifies that a
	 * derived class is a container area, meaning that it contains
	 * a collection of child areas.
	 * 
	 * note, this class may go in the future, as both containers
	 * derive from LinearContainer, and that derives from this class.
	 */
	internal class ContainerArea : Area
	{
		protected ContainerArea() {}

		/**
		 * Replaces the child node oldChild with newChild node.
		 * @param newChild The new node to put in the child list. 
		 * @param oldChild The node being replaced in the list. 
		 * @return The node replaced if found, null otherwise.
		 */
		public virtual Area ReplaceChild(Area newChild, Area oldChild)
		{
			return null;
		}
	}
}
