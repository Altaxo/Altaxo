using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: StringFreezing()]

// DO NOT EDIT AssemblyInfo.cs, it is recreated using AssemblyInfo.template whenever
// StartUp is compiled.

[assembly: AssemblyTitle("AltaxoStartup")]
[assembly: AssemblyDescription("Startup executable for Altaxo")]
[assembly: AssemblyConfiguration("REVID: 926fa6d9b34e, BRANCH: default")]
[assembly: AssemblyCompany("http://altaxo.sourceforge.net")]
[assembly: AssemblyProduct("Altaxo")]
[assembly: AssemblyCopyright("(C) Dr. Dirk Lellinger 2002-2011")]
[assembly: AssemblyTrademark("(C) Dr. Dirk Lellinger 2002-2011")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("4.0.716.1")]

[assembly: AssemblyDelaySign(false)]

internal static class RevisionClass
{
  public const string Major = "4";
  public const string Minor = "0";
  public const string Build = "716";
  public const string Revision = "1";

  public const string MainVersion = Major + "." + Minor;
  public const string FullVersion = Major + "." + Minor + "." + Build + "." + Revision;

  public const string BranchName = "default";
	public const string RevisionID = "926fa6d9b34e";
}
