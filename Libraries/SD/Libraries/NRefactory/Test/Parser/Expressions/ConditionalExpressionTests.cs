﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class ConditionalExpressionTests
	{
		#region C#
		[Test]
		public void CSharpConditionalExpressionTest()
		{
			ConditionalExpression ce = ParseUtilCSharp.ParseExpression<ConditionalExpression>("a == b ? a() : a.B");
			
			Assert.IsTrue(ce.Condition is BinaryOperatorExpression);
			Assert.IsTrue(ce.TrueExpression is InvocationExpression);
			Assert.IsTrue(ce.FalseExpression is MemberReferenceExpression);
		}
		
		[Test]
		public void CSharpConditionalIsExpressionTest()
		{
			// (as is b?) ERROR (conflict with nullables, SD2-419)
			ConditionalExpression ce = ParseUtilCSharp.ParseExpression<ConditionalExpression>("a is b ? a() : a.B");
			
			Assert.IsTrue(ce.Condition is TypeOfIsExpression);
			Assert.IsTrue(ce.TrueExpression is InvocationExpression);
			Assert.IsTrue(ce.FalseExpression is MemberReferenceExpression);
		}
		
		[Test]
		public void CSharpConditionalIsWithNullableExpressionTest()
		{
			ConditionalExpression ce = ParseUtilCSharp.ParseExpression<ConditionalExpression>("a is b? ? a() : a.B");
			
			Assert.IsTrue(ce.Condition is TypeOfIsExpression);
			Assert.IsTrue(ce.TrueExpression is InvocationExpression);
			Assert.IsTrue(ce.FalseExpression is MemberReferenceExpression);
		}
		
		[Test]
		public void CSharpConditionalIsExpressionTest2()
		{
			ConditionalExpression ce = ParseUtilCSharp.ParseExpression<ConditionalExpression>("a is b ? (a()) : a.B");
			
			Assert.IsTrue(ce.Condition is TypeOfIsExpression);
			Assert.IsTrue(ce.TrueExpression is ParenthesizedExpression);
			Assert.IsTrue(ce.FalseExpression is MemberReferenceExpression);
		}
		
		[Test]
		public void CSharpConditionalExpressionNegativeValue()
		{
			ConditionalExpression ce = ParseUtilCSharp.ParseExpression<ConditionalExpression>("isNegative ? -1 : 1");
			
			Assert.IsTrue(ce.Condition is IdentifierExpression);
			Assert.IsTrue(ce.TrueExpression is UnaryOperatorExpression);
			Assert.IsTrue(ce.FalseExpression is PrimitiveExpression);
		}
		
		
		[Test]
		public void CSharpConditionalIsWithNegativeValue()
		{
			ConditionalExpression ce = ParseUtilCSharp.ParseExpression<ConditionalExpression>("a is b ? -1 : 1");
			
			Assert.IsTrue(ce.Condition is TypeOfIsExpression);
			Assert.IsTrue(ce.TrueExpression is UnaryOperatorExpression);
			Assert.IsTrue(ce.FalseExpression is PrimitiveExpression);
		}
		
		[Test]
		public void CSharpConditionalIsWithExplicitPositiveValue()
		{
			ConditionalExpression ce = ParseUtilCSharp.ParseExpression<ConditionalExpression>("a is b ? +1 : 1");
			
			Assert.IsTrue(ce.Condition is TypeOfIsExpression);
			Assert.IsTrue(ce.TrueExpression is UnaryOperatorExpression);
			Assert.IsTrue(ce.FalseExpression is PrimitiveExpression);
		}
		
		[Test]
		public void CSharpRepeatedConditionalExpr()
		{
			ConditionalExpression ce = ParseUtilCSharp.ParseExpression<ConditionalExpression>("a ? b : c ? d : e");
			
			Assert.AreEqual("a", ((IdentifierExpression)ce.Condition).Identifier);
			Assert.AreEqual("b", ((IdentifierExpression)ce.TrueExpression).Identifier);
			Assert.IsTrue(ce.FalseExpression is ConditionalExpression);
		}
		
		[Test]
		public void CSharpNestedConditionalExpr()
		{
			ConditionalExpression ce = ParseUtilCSharp.ParseExpression<ConditionalExpression>("a ? b ? c : d : e");
			
			Assert.AreEqual("a", ((IdentifierExpression)ce.Condition).Identifier);
			Assert.AreEqual("e", ((IdentifierExpression)ce.FalseExpression).Identifier);
			Assert.IsTrue(ce.TrueExpression is ConditionalExpression);
		}
		#endregion
		
		#region VB.NET
		
		[Test]
		public void VBNetConditionalExpressionTest()
		{
			ConditionalExpression ce = ParseUtilVBNet.ParseExpression<ConditionalExpression>("If(x IsNot Nothing, x.Test, \"nothing\")");
			
			Assert.IsTrue(ce.Condition is BinaryOperatorExpression);
			Assert.IsTrue(ce.TrueExpression is MemberReferenceExpression);
			Assert.IsTrue(ce.FalseExpression is PrimitiveExpression);
		}
		
		#endregion
	}
}
