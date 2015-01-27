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
using Altaxo.Serialization.Ascii;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Serialization.Ascii
{
	public interface IFixedColumnWidthWithTabSeparationStrategyView
	{
		int TabSize { get; set; }

		ObservableCollection<Boxed<int>> StartPositions { set; }
	}

	[ExpectedTypeOfView(typeof(IFixedColumnWidthWithTabSeparationStrategyView))]
	[UserControllerForObject(typeof(FixedColumnWidthWithTabSeparationStrategy))]
	public class FixedColumnWidthWithTabSeparationStrategyController : MVCANControllerEditOriginalDocBase<FixedColumnWidthWithTabSeparationStrategy, IFixedColumnWidthWithTabSeparationStrategyView>
	{
		private ObservableCollection<Boxed<int>> _positions;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		public override void Dispose(bool isDisposing)
		{
			_positions = null;
			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_positions = new ObservableCollection<Boxed<int>>(Boxed<int>.ToBoxedItems(_doc.StartPositions));
			}

			if (null != _view)
			{
				_view.TabSize = _doc.TabSize;
				_view.StartPositions = _positions;
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc.TabSize = _view.TabSize;

			var resList = new List<int>(Boxed<int>.ToUnboxedItems(_positions));
			if (FixedColumnWidthWithoutTabSeparationStrategyController.MakeColumnStartListCompliant(resList))
			{
				_positions.Clear();
				Boxed<int>.AddRange(_positions, resList);
				Current.Gui.InfoMessageBox("Start positions were adjusted. Please check the result.");
				return false;
			}
			_doc.StartPositions = resList.ToArray();

			return ApplyEnd(true, disposeController);
		}
	}
}