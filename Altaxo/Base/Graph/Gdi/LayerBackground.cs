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
