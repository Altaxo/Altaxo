// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Visitors
{
	/// <summary>
	/// This class converts VB.NET constructs to their C# equivalents.
	/// Applying the VBNetToCSharpConvertVisitor on a CompilationUnit has the same effect
	/// as applying the VBNetConstructsConvertVisitor and ToCSharpConvertVisitor.
	/// </summary>
	public class VBNetToCSharpConvertVisitor : VBNetConstructsConvertVisitor
	{
		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			base.VisitCompilationUnit(compilationUnit, data);
			compilationUnit.AcceptVisitor(new ToCSharpConvertVisitor(), data);
			return null;
		}
	}
}
