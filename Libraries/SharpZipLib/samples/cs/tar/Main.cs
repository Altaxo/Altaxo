using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

/// <summary>
/// The tar class implements a weak reproduction of the
/// traditional UNIX tar command. It currently supports
/// creating, listing, and extracting from archives. It
/// also supports GZIP-ed archives with the '-z' flag.
/// See the usage (-? or --usage) for option details.
/// </summary>
public class Tar
{
	/// <summary>
	/// Flag that determines if debugging information is displayed.
	/// </summary>
	bool debug;
	
	/// <summary>
	/// Flag that determines if verbose feedback is provided.
	/// </summary>
	bool verbose;
	
	/// <summary>
	/// Flag that determines if IO is GZIP-ed ('-z' option).
	/// </summary>
	bool compressed;
	
	/// <summary>
	/// True if we are listing the archive. False if writing or extracting.
	/// </summary>
	bool listingArchive;
	
	/// <summary>
	/// True if we are writing the archive. False if we are extracting it.
	/// </summary>
	bool writingArchive;
	
	/// <summary>
	/// True if we are not to overwrite existing files.
	/// </summary>
	bool keepOldFiles;
	
	/// <summary>
	/// True if we are to convert ASCII text files from local line endings
	/// to the UNIX standard '\n'.
	/// </summary>
	bool asciiTranslate;
	
	/*
	/// <summary>
	/// True if a MIME file has been loaded with the '--mime' option.
	/// </summary>
	bool mimeFileLoaded;*/
	
	/// <summary>
	/// The archive name provided on the command line, null if stdio.
	/// </summary>
	string archiveName;
	
	/// <summary>
	/// The blocksize to use for the tar archive IO. Set by the '-b' option.
	/// </summary>
	int	blockSize;
	
	/// <summary>
	/// The userId to use for files written to archives. Set by '-U' option.
	/// </summary>
	int userId;
	
	/// <summary>
	/// The userName to use for files written to archives. Set by '-u' option.
	/// </summary>
	string userName;
	
	/// <summary>
	/// The groupId to use for files written to archives. Set by '-G' option.
	/// </summary>
	int groupId;
	
	/// <summary>
	/// The groupName to use for files written to archives. Set by '-g' option.
	/// </summary>
	string groupName;
	
	/// <summary>
	/// The main entry point of the tar class.
	/// </summary>
	public static void Main(string[] argv)
	{
		Tar app = new Tar();
		app.InstanceMain(argv);
	}
	
	/// <summary>
	/// Establishes the default userName with the 'user.name' property.
	/// </summary>
	public Tar()
	{
		this.debug          = false;
		this.verbose        = false;
		this.compressed     = false;
		this.archiveName    = null;
		this.listingArchive = false;
		this.writingArchive = true;
		this.keepOldFiles   = false;
		this.asciiTranslate = false;
		
		this.blockSize = TarBuffer.DEFAULT_BLKSIZE;
		
		string sysUserName = Environment.UserName;
		
		this.userId   = 0;
		this.userName = ((sysUserName == null) ? "" : sysUserName);
		
		this.groupId   = 0;
		this.groupName = "";
	}
	
	/// <summary>
	/// This is the "real" main. The class main() instantiates a tar object
	/// for the application and then calls this method. Process the arguments
	/// and perform the requested operation.
	/// </summary>
	public void InstanceMain(string[] argv)
	{
		TarArchive archive = null;
		
		int argIdx = this.ProcessArguments(argv);
		if (writingArchive) {				// WRITING
			Stream outStream = Console.OpenStandardOutput();
			
			if (this.archiveName != null && ! this.archiveName.Equals("-")) {
				outStream = File.Create(archiveName);
			}
			
			if (outStream != null) {
				if (this.compressed) {
					outStream = new GZipOutputStream(outStream);
				}
				archive = TarArchive.CreateOutputTarArchive(outStream, this.blockSize);
			}
		} else {								// EXTRACING OR LISTING
			Stream inStream = Console.OpenStandardInput();
			
			if (this.archiveName != null && ! this.archiveName.Equals( "-" )) {
				inStream = File.OpenRead(archiveName);
			}
			
			if (inStream != null) {
				if (this.compressed) {
					inStream = new GZipInputStream(inStream);
				}
				archive = TarArchive.CreateInputTarArchive(inStream, this.blockSize);
			}
		}
		
		if (archive != null) {						// SET ARCHIVE OPTIONS
			archive.SetDebug(this.debug);
			archive.IsVerbose = this.verbose;
			archive.ProgressMessageEvent += new ProgressMessageHandler(ShowTarProgressMessage);
			
			archive.SetKeepOldFiles(this.keepOldFiles);
			archive.SetAsciiTranslation(this.asciiTranslate);
			
			archive.SetUserInfo(this.userId, this.userName, this.groupId, this.groupName);
		}
		
		if (archive == null) {
			Console.Error.WriteLine( "no processing due to errors" );
		} else if (this.writingArchive) {				// WRITING
			for ( ; argIdx < argv.Length ; ++argIdx ) {
				TarEntry entry = TarEntry.CreateEntryFromFile(argv[argIdx]);
				archive.WriteEntry(entry, true);
			}
		} else if (this.listingArchive) {				// LISTING
			archive.ListContents();
		} else {										// EXTRACTING
			string userDir = Environment.CurrentDirectory;
			
			//	I don't think that this is neccessary :
			//			File destDir = new File( userDir );
			//			if ( ! destDir.exists() )
			//				{
			//				if ( ! destDir.mkdirs() )
			//					{
			//					destDir = null;
			//					Throwable ex = new Throwable
			//						( "ERROR, mkdirs() on '" + destDir.getPath()
	         //							+ "' returned false." );
			//					ex.printStackTrace( System.err );
			//					}
			//				}
			
			if (userDir != null) {
				archive.ExtractContents(userDir);
			}
		}
		if (archive != null) {						// CLOSE ARCHIVE
			archive.CloseArchive();
		}
	}
	
	/**
	* Process arguments, handling options, and return the index of the
	* first non-option argument.
	*
	* @return The index of the first non-option argument.
	*/
	int ProcessArguments(string[] args)
	{
		int idx = 0;
		bool gotOP = false;
		
		for ( ; idx < args.Length ; ++idx ) {
			string arg = args[ idx ];
			
			if (!arg.StartsWith("-")) {
				break;
			}
			
			if (arg.StartsWith("--" )) {
				if (arg.Equals( "--usage")) {
					this.Usage();
					Environment.Exit(1);
				} else if (arg.Equals( "--version")) {
					this.Version();
					Environment.Exit(1);
				}
				else {
					Console.Error.WriteLine("unknown option: " + arg);
					this.Usage();
					Environment.Exit(1);
				}
			} else {
				for (int cIdx = 1; cIdx < arg.Length; ++cIdx) {
					switch (arg[cIdx]) {
						case '?':
							this.Usage();
							Environment.Exit(1);
							break;
						case 'f':
							this.archiveName = args[++idx];
							break;
						case 'z':
							this.compressed = true;
							break;
						case 'c':
							gotOP = true;
							this.writingArchive = true;
							this.listingArchive = false;
							break;
						case 'x':
							gotOP = true;
							this.writingArchive = false;
							this.listingArchive = false;
							break;
						case 't':
							gotOP = true;
							this.writingArchive = false;
							this.listingArchive = true;
							break;
						case 'k':
							this.keepOldFiles = true;
							break;
						case 'b':
							int blks = Int32.Parse(args[++idx]);
							this.blockSize = blks * TarBuffer.DEFAULT_RCDSIZE;
							break;
						case 'u':
							this.userName = args[++idx];
							break;
						case 'U':
							this.userId = Int32.Parse(args[ ++idx ]);
							break;
						case 'g':
							this.groupName = args[++idx];
							break;
						case 'G':
							this.groupId = Int32.Parse(args[ ++idx ]);
							break;
						case 'v':
							this.verbose = true;
							break;
						case 'D':
							this.debug = true;
							break;
						default:
							Console.Error.WriteLine("unknown option: " + arg[cIdx]);
							this.Usage();
							Environment.Exit(1);
							break;
					}
				}
			}
				
			if (!gotOP) {
				Console.Error.WriteLine("you must specify an operation option (c, x, or t)");
				this.Usage();
				Environment.Exit(1);
			}
		}
		return idx;
	}
		
	
	/// <summary>
	/// Display progress information by printing it to Console.Out
	/// </summary>
	public void ShowTarProgressMessage(TarArchive archive, string message)
	{
		Console.WriteLine(message);
	}
	
	/// <summary>
	/// Print version information.
	/// </summary>
	void Version()
	{
		Console.Error.WriteLine( "$Revision: 1.8 $ $Name:  $" );
	}
	
	/// <summary>
	/// Print usage information.
	/// </summary>
	private void Usage()
	{
		Console.Error.WriteLine( "usage: tar has three basic modes:" );
		Console.Error.WriteLine( "  tar -c [options] archive files..." );
		Console.Error.WriteLine( "    Create new archive containing files." );
		Console.Error.WriteLine( "  tar -t [options] archive" );
		Console.Error.WriteLine( "    List contents of tar archive" );
		Console.Error.WriteLine( "  tar -x [options] archive" );
		Console.Error.WriteLine( "    Extract contents of tar archive." );
		Console.Error.WriteLine( "" );
		Console.Error.WriteLine( "options:" );
		Console.Error.WriteLine( "   -f file, use 'file' as the tar archive" );
		Console.Error.WriteLine( "   -v, verbose mode" );
		Console.Error.WriteLine( "   -z, use GZIP compression" );
		Console.Error.WriteLine( "   -D, debug archive and buffer operation" );
		Console.Error.WriteLine( "   -b blks, set blocking size to (blks * 512) bytes" );
		Console.Error.WriteLine( "   -u name, set user name to 'name'" );
		Console.Error.WriteLine( "   -U id, set user id to 'id'" );
		Console.Error.WriteLine( "   -g name, set group name to 'name'" );
		Console.Error.WriteLine( "   -G id, set group id to 'id'" );
		Console.Error.WriteLine( "   -?, print usage information" );
		Console.Error.WriteLine( "   --usage, print usage information" );
		Console.Error.WriteLine( "   --version, print version information" );
		Console.Error.WriteLine( "" );
		Console.Error.WriteLine( "The translation options will translate from local line" );
		Console.Error.WriteLine( "endings to UNIX line endings of '\\n' when writing tar" );
		Console.Error.WriteLine( "archives, and from UNIX line endings into loca line endings" );
		Console.Error.WriteLine( "when extracting archives." );
		Console.Error.WriteLine( "" );
		Console.Error.WriteLine( "Copyright (c) 2002 by Mike Krueger, translated from a java version" );
		Console.Error.WriteLine( "                   of tar which was :" );
		Console.Error.WriteLine( "                   Copyright (c) 1998,1999 by Tim Endres" );
		Console.Error.WriteLine( "" );
		Console.Error.WriteLine( "This program is free software licensed to you under the" );
		Console.Error.WriteLine( "GNU General Public License. See the accompanying LICENSE" );
		Console.Error.WriteLine( "file, or the webpage <http://www.gjt.org/doc/gpl> or, visit" );
		Console.Error.WriteLine( "visit www.fsf.org for more details." );
		Console.Error.WriteLine( "" );
		
		this.Version();
		
		Environment.Exit( 1 );
	}
}
			
/*
** Authored by Timothy Gerard Endres
** <mailto:time@gjt.org>  <http://www.trustice.com>
**
** This work has been placed into the public domain.
** You may use this work in any way and for any purpose you wish.
**
** THIS SOFTWARE IS PROVIDED AS-IS WITHOUT WARRANTY OF ANY KIND,
** NOT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY. THE AUTHOR
** OF THIS SOFTWARE, ASSUMES _NO_ RESPONSIBILITY FOR ANY
** CONSEQUENCE RESULTING FROM THE USE, MODIFICATION, OR
** REDISTRIBUTION OF THIS SOFTWARE.
**
*/
