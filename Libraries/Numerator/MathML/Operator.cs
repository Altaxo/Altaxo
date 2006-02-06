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
	/// index and attributes for a mathml operator "mo" element
	/// 
	/// default values
	/// form prefix | infix | postfix set by position of operator in an mrow (rule given below); 
	///		used with mo content to index operator dictionary		 * 
	/// fence true | false set by dictionary (false)
	/// separator true | false set by dictionary (false)
	/// lspace number h-unit | namedspace set by dictionary (thickmathspace)
	/// rspace number h-unit | namedspace set by dictionary (thickmathspace)
	/// stretchy true | false set by dictionary (false)
	/// symmetric true | false set by dictionary (true)
	/// maxsize number [ v-unit | h-unit ] | namedspace | infinity set by dictionary (infinity)
	/// minsize number [ v-unit | h-unit ] | namedspace set by dictionary (1)
	/// largeop true | false set by dictionary (false)
	/// movablelimits true | false set by dictionary (false)
	/// accent true | false set by dictionary (false)
	/// </summary>
	internal struct Operator
	{
		public Operator(string name, Form form, Length lSpace, Length rSpace, bool stretchy, bool fence, 
			bool accent, bool largeOp, bool moveableLimits, bool separator, Length minSize, Length maxSize, 
			bool symmetric)
		{
			Name = name;
			Form = form;
			LSpace = lSpace;
			RSpace = rSpace;
			Fence = fence;
			Separator = separator;
			Stretchy = stretchy;
			MaxSize = maxSize;
			MinSize = minSize;
			Accent = accent;
			LargeOp = largeOp;
			MoveableLimits = moveableLimits;
			Symmetric = symmetric;
		}

		public readonly string Name;
		public readonly Form Form;
		public readonly Length LSpace;
		public readonly Length RSpace;
		public readonly bool Stretchy;
		public readonly bool Fence;
		public readonly bool Accent;
		public readonly bool LargeOp;
		public readonly bool MoveableLimits;
		public readonly bool Separator;
		public readonly Length MinSize;
		public readonly Length MaxSize;
		public readonly bool Symmetric;

		/// <summary>
		/// get the default set of attributes. Note, the form must
		/// be given, as it is determined the the operators position in the
		/// row. This creates a default set of attributes with the given form, 
		/// and a empty name.
		/// </summary>
		public static Operator Default(Form form)
		{
			return new Operator("", form, new Length(LengthType.Thick), new Length(LengthType.Thick), false, 
				false, false, false, false, false, new Length(LengthType.Px, 1.0f), new Length(LengthType.Infinity), true);
		}
	}
}
