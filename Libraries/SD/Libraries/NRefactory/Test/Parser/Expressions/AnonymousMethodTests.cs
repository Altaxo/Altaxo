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
	public class AnonymousMethodTests
	{
		AnonymousMethodExpression Parse(string program)
		{
			return ParseUtilCSharp.ParseExpression<AnonymousMethodExpression>(program);
		}
		
		[Test]
		public void AnonymousMethodWithoutParameterList()
		{
			AnonymousMethodExpression ame = Parse("delegate {}");
			Assert.AreEqual(0, ame.Parameters.Count);
			Assert.AreEqual(0, ame.Body.Children.Count);
			Assert.IsFalse(ame.HasParameterList);
		}
		
		[Test]
		public void AnonymousMethodAfterCast()
		{
			CastExpression c = ParseUtilCSharp.ParseExpression<CastExpression>("(ThreadStart)delegate {}");
			Assert.AreEqual("ThreadStart", c.CastTo.Type);
			AnonymousMethodExpression ame = (AnonymousMethodExpression)c.Expression;
			Assert.AreEqual(0, ame.Parameters.Count);
			Assert.AreEqual(0, ame.Body.Children.Count);
		}
		
		[Test]
		public void EmptyAnonymousMethod()
		{
			AnonymousMethodExpression ame = Parse("delegate() {}");
			Assert.AreEqual(0, ame.Parameters.Count);
			Assert.AreEqual(0, ame.Body.Children.Count);
			Assert.IsTrue(ame.HasParameterList);
		}
		
		[Test]
		public void SimpleAnonymousMethod()
		{
			AnonymousMethodExpression ame = Parse("delegate(int a, int b) { return a + b; }");
			Assert.AreEqual(2, ame.Parameters.Count);
			// blocks can't be added without compilation unit -> anonymous method body
			// is always empty when using ParseExpression
			//Assert.AreEqual(1, ame.Body.Children.Count);
			//Assert.IsTrue(ame.Body.Children[0] is ReturnStatement);
		}
		
		[Test]
		public void AsyncSimpleAnonymousMethod()
		{
			AnonymousMethodExpression ame = Parse("async delegate { }");
			Assert.IsTrue(ame.IsAsync);
		}
	}
}
