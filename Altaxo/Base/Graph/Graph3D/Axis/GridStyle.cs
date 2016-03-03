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
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;
using System;
using System.Collections.Generic;

namespace Altaxo.Graph.Graph3D.Axis
{
	[Serializable]
	public class GridStyle
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		ICloneable
	{
		private PenX3D _minorPen;
		private PenX3D _majorPen;
		private bool _showGrid;

		private bool _showMinor;
		private bool _showZeroOnly;

		#region Serialization

		/// <summary>
		/// 2015-11-14 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GridStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				GridStyle s = (GridStyle)obj;

				info.AddValue("Visible", s._showGrid);
				if (s._showGrid)
				{
					info.AddValue("ZeroOnly", s._showZeroOnly);
					info.AddValue("MajorPen", s._majorPen);
					info.AddValue("ShowMinor", s._showMinor);
					if (s._showMinor)
						info.AddValue("MinorPen", s._minorPen);
				}
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				GridStyle s = SDeserialize(o, info, parent);
				return s;
			}

			protected virtual GridStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				GridStyle s = null != o ? (GridStyle)o : new GridStyle();

				s._showGrid = info.GetBoolean("Visible");
				if (s._showGrid)
				{
					s._showZeroOnly = info.GetBoolean("ZeroOnly");
					s._majorPen = (PenX3D)info.GetValue("MajorPen", s);

					s._showMinor = info.GetBoolean("ShowMinor");
					if (s._showMinor)
					{
						s._minorPen = (PenX3D)info.GetValue("MinorPen", s);
					}
				}

				return s;
			}
		}

		#endregion Serialization

		public GridStyle()
		{
			_showGrid = true;
		}

		public GridStyle(GridStyle from)
		{
			CopyFrom(from);
		}

		public void CopyFrom(GridStyle from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			this.MajorPen = from._majorPen;
			this.MinorPen = from._minorPen;
			this._showGrid = from._showGrid;
			this._showMinor = from._showMinor;
			this._showZeroOnly = from._showZeroOnly;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield break;
		}

		public PenX3D MajorPen
		{
			get
			{
				if (null == _majorPen)
					MajorPen = new PenX3D(NamedColors.Blue, 1);
				return _majorPen;
			}
			set
			{
				var oldValue = _majorPen;
				_majorPen = value;
				if (!object.ReferenceEquals(oldValue, value))
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public PenX3D MinorPen
		{
			get
			{
				if (null == _minorPen)
					MinorPen = new PenX3D(NamedColors.LightBlue, 1);

				return _minorPen;
			}
			set
			{
				var oldValue = _majorPen;
				_minorPen = value;
				if (!object.ReferenceEquals(oldValue, value))
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public bool ShowGrid
		{
			get { return _showGrid; }
			set
			{
				if (value != _showGrid)
				{
					_showGrid = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public bool ShowMinor
		{
			get { return _showMinor; }
			set
			{
				if (value != _showMinor)
				{
					_showMinor = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public bool ShowZeroOnly
		{
			get { return _showZeroOnly; }
			set
			{
				if (value != _showZeroOnly)
				{
					_showZeroOnly = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public void Paint(IGraphicsContext3D g, IPlotArea layer, CSPlaneID plane, int axisnumber)
		{
			if (!_showGrid)
				return;

			Scale axis = layer.Scales[axisnumber];
			TickSpacing ticking = layer.Scales[axisnumber].TickSpacing;

			RectangleD3D layerRect = new RectangleD3D(PointD3D.Empty, layer.Size);

			if (_showZeroOnly)
			{
				Altaxo.Data.AltaxoVariant var = new Altaxo.Data.AltaxoVariant(0.0);
				double rel = axis.PhysicalVariantToNormal(var);
				//_majorPen.SetEnvironment(layerRect, BrushX.GetEffectiveMaximumResolution(g, 1));
				if (rel >= 0 && rel <= 1)
				{
					Logical3D logV = new Logical3D();
					logV.SetR(plane.PerpendicularAxisNumber, plane.LogicalValue);
					logV.SetR(axisnumber, rel);
					var thirdAxisNumber = Logical3D.GetPerpendicularAxisNumber(plane.PerpendicularAxisNumber, axisnumber);
					var line = layer.CoordinateSystem.GetIsoline(logV.WithR(thirdAxisNumber, 0), logV.WithR(thirdAxisNumber, 1));
					g.DrawLine(MajorPen, line);
				}
			}
			else
			{
				double[] ticks;

				if (_showMinor)
				{
					//_minorPen.SetEnvironment(layerRect, BrushX.GetEffectiveMaximumResolution(g, 1));
					ticks = ticking.GetMinorTicksNormal(axis);
					for (int i = 0; i < ticks.Length; ++i)
					{
						Logical3D logV = new Logical3D();
						logV.SetR(plane.PerpendicularAxisNumber, plane.LogicalValue);
						logV.SetR(axisnumber, ticks[i]);
						var thirdAxisNumber = Logical3D.GetPerpendicularAxisNumber(plane.PerpendicularAxisNumber, axisnumber);
						var line = layer.CoordinateSystem.GetIsoline(logV.WithR(thirdAxisNumber, 0), logV.WithR(thirdAxisNumber, 1));
						g.DrawLine(MinorPen, line);
					}
				}

				//MajorPen.SetEnvironment(layerRect, BrushX.GetEffectiveMaximumResolution(g, 1));
				ticks = ticking.GetMajorTicksNormal(axis);
				for (int i = 0; i < ticks.Length; ++i)
				{
					Logical3D logV = new Logical3D();
					logV.SetR(plane.PerpendicularAxisNumber, plane.LogicalValue);
					logV.SetR(axisnumber, ticks[i]);
					var thirdAxisNumber = Logical3D.GetPerpendicularAxisNumber(plane.PerpendicularAxisNumber, axisnumber);
					var line = layer.CoordinateSystem.GetIsoline(logV.WithR(thirdAxisNumber, 0), logV.WithR(thirdAxisNumber, 1));
					g.DrawLine(MajorPen, line);
				}
			}
		}

		#region ICloneable Members

		public object Clone()
		{
			return new GridStyle(this);
		}

		#endregion ICloneable Members
	}
}