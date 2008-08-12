// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2929 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Type parameter of a generic class/method.
	/// </summary>
	public interface ITypeParameter : IFreezable
	{
		/// <summary>
		/// The name of the type parameter (for example "T")
		/// </summary>
		string Name { get; }
		
		int Index { get; }
		
		IList<IAttribute> Attributes { get; }
		
		/// <summary>
		/// The method this type parameter is defined for.
		/// This property is null when the type parameter is for a class.
		/// </summary>
		IMethod Method { get; }
		
		/// <summary>
		/// The class this type parameter is defined for.
		/// When the type parameter is defined for a method, this is the class containing
		/// that method.
		/// </summary>
		IClass Class  { get; }
		
		/// <summary>
		/// Gets the contraints of this type parameter.
		/// </summary>
		IList<IReturnType> Constraints { get; }
		
		/// <summary>
		/// Gets if the type parameter has the 'new()' constraint.
		/// </summary>
		bool HasConstructableConstraint { get; }
		
		/// <summary>
		/// Gets if the type parameter has the 'class' constraint.
		/// </summary>
		bool HasReferenceTypeConstraint { get; }
		
		/// <summary>
		/// Gets if the type parameter has the 'struct' constraint.
		/// </summary>
		bool HasValueTypeConstraint { get; }
	}
}
