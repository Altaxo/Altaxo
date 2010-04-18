#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


namespace Altaxo.Graph.Gdi.Shapes
{
	public  abstract partial class GraphicBase 
	{
		protected class GraphicBaseHitTestObject : HitTestObject
		{
			public GraphicBaseHitTestObject(GraphicsPath objectPath, GraphicBase parent)
				: base(objectPath, parent)
			{
			}

			public GraphicBaseHitTestObject(GraphicsPath objectPath, GraphicsPath selectionPath, GraphicBase parent)
				: base(objectPath, selectionPath, parent)
			{
			}

			public override int GetNextGripLevel(int currentGripLevel)
			{
        int newLevel = 1 + currentGripLevel;
        int maxLevel = ((GraphicBase)_hitobject).AutoSize ? 3 : 4;
        if (newLevel > maxLevel)
          newLevel = 1;
				return newLevel;
			}

			public override IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel)
			{
				if (((GraphicBase)_hitobject).AutoSize)
				{
					switch (gripLevel)
					{
						case 0:
							return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move);
						case 1:
							return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Rotate);
            case 2:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Rescale);
            case 3:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Shear);
          }
				}
				else // a normal object
				{
					switch (gripLevel)
					{
						case 0:
							return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move);
						case 1:
							return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Resize);
						case 2:
							return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Rotate);
            case 3:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Shear);
            case 4:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Rescale);
          }
				}
				return null;
			}
		}

	}
}
