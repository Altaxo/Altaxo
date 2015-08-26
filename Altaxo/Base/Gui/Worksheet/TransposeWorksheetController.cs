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

namespace Altaxo.Gui.Worksheet
{
	#region Interfaces

	public interface ITransposeWorksheetView
	{
		/// <summary>
		/// Get/sets the number of data columns that are moved to the property columns before transposing the data columns.
		/// </summary>
		int DataColumnsMoveToPropertyColumns { get; set; }

		/// <summary>
		/// Get/sets the number of property columns that are moved after transposing the data columns to the data columns collection.
		/// </summary>
		int PropertyColumnsMoveToDataColumns { get; set; }
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for TransposeWorksheetController.
	/// </summary>
	[ExpectedTypeOfView(typeof(ITransposeWorksheetView))]
	[UserControllerForObject(typeof(Altaxo.Data.TransposeOptions))]
	public class TransposeWorksheetController : MVCANControllerEditOriginalDocBase<Altaxo.Data.TransposeOptions, ITransposeWorksheetView>
	{
		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (null != _view)
			{
				_view.DataColumnsMoveToPropertyColumns = _doc.DataColumnsMoveToPropertyColumns;
				_view.PropertyColumnsMoveToDataColumns = _doc.PropertyColumnsMoveToDataColumns;
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc.DataColumnsMoveToPropertyColumns = _view.DataColumnsMoveToPropertyColumns;
			_doc.PropertyColumnsMoveToDataColumns = _view.PropertyColumnsMoveToDataColumns;

			return ApplyEnd(true, disposeController);
		}

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}
	}
}