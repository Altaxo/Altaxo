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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Graph.Gdi.Plot.ColorProvider;

namespace Altaxo.Gui.Graph.ColorProvider
{
	public interface IColorProviderAHSBGradientView
	{
		IColorProviderBaseView BaseView { get; }

		double Hue0 { get; set;}
		double Hue1 { get; set; }
		double Saturation0 { get; set; }
		double Saturation1 { get; set; }
		double Brightness0 { get; set; }
		double Brightness1 { get; set; }
		double Opaqueness0 { get; set; }
		double Opaqueness1 { get; set; }

		event Action ChoiceChanged;
	}

	[ExpectedTypeOfView(typeof(IColorProviderAHSBGradientView))]
	[UserControllerForObject(typeof(ColorProviderAHSBGradient),110)]
	public class ColorProviderAHSBGradientController : MVCANDControllerBase<ColorProviderAHSBGradient, IColorProviderAHSBGradientView>
	{
		ColorProviderBaseController _baseController;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_baseController = new ColorProviderBaseController();
				_baseController.UseDocumentCopy = UseDocument.Directly;
				_baseController.InitializeDocument(_doc);
				_baseController.MadeDirty += EhBaseControllerChanged;
			}
			if (null != _view)
			{
				_baseController.ViewObject = _view.BaseView;

				_view.Hue0 = _doc.Hue0;
				_view.Hue1 = _doc.Hue1;
				_view.Saturation0 = _doc.Saturation0;
				_view.Saturation1 = _doc.Saturation1;
				_view.Brightness0 = _doc.Brightness0;
				_view.Brightness1 = _doc.Brightness1;
				_view.Opaqueness0 = _doc.Opaqueness0;
				_view.Opaqueness1 = _doc.Opaqueness1;
			}
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

		void EhBaseControllerChanged(IMVCANDController ctrl)
		{
				OnMadeDirty();
		}

		public override bool Apply()
		{
			if (!_baseController.Apply())
				return false;

			_doc.Hue0 = _view.Hue0;
			_doc.Hue1 = _view.Hue1;
			_doc.Saturation0 = _view.Saturation0;
			_doc.Saturation1 = _view.Saturation1;
			_doc.Brightness0 = _view.Brightness0;
			_doc.Brightness1 = _view.Brightness1;
			_doc.Opaqueness0 = _view.Opaqueness0;
			_doc.Opaqueness1 = _view.Opaqueness1;

			
			if(_useDocumentCopy)
				CopyHelper.Copy(ref _originalDoc, _doc);
			
			return true;
		}
	}
}
