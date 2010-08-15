﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="unknown"/>
//     <version>$Revision: 5529 $</version>
// </file>

using ICSharpCode.NRefactory.PrettyPrinter;
using System;

namespace ICSharpCode.NRefactory.Ast
{
	public class PrimitiveExpression : Expression
	{
		string stringValue;
		
		public Parser.LiteralFormat LiteralFormat { get; set; }
		public object Value { get; set; }
		
		public string StringValue {
			get {
				if (stringValue == null)
					return CSharpOutputVisitor.ToCSharpString(this);
				else
					return stringValue;
			}
			set {
				stringValue = value == null ? String.Empty : value;
			}
		}
		
		public PrimitiveExpression(object val)
		{
			this.Value = val;
		}
		
		public PrimitiveExpression(object val, string stringValue)
		{
			this.Value       = val;
			this.StringValue = stringValue;
		}
		
		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitPrimitiveExpression(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[PrimitiveExpression: Value={1}, ValueType={2}, StringValue={0}]",
			                     this.StringValue,
			                     this.Value,
			                     this.Value == null ? "null" : this.Value.GetType().FullName
			                    );
		}
	}
}
