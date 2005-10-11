using System;
using System.Drawing;
namespace Altaxo.Graph.BackgroundStyles
{
	/// <summary>
	/// Backs the item with a color filled rectangle.
	/// </summary>
	public class BackgroundColorStyle : IBackgroundStyle
	{
    protected BrushHolder _brush;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BackgroundColorStyle),0)]
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        BackgroundColorStyle s = (BackgroundColorStyle)obj;
        info.AddValue("Brush",s._brush);
        
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        BackgroundColorStyle s = null!=o ? (BackgroundColorStyle)o : new BackgroundColorStyle();
        s._brush = (BrushHolder)info.GetValue("Brush",parent);

        return s;
      }
    }

    #endregion


		public BackgroundColorStyle()
		{
    }

    public BackgroundColorStyle(Color c)
    {
      _brush = new BrushHolder(c);
    }

    public BackgroundColorStyle(BackgroundColorStyle from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(BackgroundColorStyle from)
    {
      this._brush = from._brush==null ? null : (BrushHolder)from._brush.Clone();
    }

    public object Clone()
    {
      return new BackgroundColorStyle(this);
    }

   

    #region IBackgroundStyle Members

    public System.Drawing.RectangleF MeasureItem(System.Drawing.Graphics g, System.Drawing.RectangleF innerArea)
    {
      return innerArea;
    }

    public void Draw(System.Drawing.Graphics g, System.Drawing.RectangleF innerArea)
    {
      if(_brush!=null)
        g.FillRectangle(_brush,innerArea);
    }

    public bool SupportsColor { get { return true; }}

    public Color Color
    {
      get
      {
        return _brush==null ? Color.Transparent : _brush.Color;
      }
      set
      {
        if(value==Color.Transparent)
          _brush = null;
        else if(_brush==null)
          _brush = new BrushHolder(value);
        else
          _brush = new BrushHolder( value) ;
      }
    }
    #endregion
  }
}
