// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1943 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// GenericReturnType is a reference to a type parameter.
	/// </summary>
	public sealed class GenericReturnType : ProxyReturnType
	{
		ITypeParameter typeParameter;
		
		public ITypeParameter TypeParameter {
			get {
				return typeParameter;
			}
		}
		
		public override bool Equals(object o)
		{
			IReturnType rt = o as IReturnType;
			if (rt == null || !rt.IsGenericReturnType)
				return false;
			return typeParameter.Equals(rt.CastToGenericReturnType().typeParameter);
		}
		
		public override int GetHashCode()
		{
			return typeParameter.GetHashCode();
		}
		
		public GenericReturnType(ITypeParameter typeParameter)
		{
			if (typeParameter == null)
				throw new ArgumentNullException("typeParameter");
			this.typeParameter = typeParameter;
		}
		
		public override string FullyQualifiedName {
			get {
				return typeParameter.Name;
			}
		}
		
		public override string Name {
			get {
				return typeParameter.Name;
			}
		}
		
		public override string Namespace {
			get {
				return "";
			}
		}
		
		public override string DotNetName {
			get {
				if (typeParameter.Method != null)
					return "``" + typeParameter.Index;
				else
					return "`" + typeParameter.Index;
			}
		}
		
		public override IClass GetUnderlyingClass()
		{
			return null;
		}
		
		public override IReturnType BaseType {
			get {
				int count = typeParameter.Constraints.Count;
				if (count == 0)
					return typeParameter.Class.ProjectContent.SystemTypes.Object;
				if (count == 1)
					return typeParameter.Constraints[0];
				return new CombinedReturnType(typeParameter.Constraints,
				                              FullyQualifiedName,
				                              Name, Namespace,
				                              DotNetName);
			}
		}
		
		// remove static methods (T.ReferenceEquals() is not possible)
		public override List<IMethod> GetMethods()
		{
			List<IMethod> list = base.GetMethods();
			if (list != null) {
				list.RemoveAll(delegate(IMethod m) { return m.IsStatic || m.IsConstructor; });
				if (typeParameter.HasConstructableConstraint || typeParameter.HasValueTypeConstraint) {
					list.Add(new Constructor(ModifierEnum.Public, this,
					                         DefaultTypeParameter.GetDummyClassForTypeParameter(typeParameter)));
				}
			}
			return list;
		}
		
		public override string ToString()
		{
			return String.Format("[GenericReturnType: {0}]", typeParameter);
		}
		
		public override bool IsDefaultReturnType {
			get {
				return false;
			}
		}
		
		public override bool IsArrayReturnType {
			get {
				return false;
			}
		}
		
		public override bool IsConstructedReturnType {
			get {
				return false;
			}
		}
		
		public override bool IsGenericReturnType {
			get {
				return true;
			}
		}
		
		public override ICSharpCode.SharpDevelop.Dom.GenericReturnType CastToGenericReturnType()
		{
			return this;
		}
	}
}
