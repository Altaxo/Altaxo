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
using System.Globalization;
using System.Collections;

namespace MathML.Rendering
{
	/**
	 * general utility functions
	 */
	internal class Utility
	{
		public static ushort ParseUShort(string s)
		{
			if(s.Length >= 2 && s[0] == '0' && (s[1] == 'x' || s[1] == 'X'))
			{
				s = s.Substring(2);
				return UInt16.Parse(s, NumberStyles.HexNumber);
			}
			else
			{
				return UInt16.Parse(s);
			}
		}

		public static short ParseShort(string s)
		{
			if(s.Length >= 2 && s[0] == '0' && (s[1] == 'x' || s[1] == 'X'))
			{
				s = s.Substring(2);
				return Int16.Parse(s, NumberStyles.HexNumber);
			}
			else
			{
				return Int16.Parse(s);
			}
		}

		public static char ParseChar(string s)
		{
			if(s.Length >= 2 && s[0] == '0' && (s[1] == 'x' || s[1] == 'X'))
			{
				s = s.Substring(2);
				return (char)Int16.Parse(s, NumberStyles.HexNumber);
			}
			else
			{
				return Char.Parse(s);
			}
		}

		/**
		 * parse a string stored in the config xml that encodes a number array
		 */
		internal static short[] ParseShortArray(string s)
		{
			int i = 0, j = 0;
			ArrayList list = new ArrayList();

			// eat whitespace
			while(i < s.Length && Char.IsWhiteSpace(s[i])) i++;

			while(i < s.Length)
			{
				j = i;
				// eat the string containing the un-parsed number
				while(i < s.Length && !Char.IsWhiteSpace(s[i])) i++;

				string substr = s.Substring(j, i - j);

				list.Add(ParseShort(substr));

				// eat the space up to the next up-parsed number
				while(i < s.Length && Char.IsWhiteSpace(s[i])) i++;
			}

			return (short[])list.ToArray(typeof(short));
		}

		/// <summary>
		/// make a culture info because all the config files are encoded
		/// with US english format numbers
		/// </summary>
		internal static readonly CultureInfo CultureInfo = new CultureInfo("en-US");

		internal static float ParseSingle(string s)
		{
			return Single.Parse(s, CultureInfo);
		}
	}
}
