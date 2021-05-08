#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Altaxo.Drawing;
using Altaxo.Text;
using Markdig.Renderers;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  /// <summary>
  /// Image provider for the markdown text that describes the fit function. Essentially, this provider only handles Urls with start with 'res:'.
  /// That means the images must reside in the resources.
  /// </summary>
  /// <seealso cref="Markdig.Renderers.WpfImageProviderBase" />
  public class ResourceOnlyImageProvider
        : Markdig.Renderers.WpfImageProviderBase
  {
    public ResourceOnlyImageProvider()
    {
    }

    public override Inline GetInlineItem(string url, out bool inlineItemIsErrorMessage)
    {
      if (url.StartsWith(ImagePretext.ResourceImagePretext))
      {
        string name = url.Substring(ImagePretext.ResourceImagePretext.Length);
        ImageSource? bitmapSource = null;
        try
        {
          bitmapSource = PresentationResourceService.GetBitmapSource(name);
        }
        catch (Exception)
        {
        }

        if (bitmapSource is not null)
        {
          var image = new Image() { Source = bitmapSource };

          inlineItemIsErrorMessage = false;
          return new InlineUIContainer(image);
        }
        else
        {
          inlineItemIsErrorMessage = true;
          return new Run(string.Format("ERROR: RESOURCE '{0}' NOT FOUND!", name));
        }
      }
      else
      {
        if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
          inlineItemIsErrorMessage = false;
          return new Run();
        }
        try
        {
          var bitmapSource = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));

          // This is not nice, but we have to validate that our bitmapSource is valid already now, since we may need to resize it immediately,
          // and we want to provoke the exception if our url was not valid
          int pixelHeight = bitmapSource.PixelHeight;

          var image = new Image { Source = bitmapSource };
          inlineItemIsErrorMessage = false;
          return new InlineUIContainer(image);
        }
        catch (Exception ex)
        {
          inlineItemIsErrorMessage = true;
          return new Run(string.Format("ERROR RENDERING '{0}' ({1})", url, ex.Message));
        }
      }
    }
  }
}

