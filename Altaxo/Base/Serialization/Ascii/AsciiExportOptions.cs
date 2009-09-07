using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Options for export of ASCII files
  /// </summary>
  public class AsciiExportOptions
  {
    /// <summary>
    /// The separator char.
    /// </summary>
    public char SeparatorChar { get; protected set; }

    /// <summary>
    /// Substitute for separator char. Should the separator char be present in header or items, it is replaced by this char.
    /// </summary>
    public char SubstituteForSeparatorChar { get; protected set; }
    
    /// <summary>
    /// Holds a dictionary of column types (keys) and functions (values), which convert a AltaxoVariant into a string.
    /// Normally the ToString() function is used on AltaxoVariant to convert to string. By using this dictionary it is possible
    /// to add custom converters.
    /// </summary>
    public Dictionary<System.Type, Func<Altaxo.Data.AltaxoVariant, string>> TypeConverterDictionary { get; protected set; }




    public AsciiExportOptions()
    {
      SeparatorChar = '\t';
      SubstituteForSeparatorChar = ' ';
      TypeConverterDictionary = new Dictionary<Type, Func<Altaxo.Data.AltaxoVariant, string>>();
    }

    public void SetSeparator(char separatorChar, char substituteChar)
    {
      if (separatorChar == substituteChar)
        throw new ArgumentException("separatorChar == substituteChar");
      SeparatorChar = separatorChar;
      SubstituteForSeparatorChar = substituteChar;
    }

    public void SetSeparator(char separatorChar)
    {
      SeparatorChar = separatorChar;
      if ('\t' == SeparatorChar)
        SubstituteForSeparatorChar = ' ';
      else
        SubstituteForSeparatorChar = '_';
    }


    #region Helper function

    public string DataItemToString(Altaxo.Data.DataColumn col, int index)
    {
      if (col.IsElementEmpty(index))
        return string.Empty;

      Func<Altaxo.Data.AltaxoVariant, string> func = null;
      if (TypeConverterDictionary.TryGetValue(col.GetType(), out func))
        return func(col[index]).Replace(SeparatorChar, SubstituteForSeparatorChar);
      else
        return col[index].ToString().Replace(SeparatorChar, SubstituteForSeparatorChar);
    }

    #endregion

  }
}
