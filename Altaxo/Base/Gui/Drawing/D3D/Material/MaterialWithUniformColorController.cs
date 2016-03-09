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

using Altaxo.Drawing;
using Altaxo.Drawing.D3D.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Drawing.D3D.Material
{
	public interface IMaterialWithUniformColorView : IMaterialView
	{
		NamedColor Color { get; set; }
	}

	[ExpectedTypeOfView(typeof(IMaterialWithUniformColorView))]
	[UserControllerForObject(typeof(MaterialWithUniformColor), 101)]
	public class MaterialWithUniformColorController : MVCANControllerEditImmutableDocBase<MaterialWithUniformColor, IMaterialWithUniformColorView>
	{
		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (null != _view)
			{
				_view.SpecularIntensity = _doc.SpecularIntensity;
				_view.SpecularExponent = _doc.SpecularExponent;
				_view.SpecularMixingCoefficient = _doc.SpecularMixingCoefficient;
				_view.Color = _doc.Color;
			}
		}

		public override bool Apply(bool disposeController)
		{
			try
			{
				_doc = (MaterialWithUniformColor)_doc.WithSpecularProperties(_view.SpecularIntensity, _view.SpecularExponent, _view.SpecularMixingCoefficient).WithColor(_view.Color);
				return ApplyEnd(true, disposeController);
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox(
					string.Format(
						"Creating the material from your data failed\r\n" +
						"The message is: {0}", ex.Message), "Failure");
				return ApplyEnd(false, disposeController);
			}
		}

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}
	}
}