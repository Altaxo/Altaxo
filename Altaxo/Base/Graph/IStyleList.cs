using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	/// <summary>
	/// Lists of styles, for instance scatter styles, line styles, etc., that are used in grouping.
	/// This list has to be immutable.
	/// </summary>
	/// <typeparam name="T">Type of the style.</typeparam>
	/// <seealso cref="System.Collections.Generic.IList{T}" />
	public interface IStyleList<T> : IList<T>, Main.IImmutable where T : Main.IImmutable // TODO NET45 replace with IReadonlyList<T>
	{
		string Name { get; }

		IStyleList<T> WithName(string name);

		bool IsStructuralEquivalentTo(IEnumerable<T> anotherList);
	}
}