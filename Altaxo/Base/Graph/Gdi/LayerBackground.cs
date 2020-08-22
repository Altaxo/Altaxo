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

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altaxo.Graph.Gdi
{
  using Background;

  public class LayerBackground
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    ICloneable
  {
    private IBackgroundStyle? _background;
    private double _leftPadding;
    private double _rightPadding;
    private double _topPadding;
    private double _bottomPadding;

    
    private void CopyFrom(LayerBackground from)
    {
      if (object.ReferenceEquals(this, from))
        return;

      ChildCloneToMember(ref _background, from._background);

      _leftPadding = from._leftPadding;
      _rightPadding = from._rightPadding;
      _topPadding = from._topPadding;
      _bottomPadding = from._bottomPadding;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_background is not null)
        yield return new Main.DocumentNodeAndName(_background, "Background");
    }

    #region Serialization

    #region Version 0

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LayerBackground), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LayerBackground)obj;

        info.AddValueOrNull("Background", s._background);
        info.AddValue("LeftPadding", s._leftPadding);
        info.AddValue("TopPadding", s._topPadding);
        info.AddValue("RightPadding", s._rightPadding);
        info.AddValue("BottomPadding", s._bottomPadding);
      }

      protected virtual LayerBackground SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LayerBackground?)o ?? new LayerBackground();

        s.ChildSetMember(ref s._background , info.GetValueOrNull<Background.IBackgroundStyle> ("Background", s));
        s._leftPadding = info.GetDouble("LeftPadding");
        s._topPadding = info.GetDouble("TopPadding");
        s._rightPadding = info.GetDouble("RightPadding");
        s._bottomPadding = info.GetDouble("BottomPadding");

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        LayerBackground s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    public LayerBackground()
    {
    }

    public LayerBackground(LayerBackground from)
    {
      CopyFrom(from);
    }

    public LayerBackground(IBackgroundStyle style)
    {
      _background = style;
    }

    public LayerBackground Clone()
    {
      return new LayerBackground(this);
    }

    object ICloneable.Clone()
    {
      return new LayerBackground(this);
    }

    public void Draw(Graphics g, RectangleF rect)
    {
    }
  }
}
