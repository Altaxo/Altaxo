//This file is part of the gNumerator MathML DOM library, a complete 
//implementation of the w3c mathml dom specification
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
//andy@epsilon3.net

using System;

namespace MathML
{
	/// <summary>
	/// an entity replacement
	/// </summary>
	internal struct Entity
	{
		public Entity(string n, char code1, char code2)
		{
			Name = n;

			if(code1 != 0 && code2 != 0)
			{
				Value = new string(new char[] {code1, code2});
			}
			else
			{
				Value = new string(code1, 1);
			}
		}
		public readonly string Name;
		public readonly string Value;
	}
}
