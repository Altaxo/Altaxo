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
	public class BlockStatementTests
	{
		#region C#
		[Test]
		public void CSharpBlockStatementTest()
		{
			BlockStatement blockStmt = ParseUtilCSharp.ParseStatement<BlockStatement>("{}");
		}
		
		[Test]
		public void CSharpComplexBlockStatementPositionTest()
		{
			string code = @"{
	WebClient wc = new WebClient();
	wc.Test();
	wc.UploadStringCompleted += delegate {
		output.BeginInvoke((MethodInvoker)delegate {
		                   	output.Text += newText;
		                   });
	};
}";
			BlockStatement blockStmt = ParseUtilCSharp.ParseStatement<BlockStatement>(code);
			//Assert.AreEqual(1, blockStmt.StartLocation.Column); // does not work because ParseStatement inserts special code
			Assert.AreEqual(1, blockStmt.StartLocation.Line);
			Assert.AreEqual(2, blockStmt.EndLocation.Column);
			Assert.AreEqual(9, blockStmt.EndLocation.Line);
		}

		#endregion
		
		#region VB.NET
		// TODO
		#endregion
	}
}
