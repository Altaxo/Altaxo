﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2340 $</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	public class MockDecoration : AbstractDecoration
	{
		public MockDecoration() : base(null)
		{
		}
		
		public override string DocumentationTag {
			get {
				return String.Empty;
			}
		}
	}
}
