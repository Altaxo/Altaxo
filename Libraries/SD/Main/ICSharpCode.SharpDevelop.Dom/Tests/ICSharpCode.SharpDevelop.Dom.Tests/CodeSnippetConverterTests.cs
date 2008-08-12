// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3090 $</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{
	[TestFixture]
	public class CodeSnippetConverterTests
	{
		List<IProjectContent> referencedContents;
		CodeSnippetConverter converter;
		string errors;
		
		public CodeSnippetConverterTests()
		{
			ProjectContentRegistry pcr = new ProjectContentRegistry();
			referencedContents = new List<IProjectContent> {
				pcr.Mscorlib,
				pcr.GetProjectContentForReference("System", "System")
			};
		}
		
		[SetUp]
		public void SetUp()
		{
			converter = new CodeSnippetConverter {
				ReferencedContents = referencedContents
			};
		}
		
		[Test]
		public void FixExpressionCase()
		{
			Assert.AreEqual("AppDomain.CurrentDomain", converter.VBToCSharp("appdomain.currentdomain", out errors));
		}
		
		[Test]
		public void Statements()
		{
			Assert.AreEqual("a = Console.Title;\n" +
			                "b = Console.ReadLine();",
			                Normalize(converter.VBToCSharp("a = Console.Title\n" +
			                                               "b = Console.Readline", out errors)));
		}
		
		[Test]
		public void FixReferenceToOtherMethodInSameClass()
		{
			Assert.AreEqual("public void A()\n" +
			                "{\n" +
			                "  Test();\n" +
			                "}\n" +
			                "public void Test()\n" +
			                "{\n" +
			                "}",
			                Normalize(converter.VBToCSharp("Sub A()\n" +
			                                               " test\n" +
			                                               "End Sub\n" +
			                                               "Sub Test\n" +
			                                               "End Sub",
			                                               out errors)));
		}
		
		string Normalize(string text)
		{
			return text.Replace("\t", "  ").Replace("\r", "").Trim();
		}
	}
}
