using System;
using System.Text.RegularExpressions;
using System.IO;

namespace AssemblySvnVersion
{
	/// <summary>
	/// Contain methods to patch an AssemblyInfo.cs file so that the AssemblyVersion attribute contains the revision number.
	/// </summary>
	class AssemblyInfoPatching
	{

		/// <summary>
		/// Gets the revision number out of a specified subversion working directory. If the files in the working directory are modified,
		/// the function returns the revision number incremented by 1.
		/// </summary>
		/// <param name="dirname">The subversion working directory where the revision number should be retrieved.</param>
		/// <returns>The revision number of this directory. If modified, it returns the revisionnumber+1.</returns>
		public static int GetRevision( string dirname )
		{
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.StartInfo.RedirectStandardOutput = true; 
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.FileName="svnversion.exe";
			proc.StartInfo.Arguments = dirname;
			proc.StartInfo.CreateNoWindow = true;
               
			proc.Start();
               
			string output = proc.StandardOutput.ReadToEnd();
			proc.WaitForExit();
               
			if ( proc.ExitCode != 0 )
				throw new ApplicationException( "svnversion returned an error code" );   
                  
			Regex rex = new Regex( @"(?'first'\d*)?:?(?'second'\d*)?(?'modified'[MS])*" );
			if ( !rex.IsMatch( output ) )
				throw new ApplicationException( "svnversion output not as expected: " + output );
               
			string first = rex.Match( output ).Groups["first"].Value;
			string second = rex.Match( output ).Groups["second"].Value;
			string modified = rex.Match( output ).Groups["modified"].Value;
               
			int revision;
			if ( second != "" )
				revision = int.Parse( second );
			else
				revision = int.Parse( first );

			if(modified=="M")
				revision++;
               
			return revision;
		}
            
		/// <summary>
		/// Searches the specified file for an AssemblyVersion Attribute of the form AssemblyVersion("u.v.w.x"), where u,v,w, and x are numbers (stars are not allowed here)
		/// and substitutes the last number x by the svnversion number.
		/// </summary>
		/// <param name="file">The full filename of the file containing the AssemblyVersion attribute, normally AssemblyInfo.cs.</param>
		/// <param name="svnversion">The number of the snvversion that should be patched into the AssemblyVersion attribute as the last number.</param>
		public static void PatchAssemblyInfo( string file, int svnversion )
		{
			string searchString  = @"AssemblyVersion(Attribute)*\(\s*\""(?'major'\d*)\.(?'minor'\d*)\.(?'patch'\d*)\.(?'revision'\d*)\""\)";
			string replaceString = @"AssemblyVersion(""${major}.${minor}.${patch}." + svnversion.ToString() + @""")";
                              
			string fileContents = null;
			Console.WriteLine( file );
			using(StreamReader r = new StreamReader( file ) )
				fileContents = r.ReadToEnd();                 
                 
                  

			string newContents =
				Regex.Replace( fileContents, 
				searchString,
				replaceString);

			if(newContents != fileContents)
			{
				using( StreamWriter w = new StreamWriter( file ) )
					w.Write( newContents );
			}
		}



	/// <summary>
	/// The main entry point of the application.
	/// </summary>
	/// <param name="args">args[0] is the working directory from which to retrieve the SVN version, args[1] is the full path name
	/// to the file which contains the AssemblyVersion attribute (normally AssemblyInfo.cs).</param>
	/// <returns>Returns 0 on normal operation, 1 on wrong command line arguments, 2 if no version number could be retrieved, and 3 if the patching fails.</returns>
		[STAThread]
		static int Main(string[] args)
		{
			if(args.Length<2)
			{
				Console.WriteLine("This program patches one or more AssemblyInfo.cs file so that the AssemblyVersion");
				Console.WriteLine("attribute reflects the svn version retrieved from a SVN working directory.");
				Console.WriteLine("Note: In order to work, the AssemblyVersion attribute must contain all 4");
				Console.WriteLine("(four) numbers; the last number is substituted by this program with the actual");
				Console.WriteLine("revision number. Attributes of the form AssemblyVersion(\"1.2.*\") are not");
				Console.WriteLine("substituted by this program.");
				Console.WriteLine("If the SVN working directory was modified, the revision number plus one");
				Console.WriteLine("is used to patch the file");
				Console.WriteLine();
				Console.WriteLine("Usage: AssemblySvnVersion.exe <svn_directory> <AssemblyInfo_filename> ...");
				Console.WriteLine();
				return 1;
			}

			int revision;
			try
			{
				revision = GetRevision(args[0]);
			}
			catch(Exception)
			{
				return 2;
			}

      int nErrorSum = 0;
      for(int i=1;i<args.Length;i++)
      {
        try
        {
          PatchAssemblyInfo(args[i],revision);
        }
        catch(Exception exc)
        {
          Console.WriteLine(exc.ToString());
          nErrorSum += 3;
        }
      }
			return nErrorSum;
		}
	}
}
