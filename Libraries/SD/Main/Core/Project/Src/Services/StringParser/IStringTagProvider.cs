// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 4735 $</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	public interface IStringTagProvider
	{
		string ProvideString(string tag, StringTagPair[] customTags);
	}
}
