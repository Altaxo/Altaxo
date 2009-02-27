// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3660 $</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.AstBuilder
{
	#if NET35
	/// <summary>
	/// Extension methods for NRefactory.Ast.Expression.
	/// </summary>
	public static class ExpressionBuilder
	{
		public static IdentifierExpression Identifier(string identifier)
		{
			return new IdentifierExpression(identifier);
		}
		
		public static MemberReferenceExpression Member(this Expression targetObject, string memberName)
		{
			if (targetObject == null)
				throw new ArgumentNullException("targetObject");
			return new MemberReferenceExpression(targetObject, memberName);
		}
		
		public static InvocationExpression Call(this Expression callTarget, string methodName, params Expression[] arguments)
		{
			if (callTarget == null)
				throw new ArgumentNullException("callTarget");
			return Call(Member(callTarget, methodName), arguments);
		}
		
		public static InvocationExpression Call(this Expression callTarget, params Expression[] arguments)
		{
			if (callTarget == null)
				throw new ArgumentNullException("callTarget");
			if (arguments == null)
				throw new ArgumentNullException("arguments");
			return new InvocationExpression(callTarget, new List<Expression>(arguments));
		}
		
		public static ObjectCreateExpression New(this TypeReference createType, params Expression[] arguments)
		{
			if (createType == null)
				throw new ArgumentNullException("createType");
			if (arguments == null)
				throw new ArgumentNullException("arguments");
			return new ObjectCreateExpression(createType, new List<Expression>(arguments));
		}
		
		public static Expression CreateDefaultValueForType(TypeReference type)
		{
			if (type != null && !type.IsArrayType) {
				switch (type.Type) {
					case "System.SByte":
					case "System.Byte":
					case "System.Int16":
					case "System.UInt16":
					case "System.Int32":
					case "System.UInt32":
					case "System.Int64":
					case "System.UInt64":
					case "System.Single":
					case "System.Double":
						return new PrimitiveExpression(0, "0");
					case "System.Char":
						return new PrimitiveExpression('\0', "'\\0'");
					case "System.Object":
					case "System.String":
						return new PrimitiveExpression(null, "null");
					case "System.Boolean":
						return new PrimitiveExpression(false, "false");
					default:
						return new DefaultValueExpression(type);
				}
			} else {
				return new PrimitiveExpression(null, "null");
			}
		}
	}
	#endif
}
