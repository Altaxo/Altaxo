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
using Altaxo.Graph.Axes;
using Altaxo.Data;

namespace Altaxo.Graph.AxisLabeling
{
  

  public interface ILabelFormatting
  {
    SizeF MeasureItem(Graphics g, System.Drawing.Font font, System.Drawing.StringFormat strfmt, Data.AltaxoVariant mtick, PointF morg);
    void DrawItem(Graphics g, System.Drawing.Font font, System.Drawing.StringFormat strfmt, AltaxoVariant item, PointF morg);

    IMeasuredLabelItem[] GetMeasuredItems(Graphics g, System.Drawing.Font font, System.Drawing.StringFormat strfmt, AltaxoVariant[] items);
  }
}
