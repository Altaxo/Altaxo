﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1388 $</version>
// </file>

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: StringFreezing()]

// Use hard-binding for ICSharpCode.Core:
[assembly: Dependency("log4net", LoadHint.Always)]

[assembly: AssemblyTitle("ICSharpCode.Core")]
[assembly: AssemblyDescription("The ICSharpCode Core containing the AddInTree and service Subsystem")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
