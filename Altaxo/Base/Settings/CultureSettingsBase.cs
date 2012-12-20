using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
namespace Altaxo.Settings
{
	/// <summary>
	/// Manages the settings for a culture, i.e. number and DateTime formats etc.
	/// </summary>
	public abstract class CultureSettingsBase : ICloneable
	{
		protected static readonly int InvariantCultureID = CultureInfo.InvariantCulture.LCID;


		/// <summary>Value that uniquely identifies a culture.</summary>
		protected int _cultureID;

		/// <summary>Gets or sets the name of the culture (with region identifier).</summary>
		protected string _cultureName;

		/// <summary>Gets or sets the number decimal separator.</summary>
		protected string _numberDecimalSeparator;

		/// <summary>Gets or sets the number group separator.</summary>
		protected string _numberGroupSeparator;

		protected CultureInfo _cachedCultureAsReadOnly;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CultureSettingsBase), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (CultureSettingsBase)obj;
				info.AddValue("CultureID", s._cultureID);
				info.AddValue("CultureName", s._cultureName);
				info.AddValue("NumberDecimalSeparator", s._numberDecimalSeparator);
				info.AddValue("NumberGroupSeparator", s._numberGroupSeparator);
			}
			protected virtual CultureSettingsBase SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (CultureSettingsBase)o;

				s._cultureID = info.GetInt32("CultureID");
				s._cultureName = info.GetString("CultureName");
				s._numberDecimalSeparator = info.GetString("NumberDecimalSeparator");
				s._numberGroupSeparator = info.GetString("NumberGroupSeparator");
				s.SetCachedCultureInfo();

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion


		/// <summary>Initializes a new instance of the <see cref="UICultureSettings"/> class with nothing initialized.</summary>
		protected CultureSettingsBase()
		{
		}

		public CultureSettingsBase(CultureInfo c)
		{
			_cachedCultureAsReadOnly = CultureInfo.ReadOnly(c);
			_cultureID = c.LCID;
			if (c.LCID == CultureInfo.InvariantCulture.LCID)
				_cultureName = c.ThreeLetterISOLanguageName;
			else
				_cultureName = c.Name;
			_numberDecimalSeparator = c.NumberFormat.NumberDecimalSeparator;
			_numberGroupSeparator = c.NumberFormat.NumberGroupSeparator;
		}

		/// <summary>Copies from another instance.</summary>
		/// <param name="from">Other instance to copy from.</param>
		protected CultureSettingsBase(CultureSettingsBase from)
		{
			this._cultureID = from._cultureID;
			this._cultureName = from._cultureName;
			this._numberDecimalSeparator = from._numberDecimalSeparator;
			this._numberGroupSeparator = from._numberGroupSeparator;
			this._cachedCultureAsReadOnly = from._cachedCultureAsReadOnly;
		}

		public abstract object Clone();





		public int CultureID
		{
			get
			{
				return _cultureID;
			}
		}

		/// <summary>Gets or sets the name of the culture.</summary>
		/// <value>The name of the culture in the format &lt;languagecode2&gt;-&lt;country/regioncode2&gt; (same format than in <see cref="P:System.Globalization.CultureInfo.Name"/>).</value>
		public string CultureName
		{
			get { return _cultureName; }

		}

		/// <summary>Gets the neutral name of the culture, i.e. without region identifier..</summary>
		/// <value>The name of the neutral culture.</value>
		public string NeutralCultureName
		{
			get
			{
				var result = CultureName.Split(new char[] { '-' });
				return result[0];
			}
		}


		/// <summary>Gets or sets the number decimal separator.</summary>
		/// <value>The number decimal separator.</value>
		public string NumberDecimalSeparator
		{
			get { return _numberDecimalSeparator; }

		}

		/// <summary>Gets or sets the number group separator.</summary>
		/// <value>The number group separator.</value>
		public string NumberGroupSeparator
		{
			get { return _numberGroupSeparator; }

		}


		public CultureInfo Culture
		{
			get
			{
				return _cachedCultureAsReadOnly;
			}
		}


		/// <summary>Assembles the members of this instance to a resulting culture.</summary>
		/// <returns>A culture info which contains the values of the member variables.</returns>
		public virtual void SetCachedCultureInfo()
		{
			CultureInfo result;
			try
			{
				result = new CultureInfo(_cultureID);
			}
			catch (CultureNotFoundException)
			{
				result = (CultureInfo)CultureInfo.InvariantCulture.Clone();
			}
			result.NumberFormat.NumberDecimalSeparator = NumberDecimalSeparator;
			result.NumberFormat.NumberGroupSeparator = NumberGroupSeparator;

			_cachedCultureAsReadOnly = CultureInfo.ReadOnly(result);
		}



	}
}
