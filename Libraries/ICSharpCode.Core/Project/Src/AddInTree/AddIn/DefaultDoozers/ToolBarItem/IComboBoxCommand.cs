﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public interface IComboBoxCommand : ICommand
	{
		bool IsEnabled {
			get;
			set;
		}
	}
}
