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
	public class UncheckedExpressionTests
	{
		#region C#
		[Test]
		public void CSharpUncheckedExpressionTest()
		{
			UncheckedExpression ce = ParseUtilCSharp.ParseExpression<UncheckedExpression>("unchecked(a)");
			Assert.IsTrue(ce.Expression is IdentifierExpression);
		}
		#endregion
		
		#region VB.NET
			// No VB.NET representation
		#endregion
		
	}
}
