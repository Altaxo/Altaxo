#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using System.Text.RegularExpressions;

namespace Altaxo.Graph
{
  public class TextItem
  {
    protected enum InnerType { Empty, Text, Symbol, PlotCurveName }

    protected InnerType m_Type = InnerType.Empty;
    
    protected string m_Text; // the text to display

    protected internal bool m_bUnderlined;
    protected internal bool m_bItalic;
    protected internal bool m_bBold; // true if bold should be used
    protected internal bool m_bGreek; // true if greek charset should be used
    protected internal int  m_SubIndex;

    protected internal int m_LayerNumber=-1; // number of the layer or -1 for the current layer
    protected internal int m_PlotNumber=-1; // number of the plot curve or -1 in case this is disabled
    protected internal int m_PlotPointNumber=-1; // number of the plot point or -1 for the whole curve
    protected internal string m_PlotLabelStyle=null; // named style or name of property column
    protected internal bool m_PlotLabelStyleIsPropColName=false; // if true, then PlotLabelStyle is the name of a property column

    protected internal float m_cyLineSpace; // cached linespace value of the font
    protected internal float m_cyAscent;    // cached ascent value of the font
    protected internal float m_cyDescent; /// cached descent value of the font
    protected internal float m_Width; // cached width of the item


    // help items
    protected Font  m_Font;
    public    float m_yShift=0; 

    public TextItem(Font ft)
    {
      m_Type = InnerType.Empty;
      m_Text="";
      m_Font = (null==ft)? null: (Font)ft.Clone();
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
      m_Text=null;
      switch(args)
      {
        case 1:
          m_LayerNumber=-1;
          m_PlotNumber=arg[0];
          m_PlotPointNumber=-1;
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
      m_Text=null;
      m_PlotPointNumber = -1;
      m_LayerNumber = layerNumber;
      m_PlotNumber = plotNumber;
      m_PlotLabelStyle = plotLabelStyle;
      m_PlotLabelStyleIsPropColName = isPropCol;
    }

    public void SetAsPlotCurveName(int plotNumber)
    {
      SetAsPlotCurveName(-1,plotNumber,null,false);
    }

    public void SetAsPlotCurveName(int layerNumber, int plotNumber)
    {
      SetAsPlotCurveName(layerNumber,plotNumber,null,false);
    }


    public void SetAsPlotCurveName(int args, int[] arg)
    {
      m_Type = InnerType.PlotCurveName;
      m_Text=null;
      switch(args)
      {
        case 1:
          m_LayerNumber=-1;
          m_PlotNumber=arg[0];
          m_PlotPointNumber=-1;
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
      m_Type =  InnerType.Empty;
      m_Text="";
      m_bUnderlined = from.m_bUnderlined;
      m_bItalic     = from.m_bItalic;
      m_bBold       = from.m_bBold;
      m_bGreek = from.m_bGreek;
      m_SubIndex = from.m_SubIndex;
      m_yShift   = from.m_yShift;
      m_Font = null!=ft ? ft : from.m_Font;
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


  public class TextLine : Altaxo.Data.CollectionBase
  {
    protected internal float m_cyLineSpace; // linespace value : cyAscent + cyDescent
    protected internal float m_cyAscent;    // height of the items above the ground line
    protected internal float m_cyDescent; /// heigth of the items below the ground line
    protected internal float m_Width; // cached width of the line (sum of width of all items)

    public TextItem this[int i]
    {
      get { return (TextItem)base.InnerList[i]; }
      set 
      {
        if(i<Count)
          base.InnerList[i] = value;
        else if(i==Count)
          base.InnerList.Add(value);
        else
          throw new System.ArgumentOutOfRangeException("i",i,"The index was not in the valid range");
      }
    }

    public void Add(TextItem ti)
    {
      base.InnerList.Add(ti);
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
  

  public enum BackgroundStyle { None, BlackLine, Shadow, DarkMarbel, WhiteOut, BlackOut }

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BackgroundStyle),0)]
  public class BackgroundStyleXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      info.SetNodeContent(obj.ToString());  
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      
      string val = info.GetNodeContent();
      return System.Enum.Parse(typeof(BackgroundStyle),val,true);
    }
  }


  /// <summary>
  /// TextGraphics provides not only simple text on a graph,
  /// but also some formatting of the text, and quite important - the plot symbols
  /// to be used either in the legend or in the axis titles
  /// </summary>
  [SerializationSurrogate(0,typeof(TextGraphics.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class TextGraphics : GraphicsObject
  {
    [Serializable]
      public enum XAnchorPositionType { Left, Center, Right }
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XAnchorPositionType),0)]
      public class XAnchorPositionTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString());  
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(XAnchorPositionType),val,true);
      }
    }
    [Serializable]
      public enum YAnchorPositionType { Top, Center, Bottom }
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(YAnchorPositionType),0)]
      public class YAnchorPositionTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString());  
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(YAnchorPositionType),val,true);
      }
    }
    protected string m_Text = ""; // the text, which contains the formatting symbols
    protected Font m_Font;
    protected BrushHolder m_BrushHolder = new BrushHolder(Color.Black);
    protected BackgroundStyle m_BackgroundStyle = BackgroundStyle.None;
    protected float m_LineSpacingFactor=1.25f; // multiplicator for the line space, i.e. 1, 1.5 or 2
    protected float m_ShadowLength=5.0f; // length of the background shadow in 1/72 inch

    protected XAnchorPositionType m_XAnchorType = XAnchorPositionType.Left;
    protected YAnchorPositionType m_YAnchorType = YAnchorPositionType.Top;

    /// <summary>
    /// Hashtable where the keys are rectangles giving
    /// the position of a symbol into the list, and the values are the plot items.
    /// </summary>
    protected System.Collections.Hashtable m_CachedSymbolPositions = new System.Collections.Hashtable();

    #region Cached or temporary variables
    protected TextLine.TextLineCollection m_TextLines;
    protected bool m_bStructureInSync=false; // true when the text was interpretet and the structure created
    protected bool m_bMeasureInSync=false; // true when all items are measured
    protected float m_TextWidth=0; // the total width of the item
    protected float m_TextHeight=0; /// the total heigth of the item
    protected float m_cyBaseLineSpace=0; // line space of the base font
    protected float m_cyBaseAscent=0; // ascent of the base font
    protected float m_cyBaseDescent=0; // descent of the base font
    protected float m_WidthOfOne_n = 0; // Width of the lower letter n
    protected float m_WidthOfThree_M = 0; // Width of three upper letters M
    protected PointF m_TextOffset; // offset of text to left upper corner of outer rectangle
    #endregion // Cached or temporary variables


    #region Serialization
    /// <summary>Used to serialize the TextGraphics Version 0.</summary>
    public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes TextGraphics Version 0.
      /// </summary>
      /// <param name="obj">The TextGraphics to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        TextGraphics s = (TextGraphics)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
  
          // serialize the base class
          surr.GetObjectData(obj,info,context); // stream the data of the base object
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
        info.AddValue("Text",s.m_Text);
        info.AddValue("Font",s.m_Font);
        info.AddValue("Brush",s.m_BrushHolder);
        info.AddValue("BackgroundStyle",s.m_BackgroundStyle);
        info.AddValue("LineSpacing",s.m_LineSpacingFactor);
        info.AddValue("ShadowLength",s.m_ShadowLength);
        info.AddValue("XAnchor",s.m_XAnchorType);
        info.AddValue("YAnchor",s.m_YAnchorType);
      }
      /// <summary>
      /// Deserializes the TextGraphics Version 0.
      /// </summary>
      /// <param name="obj">The empty TextGraphics object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized TextGraphics.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        TextGraphics s = (TextGraphics)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
          // deserialize the base class
          surr.SetObjectData(obj,info,context,selector);
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
        s.m_Text = info.GetString("Text");
        s.m_Font = (Font)info.GetValue("Font",typeof(Font));
        s.m_BrushHolder = (BrushHolder)info.GetValue("Brush",typeof(BrushHolder));
        s.m_BackgroundStyle = (BackgroundStyle)info.GetValue("BackgroundStyle",typeof(BackgroundStyle));
        s.m_LineSpacingFactor = info.GetSingle("LineSpacing");
        s.m_ShadowLength = info.GetSingle("ShadowLength");
        s.m_XAnchorType = (XAnchorPositionType)info.GetValue("XAnchor",typeof(XAnchorPositionType));
        s.m_YAnchorType = (YAnchorPositionType)info.GetValue("YAnchor",typeof(YAnchorPositionType));

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextGraphics),0)]
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
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

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        TextGraphics s = null!=o ? (TextGraphics)o : new TextGraphics(); 
        info.GetBaseValueEmbedded(s,typeof(TextGraphics).BaseType,parent);

        s.m_Text = info.GetString("Text");
        s.m_Font = (Font)info.GetValue("Font",typeof(Font));
        s.m_BrushHolder = (BrushHolder)info.GetValue("Brush",typeof(BrushHolder));
        s.m_BackgroundStyle = (BackgroundStyle)info.GetValue("BackgroundStyle",typeof(BackgroundStyle));
        s.m_LineSpacingFactor = info.GetSingle("LineSpacing");
        s.m_ShadowLength = info.GetSingle("ShadowLength");
        s.m_XAnchorType = (XAnchorPositionType)info.GetValue("XAnchor",typeof(XAnchorPositionType));
        s.m_YAnchorType = (YAnchorPositionType)info.GetValue("YAnchor",typeof(YAnchorPositionType));

        return s;
      }
    }

    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
    }
    #endregion



    #region Constructors

    public TextGraphics(TextGraphics from)
      :
      base(from)
    {
      m_Text = from.m_Text;
      m_Font = null==from.Font ? null : (Font)from.Font.Clone();
      m_BrushHolder = null==m_BrushHolder ? new BrushHolder(Color.Black):(BrushHolder)from.m_BrushHolder.Clone();
      m_BackgroundStyle = from.BackgroundStyle;
      m_LineSpacingFactor = from.m_LineSpacingFactor;
      m_ShadowLength = from.m_ShadowLength;
      m_XAnchorType = from.m_XAnchorType;
      m_YAnchorType = from.m_YAnchorType;
  
      // don't clone the cached items
      m_TextLines=null;
      m_bStructureInSync=false;
      m_bMeasureInSync=false;
    }

    public TextGraphics()
    {
      m_Font = new Font(FontFamily.GenericSansSerif,18,GraphicsUnit.World);
    }

    public TextGraphics(PointF graphicPosition, string text, 
      Font textFont, Color textColor)
    {
      this.SetPosition(graphicPosition);
      this.Font = textFont;
      this.Text = text;
      this.Color = textColor;
    }


    public TextGraphics(  float posX, float posY, 
      string text, Font textFont, Color textColor)
      : this(new PointF(posX, posY), text, textFont, textColor)
    {
    }


    public TextGraphics(PointF graphicPosition, 
      string text, Font textFont, 
      Color textColor, float Rotation)
      : this(graphicPosition, text, textFont, textColor)
    {
      this.Rotation = Rotation;
    }

    public TextGraphics(float posX, float posY, 
      string text, 
      Font textFont, 
      Color textColor, float Rotation)
      : this(new PointF(posX, posY), text, textFont, textColor, Rotation)
    {
    }

    #endregion

    public void CopyFrom(TextGraphics from)
    {
      this.m_Text = from.m_Text;
      this.m_Font = from.m_Font==null ? null : (Font)from.m_Font.Clone();
      this.m_BrushHolder = from.m_BrushHolder==null ? null : (BrushHolder)from.m_BrushHolder.Clone();
      this.m_BackgroundStyle = from.m_BackgroundStyle;
      this.m_LineSpacingFactor = from.m_LineSpacingFactor;
      this.m_ShadowLength = from.m_ShadowLength;
      m_XAnchorType = from.m_XAnchorType;
      m_YAnchorType = from.m_YAnchorType;

      // don't clone the cached items
      this.m_TextLines=null;
      this.m_bStructureInSync=false;
      this.m_bMeasureInSync=false;
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
      this.m_bMeasureInSync = false; // if structure is changed, the measure is out of sync

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

      MeasureFont(g, m_Font, out m_cyBaseLineSpace, out m_cyBaseAscent, out m_cyBaseDescent);

      System.Collections.Stack itemstack = new System.Collections.Stack();

      Font currFont = (Font)m_Font.Clone();


      if(null!=m_TextLines)
        m_TextLines.Clear(); // delete old contents 
      else
        m_TextLines = new TextLine.TextLineCollection();


      TextLine currTextLine = new TextLine();
        
      // create a new text line first
      m_TextLines.Add(currTextLine);
      int currTxtIdx = 0;

      TextItem currTextItem = new TextItem(currFont);
      //      TextItem firstItem = currTextItem; // preserve the first item
      


      currTextLine.Add(currTextItem);


      while(currTxtIdx<m_Text.Length)
      {

        // search for the first occurence of a backslash
        int bi = m_Text.IndexOfAny(searchchars,currTxtIdx);

        if(bi<0) // nothing was found
        {
          // move the rest of the text to the current item
          currTextItem.Text += m_Text.Substring(currTxtIdx,m_Text.Length-currTxtIdx);
          currTxtIdx = m_Text.Length;
        }
        else // something was found
        {
          // first finish the current item by moving the text from
          // currTxtIdx to (bi-1) to the current text item
          currTextItem.Text += m_Text.Substring(currTxtIdx,bi-currTxtIdx);
          
          if('\r'==m_Text[bi]) // carriage return character : simply ignore it
          {
            // simply ignore this character, since we search for \n
            currTxtIdx=bi+1;
          }
          else if('\n'==m_Text[bi]) // newline character : create a new line
          {
            currTxtIdx = bi+1;
            // create a new line
            currTextLine = new TextLine();
            m_TextLines.Add(currTextLine);
            // create also a new text item
            currTextItem = new TextItem(currTextItem,null);
            currTextLine.Add(currTextItem);
          }
          else if('\\'==m_Text[bi]) // backslash : look what comes after
          {
            if(bi+1<m_Text.Length && (')'==m_Text[bi+1] || '\\'==m_Text[bi+1])) // if a closing brace or a backslash, take these as chars
            {
              currTextItem.Text += m_Text[bi+1];
              currTxtIdx = bi+2;
            }
              // if the backslash not followed by a symbol and than a (, 
            else if(bi+3<m_Text.Length && !char.IsSeparator(m_Text,bi+1) && '('==m_Text[bi+2])
            {
              switch(m_Text[bi+1])
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
                  currTextItem.m_SubIndex += ('+'==m_Text[bi+1] ? 1 : -1);


                  if('-'==m_Text[bi+1]) 
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
                  int closingbracepos = m_Text.IndexOf(")",bi+1);
                  if(closingbracepos<0) // no brace found, so threat this as normal text
                  {
                    currTextItem.Text += m_Text.Substring(bi,3);
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
                    int commapos = m_Text.IndexOf(",",parsepos,closingbracepos-parsepos);
                    int endpos = commapos>0 ? commapos : closingbracepos; // the end of this argument
                    try { arg[args]=System.Convert.ToInt32(m_Text.Substring(parsepos,endpos-parsepos)); }
                    catch(Exception) { break; }
                    parsepos = endpos+1;
                  }
                  if(args==0) // if not successfully parsed at least one number
                  {
                    currTextItem.Text += m_Text.Substring(bi,3);
                    currTxtIdx += 3;
                    continue;   // handle it as if it where normal text
                  }

                  itemstack.Push(currTextItem);
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
                  if((match = _regexIntArgument.Match(m_Text,bi+2)).Success)
                  {
                    plotNumber = int.Parse(match.Result("${argone}"));
                  }
                  else if((match = _regexIntIntArgument.Match(m_Text,bi+2)).Success)
                  {
                    layerNumber = int.Parse(match.Result("${argone}"));
                    plotNumber =  int.Parse(match.Result("${argtwo}"));
                  }
                  else if((match = _regexIntQstrgArgument.Match(m_Text,bi+2)).Success)
                  {
                    plotNumber     = int.Parse(match.Result("${argone}"));
                    plotLabelStyle =  match.Result("${argtwo}");
                    plotLabelStyleIsPropColName=true;
                  }
                  else if((match = _regexIntStrgArgument.Match(m_Text,bi+2)).Success)
                  {
                    plotNumber     = int.Parse(match.Result("${argone}"));
                    plotLabelStyle =  match.Result("${argtwo}");
                  }
                  else if((match = _regexIntIntStrgArgument.Match(m_Text,bi+2)).Success)
                  {
                    layerNumber = int.Parse(match.Result("${argone}"));
                    plotNumber =  int.Parse(match.Result("${argtwo}"));
                    plotLabelStyle = match.Result("${argthree}");
                  }
                  else if((match = _regexIntIntQstrgArgument.Match(m_Text,bi+2)).Success)
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
                    currTextItem.Text += m_Text.Substring(bi,2);
                    currTxtIdx += 3;
                    continue;   // handle it as if it where normal text
                  }
                }
                  break; // percent symbol
                default:
                  // take the sequence as it is
                  currTextItem.Text += m_Text.Substring(bi,3);
                  currTxtIdx = bi+3;
                  break;
              } // end of switch
            }
            else // if no formatting and also no closing brace or backslash, take it as it is
            {
              currTextItem.Text += m_Text[bi];
              currTxtIdx = bi+1;
            }
          } // end if it was a backslash
          else if(')'==m_Text[bi]) // closing brace
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
              currTextItem.Text += m_Text[bi];
              currTxtIdx = bi+1;
            }

          }
        }

      } // end of while loop

      this.m_bStructureInSync=true; // now the text was interpreted
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
      MeasureFont(g, m_Font, out m_cyBaseLineSpace, out m_cyBaseAscent, out m_cyBaseDescent);
      m_WidthOfOne_n = g.MeasureString("n",m_Font).Width;
      m_WidthOfThree_M = g.MeasureString("MMM",m_Font).Width;

      
      for(int nLine=0;nLine<m_TextLines.Count;nLine++)
      {

        float maxLineAscent=0;
        float maxLineDescent=0;
        float sumItemWidth=0;
        for(int nItem=0;nItem<m_TextLines[nLine].Count;nItem++)
        {

          TextItem ti = m_TextLines[nLine][nItem];

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
            if(obj is Altaxo.Graph.XYPlotLayer)
            {
              Graph.XYPlotLayer layer = (Graph.XYPlotLayer)obj;
              if(ti.m_LayerNumber>=0 && ti.m_LayerNumber<layer.ParentLayerList.Count)
                layer = layer.ParentLayerList[ti.m_LayerNumber];

              Graph.PlotItem pa=null;
              if(ti.m_PlotNumber<layer.PlotItems.Count)
              {
                pa = layer.PlotItems[ti.m_PlotNumber];
              }
              if(pa!=null)
              {
                ti.PlotCurveName = pa.GetName(0);

                if(ti.m_PlotLabelStyle!=null && !ti.m_PlotLabelStyleIsPropColName && pa is XYColumnPlotItem)
                {
                  Graph.XYColumnPlotItemLabelTextStyle style = Graph.XYColumnPlotItemLabelTextStyle.YS;
                  try { style = (Graph.XYColumnPlotItemLabelTextStyle)Enum.Parse(typeof(Graph.XYColumnPlotItemLabelTextStyle),ti.m_PlotLabelStyle,true); }
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
            if(obj is Altaxo.Graph.XYPlotLayer)
            {
              Graph.XYPlotLayer layer = (Graph.XYPlotLayer)obj;
              if(ti.m_LayerNumber>=0 && ti.m_LayerNumber<layer.ParentLayerList.Count)
                layer = layer.ParentLayerList[ti.m_LayerNumber];

              if(ti.m_PlotNumber<layer.PlotItems.Count)
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
        m_TextLines[nLine].m_cyAscent = maxLineAscent;
        m_TextLines[nLine].m_cyDescent = maxLineDescent;
        m_TextLines[nLine].m_cyLineSpace = maxLineAscent + maxLineDescent;
        m_TextLines[nLine].m_Width = sumItemWidth;
      
        maxLineWidth = Math.Max(sumItemWidth,maxLineWidth);
      } // for all lines

      m_TextWidth = maxLineWidth; // total width of the object
      this.m_TextHeight = m_LineSpacingFactor * m_cyBaseLineSpace * m_TextLines.Count;
      // add to the height the ascent difference of the first line and the descent difference of the last line
      if(m_TextLines.Count>0)
      {
        m_TextHeight += Math.Max(0,m_TextLines[0].m_cyAscent - m_cyBaseAscent);
        m_TextHeight += Math.Max(0,m_TextLines[m_TextLines.Count-1].m_cyDescent - m_cyBaseDescent);
      }

      // now measure the Background and the distance from outer rectangle to text
      MeasureBackground();

      m_bMeasureInSync = true;
    } // end of function MeasureStructure


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

    public BackgroundStyle BackgroundStyle
    {
      get { return m_BackgroundStyle; }
      set
      {
        if(m_BackgroundStyle != value)
        {
          m_BackgroundStyle = value;
          MeasureBackground(); // measure the background again
        }
      }
    }

    public Font Font
    {
      get
      {
        return m_Font;
      }
      set
      {
        m_Font = value;
        this.m_bStructureInSync=false; // since the font is cached in the structure, it must be renewed
        this.m_bMeasureInSync=false;
      }
    }

    public bool Empty
    {
      get { return m_Text==null || m_Text.Length==0; }
    }

    public string Text
    {
      get
      {
        return m_Text;
      }
      set
      {
        m_Text = value;
        this.m_bStructureInSync=false;
      }
    }
    public System.Drawing.Color Color
    {
      get
      {
        return m_BrushHolder.Color;
      }
      set
      {
        m_BrushHolder = new BrushHolder(value);
      }
    }

    public XAnchorPositionType XAnchor
    {
      get { return m_XAnchorType; }
      set { m_XAnchorType=value; }
    }

    public YAnchorPositionType YAnchor
    {
      get { return m_YAnchorType; }
      set { m_YAnchorType=value; }
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

      if(!this.m_bMeasureInSync)
        return;
    

      switch(this.m_BackgroundStyle)
      {
        default:
        case BackgroundStyle.None:
          break; // do nothing
        case BackgroundStyle.BlackLine:
          g.DrawRectangle(Pens.Black,0,0,m_Bounds.Width,m_Bounds.Height);
          break;
        case BackgroundStyle.BlackOut:
          g.FillRectangle(Brushes.Black,0,0,m_Bounds.Width,m_Bounds.Height);
          break;
        case BackgroundStyle.WhiteOut:
          g.FillRectangle(Brushes.White,0,0,m_Bounds.Width,m_Bounds.Height);
          break;
        case BackgroundStyle.Shadow:
          // please note: m_Bounds is already extended to the shadow
          g.FillRectangle(Brushes.Black,m_ShadowLength,m_ShadowLength,m_Bounds.Width-m_ShadowLength,m_Bounds.Height-m_ShadowLength);
          g.FillRectangle(Brushes.White,0,0,m_Bounds.Width-m_ShadowLength,m_Bounds.Height-m_ShadowLength);
          g.DrawRectangle(Pens.Black,0,0,m_Bounds.Width-m_ShadowLength,m_Bounds.Height-m_ShadowLength);
          break;
        case BackgroundStyle.DarkMarbel:
          g.FillRectangle(Brushes.Black,0.0f,0.0f,m_Bounds.Width,m_Bounds.Height);
          g.FillPolygon(Brushes.LightGray,new PointF[] {
                                                         new PointF(0,0), // upper left point
                                                         new PointF(m_Bounds.Width,0), // go to the right
                                                         new PointF(m_Bounds.Width-m_ShadowLength,m_ShadowLength), // go 45 deg left down in the upper right corner
                                                         new PointF(m_ShadowLength,m_ShadowLength), // upper left corner of the inner rectangle
                                                         new PointF(m_ShadowLength,m_Bounds.Height-m_ShadowLength), // lower left corner of the inner rectangle
                                                         new PointF(0,m_Bounds.Height) // lower left corner
                                                       });

          g.FillRectangle(Brushes.DimGray,m_ShadowLength,m_ShadowLength,m_Bounds.Width-2*m_ShadowLength,m_Bounds.Height-2*m_ShadowLength);
          break;
      } // end of switch BackgroundStyle
    }

    public override void Paint(Graphics g, object obj)
    {
      Paint(g,obj,false);
    }

    public void Paint(Graphics g, object obj, bool bForPreview)
    {
      if(!this.m_bStructureInSync)
        this.Interpret(g);

      if(!this.m_bMeasureInSync)
        this.MeasureStructure(g,obj);

      m_CachedSymbolPositions.Clear();

      System.Drawing.Drawing2D.GraphicsState gs = g.Save();
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
      

      Matrix transformmatrix= new Matrix();
      transformmatrix.Translate(X,Y);
      transformmatrix.Rotate(m_Rotation);
      transformmatrix.Translate(m_Bounds.X,m_Bounds.Y);

      if(!bForPreview)
      {
        g.TranslateTransform(X,Y);
        g.RotateTransform(m_Rotation);
        g.TranslateTransform(m_Bounds.X,m_Bounds.Y);
      }




      // first of all paint the background
      PaintBackground(g);



      // Modification of StringFormat is necessary to avoid 
      // too big spaces between successive words
      StringFormat strfmt = (StringFormat)StringFormat.GenericTypographic.Clone();
      strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

      strfmt.LineAlignment = StringAlignment.Near;
      strfmt.Alignment = StringAlignment.Near;

      float PlotSymbolWidth = g.MeasureString("MMM",m_Font,new PointF(0,0),strfmt).Width;

      float baseLineSpace, baseAscent, baseDescent;
      MeasureFont(g, m_Font, out baseLineSpace, out baseAscent, out baseDescent);
    

      float currPosY=m_TextOffset.Y + baseAscent;

      for(int nLine=0;nLine<m_TextLines.Count;nLine++)
      {
        float currPosX=m_TextOffset.X;
        for(int nItem=0;nItem<m_TextLines[nLine].Count;nItem++)
        {

          TextItem ti = m_TextLines[nLine][nItem];

          if(ti.IsEmpty)
            continue;

          if(ti.IsText)
          {
            g.DrawString(ti.Text, ti.Font, m_BrushHolder, new PointF(currPosX, currPosY + ti.m_yShift - ti.m_cyAscent), strfmt);
            // update positions
            currPosX += ti.m_Width;
          } // end of if ti.IsText
          else if(ti.IsPlotCurveName)
          {
            g.DrawString(ti.PlotCurveName, ti.Font, m_BrushHolder, new PointF(currPosX, currPosY + ti.m_yShift - ti.m_cyAscent), strfmt);
            // update positions
            currPosX += ti.m_Width;
          } // end of if ti.IsText

          else if(ti.IsSymbol && obj is Altaxo.Graph.XYPlotLayer)
          {
            Graph.XYPlotLayer layer = (Graph.XYPlotLayer)obj;
            if(ti.m_LayerNumber>=0 && ti.m_LayerNumber<layer.ParentLayerList.Count)
              layer = layer.ParentLayerList[ti.m_LayerNumber];

            if(ti.m_PlotNumber<layer.PlotItems.Count)
            {
              Graph.PlotItem pa = layer.PlotItems[ti.m_PlotNumber];
            
              PointF symbolpos = new PointF(currPosX,currPosY + ti.m_yShift  + 0.5f*ti.m_cyDescent - 0.5f*ti.m_cyAscent);
              
              if(pa is XYColumnPlotItem)
                ((XYColumnPlotItem)pa).Style.PaintSymbol(g, symbolpos, ti.m_Width);
              else if(pa is XYFunctionPlotItem)
                ((XYFunctionPlotItem)pa).Style.PaintSymbol(g, symbolpos, ti.m_Width);
              currPosX += ti.m_Width;
            
              if(!bForPreview)
              {
                GraphicsPath gp = new GraphicsPath();
                gp.AddRectangle(new RectangleF(symbolpos.X,symbolpos.Y-0.5f*ti.m_cyLineSpace,ti.m_Width,ti.m_cyLineSpace));
                gp.Transform(transformmatrix);
                this.m_CachedSymbolPositions.Add(gp,pa);
              }
            }

          } // end if ti.IsSymbol

        } // for all items in a textline
      
        currPosY += baseLineSpace*this.m_LineSpacingFactor;
      } // for all textlines
      


      g.Restore(gs);
    }

    public override object Clone()
    {
      return new TextGraphics(this);
    }

    public static DoubleClickHandler  PlotItemEditorMethod;
    public static DoubleClickHandler TextGraphicsEditorMethod;


    public override IHitTestObject HitTest(PointF pt)
    {
      IHitTestObject result;
      foreach(GraphicsPath gp in this.m_CachedSymbolPositions.Keys)
      {
        if(gp.IsVisible(pt))
        {
          result =  new HitTestObject(gp,m_CachedSymbolPositions[gp]);
          result.DoubleClick = PlotItemEditorMethod;
          return result;
        }
      }
      
      result = base.HitTest(pt);
      if(null!=result)
        result.DoubleClick = TextGraphicsEditorMethod;
      return result;

    }

    

  }
}




