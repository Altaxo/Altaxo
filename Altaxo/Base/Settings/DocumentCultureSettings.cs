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
	public class DocumentCultureSettings : CultureSettingsBase
	{
		/// <summary>
		/// Storage path for storing this instance in the application properties.
		/// </summary>
		public static string SettingsStoragePath = "Altaxo.Options.DocumentCulture";


		/// <summary>
		/// Original UI culture, must be initialized before it can be used.
		/// </summary>
		static DocumentCultureSettings _systemSettings;

		static DocumentCultureSettings _userSettings;



		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DocumentCultureSettings), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				info.AddBaseValueEmbedded(obj, typeof(DocumentCultureSettings).BaseType);
			}
			protected virtual DocumentCultureSettings SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (DocumentCultureSettings)o : new DocumentCultureSettings();
				info.GetBaseValueEmbedded(s, typeof(DocumentCultureSettings).BaseType, parent);
				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion


		/// <summary>Initializes a new instance of the <see cref="DocumentCultureSettings"/> class with nothing initialized.</summary>
		private DocumentCultureSettings()
		{
		}


		public DocumentCultureSettings(CultureInfo c)
			: base(c)
		{
		}

		public DocumentCultureSettings(DocumentCultureSettings dcs)
			: base(dcs)
		{
		}


		/// <summary>Gets or sets a value indicating whether to override the parent culture or use it.</summary>
		/// <value> If <see langword="true"/>, the culture that is stored in this object is applied. Otherwise, the culture of the superior object (for instance the operating system) is used.</value>
		public static bool UseCustomUserSettings
		{
			get { return null != _userSettings; }
		}

		public static DocumentCultureSettings UserDefault
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
					_userSettings = (DocumentCultureSettings)value.Clone();
				}
				else
				{
					_userSettings = null;
				}

				System.Threading.Thread.CurrentThread.CurrentCulture = UserDefault._cachedCultureAsReadOnly;
				Current.PropertyService.Set<DocumentCultureSettings>(SettingsStoragePath, _userSettings);
			}
		}

		public static DocumentCultureSettings SystemDefault
		{
			get
			{
				return _systemSettings;
			}
		}


		/// <summary>Initializes this class with the original culture (i.e. the culture that the user has chosen in system control panel). This must be done before the current UI culture is changed by some routine.</summary>
		/// <param name="originalCulture">The original culture.</param>
		public static void InitializeSystemSettings(CultureInfo originalCulture)
		{
			_systemSettings = new DocumentCultureSettings(originalCulture);
		}

		public static void InitializeUserSettings()
		{
			UserDefault = Current.PropertyService.Get<DocumentCultureSettings>(SettingsStoragePath, null);
		}



		/// <summary>Creates a new object that is a copy of the current instance.</summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return new DocumentCultureSettings(this);
		}
	}
}
