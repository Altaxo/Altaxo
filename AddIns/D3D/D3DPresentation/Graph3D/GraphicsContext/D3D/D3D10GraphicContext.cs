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
using System.Threading.Tasks;

namespace Altaxo.Graph3D.GraphicsContext.D3D
{
	using Altaxo.Geometry;
	using Altaxo.Graph3D;
	using Altaxo.Graph3D.GraphicsContext;
	using SharpDX;

	public class D3D10GraphicContext : GraphicContext3DBase, IDisposable
	{
		protected Dictionary<IMaterial3D, PositionIndexedTriangleBuffer> _positionIndexedTriangleBuffers = new Dictionary<IMaterial3D, PositionIndexedTriangleBuffer>(MaterialComparer.Instance);
		protected Dictionary<IMaterial3D, PositionNormalIndexedTriangleBuffer> _positionNormalIndexedTriangleBuffers = new Dictionary<IMaterial3D, PositionNormalIndexedTriangleBuffer>(MaterialComparer.Instance);

		protected Dictionary<IMaterial3D, PositionColorIndexedTriangleBuffer> _positionColorIndexedTriangleBuffers = new Dictionary<IMaterial3D, PositionColorIndexedTriangleBuffer>(MaterialComparer.Instance);
		protected Dictionary<IMaterial3D, PositionNormalColorIndexedTriangleBuffer> _positionNormalColorIndexedTriangleBuffers = new Dictionary<IMaterial3D, PositionNormalColorIndexedTriangleBuffer>(MaterialComparer.Instance);

		protected Dictionary<IMaterial3D, PositionUVIndexedTriangleBuffer> _positionUVIndexedTriangleBuffers = new Dictionary<IMaterial3D, PositionUVIndexedTriangleBuffer>(MaterialComparer.Instance);
		protected Dictionary<IMaterial3D, PositionNormalUVIndexedTriangleBuffer> _positionNormalUVIndexedTriangleBuffers = new Dictionary<IMaterial3D, PositionNormalUVIndexedTriangleBuffer>(MaterialComparer.Instance);

		private GraphicState _transformation = new GraphicState() { Transformation = MatrixD3D.Identity };

		public void Dispose()
		{
		}

		public IEnumerable<KeyValuePair<IMaterial3D, PositionIndexedTriangleBuffer>> PositionIndexedTriangleBuffers
		{
			get
			{
				return _positionIndexedTriangleBuffers;
			}
		}

		public IEnumerable<KeyValuePair<IMaterial3D, IndexedTriangleBuffer>> PositionIndexedTriangleBuffersAsIndexedTriangleBuffers
		{
			get
			{
				foreach (var entry in _positionIndexedTriangleBuffers)
					yield return new KeyValuePair<IMaterial3D, IndexedTriangleBuffer>(entry.Key, entry.Value);
			}
		}

		public IEnumerable<KeyValuePair<IMaterial3D, PositionNormalIndexedTriangleBuffer>> PositionNormalIndexedTriangleBuffers
		{
			get
			{
				return _positionNormalIndexedTriangleBuffers;
			}
		}

		public IEnumerable<KeyValuePair<IMaterial3D, IndexedTriangleBuffer>> PositionNormalIndexedTriangleBuffersAsIndexedTriangleBuffers
		{
			get
			{
				foreach (var entry in _positionNormalIndexedTriangleBuffers)
					yield return new KeyValuePair<IMaterial3D, IndexedTriangleBuffer>(entry.Key, entry.Value);
			}
		}

		public IEnumerable<KeyValuePair<IMaterial3D, PositionColorIndexedTriangleBuffer>> PositionColorIndexedTriangleBuffers
		{
			get
			{
				return _positionColorIndexedTriangleBuffers;
			}
		}

		public IEnumerable<KeyValuePair<IMaterial3D, IndexedTriangleBuffer>> PositionColorIndexedTriangleBuffersAsIndexedTriangleBuffers
		{
			get
			{
				foreach (var entry in _positionColorIndexedTriangleBuffers)
					yield return new KeyValuePair<IMaterial3D, IndexedTriangleBuffer>(entry.Key, entry.Value);
			}
		}

		public IEnumerable<KeyValuePair<IMaterial3D, PositionNormalColorIndexedTriangleBuffer>> PositionNormalColorIndexedTriangleBuffers
		{
			get
			{
				return _positionNormalColorIndexedTriangleBuffers;
			}
		}

		public IEnumerable<KeyValuePair<IMaterial3D, IndexedTriangleBuffer>> PositionNormalColorIndexedTriangleBuffersAsIndexedTriangleBuffers
		{
			get
			{
				foreach (var entry in _positionNormalColorIndexedTriangleBuffers)
					yield return new KeyValuePair<IMaterial3D, IndexedTriangleBuffer>(entry.Key, entry.Value);
			}
		}

		public IEnumerable<KeyValuePair<IMaterial3D, PositionUVIndexedTriangleBuffer>> PositionUVIndexedTriangleBuffers
		{
			get
			{
				return _positionUVIndexedTriangleBuffers;
			}
		}

		public IEnumerable<KeyValuePair<IMaterial3D, IndexedTriangleBuffer>> PositionUVIndexedTriangleBuffersAsIndexedTriangleBuffers
		{
			get
			{
				foreach (var entry in _positionUVIndexedTriangleBuffers)
					yield return new KeyValuePair<IMaterial3D, IndexedTriangleBuffer>(entry.Key, entry.Value);
			}
		}

		public IEnumerable<KeyValuePair<IMaterial3D, PositionNormalUVIndexedTriangleBuffer>> PositionNormalUVIndexedTriangleBuffers
		{
			get
			{
				return _positionNormalUVIndexedTriangleBuffers;
			}
		}

		public IEnumerable<KeyValuePair<IMaterial3D, IndexedTriangleBuffer>> PositionNormalUVIndexedTriangleBuffersAsIndexedTriangleBuffers
		{
			get
			{
				foreach (var entry in _positionNormalUVIndexedTriangleBuffers)
					yield return new KeyValuePair<IMaterial3D, IndexedTriangleBuffer>(entry.Key, entry.Value);
			}
		}

		#region Transformation

		internal GraphicState Transformation
		{
			get
			{
				return _transformation;
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

		public override void PrependTransform(MatrixD3D m)
		{
			_transformation.Transformation.PrependTransform(m);
		}

		public override void TranslateTransform(double x, double y, double z)
		{
			_transformation.Transformation.TranslatePrepend(x, y, z);
		}

		public override PositionNormalIndexedTriangleBuffers GetPositionNormalIndexedTriangleBuffer(IMaterial3D material)
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

		private PositionNormalIndexedTriangleBuffer InternalGetPositionNormalIndexedTriangleBuffer(IMaterial3D material)
		{
			PositionNormalIndexedTriangleBuffer result;
			if (!_positionNormalIndexedTriangleBuffers.TryGetValue(material, out result))
			{
				result = new PositionNormalIndexedTriangleBuffer(this);
				_positionNormalIndexedTriangleBuffers.Add(material, result);
			}

			return result;
		}

		private PositionNormalColorIndexedTriangleBuffer InternalGetPositionNormalColorIndexedTriangleBuffer(IMaterial3D material)
		{
			PositionNormalColorIndexedTriangleBuffer result;
			if (!_positionNormalColorIndexedTriangleBuffers.TryGetValue(material, out result))
			{
				result = new PositionNormalColorIndexedTriangleBuffer(this);
				_positionNormalColorIndexedTriangleBuffers.Add(material, result);
			}

			return result;
		}

		private PositionNormalUVIndexedTriangleBuffer InternalGetPositionNormalUVIndexedTriangleBuffer(IMaterial3D material)
		{
			PositionNormalUVIndexedTriangleBuffer result;
			if (!_positionNormalUVIndexedTriangleBuffers.TryGetValue(material, out result))
			{
				result = new PositionNormalUVIndexedTriangleBuffer(this);
				_positionNormalUVIndexedTriangleBuffers.Add(material, result);
			}

			return result;
		}

		public override PositionIndexedTriangleBuffers GetPositionIndexedTriangleBuffer(IMaterial3D material)
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

		private PositionIndexedTriangleBuffer InternalGetPositionIndexedTriangleBuffer(IMaterial3D material)
		{
			PositionIndexedTriangleBuffer result;
			if (!_positionIndexedTriangleBuffers.TryGetValue(material, out result))
			{
				result = new PositionIndexedTriangleBuffer(this);
				_positionIndexedTriangleBuffers.Add(material, result);
			}

			return result;
		}

		private PositionColorIndexedTriangleBuffer InternalGetPositionColorIndexedTriangleBuffer(IMaterial3D material)
		{
			PositionColorIndexedTriangleBuffer result;
			if (!_positionColorIndexedTriangleBuffers.TryGetValue(material, out result))
			{
				result = new PositionColorIndexedTriangleBuffer(this);
				_positionColorIndexedTriangleBuffers.Add(material, result);
			}

			return result;
		}

		private PositionUVIndexedTriangleBuffer InternalGetPositionUVIndexedTriangleBuffer(IMaterial3D material)
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
			public MatrixD3D Transformation;
		}

		#endregion Transformation
	}
}