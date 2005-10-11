using System;
using System.Drawing;

using Altaxo.Data;
namespace Altaxo.Graph.AxisLabeling
{
	/// <summary>
	/// Summary description for AbstractLabelFormatting.
	/// </summary>
	public abstract class AbstractLabelFormatting : ILabelFormatting
	{
    #region ILabelFormatting Members

    /// <summary>
    /// Formats on item as text. If you do not provide this function, you have to override <see>MeasureItem</see> and <see>DrawItem</see>.
    /// </summary>
    /// <param name="item">The item to format as text.</param>
    /// <returns>The formatted text representation of this item.</returns>
    protected abstract string FormatItem(Altaxo.Data.AltaxoVariant item);
   
    public abstract object Clone();
    
    /// <summary>
    /// Formats a couple of items as text. Special measured can be taken here to format all items the same way, for instance set the decimal separator to the same location.
    /// Default implementation is using the Format function for
    /// all values in the array.
    /// Only neccessary to override this function if you do not override <see>GetMeasuredItems</see>.
    /// </summary>
    /// <param name="items">The items to format.</param>
    /// <returns>The text representation of the items.</returns>
    protected virtual string[] FormatItems(Altaxo.Data.AltaxoVariant[] items)
    {
      string[] result = new string[items.Length];
      for(int i=0;i<items.Length;++i)
        result[i] = FormatItem(items[i]);

      return result;
    }


    public virtual System.Drawing.SizeF MeasureItem(System.Drawing.Graphics g, System.Drawing.Font font, System.Drawing.StringFormat strfmt, Altaxo.Data.AltaxoVariant mtick, System.Drawing.PointF morg)
    {
      string text = FormatItem(mtick);
      return g.MeasureString(text, font, morg, strfmt);
    }

    public virtual void DrawItem(System.Drawing.Graphics g, BrushHolder brush, System.Drawing.Font font, System.Drawing.StringFormat strfmt, Altaxo.Data.AltaxoVariant item, PointF morg)
    {
      string text = FormatItem(item);
      g.DrawString(text, font, brush, morg, strfmt);
    }


    public virtual IMeasuredLabelItem[] GetMeasuredItems(Graphics g, System.Drawing.Font font, System.Drawing.StringFormat strfmt, AltaxoVariant[] items)
    {
     string[] titems = FormatItems(items);

      MeasuredLabelItem[] litems = new MeasuredLabelItem[titems.Length];

      Font localfont = (Font)font.Clone();
      StringFormat localstrfmt = (StringFormat)strfmt.Clone();

      for(int i=0;i<titems.Length;++i)
      {
        litems[i] = new MeasuredLabelItem(g,localfont,localstrfmt,titems[i]);
      }

      return litems;
    }

    protected class MeasuredLabelItem : IMeasuredLabelItem
    {
      protected string _text;
      protected Font _font;
      protected System.Drawing.StringFormat _strfmt;
      protected SizeF _size;

      #region IMeasuredLabelItem Members

      public MeasuredLabelItem(Graphics g, Font font, StringFormat strfmt, string itemtext)
      {
        _text = itemtext;
        _font = font;
        _strfmt = strfmt;
        _size = g.MeasureString(_text, _font, new PointF(0,0), strfmt);
      }

      public virtual SizeF Size
      {
        get
        {
          return _size;
        }
      }

      public virtual void Draw(Graphics g, BrushHolder brush, PointF point)
      {
        g.DrawString(_text, _font, brush, point, _strfmt);
      }

      #endregion

    }

    #endregion

 
  }
}
