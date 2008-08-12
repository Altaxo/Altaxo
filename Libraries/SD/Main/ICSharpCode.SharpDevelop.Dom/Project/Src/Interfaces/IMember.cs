﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2931 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IMember : IEntity, ICloneable
	{
		/// <summary>
		/// Declaration region of the member (without body!)
		/// </summary>
		DomRegion Region {
			get;
		}
		
		/// <summary>
		/// Gets/Sets the declaring type reference (declaring type incl. type arguments).
		/// Never returns null, if the property is set to null (e.g. when this is not a specialized member),
		/// it should return the default type reference to the <see cref="DeclaringType"/>.
		/// </summary>
		IReturnType DeclaringTypeReference {
			get;
			set;
		}
		
		/// <summary>
		/// Gets the generic member this member is based on.
		/// Returns null if this is not a specialized member.
		/// Specialized members are the result of overload resolution with type substitution.
		/// </summary>
		IMember GenericMember {
			get;
		}
		
		/// <summary>
		/// Creates a copy of this member with its GenericMember property set to this member.
		/// Use this method to create copies of a member that should be regarded as the "same member"
		/// for refactoring purposes.
		/// </summary>
		IMember CreateSpecializedMember();
		
		IReturnType ReturnType {
			get;
			set;
		}
		
		IList<ExplicitInterfaceImplementation> InterfaceImplementations {
			get;
		}
	}
}
