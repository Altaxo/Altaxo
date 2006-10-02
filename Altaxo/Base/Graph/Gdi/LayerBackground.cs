using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi
{
  using Background;

  public class LayerBackground : ICloneable
  {
    IBackgroundStyle _background;
    double _leftPadding;
    double _rightPadding;
    double _topPadding;
    double _bottomPadding;

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
  }
}
