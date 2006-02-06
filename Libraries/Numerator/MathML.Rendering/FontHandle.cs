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

namespace MathML.Rendering
{
	/**
	 * A way to store and locate native font resources.
	 * This class holds onto a native font resource, and the
	 * font attributes with which the native font was created.
	 * This is created by the RenderingDevice, and is held 
	 * currently by the glyph and font areas.
	 * When this class is disposed of, it will free the native 
	 * font resource
	 * 
	 * The RenderingDevice will maintain a list of weak references
	 * to all fonts that it creates. When a request for a new font
	 * is made, the RenderingDevice will look through this list list
	 * for a matching font. If one is found, than a reference to the
	 * found font is returned.
	 * 
	 * As the RenderingDevice contains only weak references to fonts, 
	 * the standard garbage collector will take care of freeing them
	 * when there are no more strong references.
	 */
	internal class FontHandle
	{
		/**
		 * create a math font.
		 */
		internal FontHandle(IntPtr font, int height, bool italic, int weight, string name)
		{
			Handle = font;
			Height = height;
			Italic = italic;
			Weight = weight;
			Name = name;
			Debug.WriteLine(String.Format("Creating font, height; {0}, name: \"{1}\"", 
				height, name));
		}

		/**
		 * A reference to a native font resource.
		 */
		internal readonly IntPtr Handle;

		/**
		 * the height in pixels that the native font was created with
		 */
		private readonly int Height;

		/**
		 * the italic state the native font was created with
		 */
		private readonly bool Italic;

		/**
		 * the weight the native font resource was created with
		 */
		private readonly int Weight;

		/**
		 * the name that the native font resource was created with.
		 */
		private readonly string Name;

		/**
		 * compare a set of font attributes to this font resouce.
		 */
		internal bool Equals(int height, bool italic, int weight, string name)
		{
			return (Height == height && Italic == italic && Weight == weight &&
				String.Compare(Name, name, false) == 0);
		}

		/**
		 * free the native font resource
		 */
		~FontHandle()
		{
			Debug.WriteLine("Destroying native font...");
			GraphicDevice.DestroyFont(Handle);
		}
	}
}
