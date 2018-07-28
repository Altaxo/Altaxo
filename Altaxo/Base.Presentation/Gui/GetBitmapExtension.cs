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
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace Altaxo.Gui
{
  /// <summary>
  /// Markup extension that gets a BitmapSource object for a ResourceService bitmap.
  /// </summary>
  [MarkupExtensionReturnType(typeof(BitmapSource))]
  public class GetBitmapExtension : MarkupExtension
  {
    protected string _key;

    public GetBitmapExtension(string key)
    {
      this._key = key;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (PresentationResourceService.InstanceAvailable)
        return PresentationResourceService.GetBitmapSource(_key);
      else
        return _key;
    }
  }
}
