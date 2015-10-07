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

using Altaxo.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	public class Materials
	{
		public static IMaterial3D GetSolidMaterial(Altaxo.Graph.NamedColor color)
		{
			return new SolidColor(color);
		}
	}

	public class SolidColor : Altaxo.Main.SuspendableDocumentLeafNodeWithEventArgs, IMaterial3D
	{
		private Altaxo.Graph.NamedColor _color;

		public SolidColor(Altaxo.Graph.NamedColor color)
		{
			_color = color;
		}

		public NamedColor Color
		{
			get
			{
				return _color;
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public bool SupportsGetColor
		{
			get
			{
				return true;
			}
		}

		public bool SupportsSetColor
		{
			get
			{
				return false;
			}
		}

		public object Clone()
		{
			return new SolidColor(_color);
		}

		public void SetEnvironment(IGraphicContext3D g, RectangleD3D rectangleD)
		{
		}
	}
}