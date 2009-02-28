// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1139 $</version>
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
[assembly: AssemblyCopyright("(C) Dr. Dirk Lellinger 2002-2007")]
[assembly: AssemblyTrademark("(C) Dr. Dirk Lellinger 2002-2007")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("0.54.0.603")]

[assembly: AssemblyDelaySign(false)]

#if ModifiedForAltaxo
internal static class RevisionClass
{
	public const string Major = "3";
	public const string Minor = "0";
	public const string Build = "0";
	public const string Revision = "3800";

	public const string MainVersion = Major + "." + Minor;
	public const string FullVersion = Major + "." + Minor + "." + Build + "." + Revision;
}
#endif
