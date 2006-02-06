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
using System.Xml;


namespace MathML
{
	/// <summary>
	/// This interface supports all of the empty built-in operator, relation, function, 
	/// and constant and symbol elements that have the definitionURL and encoding attributes
	/// in addition to the standard set of attributes. The elements supported in order 
	/// of their appearance in Section 4.4 are: inverse, compose, ident, domain, codomain,
	/// image, quotient, exp, factorial, divide, max, min, minus, plus, power, rem, times, 
	/// root, gcd, and, or, xor, not, implies, forall, exists, abs,	conjugate, arg, real, 
	/// imaginary, lcm, floor, ceiling, eq, neq, gt, lt, geq, leq, equivalent, approx, 
	/// factorof, ln, log, int, diff, partialdiff, divergence, grad, curl, laplacian, union, 
	/// intersect, in, notin, subset, prsubset, notsubset, notprsubset, setdiff, card, 
	/// cartesianproduct, sum, product, limit, tendsto, sin, cos, tan, sec, csc, cot, 
	/// sinh, cosh, tanh, sech, csch, coth, arcsin, arccos, arctan, arcsec, arccsc, 
	/// arccot, arcsinh, arccosh, arctanh, arcsech, arccsch, arccoth, mean, sdev, variance, 
	/// median, mode, moment, determinant, transpose, selector, vectorproduct, scalarproduct, 
	/// outerproduct, integers, reals, rationals, naturalnumbers, complexes, primes, 
	/// exponentiale, imaginaryi, notanumber, true, false, emptyset, pi, eulergamma, and infinity.
	/// </summary>
	public class MathMLPredefinedSymbol : MathMLContentElement
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLPredefinedSymbol(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		/// A string that provides an override to the default semantics, or provides a more specific definition
		/// </summary>
		public string DefinitionURL
		{
			get { return GetAttribute("definitionURL"); }
			set { SetAttribute("definitionURL", value); }
		}
		
		/// <summary>
		/// A string describing the syntax in which the definition located at definitionURL is given.
		/// </summary>
		public string Encoding
		{
			get { return GetAttribute("encoding"); }
			set { SetAttribute("encoding", value); }
		}
		
		/// <summary>
		/// A string representing the number of arguments. Values include 0, 1, ... and variable.
		/// </summary>
		public string Arity 
		{
			get 
			{ 
				string name = Name;
				// optimize me, this probably should be stored in some sort of 
				// hash table or binary searchable table
				foreach(ArityPair pair in arities)
				{
					if(pair.Name == name)
					{
						return pair.Value;
					}
				}
				return "?";
			}
		}

		/// <summary>
		/// A string giving the name of the MathML element represented. This is a convenience attribute
		/// only; accessing it should be synonymous with accessing the Element::tagName attribute.
		/// </summary>
		public string SymbolName
		{
			get { return Name; }
		}	

		/// <summary>
		/// accept a visitor.
		/// return the return value of the visitor's visit method
		/// </summary>
		public override object Accept(MathMLVisitor v, object args)
		{
			return v.Visit(this, args);
		}

		private struct ArityPair
		{
			public ArityPair(string n, string v)
			{
				Name = n;
				Value = v;
			}

			public readonly string Name;
			public readonly string Value;
		}

		private static readonly ArityPair[] arities = 
		{
			new ArityPair("inverse", "1"),
			new ArityPair("compose", "variable"),
			new ArityPair("ident", "0"), // is this right???
			new ArityPair("domain", "?"),
			new ArityPair("codomain", "?"),
			new ArityPair("image", "?"),
			new ArityPair("quotient", "?"),
			new ArityPair("exp", "1"),
			new ArityPair("factorial" ,"1"),
			new ArityPair("divide","2"),
			new ArityPair("max", "?"),
			new ArityPair("min", "?"),
			new ArityPair("minus", "2"),
			new ArityPair("plus", "2"),
			new ArityPair("power", "2"),
			new ArityPair("rem", "2"),
			new ArityPair("times", "2"),
			new ArityPair("root", "?"),
			new ArityPair("gcd", "?"),
			new ArityPair("and", "?"),
			new ArityPair("or", "?"),
			new ArityPair("xor", "?"),
			new ArityPair("not", "?"),
			new ArityPair("implies", "?"),
			new ArityPair("forall", "?"),
			new ArityPair("exists", "?"),
			new ArityPair("abs", "?"),
			new ArityPair("conjugate", "?"),
			new ArityPair("arg", "?"),
			new ArityPair("real", "?"),
			new ArityPair("imaginary", "?"),
			new ArityPair("lcm", "?"),
			new ArityPair("floor", "?"),
			new ArityPair("ceiling", "?"),
			new ArityPair("eq", "?"),
			new ArityPair("neq", "?"),
			new ArityPair("gt", "?"),
			new ArityPair("lt", "?"),
			new ArityPair("geq", "?"),
			new ArityPair("leq", "?"),
			new ArityPair("equivalent", "?"),
			new ArityPair("approx", "?"),  
			new ArityPair("factorof", "?"),
			new ArityPair("ln", "?"),
			new ArityPair("log", "?"),
			new ArityPair("int", "?"),
			new ArityPair("diff", "?"),
			new ArityPair("partialdiff", "?"),
			new ArityPair("divergence", "?"),
			new ArityPair("grad", "?"),
			new ArityPair("curl", "?"),
			new ArityPair("laplacian", "?"),
			new ArityPair("union", "?"),
			new ArityPair("intersect", "?"),
			new ArityPair("in", "?"),
			new ArityPair("notin", "?"),
			new ArityPair("subset", "?"),
			new ArityPair("prsubset", "?"),
			new ArityPair("notsubset", "?"),
			new ArityPair("notprsubset", "?"),
			new ArityPair("setdiff", "?"),
			new ArityPair("card", "?"),
			new ArityPair("cartesianproduct", "?"),
			new ArityPair("sum", "?"),
			new ArityPair("product", "?"),
			new ArityPair("limit", "?"),
			new ArityPair("tendsto", "?"),
			new ArityPair("sin", "1"),
			new ArityPair("cos", "1"),
			new ArityPair("tan", "1"),
			new ArityPair("sec", "1"),
			new ArityPair("csc", "1"),
			new ArityPair("cot", "1"),
			new ArityPair("sinh", "1"),
			new ArityPair("cosh", "1"),
			new ArityPair("tanh", "1"),
			new ArityPair("sech", "1"),
			new ArityPair("csch", "1"),
			new ArityPair("coth", "1"),
			new ArityPair("arcsin", "1"),
			new ArityPair("arccos", "1"),
			new ArityPair("arctan", "1"),
			new ArityPair("arcsec", "1"),
			new ArityPair("arccsc", "1"),
			new ArityPair("arccot", "1"),
			new ArityPair("arcsinh", "1"),
			new ArityPair("arccosh", "1"),
			new ArityPair("arctanh", "1"),
			new ArityPair("arcsech", "1"),
			new ArityPair("arccsch", "1"),
			new ArityPair("arccoth", "1"),
			new ArityPair("mean", "variable"),
			new ArityPair("sdev", "variable"),
			new ArityPair("variance", "?"),
			new ArityPair("median", "variable"),
			new ArityPair("mode", "?"),
			new ArityPair("moment", "?"),
			new ArityPair("determinant", "?"),
			new ArityPair("transpose", "?"),
			new ArityPair("selector", "?"),
			new ArityPair("vectorproduct", "?"),
			new ArityPair("scalarproduct", "?"),  
			new ArityPair("outerproduct", "?"),
			new ArityPair("integers", "?"),
			new ArityPair("reals", "?"),
			new ArityPair("rationals", "?"),
			new ArityPair("naturalnumbers", "?"),
			new ArityPair("complexes", "?"),
			new ArityPair("primes", "?"),
			new ArityPair("exponentiale", "?"),
			new ArityPair("imaginaryi", "?"),
			new ArityPair("notanumber", "?"),
			new ArityPair("true", "?"),
			new ArityPair("false", "?"),
			new ArityPair("emptyset", "?"),
			new ArityPair("pi", "?"),
			new ArityPair("eulergamma", "?"),
			new ArityPair("infinity", "?")
		};
	}
}