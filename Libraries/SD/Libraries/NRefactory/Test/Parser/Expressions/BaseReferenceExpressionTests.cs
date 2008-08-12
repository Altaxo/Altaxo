﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1609 $</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class BaseReferenceExpressionTests
	{
		#region C#
		[Test]
		public void CSharpBaseReferenceExpressionTest1()
		{
			FieldReferenceExpression fre = ParseUtilCSharp.ParseExpression<FieldReferenceExpression>("base.myField");
			Assert.IsTrue(fre.TargetObject is BaseReferenceExpression);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetBaseReferenceExpressionTest1()
		{
			FieldReferenceExpression fre = ParseUtilVBNet.ParseExpression<FieldReferenceExpression>("MyBase.myField");
			Assert.IsTrue(fre.TargetObject is BaseReferenceExpression);
		}
		#endregion
	}
}
