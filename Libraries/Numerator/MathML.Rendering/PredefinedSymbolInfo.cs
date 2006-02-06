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

namespace MathML.Rendering
{
	/// <summary>
	/// type of predefined symbol
	/// </summary>
	internal enum PredefinedSymbolType
	{
		Prefix, Infix, Postfix, Fraction, Function, Power, Root, Fenced, Exp, Log
	}

	/// <summary>
	/// Addational info needed to render a MathMLPredefinedSymbol
	/// This strucrture defines the operator order (pre,in,postfix order)
	/// and the character(s) that make up a given 
	/// </summary>
	internal class PredefinedSymbolInfo
	{
		// store all the items in a static hashtable
		private static Hashtable table = new Hashtable(20);

		// populate the table
		static PredefinedSymbolInfo()
		{
			/// predefined symbols:
			/// inverse, 
			/// compose, 
			/// ident, 
			/// domain, 
			/// codomain,
			/// image, 
			/// quotient, 
			table.Add("exp", new PredefinedSymbolInfo(PredefinedSymbolType.Exp, "e", false, true));  
			table.Add("factorial", new PredefinedSymbolInfo(PredefinedSymbolType.Postfix, "!", false, true)); 
			table.Add("divide", new PredefinedSymbolInfo(PredefinedSymbolType.Fraction, "", false, false)); 
			/// max, 
			/// min, 
			table.Add("minus", new PredefinedSymbolInfo(PredefinedSymbolType.Infix, "\x2212", true, true)); // minus sign, U+2212 ISOtech 
			table.Add("plus", new PredefinedSymbolInfo(PredefinedSymbolType.Infix, "+", true, false)); 
			table.Add("power", new PredefinedSymbolInfo(PredefinedSymbolType.Power, "", false, true));
			table.Add("rem", new PredefinedSymbolInfo(PredefinedSymbolType.Infix, "mod", false, false)); 
			table.Add("times", new PredefinedSymbolInfo(PredefinedSymbolType.Infix, "\x2062", false, true)); // invisible times
			table.Add("root", new PredefinedSymbolInfo(PredefinedSymbolType.Root, "?", false, false)); 
			table.Add("gcd", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "gcd", true, false)); 
			/// and, 
			/// or, 
			/// xor, 
			/// not, 
			/// implies, 
			/// forall, 
			/// exists, 
			table.Add("abs", new PredefinedSymbolInfo(PredefinedSymbolType.Fenced, "||", false, false));
			/// conjugate, 
			/// arg, 
			table.Add("real", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "\x211c", false, false)); // blackletter capital R = real part symbol, U+211C ISOamso  
			/// imaginary, 
			table.Add("lcm", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "lcm", false, false)); 
			table.Add("floor", new PredefinedSymbolInfo(PredefinedSymbolType.Fenced, "\x230a\x230b", false, false)); //, '\x0'), // right floor, U+230B ISOamsc  
			table.Add("ceiling", new PredefinedSymbolInfo(PredefinedSymbolType.Fenced, "\x2308\x2309", false, false));
			table.Add("eq", new PredefinedSymbolInfo(PredefinedSymbolType.Infix, "=", false, false)); 
			/// neq, 
			/// gt, 
			/// lt, 
			/// geq, 
			/// leq, 
			/// equivalent, 
			/// approx, 
			/// factorof, 
			table.Add("ln", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "ln", true, false)); 
			table.Add("log", new PredefinedSymbolInfo(PredefinedSymbolType.Log, "log", true, false));
			/// int, 
			/// diff, 
			/// partialdiff, 
			/// divergence, 
			/// grad, 
			/// curl, 
			/// laplacian, 
			/// union, 
			/// intersect, 
			/// in, 
			/// notin, 
			/// subset, 
			/// prsubset, 
			/// notsubset, 
			/// notprsubset, 
			/// setdiff, 
			/// card, 
			/// cartesianproduct, 
			/// sum, 
			/// product, 
			/// limit, 
			/// tendsto, 
			table.Add("sin", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "sin", false, false)); 
			table.Add("cos", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "cos", false, false)); 
			table.Add("tan", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "tan", false, false)); 
			table.Add("sec", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "sec", false, false)); 
			table.Add("csc", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "csc", false, false)); 
			table.Add("cot", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "sin", false, false)); 
			table.Add("sinh", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "sinh", false, false)); 
			table.Add("cosh", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "cosh", false, false)); 
			table.Add("tanh", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "tanh", false, false)); 
			table.Add("sech", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "sech", false, false)); 
			table.Add("csch", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "csch", false, false)); 
			table.Add("coth", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "coth", false, false)); 
			table.Add("arcsin", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "arcsin", false, false)); 
			table.Add("arccos", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "arccos", false, false)); 
			table.Add("arctan", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "arctan", false, false)); 
			table.Add("arcsec", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "arcsec", false, false)); 
			table.Add("arccsc", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "arccsc", false, false)); 
			table.Add("arccot", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "arccot", false, false)); 
			table.Add("arcsinh", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "arcsinh", false, false)); 
			table.Add("arccosh", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "arccosh", false, false)); 
			table.Add("arctanh", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "arctanh", false, false)); 
			table.Add("arcsech", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "arcsech", false, false)); 
			table.Add("arccsch", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "arccsch", false, false)); 
			table.Add("arccoth", new PredefinedSymbolInfo(PredefinedSymbolType.Function, "arccoth", false, false)); 
			/// mean, 
			/// sdev, 
			/// variance, 
			/// median, 
			/// mode, 
			/// moment, 
			/// determinant, 
			/// transpose, 
			/// selector, 
			/// vectorproduct, 
			/// scalarproduct, 
			/// outerproduct, 
			/// integers, 
			/// reals, 
			/// rationals, 
			/// naturalnumbers, 
			/// complexes, 
			/// primes, 
			/// exponentiale, 
			/// imaginaryi, 
			table.Add("notanumber", new PredefinedSymbolInfo(PredefinedSymbolType.Infix, "NaN", false, false)); 
			/// true, 
			/// false, 
			/// emptyset, 
			/// pi, 
			/// eulergamma, 
			/// infinity.
				
			//imaginary
			//conjugate

			
			//infinity
		}

        /// <summary>
        /// lookup the predefined symbol from the symbol name
        /// </summary>
		public static PredefinedSymbolInfo Get(String name)
		{
			return (PredefinedSymbolInfo)table[name];
		}

		private PredefinedSymbolInfo(PredefinedSymbolType t, String val, bool parens, bool childParens)
		{
			Type = t;
			Value = val;
			Parens = parens;
			ChildParens = childParens;
		}

		/// <summary>
		/// how this predefined symbol should be rendered
		/// </summary>
		public readonly PredefinedSymbolType Type;

		/// <summary>
		/// the character(s) that make up this symbol
		/// </summary>
		public readonly String Value;

		/// <summary>
		/// place parens around an infix operator group.
		/// </summary>
		public readonly bool Parens;

        /// <summary>
        /// Should child elements of this element surround themselves with parens.
        /// </summary>
		public readonly bool ChildParens;

		public const String ApplyFunction = "\x2061";
	}
}
