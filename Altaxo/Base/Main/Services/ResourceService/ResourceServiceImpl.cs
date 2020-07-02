// #define USERESOURCETRACKING
// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// This Class contains two ResourceManagers, which handle string and image resources
  /// for the application. It do handle localization strings on this level.
  /// </summary>
  public class ResourceServiceImpl : IResourceService
  {
    private const string uiLanguageProperty = "CoreProperties.UILanguage";

    private const string stringResources = "StringResources";
    private const string imageResources = "BitmapResources";

    private string _resourceDirectory;
    private IPropertyService _propertyService;

    private readonly object _loadLock = new object();

    /// <summary>English strings (list of resource managers)</summary>
    private List<(string Prefix, ResourceManager Manager)> _neutralStringsResMgrs = new List<(string Prefix, ResourceManager Manager)>();

    /// <summary>Neutral/English images (list of resource managers)</summary>
    private List<(string Prefix, ResourceManager Manager)> _neutralIconsResMgrs = new List<(string Prefix, ResourceManager Manager)>();

    /// <summary>Stores images that are directly included into assemblies (compiled as embedded resource).
    /// Key is the resource name, value is the assembly where the resource is to find.
    /// We keep the entries even if we have retrieved an image, because
    /// in that way we can create images in different formats (System.Drawing.Bitmap, Wpf image source etc.)</summary>
    private Dictionary<string, Assembly> _neutralLatentImageStreams = new Dictionary<string, Assembly>();

    /// <summary>Hashtable containing the local strings from the main application.</summary>
    private Hashtable? _localStrings = null;

    private Hashtable? _localIcons = null;

    /// <summary>Strings resource managers for the current language</summary>
    private List<(string Prefix, ResourceManager Manager)> _localStringsResMgrs = new List<(string Prefix, ResourceManager Manager)>();

    /// <summary>Image resource managers for the current language</summary>
    private List<(string Prefix, ResourceManager Manager)> _localIconsResMgrs = new List<(string Prefix, ResourceManager Manager)>();

    /// <summary>List of ResourceAssembly</summary>
    private List<ResourceAssembly> _resourceAssemblies = new List<ResourceAssembly>();

    private string _currentLanguage;

    public event EventHandler? LanguageChanged;

    public ResourceServiceImpl(string resourceDirectory, IPropertyService propertyService)
    {
      _resourceDirectory = resourceDirectory ?? throw new ArgumentNullException(nameof(resourceDirectory));
      _propertyService = propertyService ?? throw new ArgumentNullException(nameof(propertyService));
      _propertyService.PropertyChanged += EhPropertyChanged;
      LoadLanguageResources(Language);
    }

    public string Language
    {
      get
      {
        return _propertyService.GetValue(uiLanguageProperty, Thread.CurrentThread.CurrentUICulture.Name);
      }
      set
      {
        if (Language != value)
        {
          _propertyService.SetValue(uiLanguageProperty, value);
        }
      }
    }

    /// <summary>
    /// Registers string resources in the resource service.
    /// </summary>
    /// <param name="baseResourceName">The base name of the resource file embedded in the assembly.</param>
    /// <param name="assembly">The assembly which contains the resource file.</param>
    /// <example><c>ResourceService.RegisterStrings("TestAddin.Resources.StringResources", GetType().Assembly);</c></example>
    public void RegisterStrings(string baseResourceName, Assembly assembly)
    {
      var prefix = GetPrefixFromBaseResourceName(baseResourceName);
      RegisterNeutralStrings(prefix, new ResourceManager(baseResourceName, assembly));
      var ra = new ResourceAssembly(this, assembly, baseResourceName, false);
      _resourceAssemblies.Add(ra);
      ra.Load();
    }

    public void RegisterNeutralStrings(ResourceManager stringManager)
    {
      RegisterNeutralStrings(string.Empty, stringManager);
    }

    public void RegisterNeutralStrings(string prefix, ResourceManager stringManager)
    {
      _neutralStringsResMgrs.Add((prefix, stringManager));
    }

    /// <summary>
    /// Registers image resources in the resource service.
    /// </summary>
    /// <param name="baseResourceName">The base name of the resource file embedded in the assembly.</param>
    /// <param name="assembly">The assembly which contains the resource file.</param>
    /// <example><c>ResourceService.RegisterImages("TestAddin.Resources.BitmapResources", GetType().Assembly);</c></example>
    public void RegisterImages(string baseResourceName, Assembly assembly)
    {
      var prefix = GetPrefixFromBaseResourceName(baseResourceName);
      RegisterNeutralImages(prefix, new ResourceManager(baseResourceName, assembly));
      var ra = new ResourceAssembly(this, assembly, baseResourceName, true);
      _resourceAssemblies.Add(ra);
      ra.Load();
    }

    private string GetPrefixFromBaseResourceName(string baseResourceName)
    {
      // we make an exception for backward compatibility here:
      // if the resources file is in a subdirectory named "Resources", then
      // we do not assume a base name
      // this ensures that old resources will be accessible with their original name


      var splitText = baseResourceName.Split(new char[] { '.' });
      string prefix;

      if (splitText.Length >= 2 && splitText[1].ToLowerInvariant() == "resources")
        prefix = string.Empty;
      else
        prefix = string.Join(".", splitText, 0, splitText.Length - 1) + ".";

      return prefix;
    }

    public void RegisterNeutralImages(ResourceManager imageManager)
    {
      RegisterNeutralImages(string.Empty, imageManager);
    }

    public void RegisterNeutralImages(string baseResourceName, ResourceManager imageManager)
    {
      _neutralIconsResMgrs.Add((baseResourceName, imageManager));
    }

    public void RegisterAssemblyResources(Assembly assembly)
    {

      // Get all resources in an assembly
      string[] resourceNames = assembly.GetManifestResourceNames();

      foreach (string resourceName in resourceNames)
      {
        var extension = Path.GetExtension(resourceName);
        switch (extension)
        {
          case ".resources":
            {
              var baseName = Path.GetFileNameWithoutExtension(resourceName);
              var prefix = Path.GetFileNameWithoutExtension(baseName);
              var name = baseName.Substring(prefix.Length).ToLowerInvariant();
              if (name != ".g")
              {
                if (name.Contains("image") || name.Contains("icon") || name.Contains("bitmap"))
                {
                  RegisterImages(baseName, assembly);
                }
                else // string resources
                {
                  RegisterStrings(baseName, assembly);
                }
              }
            }
            break;
          case ".png":
          case ".jpg":
            {
              _neutralLatentImageStreams.Add(Path.GetFileNameWithoutExtension(resourceName), assembly);
            }
            break;
        }
      }
    }

    private void EhPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == uiLanguageProperty)
      {
        LoadLanguageResources(Language);
        LanguageChanged?.Invoke(this, e);
      }
    }

    [MemberNotNull(nameof(_currentLanguage))]
    private void LoadLanguageResources(string language)
    {
      lock (_loadLock)
      {
        try
        {
          Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
        }
        catch (Exception)
        {
          try
          {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language.Split('-')[0]);
          }
          catch (Exception) { }
        }

        _localStrings = Load(stringResources, language);
        if (_localStrings is null && language.IndexOf('-') > 0)
        {
          _localStrings = Load(stringResources, language.Split('-')[0]);
        }

        _localIcons = Load(imageResources, language);
        if (_localIcons == null && language.IndexOf('-') > 0)
        {
          _localIcons = Load(imageResources, language.Split('-')[0]);
        }

        _localStringsResMgrs.Clear();
        _localIconsResMgrs.Clear();
        _currentLanguage = language;
        foreach (ResourceAssembly ra in _resourceAssemblies)
        {
          ra.Load();
        }
      }
    }

    private Hashtable? Load(string fileName)
    {
      if (File.Exists(fileName))
      {
        var resources = new Hashtable();
        var rr = new ResourceReader(fileName);
        foreach (var entryObj in rr)
        {
          if (entryObj is DictionaryEntry entry)
            resources.Add(entry.Key, entry.Value);
        }
        rr.Close();
        return resources;
      }
      return null;
    }

    private Hashtable? Load(string name, string language)
    {
      return Load(_resourceDirectory + Path.DirectorySeparatorChar + name + "." + language + ".resources");
    }

    /// <summary>
    /// Returns a string from the resource database, it handles localization
    /// transparent for the user.
    /// </summary>
    /// <returns>
    /// The string in the (localized) resource database.
    /// </returns>
    /// <param name="name">
    /// The name of the requested resource.
    /// </param>
    /// <exception cref="ResourceNotFoundException">
    /// Is thrown when the GlobalResource manager can't find a requested resource.
    /// </exception>
    public string GetString(string name)
    {
      lock (_loadLock)
      {
        string? s = null;

        // String is already in the hashtable of localized strings?
        if (_localStrings?[name] is { } lsn)
        {
          s = lsn.ToString();
        }
        else
        {
          // search all local resource managers
          foreach ((string prefix, ResourceManager resourceManger) in _localStringsResMgrs)
          {
            try
            {
              s = resourceManger.GetString(name);
            }
            catch (Exception) { }

            if (s != null)
            {
              break;
            }
          }

          if (s is null)
          {
            // search all unlocalized resource managers
            foreach ((string prefix, ResourceManager resourceManger) in _neutralStringsResMgrs)
            {
              if (prefix.Length == 0 || name.StartsWith(prefix))
              {
                try
                {
                  s = resourceManger.GetString(prefix.Length == 0 ? name : name.Substring(prefix.Length));
                }
                catch (Exception) { }

                if (s != null)
                {
                  break;
                }
              }
            }
          }
          if (s == null)
          {
            // throw an exception if not found
            throw new ResourceNotFoundException("string >" + name + "<");
          }
        }

#if USERESOURCETRACKING
        LogStringResource(name, s, Assembly.GetCallingAssembly());
#endif

        return s ?? name;
      }
    }

    public object? GetImageResource(string name)
    {
#if USERESOURCETRACKING
        LogImageResource(name, s, Assembly.GetCallingAssembly());
#endif

      lock (_loadLock)
      {
        object? iconobj = null;
        if (_localIcons?[name] is { } lin)
        {
          iconobj = lin;
        }
        else
        {
          foreach ((string prefix, ResourceManager resourceManger) in _localIconsResMgrs)
          {
            iconobj = resourceManger.GetObject(name);
            if (!(iconobj is null))
            {
              break;
            }
          }

          if (iconobj is null)
          {
            foreach ((string prefix, ResourceManager resourceManger) in _neutralIconsResMgrs)
            {
              try
              {
                iconobj = resourceManger.GetObject(name);
              }
              catch (Exception ex)
              {
                System.Diagnostics.Debug.WriteLine($"Exception in GetImageResource, Message: {ex.Message}");
              }

              if (iconobj != null)
              {
                break;
              }
            }
          }
        }
        return iconobj;
      }
    }

    public Bitmap? GetBitmap(string name)
    {
      return (Bitmap?)GetImageResource(name);
    }

    public System.IO.Stream? GetResourceStream(string name)
    {
      lock (_loadLock)
      {
        System.IO.Stream? resourceStream = null;

        if (_neutralLatentImageStreams.TryGetValue(name, out var assembly))
        {
          return assembly.GetManifestResourceStream(name);
        }


        foreach ((string prefix, ResourceManager resourceManger) in _localIconsResMgrs)
        {
          resourceStream = resourceManger.GetStream(name);
          if (resourceStream != null)
          {
            break;
          }
        }

        if (resourceStream is null)
        {
          foreach ((string prefix, ResourceManager resourceManger) in _neutralIconsResMgrs)
          {
            try
            {
              resourceStream = resourceManger.GetStream(name);
            }
            catch (Exception) { }

            if (resourceStream != null)
            {
              break;
            }
          }
        }

        return resourceStream;
      }
    }

    #region Inner classes

    private class ResourceAssembly
    {
      private ResourceServiceImpl _service;
      private Assembly _assembly;
      private string _baseResourceName;
      private bool _isIcons;

      public ResourceAssembly(ResourceServiceImpl service, Assembly assembly, string baseResourceName, bool isIcons)
      {
        _service = service;
        _assembly = assembly;
        _baseResourceName = baseResourceName;
        _isIcons = isIcons;
      }

      private ResourceManager? TrySatellite(string language)
      {
        // ResourceManager should automatically use satellite assemblies, but it doesn't work
        // and we have to do it manually.
        string fileName = Path.GetFileNameWithoutExtension(_assembly.Location) + ".resources.dll";
        fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(_assembly.Location) ?? string.Empty, language), fileName);
        if (File.Exists(fileName))
        {
          Current.Log.Info("Logging resources " + _baseResourceName + " loading from satellite " + language);
          return new ResourceManager(_baseResourceName, Assembly.LoadFrom(fileName));
        }
        else
        {
          return null;
        }
      }

      public void Load()
      {
        string currentLanguage = _service._currentLanguage;
        string logMessage = "Loading resources " + _baseResourceName + "." + currentLanguage + ": ";
        ResourceManager? manager = null;
        if (_assembly.GetManifestResourceInfo(_baseResourceName + "." + currentLanguage + ".resources") != null)
        {
          Current.Log.Info(logMessage + " loading from main assembly");
          manager = new ResourceManager(_baseResourceName + "." + currentLanguage, _assembly);
        }
        else if (currentLanguage.IndexOf('-') > 0
                                         && _assembly.GetManifestResourceInfo(_baseResourceName + "." + currentLanguage.Split('-')[0] + ".resources") != null)
        {
          Current.Log.Info(logMessage + " loading from main assembly (no country match)");
          manager = new ResourceManager(_baseResourceName + "." + currentLanguage.Split('-')[0], _assembly);
        }
        else
        {
          // try satellite assembly
          manager = TrySatellite(currentLanguage);
          if (manager == null && currentLanguage.IndexOf('-') > 0)
          {
            manager = TrySatellite(currentLanguage.Split('-')[0]);
          }
        }
        if (manager == null)
        {
          Current.Log.Warn(logMessage + "NOT FOUND");
        }
        else
        {
          if (_isIcons)
          {
            _service._localIconsResMgrs.Add((string.Empty, manager));
          }
          else
          {
            _service._localStringsResMgrs.Add((string.Empty, manager));
          }
        }
      }
    }

    #endregion Inner classes

    #region Instrumentation

    private HashSet<string>? _stringKeysForLogging;
    private void LogStringResource(string name, string value, Assembly callingAssembly)
    {
      if (null == _stringKeysForLogging)
        _stringKeysForLogging = new HashSet<string>();

      if (!_stringKeysForLogging.Contains(name))
      {
        _stringKeysForLogging.Add(name);

        using (var stream = new System.IO.FileStream(@"C:\TEMP\UsedKeysForStrings.txt", FileMode.Append, FileAccess.Write, FileShare.None))
        {
          using (var wr = new StreamWriter(stream))
          {
            wr.Write(name);
            wr.Write("\t");
            var val = value.Trim();
            var idx = val.IndexOfAny(new char[] { '\r', '\n', '\t' });
            var len = idx >= 0 ? Math.Min(val.Length, idx) : val.Length;
            wr.Write(val.Substring(0, len));
            wr.Write("\t");
            wr.Write(callingAssembly.GetName().Name);
            wr.WriteLine();
          }
        }
      }
    }

    private HashSet<string>? _imageKeysForLogging;
    private void LogImageResource(string name, Assembly callingAssembly)
    {
      if (null == _imageKeysForLogging)
        _imageKeysForLogging = new HashSet<string>();

      if (!_imageKeysForLogging.Contains(name))
      {
        _imageKeysForLogging.Add(name);

        using (var stream = new System.IO.FileStream(@"C:\TEMP\UsedKeysForImages.txt", FileMode.Append, FileAccess.Write, FileShare.None))
        {
          using (var wr = new StreamWriter(stream))
          {
            wr.Write(name);
            wr.Write("\t");
            wr.Write(callingAssembly.GetName().Name);
            wr.WriteLine();
          }
        }
      }
    }

    #endregion
  }
}
