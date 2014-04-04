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

using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Axis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	public interface IAxisLineStyleView
	{
		bool ShowLine { get; set; }

		PenX LinePen { get; set; }

		PenX MajorPen { get; set; }

		PenX MinorPen { get; set; }

		double MajorTickLength { get; set; }

		double MinorTickLength { get; set; }

		SelectableListNodeList MajorPenTicks { get; set; }

		SelectableListNodeList MinorPenTicks { get; set; }
	}

	#endregion Interfaces

	[UserControllerForObject(typeof(AxisLineStyle))]
	[ExpectedTypeOfView(typeof(IAxisLineStyleView))]
	public class AxisLineStyleController : IMVCAController
	{
		private AxisLineStyle _doc;
		private IAxisLineStyleView _view;

		public AxisLineStyleController(AxisLineStyle doc)
		{
			_doc = (AxisLineStyle)doc.Clone();
			Initialize(true);
		}

		#region IMVCController Members

		private void Initialize(bool bInit)
		{
			if (_view != null)
			{
				_view.ShowLine = true;

				_view.LinePen = _doc.AxisPen;
				_view.MajorPen = _doc.MajorPen;
				_view.MinorPen = _doc.MinorPen;

				_view.MajorTickLength = _doc.MajorTickLength;
				_view.MinorTickLength = _doc.MinorTickLength;

				var list = new List<SelectableListNode>();
				if (_doc.CachedAxisInformation != null)
				{
					list.Add(new SelectableListNode(_doc.CachedAxisInformation.NameOfFirstDownSide, 0, _doc.FirstDownMajorTicks));
					list.Add(new SelectableListNode(_doc.CachedAxisInformation.NameOfFirstUpSide, 1, _doc.FirstUpMajorTicks));
				}
				list.Sort((x, y) => string.Compare(x.Text, y.Text));
				_view.MajorPenTicks = new SelectableListNodeList(list);

				list = new List<SelectableListNode>();
				if (_doc.CachedAxisInformation != null)
				{
					list.Add(new SelectableListNode(_doc.CachedAxisInformation.NameOfFirstDownSide, 0, _doc.FirstDownMinorTicks));
					list.Add(new SelectableListNode(_doc.CachedAxisInformation.NameOfFirstUpSide, 1, _doc.FirstUpMinorTicks));
				}
				list.Sort((x, y) => string.Compare(x.Text, y.Text));
				_view.MinorPenTicks = new SelectableListNodeList(list);
			}
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IAxisLineStyleView;
				Initialize(false);
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply()
		{
			_doc.AxisPen = _view.LinePen;
			_doc.MajorPen = _view.MajorPen;
			_doc.MinorPen = _view.MinorPen;
			_doc.MajorTickLength = (float)_view.MajorTickLength;
			_doc.MinorTickLength = (float)_view.MinorTickLength;

			SelectableListNodeList list;
			list = _view.MajorPenTicks;
			foreach (var item in list)
			{
				switch ((int)item.Tag)
				{
					case 0:
						_doc.FirstDownMajorTicks = item.IsSelected;
						break;

					case 1:
						_doc.FirstUpMajorTicks = item.IsSelected;
						break;
				}
			}

			list = _view.MinorPenTicks;
			foreach (var item in list)
			{
				switch ((int)item.Tag)
				{
					case 0:
						_doc.FirstDownMinorTicks = item.IsSelected;
						break;

					case 1:
						_doc.FirstUpMinorTicks = item.IsSelected;
						break;
				}
			}

			return true;
		}

		#endregion IApplyController Members
	}
}