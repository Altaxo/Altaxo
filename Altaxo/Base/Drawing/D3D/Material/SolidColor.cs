using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D.Material
{
    public class SolidColor : IMaterial
    {
        private NamedColor _color;

        public static IMaterial NoMaterial { get; private set; } = new SolidColor(NamedColors.Transparent);

        #region Serialization

        /// <summary>
        /// 2015-11-18 initial version.
        /// </summary>
        [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SolidColor), 0)]
        private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
        {
            public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
            {
                var s = (SolidColor)obj;

                info.AddValue("Color", s._color);
            }

            public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
            {
                var color = (NamedColor)info.GetValue("Color", null);
                return new SolidColor(color);
            }
        }

        #endregion Serialization

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

        public override bool Equals(object obj)
        {
            {
                // this material is considered to be equal to another material, if this material has exactly
                var other = obj as SolidColor;
                if (null != other)
                    return this.Color == other.Color;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Color.GetHashCode();
        }

        public bool Equals(IMaterial other)
        {
            var othersd = other as SolidColor;
            return null != othersd && this.Color == othersd.Color;
        }

        public IMaterial WithColor(NamedColor color)
        {
            if (color == this._color)
                return this;
            else
                return new SolidColor(color);
        }
    }
}