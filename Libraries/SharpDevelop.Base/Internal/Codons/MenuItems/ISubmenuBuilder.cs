// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using Reflector.UserInterface;

using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.Core.Properties;


using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.Core.AddIns.Codons
{
	public interface ISubmenuBuilder
	{
		CommandBarItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner);
	}
}
