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

using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Gui;
using Altaxo.Serialization;
using Altaxo.Units;
using System;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	public interface ILayerGridPositionSizeView
	{
		double XPosition { get; set; }

		double YPosition { get; set; }

		double XSize { get; set; }

		double YSize { get; set; }

		double Rotation { get; set; }

		double Scale { get; set; }
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for LayerPositionController.
	/// </summary>
	[ExpectedTypeOfView(typeof(ILayerGridPositionSizeView))]
	public class LayerGridPositionSizeController : MVCANControllerBase<ItemLocationByGrid, ILayerGridPositionSizeView>
	{
		private GridPartitioning _parentLayerGrid;

		public override bool InitializeDocument(params object[] args)
		{
			if (args.Length < 2)
				return false;
			if (!(args[1] is GridPartitioning))
				return false;
			_parentLayerGrid = (GridPartitioning)args[1];
			return base.InitializeDocument(args);
		}

		private static double DocToUserPosition(double x)
		{
			return 1 + (x - 1) / 2;
		} // 1->1, 3->2, 5->3 usw.

		private static double DocToUserSize(double x)
		{
			return 1 + (x - 1) / 2;
		}

		private static double UserToDocPosition(double x)
		{
			return 1 + 2 * (x - 1);
		} // 1 -> 1, 2->3, 3->5 usw.

		private static double UserToDocSize(double x)
		{
			return 1 + 2 * (x - 1);
		} // 1 -> 1, 2->3, 3->5 usw.

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
			}
			if (null != _view)
			{
				_view.XPosition = DocToUserPosition(_doc.GridColumn);
				_view.YPosition = DocToUserPosition(_doc.GridRow);
				_view.XSize = DocToUserSize(_doc.GridColumnSpan);
				_view.YSize = DocToUserSize(_doc.GridRowSpan);
				_view.Rotation = _doc.Rotation;
				_view.Scale = _doc.Scale;
			}
		}

		#region IApplyController Members

		public override bool Apply()
		{
			try
			{
				_doc.GridColumn = UserToDocPosition(_view.XPosition);
				_doc.GridRow = UserToDocPosition(_view.YPosition);
				_doc.GridColumnSpan = UserToDocSize(_view.XSize);
				_doc.GridRowSpan = UserToDocSize(_view.YSize);
				_doc.Scale = _view.Scale;
				_doc.Rotation = _view.Rotation;

				_originalDoc.CopyFrom(_doc);
			}
			catch (Exception)
			{
				return false; // indicate that something failed
			}
			return true;
		}

		#endregion IApplyController Members
	}
}