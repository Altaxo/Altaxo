#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Background
{
  /// <summary>
  /// Backs the item with a color filled rectangle.
  /// </summary>
  [Serializable]
  public class WhiteOut
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IBackgroundStyle
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyles.WhiteOut", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WhiteOut), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (WhiteOut)obj;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        WhiteOut s = null != o ? (WhiteOut)o : new WhiteOut();

        return s;
      }
    }

    #endregion Serialization

    public WhiteOut()
    {
    }

    public WhiteOut(WhiteOut from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(WhiteOut from)
    {
      if (object.ReferenceEquals(this, from))
        return;
    }

    public object Clone()
    {
      return new WhiteOut(this);
    }

    #region IBackgroundStyle Members

    public RectangleD2D MeasureItem(System.Drawing.Graphics g, RectangleD2D innerArea)
    {
      return innerArea;
    }

    public void Draw(System.Drawing.Graphics g, RectangleD2D innerArea)
    {
      g.FillRectangle(Brushes.White, (float)innerArea.Left, (float)innerArea.Top, (float)innerArea.Width, (float)innerArea.Height);
    }

    public void Draw(Graphics g, BrushX brush, RectangleD2D innerArea)
    {
      throw new NotImplementedException();
    }

    public bool SupportsBrush { get { return false; } }

    public BrushX Brush
    {
      get
      {
        return null;
      }
      set
      {
      }
    }

    #endregion IBackgroundStyle Members
  }
}
