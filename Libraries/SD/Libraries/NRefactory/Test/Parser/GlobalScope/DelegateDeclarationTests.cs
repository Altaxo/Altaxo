﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class DelegateDeclarationTests
	{
		void TestDelegateDeclaration(DelegateDeclaration dd)
		{
			Assert.AreEqual("System.Void", dd.ReturnType.Type);
			Assert.AreEqual("MyDelegate", dd.Name);
		}
		
		void TestParameters(DelegateDeclaration dd)
		{
			Assert.AreEqual(3, dd.Parameters.Count);
			
			Assert.AreEqual("a", ((ParameterDeclarationExpression)dd.Parameters[0]).ParameterName);
			Assert.AreEqual("System.Int32", ((ParameterDeclarationExpression)dd.Parameters[0]).TypeReference.Type);
			
			Assert.AreEqual("secondParam", ((ParameterDeclarationExpression)dd.Parameters[1]).ParameterName);
			Assert.AreEqual("System.Int32", ((ParameterDeclarationExpression)dd.Parameters[1]).TypeReference.Type);
			
			Assert.AreEqual("lastParam", ((ParameterDeclarationExpression)dd.Parameters[2]).ParameterName);
			Assert.AreEqual("MyObj", ((ParameterDeclarationExpression)dd.Parameters[2]).TypeReference.Type);
		}
		
		#region C#
		[Test]
		public void SimpleCSharpDelegateDeclarationTest()
		{
			string program = "public delegate void MyDelegate(int a, int secondParam, MyObj lastParam);\n";
			TestDelegateDeclaration(ParseUtilCSharp.ParseGlobal<DelegateDeclaration>(program));
		}
		
		[Test]
		public void CSharpDelegateWithoutNameDeclarationTest()
		{
			string program = "public delegate void(int a, int secondParam, MyObj lastParam);\n";
			DelegateDeclaration dd = ParseUtilCSharp.ParseGlobal<DelegateDeclaration>(program, true);
			Assert.AreEqual("System.Void", dd.ReturnType.Type);
			//Assert.AreEqual("?", dd.Name);
			TestParameters(dd);
		}
		
		[Test]
		public void CSharpGenericDelegateDeclarationTest()
		{
			string program = "public delegate T CreateObject<T>(int a, int secondParam, MyObj lastParam) where T : ICloneable;\n";
			DelegateDeclaration dd = ParseUtilCSharp.ParseGlobal<DelegateDeclaration>(program);
			Assert.AreEqual("CreateObject", dd.Name);
			Assert.AreEqual("T", dd.ReturnType.Type);
			TestParameters(dd);
			Assert.AreEqual(1, dd.Templates.Count);
			Assert.AreEqual("T", dd.Templates[0].Name);
			Assert.AreEqual(1, dd.Templates[0].Bases.Count);
			Assert.AreEqual("ICloneable", dd.Templates[0].Bases[0].Type);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void SimpleVBNetDelegateDeclarationTest()
		{
			string program = "Public Delegate Sub MyDelegate(ByVal a As Integer, ByVal secondParam As Integer, ByVal lastParam As MyObj)\n";
			TestDelegateDeclaration(ParseUtilVBNet.ParseGlobal<DelegateDeclaration>(program));
		}
		#endregion
	}
}
