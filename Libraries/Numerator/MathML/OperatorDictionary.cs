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
using System.Diagnostics;

namespace MathML
{
	/// <summary>
	/// Summary description for MathMLOperatorDictionary.
	/// </summary>
	internal class OperatorDictionary
	{
		private static readonly Length zeroEM = new Length(LengthType.Em, 0);
		private static readonly Length thickMathSpace = new Length(LengthType.Thick);
		private static readonly Length mediumThickMathSpace = new Length(LengthType.Medium);
		private static readonly Length mediumMathSpace = new Length(LengthType.Medium);
		private static readonly Length thinMathSpace = new Length(LengthType.Medium);
		private static readonly Length veryThinMathSpace = new Length(LengthType.Medium);
		private static readonly Length veryVeryThinMathSpace = new Length(LengthType.Medium);
		private static readonly Length veryThickMathSpace = new Length(LengthType.Medium);
		private static readonly Length infinity = new Length(LengthType.Infinity);

		/// <summary>
		/// list of operators. These were all auto-generated from the w3c provided operator
		/// dictionary on 12-13-2003
		/// </summary>
		private static readonly Operator[] operators = 
		{
			// ( 
			new Operator("(", Form.Prefix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ) 
			new Operator(")", Form.Postfix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// [ 
			new Operator("[", Form.Prefix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ] 
			new Operator("]", Form.Postfix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// { 
			new Operator("{", Form.Prefix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// } 
			new Operator("}", Form.Postfix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// CloseCurlyDoubleQuote 
			new Operator("\x201d", Form.Postfix, zeroEM, zeroEM, false, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// CloseCurlyQuote 
			new Operator("\x2019", Form.Postfix, zeroEM, zeroEM, false, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftAngleBracket 
			new Operator("\x2329", Form.Prefix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftCeiling 
			new Operator("\x2308", Form.Prefix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftDoubleBracket 
			new Operator("\x301a", Form.Prefix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftFloor 
			new Operator("\x230a", Form.Prefix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// OpenCurlyDoubleQuote 
			new Operator("\x201c", Form.Prefix, zeroEM, zeroEM, false, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// OpenCurlyQuote 
			new Operator("\x2018", Form.Prefix, zeroEM, zeroEM, false, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightAngleBracket 
			new Operator("\x232a", Form.Postfix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightCeiling 
			new Operator("\x2309", Form.Postfix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightDoubleBracket 
			new Operator("\x301b", Form.Postfix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightFloor 
			new Operator("\x230b", Form.Postfix, zeroEM, zeroEM, true, true, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// InvisibleComma 
			new Operator("\x2063", Form.Infix, zeroEM, zeroEM, false, false, false, false, false, true, new Length(LengthType.Px, 1), infinity, true),
			// , 
			new Operator(",", Form.Infix, zeroEM, veryThickMathSpace, false, false, false, false, false, true, new Length(LengthType.Px, 1), infinity, true),
			// HorizontalLine 
			new Operator("\x2500", Form.Infix, zeroEM, zeroEM, true, false, false, false, false, false, new Length(LengthType.Px, 0), infinity, true),
			// VerticalLine 
			new Operator("\x7c", Form.Infix, zeroEM, zeroEM, true, false, false, false, false, false, new Length(LengthType.Px, 0), infinity, true),
			// ; 
			new Operator(";", Form.Infix, zeroEM, thickMathSpace, false, false, false, false, false, true, new Length(LengthType.Px, 1), infinity, true),
			// ; 
			new Operator(";", Form.Postfix, zeroEM, zeroEM, false, false, false, false, false, true, new Length(LengthType.Px, 1), infinity, true),
			// := 
			new Operator(":=", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Assign 
			new Operator("\x2254", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Because 
			new Operator("\x2235", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Therefore 
			new Operator("\x2234", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// VerticalSeparator 
			new Operator("\x2758", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// // 
			new Operator("//", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Colon 
			new Operator("\x2237", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// amp 
			new Operator("\x26", Form.Prefix, zeroEM, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// amp 
			new Operator("\x26", Form.Postfix, thickMathSpace, zeroEM, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// *= 
			new Operator("*=", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// -= 
			new Operator("-=", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// += 
			new Operator("+=", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// /= 
			new Operator("/=", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// -> 
			new Operator("->", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// : 
			new Operator(":", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// .. 
			new Operator("..", Form.Postfix, mediumMathSpace, zeroEM, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ... 
			new Operator("...", Form.Postfix, mediumMathSpace, zeroEM, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SuchThat 
			new Operator("\x220b", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleLeftTee 
			new Operator("\x2ae4", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleRightTee 
			new Operator("\x22a8", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownTee 
			new Operator("\x22a4", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftTee 
			new Operator("\x22a3", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightTee 
			new Operator("\x22a2", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Implies 
			new Operator("\x21d2", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RoundImplies 
			new Operator("\x2970", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// | 
			new Operator("|", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// || 
			new Operator("||", Form.Infix, mediumMathSpace, mediumMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Or 
			new Operator("\x2a54", Form.Infix, mediumMathSpace, mediumMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// amp;&amp; 
			new Operator("&&", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// And 
			new Operator("\x2a53", Form.Infix, mediumMathSpace, mediumMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// amp 
			new Operator("\x26", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ! 
			new Operator("!", Form.Prefix, zeroEM, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Not 
			new Operator("\x2aec", Form.Prefix, zeroEM, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Exists 
			new Operator("\x2203", Form.Prefix, zeroEM, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ForAll 
			new Operator("\x2200", Form.Prefix, zeroEM, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotExists 
			new Operator("\x2204", Form.Prefix, zeroEM, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Element 
			new Operator("\x2208", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotElement 
			new Operator("\x2209", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotReverseElement 
			new Operator("\x220c", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotSquareSubset 
			new Operator("\x228f\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotSquareSubsetEqual 
			new Operator("\x22e2", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotSquareSuperset 
			new Operator("\x2290\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotSquareSupersetEqual 
			new Operator("\x22e3", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotSubset 
			new Operator("\x2282\x20d2", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotSubsetEqual 
			new Operator("\x2288", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotSuperset 
			new Operator("\x2283\x20d2", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotSupersetEqual 
			new Operator("\x2289", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ReverseElement 
			new Operator("\x220b", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SquareSubset 
			new Operator("\x228f", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SquareSubsetEqual 
			new Operator("\x2291", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SquareSuperset 
			new Operator("\x2290", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SquareSupersetEqual 
			new Operator("\x2292", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Subset 
			new Operator("\x22d0", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SubsetEqual 
			new Operator("\x2286", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Superset 
			new Operator("\x2283", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SupersetEqual 
			new Operator("\x2287", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleLeftArrow 
			new Operator("\x21d0", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleLeftRightArrow 
			new Operator("\x21d4", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleRightArrow 
			new Operator("\x21d2", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownLeftRightVector 
			new Operator("\x2950", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownLeftTeeVector 
			new Operator("\x295e", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownLeftVector 
			new Operator("\x21bd", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownLeftVectorBar 
			new Operator("\x2956", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownRightTeeVector 
			new Operator("\x295f", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownRightVector 
			new Operator("\x21c1", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownRightVectorBar 
			new Operator("\x2957", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftArrow 
			new Operator("\x2190", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftArrowBar 
			new Operator("\x21e4", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftArrowRightArrow 
			new Operator("\x21c6", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftRightArrow 
			new Operator("\x2194", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftRightVector 
			new Operator("\x294e", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftTeeArrow 
			new Operator("\x21a4", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftTeeVector 
			new Operator("\x295a", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftVector 
			new Operator("\x21bc", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftVectorBar 
			new Operator("\x2952", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LowerLeftArrow 
			new Operator("\x2199", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LowerRightArrow 
			new Operator("\x2198", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightArrow 
			new Operator("\x2192", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightArrowBar 
			new Operator("\x21e5", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightArrowLeftArrow 
			new Operator("\x21c4", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightTeeArrow 
			new Operator("\x21a6", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightTeeVector 
			new Operator("\x295b", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightVector 
			new Operator("\x21c0", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightVectorBar 
			new Operator("\x2953", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ShortLeftArrow 
			new Operator("\x2190", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ShortRightArrow 
			new Operator("\x2192", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UpperLeftArrow 
			new Operator("\x2196", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UpperRightArrow 
			new Operator("\x2197", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// = 
			new Operator("=", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// lt 
			new Operator("\x26\x26", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// > 
			new Operator(">", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// != 
			new Operator("!=", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// == 
			new Operator("==", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// lt;= 
			new Operator("lt;=", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// >= 
			new Operator(">=", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Congruent 
			new Operator("\x2261", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// CupCap 
			new Operator("\x224d", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DotEqual 
			new Operator("\x2250", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleVerticalBar 
			new Operator("\x2225", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Equal 
			new Operator("\x2a75", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// EqualTilde 
			new Operator("\x2242", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Equilibrium 
			new Operator("\x21cc", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// GreaterEqual 
			new Operator("\x2265", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// GreaterEqualLess 
			new Operator("\x22db", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// GreaterFullEqual 
			new Operator("\x2267", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// GreaterGreater 
			new Operator("\x2aa2", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// GreaterLess 
			new Operator("\x2277", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// GreaterSlantEqual 
			new Operator("\x2a7e", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// GreaterTilde 
			new Operator("\x2273", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// HumpDownHump 
			new Operator("\x224e", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// HumpEqual 
			new Operator("\x224f", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftTriangle 
			new Operator("\x22b2", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftTriangleBar 
			new Operator("\x29cf", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftTriangleEqual 
			new Operator("\x22b4", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// le 
			new Operator("\x2264", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LessEqualGreater 
			new Operator("\x22da", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LessFullEqual 
			new Operator("\x2266", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LessGreater 
			new Operator("\x2276", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LessLess 
			new Operator("\x2aa1", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LessSlantEqual 
			new Operator("\x2a7d", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LessTilde 
			new Operator("\x2272", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NestedGreaterGreater 
			new Operator("\x226b", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NestedLessLess 
			new Operator("\x226a", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotCongruent 
			new Operator("\x2262", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotCupCap 
			new Operator("\x226d", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotDoubleVerticalBar 
			new Operator("\x2226", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotEqual 
			new Operator("\x2260", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotEqualTilde 
			new Operator("\x2242\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotGreater 
			new Operator("\x226f", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotGreaterEqual 
			new Operator("\x2271", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotGreaterFullEqual 
			new Operator("\x2266\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotGreaterGreater 
			new Operator("\x226b\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotGreaterLess 
			new Operator("\x2279", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotGreaterSlantEqual 
			new Operator("\x2a7e\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotGreaterTilde 
			new Operator("\x2275", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotHumpDownHump 
			new Operator("\x224e\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotHumpEqual 
			new Operator("\x224f\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotLeftTriangle 
			new Operator("\x22ea", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotLeftTriangleBar 
			new Operator("\x29cf\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotLeftTriangleEqual 
			new Operator("\x22ec", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotLess 
			new Operator("\x226e", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotLessEqual 
			new Operator("\x2270", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotLessGreater 
			new Operator("\x2278", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotLessLess 
			new Operator("\x226a\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotLessSlantEqual 
			new Operator("\x2a7d\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotLessTilde 
			new Operator("\x2274", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotNestedGreaterGreater 
			new Operator("\x2aa2\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotNestedLessLess 
			new Operator("\x2aa1\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotPrecedes 
			new Operator("\x2280", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotPrecedesEqual 
			new Operator("\x2aaf\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotPrecedesSlantEqual 
			new Operator("\x22e0", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotRightTriangle 
			new Operator("\x22eb", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotRightTriangleBar 
			new Operator("\x29d0\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotRightTriangleEqual 
			new Operator("\x22ed", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotSucceeds 
			new Operator("\x2281", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotSucceedsEqual 
			new Operator("\x2ab0\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotSucceedsSlantEqual 
			new Operator("\x22e1", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotSucceedsTilde 
			new Operator("\x227f\x338", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotTilde 
			new Operator("\x2241", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotTildeEqual 
			new Operator("\x2244", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotTildeFullEqual 
			new Operator("\x2247", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotTildeTilde 
			new Operator("\x2249", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// NotVerticalBar 
			new Operator("\x2224", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Precedes 
			new Operator("\x227a", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// PrecedesEqual 
			new Operator("\x2aaf", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// PrecedesSlantEqual 
			new Operator("\x227c", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// PrecedesTilde 
			new Operator("\x227e", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Proportion 
			new Operator("\x2237", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Proportional 
			new Operator("\x221d", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ReverseEquilibrium 
			new Operator("\x21cb", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightTriangle 
			new Operator("\x22b3", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightTriangleBar 
			new Operator("\x29d0", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightTriangleEqual 
			new Operator("\x22b5", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Succeeds 
			new Operator("\x227b", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SucceedsEqual 
			new Operator("\x2ab0", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SucceedsSlantEqual 
			new Operator("\x227d", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SucceedsTilde 
			new Operator("\x227f", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Tilde 
			new Operator("\x223c", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// TildeEqual 
			new Operator("\x2243", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// TildeFullEqual 
			new Operator("\x2245", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// TildeTilde 
			new Operator("\x2248", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UpTee 
			new Operator("\x22a5", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// VerticalBar 
			new Operator("\x2223", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SquareUnion 
			new Operator("\x2294", Form.Infix, mediumMathSpace, mediumMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Union 
			new Operator("\x22c3", Form.Infix, mediumMathSpace, mediumMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UnionPlus 
			new Operator("\x228e", Form.Infix, mediumMathSpace, mediumMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// - 
			new Operator("-", Form.Infix, mediumMathSpace, mediumMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// + 
			new Operator("+", Form.Infix, mediumMathSpace, mediumMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Intersection 
			new Operator("\x22c2", Form.Infix, mediumMathSpace, mediumMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// MinusPlus 
			new Operator("\x2213", Form.Infix, mediumMathSpace, mediumMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// PlusMinus 
			new Operator("\xb1", Form.Infix, mediumMathSpace, mediumMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SquareIntersection 
			new Operator("\x2293", Form.Infix, mediumMathSpace, mediumMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Vee 
			new Operator("\x22c1", Form.Prefix, zeroEM, thickMathSpace, true, false, false, true, true, false, new Length(LengthType.Px, 1), infinity, true),
			// CircleMinus 
			new Operator("\x2296", Form.Prefix, zeroEM, thickMathSpace, false, false, false, true, true, false, new Length(LengthType.Px, 1), infinity, true),
			// CirclePlus 
			new Operator("\x2295", Form.Prefix, zeroEM, thickMathSpace, false, false, false, true, true, false, new Length(LengthType.Px, 1), infinity, true),
			// Sum 
			new Operator("\x2211", Form.Prefix, zeroEM, thickMathSpace, true, false, false, true, true, false, new Length(LengthType.Px, 1), infinity, true),
			// Union 
			new Operator("\x22c3", Form.Prefix, zeroEM, thickMathSpace, true, false, false, true, true, false, new Length(LengthType.Px, 1), infinity, true),
			// UnionPlus 
			new Operator("\x228e", Form.Prefix, zeroEM, thickMathSpace, true, false, false, true, true, false, new Length(LengthType.Px, 1), infinity, true),
			// lim 
			new Operator("lim", Form.Prefix, zeroEM, thickMathSpace, false, false, false, false, true, false, new Length(LengthType.Px, 1), infinity, true),
			// max 
			new Operator("max", Form.Prefix, zeroEM, thickMathSpace, false, false, false, false, true, false, new Length(LengthType.Px, 1), infinity, true),
			// min 
			new Operator("min", Form.Prefix, zeroEM, thickMathSpace, false, false, false, false, true, false, new Length(LengthType.Px, 1), infinity, true),
			// CircleMinus 
			new Operator("\x2296", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// CirclePlus 
			new Operator("\x2295", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ClockwiseContourIntegral 
			new Operator("\x2232", Form.Prefix, zeroEM, zeroEM, true, false, false, true, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ContourIntegral 
			new Operator("\x222e", Form.Prefix, zeroEM, zeroEM, true, false, false, true, false, false, new Length(LengthType.Px, 1), infinity, true),
			// CounterClockwiseContourIntegral 
			new Operator("\x2233", Form.Prefix, zeroEM, zeroEM, true, false, false, true, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleContourIntegral 
			new Operator("\x222f", Form.Prefix, zeroEM, zeroEM, true, false, false, true, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Integral 
			new Operator("\x222b", Form.Prefix, zeroEM, zeroEM, true, false, false, true, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Cup 
			new Operator("\x22d3", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Cap 
			new Operator("\x22d2", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// VerticalTilde 
			new Operator("\x2240", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Wedge 
			new Operator("\x22c0", Form.Prefix, zeroEM, thickMathSpace, true, false, false, true, true, false, new Length(LengthType.Px, 1), infinity, true),
			// CircleTimes 
			new Operator("\x2297", Form.Prefix, zeroEM, thickMathSpace, false, false, false, true, true, false, new Length(LengthType.Px, 1), infinity, true),
			// Coproduct 
			new Operator("\x2210", Form.Prefix, zeroEM, thickMathSpace, true, false, false, true, true, false, new Length(LengthType.Px, 1), infinity, true),
			// Product 
			new Operator("\x220f", Form.Prefix, zeroEM, thickMathSpace, true, false, false, true, true, false, new Length(LengthType.Px, 1), infinity, true),
			// Intersection 
			new Operator("\x22c2", Form.Prefix, zeroEM, thickMathSpace, true, false, false, true, true, false, new Length(LengthType.Px, 1), infinity, true),
			// Coproduct 
			new Operator("\x2210", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Star 
			new Operator("\x22c6", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// CircleDot 
			new Operator("\x2299", Form.Prefix, zeroEM, thickMathSpace, false, false, false, true, true, false, new Length(LengthType.Px, 1), infinity, true),
			// * 
			new Operator("*", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// InvisibleTimes 
			new Operator("\x2062", Form.Infix, zeroEM, zeroEM, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// CenterDot 
			new Operator("\xb7", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// CircleTimes 
			new Operator("\x2297", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Vee 
			new Operator("\x22c1", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Wedge 
			new Operator("\x22c0", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Diamond 
			new Operator("\x22c4", Form.Infix, thickMathSpace, thickMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Backslash 
			new Operator("\x2216", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// / 
			new Operator("/", Form.Infix, thickMathSpace, thickMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// - 
			new Operator("-", Form.Prefix, zeroEM, veryVeryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// + 
			new Operator("+", Form.Prefix, zeroEM, veryVeryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// MinusPlus 
			new Operator("\x2213", Form.Prefix, zeroEM, veryVeryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// PlusMinus 
			new Operator("\xb1", Form.Prefix, zeroEM, veryVeryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// . 
			new Operator(".", Form.Infix, zeroEM, zeroEM, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Cross 
			new Operator("\x2a2f", Form.Infix, veryThinMathSpace, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ** 
			new Operator("**", Form.Infix, veryThinMathSpace, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// CircleDot 
			new Operator("\x2299", Form.Infix, veryThinMathSpace, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// SmallCircle 
			new Operator("\x2218", Form.Infix, veryThinMathSpace, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Square 
			new Operator("\x25a1", Form.Prefix, zeroEM, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Del 
			new Operator("\x2207", Form.Prefix, zeroEM, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// PartialD 
			new Operator("\x2202", Form.Prefix, zeroEM, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// CapitalDifferentialD 
			new Operator("\x2145", Form.Prefix, zeroEM, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DifferentialD 
			new Operator("\x2146", Form.Prefix, zeroEM, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Sqrt 
			new Operator("\x221a", Form.Prefix, zeroEM, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleDownArrow 
			new Operator("\x21d3", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleLongLeftArrow 
			new Operator("\x27f8", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleLongLeftRightArrow 
			new Operator("\x27fa", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleLongRightArrow 
			new Operator("\x27f9", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleUpArrow 
			new Operator("\x21d1", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleUpDownArrow 
			new Operator("\x21d5", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownArrow 
			new Operator("\x2193", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownArrowBar 
			new Operator("\x2913", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownArrowUpArrow 
			new Operator("\x21f5", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownTeeArrow 
			new Operator("\x21a7", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftDownTeeVector 
			new Operator("\x2961", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftDownVector 
			new Operator("\x21c3", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftDownVectorBar 
			new Operator("\x2959", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftUpDownVector 
			new Operator("\x2951", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftUpTeeVector 
			new Operator("\x2960", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftUpVector 
			new Operator("\x21bf", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftUpVectorBar 
			new Operator("\x2958", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LongLeftArrow 
			new Operator("\x27f5", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LongLeftRightArrow 
			new Operator("\x27f7", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LongRightArrow 
			new Operator("\x27f6", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ReverseUpEquilibrium 
			new Operator("\x296f", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightDownTeeVector 
			new Operator("\x295d", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightDownVector 
			new Operator("\x21c2", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightDownVectorBar 
			new Operator("\x2955", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightUpDownVector 
			new Operator("\x294f", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightUpTeeVector 
			new Operator("\x295c", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightUpVector 
			new Operator("\x21be", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightUpVectorBar 
			new Operator("\x2954", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ShortDownArrow 
			// new Operator("\x2193", Form.Infix, veryThinMathSpace, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ShortUpArrow 
			// new Operator("\x2191", Form.Infix, veryThinMathSpace, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UpArrow 
			new Operator("\x2191", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UpArrowBar 
			new Operator("\x2912", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UpArrowDownArrow 
			new Operator("\x21c5", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UpDownArrow 
			new Operator("\x2195", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UpEquilibrium 
			new Operator("\x296e", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UpTeeArrow 
			new Operator("\x21a5", Form.Infix, veryThinMathSpace, veryThinMathSpace, true, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ^ 
			new Operator("^", Form.Infix, veryThinMathSpace, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// lt;> 
			new Operator("lt;>", Form.Infix, veryThinMathSpace, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ' 
			new Operator("'", Form.Postfix, veryThinMathSpace, zeroEM, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ! 
			new Operator("!", Form.Postfix, veryThinMathSpace, zeroEM, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// !! 
			new Operator("!!", Form.Postfix, veryThinMathSpace, zeroEM, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ~ 
			new Operator("~", Form.Infix, veryThinMathSpace, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// @ 
			new Operator("@", Form.Infix, veryThinMathSpace, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// -- 
			new Operator("--", Form.Postfix, veryThinMathSpace, zeroEM, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// -- 
			new Operator("--", Form.Prefix, zeroEM, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ++ 
			new Operator("++", Form.Postfix, veryThinMathSpace, zeroEM, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ++ 
			new Operator("++", Form.Prefix, zeroEM, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ApplyFunction 
			new Operator("\x2061", Form.Infix, zeroEM, zeroEM, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// ? 
			new Operator("?", Form.Infix, veryThinMathSpace, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// _ 
			new Operator("_", Form.Infix, veryThinMathSpace, veryThinMathSpace, false, false, false, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Breve 
			new Operator("\x2d8", Form.Postfix, zeroEM, zeroEM, false, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Cedilla 
			new Operator("\xb8", Form.Postfix, zeroEM, zeroEM, false, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DiacriticalGrave 
			new Operator("\x60", Form.Postfix, zeroEM, zeroEM, false, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DiacriticalDot 
			new Operator("\x2d9", Form.Postfix, zeroEM, zeroEM, false, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DiacriticalDoubleAcute 
			new Operator("\x2dd", Form.Postfix, zeroEM, zeroEM, false, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftArrow 
			new Operator("\x2190", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftRightArrow 
			new Operator("\x2194", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftRightVector 
			new Operator("\x294e", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// LeftVector 
			new Operator("\x21bc", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DiacriticalAcute 
			new Operator("\xb4", Form.Postfix, zeroEM, zeroEM, false, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightArrow 
			new Operator("\x2192", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// RightVector 
			new Operator("\x21c0", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DiacriticalTilde 
			new Operator("\x2dc", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DoubleDot 
			new Operator("\xa8", Form.Postfix, zeroEM, zeroEM, false, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// DownBreve 
			new Operator("\x311", Form.Postfix, zeroEM, zeroEM, false, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Hacek 
			new Operator("\x2c7", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// Hat 
			new Operator("\x5e", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// OverBar 
			new Operator("\xaf", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// OverBrace 
			new Operator("\xfe37", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// OverBracket 
			new Operator("\x23b4", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// OverParenthesis 
			new Operator("\xfe35", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// TripleDot 
			new Operator("\x20db", Form.Postfix, zeroEM, zeroEM, false, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UnderBar 
			new Operator("\x332", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UnderBrace 
			new Operator("\xfe38", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UnderBracket 
			new Operator("\x23b5", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true),
			// UnderParenthesis 
			new Operator("\xfe36", Form.Postfix, zeroEM, zeroEM, true, false, true, false, false, false, new Length(LengthType.Px, 1), infinity, true)
		};

		public static Operator GetValue(string name, Form form)
		{
			// TODO lots !!!!
			for(int i=0; i<operators.Length; i++)
			{
				if(operators[i].Name == name)
				{
					Debug.WriteLine(String.Format("found operator attributes for \"{0}\"", name));
					return operators[i];
				}
			}
			Debug.WriteLine(String.Format("no operator attributes found for \"{0}\", returning default", name));
			return Operator.Default(form);
		}
	}
}
