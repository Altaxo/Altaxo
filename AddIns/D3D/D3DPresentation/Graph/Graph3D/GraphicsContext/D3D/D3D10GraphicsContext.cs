#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Threading.Tasks;

namespace Altaxo.Graph.Graph3D.GraphicsContext.D3D
{
	using Altaxo.Geometry;
	using Altaxo.Graph.Graph3D.GraphicsContext;
	using Drawing.D3D;

	public class D3D10GraphicsContext : GraphicsContext3DBase, IDisposable
	{
		protected Dictionary<IMaterial, PositionIndexedTriangleBuffer> _positionIndexedTriangleBuffers = new Dictionary<IMaterial, PositionIndexedTriangleBuffer>();

		protected Dictionary<IMaterial, PositionNormalIndexedTriangleBuffer> _positionNormalIndexedTriangleBuffers = new Dictionary<IMaterial, PositionNormalIndexedTriangleBuffer>();

		//protected Dictionary<IMaterial, PositionColorIndexedTriangleBuffer> _positionColorIndexedTriangleBuffers = new Dictionary<IMaterial, PositionColorIndexedTriangleBuffer>(MaterialComparer.Instance);

		protected Dictionary<MaterialPlusClipping, PositionColorIndexedTriangleBuffer> _positionColorIndexedTriangleBuffers = new Dictionary<MaterialPlusClipping, PositionColorIndexedTriangleBuffer>();

		protected Dictionary<MaterialPlusClipping, PositionNormalColorIndexedTriangleBuffer> _positionNormalColorIndexedTriangleBuffers = new Dictionary<MaterialPlusClipping, PositionNormalColorIndexedTriangleBuffer>();

		protected Dictionary<IMaterial, PositionUVIndexedTriangleBuffer> _positionUVIndexedTriangleBuffers = new Dictionary<IMaterial, PositionUVIndexedTriangleBuffer>();

		protected Dictionary<IMaterial, PositionNormalUVIndexedTriangleBuffer> _positionNormalUVIndexedTriangleBuffers = new Dictionary<IMaterial, PositionNormalUVIndexedTriangleBuffer>();

		private GraphicState _transformation = new GraphicState() { Transformation = Matrix4x3.Identity };

		public void Dispose()
		{
		}

		public IEnumerable<KeyValuePair<IMaterial, PositionIndexedTriangleBuffer>> PositionIndexedTriangleBuffers
		{
			get
			{
				return _positionIndexedTriangleBuffers;
			}
		}

		public IEnumerable<KeyValuePair<IMaterial, IndexedTriangleBuffer>> PositionIndexedTriangleBuffersAsIndexedTriangleBuffers
		{
			get
			{
				foreach (var entry in _positionIndexedTriangleBuffers)
					yield return new KeyValuePair<IMaterial, IndexedTriangleBuffer>(entry.Key, entry.Value);
			}
		}

		public IEnumerable<KeyValuePair<IMaterial, PositionNormalIndexedTriangleBuffer>> PositionNormalIndexedTriangleBuffers
		{
			get
			{
				return _positionNormalIndexedTriangleBuffers;
			}
		}

		public IEnumerable<KeyValuePair<IMaterial, IndexedTriangleBuffer>> PositionNormalIndexedTriangleBuffersAsIndexedTriangleBuffers
		{
			get
			{
				foreach (var entry in _positionNormalIndexedTriangleBuffers)
					yield return new KeyValuePair<IMaterial, IndexedTriangleBuffer>(entry.Key, entry.Value);
			}
		}

		public IEnumerable<KeyValuePair<IMaterial, PositionColorIndexedTriangleBuffer>> PositionColorIndexedTriangleBuffers
		{
			get
			{
				return _positionColorIndexedTriangleBuffers.Select(kvp => new KeyValuePair<IMaterial, PositionColorIndexedTriangleBuffer>(kvp.Key.Material, kvp.Value));
			}
		}

		public IEnumerable<KeyValuePair<IMaterial, IndexedTriangleBuffer>> PositionColorIndexedTriangleBuffersAsIndexedTriangleBuffers
		{
			get
			{
				foreach (var entry in _positionColorIndexedTriangleBuffers)
					yield return new KeyValuePair<IMaterial, IndexedTriangleBuffer>(entry.Key.Material, entry.Value);
			}
		}

		public IEnumerable<KeyValuePair<IMaterial, PositionNormalColorIndexedTriangleBuffer>> PositionNormalColorIndexedTriangleBuffers
		{
			get
			{
				return _positionNormalColorIndexedTriangleBuffers.Select(kvp => new KeyValuePair<IMaterial, PositionNormalColorIndexedTriangleBuffer>(kvp.Key.Material, kvp.Value));
			}
		}

		public IEnumerable<KeyValuePair<IMaterial, IndexedTriangleBuffer>> PositionNormalColorIndexedTriangleBuffersAsIndexedTriangleBuffers
		{
			get
			{
				foreach (var entry in _positionNormalColorIndexedTriangleBuffers)
					yield return new KeyValuePair<IMaterial, IndexedTriangleBuffer>(entry.Key.Material, entry.Value);
			}
		}

		public IEnumerable<KeyValuePair<IMaterial, PositionUVIndexedTriangleBuffer>> PositionUVIndexedTriangleBuffers
		{
			get
			{
				return _positionUVIndexedTriangleBuffers;
			}
		}

		public IEnumerable<KeyValuePair<IMaterial, IndexedTriangleBuffer>> PositionUVIndexedTriangleBuffersAsIndexedTriangleBuffers
		{
			get
			{
				foreach (var entry in _positionUVIndexedTriangleBuffers)
					yield return new KeyValuePair<IMaterial, IndexedTriangleBuffer>(entry.Key, entry.Value);
			}
		}

		public IEnumerable<KeyValuePair<IMaterial, PositionNormalUVIndexedTriangleBuffer>> PositionNormalUVIndexedTriangleBuffers
		{
			get
			{
				return _positionNormalUVIndexedTriangleBuffers;
			}
		}

		public IEnumerable<KeyValuePair<IMaterial, IndexedTriangleBuffer>> PositionNormalUVIndexedTriangleBuffersAsIndexedTriangleBuffers
		{
			get
			{
				foreach (var entry in _positionNormalUVIndexedTriangleBuffers)
					yield return new KeyValuePair<IMaterial, IndexedTriangleBuffer>(entry.Key, entry.Value);
			}
		}

		#region Transformation

		public override Matrix4x3 Transformation
		{
			get
			{
				return _transformation.Transformation;
			}
		}

		public override object SaveGraphicsState()
		{
			return new GraphicState { Transformation = _transformation.Transformation };
		}

		public override void RestoreGraphicsState(object graphicsState)
		{
			var gs = graphicsState as GraphicState;
			if (null != gs)
				_transformation.Transformation = gs.Transformation;
			else
				throw new ArgumentException(nameof(graphicsState) + " is not a valid graphic state!");
		}

		public override void PrependTransform(Matrix4x3 m)
		{
			_transformation.Transformation.PrependTransform(m);
		}

		public override void TranslateTransform(double x, double y, double z)
		{
			_transformation.Transformation.TranslatePrepend(x, y, z);
		}

		public override PositionNormalIndexedTriangleBuffers GetPositionNormalIndexedTriangleBuffer(IMaterial material)
		{
			var result = new PositionNormalIndexedTriangleBuffers();

			if (material.HasTexture)
			{
				result.IndexedTriangleBuffer = result.PositionNormalUVIndexedTriangleBuffer = InternalGetPositionNormalUVIndexedTriangleBuffer(material);
			}
			else if (material.HasColor)
			{
				result.IndexedTriangleBuffer = result.PositionNormalIndexedTriangleBuffer = InternalGetPositionNormalIndexedTriangleBuffer(material);
			}
			else
			{
				result.IndexedTriangleBuffer = result.PositionNormalColorIndexedTriangleBuffer = InternalGetPositionNormalColorIndexedTriangleBuffer(material);
			}
			return result;
		}

		private PositionNormalIndexedTriangleBuffer InternalGetPositionNormalIndexedTriangleBuffer(IMaterial material)
		{
			PositionNormalIndexedTriangleBuffer result;
			if (!_positionNormalIndexedTriangleBuffers.TryGetValue(material, out result))
			{
				result = new PositionNormalIndexedTriangleBuffer(this);
				_positionNormalIndexedTriangleBuffers.Add(material, result);
			}

			return result;
		}

		private PositionNormalColorIndexedTriangleBuffer InternalGetPositionNormalColorIndexedTriangleBuffer(IMaterial material)
		{
			PositionNormalColorIndexedTriangleBuffer result;
			var key = new MaterialPlusClipping(material, null);
			if (!_positionNormalColorIndexedTriangleBuffers.TryGetValue(key, out result))
			{
				result = new PositionNormalColorIndexedTriangleBuffer(this);
				_positionNormalColorIndexedTriangleBuffers.Add(key, result);
			}

			return result;
		}

		private PositionNormalUVIndexedTriangleBuffer InternalGetPositionNormalUVIndexedTriangleBuffer(IMaterial material)
		{
			PositionNormalUVIndexedTriangleBuffer result;
			if (!_positionNormalUVIndexedTriangleBuffers.TryGetValue(material, out result))
			{
				result = new PositionNormalUVIndexedTriangleBuffer(this);
				_positionNormalUVIndexedTriangleBuffers.Add(material, result);
			}

			return result;
		}

		public override PositionIndexedTriangleBuffers GetPositionIndexedTriangleBuffer(IMaterial material)
		{
			var result = new PositionIndexedTriangleBuffers();

			if (material.HasTexture)
			{
				result.IndexedTriangleBuffer = result.PositionUVIndexedTriangleBuffer = InternalGetPositionUVIndexedTriangleBuffer(material);
			}
			else if (material.HasColor)
			{
				result.IndexedTriangleBuffer = result.PositionIndexedTriangleBuffer = InternalGetPositionIndexedTriangleBuffer(material);
			}
			else
			{
				result.IndexedTriangleBuffer = result.PositionColorIndexedTriangleBuffer = InternalGetPositionColorIndexedTriangleBuffer(material);
			}
			return result;
		}

		public override PositionNormalIndexedTriangleBuffers GetPositionNormalIndexedTriangleBufferWithClipping(IMaterial material, PlaneD3D[] clipPlanes)
		{
			var result = new PositionNormalIndexedTriangleBuffers();

			if (material.HasTexture)
			{
				throw new NotImplementedException();
			}
			else if (material.HasColor)
			{
				throw new NotImplementedException();
			}
			else
			{
				result.IndexedTriangleBuffer = result.PositionNormalColorIndexedTriangleBuffer = InternalGetPositionNormalColorIndexedTriangleBuffer(material, clipPlanes);
			}
			return result;
		}

		private PositionIndexedTriangleBuffer InternalGetPositionIndexedTriangleBuffer(IMaterial material)
		{
			PositionIndexedTriangleBuffer result;
			if (!_positionIndexedTriangleBuffers.TryGetValue(material, out result))
			{
				result = new PositionIndexedTriangleBuffer(this);
				_positionIndexedTriangleBuffers.Add(material, result);
			}

			return result;
		}

		private PositionColorIndexedTriangleBuffer InternalGetPositionColorIndexedTriangleBuffer(IMaterial material)
		{
			PositionColorIndexedTriangleBuffer result;
			var key = new MaterialPlusClipping(material, null);
			if (!_positionColorIndexedTriangleBuffers.TryGetValue(key, out result))
			{
				result = new PositionColorIndexedTriangleBuffer(this);
				_positionColorIndexedTriangleBuffers.Add(key, result);
			}

			return result;
		}

		private PositionNormalColorIndexedTriangleBuffer InternalGetPositionNormalColorIndexedTriangleBuffer(IMaterial material, PlaneD3D[] clipPlanes)
		{
			// Transform the clip planes to our coordinate system

			var clipPlanesTransformed = clipPlanes.Select(plane => _transformation.Transformation.Transform(plane)).ToArray();

			PositionNormalColorIndexedTriangleBuffer result;
			var key = new MaterialPlusClipping(material, clipPlanesTransformed);
			if (!_positionNormalColorIndexedTriangleBuffers.TryGetValue(key, out result))
			{
				result = new PositionNormalColorIndexedTriangleBufferWithClipping(this, clipPlanesTransformed);
				_positionNormalColorIndexedTriangleBuffers.Add(key, result);
			}

			return result;
		}

		private PositionUVIndexedTriangleBuffer InternalGetPositionUVIndexedTriangleBuffer(IMaterial material)
		{
			PositionUVIndexedTriangleBuffer result;
			if (!_positionUVIndexedTriangleBuffers.TryGetValue(material, out result))
			{
				result = new PositionUVIndexedTriangleBuffer(this);
				_positionUVIndexedTriangleBuffers.Add(material, result);
			}

			return result;
		}

		internal class GraphicState
		{
			public Matrix4x3 Transformation;
		}

		#endregion Transformation
	}
}