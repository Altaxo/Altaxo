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

namespace Altaxo.Graph
{
  /// <summary>
  /// This class collects images (for use as textures) together with a unique name
  /// </summary>
  public class TextureManager : IEnumerable<KeyValuePair<string,ImageProxy>>
  {
    static TextureManager _builtinTextures = new TextureManager();
    static TextureManager _userTextures = new TextureManager();
    private Dictionary<string, ImageProxy> _texturesByHash = new Dictionary<string, ImageProxy>();

    /// <summary>
    /// Returns a texture manager with the builtin textures. These textures are 
    /// common to all installations.
    /// </summary>
    public static TextureManager BuiltinTextures 
    {
      get
      {
        return _builtinTextures;
      }
    }

    /// <summary>
    /// Returns a texture manager with the builtin textures. These textures are 
    /// common to all installations.
    /// </summary>
    public static TextureManager UserTextures
    {
      get
      {
        return _userTextures;
      }
    }


    private TextureManager()
    {
    }


    public void Add(ImageProxy texture)
    {
      _texturesByHash.Add(texture.ContentHash, texture);
    }

    public ImageProxy this[string hash]
    {
      get
      {
        return _texturesByHash[hash];
      }
    }

    public bool Contains(string hash)
    {
      return _texturesByHash.ContainsKey(hash);
    }

   
    #region IEnumerable<KeyValuePair<string,ImageProxy>> Members

    public IEnumerator<KeyValuePair<string, ImageProxy>> GetEnumerator()
    {
      List<KeyValuePair<string,ImageProxy>> dict = new List<KeyValuePair<string,ImageProxy>>();
      foreach(ImageProxy p in _texturesByHash.Values)
        dict.Add(new KeyValuePair<string,ImageProxy>(p.Name,p));
      dict.Sort(new KVComparer());
      return dict.GetEnumerator();
    }

    class KVComparer : IComparer<KeyValuePair<string, ImageProxy>>
    {
      #region IComparer<KeyValuePair<string,ImageProxy>> Members

      public int Compare(KeyValuePair<string, ImageProxy> x, KeyValuePair<string, ImageProxy> y)
      {
        return string.Compare(x.Key, y.Key);
      }

      #endregion
    }


    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
  }
}
