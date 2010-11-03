﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class SkipMethodBodiesTest
	{
		[Test]
		public void EmptyMethods()
		{
			string txt = @"internal sealed class Lexer : AbstractLexer
			{
				public Lexer(TextReader reader) : base(reader)
				{
				}
				
				void Method()
				{
				}
			}";
			Check(ParseUtilCSharp.ParseGlobal<TypeDeclaration>(txt, false, true));
		}
		
		[Test]
		public void NonEmptyMethods()
		{
			string txt = @"internal sealed class Lexer : AbstractLexer
			{
				public Lexer(TextReader reader) : base(reader)
				{
					if (reader == null) {
						throw new ArgumentNullException(""reader"");
					}
				}
				
				void Method()
				{
					while(something) {
						if (anything)
							break;
					}
				}
			}";
			Check(ParseUtilCSharp.ParseGlobal<TypeDeclaration>(txt, false, true));
		}
		
		void Check(TypeDeclaration td)
		{
			Assert.AreEqual("Lexer", td.Name);
			Assert.AreEqual(2, td.Children.Count);
			Assert.AreEqual(0, ((ConstructorDeclaration)td.Children[0]).Body.Children.Count);
			Assert.AreEqual(0, ((MethodDeclaration)td.Children[1]).Body.Children.Count);
		}
	}
}
