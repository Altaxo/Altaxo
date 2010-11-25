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
[assembly: AssemblyCopyright("(C) Dr. Dirk Lellinger 2002-2010")]
[assembly: AssemblyTrademark("(C) Dr. Dirk Lellinger 2002-2010")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("4.0.0.687")]

[assembly: AssemblyDelaySign(false)]

#if ModifiedForAltaxo
internal static class RevisionClass
{
  public const string Major = "4";
  public const string Minor = "0";
  public const string Build = "0";
  public const string Revision = "6361";

  public const string MainVersion = Major + "." + Minor;
  public const string FullVersion = Major + "." + Minor + "." + Build + "." + Revision;

  public const string BranchName = null;
}
#endif
