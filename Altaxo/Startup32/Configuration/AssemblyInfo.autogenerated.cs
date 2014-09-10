using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: StringFreezing()]

[assembly: AssemblyTitle("AltaxoStartup32")]
[assembly: AssemblyDescription("Startup executable for Altaxo (in 32 bit mode)")]
[assembly: AssemblyConfiguration("REVID: 2cadfa60d00293aa29dc4855bf2dbbd5fd18f63f, BRANCH: master, DATE: 2014-08-14T15:14:02Z")]
[assembly: AssemblyCompany("http://altaxo.sourceforge.net")]
[assembly: AssemblyProduct("Altaxo")]
[assembly: AssemblyCopyright("(C) Dr. Dirk Lellinger 2002-2014")]
[assembly: AssemblyTrademark("(C) Dr. Dirk Lellinger 2002-2014")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("4.0.930.1")]
[assembly: AssemblyFileVersion("4.0.930.1")]
[assembly: AssemblyInformationalVersion("4.0.930.1 2cadfa6 2014-08-14T15:14:02Z")]

[assembly: AssemblyDelaySign(false)]

internal static class RevisionClass
{
	public const string Major = "4";
	public const string Minor = "0";
	public const string Build = "930";
	public const string Revision = "1";

	public const string MainVersion = Major + "." + Minor;
	public const string FullVersion = Major + "." + Minor + "." + Build + "." + Revision;

	public const string BranchName = "master";
	public const string RevisionID = "2cadfa60d00293aa29dc4855bf2dbbd5fd18f63f";
}
