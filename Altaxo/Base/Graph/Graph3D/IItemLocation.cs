#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Graph3D
{
	public interface IItemLocation
	:
	Altaxo.Main.IDocumentLeafNode,
	Altaxo.Main.ICopyFrom
	{
		double RotationX { get; set; }
		double RotationY { get; set; }
		double RotationZ { get; set; }

		double ShearX { get; set; }

		double ShearY { get; set; }

		double ShearZ { get; set; }

		double ScaleX { get; set; }

		double ScaleY { get; set; }

		double ScaleZ { get; set; }
	}
}