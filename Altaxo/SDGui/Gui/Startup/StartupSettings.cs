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

namespace Altaxo.Gui.Startup
{
	/// <summary>
	/// This class contains properties you can use to control how SharpDevelop is launched.
	/// </summary>
	[Serializable]
	public sealed class StartupSettings
	{
		private bool useSharpDevelopErrorHandler = true;
		private string applicationName = "Altaxo";
		private string applicationRootPath;
		private bool allowAddInConfigurationAndExternalAddIns = true;
		private bool allowUserAddIns;
		private string propertiesName;
		private string configDirectory;
		private string dataDirectory;
		private string domPersistencePath;
		private string resourceAssemblyName = "AltaxoSDGui";
		internal List<string> addInDirectories = new List<string>();
		internal List<string> addInFiles = new List<string>();

		/// <summary>
		/// Gets/Sets the name of the assembly to load the BitmapResources
		/// and English StringResources from.
		/// </summary>
		public string ResourceAssemblyName
		{
			get { return resourceAssemblyName; }
			set
			{
				resourceAssemblyName = value ?? throw new ArgumentNullException(nameof(value));
			}
		}

		/// <summary>
		/// Gets/Sets whether the SharpDevelop exception box should be used for
		/// unhandled exceptions. The default is true.
		/// </summary>
		public bool UseExceptionBoxForErrorHandler
		{
			get { return useSharpDevelopErrorHandler; }
			set { useSharpDevelopErrorHandler = value; }
		}

		/// <summary>
		/// Use the file <see cref="ConfigDirectory"/>\AddIns.xml to maintain
		/// a list of deactivated AddIns and list of AddIns to load from
		/// external locations.
		/// The default value is true.
		/// </summary>
		public bool AllowAddInConfigurationAndExternalAddIns
		{
			get { return allowAddInConfigurationAndExternalAddIns; }
			set { allowAddInConfigurationAndExternalAddIns = value; }
		}

		/// <summary>
		/// Allow user AddIns stored in the "application data" directory.
		/// The default is false.
		/// </summary>
		public bool AllowUserAddIns
		{
			get { return allowUserAddIns; }
			set { allowUserAddIns = value; }
		}

		/// <summary>
		/// Gets/Sets the application name used by the MessageService and some
		/// SharpDevelop windows. The default is "SharpDevelop".
		/// </summary>
		public string ApplicationName
		{
			get { return applicationName; }
			set
			{
				applicationName = value ?? throw new ArgumentNullException(nameof(value));
			}
		}

		/// <summary>
		/// Gets/Sets the application root path to use.
		/// Use null (default) to use the base directory of the SharpDevelop AppDomain.
		/// </summary>
		public string ApplicationRootPath
		{
			get { return applicationRootPath; }
			set { applicationRootPath = value; }
		}

		/// <summary>
		/// Gets/Sets the directory used to store SharpDevelop properties,
		/// settings and user AddIns.
		/// Use null (default) to use "ApplicationData\ApplicationName"
		/// </summary>
		public string ConfigDirectory
		{
			get { return configDirectory; }
			set { configDirectory = value; }
		}

		/// <summary>
		/// Sets the data directory used to load resources.
		/// Use null (default) to use the default path "ApplicationRootPath\data".
		/// </summary>
		public string DataDirectory
		{
			get { return dataDirectory; }
			set { dataDirectory = value; }
		}

		/// <summary>
		/// Sets the name used for the properties file (without path or extension).
		/// Use null (default) to use the default name.
		/// </summary>
		public string PropertiesName
		{
			get { return propertiesName; }
			set { propertiesName = value; }
		}

		/// <summary>
		/// Sets the directory used to store the code completion cache.
		/// Use null (default) to disable the code completion cache.
		/// </summary>
		public string DomPersistencePath
		{
			get { return domPersistencePath; }
			set { domPersistencePath = value; }
		}

		/// <summary>
		/// Find AddIns by searching all .addin files recursively in <paramref name="addInDir"/>.
		/// </summary>
		public void AddAddInsFromDirectory(string addInDir)
		{
			if (addInDir == null)
				throw new ArgumentNullException(nameof(addInDir));
			addInDirectories.Add(addInDir);
		}

		/// <summary>
		/// Add the specified .addin file.
		/// </summary>
		public void AddAddInFile(string addInFile)
		{
			if (addInFile == null)
				throw new ArgumentNullException(nameof(addInFile));
			addInFiles.Add(addInFile);
		}
	}
}