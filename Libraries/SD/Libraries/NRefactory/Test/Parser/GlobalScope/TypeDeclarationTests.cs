﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class TypeDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpSimpleClassTypeDeclarationTest()
		{
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>("class MyClass  : My.Base.Class  { }");
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual("My.Base.Class", td.BaseTypes[0].Type);
			Assert.AreEqual(Modifiers.None, td.Modifier);
		}
		
		[Test]
		public void CSharpSimpleClassRegionTest()
		{
			const string program = "class MyClass\n{\n}\n";
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>(program);
			Assert.AreEqual(1, td.StartLocation.Line, "StartLocation.Y");
			Assert.AreEqual(1, td.StartLocation.Column, "StartLocation.X");
			Assert.AreEqual(1, td.BodyStartLocation.Line, "BodyStartLocation.Y");
			Assert.AreEqual(14, td.BodyStartLocation.Column, "BodyStartLocation.X");
			Assert.AreEqual(3, td.EndLocation.Line, "EndLocation.Y");
			Assert.AreEqual(2, td.EndLocation.Column, "EndLocation.Y");
		}
		
		[Test]
		public void CSharpSimplePartialClassTypeDeclarationTest()
		{
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>("partial class MyClass { }");
			Assert.IsNotNull(td);
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual(Modifiers.Partial, td.Modifier);
		}
		
		[Test]
		public void CSharpNestedClassesTest()
		{
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>("class MyClass { partial class P1 {} public partial class P2 {} static class P3 {} internal static class P4 {} }");
			Assert.IsNotNull(td);
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual(Modifiers.Partial, ((TypeDeclaration)td.Children[0]).Modifier);
			Assert.AreEqual(Modifiers.Partial | Modifiers.Public, ((TypeDeclaration)td.Children[1]).Modifier);
			Assert.AreEqual(Modifiers.Static, ((TypeDeclaration)td.Children[2]).Modifier);
			Assert.AreEqual(Modifiers.Static | Modifiers.Internal, ((TypeDeclaration)td.Children[3]).Modifier);
		}
		
		[Test]
		public void CSharpSimpleStaticClassTypeDeclarationTest()
		{
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>("static class MyClass { }");
			Assert.IsNotNull(td);
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual(Modifiers.Static, td.Modifier);
		}
		
		[Test]
		public void CSharpGenericClassTypeDeclarationTest()
		{
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>("public class G<T> {}");
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("G", td.Name);
			Assert.AreEqual(Modifiers.Public, td.Modifier);
			Assert.AreEqual(0, td.BaseTypes.Count);
			Assert.AreEqual(1, td.Templates.Count);
			Assert.AreEqual("T", td.Templates[0].Name);
		}
		
		
		[Test]
		public void CSharpGenericClassWithWhere()
		{
			string declr = @"
public class Test<T> where T : IMyInterface
{
}
";
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>(declr);
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("Test", td.Name);
			
			Assert.AreEqual(1, td.Templates.Count);
			Assert.AreEqual("T", td.Templates[0].Name);
			Assert.AreEqual("IMyInterface", td.Templates[0].Bases[0].Type);
		}
		
		[Test]
		public void CSharpComplexGenericClassTypeDeclarationTest()
		{
			string declr = @"
public class Generic<T, S> : System.IComparable where S : G<T[]> where  T : MyNamespace.IMyInterface
{
}
";
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>(declr);
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("Generic", td.Name);
			Assert.AreEqual(Modifiers.Public, td.Modifier);
			Assert.AreEqual(1, td.BaseTypes.Count);
			Assert.AreEqual("System.IComparable", td.BaseTypes[0].Type);
			
			Assert.AreEqual(2, td.Templates.Count);
			Assert.AreEqual("T", td.Templates[0].Name);
			Assert.AreEqual("MyNamespace.IMyInterface", td.Templates[0].Bases[0].Type);
			
			Assert.AreEqual("S", td.Templates[1].Name);
			Assert.AreEqual("G", td.Templates[1].Bases[0].Type);
			Assert.AreEqual(1, td.Templates[1].Bases[0].GenericTypes.Count);
			Assert.IsTrue(td.Templates[1].Bases[0].GenericTypes[0].IsArrayType);
			Assert.AreEqual("T", td.Templates[1].Bases[0].GenericTypes[0].Type);
			Assert.AreEqual(new int[] {0}, td.Templates[1].Bases[0].GenericTypes[0].RankSpecifier);
		}
		
		[Test]
		public void CSharpComplexClassTypeDeclarationTest()
		{
			string declr = @"
[MyAttr()]
public abstract class MyClass : MyBase, Interface1, My.Test.Interface2
{
}
";
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>(declr);
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual(Modifiers.Public | Modifiers.Abstract, td.Modifier);
			Assert.AreEqual(1, td.Attributes.Count);
			Assert.AreEqual(3, td.BaseTypes.Count);
			Assert.AreEqual("MyBase", td.BaseTypes[0].Type);
			Assert.AreEqual("Interface1", td.BaseTypes[1].Type);
			Assert.AreEqual("My.Test.Interface2", td.BaseTypes[2].Type);
		}
		
		[Test]
		public void CSharpSimpleStructTypeDeclarationTest()
		{
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>("struct MyStruct {}");
			
			Assert.AreEqual(ClassType.Struct, td.Type);
			Assert.AreEqual("MyStruct", td.Name);
		}
		
		[Test]
		public void CSharpSimpleInterfaceTypeDeclarationTest()
		{
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>("interface MyInterface {}");
			
			Assert.AreEqual(ClassType.Interface, td.Type);
			Assert.AreEqual("MyInterface", td.Name);
		}
		
		[Test]
		public void CSharpSimpleEnumTypeDeclarationTest()
		{
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>("enum MyEnum {}");
			
			Assert.AreEqual(ClassType.Enum, td.Type);
			Assert.AreEqual("MyEnum", td.Name);
		}
		
		[Test]
		public void ContextSensitiveKeywordTest()
		{
			TypeDeclaration td = ParseUtilCSharp.ParseGlobal<TypeDeclaration>("partial class partial<[partial: where] where> where where : partial<where> { }");
			
			Assert.AreEqual(Modifiers.Partial, td.Modifier);
			Assert.AreEqual("partial", td.Name);
			
			Assert.AreEqual(1, td.Templates.Count);
			TemplateDefinition tp = td.Templates[0];
			Assert.AreEqual("where", tp.Name);
			
			Assert.AreEqual(1, tp.Attributes.Count);
			Assert.AreEqual("partial", tp.Attributes[0].AttributeTarget);
			Assert.AreEqual(1, tp.Attributes[0].Attributes.Count);
			Assert.AreEqual("where", tp.Attributes[0].Attributes[0].Name);
			
			Assert.AreEqual(1, tp.Bases.Count);
			Assert.AreEqual("partial", tp.Bases[0].Type);
			Assert.AreEqual("where", tp.Bases[0].GenericTypes[0].Type);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleClassTypeDeclarationTest()
		{
			string program = "Class TestClass\n" +
				"End Class\n";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			
			Assert.AreEqual("TestClass", td.Name);
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual(1, td.StartLocation.Line, "start line");
			Assert.AreEqual(1, td.BodyStartLocation.Line, "bodystart line");
			Assert.AreEqual(16, td.BodyStartLocation.Column, "bodystart col");
			Assert.AreEqual(2, td.EndLocation.Line, "end line");
			Assert.AreEqual(10, td.EndLocation.Column, "end col");
		}
		
		[Test]
		public void VBNetMissingBaseClassTest()
		{
			// SD2-1499: test that this invalid code doesn't crash
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>("public class test inherits", true);
			Assert.AreEqual(0, td.BaseTypes.Count);
		}
		
		[Test]
		public void VBNetEnumWithBaseClassDeclarationTest()
		{
			string program = "Enum TestEnum As Byte\n" +
				"End Enum\n";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			
			Assert.AreEqual("TestEnum", td.Name);
			Assert.AreEqual(ClassType.Enum, td.Type);
			Assert.AreEqual("System.Byte", td.BaseTypes[0].Type);
			Assert.AreEqual(0, td.Children.Count);
		}
		
		[Test]
		public void VBNetEnumOnSingleLine()
		{
			string program = "Enum TestEnum : A : B = 1 : C : End Enum";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			
			Assert.AreEqual("TestEnum", td.Name);
			Assert.AreEqual(ClassType.Enum, td.Type);
			Assert.AreEqual(3, td.Children.Count);
		}
		
		[Test]
		public void VBNetEnumOnSingleLine2()
		{
			string program = "Enum TestEnum : A : : B = 1 :: C : End Enum";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			
			Assert.AreEqual("TestEnum", td.Name);
			Assert.AreEqual(ClassType.Enum, td.Type);
			Assert.AreEqual(3, td.Children.Count);
		}
		
		
		[Test]
		public void VBNetEnumWithSystemBaseClassDeclarationTest()
		{
			string program = "Enum TestEnum As System.UInt16\n" +
				"End Enum\n";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			
			Assert.AreEqual("TestEnum", td.Name);
			Assert.AreEqual(ClassType.Enum, td.Type);
			Assert.AreEqual("System.UInt16", td.BaseTypes[0].Type);
			Assert.AreEqual(0, td.Children.Count);
		}
		
		[Test]
		public void VBNetSimpleClassTypeDeclarationWithoutLastNewLineTest()
		{
			string program = "Class TestClass\n" +
				"End Class";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			
			Assert.AreEqual("TestClass", td.Name);
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual(1, td.StartLocation.Line, "start line");
			Assert.AreEqual(2, td.EndLocation.Line, "end line");
		}
		
		[Test]
		public void VBNetSimpleClassTypeDeclarationWithColon()
		{
			string program = "Class TestClass\n" +
				" : \n" +
				"End Class";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			
			Assert.AreEqual("TestClass", td.Name);
			Assert.AreEqual(ClassType.Class, td.Type);
		}
		
		[Test]
		public void VBNetSimplePartialClassTypeDeclarationTest()
		{
			string program = "Partial Class TestClass\n" +
				"End Class\n";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			
			Assert.AreEqual("TestClass", td.Name);
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual(Modifiers.Partial, td.Modifier);
		}
		
		[Test]
		public void VBNetPartialPublicClass()
		{
			string program = "Partial Public Class TestClass\nEnd Class\n";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(program);
			
			Assert.AreEqual("TestClass", td.Name);
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual(Modifiers.Partial | Modifiers.Public, td.Modifier);
		}
		
		[Test]
		public void VBNetGenericClassTypeDeclarationTest()
		{
			string declr = @"
Public Class Test(Of T)

End Class
";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(declr);
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("Test", td.Name);
			Assert.AreEqual(Modifiers.Public, td.Modifier);
			Assert.AreEqual(0, td.BaseTypes.Count);
			Assert.AreEqual(1, td.Templates.Count);
			Assert.AreEqual("T", td.Templates[0].Name);
		}
		
		[Test]
		public void VBNetGenericClassWithConstraint()
		{
			string declr = @"
Public Class Test(Of T As IMyInterface)

End Class
";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(declr);
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("Test", td.Name);
			
			Assert.AreEqual(1, td.Templates.Count);
			Assert.AreEqual("T", td.Templates[0].Name);
			Assert.AreEqual("IMyInterface", td.Templates[0].Bases[0].Type);
		}
		
		[Test]
		public void VBNetComplexGenericClassTypeDeclarationTest()
		{
			string declr = @"
Public Class Generic(Of T As MyNamespace.IMyInterface, S As {G(Of T()), IAnotherInterface})
	Implements System.IComparable

End Class
";
			TypeDeclaration td = ParseUtilVBNet.ParseGlobal<TypeDeclaration>(declr);
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("Generic", td.Name);
			Assert.AreEqual(Modifiers.Public, td.Modifier);
			Assert.AreEqual(1, td.BaseTypes.Count);
			Assert.AreEqual("System.IComparable", td.BaseTypes[0].Type);
			
			Assert.AreEqual(2, td.Templates.Count);
			Assert.AreEqual("T", td.Templates[0].Name);
			Assert.AreEqual("MyNamespace.IMyInterface", td.Templates[0].Bases[0].Type);
			
			Assert.AreEqual("S", td.Templates[1].Name);
			Assert.AreEqual(2, td.Templates[1].Bases.Count);
			Assert.AreEqual("G", td.Templates[1].Bases[0].Type);
			Assert.AreEqual(1, td.Templates[1].Bases[0].GenericTypes.Count);
			Assert.IsTrue(td.Templates[1].Bases[0].GenericTypes[0].IsArrayType);
			Assert.AreEqual("T", td.Templates[1].Bases[0].GenericTypes[0].Type);
			Assert.AreEqual(new int[] {0}, td.Templates[1].Bases[0].GenericTypes[0].RankSpecifier);
			Assert.AreEqual("IAnotherInterface", td.Templates[1].Bases[1].Type);
		}
		#endregion
	}
}
