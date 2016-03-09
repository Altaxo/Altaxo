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

using Altaxo.Drawing.D3D;
using Altaxo.Drawing.D3D.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Drawing.D3D
{
	public interface IMaterialView
	{
		double SpecularIntensity { get; set; }
		double SpecularExponent { get; set; }
		double SpecularMixingCoefficient { get; set; }
	}

	[ExpectedTypeOfView(typeof(IMaterialView))]
	[UserControllerForObject(typeof(IMaterial))]
	public class MaterialController : MVCANControllerEditImmutableDocBase<IMaterial, IMaterialView>
	{
		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (null != _view)
			{
				_view.SpecularIntensity = _doc.SpecularIntensity;
				_view.SpecularExponent = _doc.SpecularExponent;
				_view.SpecularMixingCoefficient = _doc.SpecularMixingCoefficient;
			}
		}

		public override bool Apply(bool disposeController)
		{
			try
			{
				_doc = _doc.WithSpecularProperties(_view.SpecularIntensity, _view.SpecularExponent, _view.SpecularMixingCoefficient);
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