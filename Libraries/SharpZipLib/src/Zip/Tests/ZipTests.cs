#if TEST

using System;
using System.IO;
using System.Security;

using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.GZip;

using NUnit.Framework;

namespace ICSharpCode.SharpZipLib.Zip
{
	class MemStreamWithoutSeek : MemoryStream
	{
		public override bool CanSeek {
			get {
				return false;
			}
		}
	}
	
	/// <summary>
	/// This class contains test cases for Zip compression and decompression
	/// </summary>
	[TestFixture]
	public class ZipTestSuite
	{
		void AddRandomDataToEntry(ZipOutputStream zipStream, int size)
		{
			if (size > 0) {
				byte [] data = new byte[size];
				System.Random rnd = new Random();
				rnd.NextBytes(data);
			
				zipStream.Write(data, 0, data.Length);
			}
		}

		byte ScatterValue(byte rhs)
		{
			return (byte) (rhs * 253 + 7);
		}
		
		void AddKnownDataToEntry(ZipOutputStream zipStream, int size)
		{
			if (size > 0) {
				byte nextValue = 0;
				byte [] data = new byte[size];
				for (int i = 0; i < size; ++i) {
					data[i] = nextValue;
					nextValue = ScatterValue(nextValue);			
				}
				zipStream.Write(data, 0, data.Length);
			}
		}
		
		byte[] MakeMemZip(ref byte[] original, CompressionMethod method, int compressionLevel, int size, string password, bool withSeek)
		{
			MemoryStream ms;
			
			if (withSeek == true) {
				ms = new MemoryStream();
			} else {
				ms = new MemStreamWithoutSeek();
			}
			
			ZipOutputStream outStream = new ZipOutputStream(ms);
			if (password != null) {
				outStream.Password = password;
			}
			
			if (method != CompressionMethod.Stored)
				outStream.SetLevel(compressionLevel); // 0 - store only to 9 - means best compression
			
			ZipEntry entry = new ZipEntry("dummyfile.tst");
			entry.CompressionMethod = method;
			
			outStream.PutNextEntry(entry);
			
			if (size > 0) {
				original = new byte[size];
				System.Random rnd = new Random();
				rnd.NextBytes(original);
			
				outStream.Write(original, 0, original.Length);
			}
			outStream.Close();
			return ms.GetBuffer();
		}
		
		void ExerciseZip(CompressionMethod method, int compressionLevel, int size, string password, bool canSeek)
		{
			byte[] originalData = null;
			byte[] compressedData = MakeMemZip(ref originalData, method, compressionLevel, size, password, canSeek);
			
			MemoryStream ms = new MemoryStream(compressedData);
			ms.Seek(0, SeekOrigin.Begin);
			
			ZipInputStream inStream = new ZipInputStream(ms);
			byte[] decompressedData = new byte[size];
			int    pos  = 0;
			if (password != null) {
				inStream.Password = password;
			}
			
			ZipEntry entry2 = inStream.GetNextEntry();
			
			if ((entry2.Flags & 8) == 0) {
				// -jr- problem here!!
				Assertion.AssertEquals("Entry size invalid", size, entry2.Size);
			}
			
			if (size > 0) {
				while (true) {
					int numRead = inStream.Read(decompressedData, pos, 4096);
					if (numRead <= 0) {
						break;
					}
					pos += numRead;
				}
			}
		
			Assertion.AssertEquals("Original and decompressed data different sizes", pos, size);
			
			if (originalData != null) {
				for (int i = 0; i < originalData.Length; ++i) {
					Assertion.AssertEquals("Decompressed data doesnt match original, compression level: " + compressionLevel, decompressedData[i], originalData[i]);
				}
			}
		}

		/// <summary>
		/// Empty zip entries can be created and read?
		/// </summary>
		[Test]
		public void EmptyZipEntries()
		{
			MemoryStream ms = new MemoryStream();
			ZipOutputStream outStream = new ZipOutputStream(ms);
			for (int i = 0; i < 10; ++i) {
				outStream.PutNextEntry(new ZipEntry(i.ToString()));
			}
			outStream.Finish();
			
			ms.Seek(0, SeekOrigin.Begin);
			
			ZipInputStream inStream = new ZipInputStream(ms);
			
			int    extractCount  = 0;
			ZipEntry entry;
			byte[] decompressedData = new byte[100];
			while ((entry = inStream.GetNextEntry()) != null) {
				while (true) {
					int numRead = inStream.Read(decompressedData, extractCount, decompressedData.Length);
					if (numRead <= 0) {
						break;
					}
					extractCount += numRead;
				}
			}
			inStream.Close();
			Assertion.AssertEquals("No data should be read from empty entries", extractCount, 0);
		}

		/// <summary>
		/// Empty zips can be created and read?
		/// </summary>
		[Test]
		public void EmptyZip()
		{
			MemoryStream ms = new MemoryStream();
			ZipOutputStream outStream = new ZipOutputStream(ms);
			outStream.Finish();
			
			ms.Seek(0, SeekOrigin.Begin);
			
			ZipInputStream inStream = new ZipInputStream(ms);
			ZipEntry entry;
			while ((entry = inStream.GetNextEntry()) != null) {
				Assertion.Assert("No entries should be found in empty zip", entry == null);
			}
		}

		/// <summary>
		/// Invalid passwords should be detected early if possible, seekable stream
		/// </summary>
		[Test]
		[ExpectedException(typeof(ZipException))]
		public void InvalidPasswordSeekable()
		{
			byte[] originalData = null;
			byte[] compressedData = MakeMemZip(ref originalData, CompressionMethod.Deflated, 3, 500, "Hola", true);
			
			MemoryStream ms = new MemoryStream(compressedData);
			ms.Seek(0, SeekOrigin.Begin);
			
			byte[] buf2 = new byte[originalData.Length];
			int    pos  = 0;
			
			ZipInputStream inStream = new ZipInputStream(ms);
			inStream.Password = "redhead";
			
			ZipEntry entry2 = inStream.GetNextEntry();
			
			while (true) {
				int numRead = inStream.Read(buf2, pos, 4096);
				if (numRead <= 0) {
					break;
				}
				pos += numRead;
			}
		}
		
		/// <summary>
		/// Invalid passwords should be detected early if possible, non seekable stream
		/// </summary>
		[Test]
		[ExpectedException(typeof(ZipException))]
		public void InvalidPasswordNonSeekable()
		{
			byte[] originalData = null;
			byte[] compressedData = MakeMemZip(ref originalData, CompressionMethod.Deflated, 3, 500, "Hola", false);
			
			MemoryStream ms = new MemoryStream(compressedData);
			ms.Seek(0, SeekOrigin.Begin);
			
			byte[] buf2 = new byte[originalData.Length];
			int    pos  = 0;
			
			ZipInputStream inStream = new ZipInputStream(ms);
			inStream.Password = "redhead";
			
			ZipEntry entry2 = inStream.GetNextEntry();
			
			while (true) {
				int numRead = inStream.Read(buf2, pos, 4096);
				if (numRead <= 0) {
					break;
				}
				pos += numRead;
			}
			
		}

		/// <summary>
		/// Setting entry comments to null should be allowed
		/// </summary>
		[Test]
		public void NullEntryComment()
		{
			ZipEntry test = new ZipEntry("null");
			test.Comment = null;
		}
		
		/// <summary>
		/// Entries with null names arent allowed
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullEntryName()
		{
			string name = null;
			ZipEntry test = new ZipEntry(name);
		}
		
		/// <summary>
		/// Adding an entry after the stream has Finished should fail
		/// </summary>
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void AddEntryAfterFinish()
		{
			MemoryStream ms = new MemoryStream();
			ZipOutputStream s = new ZipOutputStream(ms);
			s.Finish();
			s.PutNextEntry(new ZipEntry("dummyfile.tst"));
		}
		
		/// <summary>
		/// Test setting file commment to a value that is too long
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CommentTooLong()
		{
			MemoryStream ms = new MemoryStream();
			ZipOutputStream s = new ZipOutputStream(ms);
			s.SetComment(new String('A', 65536));			
		}
		
		/// <summary>
		/// Check that simply closing ZipOutputStream finishes the zip correctly
		/// </summary>
		[Test]
		public void CloseOnlyHandled()
		{
			MemoryStream ms = new MemoryStream();
			ZipOutputStream s = new ZipOutputStream(ms);
			s.PutNextEntry(new ZipEntry("dummyfile.tst"));
			s.Close();
			
			Assertion.Assert("Output stream should be finished", s.IsFinished == true);
		}

		/// <summary>
		/// Basic compress/decompress test, no encryption, size is important here as its big enough
		/// to force multiple write to output which was a problem...
		/// </summary>
		[Test]
		public void BasicDeflated()
		{
			for (int i = 0; i <= 9; ++i) {
				ExerciseZip(CompressionMethod.Deflated, i, 50000, null, true);
			}
		}
		
		/// <summary>
		/// Basic compress/decompress test, no encryption, size is important here as its big enough
		/// to force multiple write to output which was a problem...
		/// </summary>
		[Test]
		public void BasicDeflatedNonSeekable()
		{
			for (int i = 0; i <= 9; ++i) {
				ExerciseZip(CompressionMethod.Deflated, i, 50000, null, false);
			}
		}

		/// <summary>
		/// Basic stored file test, no encryption.
		/// </summary>
		[Test]
		public void BasicStored()
		{
			ExerciseZip(CompressionMethod.Stored, 0, 50000, null, true);
		}
		
		/// <summary>
		/// Basic stored file test, no encryption, non seekable output
		/// NOTE this gets converted to deflate level 0
		/// </summary>
		[Test]
		public void BasicStoredNonSeekable()
		{
			ExerciseZip(CompressionMethod.Stored, 0, 50000, null, false);
		}
		
		
		/// <summary>
		/// Basic compress/decompress test, with encryption, size is important here as its big enough
		/// to force multiple write to output which was a problem...
		/// </summary>
		[Test]
		public void BasicDeflatedEncrypted()
		{
			for (int i = 0; i <= 9; ++i) {
				ExerciseZip(CompressionMethod.Deflated, i, 50000, "Rosebud", true);
			}
		}
		
		/// <summary>
		/// Basic compress/decompress test, with encryption, size is important here as its big enough
		/// to force multiple write to output which was a problem...
		/// </summary>
		[Test]
		public void BasicDeflatedEncryptedNonSeekable()
		{
			for (int i = 0; i <= 9; ++i) {
				ExerciseZip(CompressionMethod.Deflated, i, 50000, "Rosebud", false);
			}
		}
		
		
		/// <summary>
		/// Basic stored file test, with encryption.
		/// </summary>
		[Test]
		public void BasicStoredEncrypted()
		{
			ExerciseZip(CompressionMethod.Stored, 0, 50000, "Rosebud", true);
		}
		
		/// <summary>
		/// Basic stored file test, with encryption, non seekable output.
		/// NOTE this gets converted deflate level 0
		/// </summary>
		[Test]
		public void BasicStoredEncryptedNonSeekable()
		{
			ExerciseZip(CompressionMethod.Stored, 0, 50000, "Rosebud", false);
		}

		/// <summary>
		/// Check that when the output stream cannot seek that requests for stored
		/// are in fact converted to defalted level 0
		/// </summary>
		[Test]
		public void StoredNonSeekableConvertToDeflate()
		{
			MemStreamWithoutSeek ms = new MemStreamWithoutSeek();
			
			ZipOutputStream outStream = new ZipOutputStream(ms);
			outStream.SetLevel(8);
			Assertion.AssertEquals("Compression level invalid", 8, outStream.GetLevel());
			
			ZipEntry entry = new ZipEntry("1.tst");
			entry.CompressionMethod = CompressionMethod.Stored;
			outStream.PutNextEntry(entry);
			Assertion.AssertEquals("Compression level invalid", 0, outStream.GetLevel());
			
			AddRandomDataToEntry(outStream, 100);
			entry = new ZipEntry("2.tst");
			entry.CompressionMethod = CompressionMethod.Deflated;
			outStream.PutNextEntry(entry);
			Assertion.AssertEquals("Compression level invalid", 8, outStream.GetLevel());
			AddRandomDataToEntry(outStream, 100);
			
			outStream.Close();
		}
		
		/// <summary>
		/// Extra data for separate entries should be unique to that entry
		/// </summary>
		[Test]
		public void ExtraDataUnique()
		{
			ZipEntry a = new ZipEntry("Basil");
			byte[] extra = new byte[4];
			extra[0] = 27;
			a.ExtraData = extra;
			
			ZipEntry b = new ZipEntry(a);
			b.ExtraData[0] = 89;
			Assertion.Assert("Extra data not unique" + b.ExtraData[0] + " " + a.ExtraData[0], b.ExtraData[0] != a.ExtraData[0]);
		}
		
		/// <summary>
		/// Check that adding too many entries is detected and handled
		/// </summary>
		[Test]
		[ExpectedException(typeof(ZipException))]
		public void TooManyEntries()
		{
			const int target = 65537;
			MemoryStream ms = new MemoryStream();
			ZipOutputStream s = new ZipOutputStream(ms);
			for (int i = 0; i < target; ++i) {
				s.PutNextEntry(new ZipEntry("dummyfile.tst"));
			}
			s.Finish();
			ms.Seek(0, SeekOrigin.Begin);
			ZipFile zipFile = new ZipFile(ms);
			Assertion.AssertEquals("Incorrect number of entries stored", target, zipFile.Size);
		}

		void MakeZipFile(string name, string[] names, int size, string comment)
		{
			using (FileStream fs = File.Create(name)) {
				ZipOutputStream zOut = new ZipOutputStream(fs);
				zOut.SetComment(comment);
				for (int i = 0; i < names.Length; ++i) {
					zOut.PutNextEntry(new ZipEntry(names[i]));
					AddKnownDataToEntry(zOut, size);	
				}
				zOut.Close();
				fs.Close();
			}
		}
		
		void MakeZipFile(string name, string entryNamePrefix, int entries, int size, string comment)
		{
			using (FileStream fs = File.Create(name)) {
				ZipOutputStream zOut = new ZipOutputStream(fs);
				zOut.SetComment(comment);
				for (int i = 0; i < entries; ++i) {
					zOut.PutNextEntry(new ZipEntry(entryNamePrefix + (i + 1).ToString()));
					AddKnownDataToEntry(zOut, size);	
				}
				zOut.Close();
				fs.Close();
			}
		}
		
		
		void CheckKnownEntry(Stream inStream, int expectedCount) 
		{
			byte[] buffer = new Byte[1024];
			int bytesRead;
			int total = 0;
			byte nextValue = 0;
			while ((bytesRead = inStream.Read(buffer, 0, buffer.Length)) > 0) {
				total += bytesRead;
				for (int i = 0; i < bytesRead; ++i) {
					Assertion.AssertEquals("Wrong value read from entry", nextValue, buffer[i]);
					nextValue = ScatterValue(nextValue);			
				}
			}
			Assertion.AssertEquals("Wrong number of bytes read from entry", expectedCount, total);
		}
		
		/// <summary>
		/// Simple round trip test for ZipFile class
		/// </summary>
		[Test]
		public void ZipFileRoundTrip()
		{
			string tempFile = null;
			try {
				 tempFile = Path.GetTempPath();
			} catch (SecurityException) {
			}
			
			Assertion.AssertNotNull("No permission to execute this test?", tempFile);
			
			if (tempFile != null) {
				tempFile = Path.Combine(tempFile, "SharpZipTest.Zip");
				MakeZipFile(tempFile, "", 10, 1024, "");
				
				ZipFile zipFile = new ZipFile(tempFile);
				foreach (ZipEntry e in zipFile) {
					Stream instream = zipFile.GetInputStream(e);
					CheckKnownEntry(instream, 1024);
		 		}
				
				zipFile.Close();
				
				File.Delete(tempFile);
			}
		}

		/// <summary>
		/// Check that ZipFile finds entries when its got a long comment
		/// </summary>
		[Test]
		public void ZipFileFindEntriesLongComment()
		{
			string tempFile = null;
			try	{
				 tempFile = Path.GetTempPath();
			} catch (SecurityException) {
			}
			
			Assertion.AssertNotNull("No permission to execute this test?", tempFile);
			
			if (tempFile != null) {
				tempFile = Path.Combine(tempFile, "SharpZipTest.Zip");
				string longComment = new String('A', 65535);
				MakeZipFile(tempFile, "", 1, 1, longComment);
				
				ZipFile zipFile = new ZipFile(tempFile);
				foreach (ZipEntry e in zipFile) {
					Stream instream = zipFile.GetInputStream(e);
					CheckKnownEntry(instream, 1);
		 		}
				
				zipFile.Close();
				
				File.Delete(tempFile);
			}
			
		}
		
		/// <summary>
		/// Check that ZipFile class handles no entries in zip file
		/// </summary>
		[Test]
		public void ZipFileHandlesNoEntries()
		{
			string tempFile = null;
			try {
				 tempFile = Path.GetTempPath();
			} catch (SecurityException) {
			}
			
			Assertion.AssertNotNull("No permission to execute this test?", tempFile);
			
			if (tempFile != null) {
				tempFile = Path.Combine(tempFile, "SharpZipTest.Zip");
				MakeZipFile(tempFile, "", 0, 1, "Aha");
				
				ZipFile zipFile = new ZipFile(tempFile);
				zipFile.Close();
				File.Delete(tempFile);
			}
			
		}
		
		/// <summary>
		/// Test ZipFile find method operation
		/// </summary>
		[Test]
		public void ZipFileFind()
		{
			string tempFile = null;
			try
			{
				 tempFile = Path.GetTempPath();
			}
			catch (SecurityException)
			{
			}
			
			Assertion.AssertNotNull("No permission to execute this test?", tempFile);
			
			if (tempFile != null) {
				tempFile = Path.Combine(tempFile, "SharpZipTest.Zip");
				MakeZipFile(tempFile, new String[] {"Farriera", "Champagne", "Urban myth" }, 10, "Aha");
				
				ZipFile zipFile = new ZipFile(tempFile);
				Assertion.AssertEquals("Expected 1 entry", 3, zipFile.Size);
				
				int testIndex = zipFile.FindEntry("Farriera", false);
				Assertion.AssertEquals("Case sensitive find failure", 0, testIndex);
				Assertion.Assert(string.Compare(zipFile[testIndex].Name, "Farriera", false) == 0);
				
				testIndex = zipFile.FindEntry("Farriera", true);
				Assertion.AssertEquals("Case insensitive find failure", 0, testIndex);
				Assertion.Assert(string.Compare(zipFile[testIndex].Name, "Farriera", true) == 0);
				
				testIndex = zipFile.FindEntry("urban mYTH", false);
				Assertion.AssertEquals("Case sensitive find failure", -1, testIndex);
				
				testIndex = zipFile.FindEntry("urban mYTH", true);
				Assertion.AssertEquals("Case insensitive find failure", 2, testIndex);
				Assertion.Assert(string.Compare(zipFile[testIndex].Name, "urban mYTH", true) == 0);
				
				testIndex = zipFile.FindEntry("Champane.", false);
				Assertion.AssertEquals("Case sensitive find failure", -1, testIndex);
				
				testIndex = zipFile.FindEntry("Champane.", true);
				Assertion.AssertEquals("Case insensitive find failure", -1, testIndex);
				
				zipFile.Close();
				File.Delete(tempFile);
			}
		}
		
		
		/// <summary>
		/// Test ZipEntry static file name cleaning methods
		/// </summary>
		[Test]
		public void FilenameCleaning()
		{
			Assertion.Assert(string.Compare(ZipEntry.CleanName("hello"), "hello") == 0);
			Assertion.Assert(string.Compare(ZipEntry.CleanName(@"z:\eccles"), "eccles") == 0);
			Assertion.Assert(string.Compare(ZipEntry.CleanName(@"\\server\share\eccles"), "eccles") == 0);
			Assertion.Assert(string.Compare(ZipEntry.CleanName(@"\\server\share\dir\eccles"), "dir/eccles") == 0);
			Assertion.Assert(string.Compare(ZipEntry.CleanName(@"\\server\share\eccles", false), "/eccles") == 0);
			Assertion.Assert(string.Compare(ZipEntry.CleanName(@"c:\a\b\c\deus.dat", false), "/a/b/c/deus.dat") == 0);
		}
		
	}
}

#endif
