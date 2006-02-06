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
	/// create an enum for the mathvariant attribute type
	/// this enum is not in the w3c spec, but this is very frequently
	/// used, and the use of an enum saves space, is faster, and is far
	/// less error prone that the use of strings
	/// </summary>
	public enum MathVariant
	{
		Normal = 0, Bold = 1, Italic = 2, BoldItalic = 3, DoubleStruck = 4, 
		BoldFraktur = 5, Script = 6, BoldScript = 7, Fraktur = 8, SansSerif = 8, 
		BoldSansSerif = 9, SansSerifItalic = 10, SansSerifBoldItalic = 11, 
		Monospace = 12, Unknown = 13
	}

	/// <summary>
	/// specifies whether the enclosed MathML expression should be
	/// rendered in a display style or an in-line style.
	/// </summary>
	public enum Display
	{
		Block, Inline
	}

	/// <summary>
	/// The form of a mathml operator
	/// </summary>
	public enum Form
	{
		Prefix = 0, Infix = 1, Postfix = 2
	}	

	/// <summary>
	/// define the diferent types of units a length can have
	/// </summary>
	public enum LengthType
	{
		/// <summary>
		/// length type was not declared
		/// </summary>
		Undefined, 
		
		Pure, 

        /// <summary>
        /// An infinite measurment, this should be the machine limit for a positive number
        /// </summary>		
		Infinity, 
		
		/// <summary>
		/// font relative unit, em height is the maximum distance above the baseline 
		/// reached by an uppercase symbol. Contrast with x height.
		/// </summary>
		Em, 
		
		/// <summary>
		/// size in pixels
		/// </summary>
		Px, 
		
		/// <summary>
		/// size in inches
		/// </summary>
		In, 
		
		/// <summary>
		/// size in centi-meters
		/// </summary>
		Cm, 
		
		/// <summary>
		/// size in mili-meters
		/// </summary>
		Mm, 
		
		/// <summary>
		/// size in points
		/// </summary>
		Pt, 
		
		/// <summary>
		/// size in picas
		/// </summary>
		Pc, 
		
		/// <summary>
		/// percentage of another size, this is typically used in table widths
		/// </summary>
		Percentage, Ex,
		VeryVeryThin, VeryThin, Thin, Medium, Thick, VeryThick, VeryVeryThick, 
		NegativeVeryVeryThin, NegativeVeryThin, NegativeThin, NegativeMedium, 
		NegativeThick, NegativeVeryThick, NegativeVeryVeryThick, Small, 
		Normal, Big, 
		
		/// <summary>
		/// used for table columnwidth attribute.
		/// The "auto" value means that the column should be as wide as needed, which is the default.
		/// </summary>
		Auto, 
		
		/// <summary>
		/// Used for table columnwidth attribute.
		/// If "fit" is given as a value, the remaining page width after subtracting the 
		/// widths for columns specified as "auto" and/or specific widths is divided equally 
		/// among the "fit" columns and this value is used for the column width. If 
		/// insufficient room remains to hold the contents of the "fit" columns, renderers 
		/// may linewrap or clip the contents of the "fit" columns.
		/// </summary>
		Fit
	};

	/// <summary>
	/// Alignment type for fractions and tables
	/// </summary>
	public enum Align
	{
		Top, Bottom, Left, Center, Right, Axis, Baseline
	}

	/// <summary>
	/// line styles for a table
	/// </summary>
	public enum LineStyle
	{
		None, Solid, Dashed
	}

	public enum Notation
	{
		Longdiv, Actuarial, Radical
	}

	/// <summary>
	/// An enum to describe the 'occurrence' type. This is an attribute that 
	/// can be in the form of prefix, infix, postfix, or function-model.
	/// </summary>
	public enum Occurrence
	{
		/// <summary>
		/// operator should be before the argument
		/// </summary>
		Prefix, 
		
		/// <summary>
		/// operator should be in-between the arguments
		/// </summary>
		Infix, 
		
		/// <summary>
		/// operator should follow the arguments
		/// </summary>
		Postfix, 
		
		/// <summary>
		/// arguments should be considered function arguments
		/// </summary>
		FunctionModel
	}
}
