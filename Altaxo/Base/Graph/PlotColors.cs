#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using System.Text;
using System.Drawing;

namespace Altaxo.Graph
{
  public class PlotColor
  {
    System.Drawing.Color _color;
    string _name;

    public PlotColor(System.Drawing.Color color, string name)
    {
      _color = color;
      _name = name;
    }

    public System.Drawing.Color Color
    {
      get { return _color; }
    }
   
    public string Name
    {
      get { return _name; }
    }

    public static implicit operator System.Drawing.Color(PlotColor c)
    {
      return c._color;
    }

    public override bool Equals(object obj)
    {
      if(obj is PlotColor)
        return this._color == ((PlotColor)obj)._color;

      return false;
    }

    public override int GetHashCode()
    {
      return this._color.GetHashCode();
    }
  }

  public class PlotColors : System.Collections.CollectionBase
  {

    static PlotColors _instance = new PlotColors();
    public static PlotColors Colors
    {
      get
      {
        return _instance;
      }
    }

    private PlotColors()
    {
      Add(new PlotColor(System.Drawing.Color.Black,"Black"));
      Add(new PlotColor(System.Drawing.Color.Red, "Red"));
      Add(new PlotColor(System.Drawing.Color.Green, "Green"));
      Add(new PlotColor(System.Drawing.Color.Blue, "Blue"));
      Add(new PlotColor(System.Drawing.Color.Magenta, "Magenta"));
      Add(new PlotColor(System.Drawing.Color.Yellow, "Yellow"));
      Add(new PlotColor(System.Drawing.Color.Coral, "Coral"));
    }

    public PlotColor this[int i]
    {
      get
      {
        return (PlotColor)InnerList[i];
      }
      set
      {
        InnerList[i] = value;
      }
    }

    public void Add(PlotColor c)
    {
      InnerList.Add(c);
    }

    public int IndexOf(Color c)
    {
      for (int i = 0; i < Count; i++)
      {
        if (c.ToArgb() == this[i].Color.ToArgb())
        {
          return i;
        }
      }
      return -1;
    }

    public PlotColor GetPlotColor(Color c)
    {
      int i = IndexOf(c);
      return i<0 ? null : this[i];
    }

    public string GetPlotColorName(Color c)
    {
      int i = IndexOf(c);
      return i < 0 ? null : this[i].Name;
    }

    public PlotColor GetNextPlotColor(Color c)
    {
      return GetNextPlotColor(c, 1);
    }

    public PlotColor GetNextPlotColor(Color c, int step)
    {
      int wraps;
      return GetNextPlotColor(c, step, out wraps);
    }

    public PlotColor GetNextPlotColor(Color c, int step, out int wraps)
    {
      int i = IndexOf(c);
      if (i >= 0)
      {
        wraps = Calc.BasicFunctions.NumberOfWraps(Count, i, step);
        return this[Calc.BasicFunctions.PMod(i + step, Count)];

      }
      else
      {
        // default if the color was not found
        wraps = 0;
        return this[0];
      }
    }


  }
}
