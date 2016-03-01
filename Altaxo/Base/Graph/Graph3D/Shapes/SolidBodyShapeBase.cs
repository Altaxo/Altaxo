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

using Altaxo.Drawing.D3D;
using Altaxo.Graph.Graph3D.GraphicsContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Shapes
{
	using Geometry;

	/// <summary>
	///
	/// </summary>
	public abstract class SolidBodyShapeBase : GraphicBase
	{
		protected IMaterial _material = Materials.GetSolidMaterial(Drawing.NamedColors.LightGray);

		#region Serialization

		protected SolidBodyShapeBase(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		: base(info)
		{
		}

		#endregion Serialization

		public SolidBodyShapeBase()
			: base(new ItemLocationDirect())
		{
		}

		public SolidBodyShapeBase(SolidBodyShapeBase from)
			: base(from)
		{
		}

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			if (!base.CopyFrom(obj))
				return false;

			var from = obj as SolidBodyShapeBase;
			if (null != from)
			{
				_material = from._material;

				EhSelfChanged(EventArgs.Empty);
				return true;
			}

			return false;
		}

		public IMaterial Material
		{
			get
			{
				return _material;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));
				var oldValue = _material;
				_material = value;
				if (!object.ReferenceEquals(oldValue, value))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public override IHitTestObject HitTest(HitTestPointData parentHitData)
		{
			var result = base.HitTest(parentHitData);
			if (null != result)
			{
				result.DoubleClick = EhHitDoubleClick;
			}
			return result;
		}

		protected static bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Shape properties", true);
			return true;
		}
	}
}