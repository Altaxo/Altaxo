using System;
using System.Collections;

namespace ICSharpCode.SharpRefactory.Parser.AST 
{
	public class BreakStatement : Statement
	{
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[BreakStatement]");
		}
	}
}
