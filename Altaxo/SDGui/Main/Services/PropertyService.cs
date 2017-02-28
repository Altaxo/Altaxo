using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;
using Altaxo.Main.Properties;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Xml;

namespace Altaxo.Main.Services
{
	public class PropertyService : Altaxo.Main.Services.IPropertyService, ICSharpCode.Core.IPropertyService
	{
		#region SharpDevelops member

		/// <summary>
		/// Key to store the <see cref="properties"/> collection inside the Altaxo user settings.
		/// </summary>
		private const string SharpDevelopsPropertiesKey = "BDD7395F-088C-4D14-9B41-4996AC0A4996_App-SDProperties";

		private readonly ICSharpCode.Core.Properties properties;

		private DirectoryName dataDirectory;
		private DirectoryName configDirectory;
		private FileName propertiesFileName;

		#endregion SharpDevelops member

		#region Altaxo members

		/// <summary>
		/// The user settings bag is a bag that loads the properties lazy (as Xml string) during creation of this service.
		/// See remarks for why this is neccessary.
		/// </summary>
		/// <remarks>The properties in UserSettings are lazy loaded, i.e. the property values are stored as string first. Only if the property is needed, the xml string
		/// is converted into a regular value. This is neccessary, because some of the property values will required access
		/// to other property values during deserialization. But the user settings are deserialized in the constructor of the property service,
		/// and the property service is not installed in this moment. This will lead to NullReferenceException when the first deserialized
		/// property value try to get the property service.</remarks>
		protected PropertyBagLazyLoaded _userSettings;

		public IPropertyBag UserSettings { get { return _userSettings; } }

		public IPropertyBag ApplicationSettings { get; private set; }

		public IPropertyBag BuiltinSettings { get; private set; }

		#endregion Altaxo members

		public PropertyService(DirectoryName configDirectory, DirectoryName dataDirectory, string propertiesName)
		{
			this.dataDirectory = dataDirectory;
			this.configDirectory = configDirectory;
			this.propertiesFileName = configDirectory.CombineFile(propertiesName + ".xml");

			_userSettings = InternalLoadUserSettingsBag(propertiesFileName);
			ApplicationSettings = new PropertyBag() { ParentObject = SuspendableDocumentNode.StaticInstance };
			BuiltinSettings = new PropertyBag() { ParentObject = SuspendableDocumentNode.StaticInstance };

			if (!UserSettings.TryGetValue(SharpDevelopsPropertiesKey, out properties))
			{
				properties = new ICSharpCode.Core.Properties();
			}
			properties.PropertyChanged += EhSharpDevelopProperties_Changed;
		}

		private void EhSharpDevelopProperties_Changed(object sender, PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		#region Methods/properties common to both property services

		public event PropertyChangedEventHandler PropertyChanged;

		string Altaxo.Main.Services.IPropertyService.ConfigDirectory
		{
			get
			{
				return configDirectory;
			}
		}

		public DirectoryName ConfigDirectory
		{
			get
			{
				return configDirectory;
			}
		}

		public T Get<T>(string key, T defaultValue)
		{
			if (UserSettings.TryGetValue<T>(key, out var result))
				return result;
			else if (ApplicationSettings.TryGetValue<T>(key, out result))
				return result;
			else if (BuiltinSettings.TryGetValue<T>(key, out result))
				return result;
			else
				return defaultValue;
		}

		public void Set<T>(string key, T value)
		{
			UserSettings.SetValue<T>(key, value);
		}

		#endregion Methods/properties common to both property services

		#region Altaxo's IPropertyService

		public string Get(string property)
		{
			return Get<string>(property, null);
		}

		public T GetValue<T>(PropertyKey<T> p, RuntimePropertyKind kind)
		{
			if (kind == RuntimePropertyKind.UserAndApplicationAndBuiltin && UserSettings.TryGetValue<T>(p, out var result))
				return result;
			else if (kind == RuntimePropertyKind.ApplicationAndBuiltin && ApplicationSettings.TryGetValue<T>(p, out result))
				return result;
			else if (BuiltinSettings.TryGetValue<T>(p, out result))
				return result;
			else
				throw new ArgumentOutOfRangeException(nameof(p), string.Format("No entry found for property key {0}", p));
		}

		public T GetValue<T>(PropertyKey<T> p, RuntimePropertyKind kind, Func<T> ValueCreationIfNotFound)
		{
			if (kind == RuntimePropertyKind.UserAndApplicationAndBuiltin && UserSettings.TryGetValue<T>(p, out var result))
				return result;
			else if (kind == RuntimePropertyKind.ApplicationAndBuiltin && ApplicationSettings.TryGetValue<T>(p, out result))
				return result;
			else if (BuiltinSettings.TryGetValue<T>(p, out result))
				return result;
			else if (null != ValueCreationIfNotFound)
				return ValueCreationIfNotFound();
			else
				throw new ArgumentOutOfRangeException(nameof(p), string.Format("No entry found for property key {0}", p));
		}

		public void SetValue<T>(PropertyKey<T> p, T value)
		{
			UserSettings.SetValue(p, value);
		}

		#endregion Altaxo's IPropertyService

		#region SharpDevelop's IPropertyService

		public DirectoryName DataDirectory
		{
			get
			{
				return dataDirectory;
			}
		}

		public ICSharpCode.Core.Properties MainPropertiesContainer => properties;

		public bool Contains(string key)
		{
			return properties.Contains(key);
		}

		public IReadOnlyList<T> GetList<T>(string key)
		{
			return properties.GetList<T>(key);
		}

		public void SetList<T>(string key, IEnumerable<T> value)
		{
			properties.SetList(key, value);
		}

		public virtual ICSharpCode.Core.Properties LoadExtraProperties(string key)
		{
			return new ICSharpCode.Core.Properties();
		}

		public ICSharpCode.Core.Properties NestedProperties(string key)
		{
			throw new NotImplementedException();
		}

		public void Remove(string key)
		{
			UserSettings.RemoveValue(key);
		}

		public virtual void Save()
		{
			using (LockPropertyFile())
			{
				InternalSaveUserSettingsBag();
			}
		}

		public virtual void SaveExtraProperties(string key, ICSharpCode.Core.Properties p)
		{
			throw new NotImplementedException();
		}

		public void SetNestedProperties(string key, ICSharpCode.Core.Properties nestedProperties)
		{
			throw new NotImplementedException();
		}

		#endregion SharpDevelop's IPropertyService

		#region Extra methods

		/// <summary>
		/// Acquires an exclusive lock on the properties file so that it can be opened safely.
		/// </summary>
		private static IDisposable LockPropertyFile()
		{
			Mutex mutex = new Mutex(false, "PropertyServiceSave-30F32619-F92D-4BC0-BF49-AA18BF4AC313");
			mutex.WaitOne();
			return new CallbackOnDispose(
				delegate
				{
					mutex.ReleaseMutex();
					mutex.Close();
				});
		}

		protected virtual PropertyBagLazyLoaded InternalLoadUserSettingsBag(FileName fileName)
		{
			if (!File.Exists(fileName))
			{
				return new PropertyBagLazyLoaded() { ParentObject = SuspendableDocumentNode.StaticInstance };
			}
			try
			{
				using (LockPropertyFile())
				{
					using (var str = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo())
					{
						str.BeginReading(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read));
						var result = (PropertyBagLazyLoaded)str.GetValue("UserSettings", null);
						result.ParentObject = SuspendableDocumentNode.StaticInstance;
						str.EndReading();
						return result;
					}
				}
			}
			catch (XmlException ex)
			{
				SD.MessageService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
			}
			catch (IOException ex)
			{
				SD.MessageService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
			}
			catch (Exception ex)
			{
				SD.MessageService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
			}
			return new PropertyBagLazyLoaded() { ParentObject = SuspendableDocumentNode.StaticInstance };
		}

		protected virtual void InternalSaveUserSettingsBag()
		{
			using (LockPropertyFile())
			{
				if (properties.Count > 0)
					UserSettings.SetValue(SharpDevelopsPropertiesKey, properties);
				else
					UserSettings.RemoveValue(SharpDevelopsPropertiesKey);

				using (var stream = new System.IO.FileStream(propertiesFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
				{
					Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
					info.BeginWriting(stream);
					info.AddValue("UserSettings", UserSettings);
					info.EndWriting();
					stream.Close();
				}
			}
		}

		#endregion Extra methods

		#region Serialization of Sharpdevelops Properties

		/// <summary>
		/// 2014-01-22 Initial version
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ICSharpCode.Core.Properties), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ICSharpCode.Core.Properties)obj;

				var keyList = new HashSet<string>();
				foreach (var entry in s.DictionaryEntries)
				{
					if (!info.IsSerializable(entry.Value))
						continue;
					keyList.Add(entry.Key);
				}

				info.CreateArray("Properties", keyList.Count);
				foreach (var entry in s.DictionaryEntries)
				{
					if (keyList.Contains(entry.Key))
					{
						info.CreateElement("e");
						info.AddValue("Key", entry.Key);
						info.AddValue("Value", entry.Value);
						info.CommitElement();
					}
				}
				info.CommitArray();
			}

			public void Deserialize(ICSharpCode.Core.Properties s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				int count = info.OpenArray("Properties");

				for (int i = 0; i < count; ++i)
				{
					info.OpenElement(); // "e"
					string propkey = info.GetString("Key");
					object propval = info.GetValue("Value", s);
					info.CloseElement(); // "e"
					s.Set(propkey, propval);
				}
				info.CloseArray(count);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = o as ICSharpCode.Core.Properties ?? new ICSharpCode.Core.Properties();
				Deserialize(s, info, parent);
				return s;
			}
		}

		#endregion Serialization of Sharpdevelops Properties
	}
}