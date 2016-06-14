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

namespace Altaxo.Drawing.D3D
{
	public class PenX3D : Altaxo.Main.IImmutable
	{
		#region Member variables

		private IMaterial _material;

		private ICrossSectionOfLine _crossSection;

		private IDashPattern _dashPattern;

		private ILineCap _lineStartCap;

		private ILineCap _lineEndCap;

		private ILineCap _dashStartCap;

		private bool _dashStartCapSuppressionIfSpaceInsufficient;

		private ILineCap _dashEndCap;

		private bool _dashEndCapSuppressionIfSpaceInsufficient;

		private PenLineJoin _lineJoin = PenLineJoin.Miter;

		private double _miterLimit = 10;

		#endregion Member variables

		#region Serialization

		/// <summary>
		/// 2015-11-14 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PenX3D), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (PenX3D)obj;

				info.AddValue("Material", s._material);
				info.AddValue("CrossSection", s._crossSection);

				info.AddEnum("LineJoin", s._lineJoin);
				info.AddValue("MiterLimit", s._miterLimit);

				if (null != s._lineStartCap)
					info.AddValue("LineStartCap", s._lineStartCap);

				if (null != s._lineEndCap)
					info.AddValue("LineEndCap", s._lineEndCap);

				if (null != s._dashPattern)
				{
					info.AddValue("DashPattern", s._dashPattern);

					if (null != s._dashStartCap)
					{
						info.AddValue("DashStartCap", s._dashStartCap);
						info.AddValue("DashStartCapSuppression", s._dashStartCapSuppressionIfSpaceInsufficient);
					}
					if (null != s._dashEndCap)
					{
						info.AddValue("DashEndCap", s._dashEndCap);
						info.AddValue("DashEndCapSuppression", s._dashEndCapSuppressionIfSpaceInsufficient);
					}
				}
			}

			protected virtual PenX3D SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var material = (IMaterial)info.GetValue("Material", null);
				var crossSection = (ICrossSectionOfLine)info.GetValue("CrossSection", null);

				var lineJoin = (PenLineJoin)info.GetEnum("LineJoin", typeof(PenLineJoin));
				double miterLimit = info.GetDouble("MiterLimit");

				var lineStartCap = ("LineStartCap" == info.CurrentElementName) ? (ILineCap)info.GetValue("LineStartCap", null) : null;
				var lineEndCap = ("LineEndCap" == info.CurrentElementName) ? (ILineCap)info.GetValue("LineEndCap", null) : null;
				var dashPattern = ("DashPattern" == info.CurrentElementName) ? (IDashPattern)info.GetValue("DashPattern", null) : null;
				ILineCap dashStartCap = null, dashEndCap = null;
				bool dashStartCapSuppression = false, dashEndCapSuppression = false;
				if (null != dashPattern)
				{
					if ("DashStartCap" == info.CurrentElementName)
					{
						dashStartCap = (ILineCap)info.GetValue("DashStartCap", null);
						dashStartCapSuppression = info.GetBoolean("DashStartCapSuppression");
					}

					if ("DashEndCap" == info.CurrentElementName)
					{
						dashEndCap = (ILineCap)info.GetValue("DashEndCap", null);
						dashEndCapSuppression = info.GetBoolean("DashEndCapSuppression");
					}
				}

				return new PenX3D(material, crossSection, lineJoin, miterLimit, lineStartCap, lineEndCap, dashPattern, dashStartCap, dashStartCapSuppression, dashEndCap, dashEndCapSuppression);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Serialization

		public PenX3D(NamedColor color, double thickness)
		{
			_material = Materials.GetSolidMaterial(color);
			_crossSection = new CrossSections.Rectangular(thickness, thickness);
		}

		public PenX3D(IMaterial material, ICrossSectionOfLine crossSection)
		{
			_material = material;
			_crossSection = crossSection;
		}

		public PenX3D(
			IMaterial material,
			ICrossSectionOfLine crossSection,
			PenLineJoin lineJoin,
			double miterLimit,
			ILineCap lineStartCap, ILineCap lineEndCap, IDashPattern dashPattern, ILineCap dashStartCap, bool dashStartCapSuppressionIfSpaceInsufficient, ILineCap dashEndCap, bool dashEndCapSuppressionIfSpaceInsufficient)
		{
			if (!(miterLimit >= 1))
				throw new ArgumentOutOfRangeException(nameof(miterLimit), "must be >= 1");

			_material = material;
			_crossSection = crossSection;
			_lineJoin = lineJoin;
			_miterLimit = miterLimit;
			_lineStartCap = lineStartCap;
			_lineEndCap = lineEndCap;
			_dashPattern = dashPattern;
			_dashStartCap = dashStartCap;
			_dashStartCapSuppressionIfSpaceInsufficient = dashStartCapSuppressionIfSpaceInsufficient;
			_dashEndCap = dashEndCap;
			_dashEndCapSuppressionIfSpaceInsufficient = dashEndCapSuppressionIfSpaceInsufficient;
		}

		public double Thickness1
		{
			get
			{
				return _crossSection.Size1;
			}
		}

		public PenX3D WithThickness1(double thickness1)
		{
			var newCrossSection = _crossSection.WithSize1(thickness1);
			if (!object.ReferenceEquals(newCrossSection, _crossSection))
			{
				var result = (PenX3D)this.MemberwiseClone();
				result._crossSection = newCrossSection;
				return result;
			}
			else
			{
				return this;
			}
		}

		public double Thickness2
		{
			get
			{
				return _crossSection.Size2;
			}
		}

		public PenX3D WithThickness2(double thickness2)
		{
			var newCrossSection = _crossSection.WithSize2(thickness2);
			if (!object.ReferenceEquals(newCrossSection, _crossSection))
			{
				var result = (PenX3D)this.MemberwiseClone();
				result._crossSection = newCrossSection;
				return result;
			}
			else
			{
				return this;
			}
		}

		public PenX3D WithUniformThickness(double thickness)
		{
			var newCrossSection = _crossSection.WithSize(thickness, thickness);
			if (!object.ReferenceEquals(newCrossSection, _crossSection))
			{
				var result = (PenX3D)this.MemberwiseClone();
				result._crossSection = newCrossSection;
				return result;
			}
			else
			{
				return this;
			}
		}

		public IMaterial Material
		{
			get
			{
				return _material;
			}
		}

		public PenX3D WithMaterial(IMaterial material)
		{
			if (null == material)
				throw new ArgumentNullException(nameof(material));

			var result = (PenX3D)this.MemberwiseClone();
			result._material = material;
			return result;
		}

		public NamedColor Color
		{
			get
			{
				return _material.Color;
			}
		}

		public PenX3D WithColor(NamedColor color)
		{
			var result = (PenX3D)this.MemberwiseClone();
			result._material = Materials.GetMaterialWithNewColor(result._material, color);
			return result;
		}

		public ICrossSectionOfLine CrossSection
		{
			get
			{
				return _crossSection;
			}
		}

		public PenX3D WithCrossSection(ICrossSectionOfLine crossSection)
		{
			if (!object.ReferenceEquals(crossSection, _crossSection))
			{
				var result = (PenX3D)this.MemberwiseClone();
				result._crossSection = crossSection;
				return result;
			}
			else
			{
				return this;
			}
		}

		#region Line Join

		public PenLineJoin LineJoin
		{
			get
			{
				return _lineJoin;
			}
		}

		public PenX3D WithLineJoin(PenLineJoin lineJoin)
		{
			if (!(lineJoin == _lineJoin))
			{
				var result = (PenX3D)this.MemberwiseClone();
				result._lineJoin = lineJoin;
				return result;
			}
			else
			{
				return this;
			}
		}

		public double MiterLimit
		{
			get
			{
				return _miterLimit;
			}
		}

		#endregion Line Join

		#region Dash Pattern

		/// <summary>
		/// Gets the dash pattern (or null if this pen doesn't use a dash pattern).
		/// </summary>
		/// <value>
		/// The dash pattern that this pen used. If the pen represents a solid line, the return value is null.
		/// </value>
		public IDashPattern DashPattern
		{
			get
			{
				return _dashPattern;
			}
		}

		/// <summary>
		/// Returns a new instance of this pen, with the dash pattern provided in the argument.
		/// </summary>
		/// <param name="dashPattern">The dash pattern. Can be null to represent a solid line.</param>
		/// <returns>A new instance of this pen, with the dash pattern provided in the argument.</returns>
		public PenX3D WithDashPattern(IDashPattern dashPattern)
		{
			if (dashPattern is DashPatterns.Solid)
				dashPattern = null;
			if (object.Equals(_dashPattern, dashPattern))
				return this;

			var result = (PenX3D)this.MemberwiseClone();
			result._dashPattern = dashPattern;
			return result;
		}

		#endregion Dash Pattern

		#region Line Start cap

		public ILineCap LineStartCap
		{
			get
			{
				return _lineStartCap;
			}
		}

		public PenX3D WithLineStartCap(ILineCap cap)
		{
			if (cap is LineCaps.Flat)
				cap = null;

			if (object.Equals(_lineStartCap, cap))
				return this;

			var result = (PenX3D)this.MemberwiseClone();
			result._lineStartCap = cap;
			return result;
		}

		#endregion Line Start cap

		#region Line end cap

		public ILineCap LineEndCap
		{
			get
			{
				return _lineEndCap;
			}
		}

		public PenX3D WithLineEndCap(ILineCap cap)
		{
			if (cap is LineCaps.Flat)
				cap = null;

			if (object.Equals(_lineEndCap, cap))
				return this;

			var result = (PenX3D)this.MemberwiseClone();
			result._lineEndCap = cap;
			return result;
		}

		#endregion Line end cap

		#region Dash start cap

		public ILineCap DashStartCap
		{
			get
			{
				return _dashStartCap;
			}
		}

		public PenX3D WithDashStartCap(ILineCap cap)
		{
			if (cap is LineCaps.Flat)
				cap = null;

			if (object.Equals(_dashStartCap, cap))
				return this;

			var result = (PenX3D)this.MemberwiseClone();
			result._dashStartCap = cap;
			return result;
		}

		#endregion Dash start cap

		#region Dash end cap

		public ILineCap DashEndCap
		{
			get
			{
				return _dashEndCap;
			}
		}

		public PenX3D WithDashEndCap(ILineCap cap)
		{
			if (cap is LineCaps.Flat)
				cap = null;

			if (object.Equals(_dashEndCap, cap))
				return this;

			var result = (PenX3D)this.MemberwiseClone();
			result._dashEndCap = cap;
			return result;
		}

		#endregion Dash end cap

		public static bool AreEqualUnlessThickness(PenX3D pen1, PenX3D pen2)
		{
			bool isEqual =

				pen1.Material == pen2.Material &&
				pen1.CrossSection.GetType() == pen2.CrossSection.GetType() &&
				object.Equals(pen1._dashPattern, pen2._dashPattern) &&
				object.Equals(pen1._lineStartCap, pen2._lineStartCap) &&
				object.Equals(pen1._lineEndCap, pen2._lineEndCap) &&
				object.Equals(pen1._dashStartCap, pen2._dashStartCap) &&
				object.Equals(pen1._dashEndCap, pen2._dashEndCap);

			return isEqual;
		}
	}
}