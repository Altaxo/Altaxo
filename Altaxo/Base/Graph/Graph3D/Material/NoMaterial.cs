using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Material
{
	using GraphicsContext;

	public sealed class NoMaterial : IMaterial
	{
		public static IMaterial Instance { get; private set; }

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

		public bool Equals(IMaterial other)
		{
			return object.ReferenceEquals(other, this);
		}

		public void SetEnvironment(IGraphicContext3D g, RectangleD3D rectangleD)
		{
			throw new NotImplementedException();
		}

		public IMaterial WithColor(NamedColor color)
		{
			throw new NotImplementedException();
		}
	}
}