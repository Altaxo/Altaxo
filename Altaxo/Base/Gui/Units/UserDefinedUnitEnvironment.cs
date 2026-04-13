#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Gui;

namespace Altaxo.Gui.Units
{
  /// <summary>
  /// Associates a user-defined unit environment with a display name and quantity name.
  /// </summary>
  public class UserDefinedUnitEnvironment
  {
    private string _name;

    private string _quantity;

    private QuantityWithUnitGuiEnvironment _environment;

    #region Serialization

    /// <summary>
    /// 2017-09-26 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(UserDefinedUnitEnvironment), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (UserDefinedUnitEnvironment)o;
        info.AddValue("Name", s.Name);
        info.AddValue("Quantity", s.Quantity);
        info.AddValue("Environment", s.Environment);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var name = info.GetString("Name");
        var quantity = info.GetString("Quantity");
        var env = (QuantityWithUnitGuiEnvironment)info.GetValue("Environment", null);

        return new UserDefinedUnitEnvironment(name, quantity, env);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Gets the environment name.
    /// </summary>
    public string Name { get { return _name; } }

    /// <summary>
    /// Gets the quantity name.
    /// </summary>
    public string Quantity { get { return _quantity; } }

    /// <summary>
    /// Gets the associated unit environment.
    /// </summary>
    public QuantityWithUnitGuiEnvironment Environment { get { return _environment; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDefinedUnitEnvironment"/> class.
    /// </summary>
    /// <param name="environmentName">The environment name.</param>
    /// <param name="quantityName">The quantity name.</param>
    /// <param name="environment">The associated unit environment.</param>
    public UserDefinedUnitEnvironment(string environmentName, string quantityName, QuantityWithUnitGuiEnvironment environment)
    {
      _name = environmentName;
      _quantity = quantityName;
      _environment = environment;
    }

    /// <summary>
    /// Gets an empty user-defined unit environment.
    /// </summary>
    public static UserDefinedUnitEnvironment Empty
    {
      get
      {
        return new UserDefinedUnitEnvironment(string.Empty, string.Empty, null!);
      }
    }
  }
}
