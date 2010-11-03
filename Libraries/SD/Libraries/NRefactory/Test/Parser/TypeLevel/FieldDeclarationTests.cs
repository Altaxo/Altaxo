﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class FieldDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpSimpleFieldDeclarationTest()
		{
			FieldDeclaration fd = ParseUtilCSharp.ParseTypeMember<FieldDeclaration>("int[,,,] myField;");
			Assert.AreEqual("System.Int32", fd.TypeReference.Type);
			Assert.AreEqual(new int[] { 3 } , fd.TypeReference.RankSpecifier);
			Assert.AreEqual(1, fd.Fields.Count);
			
			Assert.AreEqual("myField", ((VariableDeclaration)fd.Fields[0]).Name);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleFieldDeclarationTest()
		{
			FieldDeclaration fd = ParseUtilVBNet.ParseTypeMember<FieldDeclaration>("myField As Integer(,,,)");
			Assert.AreEqual(1, fd.Fields.Count);
			
			Assert.AreEqual("System.Int32", ((VariableDeclaration)fd.Fields[0]).TypeReference.Type);
			Assert.AreEqual("System.Int32", ((VariableDeclaration)fd.Fields[0]).TypeReference.Type);
			Assert.AreEqual("myField", ((VariableDeclaration)fd.Fields[0]).Name);
			Assert.AreEqual(new int[] { 3 } , ((VariableDeclaration)fd.Fields[0]).TypeReference.RankSpecifier);
		}
		
		[Test]
		public void VBNetMultiFieldDeclarationTest()
		{
			FieldDeclaration fd = ParseUtilVBNet.ParseTypeMember<FieldDeclaration>("a, b As String");
			Assert.AreEqual(2, fd.Fields.Count);
			
			Assert.AreEqual("System.String", ((VariableDeclaration)fd.Fields[0]).TypeReference.Type);
			Assert.IsFalse(((VariableDeclaration)fd.Fields[0]).TypeReference.IsArrayType);
			Assert.AreEqual("System.String", ((VariableDeclaration)fd.Fields[1]).TypeReference.Type);
			Assert.IsFalse(((VariableDeclaration)fd.Fields[1]).TypeReference.IsArrayType);
		}
		
		[Test]
		public void VBNetMultiFieldsOnSingleLineTest()
		{
			string program = "Class TestClass : Dim a : Dim b : End Class";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			
			Assert.AreEqual(2, td.Children.Count);
			Assert.IsTrue(td.Children[0] is FieldDeclaration);
			Assert.IsTrue(td.Children[1] is FieldDeclaration);
		}
		
		[Test]
		public void VBNetMultiFieldDeclarationTest2()
		{
			FieldDeclaration fd = ParseUtilVBNet.ParseTypeMember<FieldDeclaration>("Dim a, b() As String");
			Assert.AreEqual(2, fd.Fields.Count);
			
			Assert.AreEqual("System.String", ((VariableDeclaration)fd.Fields[0]).TypeReference.Type);
			Assert.AreEqual("System.String", ((VariableDeclaration)fd.Fields[1]).TypeReference.Type);
			Assert.IsFalse(((VariableDeclaration)fd.Fields[0]).TypeReference.IsArrayType);
			Assert.IsTrue(((VariableDeclaration)fd.Fields[1]).TypeReference.IsArrayType);
		}
		
		[Test]
		public void VBNetMultiFieldDeclarationTest3()
		{
			FieldDeclaration fd = ParseUtilVBNet.ParseTypeMember<FieldDeclaration>("Dim a(), b As String");
			Assert.AreEqual(2, fd.Fields.Count);
			
			Assert.AreEqual("System.String", ((VariableDeclaration)fd.Fields[0]).TypeReference.Type);
			Assert.AreEqual("System.String", ((VariableDeclaration)fd.Fields[1]).TypeReference.Type);
			Assert.IsTrue(((VariableDeclaration)fd.Fields[0]).TypeReference.IsArrayType);
			Assert.IsFalse(((VariableDeclaration)fd.Fields[1]).TypeReference.IsArrayType);
		}
		#endregion
	}
}
