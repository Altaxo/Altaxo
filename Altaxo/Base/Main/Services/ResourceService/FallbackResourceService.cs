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
using System.Drawing;
using System.Reflection;
using System.Resources;

namespace Altaxo.Main.Services
{
  internal sealed class FallbackResourceService : IResourceService
  {
    event EventHandler IResourceService.LanguageChanged { add { } remove { } }

    string IResourceService.Language
    {
      get { return "en"; }
      set
      {
        throw new NotImplementedException();
      }
    }

    string IResourceService.GetString(string name)
    {
      return name;
    }

    object? IResourceService.GetImageResource(string name)
    {
      return null;
    }

    System.IO.Stream? IResourceService.GetResourceStream(string name)
    {
      return null;
    }

    void IResourceService.RegisterStrings(string baseResourceName, Assembly assembly)
    {
      throw new NotImplementedException();
    }

    void IResourceService.RegisterNeutralStrings(ResourceManager stringManager, string debugInfo)
    {
      throw new NotImplementedException();
    }

    void IResourceService.RegisterImages(string baseResourceName, Assembly assembly)
    {
      throw new NotImplementedException();
    }

    void IResourceService.RegisterNeutralImages(ResourceManager imageManager, string debugInfo)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Registers all resources that are directly included into the assembly given in the argument. See remarks for how the string resources are registered.
    /// </summary>
    /// <param name="assembly">The assembly for which to register the resources.</param>
    /// <remarks>
    /// The name of a string resource is constructed from the namespace where the .resx file is located in, a dot, and the name of the resource in the .resx file.
    /// Example: the .resx file is located namespace Altaxo.Main.Services. The string resource's key is Foo. Then this string resource is registered with the name Altaxo.Main.Services.Foo.
    /// Image files directly included in the assembly have the name they are included with, but without extension.
    /// </remarks>
    void IResourceService.RegisterAssemblyResources(Assembly assembly)
    {
      throw new NotImplementedException();
    }

    public Bitmap GetBitmap(string name)
    {
      throw new NotImplementedException();
    }
  }
}
