#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2015 Dr. Dirk Lellinger
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

using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Data
{
	public interface IDataTableProxyView
	{
		void InitializeTables(SelectableListNodeList tables);
	}

	[ExpectedTypeOfView(typeof(IDataTableProxyView))]
	[UserControllerForObject(typeof(Altaxo.Data.DataTableProxy))]
	public class DataTableProxyController : MVCANControllerEditOriginalDocBase<Altaxo.Data.DataTableProxy, IDataTableProxyView>
	{
		private SelectableListNodeList _availableTables;

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				if (null == _availableTables)
					_availableTables = new SelectableListNodeList();

				foreach (var table in Current.Project.DataTableCollection)
					_availableTables.Add(new SelectableListNode(table.Name, table, object.ReferenceEquals(table, _doc.Document)));
			}

			if (null != _view)
			{
				_view.InitializeTables(_availableTables);
			}
		}

		public override bool Apply(bool disposeController)
		{
			var selNode = _availableTables.FirstSelectedNode;

			if (null == selNode)
			{
				return ApplyEnd(false, disposeController);
			}

			_doc.SetDocNode((Altaxo.Data.DataTable)selNode.Tag);

			return ApplyEnd(true, disposeController);
		}

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}
	}
}