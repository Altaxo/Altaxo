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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Altaxo.Main.Services;
using System.Windows.Controls;

namespace Altaxo.Gui
{
  public static class GuiResourceServiceExtensions
  {
    #region Resource Service Extensions

    /// <summary>
    /// Gets an <see cref="IImage"/> from a resource.
    /// </summary>
    /// <exception cref="ResourceNotFoundException">The resource with the specified name does not exist</exception>
    public static IImage GetImage(this IResourceService resourceService, string resourceName)
    {
      if (resourceService == null)
        throw new ArgumentNullException("resourceService");
      if (resourceName == null)
        throw new ArgumentNullException("resourceName");
      return new ResourceServiceImage(resourceService, resourceName);
    }

    /// <summary>
    /// Gets an image source from a resource.
    /// </summary>
    /// <exception cref="ResourceNotFoundException">The resource with the specified name does not exist</exception>
    public static ImageSource GetImageSource(this IResourceService resourceService, string resourceName)
    {
      if (resourceService == null)
        throw new ArgumentNullException("resourceService");
      if (resourceName == null)
        throw new ArgumentNullException("resourceName");
      return PresentationResourceService.GetBitmapSource(resourceName);
    }

    /// <summary>
    /// Creates a new image for the image source.
    /// </summary>
    public static Image CreateImage(this IImage image)
    {
      if (image == null)
        throw new ArgumentNullException("image");
      return new Image { Source = image.ImageSource };
    }

    #endregion Resource Service Extensions
  }
}
