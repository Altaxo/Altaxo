﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class UsingDeclarationTests
	{
		void CheckTwoSimpleUsings(CompilationUnit u)
		{
			Assert.AreEqual(2, u.Children.Count);
			Assert.IsTrue(u.Children[0] is UsingDeclaration);
			UsingDeclaration ud = (UsingDeclaration)u.Children[0];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsTrue(!ud.Usings[0].IsAlias);
			Assert.AreEqual("System", ud.Usings[0].Name);
			
			
			Assert.IsTrue(u.Children[1] is UsingDeclaration);
			ud = (UsingDeclaration)u.Children[1];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsTrue(!ud.Usings[0].IsAlias);
			Assert.AreEqual("My.Name.Space", ud.Usings[0].Name);
		}
		
		void CheckAliases(CompilationUnit u)
		{
			Assert.AreEqual(3, u.Children.Count);
			
			Assert.IsTrue(u.Children[0] is UsingDeclaration);
			UsingDeclaration ud = (UsingDeclaration)u.Children[0];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsTrue(((Using)ud.Usings[0]).IsAlias);
			Assert.AreEqual("TESTME", ud.Usings[0].Name);
			Assert.AreEqual("System", ud.Usings[0].Alias.Type);
			
			Assert.IsTrue(u.Children[1] is UsingDeclaration);
			ud = (UsingDeclaration)u.Children[1];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsTrue(((Using)ud.Usings[0]).IsAlias);
			Assert.AreEqual("myAlias", ud.Usings[0].Name);
			Assert.AreEqual("My.Name.Space", ud.Usings[0].Alias.Type);
			
			Assert.IsTrue(u.Children[2] is UsingDeclaration);
			ud = (UsingDeclaration)u.Children[2];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsTrue(((Using)ud.Usings[0]).IsAlias);
			Assert.AreEqual("StringCollection", ud.Usings[0].Name);
			Assert.AreEqual("System.Collections.Generic.List", ud.Usings[0].Alias.Type);
			Assert.AreEqual("System.String", ud.Usings[0].Alias.GenericTypes[0].Type);
		}
		
		#region C#
		[Test]
		public void CSharpWrongUsingTest()
		{
			string program = "using\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(program));
			parser.Parse();
			Assert.IsTrue(parser.Errors.Count > 0);
		}
		
		[Test]
		public void CSharpDeclarationTest()
		{
			string program = "using System;\n" +
				"using My.Name.Space;\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CheckTwoSimpleUsings(parser.CompilationUnit);
		}
		
		[Test]
		public void CSharpUsingAliasDeclarationTest()
		{
			string program = "using TESTME=System;\n" +
				"using myAlias=My.Name.Space;\n" +
				"using StringCollection = System.Collections.Generic.List<string>;\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CheckAliases(parser.CompilationUnit);
		}
		
		[Test]
		public void CSharpUsingWithAliasing()
		{
			string program = "using global::System;\n" +
				"using myAlias=global::My.Name.Space;\n" +
				"using a::b.c;\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CompilationUnit u = parser.CompilationUnit;
			Assert.AreEqual(3, u.Children.Count);
			
			Assert.IsTrue(u.Children[0] is UsingDeclaration);
			UsingDeclaration ud = (UsingDeclaration)u.Children[0];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsFalse(((Using)ud.Usings[0]).IsAlias);
			Assert.AreEqual("System", ud.Usings[0].Name);
			
			Assert.IsTrue(u.Children[1] is UsingDeclaration);
			ud = (UsingDeclaration)u.Children[1];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsTrue(((Using)ud.Usings[0]).IsAlias);
			Assert.AreEqual("myAlias", ud.Usings[0].Name);
			Assert.AreEqual("My.Name.Space", ud.Usings[0].Alias.Type);
			
			Assert.IsTrue(u.Children[2] is UsingDeclaration);
			ud = (UsingDeclaration)u.Children[2];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsFalse(((Using)ud.Usings[0]).IsAlias);
			Assert.AreEqual("a.b.c", ud.Usings[0].Name);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetWrongUsingTest()
		{
			string program = "Imports\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(program));
			parser.Parse();
			Assert.IsTrue(parser.Errors.Count > 0);
			UsingDeclaration u = (UsingDeclaration)parser.CompilationUnit.Children[0];
			foreach (Using us in u.Usings) {
				Assert.IsNotNull(us);
			}
		}
		
		[Test]
		public void VBNetWrongUsing2Test()
		{
			string program = "Imports ,\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(program));
			parser.Parse();
			Assert.IsTrue(parser.Errors.Count > 0);
			UsingDeclaration u = (UsingDeclaration)parser.CompilationUnit.Children[0];
			foreach (Using us in u.Usings) {
				Assert.IsNotNull(us);
			}
		}
		
		[Test]
		public void VBNetDeclarationTest()
		{
			string program = "Imports System\n" +
				"Imports My.Name.Space\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CheckTwoSimpleUsings(parser.CompilationUnit);
		}
		
		[Test]
		public void VBNetUsingAliasDeclarationTest()
		{
			string program = "Imports TESTME=System\n" +
				"Imports myAlias=My.Name.Space\n" +
				"Imports StringCollection = System.Collections.Generic.List(Of string)\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CheckAliases(parser.CompilationUnit);
		}
		
		[Test]
		public void VBNetComplexUsingAliasDeclarationTest()
		{
			string program = "Imports NS1, AL=NS2, NS3, AL2=NS4, NS5\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			// TODO : Extend test ...
		}
		
		[Test]
		public void VBNetXmlNamespaceUsingTest()
		{
			string program = "Imports <xmlns=\"http://icsharpcode.net/sharpdevelop/avalonedit\">";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CompilationUnit unit = parser.CompilationUnit;
			
			Assert.AreEqual(1, unit.Children.Count);
			Assert.IsTrue(unit.Children[0] is UsingDeclaration);
			UsingDeclaration ud = (UsingDeclaration)unit.Children[0];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsFalse(ud.Usings[0].IsAlias);
			Assert.IsTrue(ud.Usings[0].IsXml);
			
			Assert.AreEqual("xmlns", ud.Usings[0].XmlPrefix);
			Assert.AreEqual("http://icsharpcode.net/sharpdevelop/avalonedit", ud.Usings[0].Name);
		}
		
		[Test]
		public void VBNetXmlNamespaceWithPrefixUsingTest()
		{
			string program = "Imports <xmlns:avalonedit=\"http://icsharpcode.net/sharpdevelop/avalonedit\">";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CompilationUnit unit = parser.CompilationUnit;
			
			Assert.AreEqual(1, unit.Children.Count);
			Assert.IsTrue(unit.Children[0] is UsingDeclaration);
			UsingDeclaration ud = (UsingDeclaration)unit.Children[0];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsFalse(ud.Usings[0].IsAlias);
			Assert.IsTrue(ud.Usings[0].IsXml);
			
			Assert.AreEqual("xmlns:avalonedit", ud.Usings[0].XmlPrefix);
			Assert.AreEqual("http://icsharpcode.net/sharpdevelop/avalonedit", ud.Usings[0].Name);
		}
		
		[Test]
		public void VBNetXmlNamespaceSingleQuotedUsingTest()
		{
			string program = "Imports <xmlns='http://icsharpcode.net/sharpdevelop/avalonedit'>";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CompilationUnit unit = parser.CompilationUnit;
			
			Assert.AreEqual(1, unit.Children.Count);
			Assert.IsTrue(unit.Children[0] is UsingDeclaration);
			UsingDeclaration ud = (UsingDeclaration)unit.Children[0];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsFalse(ud.Usings[0].IsAlias);
			Assert.IsTrue(ud.Usings[0].IsXml);
			
			Assert.AreEqual("xmlns", ud.Usings[0].XmlPrefix);
			Assert.AreEqual("http://icsharpcode.net/sharpdevelop/avalonedit", ud.Usings[0].Name);
		}
		
		[Test]
		public void VBNetXmlNamespaceSingleQuotedWithPrefixUsingTest()
		{
			string program = "Imports <xmlns:avalonedit='http://icsharpcode.net/sharpdevelop/avalonedit'>";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CompilationUnit unit = parser.CompilationUnit;
			
			Assert.AreEqual(1, unit.Children.Count);
			Assert.IsTrue(unit.Children[0] is UsingDeclaration);
			UsingDeclaration ud = (UsingDeclaration)unit.Children[0];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsFalse(ud.Usings[0].IsAlias);
			Assert.IsTrue(ud.Usings[0].IsXml);
			
			Assert.AreEqual("xmlns:avalonedit", ud.Usings[0].XmlPrefix);
			Assert.AreEqual("http://icsharpcode.net/sharpdevelop/avalonedit", ud.Usings[0].Name);
		}
		#endregion
	}
}
