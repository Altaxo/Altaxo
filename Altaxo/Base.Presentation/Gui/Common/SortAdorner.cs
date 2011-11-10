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
#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Documents;
 

namespace Altaxo.Gui.Common
{
	public class SortAdorner : Adorner
	{
		private readonly static Geometry _AscGeometry =
			 Geometry.Parse("M 0,0 L 10,0 L 5,5 Z");

		private readonly static Geometry _DescGeometry =
				Geometry.Parse("M 0,5 L 10,5 L 5,0 Z");

		public ListSortDirection Direction { get; private set; }

		public bool IsSecondaryAdorner { get; private set; }

		public SortAdorner(UIElement element, ListSortDirection dir, bool isSecondaryAdorner)
			: base(element)
		{
			Direction = dir;
			IsSecondaryAdorner = isSecondaryAdorner;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			if (AdornedElement.RenderSize.Width < 20)
				return;

			drawingContext.PushTransform(
					 new TranslateTransform(
						 AdornedElement.RenderSize.Width - 15,
						(AdornedElement.RenderSize.Height - 5) / 2));

			if (IsSecondaryAdorner)
			{
				drawingContext.DrawGeometry(null, new Pen(Brushes.Black,0.5),
						Direction == ListSortDirection.Ascending ?
							_AscGeometry : _DescGeometry);
			}
			else
			{
				drawingContext.DrawGeometry(Brushes.Black, null,
						Direction == ListSortDirection.Ascending ?
							_AscGeometry : _DescGeometry);
			}

			drawingContext.Pop();
		}
	}
}
