﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3675 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IClass : IEntity
	{
		/// <summary>
		/// Region of the whole class including the body.
		/// </summary>
		DomRegion Region {
			get;
		}

		/// <summary>
		/// The default return type to use for this class.
		/// This property is mutable even when the IClass is frozen, see
		/// documentation for <see cref="HasCompoundClass"/>.
		/// This property is thread-safe.
		/// </summary>
		IReturnType DefaultReturnType { get; }
		
		ClassType ClassType {
			get;
		}
		
		/// <summary>
		/// The project content in which this class is defined.
		/// </summary>
		IProjectContent ProjectContent {
			get;
		}
		
		ICompilationUnit CompilationUnit {
			get;
		}
		
		/// <summary>
		/// Gets the using scope of contains this class.
		/// </summary>
		IUsingScope UsingScope {
			get;
		}
		
		IList<IReturnType> BaseTypes {
			get;
		}
		
		/// <summary>Gets the class associated with the base type with the same index.</summary>
		IReturnType GetBaseType(int index);
		
		IList<IClass> InnerClasses {
			get;
		}

		IList<IField> Fields {
			get;
		}

		IList<IProperty> Properties {
			get;
		}

		IList<IMethod> Methods {
			get;
		}

		IList<IEvent> Events {
			get;
		}
		
		IList<ITypeParameter> TypeParameters {
			get;
		}

		IEnumerable<IClass> ClassInheritanceTree {
			get;
		}
		
		IClass BaseClass {
			get;
		}
		
		IReturnType BaseType {
			get;
		}
		
		/// <summary>
		/// If this is a partial class, gets the compound class containing information from all parts.
		/// If this is not a partial class, a reference to this class is returned.
		/// </summary>
		IClass GetCompoundClass();
		
		IClass GetInnermostClass(int caretLine, int caretColumn);
		
		List<IClass> GetAccessibleTypes(IClass callingClass);
		
		/// <summary>
		/// Searches the member with the specified name. Returns the first member/overload found.
		/// </summary>
		IMember SearchMember(string memberName, LanguageProperties language);
		
		/// <summary>Return true if the specified class is a base class of this class; otherwise return false.</summary>
		/// <remarks>Returns false when possibleBaseClass is null.</remarks>
		bool IsTypeInInheritanceTree(IClass possibleBaseClass);
		
		bool HasPublicOrInternalStaticMembers {
			get;
		}
		bool HasExtensionMethods {
			get;
		}
		
		bool IsPartial {
			get;
		}
		
		/// <summary>
		/// Gets/sets if this class has an associated compound class.
		/// This property is mutable even if the IClass instance is frozen.
		/// This property is thread-safe.
		/// 
		/// This property may only be set by the IProjectContent implementation to which this class is added.
		/// 
		/// Rational: some languages support partial classes where only one of the parts needs
		/// the "partial" modifier. If the part without the modifier is added to the project
		/// content first, it is added without compound class and may get a DefaultReturnType that refers
		/// to itself.
		/// However, when the other part with the modifier is added, a compound class is created, and the
		/// DefaultReturnType of this class must change even though it is frozen.
		/// </summary>
		bool HasCompoundClass {
			get;
			set;
		}
		
		///// <summary>
		///// Creates a shallow copy of the class that is not frozen.
		///// </summary>
		//IClass Unfreeze();
	}
}
