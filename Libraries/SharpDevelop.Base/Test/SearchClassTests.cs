// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1872 $</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class SearchClassTests
	{
		ProjectContentRegistry projectContentRegistry = ParserService.DefaultProjectContentRegistry;
		
		#region Helper methods
		ICompilationUnit Prepare(LanguageProperties language)
		{
			DefaultProjectContent pc = new DefaultProjectContent();
			pc.ReferencedContents.Add(projectContentRegistry.Mscorlib);
			pc.Language = language;
			DefaultCompilationUnit cu = new DefaultCompilationUnit(pc);
			if (language == LanguageProperties.VBNet)
				cu.Usings.Add(CreateUsing(pc, "syStEm"));
			else
				cu.Usings.Add(CreateUsing(pc, "System"));
			return cu;
		}
		
		IUsing CreateUsing(IProjectContent pc, string @namespace)
		{
			DefaultUsing @using = new DefaultUsing(pc);
			@using.Usings.Add(@namespace);
			return @using;
		}
		
		IReturnType SearchType(string type)
		{
			ICompilationUnit cu = Prepare(LanguageProperties.CSharp);
			IReturnType c = cu.ProjectContent.SearchType(new SearchTypeRequest(type, 0, null, cu, 1, 1)).Result;
			Assert.IsNotNull(c, type + "not found");
			return c;
		}
		
		IReturnType SearchTypeVB(string type)
		{
			ICompilationUnit cu = Prepare(LanguageProperties.VBNet);
			IReturnType c = cu.ProjectContent.SearchType(new SearchTypeRequest(type, 0, null, cu, 1, 1)).Result;
			Assert.IsNotNull(c, type + "not found");
			return c;
		}

		void CheckNamespace(string @namespace, string className)
		{
			CheckNamespace(@namespace, className, LanguageProperties.CSharp);
		}

		void CheckNamespaceVB(string @namespace, string className)
		{
			CheckNamespace(@namespace, className, LanguageProperties.VBNet);
		}

		void CheckNamespace(string @namespace, string className, LanguageProperties language)
		{
			ICompilationUnit cu = Prepare(language);
			string ns = cu.ProjectContent.SearchNamespace(@namespace, null, cu, 1, 1);
			Assert.IsNotNull(ns, @namespace + " not found");
			foreach (object o in cu.ProjectContent.GetNamespaceContents(ns)) {
				IClass c = o as IClass;
				if (c != null && c.Name == className)
					return;
			}
		}
		#endregion
		
		[Test]
		public void SearchFullyQualifiedClass()
		{
			Assert.AreEqual("System.Reflection.Assembly", SearchType("System.Reflection.Assembly").FullyQualifiedName);
		}
		
		[Test]
		public void SearchFullyQualifiedClassVB()
		{
			Assert.AreEqual("System.Reflection.Assembly", SearchTypeVB("SYStem.RefleCtion.asSembly").FullyQualifiedName);
		}
		
		[Test]
		public void SearchFullyQualifiedNamespace()
		{
			CheckNamespace("System.Collections.Generic", "KeyNotFoundException");
		}
		
		[Test]
		public void SearchFullyQualifiedNamespaceVB()
		{
			CheckNamespaceVB("SyStem.COllEctions.GeNEric", "KeyNotFoundException");
		}
		
		[Test]
		public void SearchEnvironment()
		{
			Assert.AreEqual("System.Environment", SearchType("Environment").FullyQualifiedName);
		}
		
		[Test]
		public void SearchEnvironmentVB()
		{
			Assert.AreEqual("System.Environment", SearchTypeVB("EnVIroNmEnt").FullyQualifiedName);
		}

		[Test]
		public void SearchArrayList()
		{
			ICompilationUnit cu = Prepare(LanguageProperties.CSharp);
			IReturnType c = cu.ProjectContent.SearchType(new SearchTypeRequest("Collections.ArrayList", 0, null, cu, 1, 1)).Result;
			Assert.IsNull(c, "Namespaces should not be imported in C#");
		}

		[Test]
		public void SearchArrayListVB()
		{
			Assert.AreEqual("System.Collections.ArrayList", SearchTypeVB("CoLLections.ArrAyLiSt").FullyQualifiedName);
		}

		[Test]
		public void SearchNestedNamespace()
		{
			ICompilationUnit cu = Prepare(LanguageProperties.CSharp);
			string ns = cu.ProjectContent.SearchNamespace("Collections.Generic", null, cu, 1, 1);
			Assert.IsNull(ns, "Nested namespaces should not be found in C#");
		}

		[Test]
		public void SearchNestedNamespaceVB()
		{
			CheckNamespaceVB("COllEctions.GeNEric", "KeyNotFoundException");
		}
	}
}
