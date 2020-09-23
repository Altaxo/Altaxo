using NUnit.Framework;

namespace System.IO.Compression
{
  [TestFixture]
  public class ZipConformityTest : ZipTestBase
  {
    private const int _numberOfEntries = 16;
    private int _bufferSize = 1024 * 8;
    private string _zipFile1Name;
    private string _zipFile2Name;

    [SetUp]
    public void Setup()
    {
      _zipFile1Name = Path.GetTempFileName();
      _zipFile2Name = Path.GetTempFileName();


      var wrBuffer = new byte[_bufferSize];

      var fileDate = DateTime.Now;

      using (var zipStream = new FileStream(_zipFile1Name, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
      {
        var zipArchive = new ZipArchiveAxo(zipStream, ZipArchiveMode.Create, false);

        for (int i = 0; i < _numberOfEntries; ++i)
        {
          var length = 0 == i % 2 ? FillBufferRandom(wrBuffer, i) : FillBufferText(wrBuffer, i);
          var entry = zipArchive.CreateEntry(TestEntryName(i));
          entry.LastWriteTime = fileDate;
          var entryStream = entry.Open();
          entryStream.Write(wrBuffer, 0, 3);
          entryStream.Write(wrBuffer, 3, length - 3);
          entryStream.Close();
        }
        zipArchive.Dispose();
      }

      using (var zipStream = new FileStream(_zipFile2Name, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
      {
        var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, false);

        for (int i = 0; i < _numberOfEntries; ++i)
        {
          var length = 0 == i % 2 ? FillBufferRandom(wrBuffer, i) : FillBufferText(wrBuffer, i);
          var entry = zipArchive.CreateEntry(TestEntryName(i));
          entry.LastWriteTime = fileDate;
          var entryStream = entry.Open();
          entryStream.Write(wrBuffer, 0, 3);
          entryStream.Write(wrBuffer, 3, length - 3);
          entryStream.Close();
        }
        zipArchive.Dispose();
      }
    }

    [TearDown]
    public void Teardown()
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


    [Test]
    public void Test_BothAreTheSame()
    {
      CompareArchives_UsingOriginal(_zipFile1Name, _zipFile2Name, _numberOfEntries);
      CompareArchives_UsingAltaxo(_zipFile1Name, _zipFile2Name, _numberOfEntries);
      CompareArchives_LengthAndContent(_zipFile1Name, _zipFile2Name);
    }


  }
}

