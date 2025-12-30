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

namespace Altaxo.Serialization.WITec
{
  /// <summary>
  /// Represents a text data class (TDText) extracted from a WITec project node.
  /// The class reads RTF formatted text stored in the underlying node's stream data and
  /// exposes it as a string in RTF format.
  /// </summary>
  public class TDTextClass : TDataClass
  {
    /// <summary>
    /// Backing node for the "TDStream" child node containing the text stream data.
    /// </summary>
    private WITecTreeNode _tdStream;

    /// <summary>
    /// Gets the text in RTF format as read from the node's stream data. If no stream data is present, this is an empty string.
    /// </summary>
    public string TextRtfFormat { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TDTextClass"/> class using the provided node.
    /// The constructor reads the "TDStream" child node and decodes the <c>StreamData</c> byte array into an RTF string.
    /// </summary>
    /// <param name="node">The node representing this text data class in the WITec tree.</param>
    public TDTextClass(WITecTreeNode node) : base(node)
    {
      _tdStream = node.GetChild("TDStream");

      var streamData = _tdStream.GetData<byte[]>("StreamData");

      if (streamData.Length > 0)
      {
        TextRtfFormat = WITecReader.TextEncoding.GetString(streamData);
      }
      else
      {
        TextRtfFormat = string.Empty;
      }
    }
  }
}

