#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using Altaxo.Graph;

namespace Altaxo.Main
{
  /// <summary>
  /// Creates builtin textures by associating a unique name of the texture and the corresponding resource.
  /// </summary>
  /// <attribute name="resource" use="required">
  /// The name of a bitmap resource in the resource service.
  /// </attribute>
  /// <usage>Only in /Altaxo/BuiltinTextures</usage>
  /// <returns>
  /// An ImageProxy object that represents the resource. This ImageProxy is added automatically
  /// to the TextureManager.BuiltinTextures collection.
  /// </returns>
  public class TextureDoozer : IDoozer
  {
    /// <summary>
    /// Gets if the doozer handles codon conditions on its own.
    /// If this property return false, the item is excluded when the condition is not met.
    /// </summary>
    public bool HandleConditions
    {
      get
      {
        return false;
      }
    }

    public object BuildItem(object caller, Codon codon, ArrayList subItems)
    {
      string id = codon.Id;
      string resource = codon.Properties["resource"];
      ImageProxy proxy = ResourceImageProxy.FromResource(id,resource);
      TextureManager.BuiltinTextures.Add(proxy);
      return proxy;
    }
  }
}
