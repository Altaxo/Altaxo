﻿#region Copyright

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

#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Background
{
  /// <summary>
  /// Backs the item with a color filled rectangle.
  /// </summary>
  [Serializable]
  public class BlackLine
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IBackgroundStyle
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyles.BlackLine", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BlackLine), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (BlackLine)obj;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (BlackLine?)o ?? new BlackLine();

        return s;
      }
    }

    #endregion Serialization

    public BlackLine()
    {
    }

    public BlackLine(BlackLine from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(BlackLine from)
    {
      if (ReferenceEquals(this, from))
        return;
    }

    public object Clone()
    {
      return new BlackLine(this);
    }

    #region IBackgroundStyle Members

    public RectangleD2D MeasureItem(System.Drawing.Graphics g, RectangleD2D innerArea)
    {
      return innerArea;
    }

    public void Draw(System.Drawing.Graphics g, RectangleD2D innerArea)
    {
      g.DrawRectangle(Pens.Black, (float)innerArea.Left, (float)innerArea.Top, (float)innerArea.Width, (float)innerArea.Height);
    }

    public void Draw(Graphics g, BrushX brush, RectangleD2D innerArea)
    {
      throw new NotImplementedException();
    }

    public bool SupportsBrush { get { return false; } }

    [MaybeNull]
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
