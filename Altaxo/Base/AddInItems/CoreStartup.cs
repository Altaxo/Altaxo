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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Input;
using Altaxo.Main.Services;
using Altaxo.Settings;

namespace Altaxo.AddInItems
{
  /// <summary>
  /// Class that helps starting up.
  /// </summary>
  /// <remarks>
  /// Initializing requires initializing several static classes
  /// and the <see cref="AddInTree"/>. <see cref="CoreStartup"/> does this work
  /// for you, provided you use it like this:
  /// 1. Create a new CoreStartup instance
  /// 2. (Optional) Set the values of the properties.
  /// 3. Call <see cref="StartCoreServices"/>.
  /// 4. Add "preinstalled" AddIns using <see cref="AddAddInsFromDirectory"/>
  ///    and <see cref="AddAddInFile"/>.
  /// 5. (Optional) Call <see cref="ConfigureExternalAddIns"/> to support
  ///    disabling AddIns and installing external AddIns
  /// 6. (Optional) Call <see cref="ConfigureUserAddIns"/> to support installing
  ///    user AddIns.
  /// 7. Call <see cref="RunInitialization"/>.
  /// </remarks>
  public sealed class CoreStartup
  {
    private List<string> _addInFiles = new List<string>();
    private List<string> _disabledAddIns = new List<string>();
    private bool _externalAddInsConfigured;
    private AddInTreeImpl? _addInTree;
    private string _applicationName;

    /// <summary>
    /// Creates a new CoreStartup instance.
    /// </summary>
    /// <param name="applicationName">
    /// The name of your application.
    /// This is used as default title for message boxes,
    /// default name for the configuration directory etc.
    /// </param>
    public CoreStartup(string applicationName)
    {
      if (string.IsNullOrEmpty(applicationName))
        throw new ArgumentNullException(nameof(applicationName));
      _applicationName = applicationName;
    }

    /// <summary>
    /// Find AddIns by searching all .addin files recursively in <paramref name="addInDir"/>.
    /// The AddIns that were found are added to the list of AddIn files to load.
    /// </summary>
    public void AddAddInsFromDirectory(string addInDir)
    {
      if (addInDir is null)
        throw new ArgumentNullException(nameof(addInDir));

      if (Directory.Exists(addInDir))
      {
        _addInFiles.AddRange(Directory.GetFiles(addInDir, "*.addin", SearchOption.AllDirectories));
      }
    }

    /// <summary>
    /// Add the specified .addin file to the list of AddIn files to load.
    /// </summary>
    public void AddAddInFile(string addInFile)
    {
      if (addInFile is null)
        throw new ArgumentNullException(nameof(addInFile));
      _addInFiles.Add(addInFile);
    }

    /// <summary>
    /// Use the specified configuration file to store information about
    /// disabled AddIns and external AddIns.
    /// You have to call this method to support the <see cref="AddInManager"/>.
    /// </summary>
    /// <param name="addInConfigurationFile">
    /// The name of the file used to store the list of disabled AddIns
    /// and the list of installed external AddIns.
    /// A good value for this parameter would be
    /// <c>Path.Combine(<see cref="PropertyService.ConfigDirectory"/>, "AddIns.xml")</c>.
    /// </param>
    public void ConfigureExternalAddIns(string addInConfigurationFile)
    {
      _externalAddInsConfigured = true;
      AddInManager.ConfigurationFileName = addInConfigurationFile;
      AddInManager.LoadAddInConfiguration(_addInFiles, _disabledAddIns);
    }

    /// <summary>
    /// Configures user AddIn support.
    /// </summary>
    /// <param name="addInInstallTemp">
    /// The AddIn installation temporary directory.
    /// ConfigureUserAddIns will install the AddIns from this directory and
    /// store the parameter value in <see cref="AddInManager.AddInInstallTemp"/>.
    /// </param>
    /// <param name="userAddInPath">
    /// The path where user AddIns are installed to.
    /// AddIns from this directory will be loaded.
    /// </param>
    public void ConfigureUserAddIns(string addInInstallTemp, string userAddInPath)
    {
      if (!_externalAddInsConfigured)
      {
        throw new InvalidOperationException("ConfigureExternalAddIns must be called before ConfigureUserAddIns");
      }
      AddInManager.AddInInstallTemp = addInInstallTemp;
      AddInManager.UserAddInPath = userAddInPath;
      if (Directory.Exists(addInInstallTemp))
      {
        AddInManager.InstallAddIns(_disabledAddIns);
      }
      if (Directory.Exists(userAddInPath))
      {
        AddAddInsFromDirectory(userAddInPath);
      }
    }

    /// <summary>
    /// Initializes the AddIn system.
    /// This loads the AddIns that were added to the list,
    /// then it executes the <see cref="ICommand">commands</see>
    /// in <c>/Application/Autostart</c>.
    /// </summary>
    public void RunInitialization()
    {
      if (_addInTree is null)
        throw new InvalidProgramException($"Before this call, the method {nameof(StartCoreServices)} has to be called!");

      _addInTree.Load(_addInFiles, _disabledAddIns);

      // perform service registration
      var container = Altaxo.Current.GetService<System.ComponentModel.Design.IServiceContainer>();
      if (container is not null)
      {
        _addInTree.BuildItems<object>(string.Format("/{0}/Services", _applicationName), container, false);
      }

      // run workspace autostart commands
      Current.Log.Info("Running autostart commands...");
      foreach (ICommand command in _addInTree.BuildItems<ICommand>(string.Format("/{0}/Autostart", _applicationName), this, false))
      {
        try
        {
          command.Execute(null);
        }
        catch (Exception ex)
        {
          // allow startup to continue if some commands fail
          Altaxo.Current.GetRequiredService<IMessageService>().ShowException(ex);
        }
      }

      // finally, create the initial project
      Altaxo.Current.IProjectService.CreateInitialProject();
    }

    /// <summary>
    /// Starts the core services.
    /// This initializes the PropertyService and ResourceService.
    /// </summary>
    [MemberNotNull(nameof(_addInTree))]
    public void StartCoreServices(IPropertyService propertyService, Altaxo.Gui.Workbench.StartupSettings startupSettings)
    {
      var container = Altaxo.Current.GetRequiredService<System.ComponentModel.Design.IServiceContainer>();
      container.AddService(typeof(ITextOutputService), new TextOutputServiceTemporary());
      var applicationStateInfoService = new ApplicationStateInfoService();
      _addInTree = new AddInTreeImpl(applicationStateInfoService);
      container.AddService(typeof(Altaxo.Gui.Workbench.StartupSettings), startupSettings);
      CultureSettingsAtStartup.StartupDocumentCultureInfo = startupSettings.OriginalCulture;
      CultureSettingsAtStartup.StartupUICultureInfo = startupSettings.OriginalUICulture;

      container.AddService(typeof(IPropertyService), propertyService);
      SetCultureSettingsFromProperties(startupSettings); // set the document culture and the UI culture as early as possible (when PropertyService is functional)

      container.AddService(typeof(IResourceService), new ResourceServiceImpl(Path.Combine(propertyService.DataDirectory.ToString(), "resources"), propertyService));
      container.AddService(typeof(IAddInTree), _addInTree);
      container.AddService(typeof(ApplicationStateInfoService), applicationStateInfoService);
      StringParser.RegisterStringTagProvider(new ApplicationNameProvider(_applicationName));
    }

    /// <summary>
    /// Sets the culture settings from properties. This is in an extra method to make sure that the static members
    /// of CultureSettings are called only now.
    /// </summary>
    private void SetCultureSettingsFromProperties(Altaxo.Gui.Workbench.StartupSettings startupSettings)
    {
      // set the document culture and the UI culture as early as possible
      CultureSettings.PropertyKeyUICulture.ApplyProperty(Current.PropertyService.GetValue<CultureSettings>(CultureSettings.PropertyKeyUICulture, RuntimePropertyKind.UserAndApplicationAndBuiltin));
      CultureSettings.PropertyKeyDocumentCulture.ApplyProperty(Current.PropertyService.GetValue<CultureSettings>(CultureSettings.PropertyKeyDocumentCulture, RuntimePropertyKind.UserAndApplicationAndBuiltin));
    }

    private sealed class ApplicationNameProvider : IStringTagProvider
    {
      private string _appName;

      public ApplicationNameProvider(string applicationName)
      {
        _appName = applicationName;
      }

      public string? ProvideString(string tag, StringTagPair[]? customTags)
      {
        if (string.Equals(tag, "AppName", StringComparison.OrdinalIgnoreCase))
          return _appName;
        else
          return null;
      }
    }
  }
}
