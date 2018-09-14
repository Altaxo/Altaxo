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

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using Drawing;
  using Graph.Plot.Data;

  /// <summary>
  /// Used for constructor of <see cref="XYLineScatterPlotStyle" /> to choose between Line, Scatter and both.
  /// </summary>
  [Flags]
  public enum LineScatterPlotStyleKind
  {
    /// <summary>
    /// Neither line nor scatter used.
    /// </summary>
    Empty = 0,

    /// <summary>Line only. No symbol plotted, no line-symbol-gap.</summary>
    Line = 1,

    /// <summary>Scatter only. No line is plotted, no line-symbol gap.</summary>
    Scatter = 2,

    /// <summary>Both line and symbol are plotted, line symbol gap is on by default.</summary>
    LineAndScatter = 3
  }

#if true // must be kept for old deserialization

  /// <summary>
  /// Deprecated; only used for deserialization of old versions.
  /// </summary>
  internal class XYLineScatterPlotStyle
  {
    protected LinePlotStyle m_LineStyle;
    protected ScatterPlotStyle m_ScatterStyle;
    protected bool m_LineSymbolGap;

    /// <summary>The label style (is null if there is no label).</summary>
    protected LabelPlotStyle m_LabelStyle;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYLineScatterPlotStyle", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("Serialization of old versions not supported");
        /*
				XYLineScatterPlotStyle s = (XYLineScatterPlotStyle)obj;
				info.AddValue("XYPlotLineStyle",s.m_LineStyle);
				info.AddValue("XYPlotScatterStyle",s.m_ScatterStyle);
				info.AddValue("LineSymbolGap",s.m_LineSymbolGap);
				 */
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYLineScatterPlotStyle s = null != o ? (XYLineScatterPlotStyle)o : new XYLineScatterPlotStyle(info);
        // do not use settings lie s.XYPlotLineStyle= here, since the XYPlotLineStyle is cloned, but maybe not fully deserialized here!!!
        s.m_LineStyle = (LinePlotStyle)info.GetValue("XYPlotLineStyle", s);
        // do not use settings lie s.XYPlotScatterStyle= here, since the XYPlotScatterStyle is cloned, but maybe not fully deserialized here!!!
        s.m_ScatterStyle = (ScatterPlotStyle)info.GetValue("XYPlotScatterStyle", s);
        s.m_LineSymbolGap = info.GetBoolean("LineSymbolGap");

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYLineScatterPlotStyle", 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("Serialization of old versions not supported");
        /*
				XYLineScatterPlotStyle s = (XYLineScatterPlotStyle)obj;
				info.AddValue("XYPlotLineStyle",s.m_LineStyle);
				info.AddValue("XYPlotScatterStyle",s.m_ScatterStyle);
				info.AddValue("LineSymbolGap",s.m_LineSymbolGap);
				info.AddValue("LabelStyle",s.m_LabelStyle);      // new in this version
				*/
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYLineScatterPlotStyle s = null != o ? (XYLineScatterPlotStyle)o : new XYLineScatterPlotStyle(info);
        // do not use settings lie s.XYPlotLineStyle= here, since the XYPlotLineStyle is cloned, but maybe not fully deserialized here!!!
        s.m_LineStyle = (LinePlotStyle)info.GetValue("XYPlotLineStyle", s);
        // do not use settings lie s.XYPlotScatterStyle= here, since the XYPlotScatterStyle is cloned, but maybe not fully deserialized here!!!
        s.m_ScatterStyle = (ScatterPlotStyle)info.GetValue("XYPlotScatterStyle", s);
        s.m_LineSymbolGap = info.GetBoolean("LineSymbolGap");
        s.m_LabelStyle = (LabelPlotStyle)info.GetValue("XYPlotLabelStyle", s); // new in this version

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYLineScatterPlotStyle", 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("Serialization of old versions not supported");
        /*
				XYLineScatterPlotStyle s = (XYLineScatterPlotStyle)obj;
				info.AddValue("XYPlotLineStyle",s.m_LineStyle);
				info.AddValue("XYPlotScatterStyle",s.m_ScatterStyle);
				info.AddValue("LineSymbolGap",s.m_LineSymbolGap);

				int nCount = null==s.m_LabelStyle ? 0 : 1;
				info.CreateArray("OptionalStyles",nCount);
				if(null!=s.m_LabelStyle)  info.AddValue("LabelStyle",s.m_LabelStyle); // new in this version
				info.CommitArray();
				*/
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYLineScatterPlotStyle s = null != o ? (XYLineScatterPlotStyle)o : new XYLineScatterPlotStyle(info);
        // do not use settings lie s.XYPlotLineStyle= here, since the XYPlotLineStyle is cloned, but maybe not fully deserialized here!!!
        s.m_LineStyle = (LinePlotStyle)info.GetValue("XYPlotLineStyle", s);
        // do not use settings lie s.XYPlotScatterStyle= here, since the XYPlotScatterStyle is cloned, but maybe not fully deserialized here!!!
        s.m_ScatterStyle = (ScatterPlotStyle)info.GetValue("XYPlotScatterStyle", s);
        s.m_LineSymbolGap = info.GetBoolean("LineSymbolGap");

        int nCount = info.OpenArray(); // OptionalStyles

        if (nCount == 1)
          s.m_LabelStyle = (LabelPlotStyle)info.GetValue("LabelStyle", s); // new in this version

        info.CloseArray(nCount); // OptionalStyles

        return s;
      }
    }

    #endregion Serialization

    public XYLineScatterPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)

    {
      var kind = LineScatterPlotStyleKind.LineAndScatter;

      if (0 != (kind & LineScatterPlotStyleKind.Line))
        m_LineStyle = new LinePlotStyle(info, oldDeserializationRequiresFullConstruction: true);

      if (0 != (kind & LineScatterPlotStyleKind.Scatter))
        m_ScatterStyle = new ScatterPlotStyle(info, oldDeserializationRequiresFullConstruction: true);

      m_LineSymbolGap = kind == LineScatterPlotStyleKind.LineAndScatter;
    }

    /*
		public XYLineScatterPlotStyle(XYColumnPlotData pa)
		{
			this.m_LineStyle = new LinePlotStyle((Altaxo.Main.Properties.IReadOnlyPropertyBag)null);
			this.m_ScatterStyle = new ScatterPlotStyle((Altaxo.Main.Properties.IReadOnlyPropertyBag)null);
			// this.m_PlotAssociation = pa;
			this.m_LineSymbolGap = true;
		}
		*/

    public NamedColor Color
    {
      get
      {
        if (m_LineStyle != null)
          return m_LineStyle.LinePen.Color;
        if (m_ScatterStyle != null)
          return m_ScatterStyle.Color;
        else
          return NamedColors.Black;
      }
      set
      {
        if (m_LineStyle != null)
          m_LineStyle.LinePen.Color = value;
        if (m_ScatterStyle != null)
          m_ScatterStyle.Color = value;
      }
    }

    public LinePlotStyle XYPlotLineStyle
    {
      get { return m_LineStyle; }
    }

    public ScatterPlotStyle ScatterStyle
    {
      get { return m_ScatterStyle; }
    }

    public LabelPlotStyle XYPlotLabelStyle
    {
      get { return m_LabelStyle; }
    }

    public float SymbolSize
    {
      get
      {
        return (float)(null == m_ScatterStyle ? 0 : m_ScatterStyle.SymbolSize);
      }
    }

    public bool LineSymbolGap
    {
      get { return m_LineSymbolGap; }
    }
  } // end of class XYLineScatterPlotStyle

#endif
}
