#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Data
{
  /// <summary>
  /// Interface to a transformation that transformes an <see cref="AltaxoVariant"/> into another <see cref="AltaxoVariant"/>.
  /// The class that implement this interface should be immutable.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public interface IVariantToVariantTransformation : Main.IImmutable
  {
    /// <summary>
    /// Gets the type of the input value of the transformation.
    /// </summary>
    /// <value>
    /// The type of the input value of the transformation.
    /// </value>
    Type InputValueType { get; }

    /// <summary>
    /// Gets the type of the output value of the transformation.
    /// </summary>
    /// <value>
    /// The type of the output value of the transformation.
    /// </value>
    Type OutputValueType { get; }

    /// <summary>
    /// Transforms the specified value into another <see cref="AltaxoVariant"/> value.
    /// </summary>
    /// <param name="value">The value to transform.</param>
    /// <returns>The transformed value.</returns>
    AltaxoVariant Transform(AltaxoVariant value);

    /// <summary>
    /// Gets the representation of this transformation as a function.
    /// </summary>
    /// <value>
    /// The representation of this transformation as a function.
    /// </value>
    string RepresentationAsFunction { get; }

    /// <summary>
    /// Gets the representation of this transformation as a function with the argument provided in the parameter <paramref name="arg"/>.
    /// </summary>
    /// <param name="arg">The functions argument, e.g. 'x'.</param>
    /// <returns>The representation of this transformation as a function with the argument provided in the parameter <paramref name="arg"/>.</returns>
    string GetRepresentationAsFunction(string arg);

    /// <summary>
    /// Gets the representation of this transformation as an operator. If the operator representation is not applicable, null should be returned.
    /// </summary>
    /// <value>
    /// The representation of this transformation as an operator</value>
    string RepresentationAsOperator { get; }

    /// <summary>
    /// Gets the corresponding back transformation, or null if no such back transformation exists (as for instance for the absolute value transformation).
    /// </summary>
    /// <value>
    /// The back transformation.
    /// </value>
    IVariantToVariantTransformation BackTransformation { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is editable, i.e. contains methods to make new instances of this class
    /// with other behaviour.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is editable; otherwise, <c>false</c>.
    /// </value>
    bool IsEditable { get; }
  }
}
