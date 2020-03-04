using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Compression
{
    public class ZipTestBase
    {
        public static string TestEntryName(int i) => "TestEntry" + i.ToString();

        public static void CompareArchives_UsingOriginal(string zipFile1Name, string zipFile2Name, int numberOfEntries)
        {
            using (var zip1Stream = new FileStream(zipFile1Name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var zip1Archive = new ZipArchive(zip1Stream, ZipArchiveMode.Read, false);


                using (var zip2Stream = new FileStream(zipFile2Name, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var zip2Archive = new ZipArchive(zip2Stream, ZipArchiveMode.Read, false);

                    for (int i = 0; i < numberOfEntries; ++i)
                    {
                        var entry1 = zip1Archive.GetEntry(TestEntryName(i));
                        var entry2 = zip2Archive.GetEntry(TestEntryName(i));

                        Assert.AreEqual(entry1.CompressedLength, entry2.CompressedLength);
                        Assert.AreEqual(entry1.Length, entry2.Length);
                        Assert.AreEqual(entry1.ExternalAttributes, entry2.ExternalAttributes);

                        Assert.AreEqual(entry1.LastWriteTime, entry2.LastWriteTime);
                        Assert.AreEqual(entry1.Name, entry2.Name);
                        Assert.AreEqual(entry1.FullName, entry2.FullName);

                        var buffer1 = new byte[4096];
                        var buffer2 = new byte[4096];
                        using (var s1 = entry1.Open())
                        {
                            using (var s2 = entry2.Open())
                            {
                                for (; ; )
                                {
                                    var rd1 = s1.Read(buffer1, 0, buffer1.Length);
                                    var rd2 = s2.Read(buffer2, 0, buffer2.Length);

                                    Assert.AreEqual(rd1, rd2);

                                    for (int k = 0; k < rd1; ++k)
                                        Assert.AreEqual(buffer1[k], buffer2[k]);


                                    if (0 == rd1)
                                        break;
                                }
                            }
                        }



                    }
                    zip2Archive.Dispose();
                }

                zip1Archive.Dispose();
            }
        }

        public static void FillBuffer(byte[] buffer, int seed)
        {
            var rnd = new Random(seed);
            rnd.NextBytes(buffer);
        }


        public static void CompareArchives_UsingAltaxo(string zipFile1Name, string zipFile2Name, int numberOfEntries)
        {
            using (var zip1Stream = new FileStream(zipFile1Name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var zip1Archive = new ZipArchiveAxo(zip1Stream, ZipArchiveMode.Read, false);


                using (var zip2Stream = new FileStream(zipFile2Name, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var zip2Archive = new ZipArchiveAxo(zip2Stream, ZipArchiveMode.Read, false);

                    for (int i = 0; i < numberOfEntries; ++i)
                    {
                        var entry1 = zip1Archive.GetEntry(TestEntryName(i));
                        var entry2 = zip2Archive.GetEntry(TestEntryName(i));

                        Assert.AreEqual(entry1.CompressedLength, entry2.CompressedLength);
                        Assert.AreEqual(entry1.Length, entry2.Length);
                        Assert.AreEqual(entry1.ExternalAttributes, entry2.ExternalAttributes);

                        Assert.AreEqual(entry1.LastWriteTime, entry2.LastWriteTime);
                        Assert.AreEqual(entry1.Name, entry2.Name);
                        Assert.AreEqual(entry1.FullName, entry2.FullName);


                        var buffer1 = new byte[4096];
                        var buffer2 = new byte[4096];
                        using (var s1 = entry1.Open())
                        {
                            using (var s2 = entry2.Open())
                            {
                                for (; ; )
                                {
                                    var rd1 = s1.Read(buffer1, 0, buffer1.Length);
                                    var rd2 = s2.Read(buffer2, 0, buffer2.Length);

                                    Assert.AreEqual(rd1, rd2);

                                    for (int k = 0; k < rd1; ++k)
                                        Assert.AreEqual(buffer1[k], buffer2[k]);


                                    if (0 == rd1)
                                        break;
                                }
                            }
                        }

                        Assert.AreEqual(entry1.Crc32, entry2.Crc32);



                    }
                    zip2Archive.Dispose();
                }

                zip1Archive.Dispose();
            }
        }

        public static void CompareArchives_LengthAndContent(string zipFile1Name, string zipFile2Name)
        {
            var length1 = new FileInfo(zipFile1Name).Length;
            var length2 = new FileInfo(zipFile2Name).Length;
            Assert.AreEqual(length2, length1);

            var content1 = new byte[length1];
            var content2 = new byte[length2];

            using (var zip1Stream = new FileStream(zipFile1Name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                zip1Stream.Read(content1, 0, content1.Length);
            }
            using (var zip2Stream = new FileStream(zipFile2Name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                zip2Stream.Read(content2, 0, content2.Length);
            }
            for (int i = 0; i < length1; ++i)
            {
                Assert.AreEqual(content2[i], content1[i], $"Content differs at {i}");
            }
        }
    }
}
