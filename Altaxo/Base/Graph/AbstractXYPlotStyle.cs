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


namespace Altaxo.Graph
{
  /// <summary>
  /// Summary description for AbstractXYPlotStyle.
  /// </summary>
  public abstract class AbstractXYPlotStyle : ICloneable, Main.IDocumentNode
  {
    protected object m_Parent;

    public static Color[] PlotColors = 
      {
        Color.Black,
        Color.Red,
        Color.Green,
        Color.Blue,
        Color.Magenta,
        Color.Yellow,
        Color.Coral
      };


    public static string GetPlotColorName(Color c)
    {
      for(int i=0;i<PlotColors.Length;i++)
      {
        if(c.ToArgb()==PlotColors[i].ToArgb())
        {
          string name = PlotColors[i].ToString();
          return name.Substring(7,name.Length-8);
        }
      }
      return null;
    }


    public static Color GetNextPlotColor(Color c)
    {
      for(int i=0;i<PlotColors.Length;i++)
      {
        // !!!todo!!! more advanced: find the color with the closest match to a plotcolor
        // and use the next color then

        // currently implemented: find the color, if found use the next color
        // if not found, use the first plot color
        if(c.ToArgb()==PlotColors[i].ToArgb())
          return (i+1)<PlotColors.Length ? PlotColors[i+1] : PlotColors[0];
      }

      // default if the color was not found
      return PlotColors[0];
    }

    public abstract object Clone();
    public abstract void Paint(Graphics g, IPlotArea gl, object plotObject); // plots the curve with the choosen style
    public abstract SizeF PaintSymbol(Graphics g, PointF atPosition, float width); // draws a symbol that represents the style at position (0,0)

    // public abstract XYColumnPlotData XYColumnPlotData  { get; set; } 


    public abstract void SetToNextStyle(AbstractXYPlotStyle ps, PlotGroupStyle style);

    public abstract System.Drawing.Color Color { get; set; }
    
    public abstract XYPlotLineStyle     XYPlotLineStyle { get; set; }
    
    public abstract XYPlotScatterStyle    XYPlotScatterStyle { get; set; }

    public virtual float SymbolSize 
    {
      get { return 0; }
      set { }
    }
    public virtual bool LineSymbolGap 
    {
      get { return false; }
      set { }
    }
    public virtual IHitTestObject HitTest(IPlotArea layer, object plotObject, PointF hitpoint)
    {
      return null;
    }
    #region IDocumentNode Members

    public object ParentObject
    {
      get { return m_Parent;  }
      set { m_Parent = value; }
    }

    public string Name
    {
      get
      {
        Main.INamedObjectCollection noc = ParentObject as Main.INamedObjectCollection;
        return noc==null ? null : noc.GetNameOfChildObject(this);
      }
    }

    #endregion
  } // end of interface AbstractXYPlotStyle
}



