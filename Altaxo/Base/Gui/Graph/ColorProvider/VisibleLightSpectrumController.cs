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
	public interface IVisibleLightSpectrumView
	{
		IColorProviderBaseView BaseView { get; }

		double Gamma { get; set;}
		double Brightness { get; set; }

		event Action ChoiceChanged;
	}

	[ExpectedTypeOfView(typeof(IVisibleLightSpectrumView))]
	[UserControllerForObject(typeof(VisibleLightSpectrum),110)]
	public class VisibleLightSpectrumController : MVCANDControllerBase<VisibleLightSpectrum, IVisibleLightSpectrumView>
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

				_view.Gamma = _doc.Gamma;
				_view.Brightness = _doc.Brightness;
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

			_doc.Gamma = _view.Gamma;
			_doc.Brightness = _view.Brightness;

			
			if(_useDocumentCopy)
				CopyHelper.Copy(ref _originalDoc, _doc);
			
			return true;
		}
	}
}
