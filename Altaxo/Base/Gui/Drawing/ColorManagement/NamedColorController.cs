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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Drawing.ColorManagement
{
	public interface INamedColorView
	{
		void InitializeSubViews(IEnumerable<Tuple<string, object>> tabsNamesAndViews);

		void SetOldColor(AxoColor oldColor);

		void SetNewColor(AxoColor oldColor);

		void SetColorName(string name);
	}

	[ExpectedTypeOfView(typeof(INamedColorView))]
	public class NamedColorController : MVCANControllerEditImmutableDocBase<NamedColor, INamedColorView>
	{
		private ColorModelController _subControllerColorModel;

		private NamedColor _initialColor;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_subControllerColorModel, () => _subControllerColorModel = null);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_initialColor = _doc;

				_subControllerColorModel = new ColorModelController();
				_subControllerColorModel.InitializeDocument(_doc.Color);
				_subControllerColorModel.MadeDirty += EhController_Dirty;
			}
			if (null != _view)
			{
				_view.InitializeSubViews(GetTabNamesAndViews());
				_view.SetOldColor(_initialColor);
				_view.SetNewColor(_doc);
			}
		}

		private IEnumerable<Tuple<string, object>> GetTabNamesAndViews()
		{
			if (null != _subControllerColorModel)
			{
				if (_subControllerColorModel.ViewObject == null)
					Current.Gui.FindAndAttachControlTo(_subControllerColorModel);
				if (_subControllerColorModel.ViewObject != null)
					yield return new Tuple<string, object>("Models", _subControllerColorModel.ViewObject);
			}
		}

		public override bool Apply(bool disposeController)
		{
			return ApplyEnd(true, disposeController);
		}

		private void EhController_Dirty(IMVCANDController ctrl)
		{
			var color = (AxoColor)ctrl.ModelObject;
			_doc = new NamedColor(color);

			_view?.SetNewColor(color);

			_view.SetColorName(_doc.Name);
		}
	}
}