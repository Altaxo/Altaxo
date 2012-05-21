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
	public class IdentifierExpressionTests
	{
		#region C#
		[Test]
		public void CSharpIdentifierExpressionTest1()
		{
			IdentifierExpression ident = ParseUtilCSharp.ParseExpression<IdentifierExpression>("MyIdentifier");
			Assert.AreEqual("MyIdentifier", ident.Identifier);
		}
		
		[Test]
		public void CSharpIdentifierExpressionTest2()
		{
			IdentifierExpression ident = ParseUtilCSharp.ParseExpression<IdentifierExpression>("@public");
			Assert.AreEqual("public", ident.Identifier);
		}
		
		[Test]
		public void CSharpGenericMethodReference()
		{
			IdentifierExpression ident = ParseUtilCSharp.ParseExpression<IdentifierExpression>("M<int>");
			Assert.AreEqual("M", ident.Identifier);
			Assert.AreEqual(1, ident.TypeArguments.Count);
		}
		
		[Test]
		public void CSharpGenericMethodReference2()
		{
			IdentifierExpression ident = ParseUtilCSharp.ParseExpression<IdentifierExpression>("TargetMethod<string>");
			Assert.AreEqual("TargetMethod", ident.Identifier);
			Assert.AreEqual(1, ident.TypeArguments.Count);
		}
		
		[Test]
		public void AsyncIsIdentifier()
		{
			IdentifierExpression ident = ParseUtilCSharp.ParseExpression<IdentifierExpression>("async");
			Assert.AreEqual("async", ident.Identifier);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetIdentifierExpressionTest1()
		{
			IdentifierExpression ie = ParseUtilVBNet.ParseExpression<IdentifierExpression>("MyIdentifier");
			Assert.AreEqual("MyIdentifier", ie.Identifier);
		}
		
		[Test]
		public void VBNetIdentifierExpressionTest2()
		{
			IdentifierExpression ie = ParseUtilVBNet.ParseExpression<IdentifierExpression>("[Public]");
			Assert.AreEqual("Public", ie.Identifier);
		}
		
		[Test]
		public void VBNetContextKeywordsTest()
		{
			Assert.AreEqual("Assembly", ParseUtilVBNet.ParseExpression<IdentifierExpression>("Assembly").Identifier);
			Assert.AreEqual("Custom", ParseUtilVBNet.ParseExpression<IdentifierExpression>("Custom").Identifier);
			Assert.AreEqual("Off", ParseUtilVBNet.ParseExpression<IdentifierExpression>("Off").Identifier);
			Assert.AreEqual("Explicit", ParseUtilVBNet.ParseExpression<IdentifierExpression>("Explicit").Identifier);
		}
		#endregion
	}
}
