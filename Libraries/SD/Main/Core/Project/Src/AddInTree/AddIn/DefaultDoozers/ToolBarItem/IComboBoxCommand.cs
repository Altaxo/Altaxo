﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3805 $</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	public interface IComboBoxCommand : ICommand
	{
		bool IsEnabled {
			get;
			set;
		}
		
		object ComboBox {
			get;
			set;
		}
	}
}
