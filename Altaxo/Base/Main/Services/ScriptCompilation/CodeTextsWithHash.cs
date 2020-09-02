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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services.ScriptCompilation
{
  /// <summary>
  /// Stores one or more code texts in immutable form, together with a hash string that identifies it uniquely.
  /// </summary>
  public class CodeTextsWithHash : IEnumerable<string>
  {
    /// <summary>
    /// Gets the code text.
    /// </summary>
    /// <value>
    /// The code text.
    /// </value>
    public IReadOnlyList<string> CodeTexts { get; private set; }

    /// <summary>
    /// Gets the hash that is a unique identifier of the code text.
    /// </summary>
    /// <value>
    /// The hash.
    /// </value>
    public string Hash { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeTextsWithHash"/> class.
    /// </summary>
    /// <param name="codeTexts">The code text(s).</param>
    public CodeTextsWithHash(IEnumerable<string> codeTexts)
    {
      if (codeTexts is null)
        throw new ArgumentNullException(nameof(codeTexts));

      CodeTexts = codeTexts.ToImmutableArray();

      if (CodeTexts.Count == 0)
        throw new ArgumentException(nameof(codeTexts) + " is empty!", nameof(codeTexts));

      Hash = ComputeScriptTextHash(CodeTexts);
    }

    public static string ComputeScriptTextHash(IReadOnlyList<string> scripts)
    {
      int len = 0;
      for (int i = 0; i < scripts.Count; i++)
        len += scripts[i].Length;

      byte[]? hash = null;

      using (var stream = new System.IO.MemoryStream(len))
      {
        using (var sw = new System.IO.StreamWriter(stream, System.Text.Encoding.Unicode))
        {
          for (int i = 0; i < scripts.Count; i++)
          {
            sw.Write(scripts[i]);
          }
          sw.Flush();

          sw.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
          var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
          hash = md5.ComputeHash(sw.BaseStream);
          sw.Close();
        }
      }
      var Lo = System.BitConverter.ToUInt64(hash, 0);
      var Hi = System.BitConverter.ToUInt64(hash, 8);
      return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:X16}{1:X16}", Hi, Lo);
    }

    public IEnumerator<string> GetEnumerator()
    {
      return CodeTexts.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return CodeTexts.GetEnumerator();
    }
  }
}
