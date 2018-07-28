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
      if (null == codeTexts)
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

      byte[] hash = null;

      using (System.IO.MemoryStream stream = new System.IO.MemoryStream(len))
      {
        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(stream, System.Text.Encoding.Unicode))
        {
          for (int i = 0; i < scripts.Count; i++)
          {
            sw.Write(scripts[i]);
          }
          sw.Flush();

          sw.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
          System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
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
