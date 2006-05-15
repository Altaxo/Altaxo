﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1377 $</version>
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
[assembly: AssemblyCompany("ic#code")]
[assembly: AssemblyProduct("SharpDevelop")]
[assembly: AssemblyCopyright("2000-2006 AlphaSierraPapa")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("2.0.0.1410")]
