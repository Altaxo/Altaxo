﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;

namespace ICSharpCode.TextEditor.Document
{
	public class HighlightingColorNotFoundException : Exception
	{
		public HighlightingColorNotFoundException(string name) : base(name)
		{
		}
	}
}
