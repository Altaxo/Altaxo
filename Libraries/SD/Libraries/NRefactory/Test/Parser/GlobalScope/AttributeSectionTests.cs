﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class AttributeSectionTests
	{
		[Test]
		public void AttributeOnStructure()
		{
			string program = @"
<StructLayout( LayoutKind.Explicit )> _
Public Structure MyUnion

	<FieldOffset( 0 )> Public i As Integer
	< FieldOffset( 0 )> Public d As Double
	
End Structure 'MyUnion
";
			TypeDeclaration decl = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			Assert.AreEqual("StructLayout", decl.Attributes[0].Attributes[0].Name);
		}
		
		[Test]
		public void AttributeOnModule()
		{
			string program = @"
<HideModule> _
Public Module MyExtra

	Public i As Integer
	Public d As Double
	
End Module
";
			TypeDeclaration decl = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			Assert.AreEqual("HideModule", decl.Attributes[0].Attributes[0].Name);
		}
		
		[Test]
		public void GlobalAttributeVB()
		{
			string program = @"<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class Form1
	
End Class";
			TypeDeclaration decl = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			Assert.AreEqual("Microsoft.VisualBasic.CompilerServices.DesignerGenerated", decl.Attributes[0].Attributes[0].Name);
		}
		
		[Test]
		public void GlobalAttributeCSharp()
		{
			string program = @"[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
[someprefix::DesignerGenerated()]
public class Form1 {
}";
			TypeDeclaration decl = ParseUtilCSharp.ParseGlobal<TypeDeclaration>(program);
			Assert.AreEqual("Microsoft.VisualBasic.CompilerServices.DesignerGenerated", decl.Attributes[0].Attributes[0].Name);
			Assert.AreEqual("someprefix.DesignerGenerated", decl.Attributes[1].Attributes[0].Name);
		}
		
		[Test]
		public void AssemblyAttributeCSharp()
		{
			string program = @"[assembly: System.Attribute()]";
			AttributeSection decl = ParseUtilCSharp.ParseGlobal<AttributeSection>(program);
			Assert.AreEqual(new Location(1, 1), decl.StartLocation);
			Assert.AreEqual("assembly", decl.AttributeTarget);
		}
		
		[Test]
		public void AssemblyAttributeCSharpWithNamedArguments()
		{
			string program = @"[assembly: Foo(1, namedArg: 2, prop = 3)]";
			AttributeSection decl = ParseUtilCSharp.ParseGlobal<AttributeSection>(program);
			Assert.AreEqual("assembly", decl.AttributeTarget);
			var a = decl.Attributes[0];
			Assert.AreEqual("Foo", a.Name);
			Assert.AreEqual(2, a.PositionalArguments.Count);
			Assert.AreEqual(1, a.NamedArguments.Count);
			Assert.AreEqual(1, ((PrimitiveExpression)a.PositionalArguments[0]).Value);
			NamedArgumentExpression nae = a.PositionalArguments[1] as NamedArgumentExpression;
			Assert.AreEqual("namedArg", nae.Name);
			Assert.AreEqual(2, ((PrimitiveExpression)nae.Expression).Value);
			nae = a.NamedArguments[0];
			Assert.AreEqual("prop", nae.Name);
			Assert.AreEqual(3, ((PrimitiveExpression)nae.Expression).Value);
		}
		
		[Test]
		public void ModuleAttributeCSharp()
		{
			string program = @"[module: System.Attribute()]";
			AttributeSection decl = ParseUtilCSharp.ParseGlobal<AttributeSection>(program);
			Assert.AreEqual(new Location(1, 1), decl.StartLocation);
			Assert.AreEqual("module", decl.AttributeTarget);
		}
		
		[Test]
		public void TypeAttributeCSharp()
		{
			string program = @"[type: System.Attribute()] class Test {}";
			TypeDeclaration type = ParseUtilCSharp.ParseGlobal<TypeDeclaration>(program);
			AttributeSection decl = type.Attributes[0];
			Assert.AreEqual(new Location(1, 1), decl.StartLocation);
			Assert.AreEqual("type", decl.AttributeTarget);
		}
		
		[Test]
		public void AssemblyAttributeVBNet()
		{
			string program = @"<assembly: System.Attribute()>";
			AttributeSection decl = ParseUtilVBNet.ParseGlobal<AttributeSection>(program);
			Assert.AreEqual(new Location(1, 1), decl.StartLocation);
			Assert.AreEqual("assembly", decl.AttributeTarget);
		}
		
		[Test]
		public void ModuleAttributeTargetEscapedVB()
		{
			// check that this doesn't crash the parser:
			ParseUtilVBNet.ParseGlobal<AttributeSection>("<[Module]: SuppressMessageAttribute>", true);
		}
	}
}
