#if TEST

using System;
using System.IO;

using NUnit.Framework;

namespace ICSharpCode.SharpZipLib.Tar {
	
	/// <summary>
	/// This class contains test cases for Tar archive handling
	/// TODO  A whole lot more tests
	/// </summary>
	[TestFixture]
	public class TarTestSuite
	{
		int entryCount;
		
		void EntryCounter(TarArchive archive, TarEntry entry, string message)
		{
			entryCount++;
		}
		
		/// <summary>
		/// Test that an empty archive can be created and when read has 0 entries in it
		/// </summary>
		[Test]
		public void EmptyTar()
		{
			MemoryStream ms = new MemoryStream();
			TarArchive tarOut = TarArchive.CreateOutputTarArchive(ms);
			tarOut.CloseArchive();
			
			Assertion.Assert("Archive size must be > zero", ms.GetBuffer().Length > 0);
			Assertion.AssertEquals("Archive size must be a multiple of record size", ms.GetBuffer().Length % tarOut.RecordSize, 0);
			
			MemoryStream ms2 = new MemoryStream();
			ms2.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
			ms2.Seek(0, SeekOrigin.Begin);
			
			TarArchive tarIn = TarArchive.CreateInputTarArchive(ms2);
			entryCount = 0;
			tarIn.ProgressMessageEvent += new ProgressMessageHandler(EntryCounter);
			tarIn.ListContents();
			Assertion.AssertEquals("Expected 0 tar entries", 0, entryCount);
		}
	}
}

#endif
