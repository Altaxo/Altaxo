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
	/// A way to provide opperations on a mathml tree.
	/// Currently, all Accept methods on mathml elements
	/// will just call Visit on the visitor, passing allong
	/// the args parameters, and returning the return value
	/// of the vistor.
	/// </summary>
	public interface MathMLVisitor
	{
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLApplyElement e, object args);

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLBvarElement e, object args);

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLFnElement e, object args);

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLLambdaElement e, object args); 

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLListElement e, object args); 

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLSetElement e, object args); 

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLMathElement e, object args); 

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLActionElement e, object args); 

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLEncloseElement e, object args); 

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLFencedElement e, object args); 

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLPaddedElement e, object args); 

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLStyleElement e, object args); 

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLTableCellElement e, object args); 			
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLDocument e, object args); 		
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLAnnotationElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLCaseElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLConditionElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLCiElement e, object args); 		
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLCnElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLCsymbolElement e, object args);
 		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>		
		object Visit(MathMLDeclareElement e, object args); 
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLIntervalElement e, object args); 
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>		
		object Visit(MathMLMatrixElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLMatrixRowElement e, object args); 
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>		
		object Visit(MathMLPiecewiseElement e, object args); 
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>		
		object Visit(MathMLPredefinedSymbol e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLVectorElement e, object args); 		
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLAlignGroupElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLAlignMarkElement e, object args); 		
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLFractionElement e, object args); 
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>		
		object Visit(MathMLGlyphElement e, object args); 		
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLMultiScriptsElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLOperatorElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLStringLitElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLRadicalElement e, object args); 		
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLScriptElement e, object args); 		
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLSpaceElement e, object args); 	

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLTableRowElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLTableElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLLabeledRowElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLUnderOverElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLSemanticsElement e, object args); 	
		
		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>	
		object Visit(MathMLXMLAnnotationElement e, object args); 	

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLPresentationToken e, object args);	

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLPresentationContainer e, object args);

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLSeparator e, object args);	

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLPlaceholderElement e, object args);

		/// <summary>
		/// Visit the specified MathML Element element
		/// </summary>
		/// <param name="e">the element that is being visited</param>
		/// <param name="args">user supplied arguments</param>
		/// <returns>user supplied result</returns>
		object Visit(MathMLContentContainer e, object args);		
	}
}
