#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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

using Altaxo.Graph.Gdi.Plot.ColorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.ColorProvider
{
	public interface IColorProviderARGBGradientView
	{
		IColorProviderBaseView BaseView { get; }

		double Red0 { get; set; }

		double Red1 { get; set; }

		double Green0 { get; set; }

		double Green1 { get; set; }

		double Blue0 { get; set; }

		double Blue1 { get; set; }

		double Opaqueness0 { get; set; }

		double Opaqueness1 { get; set; }

		event Action ChoiceChanged;
	}

	[ExpectedTypeOfView(typeof(IColorProviderARGBGradientView))]
	[UserControllerForObject(typeof(ColorProviderARGBGradient), 110)]
	public class ColorProviderARGBGradientController : MVCANDControllerEditImmutableDocBase<ColorProviderARGBGradient, IColorProviderARGBGradientView>
	{
		private ColorProviderBaseController _baseController;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_baseController, () => _baseController = null);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_baseController = new ColorProviderBaseController() { UseDocumentCopy = UseDocument.Directly };
				_baseController.InitializeDocument(_doc);
				_baseController.MadeDirty += EhBaseControllerChanged;
			}
			if (null != _view)
			{
				_baseController.ViewObject = _view.BaseView;

				_view.Red0 = _doc.Red0;
				_view.Red1 = _doc.Red1;
				_view.Green0 = _doc.Green0;
				_view.Green1 = _doc.Green1;
				_view.Blue0 = _doc.Blue0;
				_view.Blue1 = _doc.Blue1;
				_view.Opaqueness0 = _doc.Opaqueness0;
				_view.Opaqueness1 = _doc.Opaqueness1;
			}
		}

		public override bool Apply(bool disposeController)
		{
			if (!_baseController.Apply(disposeController))
				return false;

			_doc = (ColorProviderARGBGradient)_baseController.ModelObject;

			_doc = _doc
						.WithRed0(_view.Red0)
						.WithRed1(_view.Red1)
						.WithGreen0(_view.Green0)
						.WithGreen1(_view.Green1)
						.WithBlue0(_view.Blue0)
						.WithBlue1(_view.Blue1)
						.WithOpaqueness0(_view.Opaqueness0)
						.WithOpaqueness1(_view.Opaqueness1);

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.ChoiceChanged += OnMadeDirty;
		}

		protected override void DetachView()
		{
			_view.ChoiceChanged -= OnMadeDirty;
			base.DetachView();
		}

		private void EhBaseControllerChanged(IMVCANDController ctrl)
		{
			OnMadeDirty();
		}
	}
}