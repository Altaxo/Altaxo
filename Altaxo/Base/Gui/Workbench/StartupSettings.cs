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

using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// This class contains properties about the start of the application.
  /// </summary>
  [Serializable]
  public sealed class StartupSettings
  {
    // taken from StartupArguments
    public string ApplicationName { get; private set; }

    public string[] RequestedFileList { get; private set; } = new string[0];
    public string[] ParameterList { get; private set; } = new string[0];
    public string[] StartupArgs { get; private set; } = new string[0];

    // additional parameters

    private bool _useExceptionBoxForErrorHandler = true;
    private string _applicationRootPath;
    private bool _allowAddInConfigurationAndExternalAddIns = true;
    private bool _allowUserAddIns;
    private string _propertiesName;
    private string _configDirectory;
    private string _dataDirectory;
    private string _resourceAssemblyName;
    public List<string> _addInDirectories { get; private set; } = new List<string>();
    public List<string> _addInFiles { get; private set; } = new List<string>();
    public bool RunWorkbenchOnNewThread { get; private set; }
    public System.Globalization.CultureInfo OriginalUICulture { get; private set; }
    public System.Globalization.CultureInfo OriginalCulture { get; private set; }

    static StartupSettings()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupSettings"/> class.
    /// </summary>
    /// <param name="applicationName">Name of the application.</param>
    /// <param name="startupArguments">The startup arguments.</param>
    /// <param name="requestedFileList">The requested file list.</param>
    /// <param name="parameterList">The parameter list.</param>
    /// <exception cref="System.ArgumentNullException">
    /// ApplicationName
    /// or
    /// startupArguments
    /// or
    /// requestedFileList
    /// or
    /// parameterList
    /// </exception>
    public StartupSettings(string applicationName, string[] startupArguments, string[] requestedFileList, string[] parameterList)
    {
      ApplicationName = applicationName ?? throw new ArgumentNullException(nameof(ApplicationName));
      StartupArgs = startupArguments ?? throw new ArgumentNullException(nameof(startupArguments));
      RequestedFileList = requestedFileList ?? throw new ArgumentNullException(nameof(requestedFileList));
      ParameterList = parameterList ?? throw new ArgumentNullException(nameof(parameterList));

      _resourceAssemblyName = System.Reflection.Assembly.GetCallingAssembly().GetName().Name;
      OriginalUICulture = System.Globalization.CultureInfo.CurrentUICulture;
      OriginalCulture = System.Globalization.CultureInfo.CurrentCulture;
    }

    /// <summary>
    /// Gets/Sets the name of the assembly to load the BitmapResources
    /// and English StringResources from.
    /// </summary>
    public string ResourceAssemblyName
    {
      get { return _resourceAssemblyName; }
      set
      {
        _resourceAssemblyName = value ?? throw new ArgumentNullException(nameof(value));
      }
    }

    /// <summary>
    /// Gets/Sets whether the custom exception box should be used for
    /// unhandled exceptions. The default is true.
    /// </summary>
    public bool UseExceptionBoxForErrorHandler
    {
      get { return _useExceptionBoxForErrorHandler; }
      set { _useExceptionBoxForErrorHandler = value; }
    }

    /// <summary>
    /// Use the file <see cref="ConfigDirectory"/>\AddIns.xml to maintain
    /// a list of deactivated AddIns and list of AddIns to load from
    /// external locations.
    /// The default value is true.
    /// </summary>
    public bool AllowAddInConfigurationAndExternalAddIns
    {
      get { return _allowAddInConfigurationAndExternalAddIns; }
      set { _allowAddInConfigurationAndExternalAddIns = value; }
    }

    /// <summary>
    /// Allow user AddIns stored in the "application data" directory.
    /// The default is false.
    /// </summary>
    public bool AllowUserAddIns
    {
      get { return _allowUserAddIns; }
      set { _allowUserAddIns = value; }
    }

    /// <summary>
    /// Gets/Sets the application root path to use.
    /// Use null (default) to use the base directory of the application's AppDomain.
    /// </summary>
    public string ApplicationRootPath
    {
      get { return _applicationRootPath; }
      set { _applicationRootPath = value; }
    }

    /// <summary>
    /// Gets/Sets the directory used to store application properties,
    /// settings and user AddIns.
    /// Use null (default) to use "ApplicationData\ApplicationName"
    /// </summary>
    public string ConfigDirectory
    {
      get { return _configDirectory; }
      set { _configDirectory = value; }
    }

    /// <summary>
    /// Sets the data directory used to load resources.
    /// Use null (default) to use the default path "ApplicationRootPath\data".
    /// </summary>
    public string DataDirectory
    {
      get { return _dataDirectory; }
      set { _dataDirectory = value; }
    }

    /// <summary>
    /// Sets the name used for the properties file (without path or extension).
    /// Use null (default) to use the default name.
    /// </summary>
    public string PropertiesName
    {
      get { return _propertiesName; }
      set { _propertiesName = value; }
    }

    /// <summary>
    /// Find AddIns by searching all .addin files recursively in <paramref name="addInDir"/>.
    /// </summary>
    public void AddAddInsFromDirectory(string addInDir)
    {
      if (addInDir == null)
        throw new ArgumentNullException(nameof(addInDir));
      _addInDirectories.Add(addInDir);
    }

    /// <summary>
    /// Add the specified .addin file.
    /// </summary>
    public void AddAddInFile(string addInFile)
    {
      if (addInFile == null)
        throw new ArgumentNullException(nameof(addInFile));
      _addInFiles.Add(addInFile);
    }
  }
}
