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

#nullable enable
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Xml;

namespace Altaxo.Main.Services
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Main.Properties;

  /// <summary>
  /// Default implementation of the application property service.
  /// </summary>
  public class PropertyService : Altaxo.Main.Services.IPropertyService
  {
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the data directory.
    /// </summary>
    public DirectoryName DataDirectory { get; protected set; }

    /// <summary>
    /// Gets the configuration directory.
    /// </summary>
    public DirectoryName ConfigDirectory { get; protected set; }

    /// <summary>
    /// Gets the file name of the user properties file.
    /// </summary>
    public FileName PropertiesFileName { get; protected set; }


    /// <summary>
    /// The user settings bag is a bag that loads the properties lazy (as Xml string) during creation of this service.
    /// See remarks for why this is neccessary.
    /// </summary>
    /// <remarks>The properties are lazy loaded, i.e. the property values are stored as string first. Only if the property is needed, the xml string
    /// is converted into a regular value. This is neccessary, because some of the property values will required access
    /// to other property values during deserialization. But the user settings are deserialized in the constructor of the property service,
    /// and the property service is not installed in this moment. This will lead to NullReferenceException when the first deserialized
    /// property value try to get the property service.</remarks>
    protected PropertyBagLazyLoaded _localApplicationSettings;

    /// <summary>
    /// Gets the local application settings (applications specific for this PC). This data are stored in the LOCALAPPDATA folder.
    /// </summary>
    /// <value>
    /// The local application settings.
    /// </value>
    public IPropertyBag LocalApplicationSettings { get { return _localApplicationSettings; } }
    /// <summary>
    /// Gets the file that stores local application settings.
    /// </summary>
    public FileName LocalApplicationSettingsFile { get; protected set; }

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

    /// <summary>
    /// Gets the user settings.
    /// </summary>
    public IPropertyBag UserSettings { get { return _userSettings; } }

    /// <summary>
    /// Gets the application settings.
    /// </summary>
    public IPropertyBag ApplicationSettings { get; private set; }

    /// <summary>
    /// Gets the built-in settings.
    /// </summary>
    public IPropertyBag BuiltinSettings { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyService"/> class.
    /// </summary>
    /// <param name="configDirectory">The configuration directory.</param>
    /// <param name="dataDirectory">The data directory.</param>
    /// <param name="localAppDirectory">The local application data directory.</param>
    /// <param name="propertiesName">The base settings file name.</param>
    public PropertyService(DirectoryName configDirectory, DirectoryName dataDirectory, DirectoryName localAppDirectory, string propertiesName)
    {
      DataDirectory = dataDirectory;
      ConfigDirectory = configDirectory;
      PropertiesFileName = configDirectory.CombineFile(propertiesName + ".xml");
      _userSettings = InternalLoadUserSettingsBag(PropertiesFileName);

      LocalApplicationSettingsFile = localAppDirectory.CombineFile(propertiesName + ".xml");
      _localApplicationSettings = InternalLoadUserSettingsBag(LocalApplicationSettingsFile);

      ApplicationSettings = new PropertyBag() { ParentObject = SuspendableDocumentNode.StaticInstance };
      BuiltinSettings = new PropertyBag() { ParentObject = SuspendableDocumentNode.StaticInstance };
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the changed property.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #region Property getters

    /// <inheritdoc />
    public T GetValue<T>(string property, T defaultValue)
    {
      if (UserSettings.TryGetValue<T>(property, out var result))
        return result;
      else if (ApplicationSettings.TryGetValue<T>(property, out result))
        return result;
      else if (BuiltinSettings.TryGetValue<T>(property, out result))
        return result;
      else
        return defaultValue;
    }

    /// <inheritdoc />
    [return: MaybeNull]
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

    /// <inheritdoc />
    [return: MaybeNull]
    public T GetValueOrNull<T>(PropertyKey<T> p, RuntimePropertyKind kind)
    {
      if (kind == RuntimePropertyKind.UserAndApplicationAndBuiltin && UserSettings.TryGetValue<T>(p, out var result))
        return result;
      else if (kind == RuntimePropertyKind.ApplicationAndBuiltin && ApplicationSettings.TryGetValue<T>(p, out result))
        return result;
      else if (BuiltinSettings.TryGetValue<T>(p, out result))
        return result;
      else
        return default;
    }


    /// <inheritdoc />
    [return: NotNullIfNotNull("ValueCreationIfNotFound")]
    [return: MaybeNull]
    public T GetValue<T>(PropertyKey<T> p, RuntimePropertyKind kind, Func<T>? ValueCreationIfNotFound) where T : notnull
    {
      if (kind == RuntimePropertyKind.UserAndApplicationAndBuiltin && UserSettings.TryGetValue<T>(p, out var result))
        return result;
      else if (kind == RuntimePropertyKind.ApplicationAndBuiltin && ApplicationSettings.TryGetValue<T>(p, out result))
        return result;
      else if (BuiltinSettings.TryGetValue<T>(p, out result))
        return result;
      else if (ValueCreationIfNotFound is not null)
        return ValueCreationIfNotFound();
      else
        return default;
    }

    #endregion Property getters

    #region Property setters

    /// <inheritdoc />
    public void SetValue<T>(string property, T value)
    {
      UserSettings.SetValue<T>(property, value);
      OnPropertyChanged(property);
    }

    /// <inheritdoc />
    public void SetValue<T>(PropertyKey<T> p, T value)
    {
      UserSettings.SetValue(p, value);
      OnPropertyChanged(p.GuidString);
    }

    #endregion Property setters

    #region Property removers

    /// <inheritdoc />
    public void Remove(string key)
    {
      UserSettings.RemoveValue(key);
    }

    #endregion Property removers

    #region Loading/Saving

    /// <summary>
    /// Acquires an exclusive lock on the properties file so that it can be opened safely.
    /// </summary>
    private static IDisposable LockPropertyFile()
    {
      var mutex = new Mutex(false, "PropertyServiceSave-30F32619-F92D-4BC0-BF49-AA18BF4AC313");
      mutex.WaitOne();
      return new CallbackOnDispose(
          delegate
          {
            mutex.ReleaseMutex();
            mutex.Close();
          });
    }

    /// <summary>
    /// Loads a lazily deserialized settings bag from the specified file.
    /// </summary>
    /// <param name="fileName">The settings file to load.</param>
    /// <returns>The loaded settings bag, or an empty one if loading failed.</returns>
    protected virtual PropertyBagLazyLoaded InternalLoadUserSettingsBag(FileName fileName)
    {
      if (!File.Exists(fileName))
      {
        var result = new PropertyBagLazyLoaded() { ParentObject = SuspendableDocumentNode.StaticInstance };
        return result;
      }
      try
      {
        using (LockPropertyFile())
        {
          using (var str = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo())
          {
            str.BeginReading(new FileStream(fileName.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read));
            var result = (PropertyBagLazyLoaded)str.GetValue("UserSettings", null);
            result.ParentObject = SuspendableDocumentNode.StaticInstance;
            str.EndReading();
            return result;
          }
        }
      }
      catch (XmlException ex)
      {
        Altaxo.Current.MessageService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
      }
      catch (IOException ex)
      {
        Altaxo.Current.MessageService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
      }
      catch (Exception ex)
      {
        Altaxo.Current.MessageService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
      }
      return new PropertyBagLazyLoaded() { ParentObject = SuspendableDocumentNode.StaticInstance };
    }

    /// <summary>
    /// Saves a lazily loaded settings bag to the specified file.
    /// </summary>
    /// <param name="settings">The settings to save.</param>
    /// <param name="fileName">The target file name.</param>
    protected virtual void InternalSaveUserSettingsBag(PropertyBagLazyLoaded settings, FileName fileName)
    {
      var thisVersion = UserSettings.GetType().Assembly.GetName().Version;

      if (settings.AssemblyVersionLoadedFrom is not null && thisVersion < settings.AssemblyVersionLoadedFrom)
      {
        var answer = Current.Gui.YesNoMessageBox(
            string.Format(
            "The existing UserSettings file was stored with a newer version of Altaxo (version {0}).\r\n" +
            "Do you want to store and thus overwrite UserSettings now (with version {1})?", settings.AssemblyVersionLoadedFrom, thisVersion),
            "Attention!", false);

        if (false == answer)
          return; // User settings are not stored again if they originate from a newer version of Altaxo
      }

      using (LockPropertyFile())
      {
        if (fileName.GetParentDirectory() is { } parentDirectory)
          System.IO.Directory.CreateDirectory(parentDirectory.ToString());

        System.IO.FileStream? streamForWriting = null;
        for (int i = 500; i >= 0; --i) // Try to save the file, wait up to 10 seconds for getting the file (avoid exception if another instance is currently reading the properties file, especially with Com
        {
          try
          {
            streamForWriting = new System.IO.FileStream(fileName.ToString(), System.IO.FileMode.Create, System.IO.FileAccess.Write, FileShare.None);
            break;
          }
          catch (Exception)
          {
            if (0 == i)
              throw;
          }

          System.Threading.Thread.Sleep(20);
        }

        using (var stream = streamForWriting!)
        {
          var info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
          info.BeginWriting(stream);
          info.AddValue("UserSettings", settings);
          info.EndWriting();
          stream.Close();
        }
      }
    }

    /// <inheritdoc />
    public virtual void Save()
    {
      InternalSaveUserSettingsBag(_userSettings, PropertiesFileName);

      if (_localApplicationSettings.Count > 0)
      {
        InternalSaveUserSettingsBag(_localApplicationSettings, LocalApplicationSettingsFile);
      }
      else if (File.Exists(LocalApplicationSettingsFile))
      {
        File.Delete(LocalApplicationSettingsFile);
      }
    }

    #endregion Loading/Saving
  }
}
