#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Serialization
{
  public class FileIOHelper
  {
    static Dictionary<char, char> _invalidChars;

    static FileIOHelper()
    {
      _invalidChars = new Dictionary<char, char>();

      char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
      int boxchar = 0;
      for(int i=0;i<invalidChars.Length;++i)
      {
        char c = invalidChars[i];
        char repl;

        if (c < 32)
          repl = (char)(0xF001 + c); // from a private range
        else
          repl = (char)(0x2550 + boxchar++); // from the box char range

        _invalidChars.Add(c, repl);
      }

    }

    /// <summary>
    /// Get a valid file name out of a raw file name that can contain invalid characters. Those characters are
    /// replaced by unicode box characters (range from 0x2550 upwards).
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetValidFileName(string name)
    {
      StringBuilder stb = new StringBuilder(name);
      for (int i = 0; i < name.Length; ++i)
      {
        char c = stb[i];
        if (_invalidChars.ContainsKey(c))
          stb[i] = _invalidChars[c]; // we replace this character with a box character
      }

      return stb.ToString();
    }
  }
}
