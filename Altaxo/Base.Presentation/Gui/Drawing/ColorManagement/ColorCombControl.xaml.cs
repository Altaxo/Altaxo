#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  public partial class ColorCombControl : UserControl
  {
    private Color _selectedColor;

    //
    // Initialization

    public ColorCombControl()
    {
      InitializeComponent();

      InitializeChildren();
    }

    //
    // Interface

    public Color SelectedColor
    {
      get
      { return _selectedColor; }
      set
      { _selectedColor = value; }
    }

    public event EventHandler<ColorEventArgs> ColorSelected;

    //
    // Implementation

    private const int MaxGenerations = 6; // 127 tiles

    private HexagonButton _rootCell;

    private void InitializeChildren()
    {
      // Define honeycomb of 127 hexagons, starting in the center of the canvas.
      _rootCell = new HexagonButton(FindResource("HexagonButtonStyle") as Style);
      Canvas.SetLeft(_rootCell, canvas1.Width / 2);
      Canvas.SetTop(_rootCell, canvas1.Height / 2);
      canvas1.Children.Add(_rootCell);

      _rootCell.Click += new RoutedEventHandler(cell_Clicked);

      // Expand outward (recursive loop).
      InitializeChildrenR(_rootCell, 1);

      // Apply nominal color gradients.
      CascadeChildColors();
    }

    private void InitializeChildrenR(HexagonButton parent, int generation)
    {
      if (generation > MaxGenerations)
        return;

      // Create up to 6 surrounding nodes.
      for (int i = 0; i < 6; ++i)
      {
        if (parent.Neighbors[i] == null)
        {
          var cell = new HexagonButton(FindResource("HexagonButtonStyle") as Style);
          double dx = Canvas.GetLeft(parent) + HexagonButton.Offset * Math.Cos(i * Math.PI / 3d);
          double dy = Canvas.GetTop(parent) + HexagonButton.Offset * Math.Sin(i * Math.PI / 3d);
          Canvas.SetLeft(cell, dx);
          Canvas.SetTop(cell, dy);
          canvas1.Children.Add(cell);
          parent.Neighbors[i] = cell;

          cell.Click += new RoutedEventHandler(cell_Clicked);
        }
      }

      // Set the cross-pointers on the 6 surrounding nodes.
      for (int i = 0; i < 6; ++i)
      {
        HexagonButton child = parent.Neighbors[i];
        if (child != null)
        {
          int ip3 = (i + 3) % 6;
          child.Neighbors[ip3] = parent;

          int ip1 = (i + 1) % 6;
          int ip2 = (i + 2) % 6;
          int im1 = (i + 5) % 6;
          int im2 = (i + 4) % 6;
          child.Neighbors[ip2] = parent.Neighbors[ip1];
          child.Neighbors[im2] = parent.Neighbors[im1];
        }
      }

      // Recurse into each of the 6 surrounding nodes.
      for (int i = 0; i < 6; ++i)
      {
        InitializeChildrenR(parent.Neighbors[i], generation + 1);
      }
    }

    private void CascadeChildColors()
    {
      _rootCell.NominalColor = Color.FromScRgb(1f, 1f, 1f, 1f);
      CascadeChildColorsR(_rootCell);

      foreach (HexagonButton h in canvas1.Children)
        h.Visited = false;
    }

    private void CascadeChildColorsR(HexagonButton parent)
    {
      float delta = 1f / MaxGenerations;
      float ceiling = 0.99f;

      var visitedNodes =
          new System.Collections.Generic.List<HexagonButton>(6);

      for (int i = 0; i < 6; ++i)
      {
        HexagonButton child = parent.Neighbors[i];
        if (child != null && !child.Visited)
        {
          Color c = parent.NominalColor;
          switch (i)
          {
            case 0: // increase cyan; else reduce red
              if (c.ScG < ceiling && c.ScB < ceiling)
              {
                c.ScG = Math.Min(Math.Max(0f, c.ScG + delta), 1f);
                c.ScB = Math.Min(Math.Max(0f, c.ScB + delta), 1f);
              }
              else
              {
                c.ScR = Math.Min(Math.Max(0f, c.ScR - delta), 1f);
              }
              break;

            case 1: // increase blue; else reduce yellow
              if (c.ScB < ceiling)
              {
                c.ScB = Math.Min(Math.Max(0f, c.ScB + delta), 1f);
              }
              else
              {
                c.ScR = Math.Min(Math.Max(0f, c.ScR - delta), 1f);
                c.ScG = Math.Min(Math.Max(0f, c.ScG - delta), 1f);
              }
              break;

            case 2: // increase magenta; else reduce green
              if (c.ScR < ceiling && c.ScB < ceiling)
              {
                c.ScR = Math.Min(Math.Max(0f, c.ScR + delta), 1f);
                c.ScB = Math.Min(Math.Max(0f, c.ScB + delta), 1f);
              }
              else
              {
                c.ScG = Math.Min(Math.Max(0f, c.ScG - delta), 1f);
              }
              break;

            case 3: // increase red; else reduce cyan
              if (c.ScR < ceiling)
              {
                c.ScR = Math.Min(Math.Max(0f, c.ScR + delta), 1f);
              }
              else
              {
                c.ScG = Math.Min(Math.Max(0f, c.ScG - delta), 1f);
                c.ScB = Math.Min(Math.Max(0f, c.ScB - delta), 1f);
              }
              break;

            case 4: // increase yellow; else reduce blue
              if (c.ScR < ceiling && c.ScG < ceiling)
              {
                c.ScR = Math.Min(Math.Max(0f, c.ScR + delta), 1f);
                c.ScG = Math.Min(Math.Max(0f, c.ScG + delta), 1f);
              }
              else
              {
                c.ScB = Math.Min(Math.Max(0f, c.ScB - delta), 1f);
              }
              break;

            case 5: // increase green; else reduce magenta
              if (c.ScG < ceiling)
              {
                c.ScG = Math.Min(Math.Max(0f, c.ScG + delta), 1f);
              }
              else
              {
                c.ScR = Math.Min(Math.Max(0f, c.ScR - delta), 1f);
                c.ScB = Math.Min(Math.Max(0f, c.ScB - delta), 1f);
              }
              break;
          }
          child.NominalColor = c;
          child.Visited = true;
          visitedNodes.Add(child);
        }
      }

      parent.Visited = true; // ensures root node not over-visited
      foreach (HexagonButton child in visitedNodes)
      {
        CascadeChildColorsR(child);
      }
    }

    private void cell_Clicked(object sender, RoutedEventArgs e)
    {
      var cell = sender as HexagonButton;

      _selectedColor = cell.NominalColor;
      FireColorSelected();
    }

    private void FireColorSelected()
    {
      if (ColorSelected != null)
        ColorSelected(this, new ColorEventArgs(_selectedColor));
    }
  }

  public class HexagonButton : Button
  {
    public const double Radius = 12d; // matches pathgeometry coded in XAML
    public static readonly double Offset = Radius * 2 * Math.Cos(30d * Math.PI / 180d) + 1.5;

    //
    // Interface

    public HexagonButton(Style style)
    {
      Style = style;

      _neighbors = new HexagonButton[6];
      _nominalColor = Color.FromScRgb(1f, 1f, 1f, 1f);
    }

    public Color NominalColor
    {
      get
      { return _nominalColor; }
      set
      {
        _nominalColor = value;
        Background = ConstructBackgroundGradient(0.5f);
      }
    }

    //
    // Internals

    internal HexagonButton[] Neighbors
    {
      get { return _neighbors; }
    }

    internal bool Visited;

    //
    // Implementation

    private HexagonButton[] _neighbors;
    private Color _nominalColor;

    private Brush ConstructBackgroundGradient(float inflection)
    {
      Color a = NominalColor, z = Colors.Black;
      var m = Color.FromScRgb(a.ScA / 2, a.ScR / 2, a.ScG / 2, a.ScB / 2);

      var lgb = new LinearGradientBrush
      {
        ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation,
        StartPoint = new Point(0.5, 0),
        EndPoint = new Point(0, 0.75)
      };
      lgb.GradientStops.Add(new GradientStop(a, 0d));
      lgb.GradientStops.Add(new GradientStop(m, 0.8 * (1d - inflection)));
      lgb.GradientStops.Add(new GradientStop(z, 1d));
      return lgb;
    }

    protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
    {
      base.OnMouseEnter(e);
      Background = ConstructBackgroundGradient(0.2f);
    }

    protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
    {
      base.OnMouseLeave(e);
      Background = ConstructBackgroundGradient(0.5f);
    }

    protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnIsPressedChanged(e);
      if (IsPressed)
      {
        Background = ConstructBackgroundGradient(0.8f);
      }
      else
      {
        Background = ConstructBackgroundGradient(0.5f);
      }
    }
  }

  public class ColorEventArgs : EventArgs
  {
    private Color _color;

    public ColorEventArgs()
    { }

    public ColorEventArgs(Color c)
    {
      _color = c;
    }

    public Color Color
    {
      get
      { return _color; }
      set
      { _color = value; }
    }
  }
}
