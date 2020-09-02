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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Altaxo.Main.Services;

namespace Altaxo.AddInItems
{
  public sealed class AddIn
  {
    private IAddInTree _addInTree;
    private Properties _properties = new Properties();
    private List<Runtime> _runtimes = new List<Runtime>();
    private List<string> _bitmapResources = new List<string>();
    private List<string> _stringResources = new List<string>();

    internal string? _addInFileName = null;
    private AddInManifest _manifest = new AddInManifest();
    private Dictionary<string, ExtensionPath> _paths = new Dictionary<string, ExtensionPath>();
    private AddInAction _action = AddInAction.Disable;
    private bool _enabled;

    private static bool _hasShownErrorMessage = false;

    internal AddIn(IAddInTree addInTree)
    {
      _addInTree = addInTree ?? throw new ArgumentNullException(nameof(addInTree));
    }

    /// <summary>
    /// Gets the parent AddIn-Tree that contains this AddIn.
    /// </summary>
    public IAddInTree AddInTree
    {
      get { return _addInTree; }
    }

    public object? CreateObject(string className)
    {
      Type? t = FindType(className);
      if (t is not null)
        return Activator.CreateInstance(t);
      else
        return null;
    }

    public Type? FindType(string className)
    {
      foreach (Runtime runtime in _runtimes)
      {
        if (!runtime.IsHostApplicationAssembly)
        {
          // Load dependencies only when a plugin library is first loaded.
          // This allows looking in host assemblies for service IDs.
          LoadDependencies();
        }
        Type? t = runtime.FindType(className);
        if (t is not null)
        {
          return t;
        }
      }
      if (_hasShownErrorMessage)
      {
        Current.Log.Error("Cannot find class: " + className);
      }
      else
      {
        _hasShownErrorMessage = true;
        var messageService = Altaxo.Current.GetRequiredService<IMessageService>();
        messageService.ShowError("Cannot find class: " + className + "\nFuture missing objects will not cause an error message.");
      }
      return null;
    }

    public Stream? GetManifestResourceStream(string resourceName)
    {
      LoadDependencies();
      foreach (Runtime runtime in _runtimes)
      {
        Assembly? assembly = runtime.LoadedAssembly;
        if (assembly is not null)
        {
          Stream? s = assembly.GetManifestResourceStream(resourceName);
          if (s is not null)
          {
            return s;
          }
        }
      }
      return null;
    }

    public void LoadRuntimeAssemblies()
    {
      LoadDependencies();
      foreach (Runtime runtime in _runtimes)
      {
        if (runtime.IsActive)
          runtime.Load();
      }
    }

    private volatile bool dependenciesLoaded;

    private void LoadDependencies()
    {
      // Thread-safe dependency loading:
      // Because the methods being called should be thread-safe, there's
      // no problem when we load dependencies multiple times concurrently.
      // However, we need to make sure we don't return before the dependencies are ready,
      // so "bool dependenciesLoaded" must be volatile and set only at the very end of this method.
      if (!dependenciesLoaded)
      {
        Current.Log.Info("Loading addin " + Name);

        AssemblyLocator.Init();
        foreach (AddInReference r in _manifest.Dependencies)
        {
          if (r.RequirePreload)
          {
            bool found = false;
            foreach (AddIn addIn in AddInTree.AddIns)
            {
              if (addIn.Manifest.Identities.ContainsKey(r.Name))
              {
                found = true;
                addIn.LoadRuntimeAssemblies();
              }
            }
            if (!found)
            {
              throw new AddInLoadException("Cannot load run-time dependency for " + r.ToString());
            }
          }
        }
        dependenciesLoaded = true;
      }
    }

    public override string ToString()
    {
      return "[AddIn: " + Name + "]";
    }

    private string? customErrorMessage;

    /// <summary>
    /// Gets the message of a custom load error. Used only when AddInAction is set to CustomError.
    /// Settings this property to a non-null value causes Enabled to be set to false and
    /// Action to be set to AddInAction.CustomError.
    /// </summary>
    public string? CustomErrorMessage
    {
      get { return customErrorMessage; }
      internal set
      {
        if (value is not null)
        {
          Enabled = false;
          Action = AddInAction.CustomError;
        }
        customErrorMessage = value;
      }
    }

    /// <summary>
    /// Action to execute when the application is restarted.
    /// </summary>
    public AddInAction Action
    {
      get { return _action; }
      set { _action = value; }
    }

    public IReadOnlyList<Runtime> Runtimes
    {
      get { return _runtimes; }
    }

    public Version? Version
    {
      get { return _manifest.PrimaryVersion; }
    }

    public string? FileName
    {
      get { return _addInFileName; }
      set { _addInFileName = value; }
    }

    public string Name
    {
      get { return _properties["name"]; }
    }

    public AddInManifest Manifest
    {
      get { return _manifest; }
    }

    public Dictionary<string, ExtensionPath> Paths
    {
      get { return _paths; }
    }

    public Properties Properties
    {
      get { return _properties; }
    }

    public List<string> BitmapResources
    {
      get { return _bitmapResources; }
      set { _bitmapResources = value; }
    }

    public List<string> StringResources
    {
      get { return _stringResources; }
      set { _stringResources = value; }
    }

    public bool Enabled
    {
      get { return _enabled; }
      set
      {
        _enabled = value;
        Action = value ? AddInAction.Enable : AddInAction.Disable;
      }
    }

    private static void SetupAddIn(XmlReader reader, AddIn addIn, string? hintPath)
    {
      while (reader.Read())
      {
        if (reader.NodeType == XmlNodeType.Element && reader.IsStartElement())
        {
          switch (reader.LocalName)
          {
            case "StringResources":
            case "BitmapResources":
              if (reader.AttributeCount != 1)
              {
                throw new AddInLoadException("BitmapResources requires ONE attribute.");
              }

              var filename = StringParser.Parse(reader.GetAttribute("file"));


              if (reader.LocalName == "BitmapResources")
              {
                addIn.BitmapResources.Add(filename);
              }
              else
              {
                addIn.StringResources.Add(filename);
              }

              break;

            case "Runtime":
              if (!reader.IsEmptyElement)
              {
                addIn._runtimes.AddRange(Runtime.ReadSection(reader, addIn, hintPath));
              }
              break;

            case "Include":
              if (reader.AttributeCount != 1)
              {
                throw new AddInLoadException("Include requires ONE attribute.");
              }
              if (!reader.IsEmptyElement)
              {
                throw new AddInLoadException("Include nodes must be empty!");
              }
              if (hintPath is null)
              {
                throw new AddInLoadException("Cannot use include nodes when hintPath was not specified (e.g. when AddInManager reads a .addin file)!");
              }
              string fileName = Path.Combine(hintPath, reader.GetAttribute(0));
              var xrs = new XmlReaderSettings
              {
                NameTable = reader.NameTable, // share the name table
                ConformanceLevel = ConformanceLevel.Fragment
              };
              using (XmlReader includeReader = XmlTextReader.Create(fileName, xrs))
              {
                SetupAddIn(includeReader, addIn, Path.GetDirectoryName(fileName) ?? throw new InvalidOperationException());
              }
              break;

            case "Path":
              if (reader.AttributeCount != 1)
              {
                throw new AddInLoadException("Import node requires ONE attribute.");
              }
              string pathName = reader.GetAttribute(0);
              ExtensionPath extensionPath = addIn.GetExtensionPath(pathName);
              if (!reader.IsEmptyElement)
              {
                ExtensionPath.SetUp(extensionPath, reader, "Path");
              }
              break;

            case "Manifest":
              addIn.Manifest.ReadManifestSection(reader, hintPath);
              break;

            default:
              throw new AddInLoadException("Unknown root path node:" + reader.LocalName);
          }
        }
      }
    }

    public ExtensionPath GetExtensionPath(string pathName)
    {
      if (!_paths.ContainsKey(pathName))
      {
        return _paths[pathName] = new ExtensionPath(pathName, this);
      }
      return _paths[pathName];
    }

    public static AddIn Load(IAddInTree addInTree, TextReader textReader, string? hintPath = null, XmlNameTable? nameTable = null)
    {
      if (nameTable is null)
        nameTable = new NameTable();
      try
      {
        var addIn = new AddIn(addInTree);
        using (var reader = new XmlTextReader(textReader, nameTable))
        {
          while (reader.Read())
          {
            if (reader.IsStartElement())
            {
              switch (reader.LocalName)
              {
                case "AddIn":
                  addIn._properties = Properties.ReadFromAttributes(reader);
                  SetupAddIn(reader, addIn, hintPath);
                  break;

                default:
                  throw new AddInLoadException("Unknown add-in file.");
              }
            }
          }
        }
        return addIn;
      }
      catch (XmlException ex)
      {
        throw new AddInLoadException(ex.Message, ex);
      }
    }

    public static AddIn Load(IAddInTree addInTree, string fileName, XmlNameTable? nameTable = null)
    {
      try
      {
        using (TextReader textReader = File.OpenText(fileName))
        {
          AddIn addIn = Load(addInTree, textReader, Path.GetDirectoryName(fileName), nameTable);
          addIn._addInFileName = fileName;
          return addIn;
        }
      }
      catch (AddInLoadException)
      {
        throw;
      }
      catch (Exception e)
      {
        throw new AddInLoadException("Can't load " + fileName, e);
      }
    }

    /// <summary>
    /// Gets whether the AddIn is a preinstalled component of the host application.
    /// </summary>
    public bool IsPreinstalled
    {
      get
      {
        if (!string.IsNullOrEmpty(FileName) && FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, FileName))
        {
          string hidden = Properties["addInManagerHidden"];
          return string.Equals(hidden, "true", StringComparison.OrdinalIgnoreCase)
              || string.Equals(hidden, "preinstalled", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
          return false;
        }
      }
    }
  }
}
