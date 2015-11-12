using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph3D.GraphicsContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph3D.Material
{
	public sealed class NoMaterial : IMaterial3D
	{
		public static IMaterial3D Instance { get; private set; }

		static NoMaterial()
		{
			Instance = new NoMaterial();
		}

		private NoMaterial()
		{
		}

		public NamedColor Color
		{
			get
			{
				return NamedColors.Transparent;
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
			return object.ReferenceEquals(other, this);
		}

		public void SetEnvironment(IGraphicContext3D g, RectangleD3D rectangleD)
		{
			throw new NotImplementedException();
		}

		public IMaterial3D WithColor(NamedColor color)
		{
			throw new NotImplementedException();
		}
	}
}