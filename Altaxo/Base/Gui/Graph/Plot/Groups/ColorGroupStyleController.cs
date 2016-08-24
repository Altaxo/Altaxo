#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Gui.Drawing;
using Altaxo.Gui.Drawing.ColorManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Plot.Groups
{
	[ExpectedTypeOfView(typeof(IStyleListView))]
	[UserControllerForObject(typeof(ColorGroupStyle))]
	public class ColorGroupStyleController : MVCANControllerEditOriginalDocBase<ColorGroupStyle, IStyleListView>
	{
		private ColorSetController _listController;

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_listController = new ColorSetController();
				_listController.InitializeDocument(_doc.ListOfValues);
			}

			if (null != _view)
			{
				_listController.ViewObject = _view;
			}
		}

		public override bool Apply(bool disposeController)
		{
			if (!_listController.Apply(disposeController))
				return ApplyEnd(false, disposeController);

			var colorSet = (IColorSet)_listController.ModelObject;
			if (object.ReferenceEquals(colorSet, ColorSetManager.Instance.BuiltinKnownColors))
			{
				Current.Gui.ErrorMessageBox(string.Format("The color set '{0}' is not admitted to be used as plot color set. Please choose another color set, or derive a new color set from '{0}'.", ColorSetManager.Instance.BuiltinKnownColors.Name), "ColorSet not admitted");
				return ApplyEnd(false, disposeController);
			}

			_doc.ListOfValues = colorSet;

			return ApplyEnd(true, disposeController);
		}

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_listController, () => _listController = null);
		}
	}
}