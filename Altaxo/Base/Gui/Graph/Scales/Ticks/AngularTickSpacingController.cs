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
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
	#region Interfaces

	public interface IAngularTickSpacingView
	{
		bool UsePositiveNegativeValues { get; set; }

		SelectableListNodeList MajorTicks { set; }

		SelectableListNodeList MinorTicks { set; }

		event EventHandler MajorTicksChanged;
	}

	#endregion Interfaces

	[UserControllerForObject(typeof(AngularTickSpacing), 200)]
	[ExpectedTypeOfView(typeof(IAngularTickSpacingView))]
	public class AngularScaleController : MVCANControllerBase<AngularTickSpacing, IAngularTickSpacingView>
	{
		private SelectableListNodeList _majorTickList;
		private SelectableListNodeList _minorTickList;
		private int _tempMajorDivider;

		protected override void Initialize(bool initDoc)
		{
			if (initDoc)
			{
				_tempMajorDivider = _doc.MajorTickDivider;
				BuildMajorTickList();
				BuildMinorTickList();
			}

			if (_view != null)
			{
				_view.UsePositiveNegativeValues = _doc.UseSignedValues;
				_view.MajorTicks = _majorTickList;
				_view.MinorTicks = _minorTickList;
			}
		}

		private void BuildMajorTickList()
		{
			int[] possibleDividers = _doc.GetPossibleDividers();
			int selected = _tempMajorDivider;
			_majorTickList = new SelectableListNodeList();
			foreach (int div in possibleDividers)
			{
				_majorTickList.Add(new SelectableListNode((360.0 / div).ToString() + "°", div, div == selected));
			}
		}

		private void BuildMinorTickList()
		{
			int[] possibleDividers = _doc.GetPossibleDividers();
			int selected = _doc.MinorTickDivider;
			if (selected < _tempMajorDivider)
				selected = _tempMajorDivider;
			_minorTickList = new SelectableListNodeList();
			foreach (int div in possibleDividers)
			{
				if (div >= _tempMajorDivider && 0 == (div % _tempMajorDivider))
					_minorTickList.Add(new SelectableListNode((360.0 / div).ToString() + "°", div, div == selected));
			}
		}

		private void EhMajorTicksChanged(object sender, EventArgs e)
		{
			_tempMajorDivider = (int)_majorTickList.FirstSelectedNode.Tag;
			BuildMinorTickList();
			_view.MinorTicks = _minorTickList;
		}

		protected override void AttachView()
		{
			_view.MajorTicksChanged += EhMajorTicksChanged;
		}

		protected override void DetachView()
		{
			_view.MajorTicksChanged -= EhMajorTicksChanged;
		}

		public override bool Apply(bool disposeController)
		{
			_doc.UseSignedValues = _view.UsePositiveNegativeValues;
			_doc.MajorTickDivider = _tempMajorDivider;
			_doc.MinorTickDivider = (int)_minorTickList.FirstSelectedNode.Tag;

			CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}