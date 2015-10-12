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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph3D
{
	using Altaxo.Graph3D;

	#region Interfaces

	public interface IPenAllPropertiesView
	{
		PenX3D Pen { get; set; }

		bool ShowPlotColorsOnly { set; }
	}

	#endregion Interfaces

	[ExpectedTypeOfView(typeof(IPenAllPropertiesView))]
	public class PenAllPropertiesController : IMVCAController
	{
		private IPenAllPropertiesView _view;
		private PenX3D _doc;
		private bool _showPlotColorsOnly;

		public PenAllPropertiesController(PenX3D doc)
		{
			_doc = doc;
			Initialize(true);
		}

		private void Initialize(bool initData)
		{
			if (_view != null)
			{
				_view.ShowPlotColorsOnly = _showPlotColorsOnly;
				_view.Pen = _doc;
			}
		}

		public bool ShowPlotColorsOnly
		{
			get
			{
				return _showPlotColorsOnly;
			}
			set
			{
				_showPlotColorsOnly = value;
				if (null != _view)
					_view.ShowPlotColorsOnly = value;
			}
		}

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IPenAllPropertiesView;

				if (null != _view)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply(bool disposeController)
		{
			_doc = _view.Pen;
			return true;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		#endregion IApplyController Members
	}
}