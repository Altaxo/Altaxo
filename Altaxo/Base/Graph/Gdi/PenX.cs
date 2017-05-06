#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Drawing.DashPatternManagement;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi.LineCaps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;

namespace Altaxo.Graph.Gdi
{
    #region PenX

    /// <summary>
    /// PenHolder is a serializable surrogate for a Pen object.
    /// you can implicitly or explicitly convert it to a pen, but you must
    /// not dispose the pen object you got from this, since the ownership has still the Penholder object
    /// if you convert a Penholder to a Pen, either implicitly or explicitly,
    /// the Cached property of the PenHolder is set to true to indicate that
    /// the PenHolder is holding a Pen object
    /// </summary>
    [Serializable]
    public class PenX
        :
        Main.SuspendableDocumentNodeWithEventArgs,
        ICloneable, IDisposable
    {
        protected PenX.Configured _configuredProperties; // ORed collection of the configured properties (i.e. non-standard properties)
        protected PenType _penType; // the type of the pen
        protected PenAlignment _alignment; // Alignment of the Pen
        protected BrushX _brush; // the brush of this pen
        protected NamedColor _color; // Color of this Pen object
        protected float[] _compoundArray;
        protected DashCap _dashCap;
        protected IDashPattern _dashPattern;
        protected float _cachedDashOffset;
        protected float[] _cachedDashPattern;
        protected DashStyle _cachedDashStyle;
        protected LineCapExtension _endCap;
        protected LineJoin _lineJoin;
        protected float _miterLimit;
        protected LineCapExtension _startCap;
        protected Matrix _transformation;
        protected double _width; // Width of this Pen object

        [NonSerialized()]
        private Pen _cachedPen; // the cached pen object

        #region "ConfiguredProperties"

        [Serializable]
        [Flags]
        public enum Configured
        {
            IsNull = 0x00000,
            IsNotNull = 0x00001,
            InCachedMode = 0x00002,
            PenType = 0x00004,
            Alignment = 0x00008,
            Brush = 0x00010,
            Color = 0x00020,
            CompoundArray = 0x00040,
            DashStyle = 0x00080,
            DashCap = 0x00100,
            DashOffset = 0x00200,
            DashPattern = 0x00400,
            EndCap = 0x00800,
            StartCap = 0x01000,
            CustomEndCap = 0x02000,
            CustomStartCap = 0x04000,
            LineJoin = 0x08000,
            MiterLimit = 0x10000,
            Transform = 0x20000,
            Width = 0x40000,
            All = -1
        }

        /*
				public const int Configured.NotNull         = 0x00001;
				public const int Configured.InCachedMode    = 0x00002;
				public const int Configured.PenType         = 0x00004;
				public const int Configured.Alignment       = 0x00008;
				public const int Configured.Brush           = 0x00010;
				public const int Configured.Color           = 0x00020;
				public const int Configured.CompoundArray   = 0x00040;
				public const int Configured.DashStyle       = 0x00080;
				public const int Configured.DashCap         = 0x00100;
				public const int Configured.DashOffset      = 0x00200;
				public const int Configured.DashPattern     = 0x00400;
				public const int Configured.EndCap          = 0x00800;
				public const int Configured.StartCap        = 0x01000;
				public const int Configured.CustomEndCap    = 0x02000;
				public const int Configured.CustomStartCap  = 0x04000;
				public const int Configured.LineJoin        = 0x08000;
				public const int Configured.MiterLimit      = 0x10000;
				public const int Configured.Transform       = 0x20000;
				public const int Configured.Width           = 0x40000;
		*/

        protected static Configured _GetConfiguredPropertiesVariable(Pen pen)
        {
            Configured c = Configured.IsNull;
            if (null == pen)
                return 0; // Pen is null, so nothing is configured

            c |= Configured.IsNotNull; // Pen is at least not null
            if (pen.PenType != PenType.SolidColor) c |= Configured.PenType;
            if (pen.PenType == PenType.SolidColor && pen.Color.ToArgb() != NamedColors.Black.Color.ToArgb()) c |= Configured.Color;
            if (pen.PenType != PenType.SolidColor) c |= Configured.Brush;
            if (pen.Alignment != PenAlignment.Center) c |= Configured.Alignment;
            if (pen.CompoundArray != null && pen.CompoundArray.Length > 0) c |= Configured.CompoundArray;
            if (pen.DashStyle != DashStyle.Solid) c |= Configured.DashStyle;
            if (pen.DashStyle != DashStyle.Solid && pen.DashCap != DashCap.Flat) c |= Configured.DashCap;
            if (pen.DashStyle != DashStyle.Solid && pen.DashOffset != 0) c |= Configured.DashOffset;
            if (pen.DashStyle == DashStyle.Custom && pen.DashPattern != null) c |= Configured.DashPattern;
            if (pen.EndCap != LineCap.Flat) c |= Configured.EndCap;
            if (pen.StartCap != LineCap.Flat) c |= Configured.StartCap;
            if (pen.EndCap != LineCap.Custom) c |= Configured.CustomEndCap;
            if (pen.StartCap != LineCap.Custom) c |= Configured.CustomStartCap;
            if (pen.LineJoin != LineJoin.Miter) c |= Configured.LineJoin;
            if (pen.MiterLimit != 10) c |= Configured.MiterLimit;
            if (!pen.Transform.IsIdentity) c |= Configured.Transform;
            if (pen.Width != 1) c |= Configured.Width;

            return c;
        }

        protected static Configured _GetConfiguredPropertiesVariable(PenX pen)
        {
            Configured c = Configured.IsNull;
            if (null == pen || pen._configuredProperties == 0)
                return c; // Pen is null, so nothing is configured

            c |= Configured.IsNotNull; // Pen is at least not null
            if (pen.PenType != PenType.SolidColor) c |= Configured.PenType;
            if (pen.PenType == PenType.SolidColor && pen.Color != NamedColors.Black) c |= Configured.Color;
            if (pen.PenType != PenType.SolidColor) c |= Configured.Brush;
            if (pen.Alignment != PenAlignment.Center) c |= Configured.Alignment;
            if (pen.CompoundArray != null && pen.CompoundArray.Length > 0) c |= Configured.CompoundArray;
            if (pen._cachedDashStyle != DashStyle.Solid) c |= Configured.DashStyle;
            if (pen._cachedDashStyle != DashStyle.Solid && pen.DashCap != DashCap.Flat) c |= Configured.DashCap;
            if (pen._cachedDashStyle != DashStyle.Solid && pen._cachedDashOffset != 0) c |= Configured.DashOffset;
            if (pen._cachedDashStyle == DashStyle.Custom && pen._cachedDashPattern != null) c |= Configured.DashPattern;
            if (!pen.EndCap.IsDefaultStyle) c |= Configured.EndCap;
            if (!pen.StartCap.IsDefaultStyle) c |= Configured.StartCap;
            if (pen.LineJoin != LineJoin.Miter) c |= Configured.LineJoin;
            if (pen.MiterLimit != 10) c |= Configured.MiterLimit;
            if (null != pen.Transform && !pen.Transform.IsIdentity) c |= Configured.Transform;
            if (pen.Width != 1) c |= Configured.Width;

            return c;
        }

        #endregion "ConfiguredProperties"

        #region Serialization

        [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PenHolder", 0)]
        private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
        {
            public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
            {
                throw new ApplicationException("This point should not be reached");
                /*
				PenHolder s = (PenHolder)obj;
				Configured cp = PenHolder._GetConfiguredPropertiesVariable(s);
				if (s.Cached) cp |= PenHolder.Configured.InCachedMode;

				info.AddValue("Configured", (int)cp);
				if (0 != (cp & PenHolder.Configured.PenType))
					info.AddEnum("Type", s.PenType);
				if (0 != (cp & PenHolder.Configured.Alignment))
					info.AddEnum("Alignment", s.Alignment);
				if (0 != (cp & PenHolder.Configured.Brush))
					info.AddValue("Brush", s.BrushHolder);
				if (0 != (cp & PenHolder.Configured.Color))
					info.AddValue("Color", s.Color);
				if (0 != (cp & PenHolder.Configured.CompoundArray))
					info.AddArray("CompoundArray", s.CompoundArray, s.CompoundArray.Length);
				if (0 != (cp & PenHolder.Configured.DashStyle))
					info.AddEnum("DashStyle", s.DashStyle);
				if (0 != (cp & PenHolder.Configured.DashCap))
					info.AddEnum("DashCap", s.DashCap);
				if (0 != (cp & PenHolder.Configured.DashOffset))
					info.AddValue("DashOffset", s.DashOffset);
				if (0 != (cp & PenHolder.Configured.DashPattern))
					info.AddArray("DashPattern", s.DashPattern, s.DashPattern.Length);
				if (0 != (cp & PenHolder.Configured.EndCap))
					info.AddEnum("EndCap", s.EndCap);
				if (0 != (cp & PenHolder.Configured.LineJoin))
					info.AddEnum("LineJoin", s.LineJoin);
				if (0 != (cp & PenHolder.Configured.MiterLimit))
					info.AddValue("MiterLimit", s.MiterLimit);
				if (0 != (cp & PenHolder.Configured.StartCap))
					info.AddEnum("StartCap", s.StartCap);
				if (0 != (cp & PenHolder.Configured.Transform))
					info.AddArray("Transform", s.Transform.Elements, s.Transform.Elements.Length);
				if (0 != (cp & PenHolder.Configured.Width))
					info.AddValue("Width", s.Width);
				 */
            }

            public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
            {
                PenX s = null != o ? (PenX)o : new PenX();

                s._configuredProperties = (Configured)info.GetInt32("Configured");
                Configured cp = s._configuredProperties;

                if (0 != (cp & PenX.Configured.PenType))
                    s._penType = (PenType)info.GetEnum("Type", typeof(PenType));
                else
                    s._penType = PenType.SolidColor;

                if (0 != (cp & PenX.Configured.Alignment))
                    s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));
                else
                    s._alignment = PenAlignment.Center;

                if (0 != (cp & PenX.Configured.Brush))
                    s._brush = (BrushX)info.GetValue("Brush", s);
                else
                    s._brush = new BrushX(NamedColors.Black) { ParentObject = s };

                if (0 != (cp & PenX.Configured.Color))
                    s._color = (NamedColor)info.GetValue("Color", s);
                else
                    s._color = NamedColors.Black;

                if (0 != (cp & PenX.Configured.CompoundArray))
                    info.GetArray(out s._compoundArray);
                else
                    s._compoundArray = new float[0];

                if (0 != (cp & PenX.Configured.DashStyle))
                    s._cachedDashStyle = (DashStyle)info.GetEnum("DashStyle", typeof(DashStyle));
                else
                    s._cachedDashStyle = DashStyle.Solid;

                if (0 != (cp & PenX.Configured.DashCap))
                    s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));
                else
                    s._dashCap = DashCap.Flat;

                if (0 != (cp & PenX.Configured.DashOffset))
                    s._cachedDashOffset = (float)info.GetSingle("DashOffset");
                else
                    s._cachedDashOffset = 0;

                if (0 != (cp & PenX.Configured.DashPattern))
                    info.GetArray(out s._cachedDashPattern);
                else
                    s._cachedDashPattern = null;

                s.SetDashPatternFromCachedDashPropertiesAfterOldDeserialization();

                if (0 != (cp & PenX.Configured.EndCap))
                {
                    LineCap cap = (LineCap)info.GetEnum("EndCap", typeof(LineCap));
                    s._endCap = LineCapExtension.FromName(Enum.GetName(typeof(LineCap), cap));
                }
                else
                    s._endCap = LineCapExtension.Flat;

                if (0 != (cp & PenX.Configured.LineJoin))
                    s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));
                else
                    s._lineJoin = LineJoin.Miter;

                if (0 != (cp & PenX.Configured.MiterLimit))
                    s._miterLimit = info.GetSingle("MiterLimit");
                else
                    s._miterLimit = 10;

                if (0 != (cp & PenX.Configured.StartCap))
                {
                    LineCap cap = (LineCap)info.GetEnum("StartCap", typeof(LineCap));
                    s._startCap = LineCapExtension.FromName(Enum.GetName(typeof(LineCap), cap));
                }
                else
                    s._startCap = LineCapExtension.Flat;

                if (0 != (cp & PenX.Configured.Transform))
                {
                    float[] el;
                    info.GetArray(out el);
                    s._transformation = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
                }
                else
                    s._transformation = new Matrix();

                if (0 != (cp & PenX.Configured.Width))
                    s._width = info.GetDouble("Width");
                else
                    s._width = 1;

                if (s.ParentObject == null)
                    s.ParentObject = (Main.IDocumentNode)parent;

                return s;
            }
        }

        [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PenHolder", 1)]
        [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.PenX", 2)]
        private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
        {
            public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
            {
                throw new ApplicationException("Serialization into old versions is not supported");
                /*
				PenX s = (PenX)obj;
				Configured cp = PenX._GetConfiguredPropertiesVariable(s);
				if (s.Cached) cp |= PenX.Configured.InCachedMode;

				info.AddValue("Configured", (int)cp);
				if (0 != (cp & PenX.Configured.PenType))
					info.AddEnum("Type", s.PenType);
				if (0 != (cp & PenX.Configured.Alignment))
					info.AddEnum("Alignment", s.Alignment);
				if (0 != (cp & PenX.Configured.Brush))
					info.AddValue("Brush", s.BrushHolder);
				if (0 != (cp & PenX.Configured.Color))
					info.AddValue("Color", s.Color);
				if (0 != (cp & PenX.Configured.CompoundArray))
					info.AddArray("CompoundArray", s.CompoundArray, s.CompoundArray.Length);
				if (0 != (cp & PenX.Configured.DashStyle))
					info.AddEnum("DashStyle", s.DashStyle);
				if (0 != (cp & PenX.Configured.DashCap))
					info.AddEnum("DashCap", s.DashCap);
				if (0 != (cp & PenX.Configured.DashOffset))
					info.AddValue("DashOffset", s.DashOffset);
				if (0 != (cp & PenX.Configured.DashPattern))
					info.AddArray("DashPattern", s.DashPattern, s.DashPattern.Length);
				if (0 != (cp & PenX.Configured.EndCap))
				{
					info.AddValue("EndCap", s.EndCap.Name);
					info.AddValue("EndCapSize", s.m_EndCap.MinimumAbsoluteSizePt);
				}
				if (0 != (cp & PenX.Configured.LineJoin))
					info.AddEnum("LineJoin", s.LineJoin);
				if (0 != (cp & PenX.Configured.MiterLimit))
					info.AddValue("MiterLimit", s.MiterLimit);
				if (0 != (cp & PenX.Configured.StartCap))
				{
					info.AddValue("StartCap", s.StartCap.Name);
					info.AddValue("StartCapSize", s.m_StartCap.MinimumAbsoluteSizePt);
				}
				if (0 != (cp & PenX.Configured.Transform))
					info.AddArray("Transform", s.Transform.Elements, s.Transform.Elements.Length);
				if (0 != (cp & PenX.Configured.Width))
					info.AddValue("Width", s.Width);
			*/
            }

            public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
            {
                PenX s = null != o ? (PenX)o : new PenX();

                s._configuredProperties = (Configured)info.GetInt32("Configured");
                Configured cp = s._configuredProperties;

                if (0 != (cp & PenX.Configured.PenType))
                    s._penType = (PenType)info.GetEnum("Type", typeof(PenType));
                else
                    s._penType = PenType.SolidColor;

                if (0 != (cp & PenX.Configured.Alignment))
                    s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));
                else
                    s._alignment = PenAlignment.Center;

                if (0 != (cp & PenX.Configured.Brush))
                    s._brush = (BrushX)info.GetValue("Brush", s);
                else
                    s._brush = new BrushX(NamedColors.Black);

                s._brush.ParentObject = s;

                if (0 != (cp & PenX.Configured.Color))
                    s._color = (NamedColor)info.GetValue("Color", s);
                else
                    s._color = NamedColors.Black;

                if (0 != (cp & PenX.Configured.CompoundArray))
                    info.GetArray(out s._compoundArray);
                else
                    s._compoundArray = new float[0];

                if (0 != (cp & PenX.Configured.DashStyle))
                    s._cachedDashStyle = (DashStyle)info.GetEnum("DashStyle", typeof(DashStyle));
                else
                    s._cachedDashStyle = DashStyle.Solid;

                if (0 != (cp & PenX.Configured.DashCap))
                    s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));
                else
                    s._dashCap = DashCap.Flat;

                if (0 != (cp & PenX.Configured.DashOffset))
                    s._cachedDashOffset = (float)info.GetSingle("DashOffset");
                else
                    s._cachedDashOffset = 0;

                if (0 != (cp & PenX.Configured.DashPattern))
                    info.GetArray(out s._cachedDashPattern);
                else
                    s._cachedDashPattern = null;

                s.SetDashPatternFromCachedDashPropertiesAfterOldDeserialization();

                if (0 != (cp & PenX.Configured.EndCap))
                {
                    string name = info.GetString("EndCap");
                    var size = info.GetDouble("EndCapSize");
                    s._endCap = LineCapExtension.FromNameAndAbsAndRelSize(name, size, 2);
                }
                else
                    s._endCap = LineCapExtension.Flat;

                if (0 != (cp & PenX.Configured.LineJoin))
                    s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));
                else
                    s._lineJoin = LineJoin.Miter;

                if (0 != (cp & PenX.Configured.MiterLimit))
                    s._miterLimit = info.GetSingle("MiterLimit");
                else
                    s._miterLimit = 10;

                if (0 != (cp & PenX.Configured.StartCap))
                {
                    string name = info.GetString("StartCap");
                    var size = info.GetDouble("StartCapSize");
                    s._startCap = LineCapExtension.FromNameAndAbsAndRelSize(name, size, 2);
                }
                else
                    s._startCap = LineCapExtension.Flat;

                if (0 != (cp & PenX.Configured.Transform))
                {
                    float[] el;
                    info.GetArray(out el);
                    s._transformation = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
                }
                else
                    s._transformation = new Matrix();

                if (0 != (cp & PenX.Configured.Width))
                    s._width = info.GetDouble("Width");
                else
                    s._width = 1;

                s.ParentObject = (Main.IDocumentNode)parent;
                return s;
            }
        }

        /// <summary>
        /// 2012-03-07: New in version 3: StartCap and EndCap now have a RelativeSize property. The 'StartCapSize' and 'EndCapSize' property was renamed to 'StartCapAbsSize' and 'EndCapAbsSize'.
        /// </summary>
        [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.PenX", 3)]
        private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
        {
            public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
            {
                throw new InvalidOperationException("Serialization of old version");
                /*
				PenX s = (PenX)obj;
				Configured cp = PenX._GetConfiguredPropertiesVariable(s);
				if (s.Cached) cp |= PenX.Configured.InCachedMode;

				info.AddValue("Configured", (int)cp);
				if (0 != (cp & PenX.Configured.PenType))
					info.AddEnum("Type", s.PenType);
				if (0 != (cp & PenX.Configured.Alignment))
					info.AddEnum("Alignment", s.Alignment);
				if (0 != (cp & PenX.Configured.Brush))
					info.AddValue("Brush", s.BrushHolder);
				if (0 != (cp & PenX.Configured.Color))
					info.AddValue("Color", s.Color);
				if (0 != (cp & PenX.Configured.CompoundArray))
					info.AddArray("CompoundArray", s.CompoundArray, s.CompoundArray.Length);
				if (0 != (cp & PenX.Configured.DashStyle))
					info.AddEnum("DashStyle", s.DashStyle);
				if (0 != (cp & PenX.Configured.DashCap))
					info.AddEnum("DashCap", s.DashCap);
				if (0 != (cp & PenX.Configured.DashOffset))
					info.AddValue("DashOffset", s.DashOffset);
				if (0 != (cp & PenX.Configured.DashPattern))
					info.AddArray("DashPattern", s.DashPattern, s.DashPattern.Length);
				if (0 != (cp & PenX.Configured.EndCap))
				{
					info.AddValue("EndCap", s.EndCap.Name);
					info.AddValue("EndCapAbsSize", s._endCap.MinimumAbsoluteSizePt);
					info.AddValue("EndCapRelSize", s._endCap.MinimumRelativeSize);
				}
				if (0 != (cp & PenX.Configured.LineJoin))
					info.AddEnum("LineJoin", s.LineJoin);
				if (0 != (cp & PenX.Configured.MiterLimit))
					info.AddValue("MiterLimit", s.MiterLimit);
				if (0 != (cp & PenX.Configured.StartCap))
				{
					info.AddValue("StartCap", s.StartCap.Name);
					info.AddValue("StartCapAbsSize", s._startCap.MinimumAbsoluteSizePt);
					info.AddValue("StartCapRelSize", s._startCap.MinimumRelativeSize);
				}
				if (0 != (cp & PenX.Configured.Transform))
					info.AddArray("Transform", s.Transform.Elements, s.Transform.Elements.Length);
				if (0 != (cp & PenX.Configured.Width))
					info.AddValue("Width", s.Width);
				*/
            }

            public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
            {
                PenX s = null != o ? (PenX)o : new PenX();

                s._configuredProperties = (Configured)info.GetInt32("Configured");
                Configured cp = s._configuredProperties;

                if (0 != (cp & PenX.Configured.PenType))
                    s._penType = (PenType)info.GetEnum("Type", typeof(PenType));
                else
                    s._penType = PenType.SolidColor;

                if (0 != (cp & PenX.Configured.Alignment))
                    s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));
                else
                    s._alignment = PenAlignment.Center;

                if (0 != (cp & PenX.Configured.Brush))
                    s._brush = (BrushX)info.GetValue("Brush", s);
                else
                    s._brush = new BrushX(NamedColors.Black);

                s._brush.ParentObject = s;

                if (0 != (cp & PenX.Configured.Color))
                    s._color = (NamedColor)info.GetValue("Color", s);
                else
                    s._color = NamedColors.Black;

                if (0 != (cp & PenX.Configured.CompoundArray))
                    info.GetArray(out s._compoundArray);
                else
                    s._compoundArray = new float[0];

                if (0 != (cp & PenX.Configured.DashStyle))
                    s._cachedDashStyle = (DashStyle)info.GetEnum("DashStyle", typeof(DashStyle));
                else
                    s._cachedDashStyle = DashStyle.Solid;

                if (0 != (cp & PenX.Configured.DashCap))
                    s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));
                else
                    s._dashCap = DashCap.Flat;

                if (0 != (cp & PenX.Configured.DashOffset))
                    s._cachedDashOffset = (float)info.GetSingle("DashOffset");
                else
                    s._cachedDashOffset = 0;

                if (0 != (cp & PenX.Configured.DashPattern))
                    info.GetArray(out s._cachedDashPattern);
                else
                    s._cachedDashPattern = null;

                s.SetDashPatternFromCachedDashPropertiesAfterOldDeserialization();

                if (0 != (cp & PenX.Configured.EndCap))
                {
                    string name = info.GetString("EndCap");
                    var absSize = info.GetDouble("EndCapAbsSize");
                    var relSize = info.GetDouble("EndCapRelSize");
                    s._endCap = LineCapExtension.FromNameAndAbsAndRelSize(name, absSize, relSize);
                }
                else
                    s._endCap = LineCapExtension.Flat;

                if (0 != (cp & PenX.Configured.LineJoin))
                    s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));
                else
                    s._lineJoin = LineJoin.Miter;

                if (0 != (cp & PenX.Configured.MiterLimit))
                    s._miterLimit = info.GetSingle("MiterLimit");
                else
                    s._miterLimit = 10;

                if (0 != (cp & PenX.Configured.StartCap))
                {
                    string name = info.GetString("StartCap");
                    var absSize = info.GetDouble("StartCapAbsSize");
                    var relSize = info.GetDouble("StartCapRelSize");
                    s._startCap = LineCapExtension.FromNameAndAbsAndRelSize(name, absSize, relSize);
                }
                else
                    s._startCap = LineCapExtension.Flat;

                if (0 != (cp & PenX.Configured.Transform))
                {
                    float[] el;
                    info.GetArray(out el);
                    s._transformation = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
                }
                else
                    s._transformation = new Matrix();

                if (0 != (cp & PenX.Configured.Width))
                    s._width = info.GetDouble("Width");
                else
                    s._width = 1;

                s.ParentObject = (Main.IDocumentNode)parent;
                return s;
            }
        }

        /// <summary>
        /// 2016-10-10: New in version 4: use Altaxo.Drawing.IDashPattern instead of all the dashpattern properties.
        ///
        /// </summary>
        [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PenX), 4)]
        private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
        {
            public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
            {
                PenX s = (PenX)obj;
                Configured cp = PenX._GetConfiguredPropertiesVariable(s);
                if (s.Cached) cp |= PenX.Configured.InCachedMode;

                info.AddValue("Configured", (int)cp);
                if (0 != (cp & PenX.Configured.PenType))
                    info.AddEnum("Type", s.PenType);
                if (0 != (cp & PenX.Configured.Alignment))
                    info.AddEnum("Alignment", s.Alignment);
                if (0 != (cp & PenX.Configured.Brush))
                    info.AddValue("Brush", s.BrushHolder);
                if (0 != (cp & PenX.Configured.Color))
                    info.AddValue("Color", s.Color);
                if (0 != (cp & PenX.Configured.CompoundArray))
                    info.AddArray("CompoundArray", s.CompoundArray, s.CompoundArray.Length);

                // Note: we must even save the solid pattern if it belongs to another list than the BuiltinDefault list,
                // otherwise when deserializing we wouldn't know to which list the solid dash pattern belongs to.
                if (null != s._dashPattern && (!Drawing.DashPatterns.Solid.Instance.Equals(s._dashPattern) || !object.ReferenceEquals(DashPatternListManager.Instance.BuiltinDefault, DashPatternListManager.Instance.GetParentList(s._dashPattern))))
                {
                    info.AddValue("DashPattern", s._dashPattern);
                    if (0 != (cp & PenX.Configured.DashCap))
                        info.AddEnum("DashCap", s.DashCap);
                }

                if (0 != (cp & PenX.Configured.EndCap))
                {
                    info.AddValue("EndCap", s.EndCap.Name);
                    info.AddValue("EndCapAbsSize", s._endCap.MinimumAbsoluteSizePt);
                    info.AddValue("EndCapRelSize", s._endCap.MinimumRelativeSize);
                }
                if (0 != (cp & PenX.Configured.LineJoin))
                    info.AddEnum("LineJoin", s.LineJoin);
                if (0 != (cp & PenX.Configured.MiterLimit))
                    info.AddValue("MiterLimit", s.MiterLimit);
                if (0 != (cp & PenX.Configured.StartCap))
                {
                    info.AddValue("StartCap", s.StartCap.Name);
                    info.AddValue("StartCapAbsSize", s._startCap.MinimumAbsoluteSizePt);
                    info.AddValue("StartCapRelSize", s._startCap.MinimumRelativeSize);
                }
                if (0 != (cp & PenX.Configured.Transform))
                    info.AddArray("Transform", s.Transform.Elements, s.Transform.Elements.Length);
                if (0 != (cp & PenX.Configured.Width))
                    info.AddValue("Width", s.Width);
            }

            public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
            {
                PenX s = null != o ? (PenX)o : new PenX();

                s._configuredProperties = (Configured)info.GetInt32("Configured");
                Configured cp = s._configuredProperties;

                if (0 != (cp & PenX.Configured.PenType))
                    s._penType = (PenType)info.GetEnum("Type", typeof(PenType));
                else
                    s._penType = PenType.SolidColor;

                if (0 != (cp & PenX.Configured.Alignment))
                    s._alignment = (PenAlignment)info.GetEnum("Alignment", typeof(PenAlignment));
                else
                    s._alignment = PenAlignment.Center;

                if (0 != (cp & PenX.Configured.Brush))
                    s._brush = (BrushX)info.GetValue("Brush", s);
                else
                    s._brush = new BrushX(NamedColors.Black);

                s._brush.ParentObject = s;

                if (0 != (cp & PenX.Configured.Color))
                    s._color = (NamedColor)info.GetValue("Color", s);
                else
                    s._color = NamedColors.Black;

                if (0 != (cp & PenX.Configured.CompoundArray))
                    info.GetArray(out s._compoundArray);
                else
                    s._compoundArray = new float[0];

                if (info.CurrentElementName == "DashPattern")
                {
                    s._dashPattern = (IDashPattern)info.GetValue("DashPattern", null);
                    if (!Drawing.DashPatterns.Solid.Instance.Equals(s._dashPattern))
                    {
                        if (0 != (cp & PenX.Configured.DashCap))
                            s._dashCap = (DashCap)info.GetEnum("DashCap", typeof(DashCap));
                        else
                            s._dashCap = DashCap.Flat;
                    }
                }

                s.SetCachedDashProperties();

                if (0 != (cp & PenX.Configured.EndCap))
                {
                    string name = info.GetString("EndCap");
                    var absSize = info.GetDouble("EndCapAbsSize");
                    var relSize = info.GetDouble("EndCapRelSize");
                    s._endCap = LineCapExtension.FromNameAndAbsAndRelSize(name, absSize, relSize);
                }
                else
                    s._endCap = LineCapExtension.Flat;

                if (0 != (cp & PenX.Configured.LineJoin))
                    s._lineJoin = (LineJoin)info.GetEnum("LineJoin", typeof(LineJoin));
                else
                    s._lineJoin = LineJoin.Miter;

                if (0 != (cp & PenX.Configured.MiterLimit))
                    s._miterLimit = info.GetSingle("MiterLimit");
                else
                    s._miterLimit = 10;

                if (0 != (cp & PenX.Configured.StartCap))
                {
                    string name = info.GetString("StartCap");
                    var absSize = info.GetDouble("StartCapAbsSize");
                    var relSize = info.GetDouble("StartCapRelSize");
                    s._startCap = LineCapExtension.FromNameAndAbsAndRelSize(name, absSize, relSize);
                }
                else
                    s._startCap = LineCapExtension.Flat;

                if (0 != (cp & PenX.Configured.Transform))
                {
                    float[] el;
                    info.GetArray(out el);
                    s._transformation = new Matrix(el[0], el[1], el[2], el[3], el[4], el[5]);
                }
                else
                    s._transformation = new Matrix();

                if (0 != (cp & PenX.Configured.Width))
                    s._width = info.GetDouble("Width");
                else
                    s._width = 1;

                s.ParentObject = (Main.IDocumentNode)parent;
                return s;
            }
        }

        #endregion Serialization

        public PenX()
        {
            _dashPattern = DashPatternListManager.Instance.BuiltinDefaultSolid;
        }

        public PenX(NamedColor c)
            : this(c, 1, false)
        {
        }

        public PenX(NamedColor c, double width)
            : this(c, width, false)
        {
        }

        public PenX(NamedColor c, double width, bool bCachedMode)
        {
            this._penType = PenType.SolidColor;
            _dashPattern = DashPatternListManager.Instance.BuiltinDefaultSolid;
            this._color = c;
            this._width = width;

            this._startCap = LineCapExtension.Flat;
            this._endCap = LineCapExtension.Flat;

            _SetProp(PenX.Configured.IsNotNull, true);
            _SetProp(Configured.Color, NamedColors.Black != c);
            _SetProp(Configured.Width, 1 != width);

            if (bCachedMode)
                _SetPenVariable(new Pen(ToGdi(c), (float)width));
        }

        /// <summary>
        /// Copy constructor of PenHolder
        /// </summary>
        /// <param name="pen">the PenHolder object to copy</param>
        public PenX(PenX pen)
        {
            CopyFrom(pen);
        }

        /// <summary>
        /// Copies the properties of another instance to this instance.
        /// </summary>
        /// <param name="pen">the PenHolder object to copy</param>
        public void CopyFrom(PenX pen)
        {
            if (object.ReferenceEquals(this, pen))
                return;

            _SetPenVariable(null);

            this._configuredProperties = pen._configuredProperties;
            this._penType = pen.PenType;
            this._alignment = pen.Alignment;

            if (0 != (this._configuredProperties & Configured.Brush))
                this._brush = new BrushX(pen._brush);

            this._color = pen.Color;

            if (null != pen._compoundArray)
                this._compoundArray = (float[])pen.CompoundArray.Clone();
            else
                this._compoundArray = null;

            this._dashPattern = pen._dashPattern; // immutable
            this._dashCap = pen._dashCap;

            this._cachedDashStyle = pen._cachedDashStyle;

            if (null != pen._cachedDashPattern)
                this._cachedDashPattern = (float[])pen._cachedDashPattern.Clone();
            else
                this._cachedDashPattern = null;

            this._cachedDashOffset = pen._cachedDashOffset;

            this._endCap = pen.EndCap;
            this._lineJoin = pen.LineJoin;
            this._miterLimit = pen.MiterLimit;
            this._startCap = pen.StartCap;

            if (null != pen._transformation)
                this._transformation = pen.Transform.Clone();
            else
                this._transformation = null;

            this._width = pen.Width;

            // note: there is an problem with Pen.Clone() : if the Color of the pen
            // was set to a known color, the color of the cloned pen is the same, but no longer a known color
            // therefore we avoid the cloning of the pen here

            // if(m_CachedMode && null!=pen.m_Pen)
            //   _SetPenVariable( (Pen)pen.m_Pen.Clone() );
            // else
            //   _SetPenVariable(null);
        }

        /*
		public PenHolder(Pen pen)
			: this(pen, true)
		{
		}

		public PenHolder(Pen pen, bool bCached)
		{
			this.m_CachedMode = bCached;
			_SetPropertiesFromPen(pen);
			this.m_ConfiguredProperties = _GetConfiguredPropertiesVariable(pen);
			if (bCached)
				_SetPenVariable(_GetPenFromProperties()); // do not clone the pen because there is a problem with pen cloning with known colors (see above)
		}
		*/

        public static implicit operator System.Drawing.Pen(PenX ph)
        {
            ph.Cached = true; // if implicit conversion, we maybe are not aware that the pen is never destroyed, so _we_ control the pen by caching it
            return ph.Pen;
        }

        public Pen Pen
        {
            get
            {
                if (_cachedPen == null)
                    _cachedPen = _GetPenFromProperties();

                return _cachedPen;
            }
        }

        public bool Cached
        {
            get { return false; }
            set
            {
            }
        }

        protected void _SetPenVariable(Pen pen)
        {
            if (null != _cachedPen)
                _cachedPen.Dispose();
            _cachedPen = pen;
        }

        /*
		protected void _SetPropertiesFromPen(Pen pen)
		{
			this.m_ConfiguredProperties = _GetConfiguredPropertiesVariable(pen);
			this.m_PenType = pen.PenType;
			this.m_Alignment = pen.Alignment;
			this.m_Brush = new BrushHolder(pen.Brush, false);
			this.m_Color = pen.Color;
			this.m_CompoundArray = (float[])pen.CompoundArray.Clone();
			this.m_DashCap = pen.DashCap;
			this.m_DashOffset = pen.DashOffset;
			this.m_DashPattern = (float[])pen.DashPattern.Clone();
			this.m_DashStyle = pen.DashStyle;
			this.m_EndCap = pen.EndCap;
			this.m_LineJoin = pen.LineJoin;
			this.m_MiterLimit = pen.MiterLimit;
			this.m_StartCap = pen.StartCap;
			this.m_Transform = (Matrix)pen.Transform.Clone();
			this.m_Width = pen.Width;
		}
		*/

        private static System.Drawing.Color ToGdi(NamedColor color)
        {
            var c = color.Color;
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        protected Pen _GetPenFromProperties()
        {
            Configured cp = this._configuredProperties;

            if (0 == (cp & PenX.Configured.IsNotNull))
                return null;

            Pen pen = new Pen(ToGdi(NamedColors.Black));

            // now set the optional Pen properties
            if (0 != (cp & PenX.Configured.Width))
                pen.Width = (float)(this._width);

            if (0 != (cp & PenX.Configured.Alignment))
                pen.Alignment = _alignment;

            if (0 != (cp & PenX.Configured.Color))
                pen.Color = ToGdi(this._color);
            if (0 != (cp & PenX.Configured.Brush))
                pen.Brush = this._brush;

            if (0 != (cp & PenX.Configured.CompoundArray))
                pen.CompoundArray = this._compoundArray;
            if (0 != (cp & PenX.Configured.DashStyle))
                pen.DashStyle = this._cachedDashStyle;
            if (0 != (cp & PenX.Configured.DashCap))
                pen.DashCap = this._dashCap;
            if (0 != (cp & PenX.Configured.DashOffset))
                pen.DashOffset = this._cachedDashOffset;
            if (0 != (cp & PenX.Configured.DashPattern))
                pen.DashPattern = this._cachedDashPattern;
            if (0 != (cp & PenX.Configured.EndCap))
                this._endCap.SetEndCap(pen);
            if (0 != (cp & PenX.Configured.LineJoin))
                pen.LineJoin = this._lineJoin;
            if (0 != (cp & PenX.Configured.MiterLimit))
                pen.MiterLimit = this._miterLimit;
            if (0 != (cp & PenX.Configured.StartCap))
                this._startCap.SetStartCap(pen);
            if (0 != (cp & PenX.Configured.Transform))
                pen.Transform = this._transformation;

            return pen;
        }

        private static Configured SetProp(Configured allprop, Configured prop, bool bSet)
        {
            allprop &= (Configured.All ^ prop);
            if (bSet)
                allprop |= prop;

            return allprop;
        }

        private static bool AreEqual(float[] x1, float[] x2)
        {
            if (x1 == null && x2 == null)
                return true;
            if (x1 == null || x2 == null)
                return false;
            if (x1.Length != x2.Length)
                return false;
            for (int i = 0; i < x1.Length; i++)
                if (x1[i] != x2[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Returns all differences between two pens as a flagged enum.
        /// </summary>
        /// <param name="p1">First pen to compare.</param>
        /// <param name="p2">Second pen to comare.</param>
        /// <returns>A enum where all those bits are set where the two pens are different.</returns>
        public static Configured GetDifferences(PenX p1, PenX p2)
        {
            Configured cp1 = p1._configuredProperties;
            Configured cp2 = p2._configuredProperties;

            Configured cp = cp1 & cp2;

            // for all properties that are configured both in p1 and p2, test if they are equal
            // now set the optional Pen properties
            if (0 != (cp & PenX.Configured.IsNotNull))
                cp = SetProp(cp, Configured.IsNotNull, false);

            if (0 != (cp & PenX.Configured.Width))
                cp = SetProp(cp, PenX.Configured.Width, p1._width != p2._width);

            if (0 != (cp & PenX.Configured.Alignment))
                cp = SetProp(cp, PenX.Configured.Alignment, p1._alignment != p2._alignment);

            if (0 != (cp & PenX.Configured.Color))
                cp = SetProp(cp, PenX.Configured.Color, p1._color != p2._color);

            if (0 != (cp & PenX.Configured.Brush))
                cp = SetProp(cp, PenX.Configured.Brush, !BrushX.AreEqual(p1._brush, p2._brush));

            if (0 != (cp & PenX.Configured.CompoundArray))
                cp = SetProp(cp, PenX.Configured.CompoundArray, !AreEqual(p1._compoundArray, p2._compoundArray));

            if (0 != (cp & PenX.Configured.DashStyle))
                cp = SetProp(cp, PenX.Configured.DashStyle, p1._cachedDashStyle != p2._cachedDashStyle);

            if (0 != (cp & PenX.Configured.DashCap))
                cp = SetProp(cp, PenX.Configured.DashCap, p1._dashCap != p2._dashCap);

            if (0 != (cp & PenX.Configured.DashOffset))
                cp = SetProp(cp, PenX.Configured.DashOffset, p1._cachedDashOffset != p2._cachedDashOffset);

            if (0 != (cp & PenX.Configured.DashPattern))
                cp = SetProp(cp, PenX.Configured.DashPattern, !AreEqual(p1._cachedDashPattern, p2._cachedDashPattern));

            if (0 != (cp & PenX.Configured.EndCap))
                cp = SetProp(cp, PenX.Configured.EndCap, p1._endCap != p2._endCap);

            if (0 != (cp & PenX.Configured.LineJoin))
                cp = SetProp(cp, PenX.Configured.LineJoin, p1._lineJoin != p2._lineJoin);

            if (0 != (cp & PenX.Configured.MiterLimit))
                cp = SetProp(cp, PenX.Configured.MiterLimit, p1._miterLimit != p2._miterLimit);

            if (0 != (cp & PenX.Configured.StartCap))
                cp = SetProp(cp, PenX.Configured.StartCap, p1._startCap != p2._startCap);

            if (0 != (cp & PenX.Configured.Transform))
                cp = SetProp(cp, PenX.Configured.Transform, p1._transformation != p2._transformation);

            return cp | (cp1 ^ cp2);
        }

        public static bool AreEqual(PenX p1, PenX p2)
        {
            if (p1 == null && p2 == null)
                return true;
            if (p1 == null || p2 == null)
                return false;
            if (object.ReferenceEquals(p1, p2))
                return true;

            if (p1._configuredProperties != p2._configuredProperties)
                return false;

            Configured diff = GetDifferences(p1, p2);
            return diff == 0;
        }

        public static bool AreEqualUnlessWidth(PenX p1, PenX p2)
        {
            if (p1 == null && p2 == null)
                return true;
            if (p1 == null || p2 == null)
                return false;
            if (object.ReferenceEquals(p1, p2))
                return true;

            Configured c1 = p1._configuredProperties;
            Configured c2 = p2._configuredProperties;
            c1 = SetProp(c1, Configured.Width, false);
            c2 = SetProp(c2, Configured.Width, false);

            if (c1 != c2)
                return false;

            Configured diff = GetDifferences(p1, p2);
            diff = SetProp(diff, Configured.Width, false);
            return diff == 0;
        }

        private void _SetBrushVariable(BrushX bh)
        {
            if (null != _brush)
                _brush.Dispose();

            _brush = bh;

            if (null != _brush)
                _brush.ParentObject = this;
        }

        public PenType PenType
        {
            get { return _penType; }
        }

        public PenAlignment Alignment
        {
            get { return _alignment; }
            set
            {
                bool bChanged = (_alignment != value);
                _alignment = value;
                if (bChanged)
                {
                    _SetProp(Configured.Alignment, PenAlignment.Center != value);

                    _SetPenVariable(null);

                    EhSelfChanged(EventArgs.Empty); // Fire the Changed event
                }
            }
        }

        /// <summary>
        /// Determines if this pen is visible. It is visible if it has a visible brush.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return (_brush != null && _brush.IsVisible) || _color.Color.A != 0;
            }
        }

        /// <summary>
        /// Determines if this pen is invisible. It is invisible if it has an invisible brush or the color is transparent.
        /// </summary>
        public bool IsInvisible
        {
            get
            {
                return (_brush != null && _brush.IsInvisible) || _color.Color.A == 0;
            }
        }

        public BrushX BrushHolder
        {
            get
            {
                if (_brush == null)
                    return new BrushX(this._color);
                else
                    return _brush;
            }
            set
            {
                if (null == value)
                {
                    _SetProp(Configured.PenType, false);
                    _SetProp(Configured.Color, NamedColors.Black != _color);
                    _penType = PenType.SolidColor;
                    _SetBrushVariable(null);
                }
                else if (value.BrushType == BrushType.SolidBrush)
                {
                    _penType = PenType.SolidColor;
                    _color = value.Color;
                    _SetBrushVariable(null);

                    _SetProp(Configured.PenType, PenType.SolidColor != _penType);
                    _SetProp(Configured.Color, NamedColors.Black != _color);
                    _SetProp(Configured.Brush, false);
                } // if value is SolidBrush
                else if (value.BrushType == BrushType.HatchBrush)
                {
                    _penType = PenType.HatchFill;
                    _SetBrushVariable(new BrushX(value));

                    _SetProp(Configured.PenType, true);
                    _SetProp(Configured.Color, false);
                    _SetProp(Configured.Brush, true);
                }
                else if (value.BrushType == BrushType.TextureBrush)
                {
                    _penType = PenType.TextureFill;
                    _SetBrushVariable(new BrushX(value));

                    _SetProp(Configured.PenType, true);
                    _SetProp(Configured.Color, false);
                    _SetProp(Configured.Brush, true);
                }
                else if (value.BrushType == BrushType.LinearGradientBrush)
                {
                    _penType = PenType.LinearGradient;
                    _SetBrushVariable(new BrushX(value));

                    _SetProp(Configured.PenType, true);
                    _SetProp(Configured.Color, false);
                    _SetProp(Configured.Brush, true);
                }
                else if (value.BrushType == BrushType.PathGradientBrush)
                {
                    _penType = PenType.PathGradient;
                    _SetBrushVariable(new BrushX(value));

                    _SetProp(Configured.PenType, true);
                    _SetProp(Configured.Color, false);
                    _SetProp(Configured.Brush, true);
                }
                _SetPenVariable(null);
                EhSelfChanged(EventArgs.Empty); // Fire the Changed event
            }
        }

        public NamedColor Color
        {
            get { return _color; }
            set
            {
                bool bChanged = (_color != value);
                _color = value;
                if (bChanged)
                {
                    if (null != _brush)
                        _brush.Color = value;
                    else
                        _SetProp(Configured.Color, NamedColors.Black != value);

                    _SetPenVariable(null);

                    EhSelfChanged(EventArgs.Empty); // Fire the Changed event
                }
            }
        }

        public float[] CompoundArray
        {
            get { return _compoundArray; }
            set
            {
                _SetProp(Configured.CompoundArray, null != value && value.Length > 0);
                _compoundArray = (float[])value.Clone();
                _SetPenVariable(null);
                EhSelfChanged(EventArgs.Empty); // Fire the Changed event
            }
        }

        public DashCap DashCap
        {
            get { return _dashCap; }
            set
            {
                bool bChanged = (_dashCap != value);
                _dashCap = value;
                if (bChanged)
                {
                    _SetProp(Configured.DashCap, DashCap.Flat != value);
                    _SetPenVariable(null);
                    EhSelfChanged(EventArgs.Empty); // Fire the Changed event
                }
            }
        }

        public Altaxo.Drawing.IDashPattern DashPattern
        {
            get
            {
                return _dashPattern;
            }
            set
            {
                if (null == value)
                    throw new ArgumentNullException();

                if (!object.ReferenceEquals(_dashPattern, value))
                {
                    _dashPattern = value;
                    SetCachedDashProperties();
                    _SetPenVariable(null);
                    EhSelfChanged(EventArgs.Empty); // Fire the Changed event
                }
            }
        }

        private void SetCachedDashProperties()
        {
            var value = _dashPattern;

            bool configuredDashStyle;
            bool configuredDashPattern;
            DashStyle dashStyle;
            float[] dashPattern = null;
            float dashOffset = 0;

            if (value is Drawing.DashPatterns.Solid)
            {
                configuredDashStyle = false;
                configuredDashPattern = false;
                dashStyle = DashStyle.Solid;
            }
            else if (value is Drawing.DashPatterns.Dash)
            {
                configuredDashStyle = true;
                configuredDashPattern = false;
                dashStyle = DashStyle.Dash;
            }
            else if (value is Drawing.DashPatterns.Dot)
            {
                configuredDashStyle = true;
                configuredDashPattern = false;
                dashStyle = DashStyle.Dot;
            }
            else if (value is Drawing.DashPatterns.DashDot)
            {
                configuredDashStyle = true;
                configuredDashPattern = false;
                dashStyle = DashStyle.DashDot;
            }
            else if (value is Drawing.DashPatterns.DashDotDot)
            {
                configuredDashStyle = true;
                configuredDashPattern = false;
                dashStyle = DashStyle.DashDotDot;
            }
            else
            {
                configuredDashStyle = true;
                configuredDashPattern = true;
                dashStyle = DashStyle.Custom;
                dashPattern = value.Select(x => (float)x).ToArray();
                dashOffset = (float)value.DashOffset;
            }

            if (
                configuredDashStyle != _configuredProperties.HasFlag(Configured.DashStyle) ||
                configuredDashPattern != _configuredProperties.HasFlag(Configured.DashPattern) ||
                dashStyle != _cachedDashStyle ||
                dashOffset != _cachedDashOffset ||
                !object.ReferenceEquals(dashPattern, _cachedDashPattern))
            {
                _SetProp(Configured.DashStyle, configuredDashStyle);
                _SetProp(Configured.DashPattern, configuredDashPattern);
                _cachedDashStyle = dashStyle;
                _cachedDashPattern = dashPattern;
                _cachedDashOffset = dashOffset;
            }
        }

        /// <summary>
        /// Sets the <see cref="_dashPattern"/> member after deserialization of old versions (before 2016-10-10).
        /// </summary>
        private void SetDashPatternFromCachedDashPropertiesAfterOldDeserialization()
        {
            if (!_configuredProperties.HasFlag(Configured.DashStyle))
            {
                _dashPattern = DashPatternListManager.Instance.BuiltinDefaultSolid;
            }
            else // DashStyle is configured
            {
                switch (_cachedDashStyle)
                {
                    case DashStyle.Solid:
                        _dashPattern = DashPatternListManager.Instance.BuiltinDefaultSolid;
                        break;

                    case DashStyle.Dash:
                        _dashPattern = DashPatternListManager.Instance.BuiltinDefaultDash;
                        break;

                    case DashStyle.Dot:
                        _dashPattern = DashPatternListManager.Instance.BuiltinDefaultDot;
                        break;

                    case DashStyle.DashDot:
                        _dashPattern = DashPatternListManager.Instance.BuiltinDefaultDashDot;
                        break;

                    case DashStyle.DashDotDot:
                        _dashPattern = DashPatternListManager.Instance.BuiltinDefaultDashDotDot;
                        break;

                    case DashStyle.Custom:
                        _dashPattern = new Drawing.DashPatterns.Custom(_cachedDashPattern.Select(x => (double)x), _cachedDashOffset);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private static bool IsEqual(float[] a, float[] b)
        {
            if (a == null || b == null)
                return false;
            if (a.Length != b.Length)
                return false;
            for (int i = a.Length - 1; i >= 0; i--)
                if (a[i] != b[i])
                    return false;

            return true;
        }

        public LineCapExtension EndCap
        {
            get
            {
                return this._endCap;
            }
            set
            {
                if (null == value)
                    value = LineCapExtension.Flat;
                bool bChanged = (_endCap != value);
                _endCap = value;

                if (bChanged)
                {
                    _SetProp(Configured.EndCap, !_endCap.IsDefaultStyle);
                    _SetPenVariable(null);
                    EhSelfChanged(EventArgs.Empty); // Fire the Changed event
                }
            }
        }

        public LineJoin LineJoin
        {
            get { return _lineJoin; }
            set
            {
                bool bChanged = (_lineJoin != value);
                _lineJoin = value;
                if (bChanged)
                {
                    _SetProp(Configured.LineJoin, LineJoin.Miter != value);
                    _SetPenVariable(null);
                    EhSelfChanged(EventArgs.Empty); // Fire the Changed event
                }
            }
        }

        public float MiterLimit
        {
            get { return _miterLimit; }
            set
            {
                bool bChanged = (_miterLimit != value);
                _miterLimit = value;
                if (bChanged)
                {
                    _SetProp(Configured.MiterLimit, 10 != value);
                    _SetPenVariable(null);
                    EhSelfChanged(EventArgs.Empty); // Fire the Changed event
                }
            }
        }

        public LineCapExtension StartCap
        {
            get
            {
                return this._startCap;
            }
            set
            {
                if (null == value)
                    value = LineCapExtension.Flat;

                bool bChanged = (_startCap != value);
                _startCap = value;
                if (bChanged)
                {
                    _SetProp(Configured.StartCap, !_startCap.IsDefaultStyle);
                    _SetPenVariable(null);

                    EhSelfChanged(EventArgs.Empty); // Fire the Changed event
                }
            }
        }

        public Matrix Transform
        {
            get { return _transformation; }
            set
            {
                _SetProp(Configured.Transform, null != value && !value.IsIdentity);
                _transformation = value.Clone();
                _SetPenVariable(null);
                EhSelfChanged(EventArgs.Empty); // Fire the Changed event
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                bool bChanged = (_width != value);
                _width = value;
                if (bChanged)
                {
                    _SetProp(Configured.Width, 1 != value);
                    _SetPenVariable(null);
                    EhSelfChanged(EventArgs.Empty); // Fire the Changed event
                }
            }
        }

        private void _SetProp(Configured prop, bool bSet)
        {
            this._configuredProperties &= (Configured.All ^ prop);
            if (bSet) this._configuredProperties |= prop;
        }

        public PenX Clone()
        {
            return new PenX(this);
        }

        object ICloneable.Clone()
        {
            return new PenX(this);
        }

        protected override void Dispose(bool isDisposing)
        {
            _configuredProperties = 0;
            if (null != _cachedPen) { _cachedPen.Dispose(); _cachedPen = null; }
            if (null != _transformation) { _transformation.Dispose(); _transformation = null; }
            if (null != _compoundArray) { _compoundArray = null; }
            if (null != this._cachedDashPattern) { _cachedDashPattern = null; }

            base.Dispose(isDisposing);
        }

        #region IChangedEventSource Members

        protected virtual void OnBrushChangedEventHandler(object sender, EventArgs e)
        {
            EhSelfChanged(EventArgs.Empty);
        }

        #endregion IChangedEventSource Members

        #region Document node functions

        protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
        {
            if (null != _brush)
                yield return new Main.DocumentNodeAndName(_brush, "Brush");
        }

        #endregion Document node functions

        /// <summary>
        /// Sets the environment for the creation of the pen's brush.
        /// </summary>
        /// <param name="boundingRectangle">Bounding rectangle used for gradient textures.</param>
        /// <param name="maxEffectiveResolution">Maximum effective resolution in Dpi. This information is neccessary for repeatable texture brushes. You can calculate this using <see cref="M:BrushX.GetMaximumEffectiveResolution"/></param>
        /// <returns>True if changes to the pen's brush were made. False otherwise.</returns>
        public bool SetEnvironment(RectangleD2D boundingRectangle, double maxEffectiveResolution)
        {
            bool changed = false;
            if (_brush != null)
            {
                changed |= _brush.SetEnvironment(boundingRectangle, maxEffectiveResolution);
            }

            if (changed && null != _cachedPen)
                _cachedPen.Brush = _brush.Brush;

            return changed;
        }

        /// <summary>
        /// Sets the environment for the creation of the pen's brush.
        /// </summary>
        /// <param name="boundingRectangle">Bounding rectangle used for gradient textures.</param>
        /// <param name="maxEffectiveResolution">Maximum effective resolution in Dpi. This information is neccessary for repeatable texture brushes. You can calculate this using <see cref="M:BrushX.GetMaximumEffectiveResolution"/></param>
        /// <returns>True if changes to the pen's brush were made. False otherwise.</returns>
        public bool SetEnvironment(RectangleF boundingRectangle, double maxEffectiveResolution)
        {
            bool changed = false;
            if (_brush != null)
            {
                changed |= _brush.SetEnvironment(boundingRectangle, maxEffectiveResolution);
            }

            if (changed && null != _cachedPen)
                _cachedPen.Brush = _brush.Brush;

            return changed;
        }
    } // end of class PenHolder

    #endregion PenX

    #region DashStyleEx

    [Serializable]
    public class DashStyleEx : ICloneable
    {
        private DashStyle _knownStyle;
        private float[] _customStyle;

        public DashStyleEx(DashStyle style)
        {
            if (style == DashStyle.Custom)
                throw new ArgumentOutOfRangeException("Style must not be a custom style, use the other constructor instead");

            _knownStyle = style;
        }

        public DashStyleEx(float[] customStyle)
        {
            _customStyle = (float[])customStyle.Clone();
            _knownStyle = DashStyle.Custom;
        }

        public DashStyleEx(double[] customStyle)
        {
            _customStyle = new float[customStyle.Length];
            for (int i = 0; i < customStyle.Length; ++i)
                _customStyle[i] = (float)customStyle[i];

            _knownStyle = DashStyle.Custom;
        }

        public DashStyleEx(DashStyleEx from)
        {
            CopyFrom(from);
        }

        public void CopyFrom(DashStyleEx from)
        {
            if (object.ReferenceEquals(this, from))
                return;

            this._knownStyle = from.KnownStyle;
            this._customStyle = from._customStyle == null ? null : (float[])from._customStyle.Clone();
        }

        public DashStyleEx Clone()
        {
            return new DashStyleEx(this);
        }

        object ICloneable.Clone()
        {
            return new DashStyleEx(this);
        }

        public bool IsKnownStyle
        {
            get
            {
                return _knownStyle != DashStyle.Custom;
            }
        }

        public bool IsCustomStyle
        {
            get
            {
                return _knownStyle == DashStyle.Custom;
            }
        }

        public DashStyle KnownStyle
        {
            get
            {
                return _knownStyle;
            }
        }

        public float[] CustomStyle
        {
            get
            {
                return null == _customStyle ? null : (float[])_customStyle.Clone();
            }
        }

        public void SetPenDash(Pen pen)
        {
            pen.DashStyle = _knownStyle;
            if (IsCustomStyle)
                pen.DashPattern = (float[])this._customStyle.Clone();
        }

        private static bool IsEqual(float[] a, float[] b)
        {
            if (a == null || b == null)
                return false;
            if (a.Length != b.Length)
                return false;
            for (int i = a.Length - 1; i >= 0; i--)
                if (a[i] != b[i])
                    return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is DashStyleEx)
            {
                DashStyleEx from = (DashStyleEx)obj;

                if (this.IsKnownStyle && from.IsKnownStyle && this._knownStyle == from._knownStyle)
                    return true;
                else if (this.IsCustomStyle && from.IsCustomStyle && IsEqual(this._customStyle, from._customStyle))
                    return true;
            }
            return false;
        }

        public static bool operator ==(DashStyleEx x, DashStyleEx y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(DashStyleEx x, DashStyleEx y)
        {
            return !(x.Equals(y));
        }

        public override int GetHashCode()
        {
            if (IsCustomStyle && _customStyle != null)
                return _customStyle.GetHashCode();
            else
                return _knownStyle.GetHashCode();
        }

        public override string ToString()
        {
            if (_knownStyle != DashStyle.Custom)
                return _knownStyle.ToString();
            else
            {
                System.Text.StringBuilder stb = new System.Text.StringBuilder();
                foreach (float f in _customStyle)
                {
                    stb.Append(Altaxo.Serialization.GUIConversion.ToString(f));
                    stb.Append(";");
                }
                return stb.ToString(0, stb.Length - 1);
            }
        }

        public static DashStyleEx Solid
        {
            get { return new DashStyleEx(DashStyle.Solid); }
        }

        public static DashStyleEx Dot
        {
            get { return new DashStyleEx(DashStyle.Dot); }
        }

        public static DashStyleEx Dash
        {
            get { return new DashStyleEx(DashStyle.Dash); }
        }

        public static DashStyleEx DashDot
        {
            get { return new DashStyleEx(DashStyle.DashDot); }
        }

        public static DashStyleEx DashDotDot
        {
            get { return new DashStyleEx(DashStyle.DashDotDot); }
        }

        public static DashStyleEx LongDash
        {
            get { return new DashStyleEx(new float[] { 5.0f, 3.0f }); }
        }
    }

    #endregion DashStyleEx
}