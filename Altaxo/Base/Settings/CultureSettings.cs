#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Globalization;
using System.Linq;
using System.Text;

namespace Altaxo.Settings
{
	using Altaxo.Main.Properties;

	/// <summary>
	/// Manages the settings for a culture, i.e. number and DateTime formats etc.
	/// </summary>
	public class CultureSettings : Main.ICopyFrom
	{
		public static readonly PropertyKey<CultureSettings> PropertyKeyDocumentCulture = new PropertyKey<CultureSettings>("04A3950C-1AA0-4E66-A734-A278C51BD04B", "Language\\DocumentCulture", PropertyLevel.All, typeof(object), () => new CultureSettings(CultureSettingsAtStartup.StartupDocumentCultureInfo)) { ApplicationAction = ApplyDocumentCulture };

		public static readonly PropertyKey<CultureSettings> PropertyKeyUICulture = new PropertyKey<CultureSettings>("AB6F72E7-2879-47F4-9F79-3D2A0F7C1C55", "Language\\UICulture", PropertyLevel.Application, () => new CultureSettings(CultureSettingsAtStartup.StartupUICultureInfo)) { ApplicationAction = ApplyDocumentCulture };

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

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CultureSettings), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (CultureSettings)obj;
				info.AddValue("CultureID", s._cultureID);
				info.AddValue("CultureName", s._cultureName);
				info.AddValue("NumberDecimalSeparator", s._numberDecimalSeparator);
				info.AddValue("NumberGroupSeparator", s._numberGroupSeparator);
			}

			protected virtual CultureSettings SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (CultureSettings)o : new CultureSettings();

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

		#endregion Serialization

		/// <summary>Initializes a new instance of the <see cref="CultureSettings"/> class with nothing initialized.</summary>
		protected CultureSettings()
		{
		}

		public CultureSettings(CultureInfo c)
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
		protected CultureSettings(CultureSettings from)
		{
			CopyFrom(from);
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(obj, this))
				return true;
			var from = obj as CultureSettings;
			if (null != from)
			{
				this._cultureID = from._cultureID;
				this._cultureName = from._cultureName;
				this._numberDecimalSeparator = from._numberDecimalSeparator;
				this._numberGroupSeparator = from._numberGroupSeparator;
				this._cachedCultureAsReadOnly = from._cachedCultureAsReadOnly;
				return true;
			}
			return false;
		}

		public CultureSettings Clone()
		{
			return new CultureSettings(this);
		}

		object ICloneable.Clone()
		{
			return new CultureSettings(this);
		}

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

		public override string ToString()
		{
			return string.Format("{0} | {1} | {2}", _cultureName, _numberDecimalSeparator, _numberGroupSeparator);
		}

		#region Static methods

		public static void ApplyUICulture(CultureSettings culture)
		{
			// first we set the properties that Sharpdevelop awaits to change its language,
			Current.PropertyService.Set("CoreProperties.UILanguage", culture.NeutralCultureName);
			System.Threading.Thread.CurrentThread.CurrentUICulture = culture.Culture;
			Altaxo.Serialization.GUIConversion.CultureSettings = culture.Culture;
		}

		public static void ApplyDocumentCulture(CultureSettings culture)
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = culture.Culture;
		}

		#endregion Static methods
	}

	/// <summary>
	/// Static helper class to provide the current Gui culture both at runtime, but also at design time.
	/// </summary>
	public static class GuiCulture
	{
		/// <summary>
		/// Gets the UI culture (both at design time and at runtime).
		/// </summary>
		/// <returns>The current UI culture.</returns>
		public static CultureInfo Instance
		{
			get
			{
				if (Current.PropertyService == null)
					return System.Globalization.CultureInfo.CurrentUICulture;
				else
					return Current.PropertyService.GetValue(Altaxo.Settings.CultureSettings.PropertyKeyUICulture, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin).Culture ?? System.Globalization.CultureInfo.CurrentUICulture;
			}
		}
	}

	/// <summary>
	/// Static helper class to store the culture settings at startup of Altaxo.
	/// </summary>
	public static class CultureSettingsAtStartup
	{
		private static CultureInfo _startupDocumentCultureInfo;
		private static CultureInfo _startupUICultureInfo;

		public static CultureInfo StartupDocumentCultureInfo
		{
			get { return (CultureInfo)_startupDocumentCultureInfo.Clone(); }
			set
			{
				if (null != _startupDocumentCultureInfo)
					throw new InvalidOperationException("Value already set, but it can be set only once at startup");
				_startupDocumentCultureInfo = (CultureInfo)value.Clone();
			}
		}

		public static CultureInfo StartupUICultureInfo
		{
			get { return _startupUICultureInfo; }
			set
			{
				if (null != _startupUICultureInfo)
					throw new InvalidOperationException("Value already set, but it can be set only once at startup");
				_startupUICultureInfo = (CultureInfo)value.Clone();
			}
		}
	}
}