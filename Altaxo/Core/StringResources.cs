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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo
{
  /// <summary>
  /// String resource manager for the AltaxoCore DLL and other DLLs that reference AltaxoCore.
  /// </summary>
  public class StringResources
  {
    /// <summary>
    /// The default resource manager for the neutral (English) resources.
    /// </summary>
    private System.Resources.ResourceManager _defaultResourceMgr;
    /// <summary>
    /// Dictionary mapping language codes to resource managers.
    /// </summary>
    private Dictionary<string, System.Resources.ResourceManager?> _resourceManagersByLanguage;
    /// <summary>
    /// The assembly containing the resources.
    /// </summary>
    private System.Reflection.Assembly _assemblyContainingTheResources;
    /// <summary>
    /// The resource path (namespace and folder).
    /// </summary>
    private string _resourcePath;
    /// <summary>
    /// The resource file name (without extension).
    /// </summary>
    private string _resourceName;

    /// <summary>
    /// The AltaxoCore string resources instance.
    /// </summary>
    private static StringResources _AltaxoCoreResources = new StringResources("Altaxo.Resources", "StringResources", System.Reflection.Assembly.GetExecutingAssembly());

    /// <summary>
    /// Gets the AltaxoCore string resources.
    /// </summary>
    public static StringResources AltaxoCore { get { return _AltaxoCoreResources; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringResources"/> class.
    /// </summary>
    /// <param name="path">The path to the resource. This is usually the default namespace, a dot, and the folders (separated by dots) where the resource file(s) reside.</param>
    /// <param name="resourceName">Name of the neutral (English) resource file (without the '.resource' extension).</param>
    /// <param name="assemblyContainingTheResources">The assembly containing the resources.</param>
    /// <remarks>
    /// The naming conventions will be explained using AltaxoCore as example.
    /// <para>The default namespace of the AltaxoCore DLL is 'Altaxo'.</para>
    /// <para>The resource files reside in the source folder 'Resources'.</para>
    /// <para>The neutral resource file is named 'StringResources.resources'.</para>
    /// <para>The resource file for the German culture must be named 'de.StringResources.resources' (two-letter_culture_name + dot + neutral_resource_file_name).</para>
    /// <para>This will lead to the following arguments for creating the instance:</para>
    /// <para>The <paramref name="path"/> argument is 'Altaxo.Resources' (default_namespace + dot + foldername).</para>
    /// <para>The <paramref name="resourceName"/> argument is 'StringResources'.</para>
    /// <para>The <paramref name="assemblyContainingTheResources"/> argument must refer to AltaxoCore.</para>
    /// </remarks>
    public StringResources(string path, string resourceName, System.Reflection.Assembly assemblyContainingTheResources)
    {
      _resourcePath = path;
      _resourceName = resourceName;
      _assemblyContainingTheResources = assemblyContainingTheResources;
      _defaultResourceMgr = new System.Resources.ResourceManager(path + "." + resourceName, assemblyContainingTheResources);
      _resourceManagersByLanguage = new Dictionary<string, System.Resources.ResourceManager?>
      {
        { "en", _defaultResourceMgr }
      };
    }

    /// <summary>
    /// Gets a resource string for a given resource key.
    /// </summary>
    /// <param name="resourceKey">The resource key.</param>
    /// <returns>The resource string corresponding to the given resource key.</returns>
    public string GetString(StringResourceKey resourceKey)
    {
      return GetString(resourceKey, true);
    }

    /// <summary>
    /// Gets a resource string for a given resource key, with an option to fall back to the example value if not found.
    /// </summary>
    /// <param name="resourceKey">The resource key.</param>
    /// <param name="fallbackToExampleValue">If <c>true</c>, returns the example value if the resource key is not found; otherwise, returns null.</param>
    /// <returns>The resource string corresponding to the given resource key, or the example value if not found and fallback is enabled.</returns>
    public string GetString(StringResourceKey resourceKey, bool fallbackToExampleValue)
    {

      string cu = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

      if (!_resourceManagersByLanguage.TryGetValue(cu, out var mgr))
      {
        string resName = string.Format("{0}.{1}.{2}", _resourcePath, cu, _resourceName);
        mgr = null;
        try
        {
          mgr = new System.Resources.ResourceManager(resName, _assemblyContainingTheResources);
        }
        catch (Exception)
        {
        }
        _resourceManagersByLanguage.Add(cu, mgr); // add the manager even if it is null -> this then indicates that no such resource localization exists.
      }

      string? result;
      mgr = _resourceManagersByLanguage[cu];
      if (mgr is not null && (result = mgr.GetString(resourceKey.Key)) is not null)
        return result;

      if ((result = _defaultResourceMgr.GetString(resourceKey.Key)) is not null)
        return result;

      return resourceKey.ExampleStringValue;
    }
  }
}
