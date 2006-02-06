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
	/// define  a length type. All mathml distance mesurments are given as an 
	/// encoded length string, this sruct is a decoded version of that string.
	/// </summary>
	public struct Length
	{
		/// <summary>
		/// construct a new Length object. The value is set to 0
		/// </summary>
		/// <param name="u"></param>
		public Length(LengthType u)
		{
			Type = u;
			Value = 0;
		}

		/// <summary>
		/// construct a new Length object
		/// </summary>
		/// <param name="u">the type of the Length</param>
		/// <param name="v">the value of the Length</param>
		public Length(LengthType u, float v)
		{
			Type = u;
			Value = v;
		}

		/// <summary>
		/// the type of this length
		/// </summary>
		public LengthType Type;

		/// <summary>
		/// the value of this length
		/// </summary>
		public float Value;
		
		/// <summary>
		/// gets a string representation of this length. note, this method can not be used
		/// for serializing a length to a dom, use the Utility class instead.
		/// </summary>
		/// <returns>a string representation of this length</returns>
		public override String ToString()
		{
			return "Length {Type = " + Type.ToString() + ", Value = " + Value.ToString() + "}";
		}

        /// <summary>
        /// Lengths are equal if both the type and values are identical
        /// </summary>
        /// <param name="l">a length</param>
        /// <param name="r">a length</param>
        /// <returns></returns>
		public static bool operator == (Length l, Length r)
		{
			return l.Type == r.Type && l.Value == r.Value;
		}

        /// <summary>
        /// Lengths are not equal if either the type or value is different
        /// </summary>
        /// <param name="l">a Length</param>
        /// <param name="r">a Length</param>
        /// <returns></returns>
		public static bool operator != (Length l, Length r)
		{
			return l.Type != r.Type || l.Value != r.Value;
		}

		/// <summary>
		/// determine if this length is one of the predifined named space items.
		/// </summary>
		public bool NamedSpace
		{
			get { return Type >= LengthType.VeryVeryThin && Type <= LengthType.NegativeVeryVeryThick; }
		}

		/// <summary>
		/// determine if this length is a valid h-unit type
		/// </summary>
		public bool HorizontalUnit
		{
			get { return Type >= LengthType.Em && Type <= LengthType.Percentage; }
		}

		/// <summary>
		/// determine if this length type is a valid v-unit type
		/// </summary>
		public bool VerticalUnit
		{
			get { return Type >= LengthType.Px && Type <= LengthType.Ex; }
		}

        /// <summary>
        /// Is this length evaluateable to a fixed number, i.e. this value is NOT
        /// a scaled percentage of another value, is NOT auto or Fit. 
        /// </summary>
		public bool Fixed
		{
			get 
			{
				return (Type >= LengthType.Em && Type <= LengthType.Pc) ||
					   (Type >= LengthType.VeryVeryThin && Type <= LengthType.Big) ||
						Type == LengthType.Ex;
			}
		}

		/// <summary>
		/// override the Equals method to silence the compiler warnings
		/// </summary>
		public override bool Equals(object o)
		{
			return o is Length ? this == (Length)(o) : false;
		}

		/// <summary>
		/// override the GetHashCode to silence the compiler warnings.
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
