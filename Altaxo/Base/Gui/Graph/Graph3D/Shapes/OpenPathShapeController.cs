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
using Altaxo.Graph;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Shapes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph.Graph3D.Shapes
{
	public interface IOpenPathShapeView
	{
		PenX3D DocPen { get; set; }

		object LocationView { set; }
	}

	[UserControllerForObject(typeof(OpenPathShapeBase), 101)]
	[ExpectedTypeOfView(typeof(IOpenPathShapeView))]
	public class OpenPathShapeController : MVCANControllerEditOriginalDocBase<OpenPathShapeBase, IOpenPathShapeView>
	{
		private IMVCANController _locationController;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_locationController, () => _locationController = null);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_locationController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.Location }, typeof(IMVCANController), UseDocument.Directly);
				Current.Gui.FindAndAttachControlTo(_locationController);
			}
			if (_view != null)
			{
				_view.DocPen = _doc.Pen;
				_view.LocationView = _locationController.ViewObject;
			}
		}

		#region IApplyController Members

		public override bool Apply(bool disposeController)
		{
			try
			{
				if (!_locationController.Apply(disposeController))
					return false;

				if (!object.ReferenceEquals(_doc.Location, _locationController.ModelObject))
					_doc.Location.CopyFrom((ItemLocationDirect)_locationController.ModelObject);

				_doc.Pen = _view.DocPen;
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox(string.Format("An exception has occured during applying of your settings. The message is: {0}", ex.Message));
				return false;
			}

			return ApplyEnd(true, disposeController);
		}

		#endregion IApplyController Members
	}
}