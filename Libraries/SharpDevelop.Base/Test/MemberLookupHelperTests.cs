// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.Collections.Generic;
using NUnit.Framework;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class MemberLookupHelperTests
	{
		IProjectContent msc = ProjectContentRegistry.Mscorlib;
		IProjectContent swf = ProjectContentRegistry.WinForms;
		
		public IReturnType DictionaryRT {
			get {
				return new GetClassReturnType(msc, "System.Collections.Generic.Dictionary", 2);
			}
		}
		
		public IClass EnumerableClass {
			get {
				return msc.GetClass("System.Collections.Generic.IEnumerable", 1);
			}
		}
		
		[Test]
		public void TypeParameterPassedToBaseClassTest()
		{
			IReturnType[] stringInt = { ReflectionReturnType.String, ReflectionReturnType.Int };
			IReturnType rrt = new ConstructedReturnType(DictionaryRT, stringInt);
			IReturnType res = MemberLookupHelper.GetTypeParameterPassedToBaseClass(rrt, EnumerableClass, 0);
			Assert.AreEqual("System.Collections.Generic.KeyValuePair", res.FullyQualifiedName);
			Assert.AreEqual("System.String", res.TypeArguments[0].FullyQualifiedName);
			Assert.AreEqual("System.Int32", res.TypeArguments[1].FullyQualifiedName);
		}
		
		[Test]
		public void TypeParameterPassedToBaseClassSameClass()
		{
			IReturnType[] stringArr = { ReflectionReturnType.String };
			IReturnType rrt = new ConstructedReturnType(EnumerableClass.DefaultReturnType, stringArr);
			IReturnType res = MemberLookupHelper.GetTypeParameterPassedToBaseClass(rrt, EnumerableClass, 0);
			Assert.AreEqual("System.String", res.FullyQualifiedName);
		}
		
		[Test]
		public void GetCommonType()
		{
			IReturnType res = MemberLookupHelper.GetCommonType(swf.GetClass("System.Windows.Forms.ToolStripButton").DefaultReturnType,
			                                                   swf.GetClass("System.Windows.Forms.ToolStripSeparator").DefaultReturnType);
			Assert.AreEqual("System.Windows.Forms.ToolStripItem", res.FullyQualifiedName);
		}
	}
}
