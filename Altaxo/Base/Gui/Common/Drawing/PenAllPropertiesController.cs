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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Common.Drawing
{
	using Altaxo.Graph.Gdi;

	#region Interfaces

	public interface IPenAllPropertiesView
	{
		PenX Pen { get; set; }
		bool ShowPlotColorsOnly { set; }
	}


	#endregion

	[ExpectedTypeOfView(typeof(IPenAllPropertiesView))]
	public class PenAllPropertiesController : IMVCAController
	{
		IPenAllPropertiesView _view;
		PenX _doc;
		bool _showPlotColorsOnly;

		public PenAllPropertiesController(PenX doc)
		{
			_doc = doc;
			Initialize(true);
		}

		void Initialize(bool initData)
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

		#endregion

		#region IApplyController Members

		public bool Apply()
		{
			_doc = _view.Pen;
			return true;
		}

		#endregion
	}
}
