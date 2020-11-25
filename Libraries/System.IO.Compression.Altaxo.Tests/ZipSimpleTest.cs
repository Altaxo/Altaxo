using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Compression
{

  public class ZipSimpleTest
  {
    [Fact]
    public void TestSimple()
    {
      int bufferSize = 4096 * 8;
      var wrBuffer = new byte[bufferSize];
      for (int i = 0, j = 0; i < bufferSize; i += 8, j++)
      {
        wrBuffer[i] = (byte)(j % 256);
      }


      var fileName = Path.GetTempFileName();
      using (var zipStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
      {
        var zipArchive = new ZipArchiveAxo(zipStream, ZipArchiveMode.Create, false);
        var entry = zipArchive.CreateEntry("TestEntry");
        var entryStream = entry.Open();
        entryStream.Write(wrBuffer, 0, bufferSize);
        entryStream.Close();
        zipArchive.Dispose();
      }

      var rdBuffer = new byte[bufferSize];

      // Open and verify

      using (var zipStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        var zipArchive = new ZipArchiveAxo(zipStream, ZipArchiveMode.Read, false);
        var entry = zipArchive.GetEntry("TestEntry");
        var entryStream = entry.Open();
        var rd = entryStream.Read(rdBuffer, 0, bufferSize);
        Assert.Equal(bufferSize, rd);

        for (int i = 0; i < bufferSize; ++i)
        {
          Assert.Equal(wrBuffer[i], rdBuffer[i]);
        }

        entryStream.Close();
        zipArchive.Dispose();
      }

      File.Delete(fileName);
    }

    [Fact]
    public void TestParallelSimple()
    {
      int bufferSize = 4096 * 8;
      var wrBuffer = new byte[bufferSize];
      for (int i = 0, j = 0; i < bufferSize; i += 8, j++)
      {
        wrBuffer[i] = (byte)(j % 256);
      }


      var fileName = Path.GetTempFileName();
      using (var zipStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
      {
        var zipArchive = new ZipArchiveAxo(zipStream, ZipArchiveMode.Create, false);
        var entry = zipArchive.CreateEntry("TestEntry");
        var entryStream = entry.Open();
        entryStream.Write(wrBuffer, 0, bufferSize);
        entryStream.Close();
        zipArchive.Dispose();
      }

      var rdBuffer = new byte[bufferSize];

      // Open and verify

      using (var zipStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        var zipArchive = new ZipArchiveAxo(zipStream, ZipArchiveMode.Read, false);
        var entry = zipArchive.GetEntry("TestEntry");
        var entryStream = entry.OpenStreamInReadModeParallel();
        var rd = entryStream.Read(rdBuffer, 0, bufferSize);
        Assert.Equal(bufferSize, rd);

        for (int i = 0; i < bufferSize; ++i)
        {
          Assert.Equal(wrBuffer[i], rdBuffer[i]);
        }

        entryStream.Close();
        zipArchive.Dispose();
      }

      File.Delete(fileName);
    }

    [Fact]
    public void TestMultipleEntriesSerial()
    {
      int bufferSize = 1024 * 1024 * 8;
      var wrBuffer = new byte[bufferSize];
      for (int i = 0, j = 0; i < bufferSize; i += 8, j++)
      {
        wrBuffer[i] = (byte)(j % 256);
      }


      var fileName = Path.GetTempFileName();
      using (var zipStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
      {
        var zipArchive = new ZipArchiveAxo(zipStream, ZipArchiveMode.Create, false);

        for (int i = 0; i < 4; ++i)
        {
          var entry = zipArchive.CreateEntry("TestEntry" + i.ToString());
          var entryStream = entry.Open();
          entryStream.Write(wrBuffer, 0, bufferSize);
          entryStream.Close();
        }
        zipArchive.Dispose();
      }

      var rdBuffer = new byte[bufferSize];

      // Open and verify

      var stopWatch = new System.Diagnostics.Stopwatch();
      stopWatch.Start();

      using (var zipStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        var zipArchive = new ZipArchiveAxo(zipStream, ZipArchiveMode.Read, false);

        for (int iEntry = 0; iEntry < 4; ++iEntry)
        {
          var entry = zipArchive.GetEntry("TestEntry" + iEntry.ToString());
          var entryStream = entry.OpenStreamInReadModeParallel();

          var rd = entryStream.Read(rdBuffer, 0, bufferSize);
          Assert.Equal(bufferSize, rd);

          for (int i = 0; i < bufferSize; ++i)
          {
            Assert.Equal(wrBuffer[i], rdBuffer[i]);
          }

          entryStream.Close();

        }
        zipArchive.Dispose();
      }

      var elapsed = stopWatch.Elapsed;
      System.Diagnostics.Debug.WriteLine(string.Format("TestMultipleEntriesSerial, Elapsed time: {0}", elapsed));

      File.Delete(fileName);
    }

    [Fact]
    public void TestMultipleEntriesParallel()
    {
      int bufferSize = 1024 * 1024 * 8;
      var wrBuffer = new byte[bufferSize];
      for (int i = 0, j = 0; i < bufferSize; i += 8, j++)
      {
        wrBuffer[i] = (byte)(j % 256);
      }


      var fileName = Path.GetTempFileName();
      using (var zipStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
      {
        var zipArchive = new ZipArchiveAxo(zipStream, ZipArchiveMode.Create, false);

        for (int i = 0; i < 4; ++i)
        {
          var entry = zipArchive.CreateEntry("TestEntry" + i.ToString());
          var entryStream = entry.Open();
          entryStream.Write(wrBuffer, 0, bufferSize);
          entryStream.Close();
        }
        zipArchive.Dispose();
      }

      var rdBuffer = new byte[bufferSize];

      // Open and verify

      var stopWatch = new System.Diagnostics.Stopwatch();
      stopWatch.Start();

      using (var zipStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        var zipArchive = new ZipArchiveAxo(zipStream, ZipArchiveMode.Read, false);

        var tasks = new Task[4];

        for (int iEntry = 0; iEntry < 4; ++iEntry)
        {
          var entry = zipArchive.GetEntry("TestEntry" + iEntry.ToString());
          var entryStream = entry.OpenStreamInReadModeParallel();
          tasks[iEntry] = Task.Run(() =>
          {
            var rd = entryStream.Read(rdBuffer, 0, bufferSize);
            Assert.Equal(bufferSize, rd);

            for (int i = 0; i < bufferSize; ++i)
            {
              Assert.Equal(wrBuffer[i], rdBuffer[i]);
            }

            entryStream.Close();
          });
        }

        Task.WaitAll(tasks); // Tasks must be finished before the zipArchive is closed!
        zipArchive.Dispose();
      }

      var elapsed = stopWatch.Elapsed;
      System.Diagnostics.Debug.WriteLine(string.Format("TestMultipleEntriesParallel, Elapsed time: {0}", elapsed));


      File.Delete(fileName);
    }
  }




}
