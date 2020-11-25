using Xunit;

namespace System.IO.Compression
{

  public class ZipCopyTest : ZipTestBase, IDisposable
  {
    private const int _numberOfEntries = 16;
    private int _bufferSize = 1024 * 8;
    private string _zipFile1Name;
    private string _zipFile2Name;

    public ZipCopyTest()
    {
      _zipFile1Name = Path.GetTempFileName();
      _zipFile2Name = Path.GetTempFileName();


      var wrBuffer = new byte[_bufferSize];



      using (var zipStream = new FileStream(_zipFile1Name, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
      {
        var zipArchive = new ZipArchiveAxo(zipStream, ZipArchiveMode.Create, false);

        for (int i = 0; i < _numberOfEntries; ++i)
        {
          var length = FillBufferRandom(wrBuffer, i);
          var entry = zipArchive.CreateEntry(TestEntryName(i));
          var entryStream = entry.Open();
          entryStream.Write(wrBuffer, 0, length);
          entryStream.Close();
        }
        zipArchive.Dispose();
      }
    }

    // TearDown
    public void Dispose()
    {
      try
      {
        File.Delete(_zipFile1Name);
      }
      catch (Exception)
      {

      }

      try
      {
        File.Delete(_zipFile2Name);
      }
      catch (Exception)
      {

      }
    }

    [Fact]
    public void Test1_CopyAllEntries()
    {
      var wrBuffer = new byte[_bufferSize];


      using (var zip1Stream = new FileStream(_zipFile1Name, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        var zip1Archive = new ZipArchiveAxo(zip1Stream, ZipArchiveMode.Read, false);


        using (var zip2Stream = new FileStream(_zipFile2Name, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
        {
          var zip2Archive = new ZipArchiveAxo(zip2Stream, ZipArchiveMode.Create, false);

          for (int i = 0; i < _numberOfEntries; ++i)
          {
            var entry = zip1Archive.GetEntry(TestEntryName(i));
            var entryN = zip2Archive.CopyEntryFromAnotherArchive(entry);
          }
          zip2Archive.Dispose();
        }

        zip1Archive.Dispose();
      }

      CompareArchives_LengthAndContent(_zipFile1Name, _zipFile2Name);
      CompareArchives_UsingOriginal(_zipFile1Name, _zipFile2Name, _numberOfEntries);
      CompareArchives_UsingAltaxo(_zipFile1Name, _zipFile2Name, _numberOfEntries);
    }
  }
}

