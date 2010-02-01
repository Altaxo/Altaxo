using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Altaxo.Graph.Gdi.Plot.ColorProvider
{
	/// <summary>
	/// Abstract class to calculate a color out of a relative value that is normally
	/// between 0 and 1. Special colors are used here for values between 0, above 1, and for NaN.
	/// </summary>
	public abstract class ColorProviderBase : IColorProvider
	{
		/// <summary>The color used if the values are below the lower bound.</summary>
		protected Color _colorBelow;

		/// <summary>The color used if the values are above the upper bound.</summary>
		protected Color _colorAbove;

		/// <summary>The color used for invalid values (missing values).</summary>
		protected Color _colorInvalid;

		/// <summary>Alpha channel for the generated colors. Range from 0 to 255.</summary>
		protected int _alphaChannel;

		/// <summary>
		/// Fired when some of the members changed.
		/// </summary>
		public event EventHandler Changed;


		public ColorProviderBase()
		{
			_colorBelow = Color.Black;
			_colorAbove = Color.Snow;
			_colorInvalid = Color.Transparent;
			_alphaChannel = 255;
		}

		public ColorProviderBase(ColorProviderBase from)
		{
			CopyFrom(from);	
		}

		public virtual void CopyFrom(object obj)
		{
			var from = (ColorProviderBase)obj;

			this._colorBelow = from._colorBelow;
			this._colorAbove = from._colorAbove;
			this._colorInvalid = from._colorInvalid;
			this._alphaChannel = from._alphaChannel;
		}

		#region Serialization
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorProviderBase), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ColorProviderBase s = (ColorProviderBase)obj;

				info.AddValue("ColorBelow", s._colorBelow);
				info.AddValue("ColorAbove", s._colorAbove);
				info.AddValue("ColorInvalid", s._colorInvalid);
				info.AddValue("Transparency", s.Transparency);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				ColorProviderBase s = (ColorProviderBase)o;

				s._colorBelow = (System.Drawing.Color)info.GetValue("ColorBelow", parent);
				s._colorAbove = (System.Drawing.Color)info.GetValue("ColorAbove", parent);
				s._colorInvalid = (System.Drawing.Color)info.GetValue("ColorInvalid", parent);
				s.Transparency = info.GetDouble("Transparency");

				return s;
			}
		}

		#endregion



		public abstract object Clone();

		/// <summary>
		/// Gets/sets the color used when the relative value is smaller than 0.
		/// </summary>
		public System.Drawing.Color ColorBelow
		{
			get { return _colorBelow; }
			set
			{
				Color oldValue = _colorBelow;
				_colorBelow = value;

				if (_colorBelow != oldValue)
				{
					OnChanged();
				}

			}
		}

		/// <summary>
		/// Get/sets the color used when the relative value is greater than 1.
		/// </summary>
		public System.Drawing.Color ColorAbove
		{
			get { return _colorAbove; }
			set
			{
				Color oldValue = _colorAbove;

				_colorAbove = value;
				if (_colorAbove != oldValue)
				{
					OnChanged();
				}

			}
		}

		/// <summary>
		/// Gets/sets the color when the relative value is NaN.
		/// </summary>
		public System.Drawing.Color ColorInvalid
		{
			get { return _colorInvalid; }
			set
			{
				Color oldValue = _colorInvalid;
				_colorInvalid = value;
				if (_colorInvalid != oldValue)
				{
					OnChanged();
				}
			}
		}

		/// <summary>
		/// Get/sets the transparency, which is a value between 0 (full opaque) and 1 (full transparent).
		/// </summary>
		public double Transparency
		{
			get { return 1 - _alphaChannel / 255.0; }
			set
			{
				if (0 <= value && value <= 1)
					_alphaChannel = (int)(255 * (1 - value));
				else
					_alphaChannel = 255;
			}
		}

		protected void OnChanged()
		{
			if (null != Changed)
				Changed(this,EventArgs.Empty);
		}


		public virtual Color GetOutOfBoundsColor(double relVal)
		{
			if(relVal<0)
				return _colorBelow;
			else if(relVal>1)
				return _colorAbove;
			else
				return _colorInvalid;
		}

		/// <summary>
		/// Calculates a color from the provided relative value.
		/// </summary>
		/// <param name="relVal">Value used for color calculation. Normally between 0 and 1.</param>
		/// <returns>A color associated with the relative value.</returns>
		public abstract Color GetColor(double relVal);
	}

	
}
