// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1040 $</version>
// </file>

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: StringFreezing()]

// DO NOT EDIT AssemblyInfo.cs, it is recreated using AssemblyInfo.template whenever
// StartUp is compiled.

[assembly: AssemblyTitle("AltaxoStartup")]
[assembly: AssemblyDescription("Startup executable for Altaxo")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("http://altaxo.sourceforge.net")]
[assembly: AssemblyProduct("Altaxo")]
[assembly: AssemblyCopyright("(C) Dr. Dirk Lellinger 2002-2006")]
[assembly: AssemblyTrademark("(C) Dr. Dirk Lellinger 2002-2006")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion(RevisionClass.Version + "." + RevisionClass.Revision)]

[assembly: AssemblyDelaySign(false)]

class RevisionClass {
	public const string Version = "0.53.0";
	public const string Revision = "462";
}
