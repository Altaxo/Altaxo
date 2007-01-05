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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi
{
  using Background;

  public class LayerBackground
    :
    ICloneable,
    Main.IChangedEventSource,
    Main.IChildChangedEventSink,
    Main.IDocumentNode
  {
    IBackgroundStyle _background;
    double _leftPadding;
    double _rightPadding;
    double _topPadding;
    double _bottomPadding;

    [field:NonSerialized]
    event EventHandler _changed;

    [NonSerialized]
    object _parent;

    void CopyFrom(LayerBackground from)
    {
      this._background = null == from._background ? null : (IBackgroundStyle)from._background.Clone();
      this._leftPadding = from._leftPadding;
      this._rightPadding = from._rightPadding;
      this._topPadding = from._topPadding;
      this._bottomPadding = from._bottomPadding;
    }

    #region Serialization
    #region Version 0
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LayerBackground), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LayerBackground s = (LayerBackground)obj;

        info.AddValue("Background", s._background);
        info.AddValue("LeftPadding", s._leftPadding);
        info.AddValue("TopPadding", s._topPadding);
        info.AddValue("RightPadding", s._rightPadding);
        info.AddValue("BottomPadding", s._bottomPadding);

      }
      protected virtual LayerBackground SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LayerBackground s = (o == null ? new LayerBackground() : (LayerBackground)o);

        s._background = (Background.IBackgroundStyle)info.GetValue("Background", s);
        s._leftPadding = info.GetDouble("LeftPadding");
        s._topPadding = info.GetDouble("TopPadding");
        s._rightPadding = info.GetDouble("RightPadding");
        s._bottomPadding = info.GetDouble("BottomPadding");
        

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        LayerBackground s = SDeserialize(o, info, parent);
        return s;
      }
    }
    #endregion
    #endregion


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
  
#region IChangedEventSource Members

public event EventHandler Changed
{
	add { _changed += value;}	
	remove { _changed -= value; }
}

    protected virtual void OnChanged()
    {
      if(_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this,EventArgs.Empty);

      if(null!=_changed)
        _changed(this,EventArgs.Empty);
    }

#endregion

#region IChildChangedEventSink Members

public void  EhChildChanged(object child, EventArgs e)
{
 	OnChanged();
}

#endregion

#region IDocumentNode Members

public object  ParentObject
{
	get { return _parent; }
  set { _parent = value; }
}

public string  Name
{
	get { return "LayerBackground"; }
}

#endregion
}
}
