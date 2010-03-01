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
		/// Number of colors if colors should be stepwise shown. If zero, the color is shown continuously.
		/// </summary>
		protected int _colorSteps;

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
			_colorSteps = 0;
		}

		public ColorProviderBase(ColorProviderBase from)
		{
			CopyFrom(from);	
		}

		public virtual bool CopyFrom(object obj)
		{
			bool result = false;
			var from = obj as ColorProviderBase;
			if (null != from)
			{
				this._colorBelow = from._colorBelow;
				this._colorAbove = from._colorAbove;
				this._colorInvalid = from._colorInvalid;
				this._alphaChannel = from._alphaChannel;
				this._colorSteps = from._colorSteps;
				result = true;
			}
			return result;
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
				info.AddValue("ColorSteps", s.ColorSteps);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				ColorProviderBase s = (ColorProviderBase)o;

				s._colorBelow = (System.Drawing.Color)info.GetValue("ColorBelow", parent);
				s._colorAbove = (System.Drawing.Color)info.GetValue("ColorAbove", parent);
				s._colorInvalid = (System.Drawing.Color)info.GetValue("ColorInvalid", parent);
				s.Transparency = info.GetDouble("Transparency");
				s.ColorSteps = info.GetInt32("ColorSteps");

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
				var oldValue = _alphaChannel;

				if (0 <= value && value <= 1)
					_alphaChannel = (int)(255 * (1 - value));
				else
					_alphaChannel = 255;

				if (_alphaChannel != oldValue)
					OnChanged();
			}
		}

		/// <summary>
		/// Number of discrete colors to be shown in a stepwise manner. If the value is zero, the colors are shown continuously.
		/// </summary>
		public int ColorSteps
		{
			get
			{
				return _colorSteps;
			}
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("ColorSteps is negative");

				var oldValue = _colorSteps;
				_colorSteps = value;

				if (value != oldValue)
					OnChanged();

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
		public Color GetColor(double relVal)
		{
			if (relVal >= 0 && relVal <= 1)
			{
				if (_colorSteps > 0)
					relVal = relVal < 1 ? (Math.Floor(relVal * _colorSteps) + 0.5) / _colorSteps : (_colorSteps - 0.5) / _colorSteps;

				return GetColorFrom0To1Continuously(relVal);
			}
			else
			{
				return GetOutOfBoundsColor(relVal);
			}
		}


		/// <summary>
		/// Calculates a color from the provided relative value, that is guaranteed to be between 0 and 1
		/// </summary>
		/// <param name="relVal">Value used for color calculation. Guaranteed to be between 0 and 1.</param>
		/// <returns>A color associated with the relative value.</returns>
		protected abstract Color GetColorFrom0To1Continuously(double relVal);
	}

	
}
