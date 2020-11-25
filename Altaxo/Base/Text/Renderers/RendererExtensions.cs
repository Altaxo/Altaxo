#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Text.Renderers
{
  public static class RendererExtensions
  {
    /// <summary>
    /// Helper to calculate MD5 hashes.
    /// </summary>
    private static readonly System.Security.Cryptography.MD5 _md5Hasher = System.Security.Cryptography.MD5.Create();

    public static string ExtractTextContentFrom(Markdig.Syntax.LeafBlock leafBlock)
    {
      var result = string.Empty;

      if (leafBlock.Inline is null)
        return result;

      foreach (var il in leafBlock.Inline)
      {
        result += il.ToString();
      }

      return result;
    }

    public static string CreateGuidFromHeaderTitles(List<string> headerTitles)
    {
      var stb = new System.Text.StringBuilder();

      for (int i = 0; i < headerTitles.Count; ++i)
      {
        if (i != 0)
          stb.Append(" - ");
        stb.Append(headerTitles[i]);
      }

      byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(stb.ToString());

      byte[] hash = _md5Hasher.ComputeHash(inputBytes);

      // step 2, convert byte array to hex string

      stb.Length = 0;

      for (int i = 0; i < hash.Length; i++)
      {
        stb.Append(hash[i].ToString("X2"));
      }

      return stb.ToString();
    }


    /// <summary>
    /// Creates the file name from header titles and a unique identifier.
    /// </summary>
    /// <param name="headerTitles">The header titles.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <param name="firstHeadingBlockIsParentOfAll">Set this to true, if there is only one header of level1, which is the parent of all other text.</param>
    /// <returns>A file name (without path, without extension) that should be unique and easily identifyable.</returns>
    /// <remarks>
    /// <para>Background:</para>
    /// <para>To bring the files in a version control system, it is not appropriate to simply numerate the files - then with every new section the numbers and thus the file names would change.</para>
    /// <para>We cound have used the guid created by the titles, but this would make the file names hard to identify.</para>
    /// <para>Thus we compromise: use the three first letters of the titles of max three levels, and then append some part of the guid.</para>
    /// </remarks>
    public static string CreateFileNameFromHeaderTitlesAndGuid(List<string> headerTitles, string guid, bool firstHeadingBlockIsParentOfAll)
    {
      if (headerTitles.Count == 0)
      {
        return guid;
      }
      else if (headerTitles.Count == 1)
      {
        return FirstThreeCharsFromHeaderTitle(headerTitles[0]) + "-" + guid.Substring(0, guid.Length - 4);
      }
      else
      {
        var stb = new StringBuilder();
        int offset = firstHeadingBlockIsParentOfAll ? 1 : 0; // if we have only one first level heading block, we do not use it for the name
        for (int i = offset; i < Math.Min(headerTitles.Count, 3 + offset); ++i)
        {
          stb.Append(FirstThreeCharsFromHeaderTitle(headerTitles[i]));
          stb.Append("-");
        }
        stb.Append(guid.Substring(0, guid.Length - stb.Length));
        return stb.ToString();
      }
    }

    private static readonly char[] _headerTitleSplitChars =
  {
      ' ',
      '\t',
      '!',
      '"',
      '§',
      '$',
      '%',
      '%',
      '&',
      '/',
      '{',
      '(',
      '[',
      ')',
      ']',
      '=',
      '}',
      '?',
      '\\',
      '´',
      '`',
      '*',
      '+',
      '~',
      '\'',
      '#',
      ';',
      ',',
      ':',
      '.',
      '_',
      '-',
      '^',
      '°',
      '@',
      '<',
      '>',
      '|',
    };

    private static readonly HashSet<string> _notUsedWordsFromHeaderTitle = new HashSet<string>(new string[]
    { "A", "AN", "THE" /* "AND", "OR", "BY", "A", "THE", "ON", "OF", "IN", "FROM", "WITH",  */});

    public static string FirstThreeCharsFromHeaderTitle(string title)
    {
      var words = title.ToUpperInvariant().Split(_headerTitleSplitChars, StringSplitOptions.RemoveEmptyEntries);
      var stb = new StringBuilder(3);

      for (int i = 0; i < words.Length; ++i)
      {
        var word = words[i];
        if (_notUsedWordsFromHeaderTitle.Contains(word))
          continue;

        if (char.IsLetterOrDigit(word[0]))
          stb.Append(word[0]);
        if (stb.Length == 3)
          break;
      }

      return stb.ToString();
    }
  }
}
