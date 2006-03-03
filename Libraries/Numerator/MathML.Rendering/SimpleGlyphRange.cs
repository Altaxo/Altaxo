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
using System.Globalization;

namespace MathML.Rendering
{

    internal struct SimpleGlyphRange
	{
		private ushort first;
		private ushort last;
		private ushort offset;

		public SimpleGlyphRange(XmlElement node)
		{

			first = Utility.ParseUShort(node.GetAttribute("first"));
			last = Utility.ParseUShort(node.GetAttribute("last"));
			offset = Utility.ParseUShort(node.GetAttribute("offset"));
		}

      public Area GetArea(IFormattingContext context, IFontHandle fontHandle, char c)
		{
			Area result = null;
			ushort glyphIndex = (ushort)(c + offset);
      if (glyphIndex >= first && glyphIndex <= last)
			{
				result = new GlyphArea(context, fontHandle, glyphIndex);
			}
			return result;
		}
	}
}
