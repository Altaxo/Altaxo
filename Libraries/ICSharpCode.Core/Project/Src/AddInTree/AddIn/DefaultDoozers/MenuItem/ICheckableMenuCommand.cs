﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	public interface ICheckableMenuCommand : IMenuCommand
	{
		bool IsChecked {
			get;
			set;
		}
	}
}
