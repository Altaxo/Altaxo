#if TEST
using System;
using System.IO;

using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.GZip;
using NUnit.Framework;

namespace ICSharpCode.SharpZipLib.Zip
{
	/// <summary>
	/// This class contains test cases for the Adler32 and Crc32 checksums
	/// </summary>
	[TestFixture]
	public class ZipTestSuite
	{
		void TestZip(int compressionLevel, string password)
		{
			MemoryStream ms = new MemoryStream();
			ZipOutputStream outStream = new ZipOutputStream(ms);
			if (password != null) {
				outStream.Password = password;
			}
			outStream.SetLevel(0); // 0 - store only to 9 - means best compression
			
			byte[] buf = new byte[10000];
			System.Random rnd = new Random();
			rnd.NextBytes(buf);
			
			
			ZipEntry entry = new ZipEntry("dummyfile.tst");
			outStream.PutNextEntry(entry);
			outStream.Write(buf, 0, buf.Length);
			outStream.Flush();
			outStream.Finish();
			
			ms.Seek(0, SeekOrigin.Begin);
			
			ZipInputStream inStream = new ZipInputStream(ms);
			byte[] buf2 = new byte[buf.Length];
			int    pos  = 0;
			if (password != null) {
				inStream.Password = password;
			}
			
			ZipEntry entry2 = inStream.GetNextEntry();
			
			while (true) {
				int numRead = inStream.Read(buf2, pos, 4096);
				if (numRead <= 0) {
					break;
				}
				pos += numRead;
			}
			
			for (int i = 0; i < buf.Length; ++i) {
				Assertion.AssertEquals("Failed with compression level " + compressionLevel,buf2[i], buf[i]);
			}
		}
		
		[Test]
		public void TestZipUnzip()
		{
			for (int i = 0; i <= 9; ++i) {
				TestZip(i, null);
			}
		}
		
		[Test]
		public void TestCryptedZipUnzip()
		{
			for (int i = 0; i <= 9; ++i) {
				TestZip(i, "Rosebud");
			}
		}
	}
}
#endif
