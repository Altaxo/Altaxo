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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using System.Text.RegularExpressions;



namespace Altaxo.Graph.Gdi.Shapes
{
  using Plot;
  using Plot.Data;
  using Graph.Plot.Data;
  using Background;


 

  /// <summary>
  /// TextGraphics provides not only simple text on a graph,
  /// but also some formatting of the text, and quite important - the plot symbols
  /// to be used either in the legend or in the axis titles
  /// </summary>
  [Serializable]
  public class TextGraphic : GraphicBase
  {
    protected string _text = ""; // the text, which contains the formatting symbols
    protected Font _font;
    protected BrushX _textBrush = new BrushX(Color.Black);
    protected IBackgroundStyle _background = null;
    protected float _lineSpacingFactor=1.25f; // multiplicator for the line space, i.e. 1, 1.5 or 2
    protected XAnchorPositionType _xAnchorType = XAnchorPositionType.Left;
    protected YAnchorPositionType _yAnchorType = YAnchorPositionType.Top;

    #region Cached or temporary variables


    /// <summary>
    /// Hashtable where the keys are graphic paths giving
    /// the position of a symbol into the list, and the values are the plot items.
    /// </summary>
    protected Dictionary<GraphicsPath, IGPlotItem> _cachedSymbolPositions = new Dictionary<GraphicsPath, IGPlotItem>();

    protected TextLine.TextLineCollection _cachedTextLines;
    protected bool _isStructureInSync=false; // true when the text was interpretet and the structure created
    protected bool _isMeasureInSync=false; // true when all items are measured
    protected float _cachedTextWidth=0; // the total width of the item
    protected float _cachedTextHeight=0; /// the total heigth of the item
    protected float _cyBaseLineSpace=0; // line space of the base font
    protected float _cyBaseAscent=0; // ascent of the base font
    protected float _cyBaseDescent=0; // descent of the base font
    protected float _widthOfOne_n = 0; // Width of the lower letter n
    protected float _widthOfThree_M = 0; // Width of three upper letters M
    protected PointF _cachedTextOffset; // offset of text to left upper corner of outer rectangle
    protected RectangleF _cachedExtendedTextBounds; // the text bounds extended by some margin around it
    #endregion // Cached or temporary variables


    #region Serialization

    #region ForClipboard

    protected TextGraphic(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this,info,context,null);
    }
    public override object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
    {
      base.SetObjectData(obj, info, context, selector);

      _text = info.GetString("Text");
      _font = (Font)info.GetValue("Font", typeof(Font));
      _textBrush = (BrushX)info.GetValue("Brush", typeof(BrushX));
      _background = (IBackgroundStyle)info.GetValue("BackgroundStyle", typeof(IBackgroundStyle));
      _lineSpacingFactor = info.GetSingle("LineSpacing");
      _xAnchorType = (XAnchorPositionType)info.GetValue("XAnchor", typeof(XAnchorPositionType));
      _yAnchorType = (YAnchorPositionType)info.GetValue("YAnchor", typeof(YAnchorPositionType));
      return this;
    }
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      base.GetObjectData(info, context);

      info.AddValue("Text", _text);
      info.AddValue("Font", _font);
      info.AddValue("Brush", _textBrush);
      info.AddValue("BackgroundStyle", _background);
      info.AddValue("LineSpacing", _lineSpacingFactor);
      info.AddValue("XAnchor", _xAnchorType);
      info.AddValue("YAnchor", _yAnchorType);
    }

    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
    }

    #endregion


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.TextGraphics", 0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ApplicationException("This serializer is not the actual version, and should therefore not be called");
        /*
        TextGraphics s = (TextGraphics)obj;
        info.AddBaseValueEmbedded(s,typeof(TextGraphics).BaseType);

        info.AddValue("Text",s.m_Text);
        info.AddValue("Font",s.m_Font);
        info.AddValue("Brush",s.m_BrushHolder);
        info.AddValue("BackgroundStyle",s.m_BackgroundStyle);
        info.AddValue("LineSpacing",s.m_LineSpacingFactor);
        info.AddValue("ShadowLength",s.m_ShadowLength);
        info.AddValue("XAnchor",s.m_XAnchorType);
        info.AddValue("YAnchor",s.m_YAnchorType);
        */
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        TextGraphic s = null!=o ? (TextGraphic)o : new TextGraphic(); 
        info.GetBaseValueEmbedded(s,typeof(TextGraphic).BaseType,parent);

        // we have changed the meaning of rotation in the meantime, This is not handled in GetBaseValueEmbedded, 
        // since the former versions did not store the version number of embedded bases
        s._rotation = -s._rotation;

        s._text = info.GetString("Text");
        s._font = (Font)info.GetValue("Font",typeof(Font));
        s._textBrush = (BrushX)info.GetValue("Brush",typeof(BrushX));
        s.BackgroundStyleOld = (BackgroundStyle)info.GetValue("BackgroundStyle",typeof(BackgroundStyle));
        s._lineSpacingFactor = info.GetSingle("LineSpacing");
        info.GetSingle("ShadowLength");
        s._xAnchorType = (XAnchorPositionType)info.GetValue("XAnchor",typeof(XAnchorPositionType));
        s._yAnchorType = (YAnchorPositionType)info.GetValue("YAnchor",typeof(YAnchorPositionType));

        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.TextGraphics", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextGraphic), 2)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        TextGraphic s = (TextGraphic)obj;
        info.AddBaseValueEmbedded(s, typeof(TextGraphic).BaseType);

        info.AddValue("Text", s._text);
        info.AddValue("Font", s._font);
        info.AddValue("Brush", s._textBrush);
        info.AddValue("BackgroundStyle", s._background);
        info.AddValue("LineSpacing", s._lineSpacingFactor);
        info.AddValue("XAnchor", s._xAnchorType);
        info.AddValue("YAnchor", s._yAnchorType);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        TextGraphic s = null != o ? (TextGraphic)o : new TextGraphic();
        info.GetBaseValueEmbedded(s, typeof(TextGraphic).BaseType, parent);

        s._text = info.GetString("Text");
        s._font = (Font)info.GetValue("Font", typeof(Font));
        s._textBrush = (BrushX)info.GetValue("Brush", typeof(BrushX));
        s._background = (IBackgroundStyle)info.GetValue("BackgroundStyle", typeof(IBackgroundStyle));
        s._lineSpacingFactor = info.GetSingle("LineSpacing");
        s._xAnchorType = (XAnchorPositionType)info.GetValue("XAnchor", typeof(XAnchorPositionType));
        s._yAnchorType = (YAnchorPositionType)info.GetValue("YAnchor", typeof(YAnchorPositionType));

        return s;
      }
    }

  
    #endregion



    #region Constructors

    public TextGraphic(TextGraphic from)
      :
      base(from) // all is done here, since CopyFrom is overridden
    {
    }

    public TextGraphic()
    {
      _font = new Font(FontFamily.GenericSansSerif,18,GraphicsUnit.World);
    }

    public TextGraphic(PointF graphicPosition, string text, 
      Font textFont, Color textColor)
    {
      this.SetPosition(graphicPosition);
      this.Font = textFont;
      this.Text = text;
      this.Color = textColor;
    }


    public TextGraphic(  float posX, float posY, 
      string text, Font textFont, Color textColor)
      : this(new PointF(posX, posY), text, textFont, textColor)
    {
    }


    public TextGraphic(PointF graphicPosition, 
      string text, Font textFont, 
      Color textColor, float Rotation)
      : this(graphicPosition, text, textFont, textColor)
    {
      this.Rotation = Rotation;
    }

    public TextGraphic(float posX, float posY, 
      string text, 
      Font textFont, 
      Color textColor, float Rotation)
      : this(new PointF(posX, posY), text, textFont, textColor, Rotation)
    {
    }

    #endregion

    protected override void CopyFrom(GraphicBase bfrom)
    {
      TextGraphic from = bfrom as TextGraphic;
      if (from != null)
      {
        this._text = from._text;
        this._font = from._font == null ? null : (Font)from._font.Clone();
        this._textBrush = from._textBrush == null ? null : (BrushX)from._textBrush.Clone();
        this._background = from._background == null ? null : (IBackgroundStyle)from._background.Clone();
        this._lineSpacingFactor = from._lineSpacingFactor;
        _xAnchorType = from._xAnchorType;
        _yAnchorType = from._yAnchorType;

        // don't clone the cached items
        this._cachedTextLines = null;
        this._isStructureInSync = false;
        this._isMeasureInSync = false;
      }
      base.CopyFrom(bfrom);
    }
    public void CopyFrom(TextGraphic from)
    {
      CopyFrom((GraphicBase)from);
    }


    static Regex _regexIntArgument = new Regex(@"\G\(\n*(?<argone>\d+)\n*\)");
    static Regex _regexIntIntArgument = new Regex(@"\G\(\n*(?<argone>\d+)\n*,\n*(?<argtwo>\d+)\n*\)");
    static Regex _regexIntQstrgArgument = new Regex(@"\G\(\n*(?<argone>\d+)\n*,\n*\""(?<argtwo>([^\\\""]*(\\\"")*(\\\\)*)+)\""\n*\)");
    static Regex _regexIntStrgArgument = new Regex(@"\G\(\n*(?<argone>\d+)\n*,\n*(?<argtwo>\w+)\n*\)");
    static Regex _regexIntIntStrgArgument = new Regex(@"\G\(\n*(?<argone>\d+)\n*,\n*(?<argtwo>\d+)\n*,\n*(?<argthree>\w+)\n*\)");
    // Be aware that double quote characters is in truth only one quote character, this is the syntax of a verbatim literal string
    static Regex _regexIntIntQstrgArgument = new Regex(@"\G\(\n*(?<argone>\d+)\n*,\n*(?<argtwo>\d+)\n*,\n*\""(?<argthree>([^\\\""]*(\\\"")*(\\\\)*)+)\""\n*\)");


    protected void Interpret(Graphics g)
    {
      this._isMeasureInSync = false; // if structure is changed, the measure is out of sync

      char[] searchchars = new Char[] { '\\', '\r', '\n', ')' };

      // Modification of StringFormat is necessary to avoid 
      // too big spaces between successive words
      StringFormat strfmt = (StringFormat)StringFormat.GenericTypographic.Clone();
      strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

      strfmt.LineAlignment = StringAlignment.Far;
      strfmt.Alignment = StringAlignment.Near;

      // next statement is necessary to have a consistent string length both
      // on 0 degree rotated text and rotated text
      // without this statement, the text is fitted to the pixel grid, which
      // leads to "steps" during scaling
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

      MeasureFont(g, _font, out _cyBaseLineSpace, out _cyBaseAscent, out _cyBaseDescent);

      System.Collections.Stack itemstack = new System.Collections.Stack();

      Font currFont = (Font)_font.Clone();


      if(null!=_cachedTextLines)
        _cachedTextLines.Clear(); // delete old contents 
      else
        _cachedTextLines = new TextLine.TextLineCollection();


      TextLine currTextLine = new TextLine();
        
      // create a new text line first
      _cachedTextLines.Add(currTextLine);
      int currTxtIdx = 0;

      TextItem currTextItem = new TextItem(currFont);
      //      TextItem firstItem = currTextItem; // preserve the first item
      


      currTextLine.Add(currTextItem);


      while(currTxtIdx<_text.Length)
      {

        // search for the first occurence of a backslash
        int bi = _text.IndexOfAny(searchchars,currTxtIdx);

        if(bi<0) // nothing was found
        {
          // move the rest of the text to the current item
          currTextItem.Text += _text.Substring(currTxtIdx,_text.Length-currTxtIdx);
          currTxtIdx = _text.Length;
        }
        else // something was found
        {
          // first finish the current item by moving the text from
          // currTxtIdx to (bi-1) to the current text item
          currTextItem.Text += _text.Substring(currTxtIdx,bi-currTxtIdx);
          
          if('\r'==_text[bi]) // carriage return character : simply ignore it
          {
            // simply ignore this character, since we search for \n
            currTxtIdx=bi+1;
          }
          else if('\n'==_text[bi]) // newline character : create a new line
          {
            currTxtIdx = bi+1;
            // create a new line
            currTextLine = new TextLine();
            _cachedTextLines.Add(currTextLine);
            // create also a new text item
            currTextItem = new TextItem(currTextItem,null);
            currTextLine.Add(currTextItem);
          }
          else if('\\'==_text[bi]) // backslash : look what comes after
          {
            if(bi+1<_text.Length && (')'==_text[bi+1] || '\\'==_text[bi+1])) // if a closing brace or a backslash, take these as chars
            {
              currTextItem.Text += _text[bi+1];
              currTxtIdx = bi+2;
            }
              // if the backslash not followed by a symbol and than a (, 
            else if(bi+3<_text.Length && !char.IsSeparator(_text,bi+1) && '('==_text[bi+2])
            {
              switch(_text[bi+1])
              {
                case 'b':
                case 'B':
                {
                  itemstack.Push(currTextItem);
                  currTextItem = new TextItem(currTextItem, new Font(currTextItem.Font.FontFamily,currTextItem.Font.Size,currTextItem.Font.Style | FontStyle.Bold, GraphicsUnit.World));
                  currTextLine.Add(currTextItem);
                  currTxtIdx = bi+3;
                }
                  break; // bold
                case 'i':
                case 'I':
                {
                  itemstack.Push(currTextItem);
                  currTextItem = new TextItem(currTextItem, new Font(currTextItem.Font.FontFamily,currTextItem.Font.Size,currTextItem.Font.Style | FontStyle.Italic, GraphicsUnit.World));
                  currTextLine.Add(currTextItem);
                  currTxtIdx = bi+3;
                }
                  break; // italic
                case 'u':
                case 'U':
                {
                  itemstack.Push(currTextItem);
                  currTextItem = new TextItem(currTextItem, new Font(currTextItem.Font.FontFamily,currTextItem.Font.Size,currTextItem.Font.Style | FontStyle.Underline, GraphicsUnit.World));
                  currTextLine.Add(currTextItem);
                  currTxtIdx = bi+3;
                }
                  break; // underlined
                case 's':
                case 'S': // strikeout
                {
                  itemstack.Push(currTextItem);
                  currTextItem = new TextItem(currTextItem, new Font(currTextItem.Font.FontFamily,currTextItem.Font.Size,currTextItem.Font.Style | FontStyle.Strikeout, GraphicsUnit.World));
                  currTextLine.Add(currTextItem);
                  currTxtIdx = bi+3;
                }
                  break; // end strikeout
                case 'g':
                case 'G':
                {
                  itemstack.Push(currTextItem);
                  currTextItem = new TextItem(currTextItem, new Font("Symbol",currTextItem.Font.Size,currTextItem.Font.Style, GraphicsUnit.World));
                  currTextLine.Add(currTextItem);
                  currTxtIdx = bi+3;
                }
                  break; // underlined
                case '+':
                case '-':
                {
                  itemstack.Push(currTextItem);
                  // measure the current font size
                  float cyLineSpace,cyAscent,cyDescent;
                  MeasureFont(g,currTextItem.Font,out cyLineSpace, out cyAscent, out cyDescent);
                  
                  currTextItem = new TextItem(currTextItem, new Font(currTextItem.Font.FontFamily,0.65f*currTextItem.Font.Size,currTextItem.Font.Style, GraphicsUnit.World));
                  currTextLine.Add(currTextItem);
                  currTextItem.m_SubIndex += ('+'==_text[bi+1] ? 1 : -1);


                  if('-'==_text[bi+1]) 
                    currTextItem.m_yShift += 0.15f*cyAscent; // Carefull: plus (+) means shift down
                  else
                    currTextItem.m_yShift -= 0.35f*cyAscent; // be carefull: minus (-) means shift up
                  
                  currTxtIdx = bi+3;
                }
                  break; // underlined
                case 'l': // Plot Curve Symbol
                case 'L':
                {
                  // parse the arguments
                  // either in the Form 
                  // \L(PlotCurveNumber) or
                  // \L(LayerNumber, PlotCurveNumber) or
                  // \L(LayerNumber, PlotCurveNumber, DataPointNumber)


                  // find the corresponding closing brace
                  int closingbracepos = _text.IndexOf(")",bi+1);
                  if(closingbracepos<0) // no brace found, so threat this as normal text
                  {
                    currTextItem.Text += _text.Substring(bi,3);
                    currTxtIdx += 3;
                    continue;
                  }
                  // count the commas between here and the closing brace to get
                  // the number of arguments
                  int parsepos=bi+3;
                  int[] arg = new int[3];
                  int args;
                  for(args=0;args<3 && parsepos<closingbracepos;args++)
                  {
                    int commapos = _text.IndexOf(",",parsepos,closingbracepos-parsepos);
                    int endpos = commapos>0 ? commapos : closingbracepos; // the end of this argument
                    try { arg[args]=System.Convert.ToInt32(_text.Substring(parsepos,endpos-parsepos)); }
                    catch(Exception) { break; }
                    parsepos = endpos+1;
                  }
                  if(args==0) // if not successfully parsed at least one number
                  {
                    currTextItem.Text += _text.Substring(bi,3);
                    currTxtIdx += 3;
                    continue;   // handle it as if it where normal text
                  }

                  // itemstack.Push(currTextItem); // here we don't need to put the item on the stack, since we pared until the closing brace
                  currTextItem = new TextItem(currTextItem,null);
                  currTextLine.Add(currTextItem);
                  currTextItem.SetAsSymbol(args,arg);

                  currTextItem = new TextItem(currTextItem,null); // create a normal text item behind the symbol item
                  currTextLine.Add(currTextItem); // to have room for the following text
                  currTxtIdx = closingbracepos+1;
                }
                  break; // curve symbol
                case '%': // Plot Curve Name
                {
                  // parse the arguments
                  // either in the Form 
                  // \%(PlotCurveNumber) or
                  // \%(LayerNumber, PlotCurveNumber) or
                  Match match;
                  int layerNumber=-1;
                  int plotNumber=-1;
                  string plotLabelStyle=null;
                  bool   plotLabelStyleIsPropColName=false;
                  if((match = _regexIntArgument.Match(_text,bi+2)).Success)
                  {
                    plotNumber = int.Parse(match.Result("${argone}"));
                  }
                  else if((match = _regexIntIntArgument.Match(_text,bi+2)).Success)
                  {
                    layerNumber = int.Parse(match.Result("${argone}"));
                    plotNumber =  int.Parse(match.Result("${argtwo}"));
                  }
                  else if((match = _regexIntQstrgArgument.Match(_text,bi+2)).Success)
                  {
                    plotNumber     = int.Parse(match.Result("${argone}"));
                    plotLabelStyle =  match.Result("${argtwo}");
                    plotLabelStyleIsPropColName=true;
                  }
                  else if((match = _regexIntStrgArgument.Match(_text,bi+2)).Success)
                  {
                    plotNumber     = int.Parse(match.Result("${argone}"));
                    plotLabelStyle =  match.Result("${argtwo}");
                  }
                  else if((match = _regexIntIntStrgArgument.Match(_text,bi+2)).Success)
                  {
                    layerNumber = int.Parse(match.Result("${argone}"));
                    plotNumber =  int.Parse(match.Result("${argtwo}"));
                    plotLabelStyle = match.Result("${argthree}");
                  }
                  else if((match = _regexIntIntQstrgArgument.Match(_text,bi+2)).Success)
                  {
                    layerNumber = int.Parse(match.Result("${argone}"));
                    plotNumber =  int.Parse(match.Result("${argtwo}"));
                    plotLabelStyle = match.Result("${argthree}");
                    plotLabelStyleIsPropColName=true;
                  }
      
                  if(match.Success)
                  {
                    itemstack.Push(currTextItem);
                    currTextItem = new TextItem(currTextItem,null);
                    currTextLine.Add(currTextItem);
                    currTextItem.SetAsPlotCurveName(layerNumber,plotNumber,plotLabelStyle,plotLabelStyleIsPropColName);

                    currTextItem = new TextItem(currTextItem,null); // create a normal text item behind the symbol item
                    currTextLine.Add(currTextItem); // to have room for the following text
                    currTxtIdx = bi+2+match.Length;
                  }
                  else
                  {
                    currTextItem.Text += _text.Substring(bi,2);
                    currTxtIdx += 3;
                    continue;   // handle it as if it where normal text
                  }
                }
                  break; // percent symbol
                default:
                  // take the sequence as it is
                  currTextItem.Text += _text.Substring(bi,3);
                  currTxtIdx = bi+3;
                  break;
              } // end of switch
            }
            else // if no formatting and also no closing brace or backslash, take it as it is
            {
              currTextItem.Text += _text[bi];
              currTxtIdx = bi+1;
            }
          } // end if it was a backslash
          else if(')'==_text[bi]) // closing brace
          {
            // the formating is finished, we can return to the formating of the previous section
            if(itemstack.Count>0)
            {
              TextItem preservedprevious = (TextItem)itemstack.Pop();
              currTextItem = new TextItem(preservedprevious,null);
              currTextLine.Add(currTextItem);
              currTxtIdx = bi+1;
            }
            else // if the stack is empty, take the brace as it is, and use the default style
            {
              currTextItem.Text += _text[bi];
              currTxtIdx = bi+1;
            }

          }
        }

      } // end of while loop

      this._isStructureInSync=true; // now the text was interpreted
    }
  

    protected void MeasureStructure(Graphics g, object obj)
    {
      PointF zeroPoint = new PointF(0,0);
  
      float maxLineWidth=0;

      
      // Modification of StringFormat is necessary to avoid 
      // too big spaces between successive words
      StringFormat strfmt = (StringFormat)StringFormat.GenericTypographic.Clone();
      strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

      strfmt.LineAlignment = StringAlignment.Far;
      strfmt.Alignment = StringAlignment.Near;

      // next statement is necessary to have a consistent string length both
      // on 0 degree rotated text and rotated text
      // without this statement, the text is fitted to the pixel grid, which
      // leads to "steps" during scaling
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
      MeasureFont(g, _font, out _cyBaseLineSpace, out _cyBaseAscent, out _cyBaseDescent);
      _widthOfOne_n = g.MeasureString("n",_font).Width;
      _widthOfThree_M = g.MeasureString("MMM",_font).Width;

      
      for(int nLine=0;nLine<_cachedTextLines.Count;nLine++)
      {

        float maxLineAscent=0;
        float maxLineDescent=0;
        float sumItemWidth=0;
        for(int nItem=0;nItem<_cachedTextLines[nLine].Count;nItem++)
        {

          TextItem ti = _cachedTextLines[nLine][nItem];

          if(ti.IsEmpty)
          {
            continue;
          }
          else if(ti.IsText)
          {
            MeasureFont(g,ti.Font, out ti.m_cyLineSpace, out ti.m_cyAscent, out ti.m_cyDescent);
            ti.m_Width = g.MeasureString(ti.Text, ti.Font, 0, strfmt).Width;
            
            maxLineAscent = Math.Max(ti.m_cyAscent-ti.m_yShift,maxLineAscent);
            maxLineDescent = Math.Max(ti.m_cyDescent+ti.m_yShift,maxLineDescent);
            sumItemWidth += ti.m_Width;
          }
          else if(ti.IsPlotCurveName)
          {
            // first of all, retrieve the actual name
            if(obj is XYPlotLayer)
            {
              XYPlotLayer layer = (XYPlotLayer)obj;
              if(ti.m_LayerNumber>=0 && ti.m_LayerNumber<layer.ParentLayerList.Count)
                layer = layer.ParentLayerList[ti.m_LayerNumber];

              IGPlotItem pa=null;
              if(ti.m_PlotNumber<layer.PlotItems.Flattened.Length)
              {
                pa = layer.PlotItems.Flattened[ti.m_PlotNumber];
              }
              if(pa!=null)
              {
                ti.PlotCurveName = pa.GetName(0);

                if(ti.m_PlotLabelStyle!=null && !ti.m_PlotLabelStyleIsPropColName && pa is XYColumnPlotItem)
                {
                  XYColumnPlotItemLabelTextStyle style = XYColumnPlotItemLabelTextStyle.YS;
                  try { style = (XYColumnPlotItemLabelTextStyle)Enum.Parse(typeof(XYColumnPlotItemLabelTextStyle),ti.m_PlotLabelStyle,true); }
                  catch(Exception) {}
                  ti.PlotCurveName = ((XYColumnPlotItem)pa).GetName(style);
                }

                if(ti.m_PlotLabelStyleIsPropColName && ti.m_PlotLabelStyle!=null && pa is XYColumnPlotItem)
                {
                  XYColumnPlotData pb = ((XYColumnPlotItem)pa).Data;
                  Data.DataTable tbl = null;
                  if(pb.YColumn is Data.DataColumn)
                    tbl = Data.DataTable.GetParentDataTableOf((Data.DataColumn)pb.YColumn);
                  
                  if(tbl!=null)
                  {
                    int colNumber = tbl.DataColumns.GetColumnNumber((Data.DataColumn)pb.YColumn);
                    if(tbl.PropertyColumns.ContainsColumn(ti.m_PlotLabelStyle))
                      ti.PlotCurveName = tbl.PropertyColumns[ti.m_PlotLabelStyle][colNumber].ToString();
                  }
                }
              }
            }
          
            MeasureFont(g,ti.Font, out ti.m_cyLineSpace, out ti.m_cyAscent, out ti.m_cyDescent);
            ti.m_Width = g.MeasureString(ti.PlotCurveName, ti.Font, 0, strfmt).Width;
            
            maxLineAscent = Math.Max(ti.m_cyAscent-ti.m_yShift,maxLineAscent);
            maxLineDescent = Math.Max(ti.m_cyDescent+ti.m_yShift,maxLineDescent);
            sumItemWidth += ti.m_Width;
          }

          else if(ti.IsSymbol)
          {
            if(obj is XYPlotLayer)
            {
              XYPlotLayer layer = (XYPlotLayer)obj;
              if(ti.m_LayerNumber>=0 && ti.m_LayerNumber<layer.ParentLayerList.Count)
                layer = layer.ParentLayerList[ti.m_LayerNumber];

              if(ti.m_PlotNumber<layer.PlotItems.Flattened.Length)
              {
                //Graph.PlotItem pa = layer.PlotAssociations[ti.m_PlotNumber];
                MeasureFont(g,ti.Font,out ti.m_cyLineSpace, out ti.m_cyAscent, out ti.m_cyDescent);
                ti.m_Width = g.MeasureString("MMM", ti.Font, 0, strfmt).Width;

                maxLineAscent = Math.Max(ti.m_cyAscent-ti.m_yShift,maxLineAscent);
                maxLineDescent = Math.Max(ti.m_cyDescent+ti.m_yShift,maxLineDescent);
                sumItemWidth += ti.m_Width;
              }
            }
          } // end if ti.IsSymbol
        } // end for all items in the line

        // now put the line properties
        _cachedTextLines[nLine].m_cyAscent = maxLineAscent;
        _cachedTextLines[nLine].m_cyDescent = maxLineDescent;
        _cachedTextLines[nLine].m_cyLineSpace = maxLineAscent + maxLineDescent;
        _cachedTextLines[nLine].m_Width = sumItemWidth;
      
        maxLineWidth = Math.Max(sumItemWidth,maxLineWidth);
      } // for all lines

      _cachedTextWidth = maxLineWidth; // total width of the object
      this._cachedTextHeight = _lineSpacingFactor * _cyBaseLineSpace * _cachedTextLines.Count;
      // add to the height the ascent difference of the first line and the descent difference of the last line
      if(_cachedTextLines.Count>0)
      {
        _cachedTextHeight += Math.Max(0,_cachedTextLines[0].m_cyAscent - _cyBaseAscent);
        _cachedTextHeight += Math.Max(0,_cachedTextLines[_cachedTextLines.Count-1].m_cyDescent - _cyBaseDescent);
      }

      // now measure the Background and the distance from outer rectangle to text
      MeasureBackground(g);

      _isMeasureInSync = true;
    } // end of function MeasureStructure

    /*
    protected void MeasureBackground()
    {

      float distanceXL = 0; // left distance bounds-text
      float distanceXR = 0; // right distance text-bounds
      float distanceYU = 0;   // upper y distance bounding rectangle-string
      float distanceYL = 0; // lower y distance

      if(this.m_BackgroundStyle!=BackgroundStyle.None)
      {
        // the distance to the sides should be like the character n
        distanceXL = 0.5f*m_WidthOfOne_n; // left distance bounds-text
        distanceXR = distanceXL; // right distance text-bounds
        distanceYU = m_cyBaseDescent;   // upper y distance bounding rectangle-string
        distanceYL = 0; // lower y distance

        // add some additional distance in case of special backgrounds
        switch(this.m_BackgroundStyle)
        {
          case BackgroundStyle.Shadow:
            distanceXR += this.m_ShadowLength; // the shadow extends to the right
            distanceYL += this.m_ShadowLength; // and to the lower bound
            break;
          case BackgroundStyle.DarkMarbel:
            distanceXL += this.m_ShadowLength; // darkmarbel has a rim of a Shadowlen
            distanceXR += this.m_ShadowLength; // to all sides
            distanceYU += this.m_ShadowLength;
            distanceYL += this.m_ShadowLength;
            break;
        }
      }

      SizeF size = new SizeF(m_TextWidth+distanceXL+distanceXR,m_TextHeight+distanceYU+distanceYL);

      
      float xanchor=0;
      float yanchor=0;
      if(m_XAnchorType==XAnchorPositionType.Center)
        xanchor = size.Width/2.0f;
      else if(m_XAnchorType==XAnchorPositionType.Right)
        xanchor = size.Width;

      if(m_YAnchorType==YAnchorPositionType.Center)
        yanchor = size.Height/2.0f;
      else if(m_YAnchorType==YAnchorPositionType.Bottom)
        yanchor = size.Height;
      
      this.m_Bounds = new RectangleF(new PointF(-xanchor,-yanchor),size);
      this.m_TextOffset = new PointF(distanceXL,distanceYU);
    }
     */

    protected void MeasureBackground(Graphics g)
    {

      float distanceXL = 0; // left distance bounds-text
      float distanceXR = 0; // right distance text-bounds
      float distanceYU = 0;   // upper y distance bounding rectangle-string
      float distanceYL = 0; // lower y distance

      
      if (this._background != null)
      {
        // the distance to the sides should be like the character n
        distanceXL = 0.25f * _widthOfOne_n; // left distance bounds-text
        distanceXR = distanceXL; // right distance text-bounds
        distanceYU = _cyBaseDescent;   // upper y distance bounding rectangle-string
        distanceYL = 0; // lower y distance
      }
      
      SizeF size = new SizeF(_cachedTextWidth + distanceXL + distanceXR, _cachedTextHeight + distanceYU + distanceYL);
      _cachedExtendedTextBounds = new RectangleF(PointF.Empty, size);
      RectangleF textRectangle = new RectangleF(new PointF(-distanceXL, -distanceYU), size);

      if (this._background != null)
      {
        RectangleF backgroundRect = this._background.MeasureItem(g, textRectangle);
        _cachedExtendedTextBounds.Offset(textRectangle.X - backgroundRect.X, textRectangle.Y - backgroundRect.Y);

        size = backgroundRect.Size;
        distanceXL = -backgroundRect.Left;
        distanceXR = backgroundRect.Right - _cachedTextWidth;
        distanceYU = -backgroundRect.Top;
        distanceYL = backgroundRect.Bottom - _cachedTextHeight;
      }

      float xanchor = 0;
      float yanchor = 0;
      if (_xAnchorType == XAnchorPositionType.Center)
        xanchor = size.Width / 2.0f;
      else if (_xAnchorType == XAnchorPositionType.Right)
        xanchor = size.Width;

      if (_yAnchorType == YAnchorPositionType.Center)
        yanchor = size.Height / 2.0f;
      else if (_yAnchorType == YAnchorPositionType.Bottom)
        yanchor = size.Height;

      this._bounds = new RectangleF(new PointF(-xanchor, -yanchor), size);
      this._cachedTextOffset = new PointF(distanceXL, distanceYU);
      
    }

    public IBackgroundStyle Background
    {
      get
      {
        return _background;
      }
      set
      {
        _background = value;
        _isMeasureInSync = false;
      }
    }

    private BackgroundStyle BackgroundStyleOld
    {
      get 
      {
        if (null == _background)
          return BackgroundStyle.None;
        else if (_background is BlackLine)
          return BackgroundStyle.BlackLine;
        else if (_background is BlackOut)
          return BackgroundStyle.BlackOut;
        else if (_background is DarkMarbel)
          return BackgroundStyle.DarkMarbel;
        else if (_background is RectangleWithShadow)
          return BackgroundStyle.Shadow;
        else if (_background is WhiteOut)
          return BackgroundStyle.WhiteOut;
        else
          return BackgroundStyle.None;
      }
      set
      {
        _isMeasureInSync = false;

        switch (value)
        {
            
          case BackgroundStyle.BlackLine:
            _background = new BlackLine();
            break;
          case BackgroundStyle.BlackOut:
            _background = new BlackOut();
            break;
          case BackgroundStyle.DarkMarbel:
            _background = new DarkMarbel();
            break;
          case BackgroundStyle.WhiteOut:
            _background = new WhiteOut();
            break;
          case BackgroundStyle.Shadow:
            _background = new RectangleWithShadow();
            break;
          case BackgroundStyle.None:
            _background = null;
            break;
        }
      }
    }

    public Font Font
    {
      get
      {
        return _font;
      }
      set
      {
        _font = value;
        this._isStructureInSync=false; // since the font is cached in the structure, it must be renewed
        this._isMeasureInSync=false;
      }
    }

    public bool Empty
    {
      get { return _text==null || _text.Length==0; }
    }

    public string Text
    {
      get
      {
        return _text;
      }
      set
      {
        _text = value;
        this._isStructureInSync=false;
      }
    }
    public System.Drawing.Color Color
    {
      get
      {
        return _textBrush.Color;
      }
      set
      {
        _textBrush = new BrushX(value);
      }
    }
    public BrushX TextFillBrush
    {
      get
      {
        return _textBrush;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        _textBrush = value.Clone();
      }
    }

    public XAnchorPositionType XAnchor
    {
      get { return _xAnchorType; }
      set { _xAnchorType=value; }
    }

    public YAnchorPositionType YAnchor
    {
      get { return _yAnchorType; }
      set { _yAnchorType=value; }
    }


  
    public static void MeasureFont(Graphics g, Font ft, out float cyLineSpace, out float cyAscent, out float cyDescent)
    { 
      // get some properties of the font
      cyLineSpace = ft.GetHeight(g); // space between two lines
      int   iCellSpace  = ft.FontFamily.GetLineSpacing(ft.Style);
      int   iCellAscent = ft.FontFamily.GetCellAscent(ft.Style);
      int   iCellDescent = ft.FontFamily.GetCellDescent(ft.Style);
      cyAscent  = cyLineSpace*iCellAscent/iCellSpace;
      cyDescent = cyLineSpace*iCellDescent/iCellSpace; 
    }


    protected virtual void PaintBackground(Graphics g)
    {
      // Assumptions: 
      // 1. the overall size of the structure must be measured before, i.e. bMeasureInSync is true
      // 2. the graphics object was translated and rotated before, so that the paining starts at (0,0)

      if(!this._isMeasureInSync)
        return;

      if (_background != null)
        _background.Draw(g, _cachedExtendedTextBounds);
    }

    public override void Paint(Graphics g, object obj)
    {
      Paint(g,obj,false);
    }

    public void Paint(Graphics g, object obj, bool bForPreview)
    {
      if(!this._isStructureInSync)
        this.Interpret(g);

      if(!this._isMeasureInSync)
        this.MeasureStructure(g,obj);

      _cachedSymbolPositions.Clear();

      System.Drawing.Drawing2D.GraphicsState gs = g.Save();
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
      

      Matrix transformmatrix= new Matrix();
      transformmatrix.Translate(X,Y);
      transformmatrix.Rotate(-_rotation);
      transformmatrix.Translate(_bounds.X,_bounds.Y);

      if(!bForPreview)
      {
        g.TranslateTransform(X,Y);
        g.RotateTransform(-_rotation);
        g.TranslateTransform(_bounds.X,_bounds.Y);
      }




      // first of all paint the background
      PaintBackground(g);



      // Modification of StringFormat is necessary to avoid 
      // too big spaces between successive words
      StringFormat strfmt = (StringFormat)StringFormat.GenericTypographic.Clone();
      strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

      strfmt.LineAlignment = StringAlignment.Near;
      strfmt.Alignment = StringAlignment.Near;

      float PlotSymbolWidth = g.MeasureString("MMM",_font,new PointF(0,0),strfmt).Width;

      float baseLineSpace, baseAscent, baseDescent;
      MeasureFont(g, _font, out baseLineSpace, out baseAscent, out baseDescent);
      _textBrush.Rectangle = new RectangleF(_cachedTextOffset,new SizeF(_cachedTextWidth, _cachedTextHeight));
    

      float currPosY=_cachedTextOffset.Y + baseAscent;

      for(int nLine=0;nLine<_cachedTextLines.Count;nLine++)
      {
        float currPosX=_cachedTextOffset.X;
        for(int nItem=0;nItem<_cachedTextLines[nLine].Count;nItem++)
        {

          TextItem ti = _cachedTextLines[nLine][nItem];

          if(ti.IsEmpty)
            continue;

          if(ti.IsText)
          {
            g.DrawString(ti.Text, ti.Font, _textBrush, new PointF(currPosX, currPosY + ti.m_yShift - ti.m_cyAscent), strfmt);
            // update positions
            currPosX += ti.m_Width;
          } // end of if ti.IsText
          else if(ti.IsPlotCurveName)
          {
            g.DrawString(ti.PlotCurveName, ti.Font, _textBrush, new PointF(currPosX, currPosY + ti.m_yShift - ti.m_cyAscent), strfmt);
            // update positions
            currPosX += ti.m_Width;
          } // end of if ti.IsText

          else if(ti.IsSymbol && obj is XYPlotLayer)
          {
            XYPlotLayer layer = (XYPlotLayer)obj;
            if(ti.m_LayerNumber>=0 && ti.m_LayerNumber<layer.ParentLayerList.Count)
              layer = layer.ParentLayerList[ti.m_LayerNumber];

            if(ti.m_PlotNumber<layer.PlotItems.Flattened.Length)
            {
              IGPlotItem pa = layer.PlotItems.Flattened[ti.m_PlotNumber];
            
              PointF symbolpos = new PointF(currPosX,currPosY + ti.m_yShift  + 0.5f*ti.m_cyDescent - 0.5f*ti.m_cyAscent);
              float symbolwidth = ti.m_Width;
              RectangleF symbolRect = new RectangleF(symbolpos,new SizeF(symbolwidth,0));
              symbolRect.Inflate(0, ti.Font.Size);
              pa.PaintSymbol(g,symbolRect);

              currPosX += ti.m_Width;
            
              if(!bForPreview)
              {
                GraphicsPath gp = new GraphicsPath();
                gp.AddRectangle(new RectangleF(symbolpos.X,symbolpos.Y-0.5f*ti.m_cyLineSpace,ti.m_Width,ti.m_cyLineSpace));
                gp.Transform(transformmatrix);
                this._cachedSymbolPositions.Add(gp,pa);
              }
            }

          } // end if ti.IsSymbol

        } // for all items in a textline
      
        currPosY += baseLineSpace*this._lineSpacingFactor;
      } // for all textlines
      


      g.Restore(gs);
    }

    public override object Clone()
    {
      return new TextGraphic(this);
    }

    public static DoubleClickHandler  PlotItemEditorMethod;
    public static DoubleClickHandler TextGraphicsEditorMethod;


    public override IHitTestObject HitTest(PointF pt)
    {
      IHitTestObject result;
      foreach(GraphicsPath gp in this._cachedSymbolPositions.Keys)
      {
        if(gp.IsVisible(pt))
        {
          result =  new HitTestObject(gp,_cachedSymbolPositions[gp]);
          result.DoubleClick = PlotItemEditorMethod;
          return result;
        }
      }
      
      result = base.HitTest(pt);
      if(null!=result)
        result.DoubleClick = TextGraphicsEditorMethod;
      return result;

    }

    #region IGrippableObject Members

    public override void ShowGrips(Graphics g)
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (_rotation != 0)
        g.RotateTransform(-_rotation);

      DrawRotationGrip(g,new PointF(1,1));
      g.DrawRectangle(Pens.Blue, _bounds.X, _bounds.Y, _bounds.Width, _bounds.Height);
      g.Restore(gs);
    }

    public override IGripManipulationHandle GripHitTest(PointF point)
    {
      PointF rel;

      rel = new PointF(1, 1);
      if (IsRotationGripHitted(rel, point))
        return new RotationGripHandle(this, rel);

      return null;
    }

    #endregion


    #region Inner Helper classes

    protected class TextItem
    {
      protected enum InnerType { Empty, Text, Symbol, PlotCurveName }

      protected InnerType m_Type = InnerType.Empty;

      protected string m_Text; // the text to display

      protected internal bool m_bUnderlined;
      protected internal bool m_bItalic;
      protected internal bool m_bBold; // true if bold should be used
      protected internal bool m_bGreek; // true if greek charset should be used
      protected internal int m_SubIndex;

      protected internal int m_LayerNumber = -1; // number of the layer or -1 for the current layer
      protected internal int m_PlotNumber = -1; // number of the plot curve or -1 in case this is disabled
      protected internal int m_PlotPointNumber = -1; // number of the plot point or -1 for the whole curve
      protected internal string m_PlotLabelStyle = null; // named style or name of property column
      protected internal bool m_PlotLabelStyleIsPropColName = false; // if true, then PlotLabelStyle is the name of a property column

      protected internal float m_cyLineSpace; // cached linespace value of the font
      protected internal float m_cyAscent;    // cached ascent value of the font
      protected internal float m_cyDescent; /// cached descent value of the font
      protected internal float m_Width; // cached width of the item


      // help items
      protected Font m_Font;
      public float m_yShift = 0;

      public TextItem(Font ft)
      {
        m_Type = InnerType.Empty;
        m_Text = "";
        m_Font = (null == ft) ? null : (Font)ft.Clone();
      }

      public void SetAsText(string txt)
      {
        m_Type = InnerType.Text;
        m_Text = txt;
      }

      public string Text
      {
        get { return m_Text; }
        set
        {
          m_Type = InnerType.Text;
          m_Text = value;
        }
      }

      public void SetAsSymbol(int args, int[] arg)
      {
        m_Type = InnerType.Symbol;
        m_Text = null;
        switch (args)
        {
          case 1:
            m_LayerNumber = -1;
            m_PlotNumber = arg[0];
            m_PlotPointNumber = -1;
            break;
          case 2:
            m_LayerNumber = arg[0];
            m_PlotNumber = arg[1];
            m_PlotPointNumber = -1;
            break;
          case 3:
            m_LayerNumber = arg[0];
            m_PlotNumber = arg[1];
            m_PlotPointNumber = arg[2];
            break;
        }
      }

      public string PlotCurveName
      {
        get { return m_Text; }
        set
        {
          m_Type = InnerType.PlotCurveName;
          m_Text = value;
        }
      }


      public void SetAsPlotCurveName(int layerNumber, int plotNumber, string plotLabelStyle, bool isPropCol)
      {
        m_Type = InnerType.PlotCurveName;
        m_Text = null;
        m_PlotPointNumber = -1;
        m_LayerNumber = layerNumber;
        m_PlotNumber = plotNumber;
        m_PlotLabelStyle = plotLabelStyle;
        m_PlotLabelStyleIsPropColName = isPropCol;
      }

      public void SetAsPlotCurveName(int plotNumber)
      {
        SetAsPlotCurveName(-1, plotNumber, null, false);
      }

      public void SetAsPlotCurveName(int layerNumber, int plotNumber)
      {
        SetAsPlotCurveName(layerNumber, plotNumber, null, false);
      }


      public void SetAsPlotCurveName(int args, int[] arg)
      {
        m_Type = InnerType.PlotCurveName;
        m_Text = null;
        switch (args)
        {
          case 1:
            m_LayerNumber = -1;
            m_PlotNumber = arg[0];
            m_PlotPointNumber = -1;
            break;
          case 2:
            m_LayerNumber = arg[0];
            m_PlotNumber = arg[1];
            m_PlotPointNumber = -1;
            break;
        }
      }


      /// <summary>
      /// 
      /// </summary>
      /// <param name="from"></param>
      /// <param name="ft"></param>
      public TextItem(TextItem from, Font ft)
      {
        m_Type = InnerType.Empty;
        m_Text = "";
        m_bUnderlined = from.m_bUnderlined;
        m_bItalic = from.m_bItalic;
        m_bBold = from.m_bBold;
        m_bGreek = from.m_bGreek;
        m_SubIndex = from.m_SubIndex;
        m_yShift = from.m_yShift;
        m_Font = null != ft ? ft : from.m_Font;
      }

      public Font Font
      {
        get { return m_Font; }
      }


      public bool IsEmpty
      {
        get { return m_Type == InnerType.Empty; }
      }

      public bool IsText
      {
        get { return m_Type == InnerType.Text; }
      }

      public bool IsSymbol
      {
        get { return m_Type == InnerType.Symbol; }
      }

      public bool IsPlotCurveName
      {
        get
        {
          return m_Type == InnerType.PlotCurveName;
        }
      }
    }


    protected class TextLine
    {
      List<TextItem> InnerList = new List<TextItem>();

      protected internal float m_cyLineSpace; // linespace value : cyAscent + cyDescent
      protected internal float m_cyAscent;    // height of the items above the ground line
      protected internal float m_cyDescent; /// heigth of the items below the ground line
      protected internal float m_Width; // cached width of the line (sum of width of all items)


      public int Count { get { return InnerList.Count; } }

      public TextItem this[int i]
      {
        get { return InnerList[i]; }
        set
        {
          if (i < Count)
            InnerList[i] = value;
          else if (i == Count)
            InnerList.Add(value);
          else
            throw new System.ArgumentOutOfRangeException("i", i, "The index was not in the valid range");
        }
      }

      public void Add(TextItem ti)
      {
        InnerList.Add(ti);
      }

      public class TextLineCollection : Altaxo.Data.CollectionBase
      {

        public TextLine this[int i]
        {
          get { return (TextLine)base.InnerList[i]; }
          set { base.InnerList[i] = value; }
        }

        public void Add(TextLine tl)
        {
          base.InnerList.Add(tl);
        }
      }


    } // end of class TextLine

    [Serializable]
    private enum BackgroundStyle
    {
      None,
      BlackLine,
      Shadow,
      DarkMarbel,
      WhiteOut,
      BlackOut
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyle", 0)]
    public class BackgroundStyleXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("This class is deprecated and no longer supported to serialize");
        // info.SetNodeContent(obj.ToString());  
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(BackgroundStyle), val, true);
      }
    }


    #endregion
  }
}




