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

using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
	public interface ICardinalSplinePointsView
	{
		double Tension { get; set; }

		List<PointD2D> CurvePoints { get; set; }

		event Action CurvePointsCopyTriggered;

		event Action CurvePointsPasteTriggered;
	}

	public class CardinalSplinePointsController
	{
		private ICardinalSplinePointsView _view;

		public CardinalSplinePointsController(ICardinalSplinePointsView view, List<PointD2D> curvePoints, double tension)
		{
			_view = view;

			_view.CurvePointsCopyTriggered += new Action(EhCurvePointsCopyTriggered);
			_view.CurvePointsPasteTriggered += new Action(EhCurvePointsPasteTriggered);

			_view.Tension = tension;
			_view.CurvePoints = curvePoints;
		}

		public bool Apply(out List<PointD2D> curvePoints, out double tension)
		{
			curvePoints = _view.CurvePoints;
			tension = _view.Tension;
			return true;
		}

		private void EhCurvePointsPasteTriggered()
		{
			Altaxo.Data.DataTable table = Altaxo.Worksheet.Commands.EditCommands.GetTableFromClipboard();
			if (null == table)
				return;
			Altaxo.Data.DoubleColumn xcol = null;
			Altaxo.Data.DoubleColumn ycol = null;
			// Find the first column that contains numeric values
			int i;
			for (i = 0; i < table.DataColumnCount; i++)
			{
				if (table[i] is Altaxo.Data.DoubleColumn)
				{
					xcol = table[i] as Altaxo.Data.DoubleColumn;
					break;
				}
			}
			for (i = i + 1; i < table.DataColumnCount; i++)
			{
				if (table[i] is Altaxo.Data.DoubleColumn)
				{
					ycol = table[i] as Altaxo.Data.DoubleColumn;
					break;
				}
			}

			if (!(xcol != null && ycol != null))
				return;

			int len = Math.Min(xcol.Count, ycol.Count);
			var list = new List<PointD2D>();
			for (i = 0; i < len; i++)
			{
				list.Add(new PointD2D(
					new DimensionfulQuantity(xcol[i], PositionEnvironment.Instance.DefaultUnit).AsValueIn(Units.Length.Point.Instance),
					new DimensionfulQuantity(ycol[i], PositionEnvironment.Instance.DefaultUnit).AsValueIn(Units.Length.Point.Instance)
					));
			}

			_view.CurvePoints = list;
		}

		private void EhCurvePointsCopyTriggered()
		{
			var points = _view.CurvePoints;

			var dao = Current.Gui.GetNewClipboardDataObject();
			Altaxo.Data.DoubleColumn xcol = new Altaxo.Data.DoubleColumn();
			Altaxo.Data.DoubleColumn ycol = new Altaxo.Data.DoubleColumn();
			for (int i = 0; i < points.Count; i++)
			{
				xcol[i] = new DimensionfulQuantity(points[i].X, Units.Length.Point.Instance).AsValueIn(PositionEnvironment.Instance.DefaultUnit);
				ycol[i] = new DimensionfulQuantity(points[i].Y, Units.Length.Point.Instance).AsValueIn(PositionEnvironment.Instance.DefaultUnit);
			}

			Altaxo.Data.DataTable tb = new Altaxo.Data.DataTable();
			tb.DataColumns.Add(xcol, "XPosition", Altaxo.Data.ColumnKind.V, 0);
			tb.DataColumns.Add(ycol, "YPosition", Altaxo.Data.ColumnKind.V, 0);
			Altaxo.Worksheet.Commands.EditCommands.WriteAsciiToClipBoardIfDataCellsSelected(
				tb, new Altaxo.Collections.AscendingIntegerCollection(),
				new Altaxo.Collections.AscendingIntegerCollection(),
				new Altaxo.Collections.AscendingIntegerCollection(),
				dao);
			Current.Gui.SetClipboardDataObject(dao, true);
		}
	}
}