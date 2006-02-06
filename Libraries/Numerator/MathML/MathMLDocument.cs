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
using System.IO;

namespace MathML
{
	/// <summary>
	/// This interface extends the XmlDocument interface to add access to document 
	/// properties relating to navigation. The documentElement attribute for a 
	/// MathMLDocument should be the MathMLMathElement representing the top-level 
	/// math element which is the root of the document
	/// </summary>
	public class MathMLDocument : XmlDocument
	{
		/// <summary>
		/// create a new MathML document
		/// </summary>
		public MathMLDocument()
		{
		}

        /// <summary>
        /// load the document from the a file specified by the given filename
        /// </summary>
        /// <param name="filename"></param>		
		public override void Load(string filename) 
		{
			MathMLReader reader = new MathMLReader(filename, NameTable);

			try 
			{
				Load(reader);
			}
			finally 
			{
				reader.Close();
			}
		}

        /// <summary>
        /// load the mathml document from the specified stream
        /// </summary>
        /// <param name="inStream"></param>
		public override void Load(Stream inStream) 
		{
			MathMLReader reader = new MathMLReader(inStream);

			try 
			{
				this.Load(reader);
			}
			finally 
			{
				reader.Close();
			}
		}

        /// <summary>
        /// load the document from the spedified text reader
        /// </summary>
        /// <param name="txtReader"></param>
		public override void Load(TextReader txtReader) 
		{
			MathMLReader reader = new MathMLReader(txtReader);
			try 
			{
				this.Load(reader);
			}
			finally 
			{
				reader.Close();
			}
		}
	
		/// <summary>
		/// override the base LoadXML to use a MathMLReader instead of a 
		/// XmlReader in order to resolve mathml entities.
		/// </summary>
		/// <param name="xml">a string in xml format</param>
		public override void LoadXml(string xml) 
		{
			StringReader strReader = new StringReader(xml);
			MathMLReader reader = new MathMLReader(strReader);
			try 
			{
				this.Load(reader);
			}
			finally 
			{
				reader.Close();
			}
		}	
	
		/// <summary>
		/// Create a XmlElement. This is typically called by the base class when creating a DOM tree
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="localname"></param>
		/// <param name="nsURI"></param>
		/// <returns></returns>
		public override XmlElement CreateElement(string prefix, string localname, string nsURI)
		{
			XmlElement result = null;
			switch(localname)
			{
				case "math":
					result = new MathMLMathElement(prefix, localname, nsURI, this);
					break;
				case "mi":
				case "mn":
				case "mtext":
					result = new MathMLPresentationToken(prefix, localname, nsURI, this);
					break;
				case "mo":			
					result = new MathMLOperatorElement(prefix, localname, nsURI, this);
					break;
				case "mspace":			
					result = new MathMLSpaceElement(prefix, localname, nsURI, this);
					break;
				case "ms":			
					result = new MathMLStringLitElement(prefix, localname, nsURI, this);
					break;
				case "mglyph":			
					result = new MathMLGlyphElement(prefix, localname, nsURI, this);
					break;
				case "mrow":
				case "merror":
				case "mphantom":			
					result = new MathMLPresentationContainer(prefix, localname, nsURI, this);
					break;
				case "mfrac":			
					result = new MathMLFractionElement(prefix, localname, nsURI, this);
					break;
				case "msqrt":			
				case "mroot":			
					result = new MathMLRadicalElement(prefix, localname, nsURI, this);
					break;
				case "mstyle":			
					result = new MathMLStyleElement(prefix, localname, nsURI, this);
					break;
				case "mpadded":			
					result = new MathMLPaddedElement(prefix, localname, nsURI, this);
					break;
				case "mfenced":			
					result = new MathMLFencedElement(prefix, localname, nsURI, this);
					break;
				case "menclose":			
					result = new MathMLEncloseElement(prefix, localname, nsURI, this);
					break;
				case "msub":
				case "msup":
				case "msubsup":			
					result = new MathMLScriptElement(prefix, localname, nsURI, this);
					break;
				case "munder":			
				case "mover":			
				case "munderover":			
					result = new MathMLUnderOverElement(prefix, localname, nsURI, this);
					break;
				case "mmultiscripts":			
					result = new MathMLMultiScriptsElement(prefix, localname, nsURI, this);
					break;
				case "mtable":			
					result = new MathMLTableElement(prefix, localname, nsURI, this);
					break;
				case "mlabeledtr":			
					result = new MathMLLabeledRowElement(prefix, localname, nsURI, this);
					break;
				case "mtr":			
					result = new MathMLTableRowElement(prefix, localname, nsURI, this);
					break;
				case "mtd":			
					result = new MathMLTableCellElement(prefix, localname, nsURI, this);
					break;
				case "maligngroup":			
					result = new MathMLAlignGroupElement(prefix, localname, nsURI, this);
					break;
				case "malignmark":			
					result = new MathMLAlignMarkElement(prefix, localname, nsURI, this);
					break;
				case "maction":			
					result = new MathMLActionElement(prefix, localname, nsURI, this);
					break;
				case "cn":			
					result = new MathMLCnElement(prefix, localname, nsURI, this);
					break;
				case "ci":			
					result = new MathMLCiElement(prefix, localname, nsURI, this);
					break;
				case "csymbol":			
					result = new MathMLCsymbolElement(prefix, localname, nsURI, this);
					break;
				case "apply":			
					result = new MathMLApplyElement(prefix, localname, nsURI, this);
					break;
				case "fn":			
					result = new MathMLFnElement(prefix, localname, nsURI, this);
					break;
				case "interval":			
					result = new MathMLIntervalElement(prefix, localname, nsURI, this);
					break;
				case "inverse":			
				case "compose":	
				case "ident":		
				case "domain":			
				case "codomain":		
				case "image":		
				case "quotient":			
				case "exp":			
				case "factorial":			
				case "divide":			
				case "max":			
				case "min":			
				case "minus":			
				case "plus":			
				case "power":			
				case "rem":			
				case "times":			
				case "root":			
				case "gcd":			
				case "and":			
				case "or":			
				case "xor":			
				case "not":			
				case "implies":			
				case "forall":			
				case "exists":			
				case "abs":			
				case "conjugate":			
				case "arg":			
				case "real":			
				case "imaginary":			
				case "lcm":			
				case "floor":			
				case "ceiling":			
				case "eq":			
				case "neq":			
				case "gt":			
				case "lt":			
				case "geq":			
				case "leq":			
				case "equivalent":			
				case "approx":			
				case "factorof":			
				case "int":			
				case "diff":			
				case "partialdiff":		
				case "divergence":			
				case "grad":			
				case "curl":			
				case "laplacian":	
				case "union":			
				case "intersect":			
				case "in":			
				case "notin":			
				case "subset":			
				case "prsubset":			
				case "notsubset":			
				case "notprsubset":			
				case "setdiff":
				case "card":			
				case "cartesianproduct":			
				case "sum":			
				case "product":			
				case "limit":			
				case "tendsto":			
				case "ln":			
				case "log":			
				case "sin":			
				case "cos":			
				case "tan":			
				case "sec":			
				case "csc":			
				case "cot":			
				case "sinh":			
				case "cosh":			
				case "tanh":			
				case "sech":			
				case "csch":			
				case "coth":			
				case "arcsin":			
				case "arccos":			
				case "arctan":			
				case "arccosh":			
				case "arccot":			
				case "arccoth":			
				case "arccsc":			
				case "arccsch":			
				case "arcsec":			
				case "arcsech":			
				case "arcsinh":			
				case "arctanh":			
				case "mean":			
				case "sdev":			
				case "variance":			
				case "median":			
				case "mode":			
				case "moment":		
				case "determinant":			
				case "transpose":			
				case "selector":			
				case "vectorproduct":			
				case "scalarproduct":
				case "outerproduct":	
				case "integers":		
				case "reals":	
				case "rationals":	
				case "naturalnumbers":	
				case "complexes":	
				case "primes":	
				case "exponentiale":	
				case "imaginaryi":		
				case "notanumber":
				case "true":	
				case "false":			
				case "emptyset":			
				case "pi":			
				case "eulergamma":			
				case "infinity":			
					result = new MathMLPredefinedSymbol(prefix, localname, nsURI, this);
					break;
				case "condition":			
					result = new MathMLConditionElement(prefix, localname, nsURI, this);
					break;
				case "declare":			
					result = new MathMLDeclareElement(prefix, localname, nsURI, this);
					break;
				case "lambda":			
					result = new MathMLLambdaElement(prefix, localname, nsURI, this);
					break;
				case "piecewise":			
					result = new MathMLPiecewiseElement(prefix, localname, nsURI, this);
					break;
				case "piecev":			
					result = new MathMLCaseElement(prefix, localname, nsURI, this);
					break;
				case "reln":
				case "domainofapplication":	
				case "otherwise":			
				case "lowlimit":			
				case "uplimit":	
				case "degree":	
				case "momentabout":		
				case "logbase": // TODO is this correct, 'logbase' not specified in mathml docs????
					result = new MathMLContentContainer(prefix, localname, nsURI, this);
					break;
				case "bvar":			
					result = new MathMLBvarElement(prefix, localname, nsURI, this);
					break;
				case "set":			
					result = new MathMLSetElement(prefix, localname, nsURI, this);
					break;
				case "list":			
					result = new MathMLListElement(prefix, localname, nsURI, this);
					break;				
				case "vector":			
					result = new MathMLVectorElement(prefix, localname, nsURI, this);
					break;
				case "matrix":			
					result = new MathMLMatrixElement(prefix, localname, nsURI, this);
					break;
				case "matrixrow":			
					result = new MathMLMatrixRowElement(prefix, localname, nsURI, this);
					break;
				case "annotation":			
					result = new MathMLAnnotationElement(prefix, localname, nsURI, this);
					break;
				case "semantics":			
					result = new MathMLSemanticsElement(prefix, localname, nsURI, this);
					break;
				case "annotation-xml":			
					result = new MathMLXMLAnnotationElement(prefix, localname, nsURI, this);
					break;
				case "sep":
					result = new MathMLSeparator(prefix, localname, nsURI, this);
					break;
				case "placeholder":
					result = new MathMLPlaceholderElement(prefix, localname, nsURI, this);
					break;
				default:
					result = base.CreateElement(prefix, localname, nsURI);
					break;
			}
			return result;
		}

		/// <summary>
		/// create a text node that is trimmed of leading or trailing whitespace
		/// </summary>
		public override XmlText CreateTextNode(string text)
		{
			return base.CreateTextNode (text.Trim());
		}

		/// <summary>
		/// Get the root of the mathml tree, this will allways be a 'math' element.
		/// If the mathml document happens to be a plain xml, or a html document, the
		/// true document element is not a 'math' element, but rather a 'html' or 
		/// some other type of node. This finds the first 'math' element.
		/// </summary>
		public new MathMLElement DocumentElement 
		{
			get
			{
				XmlElement element = base.DocumentElement;
				if(element is MathMLElement)
				{
					return (MathMLElement)element;
				}
				else
				{
					XmlNodeList list = element.GetElementsByTagName("math");
					XmlNode math = null;
					if(list.Count > 0 && (math = list.Item(0)) != null && math is MathMLElement)
					{
						return (MathMLElement)math;
					}
					else
					{
						throw new Exception("Error, can not find \"math\" element in document");
					}
				}
			}
		}
	}
}
