﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Description of IResolver.
	/// </summary>
	public interface IResolver
	{
		/// <summary>
		/// Resolves an expression.
		/// The caretLineNumber and caretColumn is 1 based.
		/// </summary>
		ResolveResult Resolve(ExpressionResult expressionResult,
		                      int caretLineNumber,
		                      int caretColumn,
		                      string fileName,
		                      string fileContent);
		
		ArrayList CtrlSpace(int caretLine, int caretColumn, string fileName, string fileContent, ExpressionContext context);
	}
}
