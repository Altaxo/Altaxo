using System.Collections.Generic;
using Xunit;

namespace Altaxo.Serialization
{
  public class FileIOHelperTests
  {
    [Fact]
    public void TestCompletenessOfInvalidFileNameChars()
    {
      char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();

      foreach (var c in invalidChars)
      {
        if (c != '\\')
        {
          Assert.Contains(c, (IReadOnlyDictionary<char, char>)FileIOHelper.InvalidFileNameChars);
        }
      }
    }

    [Fact]
    public void TestNotTooManyInvalidFileNameChars()
    {
      var invalidChars = new HashSet<char>(System.IO.Path.GetInvalidFileNameChars());

      foreach (var c in FileIOHelper.InvalidFileNameChars.Keys)
      {
        Assert.Contains(c, invalidChars);
      }
    }
  }
}
