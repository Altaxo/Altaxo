#if TEST

using System;
using System.IO;

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
			
			int    pos  = 0;
			ZipEntry entry;
			byte[] decompressedData = new byte[100];
			while ((entry = inStream.GetNextEntry()) != null) {
				while (true) {
					int numRead = inStream.Read(decompressedData, pos, decompressedData.Length);
					if (numRead <= 0) {
						break;
					}
					pos += numRead;
				}
			}
			inStream.Close();
			Assertion.AssertEquals("No data should be read from empty entries", pos, 0);
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
			
			Assertion.Assert("No entries should be found in empty zip", entry == null);
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
		/// TODO test isnt quite right as wrong passwor dwill give an exception anyway?
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
		/// Basic stored file test, with encryption, non seekable output
		/// </summary>
		[Test]
		public void BasicStoredEncryptedNonSeekable()
		{
			ExerciseZip(CompressionMethod.Stored, 0, 50000, "Rosebud", false);
		}

		[Test]
		public void StoredNoSeekableConvertToDeflate()
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
		/// TODO -jr- testing for 4G limits takes an eternity... what to do about that?
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
		
	}
}

#endif
