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
	public class ContinueStatementTests
	{
		#region C#
		[Test]
		public void CSharpContinueStatementTest()
		{
			ContinueStatement continueStmt = ParseUtilCSharp.ParseStatement<ContinueStatement>("continue;");
		}
		#endregion
		
		#region VB.NET
			// No VB.NET representation
		#endregion
	}
}
