#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Altaxo.Serialization.WITec
{
  /// <summary>
  /// Represents a node of the node tree of a WiTec project file.
  /// The node has a name, has primitive data, and maybe also subnodes.
  /// </summary>
  public class WITecTreeNode
  {
    /// <summary>
    /// Gets the text encoding that is used in WITec projects (Windows-1252).
    /// </summary>
    public static Encoding TextEncoding { get; } = Encoding.GetEncoding("Windows-1252");

    /// <summary>
    /// Gets the parent node, or null if this is the root node.
    /// </summary>
    public WITecTreeNode? Parent { get; }

    /// <summary>
    /// Gets the node name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the primitive data of this node.
    /// This are the basic types like string, double, int,
    /// as well as arrays thereof.
    /// </summary>
    public Dictionary<string, object> Data { get; } = [];

    public T GetData<T>(string name)
    {
      if (Data.TryGetValue(name, out var data))
      {
        if (data is T typedData)
          return typedData;
        else
          throw new InvalidDataException($"Data '{name}' is expected to has type {typeof(T)}, but the actual type is {data?.GetType()}");
      }
      else
      {
        throw new KeyNotFoundException($"Data '{name}' was not found.");
      }
    }

    /// <summary>
    /// Gets the child nodes of this node.
    /// </summary>
    public Dictionary<string, WITecTreeNode> ChildNodes { get; } = [];

    /// <summary>
    /// Gets the child node with the provided name. If is it not there, and exception will be thrown.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The child node with the provided name.</returns>
    /// <exception cref="System.IO.InvalidDataException">Node <{FullName}> is expected to have a child named \"{name}\"</exception>
    public WITecTreeNode GetChild(string name)
    {
      return ChildNodes.TryGetValue(name, out var child)
        ? child
        : throw new InvalidDataException($"Node <{FullName}> is expected to have a child named \"{name}\"");
    }

    /// <summary>
    /// Gets the full name of the node (names from the root to to this node, separated by a slash)
    /// </summary>
    public string FullName
    {
      get
      {
        var l = new List<string>();
        var p = this;
        while (p is not null)
        {
          l.Add(p.Name);
          p = p.Parent;
        }
        l.Reverse();
        return string.Join("/", l);
      }
    }

    /// <summary>
    /// Reads the RootNode and subsequently all childnodes of the Witec project file.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>The name of the root node and the root node of the Witec project file.</returns>
    public static (string Name, WITecTreeNode Node) Read(Stream stream)
    {
      var buffer = new byte[4096];
      var result = ReadNameValue(stream, buffer, null);
      return (result.Name, (WITecTreeNode)result.Value);
    }



    /// <summary>
    /// Creates a new instance of the <see cref="WITecTreeNode"/> class with the specified name, and then
    /// reading all child nodes and data of this node.
    /// </summary>
    /// <param name="nodeName">Name of the node.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="startOfData">The start of data.</param>
    /// <param name="endOfData">The end of data.</param>
    /// <param name="buffer">The buffer.</param>
    protected WITecTreeNode(string nodeName, Stream stream, long startOfData, long endOfData, byte[] buffer, WITecTreeNode? parent)
    {
      Name = nodeName;
      Parent = parent;
      stream.Seek(startOfData, SeekOrigin.Begin);
      while (stream.Position < endOfData)
      {
        var (name, data) = ReadNameValue(stream, buffer, this);

        if (data is WITecTreeNode tn)
        {
          ChildNodes.Add(name, tn);
        }
        else
        {
          Data.Add(name, data);
        }
      }
    }



    /// <summary>
    /// Reads a node, consisting of a name, and data. The data can be a Witec node, or simple data like string, double, or arrays.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="buffer">A buffer that can be used for reading the data in.</param>
    /// <param name="parentNode">The parent node of this data.</param>
    /// <returns>Tuple consisting of the name of the node, and the data. The data can be: a WitecTreeNode, a string, a double, integer, and arrays of the primitive types.</returns>
    public static (string Name, object Value) ReadNameValue(Stream stream, byte[] buffer, WITecTreeNode? parentNode)
    {
      var name = ReadString(stream, buffer);

      stream.ReadExactly(buffer, 0, sizeof(Int32) + 2 * sizeof(Int64));
      var typeOfNode = BitConverter.ToInt32(buffer, 0);
      var startOfData = BitConverter.ToInt64(buffer, sizeof(Int32));
      var endOfData = BitConverter.ToInt64(buffer, sizeof(Int32) + sizeof(Int64));

      switch (typeOfNode)
      {
        case 0: // a subnode
          var tn = new WITecTreeNode(name, stream, startOfData, endOfData, buffer, parentNode);
          return (name, tn);
        case 2: // typeof(double)
          {
            stream.Seek(startOfData, SeekOrigin.Begin);
            const int dataSize = 8;
            int numberOfPoints = (int)((endOfData - startOfData) / dataSize);
            if (numberOfPoints == 1)
            {
              stream.ReadExactly(buffer, 0, dataSize);
              return (name, BitConverter.ToDouble(buffer, 0));
            }
            else
            {
              var array = new double[numberOfPoints];
              var buf = new byte[numberOfPoints * dataSize];
              stream.ReadExactly(buf, 0, buf.Length);
              Buffer.BlockCopy(buf, 0, array, 0, numberOfPoints * dataSize);
              return (name, array);
            }
          }
        case 3: // typeof(float)
          {
            stream.Seek(startOfData, SeekOrigin.Begin);
            const int dataSize = 4;
            int numberOfPoints = (int)((endOfData - startOfData) / dataSize);
            if (numberOfPoints == 1)
            {
              stream.ReadExactly(buffer, 0, dataSize);
              return (name, BitConverter.ToSingle(buffer, 0));
            }
            else
            {
              var array = new float[numberOfPoints];
              var buf = new byte[numberOfPoints * dataSize];
              stream.ReadExactly(buf, 0, buf.Length);
              Buffer.BlockCopy(buf, 0, array, 0, numberOfPoints * dataSize);
              return (name, array);
            }
          }
        case 4: // typeof(int64)
          {
            stream.Seek(startOfData, SeekOrigin.Begin);
            const int dataSize = 8;
            int numberOfPoints = (int)((endOfData - startOfData) / dataSize);
            if (numberOfPoints == 1)
            {
              stream.ReadExactly(buffer, 0, dataSize);
              return (name, BitConverter.ToInt64(buffer, 0));
            }
            else
            {
              var array = new Int64[numberOfPoints];
              var buf = new byte[numberOfPoints * dataSize];
              stream.ReadExactly(buf, 0, buf.Length);
              Buffer.BlockCopy(buf, 0, array, 0, numberOfPoints * dataSize);
              return (name, array);
            }
          }
        case 5: // typeof(Int32)
          {
            stream.Seek(startOfData, SeekOrigin.Begin);
            const int dataSize = 4;
            int numberOfPoints = (int)((endOfData - startOfData) / dataSize);
            if (numberOfPoints == 1)
            {
              stream.ReadExactly(buffer, 0, dataSize);
              return (name, BitConverter.ToInt32(buffer, 0));
            }
            else
            {
              var array = new Int32[numberOfPoints];
              var buf = new byte[numberOfPoints * dataSize];
              stream.ReadExactly(buf, 0, buf.Length);
              Buffer.BlockCopy(buf, 0, array, 0, numberOfPoints * dataSize);
              return (name, array);
            }
          }
        case 6: // typeof(UInt32 or UInt16)
          {
            stream.Seek(startOfData, SeekOrigin.Begin);
            var len = (int)(endOfData - startOfData);
            stream.ReadExactly(buffer, 0, len);

            if (len == sizeof(UInt16))
            {
              return (name, BitConverter.ToUInt16(buffer, 0));
            }
            else if (len == sizeof(UInt32))
            {
              return (name, BitConverter.ToUInt32(buffer, 0));
            }

            if (0 != ((endOfData - startOfData) % (7 * sizeof(UInt16))))
              throw new InvalidProgramException($"Assumed data time as 7xUint16, but here are {endOfData - startOfData} bytes. Refer to Ref[2] to see what is expected.");

            var arr = new DateTime[len / (7 * sizeof(UInt16))];

            for (int i = 0, j = 0; i < arr.Length; ++i, j += 7 * sizeof(UInt16))
            {
              var year = BitConverter.ToUInt16(buffer, j);
              var month = BitConverter.ToUInt16(buffer, j + sizeof(UInt16));
              var day = BitConverter.ToUInt16(buffer, j + 2 * sizeof(UInt16));
              var hour = BitConverter.ToUInt16(buffer, j + 3 * sizeof(UInt16));
              var minute = BitConverter.ToUInt16(buffer, j + 4 * sizeof(UInt16));
              var second = BitConverter.ToUInt16(buffer, j + 5 * sizeof(UInt16));
              var millis = BitConverter.ToUInt16(buffer, j + 6 * sizeof(UInt16));
              arr[i] = new DateTime(year, month, day, hour, minute, second, millis);
            }
            return (name, arr);
          }
        case 7: // typeof(UInt8)
          {
            stream.Seek(startOfData, SeekOrigin.Begin);
            const int dataSize = 1;
            int numberOfPoints = (int)((endOfData - startOfData) / dataSize);
            if (numberOfPoints == 1)
            {
              stream.ReadExactly(buffer, 0, dataSize);
              return (name, buffer[0]);
            }
            else
            {
              var array = new byte[numberOfPoints];
              var buf = new byte[numberOfPoints * dataSize];
              stream.ReadExactly(buf, 0, buf.Length);
              Buffer.BlockCopy(buf, 0, array, 0, numberOfPoints * dataSize);
              return (name, array);
            }
          }
        case 8: // typeof(bool)
          {
            stream.Seek(startOfData, SeekOrigin.Begin);
            const int dataSize = 1;
            int numberOfPoints = (int)((endOfData - startOfData) / dataSize);
            if (numberOfPoints == 1)
            {
              stream.ReadExactly(buffer, 0, dataSize);
              return (name, buffer[0] != 0);
            }
            else
            {
              var array = new bool[numberOfPoints];
              var buf = new byte[numberOfPoints * dataSize];
              stream.ReadExactly(buf, 0, buf.Length);
              for (int i = 0; i < array.Length; ++i)
              {
                array[i] = buf[i] != 0;
              }

              return (name, array);
            }
          }
        case 9: // typeof(string)
          {
            stream.Seek(startOfData, SeekOrigin.Begin);
            var list = new List<string>();
            while (stream.Position < endOfData)
            {
              stream.ReadExactly(buffer, 0, 4);
              int stringLength = BitConverter.ToInt32(buffer, 0);
              stream.ReadExactly(buffer, 0, stringLength);
              var sstring = TextEncoding.GetString(buffer, 0, stringLength);
              list.Add(sstring);
            }
            if (list.Count == 1)
            {
              return (name, list[0]);
            }
            else
            {
              return (name, list.ToArray());
            }
          }

        default:
          throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Reads the string. This consist of a 4 byte integer that designates the string length, followed by the string data.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="buffer">Scratch array used to read the data.</param>
    /// <returns></returns>
    public static string ReadString(Stream stream, byte[] buffer)
    {
      stream.ReadExactly(buffer, 0, sizeof(Int32));
      int stringLength = BitConverter.ToInt32(buffer, 0);
      stream.ReadExactly(buffer, 0, stringLength);
      return TextEncoding.GetString(buffer, 0, stringLength);
    }
  }
}
