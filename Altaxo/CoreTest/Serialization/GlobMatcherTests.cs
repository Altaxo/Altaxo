#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Altaxo.Serialization
{
  public class GlobMatcherTests
  {
    [Theory]
    [InlineData("C:/work/*.txt", "C:/work/readme.txt", true)]
    [InlineData("C:/work/*.txt", "C:/work/sub/readme.txt", false)]
    [InlineData("C:/work/???.txt", "C:/work/abc.txt", true)]
    [InlineData("C:/work/**/file?.txt", "C:/work/a/b/file1.txt", true)]
    [InlineData("C:/work/file[1].txt", "C:/work/file[1].txt", true)]
    [InlineData("C:/work/file[1].txt", "C:/work/file1.txt", false)]
    [InlineData("C:\\work\\*.txt", "C:\\work\\readme.txt", true)]
    [InlineData("C:\\work\\*.txt", "C:\\work\\sub\\readme.txt", false)]
    [InlineData("C:\\work\\???.txt", "C:\\work\\abc.txt", true)]
    [InlineData("C:\\work\\**\\file?.txt", "C:\\work\\a\\b\\file1.txt", true)]
    [InlineData("C:\\work\\file[1].txt", "C:\\work\\file[1].txt", true)]
    [InlineData("C:\\work\\file[1].txt", "C:\\work\\file1.txt", false)]
    public void StaticIsMatch_MatchesExpectedPatterns(string pattern, string input, bool expected)
    {
      Assert.Equal(expected, GlobMatcher.IsMatch(pattern, input));
    }

    [Fact]
    public void StaticIsMatch_RespectsCaseSensitivity()
    {
      Assert.True(GlobMatcher.IsMatch("C:/work/*.txt", "C:/work/README.TXT", caseSensitive: false));
      Assert.False(GlobMatcher.IsMatch("C:/work/*.txt", "C:/work/README.TXT", caseSensitive: true));

      Assert.True(GlobMatcher.IsMatch("C:\\work\\*.txt", "C:\\work\\README.TXT", caseSensitive: false));
      Assert.False(GlobMatcher.IsMatch("C:\\work\\*.txt", "C:\\work\\README.TXT", caseSensitive: true));
    }

    [Fact]
    public void InstanceIsMatch_UsesPositiveAndNegativePatterns()
    {
      var matcher = new GlobMatcher(
        new[] { "C:/work/**/*.txt", "C:\\work\\**\\*.txt" },
        new[] { "C:/work/skip/*.txt", "C:\\work\\skip\\*.txt" },
        caseSensitive: true);

      Assert.True(matcher.IsMatch("C:/work/keep/readme.txt"));
      Assert.True(matcher.IsMatch("C:\\work\\keep\\readme.txt"));
      Assert.False(matcher.IsMatch("C:/work/skip/readme.txt"));
      Assert.False(matcher.IsMatch("C:\\work\\skip\\readme.txt"));
      Assert.False(matcher.IsMatch("C:/work/notes.md"));
    }

    [Fact]
    public void InstanceIsMatch_UsesCaseSensitivitySetting()
    {
      var caseInsensitiveMatcher = new GlobMatcher(new[] { "C:/work/*.txt", "C:\\work\\*.txt" }, Array.Empty<string>(), caseSensitive: false);
      var caseSensitiveMatcher = new GlobMatcher(new[] { "C:/work/*.txt", "C:\\work\\*.txt" }, Array.Empty<string>(), caseSensitive: true);

      Assert.True(caseInsensitiveMatcher.IsMatch("c:/work/README.TXT"));
      Assert.True(caseInsensitiveMatcher.IsMatch("c:\\work\\README.TXT"));
      Assert.False(caseSensitiveMatcher.IsMatch("c:/work/README.TXT"));
      Assert.False(caseSensitiveMatcher.IsMatch("c:\\work\\README.TXT"));
    }

    [Fact]
    public void GetMatchingFiles_ReturnsOnlyFilesMatchingPatterns_AndHonorsExclusions()
    {
      string tempRoot = Path.Combine(Path.GetTempPath(), "GlobMatcherTests_" + Guid.NewGuid().ToString("N"));
      string projectDir = Path.Combine(tempRoot, "project");
      string nestedDir = Path.Combine(projectDir, "nested");
      Directory.CreateDirectory(nestedDir);

      string keepFile = Path.Combine(projectDir, "keep.txt");
      string nestedFile = Path.Combine(nestedDir, "nested.txt");
      string ignoreFile = Path.Combine(projectDir, "ignore.md");

      File.WriteAllText(keepFile, "keep");
      File.WriteAllText(nestedFile, "nested");
      File.WriteAllText(ignoreFile, "ignore");

      try
      {
        var matcher = new GlobMatcher(
          new[] { Path.Combine(projectDir, "*.txt") },
          new[] { Path.Combine(nestedDir, "*.txt") },
          caseSensitive: true);

        var files = matcher.GetMatchingFiles();
        var actual = files.Select(f => f.FullName).OrderBy(f => f).ToArray();

        Assert.Equal(new[] { keepFile }, actual);
      }
      finally
      {
        if (Directory.Exists(tempRoot))
        {
          Directory.Delete(tempRoot, recursive: true);
        }
      }
    }

    [Fact]
    public void GetMatchingFiles_Throws_WhenPositivePatternIsRelative()
    {
      var matcher = new GlobMatcher(new[] { "relative/**/*.txt", "relative\\**\\*.txt" }, Array.Empty<string>());

      Assert.Throws<InvalidOperationException>(() => matcher.GetMatchingFiles());
    }

    [Fact]
    public void GetLongestCommonFolder_ReturnsCommonAncestor_ForMultiplePaths()
    {
      string[] slashPaths =
      {
        "C:/src/project/alpha/file1.txt",
        "C:/src/project/alpha/sub/file2.txt",
        "C:/src/project/beta/file3.txt"
      };

      string[] backslashPaths =
      {
        "C:\\src\\project\\alpha\\file1.txt",
        "C:\\src\\project\\alpha\\sub\\file2.txt",
        "C:\\src\\project\\beta\\file3.txt"
      };

      Assert.Equal(@"C:\src\project", GlobMatcher.GetLongestCommonFolder(slashPaths));
      Assert.Equal(@"C:\src\project", GlobMatcher.GetLongestCommonFolder(backslashPaths));
    }

    [Fact]
    public void GetLongestCommonFolder_UsesCaseInsensitiveComparison_ByDefault()
    {
      string[] slashPaths =
      {
        "C:/src/project/Alpha/file1.txt",
        "C:/src/project/alpha/sub/file2.txt"
      };

      string[] backslashPaths =
      {
        "C:\\src\\project\\Alpha\\file1.txt",
        "C:\\src\\project\\alpha\\sub\\file2.txt"
      };

      Assert.Equal(@"C:\src\project\Alpha", GlobMatcher.GetLongestCommonFolder(slashPaths));
      Assert.Equal(@"C:\src\project\Alpha", GlobMatcher.GetLongestCommonFolder(backslashPaths));
    }

    [Fact]
    public void GetLongestCommonFolder_ReturnsDirectoryForSinglePath_AndEmptyForNoPaths()
    {
      Assert.Equal(@"C:\work", GlobMatcher.GetLongestCommonFolder(new[] { "C:/work/file.txt" }));
      Assert.Equal(@"C:\work", GlobMatcher.GetLongestCommonFolder(new[] { "C:\\work\\file.txt" }));
      Assert.Equal(string.Empty, GlobMatcher.GetLongestCommonFolder(Array.Empty<string>()));
    }
  }
}
