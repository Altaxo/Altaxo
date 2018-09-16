#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Windows.Controls;
using System.Windows.Markup;

namespace Altaxo.Gui
{
  /// <summary>
  /// Markup extension that gets an Image with the size of 16 x 16 directly for usage in buttons, tree view items etc.
  /// </summary>
  [MarkupExtensionReturnType(typeof(Image))]
  public class GetIconExtension : MarkupExtension
  {
    protected string _key;

    public GetIconExtension(string key)
    {
      _key = key;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (PresentationResourceService.InstanceAvailable)
      {
        var imgSource = PresentationResourceService.GetBitmapSource(_key);
        return new System.Windows.Controls.Image
        {
          Height = 16,
          Width = 16,
          Source = imgSource
        };
      }
      else
      {
        return new Image();
      }
    }
  }
}
