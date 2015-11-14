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

using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph3D.GraphicsContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	public class Materials
	{
		public static MaterialWithoutColorOrTexture _materialWithoutColorOrTexture = new MaterialWithoutColorOrTexture();

		public static IMaterial3D GetSolidMaterial(NamedColor color)
		{
			return new SolidColor(color);
		}

		public static IMaterial3D GetMaterialWithNewColor(IMaterial3D material, NamedColor newColor)
		{
			return material.WithColor(newColor);
		}

		public static IMaterial3D GetSolidMaterialWithoutColorOrTexture()
		{
			return _materialWithoutColorOrTexture;
		}
	}

	public class MaterialWithoutColorOrTexture : IMaterial3D
	{
		public NamedColor Color
		{
			get
			{
				return NamedColors.Black;
			}
		}

		public bool HasColor
		{
			get
			{
				return false;
			}
		}

		public bool HasTexture
		{
			get
			{
				return false;
			}
		}

		public bool Equals(IMaterial3D other)
		{
			return other is MaterialWithoutColorOrTexture;
		}

		public void SetEnvironment(IGraphicContext3D g, RectangleD3D rectangleD)
		{
		}

		public IMaterial3D WithColor(NamedColor color)
		{
			return this;
		}
	}

	public class SolidColor : IMaterial3D
	{
		private NamedColor _color;

		public SolidColor(NamedColor color)
		{
			_color = color;
		}

		public NamedColor Color
		{
			get
			{
				return _color;
			}
		}

		public bool HasColor
		{
			get
			{
				return true;
			}
		}

		public bool HasTexture
		{
			get
			{
				return false;
			}
		}

		public bool Equals(IMaterial3D other)
		{
			var othersd = other as SolidColor;
			return null != othersd && this.Color == othersd.Color;
		}

		public void SetEnvironment(IGraphicContext3D g, RectangleD3D rectangleD)
		{
		}

		public IMaterial3D WithColor(NamedColor color)
		{
			if (color == this._color)
				return this;
			else
				return new SolidColor(color);
		}
	}
}