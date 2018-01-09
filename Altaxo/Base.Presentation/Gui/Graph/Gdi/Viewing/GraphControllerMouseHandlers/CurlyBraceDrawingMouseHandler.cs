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

using Altaxo.Graph.Gdi.Shapes;
using System;

namespace Altaxo.Gui.Graph.Gdi.Viewing.GraphControllerMouseHandlers
{
	/// <summary>
	/// Summary description for RectangleDrawingMouseHandler.
	/// </summary>
	public class CurlyBraceDrawingMouseHandler : AbstractRectangularToolMouseHandler
	{
		public CurlyBraceDrawingMouseHandler(GraphController grac)
			: base(grac)
		{
		}

		public override GraphToolType GraphToolType
		{
			get { return GraphToolType.CurlyBraceDrawing; }
		}

		protected override void FinishDrawing()
		{
			var rect = GetNormalRectangle(_Points[0].LayerCoordinates, _Points[1].LayerCoordinates);

			// deselect the text tool
			_grac.SetGraphToolFromInternal(GraphToolType.ObjectPointer);

			if (rect.Width != 0 && rect.Height != 0)
			{
				CurlyBraceShape go = new CurlyBraceShape(_grac.Doc.GetPropertyContext());
				go.SetParentSize(_grac.ActiveLayer.Size, false);
				go.SetRelativeSizePositionFromAbsoluteValues(rect.Size, rect.LeftTop);
				_grac.ActiveLayer.GraphObjects.Add(go);
			}
		}
	}
}