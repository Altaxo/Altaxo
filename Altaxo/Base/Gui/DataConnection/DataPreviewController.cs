﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.DataConnection
{
	public interface IDataPreviewView
	{
		void SetTableSource(System.Data.DataTable table);
	}

	[ExpectedTypeOfView(typeof(IDataPreviewView))]
	public class DataPreviewController : IMVCAController
	{
		private IDataPreviewView _view;
		private System.Data.DataTable dt;
		private System.Drawing.Size size;

		public DataPreviewController(System.Data.DataTable dt)
		{
			this.dt = dt;
			Initialize(true);
		}

		private void Initialize(bool initData)
		{
			if (initData)
			{
			}
			if (null != _view)
			{
				_view.SetTableSource(dt);
			}
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IDataPreviewView;
				if (null != _view)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return null; }
		}

		public void Dispose()
		{
			ViewObject = null;
		}

		public bool Apply()
		{
			return true;
		}
	}
}