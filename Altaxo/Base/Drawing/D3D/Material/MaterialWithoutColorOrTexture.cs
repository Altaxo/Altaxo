using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D.Material
{
    public class MaterialWithoutColorOrTexture : IMaterial
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

        public bool Equals(IMaterial other)
        {
            return other is MaterialWithoutColorOrTexture;
        }

        public IMaterial WithColor(NamedColor color)
        {
            return this;
        }
    }
}