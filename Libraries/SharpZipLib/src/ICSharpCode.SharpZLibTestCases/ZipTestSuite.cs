using System;
using System.IO;
using ICSharpCode.SharpUnit;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.GZip;

/// <summary>
/// This class contains test cases for the Adler32 and Crc32 checksums
/// </summary>
[TestSuiteAttribute("Test Zip Algorithm")]
public class ZipTestSuite
{
	/// <summary>
	/// Tests Crc32.
	/// </summary>
	[TestMethodAttribute()]
	public void TestInflateDeflate()
	{
		MemoryStream ms = new MemoryStream();
		Deflater deflater = new Deflater(3);
		DeflaterOutputStream outStream = new DeflaterOutputStream(ms, deflater);
		
		byte[] buf = new byte[10000000];
		System.Random rnd = new Random();
		rnd.NextBytes(buf);
		
		outStream.Write(buf, 0, buf.Length);
		outStream.Flush();
		outStream.Finish();
		
		ms.Seek(0, SeekOrigin.Begin);
		
		InflaterInputStream inStream = new InflaterInputStream(ms);
		byte[] buf2 = new byte[buf.Length];
		int    pos  = 0;
		while (true) {
			int numRead = inStream.Read(buf2, pos, 4096);
			if (numRead <= 0) {
				break;
			}
			pos += numRead;
		}
		
		for (int i = 0; i < buf.Length; ++i) {
			Assertion.AssertEquals(buf2[i], buf[i]);
		}
	}
	
	/// <summary>
	/// Tests Crc32.
	/// </summary>
	[TestMethodAttribute()]
	public void TestGZip()
	{
		MemoryStream ms = new MemoryStream();
		GZipOutputStream outStream = new GZipOutputStream(ms);
		
		byte[] buf = new byte[1000000];
		System.Random rnd = new Random();
		rnd.NextBytes(buf);
		
		outStream.Write(buf, 0, buf.Length);
		outStream.Flush();
		outStream.Finish();
		
		ms.Seek(0, SeekOrigin.Begin);
		
		GZipInputStream inStream = new GZipInputStream(ms);
		byte[] buf2 = new byte[buf.Length];
		int    pos  = 0;
		while (true) {
			int numRead = inStream.Read(buf2, pos, 4096);
			if (numRead <= 0) {
				break;
			}
			pos += numRead;
		}
		
		for (int i = 0; i < buf.Length; ++i) {
			Assertion.AssertEquals(buf2[i], buf[i]);
		}
	}

	/// <summary>
	/// Tests Crc32.
	/// </summary>
	[TestMethodAttribute()]
	public void TestZipUnzip()
	{
		MemoryStream ms = new MemoryStream();
		ZipOutputStream outStream = new ZipOutputStream(ms);
		outStream.SetLevel(1); // 0 - store only to 9 - means best compression
		
		byte[] buf = new byte[1000000];
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
		
		ZipEntry entry2 = inStream.GetNextEntry();
		
		while (true) {
			int numRead = inStream.Read(buf2, pos, 4096);
			if (numRead <= 0) {
				break;
			}
			pos += numRead;
		}
		
		for (int i = 0; i < buf.Length; ++i) {
			Assertion.AssertEquals(buf2[i], buf[i]);
		}
	}
}
