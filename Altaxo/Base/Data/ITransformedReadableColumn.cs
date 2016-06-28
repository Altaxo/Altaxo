using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Interface to a readable column that is amended with a transformation.
	/// </summary>
	/// <seealso cref="Altaxo.Data.IReadableColumn" />
	public interface ITransformedReadableColumn : IReadableColumn
	{
		/// <summary>
		/// Gets the original readable column, i.e. the readable column without the transformation.
		/// </summary>
		/// <value>
		/// The original readable column.
		/// </value>
		IReadableColumn OriginalReadableColumn { get; }

		/// <summary>
		/// Gets a new instance of this class with the same transformation, but another underlying readable column.
		/// </summary>
		/// <param name="originalReadableColumn">The new original readable column.</param>
		/// <returns>New instance of this class with the same transformation, but another underlying readable column.</returns>
		ITransformedReadableColumn WithOriginalReadableColumn(IReadableColumn originalReadableColumn);

		/// <summary>
		/// Gets the transformation.
		/// </summary>
		/// <value>
		/// The transformation.
		/// </value>
		IVariantToVariantTransformation Transformation { get; }

		/// <summary>
		/// Gets a new instance of this class with the same underlying original column, but with another transformation.
		/// </summary>
		/// <param name="transformation">The new transformation.</param>
		/// <returns>A new instance of this class with the same underlying original column, but with another transformation.</returns>
		ITransformedReadableColumn WithTransformation(IVariantToVariantTransformation transformation);
	}

	/// <summary>
	/// Interface to a transformation that transformes an <see cref="AltaxoVariant"/> into another <see cref="AltaxoVariant"/>.
	/// The class that implement this interface should be immutable.
	/// </summary>
	/// <seealso cref="Altaxo.Main.IImmutable" />
	public interface IVariantToVariantTransformation : Main.IImmutable
	{
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