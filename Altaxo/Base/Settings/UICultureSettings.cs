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
	public class UICultureSettings : CultureSettingsBase
	{
		/// <summary>
		/// Storage path for storing this instance in the application properties.
		/// </summary>
		public static string SettingsStoragePath = "Altaxo.Options.UICulture";


		/// <summary>
		/// Original UI culture, must be initialized before it can be used.
		/// </summary>
		static UICultureSettings _systemSettings;

		static UICultureSettings _userSettings;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(UICultureSettings), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				info.AddBaseValueEmbedded(obj, typeof(UICultureSettings).BaseType);
			}
			protected virtual UICultureSettings SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (UICultureSettings)o : new UICultureSettings();
				info.GetBaseValueEmbedded(s, typeof(UICultureSettings).BaseType, parent);
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
		private UICultureSettings()
		{
		}



		public UICultureSettings(CultureInfo c)
			: base(c)
		{
		}

		public UICultureSettings(UICultureSettings dcs)
			: base(dcs)
		{
		}


		/// <summary>Gets or sets a value indicating whether to override the parent culture or use it.</summary>
		/// <value> If <see langword="true"/>, the culture that is stored in this object is applied. Otherwise, the culture of the superior object (for instance the operating system) is used.</value>
		public static bool UseCustomUserSettings
		{
			get { return null != _userSettings; }
		}

		public static UICultureSettings UserDefault
		{
			get
			{
				if (null != _userSettings)
					return _userSettings;
				else
					return _systemSettings;
			}
			set
			{
				if (null != value)
				{
					_userSettings = (UICultureSettings)value.Clone();
				}
				else
				{
					_userSettings = null;
				}

				// first we set the properties that Sharpdevelop awaits to change its language,
				Current.PropertyService.Set("CoreProperties.UILanguage", UserDefault.NeutralCultureName);
				Current.PropertyService.Set(SettingsStoragePath, _userSettings);
				System.Threading.Thread.CurrentThread.CurrentUICulture = UserDefault.Culture;
				Altaxo.Serialization.GUIConversion.CultureSettings = UserDefault.Culture;
			}
		}

		public static UICultureSettings SystemDefault
		{
			get
			{
				return _systemSettings;
			}
		}

		/// <summary>Initializes this class with the original UI culture (i.e. the culture that the user has chosen in system control panel). This must be done before the current UI culture is changed by some routine.</summary>
		/// <param name="originalCulture">The original culture.</param>
		public static void InitializeSystemSettings(CultureInfo originalCulture)
		{
			_systemSettings = new UICultureSettings(originalCulture);
		}


		public static void InitializeUserSettings()
		{
			UserDefault = Current.PropertyService.Get<UICultureSettings>(SettingsStoragePath, null);
		}




		/// <summary>Creates a new object that is a copy of the current instance.</summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return new UICultureSettings(this);
		}

	}
}
