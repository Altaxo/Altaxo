#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

using Altaxo.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.ColorManagement
{
	public interface INamedColorChoiceView
	{
		bool ShowPlotColorsOnly { set; }

		NamedColor SelectedColor { get; set; }
	}

	[ExpectedTypeOfView(typeof(INamedColorChoiceView))]
	public class NamedColorChoiceController : MVCANControllerEditImmutableDocBase<NamedColor, INamedColorChoiceView>
	{
		public bool ShowPlotColorsOnly { get; set; }

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
			}
			if (null != _view)
			{
				_view.ShowPlotColorsOnly = ShowPlotColorsOnly;
				_view.SelectedColor = _doc;
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc = _view.SelectedColor;

			if (ShowPlotColorsOnly && _doc.ParentColorSet == null)
			{
				Current.Gui.ErrorMessageBox("You have chosen a custom color, but a plot color is required. Please choose one of the defined plot colors.", "Custom colors not allowed");
				return false;
			}

			if (_doc.ParentColorSet == null)
			{
				if (!Current.Gui.YesNoMessageBox(
					"You have chosen a custom color. This is not recommended, because a custom color does not belong to a color set.\r\n" +
					"The recommended way is to define a new color set which contains the color of your choice.\r\n" +
					"Do you want to use this color nevertheless?", "Warning", false))
					return false;
			}

			return ApplyEnd(true, disposeController);
		}
	}
}