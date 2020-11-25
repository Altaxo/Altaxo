#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

namespace System.IO.Compression
{
  internal class LittleEndianConverter
  {
    /// <summary>
    /// Converts a <see cref="UInt16"/> value and stores its byte representation in a buffer.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The storage position in the buffer .</param>
    public static void ToBuffer(Int16 value, byte[] buffer, int offset)
    {
      buffer[offset + 0] = (byte)((value) & 0xFF);
      buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
    }

    /// <summary>
    /// Converts a <see cref="UInt16"/> value and stores its byte representation in a buffer.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The storage position in the buffer .</param>
    public static void ToBuffer(UInt16 value, byte[] buffer, int offset)
    {
      buffer[offset + 0] = (byte)((value) & 0xFF);
      buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
    }

    /// <summary>
    /// Converts a <see cref="UInt32"/> value and stores its byte representation in a buffer.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The storage position in the buffer .</param>
    public static void ToBuffer(UInt32 value, byte[] buffer, int offset)
    {
      buffer[offset + 0] = (byte)((value) & 0xFF);
      buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
      buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
      buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
    }

    /// <summary>
    /// Converts a <see cref="UInt64"/> value and stores its byte representation in a buffer.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The storage position in the buffer .</param>
    public static void ToBuffer(UInt64 value, byte[] buffer, int offset)
    {
      buffer[offset + 0] = (byte)((value) & 0xFF);
      buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
      buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
      buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
      buffer[offset + 4] = (byte)((value >> 32) & 0xFF);
      buffer[offset + 5] = (byte)((value >> 40) & 0xFF);
      buffer[offset + 6] = (byte)((value >> 48) & 0xFF);
      buffer[offset + 7] = (byte)((value >> 56) & 0xFF);
    }

  }
}
