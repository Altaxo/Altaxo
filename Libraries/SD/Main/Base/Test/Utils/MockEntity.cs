﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 5242 $</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	public class MockEntity : AbstractEntity
	{
		public MockEntity() : base(null)
		{
		}
		
		public override string DocumentationTag {
			get {
				return String.Empty;
			}
		}
		
		public override ICompilationUnit CompilationUnit {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override EntityType EntityType {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
