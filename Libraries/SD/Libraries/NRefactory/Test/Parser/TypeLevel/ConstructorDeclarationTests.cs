﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class ConstructorDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpConstructorDeclarationTest1()
		{
			ConstructorDeclaration cd = ParseUtilCSharp.ParseTypeMember<ConstructorDeclaration>("MyClass() {}");
			Assert.IsTrue(cd.ConstructorInitializer.IsNull);
		}
		
		[Test]
		public void CSharpConstructorDeclarationTest2()
		{
			ConstructorDeclaration cd = ParseUtilCSharp.ParseTypeMember<ConstructorDeclaration>("MyClass() : this(5) {}");
			Assert.AreEqual(ConstructorInitializerType.This, cd.ConstructorInitializer.ConstructorInitializerType);
			Assert.AreEqual(1, cd.ConstructorInitializer.Arguments.Count);
		}
		
		[Test]
		public void CSharpConstructorDeclarationTest3()
		{
			ConstructorDeclaration cd = ParseUtilCSharp.ParseTypeMember<ConstructorDeclaration>("MyClass() : base(1, 2, 3) {}");
			Assert.AreEqual(ConstructorInitializerType.Base, cd.ConstructorInitializer.ConstructorInitializerType);
			Assert.AreEqual(3, cd.ConstructorInitializer.Arguments.Count);
		}
		
		[Test]
		public void CSharpStaticConstructorDeclarationTest1()
		{
			ConstructorDeclaration cd = ParseUtilCSharp.ParseTypeMember<ConstructorDeclaration>("static MyClass() {}");
			Assert.IsTrue(cd.ConstructorInitializer.IsNull);
			Assert.AreEqual(Modifiers.Static, cd.Modifier);
		}
		
		[Test]
		public void CSharpExternStaticConstructorDeclarationTest()
		{
			ConstructorDeclaration cd = ParseUtilCSharp.ParseTypeMember<ConstructorDeclaration>("extern static MyClass();");
			Assert.IsTrue(cd.ConstructorInitializer.IsNull);
			Assert.AreEqual(Modifiers.Static | Modifiers.Extern, cd.Modifier);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetConstructorDeclarationTest1()
		{
			string program = @"Sub New()
								End Sub";
			ConstructorDeclaration cd = ParseUtilVBNet.ParseTypeMember<ConstructorDeclaration>(program);
			Assert.IsTrue(cd.ConstructorInitializer.IsNull);
		}
		
		[Test]
		public void VBNetConstructorDeclarationTest2()
		{
			ConstructorDeclaration cd = ParseUtilVBNet.ParseTypeMember<ConstructorDeclaration>("Sub New(x As Integer, Optional y As String) \nEnd Sub");
			Assert.AreEqual(2, cd.Parameters.Count);
			Assert.AreEqual("System.Int32", cd.Parameters[0].TypeReference.Type);
			Assert.AreEqual("System.String", cd.Parameters[1].TypeReference.Type);
			Assert.AreEqual(ParameterModifiers.Optional, cd.Parameters[1].ParamModifier & ParameterModifiers.Optional);
		}
		#endregion
	}
}
