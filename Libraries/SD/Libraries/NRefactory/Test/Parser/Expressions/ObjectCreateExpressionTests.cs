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
	public class ObjectCreateExpressionTests
	{
		void CheckSimpleObjectCreateExpression(ObjectCreateExpression oce)
		{
			Assert.AreEqual("MyObject", oce.CreateType.Type);
			Assert.AreEqual(3, oce.Parameters.Count);
			Assert.IsTrue(oce.ObjectInitializer.IsNull);
			
			for (int i = 0; i < oce.Parameters.Count; ++i) {
				Assert.IsTrue(oce.Parameters[i] is PrimitiveExpression);
			}
		}
		
		#region C#
		[Test]
		public void CSharpSimpleObjectCreateExpressionTest()
		{
			CheckSimpleObjectCreateExpression(ParseUtilCSharp.ParseExpression<ObjectCreateExpression>("new MyObject(1, 2, 3)"));
		}
		
		[Test]
		public void CSharpNullableObjectCreateExpressionTest()
		{
			ObjectCreateExpression oce = ParseUtilCSharp.ParseExpression<ObjectCreateExpression>("new IntPtr?(1)");
			Assert.AreEqual("System.Nullable", oce.CreateType.Type);
			Assert.AreEqual(1, oce.CreateType.GenericTypes.Count);
			Assert.AreEqual("IntPtr", oce.CreateType.GenericTypes[0].Type);
		}
		
		[Test]
		public void CSharpInvalidNestedObjectCreateExpressionTest()
		{
			// this test was written because this bug caused the AbstractASTVisitor to crash
			
			InvocationExpression expr = ParseUtilCSharp.ParseExpression<InvocationExpression>("WriteLine(new MyObject(1, 2, 3,))", true);
			Assert.IsTrue(expr.TargetObject is IdentifierExpression);
			Assert.AreEqual("WriteLine", ((IdentifierExpression)expr.TargetObject).Identifier);
			
			Assert.AreEqual(1, expr.Arguments.Count); // here a second null parameter was added incorrectly
			
			Assert.IsTrue(expr.Arguments[0] is ObjectCreateExpression);
			CheckSimpleObjectCreateExpression((ObjectCreateExpression)expr.Arguments[0]);
		}
		
		[Test]
		public void CSharpInvalidTypeArgumentListObjectCreateExpressionTest()
		{
			// this test was written because this bug caused the AbstractASTVisitor to crash
			
			InvocationExpression expr = ParseUtilCSharp.ParseExpression<InvocationExpression>("WriteLine(new SomeGenericType<int, >())", true);
			Assert.IsTrue(expr.TargetObject is IdentifierExpression);
			Assert.AreEqual("WriteLine", ((IdentifierExpression)expr.TargetObject).Identifier);
			Assert.AreEqual(1, expr.Arguments.Count); // here a second null parameter was added incorrectly
			
			Assert.IsTrue(expr.Arguments[0] is ObjectCreateExpression);
			TypeReference typeRef = ((ObjectCreateExpression)expr.Arguments[0]).CreateType;
			Assert.AreEqual("SomeGenericType", typeRef.Type);
			Assert.AreEqual(1, typeRef.GenericTypes.Count);
			Assert.AreEqual("System.Int32", typeRef.GenericTypes[0].Type);
		}
		
		Expression CheckPropertyInitializationExpression(Expression e, string name)
		{
			Assert.IsInstanceOf(typeof(MemberInitializerExpression), e);
			Assert.AreEqual(name, ((MemberInitializerExpression)e).Name);
			return ((MemberInitializerExpression)e).Expression;
		}
		
		void CheckPointObjectCreation(ObjectCreateExpression oce)
		{
			Assert.AreEqual(0, oce.Parameters.Count);
			Assert.AreEqual(2, oce.ObjectInitializer.CreateExpressions.Count);
			Assert.IsInstanceOf(typeof(PrimitiveExpression), CheckPropertyInitializationExpression(oce.ObjectInitializer.CreateExpressions[0], "X"));
			Assert.IsInstanceOf(typeof(PrimitiveExpression), CheckPropertyInitializationExpression(oce.ObjectInitializer.CreateExpressions[1], "Y"));
		}
		
		[Test]
		public void CSharpObjectInitializer()
		{
			CheckPointObjectCreation(ParseUtilCSharp.ParseExpression<ObjectCreateExpression>("new Point() { X = 0, Y = 1 }"));
		}
		
		[Test]
		public void CSharpObjectInitializerWithoutParenthesis()
		{
			CheckPointObjectCreation(ParseUtilCSharp.ParseExpression<ObjectCreateExpression>("new Point { X = 0, Y = 1 }"));
		}
		
		[Test]
		public void CSharpObjectInitializerTrailingComma()
		{
			CheckPointObjectCreation(ParseUtilCSharp.ParseExpression<ObjectCreateExpression>("new Point() { X = 0, Y = 1, }"));
		}
		
		[Test]
		public void CSharpNestedObjectInitializer()
		{
			ObjectCreateExpression oce = ParseUtilCSharp.ParseExpression<ObjectCreateExpression>(
				"new Rectangle { P1 = new Point { X = 0, Y = 1 }, P2 = new Point { X = 2, Y = 3 } }"
			);
			Assert.AreEqual(0, oce.Parameters.Count);
			Assert.AreEqual(2, oce.ObjectInitializer.CreateExpressions.Count);
			CheckPointObjectCreation((ObjectCreateExpression)CheckPropertyInitializationExpression(oce.ObjectInitializer.CreateExpressions[0], "P1"));
			CheckPointObjectCreation((ObjectCreateExpression)CheckPropertyInitializationExpression(oce.ObjectInitializer.CreateExpressions[1], "P2"));
		}
		
		[Test]
		public void CSharpNestedObjectInitializerForPreinitializedProperty()
		{
			ObjectCreateExpression oce = ParseUtilCSharp.ParseExpression<ObjectCreateExpression>(
				"new Rectangle { P1 = { X = 0, Y = 1 }, P2 = { X = 2, Y = 3 } }"
			);
			Assert.AreEqual(0, oce.Parameters.Count);
			Assert.AreEqual(2, oce.ObjectInitializer.CreateExpressions.Count);
			CollectionInitializerExpression aie = (CollectionInitializerExpression)CheckPropertyInitializationExpression(oce.ObjectInitializer.CreateExpressions[0], "P1");
			Assert.IsInstanceOf(typeof(PrimitiveExpression), CheckPropertyInitializationExpression(aie.CreateExpressions[0], "X"));
			Assert.IsInstanceOf(typeof(PrimitiveExpression), CheckPropertyInitializationExpression(aie.CreateExpressions[1], "Y"));
			aie = (CollectionInitializerExpression)CheckPropertyInitializationExpression(oce.ObjectInitializer.CreateExpressions[1], "P2");
			Assert.IsInstanceOf(typeof(PrimitiveExpression), CheckPropertyInitializationExpression(aie.CreateExpressions[0], "X"));
			Assert.IsInstanceOf(typeof(PrimitiveExpression), CheckPropertyInitializationExpression(aie.CreateExpressions[1], "Y"));
		}
		
		[Test]
		public void CSharpCollectionInitializer()
		{
			ObjectCreateExpression oce = ParseUtilCSharp.ParseExpression<ObjectCreateExpression>(
				"new List<int> { 0, 1, 2 }"
			);
			Assert.AreEqual(0, oce.Parameters.Count);
			Assert.AreEqual(3, oce.ObjectInitializer.CreateExpressions.Count);
			Assert.IsInstanceOf(typeof(PrimitiveExpression), oce.ObjectInitializer.CreateExpressions[0]);
			Assert.IsInstanceOf(typeof(PrimitiveExpression), oce.ObjectInitializer.CreateExpressions[1]);
			Assert.IsInstanceOf(typeof(PrimitiveExpression), oce.ObjectInitializer.CreateExpressions[2]);
		}
		
		[Test]
		public void CSharpComplexCollectionInitializer()
		{
			ObjectCreateExpression oce = ParseUtilCSharp.ParseExpression<ObjectCreateExpression>(
				@"new List<Contact> {
	new Contact {
		Name = ""Chris"",
		PhoneNumbers = { ""206-555-0101"" }
	},
	new Contact(additionalParameter) {
		Name = ""Bob"",
		PhoneNumbers = { ""650-555-0199"", ""425-882-8080"" }
	}
}"			);
			Assert.AreEqual(0, oce.Parameters.Count);
			Assert.AreEqual(2, oce.ObjectInitializer.CreateExpressions.Count);
			
			oce = (ObjectCreateExpression)oce.ObjectInitializer.CreateExpressions[1]; // look at Bob
			Assert.AreEqual(1, oce.Parameters.Count);
			Assert.IsInstanceOf(typeof(IdentifierExpression), oce.Parameters[0]);
			Assert.IsInstanceOf(typeof(PrimitiveExpression), CheckPropertyInitializationExpression(oce.ObjectInitializer.CreateExpressions[0], "Name"));
			CollectionInitializerExpression phoneNumbers = (CollectionInitializerExpression)CheckPropertyInitializationExpression(oce.ObjectInitializer.CreateExpressions[1], "PhoneNumbers");
			Assert.AreEqual(2, phoneNumbers.CreateExpressions.Count);
		}
		
		[Test]
		public void CSharpAnonymousType()
		{
			ObjectCreateExpression oce = ParseUtilCSharp.ParseExpression<ObjectCreateExpression>(
				"new { Name = \"Test\", Price, Something.Property }"
			);
			Assert.IsTrue(oce.CreateType.IsNull);
			Assert.AreEqual(0, oce.Parameters.Count);
			Assert.AreEqual(3, oce.ObjectInitializer.CreateExpressions.Count);
			Assert.IsInstanceOf(typeof(PrimitiveExpression), CheckPropertyInitializationExpression(oce.ObjectInitializer.CreateExpressions[0], "Name"));
			Assert.IsInstanceOf(typeof(IdentifierExpression), oce.ObjectInitializer.CreateExpressions[1]);
			Assert.IsInstanceOf(typeof(MemberReferenceExpression), oce.ObjectInitializer.CreateExpressions[2]);
		}
		#endregion
		
		#region VB.NET
		
		[Test]
		public void VBNetAnonymousType()
		{
			ObjectCreateExpression oce = ParseUtilVBNet.ParseExpression<ObjectCreateExpression>(
				"New With {.Id = 1, .Name= \"Bill Gates\" }");
			
			Assert.IsTrue(oce.CreateType.IsNull);
			Assert.AreEqual(0, oce.Parameters.Count);
			Assert.AreEqual(2, oce.ObjectInitializer.CreateExpressions.Count);
			
			Assert.IsInstanceOf(typeof(PrimitiveExpression), CheckPropertyInitializationExpression(oce.ObjectInitializer.CreateExpressions[0], "Id"));
			Assert.IsInstanceOf(typeof(MemberInitializerExpression), oce.ObjectInitializer.CreateExpressions[1]);
		}
		
		[Test]
		public void VBNetAnonymousTypeWithoutProperty()
		{
			ObjectCreateExpression oce = ParseUtilVBNet.ParseExpression<ObjectCreateExpression>("New With { c }");
			
			Assert.IsTrue(oce.CreateType.IsNull);
			Assert.AreEqual(0, oce.Parameters.Count);
			Assert.AreEqual(1, oce.ObjectInitializer.CreateExpressions.Count);
			
			Assert.IsInstanceOf(typeof(IdentifierExpression), oce.ObjectInitializer.CreateExpressions[0]);
			Assert.AreEqual("c", (oce.ObjectInitializer.CreateExpressions[0] as IdentifierExpression).Identifier);
		}
		
		[Test]
		public void VBNetSimpleObjectCreateExpressionTest()
		{
			CheckSimpleObjectCreateExpression(ParseUtilVBNet.ParseExpression<ObjectCreateExpression>("New MyObject(1, 2, 3)"));
		}
		
		[Test]
		public void VBNetInvalidTypeArgumentListObjectCreateExpressionTest()
		{
			// this test was written because this bug caused the AbstractASTVisitor to crash
			
			InvocationExpression expr = ParseUtilVBNet.ParseExpression<InvocationExpression>("WriteLine(New SomeGenericType(Of Integer, )())", true);
			Assert.IsTrue(expr.TargetObject is IdentifierExpression);
			Assert.AreEqual("WriteLine", ((IdentifierExpression)expr.TargetObject).Identifier);
			Assert.AreEqual(1, expr.Arguments.Count); // here a second null parameter was added incorrectly
			
			Assert.IsTrue(expr.Arguments[0] is ObjectCreateExpression);
			TypeReference typeRef = ((ObjectCreateExpression)expr.Arguments[0]).CreateType;
			Assert.AreEqual("SomeGenericType", typeRef.Type);
			Assert.AreEqual(1, typeRef.GenericTypes.Count);
			Assert.AreEqual("System.Int32", typeRef.GenericTypes[0].Type);
		}
		
		[Test]
		public void VBNetMemberInitializationTest()
		{
			ObjectCreateExpression oce = ParseUtilVBNet.ParseExpression<ObjectCreateExpression>("new Contact() With { .FirstName = \"Bill\", .LastName = \"Gates\" }");
			Assert.AreEqual(2, oce.ObjectInitializer.CreateExpressions.Count);
			
			Assert.AreEqual("FirstName", ((MemberInitializerExpression)oce.ObjectInitializer.CreateExpressions[0]).Name);
			Assert.AreEqual("LastName", ((MemberInitializerExpression)oce.ObjectInitializer.CreateExpressions[1]).Name);
			
			Assert.IsInstanceOf(typeof(PrimitiveExpression), ((MemberInitializerExpression)oce.ObjectInitializer.CreateExpressions[0]).Expression);
			Assert.IsInstanceOf(typeof(PrimitiveExpression), ((MemberInitializerExpression)oce.ObjectInitializer.CreateExpressions[1]).Expression);
		}
		
		[Test]
		public void VBNetNullableObjectCreateExpressionTest()
		{
			ObjectCreateExpression oce = ParseUtilVBNet.ParseExpression<ObjectCreateExpression>("New Integer?");
			Assert.AreEqual("System.Nullable", oce.CreateType.Type);
			Assert.AreEqual(1, oce.CreateType.GenericTypes.Count);
			Assert.AreEqual("System.Int32", oce.CreateType.GenericTypes[0].Type);
		}

		[Test]
		public void VBNetNullableObjectArrayCreateExpressionTest()
		{
			ObjectCreateExpression oce = ParseUtilVBNet.ParseExpression<ObjectCreateExpression>("New Integer?()");
			Assert.AreEqual("System.Nullable", oce.CreateType.Type);
			Assert.AreEqual(1, oce.CreateType.GenericTypes.Count);
			Assert.AreEqual("System.Int32", oce.CreateType.GenericTypes[0].Type);
		}
		
		#endregion
	}
}
